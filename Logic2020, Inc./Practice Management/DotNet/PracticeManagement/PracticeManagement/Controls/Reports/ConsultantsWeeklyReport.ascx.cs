using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using DataTransferObjects;
using DataTransferObjects.Reports;
using PraticeManagement.Configuration.ConsReportColoring;
using PraticeManagement.FilterObjects;
using PraticeManagement.Objects;
using PraticeManagement.Utils;
using System.Web.Security;
using System.Web;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing.Imaging;
using PraticeManagement.Utils.Excel;
using System.Data;
using DataTransferObjects.Filters;

namespace PraticeManagement.Controls.Reports
{
    public partial class ConsultantsWeeklyReport : UserControl
    {
        #region Constants

        private const string PERSON_TOOLTIP_FORMAT = "{0}, Hired {1}";
        private const string NOT_HIRED_PERSON_TOOLTIP_FORMAT = "{0}";
        private const string WEEKS_SERIES_NAME = "Weeks";
        private const string MAIN_CHART_AREA_NAME = "MainArea";
        private const int DAYS_FORWARD = 184;
        private const int DEFAULT_STEP = 7;
        private const string NAME_FORMAT = "{0}, {1} ({2})";
        private const string NAME_FORMAT_WITH_DATES = "{0}, {1} ({2}): {3}-{4}";
        private const string TITLE_FORMAT_ForPdf = "Consulting {0} Report \n{1} to {2}\nFor {3} Persons; For {4} Projects\n{5}\n{6}\n{7}\n*{0} reflects person vacation time during this period.";
        private const string TITLE_FORMAT_WITHOUT_REPORT_ForPdf = "Consulting {0} \n{1} to {2}\nFor {3} Persons; For {4} Projects\n{5}\n{6}\n{7}\n*{0} reflects person vacation time during this period.";
        private const string TITLE_FORMAT = "Consulting {0} Report \n{1} to {2}\nFor {3} Persons; For {4} Projects\n{5}\n{6}\n{7}\n*{0} reflects person vacation time during this period.\nClick on a colored bar to load the individual's detail report";
        private const string TITLE_FORMAT_WITHOUT_REPORT = "Consulting {0} \n{1} to {2}\nFor {3} Persons; For {4} Projects\n{5}\n{6}\n{7}\n*{0} reflects person vacation time during this period.\nClick on a colored bar to load the individual's detail report";
        private const string POSTBACK_FORMAT = "{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}";
        private const char DELIMITER = '+';
        private const string TOOLTIP_FORMAT = "{0}-{1} {2},{3}";
        private const string TOOLTIP_FORMAT_FOR_SINGLEDAY = "{0} {1},{2}";
        private const string FULL_MONTH_NAME_FORMAT = "MMMM, yyyy";
        private const string VACATION_TOOLTIP_FORMAT = "On Vacation: {0}";
        private const string UTILIZATION_TOOLTIP_FORMAT = "U% = {0}";
        private const string CAPACITY_TOOLTIP_FORMAT = "C% = {0}";
        private const string AVERAGE_UTIL_FORMAT = "~{0}%";
        private const string VACATION_AVERAGE_UTIL_FORMAT = "~{0}%*";
        private const string Utilization = "Utilization";
        private const string Capacity = "Capacity";
        private const string NEGATIVE_AVERAGE_UTIL_FORMAT = "~({0})%";
        private const string VACATION_NEGATIVE_AVERAGE_UTIL_FORMAT = "~({0})%*";
        private const string COMPANYHOLIDAYS_KEY = "CompanyHolidays_Key";
        private const string TIMEOFFDATES_KEY = "TIMEOFFDATES_KEY";
        private const string CONSULTANTUTILIZATIONPERSON_KEY = "ConsultantUtilizationPerson_Key";
        private const string INVESTMENTRESOURCE_KEY = "InvestmentResource_Key";
        private const string NONINVESTMENTRESOURCE_KEY = "NonInvestmentResource_key";
        private const string PageCount = "Page {0} of {1}";
        private const int reportSize = 25;
        private const string ConsultantCapacityReport = "Consulting Capacity Report";
        private const string ConsultantUtilizationReport = "Consulting Utilization Report";
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;
        private const string ConsultingHeader = "For {0} Persons; For {1} Projects\n{2}\n{3}\n{4}\n* Utilization reflects person vacation time during this period.";
        private const int maxWidth = 15;

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

                CellStyles dataCellStyle = new CellStyles();

                CellStyles percentageCellStyle = new CellStyles();
                percentageCellStyle.DataFormat = "0%";
                percentageCellStyle.WrapText = true;
                CellStyles[] dataCellStylearray = { dataCellStyle, 
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    percentageCellStyle
                                                  };
                List<int> coloumnWidth = new List<int>();
                for (int i = 0; i < 3; i++)
                    coloumnWidth.Add(0);
                RowStyles datarowStyle = new RowStyles(dataCellStylearray);
                RowStyles[] rowStylearray = { headerrowStyle };

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

        private SheetStyles DataSheetStyleForHours
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

                CellStyles dataCellStyle = new CellStyles();

                CellStyles percentageCellStyle = new CellStyles();
                percentageCellStyle.DataFormat = "0%";
                percentageCellStyle.WrapText = true;

                CellStyles hoursCellStyle = new CellStyles();
                hoursCellStyle.DataFormat = "0.00";
                hoursCellStyle.WrapText = true;

