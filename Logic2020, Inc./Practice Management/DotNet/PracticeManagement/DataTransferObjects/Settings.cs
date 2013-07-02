using System;
using DataTransferObjects.Utils;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace DataTransferObjects
{
    public class Settings
    {
        #region Constants

        private const string SenioritySeparationRangeValue = "senioritySeparationRangeValue";

        #endregion Constants

        #region Properties

        public static int SenioritySeparationRange
        {
            get
            {
                return GetSenioritySeparationRange(SenioritySeparationRangeValue);
            }
        }

        #endregion Properties

        public static int GetSenioritySeparationRange(string key)
        {
            if (!IsAzureWebRole())
            {
                return Generic.GetIntConfiguration(SenioritySeparationRangeValue);
            }
            try
            {
                return Convert.ToInt32(RoleEnvironment.GetConfigurationSettingValue(key));
            }
            catch
            {
                return Generic.GetIntConfiguration(SenioritySeparationRangeValue);
            }
        }

        private static Boolean IsAzureWebRole()
        {
            try
            {
                return RoleEnvironment.IsAvailable;
            }
            catch
            {
                return false;
            }
        }
    }
}
