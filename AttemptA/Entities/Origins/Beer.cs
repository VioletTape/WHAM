using AttemptA.Attributes;

namespace AttemptA.Entities.Origins {
    public class Beer {
        [ResourceUid]
        public int Idx { get; }

        public bool IsOpened;

        public string Name { get; set; }

        public decimal Alk { get; set; }

        public Beer(int id) {
            Idx = id;
        }

        [ActionLink(1, Times = 1)]
        public bool Open() {
            if (IsOpened)
                return false;

            return IsOpened = true;
        }

        [ActionLink(2)]
        public bool Cool() {
            if (IsOpened)
                return false;

            return true;
        }

        [ActionLink(3, DependsOn = 1)]
        public bool Drink() {
            return IsOpened;
        }

        public void Foo() {
        }
    }

    public class Cup {
        [ResourceUid]
        public int  Number { get; set; }

        public string Name { get; set; }
    }
}