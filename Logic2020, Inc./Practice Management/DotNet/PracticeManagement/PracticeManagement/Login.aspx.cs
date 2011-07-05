using System;
using System.ServiceModel;
using System.Web.Security;
using DataTransferObjects;
using PraticeManagement.ActivityLogService;
using PraticeManagement.Configuration;
using PraticeManagement.PersonService;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Configuration;
using PraticeManagement.Utils;
namespace PraticeManagement
{
    public partial class Login : System.Web.UI.Page
    {
        #region Constants

        private string MessageLoginErrorUserDoesNotExist = "There is no user in the database with the username {0}";
        private string MessageLoginErrorNotApproved = "Your account has not yet been approved by the site's administrators. Please try again later...";
        private string MessageLoginErrorLockedOut = "You could not be logged in. Please see the System Administrator for help.";
        private string MessageLoginErrorNotActive = "Failed - bad password";
        private string EmptyUserNameErrorMessage = "Please enter your User Name (your e-mail address) in the box above and select the \"Forgot your password?\" link again.";
        private string UserNameWrongFormatErrorMessage = "Your User Name is an e-mail address and should be entered as \"user@domain.com\".  Please correct in the box above and select the \"Forgot your password?\" link again.";
        private string UserNameNotExistsErrorMessage = "The User Name you have entered does not exist in PM.  Please correct your entry or <a href='mailto:{0}'>contact your Administrator</a> to create a new account.";
        private string UserNameLockedOutErrorMessage = "The account for the user name you have entered has been locked out in PM.  Please <a href='mailto:{0}'>contact your Administrator</a> to unlock it.";
        private string ChangePwdAleadyRequestedErrorMessage = "A password reset has already been requested for this User Name within the last 24 hours.  If you have any questions, <a href='mailto:{0}'>please contact your Administrator.</a>";
        private string ChangePwdSuccessMessage = "Your password has been successfully reset and sent to your e-mail.";
        #endregion

        private string PMSupportEmail
        {
            get
            {
                return SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.SMTP,
                                                                   Constants.ResourceKeys.PMSupportEmailAddressKey);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                login.Focus();
            msglblForgotPWDErrorDetails.ClearMessage();
            loginErrorDetails.Text = string.Empty;
            var userNameTextBox = login.FindControl("UserName") as TextBox;
            lnkbtnForgotPwd.OnClientClick =
                string.Format(lnkbtnForgotPwd.OnClientClick, userNameTextBox.ClientID, msglblForgotPWDErrorDetails.ClientID + "_lblMessage");
        }

        private string GetErrorMessageById(int Id)
        {
            switch (Id)
            {
                case 0: return "Success";
                case 1: return string.Format(MessageLoginErrorUserDoesNotExist, login.UserName);
                case 2: return MessageLoginErrorNotApproved;
                case 3: return MessageLoginErrorLockedOut;
                case 4: return MessageLoginErrorNotActive;
                default: return "Login failed";
            }
        }

        protected void login_LoggedIn(object sender, EventArgs e)
        {
            UrlRoleMappingElementSection mapping = UrlRoleMappingElementSection.Current;
            if (mapping != null)
            {
                login.DestinationPageUrl = mapping.Mapping.FindFirstUrl(Roles.GetRolesForUser(login.UserName));
            }
            LogLoginResult(0);

            Session["IsLoggedInthroughLoginPage"] = true;
        }

        private void LogLoginResult(int loginResult)
        {
            string logText;
            string ipAddress = Request.ServerVariables["REMOTE_ADDR"];
            Person person = null;
            using (PersonServiceClient serviceClient = new PersonServiceClient())
            {
                try
                {
                    person = serviceClient.GetPersonByAlias(login.UserName);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
            logText = string.Format(LoginLogMessage,
                                    login.UserName,
                                    person == null ? "[Login/pwd were not correct]" : string.Format("{0} {1}", person.LastName, person.FirstName),
                                    ipAddress,
                                    GetErrorMessageById(loginResult));
            using (var serviceClient = new ActivityLogServiceClient())
            {
                try
                {
                    serviceClient.ActivityLogInsert(1, logText);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        protected void login_LoginError(object sender, EventArgs e)
        {
            MembershipUser user = Membership.GetUser(login.UserName);
            if (user == null)
            {
                loginErrorDetails.Text = string.Format(MessageLoginErrorUserDoesNotExist, login.UserName);
                LogLoginResult(1);
            }
            else if (!user.IsApproved)
            {
                loginErrorDetails.Text = MessageLoginErrorNotApproved;
                LogLoginResult(2);
            }
            else if (user.IsLockedOut)
            {
                loginErrorDetails.Text = MessageLoginErrorLockedOut;
                LogLoginResult(3);
            }
            else
                using (PersonServiceClient serviceClient = new PersonServiceClient())
                {
                    try
                    {
                        LogLoginResult(4);
                        Person person = serviceClient.GetPersonByAlias(login.UserName);
                        if (person != null && person.Status != null && person.Status.Id != (int)PersonStatusType.Active)
                            loginErrorDetails.Text = MessageLoginErrorNotActive;
                    }
                    catch (CommunicationException)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }
        }

        protected void lnkbtnForgotPwd_OnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(login.UserName))
            {
                msglblForgotPWDErrorDetails.ShowErrorMessage(EmptyUserNameErrorMessage);
            }
            else if (!IsValidEmail(login.UserName))
            {
                msglblForgotPWDErrorDetails.ShowErrorMessage(UserNameWrongFormatErrorMessage);
            }
            else
            {
                MembershipUser user = Membership.GetUser(login.UserName);
                if (user == null)
                {
                    msglblForgotPWDErrorDetails.ShowErrorMessage(
                        string.Format(UserNameNotExistsErrorMessage, PMSupportEmail));
                }
                else if (user.IsLockedOut)
                {
                    msglblForgotPWDErrorDetails.ShowErrorMessage(
                        string.Format(UserNameLockedOutErrorMessage, PMSupportEmail));
                }
                else
                {
                    using (var service = new PersonServiceClient())
                    {
                        string changePasswordPageUrl =
                            Request.Url.Scheme + "://" + Request.Url.Host
                            + (IsAzureWebRole() ? string.Empty : ":" + Request.Url.Port.ToString())
                            + Request.ApplicationPath + "/ChangePassword.aspx?UserName={0}&Pwd={1}";

                        if (!service.SaveUserTemporaryCredentials(login.UserName, Request.Url.AbsoluteUri, changePasswordPageUrl))
                        {
                            msglblForgotPWDErrorDetails.ShowErrorMessage(
                                string.Format(ChangePwdAleadyRequestedErrorMessage, PMSupportEmail));
                        }
                        else
                        {
                            msglblForgotPWDErrorDetails.ShowInfoMessage(ChangePwdSuccessMessage);
                        }
                    }
                }
            }
        }

        bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        private const string LoginLogMessage = @"<Login><NEW_VALUES	Login = ""{0}"" UserName = ""{1}"" IPAddress = ""{2}"" Result = ""{3}""><OLD_VALUES /></NEW_VALUES></Login>";

        private static Boolean IsAzureWebRole()
        {
            try
            {
                if (RoleEnvironment.IsAvailable)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}

