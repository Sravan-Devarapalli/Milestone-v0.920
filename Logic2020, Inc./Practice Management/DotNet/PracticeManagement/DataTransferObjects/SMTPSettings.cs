using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class SMTPSettings
    {
        [DataMember]
        public string MailServer { get; set; }

        [DataMember]
        public int PortNumber { get; set; }

        [DataMember]
        public bool SSLEnabled { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string PMSupportEmail { get; set; }
    }
}

