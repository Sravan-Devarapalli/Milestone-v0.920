using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class MilestoneUpdateObject
    {
        [DataMember]
        public bool? IsStartDateChangeReflectedForMilestoneAndPersons { get; set; }

        [DataMember]
        public bool? IsEndDateChangeReflectedForMilestoneAndPersons { get; set; }

        [DataMember]
        public bool? IsExtendedORCompleteOutOfRange { get; set; }
    }
}
