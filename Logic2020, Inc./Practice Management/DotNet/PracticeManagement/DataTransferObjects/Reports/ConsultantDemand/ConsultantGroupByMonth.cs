using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports.ConsultingDemand
{
    public class ConsultantGroupByMonth
    {
        [DataMember]
        public DateTime MonthStartDate { get; set; }

        [DataMember]
        public List<ConsultantDemandDetailsByMonth> ConsultantDetailsByMonth { get; set; }

    }
}

