using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


        public double NonBillableHours
        {
            get
            {
                if (DayTotalHoursList != null)
                {
                    return DayTotalHoursList.Sum(d => d.NonBillableHours);
                }
                else
                {
                    return 0;
                }
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

