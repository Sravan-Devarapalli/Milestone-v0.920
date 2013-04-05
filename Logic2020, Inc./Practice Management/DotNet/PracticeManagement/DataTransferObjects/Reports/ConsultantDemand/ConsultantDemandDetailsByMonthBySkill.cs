using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports.ConsultingDemand
{
    [DataContract]
    [Serializable]
    public class ConsultantDemandDetailsByMonthBySkill
    {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string OpportunityNumber { get; set; }
        [DataMember]
        public string ProjectNumber { get; set; }
        [DataMember]
        public string ProjectDescription { get; set; }
        [DataMember]
        public int? OpportunityId { get; set; }
        [DataMember]
        public int? ProjectId { get; set; }
        [DataMember]
        public string ProjectName { get; set; }
        [DataMember]
        public int AccountId { get; set; }
        [DataMember]
        public string AccountName { get; set; }
        [DataMember]
        public DateTime ResourceStartDate { get; set; }
        [DataMember]
        public int Count { get; set; }
    }
}

