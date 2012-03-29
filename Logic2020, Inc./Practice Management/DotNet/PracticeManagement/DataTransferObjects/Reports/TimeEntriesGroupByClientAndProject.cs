using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using DataTransferObjects.TimeEntry;

namespace DataTransferObjects.Reports
{
    [DataContract]
    [Serializable]
    public class TimeEntriesGroupByClientAndProject
    {
        [DataMember]
        public Client Client
        {
            get;
            set;
        }

        [DataMember]
        public Project Project
        {
            get;
            set;
        }

        [DataMember]
        public double BillableHours { get; set; }

        [DataMember]
        public double NonBillableHours { get; set; }

        [DataMember]
        public string BillableType { get; set; }

        [DataMember]
        public List<TimeEntriesGroupByDate> DayTotalHours
        {
            get;
            set;
        }

        public int ProjectTotalHoursPercent
        {
            get;
            set;
        }

        public int TotalHoursPercentExceptThisProject
        {
            get
            {
                return 100 - ProjectTotalHoursPercent;
            }
        }

        public double TotalHours
        {
            get
            {
                return DayTotalHours != null ? DayTotalHours.Sum(t => t.TotalHours) : (BillableHours + NonBillableHours);
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

