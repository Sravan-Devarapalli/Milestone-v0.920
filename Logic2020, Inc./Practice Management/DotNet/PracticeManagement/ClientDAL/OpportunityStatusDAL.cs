using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;

namespace DataAccess
{
	/// <summary>
	/// Provides an access to the OpportunityStatus database table.
	/// </summary>
	public static class OpportunityStatusDAL
	{
		#region Constants

		private const string OpportunityStatusListAllProcedure = "dbo.OpportunityStatusListAll";

		private const string OpportunityStatusIdColumn = "OpportunityStatusId";
		private const string NameColumn = "Name";

		#endregion

		#region Methods

		/// <summary>
		/// Retrieves a list of the Opportunity Statuses.
		/// </summary>
		/// <returns>A list of the <see cref="OpportunityStatus"/> objects.</returns>
		public static List<OpportunityStatus> OpportunityStatusListAll()
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(OpportunityStatusListAllProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;

				connection.Open();
				using (SqlDataReader reader = command.ExecuteReader())
				{
					List<OpportunityStatus> result = new List<OpportunityStatus>();
					ReadOpportunityStatuses(reader, result);
					return result;
				}
			}
		}

		private static void ReadOpportunityStatuses(DbDataReader reader, List<OpportunityStatus> result)
		{
			if (reader.HasRows)
			{
				int opportunityStatusIdIndex = reader.GetOrdinal(OpportunityStatusIdColumn);
				int nameIndex = reader.GetOrdinal(NameColumn);

				while (reader.Read())
				{
					result.Add(
						new OpportunityStatus()
						{
							Id = reader.GetInt32(opportunityStatusIdIndex),
							Name = reader.GetString(nameIndex)
						});
				}
			}
		}

		#endregion
	}
}

