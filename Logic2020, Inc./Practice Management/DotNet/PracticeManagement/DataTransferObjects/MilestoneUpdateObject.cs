using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}

