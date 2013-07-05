using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DataTransferObjects.TimeEntry
{
    [DataContract]
    [Serializable]
    public class TimeEntrySection
    {
        #region Properties

        [DataMember]
        public TimeEntrySectionType SectionId { get; set; }

        [DataMember]
        public Client Account { get; set; }

        [DataMember]
        public Project Project { get; set; }

        [DataMember]
        public ProjectGroup BusinessUnit
        {
            get;
            set;
        }

        [DataMember]
        public bool IsRecursive
        {
            get;
            set;
        }

        [DataMember]
        public List<TimeEntryRecord> TimeEntries
        {
            get;
            set;
        }

        public List<KeyValuePair<TimeTypeRecord, List<TimeEntryRecord>>> TimeEntriesByTimeType
        {
            get
            {
                var kvList = new List<KeyValuePair<TimeTypeRecord, List<TimeEntryRecord>>>();

                var timeEntries = new List<TimeEntryRecord>();

                if (TimeEntries != null && TimeEntries.Count > 0)
                {
                    timeEntries = TimeEntries;

                    foreach (TimeEntryRecord terecord in timeEntries)
                    {
                        if (kvList.Any(k => k.Key.Id == terecord.TimeType.Id))
                        {
                            var keyValuePair = kvList.First(k => k.Key.Id == terecord.TimeType.Id);

                            if (keyValuePair.Value == null)
                            {
                                keyValuePair = new KeyValuePair<TimeTypeRecord, List<TimeEntryRecord>>(terecord.TimeType, new List<TimeEntryRecord> { terecord });
                            }
                            else
                            {
                                keyValuePair.Value.Add(terecord);
                            }
                        }
                        else
                        {
                            kvList.Add(new KeyValuePair<TimeTypeRecord, List<TimeEntryRecord>>(terecord.TimeType, new List<TimeEntryRecord> { terecord }));
                        }
                    }
                }
                else
                {
                    int ttypeId = kvList.Count > 0 ? kvList.Min(k => k.Key.Id) : 0;
                    timeEntries.Add(new TimeEntryRecord { TimeType = new TimeTypeRecord() });
                    kvList.Add(new KeyValuePair<TimeTypeRecord, List<TimeEntryRecord>>(new TimeTypeRecord { Id = ttypeId < 1 ? ttypeId - 1 : -1 }, timeEntries));
                }

                TimeEntries = timeEntries;

                return kvList;
            }
        }

        #endregion Properties
    }
}
