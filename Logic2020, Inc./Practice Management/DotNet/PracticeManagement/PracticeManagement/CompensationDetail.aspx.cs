using System;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.PersonService;
using PraticeManagement.TimescaleService;

namespace PraticeManagement
{
    using System.Threading;
    using System.Web;
    using Utils;

    public partial class CompensationDetail : PracticeManagementPageBase
    {
        #region Constants

        private const string StartDateArgument = "StartDate";
        private const string StartDateIncorrect = "The Start Date is incorrect. There are several other compensation records for the specified period. Please edit them first.";
        private const string EndDateIncorrect = "The End Date is incorrect. There are several other compensation records for the specified period. Please edit them first.";
        private const string PeriodIncorrect = "The period is incorrect. There records falls into the period specified in an existing record.";
        private const string HireDateInCorrect = "Person cannot have the compensation for the days before his hire date.";

        #endregion

        #region Fields

        private ExceptionDetail internalException;

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

        #endregion

        #region Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            mlConfirmation.ClearMessage();
        }

        protected override void Display()
        {
            using (PersonServiceClient serviceClient = new PersonServiceClient())
            {
                try
                {
                    Person person = serviceClient.GetPersonDetail(SelectedId.Value);

                    if (SelectedStartDate.HasValue)
                    {
                        Pay pay = serviceClient.GetPayment(SelectedId.Value, SelectedStartDate.Value);

                        PopulateControls(pay);
                    }
                    else
                    {
                        if (person.PaymentHistory.Count == 0)
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
                ClearDirty();
                mlConfirmation.ShowInfoMessage(string.Format(Resources.Messages.SavedDetailsConfirmation, "Compensation"));
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
                    serviceClient.SavePay(pay);
                    personnelCompensation.StartDate = personnelCompensation.StartDate;
                    personnelCompensation.EndDate = personnelCompensation.EndDate;
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

        #endregion
    }
}

