using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;

namespace DataAccess
{
	/// <summary>
	/// Provides an access to the MonthlyExpense database table.
	/// </summary>
	public static class MonthlyExpenseDAL
	{
		#region Constants

		#region Stored Procedures

		private const string MonthlyExpenseListByNameProcedure = "dbo.MonthlyExpenseListByName";
		private const string MonthlyExpenseSaveItemProcedure = "dbo.MonthlyExpenseSaveItem";
		private const string MonthlyExpenseDeleteItemProcedure = "dbo.MonthlyExpenseDeleteItem";
		private const string MonthlyExpenseListAllProcedure = "dbo.MonthlyExpenseListAll";

		#endregion

		#region Parameters

		private const string NameParam = "@Name";
		private const string ExpenseCategoryIdParam = "@ExpenseCategoryId";
		private const string ExpenseBasisIdParam = "@ExpenseBasisId";
		private const string WeekPaidOptionIdParam = "@WeekPaidOptionId";
		private const string YearParam = "@Year";
		private const string MonthParam = "@Month";
		private const string AmountParam = "@Amount";
		private const string StartDateParam = "@StartDate";
		private const string EndDateParam = "@EndDate";
		private const string OldNameParam = "@OLD_Name";

		#endregion

		#region Columns

		private const string ExpenseCategoryIdColumn = "ExpenseCategoryId";
		private const string ExpenseBasisIdColumn = "ExpenseBasisId";
		private const string WeekPaidOptionIdColumn = "WeekPaidOptionId";
		private const string NameColumn = "Name";
		private const string YearColumn = "Year";
		private const string MonthColumn = "Month";
		private const string AmountColumn = "Amount";
		private const string ItemNameColumn = "ItemName";
		private const string ExpenseCategoryNameColumn = "ExpenseCategoryName";
		private const string ExpenseBasisNameColumn = "ExpenseBasisName";
		private const string WeekPaidOptionNameColumn = "WeekPaidOptionName";

		private const int NumberOfFixedColumns = 5;

		#endregion

		#endregion

		#region Methods

		/// <summary>
		/// Retrieves the month expense by the specified Name.
		/// </summary>
		/// <param name="itemName">The name of the expense item which the data be retrieved.</param>
		/// <returns>The <see cref="MonthlyExpense"/> object.</returns>
		public static MonthlyExpense MonthlyExpenseGetByName(string itemName)
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(MonthlyExpenseListByNameProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = connection.ConnectionTimeout;
				
				command.Parameters.AddWithValue(NameParam,
					!string.IsNullOrEmpty(itemName) ? (object)itemName : DBNull.Value);

				connection.Open();

				using (SqlDataReader reader = command.ExecuteReader())
				{
					return ReadMonthExpenses(reader);
				}
			}
		}

