using System;
using System.Net.Mail;
using System.Web.UI;
using DataTransferObjects;
using PraticeManagement.ConfigurationService;
using PraticeManagement.Controls;
using PraticeManagement.Utils;

namespace PraticeManagement.Config
{
    public partial class SMTPSetup : PracticeManagementPageBase
    {
        private const string DefaultPortNumber = "465";
        private const string SavedMessage = "SMTP SetUp details are saved successfully.";
        private const string TestSettingsMessage = "Connected to Mail Server on port {0}.<br /> Authenticated as {1}.<br /> Sending Test Email <br /> Test Successfully Completed.";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateControls();
            }

            btnSave.Enabled = false;
            lblMessage.Text = string.Empty;
            lbloutputBox.Text = string.Empty;
        }

        private void PopulateControls()
        {
            var sMTPSettings = SettingsHelper.GetSMTPSettings();
            txtMailServer.Text = sMTPSettings.MailServer;
            txtPortNumber.Text = sMTPSettings.PortNumber.ToString();
            chbEnableSSl.Checked = sMTPSettings.SSLEnabled;
            txtUserName.Text = sMTPSettings.UserName;
            txtPassword.Attributes["value"] = sMTPSettings.Password;
            txtPasswordSingleLine.Text = sMTPSettings.Password;
            txtPMSupportEmail.Text = sMTPSettings.PMSupportEmail;
        }

        public void ValidateAndSave()
        {
            Page.Validate(valSum.ValidationGroup);

            if (Page.IsValid)
            {
                var sMTPSettings = new SMTPSettings();

                sMTPSettings.MailServer = txtMailServer.Text;
                sMTPSettings.PortNumber = Convert.ToInt32(txtPortNumber.Text);
                sMTPSettings.SSLEnabled = chbEnableSSl.Checked;
                sMTPSettings.UserName = txtUserName.Text;
                sMTPSettings.Password = txtPasswordSingleLine.Text;
                sMTPSettings.PMSupportEmail = txtPMSupportEmail.Text;

                SettingsHelper.SaveSMTPSettings(sMTPSettings);

                lblMessage.Text = SavedMessage;
                PopulateControls();
                btnSave.Enabled = false;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            ValidateAndSave();
        }

        protected void chbEnableSSl_CheckedChanged(object sender, EventArgs e)
        {
            if (chbEnableSSl.Checked)
            {
                txtPortNumber.Text = DefaultPortNumber;
            }
        }

        protected void btnTestSettings_Click(object sender, EventArgs e)
        {
            try
            {
                using (var serviceClient = new ConfigurationServiceClient())
                {
                    var result = serviceClient.VerifySMTPSettings(txtMailServer.Text,
                                                                   Convert.ToInt32(txtPortNumber.Text),
                                                                   chbEnableSSl.Checked,
                                                                   txtUserName.Text,
                                                                   txtPasswordSingleLine.Text,
                                                                   txtPMSupportEmail.Text
                                                                   );

                    if (result == true)
                    {
                        lbloutputBox.Text = String.Format(TestSettingsMessage, txtPortNumber.Text, txtUserName.Text);
                        btnSave.Enabled = true;
                    }

                }
            }
            catch (SmtpException ex)
            {
                lbloutputBox.Text = ex.Message;
                btnSave.Enabled = false;
            }
            catch (Exception ex)
            {
                lbloutputBox.Text = ex.Message;
                btnSave.Enabled = false;
            }
        }

        protected void chbShowCharacters_CheckedChanged(object sender, EventArgs e)
        {
            if (chbShowCharacters.Checked)
            {
                txtPassword.Style["display"] = "none";
                txtPasswordSingleLine.Style["display"] = "inline";
            }
            else
            {
                txtPassword.Style["display"] = "inline";
                txtPasswordSingleLine.Style["display"] = "none";
                txtPassword.Attributes["value"] = txtPasswordSingleLine.Text;
            }
        }

        protected override void Display()
        {
        }
    }
}
