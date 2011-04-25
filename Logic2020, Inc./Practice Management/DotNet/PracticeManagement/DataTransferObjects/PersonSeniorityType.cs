using System.Runtime.Serialization;

namespace DataTransferObjects
{
    /// <summary>
    /// Determines the Seniority of person
    /// </summary>
    [DataContract]
    public enum PersonSeniorityType
    {
        [EnumMember]
        Admin = 1,
        [EnumMember]
        Partner = 15,
        [EnumMember]
        VP = 25,
        [EnumMember]
        Director = 35,
        [EnumMember]
        PracticeManager = 45,
        [EnumMember]
        SeniorManager = 55,
        [EnumMember]
        Manager = 65,
        [EnumMember]
        SeniorConsultant = 75,
        [EnumMember]
        Consultant = 85,
        [EnumMember]
        Analyst = 95,
        [EnumMember]
        Intern = 105
    }
}
