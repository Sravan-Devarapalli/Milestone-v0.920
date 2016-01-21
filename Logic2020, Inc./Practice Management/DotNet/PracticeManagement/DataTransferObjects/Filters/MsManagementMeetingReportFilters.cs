using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTransferObjects.Filters
{
     [Serializable]
    public class MsManagementReportFilters
    {
        public string PayTypeIds
        {
            get;
            set;
        }

        public string PersonStatusIds
        {
            get;
            set;
        }
    }
}

