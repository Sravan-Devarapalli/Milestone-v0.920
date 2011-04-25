using System;

namespace PraticeManagement.Objects
{
    public abstract class DetailedUtilizationReportBaseItem
    {
        #region ItemType enum

        public enum ItemType
        {
            ActiveMilestone,
            ProjectedMilestone,
            PipelineOpportunity,
            SendoutOpportunity,
            ProposeOpportunity,
            OpportunityGeneric
        }

        #endregion

        #region Abstract Properties

        public abstract DateTime StartDate { get; }
        public abstract DateTime EndDate { get; }
        public abstract ItemType BarType { get; }
        public abstract string Label { get; }
        public abstract string Tooltip { get; }
        public abstract string NavigateUrl { get; }

        #endregion

        #region Properties

        public DateTime ReportStartDate { get; set; }
        public DateTime ReportEndDate { get; set; }

        #endregion

        #region Constructors

        protected DetailedUtilizationReportBaseItem(DateTime reportStartDate, DateTime reportEndDate)
        {
            ReportStartDate = reportStartDate;
            ReportEndDate = reportEndDate;
        }

        #endregion
    }
}
