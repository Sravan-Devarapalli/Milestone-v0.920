using System;
using System.Collections.Generic;
using System.Linq;

namespace DataTransferObjects.Reports
{
    public class GroupByDate
    {
        public DateTime Date
        {
            get;
            set;
        }

        public List<GroupByClientAndProject> ProjectTotalHours
        {
            get;
            set;
        }

        public double TotalHours
        {
            get
            {
                return ProjectTotalHours != null ? ProjectTotalHours.Sum(p => p.TotalHours) : 0;
            }
        }
    }
}
