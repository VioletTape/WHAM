using System;

namespace AttemptA.Attributes {
    public class ActionLinkAttribute : Attribute {
        public int Action { get; }
        public int Times { get; set; }
        public int DependsOn { get; set; }

        public ActionLinkAttribute() {
        }

        public ActionLinkAttribute(int action) {
            Action = action;
        }
    }
}