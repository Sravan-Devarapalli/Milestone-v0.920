﻿using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using DataTransferObjects.TimeEntry;
using PraticeManagement.Controls;
using PraticeManagement.MilestonePersonService;
using PraticeManagement.MilestoneService;
using PraticeManagement.PersonService;
using PraticeManagement.Security;
using PraticeManagement.Utils;
using Resources;
using System.Web.UI.HtmlControls;
using System.Linq;
using PraticeManagement.PersonRoleService;
using PraticeManagement.Controls.Milestones;
using System.Data;
using PraticeManagement.ProjectService;

namespace PraticeManagement.Controls.Milestones
{
    public partial class MilestonePersonList : UserControl
    {
        #region Constants

        private const string MILESTONE_PERSON_ID_ARGUMENT = "milestonePersonId";
        private const int AMOUNT_COLUMN_INDEX = 8;
        private const string MILESTONE_PERSONS_KEY = "MilestonePersons";
        private const string ADD_MILESTONE_PERSON_ENTRIES_KEY = "ADD_MILESTONE_PERSON_ENTRIES_KEY";
        private const string PERSONSLISTFORMILESTONE_KEY = "PERSONSLISTFORMILESTONE_KEY";
        private const string ROLELISTFORPERSONS_KEY = "ROLELISTFORPERSONS_KEY";
        private const string DDLPERSON_KEY = "ddlPerson";
        private const string DDLROLE_KEY = "ddlRole";
        private const string DEFAULT_NUMBER_HOURS_PER_DAY = "8";
        private const string PERSON_ID_KEY = "PERSON_ID_KEY";
        private const string mpbar = "mpbar";
        private const string DuplPersonName = "The specified person is already assigned on this milestone.";
        private const string lblTargetMargin = "lblTargetMargin";
        private const string dpPersonStartInsert = "dpPersonStartInsert";
        private const string dpPersonEndInsert = "dpPersonEndInsert";
        private const string txtAmountInsert = "txtAmountInsert";
        private const string txtHoursPerDayInsert = "txtHoursPerDayInsert";
        private const string txtHoursInPeriodInsert = "txtHoursInPeriodInsert";
        private const string milestoneHasTimeEntries = "Cannot delete milestone person because this person has already entered time for this milestone.";
        private const string milestoneHasFeedbackEntries = "This person cannot be deleted from this milestone because project feedback has been marked as completed.  The person can be deleted from the milestone if the status of the feedback is changed to 'Not Completed' or 'Canceled'. Please navigate to the 'Project Feedback' tab for more information to make the necessary adjustments.";
        private const string allMilestoneHasTimeEntries = "Cannot delete all milestone person entries because this person has already entered time for this milestone.";
        private const string milestonePersonEntry = "MilestonePersonEntry";
        public const string BothDatesEmployementError = "{0} cannot be assigned to the project due to his/her hire date-{1} and termination date-{2} from the company. Please update his/her start and end dates to align with that date.";
        public const string TotalOutOfEmploymentError = "The person you are trying to add is not set as being active during the entire length of their participation in the milestone.  Please adjust the person's hire and compensation records, or change the dates that they are attached to this milestone.";
        public const string TerminationDateEmployementError = "{0} cannot be assigned to the project past {1} due to his/her termination from the company. Please update his/her start and end dates to align with that date.";
        public const string HireDateEmployementError = "{0} cannot be assigned to the project before {1} as he/she is not hired into the company. Please update his/her start and end dates to align with that date.";
        private string ShowPanel = "ShowPanel('{0}', '{1}','{2}','{3}','{4}','{5}');";
        private string HidePanel = "HidePanel('{0}');";
        private string OnMouseOver = "onmouseover";
        private string OnMouseOut = "onmouseout";

        #endregion

        #region Fields

        private ExceptionDetail _internalException;
        private Milestone _milestoneValue;
        private bool errorOccured = false;

        private SeniorityAnalyzer _seniorityAnalyzer;

        #endregion

        #region Properties

        private PraticeManagement.MilestoneDetail HostingPage
        {
            get { return ((PraticeManagement.MilestoneDetail)Page); }
        }

        public String ExMessage { get; set; }

        public Milestone Milestone
        {
            get
            {
                return HostingPage.Milestone;
            }
        }

        public GridView gvMilestonePersonEntriesObject
        {
            get { return gvMilestonePersonEntries; }
        }

        public MessageLabel lblResultMessageObject
        {
            get
            {
                return lblResultMessage;
            }
        }

        public ValidationSummary vsumMileStonePersonsObject
        {
            get
            {
                return vsumMileStonePersons;
            }
        }

        public List<MilestonePersonEntry> MilestonePersonsEntries
        {
            get
            {
                var entries = ViewState[MILESTONE_PERSONS_KEY] as List<MilestonePersonEntry>;
                if (entries == null)
                {
                    entries = new List<MilestonePersonEntry>();
                }

                return entries;
            }
            set
            {
                ViewState[MILESTONE_PERSONS_KEY] = value;
            }
        }


        public List<MilestonePersonEntry> AddMilestonePersonEntries
        {
            get
            {
                return ViewState[ADD_MILESTONE_PERSON_ENTRIES_KEY] as List<MilestonePersonEntry>;
            }
            set
            {
                ViewState[ADD_MILESTONE_PERSON_ENTRIES_KEY] = value;
            }
        }


        public List<Dictionary<string, string>> repeaterOldValues
        {
            get;
            set;
        }


        public Person[] PersonsListForMilestone
        {
            get
            {
                if (ViewState[PERSONSLISTFORMILESTONE_KEY] != null)
                {
                    return ViewState[PERSONSLISTFORMILESTONE_KEY] as Person[];
                }

                using (var serviceClient = new PersonServiceClient())
                {
                    try
                    {
                        Person[] persons = serviceClient.PersonListAllForMilestone(null, Milestone.StartDate, Milestone.EndDate);
                        ViewState[PERSONSLISTFORMILESTONE_KEY] = persons;
                        Array.Sort(persons);
                        return persons;
                    }
                    catch (CommunicationException)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }

            }
            set
            {
                ViewState[PERSONSLISTFORMILESTONE_KEY] = value;
            }
        }


        public PersonRole[] RoleListForPersons
        {
            get
            {
                if (ViewState[ROLELISTFORPERSONS_KEY] != null)
                {
                    return ViewState[ROLELISTFORPERSONS_KEY] as PersonRole[];
                }

                using (var serviceClient = new PersonRoleServiceClient())
                {
                    try
                    {
                        PersonRole[] roles = serviceClient.GetPersonRoles();

                        ViewState[ROLELISTFORPERSONS_KEY] = roles;
                        return roles;
                    }
                    catch (CommunicationException)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }

            }
            set
            {
                ViewState[ROLELISTFORPERSONS_KEY] = value;
            }

        }


        private bool? IsUserHasPermissionOnProject
        {
            get
            {
                int? milestoneId = HostingPage.MilestoneId;

                if (milestoneId.HasValue)
                {
                    if (ViewState["HasPermission"] == null)
                    {
                        ViewState["HasPermission"] = DataHelper.IsUserHasPermissionOnProject(HostingPage.User.Identity.Name, milestoneId.Value, false);
                    }
                    return (bool)ViewState["HasPermission"];
                }

                return null;
            }
        }

        private bool? IsUserisOwnerOfProject
        {
            get
            {
                int? id = HostingPage.MilestoneId;
                if (id.HasValue)
                {
                    if (ViewState["IsOwnerOfProject"] == null)
                    {
                        ViewState["IsOwnerOfProject"] = DataHelper.IsUserIsOwnerOfProject(HostingPage.User.Identity.Name, id.Value, false);
                    }
                    return (bool)ViewState["IsOwnerOfProject"];
                }

                return null;
            }
        }

        public bool isInsertedRowsAreNotsaved
        {
            get
            {
                return (MilestonePersonsEntries.Any(entr => entr.IsEditMode) || repPerson.Items.Count > 0) ? true : false;
            }
        }

        private bool IsSaveCommit
        {
            get;
            set;
        }

        public bool ISDatBindRows { get; set; }

        public bool IsErrorOccuredWhileUpdatingRow { get; set; }

        #endregion

        #region Methods

        #region Validation

        protected void custPersonStart_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var dpPersonStart = ((Control)source).Parent.FindControl("dpPersonStart") as DatePicker;

            var result = dpPersonStart.DateValue.Date >= (HostingPage.IsSaveAllClicked ? HostingPage.dtpPeriodFromObject.DateValue : Milestone.StartDate.Date);

            if (HostingPage.IsSaveAllClicked && HostingPage.dtpPeriodFromObject.DateValue.Date > HostingPage.Milestone.StartDate.Date)
            {
                result = true;
            }

            args.IsValid = result;
        }

        protected void custPersonEnd_ServerValidate(object source, ServerValidateEventArgs args)
        {
            CustomValidator custPerson = source as CustomValidator;
            GridViewRow gvRow = custPerson.NamingContainer as GridViewRow;
            var ddl = gvRow.FindControl("ddlPersonName") as DropDownList;
            Person person = GetPersonBySelectedValue(ddl.SelectedValue);

            var dpPersonEnd = ((Control)source).Parent.FindControl("dpPersonEnd") as DatePicker;
            bool isGreaterThanMilestone = dpPersonEnd.DateValue <= (HostingPage.IsSaveAllClicked ? HostingPage.dtpPeriodToObject.DateValue : Milestone.ProjectedDeliveryDate);

            if (HostingPage.IsSaveAllClicked && HostingPage.dtpPeriodToObject.DateValue.Date < HostingPage.Milestone.EndDate.Date)
            {
                isGreaterThanMilestone = true;
            }

            args.IsValid = isGreaterThanMilestone;
        }

