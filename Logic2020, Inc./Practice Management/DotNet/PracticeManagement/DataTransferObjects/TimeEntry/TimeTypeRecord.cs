using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace DataTransferObjects.TimeEntry
{
    /// <summary>
    /// Time type
    /// </summary>
    [DataContract]
    [Serializable]
    [DebuggerDisplay("TimeType: Id = {Id}, Time type = {Name}, In use = {InUse}, Is default = {IsDefault}")]
    public class TimeTypeRecord : IEquatable<TimeTypeRecord>
    {
        #region Properties

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool InUse { get; set; }

        [DataMember]
        public bool IsDefault { get; set; }

        #endregion

        #region Constructors

        public TimeTypeRecord()
        {
        }

        /// <summary>
        /// Init constructor of TimeTypeRecord.
        /// </summary>
        public TimeTypeRecord(int id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Init constructor of TimeTypeRecord.
        /// </summary>
        public TimeTypeRecord(string id) : this(Convert.ToInt32(id))
        {
        }

	    #endregion       
    
        #region IEquatable<TimeTypeRecord> Members

        public bool Equals(TimeTypeRecord other)
        {
            if (other == null)
                return false;

            return this.Id == other.Id;
        }

        #endregion
    }
}

