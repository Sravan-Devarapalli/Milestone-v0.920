namespace SetupUtil
{
	partial class ConnectionPropertiesDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.grbConnection = new System.Windows.Forms.GroupBox();
			this.txtDatabaseName = new System.Windows.Forms.TextBox();
			this.txtServer = new System.Windows.Forms.TextBox();
			this.chbAllowSavingPassword = new System.Windows.Forms.CheckBox();
			this.btnTest = new System.Windows.Forms.Button();
			this.lblPassword = new System.Windows.Forms.Label();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.lblUserName = new System.Windows.Forms.Label();
			this.txtUserName = new System.Windows.Forms.TextBox();
			this.rbtnSql = new System.Windows.Forms.RadioButton();
			this.rbtnIntegrated = new System.Windows.Forms.RadioButton();
			this.lblDatabaseName = new System.Windows.Forms.Label();
			this.lblServerName = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lblHelp = new System.Windows.Forms.Label();
			this.grbConnection.SuspendLayout();
			this.SuspendLayout();
			// 
			// grbConnection
			// 
			this.grbConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.grbConnection.Controls.Add(this.lblHelp);
			this.grbConnection.Controls.Add(this.txtDatabaseName);
			this.grbConnection.Controls.Add(this.txtServer);
			this.grbConnection.Controls.Add(this.chbAllowSavingPassword);
			this.grbConnection.Controls.Add(this.btnTest);
			this.grbConnection.Controls.Add(this.lblPassword);
			this.grbConnection.Controls.Add(this.txtPassword);
			this.grbConnection.Controls.Add(this.lblUserName);
			this.grbConnection.Controls.Add(this.txtUserName);
			this.grbConnection.Controls.Add(this.rbtnSql);
			this.grbConnection.Controls.Add(this.rbtnIntegrated);
			this.grbConnection.Controls.Add(this.lblDatabaseName);
			this.grbConnection.Controls.Add(this.lblServerName);
			this.grbConnection.Location = new System.Drawing.Point(-1, 0);
			this.grbConnection.Name = "grbConnection";
			this.grbConnection.Size = new System.Drawing.Size(405, 441);
			this.grbConnection.TabIndex = 0;
			this.grbConnection.TabStop = false;
			this.grbConnection.Text = "Select Connection Properties";
			// 
			// txtDatabaseName
			// 
			this.txtDatabaseName.CausesValidation = false;
			this.txtDatabaseName.Location = new System.Drawing.Point(77, 157);
			this.txtDatabaseName.Name = "txtDatabaseName";
			this.txtDatabaseName.Size = new System.Drawing.Size(296, 20);
			this.txtDatabaseName.TabIndex = 1;
			this.txtDatabaseName.Text = "PracticeManagement";
			// 
			// txtServer
			// 
			this.txtServer.CausesValidation = false;
			this.txtServer.Location = new System.Drawing.Point(77, 118);
			this.txtServer.Name = "txtServer";
			this.txtServer.Size = new System.Drawing.Size(296, 20);
			this.txtServer.TabIndex = 0;
			this.txtServer.Text = "(local)";
			// 
			// chbAllowSavingPassword
			// 
			this.chbAllowSavingPassword.AutoSize = true;
			this.chbAllowSavingPassword.CausesValidation = false;
			this.chbAllowSavingPassword.Location = new System.Drawing.Point(77, 354);
			this.chbAllowSavingPassword.Name = "chbAllowSavingPassword";
			this.chbAllowSavingPassword.Size = new System.Drawing.Size(136, 17);
			this.chbAllowSavingPassword.TabIndex = 6;
			this.chbAllowSavingPassword.Text = "Allow Saving Password";
			this.chbAllowSavingPassword.UseVisualStyleBackColor = true;
			this.chbAllowSavingPassword.Visible = false;
			// 
			// btnTest
			// 
			this.btnTest.Location = new System.Drawing.Point(77, 396);
			this.btnTest.Name = "btnTest";
			this.btnTest.Size = new System.Drawing.Size(108, 23);
			this.btnTest.TabIndex = 7;
			this.btnTest.Text = "Test Connection";
			this.btnTest.UseVisualStyleBackColor = true;
			this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
			// 
			// lblPassword
			// 
			this.lblPassword.AutoSize = true;
			this.lblPassword.Location = new System.Drawing.Point(18, 316);
			this.lblPassword.Name = "lblPassword";
			this.lblPassword.Size = new System.Drawing.Size(53, 13);
			this.lblPassword.TabIndex = 10;
			this.lblPassword.Text = "Password";
			// 
			// txtPassword
			// 
			this.txtPassword.CausesValidation = false;
			this.txtPassword.Location = new System.Drawing.Point(77, 313);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.Size = new System.Drawing.Size(299, 20);
			this.txtPassword.TabIndex = 5;
			// 
			// lblUserName
			// 
			this.lblUserName.AutoSize = true;
			this.lblUserName.Location = new System.Drawing.Point(42, 273);
			this.lblUserName.Name = "lblUserName";
			this.lblUserName.Size = new System.Drawing.Size(29, 13);
			this.lblUserName.TabIndex = 8;
			this.lblUserName.Text = "User";
			// 
			// txtUserName
			// 
			this.txtUserName.CausesValidation = false;
			this.txtUserName.Location = new System.Drawing.Point(77, 270);
			this.txtUserName.Name = "txtUserName";
			this.txtUserName.Size = new System.Drawing.Size(299, 20);
			this.txtUserName.TabIndex = 4;
			this.txtUserName.Text = "sa";
			// 
			// rbtnSql
			// 
			this.rbtnSql.AutoSize = true;
			this.rbtnSql.CausesValidation = false;
			this.rbtnSql.Location = new System.Drawing.Point(77, 233);
			this.rbtnSql.Name = "rbtnSql";
			this.rbtnSql.Size = new System.Drawing.Size(173, 17);
			this.rbtnSql.TabIndex = 3;
			this.rbtnSql.Text = "Use SQL Server Authentication";
			this.rbtnSql.UseVisualStyleBackColor = true;
			this.rbtnSql.Visible = false;
			this.rbtnSql.CheckedChanged += new System.EventHandler(this.rbtnSql_CheckedChanged);
			// 
			// rbtnIntegrated
			// 
			this.rbtnIntegrated.AutoSize = true;
			this.rbtnIntegrated.CausesValidation = false;
			this.rbtnIntegrated.Location = new System.Drawing.Point(77, 200);
			this.rbtnIntegrated.Name = "rbtnIntegrated";
			this.rbtnIntegrated.Size = new System.Drawing.Size(213, 17);
			this.rbtnIntegrated.TabIndex = 2;
			this.rbtnIntegrated.Text = "Use Windows Integrated Authentication";
			this.rbtnIntegrated.UseVisualStyleBackColor = true;
			this.rbtnIntegrated.Visible = false;
			// 
			// lblDatabaseName
			// 
			this.lblDatabaseName.AutoSize = true;
			this.lblDatabaseName.Location = new System.Drawing.Point(18, 160);
			this.lblDatabaseName.Name = "lblDatabaseName";
			this.lblDatabaseName.Size = new System.Drawing.Size(53, 13);
			this.lblDatabaseName.TabIndex = 3;
			this.lblDatabaseName.Text = "Database";
			// 
			// lblServerName
			// 
			this.lblServerName.AutoSize = true;
			this.lblServerName.Location = new System.Drawing.Point(33, 121);
			this.lblServerName.Name = "lblServerName";
			this.lblServerName.Size = new System.Drawing.Size(38, 13);
			this.lblServerName.TabIndex = 0;
			this.lblServerName.Text = "Server";
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(305, 458);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 8;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(210, 458);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// lblHelp
			// 
			this.lblHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblHelp.Location = new System.Drawing.Point(33, 22);
			this.lblHelp.Name = "lblHelp";
			this.lblHelp.Size = new System.Drawing.Size(340, 81);
			this.lblHelp.TabIndex = 11;
			this.lblHelp.Text = "Please type Server (i.e., DBSERVER or .\\SQLEXPRESS), Database, User and Password." +
				" Then click Test Connection to ensure the correctness.";
			this.lblHelp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ConnectionPropertiesDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(403, 493);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.grbConnection);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "ConnectionPropertiesDialog";
			this.Text = "Practice Management Database";
			this.grbConnection.ResumeLayout(false);
			this.grbConnection.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox grbConnection;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lblServerName;
		private System.Windows.Forms.Label lblDatabaseName;
		private System.Windows.Forms.RadioButton rbtnIntegrated;
		private System.Windows.Forms.RadioButton rbtnSql;
		private System.Windows.Forms.Label lblPassword;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Label lblUserName;
		private System.Windows.Forms.TextBox txtUserName;
		private System.Windows.Forms.Button btnTest;
		private System.Windows.Forms.CheckBox chbAllowSavingPassword;
		private System.Windows.Forms.TextBox txtServer;
		private System.Windows.Forms.TextBox txtDatabaseName;
		private System.Windows.Forms.Label lblHelp;
	}
}
