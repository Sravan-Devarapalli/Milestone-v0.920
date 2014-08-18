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
    public partial class PersonMonthCalendar : System.Web.UI.UserControl
    {
       
        #region Properties

        public PraticeManagement.Controls.PersonCalendar HostingControl
        {
            get
            {
                if (Page is PraticeManagement.Calendar)
                {
                    var control = ((PraticeManagement.Calendar)Page).CalendarControl as PraticeManagement.Controls.PersonCalendar;
                    return control;

                }

                return null;
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

        private int? PersonId
        {
            get
            {
                return HostingControl.SelectedPersonId;
            }
        }

        private CalendarItem[] CalendarItems
        {
            get 
            {
               return HostingControl.CalendarItems;
            }
            
        }

        private bool IsReadOnly
        {
            get
            {
                return !HostingControl.HasPermissionToEditCalender;
            }
            
        }

        private bool IsUserHrORAdmin
        {
            get
            {
                return HostingControl.IsUserHrORAdmin;
            }

        }

        #endregion

        #region Methods

        protected string DayOnClientClick()
        {
            return string.Format(@"return ShowPopup(this,'{0}','{1}');"
                , HostingControl.hdnDayOffObject.ClientID
                , HostingControl.hdnDateObject.ClientID
               );

        }

        protected void btnDay_OnClick(object sender, EventArgs e)
        {

            var btnDay = (LinkButton)sender;
            var date = (DateTime)Convert.ToDateTime(btnDay.Attributes["Date"]);

            var isWeekEnd = GetIsWeekend(date);

            if (btnDay.Attributes["CompanyDayOff"].ToLower() == "false" && isWeekEnd == false)
            {
                string hours = btnDay.Attributes["ActualHours"];
                string timeTypeId = btnDay.Attributes["TimeTypeId"];
                Quadruple<DateTime, DateTime, int?, string> series = ServiceCallers.Custom.Calendar(c => c.GetTimeOffSeriesPeriod(PersonId.Value, date));

                HostingControl.PopulateSingleDayPopupControls(date, timeTypeId, hours, series.Third, series.Fourth);
                HostingControl.SeriesStartDate = series.First;
                HostingControl.SeriesEndDate = series.Second;
                if (series.First == series.Second)
                {
                    HostingControl.mpeEditSingleDayPopUp.Show();
                    HostingControl.PreviousStartDate = HostingControl.PreviousEndDate = series.First;
                    HostingControl.PreviousTimeType = HostingControl.TimeTypeDdlSingleday.SelectedItem.Text;
                    HostingControl.PreviousHours = HostingControl.TxtHoursSingleDay.Text;
                }
                else
                {
                    HostingControl.PopulateEditConditionPopupControls(series.First, series.Second, date);
                    HostingControl.PopulateSeriesPopupControls(series.First, series.Second, timeTypeId, hours, series.Third, series.Fourth);
                    HostingControl.mpeSelectEditCondtionPopUp.Show();
                }

                HostingControl.pnlBodyUpdatePanel.Update();
            }
            else if (HostingControl.hdnDayOffObject.Value.ToLower() == "true" && btnDay.Attributes["CompanyDayOff"] == "true")
            {
                HostingControl.ShowHolidayAndSubStituteDay(date, btnDay.Attributes["HolidayDescription"]);
            }
            else if (HostingControl.hdnDayOffObject.Value.ToLower() == "false" && btnDay.Attributes["CompanyDayOff"] == "true")
            {
                HostingControl.ShowModifySubstituteDay(date, btnDay.Attributes["HolidayDescription"]);
            }
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
            }
        }



        protected bool GetIsWeekend(DateTime dateValue)
        {
            var result = (dateValue.DayOfWeek == DayOfWeek.Saturday
                                        || dateValue.DayOfWeek == DayOfWeek.Sunday) ? true : false;

            return result;
        }
        protected string GetDoubleFormat(double? hours)
        {
            return hours.HasValue ? hours.Value.ToString("0.00") : "";
        }

        protected string GetToolTip(string holidayDescription, double? actualHours, bool isFloatingHoliday)
        {
            string toolTip = holidayDescription;

            if (actualHours.HasValue && !isFloatingHoliday)
            {
                toolTip = holidayDescription + " - " + actualHours.Value.ToString("0.00") + " hr(s)";
            }

            return toolTip;
        }

        public bool GetIsReadOnly(bool dateLevelReadonly, bool dayOff, bool companyDayOff, DateTime date, bool isUnpaidTimeType)
        {
            if (isUnpaidTimeType && !IsUserHrORAdmin)
            {
                return true;
            }

            bool isReadOnly = dayOff
                ? (companyDayOff
                    ? (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday ? true : false)
                    : (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday ? true : false)
                  )
                : (companyDayOff
                    ? (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday ? false : false)
                    : (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday ? false : true)
                  );

            if (!isReadOnly)
            {
                if (IsReadOnly)
                {
                    return true;
                }
                else
                {
                    return dateLevelReadonly;
                }
            }

            return true;


        }

        #endregion
    }
}

