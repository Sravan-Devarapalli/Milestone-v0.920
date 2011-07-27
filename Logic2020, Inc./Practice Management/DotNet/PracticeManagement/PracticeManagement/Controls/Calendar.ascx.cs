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

namespace PraticeManagement.Controls
{
    public partial class Calendar : System.Web.UI.UserControl
    {
        #region Constants

        private const string YearKey = "Year";
        private const string ViewStatePreviousRecurringList = "ViewStatePreviousRecurringHolidaysList";

        private CalendarItem[] days;
        private bool userIsPracticeManager;
        private bool userIsSalesperson;
        private bool userIsRecruiter;
        private bool userIsAdministrator;
        private bool userIsConsultant;
        private bool userIsHR;
        private bool userIsProjectLead;
        private bool userIsDirector;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a year to be displayed within the calendar.
        /// </summary>
        private int SelectedYear
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

        private int? SelectedPersonId
        {
            get
            {
                int? personId =
                     !string.IsNullOrEmpty(ddlPerson.SelectedValue) ? (int?)int.Parse(ddlPerson.SelectedValue) : null;
                return personId;
            }
        }

        public bool CompanyHolidays
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

        #endregion

        private void UpdateCalendar()
        {
            mcJanuary.Year = mcFebruary.Year = mcMarch.Year =
                mcApril.Year = mcMay.Year = mcJune.Year =
                mcJuly.Year = mcAugust.Year = mcSeptember.Year =
                mcOctober.Year = mcNovember.Year = mcDecember.Year = SelectedYear;

            int? personId = CompanyHolidays ? null : SelectedPersonId;

            mcJanuary.PersonId = mcFebruary.PersonId = mcMarch.PersonId =
                mcApril.PersonId = mcMay.PersonId = mcJune.PersonId =
                mcJuly.PersonId = mcAugust.PersonId = mcSeptember.PersonId =
                mcOctober.PersonId = mcNovember.PersonId = mcDecember.PersonId = personId;

            lblYear.Text = SelectedYear.ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            userIsPracticeManager =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.PracticeManagerRoleName);
            userIsDirector =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName);
            userIsSalesperson =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SalespersonRoleName);
            userIsRecruiter =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.RecruiterRoleName);
            userIsAdministrator =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
            userIsConsultant =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.ConsultantRoleName);
            userIsHR =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.HRRoleName);
            userIsProjectLead =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.ProjectLead);

            if (!IsPostBack)
            {
                if (!CompanyHolidays)
                {
                    if (userIsAdministrator || userIsRecruiter || userIsConsultant || userIsSalesperson || userIsProjectLead || userIsHR)// #2817: userIsHR is added as per the requirement.
                    {
                        //DataHelper.FillPersonList(ddlPerson, Resources.Controls.CompanyCalendarTitle);
                        DataHelper.FillPersonList(ddlPerson, null, (int) PersonStatusType.Active);
                    }
                    else
                    {
                        // if user does not have any role then we are just showing him in the dropdown
                        ddlPerson.Items.Add(new ListItem(DataHelper.CurrentPerson.Name, DataHelper.CurrentPerson.Id.ToString()));
                    }

                    Person current = DataHelper.CurrentPerson;
                    // Security
                    if (!userIsAdministrator)
                    {
                        btnRetrieveCalendar.Visible = userIsPracticeManager || userIsSalesperson || userIsRecruiter || userIsDirector || userIsHR; // #2817: userIsDirector is added as per the requirement.

                        if (userIsPracticeManager || userIsDirector && current != null)// #2817: userIsDirector is added as per the requirement.
                        {
                            // Practice manager have to see the list his subordinates
                            DataHelper.FillSubordinatesList(ddlPerson,
                                current.PersonLastFirstName,
                                current.Id.Value);
                        }
                        else if (!userIsRecruiter && !userIsSalesperson && !userIsHR)// #2817: userIsHR is added as per the requirement.
                        {
                            // Non-administrator users can view and edit the own schedule only.
                            ddlPerson.Enabled = false;
                        }
                    }

                    ddlPerson.SelectedIndex =
                        ddlPerson.Items.IndexOf(ddlPerson.Items.FindByValue(current.Id.Value.ToString()));
                }
                else
                {
                    FillRecurringHolidaysList(cblRecurringHolidays, "All Recurring Holidays");
                }
                    
                trPersonDetails.Visible = !CompanyHolidays;

                trRecurringHolidaysDetails.Visible = tdDescription.Visible = CompanyHolidays;

                SelectedYear = DateTime.Today.Year;
            }

        }

        protected void FillRecurringHolidaysList(CheckBoxList cblRecurringHolidays, string firstItem)
        {
            using (var serviceClient = new CalendarService.CalendarServiceClient())
            {
                if (!string.IsNullOrEmpty(firstItem))
                {
                    cblRecurringHolidays.Items.Add(firstItem);
                }
                var list = serviceClient.GetRecurringHolidaysList();
                PreviousRecurringHolidaysList = list;

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
        }

        protected void btnNextYear_Click(object sender, EventArgs e)
        {
            SelectedYear++;
        }

        protected void cblRecurringHolidays_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var item = (ScrollingDropDown)sender;

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
                        SetRecurringHoliday(id, !check);
                    }
                    else if (check && selectedItem.Count() <= 0)
                    {
                        previousItem.Third = !check;
                        SetRecurringHoliday(id, !check);
                    }
                }
            }
        }

        private void SetRecurringHoliday(int id, bool isSet)
        {
            using (var serviceClient = new CalendarService.CalendarServiceClient())
            {
                serviceClient.SetRecurringHoliday(id, isSet);
            }
        }

        protected void calendar_PreRender(object sender, EventArgs e)
        {
            MonthCalendar calendar = sender as MonthCalendar;

            if (days == null)
            {
                int? practiceManagerId = null;
                if ((!userIsAdministrator && userIsPracticeManager) || (!userIsAdministrator && userIsDirector))// #2817:(!userIsAdministrator && userIsDirector) is added as per the requirement.
                {
                    Person current = DataHelper.CurrentPerson;
                    practiceManagerId = current != null ? current.Id : 0;
                }

                DateTime firstMonthDay = new DateTime(SelectedYear, 1, 1);
                DateTime lastMonthDay = new DateTime(SelectedYear, 12, DateTime.DaysInMonth(SelectedYear, 12));

                DateTime firstDisplayedDay = firstMonthDay.AddDays(-(double)firstMonthDay.DayOfWeek);
                DateTime lastDisplayedDay = lastMonthDay.AddDays(6.0 - (double)lastMonthDay.DayOfWeek);

                using (CalendarServiceClient serviceClient = new CalendarServiceClient())
                {
                    try
                    {
                        days =
                            serviceClient.GetCalendar(firstDisplayedDay, lastDisplayedDay, SelectedPersonId, practiceManagerId);
                    }
                    catch (CommunicationException)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }
            }

            if (days != null && !userIsAdministrator && !userIsPracticeManager && !userIsDirector &&     // #2817: userIsDirector is added as per the requirement.
                (userIsConsultant || userIsRecruiter || userIsSalesperson || userIsHR))                 // #2817: userIsHR is added as per the requirement.
            {
                // Security
                foreach (CalendarItem item in days)
                {
                    item.ReadOnly = true;
                }

                lblConsultantMessage.Visible = true;
            }

            calendar.CalendarItems = days;
        }
    }
}
