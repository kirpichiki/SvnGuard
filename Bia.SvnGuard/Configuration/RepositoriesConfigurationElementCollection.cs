using System.Configuration;

namespace Bia.SvnGuard.Configuration
{
    public class RepositoriesConfigurationElementCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        public void Add(RepositoryConfigurationElement element)
        {
            BaseAdd(element);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new RepositoryConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RepositoryConfigurationElement) element).Name;
        }
    }
}