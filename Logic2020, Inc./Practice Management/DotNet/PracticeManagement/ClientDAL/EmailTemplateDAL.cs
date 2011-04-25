using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataTransferObjects;
using System.Data.SqlClient;
using System.Data;
using DataAccess.Other;
using DataTransferObjects.ContextObjects;

namespace DataAccess
{
    /// <summary>
    /// Access EmailTemplates in db
    /// </summary>
    public static class EmailTemplateDAL
    {
        #region Constants

        #region Stored Procedures
        private const string EmailTemplateInsertProcedure = "dbo.EmailTemplateInsert";
        private const string EmailTemplateGetAllProcedure = "dbo.EmailTemplateGetAll";
        private const string EmailTemplateUpdateProcedure = "dbo.EmailTemplateUpdate";
        private const string EmailTemplateDeleteProcedure = "dbo.EmailTemplateDelete";
        private const string EmailTemplateGetByNameProcedure = "dbo.EmailTemplateGetByName";
        private const string EmailTemplateGetTemplateForProjectedProjectWithRecipientsProcedure = "dbo.EmailTemplateGetTemplateForProjectedProjectWithRecipients";
        private const string EmailTemplateGetTemplateForProjectedProjectByHireDateProcedure = "dbo.EmailTemplateGetTemplateForProjectedProjectByHireDate";
        #endregion

        #region Parameters
        private const string EmailTemplateIdParam = "@EmailTemplateId";
        private const string EmailTemplateNameParam = "@EmailTemplateName";
        private const string EmailTemplateToParam = "@EmailTemplateTo";
        private const string EmailTemplateCcParam = "@EmailTemplateCc";
        private const string EmailTemplateSubjectParam = "@EmailTemplateSubject";
        private const string EmailTemplateBodyParam = "@EmailTemplateBody";

        #endregion

        #region Columns

        private const string EmailTemplateIdColumn = "EmailTemplateId";
        private const string EmailTemplateNameColumn = "EmailTemplateName";
        private const string EmailTemplateToColumn = "EmailTemplateTo";
        private const string EmailTemplateCcColumn = "EmailTemplateCc";
        private const string EmailTemplateSubjectColumn = "EmailTemplateSubject";
        private const string EmailTemplateBodyColumn = "EmailTemplateBody";

        private const string ProjectIdColumn = "ProjectId";
        private const string ProjectOwnerColumn = "ProjectOwner";
        private const string SalespersonColumn = "Salesperson";
        private const string PracticeOwnerColumn = "PracticeOwner";
        private const string ClientNameColumn = "ClientName";
        private const string ProjectNameColumn = "ProjectName";
        private const string MileStoneStartDateColumn = "StartDate";
        private const string LineManagerColumn = "LineManager";
        private const string PersonRecruterColumn = "PersonRecruter";
        private const string PersonHireDateColumn = "HireDate";
        private const string PersonNameColumn = "PersonName";
        #endregion

        #endregion

