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
                List<PersonLevelGroupedHours> personLevelGroupedHoursList = ViewState["PersonLevelGroupedHoursList_Key"] as List<PersonLevelGroupedHours>;
                if (!HostingPage.IncludePersonWithNoTimeEntries)
                {
                    personLevelGroupedHoursList = personLevelGroupedHoursList.Where(p => p.IsPersonTimeEntered).ToList();
                }
                return personLevelGroupedHoursList;
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

        protected string GetPayTypeSortValue(string payType, string name)
        {
            if (string.IsNullOrEmpty(payType))
            {
                return "-1" + name;
            }
            return payType + name;
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {

            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {

                StringBuilder sb = new StringBuilder();
                sb.Append("TimePeriod_ByResource Report");
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(PersonLevelGroupedHoursList.Count + " Employees");
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
                    sb.Append("Pay Types");
                    sb.Append("\t");
                    sb.Append("Billable");
                    sb.Append("\t");
                    sb.Append("Non-Billable");
                    sb.Append("\t");
                    sb.Append("BD");
                    sb.Append("\t");
                    sb.Append("Internal");
                    sb.Append("\t");
                    sb.Append("Time-Off");
                    sb.Append("\t");
                    sb.Append("Total");
                    sb.Append("\t");
                    sb.Append("Utilization Percent this Period");
                    sb.Append("\t");
                    sb.AppendLine();

                    //Data
                    foreach (var personLevelGroupedHours in PersonLevelGroupedHoursList)
                    {
                        sb.Append(personLevelGroupedHours.Person.PersonLastFirstName);
                        sb.Append("\t");
                        sb.Append(personLevelGroupedHours.Person.Seniority.Name);
                        sb.Append("\t");
                        sb.Append(personLevelGroupedHours.Person.CurrentPay.TimescaleName);
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(personLevelGroupedHours.BillableHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(personLevelGroupedHours.ProjectNonBillableHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(personLevelGroupedHours.BusinessDevelopmentHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(personLevelGroupedHours.InternalHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(personLevelGroupedHours.AdminstrativeHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(personLevelGroupedHours.TotalHours));
                        sb.Append("\t");
                        sb.Append(personLevelGroupedHours.Person.UtlizationPercent + "%");
                        sb.Append("\t");
                        sb.AppendLine();
                    }

                }
                else
                {
                    sb.Append("There are no Time Entries by any Employee  for the selected range.");
                }
                //“TimePeriod_ByResource_Excel.xls”.  
                var filename = "TimePeriod_ByResource_Excel.xls";
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
                divEmptyMessage.Style["display"] = "none";
                tbHeader.Style["display"] = "";
                repResource.Visible = true;
                repResource.DataSource = PersonLevelGroupedHoursList;
                repResource.DataBind();
                PopulateHeaderSection();
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repResource.Visible = false;
                tbHeader.Style["display"] = "none";
            }
        }

        private void PopulateHeaderSection()
        {
            double billableHours = PersonLevelGroupedHoursList.Sum(p => p.BillableHours);
            double nonBillableHours = PersonLevelGroupedHoursList.Sum(p => p.NonBillableHours);
            int noOfEmployees = PersonLevelGroupedHoursList.Count;
            double totalUtlization = PersonLevelGroupedHoursList.Sum(p => p.Person.UtlizationPercent);
            var billablePercent = 0;
            var nonBillablePercent = 0;
            if (billableHours != 0 || nonBillableHours != 0)
            {
                billablePercent = DataTransferObjects.Utils.Generic.GetBillablePercentage(billableHours, nonBillableHours);
                nonBillablePercent = (100 - billablePercent);
            }
            ltPersonCount.Text = noOfEmployees + " Employees";
            lbRange.Text = HostingPage.Range;
            ltrlTotalHours.Text = (billableHours + nonBillableHours).ToString(Constants.Formatting.DoubleValueWithZeroPadding);
            ltrlAvgHours.Text = ((billableHours + nonBillableHours) / noOfEmployees).ToString(Constants.Formatting.DoubleValueWithZeroPadding);
            ltrlAvgUtilization.Text = Math.Round((totalUtlization / noOfEmployees), 0).ToString() + "%";
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

