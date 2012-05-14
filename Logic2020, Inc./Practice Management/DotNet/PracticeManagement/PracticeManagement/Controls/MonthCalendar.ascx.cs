using System;
using System.ServiceModel;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.CalendarService;
using System.Web;
using PraticeManagement.Utils;
using System.Collections.Generic;
using System.Web.UI;
using System.Linq;

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


        #region Properties


        public PraticeManagement.Controls.Calendar HostingControl
        {
            get
            {
                return ((PraticeManagement.Config.CompanyHolidays)Page).CalendarControl;
            }
        }

        /// <summary>
        /// Get or sets a year to be displayed.
        /// </summary>
        public int Year
        {
            get
            {
                return HostingControl.SelectedYear;
            }
        }

        /// <summary>
        /// Gets or sets a month to be displayed.
        /// </summary>
        public int Month
        {
            get;
            set;

        }

        private CalendarItem[] CalendarItems
        {
            get
            {
                return HostingControl.CalendarItems;
            }

        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }


        #endregion

        #region Methods

        protected string DayOnClientClick(DateTime dateValue)
        {
            if (dateValue.Date >= SettingsHelper.GetCurrentPMTime().Date)
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
                    , ""
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

        protected void btnDay_OnClick(object sender, EventArgs e)
        {
        }

        public void UpdateMonthCalendar()
        {
            if (CalendarItems != null)
            {
                DateTime firstMonthDay = new DateTime(Year, Month, 1);
                DateTime lastMonthDay = new DateTime(Year, Month, DateTime.DaysInMonth(Year, Month));

                DateTime firstDisplayedDay = firstMonthDay.AddDays(-(double)firstMonthDay.DayOfWeek);
                DateTime lastDisplayedDay = lastMonthDay.AddDays(6.0 - (double)lastMonthDay.DayOfWeek);

                var itemsToDisplay = CalendarItems.Where(ci => ci.Date >= firstDisplayedDay && ci.Date <= lastDisplayedDay);

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
            item.IsRecurringHoliday = chkMakeRecurringHoliday.Checked;
            item.IsFloatingHoliday = rbFloatingHoliday.Checked;
            item.HolidayDescription = txtHolidayDescription.Text;
            item.ActualHours = string.IsNullOrEmpty(txtActualHours.Text) ? null : (item.IsFloatingHoliday ? 8 : (Double?)Convert.ToDouble(txtActualHours.Text));
            item.RecurringHolidayId = string.IsNullOrEmpty(hdnRecurringHolidayId.Value) ? null : (int?)Convert.ToInt32(hdnRecurringHolidayId.Value);
            item.RecurringHolidayDate = (item.IsRecurringHoliday && !item.RecurringHolidayId.HasValue) ? (DateTime?)(item.DayOff ? item.Date : GetRecurringHolidayDate()) : null;
            SaveDate(item);
            HostingControl.UpdateCalendar();
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

        protected string GetDoubleFormat(double? hours)
        {
            return hours.HasValue ? hours.Value.ToString("0.00") : "";
        }

        protected string GetToolTip(string holidayDescription)
        {
            return holidayDescription;
        }

        protected bool NeedToEnable(DateTime dateValue)
        {
            var currentDate = SettingsHelper.GetCurrentPMTime();
            var result = (dateValue.Date >= currentDate.Date);
            return result;
        }

        public bool GetIsReadOnly(bool dateLevelReadonly, bool dayOff, bool companyDayOff, DateTime date)
        {
            return dateLevelReadonly;
        }

        #endregion
    }
}

