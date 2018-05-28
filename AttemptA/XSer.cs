using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AttemptA.Attributes;
using FluentAssertions.Common;
using Jil;

namespace AttemptA {
    public static class XSer {
        private static readonly Dictionary<Type, List<MethodInfo>> linksPerClass =
            new Dictionary<Type, List<MethodInfo>>();

        private static readonly Dictionary<Type, Dictionary<int, Xxx>> counters =
            new Dictionary<Type, Dictionary<int, Xxx>>();

        private static readonly Dictionary<Type, Dictionary<object, Dictionary<int, Xxx>>> cntRef
            = new Dictionary<Type, Dictionary<object, Dictionary<int, Xxx>>>();

        private static string uri; 

        public static void Register<T>() {
            var type = typeof(T);
            if (linksPerClass.ContainsKey(type))
                return;

            var actionLinkMethods = GetActionLinkMethods<T>();
            linksPerClass.Add(type, actionLinkMethods);
            counters.Add(type, new Dictionary<int, Xxx>());
            cntRef.Add(type, new Dictionary<object, Dictionary<int, Xxx>>());

            foreach (var info in actionLinkMethods) {
                var attribute = info.GetCustomAttribute<ActionLinkAttribute>();
                counters[type].Add(attribute.Action, new Xxx {MethodInfo = info, Count = 0});
            }

            // get URI of the service and potentially info, should it be enhanced with 
            // origin-source field or not 
            var customAttributes = Assembly.GetAssembly(type).GetCustomAttributes(typeof(MarkOriginsAttribute));
            if (customAttributes.Any()) {
                var attribute = (MarkOriginsAttribute)customAttributes.First();
                uri = attribute.Uri;
            }
        }
    
        public static string Serialize<T>(T obj, string method = "") {
            var res = JSON.Serialize(obj);

            var list = new List<string>();
            var type = typeof(T);

            // get resource id 
            var uid = type.GetProperties()
                .Where(p => p.GetCustomAttribute<ResourceUidAttribute>() != null)
                                           .Select(p => p.GetValue(obj))
                                           .First();
            


            //var value = type.GetPropertyByName("Id").GetValue(obj);
            if (!cntRef[type].ContainsKey(uid)) {
                var dictionary = new Dictionary<int, Xxx>();
                foreach (var pair in counters[type]) dictionary.Add(pair.Key, pair.Value.Clone());
                cntRef[type].Add(uid, dictionary);
            }

            // add origin source links 
            list.Add($"\"original-source\":\"{uri}{type.Name}/{uid}\"");

            // add action links 
            foreach (var info in linksPerClass[type]) {
                var attr = info.GetCustomAttribute<ActionLinkAttribute>();
                if (attr.DependsOn > 0
                    && cntRef[type][uid][attr.DependsOn].Count == 0)
                    continue;

                if (attr.Times > 0 &&
                    attr.Times <= cntRef[type][uid][attr.Action].Count)
                    continue;

                list.Add($"\"{info.Name}\":\"{uri}{type.Name}/{uid}/{info.Name}\"");
            }


            var sb = new StringBuilder(res.Substring(0, res.Length - 1));
            foreach (var l in list) sb.Append("," + l);

            sb.Append("}");
            var serialize = sb.ToString();
            return serialize;
        }

        public static List<MethodInfo> GetActionLinkMethods<T>() {
            return typeof(T).GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(mi => mi.HasAttribute<ActionLinkAttribute>())
                .Select(mi => mi)
                .ToList();
        }

        public static string Handle<T>(T entity, string methodName) {
            var type = typeof(T);

            var minfo = linksPerClass[type].Single(mi => mi.Name == methodName);
            minfo.Invoke(entity, new object[0]);

            // get resource id 
            var uid = type.GetProperties()
                .Where(p => p.GetCustomAttribute<ResourceUidAttribute>() != null)
                .Select(p => p.GetValue(entity))
                .First();

            cntRef[type][uid].Values.Single(im => im.MethodInfo.Name == methodName).Count++;

            return Serialize(entity);
        }

        public static void ResetCounters<T>(T entity) {
            var type = typeof(T);
            var value = type.GetPropertyByName("Id").GetValue(entity);
            cntRef[type][value].Values.ToList().ForEach(x => x.Reset());
        }

        private class Xxx {
            public MethodInfo MethodInfo { get; set; }
            public int Count { get; set; }

            public Xxx Clone() {
                return new Xxx {Count = 0, MethodInfo = MethodInfo};
            }

            public void Reset() {
                Count = 0;
            }
        }
    }
}