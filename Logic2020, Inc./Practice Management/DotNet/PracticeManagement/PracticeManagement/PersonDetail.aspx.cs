using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using DataTransferObjects.Utils;
using PraticeManagement.Configuration;
using PraticeManagement.Controls;
using PraticeManagement.Events;
using PraticeManagement.PersonService;
using Resources;
using PraticeManagement.Security;
using PraticeManagement.OpportunityService;
using DataTransferObjects.ContextObjects;
using PraticeManagement.Utils;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace PraticeManagement
{
    public partial class PersonDetail : PracticeManagementPageBase, IPostBackEventHandler
    {
        //#region Logger

        //private static readonly ILog Log =
        //    LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //#endregion

        #region Constants

        private const string PersonStatusKey = "PersonStatus";
        private const string ActivePersonStatusId = "1";
        private const string UserNameParameterName = "userName";
        private const string DuplicatePersonName = "There is another Person with the same First Name and Last Name.";
        private const string DuplicateEmail = "There is another Person with the same Email.";

        #endregion

        #region Fields

        private ExceptionDetail _internalException;
        private int _saveCode;
        private bool? _userIsAdministratorValue;
        private bool? _userIsHRValue;

        #endregion Fields

        #region Properties

        private String ExMessage { get; set; }

        public PersonStatusType? PersonStatusId
        {
            get
            {
                PersonStatusType? result =
                    !string.IsNullOrEmpty(ddlPersonStatus.SelectedValue)
                        ?
                            (PersonStatusType?)int.Parse(ddlPersonStatus.SelectedValue)
                        : null;

                return result;
            }
            set
            {
                ddlPersonStatus.SelectedIndex =
                    ddlPersonStatus.Items.IndexOf(
                        ddlPersonStatus.Items.FindByValue(value.HasValue ? ((int)value.Value).ToString() : string.Empty));
            }
        }

        /// <summary>
        /// Gets or sets original person Status ID, need to store it for comparing with new one
        /// </summary>
        public int PrevPersonStatusId
        {
            get
            {
                if (ViewState[PersonStatusKey] == null)
                {
                    ViewState[PersonStatusKey] = -1;
                }
                return (int)ViewState[PersonStatusKey];
            }
            set { ViewState[PersonStatusKey] = value; }
        }

        private int? PersonId
        {
            get
            {
                if (SelectedId.HasValue)
                {
                    return SelectedId;
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.hdnPersonId.Value))
                    {
                        int projectid;
                        if (Int32.TryParse(this.hdnPersonId.Value, out projectid))
                        {
                            return projectid;
                        }
                    }
                    return null;
                }
            }
            set
            {
                this.hdnPersonId.Value = value.ToString();
            }
        }

        /// <summary>
        /// Gets whether the current user is in the Administrator role.
        /// </summary>
        protected bool UserIsAdministrator
        {
            get
            {
                if (!_userIsAdministratorValue.HasValue)
                {
                    _userIsAdministratorValue =
                        Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
                }

                return _userIsAdministratorValue.Value;
            }
        }

        protected bool UserIsHR
        {
            get
            {
                if (!_userIsHRValue.HasValue)
                {
                    _userIsHRValue =
                        Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.HRRoleName);
                }

                return _userIsHRValue.Value;
            }
        }


        public bool UserIsRecruiter
        {
            get { return (bool)ViewState["UserIsRecruiter"]; }
            set { ViewState["UserIsRecruiter"] = value; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.FillPracticeListOnlyActive(ddlDefaultPractice, string.Empty);
                txtFirstName.Focus();
                UserIsRecruiter = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.RecruiterRoleName);
            }

            AllowContinueWithoutSave = cellActivityLog.Visible = PersonId.HasValue;
            personOpportunities.TargetPersonId = PersonId;
            mlError.ClearMessage();
            this.dvTerminationDateErrors.Visible = false;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Security
            chblRoles.Visible =
                btnResetPassword.Visible = locRolesLabel.Visible = chbLockedOut.Visible = UserIsAdministrator || UserIsHR;//#2817 UserisHR is added as per requirement.
            txtEmployeeNumber.ReadOnly = !UserIsAdministrator && !UserIsHR;//#2817 UserisHR is added as per requirement.

            if (!UserIsAdministrator && !UserIsHR && !PersonId.HasValue)//#2817 UserisHR is added as per requirement.
            {
                // Recruiter should not be able to set the person active.
                PersonStatusId = PersonStatusType.Projected;
            }
            recruiterInfo.ReadOnly = !UserIsAdministrator && !UserIsHR;//#2817 UserisHR is added as per requirement.

            //if (!UserIsAdministrator)
            //{
            //    if (!UserIsRecruiter)
            //    {
            //        DisableControls();
            //    }
            //    else if (!IsPostBack)
            //    {
            //        RestrictRecruiterPrivs();
            //    }
            //}

            btnAddDefaultRecruitingCommission.Enabled = UserIsAdministrator || UserIsRecruiter || UserIsHR;//#2817 UserisHR is added as per requirement.
            cellPermissions.Visible = UserIsAdministrator || UserIsHR;//#2817 UserisHR is added as per requirement.
        }

        private void RestrictRecruiterPrivs()
        {
            Person current = DataHelper.CurrentPerson;

            if (PersonStatusId == PersonStatusType.Active || PersonStatusId == PersonStatusType.Terminated ||
                current == null || !current.Id.HasValue ||
                recruiterInfo.RecruiterCommission.Count(
                    commission => commission.RecruiterId == current.Id.Value) == 0)
            {
                DisableControls();
            }
        }

        protected void defaultManager_OnCustomError(object sender, ErrorEventArgs errorEventArgs)
        {
            mlError.ShowErrorMessage("Line manager of this person is not active. Please select another line manager.");
        }

        protected void recruiterInfo_InfoChanged(object sender, EventArgs e)
        {
            IsDirty = true;
        }

        /// <summary>
        /// Navigates to teh person margin page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPersonMargin_Click(object sender, EventArgs e)
        {
            Redirect(
                string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                              Constants.ApplicationPages.PersonMargin,
                              PersonId));
        }

        /// <summary>
        /// Saves the user's input.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            int viewindex = mvPerson.ActiveViewIndex;
            TableCell CssSelectCell = null;
            foreach (TableCell item in tblPersonViewSwitch.Rows[0].Cells)
            {
                if (!string.IsNullOrEmpty(item.CssClass))
                {
                    CssSelectCell = item;
                }
            }

            if (ValidateAndSave() && Page.IsValid)
            {
                ClearDirty();
                mlError.ShowInfoMessage(string.Format(Resources.Messages.SavedDetailsConfirmation, "Person"));
            }
            if (!string.IsNullOrEmpty(ExMessage) || Page.IsValid)
            {
                mvPerson.ActiveViewIndex = viewindex;
                SetCssClassEmpty();
                CssSelectCell.CssClass = "SelectedSwitch";
            }
            if (Page.IsValid && PersonId.HasValue)
            {
                var person = GetPerson(PersonId);
                if (person != null)
                {
                    lblEmployeeNumber.Visible = true;
                    txtEmployeeNumber.Visible = true;
                    PopulateControls(person);
                }
            }
        }

        protected override bool ValidateAndSave()
        {
            return ValidateAndSavePersonDetails();
        }

        public bool ValidateAndSavePersonDetails()
        {
            for (int i = 0; i < mvPerson.Views.Count; i++)
            {
                SelectView(rowSwitcher.Cells[i].Controls[0], i, true);
                Page.Validate(valsPerson.ValidationGroup);
                if (!Page.IsValid)
                {
                    break;
                }
            }
            if (Page.IsValid)
            {
                int? personId = SaveData();
                if (personId.HasValue)
                {
                    PersonId = personId;
                    ClearDirty();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Switches the MultiView with the <see cref="Person"/> details.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnView_Command(object sender, CommandEventArgs e)
        {
            int viewIndex = int.Parse((string)e.CommandArgument);

            if (SaveDirty && (mvPerson.Views[viewIndex] == vwRates || mvPerson.Views[viewIndex] == vwWhatIf) &&
                !ValidateAndSave())
            {
                return;
            }

            SelectView((Control)sender, viewIndex, false);

            if (viewIndex == 8) //History
            {
                activityLog.Update();
            }

            if (viewIndex == 9) //Opportunities
            {
                personOpportunities.DatabindOpportunities();
            }
        }

        /// <summary>
        /// Redirects to Compensation Details page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnStartDate_Command(object sender, CommandEventArgs e)
        {
            if (!SaveDirty || (ValidateAndSave() && Page.IsValid))
            {
                Redirect(
                    string.Format(Constants.ApplicationPages.RedirectStartDateFormat,
                                  Constants.ApplicationPages.CompensationDetail,
                                  PersonId,
                                  HttpUtility.UrlEncode((string)e.CommandArgument)));
            }
        }

        protected void btnAddCompensation_Click(object sender, EventArgs e)
        {
            if (!PersonId.HasValue)
            {
                // Save a New Record
                Page.Validate();
                if (Page.IsValid)
                {
                    int? personId = SaveData();
                    if (Page.IsValid)
                    {
                        Redirect(string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                               Constants.ApplicationPages.CompensationDetail,
                                               personId),
                                 personId.ToString());
                    }
                }
            }
            else if (!SaveDirty || (ValidateAndSave() && Page.IsValid))
            {
                Redirect(
                    string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                  Constants.ApplicationPages.CompensationDetail,
                                  PersonId), PersonId.Value.ToString());
            }
        }

        protected void btnRecruitingCommissionStartDate_Command(object sender, CommandEventArgs e)
        {
            if (!SaveDirty || (ValidateAndSave() && Page.IsValid))
            {
                Redirect(
                    string.Format(Constants.ApplicationPages.RedirectPersonIdFormat,
                                  Constants.ApplicationPages.DefaultRecruitingCommissionDetail,
                                  e.CommandArgument,
                                  PersonId), PersonId.Value.ToString());
            }
        }

        protected void btnAddDefaultRecruitingCommission_Click(object sender, EventArgs e)
        {
            if (!PersonId.HasValue)
            {
                // Save a New Record
                Page.Validate();
                if (Page.IsValid)
                {
                    int? personId = SaveData();
                    if (Page.IsValid)
                    {
                        Redirect(string.Format(Constants.ApplicationPages.RedirectPersonIdFormat,
                                               Constants.ApplicationPages.DefaultRecruitingCommissionDetail,
                                               string.Empty,
                                               personId),
                                 personId.ToString());
                    }
                }
            }
            else if (!SaveDirty || (ValidateAndSave() && Page.IsValid))
            {
                Redirect(
                    string.Format(Constants.ApplicationPages.RedirectPersonIdFormat,
                                  Constants.ApplicationPages.DefaultRecruitingCommissionDetail,
                                  string.Empty,
                                  PersonId), PersonId.Value.ToString());
            }
        }

        protected void btnUnlock_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEmailAddress.Text) && !IsDirty)
            {
                MembershipUser user = Membership.GetUser(txtEmailAddress.Text);

                if (user != null)
                {
                    user.UnlockUser();
                }
            }
        }

        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEmailAddress.Text) && !IsDirty)
            {
                MembershipUser user = Membership.GetUser(txtEmailAddress.Text);

                if (user != null)
                {
                    user.ResetPassword();
                    btnResetPassword.Visible = false;
                    lblPaswordResetted.Visible = true;
                    chbLockedOut.Checked = user.IsLockedOut;
                }
            }
        }

        private void SetCssClassEmpty()
        {
            foreach (TableCell cell in tblPersonViewSwitch.Rows[0].Cells)
            {
                cell.CssClass = string.Empty;
            }
        }

        private void SelectView(Control sender, int viewIndex, bool selectOnly)
        {
            mvPerson.ActiveViewIndex = viewIndex;

            SetCssClassEmpty();

            ((WebControl)sender.Parent).CssClass = "SelectedSwitch";

            if (mvPerson.GetActiveView() == vwWhatIf && !selectOnly)
            {
                // Recalculation the rates
                DisplayCalculatedRate();
            }
        }

        private void DisplayCalculatedRate()
        {
            if (!SaveDirty || (ValidateAndSave() && Page.IsValid))
            {
                /*
                 *  There's no need to call GetPerson, because it
                 *      calls PersonRateCalculator constructor,
                 *      which will later be called in WhatIf.DisplayCalculatorRate
                 *      
                 *      Also, WhatIf doesn't use any Person properties
                 *      except of PersonId
                 */
                //Person person = GetPerson(SelectedId);

                var fakePerson = new Person { Id = PersonId };
                whatIf.Person = fakePerson; // person;
            }
        }

        private void DisplayPersonPermissions()
        {
            ShowPermissionsPerPage();
            ShowPermissionsPerEntities();
        }

        private void ShowPermissionsPerEntities()
        {
            //  If we're editing existing user and having administrators rights
            if (PersonId.HasValue && (UserIsAdministrator || UserIsHR))//#2817 UserisHR is added as per requirement.
            {
                rpPermissions.Visible = true;
                var permissions = DataHelper.GetPermissions(new Person { Id = PersonId.Value });
                rpPermissions.ApplyPermissions(permissions);

                //if (Log.IsInfoEnabled)
                //{
                //    Log.InfoFormat("Entered person (ID {0}) details.\n", SelectedId.Value);
                //    Log.Info(permissions);
                //    Log.Info(Generic.WalkStackTrace(new StackTrace()));
                //}
            }
        }

        private void ShowPermissionsPerPage()
        {
            var permDiff = new List<PermissionDiffeneceItem>();

            IPrincipal userNew =
                new GenericPrincipal(new GenericIdentity(txtEmailAddress.Text), GetSelectedRoles().ToArray());
            IPrincipal userCurrent =
                new GenericPrincipal(new GenericIdentity(txtEmailAddress.Text),
                                     Roles.GetRolesForUser(txtEmailAddress.Text));

            // Retrieve and sort locations
            System.Configuration.Configuration config =
                WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath);
            var locationsSorted = new List<ConfigurationLocation>(config.Locations.Count);
            locationsSorted.AddRange(config.Locations.Cast<ConfigurationLocation>());

            // Evaluate and display permissions for secure pages
            foreach (var location in locationsSorted.OrderBy(location => location.Path))
            {
                var description =
                    location.OpenConfiguration().GetSection("locationDescription") as
                    LocationDescriptionConfigurationSection;
                if (description != null && !string.IsNullOrEmpty(description.Title))
                {
                    permDiff.Add(new PermissionDiffeneceItem
                                     {
                                         Title = description.Title,
                                         Old = UrlAuthorizationModule.CheckUrlAccessForPrincipal("~/" + location.Path, userNew, "GET"),
                                         New = UrlAuthorizationModule.CheckUrlAccessForPrincipal("~/" + location.Path, userCurrent, "GET")
                                     });
                }
            }

            gvPermissionDiffenrece.DataSource = permDiff;
            gvPermissionDiffenrece.DataBind();
        }

        /// <summary>
        /// Retrives the data and display them.
        /// </summary>
        protected override void Display()
        {
            rpPermissions.Visible = false;

            DataHelper.FillPersonStatusList(ddlPersonStatus);
            DataHelper.FillSenioritiesList(ddlSeniority);

            chblRoles.DataSource = Roles.GetAllRoles();
            chblRoles.DataBind();

            int? id = PersonId;

            personProjects.PersonId = id;
            personProjects.UserIsAdministrator = UserIsAdministrator || UserIsHR; //#2817 UserIsHR is added as per requirement.

            Person person = null;

            if (id.HasValue) // Edit existing person mode
            {
                person = GetPerson(id);

                if (person != null)
                {
                    PopulateControls(person);
                }
                else
                {
                    UpdateRecruiterList();
                }
            }
            else // Add new person mode
            {
                // Hide Employee Number related controls
                lblEmployeeNumber.Visible = false;
                txtEmployeeNumber.Visible = false;
                reqEmployeeNumber.Enabled = false;

                cellRates.Visible = cellWhatIf.Visible = false;
                btnResetPassword.Visible = false;

                UpdateRecruiterList();
            }

            bool hasNotClosedCompensation =
                person != null && person.PaymentHistory != null &&
                person.PaymentHistory.Count > 0 &&
                !person.PaymentHistory[person.PaymentHistory.Count - 1].EndDate.HasValue;
            string compensationEndDate = (((person != null) && (person.PaymentHistory != null)) &&
                                            ((person.PaymentHistory.Count > 0) &&
                                                person.PaymentHistory[person.PaymentHistory.Count - 1].EndDate.HasValue))
                                                ? person.PaymentHistory[person.PaymentHistory.Count - 1].EndDate.ToString() : string.Empty;

            ltrScript.Text =
                string.Format(ltrScript.Text,
                              ddlPersonStatus.ClientID,
                              hasNotClosedCompensation ? "true" : "false",
                              (int)PersonStatusType.Terminated,
                              (int)PersonStatusType.Active,
                              ((TextBox)this.dtpTerminationDate.FindControl("txtDate")).ClientID,
                             compensationEndDate);

            UpdateSalesCommissionState();
            UpdateManagementCommissionState();

            personOpportunities.DataBind();
        }

        private static Person GetPerson(int? id)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    return serviceClient.GetPersonDetail(id.Value);
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        protected void dtpHireDate_SelectionChanged(object sender, EventArgs e)
        {
            UpdateRecruiterList();
            IsDirty = true;
            dtpHireDate.Focus();
        }

        protected void dtpTerminationDate_SelectionChanged(object sender, EventArgs e)
        {
            IsDirty = true;
            dtpTerminationDate.Focus();
        }

        protected void chblRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            IsDirty = true;
            chblRoles.Focus();
        }

        #region Populate controls

        private void PopulateControls(Person person)
        {
            //  Names, email, dates etc.
            PopulateBasicData(person);

            // Default recruiter commissions
            PopulateRecruiterCommissions(person);

            // Payment history
            PopulatePaymentAndOverheads(person);

            // Default commissions info
            PopulatePersonCommissions(person);

            // Role/Seniority
            PopulateRolesAndSeniority(person);
        }

        /// <summary>
        /// Updates a recruiter list
        /// </summary>
        private void UpdateRecruiterList()
        {
            if (!PersonId.HasValue)
            {
                var person = new Person { HireDate = dtpHireDate.DateValue };

                recruiterInfo.Person = person;

                if (!UserIsAdministrator && !UserIsHR && UserIsRecruiter)//#2817 UserisHR is added as per requirement.
                {
                    // Recruiter cannot set another resruiter for the person
                    var current = DataHelper.CurrentPerson;
                    if (current != null && current.Id.HasValue)
                    {
                        try
                        {
                            recruiterInfo.SetRecruiter(current.Id.Value);
                            btnSave.Enabled = true;
                        }
                        catch (Exception e)
                        {
                            btnSave.Enabled = false;
                            mlError.ShowErrorMessage(e.Message);
                        }
                    }
                }
            }
        }

        private void PopulateRolesAndSeniority(Person person)
        {
            if (person.Seniority != null)
            {
                ddlSeniority.SelectedIndex =
                    ddlSeniority.Items.IndexOf(ddlSeniority.Items.FindByValue(person.Seniority.Id.ToString()));
            }

            // Roles
            foreach (ListItem item in chblRoles.Items)
            {
                item.Selected = Array.IndexOf(person.RoleNames, item.Text) >= 0;
            }
        }

        private void PopulatePersonCommissions(Person person)
        {
            if (person.DefaultPersonCommissions != null)
            {
                // Sales commission
                foreach (DefaultCommission commission in person.DefaultPersonCommissions)
                {
                    if (commission.TypeOfCommission == CommissionType.Sales)
                    {
                        chbSalesCommissions.Checked = true;
                        txtSalesCommissionsGross.Text = commission.FractionOfMargin.ToString();

                        break;
                    }
                }

                // Practice management commission
                foreach (DefaultCommission commission in person.DefaultPersonCommissions)
                {
                    if (commission.TypeOfCommission == CommissionType.PracticeManagement)
                    {
                        chbManagementCommissions.Checked = true;
                        txtManagementCommission.Text = commission.FractionOfMargin.ToString();
                        rlstManagementCommission.SelectedIndex =
                            rlstManagementCommission.Items.IndexOf(rlstManagementCommission.Items.FindByValue(
                                commission.MarginTypeId.HasValue
                                    ?
                                        commission.MarginTypeId.Value.ToString()
                                    : string.Empty));

                        break;
                    }
                }
            }
        }

        private void PopulatePaymentAndOverheads(Person person)
        {
            gvCompensationHistory.DataSource = person.PaymentHistory;
            gvCompensationHistory.DataBind();

            // Overhead info
            if (person.OverheadList != null)
            {
                DisplayOverhead(person);
            }
        }

        private void PopulateRecruiterCommissions(Person person)
        {
            if (person.DefaultPersonRecruiterCommission != null)
            {
                gvRecruitingCommissions.DataSource = person.DefaultPersonRecruiterCommission;
                gvRecruitingCommissions.DataBind();
            }

            // Recruiter commissions for the given person
            recruiterInfo.Person = person;
        }

        private void PopulateBasicData(Person person)
        {
            txtFirstName.Text = person.FirstName;
            txtLastName.Text = person.LastName;
            dtpHireDate.DateValue = person.HireDate;
            dtpTerminationDate.DateValue =
                person.TerminationDate.HasValue ? person.TerminationDate.Value : DateTime.MinValue;
            txtEmailAddress.Text = person.Alias;
            txtTelephoneNumber.Text = person.TelephoneNumber.Trim();

            PopulatePracticeDropDown(person);

            PersonStatusId = person.Status != null ? (PersonStatusType?)person.Status.Id : null;
            PrevPersonStatusId = (person.Status != null) ? person.Status.Id : -1;

            txtEmployeeNumber.Text = person.EmployeeNumber;

            //Set Locked-Out CheckBox value            
            chbLockedOut.Checked = person.LockedOut;
            defaultManager.EnsureDatabound();
            // Select manager and exclude self from the list
            defaultManager.SelectedManager = person.Manager;
            //defaultManager.ExcludePerson(new Person { Id = SelectedId });
            //newManager.ExcludePerson(new Person { Id = SelectedId });

            repPracticesOwned.DataSource = person.PracticesOwned;
            repPracticesOwned.DataBind();
            if (person.IsDefaultManager)
            {
                this.hdnIsDefaultManager.Value = person.IsDefaultManager.ToString();
            }
        }

        private void PopulatePracticeDropDown(Person person)
        {
            if (person != null && person.DefaultPractice != null)
            {
                ListItem selectedPractice = ddlDefaultPractice.Items.FindByValue(person.DefaultPractice.Id.ToString());

                if (selectedPractice == null)
                {
                    selectedPractice = new ListItem(person.DefaultPractice.Name, person.DefaultPractice.Id.ToString());
                    ddlDefaultPractice.Items.Add(selectedPractice);
                    ddlDefaultPractice.SortByText();
                }

                ddlDefaultPractice.SelectedValue = selectedPractice.Value;
            }
        }

        /// <summary>
        /// Displays the person's overhead.
        /// </summary>
        /// <param name="person"></param>
        private void DisplayOverhead(Person person)
        {
            PersonOverhead[] overheads = person.OverheadList.ToArray();
            Array.Sort(overheads);

            PersonOverhead[] filteredOverheads =
                Array.FindAll(overheads, overhead => overhead.RateType == null ||
                                                     overhead.RateType.Id !=
                                                     (int)OverheadRateTypes.BillRateMultiplier);
            person.OverheadList = new List<PersonOverhead>(filteredOverheads);
            gvOverhead.DataSource = filteredOverheads;
            gvOverhead.DataBind();

            if (gvOverhead.FooterRow != null)
            {
                gvOverhead.FooterRow.Cells[1].Text = person.TotalOverhead.ToString();
                gvOverhead.FooterRow.Cells[1].Font.Bold = true;
            }

            // The person's summary
            lblRawHourlyRate.Text = person.RawHourlyRate.ToString();
            lblLoadedHourlyRate.Text = person.LoadedHourlyRate.ToString();
        }

        #endregion

        /// <summary>
        /// Collects the user input to the DTO.
        /// </summary>
        /// <param name="person">The DTO to the data be collected to.</param>
        private void PopulateData(Person person)
        {
            person.Id = PersonId;
            person.FirstName = txtFirstName.Text;
            person.LastName = txtLastName.Text;
            person.HireDate = dtpHireDate.DateValue;

            person.TerminationDate = dtpTerminationDate.DateValue != DateTime.MinValue ? (DateTime?)dtpTerminationDate.DateValue : null;

            person.Alias = txtEmailAddress.Text;
            person.TelephoneNumber = txtTelephoneNumber.Text;

            person.EmployeeNumber = txtEmployeeNumber.Text;

            person.Status = new PersonStatus { Id = (int)PersonStatusId };

            //Set Locked-Out value
            person.LockedOut = chbLockedOut.Checked;

            person.DefaultPractice =
                !string.IsNullOrEmpty(ddlDefaultPractice.SelectedValue) ?
                new Practice { Id = int.Parse(ddlDefaultPractice.SelectedValue) } : null;

            // Filling the recruiter commissions for the given person
            person.RecruiterCommission = recruiterInfo.RecruiterCommission;

            // Filling the default commissions info
            var salesCommission = new DefaultCommission
                                      {
                                          TypeOfCommission = CommissionType.Sales,
                                          FractionOfMargin =
                                              chbSalesCommissions.Checked
                                                  ? decimal.Parse(txtSalesCommissionsGross.Text)
                                                  : -1
                                      };

            var managementCommission = new DefaultCommission
                                           {
                                               TypeOfCommission = CommissionType.PracticeManagement,
                                               FractionOfMargin =
                                                   chbManagementCommissions.Checked
                                                       ? decimal.Parse(txtManagementCommission.Text)
                                                       : 0,
                                               MarginTypeId = int.Parse(rlstManagementCommission.SelectedValue)
                                           };

            person.DefaultPersonCommissions = new List<DefaultCommission> { salesCommission, managementCommission };

            // Role/Seniority
            if (!string.IsNullOrEmpty(ddlSeniority.SelectedValue))
            {
                person.Seniority = new Seniority { Id = int.Parse(ddlSeniority.SelectedValue) };
            }

            // Roles
            var roleNames = GetSelectedRoles();
            person.RoleNames = roleNames.ToArray();

            person.Manager = defaultManager.SelectedManager;
        }

        private List<string> GetSelectedRoles()
        {
            return (from ListItem item in chblRoles.Items where item.Selected select item.Text).ToList();
        }

        /// <summary>
        /// Saves the user's input.
        /// </summary>
        private int? SaveData()
        {
            string loginPageUrl = base.Request.Url.Scheme + "://" + base.Request.Url.Host + (IsAzureWebRole() ? string.Empty : (":" + base.Request.Url.Port.ToString())) + base.Request.ApplicationPath + "/Login.aspx";

            var person = new Person();
            PopulateData(person);
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    int? personId = serviceClient.SavePersonDetail(person, User.Identity.Name, loginPageUrl);
                    SaveRoles(person);

                    if (personId.Value < 0)
                    {
                        // Creating User error
                        _saveCode = personId.Value;
                        Page.Validate();
                        return null;
                    }

                    SavePersonsPermissions(person, serviceClient);

                    IsDirty = false;

                    return personId;
                }
                catch (Exception ex)
                {

                    serviceClient.Abort();
                    ExMessage = ex.Message;
                    Page.Validate();
                }
            }

            return null;
        }

        private static bool IsAzureWebRole()
        {
            try
            {
                return RoleEnvironment.IsAvailable;
            }
            catch
            {
                return false;
            }
        }



        private void SavePersonsPermissions(Person person, PersonServiceClient serviceClient)
        {
            if (bool.Parse(hfReloadPerms.Value))
            {
                PersonPermission permissions = rpPermissions.GetPermissions();

                serviceClient.SetPermissionsForPerson(person, permissions);
            }
        }

        private static void SaveRoles(Person person)
        {
            if (string.IsNullOrEmpty(person.Alias)) return;

            // Saving roles
            string[] currentRoles = Roles.GetRolesForUser(person.Alias);

            if (person.RoleNames.Length > 0)
            {
                // New roles
                string[] newRoles =
                    Array.FindAll(person.RoleNames, value => Array.IndexOf(currentRoles, value) < 0);

                if (newRoles.Length > 0)
                    Roles.AddUserToRoles(person.Alias, newRoles);
            }

            if (currentRoles.Length > 0)
            {
                // Redundant roles
                string[] redundantRoles =
                    Array.FindAll(currentRoles, value => Array.IndexOf(person.RoleNames, value) < 0);

                if (redundantRoles.Length > 0)
                    Roles.RemoveUserFromRoles(person.Alias, redundantRoles);
            }
        }

        private void DisableControls()
        {
            Control body = Page.Master.FindControl("body");
            EnableDisableControls(body, false);
        }

        public void EnableDisableControls(Control control, bool enable)
        {
            if (control is WebControl && control != btnCancelAndReturn)
            {
                var webControl = (WebControl)control;
                webControl.Enabled = enable;
            }
            if (control.Controls.Count > 0)
            {
                for (int n = 0; n < control.Controls.Count; n++)
                {
                    Control childControl = control.Controls[n];
                    if (childControl.ID != "tblPersonViewSwitch" &&
                        childControl.ID != "whatIf")
                        EnableDisableControls(childControl, enable);
                }
            }
        }

        protected void vwPermissions_PreRender(object sender, EventArgs e)
        {
            if (UserIsAdministrator || UserIsHR)//#2817 UserisHR is added as per requirement.
            {
                DisplayPersonPermissions();
                hfReloadPerms.Value = bool.TrueString;
            }
        }

        protected void custCompensationCoversMilestone_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // Checks if the person is active
            if (ddlPersonStatus.SelectedValue == ActivePersonStatusId)
            {
                args.IsValid = !PersonId.HasValue || (PersonId.HasValue && DataHelper.CurrentPayExists(PersonId.Value));
            }
        }

        protected void odsActivePersons_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters[UserNameParameterName] = DataHelper.CurrentPerson.Alias;
        }

        protected void lbSetPracticeOwner_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlDefaultPractice.SelectedValue))
            {
                var practiceList = DataHelper.GetPracticeById(int.Parse(ddlDefaultPractice.SelectedValue));
                defaultManager.SelectedManager = practiceList[0].PracticeOwner;
            }
        }

        #region Validation

        protected void custPersonName_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!String.IsNullOrEmpty(ExMessage))
            {
                args.IsValid = !(ExMessage == DuplicatePersonName);
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void custEmailAddress_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!String.IsNullOrEmpty(ExMessage))
            {
                args.IsValid = !(ExMessage == DuplicateEmail);
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void custEmployeeNumber_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid =
                _internalException == null ||
                _internalException.Message != ErrorCode.PersonEmployeeNumberUniquenesViolation.ToString();
        }

        protected void custPersonData_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid =
                _internalException == null ||
                _internalException.Message == ErrorCode.PersonNameUniquenesViolation.ToString() ||
                _internalException.Message == ErrorCode.PersonEmailUniquenesViolation.ToString() ||
                _internalException.Message == ErrorCode.PersonEmployeeNumberUniquenesViolation.ToString();

            if (_internalException != null)
            {
                ((CustomValidator)source).Text = _internalException.Message;
            }
        }

        // Any person who is projected should not have any roles checked.  
        // User should  uncheck their roles to save the record.
        protected void custRoles_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!string.IsNullOrEmpty(ddlPersonStatus.SelectedValue) &&
                int.Parse(ddlPersonStatus.SelectedValue) == (int)PersonStatusType.Projected)
            {
                // Roles
                if (chblRoles.Items.Cast<ListItem>().Any(item => item.Selected))
                {
                    args.IsValid = false;
                }
            }
        }

        protected void custSeniority_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            // Active persons must have the Seniority set.
            e.IsValid =
                e.Value != ((int)PersonStatusType.Active).ToString() ||
                !string.IsNullOrEmpty(ddlSeniority.SelectedValue);
        }

        protected void custTerminationDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            int personStatus;
            if (int.TryParse(this.ddlPersonStatus.SelectedValue, out personStatus))
            {
                bool isTerminationDateEmpty = string.IsNullOrEmpty(this.dtpTerminationDate.TextValue);
                args.IsValid = true;
                if (personStatus == 2)
                {
                    args.IsValid = !isTerminationDateEmpty;
                }
            }
        }

        /// <summary>
        /// Validates a user ctreate status code.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void custUserName_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = _saveCode == default(int);

            string message;
            switch (-_saveCode)
            {
                case (int)MembershipCreateStatus.DuplicateEmail:
                    message = Messages.DuplicateEmail;
                    break;
                case (int)MembershipCreateStatus.DuplicateUserName:
                    //  Because we're using email as username in the system,
                    //      DuplicateUserName is equal to our PersonEmailUniquenesViolation
                    message = Messages.DuplicateEmail;
                    break;
                case (int)MembershipCreateStatus.InvalidAnswer:
                    message = Messages.InvalidAnswer;
                    break;
                case (int)MembershipCreateStatus.InvalidEmail:
                    message = Messages.InvalidEmail;
                    break;
                case (int)MembershipCreateStatus.InvalidPassword:
                    message = Messages.InvalidPassword;
                    break;
                case (int)MembershipCreateStatus.InvalidQuestion:
                    message = Messages.InvalidQuestion;
                    break;
                case (int)MembershipCreateStatus.InvalidUserName:
                    message = Messages.InvalidUserName;
                    break;
                case (int)MembershipCreateStatus.ProviderError:
                    message = Messages.ProviderError;
                    break;
                case (int)MembershipCreateStatus.UserRejected:
                    message = Messages.UserRejected;
                    break;
                default:
                    message = custUserName.ErrorMessage;
                    return;
            }
            custUserName.ErrorMessage = custUserName.ToolTip = message;
        }

        protected void custReqEmailAddress_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!string.IsNullOrEmpty(ddlPersonStatus.SelectedValue) &&
                int.Parse(ddlPersonStatus.SelectedValue) == (int)PersonStatusType.Active)
            {
                args.IsValid = !string.IsNullOrEmpty(txtEmailAddress.Text);
            }
        }

        protected void custPersonStatus_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            int statusId;
            e.IsValid =
                // Only administrators can set a status to Active or Terminated
                UserIsAdministrator || UserIsHR || !int.TryParse(e.Value, out statusId) ||
                statusId == (int)PersonStatusType.Projected || statusId == (int)PersonStatusType.Inactive;//#2817 UserisHR is added as per requirement.
        }

        protected void custHireDate_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            List<RecruiterCommission> commissions = recruiterInfo.RecruiterCommission;
            Person current = DataHelper.CurrentPerson;
            e.IsValid =
                UserIsAdministrator || UserIsHR ||
                (current != null && current.Id.HasValue && commissions != null &&
                 commissions.Count > 0 &&
                 commissions.Count(commission => commission.RecruiterId != current.Id.Value) == 0);//#2817 UserisHR is added as per requirement.
        }

        #endregion

        #region Commissions

        protected void chbSalesCommissions_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSalesCommissionState();
            IsDirty = true;
        }

        protected void chbManagementCommissions_CheckedChanged(object sender, EventArgs e)
        {
            UpdateManagementCommissionState();
            IsDirty = true;
        }

        private void UpdateSalesCommissionState()
        {
            txtSalesCommissionsGross.Enabled =
                reqSalesCommissionsGross.Enabled = compSalesCommissionsGross.Enabled =
                                                   chbSalesCommissions.Checked;
        }

        private void UpdateManagementCommissionState()
        {
            txtManagementCommission.Enabled =
                reqManagementCommission.Enabled = compManagementCommission.Enabled =
                                                  rlstManagementCommission.Enabled =
                                                  chbManagementCommissions.Checked;
        }

        #endregion

        protected void valNotes_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = args.Value.Length <= 5000;
        }

        protected void valRecruterRole_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            // This is a stub for the issue #1858

            args.IsValid = true;
        }

        protected void nPerson_OnNoteAdded(object source, EventArgs args)
        {
            activityLog.Update();
        }

        public static Opportunity[] GetOpportunities(int targetPersonId)
        {
            using (OpportunityServiceClient serviceClient = new OpportunityServiceClient())
            {
                try
                {
                    var context = new OpportunityListContext { TargetPersonId = targetPersonId };
                    Opportunity[] opportunities =
                        serviceClient.OpportunityListAll(context);

                    return opportunities;
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }
        protected void custTerminationDateTE_ServerValidate(object source, ServerValidateEventArgs args)
        {
            DateTime? terminationDate = (this.dtpTerminationDate.DateValue != DateTime.MinValue) ? new DateTime?(this.dtpTerminationDate.DateValue) : null;
            bool TEsExistsAfterTerminationDate = false;
            List<Milestone> milestonesAfterTerminationDate = new List<Milestone>();
            using (PersonServiceClient serviceClient = new PersonServiceClient())
            {
                if (this.PersonId.HasValue && terminationDate.HasValue)
                {
                    TEsExistsAfterTerminationDate = serviceClient.CheckPersonTimeEntriesAfterTerminationDate(this.PersonId.Value, terminationDate.Value);
                    milestonesAfterTerminationDate.AddRange(serviceClient.GetPersonMilestonesAfterTerminationDate(this.PersonId.Value, terminationDate.Value));
                }
            }
            if (TEsExistsAfterTerminationDate || milestonesAfterTerminationDate.Any<Milestone>())
            {
                this.dvTerminationDateErrors.Visible = true;
                if (TEsExistsAfterTerminationDate)
                {
                    this.lblTimeEntriesExist.Visible = true;
                    this.lblTimeEntriesExist.Text = string.Format(this.lblTimeEntriesExist.Text, terminationDate.Value.ToString("MM/dd/yyy"));
                }
                else
                {
                    this.lblTimeEntriesExist.Visible = false;
                }
                if (milestonesAfterTerminationDate.Any<Milestone>())
                {
                    this.dvProjectMilestomesExist.Visible = true;
                    this.lblProjectMilestomesExist.Text = string.Format(this.lblProjectMilestomesExist.Text, terminationDate.Value.ToString("MM/dd/yyy"));
                    this.dtlProjectMilestones.DataSource = milestonesAfterTerminationDate;
                    this.dtlProjectMilestones.DataBind();
                }
                else
                {
                    this.dvProjectMilestomesExist.Visible = false;
                }
                this.dtpTerminationDate.DateValue = terminationDate.Value;
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void custIsDefautManager_ServerValidate(object source, ServerValidateEventArgs args)
        {
            bool isDefaultManager;
            DateTime? terminationDate = (this.dtpTerminationDate.DateValue != DateTime.MinValue) ? new DateTime?(this.dtpTerminationDate.DateValue) : null;
            if ((terminationDate.HasValue && bool.TryParse(this.hdnIsDefaultManager.Value, out isDefaultManager)) && isDefaultManager)
            {
                args.IsValid = false;
                custIsDefautManager.ToolTip = custIsDefautManager.ErrorMessage;
            }
            else
            {
                args.IsValid = true;
            }
        }



        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {

            bool result = ValidateAndSavePersonDetails();
            if (result)
            {

                var query = Request.QueryString.ToString();
                var backUrl = string.Format(
                        Constants.ApplicationPages.DetailRedirectFormat,
                        Constants.ApplicationPages.PersonDetail,
                        this.PersonId.Value);
                RedirectWithBack(eventArgument, backUrl);
            }

        }

        #endregion
    }
}

