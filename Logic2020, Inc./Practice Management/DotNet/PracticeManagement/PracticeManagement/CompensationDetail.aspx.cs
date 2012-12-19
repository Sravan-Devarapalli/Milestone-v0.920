using System;
using System.Linq;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.PersonService;

namespace PraticeManagement
{
    using System.Threading;
    using System.Web;
    using System.Web.Security;
    using Resources;
    using Utils;

    public partial class CompensationDetail : PracticeManagementPageBase
    {
        #region Constants

        private const string StartDateArgument = "StartDate";
        private const string IsStawman = "Isstrawman";

        #endregion

        #region Fields

        private ExceptionDetail internalException;
        private int _saveCode;
        private bool? _userIsAdministratorValue;
        private bool? _userIsHRValue;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value from the StartDate query string argument.
        /// </summary>
        protected DateTime? SelectedStartDate
        {
            get
            {
                return GetArgumentDateTime(StartDateArgument);
            }
        }

        protected bool? SelectedStawman
        {
            get
            {
                return GetArgumentInt32(IsStawman) != null ? (bool?)(GetArgumentInt32(IsStawman) == 1) : null;
            }
        }

        protected Person PersonDetailData
        {
            get
            {
                return (Person)ViewState["PersonDetailPageData"];
            }
            set
            {
                ViewState["PersonDetailPageData"] = value;
            }
        }

        protected PersonPermission Permissions
        {
            get
            {
                return (PersonPermission)ViewState["PersonPermissions_ViewState"];
            }
            set
            {
                ViewState["PersonPermissions_ViewState"] = value;
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

        #endregion

        #region Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            mlConfirmation.ClearMessage();

            if (SelectedStawman.HasValue && SelectedStawman.Value)
            {
                personnelCompensation.IsStrawmanMode = SelectedStawman.Value;
            }
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            if (SelectedStawman.HasValue && SelectedStawman.Value)
            {
                personnelCompensation.ShowDates();
                personnelCompensation.StartDateReadOnly = true;
                personnelCompensation.EndDateReadOnly = true;
            }
        }

        protected override void Display()
        {
            using (PersonServiceClient serviceClient = new PersonServiceClient())
            {
                try
                {
                    Person person;
                    if (PreviousPage != null && PreviousPage.PersonUnsavedData != null)
                    {
                        person = PersonDetailData = PreviousPage.PersonUnsavedData;
                        Permissions = PreviousPage.Permissions;
                    }
                    else
                    {
                        person = serviceClient.GetPersonDetail(SelectedId.Value);
                    }

                    if (SelectedStartDate.HasValue)
                    {
                        Pay pay = person.PaymentHistory.First(pa => pa.StartDate.Date == SelectedStartDate.Value.Date);
                        var now = Utils.Generic.GetNowWithTimeZone(); 
                        btnSave.Visible = (pay.EndDate.HasValue) ? !((pay.EndDate.Value.AddDays(-1) < now.Date) || (person.Status.Id == (int)PersonStatusType.Terminated)) : true;
                        PopulateControls(pay);
                    }
                    else
                    {
                        if (person.PaymentHistory.Count == 0 || (PreviousPage != null && PersonDetailData != null))
                        {
                            personnelCompensation.StartDate = person.HireDate;
                            if (person.Title != null)
                            {
                                personnelCompensation.TitleId = person.Title.TitleId;
                            }
                            if (person.DefaultPractice != null)
                            {
                                personnelCompensation.PracticeId = person.DefaultPractice.Id;
                            }
                            personnelCompensation.StartDateReadOnly = true;
                        }
                        else
                        {
                            personnelCompensation.StartDate = DateTime.Today;
                        }
                    }

                    personInfo.Person = person;
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        private void PopulateControls(Pay pay)
        {
            personnelCompensation.StartDate = pay.StartDate;
            personnelCompensation.EndDate = pay.EndDate;
            personnelCompensation.Timescale = pay.Timescale;
            personnelCompensation.Amount = pay.Amount;
            personnelCompensation.VacationDays = pay.VacationDays;
            personnelCompensation.IsYearBonus = pay.IsYearBonus;
            personnelCompensation.BonusAmount = pay.BonusAmount;
            personnelCompensation.BonusHoursToCollect = pay.BonusHoursToCollect;
            personnelCompensation.PracticeId = pay.PracticeId;
            personnelCompensation.TitleId = pay.TitleId;
            personnelCompensation.SLTApproval = pay.SLTApproval;
            personnelCompensation.SLTPTOApproval = pay.SLTPTOApproval;
        }

        #region Validation

        protected void custdateRangeBegining_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid =
                internalException == null ||
                internalException.Message != ErrorCode.CompensationStartDateIncorrect.ToString();
        }

        protected void custdateRangeEnding_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid =
                internalException == null ||
                internalException.Message != ErrorCode.EndDateIncorrect.ToString();
        }

        protected void custdateRangePeriod_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid =
                internalException == null ||
                internalException.Message != ErrorCode.PeriodIncorrect.ToString();
        }

        protected void cvEmployeePayTypeChangeViolation_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            var validator = ((CustomValidator)sender);
            DateTime enddate = personnelCompensation.EndDate.HasValue ? personnelCompensation.EndDate.Value : new DateTime(2029, 12, 31);
            e.IsValid = !ServiceCallers.Custom.Person(p => p.IsPersonTimeOffExistsInSelectedRangeForOtherthanGivenTimescale(SelectedId.Value, personnelCompensation.StartDate.Value, enddate, (int)personnelCompensation.Timescale));
            if (!e.IsValid)
            {
                validator.Text = validator.ToolTip = validator.ErrorMessage = PersonDetail.EmployeePayTypeChangeVoilationMessage;
                mpeEmployeePayTypeChange.Show();
            }
        }

