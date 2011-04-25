using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;
using DataTransferObjects.TimeEntry;
using PraticeManagement.Controls.Generic.Buttons;
using PraticeManagement.Controls.Generic.Sorting.MultisortExtender;
using PraticeManagement.Utils;
using System.Linq;

namespace PraticeManagement.Controls.TimeEntry
{
    public partial class TimeEntryManagement : UserControl
    {
        private const string LblTotalActual = "lblTotalActual";
        private const string LblTotalForecasted = "lblTotalForecasted";
        private double _totalActual = 0d;
        private double _totalForecasted = 0d;

        protected void Page_Load(object sender, EventArgs e)
        {
            UserIsAdmin =
                Roles.IsUserInRole(
                    DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
        }

        protected bool GetEditingAllowed(TimeEntryRecord timeEntryRecord)
        {
            return (UserIsAdmin ||
                        (timeEntryRecord.IsReviewed != ReviewStatus.Approved
                        && timeEntryRecord.ParentMilestonePersonEntry.ThisPerson.Id != DataHelper.CurrentPerson.Id
                        )
                   ) && !IsReadOnly;
        }

        private DropDownList GetDropDownList(int rowIndex, string controlName)
        {
            return GetControl(rowIndex, controlName) as DropDownList;
        }

        protected bool UserIsAdmin { get; private set; }

        protected bool IsReadOnly
        {
            get
            {
                return !Roles.GetRolesForUser()
                    .Any(r => !r.Equals(
                        DataTransferObjects.Constants.RoleNames.SalespersonRoleName,
                        StringComparison.CurrentCultureIgnoreCase));
            }
        }

        /// <summary>
        /// Because TimeEntryRecord is complex object,
        /// we need to initialize it's non-primitive type fields manualy,
        /// because databinding doesn't support deep objects
        /// </summary>
        protected void gvTimeEntries_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            DropDownList ddlTimeTypeUpd =
                GetDropDownList(e.RowIndex, Constants.ControlNames.DDL_TIMETYPE_EDIT);
            DropDownList ddlMilestonePersonUpd =
                GetDropDownList(e.RowIndex, Constants.ControlNames.DDL_PROJECT_MILESTONES_EDIT);

            SetUpdateParams(
                odsTimeEntries,
                Constants.MethodParameterNames.TIME_TYPE_ID,
                ddlTimeTypeUpd.SelectedValue);
            SetUpdateParams(
                odsTimeEntries,
                Constants.MethodParameterNames.MILESTONE_PERSON_ID,
                ddlMilestonePersonUpd.SelectedValue);
        }

        /// <summary>
        /// Add currently logged in person's id to the updated TE
        /// </summary>
        protected void odsTimeEntries_Updating(object sender, ObjectDataSourceMethodEventArgs e)
        {
            e.InputParameters.Add(Constants.MethodParameterNames.MODIFIED_BY_ID, DataHelper.CurrentPerson.Id);
        }

        /// <summary>
        /// When we're about to view editable row, populate project-milestones drop-down
        /// </summary>
        protected void gvTimeEntries_RowEditing(object sender, GridViewEditEventArgs e)
        {
            //  Get seleced row's time entry object person
            string personId = GetHiddenField(e.NewEditIndex, Constants.ControlNames.HF_PERSON).Value;
            //  Pass it's id to the datasource select parameter
            SetSelectParams(odsCurrentMilestones, Constants.MethodParameterNames.PERSON_ID, personId);
        }

        #region Utils

        private static void SetUpdateParams(ObjectDataSource dataSource, string parameterName, string val)
        {
            dataSource.UpdateParameters[parameterName].DefaultValue = val;
        }

        private static void SetSelectParams(ObjectDataSource dataSource, string parameterName, string val)
        {
            dataSource.SelectParameters[parameterName].DefaultValue = val;
        }

        private HiddenField GetHiddenField(int rowIndex, string controlName)
        {
            return GetControl(rowIndex, controlName) as HiddenField;
        }

        private Control GetControl(int rowIndex, string controlName)
        {
            return gvTimeEntries.Rows[rowIndex].FindControl(controlName);
        }

        #endregion

