using System;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

//using Microsoft.SqlServer.Management.Smo;
//using Microsoft.SqlServer.Management.Common;

namespace SetupUtil
{
	public partial class ConnectionPropertiesDialog : Form
	{
		#region Properties

		/// <summary>
		/// Gets or sets a selected server name.
		/// </summary>
		public string ServerName
		{
			get
			{
				return txtServer.Text;
			}
			set
			{
				txtServer.Text = value;
			}
		}

		/// <summary>
		/// Gets or sets a selected database name.
		/// </summary>
		public string DatabaseName
		{
			get
			{
				return txtDatabaseName.Text;
			}
			set
			{
				txtDatabaseName.Text = value;
			}
		}

		/// <summary>
		/// Gets or sets whether to use the Integrated Authentication.
		/// </summary>
		public bool IsTrustedConnection
		{
			get
			{
				return rbtnIntegrated.Checked;
			}
			set
			{
				rbtnIntegrated.Checked = value;
				rbtnSql.Checked = !value;
			}
		}

		/// <summary>
		/// Gets or sets a SQL Server user name.
		/// </summary>
		public string UserName
		{
			get
			{
				return txtUserName.Text;
			}
			set
			{
				txtUserName.Text = value;
			}
		}

		/// <summary>
		/// Gets or sets a SQL Server user password.
		/// </summary>
		public string Password
		{
			get
			{
				return txtPassword.Text;
			}
			set
			{
				txtPassword.Text = value;
			}
		}

		/// <summary>
		/// Gets or sets whether the password can be saved.
		/// </summary>
		public bool AllowSavingPassword
		{
			get
			{
				return chbAllowSavingPassword.Checked;
			}
			set
			{
				chbAllowSavingPassword.Checked = value;
			}
		}

		/// <summary>
		/// Gets a connection string
		/// </summary>
		public string ConnectionString
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				// Building a connection string
				sb.Append("Data Source=");
				sb.Append(ServerName);
				sb.Append(";");

				sb.Append("Initial Catalog=");
				sb.Append(DatabaseName);
				sb.Append(";");

				if (rbtnIntegrated.Checked)
				{
					sb.Append("Integrated Security=SSPI;");
				}
				else //if(rbtnSql.Checked==true)
				{
					sb.Append("User ID=");
					sb.Append(UserName);
					sb.Append(";");

					sb.Append("Password=");
					sb.Append(Password);
					sb.Append(";");
				}

				return sb.ToString();
			}
		}

		#endregion

		#region Construction

		public ConnectionPropertiesDialog()
		{
			InitializeComponent();
		}

		#endregion

		#region Methods

		private void rbtnSql_CheckedChanged(object sender, EventArgs e)
		{
			txtUserName.Enabled = txtPassword.Enabled = chbAllowSavingPassword.Enabled = rbtnSql.Checked;
		}

		private void btnTest_Click(object sender, EventArgs e)
		{
			if (Test())
			{
				MessageBox.Show("Test succeeded.");
			}
		}

		public bool Test()
		{
			using (SqlConnection connection = new SqlConnection(ConnectionString))
			{
				try
				{
					connection.Open();
					return true;
				}
				catch (SqlException ex)
				{
					MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}
			}
		}

		#endregion
	}
}

