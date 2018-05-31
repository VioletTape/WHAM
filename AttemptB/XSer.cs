using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AttemptB.Attributes;
using FluentAssertions.Common;
using Jil;

namespace AttemptB {
    public static class XSer {
        public static string Serialize<T>(T obj) {
            var res = JSON.Serialize(obj);

            var linkMethods = GetActionLinkMethods<T>();

            foreach (var linkMethod in linkMethods)
            foreach (var link in linkMethod.GetCustomAttributes<LinkAttribute>()) {
                var field = obj.GetType().GetField(link.AvailableWhen);
                if (field != null) {
                    var o = field.GetValue(obj);

                    switch (o) {
                        case int i:
                            var range = new Range(0, 0);
                            var t = (int) o;
                            break;
                        case bool b:
                            var tt = (bool) o;
                            break;
                    }

                    continue;
                }

                var property = obj.GetType().GetProperty(link.AvailableWhen);
            }

            return res;
        }

        public static List<MethodInfo> GetActionLinkMethods<T>() {
            return typeof(T).GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(mi => mi.HasAttribute<LinkAttribute>())
                .Select(mi => mi)
                .ToList();
        }
    }

    class Range {
        public int From ;
        public int To ;

        public Range(int @from = Int32.MinValue, int to = Int32.MaxValue) {
            From = @from;
            To = to;
        }
    }
}