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

        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage("Opportunity");

            var excelData = DataHelper.GetFilteredOpportunities();

            excelGrid.DataSource = excelData;
            excelGrid.DataMember = "excelDataTable";
            excelGrid.DataBind();
            excelGrid.Visible = true;
            GridViewExportUtil.Export("Opportunities.xls", excelGrid);
        }

        private void DatabindOpportunities()
        {
            opportunities.DatabindOpportunities();
        }

        protected void btnResetSort_OnClick(object sender, EventArgs e)
        {
            ofOpportunityList.ResetFilter();
            opportunities.ResetFilter();

            DatabindOpportunities();
        }

        protected override void Display()
        {
            DatabindOpportunities();
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            if (SaveDirty)
            {
                if (opportunities.SaveAllNotes())
                {
                    Redirect(eventArgument);
                }
            }
            else
            {
                Redirect(eventArgument);
            }
        }
    }
}
