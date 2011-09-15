using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Determines event sources for the user activity log.
	/// </summary>
	[DataContract]
	public enum ActivityEventSource
	{
		[EnumMember]
		All = 1,
		[EnumMember]
		Person = 2,
		[EnumMember]
		Project = 3,
		[EnumMember]
		Error = 4,
		[EnumMember]
		ProjectAndMilestones = 5,
        [EnumMember]
        TargetPerson = 6,
        [EnumMember]
        TimeEntry = 7,
        [EnumMember]
        Opportunity = 8,
        [EnumMember]
        Milestone = 9,
        [EnumMember]
        Logon = 10,
        [EnumMember]
        LoginSuccessful = 11,
        [EnumMember]
        LoginError = 12,
        [EnumMember]
        Security = 13,
        [EnumMember]
        PasswordResetRequests = 14,
        [EnumMember]
        AccountLockouts = 15,
        [EnumMember]
        AddedTimeEntries = 16,
        [EnumMember]
        ChangedTimeEntries = 17
    }
}

