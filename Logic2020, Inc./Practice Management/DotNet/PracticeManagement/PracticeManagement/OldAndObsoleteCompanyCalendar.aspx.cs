using System;
using System.ServiceModel;
using System.Web.Security;
using System.Web.UI;
using DataTransferObjects;
using PraticeManagement.CalendarService;
using PraticeManagement.Controls;

namespace PraticeManagement
{
    public partial class CompanyCalendar : System.Web.UI.Page
    {
        private const string YearKey = "Year";

        private CalendarItem[] days;
        private bool userIsPracticeManager;
        private bool userIsDirector;
        private bool userIsSalesperson;
        private bool userIsRecruiter;
        private bool userIsAdministrator;
        private bool userIsConsultant;
        private bool userIsHR;
        private bool userIsProjectLead;

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

        private void UpdateCalendar()
        {
            mcJanuary.Year = mcFebruary.Year = mcMarch.Year =
                mcApril.Year = mcMay.Year = mcJune.Year =
                mcJuly.Year = mcAugust.Year = mcSeptember.Year =
                mcOctober.Year = mcNovember.Year = mcDecember.Year = SelectedYear;

            int? personId = SelectedPersonId;

            mcJanuary.PersonId = mcFebruary.PersonId = mcMarch.PersonId =
                mcApril.PersonId = mcMay.PersonId = mcJune.PersonId =
                mcJuly.PersonId = mcAugust.PersonId = mcSeptember.PersonId =
                mcOctober.PersonId = mcNovember.PersonId = mcDecember.PersonId = personId;

            lblYear.Text = SelectedYear.ToString();
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AsyncPostBackTrigger tr =
                    new AsyncPostBackTrigger() { ControlID = btnRetrieveCalendar.UniqueID, EventName = "Click" };
                pnlHeader.Triggers.Add(tr);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            userIsPracticeManager =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.PracticeManagerRoleName);
            userIsDirector =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName);// #2817: userIsDirector is added as per the requirement.
            userIsSalesperson =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SalespersonRoleName);
            userIsRecruiter =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.RecruiterRoleName);
            userIsAdministrator =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
            userIsConsultant =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.ConsultantRoleName);
            userIsHR =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.HRRoleName);// #2817: userIsHR is added as per the requirement.
            userIsProjectLead =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.ProjectLead);

            if (!IsPostBack)
            {
                if (userIsAdministrator || userIsRecruiter || userIsConsultant || userIsSalesperson || userIsProjectLead || userIsHR)// #2817: userIsHR is added as per the requirement.
                {
                    DataHelper.FillPersonList(ddlPerson, Resources.Controls.CompanyCalendarTitle);
                }

                // Security
                if (!userIsAdministrator)
                {
                    btnRetrieveCalendar.Visible = userIsPracticeManager || userIsSalesperson || userIsRecruiter || userIsDirector || userIsHR; // #2817: userIsDirector is added as per the requirement.

                    Person current = DataHelper.CurrentPerson;

                    if (userIsPracticeManager || userIsDirector && current != null) // #2817: userIsDirector is added as per the requirement.
                    {
                        // Practice manager have to see the list his subordinates
                        DataHelper.FillSubordinatesList(ddlPerson,
                            current.PersonLastFirstName,
                            current.Id.Value);
                    }
                    else if (!userIsRecruiter && !userIsSalesperson && !userIsHR)
                    {
                        // Non-administrator users can view and edit the own schedule only.
                        ddlPerson.SelectedIndex =
                            ddlPerson.Items.IndexOf(ddlPerson.Items.FindByValue(current.Id.Value.ToString()));
                        ddlPerson.Enabled = false;
                    }
                }

                SelectedYear = DateTime.Today.Year;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (ddlPerson.SelectedIndex > 0)
            {
                lblCalendarOwnerName.Text = ddlPerson.SelectedItem.Text;
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

        protected void calendar_PreRender(object sender, EventArgs e)
        {
            MonthCalendar calendar = sender as MonthCalendar;

            if (days == null)
            {
                int? practiceManagerId = null;
                if ((!userIsAdministrator && userIsPracticeManager) || (!userIsAdministrator && userIsDirector))// #2817: userIsDirector is added as per the requirement.
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

            if (days != null && !userIsAdministrator && !userIsPracticeManager && !userIsDirector &&    // #2817: userIsDirector is added as per the requirement.
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

