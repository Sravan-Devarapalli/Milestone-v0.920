using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [Serializable]
    [DataContract]
    public class ProjectFeedback
    {
        [DataMember]
        public int? Id
        {
            get;
            set;
        }

        [DataMember]
        public Project Project
        {
            get;
            set;
        }

        [DataMember]
        public Person Person
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
        public DateTime DueDate
        {
            get;
            set;
        }

        [DataMember]
        public ProjectFeedbackStatus Status
        {
            get;
            set;
        }

        [DataMember]
        public bool IsCanceled
        {
            get;
            set;
        }

        [DataMember]
        public string CancelationReason
        {
            get;
            set;
        }

        [DataMember]
        public string CompletionCertificateBy
        {
            get;
            set;
        }

        [DataMember]
        public DateTime CompletionCertificateDate
        {
            get;
            set;
        }

        [DataMember]
        public bool IsGap
        {
            get;
            set;
        }

        [DataMember]
        public int Count
        {
            get;
            set;
        }
    }
}

