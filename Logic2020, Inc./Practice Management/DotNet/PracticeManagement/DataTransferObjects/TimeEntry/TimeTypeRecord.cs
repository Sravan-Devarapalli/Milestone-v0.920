using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Collections.Generic;

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
        public string Code { get; set; }

        [DataMember]
        public bool InUse { get; set; }

        [DataMember]
        public bool InFutureUse { get; set; }

        [DataMember]
        public bool IsDefault { get; set; }

        [DataMember]
        public bool IsSystemTimeType
        {
            get
            {
                return !IsAllowedToEdit;

            }
            set
            {
                IsAllowedToEdit = !value;
            }
        }

        [DataMember]
        public bool IsAllowedToEdit { get; set; }

        [DataMember]
        public bool IsInternal { get; set; }

        [DataMember]
        public bool IsAdministrative { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public bool IsORTTimeType { get; set; }//ORT:- "Other Reportable Time" work type.

        [DataMember]
        public bool IsUnpaidTimeType { get; set; }

        public bool IsPTOTimeType { get; set; }

        public bool IsHolidayTimeType { get; set; }

        public bool IsJuryDutyTimeType { get; set; }

        public bool IsSickLeaveTimeType { get; set; }

        [DataMember]
        public bool IsBereavementTimeType { get; set; }

        [DataMember]
        public bool IsW2HourlyAllowed { get; set; }

        [DataMember]
        public bool IsW2SalaryAllowed { get; set; }

        [DataMember]
        public string Category
        {
            get;
            set;
        }

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
        public TimeTypeRecord(string id)
            : this(Convert.ToInt32(id))
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

