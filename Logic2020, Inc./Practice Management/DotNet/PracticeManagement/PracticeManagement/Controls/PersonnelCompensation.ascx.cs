using System;
using System.Linq;
using System.Web.UI.WebControls;
using PraticeManagement.Utils;
using DataTransferObjects;
using PraticeManagement.Configuration;
using System.Collections.Generic;
using PraticeManagement.VendorService;

namespace PraticeManagement.Controls
{
    public partial class PersonnelCompensation : System.Web.UI.UserControl
    {
        public const string lockdownDatesMessage = "Start date and End dates in Compensation tab were locked down by System Administrator for dates on and before '{0}'.";
        public const string lockdownTitlesMessage = "Title field  in Compensation tab was locked down by System Administrator for dates on and before '{0}'.";
        public const string lockdownBasisMessage = "Basis field in Compensation tab was locked down by System Administrator for dates on and before '{0}'.";
        public const string lockdownPracticeMessage = "Practice Area field in Compensation tab was locked down by System Administrator for dates on and before '{0}'.";
        public const string lockdownDivisionMessage = "Division field in Compensation tab was locked down by System Administrator for dates on and before '{0}'.";
        public const string lockdownAmountMessage = "Amount field in Compensation tab was locked down by System Administrator for dates on and before '{0}'.";
        public const string lockdownPTOAccrualsMessage = "PTO Accruals field in Compensation tab was locked down by System Administrator for dates on and before '{0}'.";

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
                trTitle.Visible = trDivisionAndPractice.Visible = value;
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
                if (value.HasValue && DivisionId.HasValue)
                {
                    ddlPractice.Enabled = true;
                    DataHelper.FillPracticeListForDivsion(ddlPractice, "-- Select Practice Area --", (int)DivisionId);
                    ListItem selectedPractice = ddlPractice.Items.FindByValue(value.ToString());
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

        public int? DivisionId
        {
            set
            {
                if (value.HasValue)
                {
                    ListItem selectedDivision = ddlDivision.Items.FindByValue(value.Value.ToString());
                    if (selectedDivision == null)
                    {
                        var division = (PersonDivisionType)value;
                        if (division != 0)
                        {
                            selectedDivision = new ListItem(DataHelper.GetDescription(division), Convert.ToInt32(division).ToString());
                            ddlDivision.Items.Add(selectedDivision);
                            ddlDivision.SortByText();
                            ddlDivision.SelectedValue = selectedDivision.Value;
                        }
                    }
                    else
                    {
                        ddlDivision.SelectedValue = selectedDivision.Value;
                    }
                }
                else
                {
                    ddlDivision.SelectedIndex = 0;
                }
            }
            get
            {
                if (ddlDivision.SelectedIndex > 0)
                {
                    return int.Parse(ddlDivision.SelectedValue);
                }
                return null;
            }
        }

        public int? VendorId
        {
            get
            {
                if (ddlVendor.SelectedIndex > 0)
                {
                    return int.Parse(ddlVendor.SelectedValue);
                }
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    ListItem selectedvendor = ddlVendor.Items.FindByValue(value.Value.ToString());
                    if (selectedvendor == null)
                    {
                        using (var serviceClient = new VendorServiceClient())
                        {
                            try
                            {
                                var vendor = serviceClient.GetVendorById(value.Value);
                                if (vendor != null)
                                {
                                    selectedvendor = new ListItem(vendor.Id.ToString(), vendor.Name);
                                    ddlVendor.Items.Add(selectedvendor);
                                    ddlVendor.SortByText();
                                    ddlVendor.SelectedValue = selectedvendor.Value;
                                }
                            }
                            catch
                            {
                                serviceClient.Abort();
                                throw;
                            }
                        }
                    }
                    else
                    {
                        ddlVendor.SelectedValue = selectedvendor.Value;
                    }
                }
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
                result.DivisionId = DivisionId;
                result.PracticeId = PracticeId;
                result.TitleId = TitleId;
                result.DivisionName = ddlDivision.SelectedItem.Text;
                result.TitleName = ddlTitle.SelectedItem.Text;
                result.SLTApproval = SLTApproval;
                result.SLTPTOApproval = SLTPTOApproval;
                if (Timescale == TimescaleType._1099Ctc || Timescale == TimescaleType.PercRevenue)
                {
                    result.vendor = new Vendor { Id = Int32.Parse(ddlVendor.SelectedValue) };
                }
                else
                {
                    result.vendor = null;
                }
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
                cvVacationDays.ValidationGroup =
                cvSLTPTOApprovalValidation.ValidationGroup =
                custValLockoutDates.ValidationGroup =
                custLockoutBasis.ValidationGroup =
                custLockoutAmount.ValidationGroup =
                custLockoutPTO.ValidationGroup =
                custLockoutTitle.ValidationGroup =
                custLockOutPractice.ValidationGroup =
                custLockOutDivision.ValidationGroup =
                rfvPractice.ValidationGroup =
                reqddlVendor.ValidationGroup =
                rfvDivision.ValidationGroup =
                cvIsDivisionOrPracticeOwner.ValidationGroup = value;
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

        public bool IsCompensationPage { get; set; }

        public bool IsDivisionOrPracticeOwner
        {
            get
            {
                if (ViewState["IsDivisionOrPracticeOwner"] == null)
                {
                    return false;
                }
                else
                {
                    return (bool)ViewState["IsDivisionOrPracticeOwner"];
                }
            }
            set
            {
                cvIsDivisionOrPracticeOwner.Enabled = !value;
                cvIsDivisionOrPracticeOwner.IsValid = value;
                ViewState["IsDivisionOrPracticeOwner"] = (object)value;
            }
        }

        public string PreviousDivision
        {
            get { return (string)ViewState["PreviousDivisionId"]; }
            set { ViewState["PreviousDivisionId"] = value; }
        }

        public string PreviousPractice
        {
            get { return (string)ViewState["PreviousPracticeId"]; }
            set { ViewState["PreviousPracticeId"] = value; }
        }

        public string PreviousTitle
        {
            get { return (string)ViewState["PreviousTitleId"]; }
            set { ViewState["PreviousTitleId"] = value; }
        }

        public string PreviousPtoAccrual
        {
            get { return (string)ViewState["PreviousPtoAccrual"]; }
            set { ViewState["PreviousPtoAccrual"] = value; }
        }

        public decimal? PreviousAmount
        {
            get { return (decimal?)ViewState["PreviousAmount"]; }
            set { ViewState["PreviousAmount"] = value; }
        }

        public string PreviousBasis
        {
            get { return (string)ViewState["PreviousBasis"]; }
            set { ViewState["PreviousBasis"] = value; }
        }

        public List<DataTransferObjects.Lockout> Lockouts
        {
            get;
            set;
        }

        public bool IsAllLockout
        {
            get;
            set;
        }

        public bool IsLockout
        {
            get;
            set;
        }

        public DateTime? LockoutDate
        {
            get;
            set;
        }

        #endregion

        #region Events

        public event EventHandler CompensationMethodChanged;
        public event EventHandler PeriodChanged;
        public event EventHandler CompensationChanged;
        public event EventHandler TitleChanged;
        public event EventHandler DivisionChanged;
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
                    DataHelper.FillPersonDivisionList(ddlDivision);
                    ListItem practice = new ListItem("-- Select Practice Area --", "0");
                    DataHelper.FillVendors(ddlVendor, "-- select vendor --");
                    ddlPractice.Items.Add(practice);
                    ddlPractice.SelectedIndex = 0;
                    ddlPractice.Enabled = false;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LockdownCompensation();
            if (!IsPostBack && IsCompensationPage)
            {
                SaveAllLockdownViewStates();
            }
            //SetAmountandBasis();
            if (IsStrawmanMode)
            {
                CompensationDateVisible =
                TitleAndPracticeVisible = false;
                cvVacationDays.Enabled = true;
                lblVacationDays.Text = "PTO Accrual (In Hours)";
                rfvVacationDays.ErrorMessage = rfvVacationDays.ToolTip = "PTO Accrual (In Hours) is required.";

            }
            UpdateCompensationState();
            if (IsMarginTestPage)
            {
                txtVacationDays.MaxLength = 3;
                compVacationDays.Enabled = true;
            }
        }

        public void SaveAllLockdownViewStates()
        {
            PreviousPractice = ddlPractice.SelectedItem.Text;
            PreviousTitle = ddlTitle.SelectedItem.Text;
            PreviousPtoAccrual = txtVacationDays.Text;
            PreviousBasis = Timescale.ToString();
            PreviousAmount = Amount;
            PreviousDivision = ddlDivision.SelectedItem.Text;
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

        protected void ddlDivision_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlDivision.SelectedIndex != 0)
            {
                ddlPractice.Enabled = true;
                if (DivisionId != null)
                {
                    DataHelper.FillPracticeListForDivsion(ddlPractice, "-- Select Practice Area --", (int)DivisionId);
                }

            }
            else
            {
                ddlPractice.SelectedIndex = 0;
                ddlPractice.Enabled = false;
            }
            if (DivisionChanged != null)
            {
                DivisionChanged(this, e);
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
            txtSalaryAnnual.Enabled = reqSalaryAnnual.Enabled = compSalaryAnnual.Enabled = compSalaryWageGreaterThanZero.Enabled =
                rbtnSalaryAnnual.Checked;
            txtSalaryHourly.Enabled = reqSalaryHourly.Enabled = compSalaryHourly.Enabled = compHourlyWageGreaterThanZero.Enabled =
                rbtnSalaryHourly.Checked;
            txt1099Ctc.Enabled = req1099Ctc.Enabled = comp1099Ctc.Enabled = compHourlyGreaterThanZero.Enabled = rbtn1099Ctc.Checked;
            txtPercRevenue.Enabled = reqPercRevenue.Enabled = compPercRevenue.Enabled = compPercRevenueGreaterThanZero.Enabled = rbtnPercentRevenue.Checked;
            txtVacationDays.Enabled = rfvVacationDays.Enabled = (rbtnSalaryHourly.Checked || rbtnSalaryAnnual.Checked);
            // Bonus and vacation are  available for the w2-salary employess.

            lblVendor.Visible = ddlVendor.Visible = reqddlVendor.Enabled = (rbtn1099Ctc.Checked || rbtnPercentRevenue.Checked) && !IsMarginTestPage && !IsStrawmanMode;


            if (IsStrawmanMode)
            {
                compSalaryWageGreaterThanZero.Enabled = compHourlyWageGreaterThanZero.Enabled = compHourlyGreaterThanZero.Enabled = compPercRevenueGreaterThanZero.Enabled = false;
            }
            UpdateBonusState();
        }

        public void UpdatePTOHours()
        {
            int titleId = 0;
            if ((rbtnSalaryAnnual.Checked || rbtnSalaryHourly.Checked) && ddlTitle.SelectedIndex > 0 && int.TryParse(ddlTitle.SelectedValue, out titleId))
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

        protected void custValLockoutDates_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (IsCompensationPage && Lockouts.Any(p => p.Name == "Dates" && p.IsLockout == true))
            {
                CustomValidator custLockdown = sender as CustomValidator;
                DateTime startDate;
                DateTime? endDate;
                DateTime.TryParse(dpStartDate.TextValue, out startDate);
                endDate = dpEndDate.TextValue == string.Empty ? null : (DateTime?)dpEndDate.DateValue;
                if ((startDate.Date <= LockoutDate.Value.Date || endDate.HasValue && endDate.Value.Date <= LockoutDate.Value.Date) && !OldStartDate.HasValue && !OldEndDate.HasValue)
                {
                    e.IsValid = false;
                    custLockdown.ErrorMessage = custLockdown.ToolTip = string.Format(lockdownDatesMessage, LockoutDate.Value.ToShortDateString());
                }
                if (OldStartDate.HasValue && (startDate.Date != OldStartDate.Value.Date || endDate.HasValue && endDate.Value.Date != OldEndDate.Value.Date))
                {
                    if ((startDate.Date != OldStartDate.Value.Date && startDate.Date <= LockoutDate.Value.Date) || (endDate.HasValue && OldEndDate.HasValue && endDate.Value.Date != OldEndDate.Value.Date && endDate.Value.Date <= LockoutDate.Value.Date))
                    {
                        e.IsValid = false;
                        custLockdown.ErrorMessage = custLockdown.ToolTip = string.Format(lockdownDatesMessage, LockoutDate.Value.ToShortDateString());
                    }
                }
            }
        }

        protected void custLockOutDivision_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (IsCompensationPage && Lockouts.Any(p => p.Name == "Division" && p.IsLockout == true))
            {
                CustomValidator custLockdown = sender as CustomValidator;
                if (!OldStartDate.HasValue && !OldEndDate.HasValue)
                {
                    DateTime startDate;
                    DateTime? endDate;
                    DateTime.TryParse(dpStartDate.TextValue, out startDate);
                    endDate = dpEndDate.TextValue == string.Empty ? null : (DateTime?)dpEndDate.DateValue;
                    if ((startDate.Date <= LockoutDate.Value.Date || endDate.HasValue && endDate.Value.Date <= LockoutDate.Value.Date))
                    {
                        e.IsValid = false;
                        custLockdown.ErrorMessage = custLockdown.ToolTip = string.Format(lockdownDivisionMessage, LockoutDate.Value.ToShortDateString());
                    }
                }
                else if (OldStartDate.HasValue && (OldStartDate.Value.Date <= LockoutDate.Value.Date || (OldEndDate.HasValue && OldEndDate.Value.Date <= LockoutDate.Value.Date)) && ddlDivision.SelectedItem.Text != PreviousDivision)
                {
                    e.IsValid = false;
                    custLockdown.ErrorMessage = custLockdown.ToolTip = string.Format(lockdownDivisionMessage, LockoutDate.Value.ToShortDateString());
                }
            }
        }

