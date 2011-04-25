using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.ExpenseService;

namespace PraticeManagement
{
	public partial class ExpenseDetail : PracticeManagementPageBase
	{
		#region Constants

		private const string ItemNameArgument = "itemName";
		private const string MonthlyAmountKey = "MonthlyAmount";

		#endregion

		#region Properties

		private ExceptionDetail InternalException
		{
			get;
			set;
		}

		private string SelectedItem
		{
			get
			{
				return Request.QueryString[ItemNameArgument];
			}
		}

		protected int? SelectedCategoryId
		{
			get
			{
				return !string.IsNullOrEmpty(ddlExpenseCategory.SelectedValue) ?
					(int?)int.Parse(ddlExpenseCategory.SelectedValue) : null;
			}
		}

		protected int? SelectedBasisId
		{
			get
			{
				return !string.IsNullOrEmpty(ddlBasis.SelectedValue) ?
					(int?)int.Parse(ddlBasis.SelectedValue) : null;
			}
		}

		private Dictionary<DateTime, decimal> MonthlyAmount
		{
			get
			{
				Dictionary<DateTime, decimal> result;
				if (ViewState[MonthlyAmountKey] == null)
				{
					ViewState[MonthlyAmountKey] = new Dictionary<DateTime, decimal>();
				}

				result = (Dictionary<DateTime, decimal>)ViewState[MonthlyAmountKey];

				return result.OrderByDescending(entry => entry.Key).ToDictionary(entry => entry.Key, entry => entry.Value);
			}
			set
			{
				ViewState[MonthlyAmountKey] = value;
			}
		}

		#endregion

		#region Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            mlConfirmation.ClearMessage();
        }

		protected void gvExpenses_RowEditing(object sender, GridViewEditEventArgs e)
		{
			gvExpenses.EditIndex = e.NewEditIndex;
			e.Cancel = true;
			gvExpenses.DataSource = MonthlyAmount;
			gvExpenses.DataBind();
		}

		protected void gvExpenses_RowUpdating(object sender, GridViewUpdateEventArgs e)
		{
			Page.Validate(vsumMonthExpense.ValidationGroup);
			if (Page.IsValid)
			{
				Dictionary<DateTime, decimal> tmp = MonthlyAmount;
				DateTime key =
					Convert.ToDateTime(((HiddenField)gvExpenses.Rows[e.RowIndex].FindControl("hidKey")).Value);

				tmp[key] =
					decimal.Parse(((TextBox)gvExpenses.Rows[e.RowIndex].FindControl("txtAmount")).Text, CultureInfo.InvariantCulture);
				MonthlyAmount = tmp;

				gvExpenses.EditIndex = -1;
				gvExpenses.DataSource = MonthlyAmount;
				gvExpenses.DataBind();
				e.Cancel = true;
			}
		}

		protected void gvExpenses_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			Dictionary<DateTime, decimal> tmp = MonthlyAmount;
			DateTime key =
				Convert.ToDateTime(((HiddenField)gvExpenses.Rows[e.RowIndex].FindControl("hidKey")).Value);

			if (tmp.ContainsKey(key))
			{
				tmp.Remove(key);
			}
			
			MonthlyAmount = tmp;

