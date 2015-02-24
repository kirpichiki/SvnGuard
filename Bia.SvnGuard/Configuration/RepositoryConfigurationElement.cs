using System.Configuration;

namespace Bia.SvnGuard.Configuration
{
    public class RepositoryConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string) this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("enabled", IsRequired = true)]
        public bool Enabled
        {
            get { return (bool) this["enabled"]; }
            set { this["enabled"] = value; }
        }

        public void SaveChanges()
        {
            CurrentConfiguration.Save(ConfigurationSaveMode.Modified);
        }
    }
}