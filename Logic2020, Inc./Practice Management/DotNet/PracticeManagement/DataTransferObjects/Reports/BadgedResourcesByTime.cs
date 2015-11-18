using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{
    [DataContract]
    [Serializable]
    public class BadgedResourcesByTime
    {
        [DataMember]
        public DateTime StartDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime EndDate
        {
            get;
            set;
        }

        [DataMember]
        public int BadgedOnProjectCount //2
        {
            get;
            set;
        }

        [DataMember]
        public int ResourceCount
        {
            get;
            set;
        }

        [DataMember]
        public int BadgedNotOnProjectCount //1
        {
            get;
            set;
        }

        [DataMember]
        public int ClockNotStartedCount //3
        {
            get;
            set;
        }

        [DataMember]
        public int BlockedCount //4
        {
            get;
            set;
        }

        [DataMember]
        public int InBreakPeriodCount //5
        {
            get;
            set;
        }

        [DataMember]
        public int BadgedOnProjectExceptionCount //6
        {
            get;
            set;
        }

        [DataMember]
        public int BadgedNotOnProjectExceptionCount //7
        {
            get;
            set;
        }

        [DataMember]
        public int TypeNo
        {
            get;
            set;
        }

        [DataMember]
        public Practice Practice
        {
            get;
            set;
        }

        public int Count
        {
            get;
            set;
        }

        public decimal Percentage
        {
            get;
            set;
        }
    }
}

