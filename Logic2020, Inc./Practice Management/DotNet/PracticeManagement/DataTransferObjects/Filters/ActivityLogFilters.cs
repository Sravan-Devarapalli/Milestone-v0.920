using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTransferObjects.Filters
{
    [Serializable]
    public class ActivityLogFilters
    {
        public string EventSource
        {
            get;
            set;
        }

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

        public string SelectedPerson
        {
            get;
            set;
        }

        public string SelectedProject
        {
            get;
            set;
        }
    }
}

