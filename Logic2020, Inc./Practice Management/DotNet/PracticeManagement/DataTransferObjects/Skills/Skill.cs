using System;
using System.Runtime.Serialization;

namespace DataTransferObjects.Skills
{
    [DataContract]
    [Serializable]
    public class Skill : LookupBase
    {
        [DataMember]
        public SkillCategory Category { get; set; }
    }
}
