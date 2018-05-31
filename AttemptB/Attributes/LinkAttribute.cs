using System;

namespace AttemptB.Attributes {
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class LinkAttribute : Attribute {
        public string AvailableWhen { get; set; }

        public string NotAvailableWhen { get; set; }

        public string LinkName { get; set; }

        public int MoreThan { get; set; }
        public int LessThan { get; set; }
        public int Is { get; set; }

        public LinkAttribute() {
            AvailableWhen = string.Empty;
            NotAvailableWhen = string.Empty;
            LinkName = string.Empty;
        }

        public string GetNameOfFiled() {
            return AvailableWhen ?? NotAvailableWhen;
        }
    }
}