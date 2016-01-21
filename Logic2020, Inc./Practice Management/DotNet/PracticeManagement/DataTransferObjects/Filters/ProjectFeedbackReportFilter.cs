using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTransferObjects.Filters
{
    [Serializable]
    public class ProjectFeedbackReportFilter
    {
        public string PayTypeIds
        {
            get;
            set;
        }

        public string ClientIds
        {
            get;
            set;
        }

        public string BusinessUnitIds
        {
            get;
            set;
        }

        public string ExecutiveInChargeIds
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

        public string ProjectStatusIds
        {
            get;
            set;
        }

        public string ProjectIds
        {
            get;
            set;
        }

        public string ResourceIds
        {
            get;
            set;
        }

        public string TitleIds
        {
            get;
            set;
        }

        public string StartDateMonths
        {
            get;
            set;
        }

        public string EndDateMonths
        {
            get;
            set;
        }

        public string ProjectAccessIds
        {
            get;
            set;
        }

        public string ProjectFeedbackStatusIds
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

    }
}

