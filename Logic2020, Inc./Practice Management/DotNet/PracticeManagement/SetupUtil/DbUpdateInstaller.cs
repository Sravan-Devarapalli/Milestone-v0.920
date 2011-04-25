using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace SetupUtil
{
	[RunInstaller(true)]
	public class DbUpdateInstaller : Installer
	{
		private const string SqlPathKey = "SOFTWARE\\Microsoft\\Microsoft SQL Server\\90\\Tools\\ClientSetup";
		private const string SqlPathValue = "Path";
		private const string SqlCmdProgramm = "SQLCMD.EXE";

		private const string ServerNameKey = "ServerName";
		private const string DatabaseNameKey = "DatabaseName";
		private const string UserNameKey = "UserName";
		private const string PasswordKey = "Password";
        private const string ASSEMBLY_PATH = "assemblypath";
        private const string COMMAND_LINE_ARGUMENTS = "-S {0} -d {1} -U {2} -P {3} -i \"{4}\" -o \"{5}\"";
        private const string LOG_FILE_EXTENSION = ".log";
        private const string EXCEPTION_MESSAGE = "Cannot apply the update. See Update.log file in the installation folder for details.";
        private const string SCRIPTS_EXTENSION = "*.sql";
        private const string ONE_LEVEL_HIGHER = "..\\";

		#region Methods

		protected override void OnBeforeInstall(IDictionary savedState)
		{
			using (ConnectionPropertiesDialog dialog = new ConnectionPropertiesDialog())
			{
                if (dialog.ShowDialog() == DialogResult.OK && dialog.Test())
                {
                    savedState.Add(ServerNameKey, dialog.ServerName);
                    savedState.Add(DatabaseNameKey, dialog.DatabaseName);
                    savedState.Add(UserNameKey, dialog.UserName);
                    savedState.Add(PasswordKey, dialog.Password);
                }
                else
				{
					throw new Exception("The connection string was not set properly.");
				}                
			}
		}

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            string sqlPath;
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(SqlPathKey, false))
            {
                sqlPath = Path.Combine((string)key.GetValue(SqlPathValue), SqlCmdProgramm);
            }

            string scriptsDirectory = 
                Path.Combine(Path.GetDirectoryName(Context.Parameters[ASSEMBLY_PATH]), ONE_LEVEL_HIGHER);
            foreach (string file in Directory.GetFiles(scriptsDirectory, SCRIPTS_EXTENSION))
            {
                ApplyPatch(stateSaver, sqlPath, file);
            }
        }

        private void ApplyPatch(IDictionary stateSaver, string sqlPath, string scriptPath)
		{
			string logPath =
				Path.ChangeExtension(scriptPath, LOG_FILE_EXTENSION);

			string strArguments =
				string.Format(COMMAND_LINE_ARGUMENTS,
					stateSaver[ServerNameKey],
					stateSaver[DatabaseNameKey],
					stateSaver[UserNameKey],
					stateSaver[PasswordKey],
					scriptPath,
					logPath);

			ProcessStartInfo info = new ProcessStartInfo(sqlPath, strArguments);
			info.CreateNoWindow = true;

			using (Process sqlCmd = Process.Start(info))
			{
				sqlCmd.WaitForExit();
				if (sqlCmd.ExitCode != 0)
				{
					throw new Exception(EXCEPTION_MESSAGE);
				}
			}
		}

		#endregion
	}
}

