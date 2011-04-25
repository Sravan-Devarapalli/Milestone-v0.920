namespace PracticeManagementDatabaseUI
{
    partial class ConnectionSettingsWindow
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
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblServerName = new System.Windows.Forms.Label();
            this.tbServerName = new System.Windows.Forms.TextBox();
            this.lblDatabaseName = new System.Windows.Forms.Label();
            this.tbDatabaseName = new System.Windows.Forms.TextBox();
            this.btnDeploy = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Location = new System.Drawing.Point(237, 99);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(114, 23);
            this.btnTestConnection.TabIndex = 6;
            this.btnTestConnection.Text = "Test Connection";
            this.btnTestConnection.UseVisualStyleBackColor = true;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(184, 13);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "Specify database connection settings";
            // 
            // lblServerName
            // 
            this.lblServerName.AutoSize = true;
            this.lblServerName.Location = new System.Drawing.Point(12, 47);
            this.lblServerName.Name = "lblServerName";
            this.lblServerName.Size = new System.Drawing.Size(69, 13);
            this.lblServerName.TabIndex = 3;
            this.lblServerName.Text = "Server Name";
            // 
            // tbServerName
            // 
            this.tbServerName.Location = new System.Drawing.Point(102, 44);
            this.tbServerName.Name = "tbServerName";
            this.tbServerName.Size = new System.Drawing.Size(249, 20);
            this.tbServerName.TabIndex = 4;
            // 
            // lblDatabaseName
            // 
            this.lblDatabaseName.AutoSize = true;
            this.lblDatabaseName.Location = new System.Drawing.Point(12, 76);
            this.lblDatabaseName.Name = "lblDatabaseName";
            this.lblDatabaseName.Size = new System.Drawing.Size(84, 13);
            this.lblDatabaseName.TabIndex = 3;
            this.lblDatabaseName.Text = "Database Name";
            // 
            // tbDatabaseName
            // 
            this.tbDatabaseName.Location = new System.Drawing.Point(102, 73);
            this.tbDatabaseName.Name = "tbDatabaseName";
            this.tbDatabaseName.Size = new System.Drawing.Size(249, 20);
            this.tbDatabaseName.TabIndex = 5;
            // 
            // btnDeploy
            // 
            this.btnDeploy.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnDeploy.Location = new System.Drawing.Point(276, 154);
            this.btnDeploy.Name = "btnDeploy";
            this.btnDeploy.Size = new System.Drawing.Size(75, 23);
            this.btnDeploy.TabIndex = 0;
            this.btnDeploy.Text = "Continue";
            this.btnDeploy.UseVisualStyleBackColor = true;
            // 
            // ConnectionSettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(366, 189);
            this.Controls.Add(this.tbDatabaseName);
            this.Controls.Add(this.tbServerName);
            this.Controls.Add(this.lblDatabaseName);
            this.Controls.Add(this.lblServerName);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnTestConnection);
            this.Controls.Add(this.btnDeploy);
            this.Name = "ConnectionSettingsWindow";
            this.Text = "Database Connection Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblServerName;
        private System.Windows.Forms.TextBox tbServerName;
        private System.Windows.Forms.Label lblDatabaseName;
        private System.Windows.Forms.TextBox tbDatabaseName;
        private System.Windows.Forms.Button btnDeploy;
    }
}
