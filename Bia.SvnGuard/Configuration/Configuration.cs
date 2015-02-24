using System;
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

        public string SvnLookPath
        {
            get { return Get("SvnLookPath"); }
            set { Set("SvnLookPath", value); }
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

        public string StylecopWrapper
        {
            get { return Get("StylecopWrapper"); }
            set { Set("StylecopWrapper", value); }
        }

        public string StylecopSettings
        {
            get { return Get("StylecopSettings"); }
            set { Set("StylecopSettings", value); }
        }

        public string TempFolder
        {
            get { return Get("TempFolder"); }
            set { Set("TempFolder", value); }
        }

        public bool FirstRun
        {
            get { return Convert.ToBoolean(Get("FirstRun")); }
            set { Set("FirstRun", value.ToString()); }
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
