using System;
using System.Web.UI;
using DataTransferObjects;

namespace PraticeManagement.Controls
{
    public partial class RestrictionPanel : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack) return;

            DataHelper.FillSalespersonList(
                msddSalespersons,
                Resources.Controls.AllSalespersonsText,
                false);

            DataHelper.FillProjectOwnerList(
                msddPracticeManagers,
                Resources.Controls.AllPracticeMgrsText,
                null, false);

            DataHelper.FillPracticeList(
                msddPractices, Resources.Controls.AllPracticesText);

            DataHelper.FillClientsAndGroupsCheckBoxListInPersonDetailPage(
                msddClients, msddGroups);
        }

        public void ApplyPermissions(PersonPermission permissions)
        {
            ApplyPermissionsToControl(
                msddClients, permissions, PermissionTarget.Client);
            ApplyPermissionsToControl(
                msddGroups, permissions, PermissionTarget.Group);
            ApplyPermissionsToControl(
                msddPractices, permissions, PermissionTarget.Practice);
            ApplyPermissionsToControl(
                msddPracticeManagers, permissions, PermissionTarget.PracticeManager);
            ApplyPermissionsToControl(
                msddSalespersons, permissions, PermissionTarget.Salesperson);
        }

        public PersonPermission GetPermissions()
        {
            var p = new PersonPermission();

            p.SetPermissions(PermissionTarget.Client, msddClients.SelectedValues);
            p.SetPermissions(PermissionTarget.Group, msddGroups.SelectedValues);
            p.SetPermissions(PermissionTarget.Salesperson, msddSalespersons.SelectedValues);
            p.SetPermissions(PermissionTarget.PracticeManager, msddPracticeManagers.SelectedValues);
            p.SetPermissions(PermissionTarget.Practice, msddPractices.SelectedValues);

            return p;
        }

        private static void ApplyPermissionsToControl(
            ScrollingDropDown control, PersonPermission permissions, PermissionTarget pt)
        {
            control.SelectItems(
                permissions.GetPermissions(pt));
        }
    }
}
