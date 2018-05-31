using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AttemptB.Attributes;
using FluentAssertions.Common;
using Jil;

namespace AttemptB {
    public static class XSer {
        public static string Serialize<T>(T obj) {
            var refLinks = new List<RefLink>();

            var res = JSON.Serialize(obj);

            var linkMethods = GetActionLinkMethods<T>();

            foreach (var linkMethod in linkMethods) {
                var tempLinks = new List<RefLink>();
                foreach (var link in linkMethod.GetCustomAttributes<LinkAttribute>()) {
                    var fieldY = obj.GetType().GetField(link.AvailableWhen);
                    if (fieldY != null) {
                        var o = fieldY.GetValue(obj);

                        switch (o) {
                            case int i:
                                tempLinks.Add(new RefLink {
                                    Name = linkMethod.Name,
                                    Value = "link?",
                                    Result = i > link.MoreThan
                                });
                                break;
                            case bool b:
                                tempLinks.Add(new RefLink {
                                    Name = linkMethod.Name,
                                    Value = "link?",
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
                                    Value = "link?",
                                    Result = i < link.MoreThan
                                });
                                break;
                            case bool b:
                                tempLinks.Add(new RefLink {
                                    Name = linkMethod.Name,
                                    Value = "link?",
                                    Result = !b
                                });
                                ;
                                break;
                        }

                        continue;
                    }

                    tempLinks.Add(new RefLink {
                        Name = linkMethod.Name,
                        Value = "link?",
                        Result = true
                    });

                    var property = obj.GetType().GetProperty(link.AvailableWhen);
                }

                if (tempLinks.Count > 1) {
                    if (tempLinks.All(l => l.Result))
                        refLinks.Add(tempLinks.First());
                }
                else if (tempLinks.First().Result){
                    refLinks.Add(tempLinks.First());
                }
            }

            var serialized = new XSerFormatter().GetSerialized(refLinks);

            res = res.Substring(0, res.Length - 1) + "," + serialized + "}";

            return res;
        }

        public static List<MethodInfo> GetActionLinkMethods<T>() {
            return typeof(T).GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(mi => mi.HasAttribute<LinkAttribute>())
                .Select(mi => mi)
                .ToList();
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