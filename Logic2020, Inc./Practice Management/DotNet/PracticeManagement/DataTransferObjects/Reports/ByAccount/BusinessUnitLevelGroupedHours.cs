using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports.ByAccount
{

    [DataContract]
    [Serializable]
    public class BusinessUnitLevelGroupedHours
    {
        [DataMember]
        public ProjectGroup BusinessUnit
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

        [DataMember]
        public double BusinessDevelopmentHours
        {
            get;
            set;
        }



        [DataMember]
        public double ForecastedHours
        {
            get;
            set;
        }

        [DataMember]
        public int BusinessUnitTotalHoursPercent
        {
            get;
            set;
        }

        [DataMember]
        public double ProjectsCount { get; set; }

        public double TotalHours
        {
            get
            {
                return BillableHours + NonBillableHours + BusinessDevelopmentHours;
            }
        }


    }
}

