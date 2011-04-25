using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    /// <summary>
    /// Represents the results of the rate computing.
    /// </summary>
    [DataContract]
    [Serializable]
    public class ComputedFinancials
    {
        #region Fields

        private PracticeManagementCurrency _cogsValue;
        private PracticeManagementCurrency _marginValue;
        private PracticeManagementCurrency _revenueNetValue;
        private PracticeManagementCurrency _revenueValue;

        #endregion

        #region Constructor

        public ComputedFinancials()
        {
            Timescale = TimescaleType.Salary;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a computed Revenue.
        /// </summary>
        [DataMember]
        public PracticeManagementCurrency Revenue
        {
            get { return _revenueValue; }
            set
            {
                _revenueValue = value;
                _revenueValue.FormatStyle = NumberFormatStyle.Revenue;
            }
        }

        /// <summary>
        /// Gets or sets a computed Net Revenue.
        /// </summary>
        [DataMember]
        public PracticeManagementCurrency RevenueNet
        {
            get { return _revenueNetValue; }
            set
            {
                _revenueNetValue = value;
                _revenueNetValue.FormatStyle = NumberFormatStyle.Revenue;
            }
        }

        /// <summary>
        /// Gets or sets a computed COGS.
        /// </summary>
        [DataMember]
        public PracticeManagementCurrency Cogs
        {
            get { return _cogsValue; }
            set
            {
                _cogsValue = value;
                _cogsValue.FormatStyle = NumberFormatStyle.Cogs;
            }
        }

        /// <summary>
        /// Gets or sets a computed margin.
        /// </summary>
        [DataMember]
        public PracticeManagementCurrency GrossMargin
        {
            get { return _marginValue; }
            set
            {
                _marginValue = value;
                _marginValue.FormatStyle = NumberFormatStyle.Margin;
            }
        }

        /// <summary>
        /// Gets or sets a computed value of the sales commission.
        /// </summary>
        [DataMember]
        public PracticeManagementCurrency SalesCommission { get; set; }

        /// <summary>
        /// Gets or sets a computed value of the practice management commission.
        /// </summary>
        [DataMember]
        public PracticeManagementCurrency PracticeManagementCommission { get; set; }

        /// <summary>
        /// Gets or sets a date the financials tied with.
        /// </summary>
        [DataMember]
        public DateTime? FinancialDate { get; set; }

        /// <summary>
        /// Gets or sets a number of hours billed.
        /// </summary>
        [DataMember]
        public decimal HoursBilled { get; set; }

        /// <summary>
        /// Gets an average hourly bill rate minus discount.
        /// </summary>
        /// 
        [DataMember]
        public PracticeManagementCurrency? BillRateMinusDiscount { get; set; }

        /// <summary>
        /// Gets or sets a date since the person is available.
        /// </summary>
        /// <remarks>Uses for bench persons.</remarks>
        [DataMember]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets expenses, $.
        /// </summary>
        /// <remarks>Total expenses, $</remarks>
        [DataMember]
        public decimal Expenses { get; set; }

        /// <summary>
        /// Gets or sets reimbursed expenses, $.
        /// </summary>
        /// <remarks>Total reimbursed expenses, $</remarks>
        [DataMember]
        public decimal ReimbursedExpenses { get; set; }

        /// <summary>
        /// Gets or sets a timescale the financials were calculated for.
        /// </summary>
        /// <remarks>Uses for bench persons.</remarks>
        [DataMember]
        public TimescaleType Timescale { get; set; }

        #endregion

        #region Calculated Values

        /// <summary>
        /// Gets an average hourly bill rate without a discount.
        /// </summary>
        public PracticeManagementCurrency? GrossBillRate
        {
            get { return HoursBilled > 0 ? RevenueNet / HoursBilled : (PracticeManagementCurrency?)null; }
        }

        /// <summary>
        /// Gets an average hourly bill rate.
        /// </summary>
        public PracticeManagementCurrency? BillRate
        {
            get { return HoursBilled > 0 ? Revenue / HoursBilled : (PracticeManagementCurrency?)null; }
        }

        /// <summary>
        /// Gets an average hourly pay
        /// </summary>
        public PracticeManagementCurrency? LoadedHourlyPay
        {
            get { return HoursBilled > 0 ? Cogs / HoursBilled : (PracticeManagementCurrency?)null; }
        }

        /// <summary>
        /// Gets a margin net
        /// </summary>
        public PracticeManagementCurrency MarginNet
        {
            get
            {
                PracticeManagementCurrency result = GrossMargin - SalesCommission - PracticeManagementCommission;
                result.FormatStyle = NumberFormatStyle.Margin;
                return result;
            }
        }

        /// <summary>
        /// Gets a margin to revenue ratio in percentage.
        /// </summary>
        public decimal TargetMargin
        {
            get { return Revenue != 0M ? GrossMargin.Value * 100M / Revenue.Value : 0M; }
        }

        #endregion

    }
}
