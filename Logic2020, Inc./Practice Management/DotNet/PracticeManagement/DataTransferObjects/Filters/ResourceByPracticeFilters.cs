using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTransferObjects.Filters
{
    [Serializable]
    public class BadgeResourceFilters
    {
        public DateTime ReportStartDate
        {
            get;
            set;
        }
        public DateTime ReportEndDate
        {
            get;
            set;
        }

        public int SelectedView
        {
            get;
            set;
        }

        public string PracticeIds
        {
            get;
            set;
        }

        public string TitleIds
        {
            get;
            set;
        }

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


        public bool IsBadgedNotOnProject { get; set; }

        public bool IsBadgedOnProject { get; set; }

        public bool IsClockNotStarted { get; set; }

    }
}

