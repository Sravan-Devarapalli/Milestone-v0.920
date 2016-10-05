using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTransferObjects.Filters
{
    [Serializable]
    public class ExpenseReportFilters
    {
        public string ReportPeriod
        {
            get;
            set;
        }
        public string SelectedView
        {
            get;
            set;
        }

        public DateTime? StartDate
        {
            get;
            set;
        }

        public DateTime? EndDate
        {
            get;
            set;
        }

        public string AccountIds
        {
            get;
            set;
        }

        public string DivisionIds
        {
            get;
            set;
        }

        public string PracticeIds
        {
            get;
            set;
        }

        public string ProjectIds
        {
            get;
            set;
        }

        public string ExpenseTypeIds
        {
            get;
            set;
        }

        public bool IncudeActuals
        {
            get;
            set;
        }

        public bool IncludeEstimated
        {
            get;
            set;
        }

        public bool IsInactive { get; set; }

        public bool IsActive { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsExperimental { get; set; }

        public bool IsProjected { get; set; }

        public bool IsProposed { get; set; }

        public bool IsAtRisk { get; set; }
    }
}

