using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DataTransferObjects.TimeEntry;

namespace DataTransferObjects.CompositeObjects
{
    /// <summary>
    /// Represents project report grouped by project
    /// </summary>
    [DataContract]
    [Serializable]
    public class TimeEntryPersonProjects : IEnumerable<KeyValuePair<Project, List<TimeEntryRecord>>>
    {
        [DataMember]
        private readonly Dictionary<Project, List<TimeEntryRecord>> _projectTimeEtnries;

        public TimeEntryPersonProjects()
        {
            _projectTimeEtnries = new Dictionary<Project, List<TimeEntryRecord>>(new ProjectEqualityComparer());
        }

        public void AddTimeEntryForProject(Project project, TimeEntryRecord timeEntry)
        {
            try
            {
                _projectTimeEtnries[project].Add(timeEntry);
            }
            catch (Exception)
            {
                _projectTimeEtnries.Add(project, new List<TimeEntryRecord> {timeEntry});
            }
        }

        #region Implementation of IEnumerable

        public IEnumerator<KeyValuePair<Project, List<TimeEntryRecord>>> GetEnumerator()
        {
            return _projectTimeEtnries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
