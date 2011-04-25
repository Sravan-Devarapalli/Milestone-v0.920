using DataTransferObjects.Utils;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;

namespace DataTransferObjects
{
    public class Settings
    {
        #region Constants

        private const string SenioritySeparationRangeValue = "senioritySeparationRangeValue";

        #endregion

        #region Properties

        public static int SenioritySeparationRange
        {
            get
            {
                return GetSenioritySeparationRange(SenioritySeparationRangeValue);
            }
        }

        #endregion

        public static int GetSenioritySeparationRange(string key)
        {
            if (IsAzureWebRole())
            {
                try
                {
                    return Convert.ToInt32(RoleEnvironment.GetConfigurationSettingValue(key));
                }
                catch
                {
                    return Generic.GetIntConfiguration(SenioritySeparationRangeValue); ;
                }
            }
            else
            {
                return Generic.GetIntConfiguration(SenioritySeparationRangeValue);;
            }
        }

        private static Boolean IsAzureWebRole()
        {
            try
            {
                if (RoleEnvironment.IsAvailable)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}

