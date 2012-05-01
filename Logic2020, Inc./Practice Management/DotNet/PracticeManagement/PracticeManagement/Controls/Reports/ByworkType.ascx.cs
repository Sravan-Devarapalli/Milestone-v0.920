using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Web.UI.HtmlControls;
using DataTransferObjects.TimeEntry;
using System.Text;

namespace PraticeManagement.Controls.Reports
{
    public partial class ByworkType : System.Web.UI.UserControl
    {
        private HtmlImage ImgCategoryFilter { get; set; }

        private PraticeManagement.Reporting.ProjectSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.ProjectSummaryReport)Page); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            cblCategory.OKButtonId = btnFilterOK.ClientID;
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            var project = ServiceCallers.Custom.Project(p => p.GetProjectShortByProjectNumber(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.StartDate, HostingPage.EndDate));
            WorkTypeLevelGroupedHours[] data = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByWorkType(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.PeriodSelected == "0" ? HostingPage.StartDate : null, HostingPage.PeriodSelected == "0" ? HostingPage.EndDate : null, cblCategory.ActualSelectedItemsXmlFormat));

            StringBuilder sb = new StringBuilder();
            sb.Append(project.Client.Name);
            sb.Append("\t");
            sb.Append(project.Group.Name);
            sb.Append("\t");
            sb.AppendLine();
            //P081003 - [ProjectName]
            sb.Append(string.Format("{0} - {1}", project.ProjectNumber, project.Name));
            sb.Append("\t");
            sb.AppendLine();
            sb.Append(string.IsNullOrEmpty(project.BillableType) ? project.Status.Name : project.Status.Name + ", " + project.BillableType);
            sb.Append("\t");
            sb.AppendLine();
            sb.Append(HostingPage.ProjectRange);
            sb.Append("\t");
            sb.AppendLine();
            sb.AppendLine();

            if (data.Length > 0)
            {
                //Header
                sb.Append("WorkType");
                sb.Append("\t");
                sb.Append("Category");
                sb.Append("\t");
                sb.Append("Billable");
                sb.Append("\t");
                sb.Append("Non-Billable");
                sb.Append("\t");
                sb.Append("Total");
                sb.Append("\t");
                sb.Append("Percent of Total Hours");
                sb.Append("\t");
                sb.AppendLine();

                //Data
                foreach (var workTypeLevelGroupedHours in data)
                {
                    sb.Append(workTypeLevelGroupedHours.WorkType.Name);
                    sb.Append("\t");
                    sb.Append(workTypeLevelGroupedHours.WorkType.Category);
                    sb.Append("\t");
                    sb.Append(GetDoubleFormat(workTypeLevelGroupedHours.BillableHours));
                    sb.Append("\t");
                    sb.Append(GetDoubleFormat(workTypeLevelGroupedHours.NonBillableHours));
                    sb.Append("\t");
                    sb.Append(GetDoubleFormat(workTypeLevelGroupedHours.TotalHours));
                    sb.Append("\t");
                    sb.Append(workTypeLevelGroupedHours.WorkTypeTotalHoursPercent + "%");
                    sb.Append("\t");
                    sb.AppendLine();
                }

            }
            else
            {
                sb.Append("There are no Time Entries towards this project.");
            }
            var filename = string.Format("{0}_{1}_{2}.xls", project.ProjectNumber, project.Name, "_ByWorkType");
            filename = filename.Replace(' ', '_');
            GridViewExportUtil.Export(filename, sb);
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

        protected void repWorkType_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                ImgCategoryFilter = e.Item.FindControl("imgCategoryFilter") as HtmlImage;
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

            }
        }

        public void DataBindResource(WorkTypeLevelGroupedHours[] reportData, bool isFirstTime)
        {
            if (isFirstTime)
            {
                PopulateCategoryFilter(reportData);
            }
            if (reportData.Length > 0 || cblCategory.Items.Count > 1)
            {
                divEmptyMessage.Style["display"] = "none";
                repWorkType.Visible = true;
                double grandTotal = reportData.Sum(t => t.TotalHours);
                grandTotal = Math.Round(grandTotal, 2);
                if (grandTotal > 0)
                {
                    foreach (WorkTypeLevelGroupedHours workTypeLevelGroupedHours in reportData)
                    {
                        workTypeLevelGroupedHours.WorkTypeTotalHoursPercent = Convert.ToInt32((workTypeLevelGroupedHours.TotalHours / grandTotal) * 100);
                    }
                }
                repWorkType.DataSource = reportData;
                repWorkType.DataBind();
                cblCategory.SaveSelectedIndexesInViewState();
                ImgCategoryFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblCategory.FilterPopupClientID,
                cblCategory.SelectedIndexes, cblCategory.CheckBoxListObject.ClientID, cblCategory.WaterMarkTextBoxBehaviorID);
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repWorkType.Visible = false;
            }
            PopulateHeaderSection(reportData);
        }

        public void PopulateByWorkTypeData(bool isFirstTime = false)
        {
            WorkTypeLevelGroupedHours[] data;
            if (isFirstTime)
            {
                data = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByWorkType(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.PeriodSelected == "0" ? HostingPage.StartDate : null, HostingPage.PeriodSelected == "0" ? HostingPage.EndDate : null, null));
            }
            else
            {
                data = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByWorkType(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.PeriodSelected == "0" ? HostingPage.StartDate : null, HostingPage.PeriodSelected == "0" ? HostingPage.EndDate : null, cblCategory.SelectedItemsXmlFormat));
            }
            DataBindResource(data, isFirstTime);
        }

        private void PopulateCategoryFilter(WorkTypeLevelGroupedHours[] reportData)
        {
            var categories = reportData.Select(r => new { Name = r.WorkType.Category }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblCategory.CheckBoxListObject, "All Categories", categories.ToArray(), false, "Name", "Name");
            cblCategory.SelectAllItems(true);
            cblCategory.OKButtonId = btnFilterOK.ClientID;
        }

        private void PopulateHeaderSection(WorkTypeLevelGroupedHours[] reportData)
        {
            var project = ServiceCallers.Custom.Project(p => p.GetProjectShortByProjectNumber(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.StartDate, HostingPage.EndDate));
            double billableHours = reportData.Sum(p => p.BillableHours);
            double nonBillableHours = reportData.Sum(p => p.NonBillableHours);
            double projectedHours = reportData.Length > 0 ? reportData[0].ForecastedHours : 0d;

            var billablePercent = 0;
            var nonBillablePercent = 0;
            if (billableHours != 0 || nonBillableHours != 0)
            {
                billablePercent = DataTransferObjects.Utils.Generic.GetBillablePercentage(billableHours, nonBillableHours);
                nonBillablePercent = (100 - billablePercent);
            }

            ltrlAccount.Text = project.Client.Name;
            ltrlBusinessUnit.Text = project.Group.Name;
            ltrlProjectedHours.Text = projectedHours.ToString(Constants.Formatting.DoubleValue);
            ltrlProjectName.Text = project.Name;
            ltrlProjectNumber.Text = project.ProjectNumber;
            ltrlProjectStatusAndBillingType.Text = string.IsNullOrEmpty(project.BillableType) ? project.Status.Name : project.Status.Name + ", " + project.BillableType;
            ltrlProjectRange.Text = HostingPage.ProjectRange;
            ltrlTotalHours.Text = (billableHours + nonBillableHours).ToString(Constants.Formatting.DoubleValue);
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

        protected void btnFilterOK_OnClick(object sender, EventArgs e)
        {
            PopulateByWorkTypeData();
        }
    }
}

