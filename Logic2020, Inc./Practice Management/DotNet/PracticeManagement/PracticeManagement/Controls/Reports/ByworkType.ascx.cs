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
        private PraticeManagement.Reporting.TimePeriodSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.TimePeriodSummaryReport)Page); }
        }

        public List<WorkTypeLevelGroupedHours> WorkTypeLevelGroupedHoursList
        {
            get
            {
                List<WorkTypeLevelGroupedHours> workTypeLevelGroupedHoursList = ViewState["WorkTypeLevelGroupedHoursList_Key"] as List<WorkTypeLevelGroupedHours>;
                return workTypeLevelGroupedHoursList;
            }
            set
            {
                ViewState["WorkTypeLevelGroupedHoursList_Key"] = value;
            }

        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {

            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {

                StringBuilder sb = new StringBuilder();
                sb.Append("TimePeriod_ByWorkType Report");
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(WorkTypeLevelGroupedHoursList.Count + " WorkTypes");
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(HostingPage.Range);
                sb.Append("\t");
                sb.AppendLine();
                sb.AppendLine();

                if (WorkTypeLevelGroupedHoursList.Count > 0)
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
                    foreach (var workTypeLevelGroupedHours in WorkTypeLevelGroupedHoursList)
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
                    sb.Append("There are no Time Entries by any Employee  for the selected range.");
                }
                //“TimePeriod_ByWorkType_[StartOfRange]_[EndOfRange].xls”.  
                var filename = string.Format("{0}_{1}-{2}.xls", "TimePeriod_ByWorkType", HostingPage.StartDate.Value.ToString("MM.dd.yyyy"), HostingPage.EndDate.Value.ToString("MM.dd.yyyy"));
                GridViewExportUtil.Export(filename, sb);
            }
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

        public void DataBindResource(WorkTypeLevelGroupedHours[] reportData)
        {
            WorkTypeLevelGroupedHoursList = reportData.ToList();
            if (WorkTypeLevelGroupedHoursList.Count > 0)
            {
                divEmptyMessage.Style["display"] = "none";
                repWorkType.Visible = true;
                double grandTotal = WorkTypeLevelGroupedHoursList.Sum(t => t.TotalHours);
                grandTotal = Math.Round(grandTotal, 2);
                if (grandTotal > 0)
                {
                    foreach (WorkTypeLevelGroupedHours workTypeLevelGroupedHours in WorkTypeLevelGroupedHoursList)
                    {
                        workTypeLevelGroupedHours.WorkTypeTotalHoursPercent = Convert.ToInt32((workTypeLevelGroupedHours.TotalHours / grandTotal) * 100);
                    }
                }
                repWorkType.DataSource = WorkTypeLevelGroupedHoursList;
                repWorkType.DataBind();
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repWorkType.Visible = false;
            }
            PopulateHeaderSection();
        }

        private void PopulateHeaderSection()
        {
            double billableHours = WorkTypeLevelGroupedHoursList.Sum(p => p.BillableHours);
            double nonBillableHours = WorkTypeLevelGroupedHoursList.Sum(p => p.NonBillableHours);
            int noOfEmployees = WorkTypeLevelGroupedHoursList.Count;
            double totalUtlization = WorkTypeLevelGroupedHoursList.Sum(p => p.BillableHours);
            var billablePercent = 0;
            var nonBillablePercent = 0;
            if (billableHours != 0 || nonBillableHours != 0)
            {
                billablePercent = DataTransferObjects.Utils.Generic.GetBillablePercentage(billableHours, nonBillableHours);
                nonBillablePercent = (100 - billablePercent);
            }
            ltWorkTypeCount.Text = noOfEmployees + " WorkTypes";
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
    }
}
