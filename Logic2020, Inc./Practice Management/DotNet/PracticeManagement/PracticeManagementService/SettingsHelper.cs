using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAccess;
using DataTransferObjects;

namespace PracticeManagementService
{
    public class SettingsHelper
    {
        private const string ApplicationSettingskey = "ApplicationSettings";

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
            if (mainDictionary != null && mainDictionary.Keys.All(k => k != settingType))
            {
                mainDictionary[settingType] = GetResourceKeyValuePairs(settingType);
            }

            if (mainDictionary != null && mainDictionary[settingType].Any(kvp => kvp.Key == key))
            {
                return mainDictionary[settingType][key];
            }
            return string.Empty;
        }

        public static void SaveResourceValueToCache(SettingsType settingType, string key, string value)
        {
            if (HttpContext.Current.Cache[ApplicationSettingskey] == null) return;
            if (HttpContext.Current.Cache[ApplicationSettingskey] == null)
            {
                HttpContext.Current.Cache[ApplicationSettingskey] = new Dictionary<SettingsType, Dictionary<string, string>>();
            }
            var mainDictionary = HttpContext.Current.Cache[ApplicationSettingskey] as Dictionary<SettingsType, Dictionary<string, string>>;
            if (mainDictionary != null && mainDictionary.Keys.All(k => k != settingType))
            {
                mainDictionary[settingType] = GetResourceKeyValuePairs(settingType);
            }
            if (mainDictionary != null) mainDictionary[settingType][key] = value;
        }

        public static SMTPSettings GetSMTPSettings()
        {
            var sMTPSettings = new SMTPSettings
                {
                    MailServer =
                        GetResourceValueByTypeAndKey(SettingsType.SMTP, Constants.ResourceKeys.MailServerKey)
                };

            var sslEnabledString = GetResourceValueByTypeAndKey(SettingsType.SMTP, Constants.ResourceKeys.SSLEnabledKey);
            bool sSLEnabled;
            if (!string.IsNullOrEmpty(sslEnabledString) && bool.TryParse(sslEnabledString, out sSLEnabled))
            {
                sMTPSettings.SSLEnabled = sSLEnabled;
            }

            int portNumber;
            var portNumberString = GetResourceValueByTypeAndKey(SettingsType.SMTP, Constants.ResourceKeys.PortNumberKey);
            if (!string.IsNullOrEmpty(portNumberString) && Int32.TryParse(portNumberString, out portNumber))
            {
                sMTPSettings.PortNumber = portNumber;
            }
            else
            {
                sMTPSettings.PortNumber = sMTPSettings.SSLEnabled ? 25 : 465;
            }

            sMTPSettings.UserName =
                GetResourceValueByTypeAndKey(SettingsType.SMTP, Constants.ResourceKeys.UserNameKey);

            sMTPSettings.Password =
                GetResourceValueByTypeAndKey(SettingsType.SMTP, Constants.ResourceKeys.PasswordKey);

            sMTPSettings.PMSupportEmail =
                GetResourceValueByTypeAndKey(SettingsType.SMTP, Constants.ResourceKeys.PMSupportEmailAddressKey);

            return sMTPSettings;
        }

        public static DateTime GetCurrentPMTime()
        {
            DateTime currentDate;

            var timezone = GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.TimeZoneKey);
            var isDayLightSavingsTimeEffect = GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.IsDayLightSavingsTimeEffectKey);

            if (timezone == "-08:00" && isDayLightSavingsTimeEffect.ToLower() == "true")
            {
                currentDate = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
            }
            else
            {
                var timezoneWithoutSign = timezone.Replace("+", string.Empty);
                TimeZoneInfo ctz = TimeZoneInfo.CreateCustomTimeZone("cid", TimeSpan.Parse(timezoneWithoutSign), "customzone", "customzone");
                currentDate = TimeZoneInfo.ConvertTime(DateTime.UtcNow, ctz);
            }

            return currentDate;
        }
    }
}
