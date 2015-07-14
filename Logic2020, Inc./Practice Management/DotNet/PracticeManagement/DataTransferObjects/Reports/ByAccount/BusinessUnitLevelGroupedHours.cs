using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports.ByAccount
{
    [DataContract]
    [Serializable]
    public class BusinessUnitLevelGroupedHours
    {
        [DataMember]
        public ProjectGroup BusinessUnit
        {
            get;
            set;
        }

        [DataMember]
        public List<PersonLevelGroupedHours> PersonLevelGroupedHoursList
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
        public double BillableHoursUntilToday
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
        public double BusinessDevelopmentHours
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

        [DataMember]
        public double ForecastedHoursUntilToday
        {
            get;
            set;
        }

        [DataMember]
        public int BusinessUnitTotalHoursPercent
        {
            get;
            set;
        }

        [DataMember]
        public int ActiveProjectsCount { get; set; }

        [DataMember]
        public int CompletedProjectsCount { get; set; }

        [DataMember]
        public int ProjectsCount { get; set; }

        public double TotalHours
        {
            get
            {
                return PersonLevelGroupedHoursList != null ? PersonLevelGroupedHoursList.Sum(t => t.TotalHours) : BillableHours + NonBillableHours + BusinessDevelopmentHours;
            }
        }
        public double ActualHours
        {
            get
            {
                return PersonLevelGroupedHoursList != null ? PersonLevelGroupedHoursList.Sum(t => t.TotalHours) : BillableHours + NonBillableHours;
            }
        }

        public double BillableHoursVariance
        {
            get
            {
                return (BillableHoursUntilToday - ForecastedHoursUntilToday);
            }
        }
    }
}
