using AttemptB.Attributes;

namespace AttemptB.Entities {
    public class Beer {
        public int Idx { get; }


        public bool IsOpened;
        public int Volume = 100;

        public string Name { get; set; }

        public decimal Alk { get; set; }

        public Beer(int id) {
            Idx = id;
        }

        [Link(NotAvailableWhen = nameof(IsOpened))]
        public bool Open() {
            if (IsOpened)
                return false;

            return IsOpened = true;
        }

        [Link(AvailableWhen = nameof(IsOpened))]
        public bool Cool() {
            if (IsOpened)
                return false;

            return true;
        }

        [Link(AvailableWhen = nameof(IsOpened))]
        [Link(AvailableWhen = nameof(Volume), MoreThan = 0)]
        public bool Drink(int sip) {
            if (IsOpened)
                // calculate amount of beer in bottle
                // after a sip
                // if something remains 
                return true;
            // esle return false. 

            return false;
        }
    }
}