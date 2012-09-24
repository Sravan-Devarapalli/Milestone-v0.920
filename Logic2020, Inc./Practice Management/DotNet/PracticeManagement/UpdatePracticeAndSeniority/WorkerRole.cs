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

namespace UpdatePracticeAndSeniority
{
    public class WorkerRole : RoleEntryPoint
    {
        public const string applicationSettingsType = "Application";
        public const string timeZoneKey = "TimeZone";
        public const string IsDayLightSavingsKey = "IsDayLightSavingsTimeEffect";

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
                WorkerRole.StartAutomaticUpdation(currentDateTimeWithTimeZoneAfterSleep);
            }
        }

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

        public static void StartAutomaticUpdation(DateTime currentWithTimeZone)
        {
            SqlConnection connection = null;
            var nextRun = GetNextRunDate(currentWithTimeZone);
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
                    SqlParameter errorParam = new SqlParameter("@ErrorMessage", errorMessage);
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
                return;
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
            try
            {
                result = bool.Parse(GetConfigValue("LogEnabled"));

            }
            catch
            {
                result = false;
            }
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

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
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
                    "Error Occured while retrieving data from settings table whth Key '"
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
    }
}

