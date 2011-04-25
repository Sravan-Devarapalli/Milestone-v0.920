using System;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;
using System.Web.Security;
using System.Web.UI;
using PraticeManagement.ActivityLogService;
using PraticeManagement.PersonService;

namespace PraticeManagement.Controls.Generic
{
    public partial class BecomeUser : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (Request.IsAuthenticated &&
               Thread.CurrentPrincipal != null &&
               Thread.CurrentPrincipal.Identity != null)
            {
                var person = DataHelper.CurrentPerson;
                if (person != null && Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName))
                {
                    dvBecomeUser.Visible = true;
                    FillAndSelect();
                }
            }
        }

        protected void FillAndSelect()
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    DataHelper.FillPersonListForImpersonate(ddlBecomeUserList);
                    ddlBecomeUserList.SelectedIndex = ddlBecomeUserList.Items.IndexOf(
                        ddlBecomeUserList.Items.FindByValue(Thread.CurrentPrincipal.Identity.Name));
                    ddlBecomeUserList.Visible = true;
                    lbBecomeUserOk.Visible = true;
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        protected void lbBecomeUserOk_OnClick(object sender, EventArgs e)
        {
            var userName = ddlBecomeUserList.SelectedValue;
            logImpersonateLogin(Thread.CurrentPrincipal.Identity.Name, userName);
            var identity = new GenericIdentity(userName);
            var roles = Roles.GetRolesForUser(userName);
            var principal = new GenericPrincipal(identity, roles);
            Thread.CurrentPrincipal = principal;
            FormsAuthentication.SetAuthCookie(userName, true);
            Response.Redirect(Constants.ApplicationPages.Calendar);
        }

        protected void logImpersonateLogin(string oldUserName, string newUserName)
        {
            var ipAddress = Request.ServerVariables["REMOTE_ADDR"];
            var logText =
                string.Format(
                    @"<Login><NEW_VALUES user = ""{0}"" become = ""{1}"" IPAddress = ""{2}""><OLD_VALUES /></NEW_VALUES></Login>",
                    oldUserName,
                    newUserName,
                    ipAddress);
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
    }
}
