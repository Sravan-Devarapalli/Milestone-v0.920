using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports.ByAccount
{
    [DataContract]
    [Serializable]
    public class GroupByAccount : IComparer<GroupByAccount>
    {
        [DataMember]
        public List<BusinessUnitLevelGroupedHours> GroupedBusinessUnits { get; set; }

        [DataMember]
        public List<ProjectLevelGroupedHours> GroupedProjects { get; set; }

        [DataMember]
        public Client Account { get; set; }

        //Sort Type = 1 for status,2 for Billing Type and 3 for Total Est Billings
        public int SortType
        {
            get;
            set;
        }

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
                return GroupedBusinessUnits != null ? GroupedBusinessUnits.Sum(g => g.ActiveProjectsCount) + GroupedBusinessUnits.Sum(g => g.CompletedProjectsCount) : GroupedProjects.Count;
            }
        }

        public int ActiveProjectsCount
        {
            get
            {
                return GroupedBusinessUnits.Sum(g => g.ActiveProjectsCount);
            }
        }

        public int CompletedProjectsCount
        {
            get
            {
                return GroupedBusinessUnits.Sum(g => g.CompletedProjectsCount);
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
                return GroupedBusinessUnits != null ? GroupedBusinessUnits.Sum(g => g.BusinessDevelopmentHours) : GroupedProjects.Where(p => p.Project.TimeEntrySectionId == (int)TimeEntrySectionType.BusinessDevelopment).Sum(p => p.NonBillableHours);
            }
        }

        public Double TotalActualHours
        {
            get
            {
                return GroupedBusinessUnits != null ? GroupedBusinessUnits.Sum(g => g.ActualHours) : GroupedProjects.Sum(p => p.TotalHours);
            }
        }

        public Double TotalBillableHoursVariance
        {
            get
            {
                return GroupedBusinessUnits != null ? GroupedBusinessUnits.Sum(g => g.BillableHoursVariance) : GroupedProjects.Sum(p => p.BillableHoursVariance);
            }
        }

        public int Compare(GroupByAccount first, GroupByAccount second)
        {
            int i,returnVal = 0;
            int min = Math.Min(first.GroupedProjects.Count, second.GroupedProjects.Count);
            for(i=0;i<min;i++)
            {
                if(first.SortType == 1)
                {
                    if(first.GroupedProjects[i].Project.Status.Name.CompareTo(second.GroupedProjects[i].Project.Status.Name)==0)
                    {
                        continue;
                    }
                    else
                    {
                        returnVal = first.GroupedProjects[i].Project.Status.Name.CompareTo(second.GroupedProjects[i].Project.Status.Name);
                        break;
                    }
                }
                if (first.SortType == 2)
                {
                    if (first.GroupedProjects[i].BillingType.CompareTo(second.GroupedProjects[i].BillingType) == 0)
                    {
                        continue;
                    }
                    else
                    {
                        returnVal = first.GroupedProjects[i].BillingType.CompareTo(second.GroupedProjects[i].BillingType);
                        break;
                    }
                }
                if (first.SortType == 3)
                {
                    if (first.GroupedProjects[i].EstimatedBillings.CompareTo(second.GroupedProjects[i].EstimatedBillings) == 0)
                    {
                        continue;
                    }
                    else
                    {
                        returnVal = first.GroupedProjects[i].EstimatedBillings.CompareTo(second.GroupedProjects[i].EstimatedBillings);
                        break;
                    }
                }
            }
            if(i==min && first.GroupedProjects.Count != second.GroupedProjects.Count)
                return -1;
            else 
                return returnVal;
        }
    }
}
