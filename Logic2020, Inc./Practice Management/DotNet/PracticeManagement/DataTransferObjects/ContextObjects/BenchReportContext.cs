using System;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace DataTransferObjects.ContextObjects
{
    [DataContract]
    [Serializable]
    public class BenchReportContext : ConsultantTableReportContext
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string PracticeIds { get; set; }

        [DataMember]
        [DefaultValue(true)]
        public bool? IncludeOverheads { get; set; }

        [DataMember]
        public bool IncludeZeroCostEmployees { get; set; }

        [DataMember]
        public string TimeScaleIds { get; set; }
    }
}

