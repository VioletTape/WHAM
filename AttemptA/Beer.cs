using System;
using System.Runtime.CompilerServices;

namespace AttemptA {
    public class Beer {
        [ResourceUid] public int Id;

        public bool IsOpened;

        public Beer(int id) {
            Id = id;
        }

        public string Name { get; set; }

        public decimal Alk { get; set; }

        [ActionLink(1)]
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
    }

    public class ResourceUidAttribute : Attribute { }

    public class ActionLinkAttribute : Attribute {
        public int DependsOn {get; set; }
        public ActionLinkAttribute() { }
        public ActionLinkAttribute(string depOn) { }
        public ActionLinkAttribute(int action) { }
    }
}