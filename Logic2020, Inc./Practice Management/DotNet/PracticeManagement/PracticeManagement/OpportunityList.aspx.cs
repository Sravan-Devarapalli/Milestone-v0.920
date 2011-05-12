using System;
using System.Data;
using System.ServiceModel;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.OpportunityService;
using PraticeManagement.Controls.Generic.Filtering;
using System.Linq;
using PraticeManagement.Utils;

namespace PraticeManagement
{
    public partial class OpportunityList : PracticeManagementPageBase
    {
        protected void ofOpportunityList_OnFilterOptionsChanged(object sender, EventArgs e)
        {
            DatabindOpportunities();
        }

        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage("Opportunity");

            var opportunitiesList = DataHelper.GetFilteredOpportunities();

            var opportunitiesData =(from opp in opportunitiesList
                                    where opp != null
                                    select new
                                    {
                                        Id = opp.Id != null ? opp.Id.ToString() : string.Empty,	
                                        ProjectId = opp.ProjectId != null ? opp.ProjectId.ToString() : string.Empty,	
                                        EstimatedRevenue = opp.EstimatedRevenue != null ? opp.EstimatedRevenue.ToString() : string.Empty,		
                                        OpportunityIndex = opp.OpportunityIndex != null ? opp.OpportunityIndex.ToString() : string.Empty,
                                        Name = opp.Name != null ? opp.Name.ToString() : string.Empty,
                                        Priority = opp.Priority != null ? opp.Priority.ToString() : string.Empty,
                                        ProjectedStartDate = opp.ProjectedStartDate != null ? opp.ProjectedStartDate.ToString() : string.Empty,
                                        LastUpdate =opp.LastUpdate.ToString(),
                                        ProjectedEndDate = opp.ProjectedEndDate != null ? opp.ProjectedEndDate.ToString() : string.Empty,
                                        OpportunityNumber = opp.OpportunityNumber != null ? opp.OpportunityNumber.ToString() : string.Empty,
                                        Description = opp.Description != null ? opp.Description.ToString() : string.Empty,
                                        BuyerName = opp.BuyerName != null ? opp.BuyerName.ToString() : string.Empty,
                                        CreateDate = opp.CreateDate.ToString() ,
                                        Pipeline = opp.Pipeline != null ? opp.Pipeline.ToString() : string.Empty,
                                        Proposed = opp.Proposed != null ? opp.Proposed.ToString() : string.Empty,
                                        SendOut = opp.SendOut != null ? opp.SendOut.ToString() : string.Empty,
                                        DaysOld =  opp.DaysOld.ToString(),
                                        LastChange = opp.LastChange.ToString() ,
                                        ProposedPersonIdList = opp.ProposedPersonIdList != null ? opp.ProposedPersonIdList.ToString() : string.Empty,
                                        OutSideResources = opp.OutSideResources != null ? opp.OutSideResources.ToString() : string.Empty,
                                        ClientAndGroup = opp.ClientAndGroup != null ? opp.ClientAndGroup.ToString() : string.Empty,	
                                    }).ToList();


            excelGrid.DataSource = opportunitiesData;
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
    }
}
