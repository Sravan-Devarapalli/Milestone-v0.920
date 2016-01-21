using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTransferObjects.Filters
{
    [Serializable]
    public class TerminationReportFilters
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

        public bool ExcludeInternalPractices
        {
            get;
            set;
        }

        public string PayTypeIds
        {
            get;
            set;
        }


        public string TitleIds
        {
            get;
            set;
        }

        public string TerminationReasonIds
        {
            get;
            set;
        }

    }
}

