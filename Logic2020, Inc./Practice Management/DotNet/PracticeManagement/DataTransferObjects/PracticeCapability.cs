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
    public class PracticeCapability
    {
        [DataMember]
        public int CapabilityId { get; set; }

        [DataMember]
        public int PracticeId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool InUse { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        public string HtmlEncodedName
        {
            get
            {
                return HttpUtility.HtmlEncode(Name);
            }
        }

        [DataMember]
        public string PracticeAbbreviation { get; set; }

        public string MergedName
        {
            get
            {
                return PracticeAbbreviation + " - " + Name;
            }
        }

    }
}

