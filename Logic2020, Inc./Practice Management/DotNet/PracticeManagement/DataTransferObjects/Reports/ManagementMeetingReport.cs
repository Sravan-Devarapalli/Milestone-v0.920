using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{
    [DataContract]
    [Serializable]
    public class ManagementMeetingReport
    {
        [DataMember]
        public Person Person
        {
            get;
            set;
        }

        [DataMember]
        public List<AvailableRange> Range
        {
            get;
            set;
        }

        [DataMember]
        public Title Title
        {
            get;
            set;
        }

        public decimal Average
        {
            get;
            set;
        }

        public decimal Target
        {
            get;
            set;
        }
    }
}

