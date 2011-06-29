using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [Serializable]
    [DataContract]
    public class ColorInformation
    {
        [DataMember]
        public string ColorValue
        {
            get;
            set;
        }

        [DataMember]
        public int ColorId
        {
            get;
            set;
        }

        [DataMember]
        public string ColorDescription
        {
            get;
            set;
        }
    }
}

