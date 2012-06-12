﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Web.Script.Serialization;
using AjaxControlToolkit;
using System.Text;
using PraticeManagement.Reporting;

namespace PraticeManagement.Controls.Reports
{
    public partial class ProjectDetailTabByResource : System.Web.UI.UserControl
    {
        private PraticeManagement.Reporting.ProjectSummaryReport HostingPage
        {
            get
            {
                if (Page is ProjectSummaryReport)
                {
                    return ((PraticeManagement.Reporting.ProjectSummaryReport)Page);
                }
                return null;
            }
        }

        private PraticeManagement.Controls.Reports.ProjectSummaryByResource HostingControl
        {
            get
            {
                if (Page is ProjectSummaryReport)
                {
                    return
                        (PraticeManagement.Controls.Reports.ProjectSummaryByResource)((PraticeManagement.Reporting.ProjectSummaryReport)Page).ByResourceControl;
                }
                return null;

            }
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
            if (HostingPage != null)
            {
                btnExpandOrCollapseAll.Attributes["onclick"] = "return CollapseOrExpandAll(" + btnExpandOrCollapseAll.ClientID +
                                                                ", " + hdnCollapsed.ClientID +
                                                                ", " + hdncpeExtendersIds.ClientID +
                                                                ");";

                btnExpandOrCollapseAll.Text = btnExpandOrCollapseAll.ToolTip = (hdnCollapsed.Value.ToLower() == "true") ? "Expand All" : "Collapse All";
            }
            else
            {
                tblExportSection.Visible = false;
            }

            if (!IsPostBack)
            {
                hdnGroupBy.Value = "Person";
            }
        }

        public void DataBindByResourceDetail(List<PersonLevelGroupedHours> reportData)
        {
            if (reportData.Count > 0)
            {
                string groupby = hdnGroupBy.Value;
                if (groupby == "date")
                {
                    repPersons.Visible = false;
                    repDate2.Visible = true; ;
                    List<GroupByDateByPerson> groupByDateList = DataTransferObjects.Utils.Generic.GetGroupByDateList(reportData);
                    groupByDateList = groupByDateList.OrderBy(p => p.Date).ToList();
                    repDate2.DataSource = groupByDateList;
                    repDate2.DataBind();
                }
                else
                {
                    reportData = reportData.OrderBy(p => p.Person.PersonLastFirstName).ToList();
                    repPersons.Visible = true;
                    repDate2.Visible = false; ;
                    repPersons.DataSource = reportData;
                    repPersons.DataBind();
                }
            }
            else
            {
                repDate2.Visible = repPersons.Visible = false;
            }

            btnExpandOrCollapseAll.Visible =
            btnGroupBy.Visible =
            btnExportToPDF.Enabled =
            btnExportToExcel.Enabled = reportData.Count > 0;
            divEmptyMessage.Style["display"] = reportData.Count() > 0 ? "none" : "";
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
                if (dataitem.DayTotalHours == null || dataitem.DayTotalHours.Count == 0)
                {
                    var image = e.Item.FindControl("imgProject") as Image;
                    image.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
                }
                else
                {
                    CollapsiblePanelExtenderClientIds.Add(cpePerson.BehaviorID);
                }

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

        protected void repPerson2_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var repWorktype = e.Item.FindControl("repWorktype") as Repeater;
                GroupByPersonByWorktype dataitem = (GroupByPersonByWorktype)e.Item.DataItem;
                var rep = sender as Repeater;
                repWorktype.DataSource = dataitem.ProjectTotalHoursList;
                repWorktype.DataBind();
            }
        }

        protected void repDate2_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                CollapsiblePanelExtenderClientIds = new List<string>();

            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                var cpeDate = e.Item.FindControl("cpeDate") as CollapsiblePanelExtender;
                cpeDate.BehaviorID = cpeDate.ClientID + e.Item.ItemIndex.ToString();
                CollapsiblePanelExtenderClientIds.Add(cpeDate.BehaviorID);
                var repPerson2 = e.Item.FindControl("repPerson2") as Repeater;
                GroupByDateByPerson dataitem = (GroupByDateByPerson)e.Item.DataItem;
                //sectionId = dataitem.TimeEntrySectionId;
                repPerson2.DataSource = dataitem.ProjectTotalHours != null ? dataitem.ProjectTotalHours.OrderBy(p => p.Person.PersonLastFirstName).ToList() : dataitem.ProjectTotalHours;
                repPerson2.DataBind();
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

        protected void btnGroupBy_OnClick(object sender, EventArgs e)
        {
            string groupby = hdnGroupBy.Value;
            if (groupby == "date")
            {
                btnGroupBy.ToolTip = btnGroupBy.Text = "Group By Date";
                hdnGroupBy.Value = "Person";
            }
            else if (groupby == "Person")
            {
                btnGroupBy.ToolTip = btnGroupBy.Text = "Group By Person";
                hdnGroupBy.Value = "date";
            }

            List<PersonLevelGroupedHours> list = GetReportData();
            DataBindByResourceDetail(list);
            HostingControl.PopulateHeaderSection(list);
        }

        private List<PersonLevelGroupedHours> GetReportData()
        {
            List<PersonLevelGroupedHours> list =
                ServiceCallers.Custom.Report(r => r.ProjectDetailReportByResource(HostingPage.ProjectNumber, HostingPage.MilestoneId,
                    HostingPage.PeriodSelected == "0" ? HostingPage.StartDate : null, HostingPage.PeriodSelected == "0" ? HostingPage.EndDate : null,
                    HostingControl.cblProjectRolesControl.SelectedItemsXmlFormat)).ToList();
            return list;
        }
    }
}

