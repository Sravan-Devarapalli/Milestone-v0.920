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
        private string ProjectDetailUrl = "ProjectDetail.aspx?id={0}";
        private string ByProjectUrl = "~/Reports/ProjectSummaryReport.aspx?ProjectNumber={0}&StartDate={1}&EndDate={2}&PeriodSelected={3}";
        private HtmlImage ImgBusinessUnitFilter { get; set; }

        private HtmlImage ImgProjectStatusFilter { get; set; }

        public HtmlImage ImgBillingFilter { get; set; }

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
                var report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByProject(HostingPage.AccountId, BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value,
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

                var account = ServiceCallers.Custom.Client(c => c.GetClientDetailsShort(HostingPage.AccountId));

                StringBuilder sb = new StringBuilder();
                sb.Append("Account_ByProject Report");
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
                    sb.Append("Project");
                    sb.Append("\t");
                    sb.Append("Project Name");
                    sb.Append("\t");
                    sb.Append("Status");
                    sb.Append("\t");
                    sb.Append("Billing Type");
                    sb.Append("\t");
                    sb.Append("Projected Hours");
                    sb.Append("\t");
                    sb.Append("Billable");
                    sb.Append("\t");
                    sb.Append("Non-Billable");
                    sb.Append("\t");
                    sb.Append("Actual Hours");
                    sb.Append("\t");
                    sb.Append("Total Estimated Billings");
                    sb.Append("\t");
                    sb.Append("Billable Hours Variance");
                    sb.Append("\t");
                    sb.AppendLine();

                    //Data
                    foreach (var projectLevelGroupedHours in data)
                    {
                        sb.Append(projectLevelGroupedHours.Project.Client.Code);
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.Project.Client.HtmlEncodedName);
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.Project.Group.Code);
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.Project.Group.HtmlEncodedName);
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.Project.ProjectNumber);
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.Project.HtmlEncodedName);
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.Project.Status.Name);
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.BillingType);
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(projectLevelGroupedHours.ForecastedHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(projectLevelGroupedHours.BillableHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(projectLevelGroupedHours.NonBillableHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(projectLevelGroupedHours.TotalHours));
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.BillingType == "Fixed" ? "FF" : GetDoubleFormat(projectLevelGroupedHours.EstimatedBillings));
                        sb.Append("\t");
                        sb.Append(projectLevelGroupedHours.BillableHoursVariance);
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
                report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByProject(HostingPage.AccountId, BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value, null, null));
            }
            else
            {
                report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByProject(HostingPage.AccountId, BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value, cblProjectStatus.SelectedItems, cblBilling.SelectedItemsXmlFormat));
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

            HostingPage.TotalProjectHours = (reportData.TotalProjectHours - reportData.BusinessDevelopmentHours) > 0 ? (reportData.TotalProjectHours - reportData.BusinessDevelopmentHours) : 0d;
            HostingPage.TotalProjectedHours = reportData.TotalProjectedHours;
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
                var dataItem = (ProjectLevelGroupedHours)e.Item.DataItem;
                var lblEstimatedBillings = e.Item.FindControl("lblEstimatedBillings") as Label;
                var lblActualHours = e.Item.FindControl("lblActualHours") as Label;
                var lblExclamationMark = e.Item.FindControl("lblExclamationMark") as Label;
                var lblProjectName = e.Item.FindControl("lblProjectName") as Label;
                var hlProjectName = e.Item.FindControl("hlProjectName") as HyperLink;
                var hlActualHours = e.Item.FindControl("hlActualHours") as HyperLink;
                lblExclamationMark.Visible = dataItem.BillableHoursVariance < 0;
                lblEstimatedBillings.Text = dataItem.EstimatedBillings == -1 ? "FF" : GetDoubleFormat(dataItem.EstimatedBillings).ToString();
                lblActualHours.Visible = lblProjectName.Visible = dataItem.Project.TimeEntrySectionId == 2 || dataItem.Project.TimeEntrySectionId == 3 || dataItem.Project.TimeEntrySectionId == 4;
                hlActualHours.Visible = hlProjectName.Visible = !(dataItem.Project.TimeEntrySectionId == 2 || dataItem.Project.TimeEntrySectionId == 3 || dataItem.Project.TimeEntrySectionId == 4);
            }
        }

        protected string GetProjectDetailsLink(int? projectId)
        {
            if (projectId.HasValue)
                return Utils.Generic.GetTargetUrlWithReturn(String.Format(Constants.ApplicationPages.DetailRedirectFormat, Constants.ApplicationPages.ProjectDetail, projectId.Value),
                                                            Constants.ApplicationPages.AccountSummaryReport);
            else
                return string.Empty;
        }

        protected string GetReportByProjectLink(string projectNumber)
        {
            if (projectNumber != null)
                return String.Format(ByProjectUrl, projectNumber, (HostingPage.StartDate.HasValue) ? HostingPage.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) : null,
                    (HostingPage.EndDate.HasValue) ? HostingPage.EndDate.Value.Date.ToString(Constants.Formatting.EntryDateFormat) : null, "0");
            else
                return string.Empty;
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
                repProject.Visible = btnExportToExcel.Enabled = true;
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
                repProject.Visible = btnExportToExcel.Enabled = false;
            }
        }

        private void PopulateFilterPanels(ProjectLevelGroupedHours[] reportData)
        {
            if (HostingPage.SetSelectedFilters)
            {
                var report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByProject(HostingPage.AccountId, HostingPage.BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value, null, null));
                var businessUnitList = report.GroupedProjects.Select(r => new { Name = r.Project.Group.Name, Id = r.Project.Group.Id }).Distinct().Select(a => new ProjectGroup { Id = a.Id, Name = a.Name }).ToList().OrderBy(s => s.Name).ToArray();

                PopulateBusinessUnitFilter(businessUnitList);

                foreach (ListItem item in cblBusinessUnits.Items)
                {
                    if (reportData.Any(r => r.Project.Group.Id.Value.ToString() == item.Value))
                    {
                        item.Selected = true;
                    }
                    else
                    {
                        item.Selected = false;
                    }
                }

                var data = report.GroupedProjects.ToArray();
                PopulateProjectStatusFilter(data);
                PopulateBillingFilter(data);

            }
            else
            {
                ProjectGroup[] businessUnits = reportData.Select(r => new { Id = r.Project.Group.Id, Name = r.Project.Group.Name }).Distinct().ToList().OrderBy(s => s.Name).Select(pg => new ProjectGroup { Id = pg.Id, Name = pg.Name }).ToArray();
                PopulateBusinessUnitFilter(businessUnits);
                cblBusinessUnits.SelectAllItems(true);

                PopulateProjectStatusFilter(reportData);
                PopulateBillingFilter(reportData);
            }


        }

        private void PopulateBillingFilter(ProjectLevelGroupedHours[] reportData)
        {
            var billingtypes = reportData.Select(r => new { Text = string.IsNullOrEmpty(r.BillingType) ? "Unassigned" : r.BillingType, Value = r.BillingType }).Distinct().ToList().OrderBy(s => s.Value);
            DataHelper.FillListDefault(cblBilling.CheckBoxListObject, "All Billing Types", billingtypes.ToArray(), false, "Value", "Text");
            cblBilling.SelectAllItems(true);
        }

        private void PopulateBusinessUnitFilter(ProjectGroup[] businessUnits)
        {
            int height = 17 * businessUnits.Length;
            Unit unitHeight = new Unit((height + 17) > 50 ? 50 : height + 17);
            DataHelper.FillListDefault(cblBusinessUnits.CheckBoxListObject, "All Business Units", businessUnits, false, "Id", "HtmlEncodedName");
            cblBusinessUnits.Height = unitHeight;

        }

        private void PopulateProjectStatusFilter(ProjectLevelGroupedHours[] reportData)
        {
            var projectStatusIds = reportData.Select(r => new { Id = r.Project.Status.Id, Name = r.Project.Status.Name }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblProjectStatus.CheckBoxListObject, "All Status", projectStatusIds.ToArray(), false, "Id", "Name");
            cblProjectStatus.SelectAllItems(true);
        }

        protected void btnFilterOK_OnClick(object sender, EventArgs e)
        {
            HostingPage.BusinessUnitsFilteredIds = cblBusinessUnits.SelectedItems;
            PopulateByProjectData(false);
        }


    }
}

