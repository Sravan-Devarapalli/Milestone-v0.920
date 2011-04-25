using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Represents a calendar day
	/// </summary>
	[DataContract]
	[Serializable]
	public class CalendarItem : IComparable<CalendarItem>, IEquatable<DateTime>, IEquatable<CalendarItem>
	{
		#region Properties

		/// <summary>
		/// Gets or sets an item's date.
		/// </summary>
		[DataMember]
		public DateTime Date
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets is a day is day-off
		/// </summary>
		[DataMember]
		public bool DayOff
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets whether the day is the company-wide day off.
		/// </summary>
		[DataMember]
		public bool CompanyDayOff
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an ID of the person
		/// </summary>
		[DataMember]
		public int? PersonId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets if the calendar item is read only for some raeson.
		/// </summary>
		/// <remarks>
		/// Uses to protect data from unapproved editing or when editing is impossible.
		/// </remarks>
		[DataMember]
		public bool ReadOnly
		{
			get;
			set;
		}

		#endregion

	    public int CompareTo(CalendarItem other)
	    {
	        return other == null ? -1 : other.Date.CompareTo(Date);
	    }

	    public bool Equals(DateTime other)
	    {
	        return other.Equals(Date);
	    }

	    public bool Equals(CalendarItem other)
	    {
	        return other == null ? false : Equals(other.Date);
	    }

        public static implicit operator CalendarItem(DateTime d)
        {
            return ToCalendarItem(d);
        }

	    public static CalendarItem ToCalendarItem(DateTime d)
	    {
	        return new CalendarItem {Date = d};
	    }
	}
}

