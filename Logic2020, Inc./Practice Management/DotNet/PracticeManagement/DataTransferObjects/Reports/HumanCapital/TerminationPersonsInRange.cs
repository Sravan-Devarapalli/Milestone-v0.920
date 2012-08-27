﻿using System;
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
        public int TerminationsContractorsCountInTheRange { get; set; }

        [DataMember]
        public int TerminationsW2SalaryCountInTheRange { get; set; }

        [DataMember]
        public int TerminationsW2HourlyCountInTheRange { get; set; }

        [DataMember]
        public int TerminationsCountInTheRange { get; set; }

        public int TerminationsEmployeeCountInTheRange
        {
            get
            {
                return TerminationsW2SalaryCountInTheRange + TerminationsW2HourlyCountInTheRange;
            }
        }

        public double Attrition
        {
            get
            {
                return CalculateAttrition(ActivePersonsCountAtTheBeginning, NewHiresCountInTheRange, TerminationsEmployeeCountInTheRange);
            }
        }

        public static double CalculateAttrition(int activePersonsCountAtTheBeginning, int newHiresCountInTheRange, int terminationsEmployeeCountInTheRange)
        {
            int denominator = activePersonsCountAtTheBeginning + newHiresCountInTheRange - terminationsEmployeeCountInTheRange;
            int numerator = terminationsEmployeeCountInTheRange;
            if (denominator != 0)
            {
                return (double)((decimal)(numerator) / (decimal)denominator);
            }
            return 0d;

        }

        public double AttritionPercentage
        {
            get
            {
                return Attrition * 100;
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