        protected void custPerson_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            CustomValidator custPerson = sender as CustomValidator;
            GridViewRow gvRow = custPerson.NamingContainer as GridViewRow;
            var ddl = gvRow.FindControl("ddlPersonName") as DropDownList;
            var dpPersonStart = ((Control)sender).Parent.FindControl("dpPersonStart") as DatePicker;
            var dpPersonEnd = ((Control)sender).Parent.FindControl("dpPersonEnd") as DatePicker;
            Person person = GetPersonBySelectedValue(ddl.SelectedValue);
            Dictionary<string, List<DateTime>> PersonDatesViolations = IsRangeInThePersonEmpHistory(person, dpPersonStart.DateValue.Date, dpPersonEnd.DateValue.Date);
            args.IsValid = !(PersonDatesViolations.Count > 0);
            if (PersonDatesViolations != null && PersonDatesViolations.Keys.Any())
            {
                string firstKey = PersonDatesViolations.Keys.First();
                switch (firstKey)
                {
                    case "HireDate": custPerson.ToolTip = string.Format(HireDateEmployementError, person.LastName + ", " + person.FirstName, PersonDatesViolations[firstKey][0].ToString(Constants.Formatting.EntryDateFormat)); break;
                    case "TerminationDate": custPerson.ToolTip = string.Format(TerminationDateEmployementError, person.LastName + ", " + person.FirstName, PersonDatesViolations[firstKey][0].ToString(Constants.Formatting.EntryDateFormat)); break;
                    case "Both": custPerson.ToolTip = string.Format(BothDatesEmployementError, person.LastName + ", " + person.FirstName, PersonDatesViolations[firstKey][0].ToString(Constants.Formatting.EntryDateFormat), PersonDatesViolations[firstKey][1].ToString(Constants.Formatting.EntryDateFormat)); break;
                    case "TotalOut": custPerson.ToolTip = string.Format(TotalOutOfEmploymentError); break;
                }
            }
        }

        public Dictionary<string, List<DateTime>> IsRangeInThePersonEmpHistory(Person person, DateTime startDate, DateTime endDate)
        {
            Dictionary<string, List<DateTime>> dateValidation = new Dictionary<string, List<DateTime>>();
            if (person != null && startDate != null && endDate != null)
            {
                if (!person.IsStrawMan && person.EmploymentHistory != null)
                {
                    if (person.EmploymentHistory.Any(p => p.HireDate <= startDate && (!p.TerminationDate.HasValue || (p.TerminationDate.HasValue && endDate <= p.TerminationDate))))
                    {
                    }
                    else if (person.EmploymentHistory.Any(p => p.HireDate <= endDate && (!p.TerminationDate.HasValue || (p.TerminationDate.HasValue && startDate <= p.TerminationDate))))
                    {
                        List<Employment> employments = person.EmploymentHistory.Where(p => p.HireDate <= endDate && (!p.TerminationDate.HasValue || (p.TerminationDate.HasValue && startDate <= p.TerminationDate.Value))).ToList();
                        foreach (Employment emp in employments)
                        {
                            if (startDate < emp.HireDate && emp.TerminationDate.HasValue && endDate > emp.TerminationDate.Value && !dateValidation.Keys.Any(p => p == "Both"))
                                dateValidation.Add("Both", new List<DateTime>() { emp.HireDate, emp.TerminationDate.Value });
                            else if (startDate < emp.HireDate && !dateValidation.Keys.Any(p => p == "HireDate"))
                                dateValidation.Add("HireDate", new List<DateTime>() { emp.HireDate });
                            else if (emp.TerminationDate.HasValue && endDate > emp.TerminationDate.Value && !dateValidation.Keys.Any(p => p == "TerminationDate"))
                                dateValidation.Add("TerminationDate", new List<DateTime>() { emp.TerminationDate.Value });
                        }
                    }
                    else
                    {
                        dateValidation.Add("TotalOut", null);
                    }
                }
            }
            return dateValidation;
        }

        protected void reqHourlyRevenue_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = !Milestone.IsHourlyAmount || !string.IsNullOrEmpty(e.Value);
        }

        protected void cvMaxRows_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            CustomValidator custPerson = sender as CustomValidator;
            GridViewRow gvRow = custPerson.NamingContainer as GridViewRow;
            var ddlPersonName = gvRow.FindControl("ddlPersonName") as DropDownList;

            if (ddlPersonName.SelectedItem.Attributes[Constants.Variables.IsStrawMan].ToLowerInvariant() == "true")
            {
                e.IsValid = true;
            }
            else
            {
                if (HostingPage.IsSaveAllClicked)
                {

                    string ddlPersonSelectedValue = "", ddlRoleSelectedValue = "";
                    var count = 0;

                    bool isStrawMan = false;

                    if (MilestonePersonsEntries[gvRow.DataItemIndex].IsShowPlusButton)
                    {
                        ddlPersonSelectedValue = ddlPersonName.SelectedValue;
                        ddlRoleSelectedValue = (gvRow.FindControl("ddlRole") as DropDownList).SelectedValue;
                    }
                    else
                    {
                        var index = MilestonePersonsEntries.FindIndex(mpe => mpe.Id == MilestonePersonsEntries[gvRow.DataItemIndex].ShowingPlusButtonEntryId);

                        ddlPersonSelectedValue = MilestonePersonsEntries[index].IsEditMode ? (gvMilestonePersonEntries.Rows[index].FindControl("ddlPersonName") as DropDownList).SelectedValue : MilestonePersonsEntries[index].ThisPerson.Id.ToString();
                        ddlRoleSelectedValue = MilestonePersonsEntries[index].IsEditMode ? (gvMilestonePersonEntries.Rows[index].FindControl("ddlRole") as DropDownList).SelectedValue : (MilestonePersonsEntries[index].Role != null ? MilestonePersonsEntries[index].Role.Id.ToString() : string.Empty);

                        isStrawMan = MilestonePersonsEntries[index].IsEditMode ? (gvMilestonePersonEntries.Rows[index].FindControl("ddlPersonName") as DropDownList).SelectedItem.Attributes[Constants.Variables.IsStrawMan].ToLowerInvariant() == "true" : MilestonePersonsEntries[index].ThisPerson.IsStrawMan;
                    }

                    if (!isStrawMan)
                    {

                        var hdnPersonId = gvRow.FindControl("hdnPersonId") as HiddenField;
                        var lblRole = gvRow.FindControl("lblRole") as Label;
                        var oldRoleId = lblRole.Attributes["RoleId"];


                        for (int i = 0; i < MilestonePersonsEntries.Count(); i++)
                        {
                            var entry = MilestonePersonsEntries[i];
                            if (entry.IsShowPlusButton)
                            {
                                string personId = "", roleId = "";
                                if (entry.IsEditMode)
                                {
                                    personId = (gvMilestonePersonEntries.Rows[i].FindControl("ddlPersonName") as DropDownList).SelectedValue;
                                    roleId = (gvMilestonePersonEntries.Rows[i].FindControl("ddlRole") as DropDownList).SelectedValue;

                                }
                                else
                                {
                                    personId = entry.ThisPerson.Id.ToString();
                                    roleId = entry.Role != null ? entry.Role.Id.ToString() : string.Empty;
                                }

                                if (personId == ddlPersonSelectedValue && roleId == ddlRoleSelectedValue)
                                {
                                    count = count + entry.ExtendedResourcesRowCount;
                                }

                            }
                        }

                        var newEntriesCount = repeaterOldValues.Where(entry => entry["ddlPerson"].ToLowerInvariant() == ddlPersonSelectedValue.ToLowerInvariant() && entry["ddlRole"].ToLowerInvariant() == ddlRoleSelectedValue.ToLowerInvariant()).Count();

                        count = count + newEntriesCount;

                        if (count > 5)
                        {
                            e.IsValid = false;
                        }

                    }
                }
                else
                {
                    var ddlPerson = gvRow.FindControl("ddlPersonName") as DropDownList;
                    var ddlRole = gvRow.FindControl("ddlRole") as DropDownList;
                    var hdnPersonId = gvRow.FindControl("hdnPersonId") as HiddenField;
                    var lblRole = gvRow.FindControl("lblRole") as Label;
                    var oldRoleId = lblRole.Attributes["RoleId"];

                    var personId = ddlPerson.SelectedValue;
                    var roleId = ddlRole.SelectedValue;

                    var entry = MilestonePersonsEntries[gvRow.DataItemIndex];

                    List<int> similarEntryIndexes = new List<int>();

                    if (personId != hdnPersonId.Value || roleId != oldRoleId)
                    {
                        similarEntryIndexes = new List<int>();

                        var hdnPersonIsStrawMan = gvRow.FindControl("hdnPersonIsStrawMan") as HiddenField;

                        if (hdnPersonIsStrawMan.Value.ToLowerInvariant() == "false")
                        {
                            //Find Similar Entries Indexes
                            for (int j = 0; j < MilestonePersonsEntries.Count; j++)
                            {
                                var mpe = MilestonePersonsEntries[j];

                                if (mpe.IsNewEntry == false && mpe.ThisPerson.Id.Value.ToString() == hdnPersonId.Value
                                    && ((mpe.Role != null) ? mpe.Role.Id.ToString() : string.Empty) == oldRoleId)
                                {
                                    similarEntryIndexes.Add(j);
                                }
                            }
                        }

                    }

                    if (!similarEntryIndexes.Any(indexValue => indexValue == gvRow.DataItemIndex))
                    {
                        similarEntryIndexes.Add(gvRow.DataItemIndex);
                    }


                    List<MilestonePersonEntry> entries = MilestonePersonsEntries.Where(
                                                            (mpe, index) => similarEntryIndexes.Any(i => index != i) && mpe.IsNewEntry == false &&
                                                                            mpe.ThisPerson.Id.Value.ToString().ToLowerInvariant() == personId.ToLowerInvariant()
                                                                            && (mpe.Role != null ? mpe.Role.Id.ToString().ToLowerInvariant() : string.Empty) == roleId.ToLowerInvariant()
                                                                                         ).ToList();
                    var rowsCount = entries.Count() + similarEntryIndexes.Count;
                    if (rowsCount > 5)
                    {
                        e.IsValid = false;
                    }
                }
            }

        }

        protected void custPeriodOvberlapping_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            CustomValidator custPerson = sender as CustomValidator;
            GridViewRow gvRow = custPerson.NamingContainer as GridViewRow;
            var ddlPerson = gvRow.FindControl("ddlPersonName") as DropDownList;

            if (ddlPerson.SelectedItem.Attributes[Constants.Variables.IsStrawMan].ToLowerInvariant() == "true")
            {
                e.IsValid = true;
            }
            else
            {
                if (HostingPage.IsSaveAllClicked)
                {
                    var index = MilestonePersonsEntries[gvRow.DataItemIndex].IsShowPlusButton ? gvRow.DataItemIndex : MilestonePersonsEntries.FindIndex(mpe => mpe.Id == MilestonePersonsEntries[gvRow.DataItemIndex].ShowingPlusButtonEntryId);

                    var isStrawMan = MilestonePersonsEntries[index].IsEditMode ? (gvMilestonePersonEntries.Rows[index].FindControl("ddlPersonName") as DropDownList).SelectedItem.Attributes[Constants.Variables.IsStrawMan].ToLowerInvariant() : MilestonePersonsEntries[index].ThisPerson.IsStrawMan.ToString().ToLowerInvariant();

                    if (isStrawMan == "true")
                    {
                        e.IsValid = true;
                    }
                    else
                    {
                        ValidateOvelappingWhenSaveAllClicked(sender, e);
                    }
                }
                else
                {
                    ValidateOvelappingWhenSaveClicked(sender, e);
                }
            }

        }

        private void ValidateOvelappingWhenSaveClicked(object sender, ServerValidateEventArgs e)
        {
            CustomValidator custPerson = sender as CustomValidator;
            GridViewRow gvRow = custPerson.NamingContainer as GridViewRow;

            var dpPersonStart = gvRow.FindControl("dpPersonStart") as DatePicker;
            var dpPersonEnd = gvRow.FindControl("dpPersonEnd") as DatePicker;

            string ddlPersonSelectedValue = "";

            ddlPersonSelectedValue = (gvRow.FindControl("ddlPersonName") as DropDownList).SelectedValue;

            var hdnPersonId = gvRow.FindControl("hdnPersonId") as HiddenField;

            List<int> similarEntryIndexes = new List<int>();

            if (ddlPersonSelectedValue != hdnPersonId.Value)
            {
                similarEntryIndexes = new List<int>();

                var hdnPersonIsStrawMan = gvRow.FindControl("hdnPersonIsStrawMan") as HiddenField;

                if (hdnPersonIsStrawMan.Value.ToLowerInvariant() == "false")
                {
                    //Find Similar Entries Indexes
                    for (int j = 0; j < MilestonePersonsEntries.Count; j++)
                    {
                        var mpe = MilestonePersonsEntries[j];



                        if (mpe.IsNewEntry == false && mpe.ThisPerson.Id.Value.ToString() == hdnPersonId.Value)
                        {
                            similarEntryIndexes.Add(j);
                        }
                    }
                }
            }

            if (!similarEntryIndexes.Any(indexValue => indexValue == gvRow.DataItemIndex))
            {
                similarEntryIndexes.Add(gvRow.DataItemIndex);
            }


            // Validate overlapping with other entries.
            for (int i = 0; i < MilestonePersonsEntries.Count; i++)
            {

                var personId = MilestonePersonsEntries[i].ThisPerson.Id.ToString();

                if (!similarEntryIndexes.Any(indexVal => indexVal == i) && MilestonePersonsEntries[i].IsNewEntry == false && personId == ddlPersonSelectedValue)
                {
                    // Include Similar Entries Indexes rows start and end dates with other rows 

                    foreach (var index in similarEntryIndexes)
                    {
                        DateTime startDate = dpPersonStart.DateValue;
                        DateTime endDate =
                            dpPersonEnd.DateValue != DateTime.MinValue ? dpPersonEnd.DateValue : Milestone.ProjectedDeliveryDate;

                        if (index != gvRow.DataItemIndex)
                        {
                            startDate = MilestonePersonsEntries[index].StartDate;
                            endDate = MilestonePersonsEntries[index].EndDate.Value;
                        }

                        try
                        {
                            DateTime entryStartDate = MilestonePersonsEntries[i].StartDate;

                            DateTime entryEndDate = MilestonePersonsEntries[i].EndDate.HasValue ? MilestonePersonsEntries[i].EndDate.Value : Milestone.ProjectedDeliveryDate;


                            if ((startDate >= entryStartDate && startDate <= entryEndDate) ||
                                   (endDate >= entryStartDate && endDate <= entryEndDate) ||
                                   (endDate >= entryEndDate && startDate <= entryEndDate)
                               )
                            {
                                e.IsValid = false;
                                break;

                            }
                        }
                        catch
                        {

                        }
                    }

                    if (!e.IsValid)
                    {
                        break;
                    }

                }

                // compare Similar Entries Indexes rows
                if (e.IsValid && similarEntryIndexes.Count > 1 && similarEntryIndexes.Any(indexVal => indexVal == i))
                {
                    foreach (var index in similarEntryIndexes)
                    {
                        if (index != i)
                        {
                            DateTime startDate = dpPersonStart.DateValue;
                            DateTime endDate =
                                dpPersonEnd.DateValue != DateTime.MinValue ? dpPersonEnd.DateValue : Milestone.ProjectedDeliveryDate;

                            if (index != gvRow.DataItemIndex)
                            {
                                startDate = MilestonePersonsEntries[index].StartDate;
                                endDate = MilestonePersonsEntries[index].EndDate.Value;
                            }

                            try
                            {
                                DateTime entryStartDate = MilestonePersonsEntries[i].StartDate;

                                DateTime entryEndDate = MilestonePersonsEntries[i].EndDate.HasValue ? MilestonePersonsEntries[i].EndDate.Value : Milestone.ProjectedDeliveryDate;


                                if ((startDate >= entryStartDate && startDate <= entryEndDate) ||
                                       (endDate >= entryStartDate && endDate <= entryEndDate) ||
                                       (endDate >= entryEndDate && startDate <= entryEndDate)
                                   )
                                {
                                    e.IsValid = false;
                                    break;

                                }
                            }
                            catch
                            {

                            }
                        }
                    }

                }

            }
            if (e.IsValid)
            {
                int personId;
                int.TryParse(ddlPersonSelectedValue, out personId);
                e.IsValid = !(ServiceCallers.Custom.Person(p => p.CheckIfPersonEntriesOverlapps(Milestone.Id.Value, personId, dpPersonStart.DateValue, dpPersonEnd.DateValue)));
            }
        }

        private void ValidateOvelappingWhenSaveAllClicked(object sender, ServerValidateEventArgs e)
        {
            CustomValidator custPerson = sender as CustomValidator;
            GridViewRow gvRow = custPerson.NamingContainer as GridViewRow;
            GridView gv = gvRow.NamingContainer as GridView;
            var dpPersonStart = gvRow.FindControl("dpPersonStart") as DatePicker;
            var dpPersonEnd = gvRow.FindControl("dpPersonEnd") as DatePicker;

            string ddlPersonSelectedValue = "";

            if (MilestonePersonsEntries[gvRow.DataItemIndex].IsShowPlusButton)
            {
                ddlPersonSelectedValue = (gvRow.FindControl("ddlPersonName") as DropDownList).SelectedValue;
            }
            else
            {
                var index = MilestonePersonsEntries.FindIndex(mpe => mpe.Id == MilestonePersonsEntries[gvRow.DataItemIndex].ShowingPlusButtonEntryId);

                ddlPersonSelectedValue = MilestonePersonsEntries[index].IsEditMode ? (gvMilestonePersonEntries.Rows[index].FindControl("ddlPersonName") as DropDownList).SelectedValue : MilestonePersonsEntries[index].ThisPerson.Id.ToString();
            }

            var hdnPersonId = gvRow.FindControl("hdnPersonId") as HiddenField;
            List<int> similarEntryIndexes = new List<int>();

            if (ddlPersonSelectedValue != hdnPersonId.Value)
            {
                similarEntryIndexes = new List<int>();

                var hdnPersonIsStrawMan = gvRow.FindControl("hdnPersonIsStrawMan") as HiddenField;

                if (hdnPersonIsStrawMan.Value.ToLowerInvariant() == "false")
                {
                    //Find Similar Entries Indexes
                    for (int j = 0; j < MilestonePersonsEntries.Count; j++)
                    {
                        var mpe = MilestonePersonsEntries[j];

                        if (((mpe.IsNewEntry == false) || HostingPage.ValidateNewEntry) && mpe.ThisPerson.Id.Value.ToString() == hdnPersonId.Value)
                        {
                            similarEntryIndexes.Add(j);
                        }
                    }
                }

            }

            if (!similarEntryIndexes.Any(indexValue => indexValue == gvRow.DataItemIndex))
            {
                similarEntryIndexes.Add(gvRow.DataItemIndex);
            }

            // Validate overlapping with other entries.
            for (int i = 0; i < MilestonePersonsEntries.Count; i++)
            {

                var personId = (MilestonePersonsEntries[i].IsShowPlusButton || HostingPage.IsSaveAllClicked == false)
                    ?
                                (MilestonePersonsEntries[i].IsEditMode
                                ? MilestonePersonsEntries[i].EditedEntryValues["ddlPersonName"]
                                : MilestonePersonsEntries[i].ThisPerson.Id.ToString())
                    :
                                (MilestonePersonsEntries.First(ent => ent.Id == MilestonePersonsEntries[i].ShowingPlusButtonEntryId).IsEditMode
                                ? MilestonePersonsEntries.First(ent => ent.Id == MilestonePersonsEntries[i].ShowingPlusButtonEntryId).EditedEntryValues["ddlPersonName"]
                                : MilestonePersonsEntries.First(ent => ent.Id == MilestonePersonsEntries[i].ShowingPlusButtonEntryId).ThisPerson.Id.ToString())
                                ;
                // Exclude Comparing with Similar Entries Indexes rows
                if (!similarEntryIndexes.Any(indexVal => indexVal == i) && ((MilestonePersonsEntries[i].IsNewEntry == false)
                    || (HostingPage.ValidateNewEntry)) && personId == ddlPersonSelectedValue)
                {
                    // Include Similar Entries Indexes rows start and end dates with other rows 

                    foreach (var index in similarEntryIndexes)
                    {
                        DateTime startDate = dpPersonStart.DateValue;
                        DateTime endDate =
                            dpPersonEnd.DateValue != DateTime.MinValue ? dpPersonEnd.DateValue : Milestone.ProjectedDeliveryDate;
                        if (index != gvRow.DataItemIndex)
                        {
                            startDate = (MilestonePersonsEntries[index].IsEditMode && HostingPage.IsSaveAllClicked)
                                                ? Convert.ToDateTime(MilestonePersonsEntries[index].EditedEntryValues["dpPersonStart"])
                                                : MilestonePersonsEntries[index].StartDate;
                            endDate = (MilestonePersonsEntries[index].IsEditMode && HostingPage.IsSaveAllClicked) ?
                                                Convert.ToDateTime(MilestonePersonsEntries[index].EditedEntryValues["dpPersonEnd"])
                                                : MilestonePersonsEntries[index].EndDate.Value;
                        }

                        try
                        {
                            DateTime entryStartDate =
                                                (MilestonePersonsEntries[i].IsEditMode && HostingPage.IsSaveAllClicked)
                                                ? Convert.ToDateTime(MilestonePersonsEntries[i].EditedEntryValues["dpPersonStart"])
                                                : MilestonePersonsEntries[i].StartDate;

                            DateTime entryEndDate =
                                                (MilestonePersonsEntries[i].IsEditMode && HostingPage.IsSaveAllClicked) ?
                                                Convert.ToDateTime(MilestonePersonsEntries[i].EditedEntryValues["dpPersonEnd"])
                                                : MilestonePersonsEntries[i].EndDate.HasValue ? MilestonePersonsEntries[i].EndDate.Value : Milestone.ProjectedDeliveryDate;


                            if ((startDate >= entryStartDate && startDate <= entryEndDate) ||
                                   (endDate >= entryStartDate && endDate <= entryEndDate) ||
                                   (endDate >= entryEndDate && startDate <= entryEndDate)
                               )
                            {
                                e.IsValid = false;
                                break;

                            }
                        }
                        catch
                        {

                        }
                    }

                    if (!e.IsValid)
                    {
                        break;
                    }

                }

                if (e.IsValid && similarEntryIndexes.Count > 1 && HostingPage.IsSaveAllClicked && similarEntryIndexes.Any(indexVal => indexVal == i))
                {
                    foreach (var index in similarEntryIndexes)
                    {
                        if (index != i)
                        {
                            DateTime startDate = dpPersonStart.DateValue;
                            DateTime endDate =
                                dpPersonEnd.DateValue != DateTime.MinValue ? dpPersonEnd.DateValue : Milestone.ProjectedDeliveryDate;

                            if (index != gvRow.DataItemIndex)
                            {
                                startDate = (MilestonePersonsEntries[index].IsEditMode && HostingPage.IsSaveAllClicked)
                                                    ? Convert.ToDateTime(MilestonePersonsEntries[index].EditedEntryValues["dpPersonStart"])
                                                    : MilestonePersonsEntries[index].StartDate;
                                endDate = (MilestonePersonsEntries[index].IsEditMode && HostingPage.IsSaveAllClicked) ?
                                                    Convert.ToDateTime(MilestonePersonsEntries[index].EditedEntryValues["dpPersonEnd"])
                                                    : MilestonePersonsEntries[index].EndDate.Value;
                            }

                            try
                            {
                                DateTime entryStartDate =
                                                    (MilestonePersonsEntries[i].IsEditMode && HostingPage.IsSaveAllClicked)
                                                    ? Convert.ToDateTime(MilestonePersonsEntries[i].EditedEntryValues["dpPersonStart"])
                                                    : MilestonePersonsEntries[i].StartDate;

                                DateTime entryEndDate =
                                                    (MilestonePersonsEntries[i].IsEditMode && HostingPage.IsSaveAllClicked) ?
                                                    Convert.ToDateTime(MilestonePersonsEntries[i].EditedEntryValues["dpPersonEnd"])
                                                    : MilestonePersonsEntries[i].EndDate.HasValue ? MilestonePersonsEntries[i].EndDate.Value : Milestone.ProjectedDeliveryDate;


                                if ((startDate >= entryStartDate && startDate <= entryEndDate) ||
                                       (endDate >= entryStartDate && endDate <= entryEndDate) ||
                                       (endDate >= entryEndDate && startDate <= entryEndDate)
                                   )
                                {
                                    e.IsValid = false;
                                    break;
                                }
                            }
                            catch
                            {

                            }
                        }
                    }

                }
            }
            if (!e.IsValid)
                return;
            for (int j = 0; j < MilestonePersonsEntries.Count; j++)
            {
                if (MilestonePersonsEntries[j].IsEditMode)
                {
                    var gvdpPersonStart = gv.Rows[j].FindControl("dpPersonStart") as DatePicker;
                    var gvdpPersonEnd = gv.Rows[j].FindControl("dpPersonEnd") as DatePicker;
                    var gvddlPersonName = gv.Rows[j].FindControl("ddlPersonName") as CustomDropDown;
                    for (int i = j + 1; i < MilestonePersonsEntries.Count; i++)
                    {
                        var gvNextdpPersonStart = gv.Rows[i].FindControl("dpPersonStart") as DatePicker;
                        var gvNextdpPersonEnd = gv.Rows[i].FindControl("dpPersonEnd") as DatePicker;
                        var gvNextddlPersonName = gv.Rows[i].FindControl("ddlPersonName") as CustomDropDown;

                        if (gvddlPersonName.SelectedValue == gvNextddlPersonName.SelectedValue && gvdpPersonStart.DateValue <= gvNextdpPersonEnd.DateValue && gvNextdpPersonStart.DateValue <= gvdpPersonEnd.DateValue)
                        {
                            e.IsValid = false;
                            return;
                        }
                    }
                }
            }

            for (int j = 0; j < MilestonePersonsEntries.Count; j++)
            {
                if (MilestonePersonsEntries[j].IsEditMode)
                {
                    int personId;
                    int.TryParse(ddlPersonSelectedValue, out personId);
                    var gvdpPersonStart = gv.Rows[j].FindControl("dpPersonStart") as DatePicker;
                    var gvdpPersonEnd = gv.Rows[j].FindControl("dpPersonEnd") as DatePicker;
                    e.IsValid = !(ServiceCallers.Custom.Person(p => p.CheckIfPersonEntriesOverlapps(Milestone.Id.Value, personId, gvdpPersonStart.DateValue, gvdpPersonEnd.DateValue)));
                    if (!e.IsValid)
                        return;
                }
            }
        }

        protected void custPeriodVacationOverlapping_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            var custPersonStartDate = ((Control)sender).Parent.Parent.FindControl("custPersonStart") as CustomValidator;
            var compPersonEnd = ((Control)sender).Parent.Parent.FindControl("compPersonEnd") as CompareValidator;

            CustomValidator custPerson = sender as CustomValidator;
            GridViewRow gvRow = custPerson.NamingContainer as GridViewRow;
            var ddlPersonName = gvRow.FindControl("ddlPersonName") as DropDownList;

            var isStartDateValid = ((System.Web.UI.WebControls.BaseValidator)(custPersonStartDate)).IsValid;
            var isEndDateValid = compPersonEnd.IsValid;

            if (isStartDateValid && isEndDateValid)
            {
                var dpPersonStart = ((Control)sender).Parent.FindControl("dpPersonStart") as DatePicker;
                var dpPersonEnd = ((Control)sender).Parent.FindControl("dpPersonEnd") as DatePicker;

                DateTime startDate = dpPersonStart.DateValue;
                DateTime endDate =
                    dpPersonEnd.DateValue != DateTime.MinValue ? dpPersonEnd.DateValue : Milestone.ProjectedDeliveryDate;

                // Validate overlapping with other entries.
                PersonWorkingHoursDetailsWithinThePeriod personWorkingHoursDetailsWithinThePeriod = GetPersonWorkingHoursDetailsWithinThePeriod(startDate, endDate, ddlPersonName);
                if (personWorkingHoursDetailsWithinThePeriod.TotalWorkHoursExcludingVacationHours == 0)
                {
                    e.IsValid = false;
                }
            }
        }

        protected void cvHoursInPeriod_ServerValidate(object source, ServerValidateEventArgs e)
        {
            var reqPersonEnd = ((Control)source).Parent.FindControl("reqPersonEnd") as RequiredFieldValidator;
            var reqPersonStart = ((Control)source).Parent.FindControl("reqPersonStart") as RequiredFieldValidator;

            if (reqPersonEnd.IsValid && reqPersonStart.IsValid)
            {
                var txtHoursInPeriod = ((Control)source).Parent.FindControl("txtHoursInPeriod") as TextBox;
                var value = txtHoursInPeriod.Text.Trim();

                if (!string.IsNullOrEmpty(value))
                {
                    decimal Totalhours;
                    if (decimal.TryParse(value, out Totalhours) && Totalhours > 0M)
                    {
                        var dpPersonStart = ((Control)source).Parent.FindControl("dpPersonStart") as DatePicker;
                        var dpPersonEnd = ((Control)source).Parent.FindControl("dpPersonEnd") as DatePicker;
                        CustomValidator custPerson = source as CustomValidator;
                        GridViewRow gvRow = custPerson.NamingContainer as GridViewRow;
                        var ddl = gvRow.FindControl("ddlPersonName") as DropDownList;
                        PersonWorkingHoursDetailsWithinThePeriod personWorkingHoursDetailsWithinThePeriod = GetPersonWorkingHoursDetailsWithinThePeriod(dpPersonStart.DateValue, dpPersonEnd.DateValue, ddl);

                        // calculate hours per day according to HoursInPerod 
                        var hoursPerDay = (personWorkingHoursDetailsWithinThePeriod.TotalWorkDaysIncludingVacationDays != 0) ? decimal.Round(Totalhours / (personWorkingHoursDetailsWithinThePeriod.TotalWorkDaysIncludingVacationDays), 2) : 0;

                        e.IsValid = hoursPerDay > 0M;
                    }
                }
            }
        }

        #endregion

        public void ddlPersonName_Changed(object sender, EventArgs e)
        {
            DropDownList ddlPerson = sender as DropDownList;
            GridViewRow gvRow = ddlPerson.NamingContainer as GridViewRow;
            var dpPersonStart = gvRow.FindControl("dpPersonStart") as DatePicker;
            var dpPersonEnd = gvRow.FindControl("dpPersonEnd") as DatePicker;
            Person person = GetPersonBySelectedValue(ddlPerson.SelectedValue);
            if (person != null && !person.IsStrawMan && person.EmploymentHistory != null)
            {
                if (person.EmploymentHistory.Any(p => p.HireDate <= Milestone.ProjectedDeliveryDate && (!p.TerminationDate.HasValue || (p.TerminationDate.HasValue && Milestone.StartDate <= p.TerminationDate))))
                {
                    Employment employment = person.EmploymentHistory.First(p => p.HireDate <= Milestone.ProjectedDeliveryDate && (!p.TerminationDate.HasValue || (p.TerminationDate.HasValue && Milestone.StartDate <= p.TerminationDate)));
                    dpPersonStart.TextValue = Milestone.StartDate < employment.HireDate.Date ? employment.HireDate.Date.ToString(Constants.Formatting.EntryDateFormat) : Milestone.StartDate.ToString(Constants.Formatting.EntryDateFormat);
                    dpPersonEnd.TextValue = employment.TerminationDate.HasValue ? Milestone.ProjectedDeliveryDate < employment.TerminationDate.Value ? Milestone.ProjectedDeliveryDate.ToString(Constants.Formatting.EntryDateFormat) : employment.TerminationDate.Value.ToString(Constants.Formatting.EntryDateFormat) : Milestone.ProjectedDeliveryDate.ToString(Constants.Formatting.EntryDateFormat);
                }
            }
            if (person != null && person.IsStrawMan)
            {
                dpPersonStart.TextValue = Milestone.StartDate.ToString(Constants.Formatting.EntryDateFormat);
                dpPersonEnd.TextValue = Milestone.EndDate.ToString(Constants.Formatting.EntryDateFormat);
            }
        }

        public Person GetPersonBySelectedValue(String id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (ViewState[PERSON_ID_KEY] != null && (ViewState[PERSON_ID_KEY] as Person) != null && (ViewState[PERSON_ID_KEY] as Person).Id.ToString() == id)
                {
                    return (ViewState[PERSON_ID_KEY] as Person);
                }

                using (var serviceCLient = new PersonServiceClient())
                {
                    try
                    {
                        var person = serviceCLient.GetPersonById(int.Parse(id));
                        person.EmploymentHistory = serviceCLient.GetPersonEmploymentHistoryById(person.Id.Value).ToList();
                        ViewState[PERSON_ID_KEY] = person;
                        return person;
                    }
                    catch (CommunicationException)
                    {
                        serviceCLient.Abort();
                        throw;
                    }
                }
            }
            return null;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            thInsertMilestonePerson.Visible = (MilestonePersonsEntries == null || MilestonePersonsEntries.Count == 0);
        }

        protected override void OnInit(EventArgs e)
        {
            _seniorityAnalyzer = new SeniorityAnalyzer(DataHelper.CurrentPerson);

            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            IsSaveCommit = true;
            ISDatBindRows = true;
            lblResultMessage.ClearMessage();
            if (!IsPostBack)
            {

                GetLatestData();
            }
            StoreRepeterEntriesInObject();
            StoreGridViewEditedEntriesInObject();
        }

        public void GetLatestData()
        {
            int? id = HostingPage.MilestoneId;
            if (id.HasValue)
            {
                if (repeaterOldValues != null)
                {
                    repeaterOldValues.Clear();
                    repeaterOldValues = repeaterOldValues;

                }

                if (AddMilestonePersonEntries != null)
                {
                    AddMilestonePersonEntries.Clear();
                    AddMilestonePersonEntries = AddMilestonePersonEntries;
                    repPerson.DataSource = AddMilestonePersonEntries;
                    repPerson.DataBind();
                }
                lblResultMessage.ClearMessage();
                List<MilestonePerson> milestonePersons = GetMilestonePersons();
                FillMilestonePersonEntries(milestonePersons);
            }
        }

        private List<MilestonePersonEntry> GetSortedEntries(List<MilestonePerson> milestonePersons)
        {
            var entries = new List<MilestonePersonEntry>();

            foreach (var milestonePerson in milestonePersons)
            {
                if (milestonePerson != null)
                {
                    if (!Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName))
                    {
                        if (IsUserHasPermissionOnProject.HasValue && !IsUserHasPermissionOnProject.Value
                            && IsUserisOwnerOfProject.HasValue && !IsUserisOwnerOfProject.Value)
                            Response.Redirect(@"~\GuestPages\AccessDenied.aspx");
                    }

                    _seniorityAnalyzer.IsOtherGreater(milestonePerson.Person);

                    if (milestonePerson.Entries != null)
                        entries.AddRange(milestonePerson.Entries);
                }
            }

            return GetSortedEntries(entries);

        }

        private List<MilestonePersonEntry> GetSortedEntries(List<MilestonePersonEntry> mpentries)
        {
            var entries = mpentries.OrderBy(entry => entry.ThisPerson.LastName).ThenBy(en => en.ThisPerson.FirstName).ThenBy(en => en.Role != null ? en.Role.Id : 0).ThenBy(ent => !ent.IsNewEntry ? 0 : 1).ThenBy(ent => ent.StartDate).AsQueryable().ToList();

            entries = GetFormattedEntries(entries);

            return entries;
        }

        private List<MilestonePersonEntry> GetFormattedEntries(List<MilestonePersonEntry> entries)
        {

            foreach (var entry in entries)
            {

                entry.IsShowPlusButton = false;
                entry.ExtendedResourcesRowCount = 0;
                entry.ShowingPlusButtonEntryId = 0;

            }

            for (int i = 0; i < entries.Count; i++)
            {
                List<MilestonePersonEntry> selectedentries = null;

                if (entries[i].Role == null)
                {
                    selectedentries = entries.Where(entr => entr.ThisPerson.Id == entries[i].ThisPerson.Id && entr.Role == entries[i].Role).ToList();
                }
                else
                {
                    selectedentries = entries.Where(entr => entr.ThisPerson.Id == entries[i].ThisPerson.Id && entr.Role != null && entries[i].Role != null && entr.Role.Id == entries[i].Role.Id).ToList();
                }

                selectedentries[0].IsShowPlusButton = true;
                selectedentries[0].ExtendedResourcesRowCount = selectedentries.Count;

                for (int j = 1; j < selectedentries.Count; j++)
                {
                    selectedentries[j].ShowingPlusButtonEntryId = selectedentries[0].Id;
                }
            }

            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];

                if (entry.ThisPerson.IsStrawMan)
                {
                    entry.IsShowPlusButton = true;
                    entry.ExtendedResourcesRowCount = 1;
                    entry.ShowingPlusButtonEntryId = 0;
                }
            }

            return entries;
        }

        private void FillMilestonePersonEntries(List<MilestonePerson> milestonePersons)
        {
            var entries = GetSortedEntries(milestonePersons);
            MilestonePersonsEntries = entries;
            PopulateControls(Milestone);
            BindEntriesGrid(entries);
        }

        private void BindEntriesGrid(List<MilestonePersonEntry> entries)
        {
            var tmp = new List<MilestonePersonEntry>(entries);

            if (tmp.Count == 0)
            {
                thInsertMilestonePerson.Visible = true;
                thHourlyRate.Visible = Milestone.IsHourlyAmount;
            }
            else
            {
                thInsertMilestonePerson.Visible = false;
            }

            gvMilestonePersonEntries.DataSource = tmp;

            gvMilestonePersonEntries.Columns[AMOUNT_COLUMN_INDEX].Visible = Milestone.IsHourlyAmount;
            gvMilestonePersonEntries.DataBind();
        }

        protected void gvMilestonePersonEntries_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var entry = (MilestonePersonEntry)e.Row.DataItem;

                var imgCopy = e.Row.FindControl("imgCopy") as ImageButton;


                var imgMilestonePersonEntryEdit = e.Row.FindControl("imgMilestonePersonEntryEdit") as ImageButton;
                var imgAdditionalAllocationOfResource = e.Row.FindControl("imgAdditionalAllocationOfResource") as ImageButton;
                var lnkPersonName = e.Row.FindControl("lnkPersonName") as HyperLink;
                var lblRole = e.Row.FindControl("lblRole") as Label;
                var lblStartDate = e.Row.FindControl("lblStartDate") as Label;
                var lblEndDate = e.Row.FindControl("lblEndDate") as Label;
                var lblHoursPerDay = e.Row.FindControl("lblHoursPerDay") as Label;
                var lblAmount = e.Row.FindControl("lblAmount") as Label;
                var lableTargetMargin = e.Row.FindControl(lblTargetMargin) as Label;
                var lblHoursInPeriodDay = e.Row.FindControl("lblHoursInPeriodDay") as Label;
                var imgMilestonePersonDelete = e.Row.FindControl("imgMilestonePersonDelete") as ImageButton;
                var lbVacationHoursToolTip = e.Row.FindControl("lbVacationHoursToolTip") as Label;
                var tblHoursInPeriod1 = e.Row.FindControl("tblHoursInPeriod1") as HtmlTable;

                imgMilestonePersonEntryEdit.Visible = imgAdditionalAllocationOfResource.Visible = lnkPersonName.Visible =
                lblRole.Visible = lblStartDate.Visible = lblEndDate.Visible = lblHoursPerDay.Visible =
                lblAmount.Visible = lableTargetMargin.Visible = lblHoursInPeriodDay.Visible = imgMilestonePersonDelete.Visible =
                tblHoursInPeriod1.Visible = lbVacationHoursToolTip.Visible = !entry.IsEditMode;

                if (!entry.IsEditMode && entry.VacationHours > 0)
                {
                    lbVacationHoursToolTip.Attributes[OnMouseOver] = string.Format(ShowPanel, lbVacationHoursToolTip.ClientID, pnlTimeOffHoursToolTip.ClientID, lblTimeOffHours.ClientID, lblProjectAffectedHours.ClientID, entry.TimeOffHours.ToString("0.00"), entry.VacationHours.ToString("0.00"));
                    lbVacationHoursToolTip.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTimeOffHoursToolTip.ClientID);
                    lbVacationHoursToolTip.Text = "!";
                }
                else
                {
                    lbVacationHoursToolTip.Text = "&nbsp;&nbsp;";
                }

                var imgMilestonePersonEntryUpdate = e.Row.FindControl("imgMilestonePersonEntryUpdate") as ImageButton;
                var imgMilestonePersonEntryCancel = e.Row.FindControl("imgMilestonePersonEntryCancel") as ImageButton;
                var tblPersonName = e.Row.FindControl("tblPersonName") as HtmlTable;
                var ddlPersonName = e.Row.FindControl("ddlPersonName") as DropDownList;
                var ddlRole = e.Row.FindControl("ddlRole") as DropDownList;
                var tblStartDate = e.Row.FindControl("tblStartDate") as HtmlTable;
                var tblEndDate = e.Row.FindControl("tblEndDate") as HtmlTable;
                var tblHoursPerDay = e.Row.FindControl("tblHoursPerDay") as HtmlTable;
                var tblAmount = e.Row.FindControl("tblAmount") as HtmlTable;
                var tblHoursInPeriod = e.Row.FindControl("tblHoursInPeriod") as HtmlTable;
                var dpPersonStart = e.Row.FindControl("dpPersonStart") as DatePicker;
                var dpPersonEnd = e.Row.FindControl("dpPersonEnd") as DatePicker;
                var txtAmount = e.Row.FindControl("txtHoursPerDay") as TextBox;
                var txtHoursPerDay = e.Row.FindControl("txtAmount") as TextBox;
                var txtHoursInPeriod = e.Row.FindControl("txtHoursInPeriod") as TextBox;


                imgMilestonePersonEntryUpdate.Visible = imgMilestonePersonEntryCancel.Visible = tblPersonName.Visible =
                ddlPersonName.Visible = ddlRole.Visible = tblStartDate.Visible = tblEndDate.Visible =
                tblHoursPerDay.Visible = tblAmount.Visible = tblHoursInPeriod.Visible = entry.IsEditMode;



                imgAdditionalAllocationOfResource.Attributes["entriesCount"] = entry.ExtendedResourcesRowCount.ToString();
                imgMilestonePersonDelete.Attributes["IsOriginalResource"] = "false";

                if (!entry.IsShowPlusButton)
                {
                    imgAdditionalAllocationOfResource.Visible = ddlPersonName.Visible = ddlRole.Visible = lnkPersonName.Visible = lblRole.Visible = false;
                }
                else
                {
                    if (entry.ExtendedResourcesRowCount > 1)
                    {
                        imgMilestonePersonDelete.Attributes["IsOriginalResource"] = "true";
                    }
                }

                if (entry.IsEditMode)
                {
                    if (ddlPersonName != null)
                    {
                        DataHelper.FillPersonList(ddlPersonName, string.Empty, PersonsListForMilestone, string.Empty);

                        var mpeList = MilestonePersonsEntries.Where(entr => entr.MilestonePersonId == entry.MilestonePersonId).AsQueryable().ToList();

                        bool result = false;

                        if (mpeList.Any(en => en.HasTimeEntries == true))
                        {
                            result = true;
                        }

                        ddlPersonName.Enabled = !result;

                        if (entry.EditedEntryValues != null)
                        {
                            ddlPersonName.SelectedValue = entry.EditedEntryValues["ddlPersonName"];
                        }
                        else
                        {
                            ddlPersonName.SelectedValue = entry.ThisPerson.Id.Value.ToString();
                        }
                    }

                    if (ddlRole != null)
                    {
                        DataHelper.FillListDefault(ddlRole, string.Empty, RoleListForPersons, false);


                        if (entry.EditedEntryValues != null)
                        {
                            ddlRole.SelectedValue = entry.EditedEntryValues["ddlRole"];
                        }
                        else
                        {
                            ddlRole.SelectedValue = ddlRole.Items.FindByValue(entry.Role != null
                                                                                    ? entry.Role.Id.ToString()
                                                                                    : string.Empty).Value;
                        }

                    }


                    if (entry.EditedEntryValues != null)
                    {

                        if (dpPersonStart != null)
                        {
                            dpPersonStart.TextValue = entry.EditedEntryValues["dpPersonStart"];
                        }

                        if (dpPersonEnd != null)
                        {
                            dpPersonEnd.TextValue = entry.EditedEntryValues["dpPersonEnd"];
                        }

                        if (txtHoursPerDay != null)
                        {
                            txtHoursPerDay.Text = entry.EditedEntryValues["txtHoursPerDay"];
                        }

                        if (txtAmount != null)
                        {
                            txtAmount.Text = entry.EditedEntryValues["txtAmount"];
                        }

                        if (txtHoursInPeriod != null)
                        {
                            txtHoursInPeriod.Text = entry.EditedEntryValues["txtHoursInPeriod"];
                        }
                    }

                }
                else
                {

                    if (lblHoursPerDay != null)
                    {
                        lblHoursPerDay.Text = entry.HoursPerDay.ToString("0.00");
                    }

                    if (lblHoursInPeriodDay != null)
                    {
                        lblHoursInPeriodDay.Text = entry.ProjectedWorkload.ToString("0.00");
                    }

                    if (lnkPersonName != null)
                    {

                        lnkPersonName.NavigateUrl = GetMpeRedirectUrl(entry.MilestonePersonId);
                        lnkPersonName.Attributes["PersonId"] = entry.ThisPerson.Id.ToString();

                    }
                }

                var rowSa = new SeniorityAnalyzer(DataHelper.CurrentPerson);
                if (rowSa.IsOtherGreater(entry.ThisPerson))
                {
                    if (lableTargetMargin != null && !entry.IsEditMode)
                    {
                        lableTargetMargin.Text = Resources.Controls.HiddenCellText;
                    }
                }
                else if (lableTargetMargin != null && !entry.IsEditMode)
                {
                    lableTargetMargin.Text = (entry.ComputedFinancials != null) ? string.Format(Constants.Formatting.PercentageFormat, entry.ComputedFinancials.TargetMargin) : string.Format(Constants.Formatting.PercentageFormat, 0);
                }

                if (entry.ThisPerson.IsStrawMan)
                {
                    imgAdditionalAllocationOfResource.Visible = false;
                }
            }
        }

        protected void repPerson_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var repItem = e.Item;
            var bar = repItem.FindControl(mpbar) as MilestonePersonBar;

            if (gvMilestonePersonEntries.Rows.Count > 0)
            {
                if (!(gvMilestonePersonEntries.Rows.Count % 2 == 0))
                {
                    if (e.Item.ItemType == ListItemType.Item)
                    {
                        bar.BarColor = "#F9FAFF";
                    }
                    else if (e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        bar.BarColor = "White";
                    }
                }
            }

            DateTime startDate = Milestone.StartDate;
            DateTime endDate = Milestone.ProjectedDeliveryDate;

            var tdAmountInsert = bar.FindControl("tdAmountInsert") as HtmlTableCell;
            var ddlPerson = bar.FindControl(DDLPERSON_KEY) as DropDownList;
            var ddlRole = bar.FindControl(DDLROLE_KEY) as DropDownList;
            var dpPersonStart = bar.FindControl(dpPersonStartInsert) as DatePicker;
            var dpPersonEnd = bar.FindControl(dpPersonEndInsert) as DatePicker;
            var txtAmount = bar.FindControl(txtAmountInsert) as TextBox;
            var txtHoursPerDay = bar.FindControl(txtHoursPerDayInsert) as TextBox;
            var hdnIsFromAddBtn = bar.FindControl("hdnIsFromAddBtn") as HiddenField;

            if (dpPersonStart != null)
                dpPersonStart.DateValue = DateTime.Parse(repeaterOldValues[e.Item.ItemIndex][dpPersonStartInsert]);

            if (dpPersonEnd != null)
                dpPersonEnd.DateValue = DateTime.Parse(repeaterOldValues[e.Item.ItemIndex][dpPersonEndInsert]);

            if (ddlPerson != null)
            {
                DataHelper.FillPersonList(ddlPerson, string.Empty, PersonsListForMilestone, string.Empty);

                if (!string.IsNullOrEmpty(repeaterOldValues[e.Item.ItemIndex][DDLPERSON_KEY]))
                    ddlPerson.SelectedValue = repeaterOldValues[e.Item.ItemIndex][DDLPERSON_KEY];
            }

            if (ddlRole != null)
            {
                DataHelper.FillListDefault(ddlRole, string.Empty, RoleListForPersons, false);

                if (!string.IsNullOrEmpty(repeaterOldValues[e.Item.ItemIndex][DDLROLE_KEY]))
                    ddlRole.SelectedValue = repeaterOldValues[e.Item.ItemIndex][DDLROLE_KEY];
            }


            if (tdAmountInsert != null)
                tdAmountInsert.Visible = Milestone.IsHourlyAmount;

            if (tdAmountInsert.Visible && txtAmount != null)
                txtAmount.Text = repeaterOldValues[e.Item.ItemIndex][txtAmountInsert];

            if (txtHoursPerDay != null)
                txtHoursPerDay.Text = repeaterOldValues[e.Item.ItemIndex][txtHoursPerDayInsert];

            if (hdnIsFromAddBtn != null)
                hdnIsFromAddBtn.Value = repeaterOldValues[e.Item.ItemIndex]["hdnIsFromAddBtn"];
            if (hdnIsFromAddBtn.Value == true.ToString())
            {
                bar.ddlPersonName_Changed(ddlPerson, new EventArgs());
            }
        }

        public List<MilestonePerson> GetMilestonePersons()
        {
            using (var serviceClient = new MilestonePersonServiceClient())
            {
                try
                {
                    var mpersons = serviceClient.GetMilestonePersonsDetailsByMileStoneId(HostingPage.MilestoneId.Value).AsQueryable().ToList();
                    return mpersons;
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        private void PopulateControls(Milestone milestone)
        {
            // Security
            bool isReadOnly =
                !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName) &&
                !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SalespersonRoleName) &&
                !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.PracticeManagerRoleName) &&
                !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.BusinessUnitManagerRoleName) &&
                !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.ProjectLead) &&//added Project Lead as per #2941.
                !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName) &&// #2817: DirectorRoleName is added as per the requirement.
                !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SeniorLeadershipRoleName); // #2913: userIsSeniorLeadership is added as per the requirement.

            btnAddPerson.Visible = gvMilestonePersonEntries.Columns[0].Visible = !isReadOnly;
        }

        protected string GetMpeRedirectUrl(object milestonePersonId)
        {
            var mpePageUrl =
               string.Format(
                      Constants.ApplicationPages.RedirectMilestonePersonIdFormat,
                      Constants.ApplicationPages.MilestonePersonDetail,
                      Milestone.Id.Value,
                      milestonePersonId);

            var url = Request.Url.AbsoluteUri;

            if (!HostingPage.SelectedId.HasValue)
            {

                url = url.Replace("?", "?id=" + HostingPage.MilestoneId.Value.ToString() + "&");
            }

            return PraticeManagement.Utils.Generic.GetTargetUrlWithReturn(mpePageUrl, url);
        }

        protected void btnAddPerson_Click(object sender, EventArgs e)
        {
            lblResultMessage.ClearMessage();
            AddRowAndBindRepeater(null, true);
        }

        private void StoreRepeterEntriesInObject()
        {
            var repoldValues = new List<Dictionary<string, string>>();

            foreach (RepeaterItem repItem in repPerson.Items)
            {
                var bar = repItem.FindControl(mpbar) as MilestonePersonBar;

                var ddlPerson = bar.FindControl(DDLPERSON_KEY) as DropDownList;
                var ddlRole = bar.FindControl(DDLROLE_KEY) as DropDownList;
                var dpPersonStart = bar.FindControl(dpPersonStartInsert) as DatePicker;
                var dpPersonEnd = bar.FindControl(dpPersonEndInsert) as DatePicker;
                var txtAmount = bar.FindControl(txtAmountInsert) as TextBox;
                var txtHoursPerDay = bar.FindControl(txtHoursPerDayInsert) as TextBox;
                var txtHoursInPeriod = bar.FindControl(txtHoursInPeriodInsert) as TextBox;
                var hdnIsFromAddBtn = bar.FindControl("hdnIsFromAddBtn") as HiddenField;

                var dic = new Dictionary<string, string>();
                dic.Add(DDLPERSON_KEY, ddlPerson.SelectedValue);
                dic.Add(DDLROLE_KEY, ddlRole.SelectedValue);
                dic.Add(dpPersonStartInsert, dpPersonStart.DateValue.ToString());
                dic.Add(dpPersonEndInsert, dpPersonEnd.DateValue.ToString());
                dic.Add(txtAmountInsert, txtAmount.Text);
                dic.Add(txtHoursPerDayInsert, txtHoursPerDay.Text);
                dic.Add(txtHoursInPeriodInsert, txtHoursInPeriod.Text);
                dic.Add("hdnIsFromAddBtn", hdnIsFromAddBtn.Value.ToString());
                repoldValues.Add(dic);
            }

            repeaterOldValues = repoldValues;
        }

        private void StoreGridViewEditedEntriesInObject(int? mpeIndex = null, List<MilestonePersonEntry> mpentries = null)
        {
            var milestonePersonEntries = (mpentries == null) ? MilestonePersonsEntries : mpentries;

            if (milestonePersonEntries != null)
            {
                for (int i = 0; i < milestonePersonEntries.Count; i++)
                {
                    if (mpeIndex != null && mpeIndex.Value == i)
                    {
                        var mpe = milestonePersonEntries[i];
                        var dic = new Dictionary<string, string>();
                        dic.Add("ddlPersonName", mpe.ThisPerson.Id.ToString());
                        dic.Add("ddlRole", mpe.Role != null ? mpe.Role.Id.ToString() : string.Empty);
                        dic.Add("dpPersonStart", mpe.StartDate.ToString("MM/dd/yyyy"));
                        dic.Add("dpPersonEnd", mpe.EndDate != null ? mpe.EndDate.Value.ToString("MM/dd/yyyy") : string.Empty);
                        dic.Add("txtHoursPerDay", string.Empty);
                        dic.Add("txtAmount", string.Empty);
                        dic.Add("txtHoursInPeriod", string.Empty);

                        milestonePersonEntries[i].EditedEntryValues = dic;
                    }
                    else if (milestonePersonEntries[i].IsEditMode)
                    {
                        var gvRow = gvMilestonePersonEntries.Rows[i];

                        var ddlPerson = gvRow.FindControl("ddlPersonName") as DropDownList;
                        var ddlRole = gvRow.FindControl("ddlRole") as DropDownList;
                        var dpPersonStart = gvRow.FindControl("dpPersonStart") as DatePicker;
                        var dpPersonEnd = gvRow.FindControl("dpPersonEnd") as DatePicker;
                        var txtAmount = gvRow.FindControl("txtHoursPerDay") as TextBox;
                        var txtHoursPerDay = gvRow.FindControl("txtAmount") as TextBox;
                        var txtHoursInPeriod = gvRow.FindControl("txtHoursInPeriod") as TextBox;

                        var dic = new Dictionary<string, string>();
                        dic.Add("ddlPersonName", ddlPerson.SelectedValue);
                        dic.Add("ddlRole", ddlRole.SelectedValue);
                        dic.Add("dpPersonStart", dpPersonStart.TextValue);
                        dic.Add("dpPersonEnd", dpPersonEnd.TextValue);
                        dic.Add("txtAmount", txtAmount.Text);
                        dic.Add("txtHoursPerDay", txtHoursPerDay.Text);
                        dic.Add("txtHoursInPeriod", txtHoursInPeriod.Text);
                        milestonePersonEntries[i].EditedEntryValues = dic;

                    }
                    else
                    {
                        milestonePersonEntries[i].EditedEntryValues = null;
                    }


                }
            }

        }

        internal void AddRowAndBindRepeater(Dictionary<string, string> dic, bool isFromAddPersonButton)
        {
            if (AddMilestonePersonEntries == null)
            {
                AddMilestonePersonEntries = new List<MilestonePersonEntry>();
            }

            if (repeaterOldValues == null)
            {
                repeaterOldValues = new List<Dictionary<string, string>>();
            }

            var entry = new MilestonePersonEntry() { };
            entry.IsAddButtonEntry = isFromAddPersonButton ? true : false;
            entry.StartDate = Milestone.StartDate;
            entry.EndDate = Milestone.ProjectedDeliveryDate;

            if (dic == null)
            {
                dic = new Dictionary<string, string>();
                dic.Add(DDLPERSON_KEY, string.Empty);
                dic.Add(DDLROLE_KEY, string.Empty);
                dic.Add(dpPersonStartInsert, Milestone.StartDate.ToString());
                dic.Add(dpPersonEndInsert, Milestone.ProjectedDeliveryDate.ToString());
                dic.Add(txtAmountInsert, string.Empty);
                dic.Add(txtHoursPerDayInsert, string.Empty);
                dic.Add(txtHoursInPeriodInsert, string.Empty);
                dic.Add("hdnIsFromAddBtn", entry.IsAddButtonEntry.ToString());
            }

            repeaterOldValues.Add(dic);
            repeaterOldValues = repeaterOldValues;

            AddMilestonePersonEntries.Add(entry);

            AddMilestonePersonEntries = AddMilestonePersonEntries;
            repPerson.DataSource = AddMilestonePersonEntries;
            repPerson.DataBind();
        }

        public void RemoveItemAndDaabindRepeater(int barIndex)
        {
            AddMilestonePersonEntries.RemoveAt(barIndex);
            repeaterOldValues.RemoveAt(barIndex);
            AddMilestonePersonEntries = AddMilestonePersonEntries;
            repPerson.DataSource = AddMilestonePersonEntries;
            repPerson.DataBind();
        }

        public void AddAndBindRow(RepeaterItem repItem, bool isSaveCommit, bool iSDatBindRows = true)
        {
            var bar = repItem.FindControl(mpbar) as MilestonePersonBar;
            var ddlPerson = bar.FindControl(DDLPERSON_KEY) as DropDownList;
            var ddlRole = bar.FindControl(DDLROLE_KEY) as DropDownList;
            var dpPersonStart = bar.FindControl(dpPersonStartInsert) as DatePicker;
            var dpPersonEnd = bar.FindControl(dpPersonEndInsert) as DatePicker;
            var txtAmount = bar.FindControl(txtAmountInsert) as TextBox;
            var txtHoursPerDay = bar.FindControl(txtHoursPerDayInsert) as TextBox;
            var txtHoursInPeriod = bar.FindControl(txtHoursInPeriodInsert) as TextBox;
            var hdnIsFromAddBtn = bar.FindControl("hdnIsFromAddBtn") as HiddenField;

            var entry = new MilestonePersonEntry();
            entry.StartDate = dpPersonStart.DateValue;
            entry.EndDate = dpPersonEnd.DateValue != DateTime.MinValue ? (DateTime?)dpPersonEnd.DateValue : null;
            Person person = null;

            if (!string.IsNullOrEmpty(hdnIsFromAddBtn.Value))
            {
                entry.IsAddButtonEntry = hdnIsFromAddBtn.Value == true.ToString();
            }
            if (!string.IsNullOrEmpty(ddlPerson.SelectedValue))
            {
                person = GetPersonBySelectedValue(ddlPerson.SelectedValue);

                entry.ThisPerson = person;
            }

            // Role
            if (!string.IsNullOrEmpty(ddlRole.SelectedValue))
            {
                entry.Role =
                    new PersonRole
                    {
                        Id = int.Parse(ddlRole.SelectedValue),
                        Name = ddlRole.SelectedItem.Text
                    };
            }
            else
                entry.Role = null;

            // Amount
            if (!string.IsNullOrEmpty(txtAmount.Text))
            {
                entry.HourlyAmount = decimal.Parse(txtAmount.Text);
            }

            if (String.IsNullOrEmpty(txtHoursPerDay.Text) && String.IsNullOrEmpty(txtHoursInPeriod.Text))
            {
                txtHoursPerDay.Text = DEFAULT_NUMBER_HOURS_PER_DAY;
            }


            // Flags

            bool isHoursInPeriodChanged = !String.IsNullOrEmpty(txtHoursInPeriod.Text) &&
                                          (entry.ProjectedWorkloadWithVacation != decimal.Parse(txtHoursInPeriod.Text));

            // Check if need to recalculate Hours per day value
            // Get working days person on Milestone for current person
            DateTime endDate = entry.EndDate.HasValue ? entry.EndDate.Value : Milestone.ProjectedDeliveryDate;
            PersonWorkingHoursDetailsWithinThePeriod personWorkingHoursDetailsWithinThePeriod = GetPersonWorkingHoursDetailsWithinThePeriod(entry.StartDate, endDate, ddlPerson);
            decimal hoursPerDay;

            if (isHoursInPeriodChanged)
            {
                // Recalculate hours per day according to HoursInPerod 
                hoursPerDay = (personWorkingHoursDetailsWithinThePeriod.TotalWorkDaysIncludingVacationDays != 0) ? decimal.Round(decimal.Parse(txtHoursInPeriod.Text) / personWorkingHoursDetailsWithinThePeriod.TotalWorkDaysIncludingVacationDays, 2) : 0;
                // If calculated value more then 24 hours set 24 hours as maximum value for working day
                entry.HoursPerDay = (hoursPerDay > 24M) ? 24M : hoursPerDay;
                // Recalculate Hours In Period
                entry.ProjectedWorkloadWithVacation = entry.HoursPerDay * personWorkingHoursDetailsWithinThePeriod.TotalWorkDaysIncludingVacationDays;
            }
            else
            {
                // If Hours Per Day is ommited set 8 hours as default value for working day
                entry.HoursPerDay = !String.IsNullOrEmpty(txtHoursPerDay.Text)
                                        ? decimal.Parse(txtHoursPerDay.Text)
                                        : 8M;
                // Recalculate Hours In Period
                entry.ProjectedWorkloadWithVacation = entry.HoursPerDay * personWorkingHoursDetailsWithinThePeriod.TotalWorkDaysIncludingVacationDays;
            }



            if (MilestonePersonsEntries.Any(entr => entr.ThisPerson.Id == entry.ThisPerson.Id && entr.IsNewEntry == false))
            {
                var index = MilestonePersonsEntries.FindIndex(entr => entr.ThisPerson.Id == entry.ThisPerson.Id && entr.IsNewEntry == false);
                var mpersonEntry = MilestonePersonsEntries[index];



                entry.MilestonePersonId = mpersonEntry.MilestonePersonId;

                if (isSaveCommit)
                {
                    var entryId = ServiceCallers.Custom.MilestonePerson(mp => mp.MilestonePersonEntryInsert(entry, Context.User.Identity.Name));

                    if (iSDatBindRows)
                    {
                        MilestonePersonEntry mpentry = ServiceCallers.Custom.MilestonePerson(mp => mp.GetMilestonePersonEntry(entryId));
                        MilestonePersonsEntries.Add(mpentry);
                        MilestonePersonsEntries = GetSortedEntries(MilestonePersonsEntries);
                        BindEntriesGrid(MilestonePersonsEntries);
                        HostingPage.Milestone = null;
                        HostingPage.Project = HostingPage.Milestone.Project;
                        HostingPage.FillComputedFinancials(HostingPage.Milestone);
                    }
                }

            }
            else
            {
                if (isSaveCommit)
                {
                    var mPerson = new MilestonePerson()
                    {
                        Milestone = new Milestone()
                        {
                            Id = HostingPage.Milestone.Id
                        },
                        Person = new Person
                        {
                            Id = entry.ThisPerson.Id
                        },
                        Entries = new List<MilestonePersonEntry>()
                        {
                            entry
                        }
                    };

                    MilestonePersonEntry mpentry = null;

                    using (var serviceClient = new MilestonePersonService.MilestonePersonServiceClient())
                    {
                        var id = serviceClient.MilestonePersonAndEntryInsert(mPerson, Context.User.Identity.Name);
                        mpentry = serviceClient.GetMilestonePersonEntry(id);
                        mpentry.IsShowPlusButton = true;
                        entry = mpentry;
                    }

                    if (iSDatBindRows)
                    {
                        MilestonePersonsEntries.Add(mpentry);
                        MilestonePersonsEntries = GetSortedEntries(MilestonePersonsEntries);
                        BindEntriesGrid(MilestonePersonsEntries);
                        HostingPage.Milestone = null;
                        HostingPage.Project = HostingPage.Milestone.Project;
                        HostingPage.FillComputedFinancials(HostingPage.Milestone);
                    }
                }
            }

            if (HostingPage.ValidateNewEntry)
            {
                entry.IsRepeaterEntry = true;
                MilestonePersonsEntries.Add(entry);
                MilestonePersonsEntries = MilestonePersonsEntries;
            }

        }

        protected void imgMilestonePersonEntryEdit_OnClick(object sender, EventArgs e)
        {
            ImageButton imgEdit = sender as ImageButton;
            GridViewRow row = imgEdit.NamingContainer as GridViewRow;
            OnEditClick(row.DataItemIndex);
        }

        protected void btnDeletePersonEntry_OnClick(object sender, EventArgs e)
        {
            var milestonePersonEntryId = Convert.ToInt32(hdMilestonePersonEntryId.Value);
            var index = MilestonePersonsEntries.FindIndex(mpe => mpe.Id == milestonePersonEntryId);
            var entry = MilestonePersonsEntries[index];

            if (!entry.ThisPerson.IsStrawMan)
            {
                //if two entries are there with overlapping periods then we are allowing to delete entry as per #2925 Jason mail.
                if (CheckTimeEntriesExist(entry.MilestonePersonId, entry.StartDate, entry.EndDate, true, true))
                {
                    lblResultMessage.ShowErrorMessage(milestoneHasTimeEntries);
                    return;
                }
                //if there are any project feedback records
                if(CheckFeedbackExists(entry.MilestonePersonId))
                {
                    lblResultMessage.ShowErrorMessage(milestoneHasFeedbackEntries);
                    return;
                }
            }

            // Delete mPersonEntry 

            ServiceCallers.Custom.MilestonePerson(mp => mp.DeleteMilestonePersonEntry(milestonePersonEntryId, Context.User.Identity.Name));
            MilestonePersonsEntries.RemoveAt(index);
            MilestonePersonsEntries = MilestonePersonsEntries;
            MilestonePersonsEntries = GetSortedEntries(MilestonePersonsEntries);
            BindEntriesGrid(MilestonePersonsEntries);

            // Get latest data in detail tab

            HostingPage.Milestone = null;
            HostingPage.FillComputedFinancials(HostingPage.Milestone);
        }

        protected void btnDeleteAllPersonEntries_OnClick(object sender, EventArgs e)
        {
            var milestonePersonEntryId = Convert.ToInt32(hdMilestonePersonEntryId.Value);

            var deleteEntry = MilestonePersonsEntries.First(m => m.Id == milestonePersonEntryId);

            lblResultMessage.ClearMessage();

            var PersonEntries = MilestonePersonsEntries.FindAll(m => m.MilestonePersonId == deleteEntry.MilestonePersonId);

            int? milestonePersonEntryRoleId = deleteEntry.Role != null ? (int?)deleteEntry.Role.Id : null;

            //if two entries are there with overlapping periods then we are allowing to delete entry as per #2925 Jason mail.
            if (CheckTimeEntriesForMilestonePersonWithGivenRoleId(deleteEntry.MilestonePersonId, milestonePersonEntryRoleId))
            {
                lblResultMessage.ShowErrorMessage(allMilestoneHasTimeEntries);
                return;
            }

            //delete all mPersonEntries
            foreach (MilestonePersonEntry entry in PersonEntries)
            {
                if ((entry.Role == null && milestonePersonEntryRoleId == null) || (entry.Role != null && milestonePersonEntryRoleId != null && entry.Role.Id == milestonePersonEntryRoleId))
                {
                    // Delete mPersonEntry 
                    ServiceCallers.Custom.MilestonePerson(mp => mp.DeleteMilestonePersonEntry(entry.Id, Context.User.Identity.Name));
                    var index = MilestonePersonsEntries.FindIndex(mpe => mpe.Id == entry.Id);
                    MilestonePersonsEntries.RemoveAt(index);
                    //MilestonePersonsEntries.Remove(entry);//need to check
                }
            }
            MilestonePersonsEntries = MilestonePersonsEntries;
            MilestonePersonsEntries = GetSortedEntries(MilestonePersonsEntries);
            BindEntriesGrid(MilestonePersonsEntries);

            // Get latest data in detail tab

            HostingPage.Milestone = null;
            HostingPage.FillComputedFinancials(HostingPage.Milestone);


        }

        protected void imgMilestonePersonDelete_OnClick(object sender, EventArgs e)
        {
            ImageButton imgDelete = sender as ImageButton;

            hdMilestonePersonEntryId.Value = imgDelete.Attributes["MilestonePersonEntryId"];

            mpeDeleteMileStonePersons.Show();
        }

        public void OnEditClick(int editRowIndex)
        {
            var entries = MilestonePersonsEntries;

            for (int i = 0; i < entries.Count; i++)
            {
                if (i == editRowIndex)
                {
                    entries[i].IsEditMode = true;
                }
            }

            BindEntriesGrid(entries);
        }

        public void EditResourceByEntryId(int editEntryIdIndex)
        {
            var entries = MilestonePersonsEntries;

            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].Id == editEntryIdIndex)
                {
                    entries[i].IsEditMode = true;
                }
            }

            BindEntriesGrid(entries);
        }

        protected void imgCopy_OnClick(object sender, EventArgs e)
        {
            ImageButton imgCopy = sender as ImageButton;
            GridViewRow row = imgCopy.NamingContainer as GridViewRow;
            var dic = new Dictionary<string, string>();

            if (MilestonePersonsEntries[row.DataItemIndex].IsEditMode)
            {
                var ddlPersonName = row.FindControl("ddlPersonName") as DropDownList;
                var ddlRole = row.FindControl("ddlRole") as DropDownList;
                var dpPersonStart = row.FindControl("dpPersonStart") as DatePicker;
                var dpPersonEnd = row.FindControl("dpPersonEnd") as DatePicker;
                var txtHoursPerDay = row.FindControl("txtHoursPerDay") as TextBox;
                var txtAmount = row.FindControl("txtAmount") as TextBox;
                var txtHoursInPeriod = row.FindControl("txtHoursInPeriod") as TextBox;
                var hdnIsFromAddBtn = row.FindControl("hdnIsFromAddBtn") as HiddenField;


                dic.Add(DDLPERSON_KEY, ddlPersonName.SelectedValue);
                dic.Add(DDLROLE_KEY, ddlRole.SelectedValue);
                dic.Add(dpPersonStartInsert, dpPersonStart.DateValue.ToString());
                dic.Add(dpPersonEndInsert, dpPersonEnd.DateValue.ToString());
                dic.Add(txtAmountInsert, txtAmount.Text);
                dic.Add(txtHoursPerDayInsert, txtHoursPerDay.Text);
                dic.Add(txtHoursInPeriodInsert, txtHoursInPeriod.Text);
                dic.Add("hdnIsFromAddBtn", hdnIsFromAddBtn.Value.ToString());


            }
            else
            {
                var lnkPersonName = row.FindControl("lnkPersonName") as HyperLink;
                var lblRole = row.FindControl("lblRole") as Label;
                var lblStartDate = row.FindControl("lblStartDate") as Label;
                var lblEndDate = row.FindControl("lblEndDate") as Label;
                var lblHoursPerDay = row.FindControl("lblHoursPerDay") as Label;
                var lblAmount = row.FindControl("lblAmount") as Label;
                var lblHoursInPeriodDay = row.FindControl("lblHoursInPeriodDay") as Label;
                var hourlyrate = lblAmount.Text.Replace("$", "");

                dic.Add(DDLPERSON_KEY, lnkPersonName.Attributes["PersonId"]);
                dic.Add(DDLROLE_KEY, lblRole.Attributes["RoleId"]);
                dic.Add(dpPersonStartInsert, lblStartDate.Text);
                dic.Add(dpPersonEndInsert, lblEndDate.Text);
                dic.Add(txtAmountInsert, hourlyrate);
                dic.Add(txtHoursPerDayInsert, lblHoursPerDay.Text);
                dic.Add(txtHoursInPeriodInsert, lblHoursInPeriodDay.Text);
                dic.Add("hdnIsFromAddBtn", false.ToString());

            }

            AddRowAndBindRepeater(dic, false);

        }

        protected void imgMilestonePersonEntryCancel_OnClick(object sender, EventArgs e)
        {
            ImageButton imgMilestonePersonEntryCancel = sender as ImageButton;
            GridViewRow row = imgMilestonePersonEntryCancel.NamingContainer as GridViewRow;

            var entries = MilestonePersonsEntries;
            entries[row.DataItemIndex].IsEditMode = false;

            if (entries[row.DataItemIndex].IsNewEntry)
            {
                entries.RemoveAt(row.DataItemIndex);
            }

            MilestonePersonsEntries = GetSortedEntries(entries);

            BindEntriesGrid(entries);

            lblResultMessage.ClearMessage();
        }

        protected void imgMilestonePersonEntryUpdate_OnClick(object sender, EventArgs e)
        {
            ImageButton imgMilestonePersonEntryUpdate = sender as ImageButton;
            GridViewRow gvRow = imgMilestonePersonEntryUpdate.NamingContainer as GridViewRow;
            milestonePersonEntryUpdate(gvRow.DataItemIndex, gvRow);
        }

        private void milestonePersonEntryUpdate(int mpeIndex, GridViewRow gvRow = null, bool isValidating = true)
        {
            if (gvRow == null)
            {
                gvRow = gvMilestonePersonEntries.Rows[mpeIndex];
            }

            vsumMilestonePersonEntry.ValidationGroup = milestonePersonEntry + gvRow.DataItemIndex.ToString();

            bool result = true;
            if (isValidating)
            {
                Page.Validate(vsumMilestonePersonEntry.ValidationGroup);
                result = Page.IsValid;
            }

            if (result)
            {

                HiddenField hdnPersonId = gvRow.FindControl("hdnPersonId") as HiddenField;
                HiddenField hdnPersonIsStrawMan = gvRow.FindControl("hdnPersonIsStrawMan") as HiddenField;
                var ddl = gvRow.FindControl("ddlPersonName") as DropDownList;
                var ddlRole = gvRow.FindControl("ddlRole") as DropDownList;
                var lblRole = gvRow.FindControl("lblRole") as Label;
                var oldRoleId = lblRole.Attributes["RoleId"];

                var entries = MilestonePersonsEntries;
                var entry = entries[gvRow.DataItemIndex];

                var isEntryInEditmode = entry.IsEditMode;

                if (IsSaveCommit)
                {
                    entry.IsEditMode = false;
                }

                var milestonePersonentry = new MilestonePersonEntry()
                {
                    Id = entry.Id,
                    StartDate = entry.StartDate,
                    EndDate = entry.EndDate,
                    MilestonePersonId = entry.MilestonePersonId,
                    IsEditMode = entry.IsEditMode,
                    ProjectedWorkloadWithVacation = entry.ProjectedWorkloadWithVacation,
                    PersonTimeOffList = entry.PersonTimeOffList,
                    VacationDays = entry.VacationDays,
                    HoursPerDay = entry.HoursPerDay

                };

                if (entry.IsNewEntry)
                {
                    if (!UpdateMilestonePersonEntry(milestonePersonentry, gvMilestonePersonEntries.Rows[gvRow.DataItemIndex], false))
                    {
                        IsErrorOccuredWhileUpdatingRow = true;
                        HostingPage.lblResultObject.ShowErrorMessage("Error occured while saving resources.");
                        return;
                    }

                    if (IsSaveCommit)
                    {
                        var entryId = ServiceCallers.Custom.MilestonePerson(mp => mp.MilestonePersonEntryInsert(milestonePersonentry, Context.User.Identity.Name));
                        if (!HostingPage.IsSaveAllClicked)
                        {
                            MilestonePersonEntry mpentry = ServiceCallers.Custom.MilestonePerson(mp => mp.GetMilestonePersonEntry(entryId));
                            milestonePersonentry = mpentry;
                        }
                        MilestonePersonsEntries[gvRow.DataItemIndex] = milestonePersonentry;

                    }
                }
                else
                {
                    if (!UpdateMilestonePersonEntry(milestonePersonentry, gvMilestonePersonEntries.Rows[gvRow.DataItemIndex], true))
                    {
                        IsErrorOccuredWhileUpdatingRow = true;
                        HostingPage.lblResultObject.ShowErrorMessage("Error occured while saving resources.");
                        return;
                    }


                    if (hdnPersonId.Value == ddl.SelectedValue && oldRoleId == ddlRole.SelectedValue)
                    {
                        IsErrorOccuredWhileUpdatingRow = false;
                        if (IsSaveCommit)
                        {
                            ServiceCallers.Custom.MilestonePerson(mp => mp.UpdateMilestonePersonEntry(milestonePersonentry, Context.User.Identity.Name));
                            if (!HostingPage.IsSaveAllClicked)
                            {
                                MilestonePersonEntry mpentry = ServiceCallers.Custom.MilestonePerson(mp => mp.GetMilestonePersonEntry(entry.Id));
                                milestonePersonentry = mpentry;
                            }
                            MilestonePersonsEntries[gvRow.DataItemIndex] = milestonePersonentry;
                        }
                    }
                    else
                    {
                        IsErrorOccuredWhileUpdatingRow = false;
                        if (IsSaveCommit)
                        {
                            List<MilestonePersonEntry> similarEntryList = new List<MilestonePersonEntry>();


                            similarEntryList.Add(milestonePersonentry);

                            if (hdnPersonIsStrawMan.Value.ToLowerInvariant() == "false")
                            {
                                //Find Similar Entries Indexes
                                var entryList = MilestonePersonsEntries.Where(mpe =>
                                                                                    entry.IsShowPlusButton && mpe.ShowingPlusButtonEntryId == entry.Id
                                                                                     && mpe.Id != entry.Id)
                                                                                     .ToList();

                                similarEntryList.AddRange(entryList);
                            }

                            int mpId = milestonePersonentry.MilestonePersonId;


                            foreach (var mPEntry in similarEntryList)
                            {
                                mPEntry.Role = milestonePersonentry.Role;
                                mPEntry.ThisPerson = milestonePersonentry.ThisPerson;

                                if (HostingPage.IsSaveAllClicked)
                                {
                                    if (!mPEntry.IsNewEntry)
                                    {
                                        if (!(mPEntry.Id == milestonePersonentry.Id))
                                        {
                                            if (mPEntry.IsEditMode)
                                            {
                                                var index = MilestonePersonsEntries.FindIndex(mp => mp.IsNewEntry == false && mp.Id == mPEntry.Id);
                                                UpdateMilestonePersonEntry(mPEntry, gvMilestonePersonEntries.Rows[index], true);
                                                mPEntry.Role = milestonePersonentry.Role;
                                                mPEntry.ThisPerson = milestonePersonentry.ThisPerson;
                                                mPEntry.IsEditMode = false;

                                            }
                                        }

                                        mpId = ServiceCallers.Custom.MilestonePerson(mp => mp.UpdateMilestonePersonEntry(mPEntry, Context.User.Identity.Name));

                                        var indexVal = MilestonePersonsEntries.FindIndex(mpe => mpe.Id == mPEntry.Id);
                                        MilestonePersonsEntries[indexVal] = mPEntry;

                                    }
                                    else
                                    {
                                        var index = MilestonePersonsEntries.FindIndex(mp => mp == mPEntry);
                                        var ddlPersonName = gvMilestonePersonEntries.Rows[index].FindControl("ddlPersonName") as DropDownList;
                                        var ddlRoleName = gvMilestonePersonEntries.Rows[index].FindControl("ddlRole") as DropDownList;
                                        ddlPersonName.SelectedValue = mPEntry.ThisPerson.Id.ToString();
                                        ddlRoleName.SelectedValue = mPEntry.Role != null ? mPEntry.Role.Id.ToString() : string.Empty;
                                    }
                                }
                                else
                                {
                                    if (!mPEntry.IsNewEntry)
                                    {
                                        mpId = ServiceCallers.Custom.MilestonePerson(mp => mp.UpdateMilestonePersonEntry(mPEntry, Context.User.Identity.Name));

                                        if (mPEntry.Id == milestonePersonentry.Id)
                                        {
                                            MilestonePersonEntry latestMpentry = ServiceCallers.Custom.MilestonePerson(mp => mp.GetMilestonePersonEntry(mPEntry.Id));
                                            var index = MilestonePersonsEntries.FindIndex(mpe => mpe.Id == latestMpentry.Id);
                                            MilestonePersonsEntries[index] = latestMpentry;

                                        }
                                        else
                                        {
                                            var index = MilestonePersonsEntries.FindIndex(mpe => mpe.Id == mPEntry.Id);
                                            MilestonePersonsEntries[index] = mPEntry;
                                            if (mPEntry.IsEditMode)
                                            {
                                                MilestonePersonsEntries[index].EditedEntryValues["ddlPersonName"] = mPEntry.ThisPerson.Id.ToString();
                                                MilestonePersonsEntries[index].EditedEntryValues["ddlRole"] = mPEntry.Role != null ? mPEntry.Role.Id.ToString() : string.Empty;
                                            }
                                        }

                                    }
                                    else
                                    {
                                        var index = MilestonePersonsEntries.FindIndex(mp => mp == mPEntry);

                                        MilestonePersonsEntries[index].EditedEntryValues["ddlPersonName"] = mPEntry.ThisPerson.Id.ToString();
                                        MilestonePersonsEntries[index].EditedEntryValues["ddlRole"] = mPEntry.Role != null ? mPEntry.Role.Id.ToString() : string.Empty;
                                    }
                                }


                            }

                            foreach (var mentry in similarEntryList)
                            {
                                mentry.MilestonePersonId = mpId;
                            }


                        }
                    }
                }

                if (IsSaveCommit)
                {
                    MilestonePersonsEntries = !HostingPage.IsSaveAllClicked ? GetFormattedEntries(MilestonePersonsEntries) : MilestonePersonsEntries;
                }


                if (ISDatBindRows)
                {
                    MilestonePersonsEntries = GetSortedEntries(MilestonePersonsEntries);
                    BindEntriesGrid(MilestonePersonsEntries);
                    HostingPage.Milestone = null;
                    HostingPage.Project = HostingPage.Milestone.Project;
                    HostingPage.FillComputedFinancials(HostingPage.Milestone);
                }



            }
            else
            {
                IsErrorOccuredWhileUpdatingRow = true;
                HostingPage.lblResultObject.ShowErrorMessage("Error occured while saving resources.");
            }
        }

        private bool UpdateMilestonePersonEntry(MilestonePersonEntry entry, GridViewRow gridViewRow, bool isRowUpdating)
        {
            lblResultMessage.ClearMessage();
            var dpStartDate = gridViewRow.FindControl("dpPersonStart") as DatePicker;
            var dpEndDate = gridViewRow.FindControl("dpPersonEnd") as DatePicker;
            var ddlRole = gridViewRow.FindControl("ddlRole") as DropDownList;
            var txtHoursPerDay = gridViewRow.FindControl("txtHoursPerDay") as TextBox;
            var txtAmount = gridViewRow.FindControl("txtAmount") as TextBox;
            var txtHoursInPeriod = gridViewRow.FindControl("txtHoursInPeriod") as TextBox;
            var ddlPersonName = gridViewRow.FindControl("ddlPersonName") as DropDownList;

            if (isRowUpdating)
            {
                if (entry.StartDate < dpStartDate.DateValue
                    && CheckTimeEntriesExist(entry.MilestonePersonId, entry.StartDate, dpStartDate.DateValue, true, false)
                   )
                {
                    lblResultMessage.ShowErrorMessage("Cannot update milestone person details because there (s)he has reported hours between existing start date and currently selected start date.");
                    errorOccured = true;
                    return false;
                }
                if (entry.EndDate > dpEndDate.DateValue
                    && CheckTimeEntriesExist(entry.MilestonePersonId, dpEndDate.DateValue, entry.EndDate, false, true)
                   )
                {
                    lblResultMessage.ShowErrorMessage("Cannot update milestone person details because there (s)he has reported hours between currently selected end date and existing end date.");
                    errorOccured = true;
                    return false;
                }
            }

            entry.StartDate = dpStartDate.DateValue;
            entry.EndDate = dpEndDate.DateValue != DateTime.MinValue ? (DateTime?)dpEndDate.DateValue : null;

            if (!string.IsNullOrEmpty(ddlPersonName.SelectedValue))
            {
                var person = GetPersonBySelectedValue(ddlPersonName.SelectedValue);

                entry.ThisPerson = person;
            }

            // Role
            if (!string.IsNullOrEmpty(ddlRole.SelectedValue))
            {
                entry.Role =
                    new PersonRole
                    {
                        Id = int.Parse(ddlRole.SelectedValue),
                        Name = ddlRole.SelectedItem.Text
                    };
            }
            else
                entry.Role = null;

            // Amount
            if (!string.IsNullOrEmpty(txtAmount.Text))
            {
                entry.HourlyAmount = decimal.Parse(txtAmount.Text);
            }

            if (String.IsNullOrEmpty(txtHoursPerDay.Text) && String.IsNullOrEmpty(txtHoursInPeriod.Text))
            {
                txtHoursPerDay.Text = DEFAULT_NUMBER_HOURS_PER_DAY;
            }


            // Flags
            bool isUpdate = (entry.MilestonePersonId != 0);
            bool isHoursInPeriodChanged = !String.IsNullOrEmpty(txtHoursInPeriod.Text) &&
                                          (entry.ProjectedWorkloadWithVacation != decimal.Parse(txtHoursInPeriod.Text));

            // Check if need to recalculate Hours per day value
            // Get working days person on Milestone for current person
            DateTime endDate = entry.EndDate.HasValue ? entry.EndDate.Value : Milestone.ProjectedDeliveryDate;
            PersonWorkingHoursDetailsWithinThePeriod personWorkingHoursDetailsWithinThePeriod = GetPersonWorkingHoursDetailsWithinThePeriod(entry.StartDate, endDate, ddlPersonName);
            decimal hoursPerDay;

            // Update
            if (isUpdate)
            {
                if (isHoursInPeriodChanged)
                {
                    var newTotalHours = decimal.Parse(txtHoursInPeriod.Text);
                    // Recalculate hours per day according to HoursInPerod 
                    hoursPerDay = (personWorkingHoursDetailsWithinThePeriod.TotalWorkDaysIncludingVacationDays != 0) ? decimal.Round(newTotalHours / (personWorkingHoursDetailsWithinThePeriod.TotalWorkDaysIncludingVacationDays), 2) : 0;

                    // If calculated value more then 24 hours set 24 hours as maximum value for working day
                    entry.HoursPerDay = (hoursPerDay > 24M) ? 24M : hoursPerDay;
                    // Recalculate Hours In Period
                    entry.ProjectedWorkloadWithVacation = entry.HoursPerDay * personWorkingHoursDetailsWithinThePeriod.TotalWorkDaysIncludingVacationDays;
                }
                else
                {
                    // If Hours Per Day is ommited set 8 hours as default value for working day
                    entry.HoursPerDay = !String.IsNullOrEmpty(txtHoursPerDay.Text)
                                            ? decimal.Parse(txtHoursPerDay.Text)
                                            : 8M;
                    // Recalculate Hours In Period
                    entry.ProjectedWorkloadWithVacation = entry.HoursPerDay * personWorkingHoursDetailsWithinThePeriod.TotalWorkDaysIncludingVacationDays;
                }
            }
            // Insert
            else
            {
                if (isHoursInPeriodChanged)
                {
                    // Recalculate hours per day according to HoursInPerod 
                    hoursPerDay = (personWorkingHoursDetailsWithinThePeriod.TotalWorkDaysIncludingVacationDays != 0) ? decimal.Round(decimal.Parse(txtHoursInPeriod.Text) / personWorkingHoursDetailsWithinThePeriod.TotalWorkDaysIncludingVacationDays, 2) : 0;
                    // If calculated value more then 24 hours set 24 hours as maximum value for working day
                    entry.HoursPerDay = (hoursPerDay > 24M) ? 24M : hoursPerDay;
                    // Recalculate Hours In Period
                    entry.ProjectedWorkloadWithVacation = entry.HoursPerDay * personWorkingHoursDetailsWithinThePeriod.TotalWorkDaysIncludingVacationDays;
                }
                else
                {
                    // If Hours Per Day is ommited set 8 hours as default value for working day
                    entry.HoursPerDay = !String.IsNullOrEmpty(txtHoursPerDay.Text)
                                            ? decimal.Parse(txtHoursPerDay.Text)
                                            : 8M;
                    // Recalculate Hours In Period
                    entry.ProjectedWorkloadWithVacation = entry.HoursPerDay * personWorkingHoursDetailsWithinThePeriod.TotalWorkDaysIncludingVacationDays;
                }
            }

            return true;
        }

        private bool CheckTimeEntriesExist(int MilestonePersonId, DateTime? startDate, DateTime? endDate, bool checkStartDateEquality, bool checkEndDateEquality)
        {
            using (var serviceClient = new MilestonePersonServiceClient())
            {
                try
                {
                    return serviceClient.CheckTimeEntriesForMilestonePerson(MilestonePersonId, startDate, endDate, checkStartDateEquality, checkEndDateEquality);

                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        private bool CheckFeedbackExists(int MilestonePersonId)
        {
            using (var serviceClient = new ProjectServiceClient())
            {
                try
                {
                    return serviceClient.CheckIfFeedbackExists(MilestonePersonId,null,null);

                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        private bool CheckTimeEntriesForMilestonePersonWithGivenRoleId(int MilestonePersonId, int? MilestonePersonRoleId)
        {
            using (var serviceClient = new MilestonePersonServiceClient())
            {
                try
                {
                    return serviceClient.CheckTimeEntriesForMilestonePersonWithGivenRoleId(MilestonePersonId, MilestonePersonRoleId);

                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        protected bool GetIsHourlyRate()
        {
            return Milestone.IsHourlyAmount;
        }

        private PersonWorkingHoursDetailsWithinThePeriod GetPersonWorkingHoursDetailsWithinThePeriod(DateTime startDate, DateTime endDate, DropDownList ddlPersonName)
        {
            if (!string.IsNullOrEmpty(ddlPersonName.SelectedValue))
            {
                using (var serviceClient = new PersonServiceClient())
                {
                    try
                    {
                        return serviceClient.GetPersonWorkingHoursDetailsWithinThePeriod(int.Parse(ddlPersonName.SelectedValue), startDate,
                                                                     endDate);
                    }
                    catch (FaultException<ExceptionDetail> ex)
                    {
                        _internalException = ex.Detail;
                        serviceClient.Abort();
                        Page.Validate();
                    }
                    catch (CommunicationException ex)
                    {
                        _internalException = new ExceptionDetail(ex);
                        serviceClient.Abort();
                        Page.Validate();
                    }
                }
            }
            return null;
        }

        internal void CopyItemAndDaabindRepeater(int barIndex)
        {
            MilestonePersonEntry mpentry = AddMilestonePersonEntries[barIndex];

            AddMilestonePersonEntries.Add(mpentry);
            Dictionary<string, string> dic = repeaterOldValues[barIndex];
            repeaterOldValues.Add(dic);
            AddMilestonePersonEntries = AddMilestonePersonEntries;
            repeaterOldValues = repeaterOldValues;
            repPerson.DataSource = AddMilestonePersonEntries;
            repPerson.DataBind();
        }

        internal bool ValidateAll()
        {
            SaveToTemporaryEntry();
            var result = true;
            var mpersons = new List<MilestonePerson>();


            if (MilestonePersonsEntries.Any(mpEntry => mpEntry.IsEditMode))
            {

                for (int i = 0; i < MilestonePersonsEntries.Count; i++)
                {
                    if (MilestonePersonsEntries[i].IsEditMode)
                    {
                        vsumMilestonePersonEntry.ValidationGroup = milestonePersonEntry + i.ToString();
                        Page.Validate(vsumMilestonePersonEntry.ValidationGroup);
                        if (Page.IsValid)
                        {
                            IsSaveCommit = false;
                            ISDatBindRows = false;
                            milestonePersonEntryUpdate(i);
                            IsSaveCommit = true;
                            ISDatBindRows = true;
                            if (IsErrorOccuredWhileUpdatingRow)
                            {
                                result = false;
                            }
                        }
                        else
                        {
                            result = false;
                        }
                    }
                }
            }

            if (repPerson.Items.Count > 0)
            {

                bool output = true;
                foreach (RepeaterItem mpBar in repPerson.Items)
                {
                    var bar = mpBar.FindControl(mpbar) as MilestonePersonBar;
                    output = bar.ValidateAll(mpBar, false);
                    if (!output)
                    {
                        result = false;
                    }
                }
            }

            GetAndRemoveTemporaryEntry();
            return result;
        }

        internal bool SaveAll()
        {
            var result = true;
            var mpersons = new List<MilestonePerson>();

            if (MilestonePersonsEntries.Any(mpEntry => mpEntry.IsEditMode))
            {

                for (int i = 0; i < MilestonePersonsEntries.Count; i++)
                {
                    if (MilestonePersonsEntries[i].IsEditMode)
                    {
                        IsSaveCommit = true;
                        ISDatBindRows = false;
                        milestonePersonEntryUpdate(i, null, false);
                        IsSaveCommit = true;
                        ISDatBindRows = true;
                        if (IsErrorOccuredWhileUpdatingRow)
                        {
                            result = false;
                        }
                    }
                }
            }

            if (result)
            {
                if (repPerson.Items.Count > 0)
                {

                    bool output = true;
                    foreach (RepeaterItem mpBar in repPerson.Items)
                    {
                        var bar = mpBar.FindControl(mpbar) as MilestonePersonBar;
                        output = bar.SaveAll(mpBar, true, false);
                        if (!output)
                        {
                            result = false;
                        }
                    }
                }
            }



            return result;
        }

        protected void imgAdditionalAllocationOfResource_OnClick(object sender, EventArgs e)
        {
            var btn = sender as ImageButton;

            var entriesCount = btn.Attributes["entriesCount"];
            int count;

            if (!string.IsNullOrEmpty(entriesCount))
            {
                count = Convert.ToInt32(entriesCount) + 1;
            }
            else
            {
                count = 1;
            }

            var gvRow = btn.NamingContainer as GridViewRow;

            var entrieslist = MilestonePersonsEntries;

            var modifiedEntries = new List<MilestonePersonEntry>();

            for (int i = 0; i < entrieslist.Count; i++)
            {

                if (i == gvRow.DataItemIndex)
                {
                    entrieslist[i].ExtendedResourcesRowCount = count;
                }

                modifiedEntries.Add(entrieslist[i]);


                if (i == gvRow.DataItemIndex)
                {
                    if (count < 6)
                    {
                        var mpe = new MilestonePersonEntry()
                        {
                            ThisPerson = entrieslist[i].ThisPerson,
                            Role = entrieslist[i].Role,
                            IsEditMode = true,
                            IsShowPlusButton = false,
                            IsNewEntry = true,
                            MilestonePersonId = entrieslist[i].MilestonePersonId,
                            StartDate = entrieslist[i].EndDate < HostingPage.Milestone.EndDate ? entrieslist[i].EndDate.Value.AddDays(1).Date : entrieslist[i].EndDate.Value.Date,
                            EndDate = HostingPage.Milestone.EndDate
                        };

                        modifiedEntries.Add(mpe);
                        StoreGridViewEditedEntriesInObject(i + 1, modifiedEntries);

                    }
                }

            }

            MilestonePersonsEntries = GetSortedEntries(modifiedEntries);

            BindEntriesGrid(MilestonePersonsEntries);
        }

        internal void AddEmptyRow()
        {
            if (gvMilestonePersonEntries.Rows.Count == 0 && repPerson.Items.Count == 0)
            {
                thInsertMilestonePerson.Visible = true;
                thHourlyRate.Visible = Milestone.IsHourlyAmount;

                AddRowAndBindRepeater(null, true);
            }
            else
            {
                thInsertMilestonePerson.Visible = false;
            }
        }

        protected string GetValidationGroup(object dataItem)
        {
            var di = dataItem as GridViewRow;
            if (di != null)
            {
                return milestonePersonEntry + di.DataItemIndex.ToString();
            }
            else
            {
                return milestonePersonEntry;
            }
        }

        protected string GetTimeOffHoursToolTip(decimal vacationHours, decimal timeoffHours)
        {
            return "<label class=\"fontBold\"> Time-Off Hour(s): </label>" + timeoffHours.ToString("0.00") + "\n Affected Projected Hours:" + vacationHours.ToString("0.00");
        }

        private void SaveToTemporaryEntry()
        {

            if (MilestonePersonsEntries.Any(mpEntry => mpEntry.IsEditMode))
            {

                for (int i = 0; i < MilestonePersonsEntries.Count; i++)
                {
                    var mpentry = MilestonePersonsEntries[i];
                    if (mpentry.IsEditMode)
                    {
                        var entry = new MilestonePersonEntry();

                        entry.StartDate = mpentry.StartDate;
                        entry.EndDate = mpentry.EndDate;
                        entry.ThisPerson = new Person
                        {
                            Id = mpentry.ThisPerson.Id
                        };

                        var role = mpentry.Role != null ?
                        new PersonRole
                        {
                            Id = mpentry.Role.Id,
                            Name = mpentry.Role.Name
                        } : null;

                        entry.Role = role;

                        entry.HourlyAmount = mpentry.HourlyAmount;
                        entry.HoursPerDay = mpentry.HoursPerDay;
                        entry.ProjectedWorkloadWithVacation = mpentry.ProjectedWorkloadWithVacation;
                        entry.PersonTimeOffList = mpentry.PersonTimeOffList;

                        mpentry.PreviouslySavedEntry = entry;
                    }

                    //if (mpentry.IsNewEntry && !mpentry.IsShowPlusButton)
                    //{
                    //    var gvRow = gvMilestonePersonEntries.Rows[i];

                    //    var index = MilestonePersonsEntries.FindIndex(en => en.Id == mpentry.ShowingPlusButtonEntryId);


                    //    var ddlPersonName = gvRow.FindControl("ddlPersonName") as DropDownList;
                    //    var ddlRoleName = gvRow.FindControl("ddlRole") as DropDownList;
                    //    ddlPersonName.SelectedValue = mPEntry.ThisPerson.Id.ToString();
                    //    ddlRoleName.SelectedValue = mPEntry.Role != null ? mPEntry.Role.Id.ToString() : string.Empty;
                    //}

                }
            }
        }

        private void GetAndRemoveTemporaryEntry()
        {
            var entries = MilestonePersonsEntries.Where(mpentry => mpentry.IsRepeaterEntry == false).AsQueryable().ToList();

            MilestonePersonsEntries = entries;

            if (MilestonePersonsEntries.Any(mpEntry => mpEntry.IsEditMode))
            {

                for (int i = 0; i < MilestonePersonsEntries.Count; i++)
                {
                    var entry = MilestonePersonsEntries[i];

                    if (entry.IsEditMode)
                    {
                        var mpentry = MilestonePersonsEntries[i].PreviouslySavedEntry;
                        entry.StartDate = mpentry.StartDate;
                        entry.EndDate = mpentry.EndDate;
                        entry.ThisPerson = GetPersonBySelectedValue(mpentry.ThisPerson.Id.Value.ToString());

                        entry.Role = mpentry.Role != null ?
                        new PersonRole
                        {
                            Id = mpentry.Role.Id,
                            Name = mpentry.Role.Name
                        } : null;

                        entry.HourlyAmount = mpentry.HourlyAmount;
                        entry.HoursPerDay = mpentry.HoursPerDay;
                        entry.ProjectedWorkloadWithVacation = mpentry.ProjectedWorkloadWithVacation;
                        entry.PersonTimeOffList = mpentry.PersonTimeOffList;
                    }

                    MilestonePersonsEntries[i].PreviouslySavedEntry = null;
                }
            }

        }

        #endregion
    }
}

