using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.PersonService;
using System.Web.Security;

namespace PraticeManagement
{
    public partial class MarginTest : PracticeManagementPageBase
    {
        private Person selectedPersonValue;

        private Person SelectedPerson
        {
            get
            {
                if (selectedPersonValue == null && !string.IsNullOrEmpty(ddlPersonName.SelectedValue))
                {
                    using (PersonServiceClient serviceCLient = new PersonServiceClient())
                    {
                        try
                        {
                            selectedPersonValue = serviceCLient.GetPersonDetail(int.Parse(ddlPersonName.SelectedValue));
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

        protected override void Display()
        {
            // TODO: Remove the dummy code.
            personnelCompensation.StartDate = DateTime.Today;
            DataHelper.FillOneOffList(ddlPersonName, "-- Select a Person --", DateTime.Today);

            recruiterInfo.Person = new Person();
            personnelCompensation.PaymentsVisible =
                personnelCompensation.CompensationDateVisible =
                    personnelCompensation.DefaultHoursPerDayVisible =
                        personnelCompensation.SeniorityAndPracticeVisible = false;

            // Security            
            bool isAdmin = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
            grossMarginComputing.Visible = isAdmin;

        }

        protected void compensation_Changed(object sender, EventArgs e)
        {
            DoCompute();
        }

        private void DoCompute()
        {
            TextBox txtTargetMargin = (TextBox)whatIf.FindControl("txtTargetMargin");
            txtTargetMargin.Text = string.Empty;
            Page.Validate();
            if (Page.IsValid)
            {
                Person person = new Person();
                // Payment
                person.CurrentPay = personnelCompensation.Pay;
                bool isHourlyAmount =
                    person.CurrentPay.Timescale == TimescaleType._1099Ctc ||
                    person.CurrentPay.Timescale == TimescaleType.Hourly ||
                    person.CurrentPay.Timescale == TimescaleType.PercRevenue;

                if (isHourlyAmount)
                    person.CurrentPay.AmountHourly = person.CurrentPay.Amount;
                else
                    person.CurrentPay.AmountHourly = person.CurrentPay.Amount / Utils.Calendar.GetWorkingHoursInCurrentYear(whatIf.SelectedHorsPerWeek);
                // Commisions
                person.RecruiterCommission = recruiterInfo.RecruiterCommission;

                using (PersonServiceClient serviceClient = new PersonServiceClient())
                {
                    try
                    {
                        person.OverheadList =
                            new List<PersonOverhead>(serviceClient.GetPersonOverheadByTimescale(person.CurrentPay.Timescale));
                    }
                    catch (FaultException<ExceptionDetail>)
                    {
                        serviceClient.Abort();
                        throw;
                    }

                    if (!string.IsNullOrEmpty(ddlPersonName.SelectedValue) && ddlPersonName.SelectedValue != "-1")
                    {
                        person.Id = int.Parse(ddlPersonName.SelectedValue);
                    }
                    whatIf.Person = person;
                }
            }
            Page.Validate();
        }

        protected void ddlPersonName_SelectedIndexChanged(object sender, EventArgs e)
        {
            Person person = SelectedPerson;
            ClearControls();
            if (person != null && person.PaymentHistory != null)
            {
                PopulateControls(person.PaymentHistory[person.PaymentHistory.Count - 1]);
                if (person.RecruiterCommission != null)
                {
                    recruiterInfo.RecruiterCommission = person.RecruiterCommission;
                }
            }

            DoCompute();
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
        }

        private void ClearControls()
        {
            Pay pay = new Pay();
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

            whatIf.SetSliderDefaultValue();
        }
    }
}

