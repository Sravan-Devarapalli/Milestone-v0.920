using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports.ConsultingDemand
{
    [DataContract]
    [Serializable]
    public class ConsultantGroupBySalesStage
    {
        [DataMember]
        public String  SalesStage { get; set; }

        [DataMember]
        public List<ConsultantDemandDetailsByMonth> ConsultantDetailsBySalesStage { get; set; }

        public int TotalCount
        {
            get
            {
                if (ConsultantDetailsBySalesStage != null)
                {
                    return ConsultantDetailsBySalesStage.Sum(p => p.Count);
                }

                return 0;
            }
        }
    }
}

