using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Represents a billing info for the project.
	/// </summary>
	[DataContract]
	[Serializable]
	public class BillingInfo
	{
		#region Properties

		/// <summary>
		/// Gets or sets a record ID.
		/// </summary>
		[DataMember]
		public int? Id
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a billing contact.
		/// </summary>
		[DataMember]
		public string BillingContact
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a billing phone.
		/// </summary>
		[DataMember]
		public string BillingPhone
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a billing email.
		/// </summary>
		[DataMember]
		public string BillingEmail
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a billing type.
		/// </summary>
		[DataMember]
		public string BillingType
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a billing address line 1
		/// </summary>
		[DataMember]
		public string BillingAddress1
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a billing address line 2.
		/// </summary>
		[DataMember]
		public string BillingAddress2
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a billing city.
		/// </summary>
		[DataMember]
		public string BillingCity
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a billing state.
		/// </summary>
		[DataMember]
		public string BillingState
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a billing ZIP.
		/// </summary>
		[DataMember]
		public string BillingZip
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a purchase order.
		/// </summary>
		[DataMember]
		public string PurchaseOrder
		{
			get;
			set;
		}

		#endregion
	}
}

