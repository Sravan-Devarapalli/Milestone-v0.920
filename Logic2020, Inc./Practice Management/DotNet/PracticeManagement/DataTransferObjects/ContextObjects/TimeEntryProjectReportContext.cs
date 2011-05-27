using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace DataTransferObjects.ContextObjects
{
    /// <summary>
    /// Represents Time Entry Project Report Context
    /// </summary>
    [DataContract]
    [Serializable]
    public class TimeEntryProjectReportContext
    {
        [DataMember]
        public int ProjectId { get; set; }
        [DataMember]
        public DateTime? StartDate { get; set; }
        [DataMember]
        public DateTime? EndDate { get; set; }
        [DataMember]
        public IEnumerable<int> PersonIds { get; set; }
        [DataMember]
        public int? MilestoneId { get; set; }
    }
}

