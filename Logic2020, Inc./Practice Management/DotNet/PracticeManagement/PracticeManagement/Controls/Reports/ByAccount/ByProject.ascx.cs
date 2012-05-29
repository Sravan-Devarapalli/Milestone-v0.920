using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Web.UI.HtmlControls;
using System.Text;
using DataTransferObjects;
using DataTransferObjects.Reports.ByAccount;

namespace PraticeManagement.Controls.Reports.ByAccount
{
    public partial class ByProject : System.Web.UI.UserControl
    {
        private const string ByAccountByProjectReportExport = "Account Report By Project";

        private HtmlImage ImgBusinessUnitFilter { get; set; }

        private HtmlImage ImgProjectStatusFilter { get; set; }

        public HtmlImage ImgBillingFilter { get; set; }

        private PraticeManagement.Reporting.AccountSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.AccountSummaryReport)Page); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            cblBilling.OKButtonId = cblBusinessUnits.OKButtonId = cblProjectStatus.OKButtonId = btnFilterOK.ClientID;
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected string GetCurrencyFormat(double value)
        {
            return value > 0 ? value.ToString(Constants.Formatting.CurrencyFormat) : "$0";
        }

        protected string GetVarianceSortValue(string variance)
        {
            if (variance.Equals("N/A"))
            {
                return int.MinValue.ToString();
            }
            return variance;
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(ByAccountByProjectReportExport);

            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                var report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByProject(HostingPage.AccountId, cblBusinessUnits.SelectedItems, HostingPage.StartDate.Value, HostingPage.EndDate.Value,
                     cblProjectStatus.SelectedItems, cblBilling.SelectedItemsXmlFormat));

                var data = report.GroupedProjects.ToArray();

                string filterApplied = "Filters applied to columns: ";
                List<string> filteredColoums = new List<string>();
                if (!cblBusinessUnits.AllItemsSelected)
                {
                    filteredColoums.Add("Project");
                }
                if (!cblProjectStatus.AllItemsSelected)
                {
                    filteredColoums.Add("Status");
                }
                if (!cblBilling.AllItemsSelected)
                {
                    filteredColoums.Add("Billing");
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("Account_ByProject Report");
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(data.Length + " Projects");
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
                    sb.Append("Project");
                    sb.Append("\t");
                    sb.Append("Project Name");
                    sb.Append("\t");
                    sb.Append("Status");
                    sb.Append("\t");
                    sb.Append("Billing");
                    sb.Append("\t");
                    sb.Append("Billable");
                    sb.Append("\t");
                    sb.Append("Non-Billable");
                    sb.Append("\t");
                    sb.Append("Total");
                    sb.Append("\t");
                    sb.Append("Project Variance (in Hours)");
                    sb.Append("\t");
                    sb.AppendLine();

                    //Data
                    foreach (var projectLevelGroupedHours in data)
                    {
                        sb.Append(projectLevelGroupedHours.Project.Client.Code);
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.Project.Client.Name);
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.Project.Group.Code);
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.Project.Group.Name);
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.Project.ProjectNumber);
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.Project.Name);
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.Project.Status.Name);
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.BillingType);
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(projectLevelGroupedHours.BillableHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(projectLevelGroupedHours.NonBillableHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(projectLevelGroupedHours.TotalHours));
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.Variance);
                        sb.Append("\t");
                        sb.AppendLine();
                    }

                }
                else
                {
                    sb.Append("There are no Time Entries towards this range selected.");
                }
                //“TimePeriod_ByProject_DateRange.xls”.  
                var filename = string.Format("Account_ByProject_{0}-{1}.xls", HostingPage.StartDate.Value.ToString("MM/dd/yyyy"), HostingPage.EndDate.Value.ToString("MM/dd/yyyy"));
                GridViewExportUtil.Export(filename, sb);
            }
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

        public void PopulateByProjectData(bool isPopulateFilters = true)
        {
            GroupByAccount report;
            if (isPopulateFilters)
            {
                report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByProject(HostingPage.AccountId, HostingPage.BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value, null, null));
            }
            else
            {
                report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByProject(HostingPage.AccountId, cblBusinessUnits.SelectedItems, HostingPage.StartDate.Value, HostingPage.EndDate.Value, cblProjectStatus.SelectedItems, cblBilling.SelectedItemsXmlFormat));
            }

            DataBindProject(report.GroupedProjects.ToArray(), isPopulateFilters);

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
            HostingPage.NonBillableHours = reportData.NonBillableHours;
        }

        protected void repProject_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                ImgBusinessUnitFilter = e.Item.FindControl("imgBusinessUnitFilter") as HtmlImage;
                ImgProjectStatusFilter = e.Item.FindControl("imgProjectStatusFilter") as HtmlImage;
                ImgBillingFilter = e.Item.FindControl("imgBilling") as HtmlImage;
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

            }
        }

        public void DataBindProject(ProjectLevelGroupedHours[] reportData, bool isPopulateFilters)
        {
            if (isPopulateFilters)
            {
                PopulateFilterPanels(reportData);
            }
            if (reportData.Length > 0 || cblBusinessUnits.Items.Count > 1 || cblProjectStatus.Items.Count > 1)
            {
                divEmptyMessage.Style["display"] = "none";
                repProject.Visible = true;
                repProject.DataSource = reportData;
                repProject.DataBind();

                cblBusinessUnits.SaveSelectedIndexesInViewState();
                cblProjectStatus.SaveSelectedIndexesInViewState();
                cblBilling.SaveSelectedIndexesInViewState();

                ImgBusinessUnitFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblBusinessUnits.FilterPopupClientID,
                  cblBusinessUnits.SelectedIndexes, cblBusinessUnits.CheckBoxListObject.ClientID, cblBusinessUnits.WaterMarkTextBoxBehaviorID);
                ImgProjectStatusFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblProjectStatus.FilterPopupClientID,
                  cblProjectStatus.SelectedIndexes, cblProjectStatus.CheckBoxListObject.ClientID, cblProjectStatus.WaterMarkTextBoxBehaviorID);
                ImgBillingFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblBilling.FilterPopupClientID,
                  cblBilling.SelectedIndexes, cblBilling.CheckBoxListObject.ClientID, cblBilling.WaterMarkTextBoxBehaviorID);
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repProject.Visible = false;
            }
        }

        private void PopulateFilterPanels(ProjectLevelGroupedHours[] reportData)
        {
            PopulateBusinessUnitFilter(reportData);
            PopulateProjectStatusFilter(reportData);
            PopulateBillingFilter(reportData);
        }

        private void PopulateBillingFilter(ProjectLevelGroupedHours[] reportData)
        {
            var billingtypes = reportData.Select(r => new { Text = string.IsNullOrEmpty(r.BillingType) ? "Unassigned" : r.BillingType, Value = r.BillingType }).Distinct().ToList().OrderBy(s => s.Value);
            DataHelper.FillListDefault(cblBilling.CheckBoxListObject, "All Billing Types", billingtypes.ToArray(), false, "Value", "Text");
            cblBilling.SelectAllItems(true);
        }

        private void PopulateBusinessUnitFilter(ProjectLevelGroupedHours[] reportData)
        {
            var businessUnits = reportData.Select(r => new { Id = r.Project.Group.Id, Name = r.Project.Group.Name }).Distinct().ToList().OrderBy(s => s.Name).ToArray();
            int height = 17 * businessUnits.Length;
            Unit unitHeight = new Unit((height + 17) > 50 ? 50 : height + 17);
            DataHelper.FillListDefault(cblBusinessUnits.CheckBoxListObject, "All Business Units", businessUnits, false, "Id", "Name");
            cblBusinessUnits.Height = unitHeight;
            cblBusinessUnits.SelectAllItems(true);
        }

        private void PopulateProjectStatusFilter(ProjectLevelGroupedHours[] reportData)
        {
            var projectStatusIds = reportData.Select(r => new { Id = r.Project.Status.Id, Name = r.Project.Status.Name }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblProjectStatus.CheckBoxListObject, "All Status", projectStatusIds.ToArray(), false, "Id", "Name");
            cblProjectStatus.SelectAllItems(true);
        }

        protected void btnFilterOK_OnClick(object sender, EventArgs e)
        {
            PopulateByProjectData(false);
        }


    }
}
