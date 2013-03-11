using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class PersonWorkingHoursDetailsWithinThePeriod
    {

        [DataMember]
        public int PersonId
        {
            get;
            set;
        }

        [DataMember]
        public DateTime StartDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime EndDate
        {
            get;
            set;
        }

        [DataMember]
        public int VacationDays
        {
            get;
            set;
        }

        [DataMember]
        public int TotalWorkDaysIncludingVacationDays
        {
            get;
            set;
        }

        [DataMember]
        public decimal TotalWorkHoursExcludingVacationHours
        {
            get;
            set;
        }

    }
}

