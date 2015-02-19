using System.Configuration;

namespace Bia.SvnGuard.Configuration
{
    public class RepositoriesConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("repositories")]
        [ConfigurationCollection(typeof(RepositoriesConfigurationElementCollection), AddItemName = "repository")]
        public RepositoriesConfigurationElementCollection Repositories
        {
            get { return (RepositoriesConfigurationElementCollection) base["repositories"]; }
        }
    }
}