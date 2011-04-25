using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;

namespace DataAccess
{
	/// <summary>
	/// Provides an access to the ProjectStatus table.
	/// </summary>
	public static class ProjectStatusDAL
	{
		#region Constants

		private const string ProjectStatusListAllProcedure = "dbo.ProjectStatusListAll";

		private const string ProjectStatusIdColumn = "ProjectStatusId";
		private const string NameColumn = "Name";

		#endregion

		#region Methods

		/// <summary>
		/// Retrives the list of the project statuses.
		/// </summary>
		/// <returns>The list of the <see cref="ProjectStatus"/> objects.</returns>
		public static List<ProjectStatus> ProjectStatusListAll()
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(ProjectStatusListAllProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;

				connection.Open();
				using (SqlDataReader reader = command.ExecuteReader())
				{
					List<ProjectStatus> result = new List<ProjectStatus>();

					ReadProjectStatuses(reader, result);

					return result;
				}
			}
		}

		private static void ReadProjectStatuses(DbDataReader reader, List<ProjectStatus> result)
		{
			if (reader.HasRows)
			{
				int projectStatusIdIndex = reader.GetOrdinal(ProjectStatusIdColumn);
				int nameIndex = reader.GetOrdinal(NameColumn);

				while (reader.Read())
				{
					ProjectStatus status = new ProjectStatus();

					status.Id = reader.GetInt32(projectStatusIdIndex);
					status.Name = reader.GetString(nameIndex);

					result.Add(status);
				}
			}
		}

		#endregion
	}
}

