using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using DataTransferObjects.Reports;
using PraticeManagement.Utils.Excel;
using System.Data;
using PraticeManagement.Utils;
using DataTransferObjects.Filters;
using DataTransferObjects;

namespace PraticeManagement.Reports.Badge
{
    public partial class ResourceByPracticeReport : System.Web.UI.Page
    {
        private const string MAIN_CHART_AREA_NAME = "MainArea";
        private const string excelView = "The selected date range is too long with these parameters, report must be exported to Excel to see full details. Please click OK for an Excel export or click Cancel to view only the graph.";
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;

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
                CellStyles[] dataCellStylearray = { dataCellStyle };
                RowStyles datarowStyle = new RowStyles(dataCellStylearray);

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };

                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.MergeRegion.Add(new int[] { 0, 0, 0, coloumnsCount > 10 ? coloumnsCount : 11 - 1 });
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

                CellStyles dataDateCellStyle = new CellStyles();
                dataDateCellStyle.DataFormat = "mm/dd/yy;@";
                dataDateCellStyle.IsBold = true;
                dataDateCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

                List<CellStyles> headerCellStyleList = new List<CellStyles>();
                headerCellStyleList.Add(headerCellStyle);
                for (int i = 0; i < PracticeList[0].ResourcesCount.Count; i++)
                    headerCellStyleList.Add(dataDateCellStyle);
                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataCellStyle = new CellStyles();

                var dataCellStylearray = new List<CellStyles>() { dataCellStyle };
                for (int i = 0; i < PracticeList[0].ResourcesCount.Count; i++)
                    dataCellStylearray.Add(dataCellStyle);
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

        public string SelectedPractices
        {
            get
            {
                return filter.Practices;
            }
        }

        public string SelectedPayTypes
        {
            get
            {
                return filter.PayTypes;
            }
        }

        public string SelectedPersonStatuses
        {
            get
            {
                return filter.PersonStatus;
            }
        }

        public int SelectedView
        {
            get
            {
                return Convert.ToInt32(ddlView.SelectedValue);
            }
        }

        public string FormattedDate
        {
            get
            {
                string style = "";
                if (ddlView.SelectedValue == "1" || ddlView.SelectedValue == "7")
                    style = "dd-MMM-yy";
                else
                    style = "MMM-yyyy";
                return style;
            }
        }

        public List<int> RandomIndexes
        {
            get;
            set;
        }

        public List<GroupByPractice> PracticeList
        {
            get;
            set;
        }

