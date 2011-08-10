﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using DataTransferObjects.TimeEntry;
using PraticeManagement.Controls;
using PraticeManagement.MilestoneService;
using PraticeManagement.ProjectService;
using PraticeManagement.Security;
using PraticeManagement.Utils;
using System.Web.UI.HtmlControls;
using Resources;
using System.Reflection;

namespace PraticeManagement
{
    public partial class MilestoneDetail : PracticeManagementPageBase, IPostBackEventHandler
    {
        #region Constants

        private const string ProjectIdArgument = "projectId";
        private const int FirstMonthColumn = 5;
        private const string TotalHoursHeaderText = "Total Hours";
        private const int TotalRows = 10;
        private const int MilestoneNameLength = 55;

        private decimal _totalAmount;
        private decimal _totalReimbursed;
        private decimal _totalReimbursementAmount;
        private int _expensesCount;

        private const string LblTotalamount = "lblTotalAmount";
        private const string LblTotalreimbursement = "lblTotalReimbursed";
        private const string LblTotalreimbursementamount = "lblTotalReimbursementAmount";

        #endregion

        #region Fields

        private Project projectValue;
        private Milestone milestoneValue;

        protected int prevMilestoneId = -1;
        protected int nextMilestoneId = -1;

        private SeniorityAnalyzer seniorityAnalyzer;
        private MilestonePerson[] _milestonePersons;

        #endregion

        #region Properties

        public SeniorityAnalyzer PersonListSeniorityAnalyzer
        {
            get
            {
                return seniorityAnalyzer;
            }
        }

        private Project Project
        {
            get
            {
                if (projectValue == null)
                {
                    using (var serviceClient = new ProjectServiceClient())
                    {
                        try
                        {
                            projectValue =
                                serviceClient.GetProjectDetailWithoutMilestones(
                                    SelectedProjectId.Value, User.Identity.Name);
                        }
                        catch (FaultException<ExceptionDetail>)
                        {
                            serviceClient.Abort();
                            throw;
                        }
                    }

                }

                return projectValue;
            }
            set
            {
                projectValue = value;
            }
        }

        public Milestone Milestone
        {
            get
            {
                if (milestoneValue == null && MilestoneId.HasValue)
                {
                    if (ViewState["MileStone"] != null)
                        return ViewState["MileStone"] as Milestone;

                    milestoneValue = GetMilestoneById(MilestoneId);

                    ViewState["MileStone"] = milestoneValue;
                }

                return milestoneValue;
            }
        }

        private Milestone GetMilestoneById(int? id)
        {
            if (id.HasValue)
            {
                using (var serviceClient = new MilestoneServiceClient())
                {
                    try
                    {
                        milestoneValue = serviceClient.GetMilestoneDetail(id.Value);

                        Generic.RedirectIfNullEntity(milestoneValue, Response);

                        ViewState["MileStone"] = milestoneValue;
                    }
                    catch (FaultException<ExceptionDetail>)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }

                if (milestoneValue != null)
                {
                    milestoneValue.ActualActivity =
                        new List<TimeEntryRecord>(TimeEntryHelper.GetTimeEntriesForMilestone(milestoneValue));
                }
            }

            return milestoneValue;
        }

        protected int? SelectedProjectId
        {
            get
            {
                return GetArgumentInt32(ProjectIdArgument);
            }
        }

