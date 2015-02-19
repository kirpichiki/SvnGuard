using System.Configuration;

namespace Bia.SvnGuard.Configuration
{
    public class Configuration
    {
        private readonly System.Configuration.Configuration _configuration;

        public Configuration()
        {
            _configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        public string SvnUtilitiesPath
        {
            get { return Get("SvnUtilitiesPath"); }
            set { Set("SvnUtilitiesPath", value); }
        }

        public string StylecopPath
        {
            get { return Get("StylecopPath"); }
            set { Set("StylecopPath", value); }
        }

        public string RepositoriesPath
        {
            get { return Get("RepositoriesPath"); }
            set { Set("RepositoriesPath", value); }
        }

        public RepositoriesConfigurationSection RepositoriesConfig
        {
            get { return _configuration.GetSection("repositoriesConfiguration") as RepositoriesConfigurationSection; }
        }

        private string Get(string setting)
        {
            return _configuration.AppSettings.Settings[setting].Value;
        }

        private void Set(string setting, string value)
        {
            _configuration.AppSettings.Settings[setting].Value = value;
            _configuration.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            Properties.Settings.Default.Reload();
        }
    }
}
