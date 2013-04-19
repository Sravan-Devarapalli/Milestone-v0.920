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
    public class PricingList
    {
        public static string DefaultPricingListName
        {
            get { return Constants.GroupNames.DefaultPricingListName; }
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int? PricingListId { get; set; }

        [DataMember]
        public int ClientId { get; set; }

        [DataMember]
        public bool InUse { get; set; }

        [DataMember]
        public bool IsDefault { get; set; }

        public string HtmlEncodedName
        {
            get
            {
                return HttpUtility.HtmlEncode(Name);
            }
        }
    }
}

