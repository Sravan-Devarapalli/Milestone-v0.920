using System;
using System.Runtime.Serialization;

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
    }
}

