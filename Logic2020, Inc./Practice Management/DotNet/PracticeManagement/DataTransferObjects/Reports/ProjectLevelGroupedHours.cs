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
        public List<GroupedHours> GroupedHoursList
        {
            get;
            set;
        }

        public double BillabileTotal
        {
            get
            {
                return GroupedHoursList.Sum(G => G.BillabileTotal);
            }
        }

        public double NonBillableTotal
        {
            get
            {
                return GroupedHoursList.Sum(G => G.NonBillableTotal);
            }
        }

        public double CombinedTotal
        {
            get
            {
                return GroupedHoursList.Sum(G => G.CombinedTotal);
            }
        }
    }
}

