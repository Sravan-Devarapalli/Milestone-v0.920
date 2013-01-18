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
        public double PTOHours
        {
            get;
            set;

        }
        [DataMember]
        public double HolidayHours
        {
            get;
            set;

        }
        [DataMember]
        public double BereavementHours
        {
            get;
            set;

        }
        [DataMember]
        public double JuryDutyHours
        {
            get;
            set;

        }
        [DataMember]
        public double ORTHours
        {
            get;
            set;

        }

        [DataMember]
        public double UnpaidHours
        {
            get;
            set;
        }

        [DataMember]
        public double SickOrSafeLeaveHours
        {
            get;
            set;
        }


        [DataMember]
        public int TimeEntrySectionId { get; set; }       

        public double NonBillableHours
        {
            get
            {
                return DayTotalHours != null ? DayTotalHours.Sum(d => d.NonBillableHours) : ProjectNonBillableHours + BusinessDevelopmentHours + InternalHours + AdminstrativeHours;
            }
        }

        public bool IsPersonTimeEntered
        {
            get
            {
                if (BillableHours == 0 && NonBillableHours == 0)
                {
                    return false;
                }
                return true;
            }
        }

        [DataMember]
        public List<TimeEntriesGroupByDate> DayTotalHours
        {
            get;
            set;
        }

        [DataMember]
        public double ProjectNonBillableHours
        {
            get;
            set;

        }

        [DataMember]
        public double BusinessDevelopmentHours
        {
            get;
            set;

        }

        [DataMember]
        public double InternalHours
        {
            get;
            set;

        }


        public double AdminstrativeHours
        {
            get
            {
                return PTOHours + HolidayHours + BereavementHours + JuryDutyHours + ORTHours + UnpaidHours + SickOrSafeLeaveHours;
            }

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
        public string BillingType { get; set; }

        [DataMember]
        public double ForecastedHours { get; set; }

        [DataMember]
        public double AvailableHours { get; set; }

        [DataMember]
        public double AvailableHoursUntilToday { get; set; }

        public double TotalHours
        {
            get
            {
                return DayTotalHours != null ? DayTotalHours.Sum(t => t.TotalHours) : (BillableHours + NonBillableHours);
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

        public void AddDayTotalHours(TimeEntriesGroupByDate dt)
        {
            if (DayTotalHours.Any(dth => dth.Date == dt.Date))
            {
                var workType = dt.DayTotalHoursList[0];
                dt = DayTotalHours.First(dth => dth.Date == dt.Date);
                dt.DayTotalHoursList.Add(workType);
            }
            else
            {
                DayTotalHours.Add(dt);
            }

        }
    }
}

