using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls.ProjectExpenses;

namespace PraticeManagement.Controls.Milestones
{
    public partial class MilestoneExpenses : System.Web.UI.UserControl
    {
        #region Constants

        private const string Tbeditname = "tbEditName";
        private const string Tbeditamount = "tbEditAmount";
        private const string Tbeditexpamount = "tbEditExpAmount";
        private const string Tbeditreimbursement = "tbEditReimbursement";

        private const string LblTotalamount = "lblTotalAmount";
        private const string LblTotalExpAmount = "lblTotalExpAmount";
        private const string LblTotalreimbursement = "lblTotalReimbursed";
        private const string LblTotalreimbursementamount = "lblTotalReimbursementAmount";
        private const string tbStartDate = "txtStartDate";
        private const string tbEndDate = "txtEndDate";
        private const string lockdownMessage = "Expenses tab was locked down by System Administrator for the dates on and before '{0}'.";

        #endregion

        public PraticeManagement.MilestoneDetail HostingPage
        {
            get { return ((PraticeManagement.MilestoneDetail)Page); }
        }

        public bool IsLockout
        {
            get;
            set;
        }

        public DateTime? LockoutDate
        {
            get;
            set;
        }

        #region Fields

        private decimal _totalAmount;
        private decimal _totalExpectedAmount;
        private decimal _totalReimbursed;
        private decimal _totalReimbursementAmount;
        private int _expensesCount;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            LockdownExpenses();
        }

        protected void lnkAdd_OnClick(object sender, EventArgs e)
        {
            var footerRow = gvProjectExpenses.FooterRow;
            var ctrlName = footerRow.FindControl(Tbeditname) as TextBox;
            var ctrlAmount = footerRow.FindControl(Tbeditamount) as TextBox;
            var ctrlExpAmount = footerRow.FindControl(Tbeditexpamount) as TextBox;
            var ctrlReimb = footerRow.FindControl(Tbeditreimbursement) as TextBox;
            var txtStartDate = footerRow.FindControl(tbStartDate) as TextBox;
            var txtEndDate = footerRow.FindControl(tbEndDate) as TextBox;
            var startDate = DateTime.Parse(txtStartDate.Text);
            var endDate = DateTime.Parse(txtEndDate.Text);
            var ddlExpenseType = footerRow.FindControl("ddlExpense") as DropDownList;

            if (startDate > endDate)
            {
                CustomValidator cstEndShouldBeGreater = footerRow.FindControl("cstEndShouldBeGreater") as CustomValidator;
                cstEndShouldBeGreater.IsValid = false;
                return;
            }

            else if (HostingPage.Project.StartDate.HasValue && HostingPage.Project.EndDate.HasValue)
            {
                var isValid = true;
                if (startDate < HostingPage.Milestone.StartDate.Date ||
                    startDate > HostingPage.Milestone.EndDate.Date)
                {
                    CustomValidator cstStartDateShouldbewithinProjectPeriod = footerRow.FindControl("cstStartDateShouldbewithinProjectPeriod") as CustomValidator;
                    cstStartDateShouldbewithinProjectPeriod.IsValid = isValid = false;
                }
                if ((endDate < HostingPage.Milestone.StartDate.Date ||
                    endDate > HostingPage.Milestone.EndDate.Date) && isValid)
                {
                    CustomValidator cstEndDateShouldbewithinProjectPeriod = footerRow.FindControl("cstEndDateShouldbewithinProjectPeriod") as CustomValidator;
                    cstEndDateShouldbewithinProjectPeriod.IsValid = isValid = false;
                }
                if (isValid && IsLockout)
                {
                    CustomValidator custLockdown = footerRow.FindControl("custLockdown") as CustomValidator;
                    if (!ValidateLockdown(custLockdown, true, true, false, false, false))
                    {
                        custLockdown.IsValid = isValid = false;
                        custLockdown.ErrorMessage = custLockdown.ToolTip = string.Format(lockdownMessage, LockoutDate.Value.ToShortDateString());
                    }
                }
                if (!isValid)
                {
                    return;
                }
            }
            
            if (ctrlName != null && ctrlAmount != null && ctrlReimb != null)
                ProjectExpenseHelper.AddProjectExpense(
                        ctrlName.Text,
                        ctrlAmount.Text,
                        ctrlExpAmount.Text,
                        ctrlReimb.Text,
                        HostingPage.Project.Id.Value.ToString(),
                        startDate,
                        endDate,
                        HostingPage.MilestoneId.Value.ToString(),
                        ddlExpenseType.SelectedValue
                    );
            gvProjectExpenses.DataBind();
        }

