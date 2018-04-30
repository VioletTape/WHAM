using System;

namespace AttemptA.Attributes {

    [AttributeUsage(AttributeTargets.Assembly)]
    public class MarkOriginsAttribute : Attribute
    {
        public string EntitiesFrom { get; set; }

        public string Uri { get; set; }
    }
}