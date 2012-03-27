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
    public partial class TimePeriodSummaryByResource : System.Web.UI.UserControl
    {
        private PraticeManagement.Reporting.TimePeriodSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.TimePeriodSummaryReport)Page); }
        }

        public List<PersonLevelGroupedHours> PersonLevelGroupedHoursList
        {
            get
            {
                return ViewState["PersonLevelGroupedHoursList_Key"] as List<PersonLevelGroupedHours>;
            }
            set
            {
                ViewState["PersonLevelGroupedHoursList_Key"] = value;
            }

        }
        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected string GetCurrencyFormat(double value)
        {
            return value > 0 ? value.ToString(Constants.Formatting.CurrencyFormat) : "$0";
        }

        protected string GetBillableValue(double billableValue, bool isPersonNotAssignedToFixedProject)
        {
            if (!isPersonNotAssignedToFixedProject)
            {
                return "Fixed";
            }
            else
            {
                return billableValue > 0 ? billableValue.ToString(Constants.Formatting.CurrencyFormat) : "$0";
            }
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

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
          
            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {

                StringBuilder sb = new StringBuilder();
                sb.Append("TimePeriod_ByResource Report");
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(HostingPage.StartDate.Value.ToString("MM/dd/yyyy") + " - " + HostingPage.EndDate.Value.ToString("MM/dd/yyyy"));
                sb.Append("\t");
                sb.AppendLine();
                sb.AppendLine();

                if (PersonLevelGroupedHoursList.Count > 0)
                {
                    //Header
                    sb.Append("Resource");
                    sb.Append("\t");
                    sb.Append("Seniority");
                    sb.Append("\t");
                    sb.Append("Billable");
                    sb.Append("\t");
                    sb.Append("Non-Billable");
                    sb.Append("\t");
                    sb.Append("Total");
                    sb.Append("\t");
                    sb.Append("Value");
                    sb.Append("\t");
                    sb.Append("Utlization Percent this Period");
                    sb.Append("\t");
                    sb.AppendLine();

                    //Data
                    foreach (var personLevelGroupedHours in PersonLevelGroupedHoursList)
                    {
                        sb.Append(personLevelGroupedHours.Person.PersonLastFirstName);
                        sb.Append("\t");
                        sb.Append(personLevelGroupedHours.Person.Seniority.Name);
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(personLevelGroupedHours.BillableHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(personLevelGroupedHours.NonBillableHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(personLevelGroupedHours.TotalHours));
                        sb.Append("\t");
                        sb.Append(GetBillableValue(personLevelGroupedHours.BillableValue, personLevelGroupedHours.IsPersonNotAssignedToFixedProject));
                        sb.Append("\t");
                        sb.Append(personLevelGroupedHours.Person.UtlizationPercent + "%");
                        sb.Append("\t");
                        sb.AppendLine();
                    }

                }
                else
                {
                    sb.Append("There are no Time Entries towards this range selected.");
                }
                //“TimePeriod_ByResource_[StartOfRange]_[EndOfRange].xls”.  
                var filename = string.Format("{0}_{1}_{2}.xls", "TimePeriod_ByResource", HostingPage.StartDate.Value.ToString("MM.dd.yyyy"), HostingPage.EndDate.Value.ToString("MM.dd.yyyy"));
                GridViewExportUtil.Export(filename, sb);
            }
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        public void DataBindResource(PersonLevelGroupedHours[] reportData)
        {
            PersonLevelGroupedHoursList = reportData.ToList();
            if (PersonLevelGroupedHoursList.Count > 0)
            {
                HostingPage.SetGraphVisibility(true);
                divEmptyMessage.Style["display"] = "none";
                repResource.Visible = true;
                repResource.DataSource = reportData;
                repResource.DataBind();
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repResource.Visible = false;
                HostingPage.SetGraphVisibility(false);
            }
        }
     }
}

