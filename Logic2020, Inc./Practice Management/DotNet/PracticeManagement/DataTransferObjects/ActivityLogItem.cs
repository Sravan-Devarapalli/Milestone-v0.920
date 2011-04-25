using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Represents an activity log item.
	/// </summary>
	[Serializable]
	[DataContract]
	public class ActivityLogItem
	{
		/// <summary>
		/// Gets or sets an item's ID.
		/// </summary>
		[DataMember]
		public int Id
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an ID of the activity type.
		/// </summary>
		[DataMember]
		public int ActivityTypeId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a name of the activity.
		/// </summary>
		[DataMember]
		public string ActivityName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an ID of the SQL Server session the activity occurs within.
		/// </summary>
		[DataMember]
		public int SessionId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a date when an activity occurs.
		/// </summary>
		[DataMember]
		public DateTime LogDate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a SQL Server user name.
		/// </summary>
		[DataMember]
		public string SystemUser
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an activity source workstation.
		/// </summary>
		[DataMember]
		public string Workstation
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an activity source application.
		/// </summary>
		[DataMember]
		public string ApplicationName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a PracticeManager user.
		/// </summary>
		[DataMember]
		public Person Person
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a log item data.
		/// </summary>
		[DataMember]
		public string LogData
		{
			get;
			set;
		}
	}
}

