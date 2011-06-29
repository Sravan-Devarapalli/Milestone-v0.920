using System;
using System.Web.UI.WebControls;
using PraticeManagement.Utils;
using DataTransferObjects;
using PraticeManagement.Configuration;

namespace PraticeManagement.Controls
{
    public partial class PersonnelCompensation : System.Web.UI.UserControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets a selected start date.
        /// </summary>
        public DateTime? StartDate
        {
            get
            {
                return dpStartDate.DateValue != DateTime.MinValue ? (DateTime?)dpStartDate.DateValue : null;
            }
            set
            {
                dpStartDate.DateValue = value.HasValue ? value.Value : DateTime.MinValue;
                lblStartDate.Text = dpStartDate.TextValue;
                hidOldStartDate.Value = value.HasValue ? value.Value.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets whether the Start Date field is read-only.
        /// </summary>
        public bool StartDateReadOnly
        {
            get
            {
                return !dpStartDate.Visible;
            }
            set
            {
                dpStartDate.Visible = !value;
                lblStartDate.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets a selected end date.
        /// </summary>
        public DateTime? EndDate
        {
            get
            {
                return dpEndDate.DateValue != DateTime.MinValue ? (DateTime?)dpEndDate.DateValue.AddDays(1) : null;
            }
            set
            {
                dpEndDate.DateValue = value.HasValue ? value.Value.AddDays(-1) : DateTime.MinValue;
                hidOldEndDate.Value = value.HasValue ? value.Value.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Gets a value of the StartDate before it was edited.
        /// </summary>
        public DateTime? OldStartDate
        {
            get
            {
                DateTime result;
                if (!DateTime.TryParse(hidOldStartDate.Value, out result))
                {
                    return null;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value of the EndDate before it was edited.
        /// </summary>
        public DateTime? OldEndDate
        {
            get
            {
                DateTime result;
                if (!DateTime.TryParse(hidOldEndDate.Value, out result))
                {
                    return null;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets or sets a selected timescale.
        /// </summary>
        public TimescaleType Timescale
        {
            get
            {
                TimescaleType result;
                if (rbtnSalaryAnnual.Checked)
                {
                    result = TimescaleType.Salary;
                }
                else if (rbtnSalaryHourly.Checked)
                {
                    result = TimescaleType.Hourly;
                }
                else if (rbtn1099Ctc.Checked)
                {
                    result = TimescaleType._1099Ctc;
                }
                else
                {
                    result = TimescaleType.PercRevenue;
                }

                return result;
            }
            set
            {
                if (value == TimescaleType.Salary)
                {
                    rbtnSalaryHourly.Checked =
                        rbtn1099Ctc.Checked =
                        rbtnPercentRevenue.Checked = false;
                    rbtnSalaryAnnual.Checked = true;
                    // Clear textbox value                    
                    txtSalaryHourly.Text = String.Empty;
                    txt1099Ctc.Text = String.Empty;
                }
                else if (value == TimescaleType.Hourly)
                {
                    rbtnSalaryAnnual.Checked =
                        rbtn1099Ctc.Checked =
                        rbtnPercentRevenue.Checked = false;
                    rbtnSalaryHourly.Checked = true;
                    // Clear textbox value
                    txtSalaryAnnual.Text = String.Empty;
                    txt1099Ctc.Text = String.Empty;
                }
                else if (value == TimescaleType._1099Ctc)
                {
                    rbtnSalaryAnnual.Checked =
                        rbtnSalaryHourly.Checked =
                        rbtnPercentRevenue.Checked = false;
                    rbtn1099Ctc.Checked = true;
                    // Clear textbox value
                    txtSalaryAnnual.Text = String.Empty;
                    txtSalaryHourly.Text = String.Empty;
                }
                else
                {
                    rbtnSalaryAnnual.Checked =
                        rbtnSalaryHourly.Checked =
                        rbtn1099Ctc.Checked = false;
                    rbtnPercentRevenue.Checked = true;
                    txtPercRevenue.Text = string.Empty;
                }

                UpdateCompensationState();
            }
        }

        /// <summary>
        /// Gets or sets a selected rate;
        /// </summary>
        public decimal? Amount
        {
            get
            {
                string txtResult = string.Empty;
                switch (Timescale)
                {
                    case TimescaleType.Salary:
                        txtResult = txtSalaryAnnual.Text;
                        break;
                    case TimescaleType.Hourly:
                        txtResult = txtSalaryHourly.Text;
                        break;
                    case TimescaleType._1099Ctc:
                        txtResult = txt1099Ctc.Text;
                        break;
                    case TimescaleType.PercRevenue:
                        txtResult = txtPercRevenue.Text;
                        break;
                }

                decimal result;
                if (decimal.TryParse(txtResult, out result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                string inputValue = value.HasValue ? value.ToString() : string.Empty;

                switch (Timescale)
                {
                    case TimescaleType.Salary:
                        txtSalaryAnnual.Text = inputValue;
                        txtSalaryHourly.Text =
                            txt1099Ctc.Text =
                            txtPercRevenue.Text = string.Empty;
                        break;

                    case TimescaleType.Hourly:
                        txtSalaryHourly.Text = inputValue;
                        txtSalaryAnnual.Text =
                            txt1099Ctc.Text =
                            txtPercRevenue.Text = string.Empty;
                        break;

                    case TimescaleType._1099Ctc:
                        txt1099Ctc.Text = inputValue;
                        txtSalaryHourly.Text =
                        txtSalaryAnnual.Text =
                            txtPercRevenue.Text = string.Empty;
                        break;

                    case TimescaleType.PercRevenue:
                        txtPercRevenue.Text = inputValue;
                        txt1099Ctc.Text =
                        txtSalaryHourly.Text =
                        txtSalaryAnnual.Text = string.Empty;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets a number of the annual vacation days
        /// </summary>
        public int? VacationDays
        {
            get
            {
                return
                    !string.IsNullOrEmpty(txtVacationDays.Text) && !(rbtn1099Ctc.Checked || rbtnPercentRevenue.Checked) ?
                    (int?)int.Parse(txtVacationDays.Text) : null;
            }
            set
            {
                txtVacationDays.Text = value.HasValue ? value.Value.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets a number of the vacation days per year.
        /// </summary>
        public int? TimesPaidPerMonth
        {
            get
            {
                return !string.IsNullOrEmpty(ddlPaidPerMonth.SelectedValue) ?
                    (int?)int.Parse(ddlPaidPerMonth.SelectedValue) : null;
            }
            set
            {
                ddlPaidPerMonth.SelectedIndex =
                    ddlPaidPerMonth.Items.IndexOf(ddlPaidPerMonth.Items.FindByValue(
                    value.HasValue ? value.Value.ToString() : string.Empty));
            }
        }

        /// <summary>
        /// Gets or sets a paryent terms.
        /// </summary>
        public int? Terms
        {
            get
            {
                return
                    !string.IsNullOrEmpty(ddlPaymentTerms.SelectedValue) ?
                    (int?)int.Parse(ddlPaymentTerms.SelectedValue) : null;
            }
            set
            {
                ddlPaymentTerms.SelectedIndex =
                    ddlPaymentTerms.Items.IndexOf(ddlPaymentTerms.Items.FindByValue(
                    value.HasValue ? value.Value.ToString() : string.Empty));
            }
        }

        /// <summary>
        /// Gets or setgs if the bonus is year bonus.
        /// </summary>
        public bool IsYearBonus
        {
            get
            {
                return rbtnBonusAnnual.Checked;
            }
            set
            {
                if (value)
                {
                    rbtnBonusHourly.Checked = false;
                    rbtnBonusAnnual.Checked = true;
                }
                else
                {
                    rbtnBonusAnnual.Checked = false;
                    rbtnBonusHourly.Checked = true;
                }

                UpdateBonusState();
            }
        }

        /// <summary>
        /// Gets or sets a bonus amount.
        /// </summary>
        public decimal BonusAmount
        {
            get
            {
                decimal result;
                string tmpResult = IsYearBonus ? txtBonusAnnual.Text : txtBonusHourly.Text;
                if (!(rbtn1099Ctc.Checked || rbtnPercentRevenue.Checked))
                {
                    decimal.TryParse(tmpResult, out result);
                }
                else
                {
                    result = 0M;
                }

                return result;
            }
            set
            {
                if (IsYearBonus)
                {
                    txtBonusAnnual.Text = value.ToString();
                }
                else
                {
                    txtBonusHourly.Text = value.ToString();
                }
            }
        }

        /// <summary>
        /// Gets or sets a periodicity of the bonus payments.
        /// </summary>
        public int? BonusHoursToCollect
        {
            get
            {
                string tmpResult = !IsYearBonus ? txtBonusDuration.Text : string.Empty;
                int result;
                return !(rbtn1099Ctc.Checked || rbtnPercentRevenue.Checked) && int.TryParse(tmpResult, out result) ? (int?)result : null;
            }
            set
            {
                txtBonusDuration.Text = !IsYearBonus && value.HasValue ? value.Value.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets a default number of the billable hours per day for the person.
        /// </summary>
        public decimal DefaultHoursPerDay
        {
            get
            {
                return decimal.Parse(txtDefaultHoursPerDay.Text);
            }
            set
            {
                txtDefaultHoursPerDay.Text = value.ToString();
            }
        }

        /// <summary>
        /// Sets whether the table row with Payments data is Visible.
        /// </summary>
        public bool PaymentsVisible
        {
            set
            {
                trPayments.Visible = value;
            }
        }

        /// <summary>
        /// Sets whether the table row witn DefaultHoursPerDay data is Visible.
        /// </summary>
        public bool DefaultHoursPerDayVisible
        {
            set
            {
                trDefaultHoursPerDay.Visible = value;
            }
        }

        /// <summary>
        /// Sets whether the table row with Compenasation data is Visible.
        /// </summary>
        public bool CompensationDateVisible
        {
            set
            {
                trCompensationDate.Visible = value;
            }
        }

        /// <summary>
        /// Sets whether the table row with Seniority And Practice fields is Visible.
        /// </summary>
        public bool SeniorityAndPracticeVisible
        {
            set
            {
                trSeniorityAndPractice.Visible = value;
            }
        }

        public int? PracticeId
        {
            set
            {
                if (value.HasValue)
                {
                    ListItem selectedPractice = ddlPractice.Items.FindByValue(value.Value.ToString());
                    if (selectedPractice == null)
                    {
                        var practices = DataHelper.GetPracticeById(value);
                        if (practices != null && practices.Length > 0)
                        {

                            selectedPractice = new ListItem(practices[0].Name, practices[0].Id.ToString());
                            ddlPractice.Items.Add(selectedPractice);
                            ddlPractice.SortByText();
                            ddlPractice.SelectedValue = selectedPractice.Value;
                        }
                    }
                    else
                    {
                        ddlPractice.SelectedValue = selectedPractice.Value;
                    }
                }
            }
            get
            {
                if (ddlPractice.SelectedIndex > 0)
                {
                    return int.Parse(ddlPractice.SelectedValue);
                }
                return null;
            }
        }

        public int? SeniorityId
        {
            set
            {
                if (value.HasValue)
                {
                    ListItem selectedSeniority = ddlSeniority.Items.FindByValue(value.Value.ToString());
                    if (selectedSeniority != null)
                    {

                        ddlSeniority.SelectedValue = selectedSeniority.Value;
                    }
                }
            }
            get
            {
                if (ddlSeniority.SelectedIndex > 0)
                {
                    return int.Parse(ddlSeniority.SelectedValue);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets a selected <see cref="Pay"/>.
        /// </summary>
        public Pay Pay
        {
            get
            {
                Pay result = new Pay();

                if (StartDate.HasValue)
                    result.StartDate = StartDate.Value;
                result.EndDate = EndDate;
                result.Timescale = Timescale;
                result.Amount = Amount.Value;
                result.VacationDays = VacationDays;
                result.TimesPaidPerMonth = TimesPaidPerMonth;
                result.Terms = Terms;
                result.IsYearBonus = IsYearBonus;
                result.BonusAmount = BonusAmount;
                result.BonusHoursToCollect = BonusHoursToCollect;
                result.DefaultHoursPerDay = DefaultHoursPerDay;
                result.OldStartDate = OldStartDate;
                result.OldEndDate = OldEndDate;
                result.SeniorityId = SeniorityId;
                result.PracticeId = PracticeId;

                return result;
            }
        }

        /// <summary>
        /// Gets or sets whether the control is read-only.
        /// </summary>
        public bool ReadOnly
        {
            get
            {
                return txtVacationDays.ReadOnly;
            }
            set
            {
                dpStartDate.ReadOnly = dpEndDate.ReadOnly = txtSalaryAnnual.ReadOnly = txtSalaryHourly.ReadOnly =
                    txtBonusHourly.ReadOnly = txtBonusDuration.ReadOnly = txt1099Ctc.ReadOnly =
                    txtBonusAnnual.ReadOnly = txtDefaultHoursPerDay.ReadOnly = txtVacationDays.ReadOnly = value;

                rbtnSalaryAnnual.Enabled = rbtnSalaryHourly.Enabled = rbtnBonusHourly.Enabled =
                    rbtn1099Ctc.Enabled = rbtnBonusAnnual.Enabled =
                    ddlPaidPerMonth.Enabled = ddlPaymentTerms.Enabled =
                    rbtnPercentRevenue.Enabled = !value;

                UpdateCompensationState();
            }
        }

        /// <summary>
        /// Gets or sets whether the Auto-posting back is enabled.
        /// </summary>
        public bool AutoPostBack
        {
            get
            {
                return txtSalaryAnnual.AutoPostBack;
            }
            set
            {
                txt1099Ctc.AutoPostBack = txtBonusAnnual.AutoPostBack = txtBonusDuration.AutoPostBack =
                    txtBonusHourly.AutoPostBack = txtDefaultHoursPerDay.AutoPostBack =
                    txtSalaryAnnual.AutoPostBack = txtSalaryHourly.AutoPostBack =
                    txtVacationDays.AutoPostBack = value;
            }
        }

        public event EventHandler CompensationMethodChanged;
        public event EventHandler PeriodChanged;
        public event EventHandler CompensationChanged;

        #endregion

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                TermsConfigurationSection terms = TermsConfigurationSection.Current;
                ddlPaymentTerms.DataSource = terms != null ? terms.Terms : null;
                ddlPaymentTerms.DataBind();

                DataHelper.FillSenioritiesList(ddlSeniority, "-- Select Seniority --");
                DataHelper.FillPracticeListOnlyActive(ddlPractice, "-- Select Practice Area --");

            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            UpdateCompensationState();
        }

        protected void Compensation_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCompensationState();

            if (CompensationMethodChanged != null)
            {
                CompensationMethodChanged(this, e);
            }
        }
        protected void custValSeniority_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = ddlSeniority.SelectedIndex > 0;
        }
        protected void custValPractice_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = ddlPractice.SelectedIndex > 0;
        }

        private void UpdateCompensationState()
        {
            txtSalaryAnnual.Enabled = reqSalaryAnnual.Enabled = compSalaryAnnual.Enabled =
                rbtnSalaryAnnual.Checked;
            txtSalaryHourly.Enabled = reqSalaryHourly.Enabled = compSalaryHourly.Enabled =
                rbtnSalaryHourly.Checked;
            txt1099Ctc.Enabled = req1099Ctc.Enabled = comp1099Ctc.Enabled = rbtn1099Ctc.Checked;
            txtPercRevenue.Enabled = reqPercRevenue.Enabled = compPercRevenue.Enabled = rbtnPercentRevenue.Checked;

            // Bonus and vacation are not available for the 1099 employees.
            UpdateBonusState();
            txtVacationDays.Enabled = !(rbtn1099Ctc.Checked || rbtnPercentRevenue.Checked);
        }

        protected void Bonus_CheckedChanged(object sender, EventArgs e)
        {
            UpdateBonusState();
        }

        private void UpdateBonusState()
        {
            // Bonus and vacation are not available for the 1099 employees.
            txtBonusHourly.Enabled = txtBonusDuration.Enabled =
                compBonusHourly.Enabled = reqBonusDuration.Enabled = compBonusDuration.Enabled =
                rbtnBonusHourly.Checked && !(rbtn1099Ctc.Checked || rbtnPercentRevenue.Checked);
            txtBonusAnnual.Enabled = compBonusAnnual.Enabled = rbtnBonusAnnual.Checked && !rbtn1099Ctc.Checked;

            rbtnBonusHourly.Enabled = rbtnBonusAnnual.Enabled = !(rbtn1099Ctc.Checked || rbtnPercentRevenue.Checked);

            ForceToZero(txtBonusAnnual);
            ForceToZero(txtBonusDuration);
            ForceToZero(txtBonusHourly);
            ForceToZero(txtVacationDays);
        }

        private void ForceToZero(TextBox textBox)
        {
            if (textBox.Enabled == false)
                textBox.Text = "0";
        }

        protected void reqBonusDuration_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = BonusAmount == 0 || BonusHoursToCollect.HasValue;
        }

        protected void Period_SelectionChanged(object sender, EventArgs e)
        {
            if (PeriodChanged != null)
            {
                PeriodChanged(this, e);
            }
        }

        protected void Compensation_TextChanged(object sender, EventArgs e)
        {
            if (CompensationChanged != null)
            {
                CompensationChanged(this, e);
            }
        }
    }
}