        protected void gvProjectExpenses_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            var row = e.Row;
            switch (row.RowType)
            {
                case DataControlRowType.DataRow:
                    var expense = row.DataItem as ProjectExpense;
                    var lnkEdit = row.FindControl("lnkEdit") as LinkButton;
                    var lnkDelete = row.FindControl("lnkDelete") as LinkButton;
                    if (gvProjectExpenses.EditIndex == row.DataItemIndex)
                    {
                        var ddlExpenseType = row.FindControl("ddlExpenseType") as DropDownList;
                        DataHelper.FillExpenseTypeList(ddlExpenseType);
                        ddlExpenseType.SelectedValue = expense.Type.Id.ToString();
                    }

                    if (expense != null)
                    {
                        _totalAmount += expense.Amount;
                        _totalReimbursed += expense.Reimbursement;
                        _totalReimbursementAmount += expense.ReimbursementAmount;
                        _totalExpectedAmount += expense.ExpectedAmount;
                        _expensesCount++;

                        // Hide rows with null values.
                        // These are special rows that are used not to show
                        //      empty data grid message
                        if (!expense.Id.HasValue)
                            row.Visible = false;
                        if (IsLockout)
                        {
                            if (expense.EndDate <= LockoutDate)
                            {
                                lnkEdit.Enabled = lnkDelete.Enabled = false;
                                lnkDelete.OnClientClick = null;
                            }
                            if (expense.StartDate <= LockoutDate)
                            {
                                lnkDelete.Enabled = false;
                                lnkDelete.OnClientClick = null;
                            }
                        }
                    }

                    break;

                case DataControlRowType.Footer:
                    SetRowValue(row, LblTotalamount, _totalAmount);
                    SetRowValue(row,LblTotalExpAmount,_totalExpectedAmount);
                    SetRowValue(row, LblTotalreimbursement, string.Format("{0:0}%", (_totalReimbursed / _expensesCount)));
                    SetRowValue(row, LblTotalreimbursementamount, _totalReimbursementAmount);
                    var ddlExpense = row.FindControl("ddlExpense") as DropDownList;
                    DataHelper.FillExpenseTypeList(ddlExpense);
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

                var ddlExpenseType = gvProjectExpenses.Rows[e.RowIndex].FindControl("ddlExpenseType") as DropDownList;
                if (ddlExpenseType != null)
                {
                    var type = new ExpenseType()
                    {
                        Id = Convert.ToInt32(ddlExpenseType.SelectedValue),
                        Name = ddlExpenseType.SelectedItem.Text
                    };
                    e.NewValues["Type"] = type;
                }

                if (newStartDate != oldStartDate || newEndDate != oldEndDate)
                {


                    var startDate = Convert.ToDateTime(newStartDate);
                    var endDate = Convert.ToDateTime(newEndDate);
                    if (startDate > endDate)
                    {
                        CustomValidator cstEndShouldBeGreater = gvProjectExpenses.Rows[e.RowIndex].FindControl("cstEndShouldBeGreater") as CustomValidator;
                        cstEndShouldBeGreater.IsValid = false;
                        e.Cancel = true;
                    }
                    else
                    {
                        if (HostingPage.Project.StartDate.HasValue && HostingPage.Project.EndDate.HasValue)
                        {
                            var isValid = true;

                            if ((startDate < HostingPage.Milestone.StartDate.Date ||
                                startDate > HostingPage.Milestone.EndDate.Date))
                            {
                                CustomValidator cstStartDateShouldbewithinProjectPeriod = gvProjectExpenses.Rows[e.RowIndex].FindControl("cstStartDateShouldbewithinProjectPeriod") as CustomValidator;
                                cstStartDateShouldbewithinProjectPeriod.IsValid = isValid = false;
                            }
                            if ((endDate < HostingPage.Milestone.StartDate.Date ||
                               endDate > HostingPage.Milestone.EndDate.Date) && isValid)
                            {
                                CustomValidator cstEndDateShouldbewithinProjectPeriod = gvProjectExpenses.Rows[e.RowIndex].FindControl("cstEndDateShouldbewithinProjectPeriod") as CustomValidator;
                                cstEndDateShouldbewithinProjectPeriod.IsValid = isValid = false;
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
                    if (isValid && IsLockout)
                    {
                        CustomValidator custLockdown = gvProjectExpenses.Rows[e.RowIndex].FindControl("custLockdown") as CustomValidator;
                        if (!ValidateLockdown(custLockdown, newStartDate != oldStartDate, newEndDate != oldEndDate, oldName != newName, oldAmount != newAmount, oldReimbursement != newReimbursement))
                        {
                            custLockdown.IsValid = isValid = false;
                            custLockdown.ErrorMessage = custLockdown.ToolTip = string.Format(lockdownMessage, LockoutDate.Value.ToShortDateString());
                        }
                    }
                    if (!isValid)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
        }

        public bool ValidateLockdown(object sender, bool isStartDateChanged, bool isEndDateChanged, bool isNameChanged, bool isExpenseChanged, bool isReimberseChanged)
        {
            bool isValid = true;
            var source = sender as CustomValidator;
            var gridRow = source.NamingContainer as GridViewRow;
            var txtEndDate = gridRow.FindControl("txtEndDate") as TextBox;
            var txtStartDate = gridRow.FindControl("txtStartDate") as TextBox;
            if (!string.IsNullOrEmpty(txtEndDate.Text) && !string.IsNullOrEmpty(txtStartDate.Text))
            {
                var endDate = DateTime.Parse(txtEndDate.Text);
                var startDate = DateTime.Parse(txtStartDate.Text);
                if ((isEndDateChanged && endDate.Date <= LockoutDate.Value.Date) || (isStartDateChanged && startDate.Date <= LockoutDate.Value.Date) || (isNameChanged && startDate.Date <= LockoutDate.Value.Date) || (isExpenseChanged && startDate.Date <= LockoutDate.Value.Date) || (isReimberseChanged && startDate.Date <= LockoutDate.Value.Date))
                {
                    isValid = false;
                }
            }
            return isValid;
        }

        protected void odsProjectExpenses_OnSelecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (HostingPage.MilestoneId.HasValue)
            {
                e.InputParameters["milestoneId"] = HostingPage.MilestoneId.Value;
            }
        }

        public void BindExpenses()
        {
            gvProjectExpenses.DataBind();
        }

        public void LockdownExpenses()
        {
            DataTransferObjects.Lockout expenseItem = new DataTransferObjects.Lockout();
            using (var service = new ConfigurationService.ConfigurationServiceClient())
            {
                List<DataTransferObjects.Lockout> projectdetailItems = service.GetLockoutDetails((int)LockoutPages.Projectdetail).ToList();
                expenseItem = projectdetailItems.FirstOrDefault(p => p.Name == "Expenses");
                IsLockout = expenseItem.IsLockout;
                LockoutDate = expenseItem.LockoutDate;
            }
        }
    }
}

