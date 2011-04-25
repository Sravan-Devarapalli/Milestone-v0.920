using System;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.Controls.Persons;
using PraticeManagement.Controls.TimeEntry;
using PraticeManagement.Utils;
using System.Web.Security;
using System.Web.UI;

namespace PraticeManagement
{
    public partial class TimeEntry : PracticeManagementPageBase, IPostBackEventHandler
    {
        #region Constants

        private const string Note_ErrorMessageKey = "Note_ErrorMessage";
        private const string Hours_ErrorMessageKey = "Hours_ErrorMessage";
        private const string dropdown_ErrorMessageKey = "MilestoneProjectTimeType";

        #endregion Constants

        #region Properties

        public MilestonePersonEntry[] MilestonePersonEntries
        {
            get
            {
                return teList.MilestonePersonEntries;
            }
        }

        public TimeEntries TimeEntryControl
        {
            get
            {
                return teList;
            }
        }

        public DateTime[] SelectedDates
        {
            get
            {
                return wsChoose.SelectedDates;
            }
        }

        public Person SelectedPerson
        {
            get
            {
                return pcPersons.SelectedPerson;
            }
        }

        #endregion

        #region Methods

        protected override void OnLoadComplete(EventArgs e)
        {
            if (!IsPostBack)
            {
                UpdateTimeEntries();
            }
        }

        private void UpdateTimeEntries()
        {
            var isSelectedActive =
                SelectedPerson.Status != null &&
                SelectedPerson.Status.ToStatusType() == PersonStatusType.Active;
            var currentIsAdmin =
                Roles.IsUserInRole(
                    DataTransferObjects.Constants.RoleNames.AdministratorRoleName);

            var showGrid = isSelectedActive || currentIsAdmin;
            teList.Visible = showGrid;
            if (showGrid)
            {
                teList.SelectedDates = SelectedDates;
                teList.MilestonePersonEntries =
                    TimeEntryHelper.GetTimeEntryMilestones(
                        SelectedPerson,
                        wsChoose.SelectedStartDate,
                        wsChoose.SelectedEndDate, true);
                teList.SelectedPerson = SelectedPerson;
                teList.UpdateTimeEntries();

                mlErrors.ClearMessage();
                SetAttributesToDropDowns();
            }
            else
                mlErrors.ShowErrorMessage(Resources.Messages.NotAllowedToRecordTE);
        }

        public void SetAttributesToDropDowns()
        {
            foreach (System.Web.UI.WebControls.RepeaterItem item in (teList.FindControl("tes") as System.Web.UI.WebControls.Repeater).Items)
            {
                var timeEntryBar = item.FindControl("bar") as TimeEntryBar;
                var projectMilestone = (timeEntryBar.FindControl("ddlProjectMilestone") as System.Web.UI.WebControls.DropDownList);
                var timeType = (timeEntryBar.FindControl("ddlTimeTypes") as System.Web.UI.WebControls.DropDownList);
                projectMilestone.Attributes.Add("StartDate", this.wsChoose.SelectedStartDate.ToString());
                projectMilestone.Attributes.Add("EndDate", this.wsChoose.SelectedEndDate.ToString());
                timeType.Attributes.Add("StartDate", this.wsChoose.SelectedStartDate.ToString());
                timeType.Attributes.Add("EndDate", this.wsChoose.SelectedEndDate.ToString());
            }
        }

        protected void wsChoose_WeekChanged(object sender, WeekChangedEventArgs args)
        {
            UpdateTimeEntries();
        }

        protected void pcPersons_PersonChanged(object sender, PersonChangedEventArguments args)
        {
            UpdateTimeEntries();
            wsChoose.UpdateWeekLabel();
        }

        protected void btnAddRow_Click(object sender, EventArgs e)
        {
            if (teList.SaveData())
            {
                UpdateTimeEntries();
                teList.DatabindAndShowGrid(true);
                SetAttributesToDropDowns();
            }

            ClearDirtyState();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (teList.SaveData())
            {
                mlConfirmation.ShowInfoMessage(Resources.Messages.DataSavedSuccessfully);
                UpdateTimeEntries();
            }

            ClearDirtyState();
        }

        public void ClearDirtyState()
        {
            var pageBase = this.Page as PracticeManagementPageBase;
            if (pageBase != null)
            {
                pageBase.IsDirty = false;
            }
            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "clearDirty();", true);
        }

        protected override void Display()
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            mlConfirmation.ClearMessage();
            if (!IsPostBack)
            {
                btnSave.Attributes.Add("disabled", "disabled");
            }
        }

        private void RaiseError()
        {
            if (Session[Hours_ErrorMessageKey] != null && Session[Hours_ErrorMessageKey].ToString() == "Hours")
            {
                valTeHours.IsValid = false;
            }
            if (Session[Note_ErrorMessageKey] != null && Session[Note_ErrorMessageKey].ToString() == "Note")
            {
                valTeNote.IsValid = false;
            }
            if (Session[dropdown_ErrorMessageKey] != null && Session[dropdown_ErrorMessageKey].ToString() == "True")
            {
                valMileProjTimeTypeDropdown.IsValid = false;
            }
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            Page.Validate();
            Session[Note_ErrorMessageKey] = null;
            Session[Hours_ErrorMessageKey] = null;
            Session[dropdown_ErrorMessageKey] = null;
            if (Page.IsValid && teList.SaveData())
            {
                Redirect(eventArgument);
            }
            else
            {
                RaiseError();
            }
        }

        #endregion

    }
}

