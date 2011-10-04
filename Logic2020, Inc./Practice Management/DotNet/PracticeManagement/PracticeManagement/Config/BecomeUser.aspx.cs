﻿using System;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;
using System.Web.Security;
using PraticeManagement.ActivityLogService;
using PraticeManagement.Controls;
using PraticeManagement.PersonService;
using PraticeManagement.Configuration;
using System.Web;
using System.Web.UI;

namespace PraticeManagement.Config
{
    public partial class BecomeUser : PracticeManagementPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected override void Display()
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

            base.OnPreRender(e);
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
            var oldUserName = DataHelper.CurrentPerson.PersonLastFirstName;
            var userName = ddlBecomeUserList.SelectedValue;
            logImpersonateLogin(oldUserName, ddlBecomeUserList.Items[ddlBecomeUserList.SelectedIndex].Text);

            UserImpersonation.ImpersonateUser(userName, Request.Url.ToString(), oldUserName);

            UrlRoleMappingElementSection mapping = UrlRoleMappingElementSection.Current;
            if (mapping != null)
            {
                Response.Redirect(mapping.Mapping.FindFirstUrl(Roles.GetRolesForUser(userName)));
            }
        }

        protected void logImpersonateLogin(string oldUserName, string newUserName)
        {
            var ipAddress = Request.ServerVariables["REMOTE_ADDR"];
            var logText =
                string.Format(
                    @"<BecomeUser><NEW_VALUES User = ""{0}"" Become = ""{1}"" IPAddress = ""{2}""><OLD_VALUES /></NEW_VALUES></BecomeUser>",
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

