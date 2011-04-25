using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    public enum MilestonePersonEntryFormat
    {
        ProjectMilestoneStartDate,
        Default = ProjectMilestoneStartDate
    }

    [Serializable]
    [DataContract]
    [DebuggerDisplay("MilestonePersonEntry; MilestonePersonId = {MilestonePersonId}, StartDate = {StartDate}, EndDate = {EndDate}")]
    public class MilestonePersonEntry : IEquatable<MilestonePersonEntry>, IComparable<MilestonePersonEntry>
    {
        #region MyRegion

        private const string PROJECT_MILESTONE_START_DATE_FORMAT = "{0} ({1})";

        #endregion

        #region Properties

        /// <summary>
        /// Corresponding milestone
        /// </summary>
        [DataMember]
        public Milestone ParentMilestone { get; set; }

        /// <summary>
        /// Corresponding person
        /// </summary>
        [DataMember]
        public Person ThisPerson { get; set; }

        /// <summary>
        /// Gets or sets an ID of the parent record
        /// </summary>
        [DataMember]
        public int MilestonePersonId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets when the <see cref="Person"/> starts his/her work on the <see cref="Milestone"/>.
        /// </summary>
        [DataMember]
        public DateTime StartDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets when the <see cref="Person"/> ends his/her work on the <see cref="Milestone"/>.
        /// </summary>
        [DataMember]
        public DateTime? EndDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a number of working hours per day for the person on the given milestone.
        /// </summary>
        [DataMember]
        public decimal HoursPerDay
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a person location.
        /// </summary>
        [DataMember]
        public String Location
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a hourly bill rate for the project milestone association
        /// </summary>
        [DataMember]
        public PracticeManagementCurrency? HourlyAmount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an VacationDays of the person on Milestone
        /// </summary>
        [DataMember]
        public int VacationDays
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a calculated person's rate for the milestone.
        /// </summary>
        [DataMember]
        public ComputedFinancials ComputedFinancials
        {
            get;
            set;
        }

        /// <summary>
        /// Gets an expected workload.
        /// </summary>
        [DataMember]
        public decimal ProjectedWorkload
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an estimated Client Discount.
        /// </summary>
        [DataMember]
        [Obsolete]
        public PracticeManagementCurrency EstimatedClientDiscount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets an expected workload + vacation days.
        /// </summary>
        public decimal ProjectedWorkloadWithVacation
        {
            get
            {
                return ProjectedWorkload + VacationDays * HoursPerDay;
            }
        }

        /// <summary>
        /// Gets an expected ProjectedWorkload -  (VacationDays * HoursPerDay).
        /// </summary>
        public decimal BillableHours
        {
            get
            {
                return ProjectedWorkloadWithVacation - (VacationDays * HoursPerDay);
            }
        }

        /// <summary>
        /// Gets or sets a list of the projected bill entries.
        /// </summary>
        /// <remarks>May be used to specify as a daily as a monthly activity.</remarks>
        [DataMember]
        public List<BilledTime> EstimatedWorkloadByMonth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a person role for the milestone.
        /// </summary>
        [DataMember]
        public PersonRole Role
        {
            get;
            set;
        }

        #endregion

        #region Constructor

        public MilestonePersonEntry()
        {
        }

        /// <summary>
        /// Init constructor of MilestonePersonEntry.
        /// </summary>
        public MilestonePersonEntry(string milestonePersonId)
            : this(Convert.ToInt32(milestonePersonId))
        {
        }

        /// <summary>
        /// Init constructor of MilestonePersonEntry.
        /// </summary>
        public MilestonePersonEntry(int milestonePersonId)
        {
            MilestonePersonId = milestonePersonId;

        }

        #endregion

        #region IEquatable<MilestonePersonEntry> Members

        public bool Equals(MilestonePersonEntry other)
        {
            return other != null && MilestonePersonId == other.MilestonePersonId;
        }

        #endregion

        #region IComparable<MilestonePersonEntry> Members

        public int CompareTo(MilestonePersonEntry other)
        {
            return DateTime.Compare(StartDate, other.StartDate);
        }

        #endregion

        #region ToString

        public string ToString(MilestonePersonEntryFormat format)
        {
            switch (format)
            {
                case MilestonePersonEntryFormat.ProjectMilestoneStartDate:
                    return
                        string.Format(
                            PROJECT_MILESTONE_START_DATE_FORMAT,
                            ParentMilestone.ToString(Milestone.MilestoneFormat.ProjectMilestone),
                            StartDate.ToShortDateString());
            }

            return base.ToString();
        }

        public override string ToString()
        {
            return ToString(MilestonePersonEntryFormat.Default);
        }

        #endregion
    }
}

