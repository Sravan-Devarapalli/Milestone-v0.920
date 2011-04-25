using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.TimeEntry
{
    [DataContract]
    [Serializable]
    public class TimeEntrySums
    {
        [DataMember]
        public double TotalActualHours { get; set; }

        [DataMember]
        public double TotalForecastedHours { get; set; }
    }
}

