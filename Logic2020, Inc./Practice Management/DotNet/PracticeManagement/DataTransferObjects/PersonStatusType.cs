using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Determines the status of project
	/// </summary>
	[DataContract]
	public enum PersonStatusType
	{
		[EnumMember]
		Active = 1,
		[EnumMember]
		Terminated = 2,
		[EnumMember]
        //Projected = Contingent
		Projected = 3,
		[EnumMember]
		Inactive = 4,
        [EnumMember]
        TerminationPending = 5
	}
}
