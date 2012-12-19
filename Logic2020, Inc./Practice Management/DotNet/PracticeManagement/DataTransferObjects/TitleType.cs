using System;
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


        public int StartPosition
        {
            get;
            set;
        }

        public int EndPosition
        {
            get;
            set;
        }
    }
}

