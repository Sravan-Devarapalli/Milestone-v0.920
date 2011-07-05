using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using DataAccess;
using DataTransferObjects;

namespace PracticeManagementService
{
    /// <summary>
    /// Summary description for AttachmentService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class AttachmentService : System.Web.Services.WebService
    {
        [WebMethod]
        public void SaveProjectAttachment(ProjectAttachment sow, int projectId, string userName)
        {
            if (sow != null)
            {
                ProjectDAL.SaveProjectAttachmentData(sow, projectId, userName);
            }
        }

        [WebMethod]
        public byte[] GetProjectAttachmentData(int projectId)
        {
           return ProjectDAL.GetProjectAttachmentData(projectId);
        }

        [WebMethod]
        public void DeleteProjectAttachmentByProjectId(int projectId, string userName)
        {
            ProjectDAL.DeleteProjectAttachmentByProjectId(projectId, userName);
        }

    }
}

