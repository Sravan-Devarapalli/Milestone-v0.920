using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using System.Web.Security;

namespace PraticeManagement.Controls.TimeEntry
{
    public partial class WeekSelector : PracticeManagementUserControl
    {
        #region Constants

        private const string WeekLabelFormatStandard = "{0} {1} – {2} {3}";
        private const string START_DATE = "START_DATE";
        private const string END_DATE = "END_DATE";
        private const byte DefaultSelectedPeriod = 7;
        private const string ViewStateSelectedPeriod = "SelectedPeriodLength";

        #endregion

        #region Events

        #region Delegates

        public delegate void WeekChangedHandler(object sender, WeekChangedEventArgs args);

        #endregion

        public event WeekChangedHandler WeekChanged;

        #endregion

        #region Fields

        private static readonly List<DayOfWeek> AllowedDays;

        #endregion

        #region Properties

        public byte SelectedPeriodLength
        {
            get { return GetViewStateValue(ViewStateSelectedPeriod, DefaultSelectedPeriod); }
            set { SetViewStateValue(ViewStateSelectedPeriod, value); }
        }

        public DateTime[] SelectedDates
        {
            get
            {
                var res = new List<DateTime>();

                foreach (DateTime day in calendar.SelectedDates)
                    if (AllowedDays.Contains(day.DayOfWeek))
                        res.Add(day);

                return res.ToArray();
            }
        }

        public DateTime SelectedStartDate
        {
            get
            {
                try
                {
                    return (DateTime)ViewState[START_DATE];
                }
                catch
                {
                    return DateTime.Now;
                }
            }

            set { ViewState[START_DATE] = value; }
        }

        public DateTime SelectedEndDate
        {
            get
            {
                try
                {
                    return (DateTime)ViewState[END_DATE];
                }
                catch
                {
                    return DateTime.Now;
                }
            }

            set { ViewState[END_DATE] = value; }
        }

        private DateTime EndDate
        {
            get { return calendar.SelectedDates[calendar.SelectedDates.Count - 1]; }
        }

        private DateTime StartDate
        {
            get { return calendar.SelectedDates[0]; }
        }

        private PraticeManagement.TimeEntry HostingPage
        {
            get { return ((PraticeManagement.TimeEntry)Page); }
        }

        public Person SelectedPerson
        {
            get
            {
                return HostingPage.SelectedPerson;
            }
        }

        #endregion

        #region Constuctors

        static WeekSelector()
        {
            AllowedDays = new List<DayOfWeek>
                              {
                                  DayOfWeek.Sunday,
                                  DayOfWeek.Monday,
                                  DayOfWeek.Tuesday,
                                  DayOfWeek.Wednesday,
                                  DayOfWeek.Thursday,
                                  DayOfWeek.Friday,
                                  DayOfWeek.Saturday                                  
                              };
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string stringDay = Request.QueryString["day"];
                DateTime day = DateTime.Now;
                if (!string.IsNullOrEmpty(stringDay) && !DateTime.TryParse(stringDay, out day))
                {
                    day = DateTime.Now;
                }
                InitSelection(day);
            }

            UpdateWeekLabel();
            SetSelectedDates();
        }

        private void SelectPeriod(DateTime startDate)
        {
            calendar.SelectedDates.Clear();
            //calendar.SelectedDates.SelectRange(startDate, startDate.AddDays(6));
            for (var i = 0; i < SelectedPeriodLength; i++)
                calendar.SelectedDates.Add(startDate.AddDays(i));
        }

        public void UpdateWeekLabel()
        {
            DateTime today = StartDate;
            CultureInfo curentCulture = CultureInfo.CurrentUICulture;
            DateTime endMonth = EndDate;
            lblWeek.Text =
                string.Format(WeekLabelFormatStandard,
                              curentCulture.DateTimeFormat.GetAbbreviatedMonthName(today.Month),
                              today.Day,
                              curentCulture.DateTimeFormat.GetAbbreviatedMonthName(endMonth.Month),
                              endMonth.Day);
            var urlTemplate = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName) ?
                Constants.ApplicationPages.TimeEntryForAdmin : Constants.ApplicationPages.TimeEntry;
            hprlnkPreviousWeek.NavigateUrl = string.Format(urlTemplate, today.AddDays(-SelectedPeriodLength).ToString("yyyy-MM-dd"), SelectedPerson.Id);
            hprlnkNextWeek.NavigateUrl = string.Format(urlTemplate, today.AddDays(SelectedPeriodLength).ToString("yyyy-MM-dd"), SelectedPerson.Id);
        }

        public static int Week(DateTime tdDate)
        {
            var myCi = CultureInfo.CurrentUICulture;
            var myCal = myCi.Calendar;
            var myCwr = myCi.DateTimeFormat.CalendarWeekRule;
            var myFirstDow = myCi.DateTimeFormat.FirstDayOfWeek;
            return myCal.GetWeekOfYear(tdDate, myCwr, myFirstDow);
        }

        protected void calendar_DayRender(object sender, DayRenderEventArgs e)
        {
            e.Cell.Text = e.Day.Date.Day.ToString();
        }

        protected virtual void OnWeekChanged(DateTime day)
        {
            InitSelection(day);

            WeekChanged(this, new WeekChangedEventArgs(StartDate, EndDate));
        }

        private void InitSelection(DateTime day)
        {
            SelectPeriod(Utils.Generic.GetWeekStartDate(day));

            UpdateWeekLabel();
            SetSelectedDates();

            calendar.VisibleDate = EndDate;
        }

        private void SetSelectedDates()
        {
            SelectedEndDate = EndDate;
            SelectedStartDate = StartDate;
        }

        protected void calendar_SelectionChanged(object sender, EventArgs e)
        {
            OnWeekChanged(calendar.SelectedDate);
        }

        //protected void btnPreviousWeek_Click(object sender, ImageClickEventArgs e)
        //{
        //    ShiftAndUpdate(-DefaultSelectedPeriod);
        //}

        private void ShiftAndUpdate(int shiftBy)
        {
            OnWeekChanged(calendar.SelectedDates[0].AddDays(shiftBy));
        }

        //protected void btnNextWeek_Click(object sender, ImageClickEventArgs e)
        //{
        //    ShiftAndUpdate(DefaultSelectedPeriod);
        //}

        protected void calendar_OnVisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {
            OnWeekChanged(e.NewDate);
        }
    }
}
