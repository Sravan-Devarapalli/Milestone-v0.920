using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class UserCredentials
    {
        [DataMember]
        public string UserName
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
        [DataMember]
        public string PasswordSalt
        {
            get;
            set;
        }
    }
}

