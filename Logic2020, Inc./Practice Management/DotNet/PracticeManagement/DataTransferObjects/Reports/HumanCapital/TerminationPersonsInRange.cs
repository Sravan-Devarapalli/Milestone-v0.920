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

        public List<int> PayTypesList { get; set; }

        [DataMember]
        public int ActivePersonsCountAtTheBeginning { get; set; }

        [DataMember]
        public int NewHiresCountInTheRange { get; set; }


        public int TerminationsContractorsCountInTheRange
        {
            get
            {

                return Terminations1099HourlyCountInTheRange + Terminations1099PORCountInTheRange;
            }
        }

        [DataMember]
        public int TerminationsW2SalaryCountInTheRange { get; set; }

        [DataMember]
        public int TerminationsW2HourlyCountInTheRange { get; set; }

        [DataMember]
        public int Terminations1099HourlyCountInTheRange { get; set; }

        [DataMember]
        public int Terminations1099PORCountInTheRange { get; set; }

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

        public int TerminationsCountForSelectedPaytypes
        {
            get
            {
                int count = 0;
                if (PayTypesList == null)
                {
                    count = TerminationsCountInTheRange;
                }
                else
                {
                    foreach (int i in PayTypesList)
                    {
                        switch (i)
                        {
                            case (int)TimescaleType.Hourly:
                                count += TerminationsW2HourlyCountInTheRange;
                                break;
                            case (int)TimescaleType.Salary:
                                count += TerminationsW2SalaryCountInTheRange;
                                break;
                            case (int)TimescaleType._1099Ctc:
                                count += Terminations1099HourlyCountInTheRange;
                                break;
                            case (int)TimescaleType.PercRevenue:
                                count += Terminations1099PORCountInTheRange;
                                break;
                        }
                    }
                }
                return count;
            }
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

