using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PraticeManagement.Configuration;
using System.Reflection;
using System.IO;
using System.Drawing;

namespace PraticeManagement.Controls
{
    /// <summary>
    /// Summary description for CompanyLogoImage
    /// </summary>
    public class CompanyLogoImage : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            byte[] buffer = BrandingConfigurationManager.LogoData.Data;

            context.Response.ContentType = "image/jpeg";
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
