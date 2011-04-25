using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;

namespace DataAccess
{
	/// <summary>
	/// Provides an access to the WeekPaidOption database table
	/// </summary>
	public static class WeekPaidOptionDAL
	{
		private const string WeekPaidOptionListAllProcedure = "dbo.WeekPaidOptionListAll";

		private const string WeekPaidOptionIdColumn = "WeekPaidOptionId";
		private const string NameColumn = "Name";

		/// <summary>
		/// Retrieves the list of the Week Paid Options
		/// </summary>
		/// <returns>The list of the <see cref="WeekPaidOption"/> objects.</returns>
		public static List<WeekPaidOption> WeekPaidOptionListAll()
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(WeekPaidOptionListAllProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;

				connection.Open();

				using (SqlDataReader reader = command.ExecuteReader())
				{
					List<WeekPaidOption> result = new List<WeekPaidOption>();

					ReadWeekPaidOptions(reader, result);

					return result;
				}
			}
		}

		private static void ReadWeekPaidOptions(SqlDataReader reader, List<WeekPaidOption> result)
		{
			if (reader.HasRows)
			{
				int weekPaidOptionIdIndex = reader.GetOrdinal(WeekPaidOptionIdColumn);
				int nameIndex = reader.GetOrdinal(NameColumn);

				while (reader.Read())
				{
					result.Add(
						new WeekPaidOption() { Id = reader.GetInt32(weekPaidOptionIdIndex), Name = reader.GetString(nameIndex) });
				}
			}
		}
	}
}

