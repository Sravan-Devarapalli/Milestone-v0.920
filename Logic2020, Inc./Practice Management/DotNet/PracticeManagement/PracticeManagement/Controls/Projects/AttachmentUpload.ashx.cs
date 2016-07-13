using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using DataTransferObjects;
using PraticeManagement.VendorService;

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
                bool isProjectUpload = context.Request.RawUrl.Contains("ProjectId");
                int projectId = 0;
                int category = 0;
                int vendorId = 0;
                string loggedInUserAlias = context.Request.QueryString["LoggedInUser"];
                byte[] fileData = new byte[postedFile.InputStream.Length];
                postedFile.InputStream.Read(fileData, 0, fileData.Length);

                if (isProjectUpload)
                {
                    int.TryParse(context.Request.QueryString["ProjectId"], out projectId);
                    int.TryParse(context.Request.QueryString["categoryId"], out category);
                    PraticeManagement.AttachmentService.ProjectAttachment attachment = new PraticeManagement.AttachmentService.ProjectAttachment();
                    attachment.AttachmentFileName = postedFile.FileName;
                    attachment.AttachmentData = fileData;
                    attachment.AttachmentSize = fileData.Length;

                    attachment.Category = (PraticeManagement.AttachmentService.ProjectAttachmentCategory)category;
                    if (projectId != 0)
                    {
                        PraticeManagement.AttachmentService.AttachmentService svc = PraticeManagement.Utils.WCFClientUtility.GetAttachmentService();
                        svc.SaveProjectAttachment(attachment, projectId, loggedInUserAlias);
                        context.Response.Write("Uploaded");
                    }
                    else
                    {
                        context.Response.Write(" ");
                    }
                }
                else
                {
                    int.TryParse(context.Request.QueryString["VendorId"], out vendorId);

                    VendorAttachment attachment = new VendorAttachment();
                    attachment.AttachmentFileName = postedFile.FileName;
                    attachment.AttachmentData = fileData;
                    attachment.AttachmentSize = fileData.Length;
                    if (vendorId != 0)
                    {
                        using (var serviceClient = new VendorServiceClient())
                        {
                            serviceClient.SaveVendorAttachmentData(attachment, vendorId, loggedInUserAlias);
                            context.Response.Write("Uploaded");
                        }
                    }
                    else
                    {
                        context.Response.Write(" ");
                    }
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

