﻿using System.Configuration;

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
    }
}