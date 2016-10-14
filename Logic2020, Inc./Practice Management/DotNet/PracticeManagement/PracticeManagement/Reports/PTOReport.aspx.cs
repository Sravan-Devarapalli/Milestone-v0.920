using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using DataTransferObjects;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using DataTransferObjects.Reports;
using PraticeManagement.Utils;
using PraticeManagement.Configuration.ConsReportColoring;
using System.Web.Security;
using PraticeManagement.Objects;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing.Imaging;
using PraticeManagement.Utils.Excel;
using System.Data;
using DataTransferObjects.Filters;
using System.Text;

namespace PraticeManagement.Reports
{
    public partial class PTOReport : System.Web.UI.Page
    {
        #region Constants

        private const int DEFAULT_STEP = 7;
        private const int DAYS_FORWARD = 184;
        private const string W2Hourly = "W2-Hourly";
        private const string W2Salary = "W2-Salary";
        private const string WEEKS_SERIES_NAME = "Weeks";
        private const string MAIN_CHART_AREA_NAME = "MainArea";
        private const string FULL_MONTH_NAME_FORMAT = "MMMM, yyyy";
        private const string TITLE_FORMAT_PDF = "PTO Report \n{0} to {1}\nFor {2} Persons; with pay type: {3} \n{4}\n{5}\n{6}.";
        private const string TITLE_FORMAT = "PTO Report <br />{0} to {1}<br />For {2} Persons; with pay type: {3} <br />{4}<br />{5}<br />{6}.";
        private const string CONSULTANTPTO_KEY = "ConsultantPTO_Key";
        private const string NAME_FORMAT = "{0}, {1} ({2})";
        private const string PERSON_TOOLTIP_FORMAT = "{0}, Hired {1}";
        private const string NOT_HIRED_PERSON_TOOLTIP_FORMAT = "{0}";
        private const string VACATION_TOOLTIP_FORMAT = "PTO : {0}";
        private const string LOA_TOOLTIP_FORMAT = "Other Time Off : {0}";
        private const string PageCount = "Page {0} of {1}";
        private const int reportSize = 25;
        private int headerRowsCount = 1;
        private int coloumnsCount = 1;

        #endregion Constants

        #region Poperties

        private int _personsCount;
        private bool? _userIsAdministratorValue;
        private int page;

        protected bool UserIsAdministrator
        {
            get
            {
                if (!_userIsAdministratorValue.HasValue)
                {
                    _userIsAdministratorValue =
                        Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
                }

                return _userIsAdministratorValue.Value;
            }
        }

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
                dataCellStyle.WrapText = true;
                dataCellStyle.BorderStyle = NPOI.SS.UserModel.BorderStyle.None;
                dataCellStyle.FontHeight = 200;

                CellStyles[] dataCellStylearray = { dataCellStyle };
                RowStyles datarowStyle1 = new RowStyles(dataCellStylearray);
                RowStyles datarowStyle2 = new RowStyles(dataCellStylearray);
                datarowStyle1.Height = 350;
                datarowStyle2.Height = 1250;

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle2, datarowStyle1 };

                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.MergeRegion.Add(new int[] { 0, 0, 0, coloumnsCount - 1 });
                sheetStyle.MergeRegion.Add(new int[] { 1, 1, 0, coloumnsCount - 1 });
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

                CellStyles dateCellStyle = new CellStyles();
                dateCellStyle.IsBold = true;
                dateCellStyle.WrapText = true;
                dateCellStyle.BorderStyle = NPOI.SS.UserModel.BorderStyle.None;

                dateCellStyle.DataFormat = Granularity == 30 ? "[$-409]mmm-yy;@" : "[$-409]d-mmm;@";

                List<CellStyles> headerCellStyleList = new List<CellStyles>() { headerCellStyle, headerCellStyle, headerCellStyle, dateCellStyle };
                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataStyle = new CellStyles();

