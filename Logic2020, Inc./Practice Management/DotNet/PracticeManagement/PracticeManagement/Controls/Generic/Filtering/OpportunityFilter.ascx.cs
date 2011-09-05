using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.ContextObjects;
using System.Web.Security;

namespace PraticeManagement.Controls.Generic.Filtering
{
    public partial class OpportunityFilter : System.Web.UI.UserControl
    {
        private const string OpportunityList = "OpportunityList";

        private PraticeManagement.OpportunityList HostingPage
        {
            get { return ((PraticeManagement.OpportunityList)Page); }
        }

        private string SelectedClientIds
        {
            get
            {
                return cblClient.SelectedItems;
            }
            set
            {
                cblClient.SelectedItems = value;
            }
        }

        private string SelectedSalespersonIds
        {
            get
            {
                return cblSalesperson.SelectedItems;
            }
            set
            {
                cblSalesperson.SelectedItems = value;
            }
        }

        private string SelectedGroupIds
        {
            get
            {
                return cblOpportunityGroup.SelectedItems;
            }
            set
            {
                cblOpportunityGroup.SelectedItems = value;
            }
        }

        private string SelectedOpportunityOwnerIds
        {
            get
            {
                return cblOpportunityOwner.SelectedItems;
            }
            set
            {
                cblOpportunityOwner.SelectedItems = value;
            }
        }

        /// <summary>
        /// Gets a text to be searched for.
        /// </summary>
        public string SearchText
        {
            get
            {
                return txtSearchText.Text;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var filter = InitFilter();

                //  If current user is administrator, don't apply restrictions
                var person =
                    Roles.IsUserInRole(
                        DataHelper.CurrentPerson.Alias,
                        DataTransferObjects.Constants.RoleNames.AdministratorRoleName)
                    ? null : DataHelper.CurrentPerson;

                DataHelper.FillClientsAndGroups(
                      cblClient, cblOpportunityGroup);
                DataHelper.FillSalespersonList(
                                person, cblSalesperson,
                                Resources.Controls.AllSalespersonsText,
                                true);
                DataHelper.FillOpportunityOwnerList(cblOpportunityOwner,
                                   "All Owners",
                                   true,
                                   person);


                SetFilterValues(filter);
                

                // If person is not administrator, return list of values when [All] is selected
                //      this is needed because we apply restrictions and don't want
                //      NULL to be returned, because that would mean all and restrictions
                //      are not going to be applied
                if (person != null)
                {
                    cblSalesperson.AllSelectedReturnType = ScrollingDropDown.AllSelectedType.AllItems;
                    cblOpportunityOwner.AllSelectedReturnType = ScrollingDropDown.AllSelectedType.AllItems;
                    cblClient.AllSelectedReturnType = ScrollingDropDown.AllSelectedType.AllItems;
                    cblOpportunityGroup.AllSelectedReturnType = ScrollingDropDown.AllSelectedType.AllItems;
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            SaveFilterSettings();
        }

        public OpportunityFilterSettings GetFilterSettings()
        {
            var filter =
                 new OpportunityFilterSettings
                 {
                     ClientIdsList = SelectedClientIds,
                     OpportunityOwnerIdsList = SelectedOpportunityOwnerIds,
                     SalespersonIdsList = SelectedSalespersonIds,
                     OpportunityGroupIdsList = SelectedGroupIds,
                     ShowActive = chbActive.Checked,
                     ShowExperimental = chbExperimental.Checked,
                     ShowInactive = chbInActive.Checked,
                     ShowLost = chbLost.Checked,
                     ShowWon = chbWon.Checked

                 };
            return filter;
        }

        private void SaveFilterSettings()
        {
            OpportunityFilterSettings filter = GetFilterSettings();
            SerializationHelper.SerializeCookie(filter, OpportunityList);
        }

        private static OpportunityFilterSettings InitFilter()
        {
            return SerializationHelper.DeserializeCookie(OpportunityList) as OpportunityFilterSettings ??
                   new OpportunityFilterSettings();
        }

        private void SetFilterValues(OpportunityFilterSettings filter)
        {
            chbActive.Checked = filter.ShowActive;
            chbExperimental.Checked = filter.ShowExperimental;
            chbInActive.Checked = filter.ShowInactive;
            chbLost.Checked = filter.ShowLost;
            chbWon.Checked = filter.ShowWon;
            SelectedClientIds = filter.ClientIdsList;
            SelectedOpportunityOwnerIds = filter.OpportunityOwnerIdsList;
            SelectedSalespersonIds = filter.SalespersonIdsList;
            SelectedGroupIds = filter.OpportunityGroupIdsList;
        }

        protected void btnUpdateView_Click(object sender, EventArgs e)
        {
            var filter = GetFilterSettings();
            HostingPage.UpDateView(filter);
        }

    }
}

