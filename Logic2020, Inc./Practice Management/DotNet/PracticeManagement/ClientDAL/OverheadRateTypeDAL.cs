using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;

namespace DataAccess
{
	/// <summary>
	/// Provides an access to the OverheadRateType data.
	/// </summary>
	public static class OverheadRateTypeDAL
	{
		#region Constants

		#region Procedures

		private const string OverheadRateTypeListAllProcedure = "dbo.OverheadRateTypeListAll";
		private const string OverheadRateTypeGetByIdProcedure = "dbo.OverheadRateTypeGetById";

		#endregion

		#region Parameters

		private const string OverheadRateTypeIdParam = "@OverheadRateTypeId";

		#endregion

		#region Columns

		private const string OverheadRateTypeIdColumn = "OverheadRateTypeId";
		private const string NameColumn = "Name";
		private const string IsPercentageColumn = "IsPercentage";
		private const string HoursToCollectColumn = "HoursToCollect";

		#endregion

		#endregion

		#region Methods

		/// <summary>
		/// Retrieves a list of the <see cref="OverheadRateType"/> objects form the database.
		/// </summary>
		/// <returns>The list of the <see cref="OverheadRateType"/> objects.</returns>
		public static List<OverheadRateType> OverheadRateTypeListAll()
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(OverheadRateTypeListAllProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;

				connection.Open();
				using (SqlDataReader reader = command.ExecuteReader())
				{
					List<OverheadRateType> result = new List<OverheadRateType>();

					ReadOverheadRateTypes(reader, result);
					return result;
				}
			}
		}

		/// <summary>
		/// Retrieves an overhead rate type info by the specified ID.
		/// </summary>
		/// <param name="overheadRateTypeId">An ID of the record to be reatrieved from.</param>
		/// <returns>The <see cref="OverheadRateType"/> object if found and null otherwise.</returns>
		public static OverheadRateType OverheadRateTypeGetById(int overheadRateTypeId)
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(OverheadRateTypeGetByIdProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.AddWithValue(OverheadRateTypeIdParam, overheadRateTypeId);

				connection.Open();
				using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
				{
					List<OverheadRateType> result = new List<OverheadRateType>(1);

					ReadOverheadRateTypes(reader, result);

					return result.Count > 0 ? result[0] : null;
				}
			}
		}

		private static void ReadOverheadRateTypes(DbDataReader reader, List<OverheadRateType> result)
		{
			if (reader.HasRows)
			{
				int overheadRateTypeIdIndex = reader.GetOrdinal(OverheadRateTypeIdColumn);
				int nameIndex = reader.GetOrdinal(NameColumn);
				int isPercentageIndex = reader.GetOrdinal(IsPercentageColumn);
				int hoursToCollectIndex = reader.GetOrdinal(HoursToCollectColumn);

				while (reader.Read())
				{
					OverheadRateType rateType = new OverheadRateType();

					rateType.Id = reader.GetInt32(overheadRateTypeIdIndex);
					rateType.Name = reader.GetString(nameIndex);
					rateType.IsPercentage = reader.GetBoolean(isPercentageIndex);
					rateType.HoursToCollect = reader.GetInt32(hoursToCollectIndex);

					result.Add(rateType);
				}
			}
		}

		#endregion
	}
}

