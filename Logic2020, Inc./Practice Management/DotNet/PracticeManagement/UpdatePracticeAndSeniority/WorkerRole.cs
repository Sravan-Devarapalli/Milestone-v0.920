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
using DataAccess;
using System.Text;
using System.IO;

namespace UpdatePracticeAndSeniority
{
    public class WorkerRole : RoleEntryPoint
    {
        #region Constants

        //Settings Keys
        public const string applicationSettingsType = "Application";
        public const string timeZoneKey = "TimeZone";
        public const string IsDayLightSavingsKey = "IsDayLightSavingsTimeEffect";

        //Configuration Keys
        public const string Environment_ConfigKey = "Environment";
        public const string RunSchedularDailyAtTime_ConfigKey = "RunSchedularDailyAtTime";
        public const string UPDATED_PROFILES_LIST_EMAIL_RECIEVER_ConfigKey = "UpdatedProfilesListEmailReciever";
        public const string EmailSubject_For_ProfilesUpdatedList_ConfigKey = "EmailSubjectForProfilesUpdatedList";
        public const string Skills_Profile_PagePath_ConfigKey = "SkillsProfilePagePath";
        public const string PAYROLLDISTRIBUTIONREPORT_SCHEDULETIME_ConfigKey = "PayrollDistributionScheduleTime";
        public const string PayRollDistibutionReportReciever_ConfigKey = "PayrollDistributionReportReciever";

        //Formats
        public const string UpdatedProfileLinkFormat = "<a href='{0}?Id={1}'>{2}</a><br/>";
        public const string PayRollDistributionReportEmailSubjectFormat = "Payroll Distribution Report for the period {0}.";
        public const string PayRollDistributionReportEmailBodyFormat = "Please find the Payroll Distribution report for the period {0} in the attachments.";
        public const string AutoUpdateObjectsFailedFormat = "Failed running the procedure dbo.AutoUpdateObjects Details are: {0}";
        public const string PayrollDistributionReportFailedFormat = "Failed to send an email of Payroll Distribution Report due to: {0}";
        public const string DateFormat = "MM/dd/yyyy";
        public const string DateDotFormat = "MM.dd.yyyy";
        public const string CurrencyExcelReportFormat = "$####,###,###,###,###,##0.00";

        //Log Status
        public const string Success = "Success";
        public const string Failed = "Failed";

        //Log Messages
        public const string SchedularStarted = "Started Schedular.";
        public const string AutoUpdateObjectsRunSuccessfully = "Successfully completed running the procedure dbo.AutoUpdateObjects";
        public const string PayrollDistributionReportStartedProcess = "Started Process of emailing Payroll Distribution Report.";
        public const string PayrollDistributionReportStartedReadingData = "Started reading data Payroll Distribution Report.";
        public const string PayrollDistributionReportReadDataSuccess = "Read the Payroll Distribution Report data.";
        public const string PayrollDistributionReportStartedEmailing = "Started emailing the Payroll Distribution Report data.";
        public const string PayrollDistributionReportEmailed = "Emailed the Payroll Distribution Report.";

        //Stored Procedures
        public const string AutoUpdateObjects = "dbo.AutoUpdateObjects";

        //Parameters
        public const string NextRun = "@NextRun";
        public const string LastRun = "@LastRun";

        #endregion

        #region Properties

        public static bool IsUATEnvironment
        {
            get
            {
                var environment = GetConfigValue(Environment_ConfigKey);
                return (environment == "UAT");
            }
        }

        public static string UpdatedProfilesListEmailReciever
        {
            get
            {
                //Skills@logic2020.com
                return WorkerRole.GetConfigValue(UPDATED_PROFILES_LIST_EMAIL_RECIEVER_ConfigKey) ?? "Skills@logic2020.com";
            }
        }

        public static string EmailSubjectForProfilesUpdatedList
        {
            get
            {
                return WorkerRole.GetConfigValue(EmailSubject_For_ProfilesUpdatedList_ConfigKey) ?? "Practice Management: Added/Updated Skill profile's persons list";
            }
        }

        public static string SkillsProfilePagePath
        {
            get
            {
                return WorkerRole.GetConfigValue(Skills_Profile_PagePath_ConfigKey) ?? "https://practice.logic2020.com/SkillsProfile.aspx";
            }
        }

        public static string ConnectionString
        {
            get
            {
                return GetConnectionString();
            }
        }

        public static DateTime CurrentPMTime
        {
            get
            {
                return AddTimeZone(DateTime.UtcNow);
            }
        }

