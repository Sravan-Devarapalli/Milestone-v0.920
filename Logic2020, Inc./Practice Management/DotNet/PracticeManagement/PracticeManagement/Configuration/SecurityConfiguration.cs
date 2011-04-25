using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace PraticeManagement.Configuration
{
    public class PersonConfiguration : ConfigurationSection
    {
        #region Constants

        private const string CLIENT_ID_PROPERTY = "clientId";
        private const string PersonSectionName = "practiceManagement/personConfig";

        #endregion

        #region Properties

        public static PersonConfiguration Current
        {
            get
            {
                return ConfigurationManager.GetSection(PersonSectionName) as PersonConfiguration;
            }
        }

        [ConfigurationProperty(CLIENT_ID_PROPERTY)]
        public int ClientId
        {
            get
            {
                return (int)this[CLIENT_ID_PROPERTY];
            }
            set
            {
                this[CLIENT_ID_PROPERTY] = value;
            }
        }

        #endregion
    }
}

