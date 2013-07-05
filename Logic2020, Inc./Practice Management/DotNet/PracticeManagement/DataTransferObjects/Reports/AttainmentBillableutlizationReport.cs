using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{
    [DataContract]
    [Serializable]
    public class AttainmentBillableutlizationReport
    {
        [DataMember]
        public Person Person { get; set; }

        [DataMember]
        public List<BillableUtlizationByRange> BillableUtilizationList { get; set; }
    }
}
