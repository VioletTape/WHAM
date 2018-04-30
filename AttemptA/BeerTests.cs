using AttemptA.Entities;
using AttemptA.Entities.Origins;
using FluentAssertions;
using Jil;
using NUnit.Framework;

namespace AttemptA {
    [TestFixture]
    public class BeerTests {
        [Test]
        public void BeerWorksInGeneral() {
            var beer = new Beer(1);
            beer.Cool().Should().BeTrue();
            beer.IsOpened.Should().BeFalse();

            beer.Open().Should().BeTrue();
            beer.IsOpened.Should().BeTrue();
            beer.Cool().Should().BeFalse();
            beer.Open().Should().BeFalse();
        }

        [Test]
        public void SimpleJson() {
            var beer = new Beer(1) {Alk = 5.2m, Name = "Komes"};
            var serialize = JSON.Serialize(beer);

            serialize.Should()
                .Contain("{\"Alk\":5.2,\"IsOpened\":false,\"Id\":1,\"Name\":\"Komes\"}");
        }

        [Test]
        public void Attributes() {
            var beer = new Beer(1) {Alk = 5.2m, Name = "Komes"};

            var serialize = XSer.Serialize(beer);
        }
    }
}