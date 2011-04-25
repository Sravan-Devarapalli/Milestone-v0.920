using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Represents a montly expenses list for the item
	/// </summary>
	[DataContract]
	[Serializable]
	public class MonthlyExpense
	{
		#region Properties

		/// <summary>
		/// Gets or sets a name of the Expense Item.
		/// </summary>
		[DataMember]
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a Category of the Monthly Expense Item.
		/// </summary>
		[DataMember]
		public ExpenseCategory ExpenseCategory
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a Basis of the Monthly Expense Item.
		/// </summary>
		[DataMember]
		public ExpenseBasis ExpenseBasis
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a Week Paid Option of the Monthly Expense Item.
		/// </summary>
		[DataMember]
		public WeekPaidOption WeekPaidOption
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the amount by months for the Monthly Expense Item.
		/// </summary>
		[DataMember]
		public Dictionary<DateTime, decimal> MonthlyAmount
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a min month the expense was selected.
		/// </summary>
		[DataMember]
		public DateTime MinMonth
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a max month the expense was selected.
		/// </summary>
		[DataMember]
		public DateTime MaxMonth
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an old name of the item.
		/// </summary>
		[DataMember]
		public string OldName
		{
			get;
			set;
		}

		#endregion

		#region Construction

		/// <summary>
		/// Creates a new empty object of the <see cref="MonthlyExpense"/> class.
		/// </summary>
		public MonthlyExpense()
		{
			MinMonth = DateTime.MaxValue;
			MaxMonth = DateTime.MinValue;
		}

		#endregion
	}
}

