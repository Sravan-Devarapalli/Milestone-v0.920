using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports.ConsultingDemand
{
    [DataContract]
    [Serializable]
    public class ConsultantGroupbySkill
    {
        [DataMember]
        public string Skill { get; set; }
        [DataMember]
        public List<ConsultantDemandDetailsByMonthBySkill> ConsultantDetails { get; set; }
    }
}

