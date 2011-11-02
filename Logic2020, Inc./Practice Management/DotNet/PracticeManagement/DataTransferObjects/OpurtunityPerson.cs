using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class OpportunityPerson
    {
        [DataMember]
        public Opportunity opportunity
        {
            get;
            set;
        }

        [DataMember]
        public Person Person
        {
            get;
            set;
        }

        [DataMember]
        public int RelationType
        {
            get;
            set;
        }

        [DataMember]
        public int PersonType
        {
            get;
            set;
        }

        [DataMember]
        public int Quantity
        {
            get;
            set;
        }
    }
}

