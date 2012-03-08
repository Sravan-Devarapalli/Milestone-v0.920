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

        public int BillabileTotal
        {
            get
            {
                return GroupedHoursList.Sum(G => G.BillabileTotal);
            }
        }

        public int NonBillableTotal
        {
            get
            {
                return GroupedHoursList.Sum(G => G.NonBillableTotal);
            }
        }

        public int CombinedTotal
        {
            get
            {
                return GroupedHoursList.Sum(G => G.CombinedTotal);
            }
        }

    }
}

