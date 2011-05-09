using System;
using System.Data;
using System.ServiceModel;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.OpportunityService;
using PraticeManagement.Controls.Generic.Filtering;
using System.Web.UI;

namespace PraticeManagement
{
    public partial class DiscussionReview2 : PracticeManagementPageBase, IPostBackEventHandler
    {
        protected void ofOpportunityList_OnFilterOptionsChanged(object sender, EventArgs e)
        {
            DatabindOpportunities();
        }


        private void DatabindOpportunities()
        {
            opportunities.DatabindOpportunities();
        }

        protected override void Display()
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DatabindOpportunities();
            }
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            //if (SaveDirty)
            //{
            //    if (opportunities.SaveAllNotes())
            //    {
            //        Redirect(eventArgument);
            //    }
            //}
            //else
            //{
                Redirect(eventArgument);
            //}
        }
    }
}
