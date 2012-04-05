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

        private int sectionId;

        public List<PersonLevelGroupedHours> TimeEntriesGroupByPersonDetailList
        {
            get
            {
                return ViewState["ProjectDetailByResourceTimeEntries"] as List<PersonLevelGroupedHours>;
            }
            set
            {
                ViewState["ProjectDetailByResourceTimeEntries"] = value;
            }
        }

        private List<KeyValuePair<string, string>> CollapsiblePanelExtenderClientIds
        {
            get;
            set;
        }

        private List<string> CollapsiblePanelDateExtenderClientIds
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
            TimeEntriesGroupByPersonDetailList = reportData.ToList();

            if (TimeEntriesGroupByPersonDetailList.Count > 0)
            {
                divEmptyMessage.Style["display"] = "none";
                repPersons.Visible = btnExpandOrCollapseAll.Visible = true;
                repPersons.DataSource = TimeEntriesGroupByPersonDetailList;
                repPersons.DataBind();
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repPersons.Visible = btnExpandOrCollapseAll.Visible = false;
            }
        }

        protected void repPersons_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                CollapsiblePanelDateExtenderClientIds = new List<string>();

            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var repDate = e.Item.FindControl("repDate") as Repeater;
                PersonLevelGroupedHours dataitem = (PersonLevelGroupedHours)e.Item.DataItem;
                sectionId = dataitem.TimeEntrySectionId;
                repDate.DataSource = dataitem.DayTotalHours;
                repDate.DataBind();

            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var output = jss.Serialize(CollapsiblePanelDateExtenderClientIds);
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

                var cpeDate = e.Item.FindControl("cpeDate") as CollapsiblePanelExtender;
                cpeDate.BehaviorID = Guid.NewGuid().ToString();
                CollapsiblePanelDateExtenderClientIds.Add(cpeDate.BehaviorID);


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
            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                var project = ServiceCallers.Custom.Project(p => p.GetProjectShortByProjectNumber(HostingPage.ProjectNumber));

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
                sb.Append(project.Status.Name);
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(HostingPage.ProjectRange);
                sb.Append("\t");
                sb.AppendLine();
                sb.AppendLine();
                if (TimeEntriesGroupByPersonDetailList.Count > 0)
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
                    foreach (var timeEntriesGroupByClientAndProject in TimeEntriesGroupByPersonDetailList)
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
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

    }
}
