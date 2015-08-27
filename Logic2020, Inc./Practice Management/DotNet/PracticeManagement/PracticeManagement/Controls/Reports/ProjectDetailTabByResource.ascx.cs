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
using PraticeManagement.Reporting;
using PraticeManagement.Utils.Excel;
using System.Data;
using PraticeManagement.Utils;

namespace PraticeManagement.Controls.Reports
{
    public partial class ProjectDetailTabByResource : System.Web.UI.UserControl
    {
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;

        private SheetStyles HeaderSheetStyle
        {
            get
            {
                CellStyles cellStyle = new CellStyles();
                cellStyle.IsBold = true;
                cellStyle.BorderStyle = NPOI.SS.UserModel.BorderStyle.None;
                cellStyle.FontHeight = 200;
                CellStyles[] cellStylearray = { cellStyle };
                RowStyles headerrowStyle = new RowStyles(cellStylearray);
                headerrowStyle.Height = 350;

                CellStyles dataCellStyle = new CellStyles();
                CellStyles[] dataCellStylearray = { dataCellStyle };
                RowStyles datarowStyle = new RowStyles(dataCellStylearray);

                RowStyles[] rowStylearray = { headerrowStyle };

                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.IsAutoResize = false;

                return sheetStyle;
            }
        }

        private SheetStyles DataSheetStyle
        {
            get
            {
                CellStyles headerCellStyle = new CellStyles();
                headerCellStyle.IsBold = true;
                headerCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                List<CellStyles> headerCellStyleList = new List<CellStyles>();
                headerCellStyleList.Add(headerCellStyle);
                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataCellStyle = new CellStyles();

                CellStyles dataDateCellStyle = new CellStyles();
                dataDateCellStyle.DataFormat = "mm/dd/yy;@";

                var dataCurrancyCellStyle = new CellStyles { DataFormat = "$#,##0.00_);($#,##0.00)" };

                var dataCellStylearray = new List<CellStyles>() { dataCellStyle,
                                                    dataCellStyle, 
                                                    dataCellStyle,
                                                    dataDateCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle, 
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle, 
                                                    dataCellStyle,
                                                    dataCurrancyCellStyle,
                                                    dataCellStyle
                                                  };
                RowStyles datarowStyle = new RowStyles(dataCellStylearray.ToArray());

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.TopRowNo = headerRowsCount;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanColSplit = 0;
                sheetStyle.FreezePanRowSplit = headerRowsCount;

                return sheetStyle;
            }
        }

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

        private string ProjectNumber
        {
            get
            {
                if (Page is PraticeManagement.Reporting.TimePeriodSummaryReport)
                {
                    return ((PraticeManagement.Reporting.TimePeriodSummaryReport)Page).ByProjectControl.SelectedProjectNumber;
                }

                return HostingPage.ProjectNumber;
            }
        }

        private int? MilestoneId
        {
            get
            {
                if (Page is PraticeManagement.Reporting.TimePeriodSummaryReport)
                {
                    return null;
                }

                return HostingPage.MilestoneId;
            }
        }

        private string PeriodSelected
        {
            get
            {
                if (Page is PraticeManagement.Reporting.TimePeriodSummaryReport)
                {
                    return "0";
                }

                return HostingPage.PeriodSelected;
            }
        }

        private DateTime? StartDate
        {
            get
            {
                if (Page is PraticeManagement.Reporting.TimePeriodSummaryReport)
                {
                    return ((PraticeManagement.Reporting.TimePeriodSummaryReport)Page).StartDate;
                }

                return HostingPage.StartDate;
            }
        }

        private DateTime? EndDate
        {
            get
            {
                if (Page is PraticeManagement.Reporting.TimePeriodSummaryReport)
                {
                    return ((PraticeManagement.Reporting.TimePeriodSummaryReport)Page).EndDate;
                }

                return HostingPage.EndDate;
            }
        }

