using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataTransferObjects;
using PraticeManagement.ConfigurationService;
using System.ServiceModel;
namespace PraticeManagement.Utils
{
    public class SettingsHelper
    {
        const string ApplicationSettingskey = "ApplicationSettings";
        public static Dictionary<string, string> GetResourceKeyValuePairs(SettingsType settingType)
        {
            using (var serviceClient = new ConfigurationServiceClient())
            {
                try
                {
                    return serviceClient.GetResourceKeyValuePairs(settingType);
                }
                catch (FaultException<ExceptionDetail> ex)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        public static string GetResourceValueByTypeAndKey(SettingsType settingType, string key)
        {
            if (HttpContext.Current.Cache[ApplicationSettingskey] == null)
            {
                HttpContext.Current.Cache[ApplicationSettingskey] = new Dictionary<SettingsType, Dictionary<string, string>>();
            }
            var mainDictionary = HttpContext.Current.Cache[ApplicationSettingskey] as Dictionary<SettingsType, Dictionary<string, string>>;
            if (!mainDictionary.Any(kvp => kvp.Key == settingType))
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

        public static void SaveResourceKeyValuePairs(SettingsType settingType, Dictionary<string, string> dictionary)
        {
            using (var serviceClient = new ConfigurationServiceClient())
            {
                try
                {
                    serviceClient.SaveResourceKeyValuePairs(settingType, dictionary); ;
                }
                catch (FaultException<ExceptionDetail> ex)
                {
                    serviceClient.Abort();
                    throw;
                }
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

        public static void SaveSMTPSettings(SMTPSettings sMTPSettings)
        {
            SaveResourceKeyValuePairItem(SettingsType.SMTP, Constants.ResourceKeys.MailServerKey, sMTPSettings.MailServer);
            SaveResourceKeyValuePairItem(SettingsType.SMTP, Constants.ResourceKeys.PortNumberKey, sMTPSettings.PortNumber.ToString());
            SaveResourceKeyValuePairItem(SettingsType.SMTP, Constants.ResourceKeys.SSLEnabledKey, sMTPSettings.SSLEnabled.ToString());
            SaveResourceKeyValuePairItem(SettingsType.SMTP, Constants.ResourceKeys.UserNameKey, sMTPSettings.UserName);
            SaveResourceKeyValuePairItem(SettingsType.SMTP, Constants.ResourceKeys.PasswordKey, sMTPSettings.Password);
            SaveResourceKeyValuePairItem(SettingsType.SMTP, Constants.ResourceKeys.PMSupportEmailAddressKey, sMTPSettings.PMSupportEmail);
        }

        public static void SaveResourceKeyValuePairItem(SettingsType settingType, string key, string value)
        {
            if (HttpContext.Current.Cache[ApplicationSettingskey] == null)
            {
                HttpContext.Current.Cache[ApplicationSettingskey] = new Dictionary<SettingsType, Dictionary<string, string>>();
            }
            var mainDictionary = HttpContext.Current.Cache[ApplicationSettingskey] as Dictionary<SettingsType, Dictionary<string, string>>;
            if (!mainDictionary.Any(kvp => kvp.Key == settingType))
            {
                mainDictionary[settingType] = GetResourceKeyValuePairs(settingType);
            }

            mainDictionary[settingType][key] = value;

            using (var serviceClient = new ConfigurationServiceClient())
            {
                try
                {
                    serviceClient.SaveResourceKeyValuePairItem(settingType, key, value);
                }
                catch (FaultException<ExceptionDetail> ex)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }
    }
}
