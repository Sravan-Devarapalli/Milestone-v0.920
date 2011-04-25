using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;

namespace DataAccess
{
	/// <summary>
	/// Provides an access to the OpportunityTransitionStatus database table.
	/// </summary>
	public static class OpportunityTransitionStatusDAL
	{
		#region Constants

		private const string OpportunityTransitionStatusListAllProcedure = "dbo.OpportunityTransitionStatusListAll";

		private const string OpportunityTransitionStatusIdColumn = "OpportunityTransitionStatusId";
		private const string NameColumn = "Name";

		#endregion

		#region Constants

		/// <summary>
		/// Retrieves a list of the Opportunity Transition Statuses objects.
		/// </summary>
		/// <returns>A list of the <see cref="OpportunityTransitionStatus"/> objects.</returns>
		public static List<OpportunityTransitionStatus> OpportunityTransitionStatusListAll()
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command =
				new SqlCommand(OpportunityTransitionStatusListAllProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;

				connection.Open();
				using (SqlDataReader reader = command.ExecuteReader())
				{
					List<OpportunityTransitionStatus> result = new List<OpportunityTransitionStatus>();
					ReadOpportunityTransitionStatuses(reader, result);
					return result;
				}
			}
		}

		private static void ReadOpportunityTransitionStatuses(DbDataReader reader, List<OpportunityTransitionStatus> result)
		{
			if (reader.HasRows)
			{
				int opportunityTransitionStatusIdIndex =
					reader.GetOrdinal(OpportunityTransitionStatusIdColumn);
				int nameIndex = reader.GetOrdinal(NameColumn);

				while (reader.Read())
				{
					result.Add(
						new OpportunityTransitionStatus()
						{
							Id = reader.GetInt32(opportunityTransitionStatusIdIndex),
							Name = reader.GetString(nameIndex)
						});
				}
			}
		}

		#endregion
	}
}

