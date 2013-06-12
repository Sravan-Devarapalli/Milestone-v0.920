using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Financials
{

    [DataContract]
    [Serializable]
    public class RangeType
    {
        [DataMember]
        public DateTime StartDate { get; set; }


        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public string Range
        {
            get;
            set;
        }
    }
}

