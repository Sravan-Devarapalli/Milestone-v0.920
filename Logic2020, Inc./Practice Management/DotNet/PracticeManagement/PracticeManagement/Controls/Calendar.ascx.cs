using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.CalendarService;
using PraticeManagement.Controls;
using System.ServiceModel;
using System.Web.Security;
using PraticeManagement.PersonService;
using PraticeManagement.Utils;
using AjaxControlToolkit;

namespace PraticeManagement.Controls
{
    public partial class Calendar : System.Web.UI.UserControl
    {
        #region Constants

        private const string YearKey = "Year";
        private const string ViewStatePreviousRecurringList = "ViewStatePreviousRecurringHolidaysList";
        private const string MailToSubjectFormat = "mailto:{0}?subject=Permissions for {1}'s calendar";
        public const string showEditSeriesOrSingleDayMessage = "Do you want to edit the series ({0} – {1}) or edit the single day ({2})?";
        public const string HoursFormat = "0.00";
        public const string TimeOffValidationMessage = "Selected day(s) are not working day(s). Please select any working day(s).";
        public const string SubstituteDateValidationMessage = "The selected date is not a working day.";
        public const string HolidayDetails_Format = "{0} - {1}";
        public const string AttributeDisplay = "display";
        public const string AttributeValueNone = "none";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a year to be displayed within the calendar.
        /// </summary>
        public int SelectedYear
        {
            get
            {
                return Convert.ToInt32(ViewState[YearKey]);
            }
            set
            {
                ViewState[YearKey] = value;
                UpdateCalendar();
            }
        }


        public CalendarItem[] CalendarItems
        {
            get
            {
                return ViewState["ComPanyHolidays_CalendarItems_Key"] as CalendarItem[];
            }
            set
            {
                ViewState["ComPanyHolidays_CalendarItems_Key"] = value;
            }

        }

        public string ExceptionMessage
        {
            get;
            set;
        }

        private Triple<int, string, bool>[] PreviousRecurringHolidaysList
        {
            get
            {
                return (Triple<int, string, bool>[])ViewState[ViewStatePreviousRecurringList];
            }
            set
            {
                ViewState[ViewStatePreviousRecurringList] = value;
            }
        }

        public UpdatePanel pnlBodyUpdatePanel
        {
            get
            {
                return upnlBody;
            }
        }


        #endregion


        public void UpdateCalendar()
        {

            lblYear.Text = SelectedYear.ToString();

            DateTime firstMonthDay = new DateTime(SelectedYear, 1, 1);
            DateTime lastMonthDay = new DateTime(SelectedYear, 12, DateTime.DaysInMonth(SelectedYear, 12));

            DateTime firstDisplayedDay = firstMonthDay.AddDays(-(double)firstMonthDay.DayOfWeek);
            DateTime lastDisplayedDay = lastMonthDay.AddDays(6.0 - (double)lastMonthDay.DayOfWeek);


            var days =
                 ServiceCallers.Custom.Calendar(c => c.GetCalendar(firstDisplayedDay, lastDisplayedDay));

            CalendarItems = days;

            mcJanuary.UpdateMonthCalendar();
            mcFebruary.UpdateMonthCalendar();
            mcMarch.UpdateMonthCalendar();
            mcApril.UpdateMonthCalendar();
            mcMay.UpdateMonthCalendar();
            mcJune.UpdateMonthCalendar();
            mcJuly.UpdateMonthCalendar();
            mcAugust.UpdateMonthCalendar();
            mcSeptember.UpdateMonthCalendar();
            mcOctober.UpdateMonthCalendar();
            mcNovember.UpdateMonthCalendar();
            mcDecember.UpdateMonthCalendar();

            upnlBody.Update();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                FillRecurringHolidaysList(cblRecurringHolidays, "All Recurring Holidays");
                SelectedYear = DateTime.Today.Year;
                UpdateCalendar();
            }

            if (tdRecurringHolidaysDetails.Visible)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "", "changeAlternateitemscolrsForCBL();", true);
            }

        }

        protected void FillRecurringHolidaysList(CheckBoxList cblRecurringHolidays, string firstItem)
        {
            using (var serviceClient = new CalendarService.CalendarServiceClient())
            {
                var list = serviceClient.GetRecurringHolidaysList();
                PreviousRecurringHolidaysList = list;

                if (!string.IsNullOrEmpty(firstItem))
                {
                    var firstListItem = new ListItem(firstItem, "-1");
                    cblRecurringHolidays.Items.Add(firstListItem);

                    if (PreviousRecurringHolidaysList.Count() == PreviousRecurringHolidaysList.Where(p => p.Third == true).Count())
                    {
                        var listItem = cblRecurringHolidays.Items[0];
                        listItem.Selected = true;
                    }
                }

                foreach (var item in list)
                {
                    var listItem = new ListItem(item.Second, item.First.ToString());

                    cblRecurringHolidays.Items.Add(listItem);

                    listItem.Selected = item.Third;
                }
            }
        }

        protected void btnRetrieveCalendar_Click(object sender, EventArgs e)
        {
            UpdateCalendar();
        }

        protected void btnPrevYear_Click(object sender, EventArgs e)
        {
            SelectedYear--;
            UpdateCalendar();
        }

        protected void btnNextYear_Click(object sender, EventArgs e)
        {
            SelectedYear++;
            UpdateCalendar();
        }

        protected void cblRecurringHolidays_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var item = (ScrollingDropDown)sender;
            var user = HttpContext.Current.User.Identity.Name;

            if (PreviousRecurringHolidaysList != null)
            {
                foreach (var previousItem in PreviousRecurringHolidaysList)
                {
                    bool check = previousItem.Third;
                    int id = previousItem.First;

                    var selectedItems = item.SelectedItems.Split(',');

                    var selectedItem = selectedItems.Where(p => p.ToString() == previousItem.First.ToString());

                    if (selectedItem.Count() > 0 && !check)
                    {
                        previousItem.Third = !check;
                        SetRecurringHoliday(id, !check, user);
                    }
                    else if (check && selectedItem.Count() <= 0)
                    {
                        previousItem.Third = !check;
                        SetRecurringHoliday(id, !check, user);
                    }
                }
            }


            UpdateCalendar();
        }

        private void SetRecurringHoliday(int? id, bool isSet, string user)
        {
            using (var serviceClient = new CalendarService.CalendarServiceClient())
            {
                serviceClient.SetRecurringHoliday(id, isSet, user);
            }
        }

    }
}

