using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web;

namespace DataTransferObjects.Reports.ConsultingDemand
{
    [DataContract]
    [Serializable]
    public class ConsultantGroupbySkill
    {
        [DataMember]
        public string Skill { get; set; }

        public string HtmlEncodedSkill
        {
            get
            {
                return HttpUtility.HtmlEncode(Skill);
            }
        }

        [DataMember]
        public List<ConsultantDemandDetailsByMonthBySkill> ConsultantDetails { get; set; }
    }
}
