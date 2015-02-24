using Bia.SvnGuard.Configuration;

namespace Bia.SvnGuard.ViewModels
{
    public class RepositoryViewModel : ViewModelBase
    {
        private readonly RepositoryConfigurationElement _configurationElement;

        public string Name
        {
            get { return _configurationElement.Name; }
            set
            {
                _configurationElement.Name = value;
                _configurationElement.SaveChanges();
            }
        }

        public bool Enabled
        {
            get { return _configurationElement.Enabled; }
            set
            {
                _configurationElement.Enabled = value;
                _configurationElement.SaveChanges();
            }
        }

        public RepositoryViewModel(RepositoryConfigurationElement configurationElement)
        {
            _configurationElement = configurationElement;
        }
    }
}
