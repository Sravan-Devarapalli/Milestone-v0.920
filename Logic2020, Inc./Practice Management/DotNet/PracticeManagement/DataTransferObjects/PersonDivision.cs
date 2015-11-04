using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class PersonDivision
    {
        [DataMember]
        public int DivisionId { get; set; }

        [DataMember]
        public string DivisionName { get; set; }

        [DataMember]
        public bool Inactive { get; set; }
    }
}

