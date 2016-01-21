using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.HtmlControls;
using System.Drawing;
using DataTransferObjects;
using DataTransferObjects.Utils;
using System.Data;
using PraticeManagement.Utils.Excel;
using PraticeManagement.Utils;
using DataTransferObjects.Reports;
using System.Text;
using PraticeManagement.Controls;
using PraticeManagement.PersonStatusService;
using System.ServiceModel;
using DataTransferObjects.Filters;

namespace PraticeManagement.Reports.Badge
{
    public partial class BadgeResourceByTime : System.Web.UI.Page
    {
        private const string MAIN_CHART_AREA_NAME = "MainArea";
        private const string excelView = "The selected date range is too long with these parameters, report must be exported to Excel to see full details. Please click OK for an Excel export or click Cancel to view only the graph.";
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;

        public List<int> TotalList { get; set; }

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
                sheetStyle.MergeRegion.Add(new int[] { 0, 0, 0, coloumnsCount > 10? coloumnsCount:11 - 1 });
                sheetStyle.IsAutoResize = false;

                return sheetStyle;
            }
        }

        private SheetStyles DataSheetStyle
        {
            get
            {
                CellStyles headerWrapCellStyle = new CellStyles();
                headerWrapCellStyle.IsBold = true;
                headerWrapCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                headerWrapCellStyle.WrapText = true;
                

                CellStyles headerCellStyle = new CellStyles();
                headerCellStyle.IsBold = true;
                headerCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

                CellStyles dataDateCellStyle = new CellStyles();
                dataDateCellStyle.DataFormat = "mm/dd/yy;@";
                dataDateCellStyle.IsBold = true;
                dataDateCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

                List<CellStyles> headerCellStyleList = new List<CellStyles>();
                headerCellStyleList.Add(headerWrapCellStyle);
                for (int i = 0; i < BadgedResources.Count; i++)
                    headerCellStyleList.Add(dataDateCellStyle);
                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataCellStyle = new CellStyles();

                var dataCellStylearray = new List<CellStyles>() { dataCellStyle };
                for (int i = 0; i < BadgedResources.Count; i++)
                    dataCellStylearray.Add(dataCellStyle);
                //List<int> coloumnWidth = new List<int>();
                //coloumnWidth.Add(30);
                //for (int i = 0; i < BadgedResources.Count; i++)
                //    coloumnWidth.Add(0);
                
                RowStyles datarowStyle = new RowStyles(dataCellStylearray.ToArray());
                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.TopRowNo = headerRowsCount;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanColSplit = 0;
                sheetStyle.FreezePanRowSplit = headerRowsCount;
                //sheetStyle.ColoumnWidths = coloumnWidth;
                return sheetStyle;
            }
        }

        public int SelectedView
        {
            get
            {
                return Convert.ToInt32(ddlView.SelectedValue);
            }
        }

        public string PersonStatus
        {
            get
            {
                var clientList = new StringBuilder();
                foreach (ListItem item in cblPersonStatus.Items)
                {
                    if (item.Selected)
                        clientList.Append(item.Value).Append(',');
                    if (item.Value == "1" && item.Selected)
                    {
                        clientList.Append("2").Append(',');
                        clientList.Append("5").Append(',');
                    }
                }
                return clientList.ToString();
            }
        }

        public string PayTypes
        {
            get
            {
                return cblPayTypes.SelectedItems;
            }
        }

        public List<DataTransferObjects.Reports.BadgedResourcesByTime> BadgedResources
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dtpEnd.DateValue = DataTransferObjects.Utils.Generic.MonthEndDate(DateTime.Now);
                dtpStart.DateValue = DataTransferObjects.Utils.Generic.MonthStartDate(DateTime.Now);
                DataHelper.FillTimescaleList(this.cblPayTypes, Resources.Controls.AllTypes);
                cblPayTypes.SelectItems(new List<int>() { 1, 2 });
                FillPersonStatusList();
                cblPersonStatus.SelectItems(new List<int>() { 1, 5 });
                GetFilterValuesForSession();
            }
        }

        public void FillPersonStatusList()
        {
            using (var serviceClient = new PersonStatusServiceClient())
            {
                try
                {
                    var statuses = serviceClient.GetPersonStatuses();
                    statuses = statuses.Where(p => p.Id != 2 && p.Id != 5).ToArray();
                    DataHelper.FillListDefault(cblPersonStatus, Resources.Controls.AllTypes, statuses, false);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
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

        protected void btnUpdateView_Click(object sender, EventArgs e)
        {
            //Page.Validate("BadgeReport");
            //if (!Page.IsValid)
            //{
            //    divWholePage.Style.Add("display", "none");
            //    return;
            //}
            //divWholePage.Style.Remove("display");
            SaveFilterValuesForSession();
            PopulateChart();
            PopulateTable();
        }

        protected string GetLink(int type, DateTime startDate, DateTime endDate)
        {
            var url = "";
            switch (type)
            {
                case 1: url = Constants.ApplicationPages.BadgedNotOnProjectReport;
                    break;
                case 2: url = Constants.ApplicationPages.BadgedOnProjectReport;
                    break;
                case 3: url = Constants.ApplicationPages.ClockNotStartedReport;
                    break;
                case 4: url = Constants.ApplicationPages.BadgeBlockedReport;
                    break;
                case 5: url = Constants.ApplicationPages.BadgeBreakReport;
                    break;
                case 6: url = Constants.ApplicationPages.BadgedOnProjectException;
                    break;
                case 7: url = Constants.ApplicationPages.BadgedNotOnProjectException;
                    break;
            }
            return Utils.Generic.GetTargetUrlWithReturn(String.Format(url, startDate.ToShortDateString(), endDate.ToShortDateString(), cblPayTypes.areAllSelected ? "null" : cblPayTypes.SelectedItems, PersonStatus),
                                                        Constants.ApplicationPages.BadgedResourcesByTimeReport);
        }

        public void PopulateChart()
        {
            Page.Validate("BadgeReport");
            if (!Page.IsValid)
            {
                divWholePage.Style.Add("display", "none");
                return;
            }
            divWholePage.Style.Remove("display");
            lblRange.Text = dtpStart.DateValue.ToString(Constants.Formatting.EntryDateFormat) + " - " + dtpEnd.DateValue.ToString(Constants.Formatting.EntryDateFormat);
            var paytypes = cblPayTypes.areAllSelected ? null : cblPayTypes.SelectedItems;
            var statuses = PersonStatus;
            var data = ServiceCallers.Custom.Report(r => r.BadgedResourcesByTimeReport(paytypes, statuses, dtpStart.DateValue, dtpEnd.DateValue, Convert.ToInt32(ddlView.SelectedValue)).ToList());
            BadgedResources = data;
            chartReport.DataSource = data.Select(p => new { month = p.StartDate.ToString(FormattedDate), badgedOnProjectcount = p.BadgedOnProjectCount, badgedProjectExceptioncount = p.BadgedOnProjectExceptionCount, badgedNotOnProjectExceptioncount = p.BadgedNotOnProjectExceptionCount, badgedNotOnProjectcount = p.BadgedNotOnProjectCount, clockNotStartedCount = p.ClockNotStartedCount, blockedCount = p.BlockedCount, breakCount = p.InBreakPeriodCount }).ToList();
            chartReport.DataBind();
            InitChart(data.Count);
        }

        //protected void chartReport_Click(object sender, ImageMapEventArgs e)
        //{
        //    var query = e.PostBackValue.Split(',');
        //    var startDate = Convert.ToDateTime(query[0]);
        //    var endDate = SelectedView == 30 ? startDate.AddMonths(1).AddDays(-1) : startDate.AddDays(SelectedView - 1);
        //    var page = "";
        //    switch (query[2])
        //    {
        //        case "NotOnBadged": page = "BadgedNotOnProjectReport.aspx";
        //            break;
        //        case "ClockNotStarted": page = "ClockNotStartedReport.aspx";
        //            break;
        //        case "Blocked": page = "BadgeBlockedReport.aspx";
        //            break;
        //        case "Break": page = "BadgeBreakReport.aspx";
        //            break;
        //        case "BadgedOnProject": page = "BadgedOnProjectReport.aspx";
        //            break;

        //    }
        //    var url = string.Format("{0}?StartDate={1}&EndDate={2}",page, startDate.ToShortDateString(), endDate.ToShortDateString());
        //    ScriptManager.RegisterStartupScript(this, typeof(string), "openWindow",
        //    string.Format("window.open( '{0}', target='blank');", url), true);
        //}

        public void PopulateTable()
        {
            if ((SelectedView == 1 && BadgedResources.Count > 31) || (SelectedView == 7 && BadgedResources.Count > 28))
            {
                repReportTable.Visible = false;
                mlConfirmation.ShowWarningMessage(excelView);
                mpeBadgePanel.Show();
            }
            else
            {
                repReportTable.Visible = true;
                repReportTable.DataSource = PrepareSourceForTable();
                repReportTable.DataBind();
            }
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            GenerateExcel();
        }

        protected void btnOKValidationPanel_Click(object sender, EventArgs e)
        {
            mpeBadgePanel.Hide();
            GenerateExcel();
        }

        protected void repReportTable_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var dataItem = (ReportTable)e.Item.DataItem;
            if (e.Item.ItemType == ListItemType.Header)
            {
                var repDatesHeaders = e.Item.FindControl("repDatesHeaders") as Repeater;
                var dates = BadgedResources.Select(d => new { date = d.StartDate.ToString(FormattedDate) }).ToList();
                repDatesHeaders.DataSource = dates;
                repDatesHeaders.DataBind();
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lblCategory = e.Item.FindControl("lblCategory") as Label;
                var repCount = e.Item.FindControl("repCount") as Repeater;
                lblCategory.Text = dataItem.Category;
                if (lblCategory.Text == "TOTALS")
                {
                    lblCategory.CssClass = "fontBold";
                }
                repCount.DataSource = dataItem.Count;
                repCount.DataBind();
            }
            //else
            //{
            //    var lblTotal = e.Item.FindControl("lblTotal") as Label;
            //    lblTotal.Text = dataItem.Count.Sum(r => r.BadgedOnProjectCount).ToString();
            //}
        }

        protected void repCount_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                TotalList = new List<int>();
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = (BadgedResourcesByTime)e.Item.DataItem;
                var lblCategory = e.Item.FindControl("lblCategory") as Label;
                var lblCount = e.Item.FindControl("lblCount") as Label;
                var hlCount = e.Item.FindControl("hlCount") as HyperLink;
                TotalList.Add(dataItem.BadgedOnProjectCount);
                if (dataItem.TypeNo == 0)
                {
                    hlCount.Visible = false;
                    lblCount.Visible = true;
                    if (dataItem.TypeNo == 0)
                        lblCount.CssClass = "fontBold";
                }
                else
                {
                    hlCount.Visible = true;
                    lblCount.Visible = false;
                    hlCount.NavigateUrl = GetLink(dataItem.TypeNo, dataItem.StartDate, dataItem.EndDate);
                }
            }
            else
            {
                var lblTotal = e.Item.FindControl("lblTotal") as Label;
                lblTotal.CssClass = "fontBold";
                lblTotal.Text = TotalList.Sum().ToString();
            }
        }

        public List<ReportTable> PrepareSourceForTable()
        {
            var list = new List<ReportTable>();
            var badgednotProject = new ReportTable()
            {
                Category = "# Resources with Active Clocks, Not on Project",
                Count = BadgedResources.Select(c => new BadgedResourcesByTime() { StartDate = c.StartDate, EndDate = c.EndDate, BadgedOnProjectCount = c.BadgedNotOnProjectCount, TypeNo = 1 }).ToList()
            };
            var badgednotProjectException = new ReportTable()
            {
                Category = "# Resources with Person-Based Exceptions",
                Count = BadgedResources.Select(c => new BadgedResourcesByTime() { StartDate = c.StartDate, EndDate = c.EndDate, BadgedOnProjectCount = c.BadgedNotOnProjectExceptionCount, TypeNo = 7 }).ToList()
            };
            var badgedProject = new ReportTable()
            {
                Category = "# Badged resources on Project",
                Count = BadgedResources.Select(c => new BadgedResourcesByTime() { StartDate = c.StartDate, EndDate = c.EndDate, BadgedOnProjectCount = c.BadgedOnProjectCount, TypeNo = 2 }).ToList()
            };
            var badgedProjectException = new ReportTable()
            {
                Category = "# Resources with Project-Based Exceptions",
                Count = BadgedResources.Select(c => new BadgedResourcesByTime() { StartDate = c.StartDate, EndDate = c.EndDate, BadgedOnProjectCount = c.BadgedOnProjectExceptionCount, TypeNo = 6 }).ToList()
            };
            var clocknotStarted = new ReportTable()
            {
                Category = "# 18-Month Clock Not Started",
                Count = BadgedResources.Select(c => new BadgedResourcesByTime() { StartDate = c.StartDate, EndDate = c.EndDate, BadgedOnProjectCount = c.ClockNotStartedCount, TypeNo = 3 }).ToList()
            };
            var blocked = new ReportTable()
            {
                Category = "# Blocked",
                Count = BadgedResources.Select(c => new BadgedResourcesByTime() { StartDate = c.StartDate, EndDate = c.EndDate, BadgedOnProjectCount = c.BlockedCount, TypeNo = 4 }).ToList()
            };
            var breakPeriod = new ReportTable()
            {
                Category = "# On 6-Month Break",
                Count = BadgedResources.Select(c => new BadgedResourcesByTime() { StartDate = c.StartDate, EndDate = c.EndDate, BadgedOnProjectCount = c.InBreakPeriodCount, TypeNo = 5 }).ToList()
            };
            var totals = new ReportTable()
            {
                Category = "TOTALS",
                Count = BadgedResources.Select(c => new BadgedResourcesByTime() { BadgedOnProjectCount = c.BadgedNotOnProjectCount + c.BadgedNotOnProjectExceptionCount + c.BadgedOnProjectCount + c.BadgedOnProjectExceptionCount + c.ClockNotStartedCount + c.BlockedCount + c.InBreakPeriodCount, TypeNo = 0 }).ToList()
            };
            list.Add(badgednotProject);
            list.Add(badgednotProjectException);
            list.Add(badgedProject);
            list.Add(badgedProjectException);
            list.Add(clocknotStarted);
            list.Add(blocked);
            list.Add(breakPeriod);
            list.Add(totals);
            return list;
        }

        public DataTable PrepareDataTable(List<ReportTable> report)
        {
            DataTable data = new DataTable();
            List<object> row;

            data.Columns.Add("Category");
            foreach (var header in BadgedResources)
            {
                data.Columns.Add(header.StartDate.ToShortDateString());
            }
            data.Columns.Add("TOTALS");
            foreach (var reportItem in report)
            {
                row = new List<object>();
                row.Add(reportItem.Category);
                foreach (var item in reportItem.Count)
                    row.Add(item.BadgedOnProjectCount);
                row.Add(reportItem.Count.Sum(p => p.BadgedOnProjectCount));
                data.Rows.Add(row.ToArray());
            }
            return data;
        }

        public void GenerateExcel()
        {
            var filename = string.Format("BadgedResourceByTime_{0}-{1}.xls", dtpStart.DateValue.ToString("MM_dd_yyyy"), dtpEnd.DateValue.ToString("MM_dd_yyyy"));
            var sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();
            var paytypes = cblPayTypes.areAllSelected ? null : cblPayTypes.SelectedItems;
            var statuses = PersonStatus;
            var report = ServiceCallers.Custom.Report(r => r.BadgedResourcesByTimeReport(paytypes, statuses, dtpStart.DateValue, dtpEnd.DateValue, Convert.ToInt32(ddlView.SelectedValue)).ToList());
            BadgedResources = report;
            if (BadgedResources.Count > 0)
            {
                string dateRangeTitle = string.Format("Badged Resource by time report for the period: {0} to {1}", dtpStart.DateValue.ToString(Constants.Formatting.EntryDateFormat), dtpEnd.DateValue.ToString(Constants.Formatting.EntryDateFormat));
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                headerRowsCount = header.Rows.Count + 3;
                var data = PrepareDataTable(PrepareSourceForTable());
                coloumnsCount = data.Columns.Count;
                sheetStylesList.Add(HeaderSheetStyle);
                sheetStylesList.Add(DataSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "BadgedResourceByTime";
                dataset.Tables.Add(header);
                dataset.Tables.Add(data);
                dataSetList.Add(dataset);
            }
            NPOIExcel.Export(filename, dataSetList, sheetStylesList);
        }

        private void InitChart(int count)
        {
            if ((int)chartReport.Height.Value < 550)
                chartReport.Height = 550;
            var units = 60 * count;
            chartReport.Width = units < 1000 ? 1000 : units > 1900 ? 1900 : units;
            InitAxis(chartReport.ChartAreas[MAIN_CHART_AREA_NAME].AxisX, true, SelectedView == 1 ? "Day" : SelectedView == 7 ? "Week" : "Month", false);
            InitAxis(chartReport.ChartAreas[MAIN_CHART_AREA_NAME].AxisY, false, "Number of Resources", true);
            UpdateChartTitle();
            chartReport.Legends.Clear();
            var legend = new Legend("Legend2");

            AddLegendItems(legend);


            chartReport.Legends.Add(legend);
            chartReport.Legends["Legend2"].DockedToChartArea = MAIN_CHART_AREA_NAME;
            chartReport.Legends["Legend2"].Docking = Docking.Right;
            chartReport.Legends["Legend2"].LegendStyle = LegendStyle.Column;
            chartReport.Legends["Legend2"].Alignment = StringAlignment.Center;
            chartReport.Legends["Legend2"].ItemColumnSpacing = 70;

            chartReport.Legends["Legend2"].Font = new System.Drawing.Font("Arial", 10, FontStyle.Regular);

            chartReport.Legends["Legend2"].IsDockedInsideChartArea = false;
            chartReport.Legends["Legend2"].Position.Auto = true;
            chartReport.ChartAreas[0].AxisX.LabelStyle.Angle = -90;

            chartReport.Series["chartSeries"].IsVisibleInLegend = false;
            chartReport.Series["chartSeries1"].IsVisibleInLegend = false;
            chartReport.Series["chartSeries2"].IsVisibleInLegend = false;
            chartReport.Series["chartSeries3"].IsVisibleInLegend = false;
            chartReport.Series["chartSeries4"].IsVisibleInLegend = false;
            chartReport.Series["chartSeries5"].IsVisibleInLegend = false;
            chartReport.Series["chartSeries6"].IsVisibleInLegend = false;
            chartReport.Series["chartSeries7"].IsVisibleInLegend = false;
            chartReport.Series["chartSeries8"].IsVisibleInLegend = false;
            chartReport.Series["chartSeries9"].IsVisibleInLegend = false;
        }

        public void AddLegendItems(Legend legend)
        {
            var ltBadgedNotOnProjectException = new LegendItem();
            ltBadgedNotOnProjectException.Name = "Resources with Person-Based Exceptions";
            ltBadgedNotOnProjectException.ImageStyle = LegendImageStyle.Rectangle;
            ltBadgedNotOnProjectException.MarkerStyle = MarkerStyle.Square;
            ltBadgedNotOnProjectException.MarkerSize = 10;
            ltBadgedNotOnProjectException.SeparatorType = LegendSeparatorStyle.DoubleLine;
            ltBadgedNotOnProjectException.SeparatorColor = Color.White;
            ltBadgedNotOnProjectException.MarkerColor = Color.Black;
            ltBadgedNotOnProjectException.Color = Color.Brown;

            legend.CustomItems.Add(ltBadgedNotOnProjectException);

            var ltClockNotStarted = new LegendItem();
            ltClockNotStarted.Name = "18 Month Clock Not Started";
            ltClockNotStarted.ImageStyle = LegendImageStyle.Rectangle;
            ltClockNotStarted.MarkerStyle = MarkerStyle.Square;
            ltClockNotStarted.SeparatorType = LegendSeparatorStyle.DoubleLine;
            ltClockNotStarted.SeparatorColor = Color.White;
            ltClockNotStarted.MarkerSize = 10;
            ltClockNotStarted.MarkerColor = Color.Black;
            ltClockNotStarted.Color = Color.Gray;
            legend.CustomItems.Add(ltClockNotStarted);

            var ltBadgedNotOnProject = new LegendItem();
            ltBadgedNotOnProject.Name = "Resources with Active Clocks, Not on Project";
            ltBadgedNotOnProject.ImageStyle = LegendImageStyle.Rectangle;
            ltBadgedNotOnProject.MarkerStyle = MarkerStyle.Square;
            ltBadgedNotOnProject.MarkerSize = 10;
            ltBadgedNotOnProject.SeparatorType = LegendSeparatorStyle.DoubleLine;
            ltBadgedNotOnProject.SeparatorColor = Color.White;
            ltBadgedNotOnProject.MarkerColor = Color.Black;
            ltBadgedNotOnProject.Color = Color.Blue;
            legend.CustomItems.Add(ltBadgedNotOnProject);

            var ltBreak = new LegendItem();
            ltBreak.Name = "On 6-Month Break";
            ltBreak.ImageStyle = LegendImageStyle.Rectangle;
            ltBreak.MarkerStyle = MarkerStyle.Square;
            ltBreak.SeparatorType = LegendSeparatorStyle.DoubleLine;
            ltBreak.SeparatorColor = Color.White;
            ltBreak.MarkerSize = 10;
            ltBreak.MarkerColor = Color.Black;
            ltBreak.Color = Color.DarkBlue;
            legend.CustomItems.Add(ltBreak);

            var ltBlocked = new LegendItem();
            ltBlocked.Name = "Blocked";
            ltBlocked.ImageStyle = LegendImageStyle.Rectangle;
            ltBlocked.SeparatorType = LegendSeparatorStyle.DoubleLine;
            ltBlocked.SeparatorColor = Color.White;
            ltBlocked.MarkerStyle = MarkerStyle.Square;
            ltBlocked.MarkerSize = 10;
            ltBlocked.MarkerColor = Color.Black;
            ltBlocked.Color = Color.Orange;
            legend.CustomItems.Add(ltBlocked);

            var ltBadgedProjectException = new LegendItem();
            ltBadgedProjectException.Name = "Resources with Project-Based Exceptions";
            ltBadgedProjectException.ImageStyle = LegendImageStyle.Rectangle;
            ltBadgedProjectException.MarkerStyle = MarkerStyle.Square;
            ltBadgedProjectException.SeparatorType = LegendSeparatorStyle.DoubleLine;
            ltBadgedProjectException.SeparatorColor = Color.White;
            ltBadgedProjectException.MarkerSize = 10;
            ltBadgedProjectException.MarkerColor = Color.Black;
            ltBadgedProjectException.Color = Color.Pink;
            legend.CustomItems.Add(ltBadgedProjectException);

            var ltBadgedProject = new LegendItem();
            ltBadgedProject.Name = "Badged resources on project";
            ltBadgedProject.ImageStyle = LegendImageStyle.Rectangle;
            ltBadgedProject.SeparatorType = LegendSeparatorStyle.DoubleLine;
            ltBadgedProject.SeparatorColor = Color.White;
            ltBadgedProject.MarkerStyle = MarkerStyle.Square;
            ltBadgedProject.MarkerSize = 10;
            ltBadgedProject.MarkerColor = Color.Black;
            ltBadgedProject.Color = Color.Red;
            legend.CustomItems.Add(ltBadgedProject);

        }

        private void UpdateChartTitle()
        {
            System.Web.UI.DataVisualization.Charting.Title title = new System.Web.UI.DataVisualization.Charting.Title();
            title.Text = title.ToolTip = "Badged Resources by Time";
            chartReport.Titles.Clear();
            chartReport.Titles.Add(title);
            chartReport.Titles[0].Font = new System.Drawing.Font("Arial", 16, FontStyle.Bold);
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

        private void SaveFilterValuesForSession()
        {
            ResourceFilters filter = new ResourceFilters();
            filter.PersonStatusIds = PersonStatus;
            filter.PayTypeIds = PayTypes;
            filter.SelectedView = SelectedView;
            filter.ReportStartDate = dtpStart.DateValue;
            filter.ReportEndDate = dtpEnd.DateValue;
            ReportsFilterHelper.SaveFilterValues(ReportName.BadgeResourceByTime, filter);
        }

        private void GetFilterValuesForSession()
        {
            var filters = ReportsFilterHelper.GetFilterValues(ReportName.BadgeResourceByTime) as ResourceFilters;
            if (filters != null)
            {
                cblPayTypes.UnSelectAll();
                cblPayTypes.SelectedItems=filters.PayTypeIds;
                cblPersonStatus.UnSelectAll();
                cblPersonStatus.SelectedItems=filters.PersonStatusIds;
                ddlView.SelectedValue = (filters.SelectedView).ToString();
                dtpStart.DateValue = (DateTime)filters.ReportStartDate;
                dtpEnd.DateValue = (DateTime)filters.ReportEndDate;
                PopulateChart();
                PopulateTable();
            }
        }
    }

    public class ReportTable
    {
        public string Category
        {
            get;
            set;
        }

        public List<BadgedResourcesByTime> Count
        {
            get;
            set;
        }
    }
}

