using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Represents a Week Paid Option database entity
	/// </summary>
	[DataContract]
	[Serializable]
	public class WeekPaidOption
	{
		/// <summary>
		/// Gets or sets an ID of the Week Paid Option.
		/// </summary>
		[DataMember]
		public int Id
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a name of the Week Paid Option.
		/// </summary>
		[DataMember]
		public string Name
		{
			get;
			set;
		}
	}
}

