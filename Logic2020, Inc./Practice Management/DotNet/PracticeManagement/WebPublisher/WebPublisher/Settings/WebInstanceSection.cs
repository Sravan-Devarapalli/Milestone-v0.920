using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace WebPublisher.Settings
{
    public class WebInstanceSection : ConfigurationSection
    {
        public const string SectionName = "websites";

        public WebInstanceSection()
        {
            SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
        }

        [ConfigurationProperty("websites")]
        public WebInstanceElementCollection Default
        {
            get { return this["websites"] as WebInstanceElementCollection; }
            set { this["websites"] = value; }
        }

        [ConfigurationProperty("msbuild_path", DefaultValue=@"C:\Windows\Microsoft.NET\Framework\v3.5\msbuild.exe")]
        public string MSBuildPath
        {
            get { return this["msbuild_path"] as string; }
            set { this["msbuild_path"] = value; }
        }
    }
}

