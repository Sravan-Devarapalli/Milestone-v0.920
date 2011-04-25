using System;
using System.Web.UI.WebControls;
using DataTransferObjects.TimeEntry;
using PraticeManagement.Utils;
using System.Web;
using PraticeManagement.TimeEntryService;
using PraticeManagement.Controls;
using System.Web.Security;

namespace PraticeManagement.Config
{
    public partial class GeneralConfiguration : PracticeManagementPageBase
    {
        #region "Declarations"

        private bool userIsAdministrator;
        private bool userIsSalesperson;
        private bool userIsRecruiter;
        private bool userIsHR;

        #endregion "Declarations"

        protected override void Display()
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyPrivileges();
        }

        private void VerifyPrivileges()
        {
            userIsAdministrator =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
            userIsSalesperson =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SalespersonRoleName);
            userIsRecruiter =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.RecruiterRoleName);
            userIsHR =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.HRRoleName);
            if (userIsAdministrator)
            {
                tpnlBranding.Visible = true;
                tpnlDefaultManager.Visible = true;
                tpnlPractices.Visible = true;
                tpnlOpportunityPriorities.Visible = true;
            }
            if (userIsAdministrator || userIsSalesperson)
            {
                tpnlClientList.Visible = true;
            }
            if (userIsAdministrator || userIsRecruiter || userIsHR)
            {
                tpnlPersons.Visible = true;
            }
        }

    }
}