        #endregion

        protected void personnelCompensation_CompensationMethodChanged(object sender, EventArgs e)
        {

        }

        protected void personnelCompensation_SaveDetails(object sender, EventArgs e)
        {
            btnSave_Click(btnSave, null);
        }

        protected void personnelCompensation_PeriodChanged(object sender, EventArgs e)
        {
            IsDirty = true;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateAndSave())
            {
                if (PersonDetailData != null)
                {
                    if (_saveCode == default(int))
                    {
                        ClearDirty();

                        //var returnUrl = Request.Url.AbsoluteUri.Substring(Request.Url.AbsoluteUri.LastIndexOf("&returnTo="));
                        var returnUrl = Request.UrlReferrer.ToString();
                        if (returnUrl.LastIndexOf("&returnTo=") != -1)
                            returnUrl = returnUrl.Substring(returnUrl.LastIndexOf("&returnTo="));
                        string redirectUrl = "PersonDetail.aspx?id=" + PersonDetailData.Id + "&ShowConfirmMessage=1";
                        redirectUrl = redirectUrl + (returnUrl.Contains("persons.aspx") ? returnUrl : string.Empty);

                        Response.Redirect(redirectUrl);

                        //Server.Transfer("~" + ReturnUrl.Substring(ReturnUrl.IndexOf("/PersonDetail.aspx?id="), ReturnUrl.Length - ReturnUrl.IndexOf("/PersonDetail.aspx?id=")));
                    }
                }
                else
                {
                    ClearDirty();
                    if (SelectedId.HasValue)
                    {
                        ReturnToPreviousPage();
                    }
                }
            }
            else
            {
                if (cvEmployeePayTypeChangeViolation.IsValid)
                {
                    bool isPayTypeChangeViolationEnable = cvEmployeePayTypeChangeViolation.Enabled;
                    cvEmployeePayTypeChangeViolation.Enabled = false;
                    Page.Validate();
                    cvEmployeePayTypeChangeViolation.Enabled = isPayTypeChangeViolationEnable;
                    if (Page.IsValid)
                    {
                        // Error occured while saving.
                        CustomValidator cvc = new CustomValidator();
                        cvc.IsValid = false;
                        cvc.Text = "*";
                        cvc.Display = ValidatorDisplay.None;
                        cvc.ErrorMessage = @"Error occured while saving the Compensation.";
                        pnlBody.ContentTemplateContainer.Controls.Add(cvc);

                        if (internalException != null)
                        {
                            string data = internalException.ToString();
                            string innerexceptionMessage = internalException.InnerException.Message;
                            // Error occured while saving.
                            CustomValidator cvc2 = new CustomValidator();
                            cvc2.IsValid = false;
                            cvc2.Text = "*";
                            cvc2.Display = ValidatorDisplay.None;
                            if (data.Contains("CK_Pay_DateRange"))
                            {
                                cvc2.ErrorMessage = @"Compensation for the same period already exists.";
                                pnlBody.ContentTemplateContainer.Controls.Add(cvc2);
                            }
                            else if (innerexceptionMessage == PersonDetail.StartDateIncorrect)
                            {
                                cvc2.ErrorMessage = PersonDetail.StartDateIncorrect;
                                pnlBody.ContentTemplateContainer.Controls.Add(cvc2);
                            }
                            else if (innerexceptionMessage == PersonDetail.EndDateIncorrect)
                            {
                                cvc2.ErrorMessage = PersonDetail.EndDateIncorrect;
                                pnlBody.ContentTemplateContainer.Controls.Add(cvc2);
                            }
                            else if (innerexceptionMessage == PersonDetail.PeriodIncorrect)
                            {
                                cvc2.ErrorMessage = PersonDetail.PeriodIncorrect;
                                pnlBody.ContentTemplateContainer.Controls.Add(cvc2);
                            }
                            else if (innerexceptionMessage == PersonDetail.HireDateInCorrect)
                            {
                                cvc2.ErrorMessage = PersonDetail.HireDateInCorrect;
                                pnlBody.ContentTemplateContainer.Controls.Add(cvc2);
                            }
                            else if (innerexceptionMessage == PersonDetail.SalaryToContractException)
                            {
                                cvc2.ErrorMessage = PersonDetail.SalaryToContractMessage;
                                pnlBody.ContentTemplateContainer.Controls.Add(cvc2);
                            }
                            else
                            {
                                cvc2 = null;
                            }
                        }
                    }
                }
            }
        }

