using System;
using System.Web.Security;
using PraticeManagement.Configuration;
using PraticeManagement.Controls;
using System.Web.UI.WebControls;
using PraticeManagement.PersonService;
using System.Text.RegularExpressions;
namespace PraticeManagement
{
    public partial class ChangePassword : System.Web.UI.Page, System.Web.UI.IPostBackEventHandler
    {

        public const string userNameKey = "UserName";
        public const string PwdKey = "Pwd";
        public const string IsValidUserKey = "IsValidUser";
        public const string ChangePwdFailureText = "Change Password is failed. The user name does not exists in password reset requests list or it's been more than 24 hours that you have requested for Password reset.";

        string Username
        {
            get
            {
                return Request.QueryString[userNameKey];
            }
        }

        string Password
        {
            get
            {
                return Request.QueryString[PwdKey];
            }
        }

        bool IsValidUser
        {
            set
            {
                ViewState[IsValidUserKey] = value;
            }
            get
            {
                if (ViewState[IsValidUserKey] != null)
                {
                    return bool.Parse(ViewState[IsValidUserKey].ToString());
                }
                return false;
            }
        }

        protected void changePassword_OnChangingPassword(object sender, System.Web.UI.WebControls.LoginCancelEventArgs e)
        {
            var ChangePasswordContainer = changePassword.FindControl("ChangePasswordContainerID");
            if (changePassword.DisplayUserName && !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                if (!ValidateNewPassword())
                {
                    goto Cancel;
                }

                if (ValidateCredentialsAndSetPassword())
                {
                    hdnAreCredentialssaved.Value = "true";
                    var newPwdTextBox = ChangePasswordContainer.FindControl("NewPassword") as TextBox;
                    newPwdTextBox.Attributes["value"] = changePassword.NewPassword;
                }
                else
                {
                    msglblchangePasswordDetails.ShowErrorMessage(ChangePwdFailureText);
                }

            Cancel:

                var pwdTextBox = ChangePasswordContainer.FindControl("CurrentPassword") as TextBox;

                pwdTextBox.Attributes["value"] = changePassword.CurrentPassword;

                e.Cancel = true;
            }
        }

        private void LoginUser()
        {
            FormsAuthentication.SetAuthCookie(changePassword.UserName, true);
            UrlRoleMappingElementSection mapping = UrlRoleMappingElementSection.Current;
            if (mapping != null)
            {
                Response.Redirect(mapping.Mapping.FindFirstUrl(Roles.GetRolesForUser(changePassword.UserName)));
            }
        }

        private bool ValidateNewPassword()
        {
            var minRequiredPasswordLength = Membership.MinRequiredPasswordLength;
            var minRequiredNonAlphanumericCharacters = Membership.MinRequiredNonAlphanumericCharacters;
            var regexPasswordVal = "(?=.{" + minRequiredPasswordLength.ToString() +
                      ",})(?=(.*\\W){" + minRequiredNonAlphanumericCharacters.ToString() +
                       ",})";

            if (!Regex.IsMatch(changePassword.NewPassword, regexPasswordVal))
            {
                msglblchangePasswordDetails.ShowErrorMessage(
                    string.Format(changePassword.ChangePasswordFailureText, minRequiredPasswordLength,
                    minRequiredNonAlphanumericCharacters));
                return false;
            }
            return true;
        }

        private bool ValidateCredentialsAndSetPassword()
        {
            bool isValidUser = false;
            using (var service = new PersonServiceClient())
            {
                isValidUser = service.CheckIfTemporaryCredentialsValid(changePassword.UserName, changePassword.CurrentPassword);
                if (isValidUser)
                {
                    service.SetNewPasswordForUser(changePassword.UserName, changePassword.NewPassword);
                }
            }
            return isValidUser;
        }

        protected void Page_Load(object senser, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                UrlRoleMappingElementSection mapping = UrlRoleMappingElementSection.Current;
                var person = DataHelper.CurrentPerson;
                if (person != null && mapping != null)
                {
                    changePassword.ContinueDestinationPageUrl = mapping.Mapping.FindFirstUrl(Roles.GetRolesForUser(person.Alias));
                }

                if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
                {
                    using (var service = new PersonServiceClient())
                    {
                        bool isValidUser = service.CheckIfTemporaryCredentialsValid(Username, Password);
                        if (!isValidUser)
                        {
                            Response.Redirect(Constants.ApplicationPages.ChangePasswordErrorpage);
                        }
                    }
                    changePassword.DisplayUserName = true;
                    changePassword.UserName = Username;
                    var ChangePasswordContainer = changePassword.FindControl("ChangePasswordContainerID");
                    var pwdTextBox = ChangePasswordContainer.FindControl("CurrentPassword") as TextBox;
                    pwdTextBox.Attributes["value"] = Password;
                }
                else
                {
                    if (person == null)
                    {
                        Response.Redirect(Constants.ApplicationPages.LoginPage);
                    }
                }
            }
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            if (hdnAreCredentialssaved.Value == "true" &&
                Membership.ValidateUser(changePassword.UserName, changePassword.NewPassword))
            {
                hdnAreCredentialssaved.Value = "false";
                LoginUser();
            }
            else
            {
                Response.Redirect(Constants.ApplicationPages.ChangePasswordErrorpage);
            }
        }
    }
}

