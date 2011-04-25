using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    public enum CommissionType
    {
        [EnumMember] Sales = 1,
        [EnumMember] PracticeManagement = 2
    }
}
