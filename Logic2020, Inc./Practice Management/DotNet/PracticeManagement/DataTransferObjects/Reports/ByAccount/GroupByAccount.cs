using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports.ByAccount
{
    [DataContract]
    [Serializable]
    public class GroupByAccount
    {
        [DataMember]
        public List<BusinessUnitLevelGroupedHours> GroupedBusinessUnits { get; set; }

        [DataMember]
        public List<ProjectLevelGroupedHours> GroupedProjects { get; set; }

        [DataMember]
        public Client Account { get; set; }

        public int BusinessUnitsCount
        {
            get
            {
                return GroupedBusinessUnits != null ? GroupedBusinessUnits.Count : GroupedProjects.Select(p => p.Project.Group.Id).Distinct().Count();
            }
        }

        public int ProjectsCount
        {
            get
            {
                return GroupedBusinessUnits != null ? GroupedBusinessUnits.Sum(g => g.ProjectsCount) : GroupedProjects.Count;
            }
        }

        [DataMember]
        public int PersonsCount { get; set; }

        public Double TotalProjectHours
        {
            get
            {
                return GroupedBusinessUnits != null ? GroupedBusinessUnits.Sum(g => g.TotalHours) : GroupedProjects.Sum(p => p.TotalHours);
            }
        }

        public Double TotalProjectedHours
        {
            get
            {
                return GroupedBusinessUnits != null ? GroupedBusinessUnits.Sum(g => g.ForecastedHours) : GroupedProjects.Sum(p => p.ForecastedHours);
            }
        }

        public Double BillableHours
        {
            get
            {
                return GroupedBusinessUnits != null ? GroupedBusinessUnits.Sum(g => g.BillableHours) : GroupedProjects.Sum(p => p.BillableHours);
            }
        }

        public Double NonBillableHours
        {
            get
            {
                return GroupedBusinessUnits != null ? GroupedBusinessUnits.Sum(g => g.NonBillableHours) : GroupedProjects.Sum(p => p.NonBillableHours);
            }
        }

        public Double BusinessDevelopmentHours
        {
            get
            {
                return GroupedBusinessUnits != null ? GroupedBusinessUnits.Sum(g => g.BusinessDevelopmentHours) : GroupedProjects.Where(p => p.Project.IsBusinessDevelopment == true).Sum(p => p.NonBillableHours);
            }
        }
    }
}

