using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Text;

namespace PraticeManagement.Controls.Reports
{
    public partial class PersonSummaryReport : System.Web.UI.UserControl
    {

        private PraticeManagement.Reporting.PersonDetailTimeReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.PersonDetailTimeReport)Page); }
        }

        public List<TimeEntriesGroupByClientAndProject> TimeEntriesGroupByClientAndProjectList
        {
            get
            {
                return ViewState["TimeEntriesGroupByClientAndProjectList_Key"] as List<TimeEntriesGroupByClientAndProject>;
            }
            set
            {
                ViewState["TimeEntriesGroupByClientAndProjectList_Key"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void DatabindRepepeaterSummary(List<TimeEntriesGroupByClientAndProject> timeEntriesGroupByClientAndProjectList)
        {
            TimeEntriesGroupByClientAndProjectList = timeEntriesGroupByClientAndProjectList;

            if (timeEntriesGroupByClientAndProjectList.Count > 0)
            {
                divEmptyMessage.Style["display"] = "none";
                repSummary.Visible = true;
                double grandTotal = timeEntriesGroupByClientAndProjectList.Sum(t => t.TotalHours);
                grandTotal = Math.Round(grandTotal, 2);
                if (grandTotal > 0)
                {
                    foreach (TimeEntriesGroupByClientAndProject timeEntries in timeEntriesGroupByClientAndProjectList)
                    {
                        timeEntries.ProjectTotalHoursPercent = Convert.ToInt32((timeEntries.TotalHours / grandTotal)*100);
                    }
                }
                repSummary.DataSource = timeEntriesGroupByClientAndProjectList;
                repSummary.DataBind();
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repSummary.Visible = false;
            }
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected string GetCurrencyFormat(double value)
        {
            return value > 0 ? value.ToString(Constants.Formatting.CurrencyFormat):"$0";
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            
            StringBuilder sb = new StringBuilder();
            sb.Append(HostingPage.SelectedPersonName);
            sb.Append(",");
            sb.Append(HostingPage.StartDate.ToString("MM/dd/yyyy") + " - " + HostingPage.EndDate.ToString("MM/dd/yyyy"));
            sb.Append(",");
            
            //Header
            sb.Append("Client");
            sb.Append(",");
            sb.Append("Project Number");
            sb.Append(",");
            sb.Append("Project Name");
            sb.Append(",");
            sb.Append("Billable");
            sb.Append(",");
            sb.Append("Value");
            sb.Append(",");
            sb.Append("Non-Billable");
            sb.Append(",");
            sb.Append("Total");
            sb.Append(",");
            sb.AppendLine();
           
            //Data
            foreach (var timeEntriesGroupByClientAndProject in TimeEntriesGroupByClientAndProjectList)
            {
                sb.Append(timeEntriesGroupByClientAndProject.Client.Name);
                sb.Append(",");
                sb.Append(timeEntriesGroupByClientAndProject.Project.ProjectNumber);
                sb.Append(",");
                sb.Append(timeEntriesGroupByClientAndProject.Project.Name);
                sb.Append(",");
                sb.Append(timeEntriesGroupByClientAndProject.BillableHours);
                sb.Append(",");
                sb.Append(timeEntriesGroupByClientAndProject.BillableValue);
                sb.Append(",");
                sb.Append(timeEntriesGroupByClientAndProject.NonBillableHours);
                sb.Append(",");
                sb.Append(timeEntriesGroupByClientAndProject.TotalHours);
                sb.Append(",");
                sb.AppendLine();
            }




        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }


    }
}
