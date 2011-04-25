using System.Runtime.Serialization;

namespace DataTransferObjects
{
 	/// <summary>
	/// Determines the list of revenue types.
	/// </summary>
	[DataContract]
	public enum RevenueType
	{
        [EnumMember]
        Unknown = 0,

		/// <summary>
        /// New
		/// </summary>
		[EnumMember]
		New = 1,

		/// <summary>
		/// Old
		/// </summary>
		[EnumMember]
		Old = 2,

		/// <summary>
        /// Undefined
		/// </summary>
		[EnumMember]
        Undefined = 3
	}
}

