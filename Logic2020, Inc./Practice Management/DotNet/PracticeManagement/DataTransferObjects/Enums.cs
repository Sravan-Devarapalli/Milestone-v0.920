using System.Runtime.Serialization;
using System.ComponentModel;

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
        StrikedPerson = 2
    }

    [DataContract]
    public enum OpportunityPersonRelationType
    {
        [EnumMember]
        ProposedResource = 1,
        [EnumMember]
        TeamStructure = 2
    }

    [DataContract]
    public enum DefaultGoalType
    {
        [EnumMember]
        Client = 1,
        [EnumMember]
        Person = 2,
    }

    [DataContract]
    public enum BenchReportSortExpression
    {
        [EnumMember]
        ConsultantName = 1,
        [EnumMember]
        Practice = 2,
        [EnumMember]
        Status = 3
    }

    [DataContract]
    public enum DashBoardType
    {
        [EnumMember]
        Consulant = 1,
        [EnumMember]
        Manager = 2,
        [EnumMember]
        BusinessDevelopment = 3,
        [EnumMember]
        ClientDirector = 4,
        [EnumMember]
        SeniorLeadership = 5,
        [EnumMember]
        Recruiter = 6,
        [EnumMember]
        Admin = 7,
        [EnumMember]
        ProjectLead = 8

    }

    [DataContract]
    public enum TimeEntrySectionType
    {
        [EnumMember]
        [Description("Undefined")]
        Undefined = 0,
        [EnumMember]
        [Description("Project")] 
        Project = 1,
        [EnumMember]
        [Description("Business Development")] 
        BusinessDevelopment = 2,
        [EnumMember]
        [Description("Internal")] 
        Internal = 3,
        [EnumMember]
        [Description("Administrative")] 
        Administrative = 4

    }

    [DataContract]
    public enum PersonDivisionType
    {
        [EnumMember]
        [Description("- - Select Division - -")]
        Undefined = 0,
        [EnumMember]
        [Description("Business Development")]
        BusinessDevelopment = 1,
        [EnumMember]
        [Description("Consulting")]
        Consulting = 2,
        [EnumMember]
        [Description("Operations")]
        Operations = 3,
        [EnumMember]
        [Description("Recruiting")]
        Recruiting = 4
    }

    [DataContract]
    public enum ProjectAttachmentCategory
    {
        [EnumMember]
        [Description("- - Select Category - -")]
        Undefined = 0,
        [EnumMember]
        [Description("SOW")]
        SOW = 1,
        [EnumMember]
        [Description("MSA")]
        MSA = 2,
        [EnumMember]
        [Description("Change Request")]
        ChangeRequest = 3,
        [EnumMember]
        [Description("Proposal")]
        Proposal = 4,
        [EnumMember]
        [Description("Project Estimate")]
        ProjectEstimate = 5 
    }

    [DataContract]
    public enum BusinessType
    {
        [EnumMember]
        [Description("-- Select Business Type --")]
        Undefined = 0,

        [EnumMember]
        [Description("New Business")]
        NewBusiness = 1,

        [EnumMember]
        [Description("Extension")]
        Extension = 2
    }
}

