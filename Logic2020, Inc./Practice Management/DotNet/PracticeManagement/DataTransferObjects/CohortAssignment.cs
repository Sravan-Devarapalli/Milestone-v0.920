using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class CohortAssignment
    {
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
        public int Id
        {
            get;
            set;
        }
    }
}

