using System;
using System.Web.UI.WebControls;
using PraticeManagement.Configuration;

namespace PraticeManagement.Controls.Clients
{
    public partial class ClientProjects : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnProjectName_Command(object sender, CommandEventArgs e)
        {
            ((PracticeManagementPageBase) Page).Redirect(
                string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                   Constants.ApplicationPages.ProjectDetail,
                                   e.CommandArgument));
        }

        public bool CheckIfDefaultProject(object projectIdObj)
        {
            var defaultProjectId = MileStoneConfigurationManager.GetProjectId();
            var projectId = Int32.Parse(projectIdObj.ToString());
            return defaultProjectId.HasValue && defaultProjectId.Value == projectId;
        }
    }
}

