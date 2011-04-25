using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PracticeManagementDatabaseUI
{
    public partial class ConnectionSettingsWindow : Form
    {
        public string ServerName
        {
            get
            {
                return tbServerName.Text;
            }
        }

        public string DatabaseName
        {
            get
            {
                return tbDatabaseName.Text;
            }
        }

        public ConnectionSettingsWindow()
        {
            InitializeComponent();
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            var conn = new ConnectionProperties
                    {
                        ServerName = ServerName,
                        DatabaseName = DatabaseName
                    };

            if (Utils.IsConnectionValid(conn))
            {
                MessageBox.Show(
                    PracticeManagementDatabaseUI.Properties.Resources.ConnectionIsValid,
                    Properties.Resources.ConnectionTest);

                btnDeploy.Enabled = true;
            }
            else
                MessageBox.Show(
                    PracticeManagementDatabaseUI.Properties.Resources.ConnectionFailed,
                    Properties.Resources.ConnectionTest,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}

