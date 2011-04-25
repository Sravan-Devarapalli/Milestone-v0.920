using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Represents an item for the default recruiter commission.
	/// </summary>
	[Serializable]
	[DataContract]
	public class DefaultRecruiterCommissionItem : IComparable
	{
		#region Properties

		/// <summary>
		/// Gets or sets an ID of the parent commission header.
		/// </summary>
		[DataMember]
		public int CommissionHeaderId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a number of hours to make a payment for the recruit.
		/// </summary>
		[DataMember]
		public int HoursToCollect
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an amount of the payment for the recruit.
		/// </summary>
		[DataMember]
		public PracticeManagementCurrency Amount
		{
			get;
			set;
		}

		#endregion

		#region IComparable Members

		public int CompareTo(object obj)
		{
			return HoursToCollect - ((DefaultRecruiterCommissionItem)obj).HoursToCollect;
		}

		#endregion
	}
}

