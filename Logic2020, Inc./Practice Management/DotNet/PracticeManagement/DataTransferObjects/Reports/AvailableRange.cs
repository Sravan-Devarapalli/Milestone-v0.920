using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{
    [DataContract]
    [Serializable]
    public class AvailableRange
    {
        [DataMember]
        public DateTime StartDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime EndDate
        {
            get;
            set;
        }

        [DataMember]
        public bool IsAvailable
        {
            get;
            set;
        }

        [DataMember]
        public bool IsManagedService
        {
            get;
            set;
        }

        [DataMember]
        public bool IsPersonActive
        {
            get;
            set;
        }

        public int AvailableResourcesCount
        {
            get;
            set;
        }

        public int MSCount
        {
            get;
            set;
        }

        public int AvaliableResourcesWithOutMS
        {
            get
            {
                return AvailableResourcesCount - MSCount;
            }
        }

        public int AvailableCount
        {
            get
            {
                return IsAvailable ? 1 : 0;
            }
        }

        public string Available
        {
            get
            {
                return IsAvailable ? "1" : "-";
            }
        }

        public string RequiredResources { get; set; }

        public decimal RequiredResourcesCount { get;set; }
    }
}

