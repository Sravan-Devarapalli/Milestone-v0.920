using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [Serializable]
    [DataContract]
    public class SeniorityCategory
    {
        [DataMember]
        public int Id
        {
            get;
            set;
        }

        [DataMember]
        public string Name
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
