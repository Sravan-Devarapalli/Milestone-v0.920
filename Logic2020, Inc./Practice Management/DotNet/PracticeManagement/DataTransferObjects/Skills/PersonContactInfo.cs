using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Skills
{
    [DataContract]
    [Serializable]
    public class PersonContactInfo
    {
        [DataMember]
        public Person Person
        {
            set;
            get;
        }

        [DataMember]
        public string Info
        {
            set;
            get;
        }

        [DataMember]
        public int? DisplayOrder
        {
            set;
            get;
        }

        [DataMember]
        public DateTime ModificationDate
        {
            set;
            get;
        }

        [DataMember]
        public ContactInfoType ContactInfoType
        {
            set;
            get;
        }
        
    }
}

