
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Determines the status for the <see cref="OpportunityTransition"/>.
	/// </summary>
    [DataContract]
    public enum OpportunityTransitionStatusType
	{
        [EnumMember]
        All = 0,
        [EnumMember]
        Created = 1,
        [EnumMember]
        Notes = 2,
        [EnumMember]
        Proposed = 3,
        [EnumMember]
        Presented = 4,
        [EnumMember]
        SendOut = 5,
        [EnumMember]
        Pipeline = 6,
        [EnumMember]
        Lost = 7
	}
}

