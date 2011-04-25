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
	/// Provides an access to the billed time stored in the database.
	/// </summary>
	public static class BilledTimeDAL
	{
		#region Constants

		private const string ProjectedWorkloadGetByMilestoneProcedure = "dbo.ProjectedWorkloadGetByMilestone";
		private const string ProjectedWorkloadGetByProjectProcedure = "dbo.ProjectedWorkloadGetByProject";

		private const string MilestoneIdParam = "@MilestoneId";
		private const string PersonIdParam = "@PersonId";
		private const string StartDateParam = "@StartDate";
		private const string EndDateParam = "@EndDate";
		private const string ProjectIdParam = "@ProjectId";

		#region Columns

		private const string MilestoneIdColumn = "MilestoneId";
		private const string PersonIdColumn = "PersonId";
		private const string HoursColumn = "Hours";
		private const string BilledDateColumn = "BilledDate";
		private const string EntryStartDateColumn = "EntryStartDate";

		#endregion

		#endregion

		#region Methods

		/// <summary>
		/// Retrives the projected workload for the specified milestone by the persons
		/// </summary>
		/// <param name="milestoneId">An ID of the milestone to the data be retrieved for.</param>
		/// <returns>The list of the <see cref="BilledTime"/> objects.</returns>
		public static List<BilledTime> ProjectedWorkloadGetByMilestone(int milestoneId)
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(ProjectedWorkloadGetByMilestoneProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = connection.ConnectionTimeout;
				
				command.Parameters.AddWithValue(MilestoneIdParam, milestoneId);

				connection.Open();
				using (SqlDataReader reader = command.ExecuteReader())
				{
					List<BilledTime> result = new List<BilledTime>();

					ReadBilledTime(reader, result);

					return result;
				}
			}
		}

		/// <summary>
		/// Retrieves the projected workload for the specified project and the period
		/// </summary>
		/// <param name="projectId">An ID of the project to the data be retrieved for.</param>
		/// <param name="startDate">A period start.</param>
		/// <param name="endDate">A period end.</param>
		/// <returns>The list of the <see cref="BilledTime"/> objects.</returns>
		public static List<BilledTime> ProjectedWorkloadGetByProject(int projectId, DateTime startDate, DateTime endDate)
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(ProjectedWorkloadGetByProjectProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = connection.ConnectionTimeout;
				
				command.Parameters.AddWithValue(ProjectIdParam, projectId);
				command.Parameters.AddWithValue(StartDateParam, startDate);
				command.Parameters.AddWithValue(EndDateParam, endDate);

				connection.Open();
				using (SqlDataReader reader = command.ExecuteReader())
				{
					List<BilledTime> result = new List<BilledTime>();

					ReadBilledTime(reader, result);

					return result;
				}
			}
		}

		private static void ReadBilledTime(DbDataReader reader, List<BilledTime> result)
		{
			if (reader.HasRows)
			{
				int milestoneIdIndex = reader.GetOrdinal(MilestoneIdColumn);
				int personIdIndex = reader.GetOrdinal(PersonIdColumn);
				int hoursIndex = reader.GetOrdinal(HoursColumn);
				int billedDateIndex = reader.GetOrdinal(BilledDateColumn);
				int entryStartDateIndex = reader.GetOrdinal(EntryStartDateColumn);

				while (reader.Read())
				{
					BilledTime billedTime = new BilledTime();

					billedTime.DateBilled = reader.GetDateTime(billedDateIndex);
					billedTime.HoursBilled = reader.GetDecimal(hoursIndex);
					billedTime.EntryStartDate = reader.GetDateTime(entryStartDateIndex);

					billedTime.Biller = new Person();
					billedTime.Biller.Id = reader.GetInt32(personIdIndex);

					billedTime.MilestoneBilled = new Milestone();
					billedTime.MilestoneBilled.Id = reader.GetInt32(milestoneIdIndex);

					result.Add(billedTime);
				}
			}
		}

		#endregion
	}
}

