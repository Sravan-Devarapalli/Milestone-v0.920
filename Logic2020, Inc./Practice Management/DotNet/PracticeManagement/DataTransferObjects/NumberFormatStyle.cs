using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Determines the styles to display the numbers
	/// </summary>
	[DataContract]
	public enum NumberFormatStyle
	{
		/// <summary>
		/// General formatting without coloring
		/// </summary>
		[EnumMember]
		General = 0,

		/// <summary>
		/// Explicitly specify to use formattong as a negative number
		/// </summary>
		[EnumMember]
		Negative = 1,

		/// <summary>
		/// Specify formatting as a revenue value
		/// </summary>
		[EnumMember]
		Revenue = 2,

		/// <summary>
		/// Specifies formatting as a COGS value
		/// </summary>
		[EnumMember]
		Cogs = 4,

		/// <summary>
		/// Specify formatting as a margin value
		/// </summary>
		[EnumMember]
		Margin = 8,

		/// <summary>
		/// Specifies a value is a total
		/// </summary>
		[EnumMember]
		Total = 16,
        /*
		/// <summary>
		/// Specifies that the value must be formatted as a decimal
		/// </summary>
		[EnumMember]
		Decimal = 32
         */
        /// <summary>
        /// Specifies that the value must be formatted as percentage
        /// </summary>
        [EnumMember]
        Percent = 32
    }
}

