using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTransferObjects.Filters
{
    [Serializable]
    public class ConsultingUtilizationReportFilters
    {
        public string ReportPeriod
        {
            get;
            set;
        }

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

        public bool IncludeActiveProjects
        {
            get;
            set;
        }

        public bool IncludeProjectedProjects
        {
            get;
            set;
        }

        public bool IncludeExperimentalProjects
        {
            get;
            set;
        }

        public bool IncludeInternalProjects
        {
            get;
            set;
        }

        public bool IncludeProposedProjects
        {
            get;
            set;
        }

        public bool IncludeCompletedProjects
        {
            get;
            set;
        }

        public string TimescaleIds
        {
            get;
            set;
        }

        public string PracticeIds
        {
            get;
            set;
        }

        public int AverageUtilizataion
        {
            get;
            set;
        }

        public int SortId
        {
            get;
            set;
        }

        public bool SortByAscend
        {
            get;
            set;
        }

        public bool ExcludeInternalPractices
        {
            get;
            set;
        }

        public bool IncludeBadgeStatus
        {
            get;
            set;
        }

        public bool ExcludeInvestmentResources
        {
            get;
            set;
        }

        public string DivisionIds
        {
            get;
            set;
        }
    }
}

