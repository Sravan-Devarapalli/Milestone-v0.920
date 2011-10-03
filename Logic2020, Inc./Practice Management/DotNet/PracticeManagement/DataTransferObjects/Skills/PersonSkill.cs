using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Skills
{
    [DataContract]
    [Serializable]
    public class PersonSkill
    {
        [DataMember]
        public int? ClientId
        {
            get;
            set;
        }

        [DataMember]
        public Person Person
        {
            get;
            set;
        }

        [DataMember]
        public Skill Skill
        {
            get;
            set;
        }

        [DataMember]
        public SkillLevel SkillLevel
        {
            get;
            set;
        }

        [DataMember]
        public int? YearsExperience
        {
            get;
            set;
        }

        [DataMember]
        public DateTime LastUsed
        {
            get;
            set;
        }

        [DataMember]
        public int? DisplayOrder
        {
            get;
            set;
        }

        [DataMember]
        public DateTime ModificationDate
        {
            get;
            set;
        }
    }
}

