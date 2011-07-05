using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using System.Drawing;
using PraticeManagement.Utils;

namespace PraticeManagement.Controls.Persons
{
    public partial class PersonProjects : UserControl, IPostBackEventHandler
    {
        #region Constants

        private const string TOTAL_MARGIN_CELL_ID = "lblTotalProjectsMargin";
        private const string TOTAL_REVENUE_CELL_ID = "lblTotalProjectsRevenue";
        private const string OVERALL_MARGIN_CELL_ID = "lblOverallMargin";
        protected const string MILESTONE_TARGET = "milestone";
        protected const string PROJECT_TARGET = "project";

        private static readonly Color REAL_PROJECT_COLOR = Color.FromArgb(242, 255, 229);
        private static readonly Color NOT_REAL_PROJECT_COLOR = Color.FromArgb(255, 242, 229);

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
        }

        private void InitTotals()
        {
            projectMarginTotal =
                new PracticeManagementCurrency { Value = 0.0M, FormatStyle = NumberFormatStyle.Margin };
            projectRevenueTotal =
                new PracticeManagementCurrency { Value = 0.0M, FormatStyle = NumberFormatStyle.Revenue };
        }

        private static void SetCellValue(GridViewRowEventArgs e, string lblId, string cellValue)
        {
            ((Label)e.Row.FindControl(lblId)).Text = cellValue;
        }

        protected void gvProjects_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                case DataControlRowType.DataRow:
                    var projectStatus = (ProjectStatusType)DataBinder.Eval(e.Row.DataItem, "ProjectStatusId");

                    // See issue #1360 (In Person.projects, please dim (disable) rows where the project is Inactive)
                    //  If it's not a real project, skip it, otherwise calculate totals
                    if (projectStatus == ProjectStatusType.Experimental
                            || projectStatus == ProjectStatusType.Inactive)
                    {
                        e.Row.Style[HtmlTextWriterStyle.FontStyle] = "italic";
                    }
                    else
                        if (projectStatus == ProjectStatusType.Completed)
                        {
                            e.Row.BackColor = NOT_REAL_PROJECT_COLOR;
                        }
                        else
                        {
                            projectRevenueTotal += (decimal)DataBinder.Eval(e.Row.DataItem, "Revenue");
                            projectMarginTotal += (decimal)DataBinder.Eval(e.Row.DataItem, "GrossMargin");

                            e.Row.BackColor = REAL_PROJECT_COLOR;
                        }
                    break;

                case DataControlRowType.Footer:
                    projectMarginTotal.FormatStyle = NumberFormatStyle.Margin;
                    projectRevenueTotal.FormatStyle = NumberFormatStyle.Revenue;

                    SetCellValue(e, TOTAL_MARGIN_CELL_ID, projectMarginTotal.ToString());
                    SetCellValue(e, TOTAL_REVENUE_CELL_ID, projectRevenueTotal.ToString());

                    if (projectRevenueTotal.Value > 0)
                    {
                        SetCellValue(e,
                            OVERALL_MARGIN_CELL_ID,
                            (Math.Round(100.0M * projectMarginTotal.Value / projectRevenueTotal.Value)).ToString("F1") + "%");
                    }
                    break;
            }
        }

        protected string GetMilestoneRedirectUrl(object milestoneId, object projectId)
        {
            return Urls.GetMilestoneRedirectUrl(milestoneId, Request.Url.AbsoluteUri, Convert.ToInt32(projectId));
        }

        protected string GetProjectRedirectUrl(object projectId)
        {
            return Urls.GetProjectDetailsUrl(projectId, Request.Url.AbsoluteUri);
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

        protected string GetProjectNameCellToolTip(int projectStatusId, object fileName,string statusName)
        {
            string cssClass = ProjectHelper.GetIndicatorClassByStatusId(projectStatusId);
            string fileNameStr = fileName.ToString();
            if (projectStatusId == 3 && string.IsNullOrEmpty(fileNameStr))
            {
                cssClass = "ActiveProjectWithoutSOW";
            }

            if (projectStatusId == (int)ProjectStatusType.Active)
            {
                if (cssClass == "ActiveProjectWithoutSOW")
                {
                    statusName = "Active without SOW";
                }
                else
                {
                    statusName = "Active with SOW";
                }
            }

            return statusName;
        }

        protected string GetProjectNameCellCssClass(int projectStatusId, object fileName)
        {
            string cssClass = ProjectHelper.GetIndicatorClassByStatusId(projectStatusId);
            string fileNameStr = fileName.ToString();
            if (projectStatusId == 3 && string.IsNullOrEmpty(fileNameStr))
            {
                cssClass = "ActiveProjectWithoutSOW";
            }

            return cssClass;
        }

    }
}

