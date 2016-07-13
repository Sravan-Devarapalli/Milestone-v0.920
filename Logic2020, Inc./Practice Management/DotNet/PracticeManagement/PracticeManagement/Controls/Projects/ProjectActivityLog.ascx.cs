using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using PraticeManagement.Configuration;
using PraticeManagement.Utils;
using DataTransferObjects;
using PraticeManagement.FilterObjects;
using System.Xml.Xsl;
using PraticeManagement.Utils.Excel;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;


namespace PraticeManagement.Controls.Projects
{
    public partial class ProjectActivityLog : System.Web.UI.UserControl
    {
        #region Constants

        private const string ViewstateDisplayVisible = "DISPLAY_VISIBLE";
        private const string ProjectDisplayViewstate = "DISPLAY_PROJECT";
        private const string PersonDisplayViewstate = "DISPLAY_PERSON";
        private const string ViewstateEventSource = "EVENT_SOURCE";
        private const string OpportunityIdViewstate = "OPPORTUNITY_ID";
        private const string MilestoneIdViewstate = "MILESTONE_ID";
        private const string ModifiedByNameAttribute = "ModifiedByName";
        private const string UserNameAttribute = "UserName";
        private const string UserAttribute = "User";

        private const string LoggedInActivity = "logged in";
        private const string NewValuesTag = "new_values";
        private const string LoginTag = "login";
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;
        private const string BoldFontStyle = "<BoldFont>{0}</BoldFont>";

        #endregion

        #region Titles

        //  Provides mapping between event sources and drop-down item titles
        private static readonly Dictionary<ActivityEventSource, string> EventSourceTitles =
            new Dictionary<ActivityEventSource, string>();

        //  Provides mapping between even sources and target drop-downs to set value at
        private static readonly Dictionary<ActivityEventSource, DropDownList> ControlMapping =
            new Dictionary<ActivityEventSource, DropDownList>();

        private string _currentUrl;
        private XsltArgumentList _argumentList;

        static ProjectActivityLog()
        {
        }

        public string LabelTextBeforeDropDown
        {
            get { return Label3.Text; }
            set { Label3.Text = value; }
        }


        /// <summary>
        /// 	Default constructor of ActivityLogControl.
        /// </summary>
        public ProjectActivityLog()
        {
            ControlMapping.Clear();
            ControlMapping.Add(ActivityEventSource.Project, ddlProjects);
            ControlMapping.Add(ActivityEventSource.Person, ddlPersonName);
        }

