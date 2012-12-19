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

        public bool EndDateReadOnly
        {
            get
            {
                return !dpEndDate.Visible;
            }
            set
            {
                lblEndDate.Text = dpEndDate.DateValue == DateTime.MinValue ? string.Empty : dpEndDate.DateValue.ToShortDateString();
                dpEndDate.Visible = !value;
                lblEndDate.Visible = value;
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
                int vacationDays = 0;
                int.TryParse(txtVacationDays.Text, out vacationDays);
                if (IsStrawmanMode)
                {
                    vacationDays = vacationDays / 8;
                }
                return !string.IsNullOrEmpty(txtVacationDays.Text) && !(rbtn1099Ctc.Checked || rbtnPercentRevenue.Checked) ?
                (int?)vacationDays : null;

            }
            set
            {
                int vacationDays = value.HasValue ? value.Value : 0;
                if (IsStrawmanMode)
                {
                    vacationDays = vacationDays * 8;
                }
                txtVacationDays.Text = vacationDays.ToString();
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
        public bool TitleAndPracticeVisible
        {
            set
            {
                trTitleAndPractice.Visible = value;
            }
        }

        public bool SLTApproval
        {
            get
            {
                return bool.Parse(hdSLTApproval.Value);
            }
            set
            {
                hdSLTApproval.Value = value.ToString();
            }
        }

        public bool SLTPTOApproval
        {
            get
            {
                return bool.Parse(hdSLTPTOApproval.Value);
            }
            set
            {
                hdSLTPTOApproval.Value = value.ToString();
            }
        }

        public bool SLTApprovalPopupDisplayed
        {
            get;
            set;
        }

        public int? TitleId
        {
            set
            {
                if (value.HasValue)
                {
                    ListItem selectedTitle = ddlTitle.Items.FindByValue(value.Value.ToString());
                    if (selectedTitle != null)
                    {
                        ddlTitle.SelectedValue = selectedTitle.Value;
                    }
                }
                else
                {
                    ddlTitle.SelectedIndex = 0;
                }
            }
            get
            {
                if (ddlTitle.SelectedIndex > 0)
                {
                    return int.Parse(ddlTitle.SelectedValue);
                }
                return null;
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
                else
                {
                    ddlPractice.SelectedIndex = 0;
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
                result.IsYearBonus = IsYearBonus;
                result.BonusAmount = BonusAmount;
                result.BonusHoursToCollect = BonusHoursToCollect;
                result.OldStartDate = OldStartDate;
                result.OldEndDate = OldEndDate;
                result.PracticeId = PracticeId;
                result.TitleId = TitleId;
                result.SLTApproval = SLTApproval;
                result.SLTPTOApproval = SLTPTOApproval;
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
                    txtBonusAnnual.ReadOnly = txtVacationDays.ReadOnly = value;

                rbtnSalaryAnnual.Enabled = rbtnSalaryHourly.Enabled = rbtnBonusHourly.Enabled =
                    rbtn1099Ctc.Enabled = rbtnBonusAnnual.Enabled =
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
                    txtBonusHourly.AutoPostBack =
                    txtSalaryAnnual.AutoPostBack = txtSalaryHourly.AutoPostBack =
                    txtVacationDays.AutoPostBack = value;
            }
        }

        public bool IsStrawmanMode { get; set; }

        public string ValidationGroup
        {
            get
            {
                return reqStartDate.ValidationGroup;
            }
            set
            {
                reqStartDate.ValidationGroup =
                compStartDate.ValidationGroup =
                compDateRange.ValidationGroup =
                compEndDate.ValidationGroup =
                reqSalaryAnnual.ValidationGroup =
                compSalaryAnnual.ValidationGroup =
                compSalaryWageGreaterThanZero.ValidationGroup =
                reqSalaryHourly.ValidationGroup =
                compSalaryHourly.ValidationGroup =
                compHourlyWageGreaterThanZero.ValidationGroup =
                req1099Ctc.ValidationGroup =
                comp1099Ctc.ValidationGroup =
                compHourlyGreaterThanZero.ValidationGroup =
                reqPercRevenue.ValidationGroup =
                compPercRevenue.ValidationGroup =
                compPercRevenueGreaterThanZero.ValidationGroup =
                compBonusHourly.ValidationGroup =
                reqBonusDuration.ValidationGroup =
                compBonusDuration.ValidationGroup =
                compBonusAnnual.ValidationGroup =
                rfvVacationDays.ValidationGroup =
                rfvTitle.ValidationGroup =
                cvSLTApprovalValidation.ValidationGroup =
                cvSLTPTOApprovalValidation.ValidationGroup =
                rfvPractice.ValidationGroup = value;
            }
        }

        public string rfvTitleValidationMessage
        {
            set
            {
                rfvTitle.ErrorMessage = rfvTitle.ToolTip = value;
            }
        }

        public string rfvPracticeValidationMessage
        {
            set
            {
                rfvPractice.ErrorMessage = rfvPractice.ToolTip = value;
            }
        }

        public bool IsMarginTestPage { get; set; }

        #endregion

        #region Events

        public event EventHandler CompensationMethodChanged;
        public event EventHandler PeriodChanged;
        public event EventHandler CompensationChanged;
        public event EventHandler TitleChanged;
        public event EventHandler PracticeChanged;
        public event EventHandler SaveDetails;

        #endregion

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!IsStrawmanMode)
                {
                    DataHelper.FillTitleList(ddlTitle, "-- Select Title --");
                    DataHelper.FillPracticeListOnlyActive(ddlPractice, "-- Select Practice Area --");
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsStrawmanMode)
            {
                CompensationDateVisible =
                TitleAndPracticeVisible = false;
                cvVacationDays.Enabled = true;
                lblVacationDays.Text = "PTO Accrual (In Hours)";
                rfvVacationDays.ErrorMessage = rfvVacationDays.ToolTip = "PTO Accrual (In Hours) is required.";

            }
            UpdateCompensationState();
        }

        protected void Compensation_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCompensationState();
            UpdatePTOHours();
            SLTApproval = false;

            if (CompensationMethodChanged != null)
            {
                CompensationMethodChanged(this, e);
            }
        }

        protected void cvVacationDays_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtVacationDays.Text))
            {
                e.IsValid = false;
                int vacationDays;
                if (int.TryParse(txtVacationDays.Text, out vacationDays))
                {
                    if (vacationDays % 8 == 0)
                    {
                        e.IsValid = true;
                    }
                }
            }
        }

        protected void ddlPractice_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (PracticeChanged != null)
            {
                PracticeChanged(this, e);
            }
        }

        protected void ddlTitle_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            SLTPTOApproval = SLTApproval = false;
            UpdatePTOHours();
            if (TitleChanged != null)
            {
                TitleChanged(this, e);
            }
        }

        private void UpdateCompensationState()
        {
            txtSalaryAnnual.Enabled = reqSalaryAnnual.Enabled = compSalaryAnnual.Enabled = compSalaryWageGreaterThanZero.Enabled = txtVacationDays.Enabled = rfvVacationDays.Enabled =
                rbtnSalaryAnnual.Checked;
            txtSalaryHourly.Enabled = reqSalaryHourly.Enabled = compSalaryHourly.Enabled = compHourlyWageGreaterThanZero.Enabled =
                rbtnSalaryHourly.Checked;
            txt1099Ctc.Enabled = req1099Ctc.Enabled = comp1099Ctc.Enabled = compHourlyGreaterThanZero.Enabled = rbtn1099Ctc.Checked;
            txtPercRevenue.Enabled = reqPercRevenue.Enabled = compPercRevenue.Enabled = compPercRevenueGreaterThanZero.Enabled = rbtnPercentRevenue.Checked;
            //txtVacationDays.Enabled = !(rbtn1099Ctc.Checked || rbtnPercentRevenue.Checked);
            // Bonus and vacation are  available for the w2-salary employess.
            UpdateBonusState();


        }

        public void UpdatePTOHours()
        {
            int titleId = 0;
            if (rbtnSalaryAnnual.Checked && ddlTitle.SelectedIndex > 0 && int.TryParse(ddlTitle.SelectedValue, out titleId))
            {
                Title title = ServiceCallers.Custom.Title(t => t.GetTitleById(titleId));
                txtVacationDays.Text = title.PTOAccrual.ToString();
            }
            else
            {
                txtVacationDays.Text = "";
            }
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
            txtBonusAnnual.Enabled = compBonusAnnual.Enabled = rbtnBonusAnnual.Checked && !rbtn1099Ctc.Checked && !rbtnPercentRevenue.Checked;

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
            TextBox changeBox = sender as TextBox;
            if (changeBox.ClientID == txtSalaryAnnual.ClientID)
            {
                SLTApproval = false;
            }
            else if (changeBox.ClientID == txtVacationDays.ClientID)
            {
                SLTPTOApproval = false;
            }

            if (CompensationChanged != null)
            {
                CompensationChanged(this, e);
            }
        }
        
        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            mpeSLTApprovalPopUp.Hide();
            txtSalaryAnnual.Text = "";
            txtSalaryAnnual.Focus();
        }

        protected void btnSLTApproval_OnClick(object sender, EventArgs e)
        {
            mpeSLTApprovalPopUp.Hide();
            SLTApproval = true;
            cvSLTPTOApprovalValidation.Validate();
            if (cvSLTPTOApprovalValidation.IsValid)
            {
                if (SaveDetails != null)
                {
                    SaveDetails(this, e);
                }
            }
        }

        protected void cvSLTApprovalValidation_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            if (!IsMarginTestPage && !IsStrawmanMode)
            {
                rfvPractice.Validate();
                rfvVacationDays.Validate();
                compBonusAnnual.Validate();
                reqSalaryAnnual.Validate();
                compSalaryWageGreaterThanZero.Validate();
                decimal salary;
                int ptoAccrual = 0;
                if (decimal.TryParse(txtSalaryAnnual.Text, out salary) && TitleId.HasValue)
                {
                    Title title = ServiceCallers.Custom.Title(t => t.GetTitleById(TitleId.Value));
                    int.TryParse(txtVacationDays.Text, out ptoAccrual);
                    if (!SLTApproval && rbtnSalaryAnnual.Checked && reqSalaryAnnual.IsValid && compSalaryAnnual.IsValid && compSalaryWageGreaterThanZero.IsValid && rfvPractice.IsValid && rfvVacationDays.IsValid && compBonusAnnual.IsValid)
                    {
                        if ((title.MinimumSalary.HasValue && title.MinimumSalary.Value > salary) || (title.MaximumSalary.HasValue && salary > title.MaximumSalary.Value))
                        {
                            args.IsValid = false;
                            mpeSLTApprovalPopUp.Show();
                            SLTApprovalPopupDisplayed = true;
                        }
                    }
                    if (title.MinimumSalary.HasValue && title.MinimumSalary.Value <= salary && title.MaximumSalary.HasValue && salary <= title.MaximumSalary.Value)
                    {
                        SLTApproval = false;
                    }
                }
            }
        }

        protected void btnCancelSLTPTOApproval_OnClick(object sender, EventArgs e)
        {
            mpeSLTPTOApprovalPopUp.Hide();
            txtVacationDays.Text = "";
            txtVacationDays.Focus();

        }

        protected void btnSLTPTOApproval_OnClick(object sender, EventArgs e)
        {
            mpeSLTPTOApprovalPopUp.Hide();
            SLTPTOApproval = true;
            if (SaveDetails != null)
            {
                SaveDetails(this, e);
            }
        }

        protected void cvSLTPTOApprovalValidation_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            if (!IsMarginTestPage && !IsStrawmanMode)
            {
                rfvPractice.Validate();
                rfvVacationDays.Validate();
                compBonusAnnual.Validate();
                reqSalaryAnnual.Validate();
                compSalaryWageGreaterThanZero.Validate();
                cvSLTApprovalValidation.Validate();
                int ptoAccrual = 0;
                if (int.TryParse(txtVacationDays.Text, out ptoAccrual) && TitleId.HasValue && cvSLTApprovalValidation.IsValid)
                {
                    Title title = ServiceCallers.Custom.Title(t => t.GetTitleById(TitleId.Value));
                    if (!SLTPTOApproval && rbtnSalaryAnnual.Checked && reqSalaryAnnual.IsValid && compSalaryAnnual.IsValid && compSalaryWageGreaterThanZero.IsValid && rfvPractice.IsValid && rfvVacationDays.IsValid && compBonusAnnual.IsValid)
                    {
                        if (title.PTOAccrual != ptoAccrual)
                        {
                            args.IsValid = false;
                            mpeSLTPTOApprovalPopUp.Show();
                            SLTApprovalPopupDisplayed = true;
                        }
                    }
                    if (title.PTOAccrual == ptoAccrual)
                    {
                        SLTPTOApproval = false;
                    }
                }
            }
        }

        public void ShowDates()
        {
            trCompensationDate.Visible = true;
        }
    }
}

