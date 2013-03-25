using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports.ConsultingDemand
{
    [DataContract]
    [Serializable]
    public class ConsultantGroupbyTitle
    {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public List<ConsultantDemandDetailsByMonthByTitle> ConsultantDetails { get; set; }
    }
}

