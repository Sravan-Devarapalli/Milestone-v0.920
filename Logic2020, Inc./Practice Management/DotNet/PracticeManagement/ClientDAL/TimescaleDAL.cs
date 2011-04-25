using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;

namespace DataAccess
{
	/// <summary>
	/// Provides an access to the data in the Timescale table
	/// </summary>
	public static class TimescaleDAL
	{
		#region Constants

		private const string TimescaleGetByIdProcedure = "dbo.TimescaleGetById";
        private const string TimescaleGetAllProcedure = "dbo.TimeScaleGetAll";

		private const string TimescaleIdParam = "@TimescaleId";

		private const string TimescaleIdColumn = "TimescaleId";
		private const string NameColumn = "Name";
		private const string DefaultTermsColumn = "DefaultTerms";

		#endregion

		#region Methods

		/// <summary>
		/// Retrives a <see cref="Timescale"/> by its ID.
		/// </summary>
		/// <param name="timescaleId">An ID of the <see cref="Timescale"/> to be retrieved.</param>
		/// <returns>A <see cref="Timescale"/> object if found and null otherwise.</returns>
		public static Timescale GetById(TimescaleType timescaleId)
		{
			using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
			using (SqlCommand command = new SqlCommand(TimescaleGetByIdProcedure, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.AddWithValue(TimescaleIdParam, (int)timescaleId);

				connection.Open();

				using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
				{
					List<Timescale> result = new List<Timescale>(1);

					ReadTimescales(reader, result);

					return result.Count > 0 ? result[0] : null;
				}
			}
		}

        public static List<Timescale> GetAll()
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(TimescaleGetAllProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<Timescale> result = new List<Timescale>();

                    ReadTimescales(reader, result);

                    return result;
                }
            }
        }

		private static void ReadTimescales(DbDataReader reader, List<Timescale> result)
		{
			if (reader.HasRows)
			{
				int timescaleIdIndex = reader.GetOrdinal(TimescaleIdColumn);
				int nameIndex = reader.GetOrdinal(NameColumn);
				int defaultTermsIndex = reader.GetOrdinal(DefaultTermsColumn);

				while (reader.Read())
				{
					Timescale timescale = new Timescale();

					timescale.Id = reader.GetInt32(timescaleIdIndex);
					timescale.Name = reader.GetString(nameIndex);
					timescale.DefaultTerms =
						!reader.IsDBNull(defaultTermsIndex) ? (int?)reader.GetInt32(defaultTermsIndex) : null;

					result.Add(timescale);
				}
			}
		}

		#endregion
	}
}

