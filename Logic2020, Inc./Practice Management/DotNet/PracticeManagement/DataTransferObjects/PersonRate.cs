using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Represents the calculated person rate.
	/// </summary>
	[DataContract]
	[Serializable]
	[Obsolete]
	public class PersonRate
	{
		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private PracticeManagementCurrency estimatedRevenueValue;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private PracticeManagementCurrency estimatedMarginValue;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets a base person's salary.
		/// </summary>
		[DataMember]
		public PracticeManagementCurrency Basis
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the person's raw hourly rate (payment).
		/// </summary>
		[DataMember]
		public PracticeManagementCurrency RawHourlyRate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a loaded hourly rate.
		/// </summary>
		[DataMember]
		public PracticeManagementCurrency LoadedHourlyRate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a Less Client discount
		/// </summary>
		[DataMember]
		public PracticeManagementCurrency ClientDiscount
		{
			get;
			set;
		}

		/// <summary>
		/// Actually is a stub
		/// </summary>
		[DataMember]
		public PracticeManagementCurrency EffectiveBillRate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a Projected Milestone Revenue Contribution -
		/// &lt;number of working hours in the period&gt; multiplied by &lt;Effective Bill Rate&gt;.
		/// </summary>
		[DataMember]
		public PracticeManagementCurrency EstimatedRevenue
		{
			get
			{
				return estimatedRevenueValue;
			}
			set
			{
				estimatedRevenueValue = value;
				estimatedRevenueValue.FormatStyle = NumberFormatStyle.Revenue;
			}
		}

		/// <summary>
		/// Gets a projected COGS
		/// </summary>
		public PracticeManagementCurrency ProjectedCogs
		{
			get
			{
				PracticeManagementCurrency result = LoadedHourlyRate * EstimatedHours;
				result.FormatStyle = NumberFormatStyle.Cogs;

				return result;
			}
		}

		/// <summary>
		/// Gets a Projected Milestone Margin Contribution -
		/// &lt;Projected Milestone revenue contribution&gt; minus (&lt;number of working hours in the period&gt; * &lt;loaded hourly pay rate&gt;)
		/// </summary>
		[DataMember]
		public PracticeManagementCurrency EstimatedMargin
		{
			get
			{
				return estimatedMarginValue;
			}
			set
			{
				estimatedMarginValue = value;
				estimatedMarginValue.FormatStyle = NumberFormatStyle.Margin;
			}
		}

		/// <summary>
		/// Gets an estimated hourly margin.
		/// </summary>
		public PracticeManagementCurrency EstimatedHourlyMargin
		{
			get
			{
				PracticeManagementCurrency result = EffectiveBillRate - LoadedHourlyRate;
				result.FormatStyle = NumberFormatStyle.Margin;
				return result;
			}
		}

		/// <summary>
		/// Gets or sets a value for the sales commission.
		/// </summary>
		[DataMember]
		public PracticeManagementCurrency SalesCommission
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a number of hours estimated for the <see cref="Person"/> and <see cref="Milestone"/>.
		/// </summary>
		[DataMember]
		public decimal EstimatedHours
		{
			get;
			set;
		}

		/// <summary>
		/// Gets a value for an effective hourly gross margin
		/// </summary>
		public PracticeManagementCurrency EffectiveHourlyMargin
		{
			get
			{
				PracticeManagementCurrency result = EstimatedHourlyMargin - SalesCommission;
				result.FormatStyle = NumberFormatStyle.Margin;
				return result;
			}
		}

		#endregion
	}
}

