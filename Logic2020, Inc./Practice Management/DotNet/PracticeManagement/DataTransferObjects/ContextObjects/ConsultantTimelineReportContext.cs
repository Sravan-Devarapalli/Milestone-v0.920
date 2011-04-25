using System;
using System.Runtime.Serialization;

namespace DataTransferObjects.ContextObjects
{
    /// <summary>
    /// Pepresents project cloning context
    /// </summary>
    [DataContract]
    [Serializable]
    //[DebuggerDisplay("ProjectCloningContext: ")]
    public class ConsultantTimelineReportContext : ConsultantReportContextBase
    {
        [DataMember]
        public int Granularity { get; set; }
        [DataMember]
        public int Period { get; set; }
        [DataMember]
        public string TimescaleIdList { get; set; }
        [DataMember]
        public string PracticeIdList { get; set; }
        [DataMember]
        public bool ExcludeInternalPractices { get; set; }
        [DataMember]
        public int SortId { get; set; }
        [DataMember]
        public string SortDirection { get; set; }
        [DataMember]
        public bool IsSampleReport { get; set; }
    }
}
