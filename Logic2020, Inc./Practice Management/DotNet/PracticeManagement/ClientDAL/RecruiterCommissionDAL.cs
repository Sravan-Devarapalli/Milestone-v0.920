using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;

namespace DataAccess
{
	/// <summary>
	/// Access recruiter commission data in database
	/// </summary>
	public static class RecruiterCommissionDAL
	{
		#region Constants

		#region Stored Procedures

		private const string RecruiterCommissionListByRecruitIdProcedure = "dbo.RecruiterCommissionListByRecruitId";
		private const string RecruiterCommissionSaveProcedure = "dbo.RecruiterCommissionSave";
		private const string RecruiterCommissionDeleteProcedure = "dbo.RecruiterCommissionDelete";

		#endregion

		#region Parameters

		private const string RecruiterIdParam = "@RecruiterId";
		private const string RecruitIdParam = "@RecruitId";
		private const string HoursToCollectParam = "@HoursToCollect";
		private const string AmountParam = "@Amount";
		private const string OldHoursToCollectParam = "@OLD_HoursToCollect";

		#endregion

		#region Columns

		private const string RecruiterIdColumn = "RecruiterId";
		private const string RecruitIdColumn = "RecruitId";
		private const string HoursToCollectColumn = "HoursToCollect";
		private const string AmountColumn = "Amount";

		#endregion

		#endregion

		#region Methods

		/// <summary>
		/// Retrieves the <see cref="RecruiterCommission"/> from the database for the specified recruit.
		/// </summary>
		/// <param name="recruitId">An ID of the <see cref="Person"/> to retrieve the data for.</param>
		/// <returns>The list of the <see cref="RecruiterCommission"/> objects.</returns>
		public static List<RecruiterCommission> DefaultRecruiterCommissionListByRecruitId(int recruitId)
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command =
				new SqlCommand(RecruiterCommissionListByRecruitIdProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = connection.ConnectionTimeout;
				
				command.Parameters.AddWithValue(RecruitIdParam, recruitId);

				connection.Open();
				using (SqlDataReader reader = command.ExecuteReader())
				{
					List<RecruiterCommission> result = new List<RecruiterCommission>();

					ReadRecruiterCommissions(reader, result);

					return result;
				}
			}
		}

		/// <summary>
		/// Saves the <see cref="RecruiterCommission"/> data to the database.
		/// </summary>
		/// <param name="commission">The <see cref="RecruiterCommission"/> data to be saved.</param>
        public static void SaveRecruiterCommissionDetail(RecruiterCommission commission, SqlConnection connection = null, SqlTransaction activeTransaction = null)
		{
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }

			using (SqlCommand command = new SqlCommand(RecruiterCommissionSaveProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = connection.ConnectionTimeout;
				
				command.Parameters.AddWithValue(RecruiterIdParam, commission.RecruiterId);
				command.Parameters.AddWithValue(RecruitIdParam,
					commission.Recruit != null && commission.Recruit.Id.HasValue ?
					(object)commission.Recruit.Id.Value : DBNull.Value);
				command.Parameters.AddWithValue(HoursToCollectParam, commission.HoursToCollect);
				command.Parameters.AddWithValue(AmountParam, commission.Amount.Value);
				command.Parameters.AddWithValue(OldHoursToCollectParam,
					commission.Old_HoursToCollect.HasValue ?
					(object)commission.Old_HoursToCollect.Value : DBNull.Value);

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                if (activeTransaction != null)
                {
                    command.Transaction = activeTransaction;
                }
				command.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Deletes a specified <see cref="RecruiterCommission"/> from the database.
		/// </summary>
		/// <param name="commission">The <see cref="RecruiterCommission"/> object to be deleted.</param>
        public static void DeleteRecruiterCommissionDetail(RecruiterCommission commission, SqlConnection connection = null, SqlTransaction activeTransaction = null)
		{
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }

			using (SqlCommand command = new SqlCommand(RecruiterCommissionDeleteProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = connection.ConnectionTimeout;
				
				command.Parameters.AddWithValue(RecruitIdParam,
					commission.Recruit != null && commission.Recruit.Id.HasValue ?
					(object)commission.Recruit.Id.Value : DBNull.Value);
				command.Parameters.AddWithValue(RecruiterIdParam, commission.RecruiterId);
				command.Parameters.AddWithValue(OldHoursToCollectParam,
					commission.Old_HoursToCollect.HasValue ?
					(object)commission.Old_HoursToCollect.Value : DBNull.Value);

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                if (activeTransaction != null)
                {
                    command.Transaction = activeTransaction;
                }

				command.ExecuteNonQuery();
			}
		}

		private static void ReadRecruiterCommissions(SqlDataReader reader, List<RecruiterCommission> result)
		{
			if (reader.HasRows)
			{
				int recruiterIdIndex = reader.GetOrdinal(RecruiterIdColumn);
				int recruitIdIndex = reader.GetOrdinal(RecruitIdColumn);
				int hoursToCollectIndex = reader.GetOrdinal(HoursToCollectColumn);
				int amountIndex = reader.GetOrdinal(AmountColumn);

				while (reader.Read())
				{
					RecruiterCommission commission = new RecruiterCommission();

					commission.RecruiterId = reader.GetInt32(recruiterIdIndex);
					commission.Recruit = new Person();
					commission.Recruit.Id = reader.GetInt32(recruitIdIndex);
					commission.Old_HoursToCollect = commission.HoursToCollect =
						reader.GetInt32(hoursToCollectIndex);
					commission.Amount = reader.GetDecimal(amountIndex);

					result.Add(commission);
				}
			}
		}

		#endregion
	}
}

