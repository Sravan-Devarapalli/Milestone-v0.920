using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataTransferObjects.ContextObjects;

namespace PraticeManagement.FilterObjects
{
    [Serializable]
    public class GenericTimeEntryFilter
    {
        public string PersonIdsSessionKey { get; set; }

        public string ProjectIdsSessionKey { get; set; }

        public DateTime? MilestoneDateFrom { get; set; }

        public DateTime? MilestoneDateTo { get; set; }

        public double? ForecastedHoursFrom { get; set; }

        public double? ForecastedHoursTo { get; set; }

        public double? ActualHoursFrom { get; set; }

        public double? ActualHoursTo { get; set; }
         
        public int? MilestonePersonId { get; set; }
         
        public int? MilestoneId { get; set; }
         
        public int? TimeTypeId { get; set; }
         
        public string Notes { get; set; }
         
        public bool? IsChargable { get; set; }
         
        public bool? IsCorrect { get; set; }
         
        public string IsReviewed { get; set; }
         
        public DateTime? EntryDateFrom { get; set; }
         
        public DateTime? EntryDateTo { get; set; }
         
        public DateTime? ModifiedDateFrom { get; set; }
         
        public DateTime? ModifiedDateTo { get; set; }
         
        public int? ModifiedBy { get; set; }
         
        public string SortExpression { get; set; }
         
        public int RequesterId { get; set; }
         
        public bool? IsProjectChargeable { get; set; }
         
        public int? PageNo { get; set; }
         
        public int? PageSize { get; set; }
    }
}
