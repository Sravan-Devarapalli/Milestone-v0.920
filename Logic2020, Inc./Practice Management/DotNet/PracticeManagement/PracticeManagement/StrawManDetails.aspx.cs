using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using DataTransferObjects;
using PraticeManagement.PersonService;
using System.ServiceModel;
using PraticeManagement.Utils;
using PraticeManagement.TimescaleService;

namespace PraticeManagement
{
    public partial class StrawManDetails : PracticeManagementPageBase
    {
        #region Constants

        private const string ViewStatePersonId = "PersonIdViewState";
        private const string DuplicatePersonName = "There is another Person with the same First Name and Last Name.";
        private const string SuccessMessage = "Saved Successfully.";
        private const int NameCharactersLength = 50;
        #endregion

        #region Properties

        public int? PersonId
        {
            get
            {
                if (SelectedId.HasValue)
                {
                    return SelectedId;
                }
                else
                {
                    return (int?)ViewState[ViewStatePersonId];
                }
            }
            set
            {
                ViewState[ViewStatePersonId] = value;
            }
        }

        public Pay CurrentCompensation
        {
            get
            {
                var pay = new Pay();
                pay.Timescale = personnelCompensation.Timescale;
                pay.Amount = personnelCompensation.Amount.Value;
                pay.BonusAmount = personnelCompensation.BonusAmount;
                pay.BonusHoursToCollect = personnelCompensation.BonusHoursToCollect;
                pay.DefaultHoursPerDay = personnelCompensation.DefaultHoursPerDay;
                pay.VacationDays = personnelCompensation.VacationDays;
                pay.TimesPaidPerMonth = personnelCompensation.TimesPaidPerMonth;
                pay.Terms = personnelCompensation.Terms;

                return pay;
            }
            set
            {
                var pay = value;
                personnelCompensation.Timescale = pay.Timescale;
                personnelCompensation.Amount = pay.Amount;
                personnelCompensation.BonusAmount = pay.BonusAmount;
                personnelCompensation.BonusHoursToCollect = pay.BonusHoursToCollect;
                personnelCompensation.DefaultHoursPerDay = pay.DefaultHoursPerDay;
                personnelCompensation.VacationDays = pay.VacationDays;
                personnelCompensation.TimesPaidPerMonth = pay.TimesPaidPerMonth;
                personnelCompensation.Terms = pay.Terms;
            }
        }

        public string ExMessage { get; set; }

        #endregion

        protected void cvDupliacteName_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (!string.IsNullOrEmpty(ExMessage) && ExMessage == DuplicatePersonName)
            {
                e.IsValid = false;
            }
        }

        protected void cvNameLength_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            var item = sender as CustomValidator;
            if (item.ID == "cvLengthFirstName")
            {
                e.IsValid = tbFirstName.Text.Length <= NameCharactersLength;
            }
            else if (item.ID == "cvLengthLastName")
            {
                e.IsValid = tbLastName.Text.Length <= NameCharactersLength;
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lblSave.ClearMessage();
        }

        protected override void Display()
        {
            if (!IsPostBack)
            {
                if (PersonId.HasValue)
                {
                    Person person = GetPerson(PersonId.Value);

                    //if (!person.IsStrawMan)
                    //{
                    //    Redirect(Constants.ApplicationPages.PageNotFound);
                    //}

                    PopulateControls(person);
                }
                else
                {
                    SetDefaultTerms();
                }
            }
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

        private void PopulateControls(Person person)
        {
            tbFirstName.Text = person.FirstName;
            tbLastName.Text = person.LastName;
            var paymentHistory = new List<Pay>();
            paymentHistory.AddRange(person.PaymentHistory);

            if (person.PaymentHistory.Count > 0)
            {
                var currentPay = person.PaymentHistory.Where(p => (!p.EndDate.HasValue || SettingsHelper.GetCurrentPMTime().Date <= p.EndDate.Value.AddDays(-1)) && (p.StartDate == null || SettingsHelper.GetCurrentPMTime().Date >= p.StartDate)).First();
                CurrentCompensation = currentPay;
                paymentHistory.Remove(currentPay);
            }

            gvCompensationHistory.DataSource = paymentHistory;
            gvCompensationHistory.DataBind();
        }

        private void PopulateData(Person person)
        {
            person.Id = PersonId;
            person.FirstName = tbFirstName.Text;
            person.LastName = tbLastName.Text;

            //It should be always true in this page.
            person.IsStrawMan = true;
        }

        private static Person GetPerson(int? id)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    return serviceClient.GetStrawmanDetailsById(id.Value);
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (IsDirty)
            {
                if (ValidateAndSave())
                {
                    lblSave.ShowInfoMessage(SuccessMessage);
                }
            }
            else
            {
                lblSave.ShowInfoMessage(SuccessMessage);
            }
        }


        protected void btnStartDate_Command(object sender, CommandEventArgs e)
        {
            if (!SaveDirty || ValidateAndSave())
            {
                Redirect(
                    string.Format(Constants.ApplicationPages.RedirectStartDateAndStrawmanFormat,
                                  Constants.ApplicationPages.CompensationDetail,
                                  PersonId,
                                  HttpUtility.UrlEncode((string)e.CommandArgument),
                                  1));
            }
        }

        protected override bool ValidateAndSave()
        {
            bool result = false;
            Page.Validate();
            if (Page.IsValid)
            {
                PersonId = SaveData();
                PopulateControls(GetPerson(PersonId.Value));
                result = PersonId.HasValue;

                ClearDirty();
            }

            return result;
        }

        private int? SaveData()
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    var person = new Person();
                    PopulateData(person);
                    var currentPay = CurrentCompensation;
                    if (PersonId.HasValue)
                    {
                        var PersonOld = GetPerson(PersonId.Value);
                        if (PersonOld.PaymentHistory != null && PersonOld.PaymentHistory.Any())
                        {
                            currentPay.StartDate = SettingsHelper.GetCurrentPMTime().Date;
                        }
                    }

                    var currentLogin = User.Identity.Name;

                    //Successfully Saved.
                    return serviceClient.SaveStrawman(person, currentPay, currentLogin);
                }
                catch (Exception exMessage)
                {
                    ExMessage = exMessage.Message;
                    lblSave.ShowErrorMessage(ExMessage);

                    serviceClient.Abort();
                    Page.Validate(valSummary.ValidationGroup);
                }
            }
            return null;
        }

        protected void imgCompensationDelete_OnClick(object sender, EventArgs e)
        {
            ImageButton imgDelete = sender as ImageButton;
            GridViewRow row = imgDelete.NamingContainer as GridViewRow;

            var btnStartDate = row.FindControl("btnStartDate") as LinkButton;
            var startDate = Convert.ToDateTime(btnStartDate.Text);

            using (var service = new PersonServiceClient())
            {
                service.DeletePay(PersonId.Value, startDate);
            }
            Person person = GetPerson(PersonId.Value);
            PopulateControls(person);

        }
    }
}

