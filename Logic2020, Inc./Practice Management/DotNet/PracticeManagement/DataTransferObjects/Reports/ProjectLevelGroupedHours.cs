using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{
    /// <summary>
    /// Represents TimeEntries grouped (i.e. day/week/month/year) based on the particular Project
    /// </summary>
    [DataContract]
    [Serializable]
    public class ProjectLevelGroupedHours
    {
        [DataMember]
        public Project Project
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
        public double NonBillableHours
        {
            get;
            set;

        }

        [DataMember]
        public string BillingType { get; set; }

        [DataMember]
        public double BillableHoursUntilToday { get; set; }

        [DataMember]
        public double ForecastedHoursUntilToday
        {
            get;
            set;
        }

        [DataMember]
        public double ForecastedHours
        {
            get;
            set;
        }

        public double TotalHours
        {
            get
            {
                return BillableHours + NonBillableHours;
            }
        }

        public int VariancePercent
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


        public double BillableHoursVariance
        {
            get
            {
                return (BillableHoursUntilToday - ForecastedHoursUntilToday);
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

                sb.Append("height: 24px;");
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
                else if (!BillingType.Equals("Fixed"))
                {
                    sb.Append("Red;");
                }
                else if (BillingType.Equals("Fixed"))
                {
                    sb.Append("Green;");
                }

                sb.Append("height: 24px;");
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
                else if (!BillingType.Equals("Fixed"))
                {

                    sb.Append("Green;");
                }
                else if (BillingType.Equals("Fixed"))
                {
                    sb.Append("Red;");
                }

                sb.Append("height: 24px;");
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

                sb.Append("height: 24px;");
                sb.Append("width: ");
                sb.Append(ForecastedSecondHalfWidth + "%;");

                return sb.ToString();
            }
        }

    }
}

