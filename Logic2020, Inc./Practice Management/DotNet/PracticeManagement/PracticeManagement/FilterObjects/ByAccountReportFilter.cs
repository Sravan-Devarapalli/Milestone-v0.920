using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PraticeManagement.FilterObjects
{
    [Serializable]
    public class ByAccountReportFilter
    {
        public String AccountId { get; set; }
        public String BusinessUnitIds { get; set; }
        public String ProjectStatusIds { get; set; }
        public String RangeSelected { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
