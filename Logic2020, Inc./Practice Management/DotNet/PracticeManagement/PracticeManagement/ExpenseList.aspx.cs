using System;
using System.ServiceModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.ExpenseService;

namespace PraticeManagement
{
	public partial class ExpenseList : PracticeManagementPageBase
	{
		#region Constants

		private const int CategoryColumnIndex = 0;
		private const int ItemColumnIdex = 1;
		private const int BasisColumnIndex = 2;
		private const int PaidOptionColumnIndex = 3;
		private const int NumberOfFixedColumns = 4;

		private const string DetailsRedirect = "{0}?itemName={1}";

		private const string ExpensesKey = "Expenses";

		private const int MaxPeriodLength = 24;

		#endregion

		#region Properties

		private MonthlyExpense[] ExpensesList
		{
			get
			{
				return (MonthlyExpense[])ViewState[ExpensesKey];
			}
			set
			{
				ViewState[ExpensesKey] = value;
			}
		}

		#endregion

		#region Methods

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				DateTime thisMonth = DateTime.Today;
				thisMonth = new DateTime(thisMonth.Year, thisMonth.Month, Constants.Dates.FirstDay);

				// Set the default viewable interval.
				mpPeriodStart.SelectedYear = thisMonth.Year;
				mpPeriodStart.SelectedMonth = thisMonth.Month;

				DateTime periodEnd = thisMonth.AddMonths(Constants.Dates.DefaultViewableMonths);
				mpPeriodEnd.SelectedYear = periodEnd.Year;
				mpPeriodEnd.SelectedMonth = periodEnd.Month;
			}
			else
			{
				PopulateControls(ExpensesList);
			}

			custPeriodLengthLimit.ErrorMessage = custPeriodLengthLimit.ToolTip =
				string.Format(custPeriodLengthLimit.ErrorMessage, MaxPeriodLength);
		}

		protected void btnAddExpense_Click(object sender, EventArgs e)
		{
			Redirect(Constants.ApplicationPages.ExpenseDetail);
		}

		protected void btnExpenseCategories_Click(object sender, EventArgs e)
		{
			Redirect(Constants.ApplicationPages.ExpenseCategoryList);
		}

		protected void btnUpdate_Click(object sender, EventArgs e)
		{
			Page.Validate();
			if (Page.IsValid)
			{
				Display();
			}
		}

		protected void custPeriod_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = GetPeriodLength() > 0;
		}

		protected void custPeriodLengthLimit_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = GetPeriodLength() <= MaxPeriodLength;
		}

		protected void gvExpenses_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				MonthlyExpense item = (MonthlyExpense)e.Row.DataItem;

				// Category Name
				e.Row.Cells[CategoryColumnIndex].Controls.Add(
					new Label()
					{
						Text = item.ExpenseCategory != null ? HttpUtility.HtmlEncode(item.ExpenseCategory.Name) : string.Empty
					});

				// Item Name
				LinkButton btnItem = new LinkButton();
				btnItem.Text = HttpUtility.HtmlEncode(item.Name);
				btnItem.CommandArgument = item.Name;
				btnItem.CausesValidation = false;
				btnItem.Command += new CommandEventHandler(btnItem_Command);
				e.Row.Cells[ItemColumnIdex].Controls.Add(btnItem);
					
				// Basis Name
				e.Row.Cells[BasisColumnIndex].Controls.Add(
					new Label() { Text = item.ExpenseBasis != null ? item.ExpenseBasis.Name : string.Empty });

				// Week Paid Option name
				e.Row.Cells[PaidOptionColumnIndex].Controls.Add(
					new Label() { Text = item.WeekPaidOption != null ? item.WeekPaidOption.Name : string.Empty });

				// Expenses by months
				DateTime dtTmp = item.MinMonth;
				for (int i = NumberOfFixedColumns; i < gvExpenses.Columns.Count; i++, dtTmp = dtTmp.AddMonths(1))
				{
					e.Row.Cells[i].Controls.Add(
						new Label()
						{
							Text = item.MonthlyAmount.ContainsKey(dtTmp) ?
								((PracticeManagementCurrency)item.MonthlyAmount[dtTmp]).ToString() : string.Empty
						});
				}
			}
		}

		private void btnItem_Command(object sender, CommandEventArgs e)
		{
			Redirect(string.Format(DetailsRedirect,
				Constants.ApplicationPages.ExpenseDetail,
				HttpUtility.UrlEncode((string)e.CommandArgument)));
		}

		private void PopulateControls(MonthlyExpense[] expenses)
		{
			if (expenses.Length > 0)
			{
				// Clear the unnecessary columns
				while (gvExpenses.Columns.Count > NumberOfFixedColumns)
				{
					gvExpenses.Columns.RemoveAt(NumberOfFixedColumns);
				}

				// Create new columns
				for (DateTime dtTmp = expenses[0].MinMonth; dtTmp <= expenses[0].MaxMonth; dtTmp = dtTmp.AddMonths(1))
				{
					BoundField column = new BoundField();
                    column.HeaderText = dtTmp.ToString(Constants.Formatting.CompPerfMonthYearFormat);
					gvExpenses.Columns.Add(column);
				}
			}

			gvExpenses.DataSource = expenses;
			gvExpenses.DataBind();
		}

		protected override void Display()
		{
			DateTime monthBegin =
				new DateTime(mpPeriodStart.SelectedYear,
					mpPeriodStart.SelectedMonth,
					Constants.Dates.FirstDay);
			DateTime monthEnd =
				new DateTime(mpPeriodEnd.SelectedYear, mpPeriodEnd.SelectedMonth,
					DateTime.DaysInMonth(mpPeriodEnd.SelectedYear, mpPeriodEnd.SelectedMonth));

			using (ExpenseServiceClient serviceClient = new ExpenseServiceClient())
			{
				try
				{
					MonthlyExpense[] expenses =
						serviceClient.MonthlyExpenseListAll(monthBegin, monthEnd);
					ExpensesList = expenses;
					PopulateControls(expenses);
				}
				catch (CommunicationException)
				{
					serviceClient.Abort();
					throw;
				}
			}
		}

		/// <summary>
		/// Calculates a length of the selected period in the mounths.
		/// </summary>
		/// <returns>The number of the months within the selected period.</returns>
		private int GetPeriodLength()
		{
			int mounthsInPeriod =
				(mpPeriodEnd.SelectedYear - mpPeriodStart.SelectedYear) * Constants.Dates.LastMonth +
				(mpPeriodEnd.SelectedMonth - mpPeriodStart.SelectedMonth + 1);
			return mounthsInPeriod;
		}

		#endregion
	}
}

