﻿using System;
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
        public const string ApprovedManagersViewState = "ApprovedManagersViewState";

        private CalendarItem[] days;
        private bool userIsPracticeManager;
        private bool userIsBusinessUnitManager;
        private bool userIsAdministrator;
        private bool userIsHR;
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

        private bool HasPermissionToEditCalender
        {
            get
            {
                UpdateRoleFields();

                if (userIsAdministrator || userIsBusinessUnitManager || userIsDirector || userIsHR || userIsPracticeManager)
                {
                    return true;
                }

                return false;

            }
        }

        private Person selectedPersonWithPayHistory;

        private Person SelectedPersonWithPayHistory
        {
            get
            {
                if (selectedPersonWithPayHistory == null || selectedPersonWithPayHistory.Id != SelectedPersonId.Value)
                    selectedPersonWithPayHistory = ServiceCallers.Custom.Person(p => p.GetPayHistoryShortByPerson(SelectedPersonId.Value));

                return selectedPersonWithPayHistory;
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
                return upnlBody;
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

        public Person[] ApprovedManagers
        {
            get
            {
                if (ViewState[ApprovedManagersViewState] == null)
                {
                    ViewState[ApprovedManagersViewState] = ServiceCallers.Custom.Person(p => p.GetCurrentActivePracticeAreaManagerList());
                }
                return (Person[])ViewState[ApprovedManagersViewState];
            }
        }

        #endregion

        public void PopulateSingleDayPopupControls(DateTime date, string timeTypeId, string hours, int? approvedById, string approvedByName)
        {
            lbdateSingleDay.Text = date.ToString(Constants.Formatting.EntryDateFormat);
            hdnDateSingleDay.Value = date.ToString();
            ddlTimeTypesSingleDay.SelectedValue = timeTypeId;
            txtHoursSingleDay.Text = string.IsNullOrEmpty(hours) ? "0.00" : Convert.ToDouble(hours).ToString(HoursFormat);
            hdIsSingleDayPopDirty.Value = false.ToString();
            btnDeleteSingleDay.Enabled = true;
            var timeTypeSelectedItem = ddlTimeTypesSingleDay.SelectedItem;
            if (timeTypeSelectedItem.Attributes["IsORT"].ToLower() == "true")
            {
                if (approvedById.HasValue)
                {
                    DataHelper.FillApprovedManagersList(ddlSingleDayApprovedManagers, "- - Select a Manager - -", ApprovedManagers, false);

                    if (ApprovedManagers.Where(p => p.Id == approvedById.Value).Count() > 0)
                    {
                        ddlSingleDayApprovedManagers.SelectedValue = approvedById.Value.ToString();
                    }
                    else
                    {
                        ddlSingleDayApprovedManagers.Items.Add(new ListItem(approvedByName, approvedById.Value.ToString()));
                        ddlSingleDayApprovedManagers.SelectedValue = approvedById.Value.ToString();
                    }

                }
                else
                {
                    ddlSingleDayApprovedManagers.SelectedIndex = 0;
                }
                trORTApprovedBy.Style.Add(AttributeDisplay, string.Empty);
                trORTManagersList.Style.Add(AttributeDisplay, string.Empty);
            }
            else
            {
                ddlSingleDayApprovedManagers.SelectedIndex = 0;
                trORTApprovedBy.Style.Add(AttributeDisplay, AttributeValueNone);
                trORTManagersList.Style.Add(AttributeDisplay, AttributeValueNone);
            }
        }

        public void PopulateSeriesPopupControls(DateTime startDate, DateTime endDate, string timeTypeId, string hours, int? approvedById, string approvedByName)
        {
            dtpStartDateTimeOff.DateValue = startDate;
            dtpEndDateTimeOff.DateValue = endDate;
            ddlTimeTypesTimeOff.SelectedValue = timeTypeId;
            txthoursTimeOff.Text = string.IsNullOrEmpty(hours) ? "0.00" : Convert.ToDouble(hours).ToString(HoursFormat);
            btnDeleteTimeOff.Visible = btnDeleteTimeOff.Enabled = true;
            hdIsTimeOffPopUpDirty.Value = false.ToString();

            var timeTypeSelectedItem = ddlTimeTypesTimeOff.SelectedItem;
            if (timeTypeSelectedItem.Attributes["IsORT"].ToLower() == "true")
            {
                if (approvedById.HasValue)
                {
                    DataHelper.FillApprovedManagersList(ddlApprovedManagers, "- - Select a Manager - -", ApprovedManagers, false);
                    if (ApprovedManagers.Where(p => p.Id == approvedById.Value).Count() > 0)
                    {
                        ddlApprovedManagers.SelectedValue = approvedById.Value.ToString();
                    }
                    else
                    {
                        ddlApprovedManagers.Items.Add(new ListItem(approvedByName, approvedById.Value.ToString()));
                        ddlApprovedManagers.SelectedValue = approvedById.Value.ToString();
                    }
                }
                else
                {
                    ddlApprovedManagers.SelectedIndex = 0;
                }
                trAddORTApprovedBy.Style.Add(AttributeDisplay, string.Empty);
                trAddORTManagersList.Style.Add(AttributeDisplay, string.Empty);
            }
            else
            {
                ddlApprovedManagers.SelectedIndex = 0;
                trAddORTApprovedBy.Style.Add(AttributeDisplay, AttributeValueNone);
                trAddORTManagersList.Style.Add(AttributeDisplay, AttributeValueNone);
            }
        }

        public void PopulateEditConditionPopupControls(DateTime startDate, DateTime endDate, DateTime selectedDate)
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

            mcJanuary.IsReadOnly = mcFebruary.IsReadOnly = mcMarch.IsReadOnly =
            mcApril.IsReadOnly = mcMay.IsReadOnly = mcJune.IsReadOnly =
            mcJuly.IsReadOnly = mcAugust.IsReadOnly = mcSeptember.IsReadOnly =
            mcOctober.IsReadOnly = mcNovember.IsReadOnly = mcDecember.IsReadOnly = !HasPermissionToEditCalender;

            SetMailToContactSupport();
            if (HasPermissionToEditCalender)
            {
                trAlert.Visible = false;
            }
            else
            {
                trAlert.Visible = true;
            }
            upnlBody.Update();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            upnlErrorSingleDay.Update();
        }


        private void UpdateRoleFields()
        {
            // Persons with the role Manager, HR, Client Director, or Administrator can add/edit time on calendars
            userIsPracticeManager =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.PracticeManagerRoleName);
            userIsBusinessUnitManager =
                 Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.BusinessUnitManagerRoleName);
            userIsHR =
               Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.HRRoleName);


            userIsDirector =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName);

            userIsAdministrator =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetMailToContactSupport();

            UpdateRoleFields();

            if (!IsPostBack)
            {
                if (!CompanyHolidays)
                {
                    //#2961: allowing all persons to be in the dropdown list irrespective of role.
                    DataHelper.FillPersonList(ddlPerson, null, (int)PersonStatusType.Active);
                    Person current = DataHelper.CurrentPerson;

                    ddlPerson.SelectedIndex =
                        ddlPerson.Items.IndexOf(ddlPerson.Items.FindByValue(current.Id.Value.ToString()));

                    var administrativeTimeTypes = ServiceCallers.Custom.TimeType(p => p.GetAllAdministrativeTimeTypes(true, false));
                    DataHelper.FillListDefault(ddlTimeTypesSingleDay, "- - Make Selection - -", administrativeTimeTypes, false);
                    DataHelper.FillListDefault(ddlTimeTypesTimeOff, "- - Make Selection - -", administrativeTimeTypes, false);
                    AddAttributesToTimeTypesDropdown(ddlTimeTypesSingleDay, administrativeTimeTypes);
                    AddAttributesToTimeTypesDropdown(ddlTimeTypesTimeOff, administrativeTimeTypes);

                    DataHelper.FillApprovedManagersList(ddlApprovedManagers, "- - Select a Manager - -", ApprovedManagers, false);
                    DataHelper.FillApprovedManagersList(ddlSingleDayApprovedManagers, "- - Select a Manager - -", ApprovedManagers, false);
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

        private void AddAttributesToTimeTypesDropdown(CustomDropDown ddlTimeTypes, DataTransferObjects.TimeEntry.TimeTypeRecord[] data)
        {
            foreach (ListItem item in ddlTimeTypes.Items)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    var id = Convert.ToInt32(item.Value);
                    var obj = data.Where(tt => tt.Id == id).FirstOrDefault();
                    if (obj != null)
                    {
                        item.Attributes.Add("IsORT", obj.IsORTTimeType.ToString());
                    }
                }
                else
                {
                    item.Attributes.Add("IsORT", false.ToString());
                }
            }
        }

        protected void btnDeleteSingleDay_OnClick(object sender, EventArgs e)
        {
            Page.Validate(valSumErrorSingleDay.ValidationGroup);
            if (Page.IsValid)
            {
                int? approvedBy = null;
                var timeTypeSelectedItem = ddlTimeTypesSingleDay.SelectedItem;
                if (timeTypeSelectedItem.Attributes["IsORT"].ToLower() == "true" && !string.IsNullOrEmpty(ddlSingleDayApprovedManagers.SelectedValue))
                {
                    approvedBy = Convert.ToInt32(ddlSingleDayApprovedManagers.SelectedValue);
                }
                var date = Convert.ToDateTime(hdnDateSingleDay.Value);
                ServiceCallers.Custom.Calendar(
                                               c => c.SaveTimeOff(date,
                                                                  date,
                                                                  false,
                                                                  SelectedPersonId.Value,
                                                                  (double?)Convert.ToDouble(txtHoursSingleDay.Text),
                                                                  Convert.ToInt32(ddlTimeTypesSingleDay.SelectedValue),
                                                                  Context.User.Identity.Name,
                                                                  approvedBy
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
                double hours = Convert.ToDouble(txtHoursSingleDay.Text);
                if (hours % 0.25 < 0.125)
                {
                    hours = hours - hours % 0.25;
                }
                else
                {
                    hours = hours + (0.25 - hours % 0.25);
                }
                int? approvedBy = null;
                var timeTypeSelectedItem = ddlTimeTypesSingleDay.SelectedItem;
                if (timeTypeSelectedItem.Attributes["IsORT"].ToLower() == "true" && !string.IsNullOrEmpty(ddlSingleDayApprovedManagers.SelectedValue))
                {
                    approvedBy = Convert.ToInt32(ddlSingleDayApprovedManagers.SelectedValue);
                }
                var date = Convert.ToDateTime(hdnDateSingleDay.Value);
                ServiceCallers.Custom.Calendar(
                                               c => c.SaveTimeOff(date,
                                                                  date,
                                                                  true,
                                                                  SelectedPersonId.Value,
                                                                  (double?)hours,
                                                                  Convert.ToInt32(ddlTimeTypesSingleDay.SelectedValue),
                                                                  Context.User.Identity.Name,
                                                                  approvedBy
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
            ddlApprovedManagers.SelectedIndex = 0;
            trAddORTApprovedBy.Style.Add(AttributeDisplay, AttributeValueNone);
            trAddORTManagersList.Style.Add(AttributeDisplay, AttributeValueNone);
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
                    double hours = Convert.ToDouble(txthoursTimeOff.Text);
                    if (hours % 0.25 < 0.125)
                    {
                        hours = hours - hours % 0.25;
                    }
                    else
                    {
                        hours = hours + (0.25 - hours % 0.25);
                    }
                    int? approvedBy = null;
                    var timeTypeSelectedItem = ddlTimeTypesTimeOff.SelectedItem;
                    if (timeTypeSelectedItem.Attributes["IsORT"].ToLower() == "true" && !string.IsNullOrEmpty(ddlApprovedManagers.SelectedValue))
                    {
                        approvedBy = Convert.ToInt32(ddlApprovedManagers.SelectedValue);
                    }
                    ServiceCallers.Custom.Calendar(
                        c => c.SaveTimeOff(dtpStartDateTimeOff.DateValue,
                                                                      dtpEndDateTimeOff.DateValue,
                                                                      true,
                                                                      SelectedPersonId.Value,
                                                                      (double?)hours,
                                                                      Convert.ToInt32(ddlTimeTypesTimeOff.SelectedValue),
                                                                      Context.User.Identity.Name,
                                                                      approvedBy
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
                int? approvedBy = null;
                var timeTypeSelectedItem = ddlTimeTypesTimeOff.SelectedItem;
                if (timeTypeSelectedItem.Attributes["IsORT"].ToLower() == "true" && !string.IsNullOrEmpty(ddlApprovedManagers.SelectedValue))
                {
                    approvedBy = Convert.ToInt32(ddlApprovedManagers.SelectedValue);
                }
                ServiceCallers.Custom.Calendar(
                   c => c.SaveTimeOff(dtpStartDateTimeOff.DateValue,
                                                                 dtpEndDateTimeOff.DateValue,
                                                                 false,
                                                                 SelectedPersonId.Value,
                                                                 (double?)Convert.ToDouble(txthoursTimeOff.Text),
                                                                 Convert.ToInt32(ddlTimeTypesTimeOff.SelectedValue),
                                                                 Context.User.Identity.Name,
                                                                 approvedBy
                                                                 )
                                              );
            }
            else
            {
                mpeAddTimeOff.Show();
            }


            upnlTimeOff.Update();
        }

        protected void cvSingleDayApprovedManagers_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            var ddlTimeTypesSingleDaySelectedItem = ddlTimeTypesSingleDay.SelectedItem;
            if (ddlTimeTypesSingleDaySelectedItem.Attributes["IsORT"].ToLower() == "true" && ddlSingleDayApprovedManagers.SelectedIndex == 0)
            {
                args.IsValid = false;
                trORTApprovedBy.Style.Add(AttributeDisplay, string.Empty);
                trORTManagersList.Style.Add(AttributeDisplay, string.Empty);
            }
        }

        protected void cvApprovedManagers_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            var ddlTimeTypesSelectedItem = ddlTimeTypesTimeOff.SelectedItem;
            if (ddlTimeTypesSelectedItem.Attributes["IsORT"].ToLower() == "true" && ddlApprovedManagers.SelectedIndex == 0)
            {
                args.IsValid = false;
                trAddORTApprovedBy.Style.Add(AttributeDisplay, string.Empty);
                trAddORTManagersList.Style.Add(AttributeDisplay, string.Empty);
            }
        }

        protected void cvSubstituteDay_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (string.IsNullOrEmpty(dpSubstituteDay.TextValue) || !string.IsNullOrEmpty(ExceptionMessage))
            {
                args.IsValid = false;
            }
        }

        protected void cvModifySubstituteday_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!string.IsNullOrEmpty(ExceptionMessage))
            {
                args.IsValid = false;
            }
        }

        protected void btnDeleteSubstituteDay_Click(object sender, EventArgs e)
        {
            DeleteSubstituteDay(SelectedPersonId.Value, Convert.ToDateTime(hdnHolidayDate.Value));
            mpeDeleteSubstituteDay.Hide();
        }

        private void DeleteSubstituteDay(int personId, DateTime substituteDate)
        {
            var userName = Context.User.Identity.Name;
            ServiceCallers.Custom.Calendar(c => c.DeleteSubstituteDay(personId, substituteDate, userName));
        }

        protected void btnModifySubstituteDayDelete_Click(object sender, EventArgs e)
        {
            DeleteSubstituteDay(SelectedPersonId.Value, Convert.ToDateTime(hdnSubstituteDate.Value));
            mpeModifySubstituteDay.Hide();
        }

        protected void btnSubstituteDayOK_Click(object sender, EventArgs e)
        {
            var validationGroup = ((Button)sender).ValidationGroup;
            Page.Validate(validationGroup);
            if (Page.IsValid)
            {
                CalendarItem ci = new CalendarItem()
                {
                    SubstituteDayDate = dpSubstituteDay.DateValue,
                    Date = Convert.ToDateTime(hdnHolidayDate.Value),
                    PersonId = SelectedPersonId
                };

                try
                {
                    if (SaveSubstituteDay(ci, validationGroup))
                    {
                        mpeHolidayAndSubStituteDay.Hide();
                    }
                    else
                    {
                        mpeHolidayAndSubStituteDay.Show();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                mpeHolidayAndSubStituteDay.Show();
            }

            upnlValsummary.Update();

        }

        private bool SaveSubstituteDay(CalendarItem item, string validationGroup)
        {
            try
            {
                var userName = Context.User.Identity.Name;
                ServiceCallers.Custom.Calendar(c => c.SaveSubstituteDay(item, userName));
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message == SubstituteDateValidationMessage)
                {
                    ExceptionMessage = ex.Message;
                    Page.Validate(validationGroup);
                }
                else
                {
                    throw ex;
                }
            }
            return false;
        }

        protected void btnModifySubstituteDayOk_Click(object sender, EventArgs e)
        {
            var validationGroup = ((Button)sender).ValidationGroup;
            Page.Validate(validationGroup);
            if (Page.IsValid)
            {
                CalendarItem ci = new CalendarItem()
                {
                    SubstituteDayDate = dpModifySubstituteday.DateValue,
                    Date = Convert.ToDateTime(hdnHolidayDay.Value),
                    PersonId = SelectedPersonId
                };

                try
                {
                    if (SaveSubstituteDay(ci, validationGroup))
                    {
                        mpeModifySubstituteDay.Hide();
                    }
                    else
                    {
                        mpeModifySubstituteDay.Show();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                mpeModifySubstituteDay.Show();
            }

            upnlModifySubstituteDay.Update();
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

                if ((!userIsAdministrator) && (userIsPracticeManager || userIsBusinessUnitManager || userIsDirector || userIsHR))
                {
                    Person current = DataHelper.CurrentPerson;
                    practiceManagerId = current != null ? current.Id : 0;
                }

                DateTime firstMonthDay = new DateTime(SelectedYear, 1, 1);
                DateTime lastMonthDay = new DateTime(SelectedYear, 12, DateTime.DaysInMonth(SelectedYear, 12));

                DateTime firstDisplayedDay = firstMonthDay.AddDays(-(double)firstMonthDay.DayOfWeek);
                DateTime lastDisplayedDay = lastMonthDay.AddDays(6.0 - (double)lastMonthDay.DayOfWeek);

                using (var serviceClient = new CalendarServiceClient())
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

            if (days != null)
            {
                if (!userIsAdministrator && !userIsPracticeManager && !userIsBusinessUnitManager && !userIsDirector && !userIsHR)
                {
                    // Security
                    foreach (CalendarItem item in days)
                    {
                        item.ReadOnly = true;
                    }
                    trAlert.Visible = true;
                }
                else
                {
                    var personPayHistory = SelectedPersonWithPayHistory.PaymentHistory;

                    foreach (CalendarItem item in days)
                    {
                        if (personPayHistory.Any(p => (p.Timescale == TimescaleType.Salary && item.Date >= p.StartDate && (p.EndDate == null || item.Date <= p.EndDate))))
                        {
                            item.ReadOnly = false;
                        }
                        else
                        {
                            item.ReadOnly = true;
                        }
                    }

                    trAlert.Visible = true;
                }

                upnlBody.Update();
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

        internal void ShowModifySubstituteDay(DateTime holidayDate, string holidayDescription)
        {
            hdnHolidayDay.Value = holidayDate.ToShortDateString();
            DateTime substituteDate = GetSubstituteDate(holidayDate, SelectedPersonId.Value);
            hdnSubstituteDate.Value = substituteDate.ToShortDateString();
            dpModifySubstituteday.DateValue = substituteDate;
            lblModifySubstituteday.Text = substituteDate.ToString(Constants.Formatting.EntryDateFormat);
            lblHolidayDetails.Text = string.Format(HolidayDetails_Format, holidayDate.ToString(Constants.Formatting.EntryDateFormat), holidayDescription);
            btnModifySubstituteDayOk.Enabled = false;

            mpeModifySubstituteDay.Show();
            upnlModifySubstituteDay.Update();
        }

        private DateTime GetSubstituteDate(DateTime holidayDate, int personId)
        {
            return DataHelper.GetSubstituteDate(holidayDate, personId);
        }
    }
}

