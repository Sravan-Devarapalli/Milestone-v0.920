using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{
    [DataContract]
    [Serializable]
    public class ResourceExceptionReport
    {
        [DataMember]
        public Person Person
        {
            get;
            set;
        }
        [DataMember]
        public Project Project
        {
            get;
            set;
        }
        [DataMember]
        public double ProjectedHours
        {
            get;
            set;
        }
        [DataMember]
        public double BillableHours
        {
            get;
            set;
        }
        [DataMember]
        public double NonBillableHours
        {
            get;
            set;
        }
    }
}

