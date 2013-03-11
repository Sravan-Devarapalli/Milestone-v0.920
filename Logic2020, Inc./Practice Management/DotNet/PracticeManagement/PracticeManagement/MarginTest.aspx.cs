using System;
using System.Linq;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.PersonService;
using PraticeManagement.Security;
using PraticeManagement.Utils;

namespace PraticeManagement
{
    public partial class MarginTest : PracticeManagementPageBase
    {
        private Person selectedPersonValue;

        public const string DirtyFunctionScript = @" function getDirty() {{ return false; }}";

        private const string TblEffectiveDate = "tblEffectiveDate";

        private Person SelectedPerson
        {
            get
            {
                if (selectedPersonValue == null && !string.IsNullOrEmpty(Selectedman) && Selectedman != "-1")
                {
                    using (PersonServiceClient serviceCLient = new PersonServiceClient())
                    {
                        try
                        {
                            selectedPersonValue = serviceCLient.GetPersonDetail(int.Parse(Selectedman));
                        }
                        catch (CommunicationException)
                        {
                            serviceCLient.Abort();
                            throw;
                        }
                    }
                }

                return selectedPersonValue;
            }
            set
            {
                selectedPersonValue = value;
            }
        }

        private string Selectedman
        {
            get
            {
                var selectedValue = rbSelectPerson.Checked ? ddlPersonName.SelectedValue : ddlStrawmanName.SelectedValue;
                return selectedValue;
            }
        }

        protected override void Display()
        {
            // TODO: Remove the dummy code.
            personnelCompensation.StartDate = DateTime.Today;
            DataHelper.FillOneOffList(ddlPersonName, "-- Select a Person --", DateTime.Today);
            DataHelper.FillStrawManList(ddlStrawmanName, "-- Select a Strawman --");

            personnelCompensation.CompensationDateVisible =
            personnelCompensation.TitleAndPracticeVisible = false;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            ShowDetails();
            ScriptManager.RegisterStartupScript(this, this.GetType(), this.ClientID, DirtyFunctionScript, true);
        }

        protected void Reset_Clicked(object sender, EventArgs e)
        {
            ResetControls();
        }

        protected void compensation_Changed(object sender, EventArgs e)
        {
            DoCompute(SelectedPerson);
        }

        protected void rbMarginTest_CheckedChanged(object sender, EventArgs e)
        {
            ResetControls();
        }

        public void ResetControls()
        {
            ddlPersonName.SelectedIndex = 0;
            ddlStrawmanName.SelectedIndex = 0;
            ClearControls();
            whatIf.Person = null;
            whatIf.ClearContents();
            personnelCompensation.Timescale = TimescaleType.Hourly;
        }

        private void ShowDetails()
        {
            ddlPersonName.Visible = rbSelectPerson.Checked;
            ddlStrawmanName.Visible = rbSelectStrawman.Checked;
            personnelCompensation.Visible = !(rbSelectPerson.Checked || rbSelectStrawman.Checked);
            whatIf.FindControl(TblEffectiveDate).Visible = !(personnelCompensation.Visible);
        }

        private void DoCompute(Person selectedPerson)
        {
            Page.Validate();
            if (Page.IsValid)
            {
                Person person = new Person();

                if (selectedPerson != null)
                {
                    person.Id = selectedPerson.Id;
                    person.Seniority = selectedPerson.Seniority;
                    person.PaymentHistory = selectedPerson.PaymentHistory;
                    person.TerminationDate = selectedPerson.TerminationDate;
                }
                else
                {
                    // Payment
                    person.CurrentPay = personnelCompensation.Pay;
                }
                whatIf.Person = person;
            }
            else
            {
                whatIf.ClearContents();
            }
            Page.Validate();
        }

        protected void ddlPersonName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ddl = sender as DropDownList;

            if (ddl.SelectedIndex == 0)
            {
                ResetControls();
            }
            else
            {
                whatIf.Person = null;
                Person person = SelectedPerson;
                ClearControls();
                if (person != null && person.PaymentHistory != null)
                {

                    var today = (SettingsHelper.GetCurrentPMTime()).Date;

                    var compensation =
                        person.PaymentHistory.FirstOrDefault(c => today >= c.StartDate && (!c.EndDate.HasValue || today < c.EndDate))
                        ?? person.PaymentHistory.First(c => today < c.StartDate);
                    PopulateControls(compensation);


                    var personListAnalyzer = new SeniorityAnalyzer(DataHelper.CurrentPerson);
                    if (personListAnalyzer.IsOtherGreater(person))
                    {
                        personnelCompensation.Visible = false;
                        //whatIf.HideCalculatedValues = true;
                    }
                    else
                    {
                        personnelCompensation.Visible = true;
                        //whatIf.HideCalculatedValues = false;
                    }
                }
                else
                {
                    personnelCompensation.Visible = true;
                }
                DoCompute(person);
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
            personnelCompensation.TitleId = pay.TitleId;
            personnelCompensation.SLTApproval = pay.SLTApproval;
        }

        private void ClearControls()
        {
            Pay pay = new Pay();
            personnelCompensation.StartDate = pay.StartDate;
            personnelCompensation.EndDate = pay.EndDate;
            personnelCompensation.Timescale = pay.Timescale;
            personnelCompensation.Amount = pay.Amount;
            personnelCompensation.VacationDays = pay.VacationDays;
            personnelCompensation.IsYearBonus = pay.IsYearBonus;
            personnelCompensation.BonusAmount = pay.BonusAmount;
            personnelCompensation.BonusHoursToCollect = pay.BonusHoursToCollect;
            personnelCompensation.TitleId = pay.TitleId;
            personnelCompensation.SLTApproval = pay.SLTApproval;

            whatIf.SetSliderDefaultValue();
        }
    }
}

