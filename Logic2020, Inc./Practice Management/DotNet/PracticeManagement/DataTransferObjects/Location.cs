using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class Location
    {
        [DataMember]
        public int LocationId
        {
            get;
            set;
        }

        [DataMember]
        public string LocationCode
        {
            get;
            set;
        }

        [DataMember]
        public string LocationName
        {
            get;
            set;
        }

        [DataMember]
        public Location Parent
        {
            get;
            set;
        }

        [DataMember]
        public string Country
        {
            get;
            set;
        }

        [DataMember]
        public string TimeZone
        {
            get;
            set;
        }

        public string FormattedLocation
        {
            get
            {
                return LocationCode + ": " + LocationName;
            }
        }
    }
}

