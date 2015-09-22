using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.CornerStone
{
    [DataContract]
    [Serializable]
    public class DivisionCF
    {
        [DataMember]
        public int DivisionId
        {
            get;
            set;
        }

        [DataMember]
        public string DivisionCode
        {
            get;
            set;
        }

        [DataMember]
        public string DivisionName
        {
            get;
            set;
        }

        [DataMember]
        public DivisionCF Parent
        {
            get;
            set;
        }
    }
}

