using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Represents an ExpenseBasis database entity
	/// </summary>
	[DataContract]
	[Serializable]
	public class ExpenseBasis
	{
		/// <summary>
		/// Gets or sets an ID of the Expense Basis
		/// </summary>
		[DataMember]
		public int Id
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a name of the Expense Basis
		/// </summary>
		[DataMember]
		public string Name
		{
			get;
			set;
		}
	}
}

