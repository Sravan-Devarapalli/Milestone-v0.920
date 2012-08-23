using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports.HumanCapital
{
    [DataContract]
    [Serializable]
    public class TerminationPersonsInRange
    {
        [DataMember]
        public DateTime? StartDate { get; set; }

        [DataMember]
        public DateTime? EndDate { get; set; }

        [DataMember]
        public List<Person> PersonList { get; set; }

        [DataMember]
        public int ActivePersonsCountAtTheBeginning { get; set; }

        [DataMember]
        public int NewHiresCountInTheRange { get; set; }

        [DataMember]
        public int TerminationsCountInTheRange { get; set; }

        public double Attrition
        {
            get
            {
                int denominator = ActivePersonsCountAtTheBeginning + NewHiresCountInTheRange - TerminationsCountInTheRange;
                int numerator = TerminationsCountInTheRange * 100;
                if (denominator != 0)
                {
                    return (double)Math.Round(((decimal)(numerator) / (decimal)denominator), 2);
                }
                return 0d;
            }
        }

        public string Month
        {
            get
            {
                return StartDate.HasValue ? StartDate.Value.ToString("MMM yyyy") : string.Empty;
            }
        }
    }
}

