using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.MilestonePersonService;
using DataTransferObjects;
using System.ServiceModel;
using System.Drawing;

namespace PraticeManagement.Controls.Persons
{
    public partial class PersonProjects : System.Web.UI.UserControl
    {
        #region Constants

        private const string TOTAL_MARGIN_CELL_ID = "lblTotalProjectsMargin";
        private const string TOTAL_REVENUE_CELL_ID = "lblTotalProjectsRevenue";
        private const string OVERALL_MARGIN_CELL_ID = "lblOverallMargin";
        private const string PROJECT_STATUS_CELL_ID = "lblProjectStatus";

        private static Color REAL_PROJECT_COLOR = Color.FromArgb(242, 255, 229);
        private static Color NOT_REAL_PROJECT_COLOR = Color.FromArgb(255, 242, 229);

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
            if (PersonId.HasValue && !Page.IsPostBack)
            {
                InitTotals();

                MilestonePerson[] milestones = GetMilestones();

                gvProjects.DataSource = milestones;
                gvProjects.DataBind();

//                DataBindChart(milestones);
            }
        }

        //private void DataBindChart(MilestonePerson[] milestones)
        //{
        //    for (int i = 0; i < milestones.Length; i++)
        //    {
        //        MilestonePerson mp = milestones[i];
        //        personDetailsChart.Series["Margin"].Points.AddXY(
        //            i + 1,
        //            mp.ComputedFinancials.GrossMargin.Value);
        //        personDetailsChart.Series["Revenue"].Points.AddXY(
        //            i + 1,
        //            mp.ComputedFinancials.Revenue.Value);
        //    }
        //}

        private MilestonePerson[] GetMilestones()
        {
            MilestonePerson[] milestones;
            using (MilestonePersonServiceClient serviceClient = new MilestonePersonServiceClient())
            {
                try
                {
                    milestones = serviceClient.GetMilestonePersonListByPerson(PersonId.Value);

                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
            return milestones;
        }

        private void InitTotals()
        {
            projectMarginTotal = new PracticeManagementCurrency();
            projectMarginTotal.Value = 0.0M;
            projectMarginTotal.FormatStyle = NumberFormatStyle.Margin;

            projectRevenueTotal = new PracticeManagementCurrency();
            projectRevenueTotal.Value = 0.0M;
            projectRevenueTotal.FormatStyle = NumberFormatStyle.Revenue;
        }

        private void SetCellValue(GridViewRowEventArgs e, string lblId, string cellValue)
        {
            ((Label)e.Row.FindControl(lblId)).Text = cellValue;
        }

        protected void gvProjects_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                case DataControlRowType.DataRow:
                    MilestonePerson mp = e.Row.DataItem as MilestonePerson;
                    ProjectStatus status = mp.Milestone.Project.Status;
                    if (status != null)
                    {
                        int rawProjectStatusId = status.Id;
                        ProjectStatusType projectStatus = (ProjectStatusType)rawProjectStatusId;

                        if (mp != null && mp.ComputedFinancials != null)
                        {
                            // See issue #1360 (In Person.projects, please dim (disable) rows where the project is Inactive)
                            //  If it's not a real project, skip it, otherwise calculate totals
                            if (projectStatus == ProjectStatusType.Completed || projectStatus == ProjectStatusType.Experimental)
                            {
                                e.Row.BackColor = NOT_REAL_PROJECT_COLOR;
                            }
                            else
                            {
                                projectRevenueTotal += mp.ComputedFinancials.Revenue;
                                projectMarginTotal += mp.ComputedFinancials.GrossMargin;

                                e.Row.BackColor = REAL_PROJECT_COLOR;
                            }
                        }

                        SetCellValue(e,
                            PROJECT_STATUS_CELL_ID,
                            Enum.GetName(
                                typeof(ProjectStatusType),
                                rawProjectStatusId));

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

        /// <summary>
        /// Redirects the user to the selected project
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnProject_Command(object sender, CommandEventArgs e)
        {
            ((PracticeManagementPageBase)this.Page).Redirect(
                string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                    Constants.ApplicationPages.ProjectDetail,
                    e.CommandArgument));
        }

        /// <summary>
        /// Redirects the user to the selected milestone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnMilestone_Command(object sender, CommandEventArgs e)
        {
            string[] parts = e.CommandArgument.ToString().Split('_');

            if (parts.Length >= 2)
            {
                ((PracticeManagementPageBase) this.Page).Redirect(string.Concat(
                            string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                            Constants.ApplicationPages.MilestoneDetail,
                            parts[0]), "&projectId=", parts[1]));
            }
        }

	    #endregion    
    }
}
