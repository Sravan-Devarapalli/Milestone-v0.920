using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Represents an overhead for the person.
	/// </summary>
	[DataContract]
	[Serializable]
	public class PersonOverhead : IComparable
	{
		#region Properties

		/// <summary>
		/// Gets or sets a name of the overhead.
		/// </summary>
		[DataMember]
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a total rate of the overhead.
		/// </summary>
		[DataMember]
		public PracticeManagementCurrency Rate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a number of hours to calculate a <see cref="HourlyRate"/>
		/// </summary>
		[DataMember]
		public int HoursToCollect
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a Start Date for the overhead.
		/// </summary>
		[DataMember]
		public DateTime StartDate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an End Date for the overhead.
		/// </summary>
		[DataMember]
		public DateTime? EndDate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets if the overhaed is in the percents
		/// </summary>
		[DataMember]
		public bool IsPercentage
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an absolute value of an overhead per hour.
		/// </summary>
		[DataMember]
		public PracticeManagementCurrency HourlyValue
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an overhead rate type.
		/// </summary>
		[DataMember]
		public OverheadRateType RateType
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a bill rate multiplier.
		/// </summary>
		[DataMember]
		public decimal BillRateMultiplier
		{
			get;
			set;
		}

		/// <summary>
		/// Gets a hourly rate
		/// </summary>
		public PracticeManagementCurrency HourlyRate
		{
			get
			{
				return HoursToCollect != 0 ? Rate / HoursToCollect : 0;
			}
		}

        /// <summary>
        /// specifies wheather this overhead is Minimum Load Factor
        /// </summary>
        public bool IsMLF
        {
            get;
            set;
        }

		#endregion

		#region IComparable Members

		/// <summary>
		/// Compares the object to another one with the same type.
		/// </summary>
		/// <param name="obj">The object to compare with.</param>
		/// <returns>
		/// -1 if this object is greater than, 0 if they are equal,
		/// and 1 if the object is less than the object to compare.
		/// It provides sorting in the descending order.
		/// </returns>
		/// <remarks>
		/// The object to compare with must be an instance of the <see cref="PersonOverhead"/> class.
		/// </remarks>
		public int CompareTo(object obj)
		{
			decimal diff = HourlyValue - ((PersonOverhead)obj).HourlyValue;
			return diff < 0 ? 1 : (diff > 0 ? -1 : 0);
		}

		#endregion
	}
}

