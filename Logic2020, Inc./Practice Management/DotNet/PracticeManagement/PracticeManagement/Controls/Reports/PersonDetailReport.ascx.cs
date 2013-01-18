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
        private int SectionId;

        private string PersonDetailReportExport = "Person Detail Report";

        private int SelectedPersonId
        {
            get
            {
                if (Page is PraticeManagement.Reporting.TimePeriodSummaryReport)
                {
                    var hostingPage = (PraticeManagement.Reporting.TimePeriodSummaryReport)Page;
                    return hostingPage.ByResourceControl.SelectedPersonForDetails;
                }
                else
                {
                    var hostingPage = (PraticeManagement.Reporting.PersonDetailTimeReport)Page;
                    return hostingPage.SelectedPersonId;
                }
            }
        }

        private DateTime? StartDate
        {
            get
            {
                if (Page is PraticeManagement.Reporting.TimePeriodSummaryReport)
                {
                    var hostingPage = (PraticeManagement.Reporting.TimePeriodSummaryReport)Page;
                    return hostingPage.StartDate;
                }
                else
                {
                    var hostingPage = (PraticeManagement.Reporting.PersonDetailTimeReport)Page;
                    return hostingPage.StartDate;
                }
            }
        }

        private DateTime? EndDate
        {
            get
            {
                if (Page is PraticeManagement.Reporting.TimePeriodSummaryReport)
                {
                    var hostingPage = (PraticeManagement.Reporting.TimePeriodSummaryReport)Page;
                    return hostingPage.EndDate;
                }
                else
                {
                    var hostingPage = (PraticeManagement.Reporting.PersonDetailTimeReport)Page;
                    return hostingPage.EndDate;
                }
            }
        }

        private string Range
        {
            get
            {
                if (Page is PraticeManagement.Reporting.TimePeriodSummaryReport)
                {
                    var hostingPage = (PraticeManagement.Reporting.TimePeriodSummaryReport)Page;
                    return hostingPage.Range;
                }
                else
                {
                    var hostingPage = (PraticeManagement.Reporting.PersonDetailTimeReport)Page;
                    return hostingPage.Range;
                }
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

            if (!IsPostBack)
            {
                hdnGroupBy.Value = "project";
            }
        }

        public void DatabindRepepeaterPersonDetails(List<TimeEntriesGroupByClientAndProject> timeEntriesGroupByClientAndProjectList)
        {

            if (timeEntriesGroupByClientAndProjectList.Count > 0)
            {
                divEmptyMessage.Style["display"] = "none";
                btnExpandOrCollapseAll.Visible = btnGroupBy.Visible = true;
                string groupby = hdnGroupBy.Value;
                if (groupby == "date")
                {
                    repProjects.Visible = false;
                    repDate2.Visible = true;
                    List<GroupByDate> groupByDateList = DataTransferObjects.Utils.Generic.GetGroupByDateList(timeEntriesGroupByClientAndProjectList);
                    groupByDateList = groupByDateList.OrderBy(p => p.Date).ToList();
                    repDate2.DataSource = groupByDateList;
                    repDate2.DataBind();
                }
                else if (groupby == "project")
                {
                    repDate2.Visible = false;
                    repProjects.Visible = true;
                    repProjects.DataSource = timeEntriesGroupByClientAndProjectList;
                    repProjects.DataBind();
                }
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repDate2.Visible = repProjects.Visible = btnExpandOrCollapseAll.Visible = btnGroupBy.Visible = false;
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
                repWorktype.DataSource = dataitem.DayTotalHoursList;
                var rep = sender as Repeater;
                var cpeDate = e.Item.FindControl("cpeDate") as CollapsiblePanelExtender;
                cpeDate.BehaviorID = Guid.NewGuid().ToString();
                CollapsiblePanelDateExtenderClientIds.Add(cpeDate.BehaviorID);
                repWorktype.DataBind();
            }
        }

        protected void repDate2_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                CollapsiblePanelDateExtenderClientIds = new List<string>();

            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var repProject2 = e.Item.FindControl("repProject2") as Repeater;
                GroupByDate dataitem = (GroupByDate)e.Item.DataItem;
                repProject2.DataSource = dataitem.ProjectTotalHours;
                repProject2.DataBind();
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

        protected void repProject2_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {

            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var repWorktype = e.Item.FindControl("repWorktype") as Repeater;
                GroupByClientAndProject dataitem = (GroupByClientAndProject)e.Item.DataItem;
                SectionId = dataitem.Project.TimeEntrySectionId;
                repWorktype.DataSource = dataitem.ProjectTotalHoursList;
                var rep = sender as Repeater;
                var cpeProject = e.Item.FindControl("cpeProject") as CollapsiblePanelExtender;
                cpeProject.BehaviorID = Guid.NewGuid().ToString();
                CollapsiblePanelDateExtenderClientIds.Add(cpeProject.BehaviorID);
                repWorktype.DataBind();
            }
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected string GetDateFormat(DateTime date)
        {
            return date.ToString(Constants.Formatting.ReportDateFormat);
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

        protected string GetNoteFormated(String note)
        {
            return note.Replace("\n", "<br/>");
        }

        protected bool GetNonBillableImageVisibility(int sectionId, double nonBillableHours)
        {
            return sectionId == -1 ? SectionId == 1 && nonBillableHours > 0 : sectionId == 1 && nonBillableHours > 0;
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(PersonDetailReportExport);

            if (StartDate.HasValue && EndDate.HasValue)
            {
                var reportData = GetReportData();
                int personId = SelectedPersonId;
                var person = ServiceCallers.Custom.Person(p => p.GetStrawmanDetailsById(personId));

                StringBuilder sb = new StringBuilder();
                string personType = person.IsOffshore ? "Offshore" : string.Empty;
                string payType = person.CurrentPay.TimescaleName;
                string personStatusAndType = string.IsNullOrEmpty(personType) && string.IsNullOrEmpty(payType) ? string.Empty :
                                                                                 string.IsNullOrEmpty(payType) ? personType :
                                                                                 string.IsNullOrEmpty(personType) ? payType :
                                                                                                                     payType + ", " + personType;
                sb.Append(person.EmployeeNumber + " - " + person.FirstName + " " + person.LastName);
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(personStatusAndType);
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(Range);
                sb.Append("\t");
                sb.AppendLine();
                sb.AppendLine();
                if (reportData.Count > 0)
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
                    sb.Append("Actual Hours");
                    sb.Append("\t");
                    sb.Append("Note");
                    sb.Append("\t");
                    sb.AppendLine();

                    //Data
                    foreach (var timeEntriesGroupByClientAndProject in reportData)
                    {

                        foreach (var byDateList in timeEntriesGroupByClientAndProject.DayTotalHours)
                        {

                            foreach (var byWorkType in byDateList.DayTotalHoursList)
                            {
                                sb.Append(timeEntriesGroupByClientAndProject.Client.Code);
                                sb.Append("\t");
                                sb.Append(timeEntriesGroupByClientAndProject.Client.HtmlEncodedName);
                                sb.Append("\t");

                                sb.Append(timeEntriesGroupByClientAndProject.Project.Group.Code);
                                sb.Append("\t");
                                sb.Append(timeEntriesGroupByClientAndProject.Project.Group.HtmlEncodedName);
                                sb.Append("\t");
                                sb.Append(timeEntriesGroupByClientAndProject.Project.ProjectNumber);
                                sb.Append("\t");
                                sb.Append(timeEntriesGroupByClientAndProject.Project.HtmlEncodedName);
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
                                sb.Append(byWorkType.HtmlEncodedNoteForExport);
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
                var filename = string.Format("{0}_{1}_{2}_{3}_{4}.xls", person.LastName, person.FirstName, "Detail", StartDate.Value.ToString("MM.dd.yyyy"), EndDate.Value.ToString("MM.dd.yyyy"));
                GridViewExportUtil.Export(filename, sb);

            }
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

        protected void btnGroupBy_OnClick(object sender, EventArgs e)
        {
            string groupby = hdnGroupBy.Value;
            if (groupby == "date")
            {
                btnGroupBy.ToolTip = btnGroupBy.Text = "Group By Date";
                hdnGroupBy.Value = "project";
            }
            else if (groupby == "project")
            {
                btnGroupBy.ToolTip = btnGroupBy.Text = "Group By Project";
                hdnGroupBy.Value = "date";
            }

            var list = GetReportData();
            DatabindRepepeaterPersonDetails(list);
        }

        private List<TimeEntriesGroupByClientAndProject> GetReportData()
        {
            List<TimeEntriesGroupByClientAndProject> list = new List<TimeEntriesGroupByClientAndProject>();
            list = ServiceCallers.Custom.Report(r => r.PersonTimeEntriesDetails(SelectedPersonId, StartDate.Value, EndDate.Value)).ToList();

            if (Page is PraticeManagement.Reporting.TimePeriodSummaryReport)
            {
                var hostingPage = (PraticeManagement.Reporting.TimePeriodSummaryReport)Page;
                hostingPage.ByResourceControl.PersonDetailPopup.Show();
            }

            return list;
        }
    }
}

