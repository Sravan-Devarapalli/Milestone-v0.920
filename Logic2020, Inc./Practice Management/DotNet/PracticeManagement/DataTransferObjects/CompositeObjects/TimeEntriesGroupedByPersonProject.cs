using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using DataTransferObjects.TimeEntry;

namespace DataTransferObjects.CompositeObjects
{
    [DataContract(Name = "TimeEntriesGroupedBy{0}{1}")]
    [Serializable]
    public class TimeEntriesGroupedByPersonProject
    {
        [DataMember]
        public string PersonName { get; set; }

        [DataMember]
        public Dictionary<Project, List<TimeEntryRecord>> GroupedTimeEtnries
        { get; set; }
    }
}

