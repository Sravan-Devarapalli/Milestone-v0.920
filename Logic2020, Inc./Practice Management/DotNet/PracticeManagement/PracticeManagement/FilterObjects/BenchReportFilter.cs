using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataTransferObjects.ContextObjects;
using DataTransferObjects;
using System.Web.UI.WebControls;
using System.Runtime.Serialization;

namespace PraticeManagement.FilterObjects
{
    [Serializable]
    public class BenchReportFilter : BenchReportContext
    {
        public BenchReportSortExpression SortExpression { get; set; }
        public SortDirection SortOrder { get; set; }
        public bool FiltersChanged { get; set; }
        public int PeriodSelected { get; set; }
    }
}
