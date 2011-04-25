using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DataTransferObjects.TimeEntry;

namespace DataTransferObjects
{
    /// <summary>
    /// Data Transfer Object for a Milestone entity
    /// </summary>
    [DataContract]
    [Serializable]
    public class Milestone : IActivityPeriod
    {
        #region Fields

        private List<TimeEntryRecord> _actualActivity;

        #endregion

        #region Constants

        private const string PROJECT_MILESTONE_FORMAT = "{0} - {1}";
        private const string CLIENT_PROJECT_MILESTONE_FORMAT = "{0} - {1} - {2}";

        #endregion

        #region Nested

        /// <summary>
        /// Formats used in ToString()
        /// </summary>
        public enum MilestoneFormat
        {
            ClientProjectMilestone,
            ProjectMilestone,
            MilestoneProject
        }

        #endregion

        #region Properties - Data members

        /// <summary>
        /// Gets or sets a <see cref="Project"/> the <see cref="Milestone"/> relates to.
        /// </summary>
        [DataMember]
        public Project Project { get; set; }

        /// <summary>
        /// Gets or sets an ID of the <see cref="Milestone"/>.
        /// </summary>
        [DataMember]
        public int? Id { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Milestone"/>'s description.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="Milestone"/>'s cost.
        /// </summary>
        [DataMember]
        public PracticeManagementCurrency? Amount { get; set; }

        /// <summary>
        /// Gets or sets whether the <see cref="Amount"/> is the hourly-based amount.
        /// </summary>
        [DataMember]
        public bool IsHourlyAmount { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="Milestone"/>'s planning delivary date.
        /// </summary>
        [DataMember]
        public DateTime ProjectedDeliveryDate { get; set; }

        /// <summary>
        /// Gets or sets a computed financials for the milestone.
        /// </summary>
        [DataMember]
        public ComputedFinancials ComputedFinancials { get; set; }

        /// <summary>
        /// Gets or sets a date when the the <see cref="Milestone"/> has been delivered.
        /// </summary>
        [DataMember]
        public DateTime? ActualDeliveryDate { get; set; }

        /// <summary>
        /// Gets or sets a number of expected hours for the <see cref="Milestone"/>.
        /// </summary>
        [DataMember]
        public decimal ExpectedHours { get; set; }

        /// <summary>
        /// Gets an estimated Client Discount
        /// </summary>
        [Obsolete]
        public PracticeManagementCurrency EstimatedClientDiscount
        {
            get { return Project.Discount*Amount.Value/100M; }
        }

        /// <summary>
        /// Gets an estimated Revenue Net for the milestone.
        /// </summary>
        /// <remarks>The <see cref="Project"/> property must be set correctly.</remarks>
        [Obsolete]
        public PracticeManagementCurrency EstimatedRevenueNet
        {
            get
            {
                PracticeManagementCurrency result =
                    Amount.HasValue ? Amount.Value.Value - EstimatedClientDiscount.Value : 0M;
                result.FormatStyle = NumberFormatStyle.Revenue;

                return result;
            }
        }

        /// <summary>
        /// Gets or sets a number of the <see cref="Person"/>s assigned to the milestone.
        /// </summary>
        [DataMember]
        public int PersonCount { get; set; }

        /// <summary>
        /// Gets or sets a projected duration of the milestone in days.
        /// </summary>
        [DataMember]
        public int ProjectedDuration { get; set; }

        /// <summary>
        /// Time entries in this milestone are chargeable by default.
        /// </summary>
        [DataMember]
        public bool IsChargeable { get; set; }

        /// <summary>
        /// Chargeability of time entries in this milestone can be adjusted by consultants.
        /// </summary>
        [DataMember]
        public bool ConsultantsCanAdjust { get; set; }

        /// <summary>
        /// Persons working for the corresponding milestone
        /// </summary>
        [DataMember]
        public MilestonePerson[] MilestonePersons { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="Milestone"/>'s Start Date.
        /// </summary>
        [DataMember]
        public DateTime StartDate { get; set; }

        #endregion

        #region Properties - calculated

        /// <summary>
        /// Substitution for ToString
        /// Used where it's not allowed to use methods, but properties
        /// </summary>
        public string MilestoneProjectTitle
        {
            get { return ToString(MilestoneFormat.ProjectMilestone); }
        }

        #endregion

        #region Methods

        public string ToString(MilestoneFormat format)
        {
            switch (format)
            {
                case MilestoneFormat.ProjectMilestone:
                    return string.Format(
                                            PROJECT_MILESTONE_FORMAT,
                                            Project.Name,
                                            Description);

                case MilestoneFormat.MilestoneProject:
                    return string.Format(
                                            PROJECT_MILESTONE_FORMAT,
                                            Description,
                                            Project.Name);

                case MilestoneFormat.ClientProjectMilestone:
                    return string.Format(
                                            CLIENT_PROJECT_MILESTONE_FORMAT,
                                            Project.Client.Name,
                                            Project.Name,
                                            Description);

                default:
                    return base.ToString();
            }
        }

        #endregion

        #region IActivityPeriod members

        /// <summary>
        /// End date
        /// </summary>
        public DateTime EndDate
        {
            get
            {
                return ActualDeliveryDate.HasValue
                           ?
                               ActualDeliveryDate.Value
                           :
                               ProjectedDeliveryDate;
            }
            set
            {
                //  Setter is not implemented here, because
                //  it can be calculated for this entity
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Empty DatePoint for each date of the period
        /// </summary>
        public List<DatePoint> EmptyPeriodPoints
        {
            get
            {
                int daysnum = EndDate.Subtract(StartDate).Days;
                var points = new List<DatePoint>(daysnum);
                for (int i = 0; i <= daysnum; i++)
                {
                    points.Add(new DatePoint
                                   {
                                       Date = StartDate.Add(new TimeSpan(i, 0, 0, 0)),
                                       Value = null
                                   });
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
        /// Projected values for each date of the period
        /// </summary>
        public List<DatePoint> ProjectedActivity
        {
            get
            {
                List<DatePoint> datePoints = EmptyPeriodPoints;
                if (ComputedFinancials == null)
                    return datePoints;

                foreach (var milestonePerson in MilestonePersons)
                {
                    foreach (var milestonePersonEntry in milestonePerson.Entries)
                    {
                        var mpe = milestonePersonEntry;
                        var person = milestonePerson;
                        var allPointsInRange = datePoints.
                            FindAll(delegate(DatePoint point)
                                        {
                                            return point.Date >= mpe.StartDate
                                                   &&
                                                   point.Date <=
                                                   (mpe.EndDate.HasValue
                                                        ?
                                                            mpe.EndDate.Value
                                                        : person.EndDate)
                                                   && point.Date.DayOfWeek != DayOfWeek.Saturday
                                                   && point.Date.DayOfWeek != DayOfWeek.Sunday;
                                        });
                        allPointsInRange.ForEach(delegate(DatePoint point)
                                                     {
                                                         var hours = Convert.ToDouble(mpe.HoursPerDay);
                                                         if (point.Value.HasValue)
                                                             point.Value += hours;
                                                         else
                                                             point.Value = hours;
                                                     });
                    }
                }

                //var points = EmptyPeriodPoints;
                //foreach (var point in points)
                //{
                //    point.Value = Convert.ToDouble(ComputedFinancials.HoursBilled);
                //}

                return datePoints;
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
        /// </summary>
        public List<TimeEntryRecord> ActualActivity
        {
            get { return _actualActivity; }
            set { _actualActivity = value; }
        }

        #endregion
    }
}
