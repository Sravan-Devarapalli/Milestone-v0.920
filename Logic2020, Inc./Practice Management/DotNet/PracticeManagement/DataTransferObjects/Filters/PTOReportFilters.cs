using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTransferObjects.Filters
{
    [Serializable]
    public class PTOReportFilters
    {
        public string Period
        {
            get;
            set;
        }

        public DateTime ReportStartDate
        {
            get;
            set;
        }

        public DateTime ReportEndDate
        {
            get;
            set;
        }

        public int Granularity
        {
            get;
            set;
        }

        public bool IncludeActivePeople
        {
            get;
            set;
        }

        public bool IncludeProjectedPeople
        {
            get;
            set;
        }

        public bool IsW2Hourly
        {
            get;
            set;
        }

        public bool IsW2Salary
        {
            get;
            set;
        }

        public string TitleIds
        {
            get;
            set;
        }

        public string PracticeAreaIds
        {
            get;
            set;
        }

        public string DivisionIds
        {
            get;
            set;
        }

        public int SortId
        {
            get;
            set;
        }

        public string SortDirection
        {
            get;
            set;
        }
    }
}

