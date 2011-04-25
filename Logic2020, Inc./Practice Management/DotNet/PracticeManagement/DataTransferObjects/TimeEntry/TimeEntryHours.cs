using System;
using System.Runtime.Serialization;

namespace DataTransferObjects.TimeEntry
{
    /// <summary>
    /// Reported hours, related to certain entity
    /// </summary>
    [DataContract]
    [Serializable]
    public class TimeEntryHours : IIdNameObject
    {
        /// <summary>
        /// Represents a calendar day
        /// </summary>
        [DataMember]
        public CalendarItem Calendar;

        #region Implementation of IIdNameObject

        /// <summary>
        /// Entity id. This may be project, milestone etc id
        /// </summary>
        [DataMember]
        public int? Id { get; set; }

        /// <summary>
        /// Entity name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        #endregion

        public override string ToString()
        {
            return Name;
        }
    }
}
