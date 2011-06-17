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
        [EnumMember]
        Application = 4
    }

    [DataContract]
    public enum ProjectCalculateRangeType
    {
        [EnumMember]
        ProjectValueInRange = 1,
        [EnumMember]
        TotalProjectValue = 2,
        [EnumMember]
        CurrentFiscalYear = 3
    }

    [DataContract]
    public enum OpportunityPersonType
    {
        [EnumMember]
        NormalPerson = 1,
        [EnumMember]
        StrikedPerson = 2,
    }
}

