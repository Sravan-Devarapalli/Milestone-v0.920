using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{
    [DataContract]
    [Serializable]
    public class RevenueReport
    {
        [DataMember]
        public decimal ActualRevenuePerHour
        {
            get;
            set;
        }

        [DataMember]
        public decimal TargetRevenuePerHour
        {
            get;
            set;
        }

        [DataMember]
        public decimal HoursUtilization
        {
            get;
            set;
        }

        [DataMember]
        public decimal TotalRevenuePerAnnual
        {
            get;
            set;
        }

        public decimal RevenuePerFTE
        {
            get;
            set;
        }
        public decimal FTE
        {
            get;
            set;
        }
        public int ManagedServiceResources
        {
            get;
            set;
        }
    }
}

