using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

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
        public string Description
        {
            get;
            set;
        }
    }
}

