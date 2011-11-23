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

namespace PraticeManagement.Reporting
{
    public partial class ConsultingDemand : System.Web.UI.Page
    {
        #region Constants

        private const string WEEKS_SERIES_NAME = "Weeks";
        private const string MAIN_CHART_AREA_NAME = "MainArea";
        private const string DemandItemtextFormat = "{0} - {1} -         {2}"; // 7 spaces between 1 and 2;
        private const string DemandItemtextTooltipFormat = "{0} - {1}";
        private const string RedirectFormat = "{0}?id={1}";
        private const string EmptyObjectNumberFormat = "       {0}"; // 7 spaces
        private const string Title_Format = "Consulting Demand Report \n{0} to {1}";
        private const string DummyClientName = "!";
        private const string ChartSeriesId = "chartSeries";
        private const string FULL_MONTH_NAME_FORMAT = "MMMM, yyyy";

        #endregion

        #region Properties

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
                return date.Date.AddDays((date.Day) * -1).AddMonths(4);
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

        //public int ObjectMaxLength
        //{
        //    get;
        //    set;
        //}

        //public int ConsultantMaxLength
        //{
        //    get;
        //    set;s
        //}

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
                    }
                }
                else
                {
                    SetDefaultDateRange();//4 months(i.e current month + 3 months) is the default date range.
                }
            }
            UpdateReport();
        }

        private void SetDefaultDateRange()
        {
            var currentDateTime = SettingsHelper.GetCurrentPMTime();
            diRange.FromDate = new DateTime(currentDateTime.Year, currentDateTime.Month, 1);
            diRange.ToDate = diRange.FromDate.Value.AddMonths(5).AddDays(-1);
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
            else
            {
                SetDefaultDateRange();
            }
        }

        protected void btnUpdateView_OnClick(object sender, EventArgs e)
        {
            UpdateReport();
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
                var personIds = (report.Select(p => p.Consultant.Id.Value)).Distinct().ToList();

                //ObjectMaxLength = report.Max(r => r.ObjectName.Length);

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

                if (report.Where(r => r.Client != null && r.Client.Id.HasValue).Count() == 1)//To fix the issue of single bar in the chart, adding additional transparent bar.
                {
                    var subReport = new DataTransferObjects.ConsultantDemandItem
                    {
                        Consultant = new DataTransferObjects.Person { LastName = "", FirstName = "" },
                        ObjectType = 1,
                        StartDate = StartDate,
                        EndDate = StartDate.AddDays(0.1),
                        QuantityString = ",",
                        ObjectName = " ",
                        Client = new DataTransferObjects.Client { Name = "@" }
                    };

                    report.Add(subReport);
                }

                report = report.OrderByDescending(c => c.Consultant.PersonLastFirstName).ThenBy(c => c.ObjectType).ThenByDescending(c => c.Client.Name).ToList();
                
                //ConsultantMaxLength = report.Max(r => r.Consultant.PersonLastFirstName.Length);
                
                ConsultingDemandSeries.Points.Clear();

                for (int personIndex = 0; personIndex < report.Count; personIndex++)
                {
                    AddConsultant(report[personIndex], personIndex);
                }

                chrtConsultingDemand.Height = Resources.Controls.TimelineGeneralHeaderHeigth +
                                Resources.Controls.TimelineGeneralItemHeigth * report.Count +
                                Resources.Controls.TimelineGeneralFooterHeigth;

                if (EndDate.Subtract(StartDate).Days >= 120)
                {
                    chrtConsultingDemand.Width = EndDate.Subtract(StartDate).Days * 30;
                }
                else
                {
                    chrtConsultingDemand.Width = EndDate.Subtract(StartDate).Days * 40;
                }

                    //chrtConsultingDemand.Style.Add("text-align", "left");
                    //chrtConsultingDemand.BackImageAlignment = ChartImageAlignmentStyle.Left;

                    var chartMainArea = chrtConsultingDemand.ChartAreas[MAIN_CHART_AREA_NAME];
                    //chartMainArea.AlignmentStyle = AreaAlignmentStyles.Position;
                    //chartMainArea.AlignmentOrientation = AreaAlignmentOrientations.Horizontal;
                    //chartMainArea.InnerPlotPosition = new ElementPosition(6, 3, 100, 85);
                    //float height;
                    //if (float.TryParse(chrtConsultingDemand.Height.Value.ToString(), height))
                    //{
                    //    height = 100;
                    //}
                    //var element = new ElementPosition();
                    //element.X = 0;
                    //element.Y = 15;
                    //element.Width = 100;
                    //element.Height = 75;

                    chartMainArea.Position = new ElementPosition(-1, 15, 100, 75);// new ElementPosition(-1, 20, 100, 90);
            }
        }

        private void AddConsultant(DataTransferObjects.ConsultantDemandItem item, int personIndex)
        {
            int day = 0;
            for (var date = StartDate; date <= EndDate; date = date.AddDays(1))
            {
                //  Add another range to the person's timeline
                if (item.StartDate <= date && item.EndDate >= date)
                {
                    //day = date.Subtract(item.StartDate).Days;
                    //day = item.StartDate.Subtract(date).Days;
                    AddRange(item, date, personIndex, day);
                    //day = day + 1;
                }
                day = day + 1;
            }
            AddXAxisLabel(item, personIndex);
        }

        private void AddXAxisLabel(DataTransferObjects.ConsultantDemandItem item, int personIndex)
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

            string emptyObjectNumber = string.Format(EmptyObjectNumberFormat, '.');

            var labelText = item.Client != null && item.Client.Id.HasValue ? string.Format(DemandItemtextFormat, item.Client.Name.Length > 15 ? item.Client.Name.Substring(0, 12) + "..." : item.Client.Name, demandItemName.Length > 15 ? demandItemName.Substring(0, 12) + "..." : demandItemName, emptyObjectNumber)// item.ObjectNumber)
                : demandItemName;

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
                //Adding objectNumberLabel label is for ObjectId Link.
                var objectNumberLabel = labels.Add(
                                                personIndex - 0.49, // From position
                                                personIndex + 0.49, // To position
                                                item.ObjectNumber, // Formated person title
                                               0, // Index
                                                LabelMarkStyle.None); // Mark style: none
                objectNumberLabel.ForeColor = Color.FromArgb(8, 152, 230); //Color.FromArgb(0, 102, 153); //a onhover
                objectNumberLabel.ToolTip = item.ObjectNumber;
                objectNumberLabel.Url = item.ObjectType == 1 ?
                    Urls.OpportunityDetailsLink(item.ObjectId)
                    : Urls.GetProjectDetailsUrl(item.ObjectId,
                                                (hdnFiltersChanged.Value == "1") ?
                                                Constants.ApplicationPages.ConsultingDemandWithFilterQueryString : Constants.ApplicationPages.ConsultingDemand
                                                );
            }
        }

        private void AddRange(DataTransferObjects.ConsultantDemandItem item, DateTime date, int personIndex, int dayIndex)
        {
            var start = date;
            var end = date.AddDays(1);

            if (!string.IsNullOrEmpty(item.ObjectName))
            {
                var range = AddRange(start, end, personIndex);

                //Add color to the Bar.
                AddColorToRangeBar(item, range);

                var dailyDemands = Utils.Generic.StringToIntArray(item.QuantityString);//dailyDemands contains the range of values from Startdate and EndDate.

                //Add text on the Range Bar.
                var rangeText = AddRange(start, end.AddDays(-0.9), personIndex);
                rangeText.Color = range.Color;
                rangeText.Label = dailyDemands[dayIndex] == 0 ? string.Empty : dailyDemands[dayIndex].ToString();
                rangeText.LabelToolTip = string.Format("{0:dd-MMM} : {1}", start, dailyDemands[dayIndex]);

                //Add Tooltip on the Range Bar.
                if (item.ObjectName != " ")//To eliminate tooltip for transparent bar(it will present if the report contains only one bar).
                {
                    var tooptip = GetToolTip(item, dailyDemands);
                    range.MapAreaAttributes = "onmouseover=\"DisplayTooltip('" + tooptip + "');\" onmouseout=\"DisplayTooltip('');\" onblur=\"DisplayTooltip('');\"";
                }
            }
        }

        private void AddColorToRangeBar(DataTransferObjects.ConsultantDemandItem item, DataPoint range)
        {
            //Get color by consulting Demand.
            range.Color = Coloring.GetColorByConsultingDemand(item);
        }

        private string GetToolTip(DataTransferObjects.ConsultantDemandItem item, int[] dailyDemands)
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

                if (itemStartDate <= date && itemEndDate >= date)
                {
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
            horizAxis.Interval = 1;

            var startDate = new DateTime(StartDate.Year, StartDate.Month, 1);
            var endDate = new DateTime(EndDate.Year, EndDate.Month, 1);
            endDate = endDate.AddMonths(1).AddDays(-1);
            DateTime previousEndDate = startDate;

            for (var date = startDate; date <= endDate; date = date.AddMonths(1))
            {
                var start = (date.Year == StartDate.Year && date.Month == StartDate.Month) ? StartDate : previousEndDate.AddDays(1);
                previousEndDate = date.AddMonths(1).AddDays(-1);
                var end = (date.Year == EndDate.Year && date.Month == EndDate.Month) ? EndDate : previousEndDate;

                var label = horizAxis.CustomLabels.Add(
                            start.ToOADate(),
                            end.ToOADate(),
                            start.ToString(FULL_MONTH_NAME_FORMAT).ToUpper(),
                            1,
                            LabelMarkStyle.None);
            }
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
                lblCustomDateRange.Attributes.Add("class", "");
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
