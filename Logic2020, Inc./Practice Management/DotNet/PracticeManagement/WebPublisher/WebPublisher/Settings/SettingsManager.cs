using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace WebPublisher.Settings
{
    public class SettingsManager
    {
        #region simple singleton members
        private static SettingsManager _instance = new SettingsManager();

        public static SettingsManager Instance
        {
            get { return _instance; }
        }
        #endregion

        private Configuration config;
        private ConfigurationUserLevel configurationLevel = ConfigurationUserLevel.PerUserRoamingAndLocal;

        public SettingsManager()
        {
            config = ConfigurationManager.OpenExeConfiguration(configurationLevel);
            InitSections();
        }

        public WebInstanceSection WebSites { get; private set; }
        public ScriptSection SqlScripts { get; private set; }
        public string ConfigPath { get { return config.FilePath; } }

        private void InitSections()
        {
            WebSites = config.GetSection(WebInstanceSection.SectionName) as WebInstanceSection;
            if (WebSites == null)
            {
                WebSites = new WebInstanceSection();
                config.Sections.Add(WebInstanceSection.SectionName, WebSites);
                SaveSettings();
            }
            SqlScripts = config.GetSection(ScriptSection.SectionName) as ScriptSection;
            if (SqlScripts == null)
            {
                SqlScripts = new ScriptSection();
                config.Sections.Add(ScriptSection.SectionName, SqlScripts);
                SaveSettings();
            }
        }

        public void SaveSettings()
        {
            config.Save(ConfigurationSaveMode.Modified);
        }
    }
}

