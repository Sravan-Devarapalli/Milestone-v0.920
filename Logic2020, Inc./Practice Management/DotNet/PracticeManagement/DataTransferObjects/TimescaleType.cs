using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Determines the list of employee payment types.
	/// </summary>
	[DataContract]
	public enum TimescaleType
	{
		/// <summary>
		/// An employee recieve a hourly earnings.
		/// </summary>
		[EnumMember]
		Hourly = 1,

		/// <summary>
		/// An employee recieve a monthly salary.
		/// </summary>
		[EnumMember]
		Salary = 2,

		/// <summary>
		/// 1099
		/// </summary>
		[EnumMember]
        _1099Ctc = 3,

        /// <summary>
        /// % of Revenue
        /// </summary>
        [EnumMember]
        PercRevenue = 4
	}
}

