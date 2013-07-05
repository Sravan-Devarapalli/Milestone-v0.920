using System;
using System.Collections.Generic;
using System.Linq;

namespace DataTransferObjects.Reports
{
    public class GroupByDateByPerson
    {
        public DateTime Date
        {
            get;
            set;
        }

        public int TimeEntrySectionId { get; set; }

        public List<GroupByPersonByWorktype> ProjectTotalHours
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
