using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports.ByAccount;
using System.Text;
using PraticeManagement.Reporting;

namespace PraticeManagement.Controls.Reports.ByAccount
{
    public partial class ByBusinessDevelopment : System.Web.UI.UserControl
    {
        #region Properties

        private const string Text_GroupByBusinessUnit = "Group by Business Unit";
        private const string Text_GroupByPerson = "Group by Person";
        private const string AccountDetailByBusinessDevelopmentExport = "Account Detail Report By Business Development";

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            btnExpandOrCollapseAll.Attributes["onclick"] = "return CollapseOrExpandAll(" + btnExpandOrCollapseAll.ClientID +
                                                           ", " + hdnCollapsed.ClientID +
                                                           ", " + hdncpeExtendersIds.ClientID +
                                                           ");";

            btnExpandOrCollapseAll.Text = btnExpandOrCollapseAll.ToolTip = (hdnCollapsed.Value.ToLower() == "true") ? "Expand All" : "Collapse All";
        }

        protected void btnGroupBy_Click(object sender, EventArgs e)
        {
            if (btnGroupBy.Text == Text_GroupByPerson)
            {
                btnGroupBy.Text = Text_GroupByBusinessUnit;
                btnGroupBy.ToolTip = Text_GroupByBusinessUnit;
                mvBusinessDevelopmentReport.ActiveViewIndex = 1;
                PopulateGroupByPerson();
            }
            else
            {
                mvBusinessDevelopmentReport.ActiveViewIndex = 0;
                btnGroupBy.Text = Text_GroupByPerson;
                btnGroupBy.ToolTip = Text_GroupByPerson;
                PopulateGroupByBusinessUnit();
            }
        }

        private void PopulateGroupByBusinessUnit()
        {
            if (Page is AccountSummaryReport)
            {
                var hostingPage = Page as AccountSummaryReport;
                tpByBusinessUnit.PopulateData(hostingPage.AccountId, hostingPage.BusinessUnitIds, hostingPage.StartDate.Value, hostingPage.EndDate.Value);
            }
            else if (Page is TimePeriodSummaryReport)
            {
                var hostingPage = Page as TimePeriodSummaryReport;
                hostingPage.Total = tpByBusinessUnit.PopulateData(hostingPage.AccountId, hostingPage.BusinessUnitIds, hostingPage.StartDate.Value, hostingPage.EndDate.Value);
                hostingPage.ByProjectControl.MpeProjectDetailReport.Show();
            }
        }

        private void PopulateGroupByPerson()
        {
            if (Page is AccountSummaryReport)
            {
                var hostingPage = Page as AccountSummaryReport;
                tpByPerson.PopulateData(hostingPage.AccountId, hostingPage.BusinessUnitIds, hostingPage.StartDate.Value, hostingPage.EndDate.Value);
            }
            else if (Page is TimePeriodSummaryReport)
            {
                var hostingPage = Page as TimePeriodSummaryReport;
                hostingPage.Total = tpByPerson.PopulateData(hostingPage.AccountId, hostingPage.BusinessUnitIds, hostingPage.StartDate.Value, hostingPage.EndDate.Value);
                hostingPage.ByProjectControl.MpeProjectDetailReport.Show();
            }
        }

        public void ApplyAttributes(int count)
        {
            btnGroupBy.Visible = btnExpandOrCollapseAll.Visible =
                          btnExportToExcel.Enabled = count > 0;
        }

        public void SetExpandCollapseIdsTohiddenField(string output)
        {
            hdncpeExtendersIds.Value = output;
            btnExpandOrCollapseAll.Text = btnExpandOrCollapseAll.ToolTip = "Expand All";
            hdnCollapsed.Value = "true";
        }

