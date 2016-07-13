using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTransferObjects.Filters
{
    [Serializable]
    public class PersonsFilters
    {
        public string SearchText
        {
            get;
            set;
        }
        public string ReportView
        {
            get;
            set;
        }

        public string RecruiterIds
        {
            get;
            set;
        }

        public string PracticeAreaIds
        {
            get;
            set;
        }

        public string DivisionIds
        {
            get;
            set;
        }

        public string PayTypeIds
        {
            get;
            set;
        }

        public bool IsActive
        {
            get;
            set;
        }

        public bool IsTerminated
        {
            get;
            set;
        }

        public bool IsContingent
        {
            get;
            set;
        }

        public bool IsTeminationPending
        {
            get;
            set;
        }
    }
}

