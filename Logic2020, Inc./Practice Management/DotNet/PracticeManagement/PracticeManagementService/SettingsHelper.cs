using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataTransferObjects;
using System.ServiceModel;
using DataAccess;

namespace PracticeManagementService
{
    public class SettingsHelper
    {
        const string ApplicationSettingskey = "ApplicationSettings";

        public static Dictionary<string, string> GetResourceKeyValuePairs(SettingsType settingType)
        {
            return ConfigurationDAL.GetResourceKeyValuePairs(settingType);
        }

        public static string GetResourceValueByTypeAndKey(SettingsType settingType, string key)
        {
            if (HttpContext.Current.Cache[ApplicationSettingskey] == null)
            {
                HttpContext.Current.Cache[ApplicationSettingskey] = new Dictionary<SettingsType, Dictionary<string, string>>();
            }
            var mainDictionary = HttpContext.Current.Cache[ApplicationSettingskey] as Dictionary<SettingsType, Dictionary<string, string>>;
            if (!mainDictionary.Keys.Any(k=>k == settingType))
            {
                mainDictionary[settingType] = GetResourceKeyValuePairs(settingType);
            }

            if (mainDictionary[settingType].Any(kvp=>kvp.Key==key))
            {
                return mainDictionary[settingType][key] as string;
            }
            else
            {
                return string.Empty;
            }
        }

        public static void SaveResourceValueToCache(SettingsType settingType, string key, string value)
        {
            if (HttpContext.Current.Cache[ApplicationSettingskey] != null)
            {
                if (HttpContext.Current.Cache[ApplicationSettingskey] == null)
                {
                    HttpContext.Current.Cache[ApplicationSettingskey] = new Dictionary<SettingsType, Dictionary<string, string>>();
                }
                var mainDictionary = HttpContext.Current.Cache[ApplicationSettingskey] as Dictionary<SettingsType, Dictionary<string, string>>;
                if (!mainDictionary.Keys.Any(k=>k ==settingType))
                {
                    mainDictionary[settingType] = GetResourceKeyValuePairs(settingType);
                }
                mainDictionary[settingType][key] = value;
            }
        }

        public static SMTPSettings GetSMTPSettings()
        {
            var sMTPSettings = new SMTPSettings();
            sMTPSettings.MailServer = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.SMTP, Constants.ResourceKeys.MailServerKey);

            var sslEnabledString = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.SMTP, Constants.ResourceKeys.SSLEnabledKey);
            bool sSLEnabled;
            if (!string.IsNullOrEmpty(sslEnabledString) && bool.TryParse(sslEnabledString, out sSLEnabled))
            {
                sMTPSettings.SSLEnabled = sSLEnabled;
            }

            int portNumber;
            var portNumberString = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.SMTP, Constants.ResourceKeys.PortNumberKey);
            if (!string.IsNullOrEmpty(portNumberString) && Int32.TryParse(portNumberString, out portNumber))
            {
                sMTPSettings.PortNumber = portNumber;
            }
            else
            {
                if (sMTPSettings.SSLEnabled)
                    sMTPSettings.PortNumber = 25;
                else
                    sMTPSettings.PortNumber = 465;
            }

            sMTPSettings.UserName =
                SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.SMTP, Constants.ResourceKeys.UserNameKey);

            sMTPSettings.Password =
                SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.SMTP, Constants.ResourceKeys.PasswordKey);

            sMTPSettings.PMSupportEmail =
                SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.SMTP, Constants.ResourceKeys.PMSupportEmailAddressKey);

            return sMTPSettings;
        }

    }

}
