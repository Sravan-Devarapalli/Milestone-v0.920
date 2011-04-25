using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataTransferObjects.ContextObjects
{
    /// <summary>
    /// Represents time entry select query context
    /// </summary>
    [DataContract]
    [Serializable]
    public class TimeEntrySelectContext
    {
        [DataMember]
        public IList<int?> PersonIds { get; set; }

        [DataMember]
        public DateTime? MilestoneDateFrom { get; set; }

        [DataMember]
        public DateTime? MilestoneDateTo { get; set; }

        [DataMember]
        public double? ForecastedHoursFrom { get; set; }

        [DataMember]
        public double? ForecastedHoursTo { get; set; }

        [DataMember]
        public double? ActualHoursFrom { get; set; }

        [DataMember]
        public double? ActualHoursTo { get; set; }

        [DataMember]
        public IList<int?> ProjectIds { get; set; }

        [DataMember]
        public int? MilestonePersonId { get; set; }

        [DataMember]
        public int? MilestoneId { get; set; }

        [DataMember]
        public int? TimeTypeId { get; set; }

        [DataMember]
        public string Notes { get; set; }

        [DataMember]
        public bool? IsChargable { get; set; }

        [DataMember]
        public bool? IsCorrect { get; set; }

        [DataMember]
        public string IsReviewed { get; set; }

        [DataMember]
        public DateTime? EntryDateFrom { get; set; }

        [DataMember]
        public DateTime? EntryDateTo { get; set; }

        [DataMember]
        public DateTime? ModifiedDateFrom { get; set; }

        [DataMember]
        public DateTime? ModifiedDateTo { get; set; }

        [DataMember]
        public int? ModifiedBy { get; set; }

        [DataMember]
        public string SortExpression { get; set; }

        [DataMember]
        public int RequesterId { get; set; }

        [DataMember]
        public bool? IsProjectChargeable { get; set; }

        [DataMember]
        public int? PageNo { get; set; }

        [DataMember]
        public int? PageSize { get; set; }
    }
}
