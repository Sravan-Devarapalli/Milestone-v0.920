using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [Serializable]
    [DataContract]
    public class QuickLinks
    {
        [DataMember]
        public int Id
        {
            get;
            set;
        }

        [DataMember]
        public string LinkName
        {
            get;
            set;
        }

        [DataMember]
        public string VirtualPath
        {
            get;
            set;
        }

        [DataMember]
        public DashBoardType DashBoardType
        {
            get;
            set;
        }
    }
}

