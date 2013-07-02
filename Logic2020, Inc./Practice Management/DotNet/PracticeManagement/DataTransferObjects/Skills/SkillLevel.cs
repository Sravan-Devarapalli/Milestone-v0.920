using System;
using System.Runtime.Serialization;

namespace DataTransferObjects.Skills
{
    [DataContract]
    [Serializable]
    public class SkillLevel : LookupBase
    {
        [DataMember]
        public string Definition
        {
            set;
            get;
        }
    }
}
