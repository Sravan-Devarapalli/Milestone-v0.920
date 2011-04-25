using System;
using System.ComponentModel;

namespace PraticeManagement.Controls
{
    public partial class MonthPicker : System.Web.UI.UserControl
    {
        #region Constants

        private const int DefaultNumberOfYears = 29;
        private const string NumberOfYearsKey = "NumberOfYears";
        private const string MaxYearNumberKey = "MaxYearNumber";

        #endregion

        #region Fields

        private int numberOfYearsValue = DefaultNumberOfYears;
        private bool initiated;

        #endregion

        #region Properties

        public DateTime? YearFromDB
        {
            set;
            get;
        }

        public string OnClientChange
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets a number of years from the current one to be displayed.
        /// </summary>
        public int NumberOfYears
        {
            get
            {
                if (EnableViewState)
                {
                    object result = ViewState[NumberOfYearsKey];
                    numberOfYearsValue = result != null ? (int)result : DefaultNumberOfYears;
                }

                return numberOfYearsValue;
            }
            set
            {
                if (value <= 0 || value > DateTime.Today.Year)
                {
                    throw new IndexOutOfRangeException(Resources.Messages.InvalidNumberOfYears);
                }

                numberOfYearsValue = value;
                if (EnableViewState)
                {
                    ViewState[NumberOfYearsKey] = value;
                }

                FillYearList();
            }
        }

        /// <summary>
        /// Gets or sets a selected month
        /// </summary>
        public int SelectedMonth
        {
            get
            {
                return int.Parse(ddlMonth.SelectedValue);
            }
            set
            {
                if (value < Constants.Dates.FirstMonth || value > Constants.Dates.LastMonth)
                {
                    throw new IndexOutOfRangeException(
                        string.Format(Resources.Messages.InvalidMonthNumber, Constants.Dates.FirstMonth, Constants.Dates.LastMonth));
                }

                ddlMonth.SelectedIndex = ddlMonth.Items.IndexOf(ddlMonth.Items.FindByValue(value.ToString()));
            }
        }

        /// <summary>
        /// Gets or sets a selected year.
        /// </summary>
        public int SelectedYear
        {
            get
            {
                if (ddlYear.Items.Count == 0)
                {
                    FillYearList();
                }
                return int.Parse(ddlYear.SelectedValue);
            }
            set
            {
                if (ddlYear.Items.Count == 0)
                {
                    FillYearList();
                }
                if (value <= (MaxYearNumber - NumberOfYears) || value > MaxYearNumber)
                {
                    throw new IndexOutOfRangeException(
                        string.Format(Resources.Messages.InvalidYearNumber, MaxYearNumber - NumberOfYears + 1, MaxYearNumber));
                }

                ddlYear.SelectedIndex = ddlYear.Items.IndexOf(ddlYear.Items.FindByValue(value.ToString()));
            }
        }

        /// <summary>
        /// Gets or sets a maximun year to be displayed in the list.
        /// </summary>
        public int MaxYearNumber
        {
            get
            {
                return Convert.ToInt32(ViewState[MaxYearNumberKey] ?? 2029);
            }
            set
            {
                ViewState[MaxYearNumberKey] = value;
            }
        }

        /// <summary>
        /// Gets or sets if the Auto-Postback is enabled.
        /// </summary>
        [DefaultValue(true)]
        public bool AutoPostBack
        {
            get
            {
                return ddlMonth.AutoPostBack;
            }
            set
            {
                ddlMonth.AutoPostBack = ddlYear.AutoPostBack = value;
            }
        }

        public event EventHandler SelectedValueChanged;

        #endregion

        #region Methods

        public DateTime MonthBegin
        {
            get
            {
                return new DateTime(SelectedYear, SelectedMonth, Constants.Dates.FirstDay);
            }
        }

        public DateTime MonthEnd
        {
            get
            {
                DateTime monthBegin = MonthBegin;
                return new DateTime(
                    monthBegin.Year,
                    monthBegin.Month,
                    DateTime.DaysInMonth(monthBegin.Year, monthBegin.Month));
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !initiated)
            {
                FillYearList();
            }
            if(!string.IsNullOrEmpty(OnClientChange))
            {
                ddlMonth.Attributes.Add("onchange", OnClientChange);
                ddlYear.Attributes.Add("onchange", OnClientChange);
            }
        }

        private void FillYearList()
        {
            ddlYear.Items.Clear();

            for (int i = MaxYearNumber; i > MaxYearNumber - NumberOfYears; i--)
            {
                ddlYear.Items.Add(i.ToString());
            }

            DateTime today = DateTime.Today;
            if (YearFromDB != null)
            {
                SelectedMonth = YearFromDB.Value.Month;
                if (ddlYear.Items.FindByText(YearFromDB.Value.Year.ToString()) != null)
                {
                    SelectedYear = YearFromDB.Value.Year;
                }
            }
            else
            {
                SelectedMonth = today.Month;
                if (ddlYear.Items.FindByText(today.Year.ToString()) != null)
                {
                    SelectedYear = today.Year;
                }
            }

            initiated = true;
        }

        protected void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectedValueChanged(e);
        }

        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectedValueChanged(e);
        }

        /// <summary>
        /// Fires the SelectedValueChanged event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        private void OnSelectedValueChanged(EventArgs e)
        {
            if (SelectedValueChanged != null)
            {
                SelectedValueChanged(this, e);
            }
        }
    }
}

