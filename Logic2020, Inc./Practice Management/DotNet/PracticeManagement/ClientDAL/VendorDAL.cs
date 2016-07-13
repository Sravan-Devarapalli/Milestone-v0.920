using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Other;
using DataTransferObjects;
using System.Data.SqlClient;
using System.Data;

namespace DataAccess
{
    public class VendorDAL
    {
        public static List<Vendor> GetAllActiveVendors()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Vendor.GetAllActiveVendors, connection))
                {
                    List<Vendor> result;
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        result = new List<Vendor>();
                        ReadVendorDetails(reader, result);
                        return result;
                    }
                }
            }
        }

        public static List<VendorType> GetAllVendorTypes()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Vendor.GetVendorTypes, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    List<VendorType> result;
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        result = new List<VendorType>();
                        ReadVendorTypes(reader, result);
                        return result;
                    }
                }
            }

        }

        private static void ReadVendorTypes(SqlDataReader reader, List<VendorType> result)
        {
            if (reader.HasRows)
            {
                int idIndex = reader.GetOrdinal(Constants.ColumnNames.Id);
                int nameIndex = reader.GetOrdinal(Constants.ColumnNames.Name);
                while (reader.Read())
                {
                    VendorType vendorType = new VendorType
                    {
                        Id = reader.GetInt32(idIndex),
                        Name = reader.GetString(nameIndex)
                    };
                    result.Add(vendorType);
                }
            }

        }

        public static List<Vendor> GetListOfVendorsWithFilters(bool active, bool inactive, string vendorTypes, string looked)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Vendor.GetVendorsListWithFilters, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowActiveParam, active);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowInactiveParam, inactive);
                    command.Parameters.AddWithValue(Constants.ParameterNames.VendorTypes, vendorTypes);
                    command.Parameters.AddWithValue(Constants.ParameterNames.LookedParam, !string.IsNullOrEmpty(looked) ? (object)looked : DBNull.Value);
                    List<Vendor> result;
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        result = new List<Vendor>();
                        ReadVendorDetails(reader, result);
                        return result;
                    }
                }
            }
        }

        private static void ReadVendorDetails(SqlDataReader reader, List<Vendor> result)
        {
            if (reader.HasRows)
            {
                int idIndex = reader.GetOrdinal(Constants.ColumnNames.Id);
                int nameIndex = reader.GetOrdinal(Constants.ColumnNames.Name);
                int contactNameIndex = reader.GetOrdinal(Constants.ColumnNames.ContactName);
                int statusIndex = reader.GetOrdinal(Constants.ColumnNames.StatusColumn);
                int emailIndex = reader.GetOrdinal(Constants.ColumnNames.Email);
                int telephoneIndex = reader.GetOrdinal(Constants.ColumnNames.TelephoneNumber);
                int vendorTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.VendorTypeId);
                int vendorNameIndex = reader.GetOrdinal(Constants.ColumnNames.VendorTypeName);

                while (reader.Read())
                {
                    Vendor vendor = new Vendor
                    {
                        Id = reader.GetInt32(idIndex),
                        Name = reader.GetString(nameIndex),
                        ContactName = reader.GetString(contactNameIndex),
                        Status = reader.GetInt32(statusIndex) == 1 ? true : false,
                        Email = reader.GetString(emailIndex),
                        TelephoneNumber = reader.GetString(telephoneIndex),
                        VendorType = new VendorType { Id = reader.GetInt32(vendorTypeIdIndex), Name = reader.GetString(vendorNameIndex) }
                    };
                    result.Add(vendor);
                }
            }
        }

        public static Vendor GetVendorById(int vendorId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Vendor.GetVendorById, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.IdParam, vendorId);
                    connection.Open();
                    List<Vendor> result;
                    using (var reader = command.ExecuteReader())
                    {
                        result = new List<Vendor>();
                        ReadVendorDetails(reader, result);
                    }
                    return result.Count > 0 ? result[0] : null;
                }
            }
        }

        public static void InsertVendor(Vendor vendor, string userName)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Vendor.InsertVendor, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.NameParam, vendor.Name);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ContactName, vendor.ContactName);
                    command.Parameters.AddWithValue(Constants.ParameterNames.Status, vendor.Status);
                    command.Parameters.AddWithValue(Constants.ParameterNames.Email, vendor.Email);
                    command.Parameters.AddWithValue(Constants.ParameterNames.VendorTypeId, vendor.VendorType.Id);
                    command.Parameters.AddWithValue(Constants.ParameterNames.TelephoneNumber, vendor.TelephoneNumber);
                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userName);
                    SqlParameter IdParam = new SqlParameter(Constants.ParameterNames.IdParam, SqlDbType.Int) { Direction = ParameterDirection.Output };
                    command.Parameters.Add(IdParam);
                    connection.Open();
                    command.ExecuteNonQuery();

                    vendor.Id = (int)IdParam.Value;
                }
            }

        }


        public static void UpdateVendor(Vendor vendor, string userName)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Vendor.UpdateVendor, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.IdParam, vendor.Id);
                    command.Parameters.AddWithValue(Constants.ParameterNames.NameParam, vendor.Name);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ContactName, vendor.ContactName);
                    command.Parameters.AddWithValue(Constants.ParameterNames.Status, vendor.Status);
                    command.Parameters.AddWithValue(Constants.ParameterNames.Email, vendor.Email);
                    command.Parameters.AddWithValue(Constants.ParameterNames.VendorTypeId, vendor.VendorType.Id);
                    command.Parameters.AddWithValue(Constants.ParameterNames.TelephoneNumber, vendor.TelephoneNumber);
                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userName);
                    connection.Open();
                    command.ExecuteNonQuery();

                }
            }
        }

        public static void VendorValidations(Vendor vendor)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Vendor.VendorValidations, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.IdParam, vendor.Id.HasValue ? (object)vendor.Id.Value : DBNull.Value);
                    command.Parameters.AddWithValue(Constants.ParameterNames.NameParam, vendor.Name);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ContactName, vendor.ContactName);
                    command.Parameters.AddWithValue(Constants.ParameterNames.Email, vendor.Email);

                    connection.Open();
                    command.ExecuteNonQuery();

                }
            }
        }

        public static List<VendorAttachment> GetVendorAttachments(int vendorId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Vendor.GetVendorAttachments, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.IdParam, vendorId);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var result = new List<VendorAttachment>();
                        ReadVendorAttachments(reader, result);
                        return result;
                    }
                }
            }
        }

        public static void ReadVendorAttachments(SqlDataReader reader, List<VendorAttachment> attachments)
        {
            if (!reader.HasRows) return;
            int idIndex = reader.GetOrdinal(Constants.ColumnNames.Id);
            int attachmentFileNameIndex = reader.GetOrdinal(Constants.ColumnNames.FileName);
            int uploadedDateIndex = reader.GetOrdinal(Constants.ColumnNames.UploadedDate);
            int attachmentSizeIndex = reader.GetOrdinal(Constants.ColumnNames.AttachmentSize);
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
            int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);

            while (reader.Read())
            {
                VendorAttachment attachment = new VendorAttachment
                {
                    AttachmentId = reader.GetInt32(idIndex),
                    AttachmentFileName = reader.GetString(attachmentFileNameIndex),
                    AttachmentSize = (int)reader.GetInt64(attachmentSizeIndex),
                    UploadedDate =
                        reader.IsDBNull(uploadedDateIndex)
                            ? null
                            : (DateTime?)reader.GetDateTime(uploadedDateIndex),

                };
                if (!reader.IsDBNull(personIdIndex))
                {
                    attachment.Uploader = reader.GetString(firstNameIndex) + " " + reader.GetString(lastNameIndex);
                }
                attachments.Add(attachment);
            }
        }

        public static void SaveVendorAttachmentData(VendorAttachment attachment, int vendorId, string userName)
        {
            var connection = new SqlConnection(DataSourceHelper.DataConnection);

            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Vendor.SaveVendorAttachment, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.IdParam, vendorId);
                command.Parameters.AddWithValue(Constants.ParameterNames.UploadedDateParam, DateTime.Now);
                command.Parameters.AddWithValue(Constants.ParameterNames.AttachmentFileName, !string.IsNullOrEmpty(attachment.AttachmentFileName) ? (object)attachment.AttachmentFileName : DBNull.Value);
                command.Parameters.Add(Constants.ParameterNames.AttachmentData, SqlDbType.VarBinary, -1);
                command.Parameters[Constants.ParameterNames.AttachmentData].Value = attachment.AttachmentData != null ? (object)attachment.AttachmentData : DBNull.Value;
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam,
                    !string.IsNullOrEmpty(userName) ? (object)userName : DBNull.Value);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void DeleteVendorAttachmentById(int? attachmentId, int vendorId, string userName)
        {
            var connection = new SqlConnection(DataSourceHelper.DataConnection);

            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Vendor.DeleteVendorAttachmentById, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.AttachmentIdParam, attachmentId.HasValue ? (object)attachmentId : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.VendorId, vendorId);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam,
                    !string.IsNullOrEmpty(userName) ? (object)userName : DBNull.Value);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static byte[] GetVendorAttachmentData(int vendorId, int attachmentId)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Vendor.GetVendorAttachmentData, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.VendorId, vendorId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.AttachmentIdParam, attachmentId);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    try
                    {
                        Byte[] AttachmentData = null;

                        if (reader.HasRows)
                        {
                            int AttachmentDataIndex = reader.GetOrdinal(Constants.ColumnNames.AttachmentDataColumn);

                            while (reader.Read())
                            {
                                AttachmentData = (byte[])reader[AttachmentDataIndex];
                            }
                        }

                        return AttachmentData;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        public static List<Project> ProjectListByVendor(int vendorId)
        {
            var projectList = new List<Project>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Vendor.ProjectListByVendor, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(Constants.ParameterNames.VendorId, vendorId);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    ReadProjects(reader, projectList);
                }
            }

            return projectList;
        }

        private static void ReadProjects(SqlDataReader reader, List<Project> resultList)
        {
            try
            {
                if (!reader.HasRows) return;
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIdColumn);
                int nameIndex = reader.GetOrdinal(Constants.ColumnNames.NameColumn);
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
                int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientIdColumn);
                int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientNameColumn);
                int divisionIdIndex = divisionIdIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionId);
                int divisionNameIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionName);
                int practiceIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
                int practiceNameIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeNameColumn);
                int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDateColumn);
                int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDateColumn);
                int projectStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusIdColumn);
                int projectStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusNameColumn);
                int projectIsChargeableIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIsChargeable);
                int projectGroupIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupIdColumn);
                int projectGroupNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupNameColumn);
                while (reader.Read())
                {
                    var project = new Project
                    {
                        Id = reader.GetInt32(projectIdIndex),
                        Name = reader.GetString(nameIndex),
                        StartDate =
                            !reader.IsDBNull(startDateIndex) ? (DateTime?)reader.GetDateTime(startDateIndex) : null,
                        EndDate =
                            !reader.IsDBNull(endDateIndex) ? (DateTime?)reader.GetDateTime(endDateIndex) : null,
                        ProjectNumber = reader.GetString(projectNumberIndex),

                        IsChargeable = reader.GetBoolean(projectIsChargeableIndex),
                        Practice = new Practice
                        {
                            Id = reader.GetInt32(practiceIdIndex),
                            Name = reader.GetString(practiceNameIndex)
                        },
                        Division = new ProjectDivision
                        {
                            Id = reader.GetInt32(divisionIdIndex),
                            Name = reader.GetString(divisionNameIndex)
                        }
                    };
                    project.Status = new ProjectStatus
                    {
                        Id = reader.GetInt32(projectStatusIdIndex),
                        Name = reader.GetString(projectStatusNameIndex)
                    };
                    project.Group = new ProjectGroup
                    {
                        Id = (int)reader[projectGroupIdIndex],
                        Name = (string)reader[projectGroupNameIndex],
                    };
                    resultList.Add(project);
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<Person> PersonListByVendor(int vendorId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Vendor.PersonListByVendor, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(Constants.ParameterNames.VendorId, vendorId);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    var personsList = new List<Person>();
                    ReadPesons(reader, personsList);
                    return personsList;
                }
            }

        }

        private static void ReadPesons(SqlDataReader reader, List<Person> resultList)
        {

            if (!reader.HasRows) return;
            int aliasIndex = reader.GetOrdinal(Constants.ColumnNames.Alias);
            int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
            int hireDateIndex = reader.GetOrdinal(Constants.ColumnNames.HireDateColumn);
            int terminationDateIndex = reader.GetOrdinal(Constants.ColumnNames.TerminationDateColumn);
            int practiceIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
            int practiceAreaNameIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeNameColumn);

            int divisionIdIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionId);
            int divisionNameIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionName);

            int personStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonStatusId);
            int personStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.PersonStatusName);

            int titleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleId);
            int titleNameIndex = reader.GetOrdinal(Constants.ColumnNames.Title);

            int payPersonIdIndex = reader.GetOrdinal(Constants.ColumnNames.PayPersonIdColumn);
            int payStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
            int payEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDate);

            int timescaleIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleColumn);
            int timescaleNameIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleName);
            while (reader.Read())
            {
                var person = new Person
                {
                    Id = (int)reader.GetInt32(personIdIndex),
                    FirstName = reader.GetString(firstNameIndex),
                    LastName = reader.GetString(lastNameIndex),
                    Alias =
                        !reader.IsDBNull(aliasIndex)
                            ? reader.GetString(aliasIndex)
                            : string.Empty,
                    HireDate = (DateTime)reader[hireDateIndex],
                    TerminationDate = !reader.IsDBNull(terminationDateIndex) ? (DateTime?)reader[terminationDateIndex] : null,
                    Status = new PersonStatus
                    {
                        Id = reader.GetInt32(personStatusIdIndex),
                        Name = reader.GetString(personStatusNameIndex)
                    },
                    Title = !reader.IsDBNull(titleIdIndex) ? new Title
                    {
                        TitleId = reader.GetInt32(titleIdIndex),
                        TitleName = reader.GetString(titleNameIndex)
                    } : null,
                    DefaultPractice = !Convert.IsDBNull(reader[practiceIdIndex])
                                              ? new Practice
                                              {
                                                  Id = (int)reader[practiceIdIndex],
                                                  Name = (string)reader[practiceAreaNameIndex]
                                              } : null,
                };

                if (!reader.IsDBNull(divisionIdIndex))
                {
                    person.Division = new PersonDivision()
                    {
                        DivisionId = reader.GetInt32(divisionIdIndex),
                        DivisionName = reader.GetString(divisionNameIndex)
                    };
                }
                if (!reader.IsDBNull(payPersonIdIndex))
                {
                    Pay pay = new Pay
                    {
                        PersonId = reader.GetInt32(personIdIndex),
                        Timescale = (TimescaleType)reader.GetInt32(timescaleIndex),
                        TimescaleName = reader.GetString(timescaleNameIndex),
                        StartDate = reader.GetDateTime(payStartDateIndex),
                        EndDate =
                            !reader.IsDBNull(payEndDateIndex) ? (DateTime?)reader.GetDateTime(payEndDateIndex) : null,
                    };
                    person.CurrentPay = pay;
                }
                resultList.Add(person);
            }

        }



    }
}