        protected void InitXsltParams()
        {
            _currentUrl = HttpUtility.UrlEncode(Request.Url.AbsoluteUri) + (IsActivityLogPage ? (Request.Url.Query.Length > 0 ? string.Empty : Constants.FilterKeys.QueryStringOfApplyFilterFromCookie) : string.Empty);

            _argumentList = new XsltArgumentList();
            _argumentList.AddParam("currentUrl", string.Empty, _currentUrl);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 	Defines whether to show Display drop-down or not
        /// </summary>
        public bool ShowDisplayDropDown
        {
            get
            {
                var obj = ViewState[ViewstateDisplayVisible];

                if (obj != null)
                    return (bool)obj;

                return true;
            }

            set { ViewState[ViewstateDisplayVisible] = value; }
        }

        /// <summary>
        /// 	Defines whether to show Project drop-down or not
        /// </summary>
        public bool ShowProjectDropDown
        {
            get
            {
                var obj = ViewState[ProjectDisplayViewstate];

                if (obj != null)
                    return (bool)obj;

                return true;
            }

            set { ViewState[ProjectDisplayViewstate] = value; }
        }

        /// <summary>
        /// 	Defines whether to show Project drop-down or not
        /// </summary>
        public bool ShowPersonDropDown
        {
            get
            {
                var obj = ViewState[PersonDisplayViewstate];

                if (obj != null)
                    return (bool)obj;

                return true;
            }

            set { ViewState[PersonDisplayViewstate] = value; }
        }

        /// <summary>
        /// 	Defines whether to show Display drop-down or not
        /// </summary>
        public int? OpportunityId
        {
            get
            {
                var obj = ViewState[OpportunityIdViewstate];

                if (obj != null)
                    return (int?)obj;

                return null;
            }

            set { ViewState[OpportunityIdViewstate] = value; }
        }

        public int? MilestoneId
        {
            get
            {
                var obj = ViewState[MilestoneIdViewstate];

                if (obj != null)
                    return (int?)obj;

                return null;
            }

            set { ViewState[MilestoneIdViewstate] = value; }
        }
        /// <summary>
        /// 	Sets Display drop-down value
        /// </summary>
        public ActivityEventSource DisplayDropDownValue
        {
            get
            {
                var obj = ViewState[ViewstateEventSource];

                if (obj != null)
                    return (ActivityEventSource)obj;

                return ActivityEventSource.All;
            }
            set
            {
                ViewState[ViewstateEventSource] = value;
            }
        }

        public bool IsFreshRequest { get; set; }

        public bool IsActivityLogPage { get; set; }

        public bool ValidationSummaryEnabled
        {
            get
            {
                return valSum.Enabled;
            }
            set
            {
                valSum.Enabled = value;
            }
        }

        #region ActivityLogExport Properties

        public string SourceFilter { get { return ViewState["SourceFilter"] as string; } set { ViewState["SourceFilter"] = value; } }
        public DateTime StartDateFilter
        {
            get { return (DateTime)ViewState["StartDateFilter"]; }
            set { ViewState["StartDateFilter"] = value; }
        }
        public DateTime EndDateFilter { get { return (DateTime)ViewState["EndDateFilter"]; } set { ViewState["EndDateFilter"] = value; } }
        public string PersonId { get { return ViewState["PersonId"] as string; } set { ViewState["PersonId"] = value; } }
        public string ProjectId { get { return ViewState["ProjectId"] as string; } set { ViewState["ProjectId"] = value; } }
        //public string VendortId { get { return ViewState["VendortId"] as string; } set { ViewState["VendortId"] = value; } }
        public string OpportunityIdFilter { get { return ViewState["OpportunityIdFilter"] as string; } set { ViewState["OpportunityIdFilter"] = value; } }
        public string MilestoneIdFilter { get { return ViewState["MilestoneIdFilter"] as string; } set { ViewState["MilestoneIdFilter"] = value; } }
        public int StartRow
        {
            get
            {
                if (ViewState["StartRow"] == null)
                    ViewState["StartRow"] = 0;
                return (int)ViewState["StartRow"];
            }
            set { ViewState["StartRow"] = value; }
        }
        public int MaxRow
        {
            get
            {
                if (ViewState["MaxRow"] == null)
                    ViewState["MaxRow"] = 20;
                return (int)ViewState["MaxRow"];
            }
            set { ViewState["MaxRow"] = value; }
        }
        public bool PracticeAreas { get { return (bool)ViewState["PracticeAreas"]; } set { ViewState["PracticeAreas"] = value; } }
        public bool Division { get { return (bool)ViewState["Division"]; } set { ViewState["Division"] = value; } }
        public bool Channel { get { return (bool)ViewState["Channel"]; } set { ViewState["Channel"] = value; } }
        public bool Offering { get { return (bool)ViewState["Offering"]; } set { ViewState["Offering"] = value; } }
        public bool RevenueType { get { return (bool)ViewState["RevenueType"]; } set { ViewState["RevenueType"] = value; } }
        public bool SowBudget { get { return (bool)ViewState["SowBudget"]; } set { ViewState["SowBudget"] = value; } }
        public bool Director { get { return (bool)ViewState["Director"]; } set { ViewState["Director"] = value; } }
        public bool POAmount { get { return (bool)ViewState["POAmount"]; } set { ViewState["POAmount"] = value; } }
        public bool Capabilites { get { return (bool)ViewState["Capabilites"]; } set { ViewState["Capabilites"] = value; } }
        public bool NewOrExtension { get { return (bool)ViewState["NewOrExtension"]; } set { ViewState["NewOrExtension"] = value; } }
        public bool PoNumber { get { return (bool)ViewState["PoNumber"]; } set { ViewState["PoNumber"] = value; } }
        public bool ProjectStatus { get { return (bool)ViewState["ProjectStatus"]; } set { ViewState["ProjectStatus"] = value; } }
        public bool SalesPerson { get { return (bool)ViewState["SalesPerson"]; } set { ViewState["SalesPerson"] = value; } }
        public bool ProjectOwner { get { return (bool)ViewState["ProjectOwner"]; } set { ViewState["ProjectOwner"] = value; } }

        #endregion

        #endregion

        private PraticeManagement.ProjectDetail HostingPage
        {
            get { return ((PraticeManagement.ProjectDetail)Page); }
        }

        public DateTime? FromDateFilterValue
        {
            get { return diRange.FromDate; }
            set { diRange.FromDate = value; }
        }

        public DateTime? ToDateFilterValue
        {
            get { return diRange.ToDate; }
            set { diRange.ToDate = value; }
        }

        public bool IsFiltersReadingFromCookie { get; set; }

        private SheetStyles HeaderSheetStyle
        {
            get
            {
                CellStyles cellStyle = new CellStyles();
                cellStyle.IsBold = true;
                cellStyle.BorderStyle = NPOI.SS.UserModel.BorderStyle.None;
                cellStyle.FontHeight = 350;
                CellStyles[] cellStylearray = { cellStyle };
                RowStyles headerrowStyle = new RowStyles(cellStylearray);
                headerrowStyle.Height = 500;

                CellStyles dataCellStyle = new CellStyles();
                dataCellStyle.IsBold = true;
                dataCellStyle.BorderStyle = NPOI.SS.UserModel.BorderStyle.None;
                dataCellStyle.FontHeight = 200;
                CellStyles[] dataCellStylearray = { dataCellStyle };
                RowStyles datarowStyle = new RowStyles(dataCellStylearray);
                datarowStyle.Height = 350;
                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.MergeRegion.Add(new int[] { 0, 0, 0, coloumnsCount - 1 });
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

                CellStyles dateCellStyle = new CellStyles();
                dateCellStyle.DataFormat = "[$-409]m/d/yyyy h:mm AM/PM;@";

                CellStyles[] dataCellStylearray = { dateCellStyle, 
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

        private static string GetStringByValue(ActivityEventSource value)
        {
            //return Enum.GetName(typeof(ActivityEventSource), value);
            int val = (int)value;
            return val.ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack || IsFreshRequest)
            {
                FillEventList();
                FillFields();
                ddlProjects.DataBind();
                ddlPersonName.DataBind();

                if (!IsActivityLogPage)
                {
                    var display = ShowDisplayDropDown;
                    ddlEventSource.Visible = display;
                    lblDisplay.Visible = display;
                    spnProjects.Visible = ShowProjectDropDown;
                    spnPersons.Visible = ShowPersonDropDown;
                }
            }

            InitXsltParams();

            if (!IsPostBack)
            {
                ResetFilters();
            }

            ddlEventSource.Width = Unit.Pixel(100);
            ddlPersonName.Width = Unit.Pixel(150);
            ddlProjects.Width = Unit.Pixel(150);
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            var filename = "ProjectActivityLog.xls";
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();

            var report = ActivityLogHelper.GetActivities(SourceFilter, StartDateFilter, EndDateFilter, PersonId, ProjectId,"0", OpportunityIdFilter, MilestoneIdFilter, StartRow, MaxRow, PracticeAreas, SowBudget, Director, POAmount, Capabilites, NewOrExtension, PoNumber, ProjectStatus, SalesPerson, ProjectOwner, true).ToList();

            if (report.Count > 0)
            {
                DataTable header1 = new DataTable();
                header1.Columns.Add("Project Activity Log");
                headerRowsCount = header1.Rows.Count + 3;
                var data = PrepareDataTable(report);
                coloumnsCount = data.Columns.Count;
                sheetStylesList.Add(HeaderSheetStyle);
                sheetStylesList.Add(DataSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "ProjectActivityLog";
                dataset.Tables.Add(header1);
                dataset.Tables.Add(data);
                dataSetList.Add(dataset);
            }
            else
            {
                string dateRangeTitle = "There are no activities with the selected parameters for this project.";
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                sheetStylesList.Add(HeaderSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "ProjectActivityLog";
                dataset.Tables.Add(header);
                dataSetList.Add(dataset);
            }

            NPOIExcel.Export(filename, dataSetList, sheetStylesList);
        }

        public DataTable PrepareDataTable(List<ActivityLogItem> report)
        {
            DataTable data = new DataTable();
            List<object> row;
            data.Columns.Add("Modified Date");
            data.Columns.Add("Modified by");
            data.Columns.Add("Activity");
            data.Columns.Add("Changes");
            foreach (var activityItem in report)
            {
                row = new List<object>();
                row.Add(activityItem.LogDate);
                row.Add(GetModifiedByDetails(activityItem.Person != null ? (int?)activityItem.Person.Id.Value : null, activityItem.Person != null ? activityItem.Person.PersonLastFirstName : null, activityItem.SystemUser, activityItem.LogData));
                row.Add(NoNeedToShowActivityType(activityItem.ActivityName) + " " + string.Format(BoldFontStyle, Regex.Replace(GetActivityItem(activityItem.LogData, true), @"\s+", " ")));
                var item = Regex.Replace(GetActivityItem(activityItem.LogData, false), @"\s+", " ");
                row.Add(item);
                data.Rows.Add(row.ToArray());
            }
            return data;
        }

        public string GetActivityItem(string logData, bool item)
        {
            Xml xml = new Xml();
            xml.DocumentContent = AddDefaultProjectAndMileStoneInfo(logData).ToString();
            xml.TransformSource = item ? "~/Reports/Xslt/ActivityLogItem_TextMode.xslt" : "~/Reports/Xslt/ActivityLogChanges_TextMode.xslt";
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    xml.RenderControl(htw);
                    return htw.InnerWriter.ToString();
                }
            }
        }

        public void FillFields()
        {
            var fieldsList = new List<ListItem>();
            fieldsList.Add(new ListItem("Practice Areas", "PracticeAreas"));
            fieldsList.Add(new ListItem("Division", "Division"));
            fieldsList.Add(new ListItem("Channel", "Channel"));
            fieldsList.Add(new ListItem("Offering", "Offering"));
            fieldsList.Add(new ListItem("RevenueType", "RevenueType"));
            fieldsList.Add(new ListItem("SOW Budget", "SOWBudget"));
            fieldsList.Add(new ListItem("Executive in Charge", "ClientDirector"));
            fieldsList.Add(new ListItem("PO Amount", "POAmount"));
            fieldsList.Add(new ListItem("Capabilities", "Capabilities"));
            fieldsList.Add(new ListItem("New/Extension", "New/Extension"));
            fieldsList.Add(new ListItem("PO Number", "PONumber"));
            fieldsList.Add(new ListItem("Project Status", "ProjectStatus"));
            fieldsList.Add(new ListItem("Sales Person", "SalesPerson"));
            fieldsList.Add(new ListItem("Project Manager", "ProjectOwner"));
            DataHelper.FillListDefault(cblFields, "All Fields", fieldsList.ToArray(), false, "Value", "Text");
            //cblFields.DataSource = fieldsList;
            //cblFields.DataBind();
            cblFields.SelectAll();
        }

        private void ResetFilters()
        {
            if (!IsFiltersReadingFromCookie)
            {
                SetPeriodSelection(-7);
            }

            if (IsActivityLogPage)
            {
                if (!IsFiltersReadingFromCookie)
                {
                    ddlPeriod.SelectedValue = "-7";
                    ddlEventSource.SelectedIndex = ddlPersonName.SelectedIndex = ddlProjects.SelectedIndex = 0;
                    hdnResetFilter.Value = "false";
                    btnResetFilter.Enabled = false;
                }
                ActivityLogOnChangeEvents();
            }
            else
            {
                ddlPeriod.Attributes["onchange"] = "CheckAndShowCustomDatesPoup(this);";
            }
        }

        private void ActivityLogOnChangeEvents()
        {
            ddlPeriod.Attributes["onchange"] = "EnableResetButton(); CheckAndShowCustomDatesPoup(this);";
            ddlEventSource.Attributes["onchange"] = "disableProjectsDropDown(); EnableResetButton();";
            ddlPersonName.Attributes["onchange"] = ddlProjects.Attributes["onchange"] = "EnableResetButton();";
            diRange.OnClientChange = "EnableResetButtonForDateIntervalChange";
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {

            if (IsActivityLogPage)
            {
                ActivityLogOnChangeEvents();
                EnableProjectsDropDown();
            }
            else
            {
                ddlPeriod.Attributes["onchange"] = "CheckAndShowCustomDatesPoup(this);";
            }

            if (!Request.Url.AbsolutePath.Contains("PersonDetail.aspx"))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "SetTooltipsForallDropDowns", "SetTooltipsForallDropDowns();ModifyInnerTextToWrapText();", true);
            }

            if (IsFreshRequest)
            {
                Update();
            }

            lblCustomDateRange.Text = string.Format("({0}&nbsp;-&nbsp;{1})",
                     diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat),
                     diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat)
                     );
            if (ddlPeriod.SelectedValue == "0")
            {
                lblCustomDateRange.Attributes.Add("class", "fontBold");
                imgCalender.Attributes.Add("class", "");
            }
            else
            {
                lblCustomDateRange.Attributes.Add("class", "displayNone");
                imgCalender.Attributes.Add("class", "displayNone");
            }
            hdnStartDate.Value = diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat);
            hdnEndDate.Value = diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat);
            var clFromDate = diRange.FindControl("clFromDate") as AjaxControlToolkit.CalendarExtender;
            var clToDate = diRange.FindControl("clToDate") as AjaxControlToolkit.CalendarExtender;
            hdnStartDateCalExtenderBehaviourId.Value = clFromDate.BehaviorID;
            hdnEndDateCalExtenderBehaviourId.Value = clToDate.BehaviorID;
        }

        private void EnableProjectsDropDown()
        {
            int eventSourceValue = Convert.ToInt32(ddlEventSource.SelectedValue);
            ddlProjects.Enabled = !((eventSourceValue >= 3 && eventSourceValue <= 5) || (eventSourceValue >= 32 && eventSourceValue <= 42) || (eventSourceValue >= 21 && eventSourceValue <= 27));
        }

        private void SaveFilterSettings()
        {
            ActivityLogFilter filter = GetFilterSettings();
            SerializationHelper.SerializeCookie(filter, Constants.FilterKeys.ActivityLogFilterCookie);
        }

        private ActivityLogFilter GetFilterSettings()
        {
            var filter = new ActivityLogFilter
            {
                EventSourceSelected = ddlEventSource.SelectedValue,
                FromDateFilterValue = FromDateFilterValue,
                ToDateFilterValue = ToDateFilterValue,
                PersonSelected = ddlPersonName.SelectedValue.Trim(),
                ProjectSelected = ddlProjects.SelectedValue.Trim(),
                CurrentIndex = gvActivities.PageIndex,
                FiltersChanged = Convert.ToBoolean(hdnResetFilter.Value),
                PeriodSelected = Convert.ToInt32(ddlPeriod.SelectedValue)
            };
            return filter;
        }

        private void FillEventList()
        {
            ddlEventSource.DataBind();

            ddlEventSource.SelectedValue = GetStringByValue(DisplayDropDownValue);
        }

        /// <summary>
        /// If we're on the page with ID parameter, select corresponding entity
        /// </summary>
        private void SelectFilterType()
        {
            var entityId = Page.Request[Constants.QueryStringParameterNames.Id];
            if (!string.IsNullOrEmpty(entityId))
            {
                entityId = entityId.Trim();
                var selectedValue = ddlEventSource.SelectedValue;

                if (selectedValue == GetStringByValue(ActivityEventSource.Project))
                {
                    PrepareDropDown(entityId, ddlProjects);
                }
                else if (selectedValue == GetStringByValue(ActivityEventSource.TargetPerson))
                    PrepareDropDown(entityId, ddlPersonName);
            }
            else
            {
                var selectedValue = ddlEventSource.SelectedValue;
                if (selectedValue == GetStringByValue(ActivityEventSource.Project) && HostingPage.ProjectId.HasValue)
                {
                    entityId = HostingPage.ProjectId.Value.ToString();
                    PrepareDropDown(entityId, ddlProjects);
                }
            }
        }

        private static void PrepareDropDown(string entityId, DropDownList listControl)
        {
            if (listControl.Items.FindByValue(entityId) != null)
            {
                listControl.SelectedValue = entityId;
                listControl.Enabled = false;
            }
        }

        protected void btnUpdateView_Click(object sender, EventArgs e)
        {
            UpdateGrid();

            if (hdnResetFilter.Value == "true")
                btnResetFilter.Enabled = true;
            else
                btnResetFilter.Enabled = false;

        }

        protected void btnResetFilter_Click(object sender, EventArgs e)
        {
            ResetFilters();
            Update();
        }

        private void UpdateGrid()
        {
            gvActivities.PageIndex = 0;
            gvActivities.DataBind();
            btnExcel.Enabled = gvActivities.Rows.Count > 0;
        }

        public void Update()
        {
            UpdateGrid();
            updActivityLog.Update();
        }

        protected void gvActivities_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                InitTransformation(e, "xmlChanges");
                InitTransformation(e, "xmlActivityItem");
            }
            if (e.Row.RowType == DataControlRowType.Pager)
            {
                TableCell tblcell0 = e.Row.Cells[0];
                tblcell0.Attributes.Add("colSpan", "1");

                TableCell tblcell = new TableCell();
                tblcell.Text = "&nbsp;&nbsp; of " + gvActivities.PageCount + " Pages";
                tblcell.Attributes.Add("colSpan", "2");
                e.Row.Cells.AddAt(1, tblcell);
            }
        }

        protected void gvActivities_OnDataBound(object sender, EventArgs e)
        {
            //btnExcel.Enabled = true;
            if (IsActivityLogPage)
            {
                SaveFilterSettings();
            }
        }

        protected void ddlProjects_OnDataBound(object sender, EventArgs e)
        {
            AddEmptyRow(ddlProjects, Resources.Controls.AllProjects);
            SelectFilterType();
        }

        private static void AddEmptyRow(DropDownList dropDown, string text)
        {
            if (dropDown.Items.Count > 0)
                dropDown.Items.Insert(
                    0, new ListItem(text, string.Empty));
        }

        protected void ddlPersonName_OnDataBound(object sender, EventArgs e)
        {
            AddEmptyRow(ddlPersonName, Resources.Controls.AnyUserText);
            SelectFilterType();
        }

        private void InitTransformation(GridViewRowEventArgs e, string xmlchangesControl)
        {
            var entityChanges = e.Row.FindControl(xmlchangesControl) as Xml;

            if (entityChanges != null)
                entityChanges.TransformArgumentList = _argumentList;
        }

        private void SetPeriodSelection(int periodSelected)
        {
            DateTime currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, Constants.Dates.FirstDay);

            if (periodSelected < 0)
            {
                DateTime startMonth = new DateTime();
                DateTime endMonth = new DateTime();

                if (periodSelected > -30)
                {
                    startMonth = DateTime.Now.Date.AddDays(periodSelected + 1);
                    endMonth = DateTime.Now.Date;
                }
                else
                {
                    startMonth = currentMonth.AddMonths((periodSelected / 30) + 1);
                    endMonth = currentMonth;
                }

                diRange.FromDate = startMonth;
                diRange.ToDate = new DateTime(endMonth.Year, endMonth.Month, DateTime.DaysInMonth(endMonth.Year, endMonth.Month));
            }
            else
            {
                mpeCustomDates.Show();
            }
        }

        protected void odsActivities_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (ddlProjects.SelectedValue == "0")
            {
                //ddlProjects.DataBind();
            }
            if (ddlPeriod.SelectedValue == "0")
            {
                e.InputParameters["startDateFilter"] = StartDateFilter = diRange.FromDate.HasValue ? (DateTime)diRange.FromDate : SettingsHelper.GetCurrentPMTime().AddYears(-1);
                e.InputParameters["endDateFilter"] = EndDateFilter = diRange.ToDate.HasValue ? diRange.ToDate.Value.AddDays(1) : SettingsHelper.GetCurrentPMTime().AddDays(1);
            }
            else
            {
                var periodSelected = Convert.ToInt32(ddlPeriod.SelectedValue);

                DateTime currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, Constants.Dates.FirstDay);
                DateTime startMonth = new DateTime();
                DateTime endMonth = new DateTime();

                if (periodSelected > -30)
                {
                    startMonth = DateTime.Now.Date.AddDays(periodSelected + 1);
                    endMonth = DateTime.Now.Date;
                }
                else
                {
                    startMonth = currentMonth.AddMonths((periodSelected / 30) + 1);
                    endMonth = currentMonth;
                }

                e.InputParameters["startDateFilter"] = StartDateFilter = startMonth;
                e.InputParameters["endDateFilter"] = EndDateFilter = new DateTime(endMonth.Year, endMonth.Month, DateTime.DaysInMonth(endMonth.Year, endMonth.Month));
            }
            e.InputParameters["sourceFilter"] = SourceFilter = ddlEventSource.SelectedValue;
            e.InputParameters["opportunityId"] = OpportunityIdFilter = (this.OpportunityId == null ? null : this.OpportunityId.ToString());
            e.InputParameters["milestoneId"] = MilestoneIdFilter = (this.MilestoneId == null ? null : this.MilestoneId.ToString());
            e.InputParameters["personId"] = PersonId = string.IsNullOrEmpty(ddlPersonName.SelectedValue) ?
                                                null : ddlPersonName.SelectedValue;
            e.InputParameters["projectId"] = ProjectId = ddlProjects.SelectedValue == string.Empty ? "0" : ddlProjects.SelectedValue;
            e.InputParameters["vendorId"] = "0";
            var selectedFields = cblFields.SelectedItems;
            e.InputParameters["practiceAreas"] = PracticeAreas = selectedFields == null ? true : selectedFields.Contains("PracticeAreas");
            e.InputParameters["division"] = Division = selectedFields == null ? true : selectedFields.Contains("Division");
            e.InputParameters["channel"] = Channel = selectedFields == null ? true : selectedFields.Contains("Channel");
            e.InputParameters["offering"] = Offering = selectedFields == null ? true : selectedFields.Contains("Offering");
            e.InputParameters["revenueType"] = RevenueType = selectedFields == null ? true : selectedFields.Contains("RevenueType");
            e.InputParameters["sowBudget"] = SowBudget = selectedFields == null ? true : selectedFields.Contains("SOWBudget");
            e.InputParameters["director"] = Director = selectedFields == null ? true : selectedFields.Contains("ClientDirector");
            e.InputParameters["poAmount"] = POAmount = selectedFields == null ? true : selectedFields.Contains("POAmount");
            e.InputParameters["capabilities"] = Capabilites = selectedFields == null ? true : selectedFields.Contains("Capabilities");
            e.InputParameters["newOrExtension"] = NewOrExtension = selectedFields == null ? true : selectedFields.Contains("New/Extension");
            e.InputParameters["poNumber"] = PoNumber = selectedFields == null ? true : selectedFields.Contains("PONumber");
            e.InputParameters["projectStatus"] = ProjectStatus = selectedFields == null ? true : selectedFields.Contains("ProjectStatus");
            e.InputParameters["salesPerson"] = SalesPerson = selectedFields == null ? true : selectedFields.Contains("SalesPerson");
            e.InputParameters["projectOwner"] = ProjectOwner = selectedFields == null ? true : selectedFields.Contains("ProjectOwner");
            e.InputParameters["recordPerChange"] = true;
        }

        public object AddDefaultProjectAndMileStoneInfo(object logDataObject)
        {
            if (logDataObject != null)
            {
                var logDataStr = logDataObject.ToString();
                var XmlDoc = new XmlDocument();
                XmlDoc.LoadXml(logDataStr);
                var Root = XmlDoc.FirstChild;
                var defaultProjectId = MileStoneConfigurationManager.GetProjectId();
                var defaultMileStoneId = MileStoneConfigurationManager.GetMileStoneId();
                if (defaultProjectId.HasValue)
                {
                    var defaultProjectIdElement = XmlDoc.CreateElement("DefaultProjectId");
                    defaultProjectIdElement.InnerText = defaultProjectId.Value.ToString();
                    Root.InsertAfter(defaultProjectIdElement, Root.LastChild);
                }
                if (defaultMileStoneId.HasValue)
                {
                    var defaultMileStoneIdElement = XmlDoc.CreateElement("DefaultMileStoneId");
                    defaultMileStoneIdElement.InnerText = defaultMileStoneId.Value.ToString();
                    Root.InsertAfter(defaultMileStoneIdElement, Root.LastChild);
                }

                return Root.OuterXml;
            }
            else
            {
                return logDataObject;
            }
        }

        public string GetModifiedByDetails(object personId, object personLastFirstName, string SystemUser, object logData)
        {
            if (personId != null)
            {
                return personLastFirstName.ToString() == "N/A, " ? "N/A" : personLastFirstName.ToString();
            }

            string modifiedOrUser = string.Empty;

            if (logData != null)
            {
                var logDataStr = logData.ToString();
                var XmlDoc = new XmlDocument();
                XmlDoc.LoadXml(logDataStr);
                var Root = XmlDoc.FirstChild;
                if (Root.HasChildNodes)
                {
                    var newValues = Root.FirstChild;
                    if (Root.Name.ToLower() == LoginTag)
                    {
                        modifiedOrUser = GetAttribute(newValues, UserNameAttribute);
                    }
                    else if (Root.Name.ToLower() == "becomeuser" || Root.Name.ToLower() == "export")
                    {
                        modifiedOrUser = GetAttribute(newValues, UserAttribute);
                    }
                    else if (Root.Name.ToLower() == "note")
                    {
                        modifiedOrUser = GetAttribute(newValues, "By");
                    }
                    //else if (Root.Name.ToLower() == "error")
                    //{
                    //    modifiedOrUser = GetAttribute(newValues, "Login");
                    //}
                    else if (logData.ToString().Contains(ModifiedByNameAttribute))
                    {
                        modifiedOrUser = GetAttribute(newValues, ModifiedByNameAttribute);
                    }
                    else if (logData.ToString().Contains(UserNameAttribute))
                    {
                        modifiedOrUser = GetAttribute(newValues, UserNameAttribute);
                    }
                }
            }

            return string.IsNullOrEmpty(modifiedOrUser) ? SystemUser : modifiedOrUser;
        }

        private string GetAttribute(XmlNode newValues, string attribute)
        {
            if (newValues.Name.ToLower() == NewValuesTag && newValues.OuterXml.Contains(attribute))
            {
                var modifiedBy = newValues.Attributes.GetNamedItem(attribute);
                if (modifiedBy != null)
                    return modifiedBy.Value;
            }
            return null;
        }

        public string NoNeedToShowActivityType(object activityName)
        {
            return activityName.ToString().ToLower() == LoggedInActivity ? string.Empty : activityName.ToString();
        }
    }
}

