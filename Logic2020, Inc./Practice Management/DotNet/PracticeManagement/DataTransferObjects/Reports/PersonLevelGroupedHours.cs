using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{
    /// <summary>
    /// Represents TimeEntries grouped (i.e. day/week/month/year) based on the particular Person
    /// </summary>
    [DataContract]
    [Serializable]
    public class PersonLevelGroupedHours
    {
        [DataMember]
        public Person person
        {
            get;
            set;
        }

        [DataMember]
        public List<WorkTypeLevelGroupedHours> GroupedWorkTypeHoursList
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
                if (GroupedHoursList != null)
                {
                    return GroupedHoursList.Count > 0 ? GroupedHoursList.Sum(G => G.BillabileTotal) : 0d;
                }
                else if (GroupedWorkTypeHoursList != null)
                {
                    return GroupedWorkTypeHoursList.Count > 0 ? GroupedWorkTypeHoursList.Sum(G => G.BillabileHours) : 0d;
                }
                else
                {
                    return 0d;
                }


            }
        }

        public double NonBillableTotal
        {
            get
            {
                if (GroupedHoursList != null)
                {
                    return GroupedHoursList.Count > 0 ? GroupedHoursList.Sum(G => G.NonBillableTotal) : 0d;
                }
                else if (GroupedWorkTypeHoursList != null)
                {
                    return GroupedWorkTypeHoursList.Count > 0 ? GroupedWorkTypeHoursList.Sum(G => G.NonBillabileHours) : 0d;
                }
                else
                {
                    return 0d;
                }
            }
        }

        public double CombinedTotal
        {
            get
            {
                if (GroupedHoursList != null)
                {
                    return GroupedHoursList.Count > 0 ? GroupedHoursList.Sum(G => G.CombinedTotal) : 0d;
                }
                else if (GroupedWorkTypeHoursList != null)
                {
                    return GroupedWorkTypeHoursList.Count > 0 ? GroupedWorkTypeHoursList.Sum(G => G.TotalHours) : 0d;
                }
                else
                {
                    return 0d;
                }
            }
        }

    }
}

