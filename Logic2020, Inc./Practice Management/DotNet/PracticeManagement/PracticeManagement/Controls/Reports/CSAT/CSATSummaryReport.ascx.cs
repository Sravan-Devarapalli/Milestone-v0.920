using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using System.Text;

namespace PraticeManagement.Controls.Reports.CSAT
{
    public partial class CSATSummaryReport : System.Web.UI.UserControl
    {
        private const string CSATReportExport = "CSAT Report Export";

        private PraticeManagement.Reports.CSATReport HostingPage
        {
            get { return ((PraticeManagement.Reports.CSATReport)Page); }
        }


        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(CSATReportExport);

            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                var report = ServiceCallers.Custom.Project(r => r.CSATSummaryReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.SelectedPractices, HostingPage.SelectedAccounts,true)).ToList();
                StringBuilder sb = new StringBuilder();
                sb.Append("CSAT Report For the Period:");
                sb.Append("\t");
                sb.Append(HostingPage.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                sb.Append(" to ");
                sb.Append(HostingPage.EndDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                sb.AppendLine();

                if (report.Count > 0)
                {
                    //Header
                    sb.Append("Project Number");
                    sb.Append("\t");
                    sb.Append("Account");
                    sb.Append("\t");
                    sb.Append("Project Status");
                    sb.Append("\t");
                    sb.Append("Business Unit");
                    sb.Append("\t");
                    sb.Append("Business Group");
                    sb.Append("\t");
                    sb.Append("Project Name");
                    sb.Append("\t");
                    sb.Append("Project Owner");
                    sb.Append("\t");
                    sb.Append("Estimated Revenue");
                    sb.Append("\t");
                    sb.Append("CSAT Eligible?");
                    sb.Append("\t");
                    sb.Append("Start Date");
                    sb.Append("\t");
                    sb.Append("End Date");
                    sb.Append("\t");
                    sb.Append("Date of Project Completion");
                    sb.Append("\t");
                    sb.Append("Practice Area");
                    sb.Append("\t");
                    sb.Append("SalesPerson");
                    sb.Append("\t");
                    sb.Append("ClientDirector");
                    sb.Append("\t");
                    sb.Append("Project Manager(s)");
                    sb.Append("\t");
                    sb.Append("CSAT Owner");
                    sb.Append("\t");
                    sb.Append("CSAT Completion Date");
                    sb.Append("\t");
                    sb.Append("CSAT Reviewer");
                    sb.Append("\t");
                    sb.Append("CSAT Score");
                    sb.Append("\t");
                    sb.Append("CSAT Comments");
                    sb.Append("\t");
                    sb.AppendLine();

                    //Data
                    foreach (var reportItem in report)
                    {
                        int i; 
                        sb.Append(reportItem.ProjectNumber);
                        sb.Append("\t");
                        sb.Append(reportItem.Client.Name);
                        sb.Append("\t");
                        sb.Append(reportItem.Status.Name);
                        sb.Append("\t");
                        sb.Append(reportItem.Group.Name);
                        sb.Append("\t");
                        sb.Append(reportItem.BusinessGroup.Name);
                        sb.Append("\t");
                        sb.Append(reportItem.Name);
                        sb.Append("\t");
                        sb.Append(reportItem.ProjectOwner);
                        sb.Append("\t");
                        sb.Append("$" + reportItem.SowBudget);
                        sb.Append("\t");
                        sb.Append(reportItem.IsCSATEligible?"Yes":"No");
                        sb.Append("\t");
                        sb.Append(reportItem.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                        sb.Append("\t");
                        sb.Append(reportItem.EndDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                        sb.Append("\t");
                        sb.Append((reportItem.RecentCompletedStatusDate == null || reportItem.RecentCompletedStatusDate.Value == DateTime.MinValue) ? "" : reportItem.RecentCompletedStatusDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                        sb.Append("\t");
                        sb.Append(reportItem.Practice.HtmlEncodedName);
                        sb.Append("\t");
                        sb.Append(reportItem.SalesPersonName);
                        sb.Append("\t");
                        sb.Append(reportItem.Director.Name);
                        sb.Append("\t");
                        sb.Append(reportItem.ProjectManagerNames);
                        sb.Append("\t");
                        sb.Append(reportItem.CSATOwnerName);
                        sb.Append("\t");
                        for (i = 0; i < reportItem.CSATList.Count; i++)
                        {
                            sb.Append(reportItem.CSATList[i].CompletionDate.ToString(Constants.Formatting.EntryDateFormat));
                            sb.Append("\t");
                            sb.Append(reportItem.CSATList[i].ReviewerName);
                            sb.Append("\t");
                            sb.Append(reportItem.CSATList[i].ReferralScore);
                            sb.Append("\t");
                            sb.Append(reportItem.CSATList[i].Comments);
                            sb.Append("\t");
                            sb.AppendLine();
                            if (i != reportItem.CSATList.Count - 1)
                            {
                                sb.Append(reportItem.ProjectNumber);
                                sb.Append("\t");
                                sb.Append(reportItem.Client.Name);
                                sb.Append("\t");
                                sb.Append(reportItem.Status.Name);
                                sb.Append("\t");
                                sb.Append(reportItem.Group.Name);
                                sb.Append("\t");
                                sb.Append(reportItem.BusinessGroup.Name);
                                sb.Append("\t");
                                sb.Append(reportItem.Name);
                                sb.Append("\t");
                                sb.Append(reportItem.ProjectOwner);
                                sb.Append("\t");
                                sb.Append("$"+reportItem.SowBudget);
                                sb.Append("\t");
                                sb.Append(reportItem.IsCSATEligible ? "Yes" : "No");
                                sb.Append("\t");
                                sb.Append(reportItem.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                                sb.Append("\t");
                                sb.Append(reportItem.EndDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                                sb.Append("\t");
                                sb.Append((reportItem.RecentCompletedStatusDate == null || reportItem.RecentCompletedStatusDate.Value == DateTime.MinValue) ? "" : reportItem.RecentCompletedStatusDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                                sb.Append("\t");
                                sb.Append(reportItem.Practice.HtmlEncodedName);
                                sb.Append("\t");
                                sb.Append(reportItem.SalesPersonName);
                                sb.Append("\t");
                                sb.Append(reportItem.Director.Name);
                                sb.Append("\t");
                                sb.Append(reportItem.ProjectManagerNames);
                                sb.Append("\t");
                                sb.Append(reportItem.CSATOwnerName);
                                sb.Append("\t");
                            }
                        }
                    }

                }
                else
                {
                    sb.Append("There are no CSAT Entries towards this range selected.");
                }
                var filename = string.Format("CSAT_Report_{0}-{1}.xls", HostingPage.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat), HostingPage.EndDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                GridViewExportUtil.Export(filename, sb);
            }
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {
         
        }

        protected string GetProjectDetailsLink(int? projectId,bool flag)
        {
            if (projectId.HasValue)
                return Utils.Generic.GetTargetUrlWithReturn(String.Format(Constants.ApplicationPages.DetailRedirectFormat, Constants.ApplicationPages.ProjectDetail, projectId.Value) + (flag ? "&CSAT=true":string.Empty),
                                                            Constants.ApplicationPages.CSATReport);
            else
                return string.Empty;
        }

        protected void repSummary_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HyperLink hlCSATScore = (HyperLink)e.Item.FindControl("hlCSATScore");
                Label lblSymblvsble = (Label)e.Item.FindControl("lblSymblvsble");
                Project project = (Project)e.Item.DataItem;
                hlCSATScore.Text = project.CSATList[0].ReferralScore.ToString();
                lblSymblvsble.Text = project.HasMultipleCSATs ? "!" : "";
            }
        }
        public void PopulateData()
        {
           Project[] projects = ServiceCallers.Custom.Project(p=>p.CSATSummaryReport(HostingPage.StartDate.Value,HostingPage.EndDate.Value,HostingPage.SelectedPractices,HostingPage.SelectedAccounts,false));
           if (projects.Length > 0)
           {
               repSummary.Visible = true;
               HostingPage.HeaderTable.Visible = true;
               btnExportToExcel.Enabled = true;
               divEmptyMessage.Style["display"] = "none";
               repSummary.DataSource = projects;
               repSummary.DataBind();
               HostingPage.PopulateHeaderSection();
           }
           else
           {
               repSummary.Visible = false;
               btnExportToExcel.Enabled = false;
               HostingPage.HeaderTable.Visible = false;
               divEmptyMessage.Style["display"] = "";
           }
        }
    }
}
