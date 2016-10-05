using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using System.Linq;
using System.Collections.Generic;

namespace PraticeManagement.Controls.ProjectExpenses
{
    public partial class ProjectExpensesControl : UserControl
    {
        #region Constants

        private const string Tbeditname = "tbEditName";
        private const string Tbeditamount = "tbEditAmount";
        private const string Tbeditreimbursement = "tbEditReimbursement";

        private const string LblTotalamount = "lblTotalAmount";
        private const string LblTotalExpAmount = "lblTotalExpAmount";
        private const string LblTotalreimbursement = "lblTotalReimbursed";
        private const string LblTotalreimbursementamount = "lblTotalReimbursementAmount";
        private const string tbStartDate = "txtStartDate";
        private const string tbEndDate = "txtEndDate";
        private const string lockdownMessage = "Expenses tab was locked down by System Administrator for the dates on and before '{0}'.";

        #endregion

        public PraticeManagement.ProjectDetail HostingPage
        {
            get { return ((PraticeManagement.ProjectDetail)Page); }
        }

        #region Fields

        private decimal _totalAmount;
        private decimal _totalExpAmount;
        private decimal _totalReimbursed;
        private decimal _totalReimbursementAmount;
        private int _expensesCount;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void gvProjectExpenses_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            var row = e.Row;
            switch (row.RowType)
            {
                case DataControlRowType.DataRow:
                    var expense = row.DataItem as ProjectExpense;
                   
                    if (expense != null)
                    {
                        _totalAmount += expense.Amount;
                        _totalReimbursed += expense.Reimbursement;
                        _totalReimbursementAmount += expense.ReimbursementAmount;
                        _totalExpAmount += expense.ExpectedAmount;
                        _expensesCount++;

                        // Hide rows with null values.
                        // These are special rows that are used not to show
                        //      empty data grid message
                        if (!expense.Id.HasValue)
                            row.Visible = false;
                    }

                    break;

                case DataControlRowType.Footer:
                    SetRowValue(row, LblTotalamount, _totalAmount);
                    SetRowValue(row, LblTotalExpAmount, _totalExpAmount);
                    SetRowValue(row, LblTotalreimbursement, string.Format("{0:0}%", (_totalReimbursed / _expensesCount)));
                    SetRowValue(row, LblTotalreimbursementamount, _totalReimbursementAmount);

                    break;
            }
        }

        private static void SetRowValue(Control row, string ctrlName, string text)
        {
            var totalAmountCtrl = row.FindControl(ctrlName) as Label;
            if (totalAmountCtrl != null)
                totalAmountCtrl.Text = text;
        }

        private static void SetRowValue(Control row, string ctrlName, decimal number)
        {
            SetRowValue(row, ctrlName, ((PracticeManagementCurrency)number).ToString());
        }

        protected void gvProjectExpenses_OnRowUpdating(object sender, GridViewUpdateEventArgs e)
        {

            Page.Validate("ProjectExpensesEdit");
            if (!Page.IsValid)
            {
                e.Cancel = true;
            }
            else
            {
                string newStartDate = e.NewValues["StartDate"].ToString().Trim();
                string newEndDate = e.NewValues["EndDate"].ToString().Trim();
                string oldStartDate = e.OldValues["StartDate"].ToString().Trim();
                string oldEndDate = e.OldValues["EndDate"].ToString().Trim();
                string oldAmount = e.OldValues["Amount"].ToString().Trim();
                string newAmount = e.NewValues["Amount"].ToString().Trim();
                string oldReimbursement = e.OldValues["Reimbursement"].ToString().Trim();
                string newReimbursement = e.NewValues["Reimbursement"].ToString().Trim();
                string oldName = e.OldValues["Name"].ToString().Trim();
                string newName = e.NewValues["Name"].ToString().Trim();


                if (newStartDate != oldStartDate || newEndDate != oldEndDate)
                {


                    var startDate = Convert.ToDateTime(newStartDate);
                    var endDate = Convert.ToDateTime(newEndDate);
                    if (startDate > endDate)
                    {
                        CustomValidator cstEndShouldBeGreater = gvProjectExpenses.Rows[e.RowIndex].FindControl("cstEndShouldBeGreater") as CustomValidator;
                        cstEndShouldBeGreater.IsValid = false;
                        HostingPage.IsOtherPanelDisplay = true;
                        e.Cancel = true;
                    }
                    else
                    {
                        if (HostingPage.Project.StartDate.HasValue && HostingPage.Project.EndDate.HasValue)
                        {
                            var isValid = true;

                            if ((startDate < HostingPage.Project.StartDate.Value.Date ||
                                startDate > HostingPage.Project.EndDate.Value.Date))
                            {
                                CustomValidator cstStartDateShouldbewithinProjectPeriod = gvProjectExpenses.Rows[e.RowIndex].FindControl("cstStartDateShouldbewithinProjectPeriod") as CustomValidator;
                                cstStartDateShouldbewithinProjectPeriod.IsValid = isValid = false;
                                HostingPage.IsOtherPanelDisplay = true;
                            }
                            if ((endDate < HostingPage.Project.StartDate.Value.Date ||
                               endDate > HostingPage.Project.EndDate.Value.Date) && isValid)
                            {
                                CustomValidator cstEndDateShouldbewithinProjectPeriod = gvProjectExpenses.Rows[e.RowIndex].FindControl("cstEndDateShouldbewithinProjectPeriod") as CustomValidator;
                                cstEndDateShouldbewithinProjectPeriod.IsValid = isValid = false;
                                HostingPage.IsOtherPanelDisplay = true;
                            }
                            if (!isValid)
                            {
                                e.Cancel = true;
                                return;
                            }
                        }
                    }
                }
                if (newStartDate != oldStartDate || newEndDate != oldEndDate || oldName != newName || oldAmount != newAmount || oldReimbursement != newReimbursement)
                {
                    var isValid = true;
                    var startDate = Convert.ToDateTime(newStartDate);
                    var endDate = Convert.ToDateTime(newEndDate);
                    if (!isValid)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
        }

        private bool IsPeriodOverlaps(string startDateStr, string endDateStr, object expenseIdObj)
        {
            var startDate = Convert.ToDateTime(startDateStr);
            var endDate = Convert.ToDateTime(endDateStr);
            int? expenseId = null;
            if (expenseIdObj != null)
            {
                expenseId = Convert.ToInt32(expenseIdObj);
            }
            var projectId = Convert.ToInt32(Request.Params[Constants.QueryStringParameterNames.Id]);
            var expenses = ProjectExpenses.ProjectExpenseHelper.ProjectExpensesForProject(projectId);
            foreach (var expense in expenses)
            {
                if (expenseId.HasValue && expense.Id == expenseId.Value)
                {
                    continue;
                }
                if (startDate >= expense.StartDate && startDate <= expense.EndDate
                    || endDate >= expense.StartDate && endDate <= expense.EndDate
                    )
                {
                    return true;
                }
            }

            return false;
        }

        protected void odsProjectExpenses_OnSelecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (HostingPage.ProjectId.HasValue)
            {
                e.InputParameters["projectId"] = HostingPage.ProjectId.Value;
            }
        }

        public void BindExpenses()
        {
            gvProjectExpenses.DataBind();
        }
    }
}

