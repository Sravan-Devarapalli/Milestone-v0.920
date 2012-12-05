using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.DataVisualization.Charting;
using PraticeManagement.Controls;
using System.Drawing;
using PraticeManagement.Utils;
using PraticeManagement.FilterObjects;
using DataTransferObjects;


namespace PraticeManagement.Controls.Reporting
{
    public partial class ConsultingDemand : System.Web.UI.UserControl
    {
        #region Constants

        private const string WEEKS_SERIES_NAME = "Weeks";
        private const string MAIN_CHART_AREA_NAME = "MainArea";
        private const string DemandItemtextWithSingleObjectLinkFormat = "{0} - {1} -                ."; // 16 spaces between 1 and 2;
        private const string DemandItemtextWithTwoObjectsLinksFormat = "{0} - {1} -                 -               .";
        private const string DemandItemtextProjectLinkFormat = "{0}                   .";
        private const string DemandItemtextTooltipFormat = "{0} - {1}";
        private const string RedirectFormat = "{0}?id={1}";
        private const string Title_Format = "Consulting Demand Report \n{0} to {1}";
        private const string DummyClientName = "!";
        private const string ChartSeriesId = "chartSeries";
        private const string FULL_MONTH_NAME_FORMAT = "MMMM, yyyy";
        private const string DemandToolTip = "<b>Date</b>: {0:MMM-dd-yyyy} <br/><b>Demand:</b> {1}";
        private const string DefaultMonthsTextInDropDownFormat = "Next {0} Months";


        #endregion

        #region Properties

        public int DefaultMonths { get; set; }

        public Series ConsultingDemandSeries
        {
            get { return chrtConsultingDemand.Series[ChartSeriesId]; }
        }

        public DateTime DefaultStartDate
        {
            get
            {
                var date = Utils.SettingsHelper.GetCurrentPMTime();
                return date.Date.AddDays((date.Day - 1) * -1);
            }
        }

        public DateTime DefaultEndDate
        {
            get
            {
                var date = Utils.SettingsHelper.GetCurrentPMTime();
                date = new DateTime(date.Year, date.Month, 1);
                return date.Date.AddMonths(DefaultMonths).AddDays(-1);
            }
        }

        public DateTime StartDate
        {
            get
            {
                if (ddlPeriod.SelectedValue == "0")
                {
                    return diRange.FromDate.Value;
                }
                else
                {
                    return DefaultStartDate;
                }
            }
        }

        public DateTime EndDate
        {

            get
            {
                if (ddlPeriod.SelectedValue == "0")
                {
                    return diRange.ToDate.Value;
                }
                else
                {
                    return DefaultEndDate;
                }
            }

        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString[Constants.FilterKeys.ApplyFilterFromCookieKey] == "true")
                {
                    var cookie = SerializationHelper.DeserializeCookie(Constants.FilterKeys.ConsultingDemandFilterCookie) as ConsultingDemandFilter;
                    if (cookie != null)
                    {
                        PopulateControlsFromCookie(cookie);
                        UpdateReport();
                        updReport.Update();
                    }
                    chrtConsultingDemand.CssClass = "Width920Px";
                }
                else
                {
                    chrtConsultingDemand.CssClass = "displayNone";
                }