        protected void custLockOutPractice_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (IsCompensationPage && Lockouts.Any(p => p.Name == "Practice Area" && p.IsLockout == true))
            {
                CustomValidator custLockdown = sender as CustomValidator;
                if (!OldStartDate.HasValue && !OldEndDate.HasValue)
                {
                    DateTime startDate;
                    DateTime? endDate;
                    DateTime.TryParse(dpStartDate.TextValue, out startDate);
                    endDate = dpEndDate.TextValue == string.Empty ? null : (DateTime?)dpEndDate.DateValue;
                    if ((startDate.Date <= LockoutDate.Value.Date || endDate.HasValue && endDate.Value.Date <= LockoutDate.Value.Date))
                    {
                        e.IsValid = false;
                        custLockdown.ErrorMessage = custLockdown.ToolTip = string.Format(lockdownPracticeMessage, LockoutDate.Value.ToShortDateString());
                    }
                }
                else if (OldStartDate.HasValue && (OldStartDate.Value.Date <= LockoutDate.Value.Date || (OldEndDate.HasValue && OldEndDate.Value.Date <= LockoutDate.Value.Date)) && ddlPractice.SelectedItem.Text != PreviousPractice)
                {
                    e.IsValid = false;
                    custLockdown.ErrorMessage = custLockdown.ToolTip = string.Format(lockdownPracticeMessage, LockoutDate.Value.ToShortDateString());
                }
            }
        }

