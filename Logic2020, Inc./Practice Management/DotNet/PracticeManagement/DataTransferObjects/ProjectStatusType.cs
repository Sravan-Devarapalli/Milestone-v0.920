using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Determines the status of project
	/// </summary>
	[DataContract]
	public enum ProjectStatusType
	{
		[EnumMember]
		Inactive = 1,
		[EnumMember]
		Projected = 2,
		[EnumMember]
		Active = 3,
		[EnumMember]
		Completed = 4,
		[EnumMember]
		Experimental = 5,
        [EnumMember]
        Internal = 6

	}
}
