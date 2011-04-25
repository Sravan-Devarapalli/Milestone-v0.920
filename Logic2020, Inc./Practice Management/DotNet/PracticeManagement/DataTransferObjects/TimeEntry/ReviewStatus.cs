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
        Pending,

        [EnumMember]
        Approved,

        [EnumMember]
        Declined
    }
}