        protected void custLockoutTitle_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (IsCompensationPage && Lockouts.Any(p => p.Name == "Title" && p.IsLockout == true))
            {
                CustomValidator custLockdown = sender as CustomValidator;
                GridViewRow row = custLockdown.NamingContainer as GridViewRow;
                CustomDropDown ddlTitle = row.FindControl("ddlTitle") as CustomDropDown;
                if (!OldStartDate.HasValue && !OldEndDate.HasValue)
                {
                    DatePicker dpStartDate = row.FindControl("dpStartDate") as DatePicker;
                    DatePicker dpEndDate = row.FindControl("dpEndDate") as DatePicker;
                    DateTime startDate;
                    DateTime? endDate;
                    DateTime.TryParse(dpStartDate.TextValue, out startDate);
                    endDate = dpEndDate.TextValue == string.Empty ? null : (DateTime?)dpEndDate.DateValue;
                    if ((startDate.Date <= LockoutDate.Value.Date || endDate.HasValue && endDate.Value.Date <= LockoutDate.Value.Date))
                    {
                        e.IsValid = false;
                        custLockdown.ErrorMessage = custLockdown.ToolTip = string.Format(lockdownTitlesMessage, LockoutDate.Value.ToShortDateString());
                    }
                }
                else if (OldStartDate.HasValue && (OldStartDate.Value.Date <= LockoutDate.Value.Date || (OldEndDate.HasValue && OldEndDate.Value.Date <= LockoutDate.Value.Date)) && ddlTitle.SelectedItem.Text != PreviousTitle)
                {
                    e.IsValid = false;
                    custLockdown.ErrorMessage = custLockdown.ToolTip = string.Format(lockdownTitlesMessage, LockoutDate.Value.ToShortDateString());
                }
            }
        }

        protected void custLockoutBasis_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (IsCompensationPage && Lockouts.Any(p => p.Name == "Basis" && p.IsLockout == true))
            {
                CustomValidator custLockdown = sender as CustomValidator;

                if (!OldStartDate.HasValue && !OldEndDate.HasValue)
                {
                    DateTime startDate;
                    DateTime? endDate;
                    DateTime.TryParse(dpStartDate.TextValue, out startDate);
                    endDate = dpEndDate.TextValue == string.Empty ? null : (DateTime?)dpEndDate.DateValue;
                    if ((startDate.Date <= LockoutDate.Value.Date || endDate.HasValue && endDate.Value.Date <= LockoutDate.Value.Date))
                    {
                        e.IsValid = false;
                        custLockdown.ErrorMessage = custLockdown.ToolTip = string.Format(lockdownBasisMessage, LockoutDate.Value.ToShortDateString());
                    }
                }
                else if (OldStartDate.HasValue && (OldStartDate.Value.Date <= LockoutDate.Value.Date || (OldEndDate.HasValue && OldEndDate.Value.Date <= LockoutDate.Value.Date)) && Timescale.ToString() != PreviousBasis)
                {
                    e.IsValid = false;
                    custLockdown.ErrorMessage = custLockdown.ToolTip = string.Format(lockdownBasisMessage, LockoutDate.Value.ToShortDateString());
                }
            }
        }

        protected void custLockoutAmount_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (IsCompensationPage && Lockouts.Any(p => p.Name == "Amount" && p.IsLockout == true))
            {
                CustomValidator custLockdown = sender as CustomValidator;
                if (!OldStartDate.HasValue && !OldEndDate.HasValue)
                {
                    DateTime startDate;
                    DateTime? endDate;
                    DateTime.TryParse(dpStartDate.TextValue, out startDate);
                    endDate = dpEndDate.TextValue == string.Empty ? null : (DateTime?)dpEndDate.DateValue;
                    if ((startDate.Date <= LockoutDate.Value.Date || endDate.HasValue && endDate.Value.Date <= LockoutDate.Value.Date))
                    {
                        e.IsValid = false;
                        custLockdown.ErrorMessage = custLockdown.ToolTip = string.Format(lockdownAmountMessage, LockoutDate.Value.ToShortDateString());
                    }
                }
                else if (OldStartDate.HasValue && (OldStartDate.Value.Date <= LockoutDate.Value.Date || (OldEndDate.HasValue && OldEndDate.Value.Date <= LockoutDate.Value.Date)) && Amount != PreviousAmount)
                {
                    e.IsValid = false;
                    custLockdown.ErrorMessage = custLockdown.ToolTip = string.Format(lockdownAmountMessage, LockoutDate.Value.ToShortDateString());
                }
            }
        }

        protected void custLockoutPTO_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (IsCompensationPage && Lockouts.Any(p => p.Name == "PTO Accrual" && p.IsLockout == true))
            {
                CustomValidator custLockdown = sender as CustomValidator;

                if (!OldStartDate.HasValue && !OldEndDate.HasValue)
                {

                    DateTime startDate;
                    DateTime? endDate;
                    DateTime.TryParse(dpStartDate.TextValue, out startDate);
                    endDate = dpEndDate.TextValue == string.Empty ? null : (DateTime?)dpEndDate.DateValue;
                    if ((startDate.Date <= LockoutDate.Value.Date || endDate.HasValue && endDate.Value.Date <= LockoutDate.Value.Date))
                    {
                        e.IsValid = false;
                        custLockdown.ErrorMessage = custLockdown.ToolTip = string.Format(lockdownPTOAccrualsMessage, LockoutDate.Value.ToShortDateString());
                    }
                }
                else if (OldStartDate.HasValue && (OldStartDate.Value.Date <= LockoutDate.Value.Date || (OldEndDate.HasValue && OldEndDate.Value.Date <= LockoutDate.Value.Date)) && txtVacationDays.Text != PreviousPtoAccrual)
                {
                    e.IsValid = false;
                    custLockdown.ErrorMessage = custLockdown.ToolTip = string.Format(lockdownPTOAccrualsMessage, LockoutDate.Value.ToShortDateString());
                }
            }
        }

        public void ShowDates()
        {
            trCompensationDate.Visible = true;
        }

        public void LockdownCompensation()
        {
            using (var service = new ConfigurationService.ConfigurationServiceClient())
            {
                List<DataTransferObjects.Lockout> persondetailItems = service.GetLockoutDetails((int)LockoutPages.Persondetail).ToList();
                IsLockout = persondetailItems.Any(p => p.IsLockout == true);
                LockoutDate = persondetailItems[0].LockoutDate;
                Lockouts = persondetailItems;
                IsAllLockout = !persondetailItems.Any(p => p.IsLockout == false);
            }
        }

    }
}

