using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{
    [DataContract]
    [Serializable]
    public class GroupbyTitle
    {
        [DataMember]
        public Title Title
        {
            get;
            set;
        }

        [DataMember]
        public List<BadgedResourcesByTime> ResourcesCount
        {
            get;
            set;
        }
    }
}

