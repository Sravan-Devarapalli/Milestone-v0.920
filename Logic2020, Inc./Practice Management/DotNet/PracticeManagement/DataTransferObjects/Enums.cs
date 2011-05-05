using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    public enum BudgetCategoryType
    {
        [EnumMember]
        ClientDirector = 1,
        [EnumMember]
        PracticeArea = 2,
        [EnumMember]
        BusinessDevelopmentManager = 3
    }

    [DataContract]
    public enum SettingsType
    {
        [EnumMember]
        Reports = 1,
        [EnumMember]
        SMTP = 2,
        [EnumMember]
        Project = 3,
    }
}

