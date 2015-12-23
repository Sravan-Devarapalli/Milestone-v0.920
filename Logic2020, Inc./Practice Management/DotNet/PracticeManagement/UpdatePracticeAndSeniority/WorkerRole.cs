using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Timers;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataAccess;
using DataTransferObjects;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using PraticeManagement.Utils;
using DataTransferObjects.Reports;
using PraticeManagement.Utils.Excel;
using DataTransferObjects.CornerStone;
using Tamir.SharpSsh;
using System.Reflection;
using Tamir.SharpSsh.jsch;
using System.Collections;

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

        public const string DisableAllMails_ConfigKey = "DisableAllMails";
        public const string RunSchedularDailyAtTime_ConfigKey = "RunSchedularDailyAtTime";
        public const string UPDATED_PROFILES_LIST_EMAIL_RECIEVER_ConfigKey = "UpdatedProfilesListEmailReciever";
        public const string EmailSubject_For_ProfilesUpdatedList_ConfigKey = "EmailSubjectForProfilesUpdatedList";
        public const string Skills_Profile_PagePath_ConfigKey = "SkillsProfilePagePath";
        public const string PAYROLLDISTRIBUTIONREPORT_SCHEDULETIME_ConfigKey = "PayrollDistributionScheduleTime";
        public const string ExceptionReportsScheduleTime_ConfigKey = "ExceptionReportsScheduleTime";
        public const string RecruitingMetricsReportScheduleTime_ConfigKey = "RecruitingMetricsReportScheduleTime";
        public const string PayRollDistibutionReportReciever_ConfigKey = "PayrollDistributionReportReciever";
        public const string EmailBccRecieverList_ConfigKey = "EmailBccRecieverList";
        public const string ProjectSummaryCacheScheduleTime_ConfigKey = "ProjectSummaryCacheScheduleTime";
        public const string LoginPagePath_ConfigKey = "LoginPagePath";
        public const string WelcomeMailScheduleTime_ConfigKey = "WelcomeMailScheduleTime";
        public const string UATTestingMail_ConfigKey = "UATTestingMail";
        public const string FeedbackMailsScheduleTime_ConfigKey = "FeedbackMailsScheduleTime";
        public const string FTPHostname_ConfigKey = "FTPHostName";
        public const string FTPUserName_ConfigKey = "FTPUserName";
        public const string FTPPassword_ConfigKey = "FTPPassword";
        public const string FTPUploadTime_ConfigKey = "FTPUploadTime";


        //Formats
        public const string UpdatedProfileLinkFormat = "<a href='{0}?Id={1}'>{2}</a><br/>";

        public const string PayRollDistributionReportEmailSubjectFormat = "Payroll Distribution Report for the period {0}.";
        public const string PayRollDistributionReportEmailBodyFormat = "Please find the Payroll Distribution report for the period {0} in the attachments.";
        public const string PayrollDistributionReportFailedFormat = "Failed to send an email of Payroll Distribution Report due to: {0}";
        public const string ResourceExceptionReportFailedFormat = "Failed to send an email of Resource Exception Report due to: {0}";
        public const string DateFormat = "MM/dd/yyyy";
        public const string DateDotFormat = "MM.dd.yyyy";
        public const string CurrencyExcelReportFormat = "$####,###,###,###,###,##0.00";
        public const string FailedRunningProcedureFormat = "Failed running the procedure {0} due to: {1}";
        public const string SuccessRunningProcedureFormat = "Successfully completed running the procedure {0}";
        public const string FailedRunningDalMethodFormat = "Failed DAL Method {0} due to: {1}";
        public const string SuccessRunningDalMethodFormat = "Successfully completed DAL Method {0}";
        public const string StartedDalMethodFormat = "Started DAL Method {0}";
        public const string ExportExcelCellFormat = "&nbsp; {0}";
        public const string WelcomeEmailFailedFormat = "Failed to send the WelCome Emails due to: {0}";
        public const string ActivateDeactivateEmailsFailedFormat = "Failed to send Activate and DeActivate Account Emails due to: {0}";

        //Log Status
        public const string SuccessStatus = "Success";

        public const string FailedStatus = "Failed";

        //Log Messages
        public const string M_SchedularStarted = "Started Schedular.";

        public const string M_PayrollDistributionReportStartedProcess = "Started Process of emailing Payroll Distribution Report.";
        public const string M_PayrollDistributionReportStartedReadingData = "Started reading data Payroll Distribution Report.";
        public const string M_PayrollDistributionReportReadDataSuccess = "Read the Payroll Distribution Report data.";
        public const string M_PayrollDistributionReportStartedEmailing = "Started emailing the Payroll Distribution Report data.";
        public const string M_PayrollDistributionReportEmailed = "Emailed the Payroll Distribution Report.";
        public const string M_StartedWelcomeEmails = "Started to send Welcome emails.";
        public const string M_FinishedWelcomeEmails = "Finished to send the Welcome Emails.";
        public const string M_StartedActivateDeactivateEmails = "Started to send the Activate and DeActivate Account Emails.";
        public const string M_FinishedActivateDeactivateEmails = "Finished to send the Activate and DeActivate Account Emails.";
        public const string M_InsertProjectSummaryCacheValueStartedProcess = "Started Process of Insert ProjectSummary Cache Values (DateTime:{0}).";
        public const string M_InsertProjectSummaryCacheValueFinishedProcess = "Finished Process of Insert ProjectSummary Cache Values (DateTime:{0}).";

        public const string M_ExceptionReportsStartedProcess = "Started Process of Resource Exception Report.";
        public const string M_ExceptionReportsStartedReadingData = "Started reading data Resource Exception Report.";
        public const string M_ExceptionReportsReadDataSuccess = "Read the Resource Exception Report data.";
        public const string M_ExceptionReportsStartedEmailing = "Started emailing the Resource Exception Report data.";
        public const string M_ExceptionReportsEmailed = "Emailed the Resource Exception Report.";
        public const string M_ExceptionReportsAttachmentPreparationStarted = "Started preparation of attachment of Resource Exception Report.";
        public const string M_ExceptionReportsAttachmentPreparationSucess = "Completed preparation of attachment of Resource Exception Report.";

        public const string M_RecruitingMetricsReportStartedProcess = "Started Process of Recruiting Metrics Report.";
        public const string M_RecruitingMetricsReportStartedReadingData = "Started reading data Recruiting Metrics Report.";
        public const string M_RecruitingMetricsReportReadDataSuccess = "Read the Recruiting Metrics Report data.";
        public const string M_RecruitingMetricsReportAttachmentPreparationStarted = "Started preparation of attachment of Recruiting Metrics Report.";
        public const string M_RecruitingMetricsReportAttachmentPreparationSucess = "Completed preparation of attachment of Recruiting Metrics Report.";
        public const string M_RecruitingMetricsReportStartedEmailing = "Started emailing the Recruiting Metrics Report data.";
        public const string M_RecruitingMetricsReportEmailed = "Emailed the Recruiting Metrics Report.";
        public const string RecruitingMetricsReportFailedFormat = "Failed to send an email of Recruiting Metrics Report due to: {0}";

        public const string M_StartedFeedbackInitialEmails = "Started to send Feedback Initials emails.";
        public const string M_FinishedFeedbackInitialEmails = "Finished to send the feedback initial Emails.";
        public const string FeedbackIntialMailFailedFormat = "Failed to send the feedback initial Emails due to: {0}";
        public const string M_StartedFeedbackRemainderEmails = "Started to send Feedback remainder emails.";
        public const string M_FinishedFeedbackRemainderEmails = "Finished to send the feedback remainder Emails.";
        public const string FeedbackRemainderMailFailedFormat = "Failed to send the feedback remainder Emails due to: {0}";

        public const string M_FTPConnectionStarted = "Started to connect the FTP Server";
        public const string M_FTPConnectionCompleted = "Connection to the FTP Server Completed";
        public const string M_FTPUploadingStarted = "Uploading the files to the FTP server started";
        public const string M_FTPUploaded = "Uploaded the files to the FTP server successfully";
        public const string M_FTPFailed = "Failed in FTP server operations due to: {0}";
        public const string M_FTPReplaced = "The files are going to be replaced in FTP server";
        public const string M_FTPPlaced = "The files are going to be placed in FTP server";
        public const string M_FTPThresholdExceeded_Started = "Checking for feedback threshold exceeded started";
        public const string M_FTPThresholdExceeded_Completed = "Checking for feedback threshold exceeded completed";
        public const string M_FTPThresholdExceeded_MailStarted = "Started to send mails for feedback threshold exceeded";
        public const string M_FTPThresholdExceeded_Failed = "Failed in sending the feedback threshold exceeded mail due to: {0}";
        public const string M_FeedFileErrorStarted = "Started sending Feed file error mail.";
        public const string M_FeedFileErrorCompleted = "Completed sending Feed file error mail.";
        public const string M_PlaceInLoadErrorStart = "Started to upload feed file in load_error directory.";
        public const string M_PlaceInLoadErrorComplete = "Completed to upload feed file in load_error directory.";

        //Stored Procedures
        public const string SP_AutoUpdateObjects = "dbo.AutoUpdateObjects";

        public const string SP_PersonPasswordInsert = "dbo.PersonPasswordInsert";
        public const string SP_GetPersonsByTodayHireDate = "dbo.GetPersonsByTodayHireDate";
        public const string SP_InsertProjectSummaryCacheValue = "dbo.InsertProjectSummaryCacheValue";
        public const string SP_DatabaseCleanUpObjects = "dbo.DatabaseCleanUp";
        public const string SP_ReBuildIndexes = "dbo.ReBuildIndexes";
        public const string SP_GetPersonsForIntialMailForProjectFeedback = "dbo.GetPersonsForIntialMailForProjectFeedback";
        public const string SP_GetPersonsForRemainderMailForProjectFeedback = "dbo.GetPersonsForRemainderMailForProjectFeedback";

        //Parameters
        public const string NextRun = "@NextRun";

        public const string LastRun = "@LastRun";
        public const string CurrentDateParam = "@CurrentDate";

        //Email Templates
        public const string DeActivateAccountTemplate = "DeActivateAccountTemplate";

        public const string ActivateAccountTemplate = "ActivateAccountTemplate";
        public const string WelcomeEmailTemplate = "WelcomeEmailTemplate";
        public const string AdministratorAddedTemplate = "AdministratorAddedTemplate";
        public const string IntialProjectFeedbackNotification = "IntialProjectFeedbackNotification";
        public const string RemainderProjectFeedbackNotification = "RemainderProjectFeedbackNotification";
        public const string FeedbackRequestThresholdExceededTemplate = "FeedbackRequestThresholdExceeded";
        public const string FeedfileErrorTemplate = "FeedfileError";

        #endregion Constants

        #region Properties

        public static bool IsUATEnvironment
        {
            get
            {
                var environment = GetConfigValue(Environment_ConfigKey);
                return (environment == "UAT");
            }
        }

        public static bool DisableAllMails
        {
            get
            {
                var disableAllMails = GetConfigValue(DisableAllMails_ConfigKey);
                return (disableAllMails.ToLower() == "yes");
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

        public static TimeSpan ExceptionReportsScheduleTime
        {
            get
            {
                return TimeSpan.Parse(GetConfigValue(ExceptionReportsScheduleTime_ConfigKey));
            }
        }

        public static TimeSpan RecruitingMetricsReportScheduleTime
        {
            get
            {
                return TimeSpan.Parse(GetConfigValue(RecruitingMetricsReportScheduleTime_ConfigKey));
            }
        }

        public static TimeSpan WelcomeMailScheduleTime
        {
            get
            {
                return TimeSpan.Parse(GetConfigValue(WelcomeMailScheduleTime_ConfigKey));
            }
        }

        public static TimeSpan FeedbackMailsScheduleTime
        {
            get
            {
                return TimeSpan.Parse(GetConfigValue(FeedbackMailsScheduleTime_ConfigKey));
            }
        }

        public static string PayrollDistributionReportReciever
        {
            get
            {
                return GetConfigValue(PayRollDistibutionReportReciever_ConfigKey);
            }
        }

        public static string EmailBccRecieverList
        {
            get
            {
                return GetConfigValue(EmailBccRecieverList_ConfigKey);
            }
        }

        public static TimeSpan ProjectSummaryCacheScheduleTime
        {
            get
            {
                return TimeSpan.Parse(GetConfigValue(ProjectSummaryCacheScheduleTime_ConfigKey));
            }
        }

        public static TimeSpan FirstSchedularTime
        {
            get
            {
                return TimeSpan.Parse(GetConfigValue(RunSchedularDailyAtTime_ConfigKey));
            }
        }

        public static string LoginPagePath
        {
            get
            {
                return WorkerRole.GetConfigValue(LoginPagePath_ConfigKey) ?? "https://practice.logic2020.com/Login.aspx";
            }
        }

        public static string UATTestingEmailReciever
        {
            get
            {
                return WorkerRole.GetConfigValue(UATTestingMail_ConfigKey);
            }
        }

        public static TimeSpan FTPUploadTime
        {
            get
            {
                return TimeSpan.Parse(GetConfigValue(FTPUploadTime_ConfigKey));
            }
        }

        public static int MailsCount = 0;

        #endregion Properties

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
                WorkerRole.SaveSchedularLog(currentDateTimeWithTimeZone, SuccessStatus, M_SchedularStarted, currentDateTimeWithTimeZone);
                if (currentDateTimeWithTimeZone.TimeOfDay > FirstSchedularTime)
                {
                    currentDateTimeWithTimeZone = CurrentPMTime;
                    //For the starting of the schedular if we update schedular binaries between 00:01:00 and 02:00:00, then we need to run Project Summary Cache Task.
                    RunProjectSummaryCacheTask(currentDateTimeWithTimeZone);
                    currentDateTimeWithTimeZone = CurrentPMTime;
                    //Runs at 06:00:00 every day.
                    //RunFeedbackMails(currentDateTimeWithTimeZone); //Commented out as per 'Nick' statement in a cal on 20150805
                    //Runs at 04:30:00 every day.
                    UploadDataFeedToFTP(currentDateTimeWithTimeZone);
                    //For the starting of the schedular if we update schedular binaries between 00:01:00 and 07:00:00, then we need to run Pay roll distribution report and Welcome Email Task for new hires.
                    RunPayrollDistributionReport(currentDateTimeWithTimeZone);
                    //Runs at 07:00:00 on every monday.
                    RunResourceExceptionReport(currentDateTimeWithTimeZone);
                    //Runs at 07:00:00 on 1st of every month.
                    RunRecruitingMetricsReport(currentDateTimeWithTimeZone);
                    //Runs at 07:00:00 every day.
                    RunWelcomeEmailTaskForNewHires(currentDateTimeWithTimeZone);
                    
                }

                while (true)
                {
                    Thread.Sleep(5 * 60 * 1000);

                    currentDateTimeWithTimeZone = CurrentPMTime;
                    nextRun = GetNextRunDate(currentDateTimeWithTimeZone, FirstSchedularTime);
                    var sleepTimeSpan = nextRun > currentDateTimeWithTimeZone ?
                        nextRun - currentDateTimeWithTimeZone :
                        currentDateTimeWithTimeZone - nextRun;
                    Thread.Sleep(sleepTimeSpan);
                    currentDateTimeWithTimeZone = currentDateTimeWithTimeZone.Add(sleepTimeSpan);
                    //Runs at 02:00:00 every day.
                    RunTasks(currentDateTimeWithTimeZone);
                    currentDateTimeWithTimeZone = CurrentPMTime;
                    RunProjectSummaryCacheTask(currentDateTimeWithTimeZone);
                    currentDateTimeWithTimeZone = CurrentPMTime;
                    //Runs at 06:00:00 every day.
                    //RunFeedbackMails(currentDateTimeWithTimeZone); //Commented out as per 'Nick' statement in a cal on 20150805
                    //Runs at 04:30:00 on every day.
                    UploadDataFeedToFTP(currentDateTimeWithTimeZone);
                    //Runs at 07:00:00 on 3rd and 18th of every month.
                    RunPayrollDistributionReport(currentDateTimeWithTimeZone);
                    //Runs at 07:00:00 on every day.
                    RunResourceExceptionReport(currentDateTimeWithTimeZone);
                    //Runs at 07:00:00 on 1st of every month.
                    RunRecruitingMetricsReport(currentDateTimeWithTimeZone);
                    //Runs at 07:00:00 every day.
                    RunWelcomeEmailTaskForNewHires(currentDateTimeWithTimeZone);
                }
            }
            catch (Exception ex)
            {
                WorkerRole.SaveSchedularLog(DateTime.UtcNow, FailedStatus, "Schedular error: " + ex.Message, DateTime.UtcNow);
                throw ex;
            }
        }

        #endregion Override Methods

        #region Private Methods

        /// <summary>
        /// Gives the date and time of the next run.
        /// </summary>
        /// <param name="currentDateTimeWithTimeZone"></param>
        /// <returns></returns>
        public static DateTime GetNextRunDate(DateTime currentDateTimeWithTimeZone, TimeSpan ScheduledTime)
        {
            DateTime nextRun;
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
            var nextRun = GetNextRunDate(currentWithTimeZone, FirstSchedularTime);
            WorkerRole.StartAutomaticUpdation(currentWithTimeZone, nextRun);
            WorkerRole.EmailUpdatedProfilesList(currentWithTimeZone, nextRun);
            WorkerRole.RunDatabaseCleanUp(currentWithTimeZone);
            WorkerRole.RunReBuildIndexes(currentWithTimeZone);
        }

        /// <summary>
        /// Runs Payroll Distribution Report Task as per the time reaches( Every month 3rd and 18th at 07:00:00).
        /// </summary>
        /// <param name="currentDateTimeWithTimeZone"></param>
        public static void RunPayrollDistributionReport(DateTime currentDateTimeWithTimeZone)
        {
            if (
                ((currentDateTimeWithTimeZone.Day == 4 || currentDateTimeWithTimeZone.Day == 19) && currentDateTimeWithTimeZone.TimeOfDay < PayrollDistributionReportScheduleTime)
                || (currentDateTimeWithTimeZone.Date == new DateTime(2013, 6, 4).Date && currentDateTimeWithTimeZone.TimeOfDay < PayrollDistributionReportScheduleTime) //Added For the sake of defect #3151 . need to remove in next update.
                )
            {
                var sleeptime = PayrollDistributionReportScheduleTime - currentDateTimeWithTimeZone.TimeOfDay;
                Thread.Sleep(sleeptime);
                SendPayrollDistributionReport(CurrentPMTime);
            }
        }

        public static void RunDatabaseCleanUp(DateTime currentDateTimeWithTimeZone)
        {
            if (currentDateTimeWithTimeZone.Day == 1 && currentDateTimeWithTimeZone.Month == 1)
            {
                DateTime nextRun = currentDateTimeWithTimeZone.AddYears(1);
                SqlConnection connection = null;
                try
                {
                    var connectionString = WorkerRole.GetConnectionString();

                    if (string.IsNullOrEmpty(connectionString))
                        return;

                    connection = new SqlConnection(connectionString);
                    SqlCommand cmd = new SqlCommand(SP_DatabaseCleanUpObjects, connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1000;
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();

                    WorkerRole.SaveSchedularLog(currentDateTimeWithTimeZone, SuccessStatus, string.Format(SuccessRunningProcedureFormat, SP_DatabaseCleanUpObjects), nextRun);
                }
                catch (Exception ex)
                {
                    WorkerRole.SaveSchedularLog(currentDateTimeWithTimeZone, FailedStatus, string.Format(FailedRunningProcedureFormat, SP_DatabaseCleanUpObjects, ex.Message), nextRun);
                }
                finally
                {
                    if (connection != null && connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public static void RunReBuildIndexes(DateTime currentDateTimeWithTimeZone)
        {
            if (currentDateTimeWithTimeZone.DayOfWeek == DayOfWeek.Sunday)
            {
                DateTime nextRun = currentDateTimeWithTimeZone.AddDays(6);
                SqlConnection connection = null;
                try
                {
                    var connectionString = WorkerRole.GetConnectionString();

                    if (string.IsNullOrEmpty(connectionString))
                        return;

                    connection = new SqlConnection(connectionString);
                    SqlCommand cmd = new SqlCommand(SP_ReBuildIndexes, connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1000;
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();

                    WorkerRole.SaveSchedularLog(currentDateTimeWithTimeZone, SuccessStatus, string.Format(SuccessRunningProcedureFormat, SP_ReBuildIndexes), nextRun);
                }
                catch (Exception ex)
                {
                    WorkerRole.SaveSchedularLog(currentDateTimeWithTimeZone, FailedStatus, string.Format(FailedRunningProcedureFormat, SP_ReBuildIndexes, ex.Message), nextRun);
                }
                finally
                {
                    if (connection != null && connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Runs WelcomeEmail Task For NewHires as per the time reaches( Every day at 07:00:00).
        /// </summary>
        public static void RunWelcomeEmailTaskForNewHires(DateTime currentDateTimeWithTimeZone)
        {
            if (currentDateTimeWithTimeZone.TimeOfDay < WelcomeMailScheduleTime)
            {
                currentDateTimeWithTimeZone = CurrentPMTime;
                var nextRun = GetNextRunDate(currentDateTimeWithTimeZone, WelcomeMailScheduleTime);
                if (currentDateTimeWithTimeZone.TimeOfDay < WelcomeMailScheduleTime)
                {
                    var sleeptime = WelcomeMailScheduleTime - currentDateTimeWithTimeZone.TimeOfDay;
                    Thread.Sleep(sleeptime);
                }
                WorkerRole.GetTodaysHireDatePersonsAndSenEmails(CurrentPMTime, nextRun);
            }
        }

        public static void RunFeedbackMails(DateTime currentDateTimeWithTimeZone)
        {
            if (currentDateTimeWithTimeZone.TimeOfDay < FeedbackMailsScheduleTime)
            {
                currentDateTimeWithTimeZone = CurrentPMTime;
                var nextRun = GetNextRunDate(currentDateTimeWithTimeZone, FeedbackMailsScheduleTime);
                if (currentDateTimeWithTimeZone.TimeOfDay < FeedbackMailsScheduleTime)
                {
                    var sleeptime = FeedbackMailsScheduleTime - currentDateTimeWithTimeZone.TimeOfDay;
                    Thread.Sleep(sleeptime);
                }
                WorkerRole.GetPersonsForIntialFeedbackMailAndSendEmails(CurrentPMTime, nextRun);
                WorkerRole.GetPersonsForRemainderFeedbackMailAndSendEmails(CurrentPMTime, nextRun);
            }
        }

        /// <summary>
        /// Cache Project Summary Values Task as per the time reaches( Every day at 02:00:00).
        /// </summary>
        /// <param name="currentDateTimeWithTimeZone"></param>
        public static void RunProjectSummaryCacheTask(DateTime currentDateTimeWithTimeZone)
        {
            if (currentDateTimeWithTimeZone.TimeOfDay < ProjectSummaryCacheScheduleTime)
            {
                var sleeptime = ProjectSummaryCacheScheduleTime - currentDateTimeWithTimeZone.TimeOfDay;
                Thread.Sleep(sleeptime);
                InsertProjectSummaryCacheValue(CurrentPMTime);
            }
        }

        /// <summary>
        /// Cache Project Summary Values
        /// </summary>
        public static void InsertProjectSummaryCacheValue(DateTime currentWithTimeZone)
        {
            DateTime nextRun = currentWithTimeZone.AddDays(1);
            WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, string.Format(M_InsertProjectSummaryCacheValueStartedProcess, CurrentPMTime), nextRun);
            SqlConnection connection = null;
            try
            {
                var connectionString = WorkerRole.GetConnectionString();
                if (string.IsNullOrEmpty(connectionString))
                    return;

                connection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand(SP_InsertProjectSummaryCacheValue, connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 1000;
                SqlParameter currenDateParam = new SqlParameter(CurrentDateParam, currentWithTimeZone);
                cmd.Parameters.Add(currenDateParam);
                connection.Open();

                cmd.ExecuteNonQuery();
                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, string.Format(SuccessRunningProcedureFormat, SP_InsertProjectSummaryCacheValue), nextRun);
            }
            catch (Exception ex)
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, FailedStatus, string.Format(FailedRunningProcedureFormat, SP_InsertProjectSummaryCacheValue, ex.Message), nextRun);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, string.Format(M_InsertProjectSummaryCacheValueFinishedProcess, CurrentPMTime), nextRun);
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
                SqlCommand cmd = new SqlCommand(SP_AutoUpdateObjects, connection);
                cmd.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlParameter lastRunParam = new SqlParameter(LastRun, currentWithTimeZone);
                cmd.Parameters.Add(lastRunParam);
                SqlParameter nextRunParam = new SqlParameter(NextRun, nextRun);
                cmd.Parameters.Add(nextRunParam);

                List<Person> terminatedPersons = new List<Person>();
                using (var reader = cmd.ExecuteReader())
                {
                    ReadPersons(reader, terminatedPersons);
                }

                connection.Close();
                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, string.Format(SuccessRunningProcedureFormat, SP_AutoUpdateObjects), nextRun);

                //sending activate and deactivate account emails.
                if (terminatedPersons.Count > 0)
                {
                    SendActivateAndDeactivateAccountEmails(terminatedPersons, currentWithTimeZone, nextRun);
                }
            }
            catch (Exception ex)
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, FailedStatus, string.Format(FailedRunningProcedureFormat, SP_AutoUpdateObjects, ex.Message), nextRun);
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
            var nextRunTime = currentDate.Day == 4 ? currentDate.AddDays(19 - 4) : currentDate.AddMonths(1).AddDays(4);
            WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_PayrollDistributionReportStartedProcess, nextRunTime);
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    DateTime startDate = currentDate.Day == 4 ? currentDate.AddMonths(-1).AddDays(16 - currentDate.AddMonths(-1).Day).Date : new DateTime(currentDate.Year, currentDate.Month, 1);
                    DateTime endDate = currentDate.Day == 4 ? currentDate.Date.AddDays(-currentDate.Day) : new DateTime(currentDate.Year, currentDate.Month, 15);

                    //Read the data.
                    WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_PayrollDistributionReportStartedReadingData, nextRunTime);
                    var data = ReportDAL.PersonTimeEntriesDetailsSchedular(null, startDate, endDate, connection);

                    //Mail the data.
                    WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_PayrollDistributionReportReadDataSuccess, nextRunTime);
                    EmailPayrollDistributionReport(data, startDate, endDate, currentDate, nextRunTime);
                    WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_PayrollDistributionReportEmailed, nextRunTime);
                }
            }
            catch (Exception ex)
            {
                WorkerRole.SaveSchedularLog(currentDate, FailedStatus, string.Format(PayrollDistributionReportFailedFormat, ex.Message), nextRunTime);
            }
        }

        /// <summary>
        /// Runs Resource Exception Report Task as per the time reaches( Every monday morning at 07:00:00).
        /// </summary>
        /// <param name="currentDateTimeWithTimeZone"></param>
        public static void RunResourceExceptionReport(DateTime currentDateTimeWithTimeZone)
        {
            if (currentDateTimeWithTimeZone.DayOfWeek == DayOfWeek.Monday && currentDateTimeWithTimeZone.TimeOfDay < ExceptionReportsScheduleTime)
            {
                currentDateTimeWithTimeZone = CurrentPMTime;
                if (currentDateTimeWithTimeZone.DayOfWeek == DayOfWeek.Monday && currentDateTimeWithTimeZone.TimeOfDay < ExceptionReportsScheduleTime)
                {
                    var sleeptime = ExceptionReportsScheduleTime - currentDateTimeWithTimeZone.TimeOfDay;
                    Thread.Sleep(sleeptime);
                }
                WorkerRole.SendResourceExceptionReport(CurrentPMTime);
            }
        }

        /// <summary>
        /// Runs Recruiting Metrics Report Task as per the time reaches(1st of Every month morning at 07:00:00).
        /// </summary> 
        /// <param name="currentDateTimeWithTimeZone"></param>
        public static void RunRecruitingMetricsReport(DateTime currentDateTimeWithTimeZone)
        {
            if (currentDateTimeWithTimeZone.Day == 1 && currentDateTimeWithTimeZone.TimeOfDay < RecruitingMetricsReportScheduleTime)
            {
                currentDateTimeWithTimeZone = CurrentPMTime;
                if (currentDateTimeWithTimeZone.Day == 1 && currentDateTimeWithTimeZone.TimeOfDay < RecruitingMetricsReportScheduleTime)
                {
                    var sleeptime = RecruitingMetricsReportScheduleTime - currentDateTimeWithTimeZone.TimeOfDay;
                    Thread.Sleep(sleeptime);
                }
                WorkerRole.SendRecruitingMetricsReport(CurrentPMTime);
            }
        }

        /// <summary>
        /// Sends an email of Resource Exception Report.
        /// </summary>
        public static void SendResourceExceptionReport(DateTime currentDate)
        {
            var nextRunTime = currentDate.AddDays(7);
            WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_ExceptionReportsStartedProcess, nextRunTime);
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    DateTime EndDate = currentDate.Date.AddDays(-1);
                    DateTime StartDate = EndDate.AddDays(-6);

                    //Read the data.
                    WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_ExceptionReportsStartedReadingData, nextRunTime);
                    ResourceExceptionReport[] zeroExceptionReportList = ReportDAL.ZeroHourlyRateExceptionReport(StartDate, EndDate, connection).ToArray();
                    WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, string.Format(SuccessRunningDalMethodFormat, "ZeroHourlyRateExceptionReport"), nextRunTime);
                    ResourceExceptionReport[] unassignedExceptionReportList = ReportDAL.ResourceAssignedOrUnassignedChargingExceptionReport(StartDate, EndDate, true, connection).ToArray();
                    WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, string.Format(SuccessRunningDalMethodFormat, "ResourceAssignedOrUnassignedChargingExceptionReport for unassignedExceptionReportList"), nextRunTime);
                    ResourceExceptionReport[] assignedExceptionReportList = ReportDAL.ResourceAssignedOrUnassignedChargingExceptionReport(StartDate, EndDate, false, connection).ToArray();
                    WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, string.Format(SuccessRunningDalMethodFormat, "ResourceAssignedOrUnassignedChargingExceptionReport for assignedExceptionReportList"), nextRunTime);
                    WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_ExceptionReportsReadDataSuccess, nextRunTime);
                    WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_ExceptionReportsAttachmentPreparationStarted, nextRunTime);
                    ResourceExceptionReportExcelUtil resourceExceptionReport = new ResourceExceptionReportExcelUtil(StartDate, EndDate, zeroExceptionReportList, unassignedExceptionReportList, assignedExceptionReportList);
                    resourceExceptionReport.PopulateAttachment();
                    WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_ExceptionReportsAttachmentPreparationSucess, nextRunTime);
                    WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_ExceptionReportsStartedEmailing, nextRunTime);
                    WorkerRole.EmailResourceExceptionReports(StartDate, EndDate, resourceExceptionReport.Attachment, connection);
                    WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_ExceptionReportsEmailed, nextRunTime);
                }
            }
            catch (Exception ex)
            {
                WorkerRole.SaveSchedularLog(currentDate, FailedStatus, string.Format(ResourceExceptionReportFailedFormat, ex.Message + (ex.InnerException != null ? ": " + ex.InnerException.Message : string.Empty)), nextRunTime);
            }
        }

        /// <summary>
        /// Sends an email of Recruiting Metrics Report.
        /// </summary>
        public static void SendRecruitingMetricsReport(DateTime currentDate)
        {
            var nextRunTime = currentDate.AddMonths(1);
            WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_RecruitingMetricsReportStartedProcess, nextRunTime);
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    DateTime EndDate = currentDate.Date.AddDays(-1);
                    DateTime StartDate = EndDate.AddDays(1 - EndDate.DayOfYear);

                    //Read the data.
                    WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_RecruitingMetricsReportStartedReadingData, nextRunTime);
                    Person[] recruitingMetricsReportList = ReportDAL.RecruitingMetricsReport(StartDate, EndDate, connection).ToArray();
                    WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, string.Format(SuccessRunningDalMethodFormat, "RecrutingMetricsReport"), nextRunTime);

                    WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_RecruitingMetricsReportReadDataSuccess, nextRunTime);
                    WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_RecruitingMetricsReportAttachmentPreparationStarted, nextRunTime);
                    RecruitingMetricsReportExcelUtil recruitingMetricsReport = new RecruitingMetricsReportExcelUtil(StartDate, EndDate, recruitingMetricsReportList);
                    recruitingMetricsReport.PopulateAttachment();
                    WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_RecruitingMetricsReportAttachmentPreparationSucess, nextRunTime);
                    WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_RecruitingMetricsReportStartedEmailing, nextRunTime);
                    WorkerRole.EmailRecruitingMetricsReport(StartDate, EndDate, recruitingMetricsReport.Attachment, connection);
                    WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_RecruitingMetricsReportEmailed, nextRunTime);
                }
            }
            catch (Exception ex)
            {
                WorkerRole.SaveSchedularLog(currentDate, FailedStatus, string.Format(RecruitingMetricsReportFailedFormat, ex.Message), nextRunTime);
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
            WorkerRole.SaveSchedularLog(currentTime, SuccessStatus, M_PayrollDistributionReportStartedEmailing, nextRunTime);
            var subject = string.Format(PayRollDistributionReportEmailSubjectFormat, range);
            var body = string.Format(PayRollDistributionReportEmailBodyFormat, range);
            Email(subject, body, true, PayrollDistributionReportReciever, string.Empty, string.Empty, attachments);
        }

        /// <summary>
        /// Emails the Resource Exception Reports
        /// </summary>
        /// <param name="reportdata"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        private static void EmailResourceExceptionReports(DateTime startDate, DateTime endDate, byte[] attachmentByteArray, SqlConnection connection)
        {
            MemoryStream attachmentStream = new MemoryStream(attachmentByteArray);
            var attachment = new Attachment(attachmentStream, string.Format("ExceptionReporting_{0}_{1}.xls", startDate.ToString(Constants.Formatting.EntryDateFormat), endDate.ToString(Constants.Formatting.EntryDateFormat)), "application/vnd.ms-excel");
            var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName("ResourceExceptionReports", connection);
            var subject = string.Format(emailTemplate.Subject, startDate.ToString(Constants.Formatting.EntryDateFormat), endDate.ToString(Constants.Formatting.EntryDateFormat));
            var body = string.Format(emailTemplate.Body, startDate.ToString(Constants.Formatting.EntryDateFormat), endDate.ToString(Constants.Formatting.EntryDateFormat));
            WorkerRole.Email(subject, body, true, emailTemplate.EmailTemplateTo, string.Empty, string.Empty, new List<Attachment>() { attachment });
        }

        private static void EmailRecruitingMetricsReport(DateTime startDate, DateTime endDate, byte[] attachmentByteArray, SqlConnection connection)
        {
            MemoryStream attachmentStream = new MemoryStream(attachmentByteArray);
            var attachment = new Attachment(attachmentStream, string.Format("RecruitingMetricsReport_{0}_and_{1}.xls", startDate.ToString(Constants.Formatting.EntryDateFormat), endDate.ToString(Constants.Formatting.EntryDateFormat)), "application/vnd.ms-excel");
            var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName("RecruitingMetricsReport", connection);
            var subject = string.Format(emailTemplate.Subject, startDate.ToString(Constants.Formatting.EntryDateFormat), endDate.ToString(Constants.Formatting.EntryDateFormat));
            var body = string.Format(emailTemplate.Body, startDate.ToString(Constants.Formatting.EntryDateFormat), endDate.ToString(Constants.Formatting.EntryDateFormat));
            WorkerRole.Email(subject, body, true, emailTemplate.EmailTemplateTo, string.Empty, string.Empty, new List<Attachment>() { attachment });
        }

        protected static void gvExp_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var row = e.Row;
            if (row.RowType == DataControlRowType.Header)
            {
                row.Cells[0].Text = "Employee Id";
                row.Cells[1].Text = "Employee";
                row.Cells[2].Text = "Pay Type";
                row.Cells[3].Text = "Is Offshore?";
                row.Cells[4].Text = "Account";
                row.Cells[5].Text = "Account Name";
                row.Cells[6].Text = "Business Unit";
                row.Cells[7].Text = "Business Unit Name";
                row.Cells[8].Text = "Project";
                row.Cells[9].Text = "Project Name";
                row.Cells[10].Text = "Status";
                row.Cells[11].Text = "Billing";
                row.Cells[12].Text = "Phase";
                row.Cells[13].Text = "Work Type";
                row.Cells[14].Text = "Work Type Name";
                row.Cells[15].Text = "Date";
                row.Cells[16].Text = "Billable Hours";
                row.Cells[17].Text = "Non-Billable Hours";
                row.Cells[18].Text = "Total Hours";
                row.Cells[19].Text = "Bill Rate";
                row.Cells[20].Text = "Pay Rate";
                row.Cells[21].Text = "Note";
            }
            else if (row.RowType == DataControlRowType.DataRow)
            {
                //ExcelEncodeText(row.Cells[5]);//AccountName
                //ExcelEncodeText(row.Cells[7]);//BussinessUnitName
                //ExcelEncodeText(row.Cells[9]);//ProjectName
                //ExcelEncodeText(row.Cells[14]);//WorktypeName
                ExcelEncodeText(row.Cells[21]);//Note
                row.Style.Add(HtmlTextWriterStyle.Height, "20px");
            }
        }

        private static void ExcelEncodeText(TableCell cell)
        {
            Label lbl = new Label();
            lbl.Text = string.Format(ExportExcelCellFormat, cell.Text);

            var sw = new StringWriter();
            using (var htw = new HtmlTextWriter(sw))
            {
                lbl.RenderControl(htw);
            }

            cell.Text = sw.ToString();
        }

        /// <summary>
        /// Gives Excel data for the given reportData and range.
        /// </summary>
        /// <param name="reportdata"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static string PreparePayrollDistributionReportExcelData(List<DataTransferObjects.Reports.TimeEntriesGroupByClientAndProject> reportdata, string range)
        {
            StringBuilder attachmentData = new StringBuilder();
            attachmentData.Append("Payroll Distribution Report");
            attachmentData.Append("<br/>");
            attachmentData.AppendLine();
            attachmentData.Append(range);
            attachmentData.Append("<br/>");
            attachmentData.Append("<br/>");

            if (reportdata.Count > 0)
            {
                var data = (from r in reportdata
                            from dth in r.DayTotalHours
                            from dthl in dth.DayTotalHoursList
                            select new
                            {
                                empId = r.Person.EmployeeNumber,
                                empFirstLastName = r.Person.PersonFirstLastName,
                                payType = string.IsNullOrEmpty(dthl.PayType) ? string.Empty : dthl.PayType,
                                isOffshore = r.Person.IsOffshore ? "Yes" : "No",
                                clientCode = r.Client.Code,
                                clientName = r.Client.Name,
                                groupCode = r.Project.Group.Code,
                                groupName = r.Project.Group.Name,
                                projectNumber = r.Project.ProjectNumber,
                                projectName = r.Project.Name,
                                Status = r.Project.Status.Name,
                                Billing = r.BillableType,
                                Phase = "01",
                                WorkType = dthl.TimeType.Code,
                                WorkTypeName = dthl.TimeType.Name,
                                Date = dth.Date.ToString(DateFormat),
                                BillableHours = dthl.BillableHours,
                                NonBillableHours = dthl.NonBillableHours,
                                TotalHours = dthl.TotalHours,
                                BillRate = dthl.HourlyRate.HasValue ? dthl.HourlyRate.Value.ToString(CurrencyExcelReportFormat) : string.Empty,
                                PayRate = dthl.PayRate.HasValue ? dthl.PayRate.Value.ToString(CurrencyExcelReportFormat) : string.Empty,
                                Note = dthl.Note
                            }
                            );

                var gvExp = new GridView();
                gvExp.DataSource = data;
                gvExp.RowDataBound += new GridViewRowEventHandler(gvExp_RowDataBound);
                gvExp.DataBind();

                var str = new StringWriter();
                var htw = new HtmlTextWriter(str);

                gvExp.RenderControl(htw);

                attachmentData.Append(str.ToString());
            }
            else
            {
                attachmentData.Append("There are no Time Entries entered for the selected period.");
            }

            return attachmentData.ToString();
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
            //System.Text.Encoding Enc = System.Text.Encoding.ASCII;

            byte[] mBArray = Encoding.Default.GetBytes(data);

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
            WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, "Started verifying the updated profiles list.", nextRun);

            try
            {
                var list = GetUpdatedProfilesList(currentWithTimeZone);
                if (list != null && list.Count() > 0)
                {
                    string body = GetUpdatedProfilesListEmailBody(list);
                    Email(EmailSubjectForProfilesUpdatedList, body, true, UpdatedProfilesListEmailReciever, string.Empty, string.Empty, null);
                    WorkerRole.SaveSchedularLog(currentWithTimeZone, "Emailed", "Emailed the updated profiles list.", nextRun);
                }
                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, "Finished verifying the updated profiles list.", nextRun);
            }
            catch (Exception ex)
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, FailedStatus, "Failed to send an email of updated profiles list due to: " + ex.Message, nextRun);
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
        public static void Email(string subject, string body, bool isBodyHtml, string commaSeperatedToAddresses, string commaSeperatedBccAddresses, string commaSeperatedCcAddresses, List<Attachment> attachments, bool isHighPriority = false)
        {
            if (!IsUATEnvironment || !DisableAllMails)
            {
                var smtpSettings = GetSMTPSettings();

                MailMessage message = new MailMessage();
                message.Priority = isHighPriority ? MailPriority.High : MailPriority.Normal;
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

                if (!string.IsNullOrEmpty(commaSeperatedCcAddresses))
                {
                    var ccAddresses = commaSeperatedCcAddresses.Split(',');
                    foreach (var address in ccAddresses)
                    {
                        message.CC.Add(new MailAddress(address));
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
                if (IsUATEnvironment)
                {
                    message.To.Clear();
                    message.CC.Clear();
                    message.Bcc.Clear();
                    var uatToAddresses = UATTestingEmailReciever.Split(',');
                    for (int i = 0; i < uatToAddresses.Length; i++)
                    {
                        message.To.Add(new MailAddress(uatToAddresses[i]));
                    }
                }
                SmtpClient client = GetSmtpClient(smtpSettings);
                message.From = new MailAddress(smtpSettings.PMSupportEmail);
                client.Send(message);
            }
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
            catch (Exception ex)
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

        /// <summary>
        /// Retrieves today's hire date persons.
        /// </summary>
        public static void GetTodaysHireDatePersonsAndSenEmails(DateTime currentWithTimeZone, DateTime nextRun)
        {
            SqlConnection connection = null;
            try
            {
                var connectionString = WorkerRole.GetConnectionString();

                if (string.IsNullOrEmpty(connectionString))
                    return;
                connection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand(SP_GetPersonsByTodayHireDate, connection);
                cmd.CommandType = CommandType.StoredProcedure;
                connection.Open();

                List<Person> persons = new List<Person>();
                using (var reader = cmd.ExecuteReader())
                {
                    ReadPersons(reader, persons);
                }
                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, string.Format(SuccessRunningProcedureFormat, SP_GetPersonsByTodayHireDate), nextRun);
                //sending login credentials through email.
                //According to the changes in defect #3240 WelComeEmails can be sent in UAT also.
                if (persons.Count > 0)
                {
                    SendWelComeEmails(persons, currentWithTimeZone, nextRun);
                }
            }
            catch (Exception ex)
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, FailedStatus, string.Format(FailedRunningProcedureFormat, SP_GetPersonsByTodayHireDate) + ex.Message, nextRun);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public static void GetPersonsForIntialFeedbackMailAndSendEmails(DateTime currentWithTimeZone, DateTime nextRun)
        {
            SqlConnection connection = null;
            try
            {
                var connectionString = WorkerRole.GetConnectionString();

                if (string.IsNullOrEmpty(connectionString))
                    return;
                connection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand(SP_GetPersonsForIntialMailForProjectFeedback, connection);
                cmd.CommandType = CommandType.StoredProcedure;
                connection.Open();

                List<ProjectFeedbackMail> feedbacks = new List<ProjectFeedbackMail>();
                using (var reader = cmd.ExecuteReader())
                {
                    ReadProjectFeedback(reader, feedbacks);
                }
                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, string.Format(SuccessRunningProcedureFormat, SP_GetPersonsForIntialMailForProjectFeedback), nextRun);

                if (feedbacks.Count > 0)
                {
                    SendFeedbackIntialMails(feedbacks, currentWithTimeZone, nextRun);
                }
            }
            catch (Exception ex)
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, FailedStatus, string.Format(FailedRunningProcedureFormat, SP_GetPersonsForIntialMailForProjectFeedback) + ex.Message, nextRun);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public static void GetPersonsForRemainderFeedbackMailAndSendEmails(DateTime currentWithTimeZone, DateTime nextRun)
        {
            SqlConnection connection = null;
            try
            {
                var connectionString = WorkerRole.GetConnectionString();

                if (string.IsNullOrEmpty(connectionString))
                    return;
                connection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand(SP_GetPersonsForRemainderMailForProjectFeedback, connection);
                cmd.CommandType = CommandType.StoredProcedure;
                connection.Open();

                List<ProjectFeedbackMail> feedbacks = new List<ProjectFeedbackMail>();
                using (var reader = cmd.ExecuteReader())
                {
                    ReadProjectFeedback(reader, feedbacks);
                }
                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, string.Format(SuccessRunningProcedureFormat, SP_GetPersonsForRemainderMailForProjectFeedback), nextRun);

                if (feedbacks.Count > 0)
                {
                    SendFeedbackRemainderMails(feedbacks, currentWithTimeZone, nextRun);
                }
            }
            catch (Exception ex)
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, FailedStatus, string.Format(FailedRunningProcedureFormat, SP_GetPersonsForRemainderMailForProjectFeedback) + ex.Message, nextRun);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private static void SendFeedbackIntialMails(List<ProjectFeedbackMail> feedbacks, DateTime currentWithTimeZone, DateTime nextRun)
        {
            try
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, M_StartedFeedbackInitialEmails, nextRun);
                var feedbackMailTemplate = GetEmailTemplate(IntialProjectFeedbackNotification);

                foreach (var feedback in feedbacks)
                {
                    foreach (var resource in feedback.Resources)
                    {
                        WorkerRole.MailsCount++;
                        string url = IsUATEnvironment ? "http://65.52.17.100/ProjectDetail.aspx?id=" + feedback.Project.Id.Value.ToString() : "https://practice.logic2020.com/ProjectDetail.aspx?id=" + feedback.Project.Id.Value.ToString();
                        var subject = String.Format(feedbackMailTemplate.Subject, resource.DueDate.ToString(Constants.Formatting.EntryDateFormat), resource.Person.PersonFirstLastName, feedback.Project.ProjectNumber, feedback.Project.Name);
                        var emailBody = String.Format(feedbackMailTemplate.Body, resource.Person.PersonFirstLastName, feedback.Project.ProjectNumber, resource.DueDate.ToString(Constants.Formatting.EntryDateFormat), resource.Person.Title.TitleName, resource.ReviewStartDate.ToString(Constants.Formatting.EntryDateFormat), resource.ReviewEndDate.ToString(Constants.Formatting.EntryDateFormat), feedback.Project.Name, url);
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
                        Email(subject, emailBody, true, feedback.ProjectManagersAliasList.Substring(0, feedback.ProjectManagersAliasList.Length - 1), string.Empty, ccAddressList == "" ? "" : ccAddressList.Substring(0, ccAddressList.Length - 1), null);
                        if (WorkerRole.MailsCount % 12 == 0)
                        {
                            Thread.Sleep(65 * 1000);
                        }
                    }
                }

                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, M_FinishedFeedbackInitialEmails, nextRun);
            }
            catch (Exception ex)
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, FailedStatus, string.Format(FeedbackIntialMailFailedFormat, ex.Message + (ex.InnerException != null ? ": " + ex.InnerException.Message : string.Empty)), nextRun);
            }
        }

        private static void SendFeedbackRemainderMails(List<ProjectFeedbackMail> feedbacks, DateTime currentWithTimeZone, DateTime nextRun)
        {
            try
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, M_StartedFeedbackRemainderEmails, nextRun);
                var feedbackMailTemplate = GetEmailTemplate(RemainderProjectFeedbackNotification);

                foreach (var feedback in feedbacks)
                {
                    foreach (var resource in feedback.Resources)
                    {
                        WorkerRole.MailsCount++;
                        string url = IsUATEnvironment ? "http://65.52.17.100/ProjectDetail.aspx?id=" + feedback.Project.Id.Value.ToString() : "https://practice.logic2020.com/ProjectDetail.aspx?id=" + feedback.Project.Id.Value.ToString();
                        var subject = String.Format(feedbackMailTemplate.Subject, resource.Person.PersonFirstLastName, feedback.Project.ProjectNumber, feedback.Project.Name);
                        var emailBody = String.Format(feedbackMailTemplate.Body, resource.Person.PersonFirstLastName, feedback.Project.ProjectNumber, resource.DueDate.ToString(Constants.Formatting.EntryDateFormat), resource.Person.Title.TitleName, resource.ReviewStartDate.ToString(Constants.Formatting.EntryDateFormat), resource.ReviewEndDate.ToString(Constants.Formatting.EntryDateFormat), feedback.Project.Name, url);
                        var ccAddressList = "";
                        if (!string.IsNullOrEmpty(feedback.ProjectOwnerAlias) && !feedback.ProjectManagersAliasList.Contains(feedback.ProjectOwnerAlias))
                        {
                            ccAddressList += feedback.ProjectOwnerAlias + ",";
                        }
                        if (!string.IsNullOrEmpty(feedback.SeniorManagerAlias) && !feedback.ProjectManagersAliasList.Contains(feedback.SeniorManagerAlias))
                        {
                            ccAddressList += feedback.SeniorManagerAlias + ",";
                        }
                        if (!string.IsNullOrEmpty(feedback.ClientDirectorAlias) && !feedback.ProjectManagersAliasList.Contains(feedback.ClientDirectorAlias))
                        {
                            ccAddressList += feedback.ClientDirectorAlias + ",";
                        }
                        Email(subject, emailBody, true, feedback.ProjectManagersAliasList.Substring(0, feedback.ProjectManagersAliasList.Length - 1), string.Empty, ccAddressList == "" ? "" : ccAddressList.Substring(0, ccAddressList.Length - 1), null);
                        if (WorkerRole.MailsCount % 12 == 0)
                        {
                            Thread.Sleep(65 * 1000);
                        }
                    }
                }

                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, M_FinishedFeedbackRemainderEmails, nextRun);
                //should update next remainder date
            }
            catch (Exception ex)
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, FailedStatus, string.Format(FeedbackRemainderMailFailedFormat, ex.Message + (ex.InnerException != null ? ": " + ex.InnerException.Message : string.Empty)), nextRun);
            }
        }

        /// <summary>
        /// Sends login credentials to the persons.
        /// </summary>
        /// <param name="persons">Persons,Who have (today - 1) as hire date.</param>
        private static void SendWelComeEmails(List<Person> persons, DateTime currentWithTimeZone, DateTime nextRun)
        {
            try
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, M_StartedWelcomeEmails, nextRun);
                var welcomeEmailTemplate = GetEmailTemplate(WelcomeEmailTemplate);
                var companyName = GetCompanyName();
                var smtpSettings = GetSMTPSettings();

                foreach (Person person in persons)
                {
                    var password = GenerateRandomPassword();
                    var encodedPassword = EncodePasswordWithoutHash(password);

                    PersonEncodedPasswordInsert(person.Id.Value, encodedPassword);

                    var emailBody = String.Format(welcomeEmailTemplate.Body, person.FirstName, companyName, person.Alias, password, LoginPagePath, smtpSettings.PMSupportEmail);
                    Email(welcomeEmailTemplate.Subject, emailBody, true, person.Alias, EmailBccRecieverList, string.Empty, null);
                    UpdateIsWelcomeEmailSentForPerson(person.Id.Value);
                }

                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, M_FinishedWelcomeEmails, nextRun);
            }
            catch (Exception ex)
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, FailedStatus, string.Format(WelcomeEmailFailedFormat, ex.Message), nextRun);
            }
        }

        /// <summary>
        /// Sends activated and deactivated account emails.
        /// </summary>
        /// <param name="terminatedPersons">Persons,who have terminated due to pay or due to termination date.</param>
        private static void SendActivateAndDeactivateAccountEmails(List<Person> terminatedPersons, DateTime currentWithTimeZone, DateTime nextRun)
        {
            try
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, M_StartedActivateDeactivateEmails, nextRun);
                var deactivateAccountEmailTemplates = GetEmailTemplate(DeActivateAccountTemplate);
                var activeAccountEmailTemplate = GetEmailTemplate(ActivateAccountTemplate);
                var administratorAddedEmailTemplate = GetEmailTemplate(AdministratorAddedTemplate);

                foreach (Person person in terminatedPersons)
                {
                    var emailBody = string.Format(deactivateAccountEmailTemplates.Body, person.FirstName, person.LastName, person.TerminationDate.Value.ToString(DateFormat));
                    Email(deactivateAccountEmailTemplates.Subject, emailBody, true, deactivateAccountEmailTemplates.EmailTemplateTo, string.Empty, string.Empty, null);

                    if (person.IsTerminatedDueToPay)//rehire due to compensation change from contract to employee
                    {
                        var activeAccountEmailBody = string.Format(activeAccountEmailTemplate.Body, person.FirstName, person.LastName, person.HireDate.ToString(DateFormat), person.Alias, person.Title.TitleName, person.CurrentPay.TimescaleName, person.TelephoneNumber, person.IsOffshore ? "Yes" : "No", person.Manager!=null?person.Manager.FirstName:string.Empty,person.DivisionType.ToString());
                        Email(activeAccountEmailTemplate.Subject, activeAccountEmailBody, true, activeAccountEmailTemplate.EmailTemplateTo, string.Empty, string.Empty, null);
                        if (person.IsAdmin || person.Seniority.Name == Constants.SeniorityNames.AdminiSeniorityName)
                        {
                            var administartorAddedEmail = string.Format(administratorAddedEmailTemplate.Body, person.FirstName, person.LastName);
                            Email(administratorAddedEmailTemplate.Subject, administartorAddedEmail, true, administratorAddedEmailTemplate.EmailTemplateTo, string.Empty, string.Empty, null, true);
                        }
                    }
                }
                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, M_FinishedActivateDeactivateEmails, nextRun);
            }
            catch (Exception ex)
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, FailedStatus, string.Format(ActivateDeactivateEmailsFailedFormat, ex.Message), nextRun);
            }
        }

        /// <summary>
        /// Updates person's IsWelcomeEmailSent to 1.
        /// </summary>
        /// <param name="personId">person id.</param>
        public static void UpdateIsWelcomeEmailSentForPerson(int personId)
        {
            SqlConnection connection = null;
            try
            {
                var connectionString = WorkerRole.GetConnectionString();
                connection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("dbo.UpdateIsWelcomeEmailSentForPerson", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlParameter personIdParam = new SqlParameter("@PersonId", personId);
                cmd.Parameters.Add(personIdParam);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                throw ex;
            }
        }

        /// <summary>
        /// Gets company name,Which is used in welcome email body.
        /// </summary>
        /// <returns>Company name.</returns>
        public static string GetCompanyName()
        {
            SqlConnection connection = null;
            try
            {
                var connectionString = WorkerRole.GetConnectionString();
                connection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("dbo.GetCompanyName", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                connection.Open();
                return (string)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                connection.Close();
                throw ex;
            }
        }

        /// <summary>
        /// Reads a list of persons.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="persons"></param>
        private static void ReadPersons(SqlDataReader reader, List<Person> persons)
        {
            if (reader.HasRows)
            {
                int personFirstNameIndex = reader.GetOrdinal("FirstName");
                int personLastNameIndex;
                int personTerminationDateIndex;
                int personAliasIndex;
                int personIdIndex;
                int personTerminatedDueToPayIndex;
                int personReHireDateIndex;
                int personTelephoneNumberIndex;
                int personTimeScaleNameIndex;
                int personTitleNameIndex;
                int isAdministratorIndex;
                int seniorityNameIndex;
                int isOffShoreIndex;
                int managerIdIndex;
                int managerNameIndex;
                int divisionIdIndex;

                try
                {
                    isOffShoreIndex = reader.GetOrdinal("IsOffshore");
                }
                catch
                {
                    isOffShoreIndex = -1;
                }
                try
                {
                    managerIdIndex = reader.GetOrdinal("ManagerId");
                }
                catch
                {
                    managerIdIndex = -1;
                }
                try
                {
                    managerNameIndex = reader.GetOrdinal("ManagerName");
                }
                catch
                {
                    managerNameIndex = -1;
                }
                try
                {
                    divisionIdIndex = reader.GetOrdinal("DivisionId");
                }
                catch
                {
                    divisionIdIndex = -1;
                }

                try
                {
                    personIdIndex = reader.GetOrdinal("PersonId");
                }
                catch
                {
                    personIdIndex = -1;
                }
                try
                {
                    personLastNameIndex = reader.GetOrdinal("LastName");
                }
                catch
                {
                    personLastNameIndex = -1;
                }
                try
                {
                    personTerminationDateIndex = reader.GetOrdinal("TerminationDate");
                }
                catch
                {
                    personTerminationDateIndex = -1;
                }
                try
                {
                    personAliasIndex = reader.GetOrdinal("Alias");
                }
                catch
                {
                    personAliasIndex = -1;
                }
                try
                {
                    personTerminatedDueToPayIndex = reader.GetOrdinal("IsTerminatedDueToPay");
                }
                catch
                {
                    personTerminatedDueToPayIndex = -1;
                }

                try
                {
                    personReHireDateIndex = reader.GetOrdinal("ReHiredate");
                }
                catch
                {
                    personReHireDateIndex = -1;
                }
                try
                {
                    personTelephoneNumberIndex = reader.GetOrdinal("TelephoneNumber");
                }
                catch
                {
                    personTelephoneNumberIndex = -1;
                }
                try
                {
                    personTimeScaleNameIndex = reader.GetOrdinal("TimeScaleName");
                }
                catch
                {
                    personTimeScaleNameIndex = -1;
                }
                try
                {
                    personTitleNameIndex = reader.GetOrdinal("TitleName");
                }
                catch
                {
                    personTitleNameIndex = -1;
                }
                try
                {
                    isAdministratorIndex = reader.GetOrdinal("isAdministrator");
                }
                catch
                {
                    isAdministratorIndex = -1;
                }
                try
                {
                    seniorityNameIndex = reader.GetOrdinal("SeniorityName");
                }
                catch
                {
                    seniorityNameIndex = -1;
                }

                while (reader.Read())
                {
                    var person = new Person
                    {
                        FirstName = reader.GetString(personFirstNameIndex)
                    };
                    if (personLastNameIndex != -1)
                    {
                        person.LastName = reader.GetString(personLastNameIndex);
                    }
                    if (personTerminationDateIndex != -1)
                    {
                        person.TerminationDate = reader.GetDateTime(personTerminationDateIndex);
                    }

                    if (personAliasIndex != -1)
                    {
                        person.Alias = reader.GetString(personAliasIndex);
                    }

                    if (personIdIndex != -1)
                    {
                        person.Id = reader.GetInt32(personIdIndex);
                    }
                    if (personTerminatedDueToPayIndex != -1)
                    {
                        person.IsTerminatedDueToPay = reader.GetBoolean(personTerminatedDueToPayIndex);
                    }
                    if (personReHireDateIndex != -1)
                    {
                        person.HireDate = reader.IsDBNull(personReHireDateIndex) ? DateTime.Now : reader.GetDateTime(personReHireDateIndex);
                    }
                    if (personTelephoneNumberIndex != -1)
                    {
                        person.TelephoneNumber = reader.IsDBNull(personTelephoneNumberIndex) ? null : reader.GetString(personTelephoneNumberIndex);//update
                    }

                    if (personTimeScaleNameIndex != -1)
                    {
                        person.CurrentPay = new Pay
                        {
                            TimescaleName = reader.IsDBNull(personTimeScaleNameIndex) ? null : reader.GetString(personTimeScaleNameIndex)
                        };
                    }
                    if (personTitleNameIndex != -1)
                    {
                        person.Title = new Title
                        {
                            TitleName = reader.IsDBNull(personTitleNameIndex) ? null : reader.GetString(personTitleNameIndex)
                        };
                    }
                    if (isAdministratorIndex != -1)
                    {
                        person.IsAdmin = reader.GetInt32(isAdministratorIndex) == 1;
                    }
                    if (seniorityNameIndex != -1)
                    {
                        person.Seniority = new Seniority
                        {
                            Name = reader.IsDBNull(seniorityNameIndex) ? null : reader.GetString(seniorityNameIndex)
                        };
                    }
                    if (isOffShoreIndex != -1)
                    {
                        person.IsOffshore = reader.IsDBNull(isOffShoreIndex) ? false : reader.GetInt32(isOffShoreIndex) == 1;
                    }
                    if (managerIdIndex != -1 && !reader.IsDBNull(managerIdIndex) && !reader.IsDBNull(managerNameIndex))
                    {
                        person.Manager = new Person()
                        {
                            Id = reader.GetInt32(managerIdIndex),
                            FirstName = reader.GetString(managerNameIndex)
                        };
                    }
                    if (divisionIdIndex != -1 && !reader.IsDBNull(divisionIdIndex))
                    {
                        person.DivisionType = (PersonDivisionType)reader.GetInt32(divisionIdIndex);
                    }
                    persons.Add(person);
                }
            }
        }

        private static void ReadProjectFeedback(SqlDataReader reader, List<ProjectFeedbackMail> feedbacks)
        {
            if (reader.HasRows)
            {
                int firstNameIndex = reader.GetOrdinal("FirstName");
                int lastNameIndex = reader.GetOrdinal("LastName");
                int personIdIndex = reader.GetOrdinal("PersonId");
                int titleIdIndex = reader.GetOrdinal("TitleId");
                int titleIndex = reader.GetOrdinal("Title");
                int reviewStartDateIndex = reader.GetOrdinal("ReviewPeriodStartDate");
                int reviewEndDateIndex = reader.GetOrdinal("ReviewPeriodEndDate");
                int dueDateIndex = reader.GetOrdinal("DueDate");
                int projectIdIndex = reader.GetOrdinal("ProjectId");
                int projectNameIndex = reader.GetOrdinal("ProjectName");
                int projectNumberIndex = reader.GetOrdinal("ProjectNumber");
                int projectManagersAliasesIndex = reader.GetOrdinal("ToAddressList");
                int directorAliasIndex = reader.GetOrdinal("DirectorAlias");
                int projectOwnerAliasIndex = reader.GetOrdinal("ProjectOwnerAlias");
                int seniorManagerAliasIndex = reader.GetOrdinal("SeniorManagerAlias");

                while (reader.Read())
                {
                    var projectId = reader.GetInt32(projectIdIndex);
                    var feedback = new ProjectFeedback
                    {
                        Person = new Person()
                        {
                            Id = reader.GetInt32(personIdIndex),
                            FirstName = reader.GetString(firstNameIndex),
                            LastName = reader.GetString(lastNameIndex),
                            Title = new Title()
                            {
                                TitleId = !reader.IsDBNull(titleIdIndex) ? reader.GetInt32(titleIdIndex) : -1,
                                TitleName = !reader.IsDBNull(titleIndex) ? reader.GetString(titleIndex) : string.Empty
                            }
                        },
                        ReviewStartDate = reader.GetDateTime(reviewStartDateIndex),
                        ReviewEndDate = reader.GetDateTime(reviewEndDateIndex),
                        DueDate = reader.GetDateTime(dueDateIndex)
                    };
                    if (feedbacks.Any(p => p.Project.Id.Value == projectId))
                    {
                        var mail = feedbacks.FirstOrDefault(p => p.Project.Id.Value == projectId);
                        mail.Resources.Add(feedback);
                    }
                    else
                    {
                        var feedbackMail = new ProjectFeedbackMail()
                        {
                            Project = new Project()
                            {
                                Id = reader.GetInt32(projectIdIndex),
                                Name = reader.GetString(projectNameIndex),
                                ProjectNumber = reader.GetString(projectNumberIndex)
                            },
                            ProjectManagersAliasList = !reader.IsDBNull(projectManagersAliasesIndex) ? reader.GetString(projectManagersAliasesIndex) : string.Empty,
                            ClientDirectorAlias = !reader.IsDBNull(directorAliasIndex) ? reader.GetString(directorAliasIndex) : string.Empty,
                            ProjectOwnerAlias = !reader.IsDBNull(projectOwnerAliasIndex) ? reader.GetString(projectOwnerAliasIndex) : string.Empty,
                            SeniorManagerAlias = !reader.IsDBNull(seniorManagerAliasIndex) ? reader.GetString(seniorManagerAliasIndex) : string.Empty,
                            Resources = new List<ProjectFeedback>()
                            {
                                feedback
                            }
                        };
                        feedbacks.Add(feedbackMail);
                    }
                }
            }
        }

        /// <summary>
        /// Gets email template from database by emailtemplatename.
        /// </summary>
        /// <param name="emailTemplateName">Email template name.</param>
        /// <returns>EmailTemplate object</returns>
        private static EmailTemplate GetEmailTemplate(string emailTemplateName)
        {
            SqlConnection connection = null;
            EmailTemplate emailTemplate = new EmailTemplate();
            try
            {
                var connectionString = WorkerRole.GetConnectionString();
                connection = new SqlConnection(connectionString);
                SqlCommand cmnd = new SqlCommand("dbo.EmailTemplateGetByName", connection);
                cmnd.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlParameter emailTemplateNameParam = new SqlParameter("@EmailTemplateName", emailTemplateName);
                cmnd.Parameters.Add(emailTemplateNameParam);
                using (var reader = cmnd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int emailTemplateBodyIndex = reader.GetOrdinal("EmailTemplateBody");
                        int emailTemplateSubjectIndex = reader.GetOrdinal("EmailTemplateSubject");
                        int emailTemplateToIndex = reader.GetOrdinal("EmailTemplateTo");
                        while (reader.Read())
                        {
                            emailTemplate = new EmailTemplate
                            {
                                EmailTemplateTo = reader.IsDBNull(emailTemplateToIndex) ? string.Empty : reader.GetString(emailTemplateToIndex),
                                Subject = reader.GetString(emailTemplateSubjectIndex),
                                Body = reader.GetString(emailTemplateBodyIndex)
                            };
                        }
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                throw ex;
            }
            return emailTemplate;
        }

        /// <summary>
        /// Inserts a encoded password into database.
        /// </summary>
        /// <param name="personId">Person id.</param>
        /// <param name="encodedPassword">Encoded password without hash.</param>
        public static void PersonEncodedPasswordInsert(int personId, string encodedPassword)
        {
            SqlConnection connection = null;
            try
            {
                var connectionString = WorkerRole.GetConnectionString();
                connection = new SqlConnection(connectionString);
                SqlCommand cmnd = new SqlCommand("dbo.PersonPasswordInsert", connection);
                cmnd.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlParameter personIdParam = new SqlParameter("@PersonId", personId);
                cmnd.Parameters.Add(personIdParam);
                SqlParameter encodedPasswordParam = new SqlParameter("@encodedPassword", encodedPassword);
                cmnd.Parameters.Add(encodedPasswordParam);
                cmnd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                throw ex;
            }
        }

        /// <summary>
        /// Encodes password.
        /// </summary>
        /// <param name="password">Password to be encoded.</param>
        /// <returns>Encoded password.</returns>
        private static string EncodePasswordWithoutHash(string password)
        {
            string encodedPassword = string.Empty;
            byte[] encode = new byte[password.Length];
            encode = Encoding.UTF8.GetBytes(password);
            encodedPassword = Convert.ToBase64String(encode);
            return encodedPassword;
        }

        /// <summary>
        /// Generates random password,Which is of 7 in length and contains atleast one special character.
        /// </summary>
        /// <returns></returns>
        private static string GenerateRandomPassword()
        {
            int PasswordLength = 7;
            string randomPassword = "";
            string specialChars = "~,!,@,^,*,(,)";
            string allowedChars = "";
            allowedChars = "1,2,3,4,5,6,7,8,9,0";
            allowedChars += "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,";
            allowedChars += "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,";
            allowedChars += "~,!,@,^,*,(,)";
            allowedChars += specialChars;

            char[] seperator = { ',' };
            string[] allowedCharacters = allowedChars.Split(seperator);
            string[] specialCharacters = specialChars.Split(seperator);
            var rand = new System.Random();
            for (int i = 0; i < Convert.ToInt32(PasswordLength); i++)
            {
                randomPassword += allowedCharacters[rand.Next(0, allowedCharacters.Length)];
                if (i == PasswordLength / 2)
                {
                    randomPassword += specialCharacters[rand.Next(0, specialCharacters.Length)];
                    i++;
                }
            }
            return randomPassword;
        }

        #endregion Private Methods

        public static void SendFeedbackCountExceeded(DateTime currentDate)
        {
            var nextRunTime = currentDate.AddDays(1);
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    //Read the data.
                    WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_FTPThresholdExceeded_Started, nextRunTime);
                    var users = PersonDAL.GetUsersForCF(connection);
                    var feedbacks = users.SelectMany(p => p.ErrorFeedbacks).ToList();
                    if (feedbacks != null && feedbacks.Count > 0)
                    {
                        EmailFeedbackCountExceeded(feedbacks, connection, currentDate);
                    }
                    WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_FTPThresholdExceeded_Completed, nextRunTime);
                }
            }
            catch (Exception ex)
            {
                WorkerRole.SaveSchedularLog(currentDate, FailedStatus, string.Format(M_FTPThresholdExceeded_Failed, ex.Message + (ex.InnerException != null ? ": " + ex.InnerException.Message : string.Empty)), currentDate);
            }
        }

        private static void EmailFeedbackCountExceeded(List<ProjectFeedback> feedbacks, SqlConnection connection, DateTime currentDate)
        {
            WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_FTPThresholdExceeded_MailStarted, currentDate);
            foreach (var feedback in feedbacks)
            {
                var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName(FeedbackRequestThresholdExceededTemplate, connection);
                var body = string.Format(emailTemplate.Body, feedback.Person.Name, feedback.Project.ProjectNumber, feedback.Project.Name, feedback.Project.ProjectManagerNames, feedback.ReviewStartDate.ToString(Constants.Formatting.EntryDateFormat), feedback.ReviewEndDate.ToString(Constants.Formatting.EntryDateFormat));
                WorkerRole.Email(emailTemplate.Subject, body, true, emailTemplate.EmailTemplateTo, string.Empty, string.Empty, null);
            }
        }

        private static void EmailFeedfileError(DateTime currentDate)
        {
            WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_FeedFileErrorStarted, currentDate);
            var emailTemplate = GetEmailTemplate(FeedfileErrorTemplate);
            WorkerRole.Email(emailTemplate.Subject, emailTemplate.Body, true, emailTemplate.EmailTemplateTo, string.Empty, string.Empty, null);
            WorkerRole.SaveSchedularLog(currentDate, SuccessStatus, M_FeedFileErrorCompleted, currentDate);
        }

        private static StringBuilder PrepareDivisionTab(DateTime currentWithTimeZone)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, string.Format(StartedDalMethodFormat, "GetCFDivisions"), currentWithTimeZone);
                var data_Division = ConfigurationDAL.GetCFDivisions(connection);
                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, string.Format(SuccessRunningDalMethodFormat, "GetCFDivisions"), currentWithTimeZone);
                var sb = new StringBuilder();
                //Write columns
                sb.Append("#Division ID\tDivision Name\tParent ID\tOwner ID\tActive");
                sb.AppendLine();
                foreach (var division in data_Division)
                {
                    sb.Append(division.DivisionCode + "\t" + division.DivisionName + "\t" + (division.Parent != null ? division.Parent.DivisionCode : string.Empty) + "\t" + string.Empty + "\t" + string.Empty);
                    sb.AppendLine();
                }
                return sb;
            }
        }

        private static StringBuilder PrepareLocationTab(DateTime currentWithTimeZone)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, string.Format(StartedDalMethodFormat, "GetLocations"), currentWithTimeZone);
                var data_Location = ConfigurationDAL.GetLocations(connection);
                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, string.Format(SuccessRunningDalMethodFormat, "GetLocations"), currentWithTimeZone);
                var sb = new StringBuilder();
                //Write columns
                sb.Append("#Location ID\tLocation Name\tParent ID\tActive\tCountry Code\tLine1\tLine2\tCity\tState\tPostal Code\tTime Zone");
                sb.AppendLine();

                foreach (var location in data_Location)
                {
                    sb.Append(location.LocationCode + "\t" + location.LocationName + "\t" + (location.Parent != null ? location.Parent.LocationCode : string.Empty) + "\t" + string.Empty + "\t" + location.Country + "\t" + string.Empty + "\t" + string.Empty + "\t" + string.Empty + "\t" + string.Empty + "\t" + string.Empty + "\t" + location.TimeZone);
                    sb.AppendLine();
                }
                return sb;
            }
        }

        private static StringBuilder PrepareLevelTab(DateTime currentWithTimeZone)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, string.Format(StartedDalMethodFormat, "GetAllTitles"), currentWithTimeZone);
                var data_Level = TitleDAL.GetAllTitles(connection);
                data_Level = data_Level.Where(t => t.PositionId != 0).ToList();
                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, string.Format(SuccessRunningDalMethodFormat, "GetAllTitles"), currentWithTimeZone);
                var sb = new StringBuilder();
                //Write columns
                sb.Append("#Position ID\tPosition Name\tParent ID\tActive");
                sb.AppendLine();

                foreach (var title in data_Level)
                {
                    sb.Append(title.PositionId + "\t" + title.TitleName + "\t" + (title.Parent != null && title.Parent.TitleId != -1 ? title.Parent.TitleId.ToString() : string.Empty) + "\t" + string.Empty);
                    sb.AppendLine();
                }
                return sb;
            }
        }

        private static StringBuilder PrepareCostCentreTab()
        {
            var sb = new StringBuilder();
            //Write columns
            sb.Append("#Cost Center ID\tCost Center Name\tParent ID\tOwner ID\tActive");
            sb.AppendLine();
            sb.Append(string.Empty + "\t" + string.Empty + "\t" + string.Empty + string.Empty + "\t" + string.Empty);
            return sb;
        }

        private static StringBuilder PrepareTypeTab(DateTime currentWithTimeZone)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, string.Format(StartedDalMethodFormat, "GetSalaryPayTypes"), currentWithTimeZone);
                var data_Type = PersonDAL.GetSalaryPayTypes(connection);
                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, string.Format(SuccessRunningDalMethodFormat, "GetSalaryPayTypes"), currentWithTimeZone);
                var sb = new StringBuilder();
                //Write columns
                sb.Append("#Pay ID\tPay Type\tParent ID\tActive");
                sb.AppendLine();

                foreach (var type in data_Type)
                {
                    sb.Append(type.TimescaleCode + "\t" + type.Name + "\t" + string.Empty + "\t" + string.Empty);
                    sb.AppendLine();
                }
                return sb;
            }
        }

        private static StringBuilder PrepareUsersTab(DateTime currentWithTimeZone)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, string.Format(StartedDalMethodFormat, "GetUsersForCF"), currentWithTimeZone);
                var data_Users = PersonDAL.GetUsersForCF(connection);
                WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, string.Format(SuccessRunningDalMethodFormat, "GetUsersForCF"), currentWithTimeZone);
                var sb = new StringBuilder();
                //Write columns
                sb.Append("#User ID\tUsername\tActive\tAbsent\tAllow Reconcile\tPrefix\tFirst Name\tMiddle Name\tLast Name\tSuffix\tEmail\tPhone\tFax\tCountry Code\tLine1\tLine2\tCity\tState\tPostal Code\tDivision ID\tLocation ID\tPosition ID\tCost Center ID\tPay ID\tStatus\tLast Hire Date\tOriginal Hire Date\tRequired Approvals\tApprover ID\tManager ID\tGender\tEthnicity\t");
                for (int i = 1; i <= 8; i++)
                {
                    sb.Append(string.Format("Project{0} ID-NAME\t", i));
                    sb.Append(string.Format("Project Manager {0}\t", i));
                    sb.Append(string.Format("Review Start {0}\t", i));
                    sb.Append(string.Format("Review End {0}\t", i));
                    sb.Append(string.Format("Review Needed {0}\t", i));
                }
                sb.Append("MBO\tPractice Director\tPractice Leadership");
                sb.AppendLine();

                foreach (var person in data_Users)
                {
                    var firstName = string.IsNullOrEmpty(person.PrefferedFirstName) ? person.FirstName : person.PrefferedFirstName;
                    var userId = person.CurrentPay.TimescaleCode == "1099" ? person.EmployeeNumber : person.PaychexID;
                    var personId = person.Id.Value;
                    sb.Append(userId + "\t" + person.Alias + "\t" + string.Empty + "\t" + string.Empty + "\t" + string.Empty + "\t" + string.Empty + "\t" + firstName + "\t" + string.Empty + "\t" + person.LastName + "\t" + string.Empty + "\t" + person.Alias + "\t" + string.Empty + "\t" + string.Empty + "\tUSA" + "\t" + string.Empty + "\t" + string.Empty + "\t" + string.Empty + "\t" + string.Empty + "\t" + string.Empty + "\t" + (person.CFDivision != null ? person.CFDivision.DivisionCode : string.Empty) + "\t" + person.Location.LocationCode + "\t" + person.Title.PositionId + "\t" + string.Empty + "\t" + person.CurrentPay.TimescaleCode + "\t" + string.Empty + "\t" + string.Empty + "\t" + person.HireDate.ToString(Constants.Formatting.EntryDateFormat) + "\t1\t" + string.Empty + "\t" + person.Manager.EmployeeNumber + "\t" + string.Empty + "\t" + string.Empty + "\t");
                    for (int i = 1; i <= 8; i++)
                    {
                        if (person.ProjectFeedbacks != null && person.ProjectFeedbacks.Count >= 1 && person.ProjectFeedbacks.Any(p => p.Count == i))
                        {
                            var feedback = person.ProjectFeedbacks.FirstOrDefault(p => p.Count == i);
                            var projectManager = feedback.Project.ProjectManagerUserId;
                            var engagementManger = feedback.Project.EngagementManagerUserID;
                            var executiveInCharge = feedback.Project.ExecutiveInChargeUserId;

                            var realFeedbackManager = personId == feedback.Project.ExecutiveInChargeId ? string.Empty : ((personId != feedback.Project.ProjectManagerId && feedback.Project.ProjectManagerId != -1) ? projectManager : ((personId != feedback.Project.EngagementManagerID && feedback.Project.EngagementManagerID != -1) ? engagementManger : executiveInCharge));

                            sb.Append(feedback.Project.ProjectNumberName + "\t");
                            sb.Append(realFeedbackManager + "\t"); // Should replace with Employee Numbers of project accesses of the feedback project
                            sb.Append(feedback.ReviewStartDate.ToString(Constants.Formatting.EntryDateFormat) + "\t");
                            sb.Append(feedback.ReviewEndDate.ToString(Constants.Formatting.EntryDateFormat) + "\t");
                            sb.Append("Yes\t");
                        }
                        else
                        {
                            sb.Append(string.Empty + "\t" + string.Empty + "\t" + string.Empty + "\t" + string.Empty + "\t" + string.Empty + "\t");
                        }
                    }
                    sb.Append((person.IsMBO ? "Yes" : "No") + "\t");
                    sb.Append(personId == person.PracticeDirectorId ? string.Empty : person.PracticeDirectorEmployeeNumber + "\t");
                    sb.Append(personId == person.PracticeLeadership.Id ? string.Empty : person.PracticeLeadership.EmployeeNumber);
                    sb.AppendLine();
                }
                return sb;
            }
        }

        private static void UploadDataFeedToFTP(DateTime currentWithTimeZone)
        {
            try
            {
                if (currentWithTimeZone.TimeOfDay < FTPUploadTime)
                {
                    var sleeptime = FTPUploadTime - currentWithTimeZone.TimeOfDay;
                    Thread.Sleep(sleeptime);
                    Sftp sftp = new Sftp(GetConfigValue(FTPHostname_ConfigKey), GetConfigValue(FTPUserName_ConfigKey), GetConfigValue(FTPPassword_ConfigKey));
                    currentWithTimeZone = CurrentPMTime;
                    WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, M_FTPConnectionStarted, currentWithTimeZone);
                    sftp.Connect();
                    WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, M_FTPConnectionCompleted, currentWithTimeZone);
                    var currentFolder = Directory.GetCurrentDirectory();
                    string pathString = System.IO.Path.Combine(currentFolder, "FTP_Files");
                    if (!Directory.Exists(pathString))
                    {
                        System.IO.Directory.CreateDirectory(pathString);
                    }
                    currentWithTimeZone = CurrentPMTime;
                    using (var sw = new StreamWriter(System.IO.Path.Combine(currentFolder, "FTP_Files/Division.txt")))
                    {
                        var sb = PrepareDivisionTab(currentWithTimeZone);
                        sw.Write(sb.ToString());
                    }
                    using (var sw = new StreamWriter(System.IO.Path.Combine(currentFolder, "FTP_Files/Location.txt")))
                    {
                        var sb = PrepareLocationTab(currentWithTimeZone);
                        sw.Write(sb.ToString());
                    }
                    using (var sw = new StreamWriter(System.IO.Path.Combine(currentFolder, "FTP_Files/Level.txt")))
                    {
                        var sb = PrepareLevelTab(currentWithTimeZone);
                        sw.Write(sb.ToString());
                    }
                    using (var sw = new StreamWriter(System.IO.Path.Combine(currentFolder, "FTP_Files/Cost Center.txt")))
                    {
                        var sb = PrepareCostCentreTab();
                        sw.Write(sb.ToString());
                    }
                    using (var sw = new StreamWriter(System.IO.Path.Combine(currentFolder, "FTP_Files/Paytype.txt")))
                    {
                        var sb = PrepareTypeTab(currentWithTimeZone);
                        sw.Write(sb.ToString());
                    }
                    using (var sw = new StreamWriter(System.IO.Path.Combine(currentFolder, "FTP_Files/User.txt")))
                    {
                        var sb = PrepareUsersTab(currentWithTimeZone);
                        sw.Write(sb.ToString());
                    }
                    ArrayList fileList = sftp.GetFileList("Datafeed");
                    int count = 0;
                    foreach (var item in fileList)
                    {
                        if (item.ToString().Contains(".txt"))
                        {
                            count++;
                        }
                    }
                    if (count > 0) //If file exists 
                    {
                        var today = CurrentPMTime.ToString("yyyyMMdd");
                        var todayDirectory = "Datafeed/load_error/" + today;
                        sftp.Mkdir(todayDirectory);
                        WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, M_PlaceInLoadErrorStart, currentWithTimeZone);
                        sftp.Put(Path.Combine(Path.Combine(currentFolder, "FTP_Files"), "Division.txt"), todayDirectory + "/Division.txt");
                        sftp.Put(Path.Combine(Path.Combine(currentFolder, "FTP_Files"), "Location.txt"), todayDirectory + "/Location.txt");
                        sftp.Put(Path.Combine(Path.Combine(currentFolder, "FTP_Files"), "Level.txt"), todayDirectory + "/Level.txt");
                        sftp.Put(Path.Combine(Path.Combine(currentFolder, "FTP_Files"), "Cost Center.txt"), todayDirectory + "/Cost Center.txt");
                        sftp.Put(Path.Combine(Path.Combine(currentFolder, "FTP_Files"), "Paytype.txt"), todayDirectory + "/Paytype.txt");
                        sftp.Put(Path.Combine(Path.Combine(currentFolder, "FTP_Files"), "User.txt"), todayDirectory + "/User.txt");
                        WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, M_PlaceInLoadErrorComplete, currentWithTimeZone);
                        EmailFeedfileError(currentWithTimeZone);
                    }
                    else
                    {
                        WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, M_FTPPlaced, currentWithTimeZone);
                        WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, M_FTPUploadingStarted, currentWithTimeZone);
                        sftp.Put(Path.Combine(Path.Combine(currentFolder, "FTP_Files"), "Division.txt"), "Datafeed/" + "Division.txt");
                        sftp.Put(Path.Combine(Path.Combine(currentFolder, "FTP_Files"), "Location.txt"), "Datafeed/" + "Location.txt");
                        sftp.Put(Path.Combine(Path.Combine(currentFolder, "FTP_Files"), "Level.txt"), "Datafeed/" + "Level.txt");
                        sftp.Put(Path.Combine(Path.Combine(currentFolder, "FTP_Files"), "Cost Center.txt"), "Datafeed/" + "Cost Center.txt");
                        sftp.Put(Path.Combine(Path.Combine(currentFolder, "FTP_Files"), "Paytype.txt"), "Datafeed/" + "Paytype.txt");
                        sftp.Put(Path.Combine(Path.Combine(currentFolder, "FTP_Files"), "User.txt"), "Datafeed/" + "User.txt");
                        WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, M_FTPUploaded, currentWithTimeZone);
                    }
                    sftp.Close();
                    currentWithTimeZone = CurrentPMTime;
                    SendFeedbackCountExceeded(currentWithTimeZone);
                }
            }
            catch (Exception ex)
            {
                WorkerRole.SaveSchedularLog(currentWithTimeZone, FailedStatus, string.Format(M_FTPFailed, ex.Message + (ex.InnerException != null ? ": " + ex.InnerException.Message : string.Empty)), currentWithTimeZone);
            }
        }

    }
}