        private int? MilestoneId
        {
            get
            {
                if (SelectedId.HasValue)
                {
                    return SelectedId;
                }
                else
                {
                    int id;
                    if (Int32.TryParse(hdnMilestoneId.Value, out id))
                    {
                        return id;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            set
            {
                hdnMilestoneId.Value = value.ToString();
            }
        }

        public bool IsShowResources
        {
            get
            {
                return MilestoneId.HasValue;
            }
        }

        public PraticeManagement.Controls.Milestones.MilestonePersonList MilestonePersonEntryListControlObject
        {
            get
            {
                return MilestonePersonEntryListControl;
            }
        }

        #endregion

        protected void Page_Init(object sender, EventArgs e)
        {
            if (MilestoneId.HasValue)
            {
                seniorityAnalyzer = new SeniorityAnalyzer(DataHelper.CurrentPerson);
                activityLog.MilestoneId = MilestoneId;
                if (Milestone != null)
                {
                    PersonListSeniorityAnalyzer.OneWithGreaterSeniorityExists(
                            GetMilestonePersons(Milestone)
                            );
                }
                InitPrevNextButtons();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "SetTooltipsForallDropDowns", "SetTooltipsForallDropDowns();", true);
            
            if (!string.IsNullOrEmpty(hdnEditRowIndex.Value))
            {
                SelectView(btnResources, 2, false);
                LoadActiveTabIndex(2);
                MilestonePersonEntryListControl.OnEditClick(Convert.ToInt32(hdnEditRowIndex.Value));
                hdnEditRowIndex.Value = string.Empty;
            }
        }

        protected void cstCheckStartDateForExpensesExistance_OnServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (MilestoneId.HasValue && Milestone != null && Milestone.StartDate < dtpPeriodFrom.DateValue
                && Milestone.Project.StartDate == Milestone.StartDate)
            {
                using (var service = new MilestoneServiceClient())
                {
                    args.IsValid = !(service.CheckIfExpensesExistsForMilestonePeriod(MilestoneId.Value, dtpPeriodFrom.DateValue, null));
                }
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void cstCheckEndDateForExpensesExistance_OnServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (MilestoneId.HasValue && Milestone != null && Milestone.EndDate > dtpPeriodTo.DateValue
                && Milestone.Project.EndDate == Milestone.EndDate)
            {
                using (var service = new MilestoneServiceClient())
                {
                    args.IsValid = !(service.CheckIfExpensesExistsForMilestonePeriod(MilestoneId.Value, null, dtpPeriodTo.DateValue));
                }
            }
            else
            {
                args.IsValid = true;
            }
        }

        /// <summary>
        /// Initializes 'Previous Milestone' and 'Next Milestone' buttons
        /// </summary>
        protected void InitPrevNextButtons()
        {
            if (Milestone.Project.Id.HasValue && MilestoneId.HasValue)
            {
                var projectMiles = Project.Milestones.ToArray();

                var projMilesNum = projectMiles.Length;
                if (projMilesNum > 1)
                {
                    int mIndex = Array.FindIndex(projectMiles, v => v.Id == MilestoneId.Value);

                    if (mIndex > 0)
                        InitLink(projectMiles[mIndex - 1], lnkPrevMilestone, divLeft, captionLeft, lblLeft, ref prevMilestoneId);

                    if (mIndex < projMilesNum - 1)
                        InitLink(projectMiles[mIndex + 1], lnkNextMilestone, divRight, captionRight, lblRight, ref nextMilestoneId);
                }
                else
                {
                    divPrevNextMainContent.Visible = false;
                }
            }
        }

        private void InitLink(
            Milestone milestone,
            HyperLink hlink,
            HtmlGenericControl div,
            HtmlGenericControl span,
            HtmlGenericControl label,
            ref int milestoneId)
        {
            milestoneId = milestone.Id.Value;
            div.Visible = true;
            hlink.NavigateUrl = MilestoneRedirrectUrl(milestoneId);
            hlink.Attributes.Add("onclick", "javascript:checkDirty(\"" + milestoneId + "\")");

            span.InnerText = milestone.Description;
            label.InnerText
                = string.Format(
                    "({0} - {1})",
                    milestone.StartDate.ToString(Constants.Formatting.EntryDateFormat),
                    milestone.EndDate.ToString(Constants.Formatting.EntryDateFormat));

        }

        /// <summary>
        /// Emits a JavaScript which prevent the loss of non-saved data.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            // New record adding
            AllowContinueWithoutSave = MilestoneId.HasValue;
            btnDelete.Enabled = MilestoneId.HasValue;

            // Move milestone fields visibility
            pnlMoveMilestone.Visible = MilestoneId.HasValue;

            // Clone milestone fields visibility
            pnlCloneMilestone.Visible = MilestoneId.HasValue && SelectedProjectId.HasValue;
        }

        protected void gvPeople_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var row = e.Row;
            var milestonePerson = row.DataItem as MilestonePerson;

            if (row.RowType == DataControlRowType.DataRow && milestonePerson != null)
            {
                if (milestonePerson.Person != null && milestonePerson.Person.ProjectedFinancialsByMonth != null &&
                    milestonePerson.Person.ProjectedFinancialsByMonth.Count > 0)
                {
                    var isOtherGreater = PersonListSeniorityAnalyzer.IsOtherGreater(milestonePerson.Person);

                    var dtTemp =
                        new DateTime(milestonePerson.Milestone.StartDate.Year, milestonePerson.Milestone.StartDate.Month,
                                     1);
                    // Filling monthly workload for the person.
                    int currentColumnIndex = FirstMonthColumn;
                    for (;
                        dtTemp <= milestonePerson.Milestone.ProjectedDeliveryDate;
                        currentColumnIndex++, dtTemp = dtTemp.AddMonths(1))
                    {
                        // The person works on the milestone at the month - has some workload
                        foreach (KeyValuePair<DateTime, ComputedFinancials> financials in
                            milestonePerson.Person.ProjectedFinancialsByMonth)
                        {
                            // Find a record for the month we need for the column
                            if (financials.Key.Month == dtTemp.Month && financials.Key.Year == dtTemp.Year)
                                row.Cells[currentColumnIndex].Text =
                                    string.Format(Resources.Controls.MilestoneInterestFormat,
                                                  financials.Value.Revenue,
                                                  isOtherGreater
                                                      ? Resources.Controls.HiddenCellText
                                                      : financials.Value.GrossMargin.ToString(),
                                                  financials.Value.HoursBilled);
                        }
                    }

                    int marginColumnIndex = currentColumnIndex + 2;
                    int grossMargin = currentColumnIndex + 4;

                    if (isOtherGreater)
                    {
                        row.Cells[marginColumnIndex].Text = Resources.Controls.HiddenCellText;
                        row.Cells[grossMargin].Text = Resources.Controls.HiddenCellText;
                    }
                }

                if (milestonePerson.StartDate >= Milestone.EndDate)
                {
                    row.BackColor = Color.FromArgb(0xff, 0xe6, 0xe0);
                    lblError.ShowErrorMessage(Messages.PersonsStartGreaterMilestoneStart);
                }


                var imgEdit = row.FindControl("imgEdit") as ImageButton;

                if (imgEdit != null)
                {
                    imgEdit.Attributes.Add("RowIndex", row.RowIndex.ToString());
                }


            }
        }

        #region Validation

        protected void cvMilestoneName_Validate(object sender, ServerValidateEventArgs e)
        {
            int length = txtMilestoneName.Text.Length;

            e.IsValid = (length <= MilestoneNameLength);
        }

        protected void Revenue_CheckedChanged(object sender, EventArgs e)
        {
            UpdateRevenueState();
            LoadActiveTabIndex(mvMilestoneDetailTab.ActiveViewIndex);
        }

        #endregion

        #region Preventing dirty loss

        protected void dtpPeriod_SelectionChanged(object sender, EventArgs e)
        {
            IsDirty = true;
        }

        #endregion

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Page.Validate(vsumMilestone.ValidationGroup);
            if (Page.IsValid)
            {
                int? id = SaveData();

                if (id.HasValue)
                {
                    MilestoneId = id;
                    GetMilestoneById(MilestoneId);
                    MilestonePersonEntryListControlObject.GetLatestData();
                }

                lblResult.ShowInfoMessage(Messages.MilestoneSavedMessage);
                ClearDirty();
            }
            else
            {
                lblResult.ClearMessage();
            }

            if (Page.IsValid && MilestoneId.HasValue)
            {
                if (Milestone != null)
                {
                    Project = Milestone.Project;
                    PopulateControls(Milestone, SelectedId.HasValue ? true : false);
                }
            }

            LoadActiveTabIndex(mvMilestoneDetailTab.ActiveViewIndex);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (Milestone.Project.StartDate.Value == Milestone.StartDate ||
                Milestone.Project.EndDate.Value == Milestone.EndDate)
            {
                using (var service = new MilestoneServiceClient())
                {
                    if (service.CheckIfExpensesExistsForMilestonePeriod(MilestoneId.Value, null, null))
                    {
                        lblError.ShowErrorMessage("This milestone cannot be deleted, because project has expenses during the milestone period.");
                        return;
                    }
                }
            }
            try
            {
                DeleteRecord();
                ReturnToPreviousPage();
            }
            catch (Exception exception)
            {
                lblError.ShowErrorMessage("{0}", exception.Message);
            }
        }

