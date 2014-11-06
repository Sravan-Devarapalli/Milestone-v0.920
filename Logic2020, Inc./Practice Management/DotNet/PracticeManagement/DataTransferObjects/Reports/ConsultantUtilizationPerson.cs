using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DataTransferObjects.Reports
{
    [DataContract]
    [Serializable]
    public class ConsultantUtilizationPerson
    {
        [DataMember]
        public Person Person
        {
            get;
            set;
        }

        [DataMember]
        public List<ConsultantUtilzationByProject> ProjectUtilization
        {
            get;
            set;
        }

        [DataMember]
        public List<int> WeeklyUtilization
        {
            get;
            set;
        }

        [DataMember]
        public List<decimal> ProjectedHoursList
        {
            get;
            set;
        }

        [DataMember]
        public List<int> WeeklyVacationDays
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<DateTime,double> TimeOffDates
        {
            get;
            set;
        }
        
        [DataMember]
        public Dictionary<DateTime,string> CompanyHolidayDates
        {
            get;
            set;
        }

        [DataMember]
        public decimal AvailableHours
        {
            get;
            set;
        }

        [DataMember]
        public decimal ProjectedHours
        {
            get;
            set;
        }

        public int AverageUtilization
        {
            get { return AvailableHours == 0 ? 0 : (int)Math.Ceiling((double)(ProjectedHours * 100 / AvailableHours)); }
        }

        [DataMember]
        public int PersonVacationDays
        {
            get;
            set;
        }

        [DataMember]
        public List<int> WeeklyPayTypes
        {
            get;
            set;
        }

        [DataMember]
        public int Utilization
        {
            get;
            set;
        }
    }
}
