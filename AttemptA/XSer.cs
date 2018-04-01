using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FluentAssertions.Common;
using Jil;

namespace AttemptA {
    public static class XSer {
        public static string Serialize<T>(T obj) {
            var res = JSON.Serialize(obj);
//            return res;

            var methodInfos = typeof(T).GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(mi => mi.HasAttribute<ActionLinkAttribute>())
                .Select(mi => mi);


            var list = new List<string>();

            foreach (var info in methodInfos) {
                list.Add($"\"{info.Name}\":\"http://localhost:1234/{typeof(T).Name}/1/{info.Name}\"");
//                list.Add($"\"{info.Name}\":\"{typeof(T).Name}\"");
                
            }

            var sb = new StringBuilder(res.Substring(0,res.Length-1));
            foreach (var l in list) {
                sb.Append("," + l);
            }

            sb.Append("}");
            var serialize = sb.ToString();
            return serialize;
        }

        
    }
}