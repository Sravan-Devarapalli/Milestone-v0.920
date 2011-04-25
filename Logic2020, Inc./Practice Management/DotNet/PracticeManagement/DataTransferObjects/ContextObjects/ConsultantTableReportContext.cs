using System;
using System.Runtime.Serialization;

namespace DataTransferObjects.ContextObjects
{
    /// <summary>
    /// Represents project cloning context
    /// </summary>
    [DataContract]
    [Serializable]
    public class ConsultantTableReportContext : ConsultantReportContextBase
    {
        [DataMember]
        public DateTime End { get; set; }
    }
}

