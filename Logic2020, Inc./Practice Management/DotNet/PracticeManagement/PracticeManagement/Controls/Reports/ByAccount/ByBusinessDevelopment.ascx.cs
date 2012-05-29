using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports.ByAccount;
using System.Text;

namespace PraticeManagement.Controls.Reports.ByAccount
{
    public partial class ByBusinessDevelopment : System.Web.UI.UserControl
    {
        #region Properties

        private const string Text_GroupByBusinessUnit = "Group by Business Unit";
        private const string Text_GroupByPerson = "Group by Person";

        private PraticeManagement.Reporting.AccountSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.AccountSummaryReport)Page); }
        }



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
            PopulateByBusinessDevelopment();
        }

        private void PopulateGroupByBusinessUnit()
        {
            tpByBusinessUnit.PopulateData(HostingPage.AccountId, HostingPage.BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value);
        }

        private void PopulateGroupByPerson()
        {
            tpByPerson.PopulateData(HostingPage.AccountId, HostingPage.BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value);
        }


        public void ApplyAttributes(int count)
        {
            btnExpandOrCollapseAll.Visible = btnExportToPDF.Enabled =
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
            if (btnGroupBy.Text == Text_GroupByPerson)
            {
                btnGroupBy.Text = Text_GroupByBusinessUnit;
                btnGroupBy.ToolTip = Text_GroupByBusinessUnit;
                mvBusinessDevelopmentReport.ActiveViewIndex = 1;
                PopulateGroupByBusinessUnit();
            }
            else
            {
                mvBusinessDevelopmentReport.ActiveViewIndex = 0;
                btnGroupBy.Text = Text_GroupByPerson;
                btnGroupBy.ToolTip = Text_GroupByPerson;
                PopulateGroupByPerson();
            }
        }


        private string AccountDetailByBusinessDevelopmentExport = "Account Detail Report By Business Development";

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(AccountDetailByBusinessDevelopmentExport);

            List<BusinessUnitLevelGroupedHours> data = ServiceCallers.Custom.Report(r => r.AccountReportGroupByBusinessUnit(HostingPage.AccountId, HostingPage.BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value)).ToList();

            var account = ServiceCallers.Custom.Client(c => c.GetClientDetailsShort(HostingPage.AccountId));

            StringBuilder sb = new StringBuilder();
            sb.Append(account.Name);
            sb.Append("\t");
            sb.Append(account.Code);
            sb.Append("\t");
            sb.AppendLine();
            sb.AppendLine();

            if (data.Count > 0)
            {
                //Header
                /* Person Name 
                Work Type	Work Type Name	Date	Billable Hours	Non-Billable Hours	Total Hours	Note */
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
                                sb.Append(personLevelGroupedHoursList.Person.PersonLastFirstName);
                                sb.Append("\t");

                                sb.Append(groupByDate.Date.ToString("MM/dd/yyyy"));
                                sb.Append("\t");
                                sb.Append(dateLevel.TimeType.Code);
                                sb.Append("\t");
                                sb.Append(dateLevel.TimeType.Name);
                                sb.Append("\t");
                                sb.Append(buLevelGroupedHours.BusinessUnit.Code);
                                sb.Append("\t");
                                sb.Append(buLevelGroupedHours.BusinessUnit.Name);
                                sb.Append("\t");
                                sb.Append(dateLevel.NonBillableHours);
                                sb.Append("\t");
                                sb.Append(dateLevel.TotalHours);
                                sb.Append("\t");
                                sb.Append(dateLevel.NoteForExport);
                                sb.Append("\t");
                                sb.AppendLine();

                            }

                        }
                    }

                }
            }
            else
            {
                sb.Append("There are no Time Entries towards this project.");
            }
            var filename = string.Format("{0}_{1}_{2}.xls", account.Code, account.Name, "_ByBusinessDevlopment");
            filename = filename.Replace(' ', '_');
            GridViewExportUtil.Export(filename, sb);
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }
    }
}
