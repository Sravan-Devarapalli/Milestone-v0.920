using System.ComponentModel;
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
        ProjectLead = 8,

        [EnumMember]
        Operations = 9
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
        Recruiting = 4,

        [EnumMember]
        [Description("Technology Consulting")]
        TechnologyConsulting = 5,

        [EnumMember]
        [Description("Business Consulting")]
        BusinessConsulting = 6,

        [EnumMember]
        [Description("Director")]
        Director = 7,

        [EnumMember]
        [Description("Adicio")]
        Adicio = 8
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
        ProjectEstimate = 5,

        [EnumMember]
        [Description("Purchase Order")]
        PurchaseOrder = 6
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

    [DataContract]
    public enum AttributionTypes
    {
        [EnumMember]
        [Description("Undefined")]
        Undefined = 0,

        [EnumMember]
        [Description("Sales")]
        Sales = 1,

        [EnumMember]
        [Description("Delivery")]
        Delivery = 2
    }

    [DataContract]
    public enum AttributionRecordTypes
    {
        [EnumMember]
        [Description("Undefined")]
        Undefined = 0,

        [EnumMember]
        [Description("Person")]
        Person = 1,

        [EnumMember]
        [Description("Practice")]
        Practice = 2
    }

    [DataContract]
    public enum RecruitingMetricsType
    {
        [EnumMember]
        [Description("Undefined")]
        Undefined = 0,

        [EnumMember]
        [Description("Source")]
        Source = 1,

        [EnumMember]
        [Description("Targeted Company")]
        TargetedCompany = 2
    }

    [DataContract]
    public enum JobSeekersStatus
    {
        [EnumMember]
        [Description("Undefined")]
        Undefined = 0,

        [EnumMember]
        [Description("Active Candidate")]
        ActiveCandidate = 1,

        [EnumMember]
        [Description("Passive Candidate")]
        PassiveCandidate = 2
    }

    [DataContract]
    public enum LockoutPages
    {
        [EnumMember]
        [Description("Undefined")]
        Undefined = 0,

        [EnumMember]
        [Description("Time Entry")]
        TimeEntry = 1,

        [EnumMember]
        [Description("Calendar")]
        Calendar = 2,

        [EnumMember]
        [Description("Person detail")]
        Persondetail = 3,

        [EnumMember]
        [Description("Project detail")]
        Projectdetail = 4
    }

    [DataContract]
    public enum ReportName
    {
        [EnumMember]
        PesonsByProject,

        [EnumMember]
        ProjectsList,

        [EnumMember]
        MSManagementMeetingReport,

        [EnumMember]
        BadgeResourceByTime,

        [EnumMember]
        ResourceByPracticeReport,

        [EnumMember]
        ResourceByBusinessConsultantReport,

        [EnumMember]
        AllEmployees18MoClockReport,

        [EnumMember]
        BadgedOnProjectReport,

        [EnumMember]
        ClockNotStartedReport,

        [EnumMember]
        BadgedNotOnProjectReport,

        [EnumMember]
        BadgeBreakReport,

        [EnumMember]
        BadgeBlockedReport,

        [EnumMember]
        BadgedOnProjectBasedExceptionReport,

        [EnumMember]
        BadgedNotOnPersonBasedExceptionReport,

        [EnumMember]
        ResourcesByTechnicalConsultants,

        [EnumMember]
        BenchCostReport,

        [EnumMember]
        AuditReport,

        [EnumMember]
        CSATReport,

        [EnumMember]
        ProjectFeedbackReport,

        [EnumMember]
        UtilizationReport,

        [EnumMember]
        NewHireReport,

        [EnumMember]
        TerminationReport,

        [EnumMember]
        BillingReport,

        [EnumMember]
        AttainmentReport,

        [EnumMember]
        CommissionsAndRatesReport,

        [EnumMember]
        ByTimePeriodReport,

        [EnumMember]
        ByProjectReport,

        [EnumMember]
        ByPersonReport,

        [EnumMember]
        ActivityLogReport,

        [EnumMember]
        ConsultingDemandReport,

        [EnumMember]
        PersonsReport,

        [EnumMember]
        ConsultingUtilizationReport,

        [EnumMember]
        ConsultingCapacityReport,

        [EnumMember]
        ClientReport,

        [EnumMember]
        AccountSummaryReport,

        [EnumMember]
        PersonSummaryReport,

        [EnumMember]
        ProjectSummaryReport,

        [EnumMember]
        VendorSummary,

        [EnumMember]
        PTOReport,

        [EnumMember]
        ExpenseReport
    }

    [DataContract]
    public enum ChannelType
    {
        [EnumMember]
        [Description("Advisory Board")]
        AdvisoryBoard = 1,
        [EnumMember]
        [Description("Salesperson")]
        Salesperson = 2,
        [EnumMember]
        [Description("Employee")]
        Employee = 3,
        [EnumMember]
        [Description("Inbound Marketing")]
        InboundMarketing = 4,
        [EnumMember]
        [Description("Outbound Marketing")]
        OutboundMarketing = 5,
        [EnumMember]
        [Description("Partner")]
        Partner = 6,
        [EnumMember]
        [Description("Vendor Management System")]
        VendorManagementSystem = 7,
        [EnumMember]
        [Description("Other Referral")]
        OtherReferral = 8
    }

    [DataContract]
    public enum ProjectDivisionType
    {


        [EnumMember]
        [Description("Adicio")]
        BusinessDevelopment = 1,

        [EnumMember]
        [Description("Business Consulting")]
        Consulting = 2,

        [EnumMember]
        [Description("Business Development")]
        Operations = 3,

        [EnumMember]
        [Description("Operations")]
        Recruiting = 4,

        [EnumMember]
        [Description("Recruiting")]
        TechnologyConsulting = 5,

        [EnumMember]
        [Description("Technology Consulting")]
        BusinessConsulting = 6,

    }

    [DataContract]
    public enum OutsourceId
    {
        [EnumMember]
        NotApplicable = 3,

        [EnumMember]
        [Description("Yes")]
        Yes = 1,

        [EnumMember]
        [Description("No")]
        No = 2
    }

}

