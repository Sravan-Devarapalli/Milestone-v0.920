using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{
    [DataContract]
    [Serializable]
    public class TimeEntriesGroupByDate
    {
        [DataMember]
        public DateTime Date
        {
            get;
            set;
        }

        public double BillableHours
        {
            get
            {
                return DayTotalHoursList != null ? DayTotalHoursList.Sum(d => d.BillableHours) : 0;
            }
        }

        public double NonBillableHours
        {
            get
            {
                return DayTotalHoursList != null ? DayTotalHoursList.Sum(d => d.NonBillableHours) : 0;
            }
        }

        public double TotalHours
        {
            get
            {
                return DayTotalHoursList != null ? DayTotalHoursList.Sum(p => p.TotalHours) : 0;
            }
        }

        [DataMember]
        public List<TimeEntryByWorkType> DayTotalHoursList
        {
            get;
            set;
        }
    }
}