        protected override bool ValidateAndSave()
        {
            bool result = false;
            bool isPayTypeChangeViolationEnable = cvEmployeePayTypeChangeViolation.Enabled;
            cvEmployeePayTypeChangeViolation.Enabled = false;
            Page.Validate(vsumCompensation.ValidationGroup);
            cvEmployeePayTypeChangeViolation.Enabled = isPayTypeChangeViolationEnable;
            if (Page.IsValid && cvEmployeePayTypeChangeViolation.Enabled && SelectedId.HasValue)
            {
                cvEmployeePayTypeChangeViolation.Validate();
            }
            if (Page.IsValid)
            {
                result = SaveData();
            }

            return result;
        }

        private bool SaveData()
        {
            Pay pay = personnelCompensation.Pay;
            pay.PersonId = SelectedId.Value;

            using (PersonServiceClient serviceClient = new PersonServiceClient())
            {
                try
                {
                    if (SelectedStawman.HasValue && SelectedStawman.Value)
                    {
                        var person = new Person { Id = pay.PersonId, FirstName = personInfo.FirstName, LastName = personInfo.LastName };
                        serviceClient.SaveStrawman(person, pay, HttpContext.Current.User.Identity.Name);
                    }
                    else
                    {
                        if (PersonDetailData != null)
                        {
                            var person = PersonDetailData;
                            person.CurrentPay = pay;
                            Person oldPerson = null;
                            if (person.Id.HasValue)
                            {
                                oldPerson = serviceClient.GetPersonDetailsShort(person.Id.Value);
                                oldPerson.RoleNames = Roles.GetRolesForUser(oldPerson.Alias);
                            }
                            int? personId = serviceClient.SavePersonDetail(person, User.Identity.Name, LoginPageUrl, true);
                            PersonDetail.SaveRoles(person);
                            serviceClient.SendAdministratorAddedEmail(person, oldPerson);
                            if (personId.Value < 0)
                            {
                                // Creating User error
                                _saveCode = personId.Value;
                                Page.Validate(custUserName.ValidationGroup);

                                return true;
                            }

                            SavePersonsPermissions(person, serviceClient);

                            if (!PersonDetailData.Id.HasValue)
                                PersonDetailData.Id = personId.Value;
                            IsDirty = false;
                        }
                        else
                        {
                            serviceClient.SavePay(pay, LoginPageUrl, HttpContext.Current.User.Identity.Name);
                        }
                        personnelCompensation.StartDate = personnelCompensation.StartDate;
                        personnelCompensation.EndDate = personnelCompensation.EndDate;
                    }
                    return true;
                }
                catch (FaultException<ExceptionDetail> ex)
                {
                    internalException = ex.Detail;
                    serviceClient.Abort();
                    Logging.LogErrorMessage(
                        ex.Message,
                        ex.Source,
                        internalException.InnerException != null ? internalException.InnerException.Message : string.Empty,
                        string.Empty,
                        HttpContext.Current.Request.Url.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped),
                        string.Empty,
                        Thread.CurrentPrincipal.Identity.Name);
                    return false;
                }
            }
        }

        private void SavePersonsPermissions(Person person, PersonServiceClient serviceClient)
        {
            if (UserIsAdministrator || UserIsHR)
            {
                serviceClient.SetPermissionsForPerson(person, Permissions);
            }
        }

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

        protected void btnEmployeePayTypeChangeViolationOk_Click(object sender, EventArgs e)
        {
            cvEmployeePayTypeChangeViolation.Enabled = false;
            btnSave_Click(btnSave, new EventArgs());
            mpeEmployeePayTypeChange.Hide();
            cvEmployeePayTypeChangeViolation.Enabled = true;
        }

        protected void btnEmployeePayTypeChangeViolationCancel_Click(object source, EventArgs args)
        {
            if (SelectedStartDate.HasValue)
            {
                Person person = ServiceCallers.Custom.Person(p => p.GetPersonDetail(SelectedId.Value));
                Pay pay = person.PaymentHistory.First(pa => pa.StartDate.Date == SelectedStartDate.Value.Date);

                var now = Utils.Generic.GetNowWithTimeZone();
                btnSave.Visible = (pay.EndDate.HasValue) ? !((pay.EndDate.Value.AddDays(-1) < now.Date) || (person.Status.Id == (int)PersonStatusType.Terminated)) : true;

                PopulateControls(pay);
            }
            cvEmployeePayTypeChangeViolation.Enabled = true;
            mpeEmployeePayTypeChange.Hide();
        }

        #endregion
    }
}

