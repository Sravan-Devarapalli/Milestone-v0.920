using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace WebPublisher.Settings
{
    public class WebInstanceElement : ConfigurationElement
    {
        #region Constructors
        static WebInstanceElement()
		{
		
		}
		#endregion

		#region Properties
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
		public string Name
		{
			get { return (string)base["name"]; }
            set { base["name"] = value; }
		}

        [ConfigurationProperty("site_url", IsRequired = true, DefaultValue="http://localhost")]
		public string SiteUrl
		{
			get { return (string)base["site_url"]; }
            set { base["site_url"] = value; }
		}

        [ConfigurationProperty("site_sources", IsRequired = true)]
        public string SiteSources
        {
            get { return (string)base["site_sources"]; }
            set { base["site_sources"] = value; }
        }

        [ConfigurationProperty("site_output")]
        public string SiteOutput
        {
            get { return (string)base["site_output"]; }
            set { base["site_output"] = value; }
        }


        [ConfigurationProperty("site_name", DefaultValue = "Default Web Site")]
        public string SiteName
        {
            get { return (string)base["site_name"]; }
            set { base["site_name"] = value; }
        }
        [ConfigurationProperty("include", DefaultValue = false)]
        public bool Include
        {
            get { return (bool)base["include"]; }
            set { base["include"] = value; }
        }
		#endregion
    }
}

