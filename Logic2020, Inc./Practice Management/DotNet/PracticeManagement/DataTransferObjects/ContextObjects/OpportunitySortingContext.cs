using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.ContextObjects
{
    [DataContract]
    [Serializable]
    public class OpportunitySortingContext
    {
        [DataMember]
        public string OrderBy { get; set; }

        [DataMember]
        public string SortDirection { get; set; }
    }
}

