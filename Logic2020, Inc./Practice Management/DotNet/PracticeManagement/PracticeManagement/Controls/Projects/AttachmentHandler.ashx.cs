using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            int attachmentId = Convert.ToInt32(context.Request.QueryString["AttachmentId"]);
            string FileName = context.Request.QueryString["FileName"];
            byte[] attachmentData = null;

            AttachmentService.AttachmentService svc = Utils.WCFClientUtility.GetAttachmentService();

            attachmentData = svc.GetProjectAttachmentData(projectId, attachmentId);
                     
            context.Response.ContentType = "application/octet-stream";            

            FileName = FileName.Replace(' ', '_');
            FileName = String.Concat(Encoding.UTF8.GetBytes(FileName).Select(b =>
             {
                 if ((b >= 48 && b <= 57) || (b >= 65 && b <= 90) || (b >= 97 && b <= 122))
                 {
                     return new String((char)b, 1);
                 }
                 else
                 {
                     return String.Format("%{0:x2}", b);
                 }
             }).ToArray());

            context.Response.AddHeader(
                "content-disposition", string.Format("attachment; filename*=UTF-8''{0}", FileName));

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

