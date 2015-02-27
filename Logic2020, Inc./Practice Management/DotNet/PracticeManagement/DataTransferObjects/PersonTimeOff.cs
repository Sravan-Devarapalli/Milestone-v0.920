using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using DataTransferObjects.TimeEntry;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class PersonTimeOff
    {
        [DataMember]
        public TimeTypeRecord TimeType
        {
            get;
            set;
        }

        [DataMember]
        public DateTime TimeOffStartDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime TimeOffEndDate
        {
            get;
            set;
        }

        [DataMember]
        public Project Project
        {
            get;
            set;
        }

        [DataMember]
        public List<Project> Projects
        {
            get;
            set;
        }
    }
}

