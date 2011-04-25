using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;

namespace DataAccess
{
	/// <summary>
	/// Provides an access to the PersonStatus table.
	/// </summary>
	public static class PersonStatusDAL
	{
		#region Constants

		private const string PersonStatusListAllProcedure = "dbo.PersonStatusListAll";

		private const string PersonStatusIdColumn = "PersonStatusId";
		private const string NameColumn = "Name";

		#endregion

		#region Methods

		/// <summary>
		/// Retrives the list of the person statuses.
		/// </summary>
		/// <returns>The list of the <see cref="PersonStatus"/> objects.</returns>
		public static List<PersonStatus> PersonStatusListAll()
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(PersonStatusListAllProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;

				connection.Open();
				using (SqlDataReader reader = command.ExecuteReader())
				{
					List<PersonStatus> result = new List<PersonStatus>();

					ReadPersonStatuses(reader, result);

					return result;
				}
			}
		}

		private static void ReadPersonStatuses(DbDataReader reader, List<PersonStatus> result)
		{
			if (reader.HasRows)
			{
				int personStatusIdIndex = reader.GetOrdinal(PersonStatusIdColumn);
				int nameIndex = reader.GetOrdinal(NameColumn);

				while (reader.Read())
				{
					PersonStatus status = new PersonStatus();

					status.Id = reader.GetInt32(personStatusIdIndex);
					status.Name = reader.GetString(nameIndex);

					result.Add(status);
				}
			}
		}

		#endregion
	}
}

