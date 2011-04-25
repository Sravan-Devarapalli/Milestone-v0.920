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
    public class TimeEntryPersonReportContext
    {
        [DataMember]
        public IEnumerable<int> PersonIds { get; set; }
        [DataMember]
        public DateTime? StartDate { get; set; }
        [DataMember]
        public DateTime? EndDate { get; set; }
        [DataMember]
        public IEnumerable<int> PayTypeIds { get; set; }
        [DataMember]
        public IEnumerable<int> PracticeIds { get; set; }

    }
}
