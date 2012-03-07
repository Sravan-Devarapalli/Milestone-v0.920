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
        List<GroupedHours> GroupedHoursList
        {
            get;
            set;
        }

        [DataMember]
        public int BillabileTotal
        {
            get
            {
                return GroupedHoursList.Sum(G => G.BillabileTotal);
            }
        }

        [DataMember]
        public int NonBillableTotal
        {
            get
            {
                return GroupedHoursList.Sum(G => G.NonBillableTotal);
            }
        }

        [DataMember]
        public int CombinedTotal
        {
            get
            {
                return GroupedHoursList.Sum(G => G.CombinedTotal);
            }
        }
    }
}

