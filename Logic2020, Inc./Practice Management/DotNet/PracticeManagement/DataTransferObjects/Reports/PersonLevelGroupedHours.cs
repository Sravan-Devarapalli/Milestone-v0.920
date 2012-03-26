using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{

    [DataContract]
    [Serializable]
    public class PersonLevelGroupedHours
    {
        [DataMember]
        public Person Person
        {
            get;
            set;
        }

        [DataMember]
        public double BillableHours
        {
            get;
            set;

        }

        [DataMember]
        public double BillableValue
        {
            get;
            set;
        }

        [DataMember]
        public double NonBillableHours
        {
            get;
            set;

        }

        [DataMember]
        public double ForecastedHoursUntilToday
        {
            get;
            set;
        }

        [DataMember]
        public double BillableHoursUntilToday { get; set; }

        [DataMember]
        public bool IsPersonNotAssignedToFixedProject { get; set; }

        public double TotalHours
        {
            get
            {
                return BillableHours + NonBillableHours;
            }
        }

        private int VariancePercent
        {
            get
            {
                return ForecastedHoursUntilToday == 0 ? 0 : Convert.ToInt32((((BillableHoursUntilToday - ForecastedHoursUntilToday) / ForecastedHoursUntilToday) * 100));
            }
        }

        public string Variance
        {
            get
            {
                return ForecastedHoursUntilToday == 0 ? "N/A" : (BillableHoursUntilToday - ForecastedHoursUntilToday) >= 0 ? "+" + (BillableHoursUntilToday - ForecastedHoursUntilToday).ToString("0.00") : (BillableHoursUntilToday - ForecastedHoursUntilToday).ToString("0.00");
            }
        }

        private int BillableFirstHalfWidth
        {
            get
            {

                return VariancePercent < 0 ? (100 - (VariancePercent * (-1))) : 100;
            }
        }

        private int BillableSecondHalfWidth
        {
            get
            {
                return VariancePercent < 0 ? (VariancePercent * (-1)) : 0;
            }
        }

        public string BillableFirstHalfHtmlStyle
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("background-color: ");
                if (ForecastedHoursUntilToday == 0)
                {
                    sb.Append("Gray;");
                }
                else
                {
                    sb.Append("White;");
                }

                sb.Append("height: 20px;");
                sb.Append("width: ");
                sb.Append(BillableFirstHalfWidth + "%;");

                return sb.ToString();
            }
        }

        public string BillableSecondHalfHtmlStyle
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("background-color: ");
                if (ForecastedHoursUntilToday == 0)
                {
                    sb.Append("Gray;");
                }
                else
                {
                    sb.Append("Red;");
                }

                sb.Append("height: 20px;");
                sb.Append("width: ");
                sb.Append(BillableSecondHalfWidth + "%;");

                return sb.ToString();
            }
        }
        
        private int ForecastedFirstHalfWidth
        {
            get
            {
                return VariancePercent > 0 ? VariancePercent : 0;
            }
        }

        private int ForecastedSecondHalfWidth
        {
            get
            {
                return VariancePercent > 0 ? (100 - VariancePercent) : 100;
            }
        }

        public string ForecastedFirstHalfHtmlStyle
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("background-color: ");
                if (ForecastedHoursUntilToday == 0)
                {
                    sb.Append("Gray;");
                }
                else
                {
                    sb.Append("Green;");
                }

                sb.Append("height: 20px;");
                sb.Append("width: ");
                sb.Append(ForecastedFirstHalfWidth + "%;");

                return sb.ToString();
            }
        }

        public string ForecastedSecondHalfHtmlStyle
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("background-color: ");
                if (ForecastedHoursUntilToday == 0)
                {
                    sb.Append("Gray;");
                }
                else
                {
                    sb.Append("White;");
                }

                sb.Append("height: 20px;");
                sb.Append("width: ");
                sb.Append(ForecastedSecondHalfWidth + "%;");

                return sb.ToString();
            }
        }


    }
}

