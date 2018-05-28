using System;

namespace AttemptB.Attributes {
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class LinkAttribute : Attribute {
        public string AvailableWhen { get; set; }

        public string NotAvailableWhen { get; set; }

        public int MoreThan { get; set; }
        public int LessThan { get; set; }
        public int Is { get; set; }
    }
}