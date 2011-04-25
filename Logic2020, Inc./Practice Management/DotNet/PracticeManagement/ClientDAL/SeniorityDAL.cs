using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;

namespace DataAccess
{
	public static class SeniorityDAL
	{
		#region Constants

		private const string SeniorityListAllProcedure = "dbo.SeniorityListAll";

		private const string SeniorityIdColumn = "SeniorityId";
		private const string NameColumn = "Name";

		#endregion

		#region Methods

		/// <summary>
		/// Selects a list of the seniorities.
		/// </summary>
		/// <returns>A list of the <see cref="Seniority"/> objects.</returns>
		public static List<Seniority> ListAll()
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(SeniorityListAllProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;

				connection.Open();

				using (SqlDataReader reader = command.ExecuteReader())
				{
					List<Seniority> result = new List<Seniority>();
					ReadSeniorities(reader, result);
					return result;
				}
			}
		}

		private static void ReadSeniorities(DbDataReader reader, List<Seniority> result)
		{
			if (reader.HasRows)
			{
				int seniorityIdIndex = reader.GetOrdinal(SeniorityIdColumn);
				int nameIndex = reader.GetOrdinal(NameColumn);

				while (reader.Read())
				{
					result.Add(
						new Seniority()
						{
							Id = reader.GetInt32(seniorityIdIndex),
							Name = reader.GetString(nameIndex)
						});
				}
			}
		}

		#endregion
	}
}

