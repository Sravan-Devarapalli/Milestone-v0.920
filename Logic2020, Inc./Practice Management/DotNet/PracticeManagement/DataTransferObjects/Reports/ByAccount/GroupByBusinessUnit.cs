using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports.ByAccount
{
    [DataContract]
    [Serializable]
    public class GroupByBusinessUnit
    {
        [DataMember]
        public ProjectGroup BusinessUnit
        {
            get;
            set;
        }

        [DataMember]
        public List<TimeEntriesGroupByDate> DayTotalHours
        {
            get;
            set;
        }

        public double TotalHours
        {
            get
            {
                return DayTotalHours != null ? DayTotalHours.Sum(d => d.TotalHours) : 0d;
            }
        }

        public void AddDayTotalHours(TimeEntriesGroupByDate dt)
        {
            if (DayTotalHours.Any(dth => dth.Date == dt.Date))
            {
                var workType = dt.DayTotalHoursList[0];
                dt = DayTotalHours.First(dth => dth.Date == dt.Date);
                dt.DayTotalHoursList.Add(workType);
            }
            else
            {
                DayTotalHours.Add(dt);
            }
        }
    }
}
