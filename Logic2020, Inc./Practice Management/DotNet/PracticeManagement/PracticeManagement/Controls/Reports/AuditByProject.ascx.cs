using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Text;
using DataTransferObjects.TimeEntry;
using DataTransferObjects;

namespace PraticeManagement.Controls.Reports
{
    public partial class AuditByProject : System.Web.UI.UserControl
    {
        private string AuditReportExport = "Audit Report By Project";

        private PraticeManagement.Reporting.Audit HostingPage
        {
            get { return ((PraticeManagement.Reporting.Audit)Page); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected string GetDateFormat(DateTime date)
        {
            return date.ToString(Constants.Formatting.ReportDateFormat);
        }

        protected bool GetNonBillableImageVisibility(int timeEntrySection, bool isChargeable)
        {
            return !isChargeable && timeEntrySection == (int)TimeEntrySectionType.Project;
        }

        public void PopulateByProjectData(ProjectLevelTimeEntriesHistory[] reportDataByPerson)
        {
            var reportDataList = reportDataByPerson.OrderBy(p => p.Project.ProjectNumber).ToList();

            if (reportDataList.Count > 0)
            {
                divEmptyMessage.Style["display"] = "none";
                repProject.Visible = true;
                repProject.DataSource = reportDataList;
                repProject.DataBind();
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repProject.Visible = false;
            }
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(AuditReportExport);
            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                List<ProjectLevelTimeEntriesHistory> data = ServiceCallers.Custom.Report(r => r.TimeEntryAuditReportByProject(HostingPage.StartDate.Value, HostingPage.EndDate.Value)).ToList();
                data = data.OrderBy(p => p.Project.ProjectNumber).ToList();
                StringBuilder sb = new StringBuilder();
                sb.Append("Time Entry Audit");
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(data.Count + " Person(s) Affected");
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(HostingPage.Range);
                sb.Append("\t");
                sb.AppendLine();
                sb.AppendLine();
                if (data.Count > 0)
                {
                    //Header
                    sb.Append("Employee Id");
                    sb.Append("\t");
                    sb.Append("Person Name");
                    sb.Append("\t");
                    sb.Append("Status");
                    sb.Append("\t");
                    sb.Append("Pay Types");
                    sb.Append("\t");
                    sb.Append("Affected Date");
                    sb.Append("\t");
                    sb.Append("Modified Date");
                    sb.Append("\t");
                    sb.Append("Account Name");
                    sb.Append("\t");
                    sb.Append("Business Unit");
                    sb.Append("\t");
                    sb.Append("Project");
                    sb.Append("\t");
                    sb.Append("Project Name");
                    sb.Append("\t");
                    sb.Append("Phase");
                    sb.Append("\t");
                    sb.Append("Work Type");
                    sb.Append("\t");
                    sb.Append("B/NB");
                    sb.Append("\t");
                    sb.Append("Original");
                    sb.Append("\t");
                    sb.Append("New");
                    sb.Append("\t");
                    sb.Append("Net Change");
                    sb.Append("\t");
                    sb.Append("Note");
                    sb.Append("\t");
                    sb.AppendLine();

                    //Data
                    foreach (ProjectLevelTimeEntriesHistory projectLevelTimeEntriesHistory in data)
                    {
                        foreach (PersonLevelTimeEntriesHistory personLevelTimeEntrie in projectLevelTimeEntriesHistory.PersonLevelTimeEntries)
                        {
                            foreach (TimeEntryRecord timeEntryRecord in personLevelTimeEntrie.TimeEntryRecords)
                            {
                                sb.Append(personLevelTimeEntrie.Person.EmployeeNumber);
                                sb.Append("\t");
                                sb.Append(personLevelTimeEntrie.Person.HtmlEncodedName);
                                sb.Append("\t");
                                sb.Append(personLevelTimeEntrie.Person.Status.Name);
                                sb.Append("\t");
                                sb.Append(personLevelTimeEntrie.Person.CurrentPay != null ? personLevelTimeEntrie.Person.CurrentPay.TimescaleName : string.Empty);
                                sb.Append("\t");
                                sb.Append(timeEntryRecord.ChargeCodeDate.ToString("MM/dd/yyyy"));
                                sb.Append("\t");
                                sb.Append(timeEntryRecord.ModifiedDate.ToString("MM/dd/yyyy"));
                                sb.Append("\t");
                                sb.Append(timeEntryRecord.ChargeCode.Client.HtmlEncodedName);
                                sb.Append("\t");
                                sb.Append(timeEntryRecord.ChargeCode.ProjectGroup.HtmlEncodedName);
                                sb.Append("\t");
                                sb.Append(timeEntryRecord.ChargeCode.Project.ProjectNumber);
                                sb.Append("\t");
                                sb.Append(timeEntryRecord.ChargeCode.Project.HtmlEncodedName);
                                sb.Append("\t");
                                sb.Append(timeEntryRecord.ChargeCode.Phase);
                                sb.Append("\t");
                                sb.Append(timeEntryRecord.ChargeCode.TimeType.Name);
                                sb.Append("\t");
                                sb.Append(timeEntryRecord.IsChargeable ? "B" : "NB");
                                sb.Append("\t");
                                sb.Append(timeEntryRecord.OldHours);
                                sb.Append("\t");
                                sb.Append(timeEntryRecord.ActualHours);
                                sb.Append("\t");
                                sb.Append(timeEntryRecord.NetChange);
                                sb.Append("\t");
                                sb.Append(timeEntryRecord.HtmlEncodedNoteForExport);
                                sb.Append("\t");
                                sb.AppendLine();
                            }
                        }
                    }
                }
                else
                {
                    sb.Append("There are no Time Entries that were changed afterwards by any Employee for the selected range.");
                }
                //“Time_Entry_Audit_[StartOfRange]_[EndOfRange].xls”.  
                var filename = string.Format("{0}_{1}-{2}.xls", "Time_Entry_Audit", HostingPage.StartDate.Value.ToString("MM.dd.yyyy"), HostingPage.EndDate.Value.ToString("MM.dd.yyyy"));
                GridViewExportUtil.Export(filename, sb);
            }
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

        protected void btnGroupBy_OnClick(object sender, EventArgs e)
        {
            HostingPage.SelectView(0);
        }

        protected List<KeyValuePair<Person, TimeEntryRecord>> GetModifiedDatasource(Object personLevelTimeEntries)
        {
            List<PersonLevelTimeEntriesHistory> _personLevelTimeEntries = (List<PersonLevelTimeEntriesHistory>)personLevelTimeEntries;
            List<KeyValuePair<Person, TimeEntryRecord>> modifiedPersonLevelTimeEntries = new List<KeyValuePair<Person, TimeEntryRecord>>();

            foreach (PersonLevelTimeEntriesHistory plteh in _personLevelTimeEntries)
            {
                foreach (TimeEntryRecord ter in plteh.TimeEntryRecords)
                {
                    modifiedPersonLevelTimeEntries.Add(new KeyValuePair<Person, TimeEntryRecord>(plteh.Person, ter));
                }
            }
            return modifiedPersonLevelTimeEntries;
        }

    }
}

