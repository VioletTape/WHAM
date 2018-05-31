using AttemptB.Entities;
using NUnit.Framework;

namespace AttemptB {
    [TestFixture]
    public class BeerTests {
        [Test]
        public void Ser1() {
            var beer = new Beer(2) {
                Alk = 10,
                Name = "Komes"
            };

            var res = XSer.Serialize(beer);
        }
    }
}