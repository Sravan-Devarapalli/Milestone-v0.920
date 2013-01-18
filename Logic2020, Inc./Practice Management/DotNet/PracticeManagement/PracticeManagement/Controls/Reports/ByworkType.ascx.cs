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
        private string ShowPanel = "ShowPanel('{0}', '{1}','{2}');";
        private string HidePanel = "HidePanel('{0}');";
        private string OnMouseOver = "onmouseover";
        private string OnMouseOut = "onmouseout";
        private string ProjectSummaryReportExport = "ProjectSummary Report By WorkType";


        private Label LblBillable { get; set; }

        private Label LblNonBillable { get; set; }

        private Label LblActualHours { get; set; }

        private PraticeManagement.Reporting.ProjectSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.ProjectSummaryReport)Page); }
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        private void PopulateWorkTypeTotalHoursPercent(WorkTypeLevelGroupedHours[] reportData)
        {
            double grandTotal = reportData.Sum(t => t.TotalHours);
            grandTotal = Math.Round(grandTotal, 2);
            if (grandTotal > 0)
            {
                foreach (WorkTypeLevelGroupedHours workTypeLevelGroupedHours in reportData)
                {
                    workTypeLevelGroupedHours.WorkTypeTotalHoursPercent = Convert.ToInt32((workTypeLevelGroupedHours.TotalHours / grandTotal) * 100);
                }
            }
        }

        protected void repWorkType_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                LblBillable = e.Item.FindControl("lblBillable") as Label;
                LblNonBillable = e.Item.FindControl("lblNonBillable") as Label;
                LblActualHours = e.Item.FindControl("lblActualHours") as Label;
            }
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(ProjectSummaryReportExport);

            var project = ServiceCallers.Custom.Project(p => p.GetProjectShortByProjectNumber(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.StartDate, HostingPage.EndDate));
            WorkTypeLevelGroupedHours[] data = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByWorkType(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.PeriodSelected == "*" ? null : HostingPage.StartDate, HostingPage.PeriodSelected == "*" ? null : HostingPage.EndDate, null));
            data = data.OrderBy(p => p.WorkType.Name).ToArray();
            PopulateWorkTypeTotalHoursPercent(data);
            StringBuilder sb = new StringBuilder();
            sb.Append(project.Client.HtmlEncodedName);
            sb.Append("\t");
            sb.Append(project.Group.HtmlEncodedName);
            sb.Append("\t");
            sb.AppendLine();
            //P081003 - [ProjectName]
            sb.Append(string.Format("{0} - {1}", project.ProjectNumber, project.HtmlEncodedName));
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
                sb.Append("Billable");
                sb.Append("\t");
                sb.Append("Non-Billable");
                sb.Append("\t");
                sb.Append("Actual Hours");
                sb.Append("\t");
                sb.Append("Percent of Total Actual Hours");
                sb.Append("\t");
                sb.AppendLine();

                //Data
                foreach (var workTypeLevelGroupedHours in data)
                {
                    sb.Append(workTypeLevelGroupedHours.WorkType.Name);
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
            GridViewExportUtil.Export(Utils.Generic.EncodedFileName(filename), sb);
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

        public void DataBindResource(WorkTypeLevelGroupedHours[] reportData)
        {
            if (reportData.Length > 0)
            {
                divEmptyMessage.Style["display"] = "none";
                repWorkType.Visible = true;
                PopulateWorkTypeTotalHoursPercent(reportData);
                repWorkType.DataSource = reportData;
                repWorkType.DataBind();

                //populate header hover
                LblBillable.Attributes[OnMouseOver] = string.Format(ShowPanel, LblBillable.ClientID, pnlTotalBillableHours.ClientID, 0);
                LblBillable.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalBillableHours.ClientID);

                LblNonBillable.Attributes[OnMouseOver] = string.Format(ShowPanel, LblNonBillable.ClientID, pnlTotalNonBillableHours.ClientID, 0);
                LblNonBillable.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalNonBillableHours.ClientID);

                LblActualHours.Attributes[OnMouseOver] = string.Format(ShowPanel, LblActualHours.ClientID, pnlTotalActualHours.ClientID, 0);
                LblActualHours.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalActualHours.ClientID);
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repWorkType.Visible = false;
            }
            PopulateHeaderSection(reportData);
        }

        public void PopulateByWorkTypeData()
        {
            WorkTypeLevelGroupedHours[] data;
            data = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByWorkType(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.PeriodSelected == "*" ? null : HostingPage.StartDate, HostingPage.PeriodSelected == "*" ? null : HostingPage.EndDate, null));
            DataBindResource(data);
        }

        private void PopulateHeaderSection(WorkTypeLevelGroupedHours[] reportData)
        {
            var project = ServiceCallers.Custom.Project(p => p.GetProjectShortByProjectNumber(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.StartDate, HostingPage.EndDate));
            double billableHours = reportData.Sum(p => p.BillableHours);
            double nonBillableHours = reportData.Sum(p => p.NonBillableHours);
            double projectedHours = reportData.Length > 0 ? reportData[0].ForecastedHours : 0d;
            double actualHours = reportData.Sum(p => p.TotalHours);

            var billablePercent = 0;
            var nonBillablePercent = 0;
            if (billableHours != 0 || nonBillableHours != 0)
            {
                billablePercent = DataTransferObjects.Utils.Generic.GetBillablePercentage(billableHours, nonBillableHours);
                nonBillablePercent = (100 - billablePercent);
            }

            ltrlAccount.Text = project.Client.HtmlEncodedName;
            ltrlBusinessUnit.Text = project.Group.HtmlEncodedName;
            ltrlProjectedHours.Text = projectedHours.ToString(Constants.Formatting.DoubleValue);
            ltrlProjectName.Text = project.HtmlEncodedName;
            ltrlProjectNumber.Text = project.ProjectNumber;
            ltrlProjectStatusAndBillingType.Text = string.IsNullOrEmpty(project.BillableType) ? project.Status.Name : project.Status.Name + ", " + project.BillableType;
            ltrlProjectRange.Text = HostingPage.ProjectRange;
            ltrlTotalHours.Text = (billableHours + nonBillableHours).ToString(Constants.Formatting.DoubleValue);
            ltrlBillableHours.Text = lblTotalBillableHours.Text = lblTotalBillablePanlActual.Text = billableHours.ToString(Constants.Formatting.DoubleValue);
            ltrlNonBillableHours.Text = lblTotalNonBillableHours.Text = lblTotalNonBillablePanlActual.Text = nonBillableHours.ToString(Constants.Formatting.DoubleValue);
            ltrlBillablePercent.Text = billablePercent.ToString();
            ltrlNonBillablePercent.Text = nonBillablePercent.ToString();
            lblTotalActualHours.Text = actualHours.ToString(Constants.Formatting.DoubleValue);

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

