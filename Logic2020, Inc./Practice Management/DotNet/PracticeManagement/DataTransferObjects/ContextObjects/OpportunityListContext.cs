using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.ContextObjects
{
    [DataContract]
    [Serializable]
    public class OpportunityListContext
    {
        public OpportunityListContext()
        {
            ActiveClientsOnly = true;
        }

        [DataMember]
        public bool ActiveClientsOnly { get; set; }

        [DataMember]
        public string SearchText { get; set; }

        [DataMember]
        public int? ClientId { get; set; }

        [DataMember]
        public int? SalespersonId { get; set; }

        [DataMember]
        public int? TargetPersonId { get; set; }

        [DataMember]
        public int? CurrentId { get; set; }
    }
}

