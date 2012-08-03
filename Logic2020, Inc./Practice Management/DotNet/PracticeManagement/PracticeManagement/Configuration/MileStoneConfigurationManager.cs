using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Configuration;

namespace PraticeManagement.Configuration
{
    /// <summary>
    /// Manages Default MileStone.
    /// </summary>
    internal class MileStoneConfigurationManager
    {
        private const string ClientKey = "DefaultClientId";
        private const string ProjectKey = "DefaultProjectId";
        private const string MileStoneKey = "DefaultMileStoneId";
        private const string LowerBoundkey = "LowerBound";
        private const string UpperBoundkey = "UpperBound";

        private static readonly object _settingsGuard = new object();
        private static readonly object _configGuard = new object();

        /// <summary>
        /// Gets the Default Client Id.
        /// </summary>
        private static int? DefaultClientId
        {
            get
            {
                lock (_settingsGuard)
                {
                    using (var service = new MilestoneService.MilestoneServiceClient())
                    {
                        var defaultMilestone = service.GetDefaultMilestone();
                        if (defaultMilestone != null)
                        {
                            SaveDefaultMileStoneSettingstoCache(defaultMilestone);
                            return defaultMilestone.ClientId;
                        }
                    }

                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the Default Project Id.
        /// </summary>
        private static int? DefaultProjectId
        {
            get
            {
                lock (_settingsGuard)
                {
                    using (var service = new MilestoneService.MilestoneServiceClient())
                    {
                        var defaultMilestone = service.GetDefaultMilestone();
                        if (defaultMilestone != null)
                        {
                            SaveDefaultMileStoneSettingstoCache(defaultMilestone);
                            return defaultMilestone.ProjectId;
                        }
                    }

                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the Default MileStone Id.
        /// </summary>
        private static int? DefaultMileStoneId
        {
            get
            {
                lock (_settingsGuard)
                {

                    using (var service = new MilestoneService.MilestoneServiceClient())
                    {
                        var defaultMilestone = service.GetDefaultMilestone();
                        if (defaultMilestone != null)
                        {
                            SaveDefaultMileStoneSettingstoCache(defaultMilestone);
                            return defaultMilestone.MilestoneId;
                        }
                    }

                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the Lower Bound.
        /// </summary>
        private static int? LowerBound
        {
            get
            {
                lock (_settingsGuard)
                {
                    using (var service = new MilestoneService.MilestoneServiceClient())
                    {
                        var defaultMilestone = service.GetDefaultMilestone();
                        if (defaultMilestone != null)
                        {
                            SaveDefaultMileStoneSettingstoCache(defaultMilestone);
                            return defaultMilestone.LowerBound;
                        }
                    }

                    return null;
                }
            }
        }

        private static void SaveDefaultMileStoneSettingstoCache(DataTransferObjects.DefaultMilestone defaultMilestone)
        {
            HttpContext.Current.Cache[ProjectKey] = defaultMilestone.ProjectId;
            HttpContext.Current.Cache[ClientKey] = defaultMilestone.ClientId;
            HttpContext.Current.Cache[MileStoneKey] = defaultMilestone.MilestoneId;
            HttpContext.Current.Cache[LowerBoundkey] = defaultMilestone.LowerBound;
            HttpContext.Current.Cache[UpperBoundkey] = defaultMilestone.UpperBound;
        }

        public static void ClearDefaultMileStoneSettingsinCache()
        {
            HttpContext.Current.Cache.Remove(ProjectKey);
            HttpContext.Current.Cache.Remove(ClientKey);
            HttpContext.Current.Cache.Remove(MileStoneKey);
            HttpContext.Current.Cache.Remove(LowerBoundkey);
            HttpContext.Current.Cache.Remove(UpperBoundkey);
        }

        /// <summary>
        /// Gets the Upper Bound.
        /// </summary>
        private static int? UpperBound
        {
            get
            {
                lock (_settingsGuard)
                {
                    using (var service = new MilestoneService.MilestoneServiceClient())
                    {
                        var defaultMilestone = service.GetDefaultMilestone();
                        if (defaultMilestone != null)
                        {
                            SaveDefaultMileStoneSettingstoCache(defaultMilestone);
                            return defaultMilestone.UpperBound;
                        }
                    }

                    return null;
                }
            }
        }

        /// <summary>
        /// Gets Default Client Id.
        /// </summary>
        /// <returns></returns>
        internal static int? GetClientId()
        {
            var clientIdValue = HttpContext.Current.Cache[ClientKey];
            int clientId;

            if (clientIdValue != null && Int32.TryParse(clientIdValue.ToString(), out clientId))
            {
                return clientId;
            }
            var varDefaultClientId = DefaultClientId;
            if (varDefaultClientId.HasValue)
                HttpContext.Current.Cache[ClientKey] = varDefaultClientId;

            return varDefaultClientId;
        }

        /// <summary>
        /// Gets Default Project Id.
        /// </summary>
        /// <returns></returns>
        internal static int? GetProjectId()
        {
            var projectIdValue = HttpContext.Current.Cache[ProjectKey];
            int projectId;

            if (projectIdValue != null && Int32.TryParse(projectIdValue.ToString(), out projectId))
            {
                return projectId;
            }
            var varDefaultProjectId = DefaultProjectId;
            if (varDefaultProjectId.HasValue)
                HttpContext.Current.Cache[ProjectKey] = varDefaultProjectId;

            return varDefaultProjectId;
        }

        /// <summary>
        /// Gets Default MileStone Id.
        /// </summary>
        /// <returns></returns>
        internal static int? GetMileStoneId()
        {
            var mileStoneIdValue = HttpContext.Current.Cache[MileStoneKey];
            int mileStoneId;

            if (mileStoneIdValue != null && Int32.TryParse(mileStoneIdValue.ToString(), out mileStoneId))
            {
                return mileStoneId;
            }
            var varDefaultMileStoneId = DefaultMileStoneId;
            if (varDefaultMileStoneId.HasValue)
                HttpContext.Current.Cache[MileStoneKey] = varDefaultMileStoneId;

            return varDefaultMileStoneId;
        }

        internal static int? GetLowerBound()
        {
            var strLowerBound = HttpContext.Current.Cache[LowerBoundkey];
            int lowerBound;

            if (strLowerBound != null && !string.IsNullOrEmpty(strLowerBound.ToString()) && Int32.TryParse(strLowerBound.ToString(), out lowerBound))
            {
                return lowerBound;
            }

            var varLowerBound = LowerBound;
            if (varLowerBound.HasValue)
                HttpContext.Current.Cache[LowerBoundkey] = varLowerBound;

            return varLowerBound;
        }

        internal static int? GetUpperBound()
        {
            var strUpperBound = HttpContext.Current.Cache[UpperBoundkey];
            int upperBound;

            if (strUpperBound != null && !string.IsNullOrEmpty(strUpperBound.ToString()) && Int32.TryParse(strUpperBound.ToString(), out upperBound))
            {
                return upperBound;
            }
            var varUpperBound = UpperBound;
            if (varUpperBound.HasValue)
                HttpContext.Current.Cache[UpperBoundkey] = varUpperBound;

            return varUpperBound;
        }

        /// <summary>
        /// Updates confgiuration element in the config appSettings config file.
        /// </summary>
        /// <param name="key">setting key</param>
        /// <param name="value">setting value</param>
        /// <throws>UnauthorizedAccessException if access to appSettings.config is denied</throws>
        private static void UpdateConfigurationElement(
            string key,
            string value)
        {
            // Get the configuration file.
            System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~");

            KeyValueConfigurationElement element = config.AppSettings.Settings[key];
            if (element != null)
            {
                element.Value = value;
            }
            else
            {
                config.AppSettings.Settings.Add(key, value);
            }

            lock (_configGuard)
            {
                // Save the configuration file.
                config.Save(ConfigurationSaveMode.Minimal);
            }

            // Force a reload of the changed section.
            ConfigurationManager.RefreshSection("appSettings");

            lock (_settingsGuard)
            {
                ConfigurationManager.AppSettings[key] = value;
            }
        }

        /// <summary>
        /// Returns Config value as string
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetConfigValueAsString(string key)
        {
            lock (_settingsGuard)
            {
                return ConfigurationManager.AppSettings[key];
            }
        }
    }
}

