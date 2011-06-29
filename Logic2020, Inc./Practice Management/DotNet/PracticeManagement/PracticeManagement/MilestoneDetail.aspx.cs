using System;
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

namespace PraticeManagement
{
    public partial class MilestoneDetail : PracticeManagementPageBase, IPostBackEventHandler
    {
        #region Constants

        private const string ProjectIdArgument = "projectId";
        private const int FirstMonthColumn = 5;
        private const string TotalHoursHeaderText = "Total Hours";
        private const int TotalRows = 10;

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
            }
        }

        #region Validation

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
            Page.Validate(vsumShiftDays.ValidationGroup);
            if (Page.IsValid)
            {
                DataHelper.ShiftMilestone(
                    int.Parse(txtShiftDays.Text),
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

        protected void btnAddPerson_Click(object sender, EventArgs e)
        {
            if (!MilestoneId.HasValue)
            {
                // Save a New Record
                Page.Validate(vsumMilestone.ValidationGroup);
                if (Page.IsValid)
                {
                    int milestoneId = SaveData();
                    Redirect(string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                        Constants.ApplicationPages.MilestonePersonDetail, milestoneId), milestoneId.ToString());
                }
            }
            else if (!SaveDirty || ValidateAndSave())
            {
                Redirect(string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                    Constants.ApplicationPages.MilestonePersonDetail, MilestoneId.Value), MilestoneId.Value.ToString());
            }
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

            _milestonePersons = Milestone.MilestonePersons;

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

            if (milestone.Project.Client.Id.HasValue && milestone.Project.Client.IsMarginColorInfoEnabled)
            {
                SetBackgroundColorForMargin(milestone.Project.Client.Id.Value, milestone.ComputedFinancials.TargetMargin);
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

        private void SetBackgroundColorForMargin(int clientId, decimal targetMargin)
        {
            int margin = (int)targetMargin;
            List<ClientMarginColorInfo> cmciList = DataHelper.GetClientMarginColorInfo(clientId);

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
            btnAddPerson.Visible = btnSave.Visible = btnDelete.Visible = btnMoveMilestone.Visible = !isReadOnly;
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

        private void LoadActiveTabIndex(int viewIndex)
        {
            switch (viewIndex)
            {
                case 0: PopulatePeopleGrid(); break;
                case 1: PopulatePeopleGrid(); break;
                case 3: mpaDaily.ActivityPeriod = Milestone; break;
                case 4: mpaCumulative.ActivityPeriod = Milestone; break;
                case 5: mpaTotal.ActivityPeriod = Milestone; break;
                case 6: activityLog.Update(); break;
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

    }
}

