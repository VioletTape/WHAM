using System.Collections.Generic;
using AttemptB.Entities;
using Nancy;

namespace AttemptB.Web {
    public class HomeModule : NancyModule {
        private static readonly Dictionary<int, Beer> beers = new Dictionary<int, Beer>();

        public HomeModule() {
            Get["/beer/create"] = _ => CreateBeer(Request.Query);
            Get["/beer/"] = _ => GetElements();
            Get["/beer/{id}"] = _ => GetElement(_);
            Get["/beer/{id}/{command}"] = _ => GetElement(_);
        }


        private object GetElement(dynamic o) {
            return XSer.Serialize(beers[o.id]);
        }

        private object GetElements() {
            var list = new List<string>();
            foreach (var beer in beers) list.Add(XSer.Serialize(beer.Value));

            var join = string.Join(",", list.ToArray());

            return "[" + join + "]";
        }


        private object CreateBeer(dynamic o) {
            var beer = new Beer(o.id) {Alk = o.alk, Name = o.name};
            beers.Add(o.id, beer);
            return XSer.Serialize(beer);
        }
    }
}