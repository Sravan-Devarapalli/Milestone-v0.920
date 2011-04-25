using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;


namespace PracticeManagementDatabaseUI
{
	[RunInstaller(true)]
	public partial class DeployConnectionSettings : System.Configuration.Install.Installer
	{
		private const string ServerNameKey = "ServerNameKey";
		private const string DatabaseNameKey = "DatabaseNameKey";
        private const string DeploymentFolderName = "Deployment";
        private const string TargetdirContextKey = "setupDir";

		public DeployConnectionSettings()
		{
			InitializeComponent();
		}

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);

            string serverName = this.Context.Parameters["serverName"]; //stateSaver[ServerNameKey].ToString();
            string databaseName = this.Context.Parameters["databaseName"]; //stateSaver[DatabaseNameKey].ToString();

            //MessageBox.Show(string.Format("{0} - {1} - {2}", serverName, databaseName, vsdbcmdFolder));

            Utils.RunDeploymentProcess(
                serverName, 
                databaseName, 
                this.Context.Parameters[TargetdirContextKey].ToString().TrimEnd('\\'));
        }

        //protected override void OnBeforeInstall(IDictionary savedState)
        //{
        //    using (var dialog = new ConnectionSettingsWindow())
        //    {
        //        if (dialog.ShowDialog() == DialogResult.OK)
        //        {
        //            savedState.Add(ServerNameKey, dialog.ServerName);
        //            savedState.Add(DatabaseNameKey, dialog.DatabaseName);
        //        }
        //        else
        //        {
        //            throw new Exception(PracticeManagementDatabaseUI.Properties.Resources.ConnectionSettingsIncorrect);
        //        }
        //    }
        //}
	}
}

