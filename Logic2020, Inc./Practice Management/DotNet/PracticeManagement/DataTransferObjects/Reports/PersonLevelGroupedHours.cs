using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{
   
    [DataContract]
    [Serializable]
    public class PersonLevelGroupedHours
    {
        [DataMember]
        public Person Person
        {
            get;
            set;
        }

        [DataMember]
        public double BillabileHours
        {
            get;
            set;

        }

        [DataMember]
        public double BillableValue
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

        public double TotalHours
        {
            get
            {
                return BillabileHours + NonBillableHours;
            }
        }

    }
}

