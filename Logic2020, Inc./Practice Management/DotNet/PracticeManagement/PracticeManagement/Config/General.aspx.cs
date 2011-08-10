using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using PraticeManagement.Utils;
using DataTransferObjects;
using System.Web.Configuration;
using System.Web.Security;
using System.Configuration;
using PraticeManagement;
using PraticeManagement.PersonService;


namespace PraticeManagement.Config
{
    public partial class General : PracticeManagementPageBase, IPostBackEventHandler
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ResetDropdowns();
                PopulateControls();
            }

            mlConfirmation.ClearMessage();
        }

        private void ResetDropdowns()
        {
            ddlFailedPasswordAttemptCount.SelectedValue = "3";
            ddlPasswordAttemptWindow.SelectedValue = "15";
            ddlUnlockUser.SelectedValue = "30";
        }

        private void PopulateControls()
        {
            ddloldPassWordCheckCount.SelectedValue = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.OldPasswordCheckCountKey);
            ddlChangePasswordTimeSpanLimit.SelectedValue = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.ChangePasswordTimeSpanLimitInDaysKey);
            bool result = true;
            bool.TryParse(SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.IsLockOutPolicyEnabledKey), out result);
            chbLockOutPolicy.Checked = result;

            if (result)
            {
                ddlFailedPasswordAttemptCount.SelectedValue = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.FailedPasswordAttemptCountKey);
                ddlPasswordAttemptWindow.SelectedValue = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.PasswordAttemptWindowKey);
                ddlUnlockUser.SelectedValue = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.UnlockUserMinituesKey);
            }
            else
            {
                ddlFailedPasswordAttemptCount.SelectedValue = string.Empty;
                ddlPasswordAttemptWindow.SelectedValue = string.Empty;
                ddlUnlockUser.SelectedValue = string.Empty;
            }
            txtFormsAuthenticationTimeOutMin.Text = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.FormsAuthenticationTimeOutKey);
            EnableOrDisableLockoutPolicyControls();
        }

        protected override void Display()
        {
        }

        protected void chbLockOutPolicy_OnCheckedChanged(object sender, EventArgs e)
        {
            EnableOrDisableLockoutPolicyControls();
            if (chbLockOutPolicy.Checked)
            {
                ResetDropdowns();
            }
        }

        private void EnableOrDisableLockoutPolicyControls()
        {
            reqFailedPasswordAttemptCount.Enabled =
            reqPasswordAttemptWindow.Enabled =
            reqUnlockUser.Enabled =
            ddlPasswordAttemptWindow.Enabled = ddlFailedPasswordAttemptCount.Enabled = ddlUnlockUser.Enabled = chbLockOutPolicy.Checked;
        }

        protected void btnSave_OnClick(object sender, EventArgs e)
        {
            bool isSaved = ValidateAndSaveGeneralDetails();
            if (isSaved)
                PopulateControls();
        }

        private bool ValidateAndSaveGeneralDetails()
        {
            bool isSaved = false;

            Page.Validate(vsumSave.ValidationGroup);
            if (Page.IsValid)
            {
                SettingsHelper.SaveResourceKeyValuePairItem(SettingsType.Application, Constants.ResourceKeys.OldPasswordCheckCountKey, ddloldPassWordCheckCount.SelectedValue);
                SettingsHelper.SaveResourceKeyValuePairItem(SettingsType.Application, Constants.ResourceKeys.ChangePasswordTimeSpanLimitInDaysKey, ddlChangePasswordTimeSpanLimit.SelectedValue);
                SettingsHelper.SaveResourceKeyValuePairItem(SettingsType.Application, Constants.ResourceKeys.FailedPasswordAttemptCountKey, ddlFailedPasswordAttemptCount.SelectedValue);
                SettingsHelper.SaveResourceKeyValuePairItem(SettingsType.Application, Constants.ResourceKeys.PasswordAttemptWindowKey, ddlPasswordAttemptWindow.SelectedValue);
                SettingsHelper.SaveResourceKeyValuePairItem(SettingsType.Application, Constants.ResourceKeys.IsLockOutPolicyEnabledKey, chbLockOutPolicy.Checked.ToString());
                SettingsHelper.SaveResourceKeyValuePairItem(SettingsType.Application, Constants.ResourceKeys.UnlockUserMinituesKey, ddlUnlockUser.SelectedValue);
                var formsAuthenticationTimeOut = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.FormsAuthenticationTimeOutKey);
                if (formsAuthenticationTimeOut != txtFormsAuthenticationTimeOutMin.Text)
                {
                    SettingsHelper.SaveResourceKeyValuePairItem(SettingsType.Application, Constants.ResourceKeys.FormsAuthenticationTimeOutKey, txtFormsAuthenticationTimeOutMin.Text);
                    var person = DataHelper.CurrentPerson;
                    if(person!=null)
                    {
                        var ticket = ((System.Web.Security.FormsIdentity)(HttpContext.Current.User.Identity)).Ticket;
                        Generic.SetCustomFormsAuthenticationTicket(person.Alias, ticket.IsPersistent, this.Page,ticket.UserData);
                    }
                }
                using (var serviceClient = new PersonServiceClient())
                {
                    try
                    {
                        serviceClient.RestartCustomMembershipProvider();
                    }
                    catch (Exception ex)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }


                ClearDirty();
                mlConfirmation.ShowInfoMessage("Saved Successfully.");
                isSaved = true;
            }

            return isSaved;

        }

        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            bool result = ValidateAndSaveGeneralDetails();
            if (result)
            {
                Redirect(eventArgument);
            }

        }

        #endregion

    }
}

