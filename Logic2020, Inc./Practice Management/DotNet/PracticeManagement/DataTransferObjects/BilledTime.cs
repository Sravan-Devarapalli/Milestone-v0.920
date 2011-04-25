using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    /// <summary>
    /// Person bills time against a milestone
    /// </summary>
    [DataContract]
	[Serializable]
	[DebuggerDisplay("DataTransferObjects.BilledTime; Date = {DateBilled}, Hours = {HoursBilled}")]
	public class BilledTime
    {
        private Milestone _milestoneBilled;
        private Person _biller;
        private DateTime _dateBilled;
        private decimal _hoursBilled;

        /// <summary>
        /// <see cref="Milestone"/> against which this time is billed
        /// </summary>
        [DataMember]
        public Milestone MilestoneBilled
        {
            get { return _milestoneBilled; }
            set { _milestoneBilled = value; }
        }

        /// <summary>
        /// Date time was expended
        /// </summary>
        /// <remarks>
        /// A milestone billing granularity is a day.  If a <see cref="Person"/> bills for more than
        /// one time period in a single day, all the times are aggregated into the day's billed time.
        /// </remarks>
        [DataMember]
        public DateTime DateBilled
        {
            get { return _dateBilled; }
            set { _dateBilled = value; }
        }

        /// <summary>
        /// Hours billed by the person against the milestone on the date billed
        /// </summary>
        [DataMember]
        public decimal HoursBilled
        {
            get { return _hoursBilled; }
            set { _hoursBilled = value; }
        }

        /// <summary>
        /// <see cref="Person"/> billing the time
        /// </summary>
        public Person Biller
        {
            get { return _biller; }
            set { _biller = value; }
        }

		/// <summary>
		/// Gets or sets a date when the person starts working on the milestone.
		/// </summary>
		[DataMember]
		public DateTime EntryStartDate
		{
			get;
			set;
		}
    }
}
