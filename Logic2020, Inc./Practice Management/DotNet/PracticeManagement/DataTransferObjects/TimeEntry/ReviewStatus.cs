using System.Runtime.Serialization;

namespace DataTransferObjects.TimeEntry
{
    /// <summary>
    /// Time entry review status
    /// </summary>
    [DataContract]
    public enum ReviewStatus
    {
        [EnumMember]
        Pending = 1,

        [EnumMember]
        Approved = 2,

        [EnumMember]
        Declined = 3
    }
}
