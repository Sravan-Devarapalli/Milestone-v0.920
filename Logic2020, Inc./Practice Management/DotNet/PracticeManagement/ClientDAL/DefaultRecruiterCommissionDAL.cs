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
	/// Access default recruiter commission data in database
	/// </summary>
	public static class DefaultRecruiterCommissionDAL
	{
		#region Constants

		#region Columns

		private const string DefaultRecruiterCommissionHeaderIdColumn = "DefaultRecruiterCommissionHeaderId";
		private const string PersonIdColumn = "PersonId";
		private const string StartDateColumn = "StartDate";
		private const string EndDateColumn = "EndDate";
		private const string TextLineColumn = "TextLine";
		private const string HoursToCollectColumn = "HoursToCollect";
		private const string AmountColumn = "Amount";

		#endregion

		#region Parameters

		private const string PersonIdParam = "@PersonId";
		private const string StartDateParam = "@StartDate";
		private const string EndDateParam = "@EndDate";
		private const string DefaultRecruiterCommissionHeaderIdParam = "@DefaultRecruiterCommissionHeaderId";
		private const string HoursToCollectParam = "@HoursToCollect";
		private const string AmountParam = "@Amount";
		private const string DateParam = "@Date";

		#endregion

		#region Stored Procedures

		private const string DefaultRecruiterCommissionHeaderInsertProcedure =
			"dbo.DefaultRecruiterCommissionHeaderInsert";
		private const string DefaultRecruiterCommissionHeaderUpdateProcedure =
			"dbo.DefaultRecruiterCommissionHeaderUpdate";
		private const string DefaultRecruiterCommissionItemInsertProcedure =
			"dbo.DefaultRecruiterCommissionItemInsert";
		private const string DefaultRecruiterCommissionItemUpdateProcedure =
			"dbo.DefaultRecruiterCommissionItemUpdate";
        private const string DefaultRecruiterCommissionCleanupProcedure =
            "dbo.DefaultRecruiterCommissionCleanup";
        private const string DefaultRecruiterCommissionDeleteItemsProcedure =
			"dbo.DefaultRecruiterCommissionDeleteItems";
		private const string DefaultRecruiterCommissionListByPersonProcedure =
			"dbo.DefaultRecruiterCommissionListByPerson";
		private const string DefaultRecruiterCommissionGetByPersonDateProcedure =
			"dbo.DefaultRecruiterCommissionGetByPersonDate";
		private const string DefaultRecruiterCommissionGetByIdProcedure =
			"dbo.DefaultRecruiterCommissionGetById";
		private const string DefaultRecruiterCommissionItemListProcedure =
			"dbo.DefaultRecruiterCommissionItemList";

		#endregion


		#endregion

		/// <summary>
		/// Retrieves the default recruiter commissions for the specified person.
		/// </summary>
		/// <param name="personId">An ID of the person to retrieve the data for.</param>
		/// <returns>The list of the <see cref="DefaultRecruiterCommission"/> objects.</returns>
		public static List<DefaultRecruiterCommission> DefaultRecruiterCommissionListByPerson(int personId)
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(DefaultRecruiterCommissionListByPersonProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = connection.ConnectionTimeout;
				
				command.Parameters.AddWithValue(PersonIdParam, personId);

				connection.Open();
				using (SqlDataReader reader = command.ExecuteReader())
				{
					List<DefaultRecruiterCommission> result = new List<DefaultRecruiterCommission>();

					ReadCommissions(reader, result);

					return result;
				}
			}
		}

		/// <summary>
		/// Retrieves the actual default recruiter commissions for the specified person and date.
		/// </summary>
		/// <param name="personId">An ID of the person to retrieve the data for.</param>
		/// <param name="date">A date to retrieve the data for.</param>
		/// <returns>The <see cref="DefaultRecruiterCommission"/> object if found and null otherwise.</returns>
		public static DefaultRecruiterCommission DefaultRecruiterCommissionGetByPersonDate(int personId,
			DateTime? date)
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command =
				new SqlCommand(DefaultRecruiterCommissionGetByPersonDateProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = connection.ConnectionTimeout;
				
				command.Parameters.AddWithValue(PersonIdParam, personId);
				command.Parameters.AddWithValue(DateParam,
					date.HasValue ? (object)date.Value : DBNull.Value);

				connection.Open();
				using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
				{
					List<DefaultRecruiterCommission> result = new List<DefaultRecruiterCommission>();

					ReadCommissions(reader, result);

					return result.Count > 0 ? result[0] : null;
				}
			}
		}

		/// <summary>
		/// Retrieves the default recruiter commissions with the specified ID.
		/// </summary>
		/// <param name="commissionHeaderId">An ID of the rtecord to be retrived.</param>
		/// <returns>The <see cref="DefaultRecruiterCommission"/> object if found and null otherwise.</returns>
		public static DefaultRecruiterCommission DefaultRecruiterCommissionGetById(int commissionHeaderId)
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(DefaultRecruiterCommissionGetByIdProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = connection.ConnectionTimeout;
				
				command.Parameters.AddWithValue(DefaultRecruiterCommissionHeaderIdParam, commissionHeaderId);

				connection.Open();
				using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
				{
					List<DefaultRecruiterCommission> result = new List<DefaultRecruiterCommission>();

					ReadCommissions(reader, result);

					return result.Count > 0 ? result[0] : null;
				}
			}
		}

		/// <summary>
		/// Inserts the default recruiter commission to the database.
		/// </summary>
		/// <param name="commission">The commission data to be saved.</param>
		public static void DefaultRecruiterCommissionHeaderInsert(DefaultRecruiterCommission commission, SqlConnection connection = null, SqlTransaction activeTransaction = null)
		{
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }

			using (SqlCommand command =	new SqlCommand(DefaultRecruiterCommissionHeaderInsertProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = connection.ConnectionTimeout;
				
				command.Parameters.AddWithValue(PersonIdParam, commission.PersonId);
				command.Parameters.AddWithValue(StartDateParam, commission.StartDate);
				command.Parameters.AddWithValue(EndDateParam,
					commission.EndDate.HasValue ? (object)commission.EndDate.Value : DBNull.Value);

				SqlParameter idParam = new SqlParameter(DefaultRecruiterCommissionHeaderIdParam, SqlDbType.Int);
				idParam.Direction = ParameterDirection.Output;
				command.Parameters.Add(idParam);

				try
				{
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    if (activeTransaction != null)
                    {
                        command.Transaction = activeTransaction;
                    }

					command.ExecuteNonQuery();

					commission.CommissionHeaderId = (int)idParam.Value;
				}
				catch (SqlException ex)
				{
					throw new DataAccessException(ex);
				}
			}
		}

		/// <summary>
		/// Updates the default recruiter commission in the database.
		/// </summary>
		/// <param name="commission">The commission data to be saved.</param>
		public static void DefaultRecruiterCommissionHeaderUpdate(DefaultRecruiterCommission commission, SqlConnection connection = null, SqlTransaction activeTransaction = null)
		{
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }

			using (SqlCommand command =	new SqlCommand(DefaultRecruiterCommissionHeaderUpdateProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = connection.ConnectionTimeout;
				
				command.Parameters.AddWithValue(PersonIdParam, commission.PersonId);
				command.Parameters.AddWithValue(StartDateParam, commission.StartDate);
				command.Parameters.AddWithValue(EndDateParam,
					commission.EndDate.HasValue ? (object)commission.EndDate.Value : DBNull.Value);
				command.Parameters.AddWithValue(DefaultRecruiterCommissionHeaderIdParam, commission.CommissionHeaderId.Value);

				try
				{
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
				catch (SqlException ex)
				{
					throw new DataAccessException(ex);
				}
			}
		}

		/// <summary>
		/// Inserts a default recruiter commission item to the database.
		/// </summary>
		/// <param name="item">An item to be inserted.</param>
        public static void DefaultRecruiterCommissionItemInsert(DefaultRecruiterCommissionItem item, SqlConnection connection = null, SqlTransaction activeTransaction = null)
		{
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }

			using (SqlCommand command = new SqlCommand(DefaultRecruiterCommissionItemInsertProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = connection.ConnectionTimeout;
				
				command.Parameters.AddWithValue(DefaultRecruiterCommissionHeaderIdParam, item.CommissionHeaderId);
				command.Parameters.AddWithValue(HoursToCollectParam, item.HoursToCollect);
				command.Parameters.AddWithValue(AmountParam, item.Amount.Value);

				try
				{
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
				catch (SqlException ex)
				{
					throw new DataAccessException(ex);
				}
			}
		}

		/// <summary>
		/// Inserts a default recruiter commission item in the database.
		/// </summary>
		/// <param name="item">An item to be updated.</param>
		public static void DefaultRecruiterCommissionItemUpdate(DefaultRecruiterCommissionItem item)
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(DefaultRecruiterCommissionItemUpdateProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = connection.ConnectionTimeout;
				
				command.Parameters.AddWithValue(DefaultRecruiterCommissionHeaderIdParam, item.CommissionHeaderId);
				command.Parameters.AddWithValue(HoursToCollectParam, item.HoursToCollect);
				command.Parameters.AddWithValue(AmountParam, item.Amount);

				try
				{
					connection.Open();
					command.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					throw new DataAccessException(ex);
				}
			}
		}

		/// <summary>
		/// Deletes a default recruiter commission item from the database.
		/// </summary>
		/// <param name="item">An item to be deleted.</param>
        public static void DefaultRecruiterComissionDeleteItems(int commissionHeaderId, SqlConnection connection = null, SqlTransaction activeTransaction = null)
		{
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }

			using (SqlCommand command = new SqlCommand(DefaultRecruiterCommissionDeleteItemsProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = connection.ConnectionTimeout;
				
				command.Parameters.AddWithValue(DefaultRecruiterCommissionHeaderIdParam, commissionHeaderId);

				try
				{
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
				catch (SqlException ex)
				{
					throw new DataAccessException(ex);
				}
			}
		}


        /// <summary>
        /// Cleans up unused headers.
        /// </summary>
        /// <param name="commissionHeaderId">An ID of the rtecord to be retrived.</param>
        public static void DefaultRecruiterComissionCleanup(int commissionHeaderId, SqlConnection connection = null, SqlTransaction activeTransaction = null)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }

            using (SqlCommand command = new SqlCommand(DefaultRecruiterCommissionCleanupProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(DefaultRecruiterCommissionHeaderIdParam, commissionHeaderId);

                try
                {
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
                catch (SqlException ex)
                {
                    throw new DataAccessException(ex);
                }
            }
        }

		/// <summary>
		/// Retrives a list of items for the specified <see cref="DefaultRecruiterCommission"/>.
		/// </summary>
		/// <param name="commissionHeaderId">
		/// An ID of the default recruiter commission to retrive the data for.
		/// </param>
		/// <returns>A list of the <see cref="DefaultRecruiterCommissionItem"/> objects.</returns>
		public static List<DefaultRecruiterCommissionItem> DefaultRecruiterCommissionItemList(
			int commissionHeaderId)
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(DefaultRecruiterCommissionItemListProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = connection.ConnectionTimeout;
				
				command.Parameters.AddWithValue(DefaultRecruiterCommissionHeaderIdParam, commissionHeaderId);

				connection.Open();
				using (SqlDataReader reader = command.ExecuteReader())
				{
					List<DefaultRecruiterCommissionItem> result = new List<DefaultRecruiterCommissionItem>();
					ReadCommissionItems(reader, result);
					return result;
				}
			}
		}

		private static void ReadCommissions(DbDataReader reader, List<DefaultRecruiterCommission> result)
		{
 			if (reader.HasRows)
			{
				int defaultRecruiterCommissionHeaderIdIndex =
					reader.GetOrdinal(DefaultRecruiterCommissionHeaderIdColumn);
				int personIdIndex = reader.GetOrdinal(PersonIdColumn);
				int startDateIndex = reader.GetOrdinal(StartDateColumn);
				int endDateIndex = reader.GetOrdinal(EndDateColumn);
				int textLineIndex = reader.GetOrdinal(TextLineColumn);
				
				while (reader.Read())
				{
					DefaultRecruiterCommission commission = new DefaultRecruiterCommission();

					commission.CommissionHeaderId = reader.GetInt32(defaultRecruiterCommissionHeaderIdIndex);
					commission.PersonId = reader.GetInt32(personIdIndex);
					commission.StartDate = reader.GetDateTime(startDateIndex);
					commission.EndDate =
						!reader.IsDBNull(endDateIndex) ? (DateTime?)reader.GetDateTime(endDateIndex) : null;
					commission.TextLine =
						!reader.IsDBNull(textLineIndex) ? reader.GetString(textLineIndex) : string.Empty;

					result.Add(commission);
				}
			}
		}

		private static void ReadCommissionItems(DbDataReader reader, List<DefaultRecruiterCommissionItem> result)
		{
			if (reader.HasRows)
			{
				int defaultRecruiterCommissionHeaderIdIndex =
					reader.GetOrdinal(DefaultRecruiterCommissionHeaderIdColumn);
				int hoursToCollectIndex = reader.GetOrdinal(HoursToCollectColumn);
				int amountIndex = reader.GetOrdinal(AmountColumn);

				while (reader.Read())
				{
					DefaultRecruiterCommissionItem item =
						new DefaultRecruiterCommissionItem()
						{
							CommissionHeaderId = reader.GetInt32(defaultRecruiterCommissionHeaderIdIndex),
							HoursToCollect = reader.GetInt32(hoursToCollectIndex),
							Amount = reader.GetDecimal(amountIndex)
						};

					result.Add(item);
				}
			}
		}
	}
}

