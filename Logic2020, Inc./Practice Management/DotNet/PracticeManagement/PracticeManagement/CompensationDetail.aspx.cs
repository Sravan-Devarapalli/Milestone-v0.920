using System;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.PersonService;
using PraticeManagement.TimescaleService;
using System.Linq;

namespace PraticeManagement
{
    using System.Threading;
    using System.Web;
    using Utils;
    using System.Web.Security;
    using Resources;

    public partial class CompensationDetail : PracticeManagementPageBase
    {
        #region Constants

        private const string StartDateArgument = "StartDate";
        private const string StartDateIncorrect = "The Start Date is incorrect. There are several other compensation records for the specified period. Please edit them first.";
        private const string EndDateIncorrect = "The End Date is incorrect. There are several other compensation records for the specified period. Please edit them first.";
        private const string PeriodIncorrect = "The period is incorrect. There records falls into the period specified in an existing record.";
        private const string HireDateInCorrect = "Person cannot have the compensation for the days before his hire date.";
        private const string IsStawman = "Isstrawman";
        private const string SalaryToContractException = "Salary Type to Contract Type Violation";
        private const string SalaryToContractMessage = "To switch employee status from W2-Hourly or W2-Salary to a status of 1099 Hourly or 1099 POR, the user will have to terminate their employment using the \"Change Employee Status\" workflow, select a termination reason, and then re-activate the person's status via the \"Change Employee Status\" workflow, changing their pay type to \"1099 Hourly\" or \"1099 POR\"";

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

        protected string LoginPageUrl
        {
            get
            {
                return (string)ViewState["LoginPageUrl_ViewState"];
            }
            set
            {
                ViewState["LoginPageUrl_ViewState"] = value;
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
                    if (PreviousPage != null && PreviousPage.PersonUnsavedData != null && PreviousPage.LoginPageUrl != null)
                    {
                        person = PersonDetailData = PreviousPage.PersonUnsavedData;
                        LoginPageUrl = PreviousPage.LoginPageUrl;
                        Permissions = PreviousPage.Permissions;
                    }
                    else
                    {
                        person = serviceClient.GetPersonDetail(SelectedId.Value);
                    }

                    if (SelectedStartDate.HasValue)
                    {
                        Pay pay = person.PaymentHistory.First(pa => pa.StartDate.Date == SelectedStartDate.Value.Date);

                        btnSave.Visible = (pay.StartDate >= person.HireDate && person.Status.Id != (int)PersonStatusType.Terminated);

                        PopulateControls(pay);
                    }
                    else
                    {
                        if (person.PaymentHistory.Count == 0 || (PreviousPage != null && PersonDetailData != null))
                        {
                            personnelCompensation.StartDate = person.HireDate;
                            if (person.Seniority != null)
                            {
                                personnelCompensation.SeniorityId = person.Seniority.Id;
                            }
                            if (person.DefaultPractice != null)
                            {
                                personnelCompensation.PracticeId = person.DefaultPractice.Id;
                            }
                            if (person.DefaultPersonCommissions != null && person.DefaultPersonCommissions.Any(cl => cl.TypeOfCommission == CommissionType.Sales) && PreviousPage == null && PersonDetailData == null)
                            {
                                var salesComm = person.DefaultPersonCommissions.First(cl => cl.TypeOfCommission == CommissionType.Sales);
                                personnelCompensation.SalesCommissionFractionOfMargin = salesComm.FractionOfMargin;
                            }
                            personnelCompensation.StartDateReadOnly = true;
                        }
                        else
                        {
                            personnelCompensation.StartDate = DateTime.Today;
                        }

                        SetDefaultTerms();
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
            personnelCompensation.TimesPaidPerMonth = pay.TimesPaidPerMonth;
            personnelCompensation.Terms = pay.Terms;
            personnelCompensation.IsYearBonus = pay.IsYearBonus;
            personnelCompensation.BonusAmount = pay.BonusAmount;
            personnelCompensation.BonusHoursToCollect = pay.BonusHoursToCollect;
            personnelCompensation.DefaultHoursPerDay = pay.DefaultHoursPerDay;
            personnelCompensation.SeniorityId = pay.SeniorityId;
            personnelCompensation.PracticeId = pay.PracticeId;
            personnelCompensation.SalesCommissionFractionOfMargin = pay.SalesCommissionFractionOfMargin;
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

        #endregion

        protected void personnelCompensation_CompensationMethodChanged(object sender, EventArgs e)
        {
            SetDefaultTerms();
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
                        if(returnUrl.LastIndexOf("&returnTo=") != -1)
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
                    mlConfirmation.ShowInfoMessage(string.Format(Resources.Messages.SavedDetailsConfirmation, "Compensation"));
                }
            }
            else
            {
                Page.Validate();
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
                        else if (innerexceptionMessage == StartDateIncorrect)
                        {
                            cvc2.ErrorMessage = StartDateIncorrect;
                            pnlBody.ContentTemplateContainer.Controls.Add(cvc2);
                        }
                        else if (innerexceptionMessage == EndDateIncorrect)
                        {
                            cvc2.ErrorMessage = EndDateIncorrect;
                            pnlBody.ContentTemplateContainer.Controls.Add(cvc2);
                        }
                        else if (innerexceptionMessage == PeriodIncorrect)
                        {
                            cvc2.ErrorMessage = PeriodIncorrect;
                            pnlBody.ContentTemplateContainer.Controls.Add(cvc2);
                        }
                        else if (innerexceptionMessage == HireDateInCorrect)
                        {
                            cvc2.ErrorMessage = HireDateInCorrect;
                            pnlBody.ContentTemplateContainer.Controls.Add(cvc2);
                        }
                        else if (innerexceptionMessage == SalaryToContractException)
                        {
                            cvc2.ErrorMessage = SalaryToContractMessage;
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

        protected override bool ValidateAndSave()
        {
            bool result = false;
            Page.Validate();
            if (Page.IsValid)
            {
                result = SaveData();
            }

            return result;
        }

        private void SetDefaultTerms()
        {
            using (TimescaleServiceClient serviceClient = new TimescaleServiceClient())
            {
                try
                {
                    Timescale timescale = serviceClient.GetById(personnelCompensation.Timescale);
                    personnelCompensation.Terms = timescale != null ? timescale.DefaultTerms : null;
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
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
                            int? personId = serviceClient.SavePersonDetail(person, User.Identity.Name, LoginPageUrl);
                            SaveRoles(person);

                            if (personId.Value < 0)
                            {
                                // Creating User error
                                _saveCode = personId.Value;
                                Page.Validate(custUserName.ValidationGroup);

                                return true;
                            }

                            SavePersonsPermissions(person, serviceClient);

                            if(!PersonDetailData.Id.HasValue)
                                PersonDetailData.Id = personId.Value;
                            IsDirty = false;
                        }

                        serviceClient.SavePay(pay, HttpContext.Current.User.Identity.Name);
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

        #endregion
    }
}

