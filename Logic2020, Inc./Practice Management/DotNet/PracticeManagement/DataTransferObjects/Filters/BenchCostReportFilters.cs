using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTransferObjects.Filters
{
    [Serializable]
    public class BenchCostReportFilters
    {
        public DateTime? ReportStartDate
        {
            get;
            set;
        }

        public DateTime? ReportEndDate
        {
            get;
            set;
        }

        public string ReportPeriod
        {
            get;
            set;
        }

        public string PracticeIds
        {
            get;
            set;
        }

        public bool IsActive
        {
            get;
            set;
        }

        public bool IsProjected
        {
            get;
            set;
        }

        public bool IsCompleted
        {
            get;
            set;
        }

        public bool IsExperimental
        {
            get;
            set;
        }

        public bool IsProposed
        {
            get;
            set;
        }

        public bool IncludeOverheads
        {
            get;
            set;
        }

        public bool IncludeZeroCost
        {
            get;
            set;
        }

        public bool SeperateInternalExternalTables
        {
            get;
            set;
        }
    }
}

