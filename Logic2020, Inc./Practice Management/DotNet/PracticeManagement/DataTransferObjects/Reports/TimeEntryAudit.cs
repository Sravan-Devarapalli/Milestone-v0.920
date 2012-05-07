using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using DataTransferObjects.TimeEntry;

namespace DataTransferObjects.Reports
{
    [DataContract]
    [Serializable]
    public class TimeEntryAudit
    {
        [DataMember]
        public Person Person
        {
            get;
            set;
        }


        [DataMember]
        public DateTime AffectedDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime ModifiedDate
        {
            get;
            set;
        }

        [DataMember]
        public ChargeCode ChargeCode
        {
            get;
            set;
        }


        [DataMember]
        public double OldHours
        {
            get;
            set;
        }

        [DataMember]
        public double ActualHours
        {
            get;
            set;
        }

        public double NetChange
        {
            get
            {
                return OldHours - ActualHours;
            }
        }

    }
}

