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
        public int? OpportunityId { get; set; }

        [DataMember]
        public int? MilestoneId { get; set; }
    }
}
