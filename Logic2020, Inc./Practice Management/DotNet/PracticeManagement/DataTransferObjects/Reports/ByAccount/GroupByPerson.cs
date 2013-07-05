using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports.ByAccount
{
    [DataContract]
    [Serializable]
    public class GroupByPerson
    {
        [DataMember]
        public Person Person
        {
            get;
            set;
        }

        [DataMember]
        public List<GroupByBusinessUnit> BusinessUnitLevelGroupedHoursList
        {
            get;
            set;
        }

        public double TotalHours
        {
            get
            {
                return BusinessUnitLevelGroupedHoursList != null ? BusinessUnitLevelGroupedHoursList.Sum(bu => bu.TotalHours) : 0d;
            }
        }
    }
}
