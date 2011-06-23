using System;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Configuration.ConsReportColoring;
using PraticeManagement.Objects;
using PraticeManagement.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PraticeManagement.Controls.Reports
{
    public partial class ConsultantsWeeklyReport : UserControl
    {
        #region Constants

        private const string PERSON_TOOLTIP_FORMAT = "PayType {0}, Hired {1}";
        private const string NOT_HIRED_PERSON_TOOLTIP_FORMAT = "PayType {0}";
        private const string WEEKS_SERIES_NAME = "Weeks";
        private const string MAIN_CHART_AREA_NAME = "MainArea";
        private const int DAYS_FORWARD = 184;
        private const int DEFAULT_STEP = 7;
        private const string NAME_FORMAT = "{0}, {1} ({2})";
        private const string NAME_FORMAT_WITH_DATES = "{0}, {1} ({2}): {3}-{4}";
        private const string TITLE_FORMAT = "Consultant Utilization Report \n{0} to {1}\nFor {2} Persons; For {3} Projects\n{4}\n\n*Utilization reflects person vacation time during this period.";
        private const string POSTBACK_FORMAT = "{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}";
        private const char DELIMITER = '+';
        private const string TOOLTIP_FORMAT = "{0}-{1} {2}";
        private const string FULL_MONTH_NAME_FORMAT = "MMMM, yyyy";
        private const string VACATION_TOOLTIP_FORMAT = "On vacation";
        private const string UTILIZATION_TOOLTIP_FORMAT = "U% = {0}";
        private const string AVERAGE_UTIL_FORMAT = "~{0}%";
        private const string VACATION_AVERAGE_UTIL_FORMAT = "~{0}%*";
        #endregion

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

        #endregion

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
                    chart.CssClass = "hide";
                    tblDetails.Attributes.Add("class", "hide");
                }
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
                    utf.InternalProjects, TimescaleIds, PracticeIdList, AvgUtil, SortId, SortDirection, utf.ExcludeInternalPractices);

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
                Coloring.ColorLegend(legend);
        }

        /// <summary>
        /// 	Sets min/max values, adds long date names
        /// </summary>
        /// <param name = "horizAxis">Axis to decorate</param>
        private void InitAxis(Axis horizAxis)
        {
            //  Set min and max values
            horizAxis.Minimum = BegPeriod.ToOADate();
            horizAxis.Maximum = EndPeriod.AddDays(1).ToOADate();
            horizAxis.IsLabelAutoFit = true;
            horizAxis.IsStartedFromZero = true;

            if (utf.DetalizationSelectedValue == "1")
            {
                horizAxis.IntervalType = DateTimeIntervalType.Weeks;
                horizAxis.Interval = 1;

                horizAxis.IntervalOffset = GetOffset(BegPeriod);
                horizAxis.IntervalOffsetType = DateTimeIntervalType.Days;
            }
            else if (utf.DetalizationSelectedValue == "7")
            {
                if ((int)BegPeriod.DayOfWeek > 1)
                {
                    double period = Convert.ToDouble("-" + (int)BegPeriod.DayOfWeek);
                    horizAxis.Minimum = BegPeriod.AddDays(period + 1).ToOADate();
                }
                if ((int)EndPeriod.DayOfWeek < 6)
                {
                    double period = Convert.ToDouble((int)EndPeriod.DayOfWeek);
                    horizAxis.Maximum = EndPeriod.AddDays(6 - period).ToOADate();
                }

                horizAxis.IntervalType = DateTimeIntervalType.Weeks;
                horizAxis.Interval = 1;

                horizAxis.IntervalOffset = 0;
                horizAxis.IntervalOffsetType = DateTimeIntervalType.Days;
            }
            else
            {
                var beginPeriod = BegPeriod.AddDays(1 - BegPeriod.Day);
                var endPeriod = (new DateTime(EndPeriod.Year, EndPeriod.Month, 1)).AddMonths(1).AddDays(-1);

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
            int index = 0;
            for (index = 0; ; index++)
            {
                if (date.AddDays(index).DayOfWeek == DayOfWeek.Sunday)
                {
                    return -1 * ((index + 1) % 7);
                }
            }
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
                    TITLE_FORMAT,
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
                    experimentalProjects);

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
                    range.Color = Coloring.GetColorByUtilization(load, load < 0);
                    range.ToolTip = FormatRangeTooltip(load, pointStartDate, pointEndDate.AddDays(-1));
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
        public void AddPerson(Quadruple<Person, int[], int, int> quadruple)
        {
            var partsCount = quadruple.Second.Length;
            var csv = FormCSV(quadruple.Second);
            for (var w = 0; w < partsCount; w++)
            {
                //  Add another range to the person's timeline
                AddPersonRange(
                    quadruple.First, //  Person
                     w, //  Range index
                    quadruple.Second[w], csv); //  U% for the period

            }

            //  Add axis label
            AddLabel(quadruple.First, quadruple.Third, quadruple.Fourth);

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
                    Urls.GetPersonDetailsUrl(
                        p, Constants.ApplicationPages.ConsTimelineReport);
            }
            //  Tooltip
            label.ToolTip =
                string.Format(
                    DateTime.MinValue != p.HireDate ? PERSON_TOOLTIP_FORMAT : NOT_HIRED_PERSON_TOOLTIP_FORMAT,
                     p.CurrentPay.TimescaleName, // Current Pay Type
                    p.HireDate.ToShortDateString() // Hire date
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

        private void AddPersonRange(Person p, int w, int load, string csv)
        {
            var beginPeriod = BegPeriod;
            var endPeriod = EndPeriod;

            if (Granularity == 7)
            {
                if ((int)BegPeriod.DayOfWeek > 1)
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
                beginPeriod = BegPeriod.AddDays(1 - BegPeriod.Day);
                endPeriod = (new DateTime(EndPeriod.Year, EndPeriod.Month, 1)).AddMonths(1).AddDays(-1);

                if ((int)beginPeriod.DayOfWeek > 1)
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
            bool isHired = p.HireDate < pointEndDate;
            range.Color = Coloring.GetColorByUtilization(load, load < 0, isHired);

            if (!isHired)
            {
                range.ToolTip = p.HireDate.ToShortDateString();
            }
            else
            {
                range.ToolTip = FormatRangeTooltip(load, pointStartDate, pointEndDate.AddDays(-1));
                if (!IsSampleReport)
                {
                    range.PostBackValue = FormatRangePostbackValue(p, beginPeriod, endPeriod); // For the whole period
                    range.Url = Constants.ApplicationPages.ConsTimelineReportDetails;
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

        #region Formatting

        private static string FormatAvgPercentage(int personVacationDays, int avg)
        {
            if (personVacationDays > 0)
            {
                return
                    string.Format(
                        VACATION_AVERAGE_UTIL_FORMAT,
                        avg);
            }

            return
                string.Format(
                    AVERAGE_UTIL_FORMAT,
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
                p.Seniority.Name
                );
        }

        private static string FormatRangeTooltip(int load, DateTime pointStartDate, DateTime pointEndDate)
        {
            return string.Format(TOOLTIP_FORMAT,
                                 pointStartDate.ToString("MMM, d"),
                                 pointEndDate.ToString("MMM, d"),
                                 load < 0
                                     ? VACATION_TOOLTIP_FORMAT
                                     : string.Format(
                                         UTILIZATION_TOOLTIP_FORMAT,
                                         load));
        }

        protected void btnUpdateView_OnClick(object sender, EventArgs e)
        {
            tblDetails.Attributes.Remove("class");
            uaeDetails.EnableClientState = true;
            this.UpdateReport();
            this.hdnIsChartRenderedFirst.Value = "true";
            updConsReport.Update();
        }

        protected void btnResetFilter_OnClick(object sender, EventArgs e)
        {
            if (!(hdnIsChartRenderedFirst.Value == "true"))
            {
                chart.CssClass = "hide";
            }

        }

        #endregion
    }
}