        private void UpdateGrid()
        {
            // Resetting the grid's page index to zero as we need to update.
            gvTimeEntries.PageIndex = 0;
            gvTimeEntries.DataBind();

            repTotals.DataBind();
        }

        protected void odsTimeEntries_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["selectContext"] = this.TimeEntrySelectContext;
        }

        #region Redirection

        protected string GetPersonUrl(Person person)
        {
            if (!IsReadOnly)
            {
                return string.Format(
                    Constants.ApplicationPages.DetailRedirectWithReturnFormat,
                    Constants.ApplicationPages.PersonDetail,
                    person.Id.Value,
                    Constants.ApplicationPages.TimeEntryReport);
            }
            return string.Empty;
        }

        protected string GetMilestonePersonUrl(MilestonePersonEntry mpe)
        {
            if (!IsReadOnly)
            {
                return string.Format(
                                    Constants.ApplicationPages.RedirectMilestonePersonIdFormatWithReturn,
                                    Constants.ApplicationPages.MilestonePersonDetail,
                                    mpe.ParentMilestone.Id.Value,
                                    mpe.MilestonePersonId,
                                    HttpUtility.UrlEncode(GetMilestoneUrl(mpe.ParentMilestone))
                                );
            }
            return string.Empty;
        }

        private string GetMilestoneUrl(Milestone milestone)
        {
            if (!IsReadOnly)
            {
                return string.Format(
                                    Constants.ApplicationPages.MilestonePrevNextRedirectFormat,
                                    Constants.ApplicationPages.MilestoneDetail,
                                    milestone.Id.Value,
                                    milestone.Project.Id.Value
                            );
            }
            return string.Empty;
        }

        protected string GetProjectUrl(Project project)
        {
            if (!IsReadOnly)
            {
                return string.Format(
                        Constants.ApplicationPages.DetailRedirectWithReturnFormat,
                        Constants.ApplicationPages.ProjectDetail,
                        project.Id.Value,
                        Constants.ApplicationPages.TimeEntryReport
                    );
            }
            return string.Empty;
        }

        protected string GetClientUrl(Client client)
        {
            if (!IsReadOnly)
            {
                return string.Format(
                        Constants.ApplicationPages.DetailRedirectWithReturnFormat,
                        Constants.ApplicationPages.ClientDetails,
                        client.Id.Value,
                        Constants.ApplicationPages.TimeEntryReport
                    );
            }
            return string.Empty;
        }

        #endregion

