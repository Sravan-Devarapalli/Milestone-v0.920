using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PraticeManagement.Controls
{
    public class PersonPicture : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int personId = int.Parse(context.Request.QueryString["PersonId"]);
            PraticeManagement.AttachmentService.AttachmentService svc = PraticeManagement.Utils.WCFClientUtility.GetAttachmentService();

            byte[] data = svc.GetPersonPicture(personId);
            context.Response.OutputStream.Write(data, 0, data.Length);
            
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
