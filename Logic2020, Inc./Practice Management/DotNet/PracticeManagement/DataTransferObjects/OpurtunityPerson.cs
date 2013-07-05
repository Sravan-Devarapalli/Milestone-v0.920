using System;
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

        [DataMember]
        public DateTime? NeedBy
        {
            get;
            set;
        }
    }
}
