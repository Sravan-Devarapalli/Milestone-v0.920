using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System.Timers;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using DataTransferObjects;

namespace UpdatePracticeAndSeniority
{
    public class WorkerRole : RoleEntryPoint
    {
        #region Constants

        public const string applicationSettingsType = "Application";
        public const string timeZoneKey = "TimeZone";
        public const string IsDayLightSavingsKey = "IsDayLightSavingsTimeEffect";
        public const string UpdatedProfileLinkFormat = "<a href='{0}?Id={1}'>{2}</a><br/>";
        public const string UPDATED_PROFILES_LIST_EMAIL_RECIEVER = "UpdatedProfilesListEmailReciever";
        public const string EmailSubject_For_ProfilesUpdatedList = "EmailSubjectForProfilesUpdatedList";
        public const string Skills_Profile_PagePath = "SkillsProfilePagePath";

        #endregion

        #region Properties

        public static string UpdatedProfilesListEmailReciever
        {
            get
            {
                //Skills@logic2020.com
                return WorkerRole.GetConfigValue(UPDATED_PROFILES_LIST_EMAIL_RECIEVER) ?? "Skills@logic2020.com";
            }
        }

        public static string EmailSubjectForProfilesUpdatedList
        {
            get
            {
                return WorkerRole.GetConfigValue(EmailSubject_For_ProfilesUpdatedList) ?? "Practice Management: List of consultants who were updated their profiles";
            }
        }

        public static string SkillsProfilePagePath
        {
            get
            {
                return WorkerRole.GetConfigValue(Skills_Profile_PagePath) ?? "https://practice.logic2020.com/SkillsProfile.aspx";
            }
        }

        #endregion

