using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Skills
{
    [DataContract]
    [Serializable]
    public class PersonEmployer
    {
        

        [DataMember]
        public Person Person
        {
            get;
            set;
        }
        [DataMember]
        public Employer Employer
        {
            get;
            set;
        }

        [DataMember]
        public DateTime StartDate
        {
            set;
            get;
        }

        [DataMember]
        public DateTime EndDate
        {
            set;
            get;
        }

        [DataMember]
        public string Title
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

    }
}

