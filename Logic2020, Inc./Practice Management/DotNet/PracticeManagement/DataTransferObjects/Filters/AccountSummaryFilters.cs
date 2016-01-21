using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTransferObjects.Filters
{
    [Serializable]
    public class AccountSummaryFilters
    {
        public string ReportPeriod
        {
            get;
            set;
        }

        public string AccountId
        {
            get;
            set;
        }
        public string BusinessUnitIds
        {
            get;
            set;
        }
        public string ProjectStatusIds
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
    }
}

