using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{
    [DataContract]
    [Serializable]
    public class UtilizationByProject
    {
        [DataMember]
        public Project Project 
        {
            get;
            set;
        }

        [DataMember]
        public decimal ProjectedHours
        {
            get;
            set;
        }

        [DataMember]
        public int Utilization
        {
            get;
            set;
        }
    }
}

