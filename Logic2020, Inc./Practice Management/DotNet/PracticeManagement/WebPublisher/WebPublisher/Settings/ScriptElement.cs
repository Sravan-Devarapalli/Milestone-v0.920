using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace WebPublisher.Settings
{
    public class ScriptElement: ConfigurationElement
    {
        #region Constructors
        public ScriptElement()
		{
		
		}
		#endregion

		#region Properties
        //public string Name
        //{
        //    get { return System.IO.Path.GetFileName(Path); }
        //}

        [ConfigurationProperty("path", IsRequired = true, IsKey = true)]
        public string Path
        {
            get { return (string)base["path"]; }
            set { base["path"] = value; }
        }

        [ConfigurationProperty("include", IsRequired = true)]
        public bool Include
        {
            get { return (bool)base["include"]; }
            set { base["include"] = value; }
        }

        [ConfigurationProperty("category", IsRequired = true)]
        public string Category
        {
            get { return (string)base["category"]; }
            set { base["category"] = value; }
        }
		#endregion
    }
}

