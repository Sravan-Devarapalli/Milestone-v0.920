using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    /// <summary>
    /// A person can have a default list of recruiting comissions
    /// that can be used to inititalize a new list of recruiting comissions
    /// </summary>
    [DataContract]
	[Serializable]
	public class DefaultRecruiterCommission
    {
		/// <summary>
		/// Gets or sets an ID of the commission header.
		/// </summary>
		[DataMember]
		public int? CommissionHeaderId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an ID of the Person.
		/// </summary>
		[DataMember]
		public int PersonId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a start date for the recruiter commission.
		/// </summary>
		[DataMember]
		public DateTime StartDate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an end date for the default recruiter commission.
		/// </summary>
		[DataMember]
		public DateTime? EndDate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a textual representation for the commission items.
		/// </summary>
		[DataMember]
		public string TextLine
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a list of the payments for the recruit.
		/// </summary>
		[DataMember]
		public List<DefaultRecruiterCommissionItem> Items
		{
			get;
			set;
		}
    }
}