        private string ProjectRange
        {
            get
            {
                if (Page is PraticeManagement.Reporting.TimePeriodSummaryReport)
                {
                    return StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat);
                }
                return HostingPage.ProjectRangeForExcel;
            }
        }

        private string ProjectRoles
        {
            get
            {
                if (Page is PraticeManagement.Reporting.TimePeriodSummaryReport)
                {
                    return null;
                }

                return HostingControl.cblProjectRolesControl.SelectedItemsXmlFormat;
            }
        }

        private int sectionId;

        public HiddenField hdnGroupByControl
        {
            get
            {
                return hdnGroupBy;
            }
        }

        public Button btnGroupByControl
        {
            get
            {
                return btnGroupBy;
            }
        }

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

            if (HostingPage == null)
            {
                btnExportToPDF.Attributes.Add("disabled", "disabled");
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
                sectionId = dataitem.TimeEntrySectionId;
                repDate.DataSource = dataitem.DayTotalHours != null ? dataitem.DayTotalHours.OrderBy(p => p.Date).ToList() : dataitem.DayTotalHours;
                repDate.DataBind();
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

                var cpeDate = e.Item.FindControl("cpeDate") as CollapsiblePanelExtender;
                cpeDate.BehaviorID = cpeDate.ClientID + e.Item.ItemIndex.ToString();
                CollapsiblePanelExtenderClientIds.Add(cpeDate.BehaviorID);


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
                var cpePerson = e.Item.FindControl("cpePerson") as CollapsiblePanelExtender;
                cpePerson.BehaviorID = Guid.NewGuid().ToString();
                CollapsiblePanelExtenderClientIds.Add(cpePerson.BehaviorID);
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
                var repPerson2 = e.Item.FindControl("repPerson2") as Repeater;
                GroupByDateByPerson dataitem = (GroupByDateByPerson)e.Item.DataItem;
                sectionId = dataitem.TimeEntrySectionId;
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

            var dataSetList = new List<DataSet>();
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();

            var project = ServiceCallers.Custom.Project(p => p.GetProjectShortByProjectNumber(ProjectNumber, MilestoneId, StartDate, EndDate));
            List<PersonLevelGroupedHours> report = GetReportData(true);

            string filterApplied = "Filters applied to columns: ";
            bool isFilterApplied = false;
            if (HostingControl != null && !HostingControl.cblProjectRolesControl.AllItemsSelected)
            {
                filterApplied = filterApplied + " ProjectRoles.";
                isFilterApplied = true;
            }

            var filename = string.Format("{0}_{1}_{2}.xls", project.ProjectNumber, project.Name, "_ByResourceDetail");
            filename = filename.Replace(' ', '_');
            if (report.Count > 0)
            {
                DataTable header1 = new DataTable();
                header1.Columns.Add(project.Client.HtmlEncodedName);
                header1.Columns.Add(project.Group.HtmlEncodedName);

                var row1 = new List<object>();
                row1.Add(string.Format("{0} - {1}", project.ProjectNumber, project.HtmlEncodedName));
                row1.Add("");
                header1.Rows.Add(row1.ToArray());

                var row2 = new List<object>();
                row2.Add(string.IsNullOrEmpty(project.BillableType) ? project.Status.Name : project.Status.Name + ", " + project.BillableType);
                row2.Add("");
                header1.Rows.Add(row2.ToArray());

                var row3 = new List<object>();
                row3.Add(ProjectRange);
                row3.Add("");
                header1.Rows.Add(row3.ToArray());

                var row4 = new List<object>();
                if (isFilterApplied)
                {
                    row4.Add(filterApplied);
                    row4.Add("");
                    header1.Rows.Add(row4.ToArray());
                }
                headerRowsCount = header1.Rows.Count + 3;

                var data = PrepareDataTable(report);
                coloumnsCount = data.Columns.Count;
                sheetStylesList.Add(HeaderSheetStyle);
                sheetStylesList.Add(DataSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "Project_ByResourceDetail";
                dataset.Tables.Add(header1);
                dataset.Tables.Add(data);
                dataSetList.Add(dataset);
            }
            else
            {
                string dateRangeTitle = "There are no Time Entries towards this project.";
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                sheetStylesList.Add(HeaderSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "Project_ByResourceDetail";
                dataset.Tables.Add(header);
                dataSetList.Add(dataset);
            }


            NPOIExcel.Export(filename, dataSetList, sheetStylesList);
        }

        public DataTable PrepareDataTable(List<PersonLevelGroupedHours> report)
        {
            DataTable data = new DataTable();
            List<object> rownew;
            List<object> row;

            data.Columns.Add("Employee Id");
            data.Columns.Add("Resource");
            data.Columns.Add("Project Role");
            data.Columns.Add("Date");
            data.Columns.Add("WorkType");
            data.Columns.Add("WorkType Name");
            data.Columns.Add("Billable");
            data.Columns.Add("Non-Billable");
            data.Columns.Add("Actual Hours");
            data.Columns.Add("Projected Hours");
            data.Columns.Add("Bill Rate");
            data.Columns.Add("Note");

            foreach (var timeEntriesGroupByClientAndProject in report)
            {
                if (timeEntriesGroupByClientAndProject.DayTotalHours != null)
                {
                    foreach (var byDateList in timeEntriesGroupByClientAndProject.DayTotalHours)
                    {
                        foreach (var byWorkType in byDateList.DayTotalHoursList)
                        {
                            row = new List<object>();
                            row.Add(timeEntriesGroupByClientAndProject.Person.EmployeeNumber);
                            row.Add(timeEntriesGroupByClientAndProject.Person.HtmlEncodedName);
                            row.Add(timeEntriesGroupByClientAndProject.Person.ProjectRoleName);
                            row.Add(byDateList.Date);
                            row.Add(byWorkType.TimeType.Code);
                            row.Add(byWorkType.TimeType.Name);
                            row.Add(byWorkType.BillableHours);
                            row.Add(byWorkType.NonBillableHours);
                            row.Add(byWorkType.TotalHours);
                            row.Add(byWorkType.ForecastedHoursDaily);
                            row.Add(byWorkType.BillingType == "Fixed" ? "FF" : byWorkType.BillRate.ToString());
                            row.Add(byWorkType.NoteForExport);
                            data.Rows.Add(row.ToArray());
                        }
                    }
                }
                else
                {
                    row = new List<object>();
                    row.Add(timeEntriesGroupByClientAndProject.Person.EmployeeNumber);
                    row.Add(timeEntriesGroupByClientAndProject.Person.PersonLastFirstName);
                    row.Add(timeEntriesGroupByClientAndProject.Person.ProjectRoleName);
                    row.Add("");
                    row.Add("");
                    row.Add("");
                    row.Add("0");
                    row.Add("0");
                    row.Add("0");
                    row.Add("0");
                    row.Add("0");
                    row.Add("");
                    data.Rows.Add(row.ToArray());
                }
            }
            return data;
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

            if (HostingControl != null)
                HostingControl.PopulateHeaderSection(list);
        }

        private List<PersonLevelGroupedHours> GetReportData(bool fromExport = false)
        {
            List<PersonLevelGroupedHours> list =
                ServiceCallers.Custom.Report(r => r.ProjectDetailReportByResource(ProjectNumber, MilestoneId,
                    PeriodSelected == "*" ? null : StartDate, PeriodSelected == "*" ? null : EndDate,
                    ProjectRoles, fromExport)).ToList();

            if (Page is PraticeManagement.Reporting.TimePeriodSummaryReport)
            {
                ((PraticeManagement.Reporting.TimePeriodSummaryReport)Page).ByProjectControl.MpeProjectDetailReport.Show();
            }

            return list;
        }
    }
}

