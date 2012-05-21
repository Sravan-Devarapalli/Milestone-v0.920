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
        private string PersonSummaryReportExport = "Person Summary Report";

        private PraticeManagement.Reporting.PersonDetailTimeReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.PersonDetailTimeReport)Page); }
        }

        public void DatabindRepepeaterSummary(List<TimeEntriesGroupByClientAndProject> timeEntriesGroupByClientAndProjectList)
        {
            if (timeEntriesGroupByClientAndProjectList.Count > 0)
            {
                divEmptyMessage.Style["display"] = "none";
                repSummary.Visible = true;
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
     
        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(PersonSummaryReportExport);

            // mso-number-format:"0\.00"
            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                var timeEntriesGroupByClientAndProjectList = ServiceCallers.Custom.Report(r => r.PersonTimeEntriesSummary(HostingPage.SelectedPersonId, HostingPage.StartDate.Value, HostingPage.EndDate.Value)).ToList();

                int personId = HostingPage.SelectedPersonId;
                var person = ServiceCallers.Custom.Person(p => p.GetStrawmanDetailsById(personId));
                string personType = person.IsOffshore ? "Offshore" : string.Empty;
                string payType = person.CurrentPay.TimescaleName;
                string personStatusAndType = string.IsNullOrEmpty(personType) && string.IsNullOrEmpty(payType) ? string.Empty :
                                                                                 string.IsNullOrEmpty(payType) ? personType :
                                                                                 string.IsNullOrEmpty(personType) ? payType :
                                                                                                                     payType + ", " + personType;
                StringBuilder sb = new StringBuilder();
                sb.Append(person.FirstName + " " + person.LastName);
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(personStatusAndType);
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(HostingPage.Range);
                sb.Append("\t");
                sb.AppendLine();
                sb.AppendLine();
                if (timeEntriesGroupByClientAndProjectList.Count > 0)
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
                    sb.Append("Percent of Total Hours this Period");
                   
                    sb.Append("\t");
                    sb.AppendLine();

                    //Data
                    foreach (var timeEntriesGroupByClientAndProject in timeEntriesGroupByClientAndProjectList)
                    {
                        sb.Append(timeEntriesGroupByClientAndProject.Client.Code);
                        sb.Append("\t");
                        sb.Append(timeEntriesGroupByClientAndProject.Client.Name);
                        sb.Append("\t");
                        sb.Append(timeEntriesGroupByClientAndProject.Project.Group.Code);
                        sb.Append("\t");
                        sb.Append(timeEntriesGroupByClientAndProject.Project.Group.Name);
                        sb.Append("\t");
                        sb.Append(timeEntriesGroupByClientAndProject.Project.ProjectNumber);
                        sb.Append("\t");
                        sb.Append(timeEntriesGroupByClientAndProject.Project.Name);
                        sb.Append("\t");
                        sb.Append(timeEntriesGroupByClientAndProject.Project.Status.Name);
                        sb.Append("\t");
                        sb.Append(timeEntriesGroupByClientAndProject.BillableType);
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(timeEntriesGroupByClientAndProject.BillableHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(timeEntriesGroupByClientAndProject.NonBillableHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(timeEntriesGroupByClientAndProject.TotalHours));
                        sb.Append("\t");
                        sb.Append(timeEntriesGroupByClientAndProject.ProjectTotalHoursPercent+ "%");
                        sb.Append("\t");
                        sb.AppendLine();
                    }

                }
                else
                {
                    sb.Append("This person has not entered Time Entries for the selected period.");
                }
                //“[LastName]_[FirstName]-[“Summary” or “Detail”]-[StartOfRange]_[EndOfRange].xls”.  
                //example :Hong-Turney_Jason-Summary-03.01.2012_03.31.2012.xlsx
                var filename = string.Format("{0}_{1}_{2}_{3}_{4}.xls", person.LastName, person.FirstName, "Summary", HostingPage.StartDate.Value.ToString("MM.dd.yyyy"), HostingPage.EndDate.Value.ToString("MM.dd.yyyy"));
                GridViewExportUtil.Export(filename, sb);
            }
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

    }
}