        protected void btnMoveMilestone_Click(object sender, EventArgs e)
        {
            lblResult.ClearMessage();
            Page.Validate(vsumShiftDays.ValidationGroup);
            if (Page.IsValid)
            {
                var shiftDays = int.Parse(txtShiftDays.Text);
                var newStartDate = Milestone.StartDate.AddDays(shiftDays);
                var newEndDate = Milestone.EndDate.AddDays(shiftDays);
                if (Milestone.Project.StartDate.Value == Milestone.StartDate)
                {
                    using (var service = new MilestoneServiceClient())
                    {
                        if (service.CheckIfExpensesExistsForMilestonePeriod(MilestoneId.Value, newStartDate, null))
                        {
                            lblError.ShowErrorMessage("This milestone cannot be moved because the project has expenses earlier than new start date.\nPlease change the expenses first.");
                            return;
                        }
                    }
                }
                if (Milestone.Project.EndDate.Value == Milestone.EndDate)
                {
                    using (var service = new MilestoneServiceClient())
                    {
                        if (service.CheckIfExpensesExistsForMilestonePeriod(MilestoneId.Value, newStartDate, null))
                        {
                            lblError.ShowErrorMessage("This milestone cannot be moved because the project has expenses beyond new end date.\nPlease change the expenses first.");
                            return;
                        }
                    }
                }

                if (shiftDays < 0)
                {
                    using (var service = new MilestoneServiceClient())
                    {
                        if (!service.CanMoveFutureMilestones(MilestoneId.Value, shiftDays))
                        {
                            lblError.ShowErrorMessage("Cannot move future milestones because it leads to change in its project end date, but the project has expenses beyond new end date.\n Please change expenses first.");
                            return;
                        }
                    }
                }

                DataHelper.ShiftMilestone(
                    shiftDays,
                    MilestoneId.Value,
                    chbMoveFutureMilestones.Checked);
                ReturnToPreviousPage();
            }
        }

