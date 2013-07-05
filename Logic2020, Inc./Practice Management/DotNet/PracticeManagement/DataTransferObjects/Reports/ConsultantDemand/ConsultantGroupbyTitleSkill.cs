using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DataTransferObjects.Reports.ConsultingDemand
{
    [DataContract]
    [Serializable]
    public class ConsultantGroupbyTitleSkill
    {
        [DataMember]
        public string Title { get; set; }

        public string HtmlEncodedTitle
        {
            get
            {
                return HttpUtility.HtmlEncode(Title);
            }
        }

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
        public Dictionary<string, int> MonthCount { get; set; }

        [DataMember]
        public List<ConsultantDemandDetails> ConsultantDetails { get; set; }

        public int TotalCount
        {
            get
            {
                if (ConsultantDetails != null)
                {
                    return ConsultantDetails.Sum(p => p.Count);
                }
                return MonthCount != null ? MonthCount.Values.Sum() : 0;
            }
        }

        public string TitleSkill
        {
            get
            {
                return HtmlEncodedTitle + ", " + HtmlEncodedSkill;
            }
        }
    }
}
