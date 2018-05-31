using AttemptB.Entities;
using FluentAssertions;
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
            res.Should().Be("");
        }

        [Test]
        public void Ser2()
        {
            var beer = new Beer(2)
            {
                Alk = 10,
                Name = "Komes",
                IsOpened = true
            };

            var res = XSer.Serialize(beer);
            res.Should().Be("");
        }
    }
}