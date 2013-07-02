using System.Collections.Generic;
using System.Linq;

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
                return ProjectTotalHoursList != null ? ProjectTotalHoursList.Sum(d => d.BillableHours) : 0;
            }
        }

        public double NonBillableHours
        {
            get
            {
                return ProjectTotalHoursList != null ? ProjectTotalHoursList.Sum(d => d.NonBillableHours) : 0;
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
