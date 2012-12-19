using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DataAccess.Other;
using DataTransferObjects;
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

        #endregion
    }
}

