using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using System.Web.Security;
using System.Globalization;

namespace PraticeManagement.Controls.TimeEntry
{
    public partial class WeekSelector_New  : PracticeManagementUserControl
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

        public delegate void DatePickerChangedHandler(object sender, EventArgs args);

        #endregion

        public event DatePickerChangedHandler DatePickerChanged;

        public event WeekChangedHandler WeekChanged;

        #endregion

        #region Fields

        private static readonly List<DayOfWeek> AllowedDays;

        #endregion

        #region Properties

        public DateTime? PreviousWeekSelectedDay
        {
            get
            {
                DateTime day;
                DateTime.TryParse(hdPreviousWeekSelectedDay.Value, out day);
                return day;
            }
            set
            {
                hdPreviousWeekSelectedDay.Value = value.ToString();
            }

        }

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

        private PraticeManagement.TimeEntry_New HostingPage
        {
            get { return ((PraticeManagement.TimeEntry_New)Page); }
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

        static WeekSelector_New()
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

            SetSelectedDates();
            if (!Page.IsPostBack)
            {
                var currentPerson = DataHelper.CurrentPerson;
                var personId = currentPerson.Id.Value;
                string strSelectedPersonId = Request.QueryString["SelectedPersonId"];
                if (!string.IsNullOrEmpty(strSelectedPersonId) && !Int32.TryParse(strSelectedPersonId, out personId))
                {
                    personId = currentPerson.Id.Value;
                }
                bool isPersonActive = ServiceCallers.Custom.Person(p => p.IsPersonHaveActiveStatusDuringThisPeriod(personId, StartDate, EndDate));
                if (isPersonActive)
                {
                    var userIsAdministrator = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
                    if (userIsAdministrator)
                    {
                        var selectedValue = HostingPage.ddlPersonsDropDown.SelectedValue;
                        var persons = ServiceCallers.Custom.Person(p => p.PersonsListHavingActiveStatusDuringThisPeriod(StartDate, EndDate));
                        DataHelper.FillPersonList(HostingPage.ddlPersonsDropDown, null, persons, null);
                        HostingPage.ddlPersonsDropDown.SelectedValue = personId.ToString();
                        HostingPage.SelectedPerson = personId == currentPerson.Id.Value ? currentPerson : DataHelper.GetPerson(personId);
                        PreviousWeekSelectedDay = StartDate;
                    }
                }
                else
                {
                    if (PreviousWeekSelectedDay.HasValue)
                    {
                        HostingPage.SelectedPerson = personId == currentPerson.Id.Value ? currentPerson : DataHelper.GetPerson(personId);
                        var message = string.Format(TimeEntry_New.WeekChangeAlertMessage, HostingPage.SelectedPerson.PersonLastFirstName, StartDate, EndDate);
                        HostingPage.lbMessageControl.Text = message;
                        HostingPage.mpePersonInactiveAlertControl.Show();
                        OnWeekChanged(PreviousWeekSelectedDay.Value);
                    }
                }

            }
            UpdateWeekLabel();

        }

