using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using System.Drawing;
using PraticeManagement.Utils;
using System.Web.UI.HtmlControls;

namespace PraticeManagement.Controls.Persons
{
    public partial class PersonProjects : UserControl, IPostBackEventHandler
    {
        #region Constants

        protected const string MILESTONE_TARGET = "milestone";
        protected const string PROJECT_TARGET = "project";

        #endregion

        #region Fields

        private PracticeManagementCurrency projectMarginTotal;
        private PracticeManagementCurrency projectRevenueTotal;

        #endregion

        #region Properties

        public int? PersonId { get; set; }
        public bool UserIsAdministrator { get; set; }

        #endregion

        #region Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PersonId.HasValue && !IsPostBack)
                InitTotals();
            if (!IsPostBack)
            {
                PopulateData();
            }
        }

        protected void repProjects_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var projectStatus = (ProjectStatusType)DataBinder.Eval(e.Item.DataItem, "ProjectStatusId");
                var tr = e.Item.FindControl("trItem") as HtmlTableRow;
                // See issue #1360 (In Person.projects, please dim (disable) rows where the project is Inactive)
                //  If it's not a real project, skip it, otherwise calculate totals
                if (projectStatus == ProjectStatusType.Experimental
                        || projectStatus == ProjectStatusType.Inactive)
                {
                    tr.Style[HtmlTextWriterStyle.FontStyle] = "italic";
                }
                else
                    if (projectStatus == ProjectStatusType.Completed)
                    {
                        tr.Style["background-color"] = "rgb(255, 242, 229)";
                    }
                    else
                    {
                        projectRevenueTotal += (decimal)DataBinder.Eval(e.Item.DataItem, "Revenue");
                        projectMarginTotal += (decimal)DataBinder.Eval(e.Item.DataItem, "GrossMargin");

                        tr.Style["background-color"] = "rgb(242, 255, 229)";
                    }
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                projectMarginTotal.FormatStyle = NumberFormatStyle.Margin;
                projectRevenueTotal.FormatStyle = NumberFormatStyle.Revenue;

                var lblTotalProjectsMargin = e.Item.FindControl("lblTotalProjectsMargin") as Label;
                var lblTotalProjectsRevenue = e.Item.FindControl("lblTotalProjectsRevenue") as Label;
                var lblOverallMargin = e.Item.FindControl("lblOverallMargin") as Label;
                lblTotalProjectsMargin.Text = projectMarginTotal.ToString();
                lblTotalProjectsRevenue.Text = projectRevenueTotal.ToString();

                if (projectRevenueTotal.Value > 0)
                {
                    lblOverallMargin.Text = (Math.Round(100.0M * projectMarginTotal.Value / projectRevenueTotal.Value)).ToString("F1") + "%";
                }
            }
        }

        private void InitTotals()
        {
            projectMarginTotal =
                new PracticeManagementCurrency { Value = 0.0M, FormatStyle = NumberFormatStyle.Margin };
            projectRevenueTotal =
                new PracticeManagementCurrency { Value = 0.0M, FormatStyle = NumberFormatStyle.Revenue };
        }

        protected string GetMilestoneRedirectUrl(object milestoneId, object projectId)
        {
            return Urls.GetMilestoneRedirectUrl(milestoneId, Request.Url.AbsoluteUri, Convert.ToInt32(projectId));
        }

        protected string GetProjectRedirectUrl(object projectId)
        {
            return Urls.GetProjectDetailsUrl(projectId, Request.Url.AbsoluteUri);
        }

        public void PopulateData()
        {
            if (!PersonId.HasValue)
                return;
            var data = ServiceCallers.Custom.Person(p => p.GetPersonMilestoneWithFinancials(PersonId.Value));
            if (data.Tables[0].Rows.Count > 0)
            {
                repProjects.DataSource = data;
                repProjects.DataBind();
                divEmptyMessage.Style["display"] = "none";
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
            }
        }

        #endregion

        #region Redirect

        public void RaisePostBackEvent(string eventArgument)
        {
            var args = eventArgument.Split(':');
            var target = args[0];

            if (target == MILESTONE_TARGET)
                SaveAndRedirectToMilestone(args[1], args[2]);
            else
                SaveAndRedirectToProject(args[1]);
        }

        private void SaveAndRedirectToProject(object projectId)
        {
            if (((PersonDetail)Page).ValidateAndSavePersonDetails())
                Response.Redirect(GetProjectRedirectUrl(projectId));
        }

        private void SaveAndRedirectToMilestone(object milestoneId, object projectId)
        {
            if (((PersonDetail)Page).ValidateAndSavePersonDetails())
                Response.Redirect(GetMilestoneRedirectUrl(milestoneId, projectId));
        }

        #endregion

        protected string GetProjectNameCellToolTip(int projectStatusId, int hasAttachments, string statusName)
        {
            string cssClass = ProjectHelper.GetIndicatorClassByStatusId(projectStatusId);
            if (projectStatusId == 3 && hasAttachments == 0)
            {
                cssClass = "ActiveProjectWithoutSOW";
            }

            if (projectStatusId == (int)ProjectStatusType.Active)
            {
                if (cssClass == "ActiveProjectWithoutSOW")
                {
                    statusName = "Active without Attachment";
                }
                else
                {
                    statusName = "Active with Attachment";
                }
            }

            return statusName;
        }

        protected string GetProjectNameCellCssClass(int projectStatusId, int hasAttachments)
        {
            string cssClass = ProjectHelper.GetIndicatorClassByStatusId(projectStatusId);
            if (projectStatusId == 3 && hasAttachments == 0)
            {
                cssClass = "ActiveProjectWithoutSOW";
            }

            return cssClass;
        }

    }
}