        protected void btnClone_Click(object sender, EventArgs e)
        {
            Page.Validate(vsumClone.ValidationGroup);
            if (Page.IsValid)
            {
                int cloneDuration = int.Parse(txtCloneDuration.Text);
                using (MilestoneServiceClient serviceClient = new MilestoneServiceClient())
                {
                    try
                    {
                        int cloneId = serviceClient.MilestoneClone(MilestoneId.Value, cloneDuration);

                        Redirect(string.Concat(
                            string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                            Constants.ApplicationPages.MilestoneDetail,
                            cloneId), "&projectId=", SelectedProjectId.Value.ToString()));
                    }
                    catch (CommunicationException)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }
            }
        }

        private void SaveAndRedirect(object args)
        {
            if (!SaveDirty || ValidateAndSave())
            {
                int mpId;
                var isInt = int.TryParse(args.ToString(), out mpId);

                if (isInt)
                    Redirect(string.Format(Constants.ApplicationPages.RedirectMilestonePersonIdFormat,
                                           Constants.ApplicationPages.MilestonePersonDetail,
                                           MilestoneId.Value,
                                           args));
                else
                    Redirect(args.ToString());
            }
        }

        protected string GetMpeRedirectUrl(object args)
        {
            var mpePageUrl =
                string.Format(
                       Constants.ApplicationPages.RedirectMilestonePersonIdFormat,
                       Constants.ApplicationPages.MilestonePersonDetail,
                       MilestoneId.Value,
                       args);

            return Generic.GetTargetUrlWithReturn(mpePageUrl, Request.Url.AbsoluteUri);
        }

        protected override bool ValidateAndSave()
        {
            bool result = false;
            Page.Validate(vsumMilestone.ValidationGroup);
            if (Page.IsValid)
            {
                result = SaveData() > 0;
            }
            return result;
        }

        protected void nMilestone_OnNoteAdded(object source, EventArgs args)
        {
            activityLog.Update();
        }

        protected override void Display()
        {
            int? id = MilestoneId;
            if (id.HasValue)
            {
                if (Milestone != null)
                {
                    Project = Milestone.Project;
                    PopulateControls(Milestone, true);
                    lblResult.ClearMessage();
                }
            }
            else
            {
                tblMilestoneDetailTabViewSwitch.Visible = false;
                mvMilestoneDetailTab.Visible = false;

                // Creating a new record
                Project project = this.Project;

                if (project != null)
                {
                    PopulateProjectControls(project);

                    dtpPeriodFrom.DateValue = project.MilestoneDefaultStartDate;
                    DateTime dtPeriodTo = project.MilestoneDefaultStartDate;
                    dtpPeriodTo.DateValue = dtPeriodTo.AddDays(1);
                    chbIsChargeable.Checked = project.IsChargeable;
                }

                chbConsultantsCanAdjust.Checked = false;

                gvPeople.DataBind();
            }

            UpdateRevenueState();

            if (!IsPostBack && mvMilestoneDetailTab.ActiveViewIndex == 0 && Milestone != null)
            {
                PopulatePeopleGrid();
            }
        }

