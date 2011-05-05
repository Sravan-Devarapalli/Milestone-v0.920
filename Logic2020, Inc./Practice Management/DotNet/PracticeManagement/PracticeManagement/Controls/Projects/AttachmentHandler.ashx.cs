using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace PraticeManagement.Controls.Projects
{
    /// <summary>
    /// Summary description for ProjectAttachmentHandler
    /// </summary>
    public class ProjectAttachmentHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int projectId = Convert.ToInt32(context.Request.QueryString["ProjectId"]);
            string FileName = context.Request.QueryString["FileName"];
            byte[] attachmentData = null;

            AttachmentService.AttachmentService svc = new AttachmentService.AttachmentService();
            attachmentData = svc.GetProjectAttachmentData(projectId);

            context.Response.ContentType = "Application/pdf";
            context.Response.AddHeader(
                "content-disposition", string.Format("attachment; filename={0}", FileName));

            int len = attachmentData.Length;
            int bytes;
            byte[] buffer = new byte[1024];

            Stream outStream = context.Response.OutputStream;
            using (MemoryStream stream = new MemoryStream(attachmentData))
            {
                while (len > 0 && (bytes = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outStream.Write(buffer, 0, bytes);
                    context.Response.Flush();
                    len -= bytes;
                }
            }
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
