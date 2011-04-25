using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics;

namespace PracticeManagementDatabaseUI
{
    public class Utils
    {
        private const string LogFileName = "upgrade_log.txt";

        private const string CommandLineArgs = "{0} {1} {2}";
        private const string VsdbcmbName = "updatedb.bat";

        private const string ProcessFailedMessage = "Cannot apply the update. See Update.log file in the installation folder for details.";
        private const string PathPlusFile = "{0}\\{1}";

        public static bool IsConnectionValid(ConnectionProperties conn)
        {
            using (SqlConnection connection = new SqlConnection(conn.ConnectionString))
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static void ShowNotepad(string filePath)
        {
            ProcessStartInfo notepadProcessInfo = new ProcessStartInfo("notepad.exe", filePath);

            using (var notepadProcess = Process.Start(notepadProcessInfo))
            {
            }
        }

        public static void RunDeploymentProcess(string serverName, string databaseName)
        {
            RunDeploymentProcess(serverName, databaseName, string.Empty);
        }

        public static void RunDeploymentProcess(string serverName, string databaseName, string vsdbcmbPath)
        {
            string logPath = string.Format(PathPlusFile, vsdbcmbPath, LogFileName);
            string args = string.Format(CommandLineArgs, serverName, databaseName, vsdbcmbPath);

            string execName = string.Format(PathPlusFile, vsdbcmbPath, VsdbcmbName);
            ProcessStartInfo deploymentProcessInfo = new ProcessStartInfo(execName, args);
            deploymentProcessInfo.CreateNoWindow = true;            

            using (var deploymentProcess = Process.Start(deploymentProcessInfo))
            {
                deploymentProcess.WaitForExit();
            }

            ShowNotepad(logPath);
        }
    }
}

