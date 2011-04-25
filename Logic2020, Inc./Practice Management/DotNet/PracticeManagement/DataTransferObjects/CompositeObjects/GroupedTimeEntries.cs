using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DataTransferObjects.TimeEntry;

namespace DataTransferObjects.CompositeObjects
{
    /// <summary>
    /// Represents time entry report grouped by an entity
    /// </summary>
    [DataContract(Name = "TimeEntriesGroupedBy{0}")]
    [Serializable]
    public class GroupedTimeEntries<T> : IEnumerable<KeyValuePair<T, List<TimeEntryRecord>>> where T : IIdNameObject
    {
        [DataMember]
        private readonly Dictionary<T, List<TimeEntryRecord>> _groupedTimeEtnries;

        public GroupedTimeEntries()
        {
            _groupedTimeEtnries = new Dictionary<T, List<TimeEntryRecord>>(new IdNameEqualityComparer<T>());
        }

        public void AddTimeEntry(T entity, TimeEntryRecord timeEntry)
        {
            try
            {
                _groupedTimeEtnries[entity].Add(timeEntry);
            }
            catch (Exception)
            {
                _groupedTimeEtnries.Add(entity, new List<TimeEntryRecord> {timeEntry});
            }
        }

        #region Implementation of IEnumerable

        public IEnumerator<KeyValuePair<T, List<TimeEntryRecord>>> GetEnumerator()
        {
            return _groupedTimeEtnries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
