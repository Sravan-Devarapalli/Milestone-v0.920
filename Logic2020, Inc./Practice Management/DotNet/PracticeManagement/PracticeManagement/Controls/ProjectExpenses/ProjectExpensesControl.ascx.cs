using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;

namespace PraticeManagement.Controls.ProjectExpenses
{
    public partial class ProjectExpensesControl : UserControl
    {
        #region Constants

        private const string Tbeditname = "tbEditName";
        private const string Tbeditamount = "tbEditAmount";
        private const string Tbeditreimbursement = "tbEditReimbursement";

        private const string LblTotalamount = "lblTotalAmount";
        private const string LblTotalreimbursement = "lblTotalReimbursed";
        private const string LblTotalreimbursementamount = "lblTotalReimbursementAmount";

        #endregion

        #region Fields

        private decimal _totalAmount;
        private decimal _totalReimbursed;
        private decimal _totalReimbursementAmount;
        private int _expensesCount;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void lnkAdd_OnClick(object sender, EventArgs e)
        {
            var footerRow = gvProjectExpenses.FooterRow;
            var ctrlName = footerRow.FindControl(Tbeditname) as TextBox;
            var ctrlAmount = footerRow.FindControl(Tbeditamount) as TextBox;
            var ctrlReimb = footerRow.FindControl(Tbeditreimbursement) as TextBox;

            if (ctrlName != null && ctrlAmount != null && ctrlReimb != null)
                ProjectExpenseHelper.AddProjectExpense(
                        ctrlName.Text,
                        ctrlAmount.Text,
                        ctrlReimb.Text,
                        Request.Params[Constants.QueryStringParameterNames.Id]
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

                    if (expense != null)
                    {
                        _totalAmount += expense.Amount;
                        _totalReimbursed += expense.Reimbursement;
                        _totalReimbursementAmount += expense.ReimbursementAmount;

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
                    SetRowValue(row, LblTotalreimbursement, _totalReimbursed/_expensesCount + "%");
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
            SetRowValue(row, ctrlName, ((PracticeManagementCurrency) number).ToString());
        }
    }
}
