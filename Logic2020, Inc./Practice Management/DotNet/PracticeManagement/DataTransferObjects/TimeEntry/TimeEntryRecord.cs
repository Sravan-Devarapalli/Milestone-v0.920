using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Web;

namespace DataTransferObjects.TimeEntry
{
    /// <summary>
    /// Time type
    /// </summary>
    [DataContract]
    [Serializable]
    [DebuggerDisplay("TimeEntry: Date={ChargeCodeDate}, Actl={ActualHours}, Frcst={ForecastedHours}")]
    public class TimeEntryRecord : IComparable<TimeEntryRecord>
    {
        #region Constants

        private const string ShortNote = "...";
        private const int ShortNoteLen = 25;
        private const string NewLineSeparator = "<br/>";

        #endregion Constants

        #region Properties

        [DataMember]
        public int? Id { get; set; }

        [DataMember]
        public int? ChargeCodeId { get; set; }

        /// <summary>
        /// Date that this time entry is about .
        /// ChargeCodeDate
        /// </summary>
        [DataMember]
        public DateTime ChargeCodeDate { get; set; }

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

        [DataMember]
        public Person ApprovedBy { get; set; }

        [DataMember]
        public ChargeCode ChargeCode
        {
            get;
            set;
        }

        [DataMember]
        public double OldHours
        {
            get;
            set;
        }

        public double NetChange
        {
            get
            {
                return ActualHours - OldHours;
            }
        }

        #endregion Properties

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

        public string HtmlEncodedNote
        {
            get
            {
                return HttpUtility.HtmlEncode(Note).Replace("\r", "&#xD;").Replace("\n", "&#xA;").Replace("\t", "&#9;");
            }
        }

        public string NoteForExport
        {
            get
            {
                return Note.Replace("\n", " ").Replace("\r", " ");
            }
        }

        public string HtmlEncodedNoteForExport
        {
            get
            {
                return HttpUtility.HtmlEncode(NoteForExport);
            }
        }

        #endregion Formatting

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
            return ChargeCodeDate.CompareTo(other.ChargeCodeDate);
        }

        #endregion Implementation of IComparable<TimeEntryRecord>

        [DataMember]
        public double BillableHours { get; set; }

        [DataMember]
        public double NonBillableHours { get; set; }

        public double TotalHours { get { return BillableHours + NonBillableHours; } }

        [DataMember]
        public Person Person { get; set; }
    }
}
