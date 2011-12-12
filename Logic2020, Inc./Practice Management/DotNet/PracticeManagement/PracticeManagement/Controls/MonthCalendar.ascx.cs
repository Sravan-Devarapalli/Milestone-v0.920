using System;
using System.ServiceModel;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.CalendarService;
using System.Web;
using PraticeManagement.Utils;

namespace PraticeManagement.Controls
{
    public partial class MonthCalendar : System.Web.UI.UserControl
    {
        #region Constants

        private const string YearKey = "Year";
        private const string MonthKey = "Month";
        private const string PersonIdKey = "PersonId";
        private const string ValueArgument = "value";
        private const string FloatingHoliday = "Floating Holiday";
        private const string PTOToolTipFormat = "PTO - {0}Hrs";

        #endregion

        #region Fields

        private bool wasChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gest or sets a year to be displayed.
        /// </summary>
        public int Year
        {
            get
            {
                return Convert.ToInt32(ViewState[YearKey]);
            }
            set
            {
                if (value < DateTime.MinValue.Year || value > DateTime.MaxValue.Year)
                {
                    throw new ArgumentException(
                        string.Format(Resources.Messages.InvalidYearNumber,
                        DateTime.MinValue.Year,
                        DateTime.MaxValue.Year),
                        ValueArgument);
                }

                wasChanged = wasChanged || Year != value;
                ViewState[YearKey] = value;
            }
        }

        /// <summary>
        /// Gets or sets a month to be displayed.
        /// </summary>
        public int Month
        {
            get
            {
                return Convert.ToInt32(ViewState[MonthKey]);
            }
            set
            {
                if (value < 1 || value > 12)
                {
                    throw new ArgumentException(
                        string.Format(Resources.Messages.InvalidMonthNumber, 1, 12),
                        ValueArgument);
                }

                wasChanged = wasChanged || Month != value;
                ViewState[MonthKey] = value;
            }
        }

        public int? PersonId
        {
            get
            {
                return (int?)ViewState[PersonIdKey];
            }
            set
            {
                wasChanged = wasChanged || PersonId != value;
                ViewState[PersonIdKey] = value;
            }
        }

        public CalendarItem[] CalendarItems
        {
            private get;
            set;
        }
        public bool IsReadOnly
        {
            get
            {
                return ViewState["IsReadOnly"] == null ? true : (bool)ViewState["IsReadOnly"];
            }
            set
            {
                ViewState["IsReadOnly"] = value;
            }
        }

        public bool GetIsReadOnly(bool DateLevelReadonly)
        {
            if (IsPersonCalendar)
            {
                if (IsReadOnly)
                {
                    return true;
                }
                else
                {
                    return DateLevelReadonly;
                }
            }
            else
            {
                return DateLevelReadonly;

            }
        }
        public bool IsPersonCalendar
        {
            get
            {
                return ViewState["PersonCalendarKey"] == null ? false : (bool)ViewState["PersonCalendarKey"];
            }
            set
            {
                ViewState["PersonCalendarKey"] = value;
            }
        }

        #endregion

        #region Methods
        protected string GetExtenderBehaviourId()
        {
            return "";
        }
        protected string DayOnClientClick(DateTime dateValue)
        {
            if (dateValue.Date >= SettingsHelper.GetCurrentPMTime().Date || IsPersonCalendar)
            {
                return string.Format(@"updatingCalendarContainer = $get('{0}');
                return ShowPopup(this,'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}');"
                    , lstCalendar.ClientID
                    , mpeHoliday.BehaviorID + Month
                    , btnSaveDay.ClientID
                    , hndDayOff.ClientID
                    , hdnDate.ClientID
                    , txtHolidayDescription.ClientID
                    , chkMakeRecurringHoliday.ClientID
                    , hdnRecurringHolidayId.ClientID
                    , hdnRecurringHolidayDate.ClientID
                    , lblDate.ClientID
                    , lblValidationMessage.ClientID
                    , btnDayOK.ClientID
                    , PersonId
                    , txtActualHours.ClientID
                    , lblActualHours.ClientID
                    , rbPTO.ClientID
                    , rbFloatingHoliday.ClientID
                    , btnDayDelete.ClientID);
            }
            else
            {
                return string.Empty;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!IsPostBack || wasChanged)
            {
                Display();
            }
        }

