using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;
using System;
using System.Linq;

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

        public static List<Practice> PracticeListAllWithCapabilities()
        {
            var list = new List<Practice>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Practices.PracticeListAllWithCapabilities, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        ReadPracticesWithCapabilities(reader, list);
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

        public static List<PracticeCapability> GetPracticeCapabilities(int? practiceId, int? capabilityId)
        {
            var list = new List<PracticeCapability>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Practices.GetPracticeCapabilities, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (practiceId.HasValue)
                    {
                        command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdParam, practiceId.Value);
                    }

                    if (capabilityId.HasValue)
                    {
                        command.Parameters.AddWithValue(Constants.ParameterNames.CapabilityIdParam, capabilityId.Value);
                    }
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        ReadPracticeCapabilities(reader, list);
                    }
                }
            }
            return list;
        }

        public static void CapabilityDelete(int capabilityId, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Practices.CapabilityDelete, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.CapabilityIdParam, capabilityId);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public static void CapabilityUpdate(PracticeCapability capability, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Practices.CapabilityUpdate, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.CapabilityIdParam, capability.CapabilityId);
                command.Parameters.AddWithValue(Constants.ParameterNames.Name, capability.Name);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public static void CapabilityInsert(PracticeCapability capability, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Practices.CapabilityInsert, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.Name, capability.Name);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdParam, capability.PracticeId);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Updates practice
        /// </summary>
        /// <returns>A list of <see cref="Practice"/>s in the system</returns>
        public static void UpdatePractice(Practice practice, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Practices.Update, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdParam, practice.Id);
                command.Parameters.AddWithValue(Constants.ParameterNames.Name, practice.Name);
                command.Parameters.AddWithValue(Constants.ParameterNames.Abbreviation, string.IsNullOrEmpty(practice.Abbreviation) ? (object)DBNull.Value : practice.Abbreviation);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsActive, practice.IsActive);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsCompanyInternal, practice.IsCompanyInternal);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeManagerIdParam,
                    practice.PracticeOwner.Id.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Inserts practice
        /// </summary>
        /// <returns>A list of <see cref="Practice"/>s in the system</returns>
        public static int InsertPractice(Practice practice, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Practices.Insert, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.Name, practice.Name);
                command.Parameters.AddWithValue(Constants.ParameterNames.Abbreviation, string.IsNullOrEmpty(practice.Abbreviation) ? (object)DBNull.Value : practice.Abbreviation);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsActive, practice.IsActive);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsCompanyInternal, practice.IsCompanyInternal);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeManagerIdParam,
                    practice.PracticeOwner.Id.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);

                connection.Open();

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Removes practice
        /// </summary>
        /// <returns>A list of <see cref="Practice"/>s in the system</returns>
        public static void RemovePractice(Practice practice, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Practices.Delete, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdParam, practice.Id);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);

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

            int isNotesRequiredIndex = -1;
            try
            {
                isNotesRequiredIndex = reader.GetOrdinal(Constants.ColumnNames.IsNotesRequired);
            }
            catch
            {
                isNotesRequiredIndex = -1;
            }

            int abbreviationIndex = -1;
            try
            {
                abbreviationIndex = reader.GetOrdinal(Constants.ColumnNames.Abbreviation);
            }
            catch
            {
                abbreviationIndex = -1;
            }

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

                if (isNotesRequiredIndex > -1)
                {
                    practice.IsNotesRequiredForTimeEnry = reader.GetBoolean(isNotesRequiredIndex);
                }
                if (abbreviationIndex > -1)
                {
                    practice.Abbreviation = !reader.IsDBNull(abbreviationIndex) ? reader.GetString(abbreviationIndex) : string.Empty;
                }

                list.Add(practice);
            }
        }

        private static void ReadPracticesWithCapabilities(DbDataReader reader, List<Practice> list)
        {
            if (!reader.HasRows) return;

            var practiceIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
            var nameIndex = reader.GetOrdinal(Constants.ColumnNames.Name);
            var isActiveIndex = reader.GetOrdinal(Constants.ColumnNames.IsActive);
            int abbreviationIndex = reader.GetOrdinal(Constants.ColumnNames.Abbreviation);
            var capabilityIdIndex = reader.GetOrdinal(Constants.ColumnNames.CapabilityId);
            var capabilityNameIndex = reader.GetOrdinal(Constants.ColumnNames.CapabilityName);
            var inUseIndex = reader.GetOrdinal(Constants.ColumnNames.InUse);

            while (reader.Read())
            {
                int practiceId = reader.GetInt32(practiceIdIndex);
                Practice practice;
                if (list.Any(p => p.Id == practiceId))
                {
                    practice = list.FirstOrDefault(p => p.Id == practiceId);
                }
                else
                {
                    practice =
                        new Practice
                        {
                            Id = practiceId,
                            Name = reader.GetString(nameIndex),
                            IsActive = reader.GetBoolean(isActiveIndex),
                            Abbreviation = !reader.IsDBNull(abbreviationIndex) ? reader.GetString(abbreviationIndex) : string.Empty
                        };
                    practice.PracticeCapabilities = new List<PracticeCapability>();
                    list.Add(practice);
                }

                if (!reader.IsDBNull(capabilityIdIndex))
                {
                    var practiceCapability =
                        new PracticeCapability
                        {
                            CapabilityId = reader.GetInt32(capabilityIdIndex),
                            PracticeId = practiceId,
                            InUse = reader.GetBoolean(inUseIndex),
                            Name = reader.GetString(capabilityNameIndex)
                        };
                    practice.PracticeCapabilities.Add(practiceCapability);
                }
            }
        }

        private static void ReadPracticeCapabilities(SqlDataReader reader, List<PracticeCapability> list)
        {
            if (!reader.HasRows) return;

            var capabilityIdIndex = reader.GetOrdinal(Constants.ColumnNames.CapabilityId);
            var practiceIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
            var nameIndex = reader.GetOrdinal(Constants.ColumnNames.Name);
            var practiceAbbreviationIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeAbbreviation);

            while (reader.Read())
            {
                var practiceCapability =
                    new PracticeCapability
                    {
                        CapabilityId = reader.GetInt32(capabilityIdIndex),
                        PracticeId = reader.GetInt32(practiceIdIndex),
                        Name = reader.GetString(nameIndex),
                        PracticeAbbreviation = reader.GetString(practiceAbbreviationIndex)
                    };
                list.Add(practiceCapability);
            }
        }
    }
}

