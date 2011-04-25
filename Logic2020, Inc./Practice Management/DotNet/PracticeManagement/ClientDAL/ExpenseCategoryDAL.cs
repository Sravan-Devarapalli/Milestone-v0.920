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
	/// Provides an access to the <see cref="ExpenseCategory"/> database table.
	/// </summary>
	public static class ExpenseCategoryDAL
	{
		#region Constants

		private const string ExpenseCategoryListAllProcedure = "dbo.ExpenseCategoryListAll";
		private const string ExpenseCategoryInsertProcedure = "dbo.ExpenseCategoryInsert";
		private const string ExpenseCategoryUpdateProcedure = "dbo.ExpenseCategoryUpdate";
		private const string ExpenseCategoryDeleteProcedure = "dbo.ExpenseCategoryDelete";

		private const string ExpenseCategoryIdColumn = "ExpenseCategoryId";
		private const string NameColumn = "Name";

		private const string ExpenseCategoryIdParam = "@ExpenseCategoryId";
		private const string NameParam = "@Name";

		#endregion

		#region Methods

		/// <summary>
		/// Retrieves the list of the categories for the fixed expenses
		/// </summary>
		/// <returns>The list of the <see cref="ExpenseCategory"/> objects.</returns>
		public static List<ExpenseCategory> ExpenseCategoryListAll()
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(ExpenseCategoryListAllProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = connection.ConnectionTimeout;

				connection.Open();

				using (SqlDataReader reader = command.ExecuteReader())
				{
					List<ExpenseCategory> result = new List<ExpenseCategory>();

					ReadCategories(reader, result);

					return result;
				}
			}
		}

		/// <summary>
		/// Inserts an <see cref="ExpenseCategory"/> into the database.
		/// </summary>
		/// <param name="category">The <see cref="ExpenseCategory"/> the data be inserted from.</param>
		public static void ExpenseCategoryInsert(ExpenseCategory category)
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(ExpenseCategoryInsertProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = connection.ConnectionTimeout;
				
				command.Parameters.AddWithValue(NameParam, category.Name);

				connection.Open();
				command.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Updates an <see cref="ExpenseCategory"/> in the database.
		/// </summary>
		/// <param name="category">The <see cref="ExpenseCategory"/> the data be updated from.</param>
		public static void ExpenseCategoryUpdate(ExpenseCategory category)
		{
            if (string.IsNullOrEmpty(category.Name.Trim()))
                throw new ArgumentException("Name");

			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(ExpenseCategoryUpdateProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = connection.ConnectionTimeout;
				
				command.Parameters.AddWithValue(ExpenseCategoryIdParam, category.Id);
				command.Parameters.AddWithValue(NameParam, category.Name);

                connection.Open();
                command.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Deletes an <see cref="ExpenseCategory"/> from the database.
		/// </summary>
		/// <param name="category">The <see cref="ExpenseCategory"/> the data be deleted from.</param>
		public static void ExpenseCategoryDelete(ExpenseCategory category)
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(ExpenseCategoryDeleteProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = connection.ConnectionTimeout;
				
				command.Parameters.AddWithValue(ExpenseCategoryIdParam, category.Id);

				try
				{
					connection.Open();
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
		}

		private static void ReadCategories(DbDataReader reader, List<ExpenseCategory> result)
		{
			if (reader.HasRows)
			{
				int expenseCategoryIdIndex = reader.GetOrdinal(ExpenseCategoryIdColumn);
				int nameIndex = reader.GetOrdinal(NameColumn);

				while (reader.Read())
				{
					result.Add(
						new ExpenseCategory() { Id = reader.GetInt32(expenseCategoryIdIndex), Name = reader.GetString(nameIndex) });
				}
			}
		}

		#endregion
	}
}

