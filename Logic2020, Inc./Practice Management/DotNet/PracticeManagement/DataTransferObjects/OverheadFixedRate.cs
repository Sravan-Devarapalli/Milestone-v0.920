using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Represents the <see cref="OverheadFixedRate"/> entity.
	/// </summary>
	[DataContract]
	[Serializable]
	public class OverheadFixedRate
	{
		#region Properties

		/// <summary>
		/// Gets or sets an ID of the <see cref="OverheadFixedRate"/> entity.
		/// </summary>
		[DataMember]
		public int? Id
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a description of the <see cref="OverheadFixedRate"/> entity.
		/// </summary>
		[DataMember]
		public string Description
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a rate of the overhead.
		/// </summary>
		[DataMember]
		public PracticeManagementCurrency Rate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a start date when the overhead acts.
		/// </summary>
		[DataMember]
		public DateTime StartDate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an end date when the overhaed acts.
		/// </summary>
		[DataMember]
		public DateTime? EndDate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a rate type of the overhead.
		/// </summary>
		[DataMember]
		public OverheadRateType RateType
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets if the overhead is inactive now.
		/// </summary>
		[DataMember]
		public bool Inactive
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a list of the timescales and their applicability for the overhead.
		/// </summary>
		[DataMember]
		public Dictionary<TimescaleType, bool> Timescales
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets if the overhead is for COGS or Expense.
		/// </summary>
		[DataMember]
		public bool IsCogs
		{
			get;
			set;
		}

		/// <summary>
		/// Gets an overhead per hour.
		/// </summary>
		public PracticeManagementCurrency? RatePerHour
		{
			get
			{
				return 
					RateType != null && RateType.HoursToCollect != 0 && !RateType.IsPercentage ?
					(decimal?)Math.Round(Rate / RateType.HoursToCollect, 3) : null;
			}
		}

		#endregion

		#region Construction

		/// <summary>
		/// Creates an empty instance of the <see cref="OverheadFixedRate"/> class.
		/// </summary>
		public OverheadFixedRate()
		{
			Timescales = new Dictionary<TimescaleType, bool>();
		}

		#endregion
	}
}

