using System;
using System.Web.UI.WebControls;
using PraticeManagement.Configuration;
using DataTransferObjects;
using AjaxControlToolkit;

namespace PraticeManagement.Controls.Clients
{
    public partial class ClientProjects : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnProjectName_Command(object sender, CommandEventArgs e)
        {
            ((PracticeManagementPageBase)Page).Redirect(
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

        protected void gvProjects_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var proj = e.Row.DataItem as Project;

                if (proj.ProjectManagers.Count <= 1)
                {
                    var control = e.Row.FindControl("cpe") as CollapsiblePanelExtender;
                    var control2 = e.Row.FindControl("btnExpandCollapseFilter") as Image;
                    control.Collapsed = false;
                    control2.Style["display"] = "none";
                }
            }

        }
    }
}

