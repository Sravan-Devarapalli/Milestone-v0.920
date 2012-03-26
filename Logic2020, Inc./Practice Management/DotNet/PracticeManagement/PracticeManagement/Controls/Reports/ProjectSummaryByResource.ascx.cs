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
    public partial class ProjectSummaryByResource : System.Web.UI.UserControl
    {

        public List<PersonLevelGroupedHours> TimeEntriesGroupByPerson
        {
            get
            {
                return ViewState["ProjectSummaryByResourceTimeEntries"] as List<PersonLevelGroupedHours>;
            }
            set
            {
                ViewState["ProjectSummaryByResourceTimeEntries"] = value;
            }
        }

        private PraticeManagement.Reporting.ProjectSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.ProjectSummaryReport)Page); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void repResource_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

            }
        }

        public void DataBindResource(PersonLevelGroupedHours[] reportData)
        {
            TimeEntriesGroupByPerson = reportData.ToList();

            if (reportData.Count() > 0)
            {
                divEmptyMessage.Style["display"] = "none";
                repResource.Visible =  true;
                repResource.DataSource = reportData;
                repResource.DataBind();
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repResource.Visible = false;
            }
        }


        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {


            StringBuilder sb = new StringBuilder();
            sb.Append(HostingPage.ProjectNumber);
            sb.Append("\t");
            sb.AppendLine();
            sb.AppendLine();

            //Header
            sb.Append("Resource");
            sb.Append("\t");
            sb.Append("Project Role");
            sb.Append("\t");
            sb.Append("Billable");
            sb.Append("\t");
            sb.Append("Non-Billable");
            sb.Append("\t");
            sb.Append("Total");
            sb.Append("\t");
            sb.Append("Value");
            sb.Append("\t");
            sb.AppendLine();

            //Data
            foreach (var byPerson in TimeEntriesGroupByPerson)
            {
                sb.Append(byPerson.Person.PersonLastFirstName);
                sb.Append("\t");
                sb.Append(byPerson.Person.ProjectRoleName);
                sb.Append("\t");
                sb.Append(GetDoubleFormat(byPerson.BillableHours));
                sb.Append("\t");
                sb.Append(GetDoubleFormat(byPerson.NonBillableHours));
                sb.Append("\t");
                sb.Append(GetDoubleFormat(byPerson.TotalHours));
                sb.Append("\t");
                sb.Append(GetCurrencyFormat(byPerson.BillableValue, byPerson.IsPersonNotAssignedToFixedProject));
                sb.Append("\t");
                sb.AppendLine();
            }


            var filename = string.Format("{0}_{1}.xls", HostingPage.ProjectNumber, "_ByResource");
            GridViewExportUtil.Export(filename, sb);

        }


        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected string GetCurrencyFormat(double value, bool isNotFixed)
        {
            if (!isNotFixed)
            {
                return "Fixed";
            }

            return value > 0 ? value.ToString(Constants.Formatting.CurrencyFormat) : "$0";
        }


        protected string GetBillableSortValue(double billableValue, bool isPersonNotAssignedToFixedProject)
        {
            if (!isPersonNotAssignedToFixedProject)
            {
                return "-1";
            }
            else
            {
                return billableValue > 0 ? billableValue.ToString() : "0";
            }
        }


        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }


       
    }
}
