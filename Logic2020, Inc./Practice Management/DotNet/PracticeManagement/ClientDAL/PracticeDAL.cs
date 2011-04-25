using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;

namespace DataAccess
{
    public static class PracticeDAL
    {
        private static readonly string ConnectionString = DataSourceHelper.DataConnection;

        public static List<Practice> PracticeListAll()
        {
            return PracticeListAll(null);
        }

        public static List<Practice> PracticeListAll(Person person)
        {
            var list = new List<Practice>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Practices.GetAll, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (person != null)
                        command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, person.Id);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        ReadPractices(reader, list);
                    }
                }
            }
            return list;
        }

        public static List<Practice> PracticeGetById(int? id)
        {
            var list = new List<Practice>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Practices.GetById, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdParam, id);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        ReadPractices(reader, list);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Updates practice
        /// </summary>
        /// <returns>A list of <see cref="Practice"/>s in the system</returns>
        public static void UpdatePractice(Practice practice)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Practices.Update, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdParam, practice.Id);
                command.Parameters.AddWithValue(Constants.ParameterNames.Name, practice.Name);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsActive, practice.IsActive);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsCompanyInternal, practice.IsCompanyInternal);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeManagerIdParam,
                    practice.PracticeOwner.Id.Value);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Inserts practice
        /// </summary>
        /// <returns>A list of <see cref="Practice"/>s in the system</returns>
        public static int InsertPractice(Practice practice)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Practices.Insert, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.Name, practice.Name);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsActive, practice.IsActive);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsCompanyInternal, practice.IsCompanyInternal);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeManagerIdParam,
                    practice.PracticeOwner.Id.Value);

                connection.Open();

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Removes practice
        /// </summary>
        /// <returns>A list of <see cref="Practice"/>s in the system</returns>
        public static void RemovePractice(Practice practice)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Practices.Delete, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdParam, practice.Id);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        private static void ReadPractices(DbDataReader reader, ICollection<Practice> list)
        {
            if (!reader.HasRows) return;

            var practiceIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
            var nameIndex = reader.GetOrdinal(Constants.ColumnNames.Name);
            var isActiveIndex = reader.GetOrdinal(Constants.ColumnNames.IsActive);
            var inUseIndex = reader.GetOrdinal(Constants.ColumnNames.InUse);
            var isCompanyInternalIndex = reader.GetOrdinal(Constants.ColumnNames.IsCompanyInternal);

            var firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            var lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
            var personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);

            var statusIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonStatusId);
            var statusNameIndex = reader.GetOrdinal(Constants.ColumnNames.PersonStatusName);

            while (reader.Read())
            {
                var practice =
                    new Practice
                    {
                        Id = reader.GetInt32(practiceIdIndex),
                        Name = reader.GetString(nameIndex),
                        IsActive = reader.GetBoolean(isActiveIndex),
                        InUse = reader.GetBoolean(inUseIndex),
                        IsCompanyInternal = reader.GetBoolean(isCompanyInternalIndex),
                        PracticeOwner =
                             new Person
                             {
                                 Id = reader.GetInt32(personIdIndex),
                                 LastName = reader.GetString(lastNameIndex),
                                 FirstName = reader.GetString(firstNameIndex),
                                 Status = new PersonStatus
                                 {
                                     Id = reader.GetInt32(statusIdIndex),
                                     Name = reader.GetString(statusNameIndex)
                                 }
                             }
                    };

                list.Add(practice);
            }
        }
    }
}

