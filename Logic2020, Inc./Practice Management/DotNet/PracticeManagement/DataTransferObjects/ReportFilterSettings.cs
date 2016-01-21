using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTransferObjects
{
    [Serializable]
    public class ReportFilterSettings
    {
        public List<int> ClientIds { get; set; }
        public List<int> BusinessUnitIds { get; set; }
        public List<int> PayTypeIds { get; set; }
        public List<int> ProjectAccessPeopleIds { get; set; }
        public List<int> PersonStatusIds { get; set; }
        public List<int> PracticeIds { get; set; }
        public List<int> ProjectStatusIds { get; set; }
        public List<int> SalesPersonIds { get; set; }
        public string ReportPeriod { get; set; }
        public DateTime? ReportStartDate { get; set; }
        public DateTime? ReportEndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsInactive { get; set; }
        public bool IsInternal { get; set; }
        public bool IsProposed { get; set; }
        public bool IsProjected { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsExperimental { get; set; }
        public bool ExcludeInternalPractices { get; set; }

    }
}

