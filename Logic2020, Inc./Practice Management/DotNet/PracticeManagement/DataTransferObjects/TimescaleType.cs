using System.Runtime.Serialization;
using System.ComponentModel;

namespace DataTransferObjects
{
    /// <summary>
    /// Determines the list of employee payment types.
    /// </summary>
    [DataContract]
    public enum TimescaleType
    {
        [EnumMember]
        Undefined = 0,

        /// <summary>
        /// An employee recieve a hourly earnings.
        /// </summary>
        [EnumMember]
        [Description("W2-Hourly")]
        Hourly = 1,

        /// <summary>
        /// An employee recieve a monthly salary.
        /// </summary>
        [EnumMember]
        [Description("W2-Salary")]
        Salary = 2,

        /// <summary>
        /// 1099
        /// </summary>
        [EnumMember]
        [Description("1099-Hourly")]
        _1099Ctc = 3,

        /// <summary>
        /// % of Revenue
        /// </summary>
        [EnumMember]
        [Description("1099-POR")]
        PercRevenue = 4
    }
}