        private void Display()
        {
            if (CalendarItems != null)
            {
                DateTime firstMonthDay = new DateTime(Year, Month, 1);
                DateTime lastMonthDay = new DateTime(Year, Month, DateTime.DaysInMonth(Year, Month));

                DateTime firstDisplayedDay = firstMonthDay.AddDays(-(double)firstMonthDay.DayOfWeek);
                DateTime lastDisplayedDay = lastMonthDay.AddDays(6.0 - (double)lastMonthDay.DayOfWeek);

                CalendarItem[] itemsToDisplay = Array.FindAll<CalendarItem>(CalendarItems, delegate(CalendarItem item)
                {
                    return item.Date >= firstDisplayedDay && item.Date <= lastDisplayedDay;
                });

                lstCalendar.DataSource = itemsToDisplay;
                lstCalendar.DataBind();
                mpeHoliday.BehaviorID = mpeHoliday.BehaviorID + Month;
            }
        }

        protected void btnDayOK_OnClick(object sender, EventArgs e)
        {
            CalendarItem item = new CalendarItem();
            item.Date = DateTime.Parse(hdnDate.Value);
            item.DayOff = !(bool.Parse(hndDayOff.Value));//Here Changing dayOff value.
            item.PersonId = PersonId;
            item.IsRecurringHoliday = chkMakeRecurringHoliday.Checked;
            item.HolidayDescription = txtHolidayDescription.Text;
            item.ActualHours = string.IsNullOrEmpty(txtActualHours.Text) ? null : (Double?)Convert.ToDouble(txtActualHours.Text);
            item.RecurringHolidayId = string.IsNullOrEmpty(hdnRecurringHolidayId.Value) ? null : (int?)Convert.ToInt32(hdnRecurringHolidayId.Value);
            item.RecurringHolidayDate = (item.IsRecurringHoliday && !item.RecurringHolidayId.HasValue) ? (DateTime?)(item.DayOff ? item.Date : GetRecurringHolidayDate()) : null;
            item.IsFloatingHoliday = rbFloatingHoliday.Checked;
            SaveDate(item);
            Display();
        }

        private DateTime? GetRecurringHolidayDate()
        {
            return Convert.ToDateTime(hdnRecurringHolidayDate.Value);
        }

        private void SaveDate(CalendarItem item)
        {
            using (CalendarServiceClient serviceClient = new CalendarServiceClient())
            {
                try
                {
                    serviceClient.SaveCalendar(item, HttpContext.Current.User.Identity.Name);
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        protected string GetIsWeekend(DateTime dateValue)
        {
            string result = (dateValue.DayOfWeek == DayOfWeek.Saturday
                                        || dateValue.DayOfWeek == DayOfWeek.Sunday) ? "true" : "false";

            return result;
        }

        protected string GetToolTip(string holidayDescription, bool isFloatingHoliday, bool personDayOff, bool comapnyDayOff, double? actualHours)
        {
            string toolTip = holidayDescription;

            if(string.IsNullOrEmpty(holidayDescription) && IsPersonCalendar)
            {
                toolTip = isFloatingHoliday ? FloatingHoliday : (personDayOff && !comapnyDayOff && actualHours.HasValue ? string.Format(PTOToolTipFormat, actualHours) : string.Empty);
            }
            return toolTip;
        }

        protected bool NeedToEnable(DateTime dateValue)
        {
            if (!IsPersonCalendar)
            {
                var currentDate = SettingsHelper.GetCurrentPMTime();

                var result = (dateValue.Date >= currentDate.Date);
                return result;
            }
            return true;
        }

        #endregion
    }
}

