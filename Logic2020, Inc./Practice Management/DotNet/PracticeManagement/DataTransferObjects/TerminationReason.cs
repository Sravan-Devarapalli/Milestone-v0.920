using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    /// <summary>
    /// Data Transfer Object for a Termination Reason
    /// </summary>
    [DataContract]
    [Serializable]
    public class TerminationReason
    {
        [DataMember]
        public int? Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsW2SalaryRule { get; set; }

        [DataMember]
        public bool IsW2HourlyRule { get; set; }

        [DataMember]
        public bool Is1099Rule { get; set; }

        [DataMember]
        public bool IsContigent { get; set; }

        [DataMember]
        public bool IsVisible { get; set; }
    }
}

