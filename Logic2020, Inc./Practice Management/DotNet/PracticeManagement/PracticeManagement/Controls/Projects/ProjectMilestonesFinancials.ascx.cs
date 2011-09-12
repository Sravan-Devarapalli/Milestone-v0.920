﻿using System;
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

        #region Constants

        public const string MILESTONE_TARGET = "milestone";
        protected const string ProjectIdFormat = "projectId={0}";

        private const int GROSS_MARGIN_CELL_INDEX = 4;
        private const int MARGIN_PERCENT_CELL_INDEX = 5;

        private const string ViewSortExpression = "SortExpression";
        private const string ViewSortDirection = "SortDirection";

        private const string CssArrowClass = "arrow";
        
        private const string WordBreak = "<wbr />";

        #endregion

        private SeniorityAnalyzer milestonesSeniorityAnalyzer;

        #region Properties

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
        
        private string PreviousSortExpression
        {
            get
            {
                return (string)ViewState[ViewSortExpression];
            }
            set
            {
                ViewState[ViewSortExpression] = value;
            }
        }

        private SortDirection PreviousSortDirection
        {
            get
            {
                var value = ViewState[ViewSortDirection];
                return value == null ? SortDirection.Ascending : (SortDirection)value;
            }
            set
            {
                ViewState[ViewSortDirection] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            milestonesSeniorityAnalyzer = new SeniorityAnalyzer(DataHelper.CurrentPerson);

            if (!IsPostBack)
            {
                gvRevenueMilestones.Sort("StartDate", SortDirection.Ascending);
                PreviousSortExpression = "StartDate";
            }
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
                if (cellIndex == GROSS_MARGIN_CELL_INDEX)
                {
                    label.CssClass = Convert.ToBoolean(label.Attributes["NegativeValue"]) ? Resources.Controls.BenchCssClass : Resources.Controls.MarginCssClass;
                }
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

            if (e.Row.RowType == DataControlRowType.Header)
            {
                GridViewRow row = e.Row;

                if (row.HasControls())
                {
                    foreach (TableCell cell in row.Controls)
                    {
                        if (cell.HasControls())
                        {
                            foreach (var ctrl in cell.Controls)
                            {
                                if (ctrl is LinkButton)
                                {
                                    var lb = (LinkButton)ctrl;

                                    lb.CssClass = CssArrowClass;

                                    if (lb.CommandArgument == PreviousSortExpression)
                                    {
                                        lb.CssClass += string.Format(" sort-{0}", PreviousSortDirection == SortDirection.Ascending ? "up" : "down");
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void gvRevenueMilestones_Sorting(object sender, GridViewSortEventArgs e)
        {

            string newExpression = e.SortExpression;
            if (newExpression == PreviousSortExpression)
            {
                PreviousSortDirection = PreviousSortDirection == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;
            }
            else
            {
                PreviousSortExpression = newExpression;
                PreviousSortDirection = SortDirection.Ascending;
            }
        }

        public string GetWrappedTest(string text)
        {
            if (text.Length > 30)
            {
                text = text.Insert(30, WordBreak);
            }
            return text;
        }
    }
}

