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
            if (context.Request.QueryString["Type"] != null && context.Request.QueryString["Type"] == "download")
            {
                RequestToDownload(context);
            }
            else
            {
                RequestToRead(context);
            }
        }

        private void RequestToRead(HttpContext context)
        {
            byte[] buffer = BrandingConfigurationManager.LogoData.Data;

            context.Response.ContentType = "image/jpeg";
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        private void RequestToDownload(HttpContext context)
        {
            byte[] data = BrandingConfigurationManager.LogoData.Data;
            string fileName = BrandingConfigurationManager.LogoData.FileName;

            context.Response.Clear();
            context.Response.AddHeader(
                "content-disposition", string.Format("attachment; filename={0}", fileName));
            context.Response.ContentType = "image/jpeg";

            int length = data.Length;
            int bytes;
            byte[] buffer = new byte[1024];

            Stream outStream = context.Response.OutputStream;
            using (MemoryStream stream = new MemoryStream(data))
            {
                while (length > 0 && (bytes = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outStream.Write(buffer, 0, bytes);
                    context.Response.Flush();
                    length -= bytes;
                }
            }
            context.Response.End();
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
