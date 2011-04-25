using System.Runtime.Serialization;

namespace DataTransferObjects
{
    /// <summary>
    /// determines how to calculate an imputed hourly rate
    /// </summary>
    [DataContract]
    public enum RateType
    {
		[EnumMember]
		Hourly = 1,
		
		[EnumMember]
		Salary = 2
    }
}
