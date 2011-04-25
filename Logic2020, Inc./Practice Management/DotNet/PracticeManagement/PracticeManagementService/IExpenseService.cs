using System;
using System.Collections.Generic;
using System.ServiceModel;

using DataTransferObjects;

namespace PracticeManagementService
{
	// NOTE: If you change the interface name "IExpenseService" here, you must also update the reference to "IExpenseService" in Web.config.
	[ServiceContract]
	public interface IExpenseService
	{
		/// <summary>
		/// Retrieves the list of the Expense Bases.
		/// </summary>
		/// <returns>The list of the <see cref="ExpenseBasis"/> objects.</returns>
		[OperationContract]
		List<ExpenseBasis> GetExpenseBasisList();

		/// <summary>
		/// Retrieves the list of the Week Paid Options
		/// </summary>
		/// <returns>The list of the <see cref="WeekPaidOption"/> objects.</returns>
		[OperationContract]
		List<WeekPaidOption> GetWeekPaidOptionList();

		/// <summary>
		/// Retrieves the month expense by the specified Name.
		/// </summary>
		/// <param name="itemName">The name of the expense item which the data be retrieved.</param>
		/// <returns>The <see cref="MonthlyExpense"/> object.</returns>
		[OperationContract]
		MonthlyExpense GetExpenseDetail(string itemName);
		
		/// <summary>
		/// Saves the data of the Expense Item to the database.
		/// </summary>
		/// <param name="itemExpense">The <see cref="MonthlyExpense"/> object the data be saved from.</param>
		[OperationContract]
		void SaveExpenseItemDetail(MonthlyExpense itemExpense);

		/// <summary>
		/// Deletes a month expense item by the specified name
		/// </summary>
		/// <param name="itemExpense">The <see cref="MonthlyExpense"/> object the data be deleted from.</param>
		[OperationContract]
		void DeleteMonthlyExpense(MonthlyExpense itemExpense);

		/// <summary>
		/// Retrieves the list of the monthly expenses for the specified period.
		/// </summary>
		/// <param name="startDate">The start date of the period.</param>
		/// <param name="endDate">The end date of the period.</param>
		/// <returns>The list of the <see cref="MonthlyExpense"/> objects.</returns>
		[OperationContract]
		List<MonthlyExpense> MonthlyExpenseListAll(DateTime startDate, DateTime endDate);
	}
}

