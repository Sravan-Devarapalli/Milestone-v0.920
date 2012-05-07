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
    public class ProjectLevelTimeEntriesHistory
    {
        [DataMember]
        public Project Project
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

        public double NetChange
        {
            get
            {
                return TimeEntryRecords.Sum(t => t.NetChange);
            }
        }
    }
}

