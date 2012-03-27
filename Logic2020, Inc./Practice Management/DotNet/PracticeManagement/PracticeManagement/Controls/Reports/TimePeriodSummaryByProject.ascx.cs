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
        public List<ProjectLevelGroupedHours> TimeEntriesGroupByProject
        {
            get
            {
                return ViewState["TimeEntriesGroupByProject"] as List<ProjectLevelGroupedHours>;
            }
            set
            {
                ViewState["TimeEntriesGroupByProject"] = value;
            }
        }

        private PraticeManagement.Reporting.TimePeriodSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.TimePeriodSummaryReport)Page); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected string GetCurrencyFormat(double value)
        {
            return value > 0 ? value.ToString(Constants.Formatting.CurrencyFormat) : "$0";
        }

        protected string GetBillableValue(double billableValue, bool isFixedProject)
        {
            if (isFixedProject)
            {
                return "Fixed";
            }
            else
            {
                return billableValue > 0 ? billableValue.ToString(Constants.Formatting.CurrencyFormat) : "$0";
            }
        }

        protected string GetBillableSortValue(double billableValue, bool isFixedProject)
        {
            if (isFixedProject)
            {
                return "-1";
            }
            else
            {
                return billableValue > 0 ? billableValue.ToString() : "0";
            }
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

                StringBuilder sb = new StringBuilder();
                sb.Append("TimePeriod_ByProject Report");
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(HostingPage.StartDate.Value.ToString("MM/dd/yyyy") + " - " + HostingPage.EndDate.Value.ToString("MM/dd/yyyy"));
                sb.Append("\t");
                sb.AppendLine();
                sb.AppendLine();

                if (TimeEntriesGroupByProject.Count > 0)
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
                    sb.Append("Billable");
                    sb.Append("\t");
                    sb.Append("Non-Billable");
                    sb.Append("\t");
                    sb.Append("Total");
                    sb.Append("\t");
                    sb.Append("Value");
                    sb.Append("\t");
                    sb.Append("Project Variance (in Hours)");
                    sb.Append("\t");
                    sb.AppendLine();

                    //Data
                    foreach (var projectLevelGroupedHours in TimeEntriesGroupByProject)
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
                        sb.Append(GetDoubleFormat(projectLevelGroupedHours.BillableHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(projectLevelGroupedHours.NonBillableHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(projectLevelGroupedHours.TotalHours));
                        sb.Append("\t");
                        sb.Append(GetBillableValue(projectLevelGroupedHours.BillableValue, projectLevelGroupedHours.IsFixedProject));
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
                //“TimePeriod_ByProject_Excel.xls”.  
                var filename = "TimePeriod_ByProject_Excel.xls";
                GridViewExportUtil.Export(filename, sb);
            }
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

        public void DataBindProject(ProjectLevelGroupedHours[] reportData)
        {
            TimeEntriesGroupByProject = reportData.ToList();
            if (TimeEntriesGroupByProject.Count > 0)
            {
                HostingPage.SetGraphVisibility(true);
                divEmptyMessage.Style["display"] = "none";
                repProject.Visible = true;
                repProject.DataSource = reportData;
                repProject.DataBind();
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repProject.Visible = false;
                HostingPage.SetGraphVisibility(false);
            }
        }

    }
}
