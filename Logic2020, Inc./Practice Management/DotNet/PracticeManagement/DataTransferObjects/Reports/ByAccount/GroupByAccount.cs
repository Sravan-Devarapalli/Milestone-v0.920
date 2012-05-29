using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                if (GroupedBusinessUnits != null)
                {
                    return GroupedBusinessUnits.Count;
                }

                return GroupedProjects.Select(p => p.Project.Group.Id).Distinct().Count();
            }
        }

        public int ProjectsCount 
        {
            get
            {
                if (GroupedBusinessUnits != null)
                {
                    return GroupedBusinessUnits.Sum(g => g.ProjectsCount);
                }
                return GroupedProjects.Count;
            }
        }

        [DataMember]
        public int PersonsCount { get; set; }

        public Double TotalProjectHours 
        {
            get
            {
                if (GroupedBusinessUnits != null)
                {
                    return GroupedBusinessUnits.Sum(g => g.TotalHours);
                }
                return GroupedProjects.Sum(p => p.TotalHours);
            }
        }

        public Double BillableHours
        {
            get
            {
                if (GroupedBusinessUnits != null)
                {
                    return GroupedBusinessUnits.Sum(g => g.BillableHours);
                }
                return GroupedProjects.Sum(p => p.BillableHours);
            }
        }

        public Double NonBillableHours
        {
            get
            {
                if (GroupedBusinessUnits != null)
                {
                    return GroupedBusinessUnits.Sum(g => g.NonBillableHours);
                }
                return GroupedProjects.Sum(p => p.NonBillableHours);
            }
        }

        public Double BusinessDevelopmentHours
        {
            get
            {
                if (GroupedBusinessUnits != null)
                {
                    return GroupedBusinessUnits.Sum(g => g.BusinessDevelopmentHours);
                }
                return GroupedProjects.Where(p => p.Project.TimeEntrySectionId == (int)TimeEntrySectionType.BusinessDevelopment).Sum(p => p.NonBillableHours);
            }
        }
    }
}

