using System;
using System.Runtime.Serialization;

namespace DataTransferObjects.Financials
{
    /// <summary>
    /// Milestone person financials
    /// </summary>
    [DataContract]
    [Serializable]
    public class MilestonePersonComputedFinancials : ComputedFinancialsBase
    {
        #region Fields

        private PracticeManagementCurrency _grossHourlyBillRate;
        private PracticeManagementCurrency _loadedHourlyPayRate;

        #endregion

        #region Entity Properties

        /// <summary>
        /// Client Discount
        /// </summary>
        [DataMember]
        public decimal ProjectDiscount { get; set; }

        /// <summary>
        /// Gross Hourly Bill Rate
        /// </summary>
        [DataMember]
        public PracticeManagementCurrency GrossHourlyBillRate
        {
            get { return _grossHourlyBillRate; }
            set
            {
                _grossHourlyBillRate = value;
                _grossHourlyBillRate.FormatStyle = NumberFormatStyle.Revenue;
            }
        }

        /// <summary>
        /// Loaded Hourly Pay Rate 
        /// </summary>
        [DataMember]
        public PracticeManagementCurrency LoadedHourlyPayRate
        {
            get { return _loadedHourlyPayRate; }
            set
            {
                _loadedHourlyPayRate = value;
                _loadedHourlyPayRate.FormatStyle = NumberFormatStyle.Revenue;
            }
        }

        /// <summary>
        /// Hours in Period = [Hours per day] x [Business days]
        /// </summary>
        [DataMember]
        public decimal HoursInPeriod { get; set; }

        #endregion

        #region Calculated Properties

        /// <summary>
        /// Projected Milestone Revenue Contribution
        /// Equals [Hours in Period] x [Gross Hourly Bill Rate] - [Client Discount]
        /// </summary>
        public PracticeManagementCurrency Revenue
        {
            get
            {
                var revenueValue = (HoursInPeriod * GrossHourlyBillRate.Value) * (1 - ProjectDiscount/100.0M);

                var revenue = new PracticeManagementCurrency
                                  {
                                      Value = revenueValue,
                                      FormatStyle = NumberFormatStyle.Revenue
                                  };

                return revenue;
            }
        }

        /// <summary>
        /// Projected Milestone COGS (cost of good sold)
        /// Equals [Hours in Period] x [Loaded Hourly Pay Rate]
        /// </summary>
        public PracticeManagementCurrency Cogs
        {
            get
            {
                var cogsValue = (HoursInPeriod * LoadedHourlyPayRate.Value);

                var cogs = new PracticeManagementCurrency
                                  {
                                      Value = cogsValue,
                                      FormatStyle = NumberFormatStyle.Cogs
                                  };

                return cogs;
            }
        }

        /// <summary>
        /// Projected Milestone Margin Contribution
        /// Equals [Projected Milestone Revenue Contribution] - [Projected Milestone COGS]
        /// </summary>
        public PracticeManagementCurrency Margin
        {
            get
            {
                var margin = Revenue - Cogs;
                margin.FormatStyle = NumberFormatStyle.Margin;
                return margin;
            }
        }

        #endregion
    }
}
