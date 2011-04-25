using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DataTransferObjects.TimeEntry;

namespace DataTransferObjects
{
    /// <summary>
    /// Represents a link between the <see cref="Person"/> and <see cref="Milestone"/>.
    /// </summary>
    [DataContract]
    [Serializable]
    public class MilestonePerson : IActivityPeriod
    {
        #region Properties

        /// <summary>
        /// Gets or sets an ID of the record.
        /// </summary>
        [DataMember]
        public int? Id { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="Milestone"/> part.
        /// </summary>
        [DataMember]
        public Milestone Milestone { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="Project"/> part.
        /// </summary>
        [DataMember]
        public Person Person { get; set; }

        /// <summary>
        /// Gets or sets a list of entries of the person to the milestone.
        /// </summary>
        [DataMember]
        public List<MilestonePersonEntry> Entries { get; set; }

        /// <summary>
        /// Gets or sets a calculated person's rate for the milestone.
        /// </summary>
        /// <remarks>Uses for Milestone-Person Details</remarks>
        [DataMember]
        public ComputedFinancials ComputedFinancials { get; set; }

        /// <summary>
        /// This Property is solely used for Group by Practice Manager Report.
        /// </summary>
        [DataMember]
        public List<Practice> PracticeList { get; set; }

        #endregion

        #region Implementation of IActivityPeriod

        /// <summary>
        /// Start date
        /// </summary>
        public DateTime StartDate
        {
            get { return Entries[0].StartDate; }

            set
            {
                //  Setter is not implemented here, because
                //  it can be calculated for this entity
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// End date
        /// </summary>
        public DateTime EndDate
        {
            get
            {
                DateTime? mpeEndDate = Entries[Entries.Count - 1].EndDate;
                return mpeEndDate.HasValue ? mpeEndDate.Value : Milestone.ProjectedDeliveryDate;
            }
            set
            {
                //  Setter is not implemented here, because
                //  it can be calculated for this entity
                throw new NotImplementedException();
            }
        }

        private List<DatePoint> _emptyPeriodPoints;

        /// <summary>
        /// Empty DatePoint for each date of the period
        /// This will be substituted with working days for given person
        /// </summary>
        public List<DatePoint> EmptyPeriodPoints
        {
            get { return _emptyPeriodPoints.Select(emptyPoint => emptyPoint.Clone()).ToList(); }
            set { _emptyPeriodPoints = value; }
        }

        /// <summary>
        /// Projected values for each date of the period
        /// </summary>
        public List<DatePoint> ProjectedActivity
        {
            get
            {
                var points = EmptyPeriodPoints;

                foreach (MilestonePersonEntry entry in Entries)
                {
                    MilestonePersonEntry personEntry = entry;
                    List<DatePoint> rangePoints =
                        points.FindAll(point => (point.Date >= personEntry.StartDate)
                                                &&
                                                point.Date <= (personEntry.EndDate.HasValue
                                                                   ?
                                                                       personEntry.EndDate.Value
                                                                   :
                                                                       Milestone.ProjectedDeliveryDate));

                    foreach (DatePoint pt in rangePoints)
                        pt.Value = pt.DayOff ? 0.0 : Convert.ToDouble(entry.HoursPerDay);
                }

                return points;
            }
            set
            {
                //  Setter is not implemented here, because
                //  it can be calculated for this entity
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Actual hours for each date of the period
        /// This will be substituted with time entries for given person
        /// </summary>
        public List<TimeEntryRecord> ActualActivity { get; set; }

        #endregion
    }
}
