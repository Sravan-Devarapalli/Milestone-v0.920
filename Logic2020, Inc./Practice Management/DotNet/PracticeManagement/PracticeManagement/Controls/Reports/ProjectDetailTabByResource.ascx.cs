using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Web.Script.Serialization;
using AjaxControlToolkit;
using System.Text;

namespace PraticeManagement.Controls.Reports
{
    public partial class ProjectDetailTabByResource : System.Web.UI.UserControl
    {
        private PraticeManagement.Reporting.ProjectSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.ProjectSummaryReport)Page.Page); }
        }

        private PraticeManagement.Controls.Reports.ProjectSummaryByResource HostingControl
        {
            get { return (PraticeManagement.Controls.Reports.ProjectSummaryByResource)HostingPage.ByResourceControl; }
        }

        private int sectionId;

        private string ProjectDetailByResourceExport = "Project Detail Report By Resource";

        private List<string> CollapsiblePanelExtenderClientIds
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            btnExpandOrCollapseAll.Attributes["onclick"] = "return CollapseOrExpandAll(" + btnExpandOrCollapseAll.ClientID +
                                                            ", " + hdnCollapsed.ClientID +
                                                            ", " + hdncpeExtendersIds.ClientID +
                                                            ");";

            btnExpandOrCollapseAll.Text = btnExpandOrCollapseAll.ToolTip = (hdnCollapsed.Value.ToLower() == "true") ? "Expand All" : "Collapse All";
        }

        public void DataBindByResourceDetail(PersonLevelGroupedHours[] reportData)
        {
            if (reportData.Length > 0)
            {
                reportData = reportData.OrderBy(p => p.Person.PersonLastFirstName).ToArray();
                divEmptyMessage.Style["display"] = "none";
                repPersons.Visible = btnExpandOrCollapseAll.Visible = true;
                repPersons.DataSource = reportData;
                repPersons.DataBind();
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repPersons.Visible = btnExpandOrCollapseAll.Visible = false;
            }
            btnExportToPDF.Enabled = 
            btnExportToExcel.Enabled = reportData.Count() > 0;
        }

        protected void repPersons_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                CollapsiblePanelExtenderClientIds = new List<string>();

            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var repDate = e.Item.FindControl("repDate") as Repeater;
                PersonLevelGroupedHours dataitem = (PersonLevelGroupedHours)e.Item.DataItem;

                var cpePerson = e.Item.FindControl("cpePerson") as CollapsiblePanelExtender;
                cpePerson.BehaviorID = cpePerson.ClientID + e.Item.ItemIndex.ToString();

                sectionId = dataitem.TimeEntrySectionId;
                repDate.DataSource = dataitem.DayTotalHours != null ? dataitem.DayTotalHours.OrderBy(p => p.Date).ToList() : dataitem.DayTotalHours;
                repDate.DataBind();
                CollapsiblePanelExtenderClientIds.Add(cpePerson.BehaviorID);

            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var output = jss.Serialize(CollapsiblePanelExtenderClientIds);
                hdncpeExtendersIds.Value = output;
                btnExpandOrCollapseAll.Text = btnExpandOrCollapseAll.ToolTip = "Expand All";
                hdnCollapsed.Value = "true";
            }
        }

        protected void repDate_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {

            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var repWorktype = e.Item.FindControl("repWorktype") as Repeater;
                TimeEntriesGroupByDate dataitem = (TimeEntriesGroupByDate)e.Item.DataItem;
                var rep = sender as Repeater;

                //var cpeDate = e.Item.FindControl("cpeDate") as CollapsiblePanelExtender;
                //cpeDate.BehaviorID = cpeDate.ClientID + e.Item.ItemIndex.ToString();
                // CollapsiblePanelDateExtenderClientIds.Add(cpeDate.BehaviorID);


                repWorktype.DataSource = dataitem.DayTotalHoursList;
                repWorktype.DataBind();
            }
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected string GetPersonRole(string role)
        {
            return string.IsNullOrEmpty(role) ? "" : "(" + role + ")";
        }

        protected bool GetNoteVisibility(String note)
        {
            if (!String.IsNullOrEmpty(note))
            {
                return true;
            }
            return false;

        }

        protected string GetDateFormat(DateTime date)
        {
            return date.ToString(Constants.Formatting.ReportDateFormat);
        }

        protected bool GetNonBillableImageVisibility(double nonBillableHours)
        {
            return sectionId == 1 && nonBillableHours > 0;
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(ProjectDetailByResourceExport);

            var project = ServiceCallers.Custom.Project(p => p.GetProjectShortByProjectNumber(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.StartDate, HostingPage.EndDate));
            PersonLevelGroupedHours[] data = ServiceCallers.Custom.Report(r => r.ProjectDetailReportByResource(HostingPage.ProjectNumber, HostingPage.MilestoneId,
                HostingPage.PeriodSelected == "0" ? HostingPage.StartDate : null, HostingPage.PeriodSelected == "0" ? HostingPage.EndDate : null,
                HostingControl.cblProjectRolesControl.SelectedItemsXmlFormat));
            
            string filterApplied = "Filters applied to columns: ";
            bool isFilterApplied = false;
            if (!HostingControl.cblProjectRolesControl.AllItemsSelected)
            {
                filterApplied = filterApplied + " ProjectRoles.";
                isFilterApplied = true;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(project.Client.Name);
            sb.Append("\t");
            sb.Append(project.Group.Name);
            sb.Append("\t");
            sb.AppendLine();
            //P081003 - [ProjectName]
            sb.Append(string.Format("{0} - {1}", project.ProjectNumber, project.Name));
            sb.Append("\t");
            sb.AppendLine();
            sb.Append(string.IsNullOrEmpty(project.BillableType) ? project.Status.Name : project.Status.Name + ", " + project.BillableType);
            sb.Append("\t");
            sb.AppendLine();
            sb.Append(HostingPage.ProjectRange);
            sb.Append("\t");
            if (isFilterApplied)
            {
                sb.AppendLine();
                sb.Append(filterApplied);
                sb.Append("\t");
            }
            sb.AppendLine();
            sb.AppendLine();
            if (data.Length > 0)
            {
                //Header
                /* Person Name 
                Work Type	Work Type Name	Date	Billable Hours	Non-Billable Hours	Total Hours	Note */
                sb.Append("Resource");
                sb.Append("\t");
                sb.Append("Project Role");
                sb.Append("\t");
                sb.Append("Date");
                sb.Append("\t");
                sb.Append("WorkType");
                sb.Append("\t");
                sb.Append("WorkType Name");
                sb.Append("\t");
                sb.Append("Billable");
                sb.Append("\t");
                sb.Append("Non-Billable");
                sb.Append("\t");
                sb.Append("Total");
                sb.Append("\t");
                sb.Append("Note");
                sb.AppendLine();
                //Data
                foreach (var timeEntriesGroupByClientAndProject in data)
                {
                    if (timeEntriesGroupByClientAndProject.DayTotalHours != null)
                    {
                        foreach (var byDateList in timeEntriesGroupByClientAndProject.DayTotalHours)
                        {
                            foreach (var byWorkType in byDateList.DayTotalHoursList)
                            {
                                sb.Append(timeEntriesGroupByClientAndProject.Person.PersonLastFirstName);
                                sb.Append("\t");
                                sb.Append(timeEntriesGroupByClientAndProject.Person.ProjectRoleName);
                                sb.Append("\t");
                                sb.Append(byDateList.Date.ToString("MM/dd/yyyy"));
                                sb.Append("\t");
                                sb.Append(byWorkType.TimeType.Code);
                                sb.Append("\t");
                                sb.Append(byWorkType.TimeType.Name);
                                sb.Append("\t");
                                sb.Append(byWorkType.BillableHours);
                                sb.Append("\t");
                                sb.Append(byWorkType.NonBillableHours);
                                sb.Append("\t");
                                sb.Append(byWorkType.TotalHours);
                                sb.Append("\t");
                                sb.Append(byWorkType.NoteForExport);
                                sb.Append("\t");
                                sb.AppendLine();
                            }
                        }
                    }
                    else
                    {
                        sb.Append(timeEntriesGroupByClientAndProject.Person.PersonLastFirstName);
                        sb.Append("\t");
                        sb.Append(timeEntriesGroupByClientAndProject.Person.ProjectRoleName);
                        sb.Append("\t");
                        sb.Append("");
                        sb.Append("\t");
                        sb.Append("");
                        sb.Append("\t");
                        sb.Append("");
                        sb.Append("\t");
                        sb.Append("0");
                        sb.Append("\t");
                        sb.Append("0");
                        sb.Append("\t");
                        sb.Append("0");
                        sb.Append("\t");
                        sb.Append("");
                        sb.Append("\t");
                        sb.AppendLine();
                    }
                }
            }
            else
            {
                sb.Append("There are no Time Entries towards this project.");
            }
            var filename = string.Format("{0}_{1}_{2}.xls", project.ProjectNumber, project.Name, "_ByResourceDetail");
            filename = filename.Replace(' ', '_');
            GridViewExportUtil.Export(filename, sb);

        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(ProjectDetailByResourceExport);
            HostingPage.PDFExport();
        }

    }
}

