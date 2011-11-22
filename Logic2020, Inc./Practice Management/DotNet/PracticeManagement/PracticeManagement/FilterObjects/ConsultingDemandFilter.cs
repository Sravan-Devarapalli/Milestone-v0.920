using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PraticeManagement.FilterObjects
{
    [Serializable]
    public class ConsultingDemandFilter
    {
        public string FiltersChanged { get; set; }//0 or 1.

        public string PeriodSelected { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
