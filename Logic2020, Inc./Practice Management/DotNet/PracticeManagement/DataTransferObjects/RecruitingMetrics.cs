using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Web;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class RecruitingMetrics
    {
        [DataMember]
        public int? RecruitingMetricsId
        {
            get;
            set;
        }

        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
        public RecruitingMetricsType RecruitingMetricsType
        {
            get;
            set;
        }

        [DataMember]
        public int SortOrder
        {
            get;
            set;
        }

        [DataMember]
        public bool InUse
        {
            get;
            set;
        }

        public string HtmlEncodedName
        {
            get
            {
                return HttpUtility.HtmlEncode(Name);
            }
        }
    }
}