        private IEnumerable<Person> GetMilestonePersons(DataTransferObjects.Milestone Milestone)
        {
            foreach (var milestonePerson in Milestone.MilestonePersons)
            {
                yield return milestonePerson.Person;
            }
        }

        private void PopulatePeopleGrid()
        {
            if (Milestone == null)
                return;

            _milestonePersons = Milestone.MilestonePersons.OrderBy(mp => mp.Entries[0].ThisPerson.LastName).ThenBy(mp => mp.StartDate).AsQueryable().ToArray();

            if (_milestonePersons.Length > 0)
            {
                DateTime dtTemp =
                    new DateTime(_milestonePersons[0].Milestone.StartDate.Year, _milestonePersons[0].Milestone.StartDate.Month, 1);

                // Create the columns for the milestone months
                for (int i = FirstMonthColumn;
                    dtTemp <= _milestonePersons[0].Milestone.ProjectedDeliveryDate;
                    i++, dtTemp = dtTemp.AddMonths(1))
                {
                    var column = new BoundField
                                     {
                                         HeaderText = Resources.Controls.TableHeaderOpenTag +
                                                     dtTemp.ToString(Constants.Formatting.MonthYearFormat) +
                                                     Resources.Controls.TableHeaderCloseTag,
                                         HtmlEncode = false
                                     };
                    gvPeople.Columns.Insert(i, column);
                }
            }

            gvPeople.DataSource = _milestonePersons;
            gvPeople.DataBind();

            if (gvPeople.FooterRow != null)
            {
                for (int i = 0; i < gvPeople.FooterRow.Cells.Count - 2; i++)
                {
                    gvPeople.FooterRow.Cells[i].RowSpan = TotalRows;
                }

                if (_milestonePersons != null && _milestonePersons.Length > 0)
                {
                    // Totals by months
                    DateTime dtTemp =
                        new DateTime(_milestonePersons[0].Milestone.StartDate.Year, _milestonePersons[0].Milestone.StartDate.Month, 1);
                    DateTime dtEnd = _milestonePersons[0].Milestone.ProjectedDeliveryDate;

                    Person currentPerson = DataHelper.CurrentPerson;
                    for (int i = FirstMonthColumn; dtTemp <= dtEnd; i++, dtTemp = dtTemp.AddMonths(1))
                    {
                        SeniorityAnalyzer sa = new SeniorityAnalyzer(currentPerson);

                        ComputedFinancials financials = new ComputedFinancials();

                        bool oneGreaterExists = false;
                        foreach (MilestonePerson milestonePerson in _milestonePersons)
                        {
                            bool isOtherGreater = sa.IsOtherGreater(milestonePerson.Person);
                            if (isOtherGreater)
                                oneGreaterExists = true;

                            var financialsByMonth = milestonePerson.Person.ProjectedFinancialsByMonth;
                            if (financialsByMonth != null)
                            {
                                foreach (KeyValuePair<DateTime, ComputedFinancials> tmpFinancials in financialsByMonth)
                                {
                                    // Serch for the computed financials for the month
                                    if (tmpFinancials.Key.Month == dtTemp.Month &&
                                        tmpFinancials.Key.Year == dtTemp.Year)
                                    {
                                        financials.Revenue += tmpFinancials.Value.Revenue;
                                        financials.GrossMargin += tmpFinancials.Value.GrossMargin;
                                        financials.HoursBilled += tmpFinancials.Value.HoursBilled;
                                        break;
                                    }
                                }
                            }
                        }

                        if (financials.HoursBilled > 0)
                        {
                            gvPeople.FooterRow.Cells[i].Font.Bold = true;
                            gvPeople.FooterRow.Cells[i].Text =
                                string.Format(Resources.Controls.MilestoneSummaryInterestFormat,
                                financials.Revenue,
                                oneGreaterExists ? Resources.Controls.HiddenCellText : financials.GrossMargin.ToString(),
                                financials.HoursBilled,
                                oneGreaterExists ? Resources.Controls.HiddenCellText : financials.TargetMargin.ToString("##0.00"));
                        }
                    }
                }

                // Calculate and display totals
                if (Milestone.ComputedFinancials != null)
                {
                    lblTotalCogs.Text =
                        PersonListSeniorityAnalyzer.GreaterSeniorityExists ?
                        Resources.Controls.HiddenCellText : Milestone.ComputedFinancials.Cogs.ToString();
                    lblTotalRevenue.Text = Milestone.ComputedFinancials.Revenue.ToString();
                    lblTotalRevenueNet.Text = Milestone.ComputedFinancials.RevenueNet.ToString();

                    lblClientDiscount.Text = Milestone.Project.Discount.ToString("##0.00");

                    lblClientDiscountAmount.Text =
                        (Milestone.ComputedFinancials.Revenue - Milestone.ComputedFinancials.RevenueNet).ToString();

                    // Sales commission
                    lblSalesCommissionPercentage.Text =
                        PersonListSeniorityAnalyzer.GreaterSeniorityExists ?
                        Resources.Controls.HiddenCellText :
                        Project.SalesCommission.Sum(commission => commission.FractionOfMargin).ToString("##0.00");
                }
            }
        }