                FillPeriodDropDownWithDefaultMonths();
            }
        }

        private void FillPeriodDropDownWithDefaultMonths()
        {
            if (DefaultMonths > 0)
            {
                var defaultItem = ddlPeriod.Items[0];
                defaultItem.Text = string.Format(DefaultMonthsTextInDropDownFormat, DefaultMonths);
                defaultItem.Value = DefaultMonths.ToString();

                hdnDefaultMonths.Value = DefaultMonths.ToString();
            }
        }

        private void PopulateControlsFromCookie(ConsultingDemandFilter cookie)
        {
            if (cookie.FiltersChanged == "1")//filters changed means ddlPeriod changed to custom dates.
            {
                ddlPeriod.SelectedValue = cookie.PeriodSelected;
                diRange.FromDate = cookie.StartDate;
                diRange.ToDate = cookie.EndDate;
                hdnFiltersChanged.Value = cookie.FiltersChanged.ToString();
            }
        }

        protected void btnUpdateView_OnClick(object sender, EventArgs e)
        {
            UpdateReport();
            updReport.Update();
        }

        public void UpdateReport()
        {
            InitChart();//InitChart() will initiate Y axis, Updates Title, and Updates Legends.

            LoadChart();//LoadChart() will create chart.

            SaveFilters();//Save filters in the cookie.
        }

        private void SaveFilters()
        {
            var filter = GetFilters();
            SerializationHelper.SerializeCookie(filter, Constants.FilterKeys.ConsultingDemandFilterCookie);
        }

        private ConsultingDemandFilter GetFilters()
        {
            var filter = new ConsultingDemandFilter();

            filter.FiltersChanged = hdnFiltersChanged.Value;
            filter.PeriodSelected = ddlPeriod.SelectedValue;
            filter.StartDate = StartDate;
            filter.EndDate = EndDate;

            return filter;
        }

        private void LoadChart()
        {
            var report = PraticeManagement.Controls.Reports.ReportsHelper.GetConsultantDemand(StartDate, EndDate);

            if (report.Count > 0)
            {
                report = CustomizeTheReport(report);

                ConsultingDemandSeries.Points.Clear();
                ConsultingDemandSeries.BackImageAlignment = ChartImageAlignmentStyle.Left;

                for (int personIndex = 0; personIndex < report.Count; personIndex++)
                {
                    AddConsultant(report[personIndex], personIndex);
                }

                SetChartHeightAndWidth(report.Count);

                SetChartAreaPosition(report.Where(r => r.ObjectName != null && r.Client != null && r.Client.Name != null).Count());
            }
        }

        private List<ConsultantDemandItem> CustomizeTheReport(List<ConsultantDemandItem> report)
        {
            var personIds = (report.Select(p => p.Consultant.Id.Value)).Distinct().ToList();

            //To Display Starwmans.
            foreach (var personId in personIds)
            {
                var consultant = report.First(r => r.Consultant.Id.Value == personId).Consultant;
                var dummyClient = new DataTransferObjects.Client();
                dummyClient.Name = DummyClientName;
                var subReport = new DataTransferObjects.ConsultantDemandItem
                {
                    Consultant = consultant,
                    ObjectType = 2, //Added this for the sort order.
                    Client = dummyClient//Added this for the sort order.
                };

                report.Add(subReport);//Adding subReports to display Strawman Name in the Xaxis label.
            }

            //To Display single bar correctly if the chart contains single bar.
            if (report.Where(r => r.Client != null && r.Client.Id.HasValue).Count() == 1)//To fix the issue of single bar in the chart, adding additional transparent bar.
            {
                var subReport = new DataTransferObjects.ConsultantDemandItem
                {
                    Consultant = new DataTransferObjects.Person { LastName = "z", FirstName = "" },
                    ObjectType = 1,
                    StartDate = StartDate,
                    EndDate = StartDate.AddDays(0.1),
                    QuantityString = ",",
                    ObjectName = " ",
                    Client = new DataTransferObjects.Client { Name = "Z" }
                };

                report.Add(subReport);
            }

            report = report.OrderByDescending(c => c.Consultant.PersonLastFirstName).ThenBy(c => c.ObjectType).ThenByDescending(c => c.Client.Name).ToList();

            return report;
        }

        private void SetChartHeightAndWidth(int reportCount)
        {
            chrtConsultingDemand.Height = Resources.Controls.TimelineGeneralHeaderHeigth +
                Resources.Controls.TimelineGeneralItemHeigth * reportCount +
                            Resources.Controls.TimelineGeneralFooterHeigth + 10;

            if (EndDate.Subtract(StartDate).Days >= 120)
            {
                chrtConsultingDemand.Width = EndDate.Subtract(StartDate).Days * 9;
            }
            else
            {
                chrtConsultingDemand.Width = 940; //(EndDate.Subtract(StartDate).Days * 11) - 10;
            }
        }

        private void SetChartAreaPosition(int barsCount)
        {
            var chartMainArea = chrtConsultingDemand.ChartAreas[MAIN_CHART_AREA_NAME];

            chartMainArea.Position = new ElementPosition(-2f, barsCount <= 7 ? 25 : 17, 100, barsCount <= 7 ? 65 : 75);// new ElementPosition(-1, 20, 100, 90);
            chartMainArea.AlignmentStyle = AreaAlignmentStyles.Position;
        }

        private void AddConsultant(ConsultantDemandItem item, int personIndex)
        {
            int day = 0;
            for (var date = StartDate; date <= EndDate; date = date.AddDays(1))
            {
                //  Add another range to the person's timeline
                if (item.StartDate <= date && item.EndDate >= date)
                {
                    AddRange(item, date, personIndex, day);
                }
                day = day + 1;
            }
            AddXAxisLabel(item, personIndex);
        }

        private void AddXAxisLabel(ConsultantDemandItem item, int personIndex)
        {
            var labels =
              chrtConsultingDemand.ChartAreas[MAIN_CHART_AREA_NAME].AxisX.CustomLabels;
            string demandItemName;


            if (string.IsNullOrEmpty(item.ObjectName))
            {
                //To display Strawman Name in the chart at left side.
                demandItemName = string.Format("{0}  {1}", Convert.ToChar(187), item.Consultant.PersonLastFirstName);
            }
            else
            {
                demandItemName = item.ObjectName;
            }

            var labelText = item.Client != null && item.Client.Id.HasValue ? string.Format(item.LinkedObjectId.HasValue ? DemandItemtextWithTwoObjectsLinksFormat : DemandItemtextWithSingleObjectLinkFormat, item.Client.Name.Length > 15 ? item.Client.Name.Substring(0, 12) + "..." : item.Client.Name, demandItemName.Length > 15 ? demandItemName.Substring(0, 12) + "..." : demandItemName)// item.ObjectNumber)
                : demandItemName.Length > 40 ? demandItemName.Substring(0, 37) + "..." : demandItemName;

            //  Create new label to display at left side of the Chart.
            var label =
                labels.Add(
                    personIndex - 0.49, // From position
                    personIndex + 0.49, // To position
                    labelText, // Formated person title
                   0, // Index
                    LabelMarkStyle.None); // Mark style: none

            label.ToolTip = item.Client != null && item.Client.Id.HasValue ? string.Format(DemandItemtextTooltipFormat, item.Client.Name, demandItemName)// item.ObjectNumber)
                : demandItemName;

            if (string.IsNullOrEmpty(item.ObjectName))
            {
                label.ForeColor = Color.DarkBlue;
                label.GridTicks = GridTickTypes.Gridline;//to display gridline on the chart when Strawman Name displays at left.
            }

            if (item.Client != null && item.Client.Id.HasValue)
            {
                //Add ObjectId Link to x axis label.
                AddObjectIdLink(labels, item, personIndex);
            }
        }

        private void AddObjectIdLink(CustomLabelsCollection labels, ConsultantDemandItem item, int personIndex)
        {
            //Adding objectNumberLabel label is for ObjectId Link.
            var objectNumberLabel = labels.Add(
                                            personIndex - 0.49, // From position
                                            personIndex + 0.49, // To position
                                            (!item.LinkedObjectId.HasValue) ? item.ObjectNumber : string.Format(DemandItemtextProjectLinkFormat, item.ObjectNumber), // Formated person title
                                           0, // Index
                                            LabelMarkStyle.None); // Mark style: none
            objectNumberLabel.ForeColor = Color.FromArgb(8, 152, 230); //Color.FromArgb(0, 102, 153); //a onhover
            objectNumberLabel.ToolTip = string.IsNullOrEmpty(item.ProjectDescription) ? item.OpportunintyDescription : item.ProjectDescription;
            objectNumberLabel.Url = item.ObjectType == 1 ?
                Urls.OpportunityDetailsLink(item.ObjectId, Constants.ApplicationPages.ConsultingDemandWithFilterQueryString)
                : Urls.GetProjectDetailsUrl(item.ObjectId, Constants.ApplicationPages.ConsultingDemandWithFilterQueryString);

            if (item.LinkedObjectId.HasValue)
            {
                var linkedOpportunityNumberLabel = labels.Add(
                                                personIndex - 0.49, // From position
                                                personIndex + 0.49, // To position
                                                item.LinkedObjectNumber, // Formated person title
                                               0, // Index
                                                LabelMarkStyle.None); // Mark style: none
                linkedOpportunityNumberLabel.ForeColor = Color.FromArgb(8, 152, 230);
                linkedOpportunityNumberLabel.ToolTip = item.OpportunintyDescription;
                linkedOpportunityNumberLabel.Url = Urls.OpportunityDetailsLink(item.LinkedObjectId.Value, Constants.ApplicationPages.ConsultingDemandWithFilterQueryString);
            }

        }

        private void AddRange(ConsultantDemandItem item, DateTime date, int personIndex, int dayIndex)
        {
            var start = date;
            var end = date.AddDays(1);

            if (!string.IsNullOrEmpty(item.ObjectName))
            {
                var range = AddRange(start, end, personIndex);

                //Add color to the Bar.
                AddColorToRangeBar(item, range);

                var dailyDemands = Utils.Generic.StringToIntArray(item.QuantityString);
                if (dailyDemands[dayIndex] > 0)
                {
                    range.Color = Color.Black;
                    var tooltip = string.Format(DemandToolTip, start.Date, dailyDemands[dayIndex]);
                    range.BorderWidth = 1;
                    range.BorderColor = Color.White;
                    range.MapAreaAttributes = "onmouseover=\"DisplayTooltip('" + tooltip + "');\" onmouseout=\"DisplayTooltip('');\" onblur=\"DisplayTooltip('');\"";

                    range.CustomProperties += "PointWidth=2";
                    //range.CustomProperties = "BarLabelStyle=Center";
                }
                else
                {
                    SetBarToolTip(item, range);
                }
            }
        }

        private void SetBarToolTip(ConsultantDemandItem item, DataPoint range)
        {
            var dailyDemands = Utils.Generic.StringToIntArray(item.QuantityString);//dailyDemands contains the range of values from Startdate and EndDate.
            if (item.ObjectName != " ")//To eliminate tooltip for transparent bar(it will present if the report contains only one bar).
            {
                var tooptip = GetToolTip(item, dailyDemands);
                range.MapAreaAttributes = "onmouseover=\"DisplayTooltip('" + tooptip + "');\" onmouseout=\"DisplayTooltip('');\" onblur=\"DisplayTooltip('');\"";
            }
        }

        private void AddColorToRangeBar(ConsultantDemandItem item, DataPoint range)
        {
            //Get color by consulting Demand.
            range.Color = Coloring.GetColorByConsultingDemand(item);
        }

        private string GetToolTip(ConsultantDemandItem item, int[] dailyDemands)
        {
            var tooltip = "Total Demand by Month<br/>-----------------------------";
            int i = 0, day = 0;

            var startDate = new DateTime(StartDate.Year, StartDate.Month, 1);
            var endDate = new DateTime(EndDate.Year, EndDate.Month, 1);
            endDate = endDate.AddMonths(1).AddDays(-1);

            for (var date = startDate; date <= endDate; date = date.AddMonths(1), i++)
            {
                var itemStartDate = new DateTime(item.StartDate.Year, item.StartDate.Month, 1);
                var itemEndDate = new DateTime(item.EndDate.Year, item.EndDate.Month, 1);
                itemEndDate = itemEndDate.AddMonths(1).AddDays(-1);

                int monthlyDemand = 0;
                //Sum up the demand for the month.
                for (var perDay = date; perDay <= date.AddMonths(1).AddDays(-1); perDay = perDay.AddDays(1))
                {
                    if (perDay >= StartDate && perDay <= EndDate)
                    {
                        //dailyDemands will exist only in Selected Date range, so we need to consider only selected range instead of startdate of the month.
                        if (item.StartDate <= perDay && item.EndDate >= perDay)
                        {
                            //var index = perDay.Subtract(item.StartDate).Days;
                            monthlyDemand = monthlyDemand + dailyDemands[day];
                        }
                        day = day + 1;
                    }
                }

                if (itemStartDate <= date && itemEndDate >= date)
                {
                    tooltip += "<br/>" + String.Format("{0:MMMM yyyy} = {1}", date, monthlyDemand);
                }
            }
            return tooltip;
        }

        private DataPoint AddRange(DateTime pointStartDate, DateTime pointEndDate, double xvalue)
        {
            var ind = ConsultingDemandSeries.Points.AddXY(
                xvalue,
                pointStartDate,
                pointEndDate);

            return ConsultingDemandSeries.Points[ind];
        }

        private void InitChart()
        {
            InitAxis(chrtConsultingDemand.ChartAreas[MAIN_CHART_AREA_NAME].AxisY);
            InitAxis(chrtConsultingDemand.ChartAreas[MAIN_CHART_AREA_NAME].AxisY2);

            UpdateChartTitle();

            InitLegends();
        }

        private void UpdateChartTitle()
        {
            chrtConsultingDemand.Titles.Clear();
            chrtConsultingDemand.Titles.Add(string.Format(Title_Format, StartDate.ToString("MM/dd/yyyy"), EndDate.ToString("MM/dd/yyyy")));
            chrtConsultingDemand.Titles[0].Font = new Font("Arial", 10, FontStyle.Bold);
        }

        private void InitLegends()
        {
            foreach (var legend in chrtConsultingDemand.Legends)
            {
                Coloring.DemandColorLegends(legend);
            }
        }

        private void InitAxis(Axis horizAxis)
        {
            //  Set min and max values
            //horizAxis.ValueToPosition(200);
            horizAxis.Minimum = StartDate.ToOADate();
            horizAxis.Maximum = EndDate.AddDays(1).ToOADate();
            horizAxis.IsLabelAutoFit = true;
            horizAxis.IsStartedFromZero = true;

            horizAxis.IntervalType = DateTimeIntervalType.Days;
            horizAxis.Interval = EndDate.Subtract(StartDate).Days;

            horizAxis.TextOrientation = TextOrientation.Horizontal;
            horizAxis.LabelStyle.Angle = 0;

            //Draw a stripline for every end of the month.
            //Display Month Name above the axis.
            //Draw lines on 1st, 15th of every month(for february its 1st,14th), and end of the month.
            PrepareChartSheet(horizAxis);
        }

        private void PrepareChartSheet(Axis horizAxis)
        {
            var startDate = new DateTime(StartDate.Year, StartDate.Month, 1);
            var endDate = new DateTime(EndDate.Year, EndDate.Month, 1);
            endDate = endDate.AddMonths(1).AddDays(-1);
            DateTime previousEndDate = startDate;//Added previousEndDate variable for the custom dates(i.e if selected date is not start or end date of the month).

            for (var date = startDate; date <= endDate; date = date.AddMonths(1))
            {
                //Set Month differentiator.
                var stripLine = SetStripLine(date, Color.Blue, 2, ChartDashStyle.Solid);
                stripLine.ToolTip = date.ToString(FULL_MONTH_NAME_FORMAT);
                horizAxis.StripLines.Add(stripLine);

                var start = (date.Year == StartDate.Year && date.Month == StartDate.Month) ? StartDate : previousEndDate.AddDays(1);
                previousEndDate = date.AddMonths(1).AddDays(-1);
                var end = (date.Year == EndDate.Year && date.Month == EndDate.Month) ? EndDate : previousEndDate;

                //Set Lines at 1st, 15th of the month.
                SetChartLines(horizAxis, start, end, startDate, endDate);

                //Set Month Name.
                var label = horizAxis.CustomLabels.Add(
                            start.ToOADate(),
                            end.ToOADate(),
                            start.ToString(FULL_MONTH_NAME_FORMAT).ToUpper(),
                            1,
                            LabelMarkStyle.None);
            }
        }

        private void SetChartLines(Axis horizAxis, DateTime monthStart, DateTime monthEnd, DateTime startDate, DateTime endDate)
        {
            if (monthStart <= StartDate)
            {
                var startlineLabel = horizAxis.CustomLabels.Add(
                                    monthStart.AddDays(-1.3).ToOADate(),
                                    monthStart.AddDays(1.3).ToOADate(),
                                    monthStart.Day.ToString(),
                                    0,
                                    LabelMarkStyle.None);

                var monthStartLine = SetStripLine(monthStart, Color.Black, 1, ChartDashStyle.Solid);
                horizAxis.StripLines.Add(monthStartLine);
            }

            var intervalDays = ((monthEnd.Subtract(monthStart).Days - 1) / 2);
            var monthMiddle = monthStart.AddDays(intervalDays);
            var monthMiddlelabel = horizAxis.CustomLabels.Add(
                        monthMiddle.AddDays(-1.3).ToOADate(),
                        monthMiddle.AddDays(1.3).ToOADate(),
                        monthMiddle.Day.ToString(),
                        0,
                        LabelMarkStyle.None);

            var monthMiddleLine = SetStripLine(monthMiddle, Color.Black, 1, ChartDashStyle.Solid);
            horizAxis.StripLines.Add(monthMiddleLine);


            var label = horizAxis.CustomLabels.Add(
                        monthEnd.AddDays(-1.3).ToOADate(),
                        monthEnd.AddDays(1.3).ToOADate(),
                        monthEnd.Day.ToString(),// monthEnd.ToString(DAY_FORMAT),
                        0,
                        LabelMarkStyle.None);

            if (monthEnd < endDate)
            {
                var monthEndLine = SetStripLine(monthEnd, Color.Black, 1, ChartDashStyle.Solid);
                horizAxis.StripLines.Add(monthEndLine);
            }
        }

        private StripLine SetStripLine(DateTime date, Color stripLineColor, int stripLineWidth, ChartDashStyle stipLineBorderDashStyle)
        {
            StripLine stripLine = new StripLine();
            stripLine.BorderColor = stripLineColor;
            stripLine.BorderWidth = stripLineWidth;
            stripLine.StripWidthType = DateTimeIntervalType.Days;
            stripLine.Interval = 0;
            stripLine.IntervalOffset = date.ToOADate();
            stripLine.BorderDashStyle = stipLineBorderDashStyle;

            return stripLine;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            diRange.FromDate = StartDate;
            diRange.ToDate = EndDate;
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
            var tbFrom = diRange.FindControl("tbFrom") as TextBox;
            var tbTo = diRange.FindControl("tbTo") as TextBox;
            var clFromDate = diRange.FindControl("clFromDate") as AjaxControlToolkit.CalendarExtender;
            var clToDate = diRange.FindControl("clToDate") as AjaxControlToolkit.CalendarExtender;
            tbFrom.Attributes.Add("onchange", "ChangeStartEndDates();");
            tbTo.Attributes.Add("onchange", "ChangeStartEndDates();");
            hdnStartDateTxtBoxId.Value = tbFrom.ClientID;
            hdnEndDateTxtBoxId.Value = tbTo.ClientID;
            hdnStartDateCalExtenderBehaviourId.Value = clFromDate.BehaviorID;
            hdnEndDateCalExtenderBehaviourId.Value = clToDate.BehaviorID;

            btnResetFilter.Enabled = (hdnFiltersChanged.Value == "1");
            btnResetFilter.Attributes.Add("onclick", "if(!ResetFilters(this)){return false;}");
            hdnDefaultStartDate.Value = DefaultStartDate.ToString("MM/dd/yyyy");
            hdnDefaultEndDate.Value = DefaultEndDate.ToString("MM/dd/yyyy");
        }
    }
}

