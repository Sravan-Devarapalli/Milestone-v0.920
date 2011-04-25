using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Security;
using PraticeManagement.Utils;

namespace PraticeManagement.Controls.Projects
{
    public partial class ProjectMilestonesFinancials : UserControl
    {
        #region Delegates

        public delegate int SaveDataDelegate();

        public delegate bool ValidateAndSaveDelegate();

        #endregion

        public const string MILESTONE_TARGET = "milestone";
        protected const string ProjectIdFormat = "projectId={0}";

        private const int GROSS_MARGIN_CELL_INDEX = 4;
        private const int MARGIN_PERCENT_CELL_INDEX = 5;

        private SeniorityAnalyzer milestonesSeniorityAnalyzer;

        protected int? ProjectId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(Request.QueryString[Constants.QueryStringParameterNames.Id]);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            milestonesSeniorityAnalyzer = new SeniorityAnalyzer(DataHelper.CurrentPerson);
        }

        protected string GetMilestoneRedirectUrl(object milestoneId)
        {
            return Urls.GetMilestoneRedirectUrl(milestoneId, Request.Url.AbsoluteUri, ProjectId.Value);
        }

        private static void HideCell(GridViewRowEventArgs e, int cellIndex)
        {
            // This code is applicable only if the cell contains label in it.
            var label = e.Row.Cells[cellIndex].Controls[1] as System.Web.UI.WebControls.Label;
            if (label != null)
            {
                label.Text = Resources.Controls.HiddenCellText;
                label.CssClass = string.Empty;
            }
        }

        protected void gvRevenueMilestones_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var milestoneId = Int32.Parse(((System.Data.DataRowView)(e.Row.DataItem)).Row.ItemArray[0].ToString());
                var milestone = new Milestone { Id = milestoneId };
                if (milestonesSeniorityAnalyzer.OneWithGreaterSeniorityExists(
                    DataHelper.GetPersonsInMilestone(milestone)))
                {
                    HideCell(e, GROSS_MARGIN_CELL_INDEX);
                    HideCell(e, MARGIN_PERCENT_CELL_INDEX);
                }
            }
        }
    }
}
