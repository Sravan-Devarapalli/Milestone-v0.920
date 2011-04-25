using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace WebPublisher.Settings
{
    public class ScriptSection: ConfigurationSection
    {
        public const string SectionName = "sqlscripts";

        public ScriptSection()
        {
            SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
        }

        [ConfigurationProperty("scripts")]
        public ScriptElementCollection Scripts
        {
            get { return this["scripts"] as ScriptElementCollection; }
            set { this["scripts"] = value; }
        }
    }
}

