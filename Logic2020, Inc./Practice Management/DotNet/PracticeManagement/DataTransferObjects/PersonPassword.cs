using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class PersonPassword
    {
        [DataMember]
        public string PersonId
        {
            get;
            set;
        }

        [DataMember]
        public string Password
        {
            get;
            set;
        }
    }
}

