using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Determines event sources for the user activity log.
	/// </summary>
	[DataContract]
	public enum ActivityEventSource
	{
        //Note:- If value changes then change in ActivityLogControl.ascx also.
		[EnumMember]
        All = 1,
        [EnumMember]
        Error = 2,
		[EnumMember]
        Person = 3,
        [EnumMember]
        AddedPersons = 4,
        [EnumMember]
        ChangedPersons = 5,
        [EnumMember]
        TargetPerson = 6,
        [EnumMember]
        Project = 7,
        [EnumMember]
        AddedProjects = 8,
        [EnumMember]
        ChangedProjects = 9,
        [EnumMember]
        DeletedProjects = 10,
        [EnumMember]
        AddedSOW = 11,
        [EnumMember]
        DeletedSOW = 12,
        [EnumMember]
        Milestone = 13,
        [EnumMember]
        AddedMilestones = 14,
        [EnumMember]
        ChangedMilestones = 15,
        [EnumMember]
        DeletedMilestones = 16,
        [EnumMember]        
        Opportunity = 17,
        [EnumMember]
        AddedOpportunities = 18,
        [EnumMember]
        ChangedOpportunities = 19,
        [EnumMember]
        DeletedOpportunities = 20,
        [EnumMember]
        Exports = 21,
        [EnumMember]
        ProjectSummaryExport = 22,
        [EnumMember]
        OpportunitySummaryExport = 23,
        [EnumMember]
        TimeEntryByProjectExport = 24,
        [EnumMember]
        TimeEntryByPersonExport = 25,
        [EnumMember]
        BenchReportExport = 26,
        [EnumMember]
        ConsultantUtilTableExport = 27,
        [EnumMember]
        TimeEntry = 28,
        [EnumMember]
        AddedTimeEntries = 29,
        [EnumMember]
        ChangedTimeEntries = 30,
        [EnumMember]
        DeletedTimeEntries = 31,
        [EnumMember]
        Logon = 32,
        [EnumMember]
        LoginSuccessful = 33,
        [EnumMember]
        LoginError = 34,
        [EnumMember]
        Security = 35,
        [EnumMember]
        AccountLockouts = 36,
        [EnumMember]
        PasswordResetRequests = 37,
        [EnumMember]
        BecomeUser = 38,
    }
}