        protected void dtpStart_SelectionChanged(object sender, EventArgs e)
        {
            var day = new ListItem("1 Day", "1");
            var week = new ListItem("1 Week", "7");
            var month = new ListItem("1 Month", "30");
            if (reqBadgeStart.IsValid && cvLastBadgeStart.IsValid && reqbadgeEnd.IsValid && cvbadgeEnd.IsValid && cvBadgeRange.IsValid)
            {
                ddlView.Items.Clear();
                var totalMonths = (((dtpEnd.DateValue.Year - dtpStart.DateValue.Year) * 12) + dtpEnd.DateValue.Month - dtpStart.DateValue.Month);
                if (totalMonths >= 12)
                {
                    ddlView.Items.Add(month);
                }
                else if (totalMonths >= 3 && totalMonths < 12)
                {
                    ddlView.Items.Add(week);
                    ddlView.Items.Add(month);
                }
                else if (totalMonths < 3)
                {
                    ddlView.Items.Add(day);
                    ddlView.Items.Add(week);
                    ddlView.Items.Add(month);
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dtpEnd.DateValue = DataTransferObjects.Utils.Generic.MonthEndDate(DateTime.Now);
                dtpStart.DateValue = DataTransferObjects.Utils.Generic.MonthStartDate(DateTime.Now);
                GetFilterValuesForSession();
            }
        }

        protected void custNotMorethan2Years_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!reqBadgeStart.IsValid || !cvLastBadgeStart.IsValid || !reqbadgeEnd.IsValid || !cvbadgeEnd.IsValid)
                return;
            var totalMonths = (((dtpEnd.DateValue.Year - dtpStart.DateValue.Year) * 12) + dtpEnd.DateValue.Month - dtpStart.DateValue.Month);
            args.IsValid = totalMonths < 24;
        }

        protected void custNotBeforeJuly_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!reqBadgeStart.IsValid || !cvLastBadgeStart.IsValid || !reqbadgeEnd.IsValid || !cvbadgeEnd.IsValid)
                return;
            args.IsValid = dtpStart.DateValue >= new DateTime(2014, 7, 1);
        }

        protected void btnUpdateView_Click(object sender, EventArgs e)
        {
            SaveFilterValuesForSession();
            ValidateAndPrepareData();
           
        }

        private void ValidateAndPrepareData()
        {
            Page.Validate("BadgeReport");
            if (!Page.IsValid)
            {
                divWholePage.Style.Add("display", "none");
                return;
            }
            divWholePage.Style.Remove("display");
            var data = ServiceCallers.Custom.Report(r => r.ResourcesByPracticeReport(SelectedPayTypes, SelectedPersonStatuses, SelectedPractices, dtpStart.DateValue, dtpEnd.DateValue, SelectedView)).ToList();
            PracticeList = data;
            if (PracticeList.Count > 0)
            {
                divWholePage.Style.Remove("display");
                divEmptyMessage.Style.Add("display", "none");
                PopulateChart();
                PracticeList.Add(new GroupByPractice() { Practice = new DataTransferObjects.Practice() { Id = -1, Name = "Total" } });
                PopulateTable();
            }
            else
            {
                divWholePage.Style.Add("display", "none");
                divEmptyMessage.Style.Remove("display");
            }
        }

        public void PopulateChart()
        {
            lblRange.Text = dtpStart.DateValue.ToString(Constants.Formatting.EntryDateFormat) + " - " + dtpEnd.DateValue.ToString(Constants.Formatting.EntryDateFormat);
            chartReport.Series.Clear();
            RandomIndexes = new List<int>();
            chartReport.Legends.Clear();
            var legend = new Legend("Default1")
            {
                DockedToChartArea = MAIN_CHART_AREA_NAME,
                Docking = Docking.Bottom,
                Alignment = StringAlignment.Center,
                IsDockedInsideChartArea = false,
                LegendStyle = LegendStyle.Table
            };
            foreach (var practice in PracticeList)
            {
                var color = GetRandomColor();
                CalculateCount(practice.ResourcesCount);
                var series = new Series()
                {
                    Name = practice.Practice.HtmlEncodedName,
                    ChartArea = MAIN_CHART_AREA_NAME,
                    ChartType = SeriesChartType.Line,
                    XValueType = ChartValueType.String,
                    BorderWidth = 2,
                    IsVisibleInLegend = false,
                    Color = color,
                    XAxisType = AxisType.Primary,
                    YAxisType = AxisType.Primary,
                    YValueType = ChartValueType.Int32,
                    ToolTip = "#VALY Resources"
                };
                LegendItem legendItem = new LegendItem();
                legendItem.Name = practice.Practice.Name;
                legendItem.ImageStyle = LegendImageStyle.Rectangle;
                legendItem.MarkerStyle = MarkerStyle.Square;
                legendItem.MarkerSize = 10;
                legendItem.MarkerColor = Color.Black;
                legendItem.Color = color;
                legend.CustomItems.Add(legendItem);
                var xvalues = practice.ResourcesCount.Select(c => c.StartDate.ToString(FormattedDate)).ToList();
                var yValues = practice.ResourcesCount.Select(c => c.Count).ToList();
                series.Points.DataBindXY(xvalues, yValues);
                chartReport.Series.Add(series);
            }
            chartReport.Legends.Add(legend);
            if (PracticeList.Count > 0)
                InitChart(PracticeList[0].ResourcesCount.Count);
        }

        public void CalculateCount(List<BadgedResourcesByTime> list)
        {
            if (list == null) return;
            foreach (var item in list)
            {
                var count = 0;
                if (filter.IsBadgedNotOnProject)
                    count += item.BadgedNotOnProjectCount;
                if (filter.IsBadgedOnProject)
                    count += item.BadgedOnProjectCount;
                if (filter.IsClockNotStarted)
                    count += item.ClockNotStartedCount;
                item.Count = count;
            }
        }

        public void PopulateTable()
        {
            if ((SelectedView == 1 && PracticeList[0].ResourcesCount.Count > 31) || (SelectedView == 7 && PracticeList[0].ResourcesCount.Count > 28))
            {
                repReportTable.Visible = false;
                mlConfirmation.ShowWarningMessage(excelView);
                mpeBadgePanel.Show();
            }
            else
            {
                repReportTable.Visible = true;
                repReportTable.DataSource = PracticeList;
                repReportTable.DataBind();
            }
        }

        protected void repReportTable_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                var repDatesHeaders = e.Item.FindControl("repDatesHeaders") as Repeater;
                var dates = PracticeList[0].ResourcesCount.Select(d => new { date = d.StartDate.ToString(FormattedDate) }).ToList();
                repDatesHeaders.DataSource = dates;
                repDatesHeaders.DataBind();
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = (GroupByPractice)e.Item.DataItem;
                var lblPractice = e.Item.FindControl("lblPractice") as Label;
                var repCount = e.Item.FindControl("repCount") as Repeater;
                if (dataItem.Practice.Id == -1)
                    lblPractice.CssClass = "fontBold";
                repCount.DataSource = dataItem.Practice.Id == -1? MakeTotalList() : dataItem.ResourcesCount;
                repCount.DataBind();
            }
        }

        public List<BadgedResourcesByTime> MakeTotalList()
        {
            var rangesList = new List<BadgedResourcesByTime>();
            foreach (var practice in PracticeList)
            {
                if (practice.ResourcesCount == null) continue;
                foreach (var resourceCount in practice.ResourcesCount)
                {
                    if (rangesList.Exists(p => p.StartDate == resourceCount.StartDate))
                    {
                        var item = rangesList.FirstOrDefault(p => p.StartDate == resourceCount.StartDate);
                        item.BadgedOnProjectCount += resourceCount.BadgedOnProjectCount;
                        item.BadgedNotOnProjectCount += resourceCount.BadgedNotOnProjectCount;
                        item.ClockNotStartedCount += resourceCount.ClockNotStartedCount;
                    }
                    else
                    {
                        rangesList.Add(new BadgedResourcesByTime() { TypeNo=100, StartDate = resourceCount.StartDate, BadgedNotOnProjectCount = resourceCount.BadgedNotOnProjectCount, BadgedOnProjectCount = resourceCount.BadgedOnProjectCount, ClockNotStartedCount = resourceCount.ClockNotStartedCount });
                    }
                }
            }
            return rangesList;
        }

        protected void repCount_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = (BadgedResourcesByTime)e.Item.DataItem;
                var lblCount = e.Item.FindControl("lblCount") as Label;
                var count = 0;
                if (filter.IsBadgedNotOnProject)
                    count += dataItem.BadgedNotOnProjectCount;
                if (filter.IsBadgedOnProject)
                    count += dataItem.BadgedOnProjectCount;
                if (filter.IsClockNotStarted)
                    count += dataItem.ClockNotStartedCount;
                lblCount.Text = count.ToString();
                if (dataItem.TypeNo == 100)
                    lblCount.CssClass = "fontBold";
            }
        }

        protected void btnOKValidationPanel_Click(object sender, EventArgs e)
        {
            mpeBadgePanel.Hide();
            GenerateExcel();
        }

        private void InitChart(int count)
        {
            if ((int)chartReport.Height.Value < 550)
                chartReport.Height = 550;
            var units = 60 * count;
            chartReport.Width = units < 550 ? 550 : units > 1700 ? 1700 : units;
            InitAxis(chartReport.ChartAreas[MAIN_CHART_AREA_NAME].AxisX, true, SelectedView == 1 ? "Day" : SelectedView == 7 ? "Week" : "Month", false);
            InitAxis(chartReport.ChartAreas[MAIN_CHART_AREA_NAME].AxisY, false, "Number of Resources", true);
            UpdateChartTitle();
            chartReport.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
        }

        private void InitAxis(Axis horizAxis, bool isXAxis, string title, bool isVertical, int labelAngle = -1)
        {
            horizAxis.IsStartedFromZero = true;
            horizAxis.TextOrientation = isVertical ? TextOrientation.Rotated270 : TextOrientation.Horizontal;
            horizAxis.LabelStyle.Font = new Font("Arial", 10f);
            horizAxis.TitleFont = new System.Drawing.Font("Arial", 14, FontStyle.Bold);
            horizAxis.ArrowStyle = AxisArrowStyle.None;
            horizAxis.MajorGrid.Enabled = false;
            horizAxis.ToolTip = horizAxis.Title = title;
            horizAxis.IsMarginVisible = true;
            if (!isXAxis)
                return;
            horizAxis.Interval = 1;
        }

        private void UpdateChartTitle()
        {
            System.Web.UI.DataVisualization.Charting.Title title = new System.Web.UI.DataVisualization.Charting.Title();
            title.Text = title.ToolTip = "Available Resources by Practice";
            chartReport.Titles.Clear();
            chartReport.Titles.Add(title);
            chartReport.Titles[0].Font = new System.Drawing.Font("Arial", 16, FontStyle.Bold);
        }

        public System.Drawing.Color GetRandomColor()
        {
            var listOfColors = new List<Color>()
            {
                Color.Blue,
                Color.Black,
                Color.SaddleBrown,
                Color.Gray,
                Color.DarkBlue,

                Color.Red,
                Color.Pink,
                Color.Violet,
                Color.Tomato,
                Color.SteelBlue,
                
                Color.SkyBlue,
                Color.Purple,
                Color.PaleGreen,
                Color.Green,
                Color.Gold,
                
                Color.Firebrick,
                Color.DodgerBlue,
                Color.DeepPink,
                Color.DarkViolet,
                Color.DarkRed,

                Color.DarkMagenta,
                Color.DarkCyan,
                Color.DarkKhaki,
                Color.YellowGreen,
                Color.DarkOrange
            };
            Random rnd = new Random();
            int value = rnd.Next(25);
            while (true)
            {
                if (RandomIndexes.Any(v => v == value))
                {
                    value = rnd.Next(25);
                }
                else
                {
                    RandomIndexes.Add(value);
                    break;
                }
            }
            return listOfColors[value];
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            GenerateExcel();
        }

        public void GenerateExcel()
        {
            var filename = string.Format("AvailableResourceByPractice_{0}-{1}.xls", dtpStart.DateValue.ToString("MM_dd_yyyy"), dtpEnd.DateValue.ToString("MM_dd_yyyy"));
            var sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();
            var report = ServiceCallers.Custom.Report(r => r.ResourcesByPracticeReport(SelectedPayTypes,SelectedPersonStatuses,SelectedPractices, dtpStart.DateValue, dtpEnd.DateValue, SelectedView)).ToList();
            PracticeList = report;
            if (PracticeList.Count > 0)
            {
                string dateRangeTitle = string.Format("Available Resource by practice report for the period: {0} to {1}", dtpStart.DateValue.ToString(Constants.Formatting.EntryDateFormat), dtpEnd.DateValue.ToString(Constants.Formatting.EntryDateFormat));
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                headerRowsCount = header.Rows.Count + 3;
                var data = PrepareDataTable(PracticeList);
                coloumnsCount = data.Columns.Count;
                sheetStylesList.Add(HeaderSheetStyle);
                sheetStylesList.Add(DataSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "AvailableResourceByPractice";
                dataset.Tables.Add(header);
                dataset.Tables.Add(data);
                dataSetList.Add(dataset);
            }
            NPOIExcel.Export(filename, dataSetList, sheetStylesList);
        }

        public DataTable PrepareDataTable(List<GroupByPractice> report)
        {
            DataTable data = new DataTable();
            List<object> row;

            data.Columns.Add("Practice Name");
            foreach (var header in report[0].ResourcesCount)
            {
                data.Columns.Add(header.StartDate.ToShortDateString());
            }
            foreach (var reportItem in report)
            {
                CalculateCount(reportItem.ResourcesCount);
                row = new List<object>();
                row.Add(reportItem.Practice.Name);
                foreach (var item in reportItem.ResourcesCount)
                    row.Add(item.Count);
                data.Rows.Add(row.ToArray());
            }
            return data;
        }

        private void SaveFilterValuesForSession()
        {
            BadgeResourceFilters filters = new BadgeResourceFilters();
            filters.PayTypeIds = SelectedPayTypes;
            filters.PersonStatusIds = SelectedPersonStatuses;
            filters.PracticeIds = SelectedPractices;
            filters.SelectedView = SelectedView;
            filters.IsBadgedNotOnProject = filter.IsBadgedNotOnProject;
            filters.IsBadgedOnProject = filter.IsBadgedOnProject;
            filters.IsClockNotStarted = filter.IsClockNotStarted;
            filters.ReportStartDate=dtpStart.DateValue;
            filters.ReportEndDate = dtpEnd.DateValue;
            ReportsFilterHelper.SaveFilterValues(ReportName.ResourceByPracticeReport, filters);
        }

        private void GetFilterValuesForSession()
        {
            var filters = ReportsFilterHelper.GetFilterValues(ReportName.ResourceByPracticeReport) as BadgeResourceFilters;
            if (filters != null)
            {
                filter.PayTypes = filters.PayTypeIds;
                filter.PersonStatus = filters.PersonStatusIds;
                filter.Practices = filters.PracticeIds;
                ddlView.SelectedValue = filters.SelectedView.ToString();
                filter.IsBadgedNotOnProject = filters.IsBadgedNotOnProject;
                filter.IsBadgedOnProject = filters.IsBadgedOnProject;
                filter.IsClockNotStarted = filters.IsClockNotStarted;
                dtpStart.DateValue = filters.ReportStartDate;
                dtpEnd.DateValue = filters.ReportEndDate;
                ValidateAndPrepareData();
            }
        }
    }
}

