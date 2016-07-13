using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [Serializable]
    [DataContract]
    public class Vendor
    {
        [DataMember]
        public int? Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ContactName { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string TelephoneNumber { get; set; }

        [DataMember]
        public bool Status { get; set; }

        [DataMember]
        public VendorType VendorType { get; set; }

        [DataMember]
        public List<Project> Projects { get; set; }

        [DataMember]
        public List<VendorAttachment> Attachments { get; set; }

        public string Domain
        {
            get
            {
                return !string.IsNullOrEmpty(Email) ? Email.Split('@')[1].ToLower() : string.Empty;
            }
        }

        public string EmailWithoutDomain
        {
            get
            {
                return !string.IsNullOrEmpty(Email) ? Email.Split('@')[0] : string.Empty;
            }
        }
    }
}

