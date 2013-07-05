using System;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{
    /// <summary>
    /// Represents grouped TimeEntries between startdate and enddate.
    /// </summary>
    [DataContract]
    [Serializable]
    public class GroupedHours
    {
        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public double BillabileTotal { get; set; }

        [DataMember]
        public double NonBillableTotal { get; set; }

        public double CombinedTotal
        {
            get
            {
                return BillabileTotal + NonBillableTotal;
            }
        }

        public void SetEnddate(string groupByCerteria)
        {
            if (StartDate == null || StartDate == DateTime.MinValue) return;
            switch (groupByCerteria)
            {
                case "day":
                    EndDate = StartDate;
                    break;

                case "week":
                    EndDate = StartDate.AddDays(6);
                    break;

                case "month":
                    EndDate = StartDate.AddMonths(1).AddDays(-StartDate.AddMonths(1).Day);
                    break;

                case "year":
                    EndDate = StartDate.AddYears(1).AddDays(-StartDate.AddYears(1).DayOfYear);
                    break;
            }
        }
    }
}
