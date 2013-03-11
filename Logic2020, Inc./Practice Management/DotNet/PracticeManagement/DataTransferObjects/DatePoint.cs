using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using DataTransferObjects.TimeEntry;

namespace DataTransferObjects
{
    [Serializable]
    [DebuggerDisplay("DatePoint: Date = {Date}, Value = {Value}, DayOff = {DayOff}, CompanyDayOff = {CompanyDayOff} , TimeOffHours = {TimeOffHours}")]
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
        public bool DayOff { get; set; }

        /// <summary>
        /// Value, can be null
        /// </summary>
        [DataMember]
        public double? Value { get; set; }

        /// <summary>
        /// Is this date a company daff off for that person
        /// </summary>
        [DataMember]
        public bool CompanyDayOff { get; set; }

        /// <summary>
        /// time off hours for person
        /// </summary>
        [DataMember]
        public double? TimeOffHours { get; set; }

        #endregion

        #region Conversion

        public static DatePoint Create(CalendarItem cItem, double? val)
        {
            return new DatePoint
            {
                Date = cItem.Date,
                Value = val,
                DayOff = cItem.DayOff,
                CompanyDayOff = cItem.CompanyDayOff,
                TimeOffHours = cItem.ActualHours
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
                DayOff = DayOff,
                CompanyDayOff = CompanyDayOff,
                TimeOffHours = TimeOffHours
            };
        }

        public static DatePoint FromTimeEntry(TimeEntryRecord te)
        {
            return new DatePoint
            {
                Date = te.ChargeCodeDate,
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

