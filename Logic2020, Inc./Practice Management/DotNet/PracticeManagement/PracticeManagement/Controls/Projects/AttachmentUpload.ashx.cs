using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace PraticeManagement.Controls.Projects
{
    /// <summary>
    /// Summary description for AttachmentUpload
    /// </summary>
    public class AttachmentUpload : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                HttpPostedFile postedFile = context.Request.Files["Filedata"];
                int projectId = 0;
                int.TryParse(context.Request.QueryString["ProjectId"], out projectId);
                int category = 0;
                int.TryParse(context.Request.QueryString["categoryId"], out category);

                byte[] fileData = new byte[postedFile.InputStream.Length];
                postedFile.InputStream.Read(fileData, 0, fileData.Length);

                PraticeManagement.AttachmentService.ProjectAttachment attachment = new PraticeManagement.AttachmentService.ProjectAttachment();
                attachment.AttachmentFileName = postedFile.FileName;
                attachment.AttachmentData = fileData;
                attachment.AttachmentSize = fileData.Length;

                attachment.Category = (PraticeManagement.AttachmentService.ProjectAttachmentCategory)category;
                if (projectId != 0)
                {
                    string loggedInUserAlias = string.IsNullOrEmpty(context.User.Identity.Name) ? context.Request.QueryString["LoggedInUser"] : context.User.Identity.Name;
                    PraticeManagement.AttachmentService.AttachmentService svc = PraticeManagement.Utils.WCFClientUtility.GetAttachmentService();
                    svc.SaveProjectAttachment(attachment, projectId, loggedInUserAlias);
                    context.Response.Write("Uploaded");
                }
                else
                {

                    context.Response.Write(" ");
                }
            }
            catch (Exception ex)
            {
                context.Response.Write(" ");
                throw ex;
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

