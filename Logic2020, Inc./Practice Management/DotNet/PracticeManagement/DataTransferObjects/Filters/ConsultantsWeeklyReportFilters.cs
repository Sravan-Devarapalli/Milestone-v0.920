using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTransferObjects.Filters
{
    [Serializable]
    public class ConsultantsWeeklyReportFilters
    {
        public string ReportPeriod
        {
            get;
            set;
        }
        public string Detalization
        {
            get;
            set;
        }

        public string AvgUtil
        {
            get;
            set;
        }
        public string AvgCapacity
        {
            get;
            set;
        }

        public bool ShowMSBadge
        {
            get;
            set;
        }

        public bool ExcludeInvestmentResources
        {
            get;
            set;
        }

        public string PayTypeIds
        {
            get;
            set;
        }
        public string PracticeAreaIds
        {
            get;
            set;
        }

        public bool ExcludeInternalPractice
        {
            get;
            set;
        }

        public string PersonDivisionIds
        {
            get;
            set;
        }

        public bool IsActive { get; set; }

        public bool IsInternal { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsExperimental { get; set; }

        public bool IsProjected { get; set; }

        public bool IsProposed { get; set; }

        public string IsPersonActive
        {
            get;
            set;
        }

        public string IsPersonContingent
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
    }
}

