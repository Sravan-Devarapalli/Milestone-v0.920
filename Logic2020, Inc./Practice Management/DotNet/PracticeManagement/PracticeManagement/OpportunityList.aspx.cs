using System;
using System.Data;
using System.ServiceModel;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.OpportunityService;
using PraticeManagement.Controls.Generic.Filtering;
using System.Linq;
using PraticeManagement.Utils;
using System.Web.UI.WebControls;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Collections.Generic;

namespace PraticeManagement
{
    public partial class OpportunityList : PracticeManagementPageBase
    {
        #region Constants

        private const string CurrencyDisplayFormat = "$###,###,###,###,###,##0";
        private const string ExcelDateFormat = "mso-number-format";
        private const string ExcelDateFormatStyle = "mm-dd-yyyy";

        #endregion Constants

        #region Properties

        private Opportunity[] OpportunitiesList
        {
            get
            {
                return DataHelper.GetFilteredOpportunitiesForDiscussionReview2();
            }
        }

        private Dictionary<string, int> PriorityTrendList
        {
            get
            {
                using (var serviceClient = new OpportunityService.OpportunityServiceClient())
                {
                    return serviceClient.GetOpportunityPriorityTransitionCount(Constants.Dates.HistoryDays);
                }
            }
        }

        private Dictionary<string, int> StatusChangesList
        {
            get
            {
                using (var serviceClient = new OpportunityService.OpportunityServiceClient())
                {
                    return serviceClient.GetOpportunityStatusChangeCount(Constants.Dates.HistoryDays);
                }
            }
        }


        /// <summary>
        /// Gets a text to be searched for.
        /// </summary>
        public string SearchText
        {
            get
            {
                return ofOpportunityList.SearchText;
            }
        }

        #endregion

        private void DatabindOpportunities(OpportunityFilterSettings filter = null)
        {
            opportunities.DatabindOpportunities(filter);
        }

        protected override void Display()
        {
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var filter = ofOpportunityList.GetFilterSettings();
                UpDateView(filter);
            }
        }

        protected void ofOpportunityList_OnFilterOptionsChanged(object sender, EventArgs e)
        {
            var filter = ofOpportunityList.GetFilterSettings();
            UpDateView(filter);
        }

        internal void UpDateView(OpportunityFilterSettings filter)
        {
            DatabindOpportunities(filter);
            var summary = GetSummaryDetails();
            pnlSummary.Controls.Add(summary);
        }

        #region Export

        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage("Opportunity");

            var opportunitiesList = DataHelper.GetFilteredOpportunities();

            var opportunitiesData = (from opp in opportunitiesList
                                     where opp != null
                                     select new
                                     {
                                         Id = opp.Id != null ? opp.Id.ToString() : string.Empty,
                                         OpportunityNumber = opp.OpportunityNumber != null ? opp.OpportunityNumber.ToString() : string.Empty,
                                         Priority = opp.Priority != null ? opp.Priority.Priority : string.Empty,
                                         ProjectedStartDate = opp.ProjectedStartDate != null ? opp.ProjectedStartDate.Value.ToShortDateString() : string.Empty,
                                         ClientAndGroup = opp.ClientAndGroup != null ? opp.ClientAndGroup.ToString() : string.Empty,
                                         Buyer = opp.BuyerName != null ? opp.BuyerName.ToString() : string.Empty,
                                         Name = opp.Name != null ? opp.Name.ToString() : string.Empty,
                                         EstimatedRevenue = opp.EstimatedRevenue != null ? opp.EstimatedRevenue.ToString() : string.Empty,
                                         SalesTeam = opp.Salesperson != null ? opp.Salesperson.PersonLastFirstName : string.Empty,
                                         Owner = opp.Owner != null ? opp.Owner.PersonLastFirstName : string.Empty,
                                         CreateDate = opp.CreateDate.ToShortDateString(),
                                         LastUpdate = opp.LastUpdate.ToShortDateString(),
                                         ProjectId = opp.ProjectId != null ? opp.ProjectId.ToString() : string.Empty,
                                         OpportunityIndex = opp.OpportunityIndex != null ? opp.OpportunityIndex.ToString() : string.Empty,
                                         ProjectedEndDate = opp.ProjectedEndDate != null ? opp.ProjectedEndDate.ToString() : string.Empty,
                                         Description = opp.Description != null ? opp.Description.ToString() : string.Empty,
                                         Pipeline = opp.Pipeline != null ? opp.Pipeline.ToString() : string.Empty,
                                         SendOut = opp.SendOut != null ? opp.SendOut.ToString() : string.Empty,
                                         DaysOld = opp.DaysOld.ToString(),
                                         LastChange = opp.LastChange.ToString(),
                                         ProposedPersonIdList = opp.ProposedPersonIdList != null ? opp.ProposedPersonIdList.ToString() : string.Empty,
                                         OutSideResources = opp.OutSideResources != null ? opp.OutSideResources.ToString() : string.Empty,
                                     }).ToList(); //Note If you add new property, then change the header in RowDataBound.

            excelGrid.DataSource = opportunitiesData;
            excelGrid.RowDataBound += new GridViewRowEventHandler(excelGrid_RowDataBound);
            excelGrid.DataMember = "excelDataTable";
            excelGrid.DataBind();
            excelGrid.Visible = true;


            var summaryDetails = GetSummaryDetails(true);
            GridViewExportUtil.Export("Opportunities.xls", excelGrid, summaryDetails, "Logic20/20 Opportunities");
        }

        private Table GetSummaryDetails(bool isExporting = false)
        {
            return OpportunitiesHelper.GetFormatedSummaryDetails(OpportunitiesList, PriorityTrendList, StatusChangesList, isExporting);
        }

        public void excelGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int cellsCount = e.Row.Cells.Count;
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Text = "Opportunity Number";
                e.Row.Cells[3].Text = "Start Date";
                e.Row.Cells[4].Text = "Client - Group";
                e.Row.Cells[6].Text = "Opportunity Name";
                e.Row.Cells[7].Text = "Est.Revenue";
                e.Row.Cells[8].Text = "Sales Team";
                e.Row.Cells[9].Text = "Owner";
                e.Row.Cells[10].Text = "Created On";
                e.Row.Cells[11].Text = "Last Updated";

                for (int i = 12; i < cellsCount; i++)
                {
                    e.Row.Cells.RemoveAt(12);
                }
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                decimal? estRevenue = null;
                string revenueText = e.Row.Cells[7].Text;
                string startdate = e.Row.Cells[3].Text;

                e.Row.Cells[3].Style.Add(ExcelDateFormat, ExcelDateFormatStyle);
                e.Row.Cells[10].Style.Add(ExcelDateFormat, ExcelDateFormatStyle);
                e.Row.Cells[11].Style.Add(ExcelDateFormat, ExcelDateFormatStyle);
                if (!(string.IsNullOrEmpty(revenueText) || revenueText == "&nbsp;"))
                {
                    estRevenue = Convert.ToDecimal(revenueText);
                }
                e.Row.Cells[7].Text = estRevenue.HasValue ? estRevenue.Value.ToString(CurrencyDisplayFormat) : string.Empty;
                for (int i = 12; i < cellsCount; i++)
                {
                    e.Row.Cells.RemoveAt(12);
                }

            }
            e.Row.Cells.RemoveAt(0); //Removing OpportunityId column in the report.
        }



        #endregion


    }
}

