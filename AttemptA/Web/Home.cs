using Jil;

namespace AttemptA.Web
{
    public class HomeModule : Nancy.NancyModule
    {
        public HomeModule()
        {
            var beer = new Beer(1){Alk = 12, Name = "Barely wine"};
            var serialize = XSer.Serialize(beer);
            Get["/beer/"] = _ => serialize;

            Get["/beer/{id}/open"] = _ => OpenBeer(_);
        }

        private object OpenBeer(object o) {
            var beer = new Beer(1){Alk = 12, Name = "Barely wine"};

            beer.Open();

            return JSON.Serialize(beer);
        }
    }
}