			gvExpenses.DataSource = MonthlyAmount;
			gvExpenses.DataBind();
			e.Cancel = true;
		}

		protected void gvExpenses_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
		{
			gvExpenses.EditIndex = -1;
			gvExpenses.DataSource = MonthlyAmount;
			gvExpenses.DataBind();
			e.Cancel = true;
		}

		protected void btnAddExpense_Click(object sender, EventArgs e)
		{
			Page.Validate(vsumNewExpense.ValidationGroup);
			if (Page.IsValid)
			{
				Dictionary<DateTime, decimal> tmp = MonthlyAmount;

				tmp.Add(
					new DateTime(mpNewExpense.SelectedYear, mpNewExpense.SelectedMonth, 1),
					decimal.Parse(txtAmount.Text, CultureInfo.InvariantCulture));
				MonthlyAmount = tmp;

				txtAmount.Text = string.Empty;

				gvExpenses.DataSource = MonthlyAmount;
				gvExpenses.DataBind();
			}
		}

		#region Validation

		protected void custNewExpense_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid =
				!MonthlyAmount.ContainsKey(new DateTime(mpNewExpense.SelectedYear, mpNewExpense.SelectedMonth, 1));
		}

		protected void custMonthExpense_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = MonthlyAmount.Count > 0;
		}

		protected void custExpenseCategoryDeleted_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid =
				InternalException == null ||
				InternalException.Message != ErrorCode.ExpenseCategoryDeleted.ToString();
		}

		protected void custExpenseCategory_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid =
				InternalException == null ||
				InternalException.Message == ErrorCode.ExpenseCategoryDeleted.ToString();
			if (!args.IsValid)
			{
				custExpenseCategory.ErrorMessage = custExpenseCategory.ToolTip = InternalException.Message;
			}
		}

		protected void custAmount_ServerValidate(object sender, ServerValidateEventArgs e)
		{
			CustomValidator val = (CustomValidator)sender;
			val.ErrorMessage = val.ToolTip = GetAmountErrorMessage();

			e.IsValid = Regex.IsMatch(e.Value, GetAmountValidationExpr());
		}

		protected string GetAmountErrorMessage()
		{
			return
				SelectedBasisId == (int)ExpenseBasisType.AbsolutAmount ?
				Resources.Messages.AmountTwoDigits : Resources.Messages.AmountThreeDigits;
		}

		protected string GetAmountValidationExpr()
		{
			return
				SelectedBasisId == (int)ExpenseBasisType.AbsolutAmount ?
				"^(\\d{1,12}(\\.\\d{0,2})?|(\\.\\d{1,2}))$" : "^(\\d{1,12}(\\.\\d{0,3})?|(\\.\\d{1,3}))$";
		}

		#endregion

		protected void btnSave_Click(object sender, EventArgs e)
		{
			Page.Validate(vsumExpenseDetails.ValidationGroup);
			if (Page.IsValid && SaveData())
			{
                ClearDirty();
                mlConfirmation.ShowInfoMessage(string.Format(Resources.Messages.SavedDetailsConfirmation, "Expense"));
			}
		}

		protected void btnDelete_Click(object sender, EventArgs e)
		{
			using (ExpenseServiceClient serviceClient = new ExpenseServiceClient())
			{
				try
				{
					serviceClient.DeleteMonthlyExpense(new MonthlyExpense() { Name = SelectedItem });
				}
				catch (CommunicationException)
				{
					serviceClient.Abort();
					throw;
				}
			}

			ReturnToPreviousPage();
		}

		protected override void Display()
		{
			DataHelper.FillExpenseCategoriesList(ddlExpenseCategory, string.Empty);
			DataHelper.FillExpenseBasesList(ddlBasis, string.Empty);
			DataHelper.FillWeekPaidOptionsList(ddlWeekPaid, string.Empty);

			if (!string.IsNullOrEmpty(SelectedItem))
			{
				MonthlyExpense expenseItem = null;

				using (ExpenseServiceClient serviceClient = new ExpenseServiceClient())
				{
					try
					{
						expenseItem = serviceClient.GetExpenseDetail(SelectedItem);
					}
					catch (CommunicationException)
					{
						serviceClient.Abort();
						throw;
					}
				}

				if (expenseItem != null)
				{
					PopulateControls(expenseItem);
				}
			}
		}

		private bool SaveData()
		{
			bool result = false;
			MonthlyExpense expenseItem = new MonthlyExpense();
			PopulateData(expenseItem);

			using (ExpenseServiceClient serviceClient = new ExpenseServiceClient())
			{
				try
				{
					serviceClient.SaveExpenseItemDetail(expenseItem);
					result = true;
				}
				catch (FaultException<ExceptionDetail> ex)
				{
					InternalException = ex.Detail;
					Page.Validate(custExpenseCategory.ValidationGroup);
					serviceClient.Abort();
				}
				catch (CommunicationException)
				{
					serviceClient.Abort();
				}
			}

			return result;
		}

		private void PopulateControls(MonthlyExpense expenseItem)
		{
			txtExpenseName.Text = hidOldExpenseName.Value = expenseItem.Name;
			ddlExpenseCategory.SelectedIndex =
				ddlExpenseCategory.Items.IndexOf(
				ddlExpenseCategory.Items.FindByValue(expenseItem.ExpenseCategory != null ?
				expenseItem.ExpenseCategory.Id.ToString() : string.Empty));
			ddlBasis.SelectedIndex =
				ddlBasis.Items.IndexOf(
				ddlBasis.Items.FindByValue(expenseItem.ExpenseBasis != null ?
				expenseItem.ExpenseBasis.Id.ToString() : string.Empty));
			ddlWeekPaid.SelectedIndex =
				ddlWeekPaid.Items.IndexOf(
				ddlWeekPaid.Items.FindByValue(expenseItem.WeekPaidOption != null ?
				expenseItem.WeekPaidOption.Id.ToString() : string.Empty));

			MonthlyAmount = expenseItem.MonthlyAmount;
			gvExpenses.DataSource = MonthlyAmount;
			gvExpenses.DataBind();
		}

		private void PopulateData(MonthlyExpense expenseItem)
		{
			expenseItem.Name = txtExpenseName.Text;
			expenseItem.OldName = hidOldExpenseName.Value;

			if (!string.IsNullOrEmpty(ddlExpenseCategory.SelectedValue))
			{
				expenseItem.ExpenseCategory =
					new ExpenseCategory() { Id = SelectedCategoryId.Value };
			}
			if (!string.IsNullOrEmpty(ddlBasis.SelectedValue))
			{
				expenseItem.ExpenseBasis =
					new ExpenseBasis() { Id = SelectedBasisId.Value };
			}
			if (!string.IsNullOrEmpty(ddlWeekPaid.SelectedValue))
			{
				expenseItem.WeekPaidOption =
					new WeekPaidOption() { Id = int.Parse(ddlWeekPaid.SelectedValue) };
			}

			expenseItem.MonthlyAmount = MonthlyAmount;
		}

		#endregion
	}
}

