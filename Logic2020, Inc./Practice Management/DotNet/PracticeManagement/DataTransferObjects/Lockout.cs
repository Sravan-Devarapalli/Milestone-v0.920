using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Web;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class Lockout
    {
        private string _name;

        [DataMember]
        public int? Id
        {
            get;
            set;
        }

        [DataMember]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [DataMember]
        public LockoutPages LockoutPage
        {
            get;
            set;
        }

        [DataMember]
        public bool IsLockout
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? LockoutDate
        {
            get;
            set;
        }

        public string HtmlEncodedName
        {
            get
            {
                return HttpUtility.HtmlEncode(Name);
            }
        }
    }
}

