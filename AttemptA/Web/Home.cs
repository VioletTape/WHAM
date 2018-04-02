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
            Get["/beer/{id}/{method}"] = _ => BeerHandler(_);
            Get["/beer/{id}/init"] = _ => BeerReset();
        }

        // Should not be necessary in real life. 
        // Artificial construction only for the sake of tests
        // todo: remove later
        private object BeerReset() {
            XSer.ResetCounters(beer);
            return XSer.Serialize(beer);
        }


        private object CreateBeer() {
            beer = new Beer(1) { Alk = 12, Name = "Barely wine" };
            return XSer.Serialize(beer);
        }

        private object BeerHandler(dynamic o) {
            return XSer.Handle(beer, (string) o.method);
        }
    }
}