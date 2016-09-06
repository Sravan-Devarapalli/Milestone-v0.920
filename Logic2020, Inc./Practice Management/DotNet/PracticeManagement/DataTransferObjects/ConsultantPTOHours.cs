using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{
    [DataContract]
    [Serializable]
    public class ConsultantPTOHours
    {


        [DataMember]
        public Person Person
        {
            get;
            set;
        }

        [DataMember]
        public SortedList<DateTime, double> TimeOffDates
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<DateTime, string> CompanyHolidayDates
        {
            get;
            set;
        }

        [DataMember]
        public int PersonVacationDays
        {
            get;
            set;
        }

        public double TotalPTOHours
        {
            get
            {
                double hours = 0;
                foreach (var t in TimeOffDates)
                {
                    hours += t.Value;
                }
                return hours;
            }
        }
    }
}


