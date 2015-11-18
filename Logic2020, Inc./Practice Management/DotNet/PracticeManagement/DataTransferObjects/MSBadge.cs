using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class MSBadge
    {
        [DataMember]
        public Person Person
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? BadgeStartDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? BadgeEndDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? PlannedEndDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? BreakStartDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? BreakEndDate
        {
            get;
            set;
        }

        [DataMember]
        public bool IsBlocked
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? BlockStartDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? BlockEndDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? OrganicStartDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? OrganicEndDate
        {
            get;
            set;
        }

        [DataMember]
        public bool IsPreviousBadge
        {
            get;
            set;
        }

        [DataMember]
        public string PreviousBadgeAlias
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? LastBadgeStartDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? LastBadgeEndDate
        {
            get;
            set;
        }

        [DataMember]
        public Project Project
        {
            get;
            set;
        }

        [DataMember]
        public int BadgeDuration
        {
            get;
            set;
        }

        [DataMember]
        public int OrganicBreakDuration
        {
            get;
            set;
        }

        [DataMember]
        public bool IsException
        {
            get;
            set;
        }

        [DataMember]
        public bool IsApproved
        {
            get;
            set;
        }

        [DataMember]
        public bool ExcludeInReports
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? ExceptionStartDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? ExceptionEndDate
        {
            get;
            set;
        }

        [DataMember]
        public string BadgeStartDateSource
        {
            get;
            set;
        }

        [DataMember]
        public string BadgeEndDateSource
        {
            get;
            set;
        }

        [DataMember]
        public string PlannedEndDateSource
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? ProjectBadgeStartDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? ProjectBadgeEndDate
        {
            get;
            set;
        }

        [DataMember]
        public string ModifiedBy
        {
            get;
            set;
        }

        [DataMember]
        public int ModifiedById
        {
            get;
            set;
        }

        [DataMember]
        public DateTime ModifiedDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? DeactivatedDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? OrganicBreakStartDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? OrganicBreakEndDate
        {
            get;
            set;
        }

        [DataMember]
        public string Requester
        {
            get;
            set;
        }

        [DataMember]
        public int? RequesterId
        {
            get;
            set;
        }

        [DataMember]
        public bool IsMSManagedService
        {
            get;
            set;
        }

        [DataMember]
        public Milestone Milestone
        {
            get;
            set;
        }
    }
}

