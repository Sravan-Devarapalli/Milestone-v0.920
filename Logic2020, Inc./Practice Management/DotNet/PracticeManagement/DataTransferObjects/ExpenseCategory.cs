using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Represents an Expense Category entity
	/// </summary>
	[DataContract]
	[Serializable]
	public class ExpenseCategory
	{
		/// <summary>
		/// Gets or sets an ID of the Expense Category
		/// </summary>
		[DataMember]
		public int Id
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a name of the Expense Category
		/// </summary>
		[DataMember]
		public string Name
		{
			get;
			set;
		}
	}
}

