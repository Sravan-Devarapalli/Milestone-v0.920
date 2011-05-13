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
    public class ConsultantReportContextBase
    {
        [DataMember]
        public DateTime Start { get; set; }
        [DataMember]
        public bool? ActivePersons { get; set; }
        [DataMember]
        public bool? ProjectedPersons { get; set; }
        [DataMember]
        public bool ActiveProjects { get; set; }
        [DataMember]
        public bool ProjectedProjects { get; set; }
        [DataMember]
        public bool InternalProjects { get; set; }
        [DataMember]
        public bool ExperimentalProjects { get; set; }
        [DataMember]
        public bool CompletedProjects { get; set; }
    }
}
