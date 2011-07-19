using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    public class UserPasswordsHistory
    {
        [DataMember]
        public string HashedPassword { get; set; }

        [DataMember]
        public string PasswordSalt { get; set; }

    }
}

