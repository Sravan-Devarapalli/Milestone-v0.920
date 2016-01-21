using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTransferObjects.Filters
{
    [Serializable]
    public class TimeReports
    {


        public bool IncludePersonsWithNoTime
        {
            get;
            set;
        }
        public string ReportPeriod
        {
            get;
            set;
        }
        public string SelectedView
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

        public string ProjectNumber
        {
            get;
            set;
        }

        public string Person
        {
            get;
            set;
        }

    }
}

