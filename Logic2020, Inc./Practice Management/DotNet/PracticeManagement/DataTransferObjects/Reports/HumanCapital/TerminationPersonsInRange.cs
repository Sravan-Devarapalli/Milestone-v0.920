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

        public int Attrition
        {
            get
            {
                if ((ActivePersonsCountAtTheBeginning + NewHiresCountInTheRange - TerminationsCountInTheRange) != 0)
                {
                    return TerminationsCountInTheRange / (ActivePersonsCountAtTheBeginning + NewHiresCountInTheRange - TerminationsCountInTheRange);
                }
                return 0;
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

