using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class PersonBadgeHistories
    {
        [DataMember]
        public Person Person
        {
            get;
            set;
        }

        [DataMember]
        public List<MSBadge> BlockHistory
        {
            get;
            set;
        }

        [DataMember]
        public List<MSBadge> OverrideHistory
        {
            get;
            set;
        }

        [DataMember]
        public List<MSBadge> BadgeHistory
        {
            get;
            set;
        }
    }
}

