using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.OpportunityService;
using PraticeManagement.Controls;
using DataTransferObjects;

namespace PraticeManagement
{
    public partial class OpportunitySearch : PracticeManagementPageBase
    {

        private List<Opportunity> Opportunities
        {
            get
            {
                if (ViewState["OpportunitiesLooked"] != null)
                    return ViewState["OpportunitiesLooked"] as List<Opportunity>;
                else
                    return null;
            }
            set
            {
                ViewState["OpportunitiesLooked"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && PreviousPage != null)
            {
                txtSearchText.Text = PreviousPage.SearchText;
                DisplaySearch();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DisplaySearch();
        }

        protected void ucOpportunityList_OnFilterOptionsChanged(object sender, EventArgs e)
        {
            var opportunities = DataHelper.SortOppotunities(Opportunities.AsQueryable().ToArray());
            ucOpportunityList.DataBindLookedOpportunities(opportunities.ToArray());
        }

        private void DisplaySearch()
        {
            Page.Validate();
            if (Page.IsValid)
            {

                var opportunities = DataHelper.GetLookedOpportunities(txtSearchText.Text,
                                 DataHelper.CurrentPerson.Id.Value);
                Opportunities = opportunities.ToList();

                ucOpportunityList.DataBindLookedOpportunities(opportunities.AsQueryable().ToArray());


            }
        }

        protected override void Display()
        {

        }
    }
}
