using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTransferObjects.Filters
{
    [Serializable]
    public class ProjectsListFilters
    {
        public string ClientIds
        {
            get;
            set;
        }

        public string SalesPersonIds
        {
            get;
            set;
        }

        public string BusinessUnitIds
        {
            get;
            set;
        }

        public string PracticeIds
        {
            get;
            set;
        }

        public string DivisionIds
        {
            get;
            set;
        }

        public string ChannelIds
        {
            get;
            set;
        }

        public string OfferingIds
        {
            get;
            set;
        }

        public string RevenueTypeIds
        {
            get;
            set;
        }

        public string ProjectAccessPeopleIds
        {
            get;
            set;
        }

        public string ReportPeriod
        {
            get;
            set;
        }

        public bool IsActive
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

        public bool IsInactive { get; set; }

        public bool IsInternal { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsExperimental { get; set; }

        public bool IsProjected { get; set; }

        public bool IsProposed { get; set; }

        public bool IsAtRisk { get; set; }
    }

}

