using System;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{
    [DataContract]
    [Serializable]
    public class BillableUtlizationByRange
    {
        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public double BillableUtilization { get; set; }

        [DataMember]
        public double BillableHours { get; set; }

        [DataMember]
        public int AvailableHours { get; set; }

        [DataMember]
        public string RangeType
        {
            get;
            set;
        }
    }
}
