using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTransferObjects.Reports
{
    public class GroupByPersonByWorktype
    {
        public Person Person
        {
            get;
            set;
        }

        public double BillableHours
        {
            get
            {
                if (ProjectTotalHoursList != null)
                {
                    return ProjectTotalHoursList.Sum(d => d.BillableHours);
                }
                else
                {
                    return 0;
                }
            }
        }

        public double NonBillableHours
        {
            get
            {
                if (ProjectTotalHoursList != null)
                {
                    return ProjectTotalHoursList.Sum(d => d.NonBillableHours);
                }
                else
                {
                    return 0;
                }
            }
        }

        public double TotalHours
        {
            get
            {
                return ProjectTotalHoursList != null ? ProjectTotalHoursList.Sum(p => p.TotalHours) : 0;
            }
        }

        public List<TimeEntryByWorkType> ProjectTotalHoursList
        {
            get;
            set;
        }
    }
}