        public static TimeSpan PayrollDistributionReportScheduleTime
        {
            get
            {
                return TimeSpan.Parse(GetConfigValue(PAYROLLDISTRIBUTIONREPORT_SCHEDULETIME_ConfigKey));
            }
        }

        public static string PayrollDistributionReportReciever
        {
            get
            {
                return GetConfigValue(PayRollDistibutionReportReciever_ConfigKey);
            }
        }

        public static TimeSpan FirstSchedularTime
        {
            get
            {
                return TimeSpan.Parse(GetConfigValue(RunSchedularDailyAtTime_ConfigKey));
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
            try
            {
                // This is a sample worker implementation. Replace with your logic.
                DateTime currentDateTimeWithTimeZone, nextRun;

                Thread.Sleep(5 * 60 * 1000);
                currentDateTimeWithTimeZone = CurrentPMTime;
                WorkerRole.SaveSchedularLog(currentDateTimeWithTimeZone, Success, SchedularStarted, currentDateTimeWithTimeZone);
                
                currentDateTimeWithTimeZone = CurrentPMTime;
                //For the starting of the schedular if we update schedular binaries between 00:01:00 and 07:00:00, then we need to run Pay roll distribution report.
                if (currentDateTimeWithTimeZone.TimeOfDay > FirstSchedularTime)
                {
                    RunPayrollDistributionReport(currentDateTimeWithTimeZone);
                }

                while (true)
                {
                    Thread.Sleep(5 * 60 * 1000);

                    currentDateTimeWithTimeZone = CurrentPMTime;
                    nextRun = GetNextRunDate(currentDateTimeWithTimeZone);
                    var sleepTimeSpan = nextRun > currentDateTimeWithTimeZone ?
                        nextRun - currentDateTimeWithTimeZone :
                        currentDateTimeWithTimeZone - nextRun;
                    Thread.Sleep(sleepTimeSpan);
                    currentDateTimeWithTimeZone = currentDateTimeWithTimeZone.Add(sleepTimeSpan);
                    //Runs at 00:01:00 every day.
                    RunTasks(currentDateTimeWithTimeZone);

                    currentDateTimeWithTimeZone = CurrentPMTime;
                    //Runs at 07:00:00 on 3rd and 18th of every month.
                    RunPayrollDistributionReport(currentDateTimeWithTimeZone);
                }
            }
            catch(Exception ex)
            {
                WorkerRole.SaveSchedularLog(DateTime.UtcNow, Failed, "Schedular error: " + ex.Message, DateTime.UtcNow);
                throw ex;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gives the date and time of the next run.
        /// </summary>
        /// <param name="currentDateTimeWithTimeZone"></param>
        /// <returns></returns>
        public static DateTime GetNextRunDate(DateTime currentDateTimeWithTimeZone)
        {
            DateTime nextRun;
            TimeSpan ScheduledTime = TimeSpan.Parse(GetConfigValue(RunSchedularDailyAtTime_ConfigKey));
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

        /// <summary>
        /// Gives the date and time with current time zone for the given date and time.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Run the tasks at 00:01:00 daily
        /// </summary>
        /// <param name="currentWithTimeZone"></param>
        public static void RunTasks(DateTime currentWithTimeZone)
        {
            var nextRun = GetNextRunDate(currentWithTimeZone);
            WorkerRole.StartAutomaticUpdation(currentWithTimeZone, nextRun);
            WorkerRole.EmailUpdatedProfilesList(currentWithTimeZone, nextRun);
        }

        /// <summary>
        /// Runs Payroll Distribution Report Task as per the time reaches( Every month 3rd and 18th at 07:00:00).
        /// </summary>
        /// <param name="currentDateTimeWithTimeZone"></param>
        public static void RunPayrollDistributionReport(DateTime currentDateTimeWithTimeZone)
        {
            if ((currentDateTimeWithTimeZone.Day == 3 || currentDateTimeWithTimeZone.Day == 18) && currentDateTimeWithTimeZone.TimeOfDay < PayrollDistributionReportScheduleTime)
            {
                    var sleeptime = PayrollDistributionReportScheduleTime - currentDateTimeWithTimeZone.TimeOfDay;
                    Thread.Sleep(sleeptime);
                    SendPayrollDistributionReport(CurrentPMTime);
            }
        }

        /// <summary>
        /// Executes AutoUpdateObjects Stored Procedure.
        /// </summary>
        /// <param name="currentWithTimeZone"></param>
        /// <param name="nextRun"></param>
        public static void StartAutomaticUpdation(DateTime currentWithTimeZone, DateTime nextRun)
        {
            SqlConnection connection = null;
            try
            {
                var connectionString = WorkerRole.GetConnectionString();

                if (string.IsNullOrEmpty(connectionString))
                    return;

                connection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand(AutoUpdateObjects, connection);
                cmd.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlParameter lastRunParam = new SqlParameter(LastRun, currentWithTimeZone);
                cmd.Parameters.Add(lastRunParam);
                SqlParameter nextRunParam = new SqlParameter(NextRun, nextRun);
                cmd.Parameters.Add(nextRunParam);
                int rowsAffected = cmd.ExecuteNonQuery();
                connection.Close();
                
                WorkerRole.SaveSchedularLog(currentWithTimeZone, Success, AutoUpdateObjectsRunSuccessfully, nextRun);
            }
            catch (Exception ex)
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, Failed, string.Format(AutoUpdateObjectsFailedFormat, ex.Message), nextRun);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Sends an email of Payroll Distribution Report.
        /// </summary>
        public static void SendPayrollDistributionReport(DateTime currentDate)
        {
            var nextRunTime = currentDate.Day == 3 ? currentDate.AddDays(18 - 3) : currentDate.AddMonths(1).AddDays(3);
            WorkerRole.SaveSchedularLog(currentDate, Success, PayrollDistributionReportStartedProcess, nextRunTime);
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    DateTime startDate = currentDate.Day == 3 ? new DateTime(currentDate.Year, currentDate.Month - 1, 16) : new DateTime(currentDate.Year, currentDate.Month, 1);
                    DateTime endDate = currentDate.Day == 3 ? currentDate.Date.AddDays(-currentDate.Day) : new DateTime(currentDate.Year, currentDate.Month, 15);

                    //Read the data.
                    WorkerRole.SaveSchedularLog(currentDate, Success, PayrollDistributionReportStartedReadingData, nextRunTime);
                    var data = ReportDAL.PersonTimeEntriesDetailsSchedular(null, startDate, endDate, connection);

                    //Mail the data.
                    WorkerRole.SaveSchedularLog(currentDate, Success, PayrollDistributionReportReadDataSuccess, nextRunTime);
                    EmailPayrollDistributionReport(data, startDate, endDate, currentDate, nextRunTime);
                    WorkerRole.SaveSchedularLog(currentDate, Success, PayrollDistributionReportEmailed, nextRunTime);
                }
            }
            catch (Exception ex)
            {
                WorkerRole.SaveSchedularLog(currentDate, Failed, string.Format(PayrollDistributionReportFailedFormat, ex.Message), nextRunTime);
            }
        }

        /// <summary>
        /// Emails the Payroll Distribution Report for the given startDate and EndDate.
        /// </summary>
        /// <param name="reportdata"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        private static void EmailPayrollDistributionReport(List<DataTransferObjects.Reports.TimeEntriesGroupByClientAndProject> reportdata, DateTime startDate, DateTime endDate, DateTime currentTime, DateTime nextRunTime)
        {
            var range = startDate.Date.ToString(DateFormat) + " - " + endDate.Date.ToString(DateFormat);

            //Read the data.
            var attachmentData = PreparePayrollDistributionReportExcelData(reportdata, range);
            var fileName = string.Format("{0}_{1}_{2}.xls", "PayrollDistributionReport", startDate.Date.ToString(DateDotFormat), endDate.Date.ToString(DateDotFormat));
            var attachment = PrepareAttachment(attachmentData, fileName, "application/octat-stream");

            var attachments = new List<Attachment>();
            attachments.Add(attachment);

            //Email the data.
            WorkerRole.SaveSchedularLog(currentTime, Success, PayrollDistributionReportStartedEmailing, nextRunTime);
            var subject = string.Format(PayRollDistributionReportEmailSubjectFormat, range);
            var body = string.Format(PayRollDistributionReportEmailBodyFormat, range);
            Email(subject, body, true, PayrollDistributionReportReciever, string.Empty, attachments);
        }

        /// <summary>
        /// Gives Excel data for the given reportData and range.
        /// </summary>
        /// <param name="reportdata"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static string PreparePayrollDistributionReportExcelData(List<DataTransferObjects.Reports.TimeEntriesGroupByClientAndProject> reportdata, string range)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Payroll Distribution Report");
            sb.Append("\t");
            sb.Append(range);
            sb.Append("\t");
            sb.AppendLine();
            sb.AppendLine();
            if (reportdata.Count > 0)
            {
                //Header
                /* Account	Account Name	Business Unit	Business Unit Name	Project	Project Name	Phase
                Work Type	Work Type Name	Date	Billable Hours	Non-Billable Hours	Total Hours	Note */
                sb.Append("Employee Id");
                sb.Append("\t");
                sb.Append("Employee");
                sb.Append("\t");
                sb.Append("Pay Type");
                sb.Append("\t");
                sb.Append("Is Offshore?");
                sb.Append("\t");
                sb.Append("Account");
                sb.Append("\t");
                sb.Append("Account Name");
                sb.Append("\t");
                sb.Append("Business Unit");
                sb.Append("\t");
                sb.Append("Business Unit Name");
                sb.Append("\t");
                sb.Append("Project");
                sb.Append("\t");
                sb.Append("Project Name");
                sb.Append("\t");
                sb.Append("Status");
                sb.Append("\t");
                sb.Append("Billing");
                sb.Append("\t");
                sb.Append("Phase");
                sb.Append("\t");
                sb.Append("Work Type");
                sb.Append("\t");
                sb.Append("Work Type Name");
                sb.Append("\t");
                sb.Append("Date");
                sb.Append("\t");
                sb.Append("Billable Hours");
                sb.Append("\t");
                sb.Append("Non-Billable Hours");
                sb.Append("\t");
                sb.Append("Total Hours");
                sb.Append("\t");
                sb.Append("Bill Rate");
                sb.Append("\t");
                sb.Append("Pay Rate");
                sb.Append("\t");
                sb.Append("Note");
                sb.Append("\t");
                sb.AppendLine();

                //Data
                foreach (var timeEntriesGroupByClientAndProject in reportdata)
                {

                    foreach (var byDateList in timeEntriesGroupByClientAndProject.DayTotalHours)
                    {

                        foreach (var byWorkType in byDateList.DayTotalHoursList)
                        {
                            sb.Append(timeEntriesGroupByClientAndProject.Person.EmployeeNumber);
                            sb.Append("\t");
                            sb.Append(timeEntriesGroupByClientAndProject.Person.PersonFirstLastName);
                            sb.Append("\t");

                            sb.Append(string.IsNullOrEmpty(byWorkType.PayType) ? string.Empty : byWorkType.PayType);
                            sb.Append("\t");

                            sb.Append(timeEntriesGroupByClientAndProject.Person.IsOffshore ? "Yes" : "No");
                            sb.Append("\t");

                            sb.Append(timeEntriesGroupByClientAndProject.Client.Code);
                            sb.Append("\t");
                            sb.Append(timeEntriesGroupByClientAndProject.Client.HtmlEncodedName);
                            sb.Append("\t");

                            sb.Append(timeEntriesGroupByClientAndProject.Project.Group.Code);
                            sb.Append("\t");
                            sb.Append(timeEntriesGroupByClientAndProject.Project.Group.HtmlEncodedName);
                            sb.Append("\t");
                            sb.Append(timeEntriesGroupByClientAndProject.Project.ProjectNumber);
                            sb.Append("\t");
                            sb.Append(timeEntriesGroupByClientAndProject.Project.HtmlEncodedName);
                            sb.Append("\t");
                            sb.Append(timeEntriesGroupByClientAndProject.Project.Status.Name);
                            sb.Append("\t");
                            sb.Append(timeEntriesGroupByClientAndProject.BillableType);
                            sb.Append("\t");
                            sb.Append("01");//phase
                            sb.Append("\t");
                            sb.Append(byWorkType.TimeType.Code);
                            sb.Append("\t");
                            sb.Append(byWorkType.TimeType.Name);
                            sb.Append("\t");
                            sb.Append(byDateList.Date.ToString("MM/dd/yyyy"));
                            sb.Append("\t");
                            sb.Append(byWorkType.BillableHours);
                            sb.Append("\t");
                            sb.Append(byWorkType.NonBillableHours);
                            sb.Append("\t");
                            sb.Append(byWorkType.TotalHours);
                            sb.Append("\t");
                            sb.Append(byWorkType.HourlyRate.HasValue ? byWorkType.HourlyRate.Value.ToString(CurrencyExcelReportFormat) : string.Empty);
                            sb.Append("\t");
                            sb.Append(byWorkType.PayRate.HasValue ? byWorkType.PayRate.Value.ToString(CurrencyExcelReportFormat) : string.Empty);
                            sb.Append("\t");
                            sb.Append( " " + byWorkType.HtmlEncodedNoteForExport);
                            sb.Append("\t");
                            sb.AppendLine();
                        }
                    }
                }
            }
            else
            {
                sb.Append("This person has not entered Time Entries for the selected period.");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns attachment object if you give data, fileName and mediaType.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public static Attachment PrepareAttachment(string data, string fileName, string mediaType)
        {
            System.Text.Encoding Enc = System.Text.Encoding.ASCII;

            byte[] mBArray = Enc.GetBytes(data);

            return new Attachment(new MemoryStream(mBArray, false), fileName, "application/octat-stream");
        }

        /// <summary>
        /// Logs the message in SchedularLog Table.
        /// </summary>
        /// <param name="lastRun"></param>
        /// <param name="status"></param>
        /// <param name="errorMessage"></param>
        /// <param name="nextRun"></param>
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

        /// <summary>
        /// Gives the DB connection string.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gives the Configuration value for the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gives the settings value for the given settings key and settings type string.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gives the settings value for the given settings type enum and settings key.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetResourceValueByKeyAndType(SettingsType type, string key)
        {
            return GetResourceValueByKeyAndType(key, type.ToString());
        }

        /// <summary>
        /// This will email the list of Profiles updated/modified last day.
        /// </summary>
        /// <param name="currentWithTimeZone"></param>
        /// <param name="nextRun"></param>
        public static void EmailUpdatedProfilesList(DateTime currentWithTimeZone, DateTime nextRun)
        {
            WorkerRole.SaveSchedularLog(currentWithTimeZone, Success, "Started emailing updated profiles list.", nextRun);

            try
            {
                var list = GetUpdatedProfilesList(currentWithTimeZone);
                if (list != null && list.Count() > 0)
                {
                    string body = GetUpdatedProfilesListEmailBody(list);
                    Email(EmailSubjectForProfilesUpdatedList, body, true, UpdatedProfilesListEmailReciever, string.Empty, null);
                    WorkerRole.SaveSchedularLog(currentWithTimeZone, "Emailed", "Emailed the updated profiles list.", nextRun);
                }
                WorkerRole.SaveSchedularLog(currentWithTimeZone, Success, "Finished emailing updated profiles list.", nextRun);
            }
            catch(Exception ex)
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, Failed, "Failed to send an email of updated profiles list due to: " + ex.Message , nextRun);
            }
        }

        /// <summary>
        /// This will send an email with the given subject, body, isbodyHtml, comma seperated To addresses, Comma seperated Bcc Addresses and Any attachments.
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isBodyHtml"></param>
        /// <param name="commaSeperatedToAddresses"></param>
        /// <param name="commaSeperatedBccAddresses"></param>
        /// <param name="attachments"></param>
        public static void Email(string subject, string body, bool isBodyHtml, string commaSeperatedToAddresses, string commaSeperatedBccAddresses, List<Attachment> attachments)
        {
            var smtpSettings = GetSMTPSettings();

            MailMessage message = new MailMessage();
            var addresses = commaSeperatedToAddresses.Split(',');
            foreach (var address in addresses)
            {
                message.To.Add(new MailAddress(address));
            }

            if (!string.IsNullOrEmpty(commaSeperatedBccAddresses))
            {
                var bccAddresses = commaSeperatedBccAddresses.Split(',');
                foreach (var address in bccAddresses)
                {
                    message.Bcc.Add(new MailAddress(address));
                }
            }

            message.Subject = IsUATEnvironment ? "(UAT) " + subject : subject;

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

        /// <summary>
        /// Prepares the email body of Updated Profiles list.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static string GetUpdatedProfilesListEmailBody(Dictionary<string, int> list)
        {
            string emailBody = string.Empty;
            foreach (var key in list)
            {
                emailBody = emailBody + string.Format(UpdatedProfileLinkFormat, SkillsProfilePagePath, key.Value, key.Key);
            }
            return emailBody;
        }

        /// <summary>
        /// Gives the Updated profiles list.
        /// </summary>
        /// <param name="currentWithTimeZone"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gives the SMTP client.
        /// </summary>
        /// <param name="smtpSettings"></param>
        /// <returns></returns>
        public static SmtpClient GetSmtpClient(SMTPSettings smtpSettings)
        {
            SmtpClient client = new SmtpClient(smtpSettings.MailServer, smtpSettings.PortNumber);
            client.EnableSsl = smtpSettings.SSLEnabled;
            client.Credentials = new NetworkCredential(smtpSettings.UserName, smtpSettings.Password);

            return client;
        }
        
        /// <summary>
        /// Gives the SMTP settings object.
        /// </summary>
        /// <returns></returns>
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

