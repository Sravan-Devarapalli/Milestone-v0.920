using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using DataTransferObjects.TimeEntry;
using PraticeManagement.Utils;
using PraticeManagement.TimeEntryService;
using System.ServiceModel;
using System.Linq;

namespace PraticeManagement.Controls.TimeEntry
{
    public partial class TimeEntryBar : UserControl
    {
        #region Constants

        private const string CONTROL_STE_ITEM = "ste";

        #endregion

        #region Events

        public EventHandler RowRemoved;

        #endregion

        #region Properties

        public TeGridRow RowBehind { get; set; }
        public Person SelectedPerson
        {
            get
            {
                return HostingPage.SelectedPerson;
            }
        }

        private PraticeManagement.TimeEntry HostingPage
        {
            get { return ((PraticeManagement.TimeEntry)Page); }
        }

        public IEnumerable<SingleTimeEntry> TimeEntryControls
        {
            get
            {
                foreach (Control entry in tes.Controls)
                    yield return entry.FindControl(CONTROL_STE_ITEM) as SingleTimeEntry;
            }
        }

        private void UpdateControlStatuses()
        {
            if (RowBehind == null) return;

            if (RowBehind.MilestoneBehind != null)
            {

                DateTime weekStartDate = HostingPage.SelectedDates.First();
                DateTime weekEndDate = HostingPage.SelectedDates.Last();
                var tesFiltered = RowBehind.Cells.FindAll(cell => cell.Day.Date >= weekStartDate && cell.Day.Date <= weekEndDate && cell.TimeEntry != null);
                if (tesFiltered.Any())
                {
                    ddlProjectMilestone.SelectedValue =
                        RowBehind.MilestoneBehind.MilestonePersonId.ToString();
                }
            }

            if (RowBehind.TimeTypeBehind != null)
            {
                ddlTimeTypes.SelectedValue =
                    RowBehind.TimeTypeBehind.Id.ToString();
            }
            //else
            //{
            //    var defaultTimeType = GetDefaultTimeType();

            //    if (defaultTimeType != null)
            //        ddlTimeTypes.SelectedValue = defaultTimeType;
            //}
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                AddTitlestoListItems(ddlProjectMilestone);
                AddTitlestoListItems(ddlTimeTypes);
            }
            if (RowBehind != null)
            {
                UpdateControlStatuses();
            }
        }

        private string GetDefaultTimeType()
        {
            var allTimeTypes = odsTimeTypes.Select();
            foreach (object tt in allTimeTypes)
            {
                var timeType = tt as TimeTypeRecord;
                if (timeType.IsDefault)
                    return timeType.Id.ToString();
            }

            return null;
        }

        public void ValidateControl()
        {
            Page.Validate(ClientID);
        }

        public void UpdateTimeEntries()
        {
            TimeEntryHelper.FillProjectMilestones(
                ddlProjectMilestone,
                HostingPage.MilestonePersonEntries);

            UpdateControlStatuses();

            tes.DataSource = RowBehind;
            tes.DataBind();
        }

        protected void repEntries_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var ste = e.Item.FindControl(CONTROL_STE_ITEM) as SingleTimeEntry;
            var cell = e.Item.DataItem as TeGridCell;

