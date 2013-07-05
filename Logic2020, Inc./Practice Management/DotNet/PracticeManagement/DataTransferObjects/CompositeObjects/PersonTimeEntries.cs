using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DataTransferObjects.TimeEntry;

namespace DataTransferObjects.CompositeObjects
{
    [DataContract]
    [Serializable]
    public class PersonTimeEntries
    {
        [DataMember]
        public Person Person { get; set; }

        public PersonTimeEntries()
        {
            GroupedTimeEtnries = new Dictionary<ChargeCode, List<TimeEntryRecord>>();
        }

        [DataMember]
        public Dictionary<ChargeCode, List<TimeEntryRecord>> GroupedTimeEtnries
        {
            get;
            set;
        }

        public void AddTimeEntry(TimeEntryRecord timeEntry)
        {
            if (GroupedTimeEtnries.Any(k => k.Key.ChargeCodeId == timeEntry.ChargeCode.ChargeCodeId))
            {
                var cc = GroupedTimeEtnries.First(k => k.Key.ChargeCodeId == timeEntry.ChargeCode.ChargeCodeId).Key;
                GroupedTimeEtnries[cc].Add(timeEntry);
            }
            else
            {
                GroupedTimeEtnries.Add(timeEntry.ChargeCode, new List<TimeEntryRecord> { timeEntry });
            }
        }
    }
}
