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
using PraticeManagement.Security;
using PraticeManagement.Utils;
using System.Web.UI.HtmlControls;

namespace PraticeManagement.Controls
{
    public partial class WhatIf : System.Web.UI.UserControl
    {
        private const string PersonKey = "PersonValue";
        private const string HorsPerWeekDefaultValue = "40";
        private const string BillRateDefaultValue = "120";
        private const string MLFText = "Minimum Load Factor (MLF)";
        private const string ClientDiscountDefaultValue = "0";
        private const string LoggedInPersonSalesCommissionKey = "LoggedInPersonSalesCommission";
        private const string ComputeRate = "ComputeRate";
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
                    txtClientDiscount.Text = ClientDiscountDefaultValue;
                    Page.Validate(ComputeRate);
                    if (Page.IsValid)
                    {
                        DisplayCalculatedRate();
                    }
                    else
                    {
                        ClearContents();
                    }
                }
            }
        }

        public DateTime EffectiveDate
        {
            get
            {
                return dtpEffectiveDate.DateValue != DateTime.MinValue ? dtpEffectiveDate.DateValue : DateTime.Now.Date;
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
            }
        }

        public bool IsMarginTestPage
        {
            set;
            get;
        }

        public bool HideCalculatedValues
        {
            get
            {
                var personListAnalyzer = new SeniorityAnalyzer(DataHelper.CurrentPerson);
                return personListAnalyzer.IsOtherGreater(Person);
            }
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

        public decimal ClientDiscount
        {
            get
            {

                decimal clientDiscount = 0.0M; // Default Sales Commission
                if (!string.IsNullOrEmpty(txtClientDiscount.Text))
                {
                    decimal.TryParse(txtClientDiscount.Text, out clientDiscount);
                    clientDiscount = (clientDiscount / 100);
                }
                return clientDiscount;
            }
        }

        public void SetSliderDefaultValue()
        {
            txtHorsPerWeekSlider.Text = HorsPerWeekDefaultValue;
            txtBillRateSlider.Text = BillRateDefaultValue;
            dtpEffectiveDate.DateValue = DateTime.Now.Date;
            txtClientDiscount.Text = ClientDiscountDefaultValue;
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

            if (!IsPostBack)
            {
                dtpEffectiveDate.DateValue = DateTime.Now.Date;
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


        protected void txtBillRateSlider_TextChanged(object sender, EventArgs e)
        {
            Page.Validate(ComputeRate);
            if (Page.IsValid && Person != null)
            {
                DisplayCalculatedRate();
            }
            else
            {
                ClearContents();
            }
        }



        protected void dtpEffectiveDate_SelectionChanged(object sender, EventArgs e)
        {
            Page.Validate(ComputeRate);
            if (Page.IsValid && Person != null)
            {
                DisplayCalculatedRate();
            }
            else
            {
                ClearContents();
            }
        }

        protected void txtClientDiscount_TextChanged(object sender, EventArgs e)
        {
            Page.Validate(ComputeRate);
            if ((Page.IsValid && Person != null && validatePercentage.IsMatch(txtClientDiscount.Text) ||
                                                    string.IsNullOrEmpty(txtClientDiscount.Text)))
            {
                DisplayCalculatedRate();
            }
            else
            {
                ClearContents();
            }
        }

        #region Projected rates

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

                    if (tmpPerson.PaymentHistory != null && tmpPerson.PaymentHistory.Count > 0)
                    {
                        tmpPerson.CurrentPay = tmpPerson.PaymentHistory.FirstOrDefault(c => EffectiveDate >= c.StartDate && (!c.EndDate.HasValue || EffectiveDate < c.EndDate))
                           ?? tmpPerson.PaymentHistory.First(c => EffectiveDate < c.StartDate);

                    }
                    ComputedFinancialsEx rate = serviceClient.CalculateProposedFinancialsPerson(tmpPerson, billRate, hoursPerWeek, ClientDiscount, IsMarginTestPage, EffectiveDate);

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

        private void SetBackgroundColorForMargin(decimal targetMargin, HtmlTableCell td, TextBox txt = null)
        {
            if (txt != null)
            {
                txt.Style["background-color"] = "White";
            }
            else
            {
                td.Style["background-color"] = "White";
            }


            int margin = (int)targetMargin;
            List<ClientMarginColorInfo> cmciList = new List<ClientMarginColorInfo>();

            if (Convert.ToBoolean(SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.IsDefaultMarginInfoEnabledForAllPersonsKey)))
            {
                cmciList = SettingsHelper.GetMarginColorInfoDefaults(DefaultGoalType.Person);
            }

            if (cmciList != null)
            {
                foreach (var item in cmciList)
                {
                    if (margin >= item.StartRange && margin <= item.EndRange)
                    {
                        if (txt != null)
                        {
                            txt.Style["background-color"] = item.ColorInfo.ColorValue;
                        }
                        else
                        {
                            td.Style["background-color"] = item.ColorInfo.ColorValue;
                        }
                        break;
                    }
                }
            }
        }

        private void DisplayRate(ComputedFinancialsEx rate)
        {
            lblMonthlyRevenueWithoutRecruiting.Text = rate.Revenue.ToString();
            if (!HideCalculatedValues)
            {
                lblMonthlyGrossMarginWithoutRecruiting.Text = rate.MarginWithoutRecruiting.ToString();
                lblMonthlyCogsWithoutRecruiting.Text = rate.CogsWithoutRecruiting.ToString();
                lblTargetMarginWithoutRecruiting.Text =
                    string.Format(Constants.Formatting.PercentageFormat, rate.TargetMarginWithoutRecruiting);
            }
            else
            {
                lblMonthlyGrossMarginWithoutRecruiting.Text =
                lblMonthlyCogsWithoutRecruiting.Text =
                lblTargetMarginWithoutRecruiting.Text =
                   Resources.Controls.HiddenCellText;
                lblMonthlyCogsWithoutRecruiting.CssClass = "Cogs";
                lblMonthlyGrossMarginWithoutRecruiting.CssClass = "Margin";
            }

            SetBackgroundColorForMargin(rate.TargetMarginWithoutRecruiting, tdTargetMarginWithoutRecruiting);

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

        public void ClearContents()
        {
            lblMonthlyRevenueWithoutRecruiting.Text =
            lblMonthlyGrossMarginWithoutRecruiting.Text =
            lblMonthlyCogsWithoutRecruiting.Text =
            lblTargetMarginWithoutRecruiting.Text =
            lblMonthlyRevenueWithoutRecruiting.CssClass =
            lblMonthlyCogsWithoutRecruiting.CssClass =
            lblMonthlyGrossMarginWithoutRecruiting.CssClass = string.Empty;
           
            gvOverheadWhatIf.Visible = false;
            gvOverheadWhatIf.DataBind();
        }

        #endregion

        protected void cvWithTerminationDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (Person != null && Person.TerminationDate.HasValue)
            {
                args.IsValid = EffectiveDate <= Person.TerminationDate;
            }
        }

        protected void cvNotHavingCompensation_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (Person != null && cvWithTerminationDate.IsValid && Person.PaymentHistory != null)
            {
                args.IsValid = Person.PaymentHistory.Any(c => EffectiveDate >= c.StartDate && (!c.EndDate.HasValue || EffectiveDate < c.EndDate));
            }

        }

    }
}

