using System;
using System.ComponentModel;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.PersonService;
using Resources;
using System.Linq;
using System.Collections.Generic;

namespace PraticeManagement.Controls
{
    public partial class WhatIf : System.Web.UI.UserControl
    {
        private const string PersonKey = "PersonValue";
        private const string HorsPerWeekDefaultValue = "40";
        private const string BillRateDefaultValue = "120";
        //private const string SalesCommissionLineText = "Sales Commission (SCPH)";
        private const string MLFText = "Minimum Load Factor (MLF)";
        private const string LoggedInPersonSalesCommissionKey = "LoggedInPersonSalesCommission";
        private Regex validatePercentage =
            new Regex("(\\d+\\.?\\d*)%?", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public Person Person
        {
            get
            {
                return ViewState[PersonKey] as Person;
            }
            set
            {
                ViewState[PersonKey] = value;
                if (value != null)
                {
                    txtClientDiscount.Text = "0%";
                    DisplayCalculatedRate();
                }
            }
        }

        public decimal SelectedHorsPerWeek
        {
            get
            {
                decimal hoursPerWeek =
                                !string.IsNullOrEmpty(txtHorsPerWeekSlider_BoundControl.Text) ?
                                decimal.Parse(txtHorsPerWeekSlider_BoundControl.Text) : (decimal)sldHoursPerMonth.Minimum;
                return hoursPerWeek;
            }
        }
        [DefaultValue(false)]
        public bool DisplayDefinedTermsAndCalcs
        {
            set
            {
                this.tdgrossMarginComputing.Visible = value;
                if (value)
                {
                    bool isAdmin = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
                    grossMarginComputing.Visible = isAdmin;
                }
            }
        }

        public bool IsMarginTestPage
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets whether the Target Margin should be displayed.
        /// </summary>
        [DefaultValue(false)]
        public bool DisplayTargetMargin
        {
            get
            {
                return trTargetMargin.Visible;
            }
            set
            {
                trTargetMargin.Visible = value;
            }
        }

        //public decimal DefaultSalesCommission
        //{
        //    get
        //    {

        //        decimal tempDefaultSalesCommission = 0.0M; // Default Sales Commission 
                //Match m = validatePercentage.Match(txtDefaultSalesCommission.Text);
                //decimal.TryParse(m.Groups[1].Captures[0].Value, out tempDefaultSalesCommission);
                //tempDefaultSalesCommission = (tempDefaultSalesCommission / 100);
        //        return tempDefaultSalesCommission;
        //    }
        //}

        public decimal ClientDiscount
        {
            get
            {

                decimal clientDiscount = 0.0M; // Default Sales Commission
                if (!string.IsNullOrEmpty(txtClientDiscount.Text))
                {
                    Match m = validatePercentage.Match(txtClientDiscount.Text);
                    decimal.TryParse(m.Groups[1].Captures[0].Value, out clientDiscount);
                    clientDiscount = (clientDiscount / 100);
                }
                return clientDiscount;
            }
        }

        /// <summary>
        /// Gets or sets whether the Target Margin can be changed and the rate recalculated according to this change.
        /// </summary>
        [DefaultValue(false)]
        public bool TargetMarginReadOnly
        {
            get
            {
                return lblTargetMargin.Visible;
            }
            set
            {
                lblTargetMargin.Visible = value;
                txtTargetMargin.Visible = custTargetMargin.Enabled = !value;
            }
        }

        public void SetSliderDefaultValue()
        {
            txtHorsPerWeekSlider.Text = HorsPerWeekDefaultValue;
            txtBillRateSlider.Text = BillRateDefaultValue;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Slider labels
            lblHoursMin.Text = sldHoursPerMonth.Minimum.ToString();
            lblHoursMax.Text = sldHoursPerMonth.Maximum.ToString();

            lblBillRateMin.Text = sldBillRate.Minimum.ToString();
            lblBillRateMax.Text = sldBillRate.Maximum.ToString();

            // Security            
            bool isAdmin = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
            gvOverheadWhatIf.Visible = isAdmin;
        }

        protected void custTargetMargin_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = validatePercentage.IsMatch(e.Value);
            if (e.IsValid)
            {
                Match m = validatePercentage.Match(e.Value);
                decimal value;
                e.IsValid = decimal.TryParse(m.Groups[1].Captures[0].Value, out value) && value < 100M;
            }
        }

        protected void custDefaultSalesCommision_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = validatePercentage.IsMatch(e.Value);
            if (e.IsValid)
            {
                Match m = validatePercentage.Match(e.Value);
                decimal value;
                e.IsValid = decimal.TryParse(m.Groups[1].Captures[0].Value, out value) && value >= 0.0M && value <= 10M;
            }
        }

