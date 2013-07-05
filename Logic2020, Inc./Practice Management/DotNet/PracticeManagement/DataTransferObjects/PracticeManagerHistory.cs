using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class PracticeManagerHistory
    {
        [DataMember]
        public int PracticeId
        {
            get;
            set;
        }

        [DataMember]
        public int PracticeManagerId
        {
            get;
            set;
        }

        [DataMember]
        public string PracticeManagerLastName
        {
            get;
            set;
        }

        [DataMember]
        public string PracticeManagerFirstName
        {
            get;
            set;
        }

        /// <summary>
        /// Date from which the practice manager owned up the practice.
        /// </summary>
        [DataMember]
        public DateTime StartDate
        {
            get;
            set;
        }

        /// <summary>
        /// End Date, the practice manager owned up the practice.
        /// </summary>
        [DataMember]
        public DateTime EndDate
        {
            get;
            set;
        }
    }
}