        public void PopulateByBusinessDevelopment()
        {
            if (mvBusinessDevelopmentReport.ActiveViewIndex == 1)
            {
                PopulateGroupByPerson();
            }
            else
            {
                PopulateGroupByBusinessUnit();
            }
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            int accountId = 0;
            string businessUnitIds = string.Empty;
            DateTime? startDate;
            DateTime? endDate;
            string range = string.Empty;
            int businessUnitsCount = 0, projectsCount = 0, personsCount = 0;

            if (Page is TimePeriodSummaryReport)
            {
                var hostingPage = Page as TimePeriodSummaryReport;
                accountId = hostingPage.AccountId;
                businessUnitIds = hostingPage.BusinessUnitIds;
                startDate = hostingPage.StartDate;
                endDate = hostingPage.EndDate;
                range = hostingPage.Range;
            }
            else
            {
                var hostingPage = Page as AccountSummaryReport;
                accountId = hostingPage.AccountId;
                businessUnitIds = hostingPage.BusinessUnitIds;
                startDate = hostingPage.StartDate;
                endDate = hostingPage.EndDate;
                range = hostingPage.Range;
            }

            DataHelper.InsertExportActivityLogMessage(AccountDetailByBusinessDevelopmentExport);

            List<BusinessUnitLevelGroupedHours> data = ServiceCallers.Custom.Report(r => r.AccountReportGroupByBusinessUnit(accountId, businessUnitIds, startDate.Value, endDate.Value)).ToList();

            var account = ServiceCallers.Custom.Client(c => c.GetClientDetailsShort(accountId));

            if (Page is TimePeriodSummaryReport)
            {
                businessUnitsCount = data.Select(r => r.BusinessUnit.Id.Value).Distinct().Count();
                projectsCount = data.Count > 0 ? 1 : 0;
                personsCount = data.SelectMany(g => g.PersonLevelGroupedHoursList.Select(p => p.Person.Id.Value)).Distinct().Count();
            }
            else
            {
                var hostingPage = Page as AccountSummaryReport;
                businessUnitsCount = hostingPage.BusinessUnitsCount;
                projectsCount = hostingPage.ProjectsCount;
                personsCount = hostingPage.PersonsCount;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Account_ByBusinessDevelopment Report");
            sb.Append("\t");
            sb.AppendLine();
            sb.Append(account.HtmlEncodedName);
            sb.Append("\t");
            sb.Append(account.Code);
            sb.Append("\t");
            sb.AppendLine();
            sb.Append(businessUnitsCount + " Business Units");
            sb.Append("\t");
            sb.Append(projectsCount + " Projects");
            sb.Append("\t");
            sb.Append(personsCount + " Persons");
            sb.Append("\t");
            sb.AppendLine();
            sb.Append(range);
            sb.Append("\t");
            sb.AppendLine();
            sb.AppendLine();

            if (data.Count > 0)
            {
                //Header
                /* Person Name 
                Work Type	Work Type Name	Date	Billable Hours	Non-Billable Hours	Total Hours	Note */
                sb.Append("Employee Id");
                sb.Append("\t");
                sb.Append("Resource");
                sb.Append("\t");
                sb.Append("Date");
                sb.Append("\t");
                sb.Append("WorkType");
                sb.Append("\t");
                sb.Append("WorkType Name");
                sb.Append("\t");
                sb.Append("Business Unit");
                sb.Append("\t");
                sb.Append("Business Unit Name");
                sb.Append("\t");
                sb.Append("Non-Billable");
                sb.Append("\t");
                sb.Append("Total");
                sb.Append("\t");
                sb.Append("Note");
                sb.AppendLine();
                //Data
                foreach (var buLevelGroupedHours in data)
                {

                    foreach (var personLevelGroupedHoursList in buLevelGroupedHours.PersonLevelGroupedHoursList)
                    {
                        foreach (var groupByDate in personLevelGroupedHoursList.DayTotalHours)
                        {

                            foreach (var dateLevel in groupByDate.DayTotalHoursList)
                            {
                                sb.Append(personLevelGroupedHoursList.Person.EmployeeNumber);
                                sb.Append("\t");
                                sb.Append(personLevelGroupedHoursList.Person.HtmlEncodedName);
                                sb.Append("\t");

                                sb.Append(groupByDate.Date.ToString("MM/dd/yyyy"));
                                sb.Append("\t");
                                sb.Append(dateLevel.TimeType.Code);
                                sb.Append("\t");
                                sb.Append(dateLevel.TimeType.Name);
                                sb.Append("\t");
                                sb.Append(buLevelGroupedHours.BusinessUnit.Code);
                                sb.Append("\t");
                                sb.Append(buLevelGroupedHours.BusinessUnit.HtmlEncodedName);
                                sb.Append("\t");
                                sb.Append(dateLevel.NonBillableHours);
                                sb.Append("\t");
                                sb.Append(dateLevel.TotalHours);
                                sb.Append("\t");
                                sb.Append(dateLevel.HtmlEncodedNoteForExport);
                                sb.Append("\t");
                                sb.AppendLine();

                            }

                        }
                    }

                }
            }
            else
            {
                sb.Append("There are no Time Entries towards this account.");
            }
            var filename = string.Format("{0}_{1}_{2}.xls", account.Code, account.Name, "_ByBusinessDevlopment");
            filename = filename.Replace(' ', '_');
            GridViewExportUtil.Export(Utils.Generic.EncodedFileName(filename), sb);

        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }
    }
}

