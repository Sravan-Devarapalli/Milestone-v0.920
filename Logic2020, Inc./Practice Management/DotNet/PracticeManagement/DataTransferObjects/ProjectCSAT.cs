using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class ProjectCSAT
    {
        [DataMember]
        public int Id
        {
            get;
            set;
        }

        [DataMember]
        public int ProjectId
        {
            get;
            set;
        }

        [DataMember]
        public DateTime ReviewStartDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime ReviewEndDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime CompletionDate
        {
            get;
            set;
        }

        [DataMember]
        public int ReviewerId
        {
            get;
            set;
        }

        [DataMember]
        public string ReviewerName
        {
            get;
            set;
        }

        [DataMember]
        public int ReferralScore
        {
            get;
            set;
        }

        [DataMember]
        public string Comments
        {
            get;
            set;
        }
    }
}