        protected void custClientDiscount_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (!string.IsNullOrEmpty(cvClientDiscount.Text))
            {
                e.IsValid = validatePercentage.IsMatch(e.Value);
                if (e.IsValid)
                {
                    Match m = validatePercentage.Match(e.Value);
                    decimal value;
                    e.IsValid = decimal.TryParse(m.Groups[1].Captures[0].Value, out value);
                }
            }
            else
            {
                e.IsValid = true;
            }
        }

        protected void txtBillRateSlider_TextChanged(object sender, EventArgs e)
        {
            Page.Validate("ComputeRate");
            if (Page.IsValid && Person != null)
            {
                DisplayCalculatedRate();
            }
        }

        protected void txtTargetMargin_TextChanged(object sender, EventArgs e)
        {
            Page.Validate("ComputeRate");
            if (Page.IsValid && Person != null)
            {
                DisplayFromTargetMargin();
            }
        }

        //protected void txtDefaultSalesCommission_TextChanged(object sender, EventArgs e)
        //{
        //    Page.Validate("ComputeRate");
        //    if (Page.IsValid && Person != null && validatePercentage.IsMatch(txtDefaultSalesCommission.Text))
        //    {
        //        DisplayCalculatedRate();
        //        //DisplayFromTargetMargin();
        //    }
        //}

        protected void txtClientDiscount_TextChanged(object sender, EventArgs e)
        {
            Page.Validate("ComputeRate");
            if (Page.IsValid && Person != null && (validatePercentage.IsMatch(txtClientDiscount.Text) ||
                                                    string.IsNullOrEmpty(txtClientDiscount.Text)))
            {
                DisplayCalculatedRate();
                //DisplayFromTargetMargin();
            }
        }

        #region Projected rates

        private void DisplayFromTargetMargin()
        {
            if (validatePercentage.IsMatch(txtTargetMargin.Text))
            {
                decimal targetMargin;
                Match m = validatePercentage.Match(txtTargetMargin.Text);
                decimal.TryParse(m.Groups[1].Captures[0].Value, out targetMargin);

                using (PersonServiceClient serviceClient = new PersonServiceClient())
                {
                    //List<PersonOverhead> overheads = Person.OverheadList;
                    try
                    {
                        int id = Person != null && Person.Id.HasValue ? Person.Id.Value : 0;
                        decimal hoursPerWeek =
                            !string.IsNullOrEmpty(txtHorsPerWeekSlider_BoundControl.Text) ?
                            decimal.Parse(txtHorsPerWeekSlider_BoundControl.Text) : (decimal)sldHoursPerMonth.Minimum;

                        Person tmpPerson = Person;
                        tmpPerson.OverheadList = null;

                        ComputedFinancialsEx rate =
                            serviceClient.CalculateProposedFinancialsPersonTargetMargin(tmpPerson, targetMargin, hoursPerWeek, ClientDiscount,IsMarginTestPage);

                        DisplayRate(rate);

                        if (rate.BillRate.HasValue)
                        {
                            txtBillRateSlider.Text = txtBillRateSlider_BoundControl.Text =
                                Math.Round(rate.BillRate.Value.Value).ToString();
                        }
                    }
                    catch (FaultException<ExceptionDetail>)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                    //finally
                    //{
                    //    Person.OverheadList = overheads;
                    //}
                }
            }
        }

        private void DisplayCalculatedRate()
        {
            decimal billRate =
                !string.IsNullOrEmpty(txtBillRateSlider_BoundControl.Text) ?
                decimal.Parse(txtBillRateSlider_BoundControl.Text) : (decimal)sldBillRate.Minimum;

            pnlWhatIf.Visible = true;

            using (PersonServiceClient serviceClient = new PersonServiceClient())
            {
                try
                {
                    decimal hoursPerWeek =
                        !string.IsNullOrEmpty(txtHorsPerWeekSlider_BoundControl.Text) ?
                        decimal.Parse(txtHorsPerWeekSlider_BoundControl.Text) : (decimal)sldHoursPerMonth.Minimum;

                    Person tmpPerson = Person;
                    tmpPerson.OverheadList = null;

                    ComputedFinancialsEx rate = serviceClient.CalculateProposedFinancialsPerson(tmpPerson, billRate, hoursPerWeek, ClientDiscount, IsMarginTestPage);

                    DisplayRate(rate);

                }
                catch (FaultException<ExceptionDetail> exception)
                {
                    serviceClient.Abort();
                    pnlWhatIf.Visible = false;
                    mlMessage.ShowErrorMessage(Messages.WhatIfError, exception.Message);
                }
            }
        }

        private void DisplayRate(ComputedFinancialsEx rate)
        {
            lblMonthlyRevenue.Text = lblMonthlyRevenueWithoutRecruiting.Text = rate.Revenue.ToString();
            lblMonthlyGogs.Text = rate.Cogs.ToString();
            lblMonthlyGrossMargin.Text = rate.GrossMargin.ToString();
            txtTargetMargin.Text = lblTargetMargin.Text =
                string.Format(Constants.Formatting.PercentageFormat, rate.TargetMargin);

            lblMonthlyGrossMarginWithoutRecruiting.Text = rate.MarginWithoutRecruiting.ToString();
            lblMonthlyCogsWithoutRecruiting.Text = rate.CogsWithoutRecruiting.ToString();
            lblTargetMarginWithoutRecruiting.Text =
                string.Format(Constants.Formatting.PercentageFormat, rate.TargetMarginWithoutRecruiting);

            var overheads = rate.OverheadList;
            var mlf = overheads.Find(oh => oh.Name == MLFText);            
            if (mlf != null)
            {
                overheads.Remove(mlf);
                overheads.Insert(overheads.Count(), mlf);
            }
            gvOverheadWhatIf.DataSource = overheads;
            gvOverheadWhatIf.DataBind();

            if (gvOverheadWhatIf.FooterRow != null)
            {
                Label lblLoadedHourlyRate =
                    gvOverheadWhatIf.FooterRow.FindControl("lblLoadedHourlyRate") as Label;
                lblLoadedHourlyRate.Text = rate.LoadedHourlyRate.ToString();
            }
        }

        #endregion

    }
}
