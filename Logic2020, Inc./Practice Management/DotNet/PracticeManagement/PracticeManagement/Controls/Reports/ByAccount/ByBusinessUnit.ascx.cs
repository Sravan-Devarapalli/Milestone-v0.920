using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DataTransferObjects.Reports.ByAccount;
using System.Text;

namespace PraticeManagement.Controls.Reports.ByAccount
{
    public partial class ByBusinessUnit : System.Web.UI.UserControl
    {
        #region Properties

        private const string ByAccountByBusinessUnitReportExport = "Account Report By Business Unit";

        private HtmlImage ImgBusinessUnitFilter { get; set; }

        private PraticeManagement.Reporting.AccountSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.AccountSummaryReport)Page); }
        }

        private String BusinessUnitIds
        {
            get
            {
                if (cblBusinessUnits == null || cblBusinessUnits.SelectedItems == null || (cblBusinessUnits.SelectedItems == "" && cblBusinessUnits.SelectedIndexesList.Count > 0))
                {
                    return HostingPage.BusinessUnitIds;
                }

                return cblBusinessUnits.SelectedItems;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            cblBusinessUnits.OKButtonId = btnFilterOK.ClientID;
        }

        protected void repBusinessUnit_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                ImgBusinessUnitFilter = e.Item.FindControl("imgBusinessUnitFilter") as HtmlImage;
            }
        }

        protected void btnFilterOK_OnClick(object sender, EventArgs e)
        {
            PopulateByBusinessUnitReport(false);
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        public void PopulateByBusinessUnitReport(bool isPopulateFilters = true)
        {
            GroupByAccount report;
            if (isPopulateFilters)
            {
                report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByBusinessUnit(HostingPage.AccountId, HostingPage.BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value));
            }
            else
            {
                report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByBusinessUnit(HostingPage.AccountId, BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value));
            }

            DataBindBusinesUnit(report.GroupedBusinessUnits.ToArray(), isPopulateFilters);

            SetHeaderSectionValues(report);
        }

        private void SetHeaderSectionValues(GroupByAccount reportData)
        {
            HostingPage.UpdateHeaderSection = true;

            HostingPage.BusinessUnitsCount = reportData.BusinessUnitsCount;
            HostingPage.ProjectsCount = reportData.ProjectsCount;
            HostingPage.PersonsCount = reportData.PersonsCount;

            HostingPage.TotalProjectHours = reportData.TotalProjectHours;
            HostingPage.BDHours = reportData.BusinessDevelopmentHours;
            HostingPage.BillableHours = reportData.BillableHours;
            HostingPage.NonBillableHours = reportData.NonBillableHours + HostingPage.BDHours;
        }

        public void DataBindBusinesUnit(BusinessUnitLevelGroupedHours[] reportData, bool isPopulateFilters)
        {
            var reportDataList = reportData.ToList();
            if (isPopulateFilters)
            {
                PopulateFilterPanels(reportDataList);
            }
            if (reportDataList.Count > 0 || cblBusinessUnits.Items.Count > 1)
            {
                divEmptyMessage.Style["display"] = "none";
                repBusinessUnit.Visible = true;
                repBusinessUnit.DataSource = reportDataList;
                repBusinessUnit.DataBind();
                cblBusinessUnits.SaveSelectedIndexesInViewState();
                ImgBusinessUnitFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblBusinessUnits.FilterPopupClientID,
                  cblBusinessUnits.SelectedIndexes, cblBusinessUnits.CheckBoxListObject.ClientID, cblBusinessUnits.WaterMarkTextBoxBehaviorID);

            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repBusinessUnit.Visible = false;
            }
        }

        private void PopulateFilterPanels(List<BusinessUnitLevelGroupedHours> reportData)
        {
            PopulateBusinessUnitFilter(reportData);
        }

        private void PopulateBusinessUnitFilter(List<BusinessUnitLevelGroupedHours> reportData)
        {
            var businessUnitList = reportData.Select(r => new { Name = r.BusinessUnit.Name, Id = r.BusinessUnit.Id }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblBusinessUnits.CheckBoxListObject, "All Business Units", businessUnitList.ToArray(), false, "Id", "Name");
            cblBusinessUnits.SelectAllItems(true);
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(ByAccountByBusinessUnitReportExport);

            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                var report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByBusinessUnit(HostingPage.AccountId, cblBusinessUnits.SelectedItems, HostingPage.StartDate.Value, HostingPage.EndDate.Value));
                var data = report.GroupedBusinessUnits.ToArray();

                string filterApplied = "Filters applied to columns: ";
                List<string> filteredColoums = new List<string>();
                if (!cblBusinessUnits.AllItemsSelected)
                {
                    filteredColoums.Add("Business Unit");
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("Account_ByBusinessUnit Report");
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(data.Length + " Business Units");
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(HostingPage.Range);
                sb.Append("\t");
                if (filteredColoums.Count > 0)
                {
                    sb.AppendLine();
                    for (int i = 0; i < filteredColoums.Count; i++)
                    {
                        if (i == filteredColoums.Count - 1)
                            filterApplied = filterApplied + filteredColoums[i] + ".";
                        else
                            filterApplied = filterApplied + filteredColoums[i] + ",";
                    }
                    sb.Append(filterApplied);
                    sb.Append("\t");
                }
                sb.AppendLine();
                sb.AppendLine();

                if (data.Length > 0)
                {
                    //Header
                    sb.Append("Account");
                    sb.Append("\t");
                    sb.Append("Account Name");
                    sb.Append("\t");
                    sb.Append("Business Unit");
                    sb.Append("\t");
                    sb.Append("Business Unit Name");
                    sb.Append("\t");
                    sb.Append("# of Projects");
                    sb.Append("\t");
                    sb.Append("Billable");
                    sb.Append("\t");
                    sb.Append("Non-Billable");
                    sb.Append("\t");
                    sb.Append("BD");
                    sb.Append("\t");
                    sb.Append("Total");
                    sb.Append("\t");
                    sb.Append("Percent of Total Hours");
                    sb.Append("\t");
                    sb.AppendLine();

                    //Data
                    foreach (var businessUnitLevelGroupedHours in data)
                    {
                        sb.Append(report.Account.Code);
                        sb.Append("\t");
                        sb.Append(report.Account.Name);
                        sb.Append("\t");
                        sb.Append(businessUnitLevelGroupedHours.BusinessUnit.Code);
                        sb.Append("\t");
                        sb.Append(businessUnitLevelGroupedHours.BusinessUnit.Name);
                        sb.Append("\t");
                        sb.Append(businessUnitLevelGroupedHours.ProjectsCount);
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(businessUnitLevelGroupedHours.BillableHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(businessUnitLevelGroupedHours.NonBillableHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(businessUnitLevelGroupedHours.BusinessDevelopmentHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(businessUnitLevelGroupedHours.TotalHours));
                        sb.Append("\t");
                        sb.Append(businessUnitLevelGroupedHours.BusinessUnitTotalHoursPercent);
                        sb.Append("\t");
                        sb.AppendLine();
                    }

                }
                else
                {
                    sb.Append("There are no Time Entries towards this range selected.");
                }
                //“TimePeriod_ByProject_DateRange.xls”.  
                var filename = string.Format("Account_ByBusinessUnit_{0}-{1}.xls", HostingPage.StartDate.Value.ToString("MM/dd/yyyy"), HostingPage.EndDate.Value.ToString("MM/dd/yyyy"));
                GridViewExportUtil.Export(filename, sb);
            }
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }
    }
}
