using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using DataAccess;
using DataTransferObjects;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Web;
using System.Configuration;

namespace PracticeManagementService
{
    internal static class MailUtil
    {
        #region Methods

        private static bool IsAzureWebRole()
        {
            try
            {
                return RoleEnvironment.IsAvailable;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsUAT
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["Environment"] == "UAT";
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Sends a notification to user with his/her credentials.
        /// </summary>
        /// <param name="person"></param>
        /// <param name="password"></param>
        /// <param name="template"></param>
        internal static void SendResetPasswordNotification(Person person, string password)
        {
            var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName(Resources.Messages.ResetPasswordEmailTemplateName);
            DateTime now = DateTime.Now;
            string loginUrl = "http://practice.logic2020.com/";
            if (IsUAT)
            {
                loginUrl = "http://65.52.17.100/Login.aspx";
            }

            var body = string.Format(emailTemplate.Body, person.FirstName, person.LastName, person.Alias, password, now.ToLongDateString(), now.ToShortTimeString(), loginUrl);
            Email(emailTemplate.Subject, body, true, person.Alias, string.Empty, null, false, string.Format("{0} {0}", person.FirstName, person.LastName));
        }

        internal static void SendForgotPasswordNotification(string username, string password, EmailTemplate emailTemplate, string PMLoginPageUrl, string PMChangePasswordPageUrl)
        {
            PMChangePasswordPageUrl = string.Format(PMChangePasswordPageUrl, username, System.Web.HttpUtility.UrlEncode(password));
            var body = string.Format(emailTemplate.Body, PMLoginPageUrl, PMChangePasswordPageUrl, password);
            Email(emailTemplate.Subject, body, true, username, string.Empty, null);
        }

        internal static void SendWelcomeEmail(string firstName, string username, string password, string companyName, string loginPageUrl)
        {
            var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName(Resources.Messages.WelcomeEmailTemplateName);
            var smtpSettings = SettingsHelper.GetSMTPSettings();
            var body = string.Format(emailTemplate.Body, firstName, companyName, username, password, loginPageUrl, smtpSettings.PMSupportEmail);
            Email(emailTemplate.Subject, body, true, username, string.Empty, null);
        }

        internal static void SendAdministratorAddedEmail(string firstName, string lastName)
        {
            var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName(Resources.Messages.AdministratorAddedTemplateName);
            Email(emailTemplate.Subject, string.Format(emailTemplate.Body, firstName, lastName), true, emailTemplate.EmailTemplateTo, string.Empty, null, true);
        }

        internal static void SendActivateAccountEmail(string firstName, string lastName, string startDate, string emailAddress, string title, string payType, string telephoneNumber)
        {
            var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName(Resources.Messages.ActivateAccountTemplateName);
            var body = string.Format(emailTemplate.Body, firstName, lastName, startDate, emailAddress, title, payType, telephoneNumber);
            Email(emailTemplate.Subject, body, true, emailTemplate.EmailTemplateTo, string.Empty, null);
        }

        internal static void SendHireDateChangedEmail(string firstName, string lastName, string oldHireDate, string newHireDate, string emailAddress, string title, string payType, string telephoneNumber)
        {
            var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName(Resources.Messages.HireDateChangedTemplateName);
            var body = string.Format(emailTemplate.Body, firstName, lastName, oldHireDate, newHireDate, emailAddress, title, payType, telephoneNumber);
            Email(emailTemplate.Subject, body, true, emailTemplate.EmailTemplateTo, string.Empty, null);
        }

        internal static void SendDeactivateAccountEmail(string firstName, string lastName, string date)
        {
            var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName(Resources.Messages.DeActivateAccountTemplateName);
            var body = string.Format(emailTemplate.Body, firstName, lastName, date);
            Email(emailTemplate.Subject, body, true, emailTemplate.EmailTemplateTo, string.Empty, null);
        }

        internal static void SendLockedOutNotificationEmail(string userName, string loginPageUrl)
        {
            var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName(Resources.Messages.LockedOutEmailTemplateName);
            var companyName = ConfigurationDAL.GetCompanyName();
            var lockedOutMinitues = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.UnlockUserMinituesKey);
            string output = string.Empty;
            int mins = 30;
            try
            {
                mins = Convert.ToInt32(lockedOutMinitues);
                output = lockedOutMinitues + " minute(s)";
                if (mins > 59)
                {
                    output = (mins / 60).ToString() + " hour(s)";
                }
                if (mins == 1440)
                {
                    output = "1 day";
                }
            }
            catch
            {
                output = "30 minute(s)";
            }
            var body = string.Format(emailTemplate.Body, output);
            Email(string.Format(emailTemplate.Subject, companyName), body, true, userName, string.Empty, null, false, null, emailTemplate.EmailTemplateCc);
        }

        public static bool VerifySMTPSettings(string mailServer, int portNumber, bool sSLEnabled, string userName, string password, string pMSupportEmail)
        {
            var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName(Resources.Messages.TestSettingsEmailTemplateName);
            Email(emailTemplate.Subject, emailTemplate.Body, true, pMSupportEmail, string.Empty, null);
            return true;
        }

        public static SmtpClient GetSmtpClient(SMTPSettings smtpSettings = null)
        {
            if (smtpSettings == null)
            {
                smtpSettings = SettingsHelper.GetSMTPSettings();
            }

            SmtpClient client = new SmtpClient(smtpSettings.MailServer, smtpSettings.PortNumber);
            client.EnableSsl = smtpSettings.SSLEnabled;
            client.Credentials = new NetworkCredential(smtpSettings.UserName, smtpSettings.Password);

            return client;
        }

        /// <summary>
        /// This will send an email from PMSupportEmail address with the given subject, body, isbodyHtml, comma seperated To addresses, Comma seperated Bcc Addresses and Any attachments.
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isBodyHtml"></param>
        /// <param name="commaSeperatedToAddresses"></param>
        /// <param name="commaSeperatedBccAddresses"></param>
        /// <param name="attachments"></param>
        public static void Email(string subject, string body, bool isBodyHtml, string commaSeperatedToAddresses, string commaSeperatedBccAddresses, List<Attachment> attachments, bool isHighPriority = false, string commaSeperatedToAddressesDisplayNames = "", string commaSeperatedCCAddresses = "")
        {
            var smtpSettings = SettingsHelper.GetSMTPSettings();

            MailMessage message = new MailMessage();
            message.Priority = isHighPriority ? MailPriority.High : MailPriority.Normal;
            var addresses = commaSeperatedToAddresses.Split(',');
            string[] addressesDisplayName = null;
            addressesDisplayName = !string.IsNullOrEmpty(commaSeperatedToAddressesDisplayNames) ? commaSeperatedToAddressesDisplayNames.Split(',') : addresses;
            addressesDisplayName = !string.IsNullOrEmpty(commaSeperatedToAddressesDisplayNames) && addressesDisplayName.Length == addresses.Length ? addressesDisplayName : addresses;

            for (int i = 0; i < addresses.Length; i++)
            {
                message.To.Add(new MailAddress(addresses[i], addressesDisplayName[i]));
            }

            if (!string.IsNullOrEmpty(commaSeperatedBccAddresses))
            {
                var bccAddresses = commaSeperatedBccAddresses.Split(',');
                foreach (var address in bccAddresses)
                {
                    message.Bcc.Add(new MailAddress(address));
                }
            }

            if (!string.IsNullOrEmpty(commaSeperatedCCAddresses))
            {
                var ccAddresses = commaSeperatedCCAddresses.Split(',');
                foreach (var address in ccAddresses)
                {
                    message.CC.Add(new MailAddress(address));
                }
            }
            try
            {
                if (!IsUAT)
                {
                    message.Subject = subject;
                }
                else
                {
                    message.Subject = "(UAT) " + subject;
                }
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, message.Subject, "MailUtils", string.Empty,
                   HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
            }

            message.Body = body;

            message.IsBodyHtml = isBodyHtml;

            if (attachments != null && attachments.Count > 0)
            {
                foreach (var item in attachments)
                {
                    message.Attachments.Add(item);
                }
            }

            SmtpClient client = MailUtil.GetSmtpClient(smtpSettings);

            message.From = new MailAddress(smtpSettings.PMSupportEmail);
            client.Send(message);
        }

        #endregion
    }
}

