using System.Runtime.InteropServices;
using Jil;

namespace AttemptA.Web
{
    public class HomeModule : Nancy.NancyModule
    {
        private static Beer beer;

        public HomeModule()
        {
            XSer.Register<Beer>();
          
            Get["/beer/"] = _ => CreateBeer();

            // case 1           
//            Get["/beer/{id}/open"] = _ => OpenBeer(_);

            // case 2
            Get["/beer/{id}/{method}"] = _ => BeerHandler(_);
            Get["/beer/{id}/init"] = _ => BeerReset();
        }

        private object BeerReset() {
            XSer.ResetCounters(beer);
            return XSer.Serialize(beer);
        }

        private object CreateBeer() {
            beer = new Beer(1) { Alk = 12, Name = "Barely wine" };
            return XSer.Serialize(beer);
        }

        // case 2
        private object BeerHandler(dynamic o) {
            var beerId = (int)o.id;
            var method = (string) o.method;

            var handle = XSer.Handle(beer, method);

            return handle;
        }

        // case 1 
        private object OpenBeer(dynamic o) {
            var id = (int)o.id;

           beer.Open();

            return XSer.Serialize(beer, _ => beer.Open());
        }
    }
}