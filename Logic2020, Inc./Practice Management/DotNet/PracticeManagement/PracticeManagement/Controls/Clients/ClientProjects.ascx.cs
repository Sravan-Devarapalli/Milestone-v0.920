using System;
using System.Web.UI.WebControls;

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
    }
}

