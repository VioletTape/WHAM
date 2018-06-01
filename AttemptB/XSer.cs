using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using AttemptB.Attributes;
using AttemptB.Config;
using FluentAssertions.Common;
using Jil;

namespace AttemptB {
    public static class XSer {
        private static WhamSettings GetConfig() {
            return (WhamSettings) ConfigurationManager.GetSection("wham");
        }

        public static string Serialize<T>(T obj) {
            var refLinks = new List<RefLink>();

            var res = JSON.Serialize(obj);

            var domainUrl = GetConfig().DomainUrl.Name;
            var controllerHandler = GetTypeHandler(typeof(T));
            var uid = GetUidInstrumentedFields<T>().FirstOrDefault()?.GetValue(obj)
                      ?? GetUidInstrumentedProperties<T>().FirstOrDefault()?.GetValue(obj)
                      ?? 0;


            var linkMethods = GetLinkInstrumentedMethods<T>();

            var tempLinks = new List<RefLink>();
            foreach (var linkMethod in linkMethods) {
                tempLinks.Clear();

                foreach (var link in linkMethod.GetCustomAttributes<LinkAttribute>()) {
                    var fieldY = obj.GetType().GetField(link.AvailableWhen);
                    if (fieldY != null) {
                        var o = fieldY.GetValue(obj);

                        switch (o) {
                            case int i:
                                tempLinks.Add(new RefLink {
                                    Name = linkMethod.Name,
                                    Value = $"{domainUrl}/{controllerHandler}/{uid}/{linkMethod.Name}",
                                    Result = i > link.MoreThan
                                });
                                break;
                            case bool b:
                                tempLinks.Add(new RefLink {
                                    Name = linkMethod.Name,
                                    Value = $"{domainUrl}/{controllerHandler}/{uid}/{linkMethod.Name}",
                                    Result = b
                                });
                                ;
                                break;
                        }

                        continue;
                    }

                    var fieldN = obj.GetType().GetField(link.NotAvailableWhen);
                    if (fieldN != null) {
                        var o = fieldN.GetValue(obj);

                        switch (o) {
                            case int i:
                                tempLinks.Add(new RefLink {
                                    Name = linkMethod.Name,
                                    Value = $"{domainUrl}/{controllerHandler}/{uid}/{linkMethod.Name}",
                                    Result = i < link.MoreThan
                                });
                                break;
                            case bool b:
                                tempLinks.Add(new RefLink {
                                    Name = linkMethod.Name,
                                    Value = $"{domainUrl}/{controllerHandler}/{uid}/{linkMethod.Name}",
                                    Result = !b
                                });
                                ;
                                break;
                        }

                        continue;
                    }

                    tempLinks.Add(new RefLink {
                        Name = linkMethod.Name,
                        Value = $"{domainUrl}/{controllerHandler}/{uid}/{linkMethod.Name}",
                        Result = true
                    });

                    var property = obj.GetType().GetProperty(link.AvailableWhen);
                }

                if (tempLinks.Count > 1) {
                    if (tempLinks.All(l => l.Result))
                        refLinks.Add(tempLinks.First());
                }
                else if (tempLinks.First().Result) {
                    refLinks.Add(tempLinks.First());
                }
            }

            var serialized = new XSerFormatter().GetSerialized(refLinks);

            res = res.Substring(0, res.Length - 1) + "," + serialized + "}";

            return res;
        }

        public static List<MethodInfo> GetLinkInstrumentedMethods<T>() {
            return typeof(T).GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(mi => mi.HasAttribute<LinkAttribute>())
                .Select(mi => mi)
                .ToList();
        }

        public static List<FieldInfo> GetUidInstrumentedFields<T>()
        {
            return typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Where(mi => mi.HasAttribute<ResourceUidAttribute>())
                .Select(mi => mi)
                .ToList();
        }

        public static List<PropertyInfo> GetUidInstrumentedProperties<T>()
        {
            return typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(mi => mi.HasAttribute<ResourceUidAttribute>())
                .Select(mi => mi)
                .ToList();
        }

        public static string GetTypeHandler(Type type) {
            return type.GetCustomAttribute<ResourceUidAttribute>()?.Name ?? type.Name;
        }
    }

    public class RefLink {
        public string Name;
        public object Value;
        public bool Result;
    }

    internal class Range {
        public int From;
        public int To;

        public Range(int from = int.MinValue, int to = int.MaxValue) {
            From = from;
            To = to;
        }
    }

    public class XSerFormatter {
        public string GetSerialized(List<RefLink> list) {
            var sb = new StringBuilder(" ");

            foreach (var refLink in list) sb.AppendLine($"\"{refLink.Name}\":\"{refLink.Value}\",");
            return sb.ToString(0, sb.Length - 3);
        }
    }
}