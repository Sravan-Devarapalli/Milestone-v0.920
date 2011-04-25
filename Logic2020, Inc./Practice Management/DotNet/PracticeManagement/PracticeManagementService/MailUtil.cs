using System;
using System.Net.Mail;
using System.Configuration;
using DataTransferObjects;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Net;
using DataAccess;

namespace PracticeManagementService
{
    internal static class MailUtil
    {
        #region Constants

        internal const string NewUserEmailTemplateKey = "NewUserEmailTemplate";
        internal const string ModifiedUserEmailTemplateKey = "ModifiedUserEmailTemplate";
        internal const string ResetPasswordEmailTemplateKey = "ResetPasswordEmailTemplate";
        private const string TestSettingsEmailTemplateKey = "TestSettingsEmailTemplate";

        #endregion

        #region Methods

        /// <summary>
        /// Sends a notification to user with his/her credentials.
        /// </summary>
        /// <param name="person"></param>
        /// <param name="password"></param>
        /// <param name="template"></param>
        internal static void SendNotification(Person person, string password, string template)
        {
            MailMessage message = new MailMessage();
            DateTime now = DateTime.Now;
            message.To.Add(new MailAddress(person.Alias, string.Format("{0} {0}", person.FirstName, person.LastName)));
            message.Subject = "Practice Management - User Account";
            message.Body =
                string.Format(template, person.FirstName, person.LastName, person.Alias, password, now.ToLongDateString(), now.ToShortTimeString());
            message.IsBodyHtml = true;

            SmtpClient client = MailUtil.GetSmtpClient();
            var smtpSettings = SettingsHelper.GetSMTPSettings();
            message.From = new MailAddress(smtpSettings.PMSupportEmail);
            client.Send(message);
        }

        internal static void SendForgotPasswordNotification(string username, string password, EmailTemplate template,
                                                            string PMLoginPageUrl, string PMChangePasswordPageUrl)
        {
            PMChangePasswordPageUrl = string.Format(PMChangePasswordPageUrl, username,
                System.Web.HttpUtility.UrlEncode(password));
            MailMessage message = new MailMessage();
            DateTime now = DateTime.Now;
            message.To.Add(new MailAddress(username));
            message.Subject = template.Subject;
            message.Body =
                string.Format(template.Body, PMLoginPageUrl, PMChangePasswordPageUrl, password);
            message.IsBodyHtml = true;

            SmtpClient client = MailUtil.GetSmtpClient();

            var smtpSettings = SettingsHelper.GetSMTPSettings();
            message.From = new MailAddress(smtpSettings.PMSupportEmail);

            client.Send(message);
        }

        private static bool IsSSLEnabled()
        {
            bool enableSSL = false;

            try
            {
                if (IsAzureWebRole())
                {
                    if (Boolean.TryParse(RoleEnvironment.GetConfigurationSettingValue("EnableSSL"), out enableSSL))
                    {
                        return enableSSL;
                    }
                    return enableSSL;
                }
                else
                {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings.Get("EnableSSL"));
                }
            }
            catch
            {
                return enableSSL;
            }
        }

        private static Boolean IsAzureWebRole()
        {
            try
            {
                if (RoleEnvironment.IsAvailable)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool VerifySMTPSettings(string mailServer, int portNumber, bool sSLEnabled, string userName, string password, string pMSupportEmail)
        {
            var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName(TestSettingsEmailTemplateKey);

            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress(pMSupportEmail));
            message.Subject = emailTemplate.Subject;
            message.Body = emailTemplate.Body;
            message.IsBodyHtml = true;

            SmtpClient client = new SmtpClient(mailServer, portNumber);
            client.EnableSsl = sSLEnabled;
            client.Credentials = new NetworkCredential(userName, password);
            message.From = new MailAddress(userName);
            client.Send(message);

            return true;
        }

        public static SmtpClient GetSmtpClient()
        {
            var smtpSettings = SettingsHelper.GetSMTPSettings();

            SmtpClient client = new SmtpClient(smtpSettings.MailServer, smtpSettings.PortNumber);
            client.EnableSsl = smtpSettings.SSLEnabled;
            client.Credentials = new NetworkCredential(smtpSettings.UserName, smtpSettings.Password);

            return client;
        }

        #endregion
    }
}

