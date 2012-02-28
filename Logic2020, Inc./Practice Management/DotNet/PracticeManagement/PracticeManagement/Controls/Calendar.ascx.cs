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

        private CalendarItem[] days;
        private bool userIsPracticeManager;
        private bool userIsSalesperson;
        private bool userIsRecruiter;
        private bool userIsAdministrator;
        private bool userIsConsultant;
        private bool userIsHR;
        private bool userIsProjectLead;
        private bool userIsDirector;
        private bool userIsSeniorLeadership;

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

        public string ExceptionMessage
        {
            get;
            set;
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
        
        private bool SelectedPersonHasPermissionToEditCalender
        {
            get
            {
                string HasPermissionToEditCalender = true.ToString();
                if (ddlPerson.SelectedItem != null)
                {
                    HasPermissionToEditCalender = ddlPerson.SelectedItem.Attributes[Constants.Variables.HasPermissionToEditCalender];
                }
                return Convert.ToBoolean(HasPermissionToEditCalender.ToLower());
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

        public UpdatePanel pnlBodyUpdatePanel
        {
            get
            {
                return pnlBody;
            }
        }

        public ModalPopupExtender mpeSelectEditCondtionPopUp
        {
            get
            {
                return mpeSelectEditCondtion;
            }
        }

        public ModalPopupExtender mpeEditSingleDayPopUp
        {
            get
            {
                return mpeEditSingleDay;
            }
        }

        #endregion

        public void PopulateSingleDayPopupControls(DateTime date, string timeTypeId, string hours)
        {
            lbdateSingleDay.Text = date.ToString(Constants.Formatting.EntryDateFormat);
            hdnDateSingleDay.Value = date.ToString();
            ddlTimeTypesSingleDay.SelectedValue = timeTypeId;
            txtHoursSingleDay.Text = Convert.ToDouble(hours).ToString(HoursFormat);
            hdIsSingleDayPopDirty.Value = false.ToString();
            btnDeleteSingleDay.Enabled = true;
        }

        public void PopulateSeriesPopupControls(DateTime startDate, DateTime endDate, string timeTypeId, string hours)
        {
            dtpStartDateTimeOff.DateValue = startDate;
            dtpEndDateTimeOff.DateValue = endDate;
            ddlTimeTypesTimeOff.SelectedValue = timeTypeId;
            txthoursTimeOff.Text = Convert.ToDouble(hours).ToString(HoursFormat);
            btnDeleteTimeOff.Visible = btnDeleteTimeOff.Enabled = true;
            hdIsTimeOffPopUpDirty.Value = false.ToString(); 
        }

        public void PopulateEditConditionPopupControls(DateTime startDate, DateTime endDate,DateTime selectedDate)
        {
            rbEditSeries.Checked = true;
            rbEditSingleDay.Checked = false;
            lbDate.Text = String.Format(Calendar.showEditSeriesOrSingleDayMessage, startDate.ToString(Constants.Formatting.EntryDateFormat), endDate.ToString(Constants.Formatting.EntryDateFormat), selectedDate.ToString(Constants.Formatting.EntryDateFormat));

        }
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

            mcJanuary.IsPersonCalendar = mcFebruary.IsPersonCalendar = mcMarch.IsPersonCalendar =
                mcApril.IsPersonCalendar = mcMay.IsPersonCalendar = mcJune.IsPersonCalendar =
                mcJuly.IsPersonCalendar = mcAugust.IsPersonCalendar = mcSeptember.IsPersonCalendar =
                mcOctober.IsPersonCalendar = mcNovember.IsPersonCalendar = mcDecember.IsPersonCalendar = !CompanyHolidays;
            mcJanuary.IsReadOnly = mcFebruary.IsReadOnly = mcMarch.IsReadOnly =
            mcApril.IsReadOnly = mcMay.IsReadOnly = mcJune.IsReadOnly =
            mcJuly.IsReadOnly = mcAugust.IsReadOnly = mcSeptember.IsReadOnly =
            mcOctober.IsReadOnly = mcNovember.IsReadOnly = mcDecember.IsReadOnly = !SelectedPersonHasPermissionToEditCalender;

            SetMailToContactSupport();
            if (SelectedPersonHasPermissionToEditCalender)
            {
                trAlert.Visible = false;
            }
            else
            {
                trAlert.Visible = true;
            }
            pnlBody.Update();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            upnlErrorSingleDay.Update();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetMailToContactSupport();
            userIsPracticeManager =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.PracticeManagerRoleName);
            userIsDirector =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName);
            userIsSeniorLeadership =
                 Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SeniorLeadershipRoleName); // #2913: userIsSeniorLeadership is added as per the requirement.

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
                    //#2961: allowing all persons to be in the dropdown list irrespective of role.
                    DataHelper.FillPersonList(ddlPerson, null, (int)PersonStatusType.Active);
                    Person current = DataHelper.CurrentPerson;
                    // Security
                    Person[] persons = ServiceCallers.Custom.Person(p => p.GetCareerCounselorHierarchiPersons(current.Id.Value));
                    var personsList = persons.ToList();
                    personsList.Add(current);

                    foreach (ListItem personitem in ddlPerson.Items)
                    {
                        int personId = Convert.ToInt32(personitem.Value);
                        string HasPermissionToEditCalender = false.ToString();
                        if (!userIsAdministrator)
                        {
                            if (userIsPracticeManager || userIsDirector || userIsRecruiter || userIsSalesperson || userIsProjectLead || userIsConsultant && current != null)// #2817: userIsDirector is added as per the requirement.
                            {
                                HasPermissionToEditCalender = personsList.Any(p => p.Id == personId) ? true.ToString() : false.ToString();
                            }
                        }
                        else
                        {
                            HasPermissionToEditCalender = true.ToString();
                        }
                        personitem.Attributes[Constants.Variables.HasPermissionToEditCalender] = HasPermissionToEditCalender;
                    }

                    ddlPerson.SelectedIndex =
                        ddlPerson.Items.IndexOf(ddlPerson.Items.FindByValue(current.Id.Value.ToString()));

                    var administrativeTimeTypes = ServiceCallers.Custom.TimeType(p => p.GetAllAdministrativeTimeTypes(true, false));
                    DataHelper.FillListDefault(ddlTimeTypesSingleDay, "- - Make Selection - -", administrativeTimeTypes, false);
                    DataHelper.FillListDefault(ddlTimeTypesTimeOff, "- - Make Selection - -", administrativeTimeTypes, false);

                }
                else
                {
                    FillRecurringHolidaysList(cblRecurringHolidays, "All Recurring Holidays");
                }

                trPersonDetails.Visible = !CompanyHolidays;

                tdRecurringHolidaysDetails.Visible = tdDescription.Visible = CompanyHolidays;

                SelectedYear = DateTime.Today.Year;
            }

            if (tdRecurringHolidaysDetails.Visible)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "", "changeAlternateitemscolrsForCBL();", true);
            }

        }

        protected void btnDeleteSingleDay_OnClick(object sender, EventArgs e)
        {
            Page.Validate(valSumErrorSingleDay.ValidationGroup);
            if (Page.IsValid)
            {
                var date = Convert.ToDateTime(hdnDateSingleDay.Value);
                ServiceCallers.Custom.Calendar(
                                               c => c.SaveTimeOff(date,
                                                                  date,
                                                                  false,
                                                                  SelectedPersonId.Value,
                                                                  (double?)Convert.ToDouble(txtHoursSingleDay.Text),
                                                                  Convert.ToInt32(ddlTimeTypesSingleDay.SelectedValue),
                                                                  Context.User.Identity.Name
                                                                  )
                                               );

                mpeEditSingleDay.Hide();
            }
            else
            {
                mpeEditSingleDay.Show();
            }


            upnlErrorSingleDay.Update();

        }

        protected void btnOkSingleDay_OnClick(object sender, EventArgs e)
        {

            Page.Validate(valSumErrorSingleDay.ValidationGroup);
            if (Page.IsValid)
            {
                var date = Convert.ToDateTime(hdnDateSingleDay.Value);
                ServiceCallers.Custom.Calendar(
                                               c => c.SaveTimeOff(date,
                                                                  date,
                                                                  true,
                                                                  SelectedPersonId.Value,
                                                                  (double?)Convert.ToDouble(txtHoursSingleDay.Text),
                                                                  Convert.ToInt32(ddlTimeTypesSingleDay.SelectedValue),
                                                                  Context.User.Identity.Name
                                                                  )
                                               );

                mpeEditSingleDay.Hide();
            }
            else
            {
                if (!String.IsNullOrEmpty(hdIsSingleDayPopDirty.Value))
                {
                    var isPopupDirty = Convert.ToBoolean(hdIsSingleDayPopDirty.Value);
                    if (isPopupDirty)
                    {
                        btnDeleteSingleDay.Enabled = false;
                    }
                }
                mpeEditSingleDay.Show();
            }

            upnlErrorSingleDay.Update();

        }

        protected void btnAddTimeOff_Click(object sender, EventArgs e)
        {
            btnDeleteTimeOff.Visible = false;
            btnDeleteTimeOff.Enabled = true;
            dtpStartDateTimeOff.DateValue = DateTime.Today;
            dtpEndDateTimeOff.DateValue = DateTime.Today;
            ddlTimeTypesTimeOff.SelectedIndex = 0;
            txthoursTimeOff.Text = "8.00";
            mpeAddTimeOff.Show();
            upnlTimeOff.Update();
        }

        protected void btnOkTimeOff_Click(object sender, EventArgs e)
        {
            Page.Validate(valSumTimeOff.ValidationGroup);
            if (Page.IsValid)
            {
                try
                {
                    ServiceCallers.Custom.Calendar(
                        c => c.SaveTimeOff(dtpStartDateTimeOff.DateValue,
                                                                      dtpEndDateTimeOff.DateValue,
                                                                      true,
                                                                      SelectedPersonId.Value,
                                                                      (double?)Convert.ToDouble(txthoursTimeOff.Text),
                                                                      Convert.ToInt32(ddlTimeTypesTimeOff.SelectedValue),
                                                                      Context.User.Identity.Name
                                                                      )
                                                   );
                }
                catch (Exception ex)
                {
                    if (ex.Message == TimeOffValidationMessage)
                    {
                        ExceptionMessage = ex.Message;
                        Page.Validate(valSumTimeOff.ValidationGroup);
                        mpeAddTimeOff.Show();
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(hdIsTimeOffPopUpDirty.Value))
                {
                    var isPopupDirty = Convert.ToBoolean(hdIsTimeOffPopUpDirty.Value);
                    if (isPopupDirty)
                    {
                        btnDeleteTimeOff.Enabled = false;
                    }
                }
                mpeAddTimeOff.Show();
            }

            upnlTimeOff.Update();
        }

        protected void cvStartDateEndDateTimeOff_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!string.IsNullOrEmpty(ExceptionMessage))
            {
                args.IsValid = false;
            }
        }

        protected void btnDeleteTimeOff_Click(object sender, EventArgs e)
        {
            Page.Validate(valSumTimeOff.ValidationGroup);
            if (Page.IsValid)
            {
                ServiceCallers.Custom.Calendar(
                   c => c.SaveTimeOff(dtpStartDateTimeOff.DateValue,
                                                                 dtpEndDateTimeOff.DateValue,
                                                                 false,
                                                                 SelectedPersonId.Value,
                                                                 (double?)Convert.ToDouble(txthoursTimeOff.Text),
                                                                 Convert.ToInt32(ddlTimeTypesTimeOff.SelectedValue),
                                                                 Context.User.Identity.Name
                                                                 )
                                              );
            }
            else
            {
                mpeAddTimeOff.Show();
            }


            upnlTimeOff.Update();
        }

        protected void cvSubstituteDay_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (string.IsNullOrEmpty(dpSubstituteDay.TextValue) || !string.IsNullOrEmpty(ExceptionMessage))
            {
                args.IsValid = false;
            }
        }

        protected void cvHoursSingleDay_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!string.IsNullOrEmpty(txtHoursSingleDay.Text))
            {
                var hours = Convert.ToDouble(txtHoursSingleDay.Text);
                if (hours % 0.25 != 0)
                {
                    args.IsValid = false;
                }
            }
        }

        protected void cvHoursPerDay_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!string.IsNullOrEmpty(txtHoursSingleDay.Text))
            {
                var hours = Convert.ToDouble(txthoursTimeOff.Text);
                if (hours % 0.25 != 0)
                {
                    args.IsValid = false;
                }
            }
        }
      
        protected void btnDeleteSubstituteDay_Click(object sender, EventArgs e)
        {
            var userName = Context.User.Identity.Name;
            ServiceCallers.Custom.Calendar(c => c.DeleteSubstituteDay(SelectedPersonId.Value, Convert.ToDateTime(hdnHolidayDate.Value), userName));
            mpeDeleteSubstituteDay.Hide();
        }

        protected void btnSubstituteDayOK_Click(object sender, EventArgs e)
        {
            Page.Validate(cvSubstituteDay.ValidationGroup);
            if (Page.IsValid)
            {
                CalendarItem ci = new CalendarItem()
                {
                    SubstituteDayDate = dpSubstituteDay.DateValue,
                    Date = Convert.ToDateTime(hdnHolidayDate.Value),
                    PersonId = SelectedPersonId
                };

                var userName = Context.User.Identity.Name;

                try
                {
                    ServiceCallers.Custom.Calendar(c => c.SaveSubstituteDay(ci, userName));
                    mpeHolidayAndSubStituteDay.Hide();
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Selected date is not a Working day.Please select any Working day.")
                    {
                        ExceptionMessage = ex.Message;
                        Page.Validate(dpSubstituteDay.ValidationGroup);
                        mpeHolidayAndSubStituteDay.Show();
                    }
                    else
                    {
                        throw ex;
                    }

                }
            }
            else
            {
                mpeHolidayAndSubStituteDay.Show();
            }

            upnlValsummary.Update();

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
        }

        protected void btnNextYear_Click(object sender, EventArgs e)
        {
            SelectedYear++;
        }

        protected void cblRecurringHolidays_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var item = (ScrollingDropDown)sender;
            var user = HttpContext.Current.User.Identity.Name;

            if (item.Items[0].Selected)
            {
                SetRecurringHoliday(null, true, user);
                foreach (var previousItem in PreviousRecurringHolidaysList)
                {
                    previousItem.Third = true;
                }
            }
            else if (PreviousRecurringHolidaysList != null)
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
        }

        private void SetRecurringHoliday(int? id, bool isSet, string user)
        {
            using (var serviceClient = new CalendarService.CalendarServiceClient())
            {
                serviceClient.SetRecurringHoliday(id, isSet, user);
            }
        }

        private void SetMailToContactSupport()
        {
            var _contactSupport = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.SMTP, Constants.ResourceKeys.PMSupportEmailAddressKey);
            contactSupportMailToLink.NavigateUrl = string.Format(MailToSubjectFormat, _contactSupport, DataHelper.CurrentPerson.PersonLastFirstName);
        }
        
        protected void calendar_PreRender(object sender, EventArgs e)
        {
            MonthCalendar calendar = sender as MonthCalendar;

            if (days == null)
            {
                int? practiceManagerId = null;
                if ((!userIsAdministrator && userIsPracticeManager) ||
                    (!userIsAdministrator && userIsDirector) ||
                    (!userIsAdministrator && userIsSeniorLeadership)
                    )// #2817:(!userIsAdministrator && userIsDirector) is added as per the requirement.
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

            if (days != null && !userIsAdministrator && !userIsPracticeManager && !userIsDirector && !userIsSeniorLeadership &&     // #2817: userIsDirector is added as per the requirement.
                (userIsConsultant || userIsRecruiter || userIsSalesperson || userIsHR))                 // #2817: userIsHR is added as per the requirement.
            {
                // Security
                foreach (CalendarItem item in days)
                {
                    item.ReadOnly = true;
                }
                trAlert.Visible = true;
                pnlBody.Update();
                // lblConsultantMessage.Visible = true;
            }

            btnAddTimeOff.Visible = !trAlert.Visible;

            calendar.CalendarItems = days;
        }

        internal void ShowHolidayAndSubStituteDay(DateTime date, string holiDayDescription)
        {
            hdnHolidayDate.Value = lblHolidayDate.Text = date.ToString(Constants.Formatting.EntryDateFormat);
            lblHolidayName.Text = holiDayDescription;
            dpSubstituteDay.TextValue = "";
            mpeHolidayAndSubStituteDay.Show();
            upnlValsummary.Update();
        }
    }
}

