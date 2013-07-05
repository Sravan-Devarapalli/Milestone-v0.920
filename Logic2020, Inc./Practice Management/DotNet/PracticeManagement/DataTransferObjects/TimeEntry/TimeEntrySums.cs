using System;
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
