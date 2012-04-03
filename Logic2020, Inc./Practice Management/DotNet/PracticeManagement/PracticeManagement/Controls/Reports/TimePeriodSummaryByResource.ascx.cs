using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Web.Security;

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
                sb.Append(HostingPage.Range);
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
                //“TimePeriod_ByResource_[StartOfRange]_[EndOfRange].xls”.  
                var filename = string.Format("{0}_{1}-{2}.xls", "TimePeriod_ByResource", HostingPage.StartDate.Value.ToString("MM.dd.yyyy"), HostingPage.EndDate.Value.ToString("MM.dd.yyyy"));
                GridViewExportUtil.Export(filename, sb);
            }
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

        protected void btnPayCheckExport_OnClick(object sender, EventArgs e)
        {
            
            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                List<PersonLevelPayCheck> personLevelPayCheckList = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryByResourcePayCheck(HostingPage.StartDate.Value, HostingPage.EndDate.Value)).ToList();
                StringBuilder sb = new StringBuilder();
                sb.Append(" Paychex ");
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(personLevelPayCheckList.Count.ToString() + " Employees");
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(HostingPage.Range);
                sb.Append("\t");
                sb.AppendLine();
                sb.AppendLine();
                bool IsUserAdminstrator = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
                Dictionary<string, double> workTypeLevelTimeOffHours = personLevelPayCheckList[0].WorkTypeLevelTimeOffHours;
                if (PersonLevelGroupedHoursList.Count > 0)
                {
                    //Header
                    sb.Append("BranchID");
                    sb.Append("\t");
                    sb.Append("DeptID");
                    sb.Append("\t");
                    sb.Append("EmployeeID");
                    sb.Append("\t");
                    if (IsUserAdminstrator)
                    {
                        sb.Append("PaychexID");
                        sb.Append("\t");
                    }
                    sb.Append("Last Name");
                    sb.Append("\t");
                    sb.Append("First Name");
                    sb.Append("\t");
                    sb.Append("Pay Types");
                    sb.Append("\t");
                    sb.Append("Hours");
                    sb.Append("\t");
                    foreach (string worktype in workTypeLevelTimeOffHours.Keys)
                    {
                        sb.Append(worktype);
                        sb.Append("\t");
                    }
                    sb.Append("Total");
                    sb.Append("\t");
                    sb.AppendLine();

                    //Data
                    foreach (var personLevelPayCheck in personLevelPayCheckList)
                    {
                        sb.Append(personLevelPayCheck.BranchID);
                        sb.Append("\t");
                        sb.Append(personLevelPayCheck.DeptID);
                        sb.Append("\t");
                        sb.Append(personLevelPayCheck.Person.EmployeeNumber);
                        sb.Append("\t");
                        if (IsUserAdminstrator)
                        {
                            sb.Append("");
                            sb.Append("\t");
                        }
                        sb.Append(personLevelPayCheck.Person.LastName);
                        sb.Append("\t");
                        sb.Append(personLevelPayCheck.Person.FirstName);
                        sb.Append("\t");
                        sb.Append(personLevelPayCheck.Person.CurrentPay.TimescaleName);
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(personLevelPayCheck.TotalHoursExcludingTimeOff));
                        sb.Append("\t");
                        foreach (double worTypeHours in workTypeLevelTimeOffHours.Values)
                        {
                            sb.Append(worTypeHours);
                            sb.Append("\t");
                        }
                        sb.Append(GetDoubleFormat(personLevelPayCheck.TotalHoursIncludingTimeOff));
                        sb.Append("\t");
                        sb.AppendLine();
                    }

                }
                else
                {
                    sb.Append("There are no Time Entries by any Employee  for the selected range.");
                }
                //“Paychex_[StartOfRange]_[EndOfRange].xls”.  
                var filename = string.Format("{0}_{1}-{2}.xls", "Paychex", HostingPage.StartDate.Value.ToString("MM.dd.yyyy"), HostingPage.EndDate.Value.ToString("MM.dd.yyyy"));
                GridViewExportUtil.Export(filename, sb);
            }
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
                repResource.Visible = true;
                repResource.DataSource = PersonLevelGroupedHoursList;
                repResource.DataBind();
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repResource.Visible = false;
            }
            PopulateHeaderSection();
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
            ltrlTotalHours.Text = (billableHours + nonBillableHours).ToString(Constants.Formatting.DoubleValue);
            ltrlAvgHours.Text = noOfEmployees > 0 ? ((billableHours + nonBillableHours) / noOfEmployees).ToString(Constants.Formatting.DoubleValue) : "0.00";
            ltrlAvgUtilization.Text = noOfEmployees > 0 ? Math.Round((totalUtlization / noOfEmployees), 0).ToString() + "%" : "0%";
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

