using System.Collections.Generic;
using System.Text;
using AttemptA.Entities.Origins;
using Nancy;

namespace AttemptA.Web {
    public class CupModule : NancyModule {
        public CupModule() {
            XSer.Register<Cup>();

            Get["/cup/"] = _ => CreateCup();
        }

        private object CreateCup() {
            var cup = new Cup {Number = 12, Name = "Big cup"};
            return XSer.Serialize(cup);
        }
    }

    public class HomeModule : NancyModule {
        private static Dictionary<int, Beer> beers = new Dictionary<int, Beer>();

        public HomeModule() {
            XSer.Register<Beer>();

            Get["/beer/create"] = _ => CreateBeer(Request.Query);
            Get["/beer/"] = _ => GetElements();
            Get["/beer/{id}"] = _ => GetElement(_);
            Get["/beer/{id}/{method}"] = _ => BeerHandler(_);
            Get["/beer/{id}/init"] = _ => BeerReset(_);
        }


        private object GetElement(dynamic o) {
            return XSer.Serialize(beers[o.id]);
        }

        private object GetElements()
        {
            var list = new List<string>();
            foreach (var beer in beers) {
                list.Add(XSer.Serialize(beer.Value));
            }

            var @join = string.Join(",", list.ToArray());

            return "[" + @join + "]";
        }

        // Should not be necessary in real life. 
        // Artificial construction only for the sake of tests
        // todo: remove later
        private object BeerReset(dynamic o) {
            XSer.ResetCounters(beers[o.id]);
            return XSer.Serialize(beers[o.id]);
        }


        private object CreateBeer(dynamic o) {
            var beer = new Beer(o.id) {Alk = o.alk, Name = o.name};
            beers.Add(o.id, beer);
            return XSer.Serialize(beer);
        }

        private object BeerHandler(dynamic o) {
            return XSer.Handle(beers[o.id], (string) o.method);
        }
    }
}