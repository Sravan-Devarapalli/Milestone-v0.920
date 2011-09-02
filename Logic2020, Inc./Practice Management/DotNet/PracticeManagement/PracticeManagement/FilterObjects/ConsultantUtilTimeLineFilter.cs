using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PraticeManagement.FilterObjects
{
    [Serializable]
    public class ConsultantUtilTimeLineFilter
    {
        public int? PersonId { get; set; }

        public string ChartTitle { get; set; }

        public bool ActivePersons { get; set; }

        public bool ProjectedPersons { get; set; }

        public string PracticesSelected { get; set; }

        public bool ActiveProjects { get; set; }

        public bool ProjectedProjects { get; set; }

        public bool ExperimentalProjects { get; set; }

        public bool InternalProjects { get; set; }

        public string TimescalesSelected { get; set; }

        public bool ExcludeInternalPractices { get; set; }

        public string SortDirection { get; set; }

        public int SortId { get; set; }

        public int AvgUtil { get; set; }

        public string Period { get; set; }

        public string DetalizationSelectedValue { get; set; }

        public DateTime BegPeriod { get; set; }

        public DateTime EndPeriod { get; set; }

        public bool FiltersChanged { get; set; }
    }
}