            ste.VerticalTotalCalculatorExtenderId = extTotalHours.ClientID;
            extTotalHours.ControlsToCheck += ste.Controls[1].ClientID + ";";
            ste.ProjectMilestoneDropdownClientId = this.ddlProjectMilestone.ClientID;
            ste.TimeTypeDropdownClientId = this.ddlTimeTypes.ClientID;
            InitTimeEntryControl(ste, cell);
        }

        private void InitTimeEntryControl(SingleTimeEntry ste, TeGridCell cell)
        {
            var dateBehind = cell.Day.Date;
            ste.DateBehind = dateBehind;
            ste.TimeEntryBehind = cell.TimeEntry;

            var selMpe = Array.FindAll(HostingPage.MilestonePersonEntries, mpe => mpe.MilestonePersonId == cell.MilestoneBehind.MilestonePersonId);

            DisableInvalidDatesAndChargeability(ste, dateBehind, selMpe);
        }

        private void DisableInvalidDatesAndChargeability(SingleTimeEntry ste, DateTime dateBehind, MilestonePersonEntry[] entries)
        {
            ste.CanelControlStyle();

            var personHired = dateBehind >= SelectedPerson.HireDate &&
                              dateBehind <= (SelectedPerson.TerminationDate.HasValue
                                                 ?
                                                     SelectedPerson.TerminationDate.Value
                                                 :
                                                     DateTime.MaxValue);

            var withinMilestoneDates = false;
            bool isChargeable = false;
            bool canAdjust = false;
            if (entries.Length > 0)
            {
                isChargeable = entries[0].ParentMilestone.IsChargeable;
                withinMilestoneDates = entries.ToList().Exists(mpe => (dateBehind >= mpe.StartDate && dateBehind <= (mpe.EndDate ?? mpe.ParentMilestone.ProjectedDeliveryDate)));
                canAdjust = entries[0].ParentMilestone.ConsultantsCanAdjust;
            }

            ste.IsChargeable = isChargeable;
            ste.Disabled = !personHired || !withinMilestoneDates;
            bool disabled = false;
            if (ddlProjectMilestone.SelectedIndex == 0 || ddlTimeTypes.SelectedIndex == 0
            || (ste.TimeEntryBehind != null && ste.TimeEntryBehind.ParentMilestonePersonEntry != null
                && ste.TimeEntryBehind.IsReviewed == ReviewStatus.Approved
                && ste.TimeEntryBehind.TimeType.Id.ToString() == ddlTimeTypes.SelectedValue
                && ste.TimeEntryBehind.ParentMilestonePersonEntry.MilestonePersonId.ToString() == ddlProjectMilestone.SelectedValue)
                )
            {
                disabled = ste.Disabled = true;
            }
            if (!disabled && ste.TimeEntryBehind == null && entries.Any() 
                && entries.First().ParentMilestone.Project.Status != null)
            {
                var projectStatusId = entries.First().ParentMilestone.Project.Status.Id;
                if (projectStatusId == (int)ProjectStatusType.Completed
                        || projectStatusId == (int)ProjectStatusType.Experimental
                        || projectStatusId == (int)ProjectStatusType.Inactive
                    )
                {
                    ste.Disabled = true;
                }

            }
            if (personHired)
                ste.ChargeabilityEnabled = canAdjust;
        }


        protected bool steItem_OnReadyToUpdateTE(object sender, ReadyToUpdateTeArguments args)
        {
            ValidateControl();
            return UpdateTimeEntry(args.TimeEntry);
        }

        /// <summary>
        /// Updates time entry and shows corresponding message
        /// </summary>
        private bool UpdateTimeEntry(TimeEntryRecord timeEntry)
        {
            if (ddlProjectMilestone.SelectedIndex == 0 || ddlTimeTypes.SelectedIndex == 0)
            {
                return false;
            }
            timeEntry.ParentMilestonePersonEntry =
                new MilestonePersonEntry(ddlProjectMilestone.SelectedValue);
            timeEntry.TimeType =
                new TimeTypeRecord(ddlTimeTypes.SelectedValue);
            timeEntry.ModifiedBy = DataHelper.CurrentPerson;
            timeEntry.ParentMilestonePersonEntry.ThisPerson = SelectedPerson;

            if (timeEntry.Id.HasValue)
                return TimeEntryHelper.UpdateTimeEntry(timeEntry);
            else
                return TimeEntryHelper.AddTimeEntry(timeEntry);
        }

        protected void ddlProjectMilestone_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            MilestoneTimeTypeChanged(sender, true);
        }

        private void MilestoneTimeTypeChanged(object sender, bool isMilestoneDropDown)
        {
            var DropDown = sender as DropDownList;

            //Clear the bordercolor when there is an error previously and highlighted this control.
            DropDown.BorderColor = new System.Drawing.Color();
            int selMpId;
            MilestonePersonEntry[] selMpe = null;
            DateTime weekStartDate = Convert.ToDateTime(DropDown.Attributes["StartDate"]);
            DateTime weekEndDate = Convert.ToDateTime(DropDown.Attributes["EndDate"]);
            var tesFiltered = TimeEntryControls.ToList().FindAll(control => control.DateBehind != null
                                    && control.DateBehind >= weekStartDate
                                    && control.DateBehind <= weekEndDate);
            selMpId = Convert.ToInt32(ddlProjectMilestone.SelectedValue);
            selMpe = Array.FindAll(HostingPage.MilestonePersonEntries, mpe => mpe.MilestonePersonId == selMpId);


            if (tesFiltered.Any())
            {
                bool enableSaveAllButton = false;
                foreach (var entryControl in tesFiltered)
                {
                    entryControl.UpdatehiddenHours();
                    ImageButton imgNote = entryControl.FindControl("imgNote") as ImageButton;
                    TextBox tbNotes = entryControl.FindControl("tbNotes") as TextBox;
                    imgNote.ImageUrl = string.IsNullOrEmpty(tbNotes.Text) ?
                        PraticeManagement.Constants.ApplicationResources.AddCommentIcon :
                        PraticeManagement.Constants.ApplicationResources.RecentCommentIcon;
                    DisableInvalidDatesAndChargeability(entryControl, entryControl.DateBehind, selMpe);

                    if (!enableSaveAllButton &&
                            (entryControl.TimeEntryBehind != null ||
                             entryControl.Modified ||
                             !string.IsNullOrEmpty(tbNotes.Text)
                            ) && ddlTimeTypes.SelectedIndex > 0 && ddlProjectMilestone.SelectedIndex > 0
                        )
                    {
                        enableSaveAllButton = true;
                    }
                }
                if (enableSaveAllButton)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "TimeTypeOrProjectMilestoneChanged", "EnableSaveButton(true);", true);
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "TimeTypeOrProjectMilestoneChanged", "EnableSaveButton(false);", true);
                }
            }
        }

        protected void ddlTimeTypes_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            MilestoneTimeTypeChanged(sender, false);
        }

        public IEnumerable<TimeTypeRecord> GetAllTimeTypes()
        {
            TimeTypeRecord[] result;

            using (var serviceClient = new TimeEntryServiceClient())
            {
                try
                {
                    result
                        = serviceClient
                            .GetAllTimeTypes()
                            .Select(t => new TimeTypeRecord()
                                {
                                    Name = t.Name,
                                    Id = t.Id,
                                    InUse = t.InUse,
                                    IsDefault = t.IsDefault
                                })
                            .ToArray();
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }

            return result;
        }

        protected string GetDayOffCssCalss(TeGridCell teGridCell)
        {
            return Utils.Calendar.GetCssClassByCalendarItem(teGridCell.Day); //string.Empty;
        }

        protected void ddlTimeTypes_DataBound(object sender, EventArgs e)
        {
            if (ddlTimeTypes.Items.FindByValue("-1") == null)
                ddlTimeTypes.Items.Insert(0, (new ListItem("2. Select Time Type", "-1")));
            AddTitlestoListItems(ddlTimeTypes);
        }

        protected void imgDropTes_Click(object sender, ImageClickEventArgs e)
        {
            var daysNumber = HostingPage.SelectedDates.Length;
            var startDate = HostingPage.SelectedDates[0];
            var endDate = HostingPage.SelectedDates[daysNumber - 1];

            TimeEntryHelper.RemoveTimeEntries(ddlProjectMilestone.SelectedValue, ddlTimeTypes.SelectedValue, startDate, endDate);

            HostingPage.TimeEntryControl.UpdateTimeEntries();
            HostingPage.ClearDirtyState();
            HostingPage.SetAttributesToDropDowns();
        }

        public void AddTitlestoListItems(DropDownList dropDownList)
        {
            if (dropDownList == null)
            {
                return;
            }

            foreach (ListItem item in dropDownList.Items)
            {
                item.Attributes.Add("title", item.Text);
            }
        }
    }
}

