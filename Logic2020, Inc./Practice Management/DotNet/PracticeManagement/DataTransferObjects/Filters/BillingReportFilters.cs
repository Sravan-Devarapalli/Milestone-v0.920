using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTransferObjects.Filters
{
    [Serializable]
    public class BillingReportFilters
    {
        public string AccountIds
        {
            get;
            set;
        }
        public DateTime? StartDate
        {
            get;
            set;
        }
        public DateTime? EndDate
        {
            get;
            set;
        }
        public string PracticeIds
        {
            get;
            set;
        }
        public string BusinessUnitIds
        {
            get;
            set;
        }
        public string ExecutiveInchargeIds
        {
            get;
            set;
        }
        public string UnitOfMeassure
        {
            get;
            set;
        }

        public string DirectorFilteredIds
        {
            get;
            set;
        }

        public string AccountFilteredIds
        {
            get;
            set;
        }

        public string PracticeFilteredIds
        {
            get;
            set;
        }

        public string SalespersonFilteredIds
        {
            get;
            set;
        }

        public string SeniorManagerFilteredIds
        {
            get;
            set;
        }

        public string ProjectManagerFilteredIds
        {
            get;
            set;
        }

        public bool isFiltersApplied
        {
            get;
            set;
        }
    }
}

