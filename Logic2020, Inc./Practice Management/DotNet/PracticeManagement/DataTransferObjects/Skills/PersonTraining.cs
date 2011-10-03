using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Skills
{
    [DataContract]
    [Serializable]
    public class PersonTraining
    {
        [DataMember]
        public int? ClientId
        {
            get;
            set;
        }
        public Person Person
        {
            set;
            get;
        }

        public TrainingType TrainingType
        {
            get;
            set;
        }

        [DataMember]
        public string Info
        {
            set;
            get;
        }

        [DataMember]
        public string Institution
        {
            set;
            get;
        }

        [DataMember]
        public DateTime StartDate
        {
            set;
            get;
        }

        [DataMember]
        public DateTime EndDate
        {
            set;
            get;
        }

        [DataMember]
        public int? DisplayOrder
        {
            set;
            get;
        }

        [DataMember]
        public DateTime ModificationDate
        {
            set;
            get;
        }


    }
}