        #region Methods
        public static List<EmailTemplate> GetAllEmailTemplates()
        {
            using (SqlConnection connection = new SqlConnection(DataAccess.Other.DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(EmailTemplateGetAllProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var templates = new List<EmailTemplate>();
                    ReadEmailTemplates(reader, templates);

                    return templates;
                }
            }
        }

        public static EmailTemplate EmailTemplateGetByName(string emailTemplateName)
        {
            using (SqlConnection connection = new SqlConnection(DataAccess.Other.DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(EmailTemplateGetByNameProcedure, connection))
            {
                command.Parameters.AddWithValue(EmailTemplateNameParam, emailTemplateName);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var templates = new List<EmailTemplate>();
                    ReadEmailTemplates(reader, templates);
                    if (templates.Any())
                        return templates[0];
                    else
                        return null;
                }
            }
        }


        public static bool UpdateEmailTemplate(EmailTemplate template)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(EmailTemplateUpdateProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(EmailTemplateIdParam, template.Id);
                command.Parameters.AddWithValue(EmailTemplateNameParam, template.Name);
                command.Parameters.AddWithValue(EmailTemplateToParam, template.EmailTemplateTo);
                command.Parameters.AddWithValue(EmailTemplateCcParam, template.EmailTemplateCc);
                command.Parameters.AddWithValue(EmailTemplateSubjectParam, template.Subject);
                command.Parameters.AddWithValue(EmailTemplateBodyParam, template.Body);

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

            return true;
        }

        public static bool AddEmailTemplate(EmailTemplate template)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(EmailTemplateInsertProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(EmailTemplateNameParam, template.Name);
                command.Parameters.AddWithValue(EmailTemplateToParam, template.EmailTemplateTo);
                command.Parameters.AddWithValue(EmailTemplateCcParam, template.EmailTemplateCc);
                command.Parameters.AddWithValue(EmailTemplateSubjectParam, template.Subject);
                command.Parameters.AddWithValue(EmailTemplateBodyParam, template.Body);

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

            return true;
        }

        public static bool DeleteEmailTemplate(int templateId)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(EmailTemplateDeleteProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(EmailTemplateIdParam, templateId);

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

            return true;
        }

        public static void CheckProjectedProjectsByHireDate(int templateId, Dictionary<int, EmailRecepients> projects, out string body, out string subject)
        {
            subject = string.Empty;
            body = string.Empty;

            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(EmailTemplateGetTemplateForProjectedProjectByHireDateProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(EmailTemplateIdParam, templateId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int projectIdIndex = reader.GetOrdinal(ProjectIdColumn);
                        int lineManagerIndex = reader.GetOrdinal(LineManagerColumn);
                        int personRecruiterIndex = reader.GetOrdinal(PersonRecruterColumn);
                        int practiceOwnerIndex = reader.GetOrdinal(PracticeOwnerColumn);
                        int personHireDateIndex = reader.GetOrdinal(PersonHireDateColumn);
                        int personNameIndex = reader.GetOrdinal(PersonNameColumn);

                        while (reader.Read())
                        {
                            int projectId = reader.GetInt32(projectIdIndex);
                            if (!projects.ContainsKey(projectId))
                            {
                                projects.Add(projectId, new EmailRecepients());
                            }

                            projects[projectId].ToPersonAddresses.Add(reader.GetString(practiceOwnerIndex));
                            projects[projectId].ToPersonAddresses.Add(reader.GetString(lineManagerIndex));
                            projects[projectId].ToPersonAddresses.Add(reader.GetString(personRecruiterIndex));
                            projects[projectId].Parameters.Add("HireDate", reader.GetDateTime(personHireDateIndex).ToShortDateString());
                            projects[projectId].Parameters.Add("PersonName", reader.GetString(personNameIndex));
                        }

                        reader.NextResult(); // step to email template select
                    }

                    ReadEmailMembers(reader, out body, out subject);
                }
            }
        }

        private static void ReadEmailMembers(SqlDataReader reader, out string body, out string subject)
        {
            subject = string.Empty;
            body = string.Empty;

            if (reader.HasRows) // read template
            {
                int emailTemplateSubjectIndex = reader.GetOrdinal(EmailTemplateSubjectColumn);
                int emailTemplateBodyIndex = reader.GetOrdinal(EmailTemplateBodyColumn);

                while (reader.Read())
                {
                    subject = reader.GetString(emailTemplateSubjectIndex);
                    body = reader.GetString(emailTemplateBodyIndex);
                }
            }
        }
        public static void CheckProjectedProjects(int templateId, Dictionary<int, EmailRecepients> projects, out string body, out string subject)
        {
            subject = string.Empty;
            body = string.Empty;

            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(EmailTemplateGetTemplateForProjectedProjectWithRecipientsProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(EmailTemplateIdParam, templateId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int projectIdIndex = reader.GetOrdinal(ProjectIdColumn);
                        int projectOwnerIndex = reader.GetOrdinal(ProjectOwnerColumn);
                        int salesPersonIndex = reader.GetOrdinal(SalespersonColumn);
                        int practiceOwnerIndex = reader.GetOrdinal(PracticeOwnerColumn);
                        int clientNameIndex = reader.GetOrdinal(ClientNameColumn);
                        int projectNameIndex = reader.GetOrdinal(ProjectNameColumn);
                        int milestoneStartDateIndex = reader.GetOrdinal(MileStoneStartDateColumn);

                        while (reader.Read())
                        {
                            int projectId = reader.GetInt32(projectIdIndex);
                            if (!projects.ContainsKey(projectId))
                            {
                                projects.Add(projectId, new EmailRecepients());
                            }

                            projects[projectId].ToPersonAddresses.Add(reader.GetString(projectOwnerIndex));
                            projects[projectId].ToPersonAddresses.Add(reader.GetString(salesPersonIndex));
                            projects[projectId].ToPersonAddresses.Add(reader.GetString(practiceOwnerIndex));
                            projects[projectId].Parameters.Add("ClientName", reader.GetString(clientNameIndex));
                            projects[projectId].Parameters.Add("ProjectName", reader.GetString(projectNameIndex));
                            projects[projectId].Parameters.Add("MilestoneStartDate", reader.GetDateTime(milestoneStartDateIndex).ToShortDateString());
                        }

                        reader.NextResult(); // step to email template select
                    }

                    ReadEmailMembers(reader, out body, out subject);
                }
            }
        }