		/// <summary>
		/// Saves the data of the Expense Item to the database.
		/// </summary>
		/// <param name="itemExpense">The <see cref="MonthlyExpense"/> object the data be saved from.</param>
		public static void MonthlyExpenseSave(MonthlyExpense itemExpense)
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(MonthlyExpenseSaveItemProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = connection.ConnectionTimeout;
				
				command.Parameters.AddWithValue(NameParam,
					!string.IsNullOrEmpty(itemExpense.Name) ? (object)itemExpense.Name : DBNull.Value);
				command.Parameters.AddWithValue(ExpenseCategoryIdParam,
					itemExpense.ExpenseCategory != null ? (object)itemExpense.ExpenseCategory.Id : DBNull.Value);
				command.Parameters.AddWithValue(ExpenseBasisIdParam,
					itemExpense.ExpenseBasis != null ? (object)itemExpense.ExpenseBasis.Id : DBNull.Value);
				command.Parameters.AddWithValue(WeekPaidOptionIdParam,
					itemExpense.WeekPaidOption != null ? (object)itemExpense.WeekPaidOption.Id : DBNull.Value);
				command.Parameters.AddWithValue(OldNameParam,
					!string.IsNullOrEmpty(itemExpense.OldName) ? (object)itemExpense.OldName : DBNull.Value);

				// Creating the parameters with some dummy values
				command.Parameters.AddWithValue(YearParam, 0);
				command.Parameters.AddWithValue(MonthParam, 0);
				command.Parameters.AddWithValue(AmountParam, 0M);

				try
				{
					connection.Open();

					foreach (KeyValuePair<DateTime, decimal> expense in itemExpense.MonthlyAmount)
					{
						command.Parameters[YearParam].Value = expense.Key.Year;
						command.Parameters[MonthParam].Value = expense.Key.Month;
						command.Parameters[AmountParam].Value = expense.Value;

						command.ExecuteNonQuery();
					}
				}
				catch (SqlException ex)
				{
					throw new DataAccessException(ex);
				}
			}
		}

		/// <summary>
		/// Deletes a month expense item by the specified name
		/// </summary>
		/// <param name="itemExpense">The <see cref="MonthlyExpense"/> object the data be deleted from.</param>
		public static void MonthlyExpenseDelete(MonthlyExpense itemExpense)
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(MonthlyExpenseDeleteItemProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = connection.ConnectionTimeout;
				
				command.Parameters.AddWithValue(NameParam,
					!string.IsNullOrEmpty(itemExpense.Name) ? (object)itemExpense.Name : DBNull.Value);

				try
				{
					connection.Open();

					command.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					throw new DataAccessException(ex);
				}
			}
		}

		/// <summary>
		/// Retrieves the list of the monthly expenses for the specified period.
		/// </summary>
		/// <param name="startDate">The start date of the period.</param>
		/// <param name="endDate">The end date of the period.</param>
		/// <returns>The list of the <see cref="MonthlyExpense"/> objects.</returns>
		public static List<MonthlyExpense> MonthlyExpenseListAll(DateTime startDate, DateTime endDate)
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(MonthlyExpenseListAllProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = connection.ConnectionTimeout;
				
				command.Parameters.AddWithValue(StartDateParam, startDate);
				command.Parameters.AddWithValue(EndDateParam, endDate);

				connection.Open();

				using (SqlDataReader reader = command.ExecuteReader())
				{
					List<MonthlyExpense> result = new List<MonthlyExpense>();

					ReadExpenses(reader, result);

					return result;
				}
			}
		}

		private static MonthlyExpense ReadMonthExpenses(DbDataReader reader)
		{
			MonthlyExpense result = null;

			if (reader.HasRows)
			{
				int expenseCategoryIdIndex = reader.GetOrdinal(ExpenseCategoryIdColumn);
				int expenseBasisIdIndex = reader.GetOrdinal(ExpenseBasisIdColumn);
				int weekPaidOptionIdIndex = reader.GetOrdinal(WeekPaidOptionIdColumn);
				int nameIndex = reader.GetOrdinal(NameColumn);
				int yearIndex = reader.GetOrdinal(YearColumn);
				int monthIndex = reader.GetOrdinal(MonthColumn);
				int amountIndex = reader.GetOrdinal(AmountColumn);

				while (reader.Read())
				{
					if (result == null)
					{
						result = new MonthlyExpense();

						result.Name = reader.GetString(nameIndex);
						result.MonthlyAmount = new Dictionary<DateTime, decimal>();

						result.ExpenseBasis = new ExpenseBasis();
						result.ExpenseBasis.Id = reader.GetInt32(expenseBasisIdIndex);

						result.ExpenseCategory = new ExpenseCategory();
						result.ExpenseCategory.Id = reader.GetInt32(expenseCategoryIdIndex);

						result.WeekPaidOption = new WeekPaidOption();
						result.WeekPaidOption.Id = reader.GetInt32(weekPaidOptionIdIndex);
					}

					DateTime month =
						new DateTime(reader.GetInt32(yearIndex), reader.GetInt32(monthIndex), 1);
					result.MonthlyAmount.Add(month, reader.GetDecimal(amountIndex));

					result.MinMonth = month < result.MinMonth ? month : result.MinMonth;
					result.MaxMonth = month > result.MaxMonth ? month : result.MaxMonth;
				}
			}
			
			return result;
		}

		private static void ReadExpenses(DbDataReader reader, List<MonthlyExpense> result)
		{
			if (reader.HasRows)
			{
				int itemNameIndex = reader.GetOrdinal(ItemNameColumn);
				int expenseCategoryNameIndex = reader.GetOrdinal(ExpenseCategoryNameColumn);
				int expenseBasisNameIndex = reader.GetOrdinal(ExpenseBasisNameColumn);
				int weekPaidOptionNameIndex = reader.GetOrdinal(WeekPaidOptionNameColumn);

				while (reader.Read())
				{
					MonthlyExpense item = new MonthlyExpense();
					item.Name = item.OldName = reader.GetString(itemNameIndex);

					item.ExpenseCategory =
						new ExpenseCategory() { Name = reader.GetString(expenseCategoryNameIndex) };

					item.ExpenseBasis =
						new ExpenseBasis() { Name = reader.GetString(expenseBasisNameIndex) };

					item.WeekPaidOption =
						new WeekPaidOption() { Name = reader.GetString(weekPaidOptionNameIndex) };

					item.MonthlyAmount = new Dictionary<DateTime, decimal>();

					for (int i = NumberOfFixedColumns; i < reader.FieldCount; i++)
					{
						string columnName = reader.GetName(i);
						DateTime month =
							new DateTime(int.Parse(columnName.Substring(0, 4)), int.Parse(columnName.Substring(4)), 1);
						item.MonthlyAmount.Add(month, reader.GetDecimal(i));


						item.MinMonth = month < item.MinMonth ? month : item.MinMonth;
						item.MaxMonth = month > item.MaxMonth ? month : item.MaxMonth;
					}

					result.Add(item);
				}
			}
		}

		#endregion
	}
}

