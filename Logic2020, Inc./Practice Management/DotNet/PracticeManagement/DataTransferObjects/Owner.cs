using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class Owner
    {
        [DataMember]
        public string Target
        {
            get;
            set;
        }

        [DataMember]
        public bool IsDivisionOwner
        {
            get;
            set;
        }

        [DataMember]
        public bool isAssignedToProjects
        {
            get;
            set;
        }
    }
}

