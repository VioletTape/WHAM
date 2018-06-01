using System.Configuration;

namespace AttemptB.Config {
    public class WhamSettings : ConfigurationSection {
        [ConfigurationProperty("domainUrl")]
        public DomainUrlElement DomainUrl
        {
            get => (DomainUrlElement)this["domainUrl"];
            set => this["domainUrl"] = value;
        }
    }

    public class DomainUrlElement : ConfigurationElement {
        [ConfigurationProperty("name", DefaultValue = "http://localhost", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{};'\"|\\", MinLength = 10)]
        public string Name {
            get => (string) this["name"];
            set => this["name"] = value;
        }
    }
}