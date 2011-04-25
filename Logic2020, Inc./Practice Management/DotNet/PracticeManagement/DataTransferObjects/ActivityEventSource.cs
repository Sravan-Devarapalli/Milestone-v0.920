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
    }
}

