using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{
    [DataContract]
    [Serializable]
    public class PersonLevelPayCheck
    {
        [DataMember]
        public Person Person
        {
            get;
            set;
        }

        [DataMember]
        public int BranchID
        {
            get;
            set;
        }

        [DataMember]
        public int DeptID
        {
            get;
            set;
        }

        [DataMember]
        public double TotalHoursExcludingTimeOff
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<string, double> WorkTypeLevelTimeOffHours
        {
            get;
            set;
        }

        public double TotalHoursIncludingTimeOff
        {
            get
            {
                double timeOffTotal = WorkTypeLevelTimeOffHours.Sum(t => t.Value);
                return TotalHoursExcludingTimeOff + timeOffTotal;
            }
        }
    }
}
