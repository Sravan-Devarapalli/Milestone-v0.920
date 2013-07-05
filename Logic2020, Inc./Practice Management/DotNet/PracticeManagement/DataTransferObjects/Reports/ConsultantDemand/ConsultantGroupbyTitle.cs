using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web;

namespace DataTransferObjects.Reports.ConsultingDemand
{
    [DataContract]
    [Serializable]
    public class ConsultantGroupbyTitle
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
        public List<ConsultantDemandDetailsByMonthByTitle> ConsultantDetails { get; set; }
    }
}
