using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using AjaxControlToolkit;
using System.Web.UI.HtmlControls;
using System.Web.Script.Serialization;
using System.Text;
namespace PraticeManagement.Controls.Reports
{
    public partial class PersonDetailReport : System.Web.UI.UserControl
    {
        private PraticeManagement.Reporting.PersonDetailTimeReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.PersonDetailTimeReport)Page); }
        }

        private int SectionId;

        public List<TimeEntriesGroupByClientAndProject> TimeEntriesGroupByClientAndProjectList
        {
            get
            {
                return ViewState["TimeEntriesGroupByClientAndProjectList_Key_Detail"] as List<TimeEntriesGroupByClientAndProject>;
            }
            set
            {
                ViewState["TimeEntriesGroupByClientAndProjectList_Key_Detail"] = value;
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

        public void DatabindRepepeaterProjectDetails(List<TimeEntriesGroupByClientAndProject> timeEntriesGroupByClientAndProjectList)
        {
            TimeEntriesGroupByClientAndProjectList = timeEntriesGroupByClientAndProjectList;

            if (timeEntriesGroupByClientAndProjectList.Count > 0)
            {
                divEmptyMessage.Style["display"] = "none";
                repProjects.Visible = btnExpandOrCollapseAll.Visible = true;
                repProjects.DataSource = timeEntriesGroupByClientAndProjectList;
                repProjects.DataBind();
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repProjects.Visible = btnExpandOrCollapseAll.Visible = false;
            }
        }

        protected void repProjects_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                CollapsiblePanelDateExtenderClientIds = new List<string>();

            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var repDate = e.Item.FindControl("repDate") as Repeater;
                TimeEntriesGroupByClientAndProject dataitem = (TimeEntriesGroupByClientAndProject)e.Item.DataItem;
                SectionId = dataitem.Project.TimeEntrySectionId;
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

        protected string GetProjectStatus(string status)
        {
            return string.IsNullOrEmpty(status) ? "" : "(" + status + ")";
        }

        protected bool GetNoteVisibility(String note)
        {
            if (String.IsNullOrEmpty(note))
            {
                return false;
            }
            return true;

        }

        protected bool GetNonBillableImageVisibility(double nonBillableHours)
        {
            return SectionId == 1 && nonBillableHours > 0;
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                int personId = HostingPage.SelectedPersonId;
                var person = ServiceCallers.Custom.Person(p => p.GetStrawmanDetailsById(personId));

                StringBuilder sb = new StringBuilder();
                sb.Append(person.FirstName + " " + person.LastName);
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(person.CurrentPay.TimescaleName);
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(HostingPage.Range);
                sb.Append("\t");
                sb.AppendLine();
                sb.AppendLine();
                if (TimeEntriesGroupByClientAndProjectList.Count > 0)
                {
                    //Header
                    /* Account	Account Name	Business Unit	Business Unit Name	Project	Project Name	Phase	
                    Work Type	Work Type Name	Date	Billable Hours	Non-Billable Hours	Total Hours	Note */
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
                    sb.Append("Phase");
                    sb.Append("\t");
                    sb.Append("Work Type");
                    sb.Append("\t");
                    sb.Append("Work Type Name");
                    sb.Append("\t");
                    sb.Append("Date");
                    sb.Append("\t");
                    sb.Append("Billable Hours");
                    sb.Append("\t");
                    sb.Append("Non-Billable Hours");
                    sb.Append("\t");
                    sb.Append("Total Hours");
                    sb.Append("\t");
                    sb.Append("Note");
                    sb.Append("\t");
                    sb.AppendLine();

                    //Data
                    foreach (var timeEntriesGroupByClientAndProject in TimeEntriesGroupByClientAndProjectList)
                    {

                        foreach (var byDateList in timeEntriesGroupByClientAndProject.DayTotalHours)
                        {

                            foreach (var byWorkType in byDateList.DayTotalHoursList)
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
                                sb.Append("01");//phase
                                sb.Append("\t");
                                sb.Append(byWorkType.TimeType.Code);
                                sb.Append("\t");
                                sb.Append(byWorkType.TimeType.Name);
                                sb.Append("\t");
                                sb.Append(byDateList.Date.ToString("MM/dd/yyyy"));
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
                }
                else
                {
                    sb.Append("This person has not entered Time Entries for the selected period.");
                }
                var filename = string.Format("{0}_{1}_{2}_{3}_{4}.xls", person.LastName, person.FirstName, "Detail", HostingPage.StartDate.Value.ToString("MM.dd.yyyy"), HostingPage.EndDate.Value.ToString("MM.dd.yyyy"));
                GridViewExportUtil.Export(filename, sb);

            }
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

    }
}

