using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using DataTransferObjects.TimeEntry;

namespace DataTransferObjects
{
    [Serializable]
    [DebuggerDisplay("DatePoint: Date = {Date}, Value = {Value}, DayOff = {DayOff}")]
    public class DatePoint : IEquatable<DatePoint>, IComparable<DatePoint>
    {
        #region Properties

        /// <summary>
        /// Date
        /// </summary>
        [DataMember]
        public DateTime Date { get; set; }

        /// <summary>
        /// Is this date a daff off for that person
        /// </summary>
        [DataMember]
        public bool DayOff{ get; set; }

        /// <summary>
        /// Value, can be null
        /// </summary>
        [DataMember]
        public double? Value { get; set; }

        #endregion

        #region Conversion

        public static DatePoint Create(CalendarItem cItem, double? val)
        {
            return new DatePoint
                       {
                           Date = cItem.Date,
                           Value = val,
                           DayOff = cItem.DayOff
                       };
        }

        public static DatePoint Create(CalendarItem cItem)
        {
            return Create(cItem, null);
        }

        public DatePoint Clone()
        {
            return new DatePoint
                       {
                           Date = Date,
                           Value = Value,
                           DayOff = DayOff
                       };
        }

        public static DatePoint FromTimeEntry(TimeEntryRecord te)
        {
            return new DatePoint
                       {
                           Date = te.MilestoneDate,
                           Value = te.ActualHours
                       };
        }

        #endregion    

        #region IEquatable<DatePoint> Members

        public bool Equals(DatePoint other)
        {
            return (Date == other.Date) && (Value == other.Value) && (DayOff == other.DayOff);
        }

        #endregion

        #region IComparable<DatePoint> Members

        public int CompareTo(DatePoint other)
        {
            return Date.CompareTo(other.Date);
        }

        #endregion
    }
}
