using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using DataAccess;
using DataTransferObjects;

namespace PracticeManagementService
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class ExpenseService : IExpenseService
	{
		/// <summary>
		/// Retrieves the list of the Expense Bases.
		/// </summary>
		/// <returns>The list of the <see cref="ExpenseBasis"/> objects.</returns>
		public List<ExpenseBasis> GetExpenseBasisList()
		{
			return ExpenseBasisDAL.ExpenseBasisListAll();
		}

		/// <summary>
		/// Retrieves the list of the Week Paid Options
		/// </summary>
		/// <returns>The list of the <see cref="WeekPaidOption"/> objects.</returns>
		public List<WeekPaidOption> GetWeekPaidOptionList()
		{
			return WeekPaidOptionDAL.WeekPaidOptionListAll();
		}

		/// <summary>
		/// Retrieves the month expense by the specified Name.
		/// </summary>
		/// <param name="itemName">The name of the expense item which the data be retrieved.</param>
		/// <returns>The <see cref="MonthlyExpense"/> object.</returns>
		public MonthlyExpense GetExpenseDetail(string itemName)
		{
			return MonthlyExpenseDAL.MonthlyExpenseGetByName(itemName);
		}

		/// <summary>
		/// Saves the data of the Expense Item to the database.
		/// </summary>
		/// <param name="itemExpense">The <see cref="MonthlyExpense"/> object the data be saved from.</param>
		public void SaveExpenseItemDetail(MonthlyExpense itemExpense)
		{
			MonthlyExpenseDAL.MonthlyExpenseSave(itemExpense);
		}
	
		/// <summary>
		/// Deletes a month expense item by the specified name
		/// </summary>
		/// <param name="itemExpense">The <see cref="MonthlyExpense"/> object the data be deleted from.</param>
		public void DeleteMonthlyExpense(MonthlyExpense itemExpense)
		{
			MonthlyExpenseDAL.MonthlyExpenseDelete(itemExpense);
		}

		/// <summary>
		/// Retrieves the list of the monthly expenses for the specified period.
		/// </summary>
		/// <param name="startDate">The start date of the period.</param>
		/// <param name="endDate">The end date of the period.</param>
		/// <returns>The list of the <see cref="MonthlyExpense"/> objects.</returns>
		public List<MonthlyExpense> MonthlyExpenseListAll(DateTime startDate, DateTime endDate)
		{
			return MonthlyExpenseDAL.MonthlyExpenseListAll(startDate, endDate);
		}
	}
}

