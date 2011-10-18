using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Skills
{
    [DataContract]
    [Serializable]
    public class PersonIndustry
    {
       

        [DataMember]
        public Person Person { get; set; }

        [DataMember]
        public Industry Industry { get; set; }

        [DataMember]
        public int YearsExperience { get; set; }

        [DataMember]
        public int DisplayOrder { get; set; }

        [DataMember]
        public DateTime? ModificationDate { get; set; }


    }
}

