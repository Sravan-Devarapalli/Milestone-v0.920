using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;

namespace PraticeManagement.Controls
{
    public partial class Calendar : System.Web.UI.UserControl
    {
        #region Constants

        private const string YearKey = "Year";
        private const string ViewStatePreviousRecurringList = "ViewStatePreviousRecurringHolidaysList";
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

        public bool AllHolidaysSelected
        {
            get { return (bool) ViewState["AllHolidaysSelected_Key"]; }
            set { ViewState["AllHolidaysSelected_Key"] = value; }
        }

        #endregion


        public void UpdateCalendar()
        {

            lblYear.Text = SelectedYear.ToString();

            var firstMonthDay = new DateTime(SelectedYear, 1, 1);
            var lastMonthDay = new DateTime(SelectedYear, 12, DateTime.DaysInMonth(SelectedYear, 12));

            var firstDisplayedDay = firstMonthDay.AddDays(-(double)firstMonthDay.DayOfWeek);
            var lastDisplayedDay = lastMonthDay.AddDays(6.0 - (double)lastMonthDay.DayOfWeek);


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
                AllHolidaysSelected = cblRecurringHolidays.Items[0].Selected;
            }

            if (tdRecurringHolidaysDetails.Visible)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "", "changeAlternateitemscolrsForCBL();", true);
            }

        }

        protected void FillRecurringHolidaysList(CheckBoxList cblRecurringHolidaysList, string firstItem)
        {
            using (var serviceClient = new CalendarService.CalendarServiceClient())
            {
                var list = serviceClient.GetRecurringHolidaysList();
                PreviousRecurringHolidaysList = list;

                if (!string.IsNullOrEmpty(firstItem))
                {
                    var firstListItem = new ListItem(firstItem, "-1");
                    cblRecurringHolidaysList.Items.Add(firstListItem);

                    if (PreviousRecurringHolidaysList.Count() == PreviousRecurringHolidaysList.Count(p => p.Third))
                    {
                        var listItem = cblRecurringHolidaysList.Items[0];
                        listItem.Selected = true;
                    }
                }

                foreach (var item in list)
                {
                    var listItem = new ListItem(item.Second, item.First.ToString());

                    cblRecurringHolidaysList.Items.Add(listItem);

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
            if ((item.Items[0].Selected && AllHolidaysSelected != item.Items[0].Selected) || (!item.Items[0].Selected && item.AllNotSelected && AllHolidaysSelected != item.Items[0].Selected))
            {
                SetRecurringHoliday(null, item.Items[0].Selected, user);
                foreach (var pre in PreviousRecurringHolidaysList)
                    pre.Third = item.Items[0].Selected;
            }
            else if (PreviousRecurringHolidaysList != null)
            {
                foreach (var previousItem in PreviousRecurringHolidaysList)
                {
                    var check = previousItem.Third;
                    var id = previousItem.First;

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
            AllHolidaysSelected = item.Items[0].Selected;
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

