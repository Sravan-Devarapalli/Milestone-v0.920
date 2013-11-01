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
        private const string TITLE_FORMAT = "Consulting {0} Report \n{1} to {2}\nFor {3} Persons; For {4} Projects\n{5}\n\n*{0} reflects person vacation time during this period.";
        private const string TITLE_FORMAT_WITHOUT_REPORT = "Consulting {0} \n{1} to {2}\nFor {3} Persons; For {4} Projects\n{5}\n\n*{0} reflects person vacation time during this period.";
        private const string POSTBACK_FORMAT = "{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}";
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

        #endregion Constants

        #region Fields

        private int _personsCount;

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
            get;
            set;
        }

        public Dictionary<DateTime, string> CompanyHolidays
        {
            get { return ViewState[COMPANYHOLIDAYS_KEY] as Dictionary<DateTime, string>; }
            set { ViewState[COMPANYHOLIDAYS_KEY] = value; }
        }

        #endregion Fields

        protected void Page_PreRender(object sender, EventArgs e)
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
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
                        tblDetails.Attributes.Add("class", "hide");
                    }
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
                    cookie.ActiveProjects, cookie.ProjectedProjects, cookie.InternalProjects, cookie.ExperimentalProjects);

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
                    utf.InternalProjects, TimescaleIds, PracticeIdList, AvgUtil, SortId, (IsCapacityMode && SortId == 0) ? (SortDirection == "Desc" ? "Asc" : "Desc") : SortDirection, utf.ExcludeInternalPractices);

            foreach (var quadruple in report)
                AddPerson(quadruple);

            chart.Height = Resources.Controls.TimelineGeneralHeaderHeigth +
                           Resources.Controls.TimelineGeneralItemHeigth * report.Count +
                           Resources.Controls.TimelineGeneralFooterHeigth;
        }

        /// <summary>
        /// 	Init axises, title and legends
        /// </summary>
        private void InitChart()
        {
            InitAxis(chart.ChartAreas[MAIN_CHART_AREA_NAME].AxisY);
            InitAxis(chart.ChartAreas[MAIN_CHART_AREA_NAME].AxisY2);
            UpdateChartTitle();
            InitLegends();
        }

        /// <summary>
        /// 	Apply color coding to all legends
        /// </summary>
        private void InitLegends()
        {
            foreach (var legend in chart.Legends)
            {
                if (IsCapacityMode)
                {
                    Coloring.CapacityColorLegends(legend);
                }
                else
                {
                    Coloring.ColorLegend(legend);
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
        private void UpdateChartTitle()
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
                && utf.InternalProjects && utf.ExperimentalProjects)
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
            }
            if (string.IsNullOrEmpty(projectsPlaceHolder))
            {
                projectsPlaceHolder = "No";
            }

            chart.Titles.Add(
                string.Format(
                    IsCapacityMode ? TITLE_FORMAT_WITHOUT_REPORT : TITLE_FORMAT,
                    IsCapacityMode ? Capacity : Utilization,
                    BegPeriod.ToString("MM/dd/yyyy"),
                    EndPeriod.ToString("MM/dd/yyyy"),
                    personsPlaceHolder, projectsPlaceHolder, utf.PracticesFilterText()));
        }

        protected void Chart_Click(object sender, ImageMapEventArgs e)
        {
            UpdateReport();
            var query = e.PostBackValue.Split(DELIMITER);

            // Exctract data from query
            var personId = int.Parse(query[0]);
            var repStartDate = DateTime.Parse(query[1]);
            var repEndDate = DateTime.Parse(query[2]);
            var activeProjects = bool.Parse(query[4]);
            var projectedProjects = bool.Parse(query[5]);
            var internalProjects = bool.Parse(query[6]);
            var experimentalProjects = bool.Parse(query[7]);

            ShowDetailedReport(personId, repStartDate, repEndDate, query[3],
            activeProjects, projectedProjects, internalProjects, experimentalProjects);

            System.Web.UI.ScriptManager.RegisterClientScriptBlock(updConsReport, updConsReport.GetType(), "focusDetailReport", "window.location='#details';", true);

            SaveFilters(personId, query[3]);
        }

        private void SaveFilters(int? personId, string chartTitle)
        {
            var filter = utf.SaveFilters(personId, chartTitle);
            SerializationHelper.SerializeCookie(filter, IsCapacityMode ? Constants.FilterKeys.ConsultingCapacityFilterCookie : Constants.FilterKeys.ConsultantUtilTimeLineFilterCookie);
        }

        private void ShowDetailedReport(int personId, DateTime repStartDate, DateTime repEndDate, string chartTitle,
            bool activeProjects, bool projectedProjects, bool internalProjects, bool experimentalProjects)
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
                    IsCapacityMode);

            var utilizationDaily = DataHelper.ConsultantUtilizationDailyByPerson(repStartDate, ParseInt(repEndDate.Subtract(repStartDate).Days.ToString(), DAYS_FORWARD),
                utf.ActiveProjects, utf.ProjectedProjects, utf.InternalProjects, utf.ExperimentalProjects, personId);
            var avgUtils = utilizationDaily.First().Second;
            for (int index = 0; index < avgUtils.Length; index++)
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
                    range.Color = Coloring.GetColorByUtilization(load, load < 0 ? 2 : 0);
                    string holidayDescription = CompanyHolidays.Keys.Any(t => t == pointStartDate) ? CompanyHolidays[pointStartDate] : string.Empty;
                    range.ToolTip = FormatRangeTooltip(load, pointStartDate, pointEndDate.AddDays(-1), load < 0 ? 2 : 0,null,false,holidayDescription);
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
        public void AddPerson(ConsultantUtilizationPerson quadruple)
        {
            var partsCount = quadruple.WeeklyUtilization.Count;
            var csv = FormCSV(quadruple.WeeklyUtilization.ToArray());
            for (var w = 0; w < partsCount; w++)
            {
                TimescaleType payType = (TimescaleType)quadruple.WeeklyPayTypes[w];
                //  Add another range to the person's timeline
                AddPersonRange(
                    quadruple.Person, //  Person
                     w, //  Range index
                     IsCapacityMode ? 100 - quadruple.WeeklyUtilization[w] : quadruple.WeeklyUtilization[w], csv,
                     payType == TimescaleType.Undefined ? "No Pay Type" : DataHelper.GetDescription(payType), quadruple.WeeklyVacationDays[w], quadruple.TimeOffDates, quadruple.CompanyHolidayDates
                     ); //  U% or C% for the period
            }

            //  Add axis label
            AddLabel(quadruple.Person, IsCapacityMode ? 100 - quadruple.AverageUtilization : quadruple.AverageUtilization, quadruple.PersonVacationDays);

            //  Increase persons counter
            _personsCount++;
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
        private void AddLabel(Person p, int avg, int vacationDays)
        {
            //  Get labels collection
            var labels =
                chart.ChartAreas[MAIN_CHART_AREA_NAME].AxisX.CustomLabels;
            //  Create new label
            var label =
                labels.Add(
                    _personsCount - 0.49, // From position
                    _personsCount + 0.49, // To position
                    FormatPersonName(p), // Formated person title
                    0, // Index
                    LabelMarkStyle.None); // Mark style: none

            if (!IsSampleReport)
            {
                //  Url to person details page, return to report
                label.Url =
                    Urls.GetPersonDetailsUrl(p,
                    IsCapacityMode ? (Request.Url.AbsoluteUri.Contains("#details") ? Constants.ApplicationPages.ConsultingCapacityWithFilterQueryStringAndDetails : Constants.ApplicationPages.ConsultingCapacityWithFilterQueryString)
                     : (Request.Url.AbsoluteUri.Contains("#details") ? Constants.ApplicationPages.UtilizationTimelineWithFilterQueryStringAndDetails : Constants.ApplicationPages.UtilizationTimelineWithFilterQueryString)
                     );
            }
            //  Tooltip
            label.ToolTip =
                string.Format(
                    DateTime.MinValue != p.HireDate ? PERSON_TOOLTIP_FORMAT : NOT_HIRED_PERSON_TOOLTIP_FORMAT,
                     p.CurrentPay.TimescaleName, // Current Pay Type
                    p.HireDate.ToString("MM/dd/yyyy") // Hire date
                //,avg // Average U%
                    );

            //  Get labels collection
            labels = chart.ChartAreas[MAIN_CHART_AREA_NAME].AxisX2.CustomLabels;
            //  Create new label
            label =
                labels.Add(
                    _personsCount - 0.49, // From position
                    _personsCount + 0.49, // To position
                    FormatAvgPercentage(vacationDays, avg), // Formated person title
                    0, // Index
                    LabelMarkStyle.None); // Mark style: none
        }

        private void AddPersonRange(Person p, int w, int load, string csv, string payType, int vacationDays, List<DateTime> timeoffDates, Dictionary<DateTime, string> companyHolidayDates)
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
                pointStartDate = beginPeriod.AddMonths(w);
                pointEndDate = endPeriod > beginPeriod.AddMonths(w + 1) ? beginPeriod.AddMonths(w + 1) : endPeriod.AddDays(1);
            }
            else
            {
                var delta = period - (w * Granularity - 1);
                if (delta <= Granularity)
                {
                    pointEndDate = beginPeriod.AddDays(period);
                }
            }

            var range = AddRange(pointStartDate, pointEndDate, _personsCount);
            List<DataPoint> innerRangeList = new List<DataPoint>();
            bool isHiredIntheEmployeementRange = p.EmploymentHistory.Any(ph => ph.HireDate < pointEndDate && (!ph.TerminationDate.HasValue || ph.TerminationDate.Value >= pointStartDate));
            bool isRangeComapanyHolidays = IsRangeComapanyHolidays(pointStartDate, pointEndDate, companyHolidayDates, false) == 2;
            int rangeType = IsCapacityMode ? ((load > 100 && !isRangeComapanyHolidays) ? 1 : (load > 100 && isRangeComapanyHolidays) ? 2 : 0) : ((load < 0 && !isRangeComapanyHolidays) ? 1 : (load < 0 && isRangeComapanyHolidays) ? 2 : 0);
            range.Color = IsCapacityMode ? Coloring.GetColorByCapacity(load, rangeType, isHiredIntheEmployeementRange, isWeekEnd) : Coloring.GetColorByUtilization(load, rangeType, isHiredIntheEmployeementRange);
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
            {   //vacationDays doesn't include saturdays and sundays
                //if some part of the range has vacation days(not the whole range) OR the whole range is vacation days
                if ((vacationDays > 0 && !(IsCapacityMode ? load > 100 : load < 0)) || (IsCapacityMode ? load > 100 : load < 0))
                {
                    range.ToolTip = "";
                    range.Color = Color.White;
                    ConsReportColoringElementSection coloring = ConsReportColoringElementSection.ColorSettings;
                    List<Quadruple<DateTime, DateTime, int, string>> weekDatesRange = new List<Quadruple<DateTime, DateTime, int, string>>();//third parameter in the list int will have 3 possible values '0' for utilization '1' for timeoffs '2' for companyholiday
                    bool IsWholeRangeVacation = true;
                    bool IsWholeRangeCompanyHolidays = true;
                    for (var d = pointStartDate; d < pointEndDate; d = d.AddDays(1))
                    {
                        if (!timeoffDates.Any(t => t == d))
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
                            int dayType = timeoffDates.Any(t => t == d) ? 1 : payType == "W2-Salary" ? IsRangeComapanyHolidays(d, d.AddDays(1), companyHolidayDates, (IsCapacityMode ? load > 100 : load < 0), (IsCapacityMode ? load > 100 : load < 0) && !IsWholeRangeCompanyHolidays) : 0;

                            string holidayDescription = companyHolidayDates.Keys.Any(t => t == d) ? companyHolidayDates[d] : "Weekly off";
                            if (weekDatesRange.Any(tri => tri.Second == d.AddDays(-1) && dayType == tri.Third && dayType != 2))
                            {
                                var tripleRange = weekDatesRange.First(tri => tri.Second == d.AddDays(-1) && dayType == tri.Third);
                                tripleRange.Second = d;
                            }
                            else
                            {
                                weekDatesRange.Add(new Quadruple<DateTime, DateTime, int, string>(d, d, dayType, holidayDescription));
                            }
                        }

                        foreach (var tripleR in weekDatesRange)
                        {
                            var innerRange = AddRange(tripleR.First, tripleR.Second.AddDays(1), _personsCount);
                            innerRange.Color = IsCapacityMode ? Coloring.GetColorByCapacity(load, tripleR.Third, isHiredIntheEmployeementRange, isWeekEnd) : Coloring.GetColorByUtilization(load, tripleR.Third, isHiredIntheEmployeementRange);
                            innerRange.ToolTip = FormatRangeTooltip(load, tripleR.First, tripleR.Second, tripleR.Third, payType, IsCapacityMode, tripleR.Fourth);
                            innerRangeList.Add(innerRange);
                        }
                    }


                    else //If the whole range is vacation days
                    {
                        range.Color = IsCapacityMode ? Coloring.GetColorByCapacity(load, 1, isHiredIntheEmployeementRange, isWeekEnd) : Coloring.GetColorByUtilization(load, 1, isHiredIntheEmployeementRange);
                        range.ToolTip = FormatRangeTooltip(load, pointStartDate, pointEndDate.AddDays(-1), 1);
                    }
                }

                //If the whole range is working days
                else
                {
                    range.ToolTip = FormatRangeTooltip(load, pointStartDate, pointEndDate.AddDays(-1), 0, payType, IsCapacityMode);
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
                }
            }
        }

        private DataPoint AddRange(DateTime pointStartDate, DateTime pointEndDate, double yvalue)
        {
            var ind = WeeksSeries.Points.AddXY(
                yvalue,
                pointStartDate,
                pointEndDate);

            var range = WeeksSeries.Points[ind];
            return range;
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
            //returns true if the given range is companyholidays or saturdays or sundays otherwise false

            for (var i = startDate; i < endDate; i = i.AddDays(1))
            {
                if (IsMixedVacationDays && !companyHolidayDates.Keys.Any(d => d == i) && (i.DayOfWeek == DayOfWeek.Saturday || i.DayOfWeek == DayOfWeek.Sunday))
                    return 1;
                if (companyHolidayDates.Keys.Any(d => d == i) || (includeWeekends && (i.DayOfWeek == DayOfWeek.Saturday || i.DayOfWeek == DayOfWeek.Sunday)))
                    return 2;
            }
            return 0;
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
                              utf.ExperimentalProjects.ToString()
                              );
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

        private static string FormatRangeTooltip(int load, DateTime pointStartDate, DateTime pointEndDate, int dayType, string payType = null, bool IsCapacityMode = false, string holidayDescription = null)
        {
            //dayType = '0' for utilization '1' for timeoffs '2' for companyholiday
            string tooltip = "";

            if (pointStartDate == pointEndDate)
            {
                tooltip = dayType == 1 ?
                           string.Format(VACATION_TOOLTIP_FORMAT, pointStartDate.ToString("MMM. d")) :
                           dayType == 2 ? holidayDescription + ": " + pointStartDate.ToString("MMM. d") :
                           string.Format(TOOLTIP_FORMAT_FOR_SINGLEDAY,
                                 pointStartDate.ToString("MMM, d"), string.Format(
                                     IsCapacityMode ? CAPACITY_TOOLTIP_FORMAT : UTILIZATION_TOOLTIP_FORMAT,
                                         load), payType);
            }
            else
            {
                tooltip = dayType == 1 ?
                          string.Format(VACATION_TOOLTIP_FORMAT, pointStartDate.ToString("MMM. d") + " - " +
                                     pointEndDate.ToString("MMM. d")) :
                          string.Format(TOOLTIP_FORMAT,
                                     pointStartDate.ToString("MMM, d"),
                                     pointEndDate.ToString("MMM, d"),
                                      string.Format(
                                         IsCapacityMode ? CAPACITY_TOOLTIP_FORMAT : UTILIZATION_TOOLTIP_FORMAT,
                                             load), payType);
            }

            return tooltip;
        }

        protected void btnUpdateView_OnClick(object sender, EventArgs e)
        {
            tblDetails.Attributes.Remove("class");
            uaeDetails.EnableClientState = true;
            this.UpdateReport();
            this.hdnIsChartRenderedFirst.Value = "true";
            updConsReport.Update();

            SaveFilters(null, null);
        }

        protected void btnResetFilter_OnClick(object sender, EventArgs e)
        {
            if (!(hdnIsChartRenderedFirst.Value == "true"))
            {
                chart.CssClass = "hide";
            }
            if (IsCapacityMode)
            {
                utf.ResetSortDirectionForCapacityMode();
            }
            SaveFilters(null, null);
        }

        #endregion Formatting
    }
}

