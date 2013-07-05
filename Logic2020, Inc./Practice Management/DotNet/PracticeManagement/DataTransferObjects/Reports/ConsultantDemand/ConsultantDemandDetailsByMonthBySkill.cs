using System;
using System.Runtime.Serialization;
using System.Web;

namespace DataTransferObjects.Reports.ConsultingDemand
{
    [DataContract]
    [Serializable]
    public class ConsultantDemandDetailsByMonthBySkill
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
        public string OpportunityNumber { get; set; }

        [DataMember]
        public string ProjectNumber { get; set; }

        [DataMember]
        public string ProjectDescription { get; set; }

        public string HtmlEncodedProjectDescription
        {
            get
            {
                return HttpUtility.HtmlEncode(ProjectDescription);
            }
        }

        [DataMember]
        public int? OpportunityId { get; set; }

        [DataMember]
        public int? ProjectId { get; set; }

        [DataMember]
        public string ProjectName { get; set; }

        public string HtmlEncodedProjectName
        {
            get
            {
                return HttpUtility.HtmlEncode(ProjectName);
            }
        }

        [DataMember]
        public string SalesStage { get; set; }

        [DataMember]
        public int AccountId { get; set; }

        [DataMember]
        public string AccountName { get; set; }

        public string HtmlEncodedAccountName
        {
            get
            {
                return HttpUtility.HtmlEncode(AccountName);
            }
        }

        [DataMember]
        public DateTime ResourceStartDate { get; set; }

        [DataMember]
        public int Count { get; set; }
    }
}