                CellStyles[] dataCellStylearray = { dataStyle };
                List<int> coloumnWidth = new List<int>();
                for (int i = 0; i < 3; i++)
                    coloumnWidth.Add(0);
                RowStyles datarowStyle = new RowStyles(dataCellStylearray.ToArray());
                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };

                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.TopRowNo = headerRowsCount;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanColSplit = 0;
                sheetStyle.FreezePanRowSplit = headerRowsCount;
                sheetStyle.ColoumnWidths = coloumnWidth;
                sheetStyle.IsAutoResize = true;
                return sheetStyle;
            }
        }

        public bool ActivePersons
        {
            get
            {
                return chbActivePersons.Checked;
            }
            set
            {
                chbActivePersons.Checked = value;
            }
        }

        public bool ProjectedPersons
        {
            get
            {
                return chbProjectedPersons.Checked;
            }
            set
            {
                chbProjectedPersons.Checked = value;
            }
        }

        public bool PayTypeIsW2Salary
        {
            get
            {
                return chbW2salary.Checked;
            }
            set
            {
                chbW2salary.Checked = value;
            }
        }

        public bool PayTypeIsW2Hourly
        {
            get
            {
                return chbW2Hourly.Checked;
            }
            set
            {
                chbW2Hourly.Checked = value;
            }
        }

        public string PracticesSelected
        {
            get
            {
                return cblPractices.SelectedItems;
            }
            set
            {
                cblPractices.SelectedItems = value;
            }
        }

        public string DivisionsSelected
        {
            get
            {
                return cblDivisions.SelectedItems;
            }
            set
            {
                cblDivisions.SelectedItems = value;
            }
        }

        public string TitlesSelected
        {
            get
            {
                return cblTitles.SelectedItems;
            }
            set
            {
                cblTitles.SelectedItems = value;
            }
        }

        public int SortId
        {
            get
            {
                return int.Parse(ddlSortBy.SelectedValue);
            }
            set
            {
                ddlSortBy.SelectedValue = value.ToString();
            }
        }

        public string SortDirection
        {
            get
            {
                return rbSortbyAsc.Checked ? "ASC" : "DESC";
            }
            set
            {
                if (value == "ASC")
                {
                    rbSortbyAsc.Checked = true;
                    rbSortbyDesc.Checked = false;
                }
                else
                {
                    rbSortbyDesc.Checked = true;
                    rbSortbyAsc.Checked = false;
                }

            }
        }

        public int Granularity
        {
            get
            {
                return int.Parse(ddlDetalization.SelectedValue);
            }
            set
            {
                ddlDetalization.SelectedValue = value.ToString();
            }
        }

        public string Period
        {
            get
            {
                return ddlPeriod.SelectedValue;
            }
            set
            {
                ddlPeriod.SelectedValue = value;
            }
        }

        public DateTime BegPeriod
        {
            get
            {
                var selectedVal = int.Parse(Period);
                if (selectedVal == 0)
                {
                    return diRange.FromDate.Value;
                }
                else
                {
                    var now = Utils.Generic.GetNowWithTimeZone();

                    if (selectedVal == 7) // next week , 
                    {

                        return now.Date;

                    }
                    else if (selectedVal == 3)//next 3 month
                    {
                        return new DateTime(now.Year, now.Month, 1).Date;
                    }
                    else if (selectedVal == 1 || selectedVal == 2)//curent month, Previous month
                    {
                        var startDate = now.Date.AddMonths(1 - selectedVal).Date;

                        return new DateTime(startDate.Year, startDate.Month, 1).Date;

                    }
                    else
                    {
                        var startDate = new DateTime(now.Date.AddYears(12 - selectedVal).Date.Year, 1, 1);

                        return startDate;

                    }
                }
            }
            set
            {
                var selectedVal = int.Parse(Period);
                if (selectedVal == 0)
                {
                    diRange.FromDate = value;
                }
            }
        }

        public DateTime EndPeriod
        {
            get
            {
                var selectedVal = int.Parse(Period);
                if (selectedVal == 0)
                {
                    return diRange.ToDate.Value;


                }
                else
                {
                    var now = Utils.Generic.GetNowWithTimeZone();
                    if (selectedVal == 7) // next week
                    {

                        return now.Date.AddDays(selectedVal - 1).Date;

                    }
                    else if (selectedVal == 3)//next 3 months
                    {
                        var endDate = now.Date.AddMonths(3);

                        return new DateTime(endDate.Year, endDate.Month, 1).AddDays(-1);

                    }
                    else if (selectedVal == 1 || selectedVal == 2)//curent month, Previous month
                    {
                        var endDate = now.Date.AddMonths(2 - selectedVal);

                        return new DateTime(endDate.Year, endDate.Month, 1).AddDays(-1);

                    }
                    else
                    {
                        var endDate = new DateTime(now.Date.AddYears(12 - selectedVal).Date.Year, 12, 31);

                        return endDate.Date;
                    }
                }
            }
            set
            {
                var selectedVal = int.Parse(Period);
                if (selectedVal == 0)
                {
                    diRange.ToDate = value;
                }
            }
        }

        public bool IsshowTodayBar
        {
            get
            {
                return Period == "0";
            }
        }

        public string DetalizationSelectedValue
        {
            get
            {
                return ddlDetalization.SelectedValue;
            }
        }

        public List<ConsultantPTOHours> ConsultantPTOPerson
        {
            get
            {
                if (ViewState[CONSULTANTPTO_KEY] == null)
                    ViewState[CONSULTANTPTO_KEY] = new List<ConsultantPTOHours>();
                return ViewState[CONSULTANTPTO_KEY] as List<ConsultantPTOHours>;
            }
            set { ViewState[CONSULTANTPTO_KEY] = value; }
        }

        public Series WeeksSeries
        {
            get { return chart.Series[WEEKS_SERIES_NAME]; }
        }

        public Series WeeksSeriesPdf
        {
            get { return chartPdf.Series[WEEKS_SERIES_NAME]; }
        }

        public int PartsCount
        {
            get
            {
                if (Granularity == 1)
                {
                    return Math.Abs((int)(BegPeriod - EndPeriod).TotalDays) + 1;
                }
                if (Granularity == 7)
                {
                    int count = Math.Abs((int)(BegPeriod - EndPeriod).TotalDays) / 7 + 1;
                    if (EndPeriod.DayOfWeek == 0)
                    {
                        count++;
                    }
                    return count;
                }
                else
                {
                    return Math.Abs((BegPeriod.Month - EndPeriod.Month) + 12 * (BegPeriod.Year - EndPeriod.Year)) + 1;
                }

            }
            set { }
        }

        private bool IsFromBrowserSession
        {
            get;
            set;
        }

        #endregion Poperties

        #region PageEvents

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateControls();
            }
            AddAttributesToCheckBoxes(cblPractices);
            AddAttributesToCheckBoxes(cblDivisions);
            AddAttributesToCheckBoxes(cblTitles);

            if (hdnFiltersChanged.Value == "false")
            {
                btnResetFilter.Attributes.Add("disabled", "true");
            }
            else
            {
                btnResetFilter.Attributes.Remove("disabled");
            }
            if (!IsPostBack)
            {
                GetFilterValuesForSession();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (Period == "0")
            {
                lblCustomDateRange.Attributes.Add("class", "fontBold");
                imgCalender.Attributes.Add("class", "");
            }
            else
            {
                lblCustomDateRange.Attributes.Add("class", "displayNone");
                imgCalender.Attributes.Add("class", "displayNone");
                diRange.FromDate = BegPeriod;
                diRange.ToDate = EndPeriod;
            }

            lblCustomDateRange.Text = string.Format("({0}&nbsp;-&nbsp;{1})",
                    diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat),
                    diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat)
                    );

            hdnStartDate.Value = diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat);
            hdnEndDate.Value = diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat);
            var tbFrom = diRange.FindControl("tbFrom") as TextBox;
            var tbTo = diRange.FindControl("tbTo") as TextBox;
            var clFromDate = diRange.FindControl("clFromDate") as AjaxControlToolkit.CalendarExtender;
            var clToDate = diRange.FindControl("clToDate") as AjaxControlToolkit.CalendarExtender;
            hdnStartDateTxtBoxId.Value = tbFrom.ClientID;
            hdnEndDateTxtBoxId.Value = tbTo.ClientID;
            hdnStartDateCalExtenderBehaviourId.Value = clFromDate.BehaviorID;
            hdnEndDateCalExtenderBehaviourId.Value = clToDate.BehaviorID;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "", "SaveItemsToArray();EnableOrDisableItemsOfDetalization();", true);

            if (chart.Height.Value > 2500)
            {
                chart.ChartAreas[MAIN_CHART_AREA_NAME].InnerPlotPosition.X = 30;
                chart.ChartAreas[MAIN_CHART_AREA_NAME].InnerPlotPosition.Width = 55;

                chart.ChartAreas[MAIN_CHART_AREA_NAME].InnerPlotPosition.Y = 1f;
                chart.ChartAreas[MAIN_CHART_AREA_NAME].InnerPlotPosition.Height = 98f;

                if (chart.Legends != null && chart.Legends.Count > 0)
                {
                    chart.Legends.FirstOrDefault().Position = new ElementPosition { X = 0, Y = 0, Width = 100, Height = 1 };
                    chart.Legends.Last().Position = new ElementPosition { X = 0, Y = 99, Width = 100, Height = 1 };
                }
            }
        }

        #endregion PageEvents

        public string PracticesFilterText()
        {
            string PracticesFilterText = "Not Including All Practice Areas";
            if (cblPractices.Items.Count > 0)
            {
                if (cblPractices.Items[0].Selected)
                {
                    PracticesFilterText = "Including All Practice Areas";
                }
                else
                {
                    for (int index = 1; index < cblPractices.Items.Count; index++)
                    {
                        if (!cblPractices.Items[index].Selected)
                        {
                            return PracticesFilterText;
                        }
                    }
                    PracticesFilterText = "Including All Practice Areas";
                }
            }
            return PracticesFilterText;
        }

        public string DivisionsFilterText()
        {
            string includeText = "Not Including All Divisions";
            if (cblDivisions.Items.Count > 0)
            {
                if (cblDivisions.Items[0].Selected)
                {
                    return includeText = "Including All Divisions";
                }
                else
                {
                    includeText = "Including " + cblDivisions.SelectedItemsText.Remove(cblDivisions.SelectedItemsText.Length - 1) + " Division(s)";
                }
            }
            return includeText;
        }

        public string TitlesFilterText()
        {
            string includeText = "Not Including All Titles";
            if (cblTitles.Items.Count > 0)
            {
                if (cblTitles.Items[0].Selected)
                {
                    return includeText = "Including All Titles";
                }
                else
                {
                    for (int index = 1; index < cblTitles.Items.Count; index++)
                    {
                        if (!cblTitles.Items[index].Selected)
                        {
                            return includeText;
                        }
                    }
                    includeText = "Including All Titles";
                }
            }
            return includeText;
        }

        protected void btnResetFilter_OnClick(object sender, EventArgs e)
        {
            Reset();
            UpdateReport();
            updConsReport.Update();
            SaveFilterValuesForSession();
        }

        private void Reset()
        {
            Period = "1";
            chbActivePersons.Checked = true;
            chbProjectedPersons.Checked = false;
            chbW2Hourly.Checked = chbW2salary.Checked = true;
            cblPractices.SelectAll();
            cblDivisions.SelectAll();
            cblTitles.SelectAll();
            Granularity = 7;
            this.SortDirection = "ASC";
            SortId = 1;
            if (!IsPostBack && IsFromBrowserSession)
            {
                chart.CssClass = "hide";
            }
            hdnFiltersChanged.Value = "false";
            btnResetFilter.Attributes.Add("disabled", "true");
        }

        #region User defined Methods

        private void AddAttributesToCheckBoxes(ScrollingDropDown ddlpractices)
        {
            foreach (System.Web.UI.WebControls.ListItem item in ddlpractices.Items)
            {
                item.Attributes.Add("onclick", "EnableResetButton();");
            }
        }

        private static string FormatPersonName(Person p)
        {
            return string.Format(
                NAME_FORMAT,
                p.LastName,
                p.FirstName,
                p.Title.TitleName
                );
        }

        public void PopulateControls()
        {
            DataHelper.FillPracticeListOnlyActive(cblPractices, Resources.Controls.AllPracticesText);
            cblPractices.SelectAll();
            DataHelper.FillPersonDivisionList(cblDivisions, "All Divisions");
            cblDivisions.SelectAll();
            DataHelper.FillTitleList(cblTitles, "All Titles");
            cblTitles.SelectAll();
            chbW2Hourly.Checked = true;
            chbW2salary.Checked = true;
            var resoureDictionary = DataHelper.GetResourceKeyValuePairs(SettingsType.Reports);

            if (resoureDictionary != null && resoureDictionary.Keys.Count > 0)
            {
                diRange.FromDate = Convert.ToDateTime(resoureDictionary[Constants.ResourceKeys.StartDateKey]);

                diRange.ToDate = Convert.ToDateTime(resoureDictionary[Constants.ResourceKeys.EndDateKey]);
                Period = "1";
                ddlDetalization.SelectedValue = ddlDetalization.Items.FindByValue(resoureDictionary[Constants.ResourceKeys.GranularityKey]).Value;
                ddlSortBy.SelectedValue = ddlSortBy.Items.FindByValue(resoureDictionary[Constants.ResourceKeys.SortIdKey]).Value;

                chbActivePersons.Checked = Convert.ToBoolean(resoureDictionary[Constants.ResourceKeys.ActivePersonsKey]);
                chbProjectedPersons.Checked = Convert.ToBoolean(resoureDictionary[Constants.ResourceKeys.ProjectedPersonsKey]);

                rbSortbyAsc.Checked = resoureDictionary[Constants.ResourceKeys.SortDirectionKey] == "Desc" ? true : false;
            }
        }

        protected void btnUpdateView_OnClick(object sender, EventArgs e)
        {
            UpdateReport();
            updConsReport.Update();
            SaveFilterValuesForSession();
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {

            var filename = string.Format("PTO Report_{0}-{1}.xls", BegPeriod.ToString("MM_dd_yyyy"), EndPeriod.ToString("MM_dd_yyyy"));
            DataHelper.InsertExportActivityLogMessage("PTO Report");
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();

            var report =
            ServiceCallers.Custom.Person(client => client.GetConsultantPTOEntries(BegPeriod, EndPeriod, ActivePersons, ProjectedPersons, PayTypeIsW2Salary, PayTypeIsW2Hourly, PracticesSelected, DivisionsSelected, TitlesSelected, SortId, SortDirection)).ToList(); ;

            SortPersons(report);

            string personsPlaceHolder = string.Empty;
            string payTypePlaceHolder = string.Empty;
            if (ProjectedPersons && ActivePersons)
            {
                personsPlaceHolder = "All";
            }
            else if (ActivePersons)
            {
                personsPlaceHolder = "Active";
            }
            else if (ProjectedPersons)
            {
                personsPlaceHolder = "Projected";
            }
            else
            {
                personsPlaceHolder = "No";
            }

            if (PayTypeIsW2Hourly && PayTypeIsW2Salary)
            {
                payTypePlaceHolder = "W2-Salary and W2-Hourly";
            }
            else if (PayTypeIsW2Salary)
            {
                payTypePlaceHolder = "W2-Salary";
            }
            else if (PayTypeIsW2Hourly)
            {
                payTypePlaceHolder = "W2-Hourly";
            }
            else
            {
                payTypePlaceHolder = "No";
            }

            if (report.Count > 0)
            {
                DataTable header1 = new DataTable();
                header1.Columns.Add(string.Format("Period: {0}-{1}", Period == "0" ? diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat) : BegPeriod.ToString(Constants.Formatting.EntryDateFormat),
                        Period == "0" ? diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat) : EndPeriod.ToString(Constants.Formatting.EntryDateFormat)));

                List<object> row1 = new List<object>();
                row1.Add(string.Format(TITLE_FORMAT_PDF,
                        Period == "0" ? diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat) : BegPeriod.ToString(Constants.Formatting.EntryDateFormat),
                        Period == "0" ? diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat) : EndPeriod.ToString(Constants.Formatting.EntryDateFormat),
                        personsPlaceHolder, payTypePlaceHolder, PracticesFilterText(), DivisionsFilterText(), TitlesFilterText()));
                header1.Rows.Add(row1.ToArray());
                headerRowsCount = header1.Rows.Count + 3;

                DataTable data;

                data = PrepareDataTable(report);
                coloumnsCount = data.Columns.Count;
                var dataset = new DataSet();
                dataset.DataSetName = "PTO_Report";
                dataset.Tables.Add(header1);
                dataset.Tables.Add(data);
                dataSetList.Add(dataset);


                sheetStylesList.Add(HeaderSheetStyle);
                sheetStylesList.Add(DataSheetStyle);
            }
            else
            {
                string dateRangeTitle = "There are no people with the selected parameters.";
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                sheetStylesList.Add(HeaderSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "Consulting_Utilization";
                dataset.Tables.Add(header);
                dataSetList.Add(dataset);
            }


            NPOIExcel.Export(filename, dataSetList, sheetStylesList);
        }

        public DataTable PrepareDataTable(List<ConsultantPTOHours> report)
        {
            DataTable data = new DataTable();
            List<object> row;
            data.Columns.Add("Person Name");
            data.Columns.Add("Title");
            data.Columns.Add("Pay type");

            int count = PartsCount;
            if (Granularity == 7)
            {
                var temp = BegPeriod.AddDays(-(int)BegPeriod.DayOfWeek);
                var temp2 = EndPeriod.AddDays(6 - (int)EndPeriod.DayOfWeek);
                count = Math.Abs((int)(temp - temp2).TotalDays) / 7 + 1;
            }

            for (int i = 0; i < count; i++)
            {
                var beginPeriod = BegPeriod;
                var endPeriod = EndPeriod;

                var period = (endPeriod.Subtract(beginPeriod).Days + 1);
                var pointStartDate = beginPeriod.AddDays(i * Granularity);
                if (Granularity == 7)
                {

                    if (i == 0)
                    {
                        pointStartDate = beginPeriod;
                    }
                    else
                    {
                        pointStartDate = beginPeriod.AddDays((7 * i) - (int)beginPeriod.DayOfWeek);
                    }
                }
                else if (Granularity == 30)
                {
                    pointStartDate = beginPeriod.AddMonths(i);
                }

                data.Columns.Add(pointStartDate.ToShortDateString());
            }

            data.Columns.Add("Scheduled PTO Hours");

            foreach (var person in report)
            {
                row = new List<object>();
                row.Add(person.Person.PersonLastFirstName);
                row.Add(person.Person.Title.TitleName);
                row.Add(person.Person.CurrentPay.TimescaleName);
                for (int i = 0; i < count; i++)
                {
                    double weeklyPTOHours = 0;
                    var beginPeriod = BegPeriod;
                    var endPeriod = EndPeriod;


                    var period = (endPeriod.Subtract(beginPeriod).Days + 1);
                    var pointStartDate = beginPeriod.AddDays(i * Granularity);
                    DateTime pointEndDate = beginPeriod.AddDays(((i + 1) * Granularity));
                    if (Granularity == 7)
                    {
                        if (i == 0)
                        {
                            pointStartDate = beginPeriod;
                            pointEndDate = beginPeriod.AddDays(7 - (int)beginPeriod.DayOfWeek);
                        }
                        else
                        {
                            pointStartDate = beginPeriod.AddDays((7 * i) - (int)beginPeriod.DayOfWeek);
                            pointEndDate = pointStartDate.AddDays(7);
                        }
                    }
                    else if (Granularity == 30)
                    {
                        var start = beginPeriod.AddMonths(i);
                        var end = beginPeriod.AddMonths(i + 1);
                        pointStartDate = new DateTime(start.Year, start.Month, 1);
                        pointEndDate = new DateTime(end.Year, end.Month, 1);
                    }
                    else
                    {
                        var delta = period - (i * Granularity - 1);
                        if (delta <= Granularity)
                        {
                            pointEndDate = beginPeriod.AddDays(period);
                        }
                    }
                    for (DateTime d = pointStartDate; d < pointEndDate; d = d.AddDays(1))
                    {
                        weeklyPTOHours += person.PTOOffDates.Keys.Any(t => t == d) ? person.PTOOffDates[d] : 0;
                    }

                    row.Add(weeklyPTOHours);
                }

                row.Add(person.TotalPTOHours);
                data.Rows.Add(row.ToArray());

            }
            return data;
        }

        protected void btnPDFExport_Click(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage("PTO Report - PDF");
            PDFExport();
        }

        public void PDFExport()
        {
            HtmlToPdfBuilder builder = new HtmlToPdfBuilder(iTextSharp.text.PageSize.A4);
            string filename = "PTO Report.pdf";
            byte[] pdfDataInBytes = this.RenderPdf(builder);

            HttpContext.Current.Response.ContentType = "Application/pdf";
            HttpContext.Current.Response.AddHeader(
                "content-disposition", string.Format("attachment; filename={0}", Utils.Generic.EncodedFileName(filename)));

            int len = pdfDataInBytes.Length;
            int bytes;
            byte[] buffer = new byte[1024];
            Stream outStream = HttpContext.Current.Response.OutputStream;
            using (MemoryStream stream = new MemoryStream(pdfDataInBytes))
            {
                while (len > 0 && (bytes = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outStream.Write(buffer, 0, bytes);
                    HttpContext.Current.Response.Flush();
                    len -= bytes;
                }
            }
        }

        private byte[] RenderPdf(HtmlToPdfBuilder builder)
        {
            MemoryStream file = new MemoryStream();
            Document document = new Document(builder.PageSize);
            document.SetPageSize(iTextSharp.text.PageSize.A4_LANDSCAPE.Rotate());
            PdfWriter writer = PdfWriter.GetInstance(document, file);
            document.Open();
            float topMargin = 0f;

            ConsultantPTOPerson.Reverse();
            int count = ConsultantPTOPerson.Count;
            for (int i = 0; i < Math.Ceiling((double)count / reportSize); i++)
            {
                ChartForPdf(i);
                document.SetMargins(document.LeftMargin, document.RightMargin, topMargin, topMargin);
                document.NewPage();
                document.Add(ConsultingImage(i));
            }

            document.Close();
            return file.ToArray();
        }

        public void ChartForPdf(int i)
        {
            page = 0;
            chartPdf.Titles.Clear();
            foreach (var series in chartPdf.Series)
            {
                series.Points.Clear();
            }
            chartPdf.ChartAreas[MAIN_CHART_AREA_NAME].AxisX.CustomLabels.Clear();
            chartPdf.ChartAreas[MAIN_CHART_AREA_NAME].AxisX2.CustomLabels.Clear();
            InitChart(true, i);
            List<ConsultantPTOHours> report = ConsultantPTOPerson;

            report = report.Skip(i * reportSize).Take(reportSize).ToList();
            report.Reverse();
            foreach (var quadruple in report)
            {
                AddPerson(quadruple, true);
                page = 10;
            }

            chartPdf.Height = Resources.Controls.TimelineGeneralHeaderHeigth +

                (report.Count == reportSize ? Resources.Controls.TimelineGeneralItemHeigth * report.Count : (report.Count <= 14) ? 70 + report.Count * 30 : 80 + report.Count * 22) +
                              Resources.Controls.TimelineGeneralFooterHeigth;
        }

        public PdfPTable ConsultingImage(int i)
        {
            PdfPTable headerTable = new PdfPTable(1);
            MemoryStream chartImage = new MemoryStream();
            chartPdf.SaveImage(chartImage, ChartImageFormat.Png);

            System.Drawing.Image img = System.Drawing.Image.FromStream(chartImage);
            Bitmap objBitmap = new Bitmap(img, new Size(img.Width, img.Height));
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance((System.Drawing.Image)objBitmap, ImageFormat.Png);
            PdfPCell logo = new PdfPCell(image, true);

            logo.Border = PdfPCell.NO_BORDER;
            headerTable.AddCell(logo);
            return headerTable;
        }

        private void SortPersons(List<ConsultantPTOHours> report)
        {
            if (SortId == 2)
            {
                report.Sort((a, b) => a.PTOOffDates.FirstOrDefault().Key.CompareTo(b.PTOOffDates.FirstOrDefault().Key));
                if (SortDirection == "ASC")
                {
                    report.Reverse();
                }
            }
        }

        public void UpdateReport()
        {
            var report =
             ServiceCallers.Custom.Person(client => client.GetConsultantPTOEntries(BegPeriod, EndPeriod, ActivePersons, ProjectedPersons, PayTypeIsW2Salary, PayTypeIsW2Hourly, PracticesSelected, DivisionsSelected, TitlesSelected, SortId, SortDirection));
            if (report != null && report.ToList().Count > 0)
            {
                InitChart();
                ConsultantPTOPerson = report.ToList();

                ConsultantPTOPerson.Reverse();

                SortPersons(ConsultantPTOPerson);
                foreach (var quadruple in ConsultantPTOPerson)
                {
                    AddPerson(quadruple);
                }
                if (ConsultantPTOPerson.Count != 1)
                {
                    chart.Height = Resources.Controls.TimelineGeneralHeaderHeigth +
                               Resources.Controls.TimelineGeneralItemHeigth * ConsultantPTOPerson.Count +
                               Resources.Controls.TimelineGeneralFooterHeigth;
                }
                else
                {
                    chart.Height = 200;
                }

                chart.Visible = true;
                nonInv.Visible = false;
                tblExport.Visible = true;
                titleDiv.Visible = true;
            }
            else
            {
                chart.Visible = false;
                nonInv.Visible = true;
                tblExport.Visible = false;
                titleDiv.Visible = false;
            }
        }

        public void AddPerson(ConsultantPTOHours quadruple, bool isPdf = false)
        {
            var partsCount = PartsCount;

            for (var w = 0; w < partsCount; w++)
            {
                TimescaleType payType = quadruple.Person.CurrentPay.Timescale;
                //  Add another range to the person's timeline
                AddPersonRange(
                    quadruple.Person, //  Person
                     w, //  Range index
                     payType == TimescaleType.Undefined ? "No Pay Type" : DataHelper.GetDescription(payType), quadruple.PTOOffDates, quadruple.CompanyHolidayDates, quadruple.LeaveOfAbsence, isPdf
                     );
            }

            //  Add axis label
            AddLabel(quadruple.Person, quadruple.TotalPTOHours, isPdf);

            //  Increase persons counter
            _personsCount++;

        }

        private void AddPersonRange(Person p, int w, string payType, SortedList<DateTime, double> timeoffDates, Dictionary<DateTime, string> companyHolidayDates, Dictionary<DateTime, double> LOA, bool isPdf)
        {
            if (LOA == null)
            {
                LOA = new Dictionary<DateTime, double>();
            }
            if (companyHolidayDates == null)
            {
                companyHolidayDates = new Dictionary<DateTime, string>();
            }
            var beginPeriod = BegPeriod;
            var endPeriod = EndPeriod;

            var period = (endPeriod.Subtract(beginPeriod).Days + 1);
            var pointStartDate = beginPeriod.AddDays(w * Granularity);
            var pointEndDate = beginPeriod.AddDays(((w + 1) * Granularity));


            if (Granularity == 30)
            {
                var numberOfMonths = (endPeriod.Year - beginPeriod.Year) * 12 + endPeriod.Month - beginPeriod.Month;
                var tempDate = beginPeriod.AddMonths(w);
                pointStartDate = w == 0 ? beginPeriod.AddMonths(w) : new DateTime(tempDate.Year, tempDate.Month, 1);
                pointEndDate = w == numberOfMonths ? endPeriod.AddDays(1) : new DateTime(tempDate.Year, tempDate.Month, 1).AddMonths(1);
            }
            else
            {
                var delta = period - (w * Granularity - 1);
                if (delta <= Granularity)
                {
                    pointEndDate = beginPeriod.AddDays(period);
                }
            }
            var range = AddRange(pointStartDate, pointEndDate, _personsCount, isPdf);
            List<DataPoint> innerRangeList = new List<DataPoint>();
            bool isHiredIntheEmployeementRange = p.EmploymentHistory.Any(ph => ph.HireDate < pointEndDate && (!ph.TerminationDate.HasValue || ph.TerminationDate.Value >= pointStartDate));

            range.Color = GetColorForDay(0, isHiredIntheEmployeementRange);

            if (!isHiredIntheEmployeementRange)
            {
                DateTime? oldTerminationdate = p.EmploymentHistory.Any(ph => ph.TerminationDate.HasValue && ph.TerminationDate.Value < pointStartDate) ? p.EmploymentHistory.Last(ph => ph.TerminationDate.HasValue && ph.TerminationDate.Value < pointStartDate).TerminationDate : (DateTime?)null;
                DateTime? newHireDate = p.EmploymentHistory.Any(ph => ph.HireDate >= pointEndDate) ? p.EmploymentHistory.First(ph => ph.HireDate >= pointEndDate).HireDate : (DateTime?)null;

                string tooltip = "";

                if (oldTerminationdate.HasValue && newHireDate.HasValue)
                {
                    tooltip = string.Format("Terminated: {0}{1} ReHired: {2}", oldTerminationdate.Value.ToString("MM/dd/yyyy"), Environment.NewLine, newHireDate.Value.ToString("MM/dd/yyyy"));
                }
                else if (newHireDate.HasValue)
                {
                    tooltip = string.Format("Hired: {0}", newHireDate.Value.ToString("MM/dd/yyyy"));
                }
                else if (oldTerminationdate.HasValue)
                {
                    tooltip = string.Format("Terminated: {0}", oldTerminationdate.Value.ToString("MM/dd/yyyy"));
                }
                range.ToolTip = tooltip;
            }
            else
            {
                range.ToolTip = "";
                range.Color = Color.White;

                List<Quadruple<DateTime, DateTime, DayType, string>> weekDatesRange = new List<Quadruple<DateTime, DateTime, DayType, string>>();//third parameter in the list int will have 3 possible values '0' for utilization '1' for timeoffs '2' for companyholiday

                for (var d = pointStartDate; d < pointEndDate; d = d.AddDays(1))
                {
                    int dayType = timeoffDates.Any(t => t.Key == d) ? 1 : companyHolidayDates.Any(t => t.Key == d) ? 2 : LOA.Any(t => t.Key == d) ? 3 : 0;
                    string holidayDescription = companyHolidayDates.Keys.Any(t => t == d) ? companyHolidayDates[d] : timeoffDates.Keys.Any(t => t == d) ? timeoffDates[d].ToString() : LOA.Any(t => t.Key == d) ? LOA[d].ToString() : "8";

                    if (weekDatesRange.Any(tri => tri.Second == d.AddDays(-1) && dayType == tri.Third.Type && tri.Fourth == holidayDescription))
                    {
                        var tripleRange = weekDatesRange.First(tri => tri.Second == d.AddDays(-1) && dayType == tri.Third.Type);
                        tripleRange.Second = d;
                    }
                    else
                    {
                        weekDatesRange.Add(new Quadruple<DateTime, DateTime, DayType, string>(d, d, new DayType() { Type = dayType }, holidayDescription));
                    }
                }

                foreach (var tripleR in weekDatesRange)
                {
                    var innerRange = AddRange(tripleR.First, tripleR.Second.AddDays(1), _personsCount, isPdf);
                    innerRange.Color = GetColorForDay(tripleR.Third.Type, isHiredIntheEmployeementRange);
                    innerRange.ToolTip = FormatRangeTooltip(tripleR.First, tripleR.Second, tripleR.Third, payType, tripleR.Fourth);
                    innerRange.BackSecondaryColor = Color.Black;
                    innerRangeList.Add(innerRange);
                }
            }
        }


        private static string FormatRangeTooltip(DateTime pointStartDate, DateTime pointEndDate, DayType dayType, string payType = null, string holidayDescription = null)
        {
            //dayType = '1' for PTO 
            //          '2' for company holiday 
            //          '3' for Leave of Absence(all other time off's)
            string tooltip = "";

            if (pointStartDate == pointEndDate)
            {
                tooltip = (dayType.Type == 1 || dayType.Type == 3) ?
                    string.Format(dayType.Type == 1 ? VACATION_TOOLTIP_FORMAT : LOA_TOOLTIP_FORMAT, pointStartDate.ToString("MMM. d")) + (holidayDescription != "8" ? "- " + holidayDescription + " hours" : string.Empty) :
                    dayType.Type == 2 ? holidayDescription + ":" + pointStartDate.ToString("MMM. d") : "";
            }
            else
            {
                tooltip = (dayType.Type == 1 || dayType.Type == 3) ?
                          string.Format(dayType.Type == 1 ? VACATION_TOOLTIP_FORMAT : LOA_TOOLTIP_FORMAT, pointStartDate.ToString("MMM. d") + " - " +
                          pointEndDate.ToString("MMM. d")) + (holidayDescription != "8" ? "- " + holidayDescription + " hours" : string.Empty) : dayType.Type == 2 ? holidayDescription + ":" + (pointStartDate.ToString("MMM. d") + " - " +
                          pointEndDate.ToString("MMM. d")) : "";
            }

            return tooltip;
        }

        private Color GetColorForDay(int dayType, bool isHiredIntheEmployeementRange)
        {
            ConsReportColoringElementSection coloring =
                    ConsReportColoringElementSection.ColorSettings;

            if (!isHiredIntheEmployeementRange)
            {
                return coloring.HiredColor;
            }

            //  If that's vacation, return vacation color
            if (dayType == 1)
            {
                return coloring.VacationColor;
            }
            else if (dayType == 2)
            {
                return coloring.CompanyHolidayColor;
            }
            else if (dayType == 3)
            {
                return Color.Navy;
            }
            else
            {
                return Color.White;
            }
        }

        private void AddLabel(Person p, double totalPTOHours, bool isPdf)
        {
            //  Get labels collection
            if (isPdf)
            {
                var labels = chartPdf.ChartAreas[MAIN_CHART_AREA_NAME].AxisX.CustomLabels;
                var labels2 = chartPdf.ChartAreas[MAIN_CHART_AREA_NAME].AxisX2.CustomLabels;
                label(labels, labels2, p, _personsCount, totalPTOHours, isPdf);
            }
            else
            {

                var chartLabel1 = chart.ChartAreas[MAIN_CHART_AREA_NAME].AxisX.CustomLabels;
                var chartLabel2 = chart.ChartAreas[MAIN_CHART_AREA_NAME].AxisX2.CustomLabels;
                label(chartLabel1, chartLabel2, p, _personsCount, totalPTOHours);
            }
        }

        private void label(CustomLabelsCollection labelsX1, CustomLabelsCollection labelsX2, Person p, int count, double totalPTOHours, bool isPdf = false)
        {
            int tempCount;
            if (ConsultantPTOPerson.Count == 1)
            {
                tempCount = count + 1;
            }
            else
            {
                tempCount = count;
            }
            var label = labelsX1.Add(
                     tempCount - 0.49, // From position
                     tempCount + 0.49, // To position
                     FormatPersonName(p), // Formated person title
                     0, // Index
                     LabelMarkStyle.SideMark);

            chart.ChartAreas[MAIN_CHART_AREA_NAME].AxisX.LabelStyle.Font = new System.Drawing.Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);

            //  Url to person details page, return to report
            label.Url = getPersonUrl(p);

            //  Tooltip
            label.ToolTip = string.Format(DateTime.MinValue != p.HireDate ? PERSON_TOOLTIP_FORMAT : NOT_HIRED_PERSON_TOOLTIP_FORMAT,
                                            p.CurrentPay.TimescaleName, // Current Pay Type
                                            p.HireDate.ToString("MM/dd/yyyy") // Hire date
                    );
            if (page == 0 && isPdf)
            {
                labelsX2.Add(tempCount + 25 - 0.49, // From position
                                   tempCount + 25 + 0.49, // To position
                                   "Scheduled PTO Hours",
                                   0, // Index
                                   LabelMarkStyle.None);
            }

            if (count == ConsultantPTOPerson.Count - 1)
            {
                labelsX2.Add(
                           tempCount + 1 - 0.49, // From position
                           tempCount + 1 + 0.49, // To position
                           "Scheduled PTO Hours",
                           0, // Index
                           LabelMarkStyle.None);
            }
            var labelx2 = labelsX2.Add(
                    tempCount - 0.49, // From position
                    tempCount + 0.49, // To position
                    string.Empty.PadRight(12) + totalPTOHours.ToString(),
                    0, // Index
                    LabelMarkStyle.None);
        }

        private string getPersonUrl(Person p)
        {
            if (UserIsAdministrator)
            {
                return Urls.GetPersonDetailsUrl(p,
                        Constants.ApplicationPages.PTOTimelineWithFilterQueryString
                        );
            }
            return Urls.GetSkillsProfileUrl(p);
        }

        private DataPoint AddRange(DateTime pointStartDate, DateTime pointEndDate, double yvalue, bool isPdf)
        {
            int ind;

            if (ConsultantPTOPerson.Count == 1)
            {
                yvalue = 1;
            }

            if (isPdf)
            {
                ind = WeeksSeriesPdf.Points.AddXY(yvalue, pointStartDate, pointEndDate);
                return WeeksSeriesPdf.Points[ind];
            }
            else
            {
                ind = WeeksSeries.Points.AddXY(yvalue, pointStartDate, pointEndDate);
                return WeeksSeries.Points[ind];
            }
        }

        private void InitChart(bool isPdf = false, int pageNumber = 0)
        {
            if (isPdf)
            {
                SetFont();
                InitAxis(chartPdf.ChartAreas[MAIN_CHART_AREA_NAME].AxisY);
                InitAxis(chartPdf.ChartAreas[MAIN_CHART_AREA_NAME].AxisY2);
            }

            InitAxis(chart.ChartAreas[MAIN_CHART_AREA_NAME].AxisY);
            InitAxis(chart.ChartAreas[MAIN_CHART_AREA_NAME].AxisY2);
            UpdateChartTitle(isPdf, pageNumber);
            InitLegends(isPdf);
        }

        private void SetFont()
        {
            chartPdf.ChartAreas[MAIN_CHART_AREA_NAME].AxisY.LabelStyle.Font = new System.Drawing.Font("Candara", 9f);
            chartPdf.ChartAreas[MAIN_CHART_AREA_NAME].AxisY2.LabelStyle.Font = new System.Drawing.Font("Candara", 9f);
            chartPdf.ChartAreas[MAIN_CHART_AREA_NAME].AxisX.LabelStyle.Font = new System.Drawing.Font("Candara", 9f);
            chartPdf.ChartAreas[MAIN_CHART_AREA_NAME].AxisX2.LabelStyle.Font = new System.Drawing.Font("Candara", 9f);
        }

        private void InitAxis(Axis horizAxis)
        {
            var beginPeriodLocal = BegPeriod;
            var endPeriodLocal = EndPeriod;

            //  Set min and max values
            horizAxis.Minimum = beginPeriodLocal.ToOADate();
            horizAxis.Maximum = endPeriodLocal.AddDays(1).ToOADate();
            horizAxis.IsLabelAutoFit = true;
            horizAxis.IsStartedFromZero = true;
            horizAxis.LabelStyle.TruncatedLabels = true;
            horizAxis.MapAreaAttributes = "height: 10px;";

            if (IsshowTodayBar)
            {
                StripLine stripLine = new StripLine();
                stripLine.ForeColor = Color.Blue;
                stripLine.BorderColor = Color.Blue;
                stripLine.BorderWidth = 2;
                stripLine.StripWidthType = DateTimeIntervalType.Days;
                stripLine.Interval = 0;
                stripLine.IntervalOffset = DateTime.Today.ToOADate();
                stripLine.BorderDashStyle = ChartDashStyle.Solid;
                stripLine.ToolTip = "Today";
                horizAxis.StripLines.Add(stripLine);
            }

            if (DetalizationSelectedValue == "1")
            {
                horizAxis.IntervalType = DateTimeIntervalType.Weeks;
                horizAxis.Interval = 1;

                horizAxis.IntervalOffset = GetOffset(BegPeriod);
                horizAxis.IntervalOffsetType = DateTimeIntervalType.Days;
            }
            else if (DetalizationSelectedValue == "7")
            {
                horizAxis.Minimum = beginPeriodLocal.ToOADate();
                horizAxis.Maximum = endPeriodLocal.AddDays(1).ToOADate();

                horizAxis.IntervalType = DateTimeIntervalType.Weeks;
                horizAxis.Interval = 1;

                horizAxis.IntervalOffset = 0;
                horizAxis.IntervalOffsetType = DateTimeIntervalType.Days;
            }
            else
            {
                var beginPeriod = BegPeriod;
                var endPeriod = EndPeriod;

                if ((int)beginPeriod.DayOfWeek > 1)
                {
                    double period = Convert.ToDouble("-" + (int)beginPeriod.DayOfWeek);
                    horizAxis.Minimum = beginPeriod.AddDays(period + 1).ToOADate();
                }
                if ((int)endPeriod.DayOfWeek < 6)
                {
                    double period = Convert.ToDouble((int)endPeriod.DayOfWeek);
                    horizAxis.Maximum = endPeriod.AddDays(6 - period).ToOADate();
                }

                horizAxis.IntervalType = DateTimeIntervalType.Months;
                horizAxis.Interval = 1;
            }
        }

        private int GetOffset(DateTime date)
        {
            //Offset for sunday is 0,monday is -6,tuesday is -5,wednesday is -4,thursday is -3,friday is -2,saturday is -1
            if (date.DayOfWeek == DayOfWeek.Sunday)
                return 0;
            else
                return -1 * (7 - (int)date.DayOfWeek);
        }

        private void UpdateChartTitle(bool isPdf, int pageNumber)
        {
            //  Add chart title
            string personsPlaceHolder = string.Empty;
            string payTypePlaceHolder = string.Empty;
            if (ProjectedPersons && ActivePersons)
            {
                personsPlaceHolder = "All";
            }
            else if (ActivePersons)
            {
                personsPlaceHolder = "Active";
            }
            else if (ProjectedPersons)
            {
                personsPlaceHolder = "Projected";
            }
            else
            {
                personsPlaceHolder = "No";
            }

            if (PayTypeIsW2Hourly && PayTypeIsW2Salary)
            {
                payTypePlaceHolder = "W2-Salary and W2-Hourly";
            }
            else if (PayTypeIsW2Salary)
            {
                payTypePlaceHolder = "W2-Salary";
            }
            else if (PayTypeIsW2Hourly)
            {
                payTypePlaceHolder = "W2-Hourly";
            }
            else
            {
                payTypePlaceHolder = "No";
            }


            if (isPdf)
            {
                System.Web.UI.DataVisualization.Charting.Title title_top = new System.Web.UI.DataVisualization.Charting.Title(string.Format(
                   TITLE_FORMAT_PDF,
                     Period == "0" ? diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat) : BegPeriod.ToString(Constants.Formatting.EntryDateFormat),
                        Period == "0" ? diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat) : EndPeriod.ToString(Constants.Formatting.EntryDateFormat),
                    personsPlaceHolder, payTypePlaceHolder, PracticesFilterText(), DivisionsFilterText(), TitlesFilterText()));
                title_top.Font = new System.Drawing.Font("Candara", 9f);

                chartPdf.Titles.Add(title_top);

                System.Web.UI.DataVisualization.Charting.Title title_bottom = new System.Web.UI.DataVisualization.Charting.Title(string.Format(PageCount, pageNumber + 1, Math.Ceiling((double)ConsultantPTOPerson.Count / reportSize)));
                title_bottom.Alignment = ContentAlignment.BottomRight;
                title_bottom.Docking = Docking.Bottom;
                title_bottom.Font = new System.Drawing.Font("Candara", 9f);
                chartPdf.Titles.Add(title_bottom);
            }
            else
            {
                lblTitle.Text = string.Format(
                       TITLE_FORMAT,
                       Period == "0" ? diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat) : BegPeriod.ToString(Constants.Formatting.EntryDateFormat),
                       Period == "0" ? diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat) : EndPeriod.ToString(Constants.Formatting.EntryDateFormat),
                       personsPlaceHolder, payTypePlaceHolder, PracticesFilterText(), DivisionsFilterText(), TitlesFilterText());
            }

        }

        private void InitLegends(bool isPdf = false)
        {
            chart.Legends.Clear();

            chartPdf.Legends.Clear();
            var legendTop = new Legend()
            {
                DockedToChartArea = MAIN_CHART_AREA_NAME,
                Docking = Docking.Top,
                Alignment = StringAlignment.Center,
                IsDockedInsideChartArea = false,
                LegendStyle = LegendStyle.Table,
            };
            var legendBtm = new Legend()
            {
                DockedToChartArea = MAIN_CHART_AREA_NAME,
                Docking = Docking.Bottom,
                Alignment = StringAlignment.Center,
                IsDockedInsideChartArea = false,
                LegendStyle = LegendStyle.Table
            };


            if (isPdf)
            {
                chartPdf.Legends.Add(legendTop);
                chartPdf.Legends.Add(legendBtm);
            }
            chart.Legends.Add(legendTop);
            chart.Legends.Add(legendBtm);


            LegendCollection lcollection = isPdf ? chartPdf.Legends : chart.Legends;
            foreach (var legend in lcollection)
            {
                ColorLegends(legend);
            }

        }

        private void ColorLegends(Legend legend)
        {
            LegendItemsCollection legendItems = legend.CustomItems;
            legendItems.Clear();
            ConsReportColoringElementSection coloring =
              ConsReportColoringElementSection.ColorSettings;
            legendItems.Add(coloring.VacationColor, "PTO");
            legendItems.Add(coloring.CompanyHolidayColor, "Company Holiday");
            legendItems.Add(Color.Navy, "Other Time Off");
        }

        //Filter parameters for browser session
        private void SaveFilterValuesForSession()
        {
            PTOReportFilters filters = new PTOReportFilters();
            filters.IncludeActivePeople = ActivePersons;
            filters.IncludeProjectedPeople = ProjectedPersons;
            filters.IsW2Hourly = PayTypeIsW2Hourly;
            filters.IsW2Salary = PayTypeIsW2Salary;
            filters.Period = Period;
            filters.ReportStartDate = Period == "0" ? diRange.FromDate.Value : BegPeriod;
            filters.ReportEndDate = Period == "0" ? diRange.ToDate.Value : EndPeriod;
            filters.Granularity = Granularity;
            filters.PracticeAreaIds = PracticesSelected;
            filters.DivisionIds = DivisionsSelected;
            filters.TitleIds = TitlesSelected;
            filters.SortDirection = this.SortDirection;
            filters.SortId = SortId;
            ReportsFilterHelper.SaveFilterValues(ReportName.PTOReport, filters);
        }

        private void GetFilterValuesForSession()
        {
            var filters = ReportsFilterHelper.GetFilterValues(ReportName.PTOReport) as PTOReportFilters;
            if (filters != null)
            {
                ActivePersons = filters.IncludeActivePeople;
                ProjectedPersons = filters.IncludeProjectedPeople;
                PayTypeIsW2Hourly = filters.IsW2Hourly;
                PayTypeIsW2Salary = filters.IsW2Salary;
                Period = filters.Period;
                BegPeriod = filters.ReportStartDate;
                EndPeriod = filters.ReportEndDate;
                Granularity = filters.Granularity;
                PracticesSelected = filters.PracticeAreaIds;
                DivisionsSelected = filters.DivisionIds;
                TitlesSelected = filters.TitleIds;
                SortId = filters.SortId;
                this.SortDirection = filters.SortDirection;
                UpdateReport();
                updConsReport.Update();
            }
            else
            {
                IsFromBrowserSession = true;
                Reset();

            }
        }

        #endregion User defined Methods
    }

    public class DayType
    {
        //dayType = '1' for PTO '2' for company holiday
        public int Type
        {
            get;
            set;
        }
    }
}

