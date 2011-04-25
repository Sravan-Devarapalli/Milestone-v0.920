using System.Runtime.Serialization;

namespace DataTransferObjects
{
    /// <summary>
    /// Determines an opportunity statuses.
    /// NOTE: these values are linked to NoteTarget database table
    /// </summary>
    [DataContract]
    public enum NoteTarget
    {
        [EnumMember]
        Milestone = 1,
        [EnumMember]
        Project = 2,
        [EnumMember]
        Person = 3,
        [EnumMember]
        Opportunity = 4,
        [EnumMember]
        BillingInfo = 5
    }
}

