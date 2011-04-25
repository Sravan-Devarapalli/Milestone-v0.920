using System;
using System.Diagnostics;
using DataTransferObjects;

namespace PraticeManagement.Controls.MilestonePersons
{
    [DebuggerDisplay("DatePoint: Date = {Date}, Value = {Value")]
    public class DatePoint : IEquatable<DatePoint>, IComparable<DatePoint>
    {
        public DateTime Date { get; set; }
        public double? Value { get; set; }

        #region Conversion

	    public static DatePoint Create(CalendarItem cItem, double? val)
        {
            return new DatePoint()
            {
                Date = cItem.Date,
                Value = val
            };
        }

        public static DatePoint Create(CalendarItem cItem)
        {
            return Create(cItem, null);
        }

	    #endregion    

        #region IEquatable<DatePoint> Members

        public bool Equals(DatePoint other)
        {
            return (this.Date == other.Date) && (this.Value == other.Value);
        }

        #endregion

        #region IComparable<DatePoint> Members

        public int CompareTo(DatePoint other)
        {
            return this.Date.CompareTo(other.Date);
        }

        #endregion
    }
}

