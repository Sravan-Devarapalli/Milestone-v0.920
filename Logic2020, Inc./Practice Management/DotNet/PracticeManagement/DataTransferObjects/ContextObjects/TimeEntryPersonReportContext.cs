using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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
        public int PersonId { get; set; }

        [DataMember]
        public DateTime? StartDate { get; set; }

        [DataMember]
        public DateTime? EndDate { get; set; }

        [DataMember]
        public List<int> PayTypeIds { get; set; }

        [DataMember]
        public List<int> PracticeIds { get; set; }

        [DataMember]
        public List<int> PersonIds { get; set; }
    }
}
