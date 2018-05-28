using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AttemptB.Attributes;
using FluentAssertions.Common;
using Jil;

namespace AttemptB {
    public static class XSer {


        public static string Serialize<T>(T obj, string method = "") {
            var res = JSON.Serialize(obj);
            
            return res;
        }

        public static List<MethodInfo> GetActionLinkMethods<T>() {
            return typeof(T).GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(mi => mi.HasAttribute<LinkAttribute>())
                .Select(mi => mi)
                .ToList();
        }

        

       

       
    }
}