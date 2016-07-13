using System;
using System.Runtime.Serialization;

namespace DataTransferObjects.ContextObjects
{
    /// <summary>
    /// Pepresents activity log select query context
    /// </summary>
    [DataContract]
    [Serializable]
    public class ActivityLogSelectContext
    {
        [DataMember]
        public ActivityEventSource Source { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public int? PersonId { get; set; }

        [DataMember]
        public int? ProjectId { get; set; }

        [DataMember]
        public int? VendorId { get; set; }

        [DataMember]
        public int? OpportunityId { get; set; }

        [DataMember]
        public int? MilestoneId { get; set; }

        [DataMember]
        public bool PracticeAreas{ get; set; }

        [DataMember]
        public bool SowBudget { get; set; }

        [DataMember]
        public bool Director { get; set; }

        [DataMember]
        public bool POAmount { get; set; }

        [DataMember]
        public bool Capabilities { get; set; }

        [DataMember]
        public bool NewOrExtension { get; set; }

        [DataMember]
        public bool ProjectStatus { get; set; }

        [DataMember]
        public bool SalesPerson { get; set; }

        [DataMember]
        public bool ProjectOwner { get; set; }

        [DataMember]
        public bool PONumber { get; set; }

        [DataMember]
        public bool RecordPerSingleChange { get; set; }

        [DataMember]
        public bool Division { get; set; }

        [DataMember]
        public bool Channel { get; set; }

        [DataMember]
        public bool Offering { get; set; }

        [DataMember]
        public bool RevenueType { get; set; }

    }
}

