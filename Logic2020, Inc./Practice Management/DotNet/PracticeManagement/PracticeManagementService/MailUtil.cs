using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web;
using DataAccess;
using DataTransferObjects;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Threading;

namespace PracticeManagementService
{
    internal static class MailUtil
    {
        #region Methods

        private static string PersonLastNameFormat = "{0}'s";

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

        private static bool IsDemo
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["Environment"] == "DEMO";
                }
                catch
                {
                    return false;
                }
            }
        }

        private static bool IsMailsEnable
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["IsMailsEnable"] == "1";
                }
                catch
                {
                    return false;
                }
            }
        }

        private static string UATTestingMail
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["UATTestingMail"];
                }
                catch
                {
                    return "";
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
            Email(emailTemplate.Subject, body, true, person.Alias, string.Empty, null, false, string.Format("{0} {1}", person.FirstName, person.LastName));
        }

        internal static void SendForgotPasswordNotification(string username, string password, EmailTemplate emailTemplate, string PMLoginPageUrl, string PMChangePasswordPageUrl)
        {
            PMChangePasswordPageUrl = string.Format(PMChangePasswordPageUrl, username, HttpUtility.UrlEncode(password));
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

        internal static void SendActivateAccountEmail(string firstName, string lastName, string startDate, string emailAddress, string title, string payType, string telephoneNumber,string isOffshore,string manager,string division)
        {
            var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName(Resources.Messages.ActivateAccountTemplateName);
            var body = string.Format(emailTemplate.Body, firstName, lastName, startDate, emailAddress, title, payType, telephoneNumber,isOffshore,manager,division);
            Email(emailTemplate.Subject, body, true, emailTemplate.EmailTemplateTo, string.Empty, null);
        }

        internal static void SendClientAddedEmail(string currentPerson, string startDate, string clientName, string isHouseAccount, string salesperson, string director)
        {
            var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName(Resources.Messages.ClientAddedTemplateName);
            var subject = string.Format(emailTemplate.Subject, clientName);
            var body = string.Format(emailTemplate.Body, currentPerson, startDate, clientName, isHouseAccount, salesperson, director);
            Email(subject, body, true, emailTemplate.EmailTemplateTo, string.Empty, null);
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

        //internal static void SendCompensationChangeEmail(string firstName,string lastName, string currentBasis, string newBasis, string effectiveDate)
        //{
        //    var emailTempalte = EmailTemplateDAL.EmailTemplateGetByName(Resources.Messages.CompensationChangeEmailTemplate);
        //    var body = string.Format(emailTempalte.Body, firstName, lastName, currentBasis, newBasis, effectiveDate);
        //    Email(emailTempalte.Subject, body, true, emailTempalte.EmailTemplateTo, string.Empty, null);
        //}

        //internal static void SendCompensationChangeRehireEmail(string firstName, string lastName,string effectiveDate, string email, string title, string phoneNumber, bool offshore, string manager, string division, string currentBasis, string newBasis )
        //{
        //    var emailTempalte = EmailTemplateDAL.EmailTemplateGetByName(Resources.Messages.CompensationChange_rehire_TemplateName);
        //    var body = string.Format(emailTempalte.Body, firstName, lastName, effectiveDate,email, title, phoneNumber, offshore?"Yes":"No", manager, division, currentBasis, newBasis);
        //    Email(emailTempalte.Subject, body, true, emailTempalte.EmailTemplateTo, string.Empty, null);
        //}

        internal static void SendMSBadgeRequestEmail(Project project,int milestoneId)
        {
            string url = IsUAT ? string.Format("http://65.52.17.100/MilestoneDetail.aspx?id={0}&projectId={1}" ,milestoneId, project.Id.Value): string.Format("https://practice.logic2020.com/MilestoneDetail.aspx?id={0}&projectId={1}" ,milestoneId, project.Id.Value);
            var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName(Resources.Messages.MSBadgeRequestTemplateName);
            var body = string.Format(project.MailBody, url);
            Email(emailTemplate.Subject, body, true, emailTemplate.EmailTemplateTo, string.Empty, null);
        }

        internal static void SendMSBadgeRequestApprovedEmail(string personName, string toAddress)
        {
            var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName(Resources.Messages.MSBadgeRequestApprovedTemplate);
            var body = string.Format(emailTemplate.Body, personName);
            Email(emailTemplate.Subject, body, true, toAddress, string.Empty, null);
        }

        internal static void SendProjectFeedbackInitialMailNotification(ProjectFeedbackMail feedback)
        {
            string url = IsUAT ? "http://65.52.17.100/ProjectDetail.aspx?id=" + feedback.Project.Id.Value.ToString() : "https://practice.logic2020.com/ProjectDetail.aspx?id=" + feedback.Project.Id.Value.ToString();
            var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName(Resources.Messages.IntialProjectFeedbackNotification);
            var subject = String.Format(emailTemplate.Subject, feedback.Resources[0].DueDate.ToString(Constants.Formatting.EntryDateFormat), feedback.Resources[0].Person.PersonFirstLastName, feedback.Project.ProjectNumber, feedback.Project.Name);
            var emailBody = String.Format(emailTemplate.Body, feedback.Resources[0].Person.PersonFirstLastName, feedback.Project.ProjectNumber, feedback.Resources[0].DueDate.ToString(Constants.Formatting.EntryDateFormat), feedback.Resources[0].Person.Title.TitleName, feedback.Resources[0].ReviewStartDate.ToString(Constants.Formatting.EntryDateFormat), feedback.Resources[0].ReviewEndDate.ToString(Constants.Formatting.EntryDateFormat), feedback.Project.Name, url);
            var ccAddressList = "";
            if (!string.IsNullOrEmpty(feedback.ProjectOwnerAlias) && !feedback.ProjectManagersAliasList.Contains(feedback.ProjectOwnerAlias))
            {
                ccAddressList += feedback.ProjectOwnerAlias + ",";
            }
            if (!string.IsNullOrEmpty(feedback.SeniorManagerAlias) && !feedback.ProjectManagersAliasList.Contains(feedback.SeniorManagerAlias) && !ccAddressList.Contains(feedback.SeniorManagerAlias))
            {
                ccAddressList += feedback.SeniorManagerAlias + ",";
            }
            if (!string.IsNullOrEmpty(feedback.ClientDirectorAlias) && !feedback.ProjectManagersAliasList.Contains(feedback.ClientDirectorAlias) && !ccAddressList.Contains(feedback.ClientDirectorAlias))
            {
                ccAddressList += feedback.ClientDirectorAlias + ",";
            }
            Email(subject, emailBody, true, feedback.ProjectManagersAliasList.Substring(0, feedback.ProjectManagersAliasList.Length - 1), string.Empty, null, false, string.Empty, ccAddressList == "" ? "" : ccAddressList.Substring(0, ccAddressList.Length - 1));
        }

        internal static void SendReviewCanceledMailNotification(List<ProjectFeedbackMail> feedbacks)
        {
            int count = 0;
            foreach(var feedback in feedbacks)
            {
                count++;
                string url = IsUAT ? "http://65.52.17.100/ProjectDetail.aspx?id=" + feedback.Project.Id.Value.ToString() : "https://practice.logic2020.com/ProjectDetail.aspx?id=" + feedback.Project.Id.Value.ToString();
                var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName(Resources.Messages.ProjectReviewCanceledNotification);
                var subject = String.Format(emailTemplate.Subject, feedback.Resources[0].Person.PersonFirstLastName, feedback.Project.ProjectNumber, feedback.Project.Name);
                var emailBody = String.Format(emailTemplate.Body, string.Format(PersonLastNameFormat, feedback.Resources[0].Person.LastName), feedback.Project.ProjectNumber, feedback.Project.Name, url);
                var ccAddressList = "";
                if (!string.IsNullOrEmpty(feedback.ProjectOwnerAlias) && !feedback.ProjectManagersAliasList.Contains(feedback.ProjectOwnerAlias))
                {
                    ccAddressList += feedback.ProjectOwnerAlias + ",";
                }
                if (!string.IsNullOrEmpty(feedback.SeniorManagerAlias) && !feedback.ProjectManagersAliasList.Contains(feedback.SeniorManagerAlias) && !ccAddressList.Contains(feedback.SeniorManagerAlias))
                {
                    ccAddressList += feedback.SeniorManagerAlias + ",";
                }
                if (!string.IsNullOrEmpty(feedback.ClientDirectorAlias) && !feedback.ProjectManagersAliasList.Contains(feedback.ClientDirectorAlias) && !ccAddressList.Contains(feedback.ClientDirectorAlias))
                {
                    ccAddressList += feedback.ClientDirectorAlias + ",";
                }
                Email(subject, emailBody, true, feedback.ProjectManagersAliasList.Substring(0, feedback.ProjectManagersAliasList.Length - 1), string.Empty, null, false, string.Empty, ccAddressList == "" ? "" : ccAddressList.Substring(0, ccAddressList.Length - 1));
                if (count % 13 == 0)
                {
                    Thread.Sleep(60 * 1000);
                }
            }
        }

        internal static void SendCohortAssignmentChangeEmail(string personName, string oldCohort, string newCohort)
        {
            var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName(Resources.Messages.CohortAssignmentChangeTemplateName);
            var body = string.Format(emailTemplate.Body, personName, oldCohort, newCohort);
            Email(emailTemplate.Subject, body, true, emailTemplate.EmailTemplateTo, string.Empty, null);
        }

        internal static void SendLockedOutNotificationEmail(string userName, string loginPageUrl)
        {
            var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName(Resources.Messages.LockedOutEmailTemplateName);
            var companyName = ConfigurationDAL.GetCompanyName();
            var lockedOutMinitues = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.UnlockUserMinituesKey);
            string output;
            try
            {
                int mins = Convert.ToInt32(lockedOutMinitues);
                output = lockedOutMinitues + " minute(s)";
                if (mins > 59)
                {
                    output = (mins / 60) + " hour(s)";
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

        internal static void SendResourceExceptionReportsEmail(DateTime startDate, DateTime endDate, byte[] attachmentByteArray)
        {
            MemoryStream attachmentStream = new MemoryStream(attachmentByteArray);
            var attachment = new Attachment(attachmentStream, string.Format("ExceptionReporting_{0}_{1}.xls", startDate.ToString(Constants.Formatting.EntryDateFormat), endDate.ToString(Constants.Formatting.EntryDateFormat)), "application/vnd.ms-excel");
            var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName(Resources.Messages.ResourceExceptionReportsTemplateName);
            var subject = string.Format(emailTemplate.Subject, startDate.ToString(Constants.Formatting.EntryDateFormat), endDate.ToString(Constants.Formatting.EntryDateFormat));
            var body = string.Format(emailTemplate.Body, startDate.ToString(Constants.Formatting.EntryDateFormat), endDate.ToString(Constants.Formatting.EntryDateFormat));
            Email(subject, body, true, emailTemplate.EmailTemplateTo, string.Empty, new List<Attachment>() { attachment });
        }

        internal static void SendRecruitingMetricsReportEmail(DateTime startDate, DateTime endDate, byte[] attachmentByteArray)
        {
            MemoryStream attachmentStream = new MemoryStream(attachmentByteArray);
            var attachment = new Attachment(attachmentStream, string.Format("RecruitingMetrics_{0}_and_{1}.xls", startDate.ToString(Constants.Formatting.EntryDateFormat), endDate.ToString(Constants.Formatting.EntryDateFormat)), "application/vnd.ms-excel");
            var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName(Resources.Messages.RecruitingMetricsReportTemplateName);
            var subject = string.Format(emailTemplate.Subject, startDate.ToString(Constants.Formatting.EntryDateFormat), endDate.ToString(Constants.Formatting.EntryDateFormat));
            var body = string.Format(emailTemplate.Body, startDate.ToString(Constants.Formatting.EntryDateFormat), endDate.ToString(Constants.Formatting.EntryDateFormat));
            Email(subject, body, true, emailTemplate.EmailTemplateTo, string.Empty, new List<Attachment>() { attachment });
        }

        public static SmtpClient GetSmtpClient(SMTPSettings smtpSettings = null)
        {
            if (smtpSettings == null)
            {
                smtpSettings = SettingsHelper.GetSMTPSettings();
            }

            SmtpClient client = new SmtpClient(smtpSettings.MailServer, smtpSettings.PortNumber)
                {
                    EnableSsl = smtpSettings.SSLEnabled,
                    Credentials = new NetworkCredential(smtpSettings.UserName, smtpSettings.Password)
                };

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
            if (!IsMailsEnable && IsUAT)
                return;
            var smtpSettings = SettingsHelper.GetSMTPSettings();

            MailMessage message = new MailMessage { Priority = isHighPriority ? MailPriority.High : MailPriority.Normal };
            var addresses = commaSeperatedToAddresses.Split(',');
            string[] addressesDisplayName = !string.IsNullOrEmpty(commaSeperatedToAddressesDisplayNames) ? commaSeperatedToAddressesDisplayNames.Split(',') : addresses;
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
                if (IsUAT)
                {
                    message.Subject = "(UAT) " + subject;
                    message.To.Clear();
                    message.CC.Clear();
                    message.Bcc.Clear();
                    var uatToAddresses = UATTestingMail.Split(',');
                    for (int i = 0; i < uatToAddresses.Length; i++)
                    {
                        message.To.Add(new MailAddress(uatToAddresses[i]));
                    }
                }
                else if (IsDemo)
                {
                    message.Subject = "(Demo) " + subject;
                }
                else
                {
                    message.Subject = subject;
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

            SmtpClient client = GetSmtpClient(smtpSettings);

            message.From = new MailAddress(smtpSettings.PMSupportEmail);
            client.Send(message);
        }

        #endregion Methods
    }
}

