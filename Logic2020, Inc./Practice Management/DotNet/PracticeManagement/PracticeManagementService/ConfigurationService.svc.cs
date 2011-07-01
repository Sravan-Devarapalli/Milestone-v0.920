using System;
using System.Collections.Generic;
using DataTransferObjects;
using DataAccess;
using DataTransferObjects.ContextObjects;
using System.Net.Mail;

namespace PracticeManagementService
{
    [System.ServiceModel.Activation.AspNetCompatibilityRequirements(RequirementsMode = System.ServiceModel.Activation.AspNetCompatibilityRequirementsMode.Allowed)]
    public class ConfigurationService : IConfigurationService
    {
        #region Constants

        const string ErrorLogMessage = @"<Error><NEW_VALUES	Login = ""{0}"" SourcePage = ""{1}"" SourceQuery = ""{2}""  ExcMsg=""{3}"" ExcSrc=""{4}"" InnerExcMsg=""{5}"" InnerExcSrc=""{6}""><OLD_VALUES /></NEW_VALUES></Error>";

        #endregion  Constants

        #region "IConfigurationService Members"

        public List<EmailTemplate> GetAllEmailTemplates()
        {
            return EmailTemplateDAL.GetAllEmailTemplates();
        }

        public  EmailTemplate EmailTemplateGetByName(string emailTemplateName)
        {
            return EmailTemplateDAL.EmailTemplateGetByName(emailTemplateName);
        }

        public bool UpdateEmailTemplate(EmailTemplate template)
        {
            return EmailTemplateDAL.UpdateEmailTemplate(template);
        }

        public bool AddEmailTemplate(EmailTemplate template)
        {
            return EmailTemplateDAL.AddEmailTemplate(template);
        }

        public bool DeleteEmailTemplate(int templateId)
        {
            return EmailTemplateDAL.DeleteEmailTemplate(templateId);
        }

        public void CheckProjectedProjects(int templateId, string companyName)
        {
            try
            {
                var projects = new Dictionary<int, EmailRecepients>();
                string body, subject = string.Empty;
                EmailTemplateDAL.CheckProjectedProjects(templateId, projects, out body, out subject);
                SendEmails(projects, body, subject, companyName);
            }
            catch (Exception e)
            {
                string logData = string.Format(ErrorLogMessage, "CheckProjectedProjects", "ConfigurationService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
            }

        }

        public EmailData GetEmailData(EmailContext emailContext)
        {
            return EmailTemplateDAL.GetEmailData(emailContext);
        }

        public void CheckProjectedProjectsByHireDate(int templateId, string companyName)
        {
            try
            {
                var projects = new Dictionary<int, EmailRecepients>();
                string body, subject = string.Empty;
                EmailTemplateDAL.CheckProjectedProjectsByHireDate(templateId, projects, out body, out subject);
                SendEmails(projects, body, subject, companyName);
            }
            catch (Exception e)
            {
                string logData = string.Format(ErrorLogMessage, "CheckProjectedProjectsByHireDate", "ConfigurationService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
            }
        }

        public CompanyLogo GetCompanyLogoData()
        {
            return ConfigurationDAL.GetCompanyLogoData();
        }

        public void SaveCompanyLogoData(string title, string imagename, string imagePath, byte[] data)
        {
            ConfigurationDAL.SaveCompanyLogoData(title, imagename, imagePath, data);
        }

        #endregion "IConfigurationService Members" 

        #region "Email related functions"

        private void SendEmails(Dictionary<int, EmailRecepients> projects, string body, string subject, string companyName)
        {
            if (!string.IsNullOrEmpty(body) && !string.IsNullOrEmpty(subject))
            {
                var provider = (PracticeManagement.IEMailDeliveryProvider)Activator.CreateInstance(Type.GetType(System.Configuration.ConfigurationManager.AppSettings["EmailDeliveryProviderType"]));
                foreach (KeyValuePair<int, EmailRecepients> project in projects)
                {
                    string resSubject = subject.Replace("<CompanyName>", companyName);
                    string resBody = body;
                    foreach (KeyValuePair<string, string> param in project.Value.Parameters)
                    {
                        resSubject = resSubject.Replace(string.Format("<{0}>", param.Key), param.Value);
                        resBody = resBody.Replace(string.Format("<{0}>", param.Key), param.Value);
                    }

                    provider.SendMail(project.Value.ToPersonAddresses,
                                        project.Value.CcPersonAddresses,
                                        System.Configuration.ConfigurationManager.AppSettings["EmailAddressFrom"],
                                        resSubject,
                                        resBody);
                }
            }
        }

        #endregion "Email related functions"

        public void SaveResourceKeyValuePairs(SettingsType settingType, Dictionary<string, string> dictionary)
        {
            ConfigurationDAL.SaveResourceKeyValuePairs(settingType, dictionary);
        }

        public bool SaveResourceKeyValuePairItem(SettingsType settingType, string key, string value)
        {
            SettingsHelper.SaveResourceValueToCache(settingType, key, value);
            return ConfigurationDAL.SaveResourceKeyValuePairItem(settingType, key, value);
        }

        public Dictionary<string, string> GetResourceKeyValuePairs(SettingsType settingType)
        {
            return ConfigurationDAL.GetResourceKeyValuePairs(settingType);
        }

        public bool VerifySMTPSettings(string mailServer, int portNumber, bool sSLEnabled, string userName, string password, string pMSupportEmail)
        {
            try
            {
                return MailUtil.VerifySMTPSettings(mailServer, portNumber, sSLEnabled, userName, password, pMSupportEmail);
            }
            catch (SmtpException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void SaveMarginInfoDetail(List<Triple<DefaultGoalType, Triple<SettingsType, string, string>, List<ClientMarginColorInfo>>> marginInfoList)
        {
            ConfigurationDAL.SaveMarginInfoDetail(marginInfoList);
        }

        public List<ClientMarginColorInfo> GetMarginColorInfoDefaults(DefaultGoalType goalType)
        {
            return ConfigurationDAL.GetMarginColorInfoDefaults(goalType);
        }
    }
}

