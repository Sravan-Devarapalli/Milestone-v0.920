using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [Serializable]
    [DataContract]
    public class ClientMarginColorInfo
    {
        [DataMember]
        public int Id
        {
            get;
            set;
        }

        [DataMember]
        public int EndRange
        {
            get;
            set;
        }
       
        
        [DataMember]
        public int StartRange
        {
            get;
            set;
        }


        [DataMember]
        public ColorInformation ColorInfo
        {
            get;
            set;
        }
       
        
    }
}

