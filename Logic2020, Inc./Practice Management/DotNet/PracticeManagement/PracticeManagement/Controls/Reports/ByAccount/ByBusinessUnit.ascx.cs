using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DataTransferObjects.Reports.ByAccount;
using System.Text;
using DataTransferObjects;

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
                if (HostingPage.BusinessUnitsFilteredIds == null)
                {
                    return HostingPage.BusinessUnitIds;
                }

                if (HostingPage.BusinessUnitsFilteredIds != null)
                {
                    return HostingPage.BusinessUnitsFilteredIds;
                }
                HostingPage.BusinessUnitsFilteredIds = cblBusinessUnits.SelectedItems;
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
            HostingPage.BusinessUnitsFilteredIds = cblBusinessUnits.SelectedItems;
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
                report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByBusinessUnit(HostingPage.AccountId, BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value));
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

            HostingPage.TotalProjectHours = (reportData.TotalProjectHours - reportData.BusinessDevelopmentHours) > 0 ? (reportData.TotalProjectHours - reportData.BusinessDevelopmentHours) : 0d;
            HostingPage.TotalProjectedHours = reportData.TotalProjectedHours;
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
                repBusinessUnit.Visible = btnExportToExcel.Enabled = true;
                repBusinessUnit.DataSource = reportDataList;
                repBusinessUnit.DataBind();
                cblBusinessUnits.SaveSelectedIndexesInViewState();
                ImgBusinessUnitFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblBusinessUnits.FilterPopupClientID,
                  cblBusinessUnits.SelectedIndexes, cblBusinessUnits.CheckBoxListObject.ClientID, cblBusinessUnits.WaterMarkTextBoxBehaviorID);
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repBusinessUnit.Visible = btnExportToExcel.Enabled = false;
            }
        }

        private void PopulateFilterPanels(List<BusinessUnitLevelGroupedHours> reportData)
        {
            if (HostingPage.SetSelectedFilters)
            {

                var report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByBusinessUnit(HostingPage.AccountId, HostingPage.BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value));

                var businessUnitList = report.GroupedBusinessUnits.Select(r => new ProjectGroup { Name = r.BusinessUnit.Name, Id = r.BusinessUnit.Id }).Distinct().ToList().OrderBy(s => s.Name).ToArray();

                PopulateBusinessUnitFilter(businessUnitList);

                foreach (ListItem item in cblBusinessUnits.Items)
                {
                    if (reportData.Any(r => r.BusinessUnit.Id.Value.ToString() == item.Value))
                    {
                        item.Selected = true;
                    }
                    else
                    {
                        item.Selected = false;
                    }
                }
            }
            else
            {
                var businessUnitList = reportData.Select(r => new ProjectGroup { Name = r.BusinessUnit.Name, Id = r.BusinessUnit.Id }).Distinct().ToList().OrderBy(s => s.Name).ToArray();
                PopulateBusinessUnitFilter(businessUnitList);
                cblBusinessUnits.SelectAllItems(true);
            }
        }



        private void PopulateBusinessUnitFilter(ProjectGroup[] businessUnits)
        {
            DataHelper.FillListDefault(cblBusinessUnits.CheckBoxListObject, "All Business Units", businessUnits, false, "Id", "HtmlEncodedName");

        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(ByAccountByBusinessUnitReportExport);

            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                var report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByBusinessUnit(HostingPage.AccountId, BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value));
                var data = report.GroupedBusinessUnits.ToArray();

                string filterApplied = "Filters applied to columns: ";
                List<string> filteredColoums = new List<string>();
                if (!cblBusinessUnits.AllItemsSelected)
                {
                    filteredColoums.Add("Business Unit");
                }

                var account = ServiceCallers.Custom.Client(c => c.GetClientDetailsShort(HostingPage.AccountId));

                StringBuilder sb = new StringBuilder();
                sb.Append("Account_ByBusinessUnit Report");
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(account.HtmlEncodedName);
                sb.Append("\t");
                sb.Append(account.Code);
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(HostingPage.BusinessUnitsCount + " Business Unit(s)");
                sb.Append("\t");
                sb.Append(HostingPage.ProjectsCount + " Project(s)");
                sb.Append("\t");
                sb.Append(HostingPage.PersonsCount.ToString() == "1" ? HostingPage.PersonsCount + " Person" : HostingPage.PersonsCount + " People");
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
                    sb.Append("# of Active Projects");
                    sb.Append("\t");
                    sb.Append("# of Completed Projects");
                    sb.Append("\t");
                    sb.Append("Projected Hours");
                    sb.Append("\t");
                    sb.Append("Billable");
                    sb.Append("\t");
                    sb.Append("Non-Billable");
                    sb.Append("\t");
                    sb.Append("Actual Hours");
                    sb.Append("\t");
                    sb.Append("BD");
                    sb.Append("\t");
                    sb.Append("Total BU Hours");
                    sb.Append("\t");
                    sb.Append("Percent of Total Hours");
                    sb.Append("\t");
                    sb.AppendLine();

                    //Data
                    foreach (var businessUnitLevelGroupedHours in data)
                    {
                        sb.Append(report.Account.Code);
                        sb.Append("\t");
                        sb.Append(report.Account.HtmlEncodedName);
                        sb.Append("\t");
                        sb.Append(businessUnitLevelGroupedHours.BusinessUnit.Code);
                        sb.Append("\t");
                        sb.Append(businessUnitLevelGroupedHours.BusinessUnit.HtmlEncodedName);
                        sb.Append("\t");
                        sb.Append(businessUnitLevelGroupedHours.ActiveProjectsCount);
                        sb.Append("\t");
                        sb.Append(businessUnitLevelGroupedHours.CompletedProjectsCount);
                        sb.Append("\t");
                        sb.Append(businessUnitLevelGroupedHours.ForecastedHours);
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(businessUnitLevelGroupedHours.BillableHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(businessUnitLevelGroupedHours.NonBillableHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(businessUnitLevelGroupedHours.ActualHours));
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
                    sb.Append("There are no projects with Active or Completed statuses for the report parameters selected.");
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

