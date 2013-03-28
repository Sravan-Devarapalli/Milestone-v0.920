using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports.ConsultingDemand
{
    [DataContract]
    [Serializable]
    public class ConsultantGroupbyTitleSkill
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Skill { get; set; }

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
                else if (MonthCount != null)
                {
                    return MonthCount.Values.Sum();
                }
                return 0;
            }
        }

        public string TitleSkill
        {
            get
            {
                return Title + "," + Skill;
            }
        }

    }
}

