﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataTransferObjects;
using PraticeManagement.ConfigurationService;
using System.ServiceModel;
using DataTransferObjects.TimeEntry;
namespace PraticeManagement.Utils
{
    public class SettingsHelper
    {
        const string ApplicationSettingskey = "ApplicationSettings";
        const string CLIENT_MARGIN_COLORINFO_DEFAULT_THRESHOLDS_LIST_KEY = "CLIENT_MARGIN_COLORINFO_DEFAULT_THRESHOLDS_LIST_KEY";
        const string PERSON_MARGIN_COLORINFO_THRESHOLDS_LIST_KEY = "PERSON_MARGIN_COLORINFO_THRESHOLDS_LIST_KEY";
        const string TimeType_System = "Time_Type_System";

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

        public static List<ClientMarginColorInfo> GetMarginColorInfoDefaults(DefaultGoalType goaltype)
        {
            if (goaltype == DefaultGoalType.Client && HttpContext.Current.Cache[CLIENT_MARGIN_COLORINFO_DEFAULT_THRESHOLDS_LIST_KEY] != null)
            {
                return HttpContext.Current.Cache[CLIENT_MARGIN_COLORINFO_DEFAULT_THRESHOLDS_LIST_KEY] as List<ClientMarginColorInfo>;
            }
            else if (HttpContext.Current.Cache[PERSON_MARGIN_COLORINFO_THRESHOLDS_LIST_KEY] != null)
            {
                return HttpContext.Current.Cache[PERSON_MARGIN_COLORINFO_THRESHOLDS_LIST_KEY] as List<ClientMarginColorInfo>;
            }

            using (var serviceClient = new ConfigurationServiceClient())
            {
                try
                {
                    var result = serviceClient.GetMarginColorInfoDefaults(goaltype);

                    if (result != null)
                    {
                        var marginInfoList = result.AsQueryable().ToList();

                        if (goaltype == DefaultGoalType.Client)
                        {
                            HttpContext.Current.Cache[CLIENT_MARGIN_COLORINFO_DEFAULT_THRESHOLDS_LIST_KEY] = marginInfoList;
                        }
                        else
                        {
                            HttpContext.Current.Cache[PERSON_MARGIN_COLORINFO_THRESHOLDS_LIST_KEY] = marginInfoList;
                        }

                        return marginInfoList;
                    }

                    return null;
                }
                catch (FaultException<ExceptionDetail> ex)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }


        internal static void RemoveMarginColorInfoDefaults()
        {
            HttpContext.Current.Cache.Remove(CLIENT_MARGIN_COLORINFO_DEFAULT_THRESHOLDS_LIST_KEY);
            HttpContext.Current.Cache.Remove(PERSON_MARGIN_COLORINFO_THRESHOLDS_LIST_KEY);
        }

        public static DateTime GetCurrentPMTime()
        {
            DateTime currentDate;

            var timezone = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.TimeZoneKey);
            var isDayLightSavingsTimeEffect = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.IsDayLightSavingsTimeEffectKey);

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

        public static List<TimeTypeRecord> GetSystemTimeTypes()
        {
            if (HttpContext.Current.Cache[TimeType_System] == null)
            {
                using (var serviceClient = new TimeEntryService.TimeEntryServiceClient())
                {
                    HttpContext.Current.Cache[TimeType_System] = serviceClient.GetAllTimeTypes().Where(tt => tt.IsSystemTimeType).ToList();
                }
            }
            
            return HttpContext.Current.Cache[TimeType_System] as List<TimeTypeRecord>;
        }
    }
}
