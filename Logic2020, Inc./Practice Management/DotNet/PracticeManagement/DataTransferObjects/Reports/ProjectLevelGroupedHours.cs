using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{
    /// <summary>
    /// Represents TimeEntries grouped (i.e. day/week/month/year) based on the particular Project
    /// </summary>
    [DataContract]
    [Serializable]
    public class ProjectLevelGroupedHours
    {
        [DataMember]
        public Project Project
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
        public double BillableValue
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

        [DataMember]
        public bool IsFixedProject { get; set; }

        [DataMember]
        public double BillableHoursUntilToday { get; set; }


        [DataMember]
        public double ForecastedHoursUntilToday
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

    }
}

