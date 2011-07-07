namespace PraticeManagement.Configuration
{
    using System;
    using System.Configuration;
    using System.Web;
    using System.Web.Configuration;
    using PraticeManagement.Controls;
    using DataTransferObjects;
    using System.Drawing;
    using System.IO;
    using PraticeManagement.ConfigurationService;

    /// <summary>
    /// Manages company Logo and Image.
    /// </summary>
    internal class BrandingConfigurationManager
    {
        private const string LogoImagePathKey = "LogoImagePath";
        private const string CompanyTitleKey = "CompanyTitle";
        private const string CompanyLogoKey = "CompanyLogo";
        private const string imageUrl = @"~/Controls/CompanyLogoImage.ashx";
        private static readonly object _settingsGuard = new object();
        private static readonly object _configGuard = new object();

        /// <summary>
        /// Gets the configured image URL.
        /// </summary>
        private static string ImageUrl
        {
            get
            {
                return imageUrl;
            }
        }

        public static CompanyLogo LogoData
        {
            get
            {
                return BrandingConfigurationManager.GetCompanyLogoData();
            }
        }
        /// <summary>
        /// Gets the configured image URL.
        /// </summary>
        private static string CompanyTitle
        {
            get
            {
                return (LogoData != null && LogoData.Title != null) ? LogoData.Title : string.Empty;
            }
        }

        /// <summary>
        /// Gets configured logo image url.
        /// Checks whether it is valid virtual path, returns empty string if 
        /// invalid virtual path was set.
        /// </summary>
        /// <returns>Configured logo image URL.</returns>
        internal static string GetLogoImageUrl()
        {

            string logoImagePath = HttpContext.Current.Cache[LogoImagePathKey] as string;

            // Check if it was already added to cache.
            if (!string.IsNullOrEmpty(logoImagePath))
            {
                return logoImagePath;
            }

            logoImagePath = GetValidatedLogoPath();

            // Save URL.
            HttpContext.Current.Cache[LogoImagePathKey] = logoImagePath;

            return logoImagePath;
        }

        /// <summary>
        /// Gets configured company title.
        /// </summary>
        /// <returns></returns>
        internal static string GetCompanyTitle()
        {
            string title = HttpContext.Current.Cache[CompanyTitleKey] as string;

            // Check if it was already added to cache.
            if (!string.IsNullOrEmpty(title))
            {
                return title;
            }
            else
            {
                HttpContext.Current.Cache[CompanyTitleKey] = CompanyTitle;
                return CompanyTitle;
            }
        }

        /// <summary>
        /// Updates logo image path value.
        /// </summary>
        /// <param name="url"></param>
        internal static void UpdateLogoImagePath(string url)
        {
            UpdateConfigurationElement(LogoImagePathKey, HttpUtility.UrlEncode(url));
            HttpContext.Current.Cache[LogoImagePathKey] = GetValidatedLogoPath();
        }

        /// <summary>
        /// Updates company title value.
        /// </summary>
        /// <param name="title">title value</param>
        internal static void UpdateCompanyTitle(string title)
        {
            title = HttpUtility.HtmlAttributeEncode(title);
            UpdateConfigurationElement(CompanyTitleKey, title);
            HttpContext.Current.Cache[CompanyTitleKey] = title;
        }

        /// <summary>
        /// Gets validate logo path.
        /// </summary>
        /// <returns></returns>
        private static string GetValidatedLogoPath()
        {
            string validatedUrl = HttpUtility.UrlDecode(ImageUrl);

            // If it is virtual path we must check it.
            if (validatedUrl.StartsWith("~", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    HttpContext.Current.Server.MapPath(validatedUrl);
                }
                catch (HttpException)
                {
                    // Invalid virtual path.
                    validatedUrl = string.Empty;
                }
            }

            return validatedUrl;
        }

        /// <summary>
        /// Updates confgiuration element in the config appSettings config file.
        /// </summary>
        /// <param name="key">setting key</param>
        /// <param name="value">setting value</param>
        /// <throws>UnauthorizedAccessException if access to appSettings.config is denied</throws>
        private static void UpdateConfigurationElement(string key, string value)
        {
            // Get the configuration file.
            Configuration config = WebConfigurationManager.OpenWebConfiguration("~");

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

        public static void SaveCompanyLogoData(string title, string imagename, string imagePath, Byte[] data)
        {
            using (var serviceClient = new ConfigurationServiceClient())
            {
                try
                {
                    serviceClient.SaveCompanyLogoData(title, imagename, imagePath, data);

                    CompanyLogo companyLogo = new CompanyLogo();
                    companyLogo.Title = title;
                    companyLogo.FileName = imagename;
                    companyLogo.FilePath = imagePath;
                    companyLogo.Data = data;

                    HttpContext.Current.Cache[CompanyLogoKey] = companyLogo;
                }
                catch(Exception e)
                {
                    serviceClient.Abort();
                }
            }

            HttpContext.Current.Cache[LogoImagePathKey] = GetValidatedLogoPath();
            HttpContext.Current.Cache[CompanyTitleKey] = title;
        }

        private static CompanyLogo GetCompanyLogoData()
        {
            if (HttpContext.Current.Cache[CompanyLogoKey] == null)
            {
                using (var serviceClient = new ConfigurationServiceClient())
                {
                    HttpContext.Current.Cache[CompanyLogoKey] = serviceClient.GetCompanyLogoData();
                }
            }

            return (CompanyLogo)HttpContext.Current.Cache[CompanyLogoKey];
        }
    }
}