        protected void odsTimeEntries_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (e.ReturnValue is TimeEntryRecord[])
            {
                var timeEntryRecords = (TimeEntryRecord[])e.ReturnValue;
                lblRowsNumber.Text = timeEntryRecords == null ?
                    "Error occured" :
                    timeEntryRecords.Length.ToString();
            }
        }

        protected void gvTimeEntries_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)                
            {
                case DataControlRowType.Header:
                    _totalActual = 0d;
                    _totalForecasted = 0d;
                    break;

                case DataControlRowType.DataRow:
                    var item = e.Row.DataItem as TimeEntryRecord;

                    DimDeclinedTimeEntry(e, item);
                    DisableReviewedTeEditing(e, item);
                    AddApprovedVerification(e, item);

                    _totalActual += item.ActualHours;
                    _totalForecasted += item.ForecastedHours;
                    break;

                case DataControlRowType.Footer:
                    ((Label) e.Row.FindControl(LblTotalActual)).Text = _totalActual.ToString(Constants.Formatting.DoubleFormat);
                    ((Label) e.Row.FindControl(LblTotalForecasted)).Text = _totalForecasted.ToString(Constants.Formatting.DoubleFormat);
                    break;
            }
        }

        private static void AddApprovedVerification(GridViewRowEventArgs e, TimeEntryRecord item)
        {
            var lnkEdit = e.Row.FindControl("lnkEdit") as ImageButton;
            var lnkDelete = e.Row.FindControl("btnRemove") as ImageButton;

            if (item.IsReviewed == ReviewStatus.Approved)
            {
                if (lnkEdit != null)
                {
                    lnkEdit.Attributes.Add("onclick", "return notifyReviewed('change')");
                }

                if (lnkDelete != null)
                {
                    lnkDelete.Attributes.Add("onclick", "return notifyReviewed('delete')");
                }
            }
            else
            {
                if (lnkDelete != null)
                {
                    lnkDelete.Attributes.Add("onclick", "return confirm('Are you sure you want to delete this time entry?')");
                }
            }
                
        }

        private void DisableReviewedTeEditing(GridViewRowEventArgs e, TimeEntryRecord item)
        {
            if (!GetEditingAllowed(item))
                e.Row.Cells[0].Enabled = false;
        }

        private static void DimDeclinedTimeEntry(GridViewRowEventArgs e, TimeEntryRecord item)
        {
            if (item.IsReviewed == ReviewStatus.Declined)
                e.Row.CssClass = Constants.CssClassNames.DIMMED_ROW;
        }

        protected void btnRemove_Command(object sender, CommandEventArgs e)
        {
            odsTimeEntries.DeleteParameters[Constants.MethodParameterNames.ID].DefaultValue = e.CommandArgument.ToString();
            odsTimeEntries.Delete();
        }

        #region Icon buttons

        protected void IconButtonClicked(object sender, ImageClickEventArgs e)
        {
            var button = sender as IMultistateImageButton;

            if (button == null) return;

            var timeEntry = new TimeEntryRecord { Id = button.EntityId };
            switch (button.EntityName)
            {
                case Constants.EntityNames.IsChargeableEntity:
                    TimeEntryHelper.ToggleIsChargeable(timeEntry);
                    //var chanrgBtn = sender as IsChargeableImageButton;
                    //chanrgBtn.State = !chanrgBtn.State;
                    break;

                case Constants.EntityNames.IsCorrectEntity:
                    TimeEntryHelper.ToggleIsCorrect(timeEntry);
                    //var corrBtn = sender as IsCorrectImageButton;
                    //corrBtn.State = !corrBtn.State;
                    break;

                case Constants.EntityNames.ReviewStatusEntity:
                    TimeEntryHelper.ToggleReviewStatus(timeEntry);
                    //var revBtn = sender as ReviewStatusImageButton;
                    //revBtn.State = 
                    //    TimeEntryRecord.GetNextReviewStatus(revBtn.State);
                    break;
            }

            gvTimeEntries.DataBind();
        }

        #endregion

        private void ResetFiltersAndSorting()
        {
            tfMain.ResetFilters();
            hfSortingSync.Value = string.Empty;
        }

        protected void tfMain_OnResetAllFilters(object sender, EventArgs e)
        {
            ResetFiltersAndSorting();
            UpdateGrid();
        }

        protected void tfMain_OnUpdate(object sender, EventArgs e)
        {
            UpdateGrid();
        }

        public TimeEntrySelectContext TimeEntrySelectContext
        {
            get
            {
                return new TimeEntrySelectContext
                           {
                               PersonIds = tfMain.PersonIds,
                               MilestoneDateFrom = tfMain.MilestoneDateFrom,
                               MilestoneDateTo = tfMain.MilestoneDateTo,
                               ForecastedHoursFrom = tfMain.ForecastedHoursFrom,
                               ForecastedHoursTo = tfMain.ForecastedHoursTo,
                               ActualHoursFrom = tfMain.ActualHoursFrom,
                               ActualHoursTo = tfMain.ActualHoursTo,
                               ProjectIds = tfMain.ProjectIds,
                               MilestoneId = tfMain.MilestoneId,
                               TimeTypeId = tfMain.TimeTypeId,
                               Notes = tfMain.Notes,
                               IsChargable = tfMain.IsChargable,
                               IsCorrect = tfMain.IsCorrect,
                               IsReviewed = string.IsNullOrEmpty(tfMain.IsReviewed) ? null : tfMain.IsReviewed,
                               EntryDateFrom = tfMain.EntryDateFrom,
                               EntryDateTo = tfMain.EntryDateTo,
                               ModifiedDateFrom = tfMain.ModifiedDateFrom,
                               ModifiedDateTo = tfMain.ModifiedDateTo,
                               SortExpression = MultisortExtender.GetSortString(hfSortingSync.Value),
                               RequesterId = DataHelper.CurrentPerson.Id.Value,
                               IsProjectChargeable = tfMain.IsProjectChargeable,
                               PageNo = gvTimeEntries.PageIndex,
                               PageSize = gvTimeEntries.PageSize
                           };
            }
        }
    }
}

