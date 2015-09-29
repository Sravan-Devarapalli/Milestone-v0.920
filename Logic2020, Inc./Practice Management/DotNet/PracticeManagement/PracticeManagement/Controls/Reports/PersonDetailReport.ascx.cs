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
using PraticeManagement.Utils.Excel;
using System.Data;
using PraticeManagement.Utils;
namespace PraticeManagement.Controls.Reports
{
    public partial class PersonDetailReport : System.Web.UI.UserControl
    {
        private int SectionId;

        private string PersonDetailReportExport = "Person Detail Report";
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

                RowStyles[] rowStylearray = { headerrowStyle};

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

                CellStyles[] dataCellStylearray = { dataCellStyle,
                                                    dataCellStyle, 
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle, 
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle, 
                                                    dataCellStyle,
                                                    dataDateCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle, 
                                                    dataCellStyle,
                                                    dataCellStyle
                                                  };

                RowStyles datarowStyle = new RowStyles(dataCellStylearray);

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.TopRowNo = headerRowsCount;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanColSplit = 0;
                sheetStyle.FreezePanRowSplit = headerRowsCount;

                return sheetStyle;
            }
        }

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
                    return hostingPage.RangeForExcel;
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
            var dataSetList = new List<DataSet>();
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();

            if (StartDate.HasValue && EndDate.HasValue)
            {
                var reportData = GetReportData();
                int personId = SelectedPersonId;
                var person = ServiceCallers.Custom.Person(p => p.GetStrawmanDetailsById(personId));

                string personType = person.IsOffshore ? "Offshore" : string.Empty;
                string payType = person.CurrentPay.TimescaleName;
                string personStatusAndType = string.IsNullOrEmpty(personType) && string.IsNullOrEmpty(payType) ? string.Empty :
                                                                                 string.IsNullOrEmpty(payType) ? personType :
                                                                                 string.IsNullOrEmpty(personType) ? payType :
                                                                                                                     payType + ", " + personType;
                var filename = string.Format("{0}_{1}_{2}_{3}_{4}.xls", person.LastName, (string.IsNullOrEmpty(person.PrefferedFirstName) ? person.FirstName : person.PrefferedFirstName), "Detail", StartDate.Value.ToString("MM.dd.yyyy"), EndDate.Value.ToString("MM.dd.yyyy"));
                if (reportData.Count > 0)
                {
                    DataTable header1 = new DataTable();
                    header1.Columns.Add(person.EmployeeNumber + " - " + (string.IsNullOrEmpty(person.PrefferedFirstName) ? person.FirstName : person.PrefferedFirstName) + " " + person.LastName);

                    header1.Rows.Add(personStatusAndType);
                    header1.Rows.Add(Range);
                    headerRowsCount = header1.Rows.Count + 3;

                    var data = PrepareDataTable(reportData);
                    coloumnsCount = data.Columns.Count;
                    sheetStylesList.Add(HeaderSheetStyle);
                    sheetStylesList.Add(DataSheetStyle);
                    var dataset = new DataSet();
                    dataset.DataSetName = "PersonDetail";
                    dataset.Tables.Add(header1);
                    dataset.Tables.Add(data);
                    dataSetList.Add(dataset);
                }
                else
                {
                    string dateRangeTitle = "This person has not entered Time Entries for the selected period.";
                    DataTable header = new DataTable();
                    header.Columns.Add(dateRangeTitle);
                    sheetStylesList.Add(HeaderSheetStyle);
                    var dataset = new DataSet();
                    dataset.DataSetName = "PersonDetail";
                    dataset.Tables.Add(header);
                    dataSetList.Add(dataset);
                }
                NPOIExcel.Export(filename, dataSetList, sheetStylesList);
            }
        }

        public DataTable PrepareDataTable(List<TimeEntriesGroupByClientAndProject> report)
        {
            DataTable data = new DataTable();
            List<object> rownew;
            List<object> row;

            data.Columns.Add("Account");
            data.Columns.Add("Account Name");
            data.Columns.Add("Business Unit");
            data.Columns.Add("Business Unit Name");
            data.Columns.Add("Project");
            data.Columns.Add("Project Name");
            data.Columns.Add("Status");
            data.Columns.Add("Billing");
            data.Columns.Add("Phase");
            data.Columns.Add("Work Type");
            data.Columns.Add("Work Type Name");
            data.Columns.Add("Date");
            data.Columns.Add("Billable Hours");
            data.Columns.Add("Non-Billable Hours");
            data.Columns.Add("Actual Hours");
            data.Columns.Add("Note");
            foreach (var timeEntriesGroupByClientAndProject in report)
            {

                foreach (var byDateList in timeEntriesGroupByClientAndProject.DayTotalHours)
                {

                    foreach (var byWorkType in byDateList.DayTotalHoursList)
                    {
                        row = new List<object>();
                        row.Add(timeEntriesGroupByClientAndProject.Client.Code);
                        row.Add(timeEntriesGroupByClientAndProject.Client.HtmlEncodedName);
                        row.Add(timeEntriesGroupByClientAndProject.Project.Group.Code);
                        row.Add(timeEntriesGroupByClientAndProject.Project.Group.HtmlEncodedName);
                        row.Add(timeEntriesGroupByClientAndProject.Project.ProjectNumber);
                        row.Add(timeEntriesGroupByClientAndProject.Project.HtmlEncodedName);
                        row.Add(timeEntriesGroupByClientAndProject.Project.Status.Name);
                        row.Add(timeEntriesGroupByClientAndProject.BillableType);
                        row.Add("01");
                        row.Add(byWorkType.TimeType.Code);
                        row.Add(byWorkType.TimeType.Name);
                        row.Add(byDateList.Date);
                        row.Add(byWorkType.BillableHours);
                        row.Add(byWorkType.NonBillableHours);
                        row.Add(byWorkType.TotalHours);
                        row.Add(byWorkType.NoteForExport);
                        data.Rows.Add(row.ToArray());
                    }
                }
            }
            return data;
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

