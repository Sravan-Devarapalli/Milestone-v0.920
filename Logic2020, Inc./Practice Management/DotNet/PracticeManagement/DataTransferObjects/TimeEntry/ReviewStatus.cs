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
        Undefined = 0,//We are not using this value in PM but to overcome issues we kept this Undefined value.

        [EnumMember]
        Pending = 1,

        [EnumMember]
        Approved = 2,

        [EnumMember]
        Declined = 3
    }
}
