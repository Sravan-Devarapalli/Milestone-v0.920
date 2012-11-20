using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [Serializable]
    [DataContract]
    public class TitleType
    {
        [DataMember]
        public int TitleTypeId
        {
            get;
            set;
        }


        [DataMember]
        public string TitleTypeName
        {
            get;
            set;
        }
    }
}