        protected void imgbtnPrevWeek_OnClick(object sender, ImageClickEventArgs e)
        {
            bool result = true;

            if (HostingPage.IsDirty)
            {
               result = HostingPage.SaveData();
            }

            if (result)
            {
                DateTime pervWeekStartDate = StartDate.AddDays(-SelectedPeriodLength);
                DateTime pervWeekEndDate = EndDate.AddDays(-SelectedPeriodLength);
                int personId;

                var userIsAdministrator = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);

                if (userIsAdministrator)
                {
                    personId = int.Parse(HostingPage.ddlPersonsDropDown.SelectedValue);
                }
                else
                {
                    personId = DataHelper.CurrentPerson.Id.Value;
                }


                bool isPersonActive = ServiceCallers.Custom.Person(p => p.IsPersonHaveActiveStatusDuringThisPeriod(personId, pervWeekStartDate, pervWeekEndDate));

                if (!isPersonActive)
                {
                    var message = string.Format(TimeEntry_New.WeekChangeAlertMessage, HostingPage.SelectedPerson.PersonLastFirstName, pervWeekStartDate.ToString("yyyy/MM/dd"), pervWeekEndDate.ToString("yyyy/MM/dd"));
                    HostingPage.lbMessageControl.Text = message;
                    HostingPage.mpePersonInactiveAlertControl.Show();
                }
                else
                {
                    var urlTemplate = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName) ?
                        Constants.ApplicationPages.TimeEntry_NewForAdmin : Constants.ApplicationPages.TimeEntry_New;
                    string url = string.Format(urlTemplate, pervWeekStartDate.ToString("yyyy-MM-dd"), SelectedPerson.Id);
                    Response.Redirect(url);
                }
            }
        }

        protected void imgbtnNextWeek_OnClick(object sender, ImageClickEventArgs e)
        {
              bool result = true;

            if (HostingPage.IsDirty)
            {
               result = HostingPage.SaveData();
            }

            if (result)
            {
                DateTime nextWeekStartDate = StartDate.AddDays(SelectedPeriodLength);
                DateTime nextWeekEndDate = EndDate.AddDays(SelectedPeriodLength);
                int personId;
                var userIsAdministrator = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
                if (userIsAdministrator)
                {
                    personId = int.Parse(HostingPage.ddlPersonsDropDown.SelectedValue);
                }
                else
                {
                    personId = DataHelper.CurrentPerson.Id.Value;
                }

                bool isPersonActive = ServiceCallers.Custom.Person(p => p.IsPersonHaveActiveStatusDuringThisPeriod(personId, nextWeekStartDate, nextWeekEndDate));
                if (!isPersonActive)
                {
                    var message = string.Format(TimeEntry_New.WeekChangeAlertMessage, HostingPage.SelectedPerson.PersonLastFirstName, nextWeekStartDate.ToString("yyyy-MM-dd"), nextWeekEndDate.ToString("yyyy-MM-dd"));
                    HostingPage.lbMessageControl.Text = message;
                    HostingPage.mpePersonInactiveAlertControl.Show();
                }
                else
                {
                    var urlTemplate = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName) ?
                        Constants.ApplicationPages.TimeEntry_NewForAdmin : Constants.ApplicationPages.TimeEntry_New;
                    string url = string.Format(urlTemplate, nextWeekStartDate.ToString("yyyy-MM-dd"), SelectedPerson.Id);
                    Response.Redirect(url);
                }
            }
        }

        private void BindPersonsList()
        {
            bool isPersonActive = true;
            var currentPerson = DataHelper.CurrentPerson;
            var personId = currentPerson.Id.Value;
            string strSelectedPersonId = Request.QueryString["SelectedPersonId"];
            string strSelectedDate = Request.QueryString["day"];
            if (!string.IsNullOrEmpty(HostingPage.ddlPersonsDropDown.SelectedValue))
            {
                personId = int.Parse(HostingPage.ddlPersonsDropDown.SelectedValue);
            }
            else
            {
                if (!string.IsNullOrEmpty(strSelectedPersonId) && !Int32.TryParse(strSelectedPersonId, out personId))
                {
                    personId = currentPerson.Id.Value;
                }
            }
            isPersonActive = ServiceCallers.Custom.Person(p => p.IsPersonHaveActiveStatusDuringThisPeriod(personId, StartDate, EndDate));
            if (!isPersonActive)
            {
                if (PreviousWeekSelectedDay.HasValue)
                {
                    HostingPage.SelectedPerson = personId == currentPerson.Id.Value ? currentPerson : DataHelper.GetPerson(personId);
                    var message = string.Format(TimeEntry_New.WeekChangeAlertMessage, HostingPage.SelectedPerson.PersonLastFirstName, StartDate.ToString("yyyy/MM/dd"), EndDate.ToString("yyyy/MM/dd"));
                    HostingPage.lbMessageControl.Text = message;
                    HostingPage.mpePersonInactiveAlertControl.Show();
                    if (PreviousWeekSelectedDay.Value == DateTime.MinValue)
                        PreviousWeekSelectedDay = Convert.ToDateTime(string.IsNullOrEmpty(strSelectedDate) ? DateTime.Now.ToShortDateString() : strSelectedDate);
                    OnWeekChanged(PreviousWeekSelectedDay.Value);
                }
            }
            else
            {
                var userIsAdministrator = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
                if (userIsAdministrator)
                {
                    var selectedValue = HostingPage.ddlPersonsDropDown.SelectedValue;
                    var persons = ServiceCallers.Custom.Person(p => p.PersonsListHavingActiveStatusDuringThisPeriod(StartDate, EndDate));
                    DataHelper.FillPersonList(HostingPage.ddlPersonsDropDown, null, persons, null);
                    HostingPage.ddlPersonsDropDown.SelectedValue = selectedValue;
                }
                PreviousWeekSelectedDay = StartDate;
            }
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
            BindPersonsList();
            WeekChanged(this, new WeekChangedEventArgs(StartDate, EndDate));
        }

        private void InitSelection(DateTime day)
        {
            SelectPeriod(Utils.Generic.GetWeekStartDate(day));

            UpdateWeekLabel();
            SetSelectedDates();

            calendar.VisibleDate = EndDate;
            txtDate.Text = StartDate.ToShortDateString();
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

        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            DatePickerChanged(sender, e);
        }


        private void ShiftAndUpdate(int shiftBy)
        {
            OnWeekChanged(calendar.SelectedDates[0].AddDays(shiftBy));
        }

        protected void calendar_OnVisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {
            OnWeekChanged(e.NewDate);
        }

        public void SetDate(DateTime dateValue)
        {
            calendar.SelectedDate = dateValue;
            OnWeekChanged(calendar.SelectedDate);
        }
    }
}