        public static EmailData GetEmailData(EmailContext emailContext)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(emailContext.StorerProcedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(EmailTemplateIdParam, emailContext.EmailTemplateId);

                connection.Open();

                var adapter = new SqlDataAdapter(command);
                var data = new DataSet();
                adapter.Fill(data, emailContext.StorerProcedureName);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.NextResult(); // step to email template select 
                        var emailTemplate = ReadEmailMembers(reader);

                        return new EmailData
                                   {
                                       Data = data,
                                       EmailTemplate = emailTemplate
                                   };
                    }

                    return new EmailData();
                }
            }
        }

        private static EmailTemplate ReadEmailMembers(SqlDataReader reader)
        {
            if (reader.HasRows) // read template
            {
                var emailTemplateSubjectIndex = reader.GetOrdinal(EmailTemplateSubjectColumn);
                var emailTemplateBodyIndex = reader.GetOrdinal(EmailTemplateBodyColumn);

                while (reader.Read())
                {
                    return new EmailTemplate
                               {
                                   Subject = reader.GetString(emailTemplateSubjectIndex),
                                   Body = reader.GetString(emailTemplateBodyIndex)
                               };
                }
            }

            return null;
        }

        #endregion


        private static void ReadEmailTemplates(SqlDataReader reader, List<EmailTemplate> templates)
        {
            if (reader.HasRows)
            {
                int emailTemplateIdIndex = reader.GetOrdinal(EmailTemplateIdColumn);
                int emailTemplateNameIndex = reader.GetOrdinal(EmailTemplateNameColumn);
                int emailTemplateToIndex = reader.GetOrdinal(EmailTemplateToColumn);
                int emailTemplateCcIndex = reader.GetOrdinal(EmailTemplateCcColumn);
                int emailTemplateSubjectIndex = reader.GetOrdinal(EmailTemplateSubjectColumn);
                int emailTemplateBodyIndex = reader.GetOrdinal(EmailTemplateBodyColumn);


                while (reader.Read())
                {
                    EmailTemplate template = new EmailTemplate();

                    template.Id = reader.GetInt32(emailTemplateIdIndex);
                    template.Name = reader.GetString(emailTemplateNameIndex);
                    template.EmailTemplateTo = reader.IsDBNull(emailTemplateToIndex) ? null : reader.GetString(emailTemplateToIndex);
                    template.EmailTemplateCc = reader.IsDBNull(emailTemplateCcIndex) ? null : reader.GetString(emailTemplateCcIndex);
                    template.Subject = reader.GetString(emailTemplateSubjectIndex);
                    template.Body = reader.GetString(emailTemplateBodyIndex);
                    templates.Add(template);
                }
            }
        }

        
    }

    public class EmailRecepients
    {
        public List<string> ToPersonAddresses { get; set; }
        public List<string> CcPersonAddresses { get; set; }

        public Dictionary<string, string> Parameters { get; set; }

        public EmailRecepients()
        {
            ToPersonAddresses = new List<string>();
            CcPersonAddresses = new List<string>();
            Parameters = new Dictionary<string, string>();
        }
    }
}