        #region Override Methods

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }

        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            DateTime currentDateTimeWithTimeZone, nextRun;
            while (true)
            {
                Thread.Sleep(5 * 60 * 1000);

                currentDateTimeWithTimeZone = AddTimeZone(DateTime.UtcNow);
                nextRun = GetNextRunDate(currentDateTimeWithTimeZone);
                var sleepTimeSpan = nextRun > currentDateTimeWithTimeZone ?
                    nextRun - currentDateTimeWithTimeZone :
                    currentDateTimeWithTimeZone - nextRun;
                Thread.Sleep(sleepTimeSpan);
                var currentDateTimeWithTimeZoneAfterSleep = currentDateTimeWithTimeZone.Add(sleepTimeSpan);
                RunTasks(currentDateTimeWithTimeZoneAfterSleep);
            }
        }

        #endregion

        #region Private Methods

        public static DateTime GetNextRunDate(DateTime currentDateTimeWithTimeZone)
        {
            DateTime nextRun;
            TimeSpan ScheduledTime = TimeSpan.Parse(GetConfigValue("RunSchedularDailyAtTime"));
            if (ScheduledTime > currentDateTimeWithTimeZone.TimeOfDay)
            {
                nextRun = new DateTime(currentDateTimeWithTimeZone.Year, currentDateTimeWithTimeZone.Month, currentDateTimeWithTimeZone.Day,
                                        ScheduledTime.Hours, ScheduledTime.Minutes, ScheduledTime.Seconds);
            }
            else
            {
                var nextDay = currentDateTimeWithTimeZone.AddDays(1);
                nextRun = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day,
                                          ScheduledTime.Hours, ScheduledTime.Minutes, ScheduledTime.Seconds);
            }
            return nextRun;
        }

        public static DateTime AddTimeZone(DateTime date)
        {
            DateTime dateTimeWithTimeZone;
            string timezone = GetResourceValueByKeyAndType(timeZoneKey, applicationSettingsType);
            string isDayLightSavingsTimeEffect = GetResourceValueByKeyAndType(IsDayLightSavingsKey, applicationSettingsType);
            if (timezone == "-08:00" && isDayLightSavingsTimeEffect.ToLower() == "true")
            {
                dateTimeWithTimeZone = TimeZoneInfo.ConvertTime(date, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
            }
            else
            {
                var timezoneWithoutSign = timezone.Replace("+", string.Empty);
                TimeZoneInfo ctz = TimeZoneInfo.CreateCustomTimeZone("cid", TimeSpan.Parse(timezoneWithoutSign), "customzone", "customzone");
                dateTimeWithTimeZone = TimeZoneInfo.ConvertTime(date, ctz);
            }

            return dateTimeWithTimeZone;

        }

        public static void RunTasks(DateTime currentWithTimeZone)
        {
            var nextRun = GetNextRunDate(currentWithTimeZone);
            WorkerRole.StartAutomaticUpdation(currentWithTimeZone, nextRun);
            WorkerRole.EmailUpdatedProfilesList(currentWithTimeZone, nextRun);
        }

        public static void StartAutomaticUpdation(DateTime currentWithTimeZone, DateTime nextRun)
        {
            SqlConnection connection = null;
            try
            {
                var connectionString = WorkerRole.GetConnectionString();

                if (string.IsNullOrEmpty(connectionString))
                    return;

                connection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("dbo.AutoUpdateObjects", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlParameter lastRunParam = new SqlParameter("@LastRun", currentWithTimeZone);
                cmd.Parameters.Add(lastRunParam);
                SqlParameter nextRunParam = new SqlParameter("@NextRun", nextRun);
                cmd.Parameters.Add(nextRunParam);
                int rowsAffected = cmd.ExecuteNonQuery();
                connection.Close();
                
                WorkerRole.SaveSchedularLog(currentWithTimeZone, "Success", "Successfully completed running the procedure dbo.AutoUpdateObjects", nextRun);
            }
            catch (Exception ex)
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, "Failed", "Failed running the procedure dbo.AutoUpdateObjects Details are: " + ex.Message, nextRun);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public static void SaveSchedularLog(DateTime lastRun, string status, string errorMessage, DateTime nextRun)
        {
            SqlConnection connection = null;

            if (!LogEnabled())
            {
                return;
            }
            try
            {
                var connectionString = WorkerRole.GetConnectionString();

                if (string.IsNullOrEmpty(connectionString))
                    return;

                connection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("dbo.SaveSchedularLog", connection);
                SqlParameter lastRunParam = new SqlParameter("@LastRun", lastRun);
                cmd.Parameters.Add(lastRunParam);

                SqlParameter statusParam = new SqlParameter("@Status", status);
                cmd.Parameters.Add(statusParam);

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    SqlParameter errorParam = new SqlParameter("@Comments", errorMessage);
                    cmd.Parameters.Add(errorParam);
                }

                SqlParameter nextRunParam = new SqlParameter("@NextRun", nextRun);
                cmd.Parameters.Add(nextRunParam);

                cmd.CommandType = CommandType.StoredProcedure;

                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch
            {
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public static string GetConnectionString()
        {
            try
            {
                string connection = GetConfigValue("DBConnection");
                return connection;
            }
            catch
            {
                return null;
            }
        }

        public static bool LogEnabled()
        {
            bool result = false;
            bool.TryParse(GetConfigValue("LogEnabled"), out result);
            return result;
        }

        public static int GetSleepTime()
        {
            int result = 0;
            try
            {
                result = int.Parse(GetConfigValue("SleepTimeInMin"));

                result = result * 60 * 1000;

            }
            catch
            {
                result = 24 * 60 * 60 * 1000;
            }
            return result;
        }

        public static string GetConfigValue(string key)
        {
            string result = string.Empty;
            try
            {
                result = RoleEnvironment.GetConfigurationSettingValue(key);
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        }

        public static string GetResourceValueByKeyAndType(string key, string type)
        {
            SqlConnection connection = null;
            var result = string.Empty;
            object tempResult;
            try
            {
                var connectionString = WorkerRole.GetConnectionString();

                if (string.IsNullOrEmpty(connectionString))
                    return result;

                connection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("dbo.GetResourceValueByKeyAndType", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter typeParam = new SqlParameter("@SettingType", type);
                cmd.Parameters.Add(typeParam);
                SqlParameter keyParam = new SqlParameter("@Key", key);
                cmd.Parameters.Add(keyParam);
                connection.Open();
                tempResult = cmd.ExecuteScalar();
                if (tempResult != null)
                    result = tempResult.ToString();
                connection.Close();
            }
            catch (Exception ex)
            {
                WorkerRole.SaveSchedularLog(DateTime.Now, "Error",
                    "Error Occured while retrieving data from settings table with Key '"
                    + key + "' and setting type '" + type + "'. The error message is  "
                    + ex.Message, DateTime.Now);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return result;
        }

        public static string GetResourceValueByKeyAndType(SettingsType type, string key)
        {
            return GetResourceValueByKeyAndType(key, type.ToString());
        }

        public static void EmailUpdatedProfilesList(DateTime currentWithTimeZone, DateTime nextRun)
        {
            WorkerRole.SaveSchedularLog(currentWithTimeZone, "Success", "Started emailing updated profiles list.", nextRun);

            try
            {
                var list = GetUpdatedProfilesList(currentWithTimeZone);
                if (list != null && list.Count() > 0)
                {
                    string body = GetUpdatedProfilesListEmailBody(list);
                    Email(EmailSubjectForProfilesUpdatedList, body, true, UpdatedProfilesListEmailReciever);
                }
                WorkerRole.SaveSchedularLog(currentWithTimeZone, "Success", "Finished emailing updated profiles list.", nextRun);
            }
            catch(Exception ex)
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, "Failed", "Failed to send an email of updated profiles list due to: " + ex.Message , nextRun);
            }
        }

        public static void Email(string subject, string body, bool isBodyHtml, string toAddress)
        {
            var smtpSettings = GetSMTPSettings();

            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress(toAddress));
            message.Subject = subject;

            message.Body = body;

            message.IsBodyHtml = isBodyHtml;

            SmtpClient client = GetSmtpClient(smtpSettings);

            message.From = new MailAddress(smtpSettings.PMSupportEmail);
            client.Send(message);
        }

        private static string GetUpdatedProfilesListEmailBody(Dictionary<string, int> list)
        {
            string emailBody = string.Empty;
            foreach (var key in list)
            {
                emailBody = emailBody + string.Format(UpdatedProfileLinkFormat, SkillsProfilePagePath, key.Value, key.Key);
            }
            return emailBody;
        }

        private static Dictionary<string, int> GetUpdatedProfilesList(DateTime currentWithTimeZone)
        {
            var list = new Dictionary<string, int>();
            SqlConnection connection = null;
            try
            {
                var connectionString = WorkerRole.GetConnectionString();

                if (string.IsNullOrEmpty(connectionString))
                    return null;

                connection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("Skills.GetUpdatedProfilesList", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter typeParam = new SqlParameter("@Today", currentWithTimeZone.Date);
                cmd.Parameters.Add(typeParam);

                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int personIdIndex = reader.GetOrdinal("PersonId");
                        int personNameIndex = reader.GetOrdinal("PersonName");
                        while (reader.Read())
                        {
                            list.Add(reader.GetString(personNameIndex), reader.GetInt32(personIdIndex));
                        }
                    }
                }
                connection.Close();
            }
            catch(Exception ex)
            {
                connection.Close();
                throw ex;
            }
            return list;
        }

        public static SmtpClient GetSmtpClient(SMTPSettings smtpSettings)
        {
            SmtpClient client = new SmtpClient(smtpSettings.MailServer, smtpSettings.PortNumber);
            client.EnableSsl = smtpSettings.SSLEnabled;
            client.Credentials = new NetworkCredential(smtpSettings.UserName, smtpSettings.Password);

            return client;
        }
        
        public static SMTPSettings GetSMTPSettings()
        {
            var sMTPSettings = new SMTPSettings();
            sMTPSettings.MailServer = GetResourceValueByKeyAndType(SettingsType.SMTP, Constants.ResourceKeys.MailServerKey);

            var sslEnabledString = GetResourceValueByKeyAndType(SettingsType.SMTP, Constants.ResourceKeys.SSLEnabledKey);
            bool sSLEnabled;
            if (!string.IsNullOrEmpty(sslEnabledString) && bool.TryParse(sslEnabledString, out sSLEnabled))
            {
                sMTPSettings.SSLEnabled = sSLEnabled;
            }

            int portNumber;
            var portNumberString = GetResourceValueByKeyAndType(SettingsType.SMTP, Constants.ResourceKeys.PortNumberKey);
            if (!string.IsNullOrEmpty(portNumberString) && Int32.TryParse(portNumberString, out portNumber))
            {
                sMTPSettings.PortNumber = portNumber;
            }
            else
            {
                if (sMTPSettings.SSLEnabled)
                    sMTPSettings.PortNumber = 25;
                else
                    sMTPSettings.PortNumber = 465;
            }

            sMTPSettings.UserName =
                GetResourceValueByKeyAndType(SettingsType.SMTP, Constants.ResourceKeys.UserNameKey);

            sMTPSettings.Password =
                GetResourceValueByKeyAndType(SettingsType.SMTP, Constants.ResourceKeys.PasswordKey);

            sMTPSettings.PMSupportEmail =
                GetResourceValueByKeyAndType(SettingsType.SMTP, Constants.ResourceKeys.PMSupportEmailAddressKey);

            return sMTPSettings;
        }

        #endregion
    }
}

