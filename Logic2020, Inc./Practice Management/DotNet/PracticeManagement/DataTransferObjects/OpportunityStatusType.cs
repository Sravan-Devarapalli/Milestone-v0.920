using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Determines an opportunity statuses.
	/// </summary>
	[DataContract]
	public enum OpportunityStatusType
	{
		[EnumMember]
		Active = 1,
		[EnumMember]
		Lost = 2,
		[EnumMember]
		Inactive = 3,
		[EnumMember]
		Won = 4,
        [EnumMember]
        Experimental = 5
    }
}

