using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Reprecentes an opportunity status.
	/// </summary>
	[Serializable]
	[DataContract]
	public class OpportunityStatus
	{
		/// <summary>
		/// Gets or sets an ID of the opportunity status.
		/// </summary>
		[DataMember]
		public int Id
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a name of the opportunity status.
		/// </summary>
		[DataMember]
		public string Name
		{
			get;
			set;
		}
	}
}

