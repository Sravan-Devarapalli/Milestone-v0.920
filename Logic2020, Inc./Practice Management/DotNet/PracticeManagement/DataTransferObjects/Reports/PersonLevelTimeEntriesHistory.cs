using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DataTransferObjects.TimeEntry;

namespace DataTransferObjects.Reports
{
    [DataContract]
    [Serializable]
    public class PersonLevelTimeEntriesHistory
    {
        [DataMember]
        public Person Person
        {
            get;
            set;
        }

        [DataMember]
        public List<TimeEntryRecord> TimeEntryRecords
        {
            get;
            set;
        }

        public double BillableNetChange
        {
            get
            {
                return TimeEntryRecords.Sum(t => t.IsChargeable ? t.NetChange : 0);
            }
        }

        public double NonBillableNetChange
        {
            get
            {
                return TimeEntryRecords.Sum(t => !t.IsChargeable ? t.NetChange : 0);
            }
        }

        public double NetChange
        {
            get
            {
                return TimeEntryRecords.Sum(t => t.NetChange);
            }
        }
    }
}
