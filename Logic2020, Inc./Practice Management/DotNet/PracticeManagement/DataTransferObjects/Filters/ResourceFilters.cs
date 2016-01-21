using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTransferObjects.Filters
{
    [Serializable]
    public class ResourceFilters
    {
        public string PersonStatusIds
        {
            get;
            set;
        }
        public string PayTypeIds
        {
            get;
            set;
        }

        public int SelectedView
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

