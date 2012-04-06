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
    public class WorkTypeLevelGroupedHours
    {
        [DataMember]
        public TimeTypeRecord WorkType
        {
            get;
            set;
        }

        [DataMember]
        public double BillableHours
        {
            get;
            set;
        }

        [DataMember]
        public double NonBillableHours
        {
            get;
            set;
        }

        
        public double TotalHours
        {
            get 
            {
                return BillableHours + NonBillableHours;
            }
        }

        public int WorkTypeTotalHoursPercent
        {
            get;
            set;
        }

    }
}

