using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;

namespace DataAccess
{
	/// <summary>
	/// Provides an access to the PersonRole database table.
	/// </summary>
	public static class PersonRoleDAL
	{
		#region Constants

		private const string PersonRoleListAllProcedure = "dbo.PersonRoleListAll";

		private const string PersonRoleIdColumn = "PersonRoleId";
		private const string NameColumn = "Name";

		#endregion

		#region Methods

		/// <summary>
		/// Retrives a list of the avalable role for the person.
		/// </summary>
		/// <returns>The list of <see cref="PersonRole"/> objects.</returns>
		public static List<PersonRole> PersonRoleListAll()
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(PersonRoleListAllProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;

				connection.Open();

				using (SqlDataReader reader = command.ExecuteReader())
				{
					List<PersonRole> result = new List<PersonRole>();

					ReadPersonRoles(reader, result);

					return result;
				}
			}
		}

		private static void ReadPersonRoles(SqlDataReader reader, List<PersonRole> result)
		{
			if (reader.HasRows)
			{
				int personRoleIdIndex = reader.GetOrdinal(PersonRoleIdColumn);
				int nameIndex = reader.GetOrdinal(NameColumn);

				while (reader.Read())
				{
					PersonRole role = new PersonRole();

					role.Id = reader.GetInt32(personRoleIdIndex);
					role.Name = reader.GetString(nameIndex);

					result.Add(role);
				}
			}
		}

		#endregion
	}
}

