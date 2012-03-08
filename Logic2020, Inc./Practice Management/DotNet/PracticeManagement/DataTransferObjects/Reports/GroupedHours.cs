using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public int BillabileTotal { get; set; }

        [DataMember]
        public int NonBillableTotal { get; set; }

        [DataMember]
        public int CombinedTotal
        {
            get
            {
                return BillabileTotal + NonBillableTotal;
            }
        }

        public void SetEnddate(string groupByCerteria)
        {
            if(StartDate != null)
            {
                switch (groupByCerteria)
                { 
                    case "day":
                        EndDate = StartDate;
                        break;
                    case "week":
                       EndDate = StartDate.AddDays(6);
                       break;
                    case "month":
                       EndDate = StartDate.AddMonths(1).AddDays(-1);
                       break;
                    case "year":
                       EndDate = StartDate.AddYears(1).AddDays(-1);
                       break;

                }

            }
        }

    }
}

