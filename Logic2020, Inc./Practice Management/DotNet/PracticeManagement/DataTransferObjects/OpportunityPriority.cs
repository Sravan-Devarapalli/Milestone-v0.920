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
    public class OpportunityPriority
    {
        [DataMember]
        public int Id
        {
            get;
            set;
        }

        [DataMember]
        public string Priority
        {
            get;
            set;
        }

        [DataMember]
        public bool InUse { get; set; }

        [DataMember]
        public string Description
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

        /// <summary>
        /// Sales Stage
        /// </summary>
        [DataMember]
        public string DisplayName
        {
            get;
            set;
        }

        public string HtmlEncodedDisplayName
        {
            get
            {
                return HttpUtility.HtmlEncode(DisplayName);
            }
        }
    }
}

