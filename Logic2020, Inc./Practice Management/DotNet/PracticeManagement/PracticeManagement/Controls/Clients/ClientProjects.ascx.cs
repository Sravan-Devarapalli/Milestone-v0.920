using System;
using System.Web.UI.WebControls;
using PraticeManagement.Configuration;
using DataTransferObjects;
using AjaxControlToolkit;

namespace PraticeManagement.Controls.Clients
{
    public partial class ClientProjects : System.Web.UI.UserControl
    {
        public string ClientId
        {
            get { return Request.QueryString["id"]; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string GetProjectLinkURL(object sender)
        {
            int projectId;
            if (sender is Project)
            {
                var project = sender as Project;
                projectId = project.Id.Value;
            }
            else if (sender == null)
            {
                return string.Empty;
            }
            else
            {
                projectId = (int)sender;
            }
            return PraticeManagement.Utils.Generic.GetTargetUrlWithReturn(string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                    Constants.ApplicationPages.ProjectDetail,
                    projectId), Request.Url.AbsoluteUri + (Request.Url.Query.Length > 0 ? string.Empty : Constants.FilterKeys.QueryStringOfApplyFilterFromCookie));
        }

        public bool CheckIfDefaultProject(object projectIdObj)
        {
            var defaultProjectId = MileStoneConfigurationManager.GetProjectId();
            var projectId = Int32.Parse(projectIdObj.ToString());
            return defaultProjectId.HasValue && defaultProjectId.Value == projectId;
        }

        //protected void repProjects_ItemDataBound(object sender, RepeaterItemEventArgs e)
        //{
        //    if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;
        //    var proj = (Project)e.Item.DataItem;
        //    //if (proj.ProjectManagers == null || proj.ProjectManagers.Count <= 1)
        //    //{
        //    //    var cpe = e.Item.FindControl("cpe") as CollapsiblePanelExtender;
        //    //    var btnExpandCollapseFilter = e.Item.FindControl("btnExpandCollapseFilter") as Image;
        //    //    cpe.Collapsed = false;
        //    //    btnExpandCollapseFilter.Style["display"] = "none";
        //    //}
        //}

        public void DisplayProjects()
        {
            var projects = DataHelper.ListProjectsByClient(Convert.ToInt32(ClientId), string.Empty);
            if (projects.Length == 0)
            {
                divEmptyMessage.Style["display"] = "";
                return;
            }
            repProjects.DataSource = projects;
            repProjects.DataBind();
            divEmptyMessage.Style["display"] = "none";
        }
    }
}