        private void SetFooterLabelText(string labelValue, string labelId)
        {
            var lbl = this.FindControl(labelId) as Label;
            //gvPeople.FooterRow != null ? gvPeople.FooterRow.FindControl(labelId) as Label : null;

            if (lbl != null)
                lbl.Text = labelValue;
        }

        private void UpdateRevenueState()
        {
            txtFixedRevenue.Enabled = reqFixedRevenue.Enabled = compFixedRevenue.Enabled =
                rbtnFixedRevenue.Checked;
        }

        private int SaveData()
        {
            Milestone milestone = new Milestone();
            PopulateData(milestone);

            using (MilestoneServiceClient serviceClient = new MilestoneServiceClient())
            {
                try
                {
                    return serviceClient.SaveMilestoneDetail(milestone, User.Identity.Name);
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        private void DeleteRecord()
        {
            var milestone = new Milestone { Id = MilestoneId };

            using (var serviceClient = new MilestoneServiceClient())
            {
                try
                {
                    serviceClient.DeleteMilestone(milestone, User.Identity.Name);
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        private void PopulateControls(Milestone milestone, bool fillComputedFinancials)
        {
            txtMilestoneName.Text = milestone.Description;
            dtpPeriodFrom.DateValue = milestone.StartDate;
            dtpPeriodTo.DateValue = milestone.ProjectedDeliveryDate;
            rbtnFixedRevenue.Checked = !milestone.IsHourlyAmount;
            rbtnHourlyRevenue.Checked = milestone.IsHourlyAmount;
            chbIsChargeable.Checked = milestone.IsChargeable;
            chbConsultantsCanAdjust.Checked = milestone.ConsultantsCanAdjust;

            if (rbtnFixedRevenue.Checked)
                txtFixedRevenue.Text = milestone.Amount.Value.Value.ToString();

            PopulateProjectControls(milestone.Project);

            if (fillComputedFinancials)
                FillComputedFinancials(milestone);

            SetControlsChangebility();
        }

        private void FillComputedFinancials(Milestone milestone)
        {
            //Fill Projected Sales Commission Cell
            SetFooterLabelWithSeniority(
                milestone.ComputedFinancials == null ? string.Empty : milestone.ComputedFinancials.SalesCommission.ToString(),
                lblProjectedSalesCommission);

            //Fill Practice Manager Commission Cell
            SetFooterLabelWithSeniority(
                milestone.ComputedFinancials == null ? string.Empty :
                milestone.ComputedFinancials.PracticeManagementCommission.ToString(),
                lblPracticeManagerCommission);

            //Fill Total Margin Cell
            SetFooterLabelWithSeniority(
                milestone.ComputedFinancials == null ? string.Empty : milestone.ComputedFinancials.GrossMargin.ToString(),
                lblTotalMargin);

            //Fill Target Margin Cell
            SetFooterLabelWithSeniority(
                milestone.ComputedFinancials == null ? string.Empty :
                        string.Format(Constants.Formatting.PercentageFormat, milestone.ComputedFinancials.TargetMargin),
                lblTargetMargin);

            if (milestone.Project.Client.Id.HasValue && milestone != null && milestone.ComputedFinancials != null)
            {
                SetBackgroundColorForMargin(milestone.Project.Client.Id.Value, milestone.ComputedFinancials.TargetMargin, milestone.Project.Client.IsMarginColorInfoEnabled);
            }

            if (Milestone.ComputedFinancials != null)
            {
                lblTotalCogs.Text =
                    PersonListSeniorityAnalyzer.GreaterSeniorityExists ?
                    Resources.Controls.HiddenCellText : Milestone.ComputedFinancials.Cogs.ToString();
                lblTotalRevenue.Text = Milestone.ComputedFinancials.Revenue.ToString();
                lblTotalRevenueNet.Text = Milestone.ComputedFinancials.RevenueNet.ToString();

                lblClientDiscountAmount.Text =
                    (Milestone.ComputedFinancials.Revenue - Milestone.ComputedFinancials.RevenueNet).ToString();

                lblClientDiscount.Text = Milestone.Project.Discount.ToString("##0.00");

                // Sales commission
                lblSalesCommissionPercentage.Text =
                    PersonListSeniorityAnalyzer.GreaterSeniorityExists ?
                    Resources.Controls.HiddenCellText :
                    Project.SalesCommission.Sum(commission => commission.FractionOfMargin).ToString("##0.00");
            }

            //Fill Final Milestone Margin Cell
            SetFooterLabelWithSeniority(
                milestone.ComputedFinancials == null ? string.Empty :
                        milestone.ComputedFinancials.MarginNet.ToString(),
                lblFinalMilestoneMargin);

            //Fill Expenses Cell
            SetFooterLabelWithSeniority(
                milestone.ComputedFinancials == null ? string.Empty :
                        ((PracticeManagementCurrency)milestone.ComputedFinancials.Expenses).ToString(),
                lblExpenses);

            //Fill Final Milestone Margin Cell
            SetFooterLabelWithSeniority(
                milestone.ComputedFinancials == null ? string.Empty :
                        ((PracticeManagementCurrency)milestone.ComputedFinancials.ReimbursedExpenses).ToString(),
                lblReimbursedExpenses);
        }

        private void SetBackgroundColorForMargin(int clientId, decimal targetMargin, bool? individualClientMarginColorInfoEnabled)
        {
            int margin = (int)targetMargin;
            List<ClientMarginColorInfo> cmciList = new List<ClientMarginColorInfo>();

            if (individualClientMarginColorInfoEnabled.HasValue && individualClientMarginColorInfoEnabled.Value)
            {
                cmciList = DataHelper.GetClientMarginColorInfo(clientId);
            }
            else if (Convert.ToBoolean(SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.IsDefaultMarginInfoEnabledForAllClientsKey)))
            {
                cmciList = SettingsHelper.GetMarginColorInfoDefaults(DefaultGoalType.Client);
            }

            if (cmciList != null)
            {
                foreach (var item in cmciList)
                {
                    if (margin >= item.StartRange && margin <= item.EndRange)
                    {
                        tdTargetMargin.Style["background-color"] = item.ColorInfo.ColorValue;
                        break;
                    }
                }
            }
        }

        private void SetControlsChangebility()
        {
            // Security
            var isReadOnly =
                !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName) &&
                !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SalespersonRoleName) &&
                !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.PracticeManagerRoleName) &&
                !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName);// #2817: DirectorRoleName is added as per the requirement.

            txtMilestoneName.ReadOnly = dtpPeriodFrom.ReadOnly =
                dtpPeriodTo.ReadOnly = txtFixedRevenue.ReadOnly = isReadOnly;

            rbtnFixedRevenue.Enabled = rbtnHourlyRevenue.Enabled = !isReadOnly;
            btnSave.Visible = btnDelete.Visible = btnMoveMilestone.Visible = !isReadOnly;
        }

        private void SetFooterLabelWithSeniority(string labelValue, ITextControl label)
        {
            label.Text =
                PersonListSeniorityAnalyzer != null && PersonListSeniorityAnalyzer.GreaterSeniorityExists ?
                    Resources.Controls.HiddenCellText : labelValue;

        }

        private void PopulateProjectControls(Project project)
        {
            pdProjectInfo.Populate(project);

            if (project != null && gvPeople.FooterRow != null) lblClientDiscount.Text = project.Discount.ToString();
        }

        private void PopulateData(Milestone milestone)
        {
            milestone.Project = new Project { Id = SelectedProjectId };

            milestone.Id = MilestoneId;
            milestone.Description = txtMilestoneName.Text;
            milestone.StartDate = dtpPeriodFrom.DateValue;
            milestone.ProjectedDeliveryDate = dtpPeriodTo.DateValue;
            milestone.IsHourlyAmount = rbtnHourlyRevenue.Checked;
            milestone.Amount =
                rbtnFixedRevenue.Checked ? (decimal?)decimal.Parse(txtFixedRevenue.Text) : null;
            milestone.IsChargeable = chbIsChargeable.Checked;
            milestone.ConsultantsCanAdjust = chbConsultantsCanAdjust.Checked;
        }

        private string MilestoneRedirrectUrl(int milestoneId)
        {
            var redirrectUrl = string.Format(
                Constants.ApplicationPages.MilestonePrevNextRedirectFormat,
                Constants.ApplicationPages.MilestoneDetail,
                milestoneId, Project.Id.Value);

            return Generic.GetTargetUrlWithReturn(redirrectUrl, Request.Url.AbsoluteUri);
        }

        #region Implementation of IPostBackEventHandler

        /// <summary>
        /// When implemented by a class, enables a server control to process an event raised when a form is posted to the server.
        /// </summary>
        /// <param name="eventArgument">A <see cref="T:System.String"/> that represents an optional event argument 
        /// to be passed to the event handler. </param>
        public void RaisePostBackEvent(string eventArgument)
        {
            SaveAndRedirect(eventArgument);
        }

        #endregion

        protected void gvPeople_OnDataBound(object sender, EventArgs e)
        {
            gvPeople.FooterStyle.BackColor =
                gvPeople.Rows.Count % 2 == 0 ?
                    Color.White : Color.FromArgb(0xf9, 0xfa, 0xff);
        }

        protected void btnView_Command(object sender, CommandEventArgs e)
        {
            int viewIndex = int.Parse((string)e.CommandArgument);
            SelectView((Control)sender, viewIndex, false);

            LoadActiveTabIndex(viewIndex);
        }

        protected void odsMilestoneExpenses_OnSelecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (MilestoneId.HasValue)
            {
                e.InputParameters["milestoneId"] = MilestoneId.Value;
            }
        }

