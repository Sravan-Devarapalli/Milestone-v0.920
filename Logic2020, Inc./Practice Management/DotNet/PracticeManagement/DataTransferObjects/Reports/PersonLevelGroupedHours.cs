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
                    return GroupedHoursList.Sum(G => G.BillabileTotal);
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
                    return GroupedHoursList.Sum(G => G.NonBillableTotal);
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
                    return GroupedHoursList.Sum(G => G.CombinedTotal);
                }
                else
                {
                    return 0d;
                }
            }
        }

    }
}

