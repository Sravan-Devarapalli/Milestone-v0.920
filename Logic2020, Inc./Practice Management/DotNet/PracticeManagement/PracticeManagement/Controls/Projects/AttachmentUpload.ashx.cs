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
                HttpPostedFile postedFile = context.Request.Files["Filedata"];
                int projectId = Convert.ToInt32(context.Request.QueryString["ProjectId"]);
                int category = Convert.ToInt32(context.Request.QueryString["categoryId"]);

                byte[] fileData = new byte[postedFile.InputStream.Length];
                postedFile.InputStream.Read(fileData, 0, fileData.Length);

                PraticeManagement.AttachmentService.ProjectAttachment attachment = new PraticeManagement.AttachmentService.ProjectAttachment();
                attachment.AttachmentFileName = postedFile.FileName;
                attachment.AttachmentData = fileData;
                attachment.AttachmentSize = fileData.Length;                

                attachment.Category = (PraticeManagement.AttachmentService.ProjectAttachmentCategory)category;
                if (projectId != 0)
                {
                    PraticeManagement.AttachmentService.AttachmentService svc = PraticeManagement.Utils.WCFClientUtility.GetAttachmentService();
                    svc.SaveProjectAttachment(attachment, projectId, context.User.Identity.Name);
                    context.Response.Write("Uploaded");
                }
                else
                {
                    attachment.UploadedDate = DateTime.Now;
                    string file = AttachmentUpload.ToJSON(attachment);

                    context.Response.Write(file);
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string ToJSON(Object obj)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            return serializer.Serialize(obj);

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
