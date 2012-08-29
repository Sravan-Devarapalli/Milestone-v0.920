using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    /// <summary>
    /// Data Transfer Object for a Person entity
    /// </summary>
    [DataContract]
    [Serializable]
    public class Employment
    {
        [DataMember]
        public int PersonId { get; set; }

        [DataMember]
        public DateTime HireDate { get; set; }

        [DataMember]
        public DateTime? TerminationDate { get; set; }

        [DataMember]
        public int? TerminationReasonId { get; set; }
    }
}

