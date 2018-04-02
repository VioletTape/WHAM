using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using FluentAssertions.Common;
using Jil;

namespace AttemptA {
    public static class XSer {
        private static readonly Dictionary<Type, List<MethodInfo>> linksPerClass =
            new Dictionary<Type, List<MethodInfo>>();

        private static readonly Dictionary<Type, Dictionary<int,Xxx>> counters =
            new Dictionary<Type, Dictionary<int, Xxx>>();

        private static readonly Dictionary<Type, Dictionary<object, Dictionary<int, Xxx>>> cntRef 
            = new Dictionary<Type, Dictionary<object, Dictionary<int, Xxx>>>();

        public static void Register<T>() {
            var type = typeof(T);
            if (linksPerClass.ContainsKey(type))
                return;

            var actionLinkMethods = GetActionLinkMethods<T>();
            linksPerClass.Add(type, actionLinkMethods);
            counters.Add(type, new Dictionary<int, Xxx>());

            foreach (var info in actionLinkMethods) {
                var attribute = info.GetCustomAttribute<ActionLinkAttribute>();
                counters[type].Add(attribute.Action, new Xxx{MethodInfo = info, Count = 0});
            }
        }

        public static string Serialize<T>(T obj, string method = "") {
            var res = JSON.Serialize(obj);

            var list = new List<string>();
            var type = typeof(T);

            var value = type.GetPropertyByName("Id").GetValue(obj);
            cntRef[type][value] = counters[type];

            foreach (var info in linksPerClass[type]) {
                var attr = info.GetCustomAttribute<ActionLinkAttribute>();
                if (attr.DependsOn > 0
                    && counters[type][attr.DependsOn].Count == 0) {
                        continue;
                }

                if (attr.Times > 0 &&  
                    attr.Times <= counters[type][attr.Action].Count) {
                    continue;
                }

                list.Add($"\"{info.Name}\":\"http://localhost:1234/{type.Name}/1/{info.Name}\"");
            }

            var sb = new StringBuilder(res.Substring(0, res.Length - 1));
            foreach (var l in list) sb.Append("," + l);

            sb.Append("}");
            var serialize = sb.ToString();
            return serialize;
        }

        internal static object Serialize(Beer beer, Expression<Func<object, object>> p) {
            var bodyNodeType = p.Body as UnaryExpression;
            var expression = bodyNodeType.Operand as MethodCallExpression;
            var methodName = expression.Method.Name;

            return "";
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

            counters[type].Values.Single(im => im.MethodInfo.Name == methodName).Count++;

            return Serialize(entity);
        }

        public static void ResetCounters<T>(T entity) {

        }

        private class Xxx {
            public MethodInfo MethodInfo { get; set; }
            public int Count { get; set; }
        }
    }
}