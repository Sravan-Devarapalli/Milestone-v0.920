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
    public partial class PersonCalendar : System.Web.UI.UserControl
    {
        #region Constants

        private const string YearKey = "Year";
        private const string MailToSubjectFormat = "mailto:{0}?subject=Permissions for {1}'s calendar";
        public const string showEditSeriesOrSingleDayMessage = "Do you want to edit the series ({0} – {1}) or edit the single day ({2})?";
        public const string HoursFormat = "0.00";
        public const string TimeOffValidationMessage = "Selected day(s) are not working day(s). Please select any working day(s).";
        public const string SubstituteDateValidationMessage = "The selected date is not a working day.";
        public const string HolidayDetails_Format = "{0} - {1}";
        public const string AttributeDisplay = "display";
        public const string AttributeValueNone = "none";
        public const string IsW2HourlyAllowedAttribute = "IsW2HourlyAllowed";
        public const string IsW2SalaryAllowedAttribute = "IsW2SalaryAllowed";
        public const string IsUnpaidAttribute = "IsUnpaid";
        public const string IsORTAttribute = "IsORT";
        public const string DefaultHours = "8.00";
        public const string EmptyHours = "0.00";
        private bool userIsPracticeManager;
        private bool userIsBusinessUnitManager;
        private bool userIsAdministrator;
        private bool userIsHR;
        private bool userIsDirector;
        public const string SalaryVoliationForMessage = "Invalid Pay Type: Employee is not a W2-Salary employee for the selected day(s).";
        public const string HourlyVoliationForMessage = "Invalid Pay Type: Employee is not a W2-Hourly employee for the selected day(s).";
        public const string EmployeeVoliationForMessage = "Invalid Pay Type: Employee is not a W2-Salary/W2-Hourly employee for the selected day(s).";


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

        public string ExceptionMessage
        {
            get;
            set;
        }

        public int? SelectedPersonId
        {
            get
            {
                return Convert.ToInt32(ViewState["SelectedPerson_Key"]);
            }
            set
            {
                ViewState["SelectedPerson_Key"] = value;
            }
        }

        public HiddenField hdnDayOffObject
        {
            get
            {
                return hndDayOff;
            }
        }

        public HiddenField hdnDateObject
        {
            get
            {
                return hdnDate;
            }
        }

        public bool HasPermissionToEditCalender
        {
            get
            {
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

        public CalendarItem[] CalendarItems
        {
            get
            {
                return ViewState["CalendarItems_Key"] as CalendarItem[];
            }
            set
            {
                ViewState["CalendarItems_Key"] = value;
            }

        }

        public bool IsUserHrORAdmin
        {
            get
            {
                return userIsAdministrator || userIsHR;
            }
        }

        private Person selectedPerson;

        private Person SelectedPerson
        {
            get
            {
                if (selectedPerson == null)
                    selectedPerson = DataHelper.GetPersonHireAndTerminationDate(SelectedPersonId.Value);
                return selectedPerson;
            }
        }

        #endregion

        public void PopulateSingleDayPopupControls(DateTime date, string timeTypeId, string hours, int? approvedById, string approvedByName)
        {
            lbdateSingleDay.Text = date.ToString(Constants.Formatting.EntryDateFormat);
            hdnDateSingleDay.Value = date.ToString();
            //ddlTimeTypesSingleDay.SelectedValue = timeTypeId;
            ListItem selectedTimeType = null;
            selectedTimeType = ddlTimeTypesSingleDay.Items.FindByValue(timeTypeId.ToString());
            if (selectedTimeType == null)
            {
                var timetype = ServiceCallers.Custom.TimeType(te => te.GetWorkTypeById(int.Parse(timeTypeId)));
                selectedTimeType = new ListItem(timetype.Name, timeTypeId.ToString());
                selectedTimeType.Attributes.Add(IsORTAttribute, timetype.IsORTTimeType.ToString());
                selectedTimeType.Attributes.Add(IsUnpaidAttribute, timetype.IsUnpaidTimeType.ToString());
                selectedTimeType.Attributes.Add(IsW2HourlyAllowedAttribute, timetype.IsW2HourlyAllowed.ToString());
                selectedTimeType.Attributes.Add(IsW2SalaryAllowedAttribute, timetype.IsW2SalaryAllowed.ToString());
                ddlTimeTypesSingleDay.Items.Add(selectedTimeType);
            }
            ddlTimeTypesSingleDay.SelectedValue = selectedTimeType.Value;
            txtHoursSingleDay.Text = string.IsNullOrEmpty(hours) ? EmptyHours : Convert.ToDouble(hours).ToString(HoursFormat);
            hdIsSingleDayPopDirty.Value = false.ToString();
            btnDeleteSingleDay.Enabled = true;
        }

        public void PopulateSeriesPopupControls(DateTime startDate, DateTime endDate, string timeTypeId, string hours, int? approvedById, string approvedByName)
        {
            dtpStartDateTimeOff.DateValue = startDate;
            dtpEndDateTimeOff.DateValue = endDate;
            //ddlTimeTypesTimeOff.SelectedValue = timeTypeId;
            ListItem selectedTimeType = null;
            selectedTimeType = ddlTimeTypesTimeOff.Items.FindByValue(timeTypeId.ToString());
            if (selectedTimeType == null)
            {
                var timetype = ServiceCallers.Custom.TimeType(te => te.GetWorkTypeById(int.Parse(timeTypeId)));
                selectedTimeType = new ListItem(timetype.Name, timeTypeId.ToString());
                selectedTimeType.Attributes.Add(IsORTAttribute, timetype.IsORTTimeType.ToString());
                selectedTimeType.Attributes.Add(IsUnpaidAttribute, timetype.IsUnpaidTimeType.ToString());
                selectedTimeType.Attributes.Add(IsW2HourlyAllowedAttribute, timetype.IsW2HourlyAllowed.ToString());
                selectedTimeType.Attributes.Add(IsW2SalaryAllowedAttribute, timetype.IsW2SalaryAllowed.ToString());
                ddlTimeTypesTimeOff.Items.Add(selectedTimeType);
            }
            ddlTimeTypesTimeOff.SelectedValue = selectedTimeType.Value;
            txthoursTimeOff.Text = string.IsNullOrEmpty(hours) ? EmptyHours : Convert.ToDouble(hours).ToString(HoursFormat);
            btnDeleteTimeOff.Visible = btnDeleteTimeOff.Enabled = true;
            hdIsTimeOffPopUpDirty.Value = false.ToString();
        }

        public void PopulateEditConditionPopupControls(DateTime startDate, DateTime endDate, DateTime selectedDate)
        {
            rbEditSeries.Checked = true;
            rbEditSingleDay.Checked = false;
            lbDate.Text = String.Format(Calendar.showEditSeriesOrSingleDayMessage, startDate.ToString(Constants.Formatting.EntryDateFormat), endDate.ToString(Constants.Formatting.EntryDateFormat), selectedDate.ToString(Constants.Formatting.EntryDateFormat));

        }

        private void UpdateCalendar()
        {
            lblYear.Text = SelectedYear.ToString();

            SetMailToContactSupport();

            trAlert.Visible = !HasPermissionToEditCalender;
            btnAddTimeOff.Visible = HasPermissionToEditCalender;


            int? practiceManagerId = null;

            if ((!userIsAdministrator) && (userIsPracticeManager || userIsBusinessUnitManager || userIsDirector || userIsHR))
            {
                practiceManagerId = DataHelper.CurrentPerson.Id;
            }

            DateTime firstMonthDay = new DateTime(SelectedYear, 1, 1);
            DateTime lastMonthDay = new DateTime(SelectedYear, 12, DateTime.DaysInMonth(SelectedYear, 12));

            DateTime firstDisplayedDay = firstMonthDay.AddDays(-(double)firstMonthDay.DayOfWeek);
            DateTime lastDisplayedDay = lastMonthDay.AddDays(6.0 - (double)lastMonthDay.DayOfWeek);


            var days =
                 ServiceCallers.Custom.Calendar(c => c.GetPersonCalendar(firstDisplayedDay, lastDisplayedDay, SelectedPersonId, practiceManagerId));


            if (days != null)
            {
                if (!HasPermissionToEditCalender)
                {
                    // Security
                    foreach (CalendarItem item in days)
                    {
                        item.ReadOnly = true;
                    }
                }
                else
                {
                    var personPayHistory = SelectedPersonWithPayHistory.PaymentHistory;

                    var isVisible = false;

                    foreach (CalendarItem item in days)
                    {
                        if (item.Date >= SelectedPerson.HireDate
                            && (!SelectedPerson.TerminationDate.HasValue || (SelectedPerson.TerminationDate.HasValue && item.Date <= SelectedPerson.TerminationDate))
                            && personPayHistory.Any(p => ((p.Timescale == TimescaleType.Salary || (p.Timescale == TimescaleType.Hourly && !item.CompanyDayOff && !item.IsFloatingHoliday)) && item.Date >= p.StartDate && (p.EndDate == null || item.Date <= p.EndDate)))
                            )
                        {
                            item.ReadOnly = false;
                            isVisible = true;
                        }
                        else
                        {
                            item.ReadOnly = true;
                        }
                    }

                    btnAddTimeOff.Visible = isVisible;
                }

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
                string statusIds = (int)PersonStatusType.Active + "," + (int)PersonStatusType.TerminationPending;
                //#2961: allowing all persons to be in the dropdown list irrespective of role.
                DataHelper.FillPersonList(ddlPerson, null, statusIds);
                Person current = DataHelper.CurrentPerson;

                ddlPerson.SelectedValue = current.Id.Value.ToString();
                SelectedYear = DateTime.Today.Year;
                SelectedPersonId = current.Id;
                PopulateTimeTypesDropDown();
                UpdateCalendar();
            }

        }

        private void PopulateTimeTypesDropDown()
        {
            bool isUserHr = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.HRRoleName);
            bool isUserAdminstrator = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
            bool isUnpaidWorktypeInclude = false;
            if (isUserHr || isUserAdminstrator)
            {
                isUnpaidWorktypeInclude = true;
            }
            DateTime yearStartdate = new DateTime(SelectedYear, 1, 1);
            DateTime yearEnddate = new DateTime(SelectedYear, 12, 31);
            DateTime startdate = SelectedPerson.HireDate > yearStartdate ? SelectedPerson.HireDate : yearStartdate;
            var administrativeTimeTypes = ServiceCallers.Custom.Person(p => p.GetPersonAdministrativeTimeTypesInRange(SelectedPersonId.Value, startdate, yearEnddate, true, false, isUnpaidWorktypeInclude, true));
            DataHelper.FillListDefault(ddlTimeTypesSingleDay, "- - Make Selection - -", administrativeTimeTypes, false);
            DataHelper.FillListDefault(ddlTimeTypesTimeOff, "- - Make Selection - -", administrativeTimeTypes, false);
            AddAttributesToTimeTypesDropdown(ddlTimeTypesSingleDay, administrativeTimeTypes);
            AddAttributesToTimeTypesDropdown(ddlTimeTypesTimeOff, administrativeTimeTypes);
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
                        item.Attributes.Add(IsORTAttribute, obj.IsORTTimeType.ToString());
                        item.Attributes.Add(IsUnpaidAttribute, obj.IsUnpaidTimeType.ToString());
                        item.Attributes.Add(IsW2HourlyAllowedAttribute, obj.IsW2HourlyAllowed.ToString());
                        item.Attributes.Add(IsW2SalaryAllowedAttribute, obj.IsW2SalaryAllowed.ToString());
                    }
                }
                else
                {
                    item.Attributes.Add(IsORTAttribute, false.ToString());
                    item.Attributes.Add(IsUnpaidAttribute, false.ToString());
                    item.Attributes.Add(IsW2HourlyAllowedAttribute, false.ToString());
                    item.Attributes.Add(IsW2SalaryAllowedAttribute, false.ToString());
                }
            }
        }

        protected void btnDeleteSingleDay_OnClick(object sender, EventArgs e)
        {
            Page.Validate(valSumErrorSingleDay.ValidationGroup);
            if (Page.IsValid)
            {
                int? approvedBy = null;

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
                UpdateCalendar();

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
                if (timeTypeSelectedItem.Attributes[IsORTAttribute].ToLower() == "true" || timeTypeSelectedItem.Attributes[IsUnpaidAttribute].ToLower() == "true")
                {
                    approvedBy = Convert.ToInt32(DataHelper.CurrentPerson.Id);
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

                UpdateCalendar();

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
            txthoursTimeOff.Text = DefaultHours;
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

                    if (timeTypeSelectedItem.Attributes[IsORTAttribute].ToLower() == "true" || timeTypeSelectedItem.Attributes[IsUnpaidAttribute].ToLower() == "true")
                    {
                        approvedBy = Convert.ToInt32(DataHelper.CurrentPerson.Id);
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

                    UpdateCalendar();
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
                UpdateCalendar();
            }
            else
            {
                mpeAddTimeOff.Show();
            }


            upnlTimeOff.Update();
        }

        protected void cvSubstituteDay_ServerValidate(object source, ServerValidateEventArgs args)
        {
            //if (rfvSubstituteDay.IsValid)
            //{
            if (string.IsNullOrEmpty(dpSubstituteDay.TextValue) || !string.IsNullOrEmpty(ExceptionMessage))
            {
                args.IsValid = false;
            }
            //}
        }

        protected void cvModifySubstituteday_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!string.IsNullOrEmpty(ExceptionMessage))
            {
                args.IsValid = false;
            }
        }

        protected void cvSingleDay_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var validator = source as CustomValidator;
            var date = Convert.ToDateTime(hdnDateSingleDay.Value);
            validator.ErrorMessage = validator.ToolTip = EmployeeVoliationForMessage;
            var personPayHistory = SelectedPersonWithPayHistory.PaymentHistory;
            var selectTimeType = ddlTimeTypesSingleDay.SelectedItem;
            bool isW2SalaryAllowed = selectTimeType.Attributes[IsW2SalaryAllowedAttribute] == true.ToString();
            bool isUnpaidTimeType = selectTimeType.Attributes[IsUnpaidAttribute] == true.ToString();
            bool isW2HourlyAllowed = selectTimeType.Attributes[IsW2HourlyAllowedAttribute] == true.ToString();

            CalendarItem ci = CalendarItems.ToList().First(c => c.Date == date);
            if (ci.ReadOnly)
            {
                args.IsValid = false;
            }
            else
            {
                if (isUnpaidTimeType && !personPayHistory.Any(p => date >= p.StartDate && date <= p.EndDate && (p.Timescale == TimescaleType.Salary || p.Timescale == TimescaleType.Hourly)))
                {
                    args.IsValid = false;
                    return;
                }

                if (!isUnpaidTimeType && isW2HourlyAllowed && !personPayHistory.Any(p => date >= p.StartDate && date <= p.EndDate && p.Timescale == TimescaleType.Hourly))
                {
                    args.IsValid = false;
                    validator.ErrorMessage = validator.ToolTip = HourlyVoliationForMessage;
                    return;
                }

                if (!isUnpaidTimeType && isW2SalaryAllowed && !personPayHistory.Any(p => date >= p.StartDate && date <= p.EndDate && p.Timescale == TimescaleType.Salary))
                {
                    args.IsValid = false;
                    validator.ErrorMessage = validator.ToolTip = SalaryVoliationForMessage;

                    return;
                }
            }

        }

        protected void cvNotW2Salary_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var validator = source as CustomValidator;
            validator.ErrorMessage = validator.ToolTip = SalaryVoliationForMessage;
            var startDate = dtpStartDateTimeOff.DateValue.Date;
            var endDate = dtpEndDateTimeOff.DateValue.Date;
            var personPayHistory = SelectedPersonWithPayHistory.PaymentHistory;
            var selectTimeType = ddlTimeTypesTimeOff.SelectedItem;
            bool isW2SalaryAllowed = selectTimeType.Attributes[IsW2SalaryAllowedAttribute] == true.ToString();
            bool isUnpaidTimeType = selectTimeType.Attributes[IsUnpaidAttribute] == true.ToString();
            while (startDate <= endDate)
            {
                if (isW2SalaryAllowed && !personPayHistory.Any(p => startDate >= p.StartDate && startDate <= p.EndDate && p.Timescale == TimescaleType.Salary))
                {
                    if (!isUnpaidTimeType || (isUnpaidTimeType && !personPayHistory.Any(p => startDate >= p.StartDate && startDate <= p.EndDate && (p.Timescale == TimescaleType.Salary || p.Timescale == TimescaleType.Hourly))))
                    {
                        args.IsValid = false;
                        break;
                    }
                }

                startDate = startDate.AddDays(1);
            }

        }

        protected void cvNotW2Hourly_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var validator = source as CustomValidator;
            validator.ErrorMessage = validator.ToolTip = HourlyVoliationForMessage;
            var startDate = dtpStartDateTimeOff.DateValue.Date;
            var endDate = dtpEndDateTimeOff.DateValue.Date;
            var personPayHistory = SelectedPersonWithPayHistory.PaymentHistory;
            var selectTimeType = ddlTimeTypesTimeOff.SelectedItem;
            bool isW2HourlyAllowed = selectTimeType.Attributes[IsW2HourlyAllowedAttribute] == true.ToString();
            bool isUnpaidTimeType = selectTimeType.Attributes[IsUnpaidAttribute] == true.ToString();
            while (startDate <= endDate)
            {
                if (isW2HourlyAllowed && !personPayHistory.Any(p => startDate >= p.StartDate && startDate <= p.EndDate && p.Timescale == TimescaleType.Hourly))
                {
                    if (!isUnpaidTimeType || (isUnpaidTimeType && !personPayHistory.Any(p => startDate >= p.StartDate && startDate <= p.EndDate && (p.Timescale == TimescaleType.Salary || p.Timescale == TimescaleType.Hourly))))
                    {
                        args.IsValid = false;
                        break;
                    }
                }

                startDate = startDate.AddDays(1);
            }

        }

        protected void cvSubstituteDayNotHavingW2Salary_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var substituteDate = dpSubstituteDay.DateValue.Date;
            var personPayHistory = SelectedPersonWithPayHistory.PaymentHistory;

            if (!IsPersonNotHired(substituteDate) && !personPayHistory.Any(p => substituteDate >= p.StartDate && substituteDate <= p.EndDate && p.Timescale == TimescaleType.Salary))
            {
                args.IsValid = false;

            }
        }

        protected void cvModifiedSubstituteDayNotHavingW2Salary_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var modifiedSubDate = dpModifySubstituteday.DateValue.Date;
            var personPayHistory = SelectedPersonWithPayHistory.PaymentHistory;

            if (!IsPersonNotHired(modifiedSubDate) && !personPayHistory.Any(p => modifiedSubDate >= p.StartDate && modifiedSubDate <= p.EndDate && p.Timescale == TimescaleType.Salary))
            {
                args.IsValid = false;

            }

        }

        protected void cvPersonNotHired_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var startDate = dtpStartDateTimeOff.DateValue.Date;

            if (IsPersonNotHired(startDate))
            {
                args.IsValid = false;
            }

        }

        protected void cvPersonTerminated_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var endDate = dtpEndDateTimeOff.DateValue.Date;

            if (IsPersonTerminated(endDate))
            {
                args.IsValid = false;
            }
        }

        protected void cvValidateSubDateWithHireDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            //if (rfvSubstituteDay.IsValid)
            //{
            var substituteDate = dpSubstituteDay.DateValue.Date;
            if (IsPersonNotHired(substituteDate))
            {
                args.IsValid = false;
            }
            //}

        }

        protected void cvValidateSubDateWithTermDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            //    if (rfvSubstituteDay.IsValid)
            //    {
            var substituteDate = dpSubstituteDay.DateValue.Date;
            if (IsPersonTerminated(substituteDate))
            {
                args.IsValid = false;
            }
            //}
        }

        protected void cvValidateModifiedSubDateWithHireDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var modifiedSubDate = dpModifySubstituteday.DateValue.Date;

            if (IsPersonNotHired(modifiedSubDate))
            {
                args.IsValid = false;
            }
        }

        protected void cvValidateModifiedSubDateWithTermDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var modifiedSubDate = dpModifySubstituteday.DateValue.Date;

            if (IsPersonTerminated(modifiedSubDate))
            {
                args.IsValid = false;
            }
        }

        public Boolean IsPersonNotHired(DateTime date)
        {
            if (date < SelectedPerson.HireDate)
            {
                return true;
            }
            return false;

        }

        public Boolean IsPersonTerminated(DateTime date)
        {
            if (selectedPerson.TerminationDate.HasValue && SelectedPerson.TerminationDate < date)
            {
                return true;
            }
            return false;
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
            UpdateCalendar();
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
                        UpdateCalendar();
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
                        UpdateCalendar();
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

        protected void btnRetrieveCalendar_Click(object sender, EventArgs e)
        {
            SelectedPersonId = Convert.ToInt32(ddlPerson.SelectedValue);
            PopulateTimeTypesDropDown();
            UpdateCalendar();
        }

        protected void btnPrevYear_Click(object sender, EventArgs e)
        {
            SelectedPersonId = Convert.ToInt32(ddlPerson.SelectedValue);
            SelectedYear--;
            PopulateTimeTypesDropDown();
            UpdateCalendar();
        }

        protected void btnNextYear_Click(object sender, EventArgs e)
        {
            SelectedPersonId = Convert.ToInt32(ddlPerson.SelectedValue);
            SelectedYear++;
            PopulateTimeTypesDropDown();
            UpdateCalendar();
        }

        private void SetMailToContactSupport()
        {
            var _contactSupport = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.SMTP, Constants.ResourceKeys.PMSupportEmailAddressKey);
            contactSupportMailToLink.NavigateUrl = string.Format(MailToSubjectFormat, _contactSupport, DataHelper.CurrentPerson.PersonLastFirstName);
        }

        internal void ShowHolidayAndSubStituteDay(DateTime date, string holiDayDescription)
        {
            hdnHolidayDate.Value = lblHolidayDate.Text = date.ToString(Constants.Formatting.EntryDateFormat);
            lblHolidayName.Text = HttpUtility.HtmlEncode(holiDayDescription);
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
            lblHolidayDetails.Text = string.Format(HolidayDetails_Format, holidayDate.ToString(Constants.Formatting.EntryDateFormat), HttpUtility.HtmlEncode(holidayDescription));
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

