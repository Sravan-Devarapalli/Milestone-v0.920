using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace DataTransferObjects.TimeEntry
{
    /// <summary>
    /// Time type
    /// </summary>
    [DataContract]
    [Serializable]
    [DebuggerDisplay("TimeEntry: Date={MilestoneDate}, Actl={ActualHours}, Frcst={ForecastedHours}")]
    public class TimeEntryRecord : IComparable<TimeEntryRecord>
    {
        #region Constants

        private const string ShortNote = "...";
        private const int ShortNoteLen = 25;
        private const string NewLineSeparator = "<br/>";

        #endregion

        #region Properties

        [DataMember]
        public int? Id { get; set; }

        /// <summary>
        /// Date that this time entry is about
        /// </summary>
        [DataMember]
        public DateTime MilestoneDate { get; set; }

        /// <summary>
        /// Date when the user had entered this time entry
        /// </summary>
        [DataMember]
        public DateTime EntryDate { get; set; }

        /// <summary>
        /// Last modified date
        /// </summary>
        [DataMember]
        public DateTime ModifiedDate { get; set; }

        [DataMember]
        public MilestonePersonEntry ParentMilestonePersonEntry { get; set; }

        [DataMember]
        public double ActualHours { get; set; }

        [DataMember]
        public double ForecastedHours { get; set; }

        [DataMember]
        public TimeTypeRecord TimeType { get; set; }

        [DataMember]
        public Person ModifiedBy { get; set; }

        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public ReviewStatus IsReviewed { get; set; }

        [DataMember]
        public bool IsChargeable { get; set; }

        [DataMember]
        public bool IsCorrect { get; set; }

        #endregion

        #region Formatting

        public string HtmlNote
        {
            get
            {
                return Note.
                    Substring(0, Note.Length > ShortNoteLen ? ShortNoteLen : Note.Length).
                    Replace(Environment.NewLine, NewLineSeparator) + 
                   (Note.Length > ShortNoteLen ? ShortNote : String.Empty);
            }
        }

        #endregion

        #region Implementation of IComparable<TimeEntryRecord>

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(TimeEntryRecord other)
        {
            return MilestoneDate.CompareTo(other.MilestoneDate);
        }

        #endregion
    }
}

