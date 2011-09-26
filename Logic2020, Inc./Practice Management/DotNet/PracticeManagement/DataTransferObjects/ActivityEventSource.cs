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
        Opportunity = 7,
        [EnumMember]
        AddedOpportunities = 8,
        [EnumMember]
        ChangedOpportunities = 9,
        [EnumMember]
        DeletedOpportunities = 10,
		[EnumMember]
        Project = 11,
        [EnumMember]
        AddedProjects = 12,
        [EnumMember]
        ChangedProjects = 13,
        [EnumMember]
        DeletedProjects = 14,
        [EnumMember]
        Milestone = 15,
        [EnumMember]
        AddedMilestones = 16,
        [EnumMember]
        ChangedMilestones = 17,
        [EnumMember]
        DeletedMilestones = 18,
        //[EnumMember]
        //MilestoneResources = 19,
        //[EnumMember]
        //AddedMilestoneResources = 20,
        //[EnumMember]
        //ChangedMilestoneResources = 21,
        //[EnumMember]
        //DeletedMilestoneResources = 22,
		[EnumMember]
        ProjectAndMilestones = 23,
        [EnumMember]
        Logon = 24,
        [EnumMember]
        LoginSuccessful = 25,
        [EnumMember]
        LoginError = 26,
        [EnumMember]
        Security = 27,
        [EnumMember]
        PasswordResetRequests = 28,
        [EnumMember]
        AccountLockouts = 29,
        [EnumMember]
        BecomeUsers = 30,
        [EnumMember]
        TimeEntry = 31,
        [EnumMember]
        AddedTimeEntries = 32,
        [EnumMember]
        ChangedTimeEntries = 33,
        [EnumMember]
        DeletedTimeEntries = 34,
        [EnumMember]
        ProjectAttachment = 35,
        [EnumMember]
        AddedProjectAttachments = 36,
        [EnumMember]
        DeletedProjectAttachments = 37,
        [EnumMember]
        Notes = 38,
        [EnumMember]
        ProjectNotes = 39,
        [EnumMember]
        MilestoneNotes = 40,
        [EnumMember]
        OpportunityNotes = 41,
        [EnumMember]
        PersonNotes = 42,
        [EnumMember]
        Exports = 43
    }
}

