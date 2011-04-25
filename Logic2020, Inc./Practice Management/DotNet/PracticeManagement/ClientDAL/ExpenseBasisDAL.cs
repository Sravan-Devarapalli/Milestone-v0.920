using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;

namespace DataAccess
{
	/// <summary>
	/// Provides an access to the ExpenseBasis database table.
	/// </summary>
	public static class ExpenseBasisDAL
	{
		private const string ExpenseBasisListAllProcedure = "dbo.ExpenseBasisListAll";

		private const string ExpenseBasisIdColumn = "ExpenseBasisId";
		private const string NameColumn = "Name";

		/// <summary>
		/// Retrieves the list of the Expense Bases.
		/// </summary>
		/// <returns>The list of the <see cref="ExpenseBasis"/> objects.</returns>
		public static List<ExpenseBasis> ExpenseBasisListAll()
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(ExpenseBasisListAllProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;

				connection.Open();

				using (SqlDataReader reader = command.ExecuteReader())
				{
					List<ExpenseBasis> result = new List<ExpenseBasis>();

					ReadExpenseBases(reader, result);

					return result;
				}
			}
		}

		private static void ReadExpenseBases(DbDataReader reader, List<ExpenseBasis> result)
		{
			if (reader.HasRows)
			{
				int expenseBasisIdIndex = reader.GetOrdinal(ExpenseBasisIdColumn);
				int nameIndex = reader.GetOrdinal(NameColumn);

				while (reader.Read())
				{
					result.Add(
						new ExpenseBasis() { Id = reader.GetInt32(expenseBasisIdIndex), Name = reader.GetString(nameIndex) });
				}
			}
		}
	}
}

