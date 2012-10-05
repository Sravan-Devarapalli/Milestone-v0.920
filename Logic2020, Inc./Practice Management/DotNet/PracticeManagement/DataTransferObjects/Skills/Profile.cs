using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Skills
{
    [DataContract]
    [Serializable]
    public class Profile
    {
        [DataMember]
        public int? ProfileId
        {
            get;
            set;
        }

        [DataMember]
        public string ProfileName
        {
            get;
            set;
        }

        [DataMember]
        public string ProfileUrl
        {
            get;
            set;
        }

        [DataMember]
        public int ModifiedBy
        {
            get;
            set;
        }

        [DataMember]
        public string ModifiedByName
        {
            get;
            set;
        }

        [DataMember]
        public DateTime ModifiedDate
        {
            get;
            set;
        }

        [DataMember]
        public bool IsDefault
        {
            get;
            set;
        }
    }
}

