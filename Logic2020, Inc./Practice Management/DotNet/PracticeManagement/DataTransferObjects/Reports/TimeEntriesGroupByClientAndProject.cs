﻿using System;
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
        public double BillableValue { get; set; }

        [DataMember]
        public double BillableHours { get; set; }

        [DataMember]
        public double NonBillableHours { get; set; }


        [DataMember]
        public List<TimeEntriesGroupByDate> DayTotalHours
        {
            get;
            set;
        }


        public int BillablePercent
        {
            get
            {
                return (int)(100 * BillableHours / (BillableHours + NonBillableHours));
            }
        }

        public int NonBillablePercent
        {
            get
            {
                return (100 - BillablePercent);
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
                dt = DayTotalHours.First(dth => dth.Date == dt.Date);
                 dt.DayTotalHoursList.Add(dt.DayTotalHoursList[0]);
            }
            else
            {
                DayTotalHours.Add(dt);
            }

        }
    }
}

