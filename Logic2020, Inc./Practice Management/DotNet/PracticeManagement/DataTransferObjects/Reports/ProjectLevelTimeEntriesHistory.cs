using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{
    [DataContract]
    [Serializable]
    public class ProjectLevelTimeEntriesHistory
    {
        [DataMember]
        public Project Project
        {
            get;
            set;
        }

        [DataMember]
        public List<PersonLevelTimeEntriesHistory> PersonLevelTimeEntries
        {
            get;
            set;
        }

        public double BillableNetChange
        {
            get
            {
                return PersonLevelTimeEntries.Sum(t => t.BillableNetChange);
            }
        }

        public double NonBillableNetChange
        {
            get
            {
                return PersonLevelTimeEntries.Sum(t => t.NonBillableNetChange);
            }
        }

        public double NetChange
        {
            get
            {
                return PersonLevelTimeEntries.Sum(t => t.NetChange);
            }
        }
    }
}
