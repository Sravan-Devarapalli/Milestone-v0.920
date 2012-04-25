using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Web.UI.HtmlControls;
using System.Text;

namespace PraticeManagement.Controls.Reports
{
    public partial class TimePeriodSummaryByProject : System.Web.UI.UserControl
    {
        private HtmlImage ImgClientFilter { get; set; }

        private HtmlImage ImgProjectStatusFilter { get; set; }

        private PraticeManagement.Reporting.TimePeriodSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.TimePeriodSummaryReport)Page); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            cblClients.OKButtonId = cblProjectStatus.OKButtonId = btnFilterOK.ClientID;
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
            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                var data = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryReportByProject(HostingPage.StartDate.Value, HostingPage.EndDate.Value, cblClients.SelectedItems, cblProjectStatus.SelectedItems));
                StringBuilder sb = new StringBuilder();
                sb.Append("TimePeriod_ByProject Report");
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(data.Length + " Projects");
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(HostingPage.Range);
                sb.Append("\t");
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
                var filename = string.Format("TimePeriod_ByProject_{0}-{1}.xls", HostingPage.StartDate.Value.ToString("MM/dd/yyyy"), HostingPage.EndDate.Value.ToString("MM/dd/yyyy"));
                GridViewExportUtil.Export(filename, sb);
            }
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

        public void PopulateByProjectData(bool isPopulateFilters = true)
        {
            ProjectLevelGroupedHours[] data;
            if (isPopulateFilters)
            {
                data = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryReportByProject(HostingPage.StartDate.Value, HostingPage.EndDate.Value, null, null));
            }
            else
            {
                data = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryReportByProject(HostingPage.StartDate.Value, HostingPage.EndDate.Value, cblClients.SelectedItems, cblProjectStatus.SelectedItems));
            }
            DataBindProject(data, isPopulateFilters);
        }

        protected void repProject_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                ImgClientFilter = e.Item.FindControl("imgClientFilter") as HtmlImage;
                ImgProjectStatusFilter = e.Item.FindControl("imgProjectStatusFilter") as HtmlImage;
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
            if (reportData.Length > 0 || cblClients.Items.Count > 1 || cblProjectStatus.Items.Count > 1)
            {
                divEmptyMessage.Style["display"] = "none";
                repProject.Visible = true;
                repProject.DataSource = reportData;
                repProject.DataBind();
                ImgClientFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblClients.FilterPopupClientID,
                  cblClients.SelectedIndexes, cblClients.CheckBoxListObject.ClientID, cblClients.WaterMarkTextBoxBehaviorID);
                ImgProjectStatusFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblProjectStatus.FilterPopupClientID,
                  cblProjectStatus.SelectedIndexes, cblProjectStatus.CheckBoxListObject.ClientID, cblProjectStatus.WaterMarkTextBoxBehaviorID);
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repProject.Visible = false;
            }
            PopulateHeaderSection(reportData);
        }

        private void PopulateFilterPanels(ProjectLevelGroupedHours[] reportData)
        {
            PopulateClientFilter(reportData);
            PopulateProjectStatusFilter(reportData);
        }

        private void PopulateClientFilter(ProjectLevelGroupedHours[] reportData)
        {
            var clients = reportData.Select(r => new { Id = r.Project.Client.Id, Name = r.Project.Client.Name }).Distinct().ToList().OrderBy(s => s.Name).ToArray();
            int height = 17 * clients.Length;
            Unit unitHeight = new Unit((height + 17) > 50 ? 50 : height + 17);
            DataHelper.FillListDefault(cblClients.CheckBoxListObject, "All Clients", clients, false, "Id", "Name");
            cblClients.Height = unitHeight;
            cblClients.SelectAllItems(true);
        }

        private void PopulateProjectStatusFilter(ProjectLevelGroupedHours[] reportData)
        {
            var projectStatusIds = reportData.Select(r => new { Id = r.Project.Status.Id, Name = r.Project.Status.Name }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblProjectStatus.CheckBoxListObject, "All Status", projectStatusIds.ToArray(), false, "Id", "Name");
            cblProjectStatus.SelectAllItems(true);
        }

        private void PopulateHeaderSection(ProjectLevelGroupedHours[] reportData)
        {
            double billableHours = reportData.Sum(p => p.BillableHours);
            double nonBillableHours = reportData.Sum(p => p.NonBillableHours);
            int noOfEmployees = reportData.Length;
            var billablePercent = 0;
            var nonBillablePercent = 0;
            if (billableHours != 0 || nonBillableHours != 0)
            {
                billablePercent = DataTransferObjects.Utils.Generic.GetBillablePercentage(billableHours, nonBillableHours);
                nonBillablePercent = (100 - billablePercent);
            }
            ltProjectCount.Text = noOfEmployees + " Projects";
            lbRange.Text = HostingPage.Range;
            ltrlTotalHours.Text = (billableHours + nonBillableHours).ToString(Constants.Formatting.DoubleValue);
            ltrlAvgHours.Text = noOfEmployees > 0 ? ((billableHours + nonBillableHours) / noOfEmployees).ToString(Constants.Formatting.DoubleValue) : "0.00";
            ltrlBillableHours.Text = billableHours.ToString(Constants.Formatting.DoubleValue);
            ltrlNonBillableHours.Text = nonBillableHours.ToString(Constants.Formatting.DoubleValue);
            ltrlBillablePercent.Text = billablePercent.ToString();
            ltrlNonBillablePercent.Text = nonBillablePercent.ToString();

            if (billablePercent == 0 && nonBillablePercent == 0)
            {
                trBillable.Height = "1px";
                trNonBillable.Height = "1px";
            }
            else if (billablePercent == 100)
            {
                trBillable.Height = "80px";
                trNonBillable.Height = "1px";
            }
            else if (billablePercent == 0 && nonBillablePercent == 100)
            {
                trBillable.Height = "1px";
                trNonBillable.Height = "80px";
            }
            else
            {
                int billablebarHeight = (int)(((float)80 / (float)100) * billablePercent);
                trBillable.Height = billablebarHeight.ToString() + "px";
                trNonBillable.Height = (80 - billablebarHeight).ToString() + "px";
            }

        }

        protected string GetProjectSummaryReportUrl(string projectNumber)
        {
            string projectSummaryReportUrl = string.Format(Constants.ApplicationPages.RedirectProjectSummaryReportIdFormat, projectNumber, HostingPage.RangeSelected, HostingPage.StartDate.Value.ToString("yyyy/MM/dd"), HostingPage.EndDate.Value.ToString("yyyy/MM/dd"), "0");
            string timePeriodReportUrl = string.Format(Constants.ApplicationPages.RedirectTimePeriodSummaryReportFormat, HostingPage.RangeSelected, HostingPage.StartDate.Value.ToString("yyyy/MM/dd"), HostingPage.EndDate.Value.ToString("yyyy/MM/dd"), HostingPage.SelectedView, HostingPage.IncludePersonWithNoTimeEntries);
            return PraticeManagement.Utils.Generic.GetTargetUrlWithReturn(projectSummaryReportUrl, timePeriodReportUrl);
        }

        protected void btnFilterOK_OnClick(object sender, EventArgs e)
        {
            PopulateByProjectData(false);
        }

        protected void btnFilterCancel_OnClick(object sender, EventArgs e)
        {

        }



    }
}

