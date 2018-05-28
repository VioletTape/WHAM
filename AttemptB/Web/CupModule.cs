using Nancy;

namespace AttemptB.Web {
    public class CupModule : NancyModule {
        public CupModule() {

            Get["/cup/"] = _ => CreateCup();
        }

        private object CreateCup() {
            return "";
        }
    }
}