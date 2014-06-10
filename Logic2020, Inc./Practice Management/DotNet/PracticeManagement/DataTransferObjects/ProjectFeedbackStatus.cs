using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Web;

namespace DataTransferObjects
{
    [Serializable]
    [DataContract]
    public class ProjectFeedbackStatus
    {
        [DataMember]
        public int? Id
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

        public string HtmlEncodedName
        {
            get
            {
                return HttpUtility.HtmlEncode(Name);
            }
        }
    }
}