        protected void gvMilestoneExpenses_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            var row = e.Row;
            switch (row.RowType)
            {
                case DataControlRowType.DataRow:
                    var expense = row.DataItem as ProjectExpense;

                    if (expense != null)
                    {
                        _totalAmount += expense.Amount;
                        _totalReimbursed += expense.Reimbursement;
                        _totalReimbursementAmount += expense.ReimbursementAmount;

                        _expensesCount++;

                        // Hide rows with null values.
                        // These are special rows that are used not to show
                        //      empty data grid message
                        if (!expense.Id.HasValue)
                            row.Visible = false;
                    }

                    break;

                case DataControlRowType.Footer:
                    SetRowValue(row, LblTotalamount, _totalAmount);
                    SetRowValue(row, LblTotalreimbursement, string.Format("{0:0}%", (_totalReimbursed / _expensesCount)));
                    SetRowValue(row, LblTotalreimbursementamount, _totalReimbursementAmount);

                    break;
            }
        }
        private static void SetRowValue(Control row, string ctrlName, decimal number)
        {
            SetRowValue(row, ctrlName, ((PracticeManagementCurrency)number).ToString());
        }

        private static void SetRowValue(Control row, string ctrlName, string text)
        {
            var totalAmountCtrl = row.FindControl(ctrlName) as Label;
            if (totalAmountCtrl != null)
                totalAmountCtrl.Text = text;
        }

        private void LoadActiveTabIndex(int viewIndex)
        {
            switch (viewIndex)
            {
                case 0: PopulatePeopleGrid(); break;
                case 1: PopulatePeopleGrid(); break;
                case 4: mpaDaily.ActivityPeriod = Milestone; break;
                case 5: mpaCumulative.ActivityPeriod = Milestone; break;
                case 6: mpaTotal.ActivityPeriod = Milestone; break;
                case 7: activityLog.Update(); break;
            }
        }

        private void SelectView(Control sender, int viewIndex, bool selectOnly)
        {
            mvMilestoneDetailTab.ActiveViewIndex = viewIndex;

            foreach (TableCell cell in tblMilestoneDetailTabViewSwitch.Rows[0].Cells)
            {
                cell.CssClass = string.Empty;
            }

            ((WebControl)sender.Parent).CssClass = "SelectedSwitch";
        }

        protected bool GetVisibleValue()
        {
            return MilestoneId.HasValue;
        }
    }
}

