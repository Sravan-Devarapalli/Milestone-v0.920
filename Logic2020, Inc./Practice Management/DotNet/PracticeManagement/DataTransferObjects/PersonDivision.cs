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
    public class PersonDivision
    {
        [DataMember]
        public int DivisionId
        {
            get;
            set;
        }

        [DataMember]
        public string DivisionName
        {
            get;
            set;
        }

        [DataMember]
        public bool Inactive
        {
            get;
            set;
        }

        [DataMember]
        public bool ShowCareerManagerLink
        {
            get;
            set;
        }

        [DataMember]
        public Person DivisionOwner
        {
            get;
            set;
        }

        public int DivisionOwnerId
        {
            get { return DivisionOwner != null && DivisionOwner.Id.HasValue ? DivisionOwner.Id.Value : -1; }
        
        }

        public string HtmlEncodedName
        {
            get
            {
                return HttpUtility.HtmlEncode(DivisionName);
            }
        }
    }
}

