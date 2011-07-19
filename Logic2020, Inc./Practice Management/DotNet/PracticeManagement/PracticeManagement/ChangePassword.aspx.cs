using System;
using System.Web.Security;
using PraticeManagement.Configuration;
using PraticeManagement.Controls;
using System.Web.UI.WebControls;
using PraticeManagement.PersonService;
using System.Text.RegularExpressions;
using System.Web;
using PraticeManagement.Utils;
using DataTransferObjects;
namespace PraticeManagement
{
    public partial class ChangePassword : System.Web.UI.Page, System.Web.UI.IPostBackEventHandler
    {

        public const string userNameKey = "UserName";
        public const string PwdKey = "Pwd";
        public const string IsValidUserKey = "IsValidUser";
        public const string ChangePwdFailureText = "Change Password is failed. The user name does not exists in password reset requests list or it's been more than 24 hours that you have requested for Password reset.";
        public const string IsNewPasswordMatchedWithOldPasswordFailureText = "Last {0} password(s) cannot be reused.";
        public const string PasswordChangeMorethanOnceFailureText = "Password cannot be changed more than once in {0}.";
       
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

        private bool IsFirstTimeLogin { get; set; }

        protected void changePassword_OnChangingPassword(object sender, System.Web.UI.WebControls.LoginCancelEventArgs e)
        {
            var ChangePasswordContainer = changePassword.FindControl("ChangePasswordContainerID");
            if (changePassword.DisplayUserName && !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                if (!ValidateNewPassword())
                {
                    goto Cancel;
                }

                if (ValidateCredentials())
                {
                    if (!IsNewPasswordMatchedWithOldPassword())
                    {
                        SetNewPasswordForUser();
                        hdnAreCredentialssaved.Value = "true";
                        var newPwdTextBox = ChangePasswordContainer.FindControl("NewPassword") as TextBox;
                        newPwdTextBox.Attributes["value"] = changePassword.NewPassword;
                    }
                    else
                    {
                        var count = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.OldPasswordCheckCountKey);
                        var failureText = string.Empty;
                        failureText = !string.IsNullOrEmpty(count) ? string.Format(IsNewPasswordMatchedWithOldPasswordFailureText, count) : string.Format(IsNewPasswordMatchedWithOldPasswordFailureText, 3);
                        msglblchangePasswordDetails.ShowErrorMessage(failureText);
                    }
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
            else
            {
                bool isValidPassword = Membership.ValidateUser(changePassword.UserName, changePassword.CurrentPassword);//ThulasiramP

                if (isValidPassword)
                {
                    if (IsNewPasswordMatchedWithOldPassword())
                    {
                        var count = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.OldPasswordCheckCountKey);
                        var failureText = string.Empty;
                        failureText = !string.IsNullOrEmpty(count) ? string.Format(IsNewPasswordMatchedWithOldPasswordFailureText, count) : string.Format(IsNewPasswordMatchedWithOldPasswordFailureText, 3);
                        msglblchangePasswordDetails.ShowErrorMessage(failureText);
                        e.Cancel = true;
                    }
                    else
                    {
                        string changePasswordTimeSpanLimitInDays = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.ChangePasswordTimeSpanLimitInDaysKey);

                        if (changePasswordTimeSpanLimitInDays != "*" && changePasswordTimeSpanLimitInDays != string.Empty)
                        {
                            int days = Convert.ToInt32(changePasswordTimeSpanLimitInDays);
                            MembershipUser user = Membership.GetUser(changePassword.UserName);
                            if (user != null && !IsFirstTimeLogin)
                            {
                                TimeSpan daysTs = new TimeSpan(days, 0, 0, 0).Duration();
                                DateTime lastPasswordChanged = user.LastPasswordChangedDate.ToUniversalTime();
                                DateTime now = DateTime.UtcNow.ToUniversalTime();
                                TimeSpan ts = now.Subtract(lastPasswordChanged).Duration();
                                
                                if (ts < daysTs)
                                {
                                    var text = string.Empty;
                                    if (days == 1)
                                    {
                                        text = "24 hours";
                                    }
                                    else
                                    {
                                        text = "1 week";
                                    }
                                    var failureText = string.Empty;
                                    failureText =  string.Format(PasswordChangeMorethanOnceFailureText, text);
                                    msglblchangePasswordDetails.ShowErrorMessage(failureText);
                                    e.Cancel = true;
                                }

                            }
                        }
                    }
                }
                else
                {
                    msglblchangePasswordDetails.ShowErrorMessage("You entered Invalid Password.");
                    e.Cancel = true;
                }
            }
        }

        private void SetNewPasswordForUser()
        {
            using (var service = new PersonServiceClient())
            {
                service.SetNewPasswordForUser(changePassword.UserName, changePassword.NewPassword);
            }
        }

        private bool IsNewPasswordMatchedWithOldPassword()
        {
            var historyList = DataHelper.GetPasswordHistoryByUserName(changePassword.UserName);

            bool isMatchedWithOldPassword = false;

            if (changePassword.CurrentPassword == changePassword.NewPassword)
            {
                isMatchedWithOldPassword = true;
            }

            if (changePassword.DisplayUserName && !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                MembershipUser user = Membership.GetUser(Username);
                if (user != null)
                {
                    bool isPreviousPassword = Membership.ValidateUser(changePassword.UserName, changePassword.NewPassword);
                    if (isPreviousPassword)
                    {
                        isMatchedWithOldPassword = true;
                    }
                }
            }

            if (!isMatchedWithOldPassword)
            {
                foreach (var item in historyList)
                {
                    string hashPassword = DataHelper.GetEncodedPassword(changePassword.NewPassword, item.PasswordSalt);
                    if (hashPassword == item.HashedPassword)
                    {
                        isMatchedWithOldPassword = true;
                        break;
                    }
                }
            }

            return isMatchedWithOldPassword;
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

        private bool ValidateCredentials()
        {
            bool isValidUser = false;
            using (var service = new PersonServiceClient())
            {
                isValidUser = service.CheckIfTemporaryCredentialsValid(changePassword.UserName, changePassword.CurrentPassword);
            }
            return isValidUser;
        }

        protected void Page_Load(object senser, EventArgs e)
        {
            MembershipUser user = Membership.GetUser(HttpContext.Current.User.Identity.Name);
            TimeSpan ts = new TimeSpan(00, 00, 20);
            if (user != null
           && user.CreationDate.Subtract(user.LastPasswordChangedDate).Duration() < ts)
            {
                IsFirstTimeLogin = true;
            }

            if (!Page.IsPostBack)
            {
                if (user != null
               && user.CreationDate.Subtract(user.LastPasswordChangedDate).Duration() < ts)
                {
                    System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "", "alert('This is your first time logging in to Practice Management. You must change your password before continuing!');", true);
                }


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