                List<CellStyles> datacells = new List<CellStyles>();
                for (int i = 0; i < 3; i++)
                {
                    datacells.Add(dataCellStyle);
                }
                for (int i = 3; i < coloumnsCount; i++)
                {
                    datacells.Add(hoursCellStyle);
                }
                //datacells.Add(percentageCellStyle);
                var dataCellStylearray = datacells.ToArray();
                RowStyles datarowStyle = new RowStyles(dataCellStylearray);
                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.TopRowNo = headerRowsCount;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanColSplit = 0;
                sheetStyle.FreezePanRowSplit = headerRowsCount;
                sheetStyle.IsAutoResize = true;
                return sheetStyle;
            }
        }

        public List<int> Heights
        {
            get;
            set;
        }

        #endregion Constants

        #region Fields

        private int _personsCount;
        private int _investmentPersonCount;
        private bool? _userIsAdministratorValue;
        private int page;
        /// <summary>
        /// 	Report's 'step' in days
        /// </summary>
        private string TimescaleIds
        {
            get
            {
                return utf.TimescalesSelected;
            }
        }

        /// <summary>
        /// 	Report's 'step' in days
        /// </summary>
        private string PracticeIdList
        {
            get
            {
                return utf.PracticesSelected;
            }
        }

        private string DivisionIdList
        {
            get
            {
                return utf.DivisionsSelected;
            }
        }

        /// <summary>
        /// 	Report's 'step' in days
        /// </summary>
        private int Granularity
        {
            get
            {
                return utf.Granularity;
            }
        }

        /// <summary>
        /// 	Report's 'step' in days
        /// </summary>
        private int AvgUtil
        {
            get
            {
                return utf.AvgUtil;
            }
        }

        /// <summary>
        /// 	Period to generate report to in days
        /// </summary>
        private int Period
        {
            get
            {
                return utf.Period;
            }
        }

        /// <summary>
        /// 	Chart's week series
        /// </summary>
        public Series WeeksSeries
        {
            get { return chart.Series[WEEKS_SERIES_NAME]; }
        }

        public Series WeeksSeriesPdf
        {
            get { return chartPdf.Series[WEEKS_SERIES_NAME]; }
        }

        public Series WeeksSeriesInvestment
        {
            get { return investmentChart.Series[WEEKS_SERIES_NAME]; }
        }


        /// <summary>
        /// 	Report's start date
        /// </summary>
        private DateTime BegPeriod
        {
            get
            {
                return utf.BegPeriod;
            }
        }

        /// <summary>
        /// 	Report's end date
        /// </summary>
        private DateTime EndPeriod
        {
            get
            {
                return utf.EndPeriod;
            }
        }

        private int SortId
        {
            get
            {
                return utf.SortId;
            }
        }

        private string SortDirection
        {
            get
            {
                return utf.SortDirection;
            }
        }

        public bool IsSampleReport
        {
            get
            {
                return hdnIsSampleReport.Value.ToLowerInvariant() == "true" ? true : false;
            }
            set
            {
                hdnIsSampleReport.Value = value.ToString();
            }
        }

        public bool IsCapacityMode
        {
            get
            {
                if (ViewState["IsCapacityMode_Key"] == null)
                    ViewState["IsCapacityMode_Key"] = false;
                return (bool)ViewState["IsCapacityMode_Key"];
            }
            set { ViewState["IsCapacityMode_Key"] = value; }
        }

        public Dictionary<DateTime, string> CompanyHolidays
        {
            get { return ViewState[COMPANYHOLIDAYS_KEY] as Dictionary<DateTime, string>; }
            set { ViewState[COMPANYHOLIDAYS_KEY] = value; }
        }

        public Dictionary<DateTime, double> TimeOffDates
        {
            get { return ViewState[TIMEOFFDATES_KEY] as Dictionary<DateTime, double>; }
            set { ViewState[TIMEOFFDATES_KEY] = value; }
        }

        public List<ConsultantUtilizationPerson> ConsultantUtilizationPerson
        {
            get
            {
                if (ViewState[CONSULTANTUTILIZATIONPERSON_KEY] == null)
                    ViewState[CONSULTANTUTILIZATIONPERSON_KEY] = new List<ConsultantUtilizationPerson>();
                return ViewState[CONSULTANTUTILIZATIONPERSON_KEY] as List<ConsultantUtilizationPerson>;
            }
            set { ViewState[CONSULTANTUTILIZATIONPERSON_KEY] = value; }
        }

        public List<ConsultantUtilizationPerson> InvestmentResources
        {
            get
            {
                if (ViewState[INVESTMENTRESOURCE_KEY] == null)
                    ViewState[INVESTMENTRESOURCE_KEY] = new List<ConsultantUtilizationPerson>();
                return ViewState[INVESTMENTRESOURCE_KEY] as List<ConsultantUtilizationPerson>;
            }
            set { ViewState[INVESTMENTRESOURCE_KEY] = value; }
        }

        public List<ConsultantUtilizationPerson> NonInvestmentResources
        {
            get
            {
                if (ViewState[NONINVESTMENTRESOURCE_KEY] == null)
                    ViewState[NONINVESTMENTRESOURCE_KEY] = new List<ConsultantUtilizationPerson>();
                return ViewState[NONINVESTMENTRESOURCE_KEY] as List<ConsultantUtilizationPerson>;
            }
            set { ViewState[NONINVESTMENTRESOURCE_KEY] = value; }
        }

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

        public float ChartImageHeight { get; set; }

        public float ChartImageWidth { get; set; }

        public bool ExculdeInvestmentResources
        {
            get
            {
                return utf.ExcludeInvestmentResources;
            }
        }

        public bool isInvestmentPdfChart
        {
            get;
            set;
        }

        #endregion Fields

        protected void Page_PreRender(object sender, EventArgs e)
        {
            
            chart.ChartAreas[MAIN_CHART_AREA_NAME].InnerPlotPosition.X = 35;
            chart.ChartAreas[MAIN_CHART_AREA_NAME].InnerPlotPosition.Width = 50;
            if (chart.Height.Value > 500)
            {
                chart.ChartAreas[MAIN_CHART_AREA_NAME].InnerPlotPosition.Y = 3;
                chart.ChartAreas[MAIN_CHART_AREA_NAME].InnerPlotPosition.Height = 94;
            }
            else {
                chart.ChartAreas[MAIN_CHART_AREA_NAME].InnerPlotPosition.Y = 20;
                chart.ChartAreas[MAIN_CHART_AREA_NAME].InnerPlotPosition.Height = 60;
            }
            
            if (!ExculdeInvestmentResources)
            {
                investmentChart.Visible = false;
            }
            else
            {
                investmentChart.ChartAreas[MAIN_CHART_AREA_NAME].InnerPlotPosition.X = 35;
                investmentChart.ChartAreas[MAIN_CHART_AREA_NAME].InnerPlotPosition.Width = 50;
                investmentChart.ChartAreas[MAIN_CHART_AREA_NAME].InnerPlotPosition.Y = 20;
                investmentChart.ChartAreas[MAIN_CHART_AREA_NAME].InnerPlotPosition.Height = 60;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnExportToExcel.Enabled = !IsCapacityMode;
            }

            if (IsSampleReport)
            {
                utf.IsSampleReport = true;
                utf.PopulateControls();
                chart.Click -= Chart_Click;
                updPersonDetails.Visible = false;
                updFilters.Visible = false;
                this.UpdateReport();
                updConsReport.Update();
            }
            else
            {
                if (!IsPostBack)
                {
                    if (IsCapacityMode)
                    {
                        utf.IsCapacityMode = IsCapacityMode;
                    }

                    if (Request.QueryString[Constants.FilterKeys.ApplyFilterFromCookieKey] == "true")
                    {
                        var cookie = SerializationHelper.DeserializeCookie(IsCapacityMode ? Constants.FilterKeys.ConsultingCapacityFilterCookie : Constants.FilterKeys.ConsultantUtilTimeLineFilterCookie) as ConsultantUtilTimeLineFilter;
                        if (cookie != null)
                        {
                            PopulateControlsFromCookie(cookie);
                        }
                    }
                    else
                    {
                        chart.CssClass = "hide";
                    }
                    GetFilterValuesForSession();
                }
            }
        }

        private void PopulateControlsFromCookie(ConsultantUtilTimeLineFilter cookie)
        {
            utf.PopulateControls(cookie);
            uaeDetails.EnableClientState = true;
            this.UpdateReport();
            this.hdnIsChartRenderedFirst.Value = "true";
            updConsReport.Update();

            if (cookie.PersonId.HasValue)
            {
                ShowDetailedReport(cookie.PersonId.Value, cookie.BegPeriod.Date, cookie.EndPeriod.Date, cookie.ChartTitle,
                    cookie.ActiveProjects, cookie.ProjectedProjects, cookie.InternalProjects, cookie.ExperimentalProjects, cookie.ProposedProjects, cookie.CompletedProjects);

                System.Web.UI.ScriptManager.RegisterStartupScript(updConsReport, updConsReport.GetType(), "focusDetailReport", "window.location='#details';", true);
            }
        }

        public void UpdateReport()
        {
            InitChart();

            var report =
                DataHelper.GetConsultantsWeeklyReport(
                    BegPeriod, Granularity, Period,
                    utf.ActivePersons, utf.ProjectedPersons,
                    utf.ActiveProjects, utf.ProjectedProjects,
                    utf.ExperimentalProjects,
                    utf.InternalProjects, utf.ProposedProjects, utf.CompletedProjects, TimescaleIds, PracticeIdList, AvgUtil, SortId, (IsCapacityMode && SortId == 0) ? (SortDirection == "Desc" ? "Asc" : "Desc") : SortDirection, utf.ExcludeInternalPractices, 0, utf.IncludeBadgeStatus, DivisionIdList);
            ConsultantUtilizationPerson = report;

            InvestmentResources = report.Where(r => r.Person.IsInvestmentResource).ToList();
            NonInvestmentResources = report.Where(r => !r.Person.IsInvestmentResource).ToList();

            if (ExculdeInvestmentResources)
            {
                foreach (var quadruple in InvestmentResources)
                {
                    AddPerson(quadruple);
                }
                foreach (var quadruple in NonInvestmentResources)
                {
                    AddPerson(quadruple);
                }
                chart.Height = Resources.Controls.TimelineGeneralHeaderHeigth +
                           Resources.Controls.TimelineGeneralItemHeigth * NonInvestmentResources.Count +
                           Resources.Controls.TimelineGeneralFooterHeigth;
                investmentChart.Height = Resources.Controls.TimelineGeneralHeaderHeigth +
                           Resources.Controls.TimelineGeneralItemHeigth * InvestmentResources.Count +
                           Resources.Controls.TimelineGeneralFooterHeigth + 1;

            }
            else
            {
                foreach (var quadruple in report)
                    AddPerson(quadruple);
                chart.Height = Resources.Controls.TimelineGeneralHeaderHeigth +
                           Resources.Controls.TimelineGeneralItemHeigth * report.Count +
                           Resources.Controls.TimelineGeneralFooterHeigth;
            }

            if (!IsSampleReport)
            {
                SaveFilterValuesForSession();
            }
            if (InvestmentResources.Count > 0)
            {
                investmentChart.Visible = true;
                emptyInvestment.Visible = false;
            }
            else
            {
                investmentChart.Visible = false;
                emptyInvestment.Visible = true;
            }
            if (NonInvestmentResources.Count > 0)
            {
                chart.Visible = true;
                nonInv.Visible = false;
                //nonInv.Style.Add("display", "none");
            }
            else
            {
                chart.Visible = false;
                nonInv.Visible = true;
                nonInv.Style.Add("display", "inline");
            }
        }

        /// <summary>
        /// 	Init axises, title and legends
        /// </summary>
        private void InitChart(bool isPdf = false, int pageNumber = 0)
        {
            if (isPdf)
            {
                SetFont();
                InitAxis(chartPdf.ChartAreas[MAIN_CHART_AREA_NAME].AxisY);
                InitAxis(chartPdf.ChartAreas[MAIN_CHART_AREA_NAME].AxisY2);
            }
            if (ExculdeInvestmentResources)
            {
                InitAxis(investmentChart.ChartAreas[MAIN_CHART_AREA_NAME].AxisY);
                InitAxis(investmentChart.ChartAreas[MAIN_CHART_AREA_NAME].AxisY2);
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

        /// <summary>
        /// 	Apply color coding to all legends
        /// </summary>
        private void InitLegends(bool isPdf = false)
        {
            chart.Legends.Clear();
            investmentChart.Legends.Clear();
            chartPdf.Legends.Clear();
            var legendTop = new Legend()
            {
                DockedToChartArea = MAIN_CHART_AREA_NAME,
                Docking = Docking.Top,
                Alignment = StringAlignment.Center,
                IsDockedInsideChartArea = false,
                LegendStyle = LegendStyle.Table

            };
            var legendBtm = new Legend()
            {
                DockedToChartArea = MAIN_CHART_AREA_NAME,
                Docking = Docking.Bottom,
                Alignment = StringAlignment.Center,
                IsDockedInsideChartArea = false,
                LegendStyle = LegendStyle.Table
            };

            var InvestmentResourceLegend = new Legend("InvestmentResourcesLegend")
            {
                Alignment = StringAlignment.Center,
                IsDockedInsideChartArea = false,
                DockedToChartArea = MAIN_CHART_AREA_NAME,
                Docking = ExculdeInvestmentResources ? Docking.Top : Docking.Bottom,
                LegendStyle = LegendStyle.Table
            };
            if (isPdf)
            {
                chartPdf.Legends.Add(legendTop);
                chartPdf.Legends.Add(ExculdeInvestmentResources ? legendBtm : InvestmentResourceLegend);

            }
            chart.Legends.Add(legendTop);
            if (!ExculdeInvestmentResources)
            {
                chart.Legends.Add(InvestmentResourceLegend);
            }
            else
            {
                investmentChart.Legends.Add(InvestmentResourceLegend);
            }

            LegendCollection lcollection = isPdf ? chartPdf.Legends : chart.Legends;
            foreach (var legend in lcollection)
            {
                if (IsCapacityMode)
                {
                    Coloring.CapacityColorLegends(legend, isInvestmentPdfChart);
                }
                else
                {
                    Coloring.ColorLegend(legend, utf.IncludeBadgeStatus, isPdf, isInvestmentPdfChart);
                }
            }
            if (ExculdeInvestmentResources)
            {
                if (IsCapacityMode)
                {
                    Coloring.CapacityColorLegends(investmentChart.Legends[0], isInvestmentPdfChart);
                }
                else
                {
                    Coloring.ColorLegend(investmentChart.Legends[0], utf.IncludeBadgeStatus, isPdf, true);
                }
            }
        }

        /// <summary>
        /// 	Sets min/max values, adds long date names
        /// </summary>
        /// <param name = "horizAxis">Axis to decorate</param>
        private void InitAxis(Axis horizAxis)
        {
            var beginPeriodLocal = BegPeriod;
            var endPeriodLocal = EndPeriod;
            if (Granularity == 7)
            {
                if ((int)BegPeriod.DayOfWeek > 0)
                {
                    beginPeriodLocal = BegPeriod.AddDays(-1 * ((int)BegPeriod.DayOfWeek));
                }
                if ((int)EndPeriod.DayOfWeek < 6)
                {
                    endPeriodLocal = EndPeriod.AddDays(6 - ((int)EndPeriod.DayOfWeek));
                }
            }
            else if (Granularity == 30)
            {
                beginPeriodLocal = BegPeriod;
                endPeriodLocal = EndPeriod;

                if ((int)beginPeriodLocal.DayOfWeek > 0)
                {
                    beginPeriodLocal = beginPeriodLocal.AddDays(-1 * ((int)beginPeriodLocal.DayOfWeek));
                }
                if ((int)endPeriodLocal.DayOfWeek < 6)
                {
                    endPeriodLocal = endPeriodLocal.AddDays(6 - ((int)endPeriodLocal.DayOfWeek));
                }
            }
            //  Set min and max values
            horizAxis.Minimum = beginPeriodLocal.ToOADate();
            horizAxis.Maximum = endPeriodLocal.AddDays(1).ToOADate();
            horizAxis.IsLabelAutoFit = true;
            horizAxis.IsStartedFromZero = true;

            if (utf.IsshowTodayBar)
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

            if (utf.DetalizationSelectedValue == "1")
            {
                horizAxis.IntervalType = DateTimeIntervalType.Weeks;
                horizAxis.Interval = 1;

                horizAxis.IntervalOffset = GetOffset(BegPeriod);
                horizAxis.IntervalOffsetType = DateTimeIntervalType.Days;
            }
            else if (utf.DetalizationSelectedValue == "7")
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
            // Add month names
            var diff = EndPeriod.Subtract(BegPeriod);
            if (diff.Days > 31)
            {
                for (var i = 0; i <= diff.Days / 31; i++)
                {
                    var currMonth = BegPeriod.AddMonths(i);
                    horizAxis.CustomLabels.Add(
                        currMonth.ToOADate(),
                        BegPeriod.AddMonths(i + 1).ToOADate(),
                        currMonth.ToString(FULL_MONTH_NAME_FORMAT),
                        1,
                        LabelMarkStyle.None);
                }
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

        /// <summary>
        /// 	Format chart title according to
        /// 	period and granularity selected
        /// </summary>
        private void UpdateChartTitle(bool isPdf, int pageNumber)
        {
            //  Add chart title
            string personsPlaceHolder = string.Empty, projectsPlaceHolder = string.Empty, practicesPlaceHolder = string.Empty;
            if (utf.ProjectedPersons && utf.ActivePersons)
            {
                personsPlaceHolder = "All";
            }
            else if (utf.ActivePersons)
            {
                personsPlaceHolder = "Active";
            }
            else if (utf.ProjectedPersons)
            {
                personsPlaceHolder = "Projected";
            }
            else
            {
                personsPlaceHolder = "No";
            }

            if (utf.ActiveProjects && utf.ProjectedProjects
                && utf.InternalProjects && utf.ExperimentalProjects && utf.ProposedProjects)
            {
                projectsPlaceHolder = "All";
            }
            else
            {
                if (utf.ActiveProjects)
                    projectsPlaceHolder = "Active";

                if (utf.ProjectedProjects)
                {
                    if (string.IsNullOrEmpty(projectsPlaceHolder))
                    {
                        projectsPlaceHolder = "Projected";
                    }
                    else
                    {
                        projectsPlaceHolder += "/Projected";
                    }
                }
                if (utf.CompletedProjects)
                {
                    if (string.IsNullOrEmpty(projectsPlaceHolder))
                    {
                        projectsPlaceHolder = "Completed";
                    }
                    else
                    {
                        projectsPlaceHolder += "/Completed";
                    }
                }
                if (utf.InternalProjects)
                {
                    if (string.IsNullOrEmpty(projectsPlaceHolder))
                    {
                        projectsPlaceHolder = "Internal";
                    }
                    else
                    {
                        projectsPlaceHolder += "/Internal";
                    }
                }
                if (utf.ExperimentalProjects)
                {
                    if (string.IsNullOrEmpty(projectsPlaceHolder))
                    {
                        projectsPlaceHolder = "Experimental";
                    }
                    else
                    {
                        projectsPlaceHolder += "/Experimental";
                    }
                }
                if (utf.ProposedProjects)
                {
                    if (string.IsNullOrEmpty(projectsPlaceHolder))
                    {
                        projectsPlaceHolder = "Proposed";
                    }
                    else
                    {
                        projectsPlaceHolder += "/Proposed";
                    }
                }
            }
            if (string.IsNullOrEmpty(projectsPlaceHolder))
            {
                projectsPlaceHolder = "No";
            }
            if (isPdf)
            {
                System.Web.UI.DataVisualization.Charting.Title title_top = new System.Web.UI.DataVisualization.Charting.Title(string.Format(
                    IsCapacityMode ? TITLE_FORMAT_WITHOUT_REPORT_ForPdf : TITLE_FORMAT_ForPdf,
                    IsCapacityMode ? Capacity : Utilization,
                    BegPeriod.ToString("MM/dd/yyyy"),
                    EndPeriod.ToString("MM/dd/yyyy"),
                    personsPlaceHolder, projectsPlaceHolder, utf.PracticesFilterText(), isInvestmentPdfChart ? "Investment Resources" : "Non-Investment Resources", utf.DivisionsFilterText()));
                title_top.Font = new System.Drawing.Font("Candara", 9f);

                chartPdf.Titles.Add(title_top);

                System.Web.UI.DataVisualization.Charting.Title title_bottom = new System.Web.UI.DataVisualization.Charting.Title(string.Format(PageCount, pageNumber + 1, Math.Ceiling((double)ConsultantUtilizationPerson.Count / reportSize)));
                title_bottom.Alignment = ContentAlignment.BottomRight;
                title_bottom.Docking = Docking.Bottom;
                title_bottom.Font = new System.Drawing.Font("Candara", 9f);
                chartPdf.Titles.Add(title_bottom);
            }
            else
            {
                chart.Titles.Add(
                    string.Format(
                        IsCapacityMode ? TITLE_FORMAT_WITHOUT_REPORT : TITLE_FORMAT,
                        IsCapacityMode ? Capacity : Utilization,
                        BegPeriod.ToString("MM/dd/yyyy"),
                        EndPeriod.ToString("MM/dd/yyyy"),
                        personsPlaceHolder, projectsPlaceHolder, utf.PracticesFilterText(), utf.InvestmentResourceFilterText(), utf.DivisionsFilterText()));
            }
            if (ExculdeInvestmentResources)
            {
                investmentChart.Titles.Add("Investment Resources");
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(IsCapacityMode ? ConsultantCapacityReport : ConsultantUtilizationReport);
            PDFExport();
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            mpeExportOptions.Show();
        }

        protected void btnOkExport_Click(object sender, EventArgs e)
        {
            int optionNumber = rbUtilization.Checked ? 0 : rbProjectUtilization.Checked ? 1 : 2;
            Export(optionNumber);
        }

        private void Export(int optionNumber)
        {
            var filename = string.Format(optionNumber == 0 ? "ConsultingUtilization_UtilizationOnly_{0}-{1}.xls" : optionNumber == 1 ? "ConsultingUtilization_ProjectUtilization_{0}-{1}.xls" : "ConsultingUtilization_Hours_{0}-{1}.xls", BegPeriod.ToString("MM_dd_yyyy"), EndPeriod.ToString("MM_dd_yyyy"));
            DataHelper.InsertExportActivityLogMessage(ConsultantUtilizationReport);
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();

            var report = DataHelper.GetConsultantsWeeklyReport(
                    BegPeriod, Granularity, Period,
                    utf.ActivePersons, utf.ProjectedPersons,
                    utf.ActiveProjects, utf.ProjectedProjects,
                    utf.ExperimentalProjects,
                    utf.InternalProjects, utf.ProposedProjects, utf.CompletedProjects, TimescaleIds, PracticeIdList, AvgUtil, SortId, (IsCapacityMode && SortId == 0) ? (SortDirection == "Desc" ? "Desc" : "Asc") : SortDirection, utf.ExcludeInternalPractices, optionNumber == 2 ? 0 : optionNumber, false, DivisionIdList);
            report.Reverse();
            string personsPlaceHolder = string.Empty, projectsPlaceHolder = string.Empty, practicesPlaceHolder = string.Empty;
            if (utf.ProjectedPersons && utf.ActivePersons)
            {
                personsPlaceHolder = "All";
            }
            else if (utf.ActivePersons)
            {
                personsPlaceHolder = "Active";
            }
            else if (utf.ProjectedPersons)
            {
                personsPlaceHolder = "Projected";
            }
            else
            {
                personsPlaceHolder = "No";
            }

            if (utf.ActiveProjects && utf.ProjectedProjects
                && utf.InternalProjects && utf.ExperimentalProjects && utf.ProposedProjects)
            {
                projectsPlaceHolder = "All";
            }
            else
            {
                if (utf.ActiveProjects)
                    projectsPlaceHolder = "Active";

                if (utf.ProjectedProjects)
                {
                    if (string.IsNullOrEmpty(projectsPlaceHolder))
                    {
                        projectsPlaceHolder = "Projected";
                    }
                    else
                    {
                        projectsPlaceHolder += "/Projected";
                    }
                }
                if (utf.CompletedProjects)
                {
                    if (string.IsNullOrEmpty(projectsPlaceHolder))
                    {
                        projectsPlaceHolder = "Completed";
                    }
                    else
                    {
                        projectsPlaceHolder += "/Completed";
                    }
                }
                if (utf.InternalProjects)
                {
                    if (string.IsNullOrEmpty(projectsPlaceHolder))
                    {
                        projectsPlaceHolder = "Internal";
                    }
                    else
                    {
                        projectsPlaceHolder += "/Internal";
                    }
                }
                if (utf.ExperimentalProjects)
                {
                    if (string.IsNullOrEmpty(projectsPlaceHolder))
                    {
                        projectsPlaceHolder = "Experimental";
                    }
                    else
                    {
                        projectsPlaceHolder += "/Experimental";
                    }
                }
                if (utf.ProposedProjects)
                {
                    if (string.IsNullOrEmpty(projectsPlaceHolder))
                    {
                        projectsPlaceHolder = "Proposed";
                    }
                    else
                    {
                        projectsPlaceHolder += "/Proposed";
                    }
                }
            }
            if (string.IsNullOrEmpty(projectsPlaceHolder))
            {
                projectsPlaceHolder = "No";
            }

            if (report.Count > 0)
            {
                DataTable header1 = new DataTable();
                header1.Columns.Add(string.Format("Period: {0}-{1}", BegPeriod.ToString("MM/dd/yyyy"), EndPeriod.ToString("MM/dd/yyyy")));

                List<object> row1 = new List<object>();
                row1.Add(string.Format(ConsultingHeader, personsPlaceHolder, projectsPlaceHolder, utf.PracticesFilterText(), utf.InvestmentResourceFilterText(), utf.DivisionsFilterText()));
                header1.Rows.Add(row1.ToArray());
                headerRowsCount = header1.Rows.Count + 3;

                DataTable headerForInvestment = new DataTable();
                headerForInvestment.Columns.Add(string.Format("Period: {0}-{1}", BegPeriod.ToString("MM/dd/yyyy"), EndPeriod.ToString("MM/dd/yyyy")));
                List<object> row2 = new List<object>();
                row2.Add(string.Format(ConsultingHeader, personsPlaceHolder, projectsPlaceHolder, utf.PracticesFilterText(), utf.InvestmentResourceFilterText(), utf.DivisionsFilterText()));
                headerForInvestment.Rows.Add(row2.ToArray());

                DataTable NonInvData, InvData, data;

                if (!ExculdeInvestmentResources)
                {
                    data = PrepareDataTable(report, optionNumber, true);
                    coloumnsCount = data.Columns.Count;
                    var dataset = new DataSet();
                    dataset.DataSetName = "Consulting_Utilization";
                    dataset.Tables.Add(header1);
                    dataset.Tables.Add(data);
                    dataSetList.Add(dataset);
                }
                else
                {
                    var nonInvResources = report.Where(p => p.Person.IsInvestmentResource == false).ToList();
                    var InvResources = report.Where(p => p.Person.IsInvestmentResource == true).ToList();
                    NonInvData = PrepareDataTable(nonInvResources, optionNumber);
                    InvData = PrepareDataTable(InvResources, optionNumber, true);
                    coloumnsCount = NonInvData.Columns.Count;
                    var NonInvDataset = new DataSet();
                    NonInvDataset.DataSetName = "Non-Investment Resources";
                    NonInvDataset.Tables.Add(header1);
                    NonInvDataset.Tables.Add(NonInvData);
                    dataSetList.Add(NonInvDataset);
                    var InvDataset = new DataSet();
                    InvDataset.DataSetName = "Investment Resources";
                    InvDataset.Tables.Add(headerForInvestment);
                    InvDataset.Tables.Add(InvData);
                    dataSetList.Add(InvDataset);
                }

                sheetStylesList.Add(HeaderSheetStyle);
                if (optionNumber == 1)
                {
                    var dataStyle = DataSheetStyle;
                    var rowStylesList = dataStyle.rowStyles.ToList();
                    for (int i = 0; i < Heights.Count; i++)
                    {
                        CellStyles dataCellStyle = new CellStyles();

                        CellStyles percentageCellStyle = new CellStyles();
                        percentageCellStyle.DataFormat = "0%";
                        percentageCellStyle.WrapText = true;
                        CellStyles[] dataCellStylearray = { dataCellStyle, 
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    percentageCellStyle
                                                  };
                        var datarowStyle = new RowStyles(dataCellStylearray);
                        datarowStyle.Height = Heights[i] == 0 ? (short)300 : (short)(Heights[i] * 300);
                        rowStylesList.Add(datarowStyle);
                    }
                    dataStyle.rowStyles = rowStylesList.ToArray();
                    for (int i = 0; i < report[0].ProjectUtilization.Count; i++)
                        dataStyle.ColoumnWidths.Add(20);
                    sheetStylesList.Add(dataStyle);
                    if (ExculdeInvestmentResources)
                    {
                        coloumnsCount++;
                        sheetStylesList.Add(HeaderSheetStyle);
                        sheetStylesList.Add(dataStyle);
                    }
                }
                if (optionNumber == 2)
                {
                    sheetStylesList.Add(DataSheetStyleForHours);
                    if (ExculdeInvestmentResources)
                    {

                        CellStyles percentageCellStyle = new CellStyles();
                        percentageCellStyle.DataFormat = "0%";
                        percentageCellStyle.WrapText = true;
                        var datasheet = DataSheetStyleForHours;
                        var row = datasheet.rowStyles.ToList();
                        var cellStyle = row[1].cellStyles.ToList();
                        cellStyle.Add(percentageCellStyle);
                        RowStyles rowstyle = new RowStyles(cellStyle.ToArray());
                        datasheet.rowStyles[1] = rowstyle;
                        coloumnsCount++;
                        sheetStylesList.Add(HeaderSheetStyle);
                        sheetStylesList.Add(datasheet);
                    }
                }
                else
                {
                    var dataStyle = DataSheetStyle;
                    var rowStylesList = dataStyle.rowStyles.ToList();
                    CellStyles dataCellStyle = new CellStyles();

                    CellStyles percentageCellStyle = new CellStyles();
                    percentageCellStyle.DataFormat = "0%";
                    percentageCellStyle.WrapText = true;
                    CellStyles[] dataCellStylearray = { dataCellStyle, 
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    percentageCellStyle
                                                  };
                    RowStyles datarowStyle = new RowStyles(dataCellStylearray);
                    rowStylesList.Add(datarowStyle);
                    dataStyle.rowStyles = rowStylesList.ToArray();
                    sheetStylesList.Add(dataStyle);
                    if (ExculdeInvestmentResources)
                    {
                        coloumnsCount++;
                        sheetStylesList.Add(HeaderSheetStyle);
                        sheetStylesList.Add(dataStyle);
                    }
                }

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

        public DataTable PrepareDataTable(List<ConsultantUtilizationPerson> report, int optionNumber, bool includeTargetUtil = false)
        {
            DataTable data = new DataTable();
            List<object> row;
            data.Columns.Add("Person Name");
            data.Columns.Add("Title");
            data.Columns.Add("Pay type");
            if ((optionNumber == 0 || optionNumber == 2) && report.Count > 0)
            {
                for (int i = 0; i < report[0].WeeklyUtilization.Count; i++)
                {
                    var beginPeriod = BegPeriod;
                    var endPeriod = EndPeriod;

                    if (Granularity == 7)
                    {
                        if ((int)BegPeriod.DayOfWeek > 0)
                        {
                            beginPeriod = BegPeriod.AddDays(-1 * ((int)BegPeriod.DayOfWeek));
                        }
                        if ((int)EndPeriod.DayOfWeek < 6)
                        {
                            endPeriod = EndPeriod.AddDays(6 - ((int)EndPeriod.DayOfWeek));
                        }
                    }
                    else if (Granularity == 30)
                    {
                        beginPeriod = BegPeriod;
                        endPeriod = EndPeriod;

                        if ((int)beginPeriod.DayOfWeek > 0)
                        {
                            beginPeriod = beginPeriod.AddDays(-1 * ((int)beginPeriod.DayOfWeek));
                        }
                        if ((int)endPeriod.DayOfWeek < 6)
                        {
                            endPeriod = endPeriod.AddDays(6 - ((int)endPeriod.DayOfWeek));
                        }
                    }
                    var period = (endPeriod.Subtract(beginPeriod).Days + 1);
                    var pointStartDate = beginPeriod.AddDays(i * Granularity);
                    var pointEndDate = beginPeriod.AddDays(((i + 1) * Granularity));
                    if (Granularity == 30)
                    {
                        pointStartDate = beginPeriod.AddMonths(i);
                        pointEndDate = endPeriod > beginPeriod.AddMonths(i + 1) ? beginPeriod.AddMonths(i + 1) : endPeriod.AddDays(1);
                    }
                    else
                    {
                        var delta = period - (i * Granularity - 1);
                        if (delta <= Granularity)
                        {
                            pointEndDate = beginPeriod.AddDays(period);
                        }
                    }
                    data.Columns.Add(pointStartDate.ToShortDateString());
                }
            }
            else if (optionNumber == 1 && report.Count > 0)
            {
                foreach (var item in report[0].ProjectUtilization)
                {
                    data.Columns.Add(item.StartDate.ToShortDateString());
                }
            }
            data.Columns.Add(optionNumber == 2 ? "Average Hours" : "Utilization %");
            if (includeTargetUtil)
            {
                data.Columns.Add("Target Utilization %");
            }
            if ((optionNumber == 0 || optionNumber == 2) && report.Count > 0)
            {
                foreach (var person in report)
                {
                    row = new List<object>();
                    row.Add(person.Person.PersonLastFirstName);
                    row.Add(person.Person.Title.TitleName);
                    row.Add(person.Person.CurrentPay.TimescaleName);
                    for (int i = 0; i < person.WeeklyUtilization.Count; i++)
                    {
                        var beginPeriod = BegPeriod;
                        var endPeriod = EndPeriod;

                        if (Granularity == 7)
                        {
                            if ((int)BegPeriod.DayOfWeek > 0)
                            {
                                beginPeriod = BegPeriod.AddDays(-1 * ((int)BegPeriod.DayOfWeek));
                            }
                            if ((int)EndPeriod.DayOfWeek < 6)
                            {
                                endPeriod = EndPeriod.AddDays(6 - ((int)EndPeriod.DayOfWeek));
                            }
                        }
                        else if (Granularity == 30)
                        {
                            beginPeriod = BegPeriod;
                            endPeriod = EndPeriod;

                            if ((int)beginPeriod.DayOfWeek > 0)
                            {
                                beginPeriod = beginPeriod.AddDays(-1 * ((int)beginPeriod.DayOfWeek));
                            }
                            if ((int)endPeriod.DayOfWeek < 6)
                            {
                                endPeriod = endPeriod.AddDays(6 - ((int)endPeriod.DayOfWeek));
                            }
                        }
                        var period = (endPeriod.Subtract(beginPeriod).Days + 1);
                        var pointStartDate = beginPeriod.AddDays(i * Granularity);
                        var pointEndDate = beginPeriod.AddDays(((i + 1) * Granularity));
                        if (Granularity == 30)
                        {
                            pointStartDate = beginPeriod.AddMonths(i);
                            pointEndDate = endPeriod > beginPeriod.AddMonths(i + 1) ? beginPeriod.AddMonths(i + 1) : endPeriod.AddDays(1);
                        }
                        else
                        {
                            var delta = period - (i * Granularity - 1);
                            if (delta <= Granularity)
                            {
                                pointEndDate = beginPeriod.AddDays(period);
                            }
                        }
                        if (person.WeeklyUtilization[i] == -1 && pointEndDate == pointStartDate.AddDays(1) && person.CompanyHolidayDates.Any(d => d.Key == pointStartDate))
                        {
                            var first = person.CompanyHolidayDates.FirstOrDefault(d => d.Key == pointStartDate);
                            row.Add(first.Value);
                        }
                        else if (person.WeeklyUtilization[i] == -1)
                        {
                            row.Add("On Vacation");
                        }
                        else
                        {
                            if (optionNumber == 0)
                                row.Add(((double)person.WeeklyUtilization[i] / 100).ToString());
                            else if (optionNumber == 2)
                                row.Add(Math.Round(person.ProjectedHoursList[i], 2));
                        }
                    }
                    if (optionNumber == 0)
                        row.Add(person.PersonVacationDays > 0 ? person.AverageUtilization.ToString() + "%*" : ((double)person.AverageUtilization / 100).ToString());
                    else if (optionNumber == 2)
                        row.Add(Math.Round((decimal)person.ProjectedHoursList.Sum() / person.ProjectedHoursList.Count, 2));
                    if (includeTargetUtil && person.Person.TargetUtilization != null)
                    {
                        row.Add(((double)person.Person.TargetUtilization / 100).ToString());
                    }
                    data.Rows.Add(row.ToArray());
                }
            }
            else if (optionNumber == 1 && report.Count > 0)
            {
                Heights = new List<int>();
                foreach (var person in report)
                {
                    int temp = 0;
                    row = new List<object>();
                    row.Add(person.Person.PersonLastFirstName);
                    row.Add(person.Person.Title.TitleName);
                    row.Add(person.Person.CurrentPay.TimescaleName);
                    foreach (var util in person.ProjectUtilization)
                    {
                        if (util.Format == "-1" && util.StartDate == util.EndDate && person.CompanyHolidayDates.Any(d => d.Key == util.StartDate))
                        {
                            var first = person.CompanyHolidayDates.FirstOrDefault(d => d.Key == util.StartDate);
                            row.Add(first.Value);
                        }
                        else if (util.Format == "-1")
                        {
                            row.Add("On Vacation");
                        }
                        else
                        {
                            row.Add(util.Format == "0" ? 0.ToString() : util.Format);
                            var value = (int)Math.Ceiling((decimal)util.Format.Length / maxWidth);
                            temp = value > temp ? value : temp;
                        }
                    }
                    Heights.Add(temp);
                    row.Add(person.PersonVacationDays > 0 ? person.AverageUtilization.ToString() + "%*" : ((double)person.AverageUtilization / 100).ToString()); //row.Add(person.AverageUtilization);
                    if (includeTargetUtil && person.Person.TargetUtilization != null)
                    {
                        row.Add(((double)person.Person.TargetUtilization / 100).ToString());
                    }
                    data.Rows.Add(row.ToArray());
                }
            }
            return data;
        }

        protected void Chart_Click(object sender, ImageMapEventArgs e)
        {
            //UpdateReport();
            var query = e.PostBackValue.Split(DELIMITER);

            // Exctract data from query
            var personId = int.Parse(query[0]);
            var repStartDate = DateTime.Parse(query[1]);
            var repEndDate = DateTime.Parse(query[2]);
            var activeProjects = bool.Parse(query[4]);
            var projectedProjects = bool.Parse(query[5]);
            var internalProjects = bool.Parse(query[6]);
            var experimentalProjects = bool.Parse(query[7]);
            var proposedProjects = bool.Parse(query[8]);
            var completedProjects = bool.Parse(query[9]);

            ShowDetailedReport(personId, repStartDate, repEndDate, query[3],
            activeProjects, projectedProjects, internalProjects, experimentalProjects, proposedProjects, completedProjects);

            System.Web.UI.ScriptManager.RegisterClientScriptBlock(updConsReport, updConsReport.GetType(), "focusDetailReport", "window.location='#details';", true);

            SaveFilters(personId, query[3]);
            hdnpopup.Value = "true";
            mpeConsultantDetailReport.Show();
        }

        private void SaveFilters(int? personId, string chartTitle)
        {
            var filter = utf.SaveFilters(personId, chartTitle);
            SerializationHelper.SerializeCookie(filter, IsCapacityMode ? Constants.FilterKeys.ConsultingCapacityFilterCookie : Constants.FilterKeys.ConsultantUtilTimeLineFilterCookie);
        }

        private void ShowDetailedReport(int personId, DateTime repStartDate, DateTime repEndDate, string chartTitle,
            bool activeProjects, bool projectedProjects, bool internalProjects, bool experimentalProjects, bool proposedProjects, bool completedProjects)
        {
            chartDetails.Visible = true;

            chartDetails.Titles["MilestonesTitle"].Text = chartTitle;
            var points = chartDetails.Series["Milestones"].Points;
            points.Clear();

            // Get report
            var bars =
                DataHelper.GetMilestonePersons(
                    personId,
                    repStartDate,
                    repEndDate,
                    activeProjects,
                    projectedProjects,
                    internalProjects,
                    experimentalProjects,
                    proposedProjects,
                    completedProjects,
                    IsCapacityMode);

            var utilizationDaily = DataHelper.ConsultantUtilizationDailyByPerson(repStartDate, ParseInt(repEndDate.Subtract(repStartDate).Days.ToString(), DAYS_FORWARD),
                utf.ActiveProjects, utf.ProjectedProjects, utf.InternalProjects, utf.ExperimentalProjects, utf.ProposedProjects, utf.CompletedProjects, personId);
            var avgUtils = utilizationDaily.First().WeeklyUtilization;
            for (int index = 0; index < avgUtils.Count; index++)
            {
                var pointStartDate = repStartDate.AddDays(index);
                var pointEndDate = repStartDate.AddDays(index + 1);
                int load = avgUtils[index];
                if (load < 0)
                {
                    var ind = chartDetails.Series["Milestones"].Points.AddXY(
                    1,
                    pointStartDate,
                    pointEndDate);

                    var range = chartDetails.Series["Milestones"].Points[ind];
                    range.Color = Coloring.GetColorByUtilization(load, load == -1 ? 2 : (load == -2 ? 1 : 0));
                    string holidayDescription = CompanyHolidays == null ? string.Empty : CompanyHolidays.Keys.Any(t => t == pointStartDate) ? CompanyHolidays[pointStartDate] : utilizationDaily.First().TimeOffDates.Keys.Any(t => t == pointStartDate) ? utilizationDaily.First().TimeOffDates[pointStartDate].ToString() : string.Empty;
                    range.ToolTip = FormatRangeTooltip(load, pointStartDate, pointEndDate.AddDays(-1), new BadgeType() { DayType = load == -1 ? 2 : (load == -2 ? 1 : 0) }, null, false, holidayDescription);
                }
            }

            var minDate = DateTime.MaxValue;
            var maxDate = DateTime.MinValue;
            for (int barIndex = 0; barIndex < bars.Count; barIndex++)
            {
                //  Add point to the plot
                var ptStart = bars[barIndex].StartDate;
                var ptEnd = bars[barIndex].EndDate.AddDays(1);
                var ind = points.AddXY(barIndex + 2, ptStart, ptEnd);
                var pt = points[ind];

                //  Mark projected projects
                switch (bars[barIndex].BarType)
                {
                    case DetailedUtilizationReportBaseItem.ItemType.ProjectedMilestone:
                    case DetailedUtilizationReportBaseItem.ItemType.ActiveMilestone:
                        pt.Color = ConsReportColoringElementSection.ColorSettings.MilestoneColor;
                        pt.BackGradientStyle = GradientStyle.TopBottom;
                        break;

                    case DetailedUtilizationReportBaseItem.ItemType.OpportunityGeneric:
                    case DetailedUtilizationReportBaseItem.ItemType.SendoutOpportunity:
                    case DetailedUtilizationReportBaseItem.ItemType.ProposeOpportunity:
                    case DetailedUtilizationReportBaseItem.ItemType.PipelineOpportunity:
                        pt.Color = ConsReportColoringElementSection.ColorSettings.OpportunityColor;
                        pt.BackGradientStyle = GradientStyle.Center;
                        break;
                }

                // Make it clickable
                pt.Url = bars[barIndex].NavigateUrl;
                // Set proper tooltip
                pt.ToolTip = bars[barIndex].Tooltip;

                // Set proper label and make it clickable
                pt.Label = bars[barIndex].Label;
                if (bars[barIndex] is DetailedUtilizationReportOpportunityItem)
                {
                    var opptItem = bars[barIndex] as DetailedUtilizationReportOpportunityItem;
                }
                else
                {
                    var opptItem = bars[barIndex] as DetailedUtilizationReportOpportunityItem;
                }
                pt.LabelUrl = pt.Url;
                pt.LabelToolTip = pt.ToolTip;

                // Find min and max dates
                if (minDate.CompareTo(ptStart) > 0) minDate = ptStart;
                if (maxDate.CompareTo(ptEnd) < 0) maxDate = ptEnd;
            }

            SetAxisMaxAndMin(repStartDate, repEndDate);

            chartDetails.Height = 2 * Resources.Controls.TimelineDetailedHeaderFooterHeigth +
                                  bars.Count * Resources.Controls.TimelineDetailedItemHeigth;
        }

        private void SetAxisMaxAndMin(DateTime minDate, DateTime maxDate)
        {
            var horizAxis = chartDetails.ChartAreas[0].AxisY;
            horizAxis.Minimum = minDate.ToOADate();
            horizAxis.Maximum = maxDate.ToOADate();
        }

        /// <summary>
        /// 	Add person to the graph.
        /// </summary>
        /// <param name = "triple">Person - loads per range - average u%</param>
        public void AddPerson(ConsultantUtilizationPerson quadruple, bool isPdf = false)
        {
            var partsCount = quadruple.WeeklyUtilization.Count;
            //var csv = FormCSV(quadruple.WeeklyUtilization.ToArray());
            for (var w = 0; w < partsCount; w++)
            {
                TimescaleType payType = (TimescaleType)quadruple.WeeklyPayTypes[w];
                //  Add another range to the person's timeline
                AddPersonRange(
                    quadruple.Person, //  Person
                     w, //  Range index
                     IsCapacityMode ? 100 - quadruple.WeeklyUtilization[w] : quadruple.WeeklyUtilization[w],
                     payType == TimescaleType.Undefined ? "No Pay Type" : DataHelper.GetDescription(payType), quadruple.WeeklyVacationDays[w], quadruple.TimeOffDates, quadruple.CompanyHolidayDates, isPdf, quadruple.Person.Projects
                     ); //  U% or C% for the period
            }

            //  Add axis label
            AddLabel(quadruple.Person, IsCapacityMode ? 100 - quadruple.AverageUtilization : quadruple.AverageUtilization, quadruple.PersonVacationDays, isPdf);

            //  Increase persons counter

            if (ExculdeInvestmentResources && quadruple.Person.IsInvestmentResource)
            {
                _investmentPersonCount++;
            }
            else
            {
                _personsCount++;
            }
        }

        private string FormCSV(int[] avgUtils)
        {
            StringBuilder sb = new StringBuilder(string.Empty);
            foreach (var avgUtil in avgUtils)
            {
                sb.Append("," + avgUtil.ToString());
            }

            return sb.ToString().Substring(1);
        }

        /// <summary>
        /// 	Adds label to the vertical axis
        /// </summary>
        /// <param name = "p">Person</param>
        /// <param name = "avg">Average load</param>
        private void AddLabel(Person p, int avg, int vacationDays, bool isPdf)
        {
            //  Get labels collection
            if (ExculdeInvestmentResources && !isPdf)
            {
                var NonInvestmentlabel1 = chart.ChartAreas[MAIN_CHART_AREA_NAME].AxisX.CustomLabels;
                var Investmentlabel1 = investmentChart.ChartAreas[MAIN_CHART_AREA_NAME].AxisX.CustomLabels;
                var NonInvestmentlabel2 = chart.ChartAreas[MAIN_CHART_AREA_NAME].AxisX2.CustomLabels;
                var Investmentlabel2 = investmentChart.ChartAreas[MAIN_CHART_AREA_NAME].AxisX2.CustomLabels;
                if (p.IsInvestmentResource)
                {

                    label(Investmentlabel1, Investmentlabel2, p, _investmentPersonCount, vacationDays, avg, true);
                }
                else
                {
                    label(NonInvestmentlabel1, NonInvestmentlabel2, p, _personsCount, vacationDays, avg);
                }
            }
            else if (isPdf)
            {
                var labels = chartPdf.ChartAreas[MAIN_CHART_AREA_NAME].AxisX.CustomLabels;
                var labels2 = chartPdf.ChartAreas[MAIN_CHART_AREA_NAME].AxisX2.CustomLabels;
                label(labels, labels2, p, isInvestmentPdfChart ? _investmentPersonCount : _personsCount, vacationDays, avg, isInvestmentPdfChart ? true : false, isPdf);
            }
            else
            {

                var chartLabel1 = chart.ChartAreas[MAIN_CHART_AREA_NAME].AxisX.CustomLabels;
                var chartLabel2 = chart.ChartAreas[MAIN_CHART_AREA_NAME].AxisX2.CustomLabels;
                label(chartLabel1, chartLabel2, p, _personsCount, vacationDays, avg);
            }
        }

        private void label(CustomLabelsCollection labelsX1, CustomLabelsCollection labelsX2, Person p, int count, int vacationDays, int avg, bool tag = false, bool isPdf = false)
        {
            var label = labelsX1.Add(
                     count - 0.49, // From position
                     count + 0.49, // To position
                     FormatPersonName(p), // Formated person title
                     0, // Index
                     LabelMarkStyle.SideMark);

            chart.ChartAreas[MAIN_CHART_AREA_NAME].AxisX.LabelStyle.Font = new System.Drawing.Font(FontFamily.GenericSansSerif, 10, p.IsInvestmentResource ? FontStyle.Italic : FontStyle.Regular);
            if (!IsSampleReport)
            {
                //  Url to person details page, return to report
                label.Url = getPersonUrl(p);
            }
            //  Tooltip
            //label.LabelMark = LabelMarkStyle.Box;
            label.ToolTip = string.Format(DateTime.MinValue != p.HireDate ? PERSON_TOOLTIP_FORMAT : NOT_HIRED_PERSON_TOOLTIP_FORMAT,
                                            p.CurrentPay.TimescaleName, // Current Pay Type
                                            p.HireDate.ToString("MM/dd/yyyy") // Hire date
                //,avg // Average U%
                    );
            string target = p.IsInvestmentResource && p.TargetUtilization != null ? (IsCapacityMode ? (100 - p.TargetUtilization).ToString() + "%" : p.TargetUtilization.ToString() + "%") : string.Empty;


            if (!ExculdeInvestmentResources && !isPdf)
            {
                if (count == ConsultantUtilizationPerson.Count - 1)
                {
                    labelsX2.Add(
                               count + 1 - 0.49, // From position
                               count + 1 + 0.49, // To position
                               string.Empty.PadRight(12) + (IsCapacityMode ? "Target Capacity" : "Target Utilization"),
                               0, // Index
                               LabelMarkStyle.None);
                }
            }
            else if (isPdf && !ExculdeInvestmentResources)
            {
                if (page == 0)
                {
                    labelsX2.Add(count + 25 - 0.49, // From position
                                       count + 25 + 0.49, // To position
                                       string.Empty.PadRight(12) + (IsCapacityMode ? "Target Capacity" : "Target Utilization"),
                                       0, // Index
                                       LabelMarkStyle.None);
                }
            }
            else
            {
                if (count == InvestmentResources.Count - 1 && tag)
                {
                    labelsX2.Add(
                               count + 1 - 0.49, // From position
                               count + 1 + 0.49, // To position
                               string.Empty.PadRight(12) + (IsCapacityMode ? "Target Capacity" : "Target Utilization"),
                               0, // Index
                               LabelMarkStyle.None);
                }
            }

            string x = FormatAvgPercentage(vacationDays, avg);
            var labelx2 = labelsX2.Add(
                    count - 0.49, // From position
                    count + 0.49, // To position
                    FormatAvgPercentage(vacationDays, avg).PadRight(10 + 8 - x.Length) + target,
                    0, // Index
                    LabelMarkStyle.None);
            if (!ExculdeInvestmentResources)
            {
                label.ForeColor = p.IsInvestmentResource ? Color.Blue : Color.Black;
                labelx2.ForeColor = p.IsInvestmentResource ? Color.Blue : Color.Black;
            }
        }

        private string getPersonUrl(Person p)
        {
            if (UserIsAdministrator)
            {
                return Urls.GetPersonDetailsUrl(p,
                       IsCapacityMode ? (Request.Url.AbsoluteUri.Contains("#details") ? Constants.ApplicationPages.ConsultingCapacityWithFilterQueryStringAndDetails : Constants.ApplicationPages.ConsultingCapacityWithFilterQueryString)
                        : (Request.Url.AbsoluteUri.Contains("#details") ? Constants.ApplicationPages.UtilizationTimelineWithFilterQueryStringAndDetails : Constants.ApplicationPages.UtilizationTimelineWithFilterQueryString)
                        );
            }
            return Urls.GetSkillsProfileUrl(p);
        }

        private void AddPersonRange(Person p, int w, int load, string payType, int vacationDays, Dictionary<DateTime, double> timeoffDates, Dictionary<DateTime, string> companyHolidayDates, bool isPdf, List<Project> projects)
        {
            if (companyHolidayDates == null)
                companyHolidayDates = new Dictionary<DateTime, string>();
            var beginPeriod = BegPeriod;
            var endPeriod = EndPeriod;

            if (Granularity == 7)
            {
                if ((int)BegPeriod.DayOfWeek > 0)
                {
                    beginPeriod = BegPeriod.AddDays(-1 * ((int)BegPeriod.DayOfWeek));
                }
                if ((int)EndPeriod.DayOfWeek < 6)
                {
                    endPeriod = EndPeriod.AddDays(6 - ((int)EndPeriod.DayOfWeek));
                }
            }
            else if (Granularity == 30)
            {
                beginPeriod = BegPeriod;
                endPeriod = EndPeriod;

                if ((int)beginPeriod.DayOfWeek > 0)
                {
                    beginPeriod = beginPeriod.AddDays(-1 * ((int)beginPeriod.DayOfWeek));
                }
                if ((int)endPeriod.DayOfWeek < 6)
                {
                    endPeriod = endPeriod.AddDays(6 - ((int)endPeriod.DayOfWeek));
                }
            }
            var period = (endPeriod.Subtract(beginPeriod).Days + 1);
            var pointStartDate = beginPeriod.AddDays(w * Granularity);
            var pointEndDate = beginPeriod.AddDays(((w + 1) * Granularity));
            bool isWeekEnd = false;

            if (Granularity == 30)
            {
                var numberOfMonths = (endPeriod.Year - beginPeriod.Year) * 12 + endPeriod.Month - beginPeriod.Month;
                var tempDate = beginPeriod.AddMonths(w);
                pointStartDate = w == 0 ? beginPeriod.AddMonths(w) : new DateTime(tempDate.Year, tempDate.Month, 1);
                pointEndDate = w == numberOfMonths ? endPeriod : new DateTime(tempDate.Year, tempDate.Month, 1).AddMonths(1);
            }
            else
            {
                var delta = period - (w * Granularity - 1);
                if (delta <= Granularity)
                {
                    pointEndDate = beginPeriod.AddDays(period);
                }
            }

            var range = AddRange(pointStartDate, pointEndDate, _personsCount, isPdf, ExculdeInvestmentResources ? p.IsInvestmentResource : false);
            List<DataPoint> innerRangeList = new List<DataPoint>();
            List<DataPoint> investmentInnerRangeList = new List<DataPoint>();
            bool isHiredIntheEmployeementRange = p.EmploymentHistory.Any(ph => ph.HireDate < pointEndDate && (!ph.TerminationDate.HasValue || ph.TerminationDate.Value >= pointStartDate));
            bool isRangeComapanyHolidays = IsRangeComapanyHolidays(pointStartDate, pointEndDate, companyHolidayDates, false) == 2;
            int rangeType = IsCapacityMode ? ((load > 100 && !isRangeComapanyHolidays) ? 1 : (load > 100 && isRangeComapanyHolidays) ? 2 : 0) : ((load < 0 && !isRangeComapanyHolidays) ? 1 : (load < 0 && isRangeComapanyHolidays) ? 2 : 0);
            range.Color = IsCapacityMode ? Coloring.GetColorByCapacity(load, rangeType, isHiredIntheEmployeementRange, isWeekEnd, p.IsInvestmentResource && p.TargetUtilization != null ? (int)p.TargetUtilization : -1) : Coloring.GetColorByUtilization(load, rangeType, isHiredIntheEmployeementRange, p.IsInvestmentResource && p.TargetUtilization != null ? (int)p.TargetUtilization : -1, p.CurrentPay.Timescale == TimescaleType.Hourly);
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
                ConsReportColoringElementSection coloring = ConsReportColoringElementSection.ColorSettings;
                List<Quadruple<DateTime, DateTime, BadgeType, string>> weekDatesRange = new List<Quadruple<DateTime, DateTime, BadgeType, string>>();//third parameter in the list int will have 3 possible values '0' for utilization '1' for timeoffs '2' for companyholiday
                bool IsWholeRangeVacation = true;
                bool IsWholeRangeCompanyHolidays = true;
                for (var d = pointStartDate; d < pointEndDate; d = d.AddDays(1))
                {
                    if (!timeoffDates.Any(t => t.Key == d))
                    {
                        if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                        {
                            IsWholeRangeVacation = false;
                            break;
                        }
                    }
                }
                if (payType == "W2-Salary")
                {
                    for (var d = pointStartDate; d < pointEndDate; d = d.AddDays(1))
                    {
                        if (!companyHolidayDates.Select(s => s.Key).Any(t => t == d))
                        {
                            if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                            {
                                IsWholeRangeCompanyHolidays = false;
                                break;
                            }
                        }
                    }
                }
                else
                    IsWholeRangeCompanyHolidays = false;
                if (!IsWholeRangeVacation)
                {

                    for (var d = pointStartDate; d < pointEndDate; d = d.AddDays(1))
                    {
                        int dayType = timeoffDates.Any(t => t.Key == d) ? 1 : payType == "W2-Salary" ? IsRangeComapanyHolidays(d, d.AddDays(1), companyHolidayDates, (IsCapacityMode ? load > 100 : load < 0), (IsCapacityMode ? load > 100 : load < 0) && !IsWholeRangeCompanyHolidays) : 0;
                        string holidayDescription = companyHolidayDates.Keys.Any(t => t == d) ? companyHolidayDates[d] : timeoffDates.Keys.Any(t => t == d) ? timeoffDates[d].ToString() : "8";
                        int badgeType = GetBadgeType(p, d);
                        if (weekDatesRange.Any(tri => tri.Second == d.AddDays(-1) && dayType == tri.Third.DayType && dayType != 2 && tri.Fourth == holidayDescription && badgeType == tri.Third.BadgedType))
                        {
                            var tripleRange = weekDatesRange.First(tri => tri.Second == d.AddDays(-1) && dayType == tri.Third.DayType && badgeType == tri.Third.BadgedType);
                            tripleRange.Second = d;
                        }
                        else
                        {
                            weekDatesRange.Add(new Quadruple<DateTime, DateTime, BadgeType, string>(d, d, new BadgeType() { DayType = dayType, BadgedType = badgeType }, holidayDescription));
                        }
                    }

                    foreach (var tripleR in weekDatesRange)
                    {
                        var innerRange = AddRange(tripleR.First, tripleR.Second.AddDays(1), _personsCount, isPdf, ExculdeInvestmentResources ? p.IsInvestmentResource : false);
                        innerRange.Color = IsCapacityMode ? Coloring.GetColorByCapacity(load, tripleR.Third.DayType, isHiredIntheEmployeementRange, isWeekEnd, p.IsInvestmentResource && p.TargetUtilization != null ? (int)p.TargetUtilization : -1) : Coloring.GetColorByUtilization(load, tripleR.Third.DayType, isHiredIntheEmployeementRange, p.IsInvestmentResource && p.TargetUtilization != null ? (int)p.TargetUtilization : -1, p.CurrentPay.Timescale == TimescaleType.Hourly);
                        innerRange.ToolTip = FormatRangeTooltip(load, tripleR.First, tripleR.Second, tripleR.Third, payType, IsCapacityMode, tripleR.Fourth, GetProjectsHoverText(projects, tripleR.First, tripleR.Second));
                        innerRange.BackHatchStyle = GetAppropriateHatchStyle(tripleR.Third.BadgedType);
                        innerRange.BackSecondaryColor = Color.Black;

                        if (ExculdeInvestmentResources && p.IsInvestmentResource)
                        {
                            investmentInnerRangeList.Add(innerRange);
                        }
                        else
                        {
                            innerRangeList.Add(innerRange);
                        }
                    }
                }


                else //If the whole range is vacation days
                {
                    for (var d = pointStartDate; d < pointEndDate; d = d.AddDays(1))
                    {
                        int dayType = 1;
                        string holidayDescription = timeoffDates.Keys.Any(t => t == d) ? timeoffDates[d].ToString() : "8";
                        int badgeType = GetBadgeType(p, d);

                        if (weekDatesRange.Any(tri => tri.Second == d.AddDays(-1) && dayType == tri.Third.DayType && tri.Fourth == holidayDescription && badgeType == tri.Third.BadgedType))
                        {
                            var tripleRange = weekDatesRange.First(tri => tri.Second == d.AddDays(-1) && dayType == tri.Third.DayType && badgeType == tri.Third.BadgedType);
                            tripleRange.Second = d;
                        }
                        else
                        {
                            weekDatesRange.Add(new Quadruple<DateTime, DateTime, BadgeType, string>(d, d, new BadgeType() { DayType = dayType, BadgedType = badgeType }, holidayDescription));
                        }
                    }

                    foreach (var tripleR in weekDatesRange)
                    {
                        var innerRange = AddRange(tripleR.First, tripleR.Second.AddDays(1), _personsCount, isPdf, ExculdeInvestmentResources ? p.IsInvestmentResource : false);
                        innerRange.Color = IsCapacityMode ? Coloring.GetColorByCapacity(load, 1, isHiredIntheEmployeementRange, isWeekEnd, p.IsInvestmentResource && p.TargetUtilization != null ? (int)p.TargetUtilization : -1) : Coloring.GetColorByUtilization(load, 1, isHiredIntheEmployeementRange, p.IsInvestmentResource && p.TargetUtilization != null ? (int)p.TargetUtilization : -1, p.CurrentPay.Timescale == TimescaleType.Hourly);
                        innerRange.ToolTip = FormatRangeTooltip(load, tripleR.First, tripleR.Second, tripleR.Third, null, false, tripleR.Fourth, GetProjectsHoverText(projects, tripleR.First, tripleR.Second));
                        innerRange.BackHatchStyle = GetAppropriateHatchStyle(tripleR.Third.BadgedType);
                        innerRange.BackSecondaryColor = Color.Black;
                        if (ExculdeInvestmentResources && p.IsInvestmentResource)
                        {
                            investmentInnerRangeList.Add(innerRange);
                        }
                        else
                        {
                            innerRangeList.Add(innerRange);
                        }
                    }
                }

                if (!IsSampleReport)
                {
                    CompanyHolidays = companyHolidayDates;
                    range.PostBackValue = FormatRangePostbackValue(p, beginPeriod, endPeriod); // For the whole period
                    range.Url = IsCapacityMode ? (Request.QueryString[Constants.FilterKeys.ApplyFilterFromCookieKey] == "true" ? Constants.ApplicationPages.ConsultingCapacityWithFilterQueryStringAndDetails : Constants.ApplicationPages.ConsultingCapacityWithDetails)
                                    : (Request.QueryString[Constants.FilterKeys.ApplyFilterFromCookieKey] == "true" ? Constants.ApplicationPages.UtilizationTimelineWithFilterQueryStringAndDetails : Constants.ApplicationPages.ConsTimelineReportDetails);

                    if (innerRangeList.Any())
                    {
                        foreach (var r in innerRangeList)
                        {
                            r.PostBackValue = range.PostBackValue;
                            r.Url = range.Url;
                        }
                    }
                    if (investmentInnerRangeList.Any())
                    {
                        foreach (var r in investmentInnerRangeList)
                        {
                            r.PostBackValue = range.PostBackValue;
                            r.Url = range.Url;
                        }
                    }
                }
            }
        }

        public string GetProjectsHoverText(List<Project> projects, DateTime startDate, DateTime endDate)
        {
            var text = "";
            if (projects == null)
                return text;
            foreach (var project in projects)
            {
                foreach (var milestone in project.Milestones)
                {
                    if (milestone.StartDate.Date <= endDate.Date && startDate.Date <= milestone.EndDate.Date)
                    {
                        text += string.Format("; {0} - {1}", project.ProjectNumber, project.Name);
                        break;
                    }
                }
            }
            return text;
        }

        public ChartHatchStyle GetAppropriateHatchStyle(int badgeType)
        {
            var result = new ChartHatchStyle();
            switch (badgeType)
            {
                case 0: result = ChartHatchStyle.None;
                    break;
                case 1: result = ChartHatchStyle.LargeGrid;
                    break;
                case 2: result = ChartHatchStyle.Vertical;
                    break;
                case 3: result = ChartHatchStyle.Divot;
                    break;
                case 4: result = ChartHatchStyle.Divot;
                    break;
                case 5: result = ChartHatchStyle.DiagonalBrick;
                    break;
            }
            return result;
        }

        public int GetBadgeType(Person person, DateTime startDate)
        {
            if (!utf.IncludeBadgeStatus || IsCapacityMode)
                return 0;
            if (person.Id == 3767)
            {

            }
            if (person.Badge.IsMSManagedService)
            {
                return 5;
            }
            if (person.BadgedProjects != null)
            {
                foreach (var badgeProject in person.BadgedProjects)
                {
                    if (startDate.Date <= badgeProject.BadgeEndDate.Value.Date && badgeProject.BadgeStartDate.Value.Date <= startDate.Date)
                        return 1;
                }
            }
            if (person.Badge.BadgeStartDate.HasValue && startDate.Date <= person.Badge.BadgeEndDate.Value.Date && person.Badge.BadgeStartDate.Value.Date <= startDate.Date)
                return 2;
            if (person.Badge.BreakStartDate.HasValue && startDate.Date <= person.Badge.BreakEndDate.Value.Date && person.Badge.BreakStartDate.Value.Date <= startDate.Date)
                return 3;
            if (person.Badge.BlockStartDate.HasValue && startDate.Date <= person.Badge.BlockEndDate.Value.Date && person.Badge.BlockStartDate.Value.Date <= startDate.Date)
                return 4;
            return 0;
        }

        private DataPoint AddRange(DateTime pointStartDate, DateTime pointEndDate, double yvalue, bool isPdf, bool isInvestment = false)
        {
            int ind;

            if (isInvestment && !isPdf)
            {
                ind = WeeksSeriesInvestment.Points.AddXY(_investmentPersonCount, pointStartDate, pointEndDate);
                return WeeksSeriesInvestment.Points[ind];
            }
            else if (isPdf)
            {
                ind = WeeksSeriesPdf.Points.AddXY(isInvestment ? _investmentPersonCount : yvalue, pointStartDate, pointEndDate);
                return WeeksSeriesPdf.Points[ind];
            }
            else
            {
                ind = WeeksSeries.Points.AddXY(yvalue, pointStartDate, pointEndDate);
                return WeeksSeries.Points[ind];
            }

        }

        private static int ParseInt(string val, int def)
        {
            try
            {
                return int.Parse(val);
            }
            catch
            {
                return def;
            }
        }

        private int IsRangeComapanyHolidays(DateTime startDate, DateTime endDate, Dictionary<DateTime, string> companyHolidayDates, bool includeWeekends, bool IsMixedVacationDays = false)
        {
            //returns '0' for utilization '1' for timeoffs '2' for companyholiday

            for (var i = startDate; i < endDate; i = i.AddDays(1))
            {
                if (IsMixedVacationDays && !companyHolidayDates.Keys.Any(d => d == i) && (i.DayOfWeek == DayOfWeek.Saturday || i.DayOfWeek == DayOfWeek.Sunday))
                    return 1;
                if (companyHolidayDates.Keys.Any(d => d == i) || (includeWeekends && (i.DayOfWeek == DayOfWeek.Saturday || i.DayOfWeek == DayOfWeek.Sunday)))
                    return 2;
            }
            return 0;
        }

        private byte[] RenderPdf(HtmlToPdfBuilder builder)
        {
            MemoryStream file = new MemoryStream();
            Document document = new Document(builder.PageSize);
            document.SetPageSize(iTextSharp.text.PageSize.A4_LANDSCAPE.Rotate());
            PdfWriter writer = PdfWriter.GetInstance(document, file);
            document.Open();
            float topMargin = 0f;
            if (!ExculdeInvestmentResources)
            {
                ConsultantUtilizationPerson.Reverse();
                int count = ConsultantUtilizationPerson.Count;
                for (int i = 0; i < Math.Ceiling((double)count / reportSize); i++)
                {
                    ChartForPdf(i);
                    document.SetMargins(document.LeftMargin, document.RightMargin, topMargin, topMargin);
                    document.NewPage();
                    document.Add(ConsultingImage(i));
                }
            }
            else
            {
                NonInvestmentResources.Reverse();
                InvestmentResources.Reverse();
                int count = NonInvestmentResources.Count;
                for (int i = 0; i < Math.Ceiling((double)count / reportSize); i++)
                {
                    ChartForPdf(i);
                    document.SetMargins(document.LeftMargin, document.RightMargin, topMargin, topMargin);
                    document.NewPage();
                    document.Add(ConsultingImage(i));
                }
                count = InvestmentResources.Count;
                isInvestmentPdfChart = true;
                for (int i = 0; i < Math.Ceiling((double)count / reportSize); i++)
                {
                    ChartForPdf(i);
                    document.SetMargins(document.LeftMargin, document.RightMargin, topMargin, topMargin);
                    document.NewPage();
                    document.Add(ConsultingImage(i));
                }
            }



            document.Close();
            return file.ToArray();
        }

        public void PDFExport()
        {
            HtmlToPdfBuilder builder = new HtmlToPdfBuilder(iTextSharp.text.PageSize.A4);
            string filename = IsCapacityMode ? "ConsultingCapacity.pdf" : "ConsultingUtilization.pdf";
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

        public void ChartForPdf(int i)
        {
            page = 0;
            chartPdf.Titles.Clear();
            foreach (var series in chartPdf.Series)
                series.Points.Clear();
            chartPdf.ChartAreas[MAIN_CHART_AREA_NAME].AxisX.CustomLabels.Clear();
            chartPdf.ChartAreas[MAIN_CHART_AREA_NAME].AxisX2.CustomLabels.Clear();
            InitChart(true, i);
            List<ConsultantUtilizationPerson> report = new List<ConsultantUtilizationPerson>();
            if (ExculdeInvestmentResources)
            {
                if (!isInvestmentPdfChart)
                {
                    report = NonInvestmentResources;
                }
                else
                {
                    report = InvestmentResources;
                }
            }
            else
            {
                report = ConsultantUtilizationPerson;
            }
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

        #region Formatting

        private static string FormatAvgPercentage(int personVacationDays, int avg)
        {
            if (personVacationDays > 0)
            {
                return
                    string.Format(
                    avg < 0 ? VACATION_NEGATIVE_AVERAGE_UTIL_FORMAT : VACATION_AVERAGE_UTIL_FORMAT,
                        avg);
            }

            return
                string.Format(
                avg < 0 ? NEGATIVE_AVERAGE_UTIL_FORMAT : AVERAGE_UTIL_FORMAT,
                    avg);
        }

        private string FormatRangePostbackValue(Person p, DateTime beg, DateTime end)
        {
            return string.Format(
                POSTBACK_FORMAT, DELIMITER,
                p.Id,
                beg.ToShortDateString(),
                end.ToShortDateString(),
                string.Format(NAME_FORMAT_WITH_DATES,
                              p.LastName,
                              p.FirstName,
                              p.Status.Name,
                              beg.ToShortDateString(),
                              end.ToShortDateString()),
                              utf.ActiveProjects.ToString(),
                              utf.ProjectedProjects.ToString(),
                              utf.InternalProjects.ToString(),
                              utf.ExperimentalProjects.ToString(),
                              utf.ProposedProjects.ToString(),
                              utf.CompletedProjects.ToString()
                              );
        }

        private static string FormatPersonName(Person p)
        {
            string x = "abc";

            return string.Format(
                NAME_FORMAT,
                p.LastName,
                p.FirstName,
                p.Title.TitleName
                );
        }

        private static string FormatRangeTooltip(int load, DateTime pointStartDate, DateTime pointEndDate, BadgeType dayType, string payType = null, bool IsCapacityMode = false, string holidayDescription = null, string projects = "")
        {
            //dayType = '0' for utilization '1' for timeoffs '2' for companyholiday
            string tooltip = "";

            if (pointStartDate == pointEndDate)
            {
                tooltip = dayType.DayType == 1 ?
                    string.Format(VACATION_TOOLTIP_FORMAT, pointStartDate.ToString("MMM. d")) + (holidayDescription != "8" ? "- " + holidayDescription + " hours" : string.Empty) + (BadgeTooltip(dayType.BadgedType) != "" ? ", " + BadgeTooltip(dayType.BadgedType) : "") :
                           dayType.DayType == 2 ? holidayDescription + ": " + pointStartDate.ToString("MMM. d") :
                           string.Format(TOOLTIP_FORMAT_FOR_SINGLEDAY,
                                 pointStartDate.ToString("MMM, d"), string.Format(
                                     IsCapacityMode ? CAPACITY_TOOLTIP_FORMAT : UTILIZATION_TOOLTIP_FORMAT,
                                         load), payType) + (BadgeTooltip(dayType.BadgedType) != "" ? ", " + BadgeTooltip(dayType.BadgedType) : "") + projects;
            }
            else
            {
                tooltip = dayType.DayType == 1 ?
                          string.Format(VACATION_TOOLTIP_FORMAT, pointStartDate.ToString("MMM. d") + " - " +
                          pointEndDate.ToString("MMM. d")) + (holidayDescription != "8" ? "- " + holidayDescription + " hours" : string.Empty) + (BadgeTooltip(dayType.BadgedType) != "" ? ", " + BadgeTooltip(dayType.BadgedType) : "") :
                          string.Format(TOOLTIP_FORMAT,
                                     pointStartDate.ToString("MMM, d"),
                                     pointEndDate.ToString("MMM, d"),
                                      string.Format(
                                         IsCapacityMode ? CAPACITY_TOOLTIP_FORMAT : UTILIZATION_TOOLTIP_FORMAT,
                                             load), payType) + (BadgeTooltip(dayType.BadgedType) != "" ? ", " + BadgeTooltip(dayType.BadgedType) : "") + projects;
            }

            return tooltip;
        }

        public static string BadgeTooltip(int badgeType)
        {
            var result = "";
            switch (badgeType)
            {
                case 1: result = "MS Badged";
                    break;
                case 2: result = "18 mos Window Active";
                    break;
                case 3: result = "6-Month Break";
                    break;
                case 4: result = "Block";
                    break;
            }
            return result;
        }

        protected void btnUpdateView_OnClick(object sender, EventArgs e)
        {
            uaeDetails.EnableClientState = true;
            this.UpdateReport();
            this.hdnIsChartRenderedFirst.Value = "true";
            tblExport.Visible = true;
            updConsReport.Update();

            SaveFilters(null, null);
        }

        public void UpdateReportFromFilters()
        {
            this.UpdateReport();
            chart.CssClass = "ConsultantsWeeklyReportAlignCenter";
            tblExport.Visible = true;
            uaeDetails.EnableClientState = true;
        }

        protected void btnResetFilter_OnClick(object sender, EventArgs e)
        {
            //if (!(hdnIsChartRenderedFirst.Value == "true"))
            //{
            //    chart.CssClass = "hide";
            //}
            if (IsCapacityMode)
            {
                utf.ResetSortDirectionForCapacityMode();
            }
            this.UpdateReport();
            updConsReport.Update();
            SaveFilters(null, null);
        }

        #endregion Formatting

        private void SaveFilterValuesForSession()
        {
            ConsultingUtilizationReportFilters filter = new ConsultingUtilizationReportFilters();
            utf.SaveFiltersFromControls(filter);
            if (IsCapacityMode)
            {
                ReportsFilterHelper.SaveFilterValues(ReportName.ConsultingCapacityReport, filter);
            }
            else
            {
                ReportsFilterHelper.SaveFilterValues(ReportName.ConsultingUtilizationReport, filter);
            }
        }

        private void GetFilterValuesForSession()
        {
            ConsultingUtilizationReportFilters filters;
            if (IsCapacityMode)
            {
                filters = ReportsFilterHelper.GetFilterValues(ReportName.ConsultingCapacityReport) as ConsultingUtilizationReportFilters;
            }
            else
            {
                filters = ReportsFilterHelper.GetFilterValues(ReportName.ConsultingUtilizationReport) as ConsultingUtilizationReportFilters;
            }
            if (filters != null)
            {
                utf.Filters = filters;
            }
        }

    }

    public class BadgeType
    {
        //dayType = '0' for utilization '1' for timeoffs '2' for companyholiday
        public int DayType
        {
            get;
            set;
        }

        //BadgedType = 1 for badged,2 for 18 mo window,3 for 6 mo break and block, 4 for block dates
        public int BadgedType
        {
            get;
            set;
        }
    }
}


