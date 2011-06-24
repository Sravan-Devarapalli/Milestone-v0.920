using System;
using DataTransferObjects;

namespace PraticeManagement.Objects
{
    class DetailedUtilizationReportOpportunityItem : DetailedUtilizationReportBaseItem
    {
        /// <summary>
        /// Opportunity #, Priority, Client, Opportunity Name, Last Updated
        /// </summary>
        private const string OpportunityLabelFormat = "{0} - {1} - {2} - {3} ({4}) - {5:d}";

        public OpportunityTransition OpportunityTransition { get; set; }

        public DetailedUtilizationReportOpportunityItem(DateTime reportStartDate, DateTime reportEndDate, OpportunityTransition opportunityTransition) : 
                base(reportStartDate, reportEndDate)
        {
            OpportunityTransition = opportunityTransition;
        }

        public override DateTime StartDate
        {
            get
            {
                return ReportStartDate;
                //return OpportunityTransition.ProjectedStartDate.HasValue ? OpportunityTransition.ProjectedStartDate.Value : ReportStartDate;
            }
        }

        public override DateTime EndDate
        {
            get
            {
                return ReportEndDate;
                //return OpportunityTransition.ProjectedEndDate.HasValue ? OpportunityTransition.ProjectedEndDate.Value : ReportEndDate;
            }
        }

        public override ItemType BarType
        {
            get
            {
                var statusType = OpportunityTransition.OpportunityTransitionStatus.StatusType;

                switch (statusType)
                {
                        case OpportunityTransitionStatusType.Pipeline:
                            return ItemType.PipelineOpportunity;

                        case OpportunityTransitionStatusType.Proposed:
                            return ItemType.ProposeOpportunity;

                        case OpportunityTransitionStatusType.SendOut:
                            return ItemType.SendoutOpportunity;

                    default:
                        return ItemType.OpportunityGeneric;
                }
            }
        }

        public override string Label
        {
            get
            {
                return FormatOpportunityLabel(OpportunityTransition);
            }
        }

        private static string FormatOpportunityLabel(OpportunityTransition trans)
        {
            var opp = trans.Opportunity;

            // Opportunity #, Priority, Client, Opportunity Name, Last Updated
            return string.Format(OpportunityLabelFormat,
                                 opp.OpportunityNumber,
                                 opp.Priority.Priority,
                                 opp.Client.Name,
                                 opp.Name,
                                 trans.OpportunityTransitionStatus.Name,
                                 opp.LastUpdate);
        }

        public override string Tooltip
        {
            get
            {
                return FormatOpportunityLabel(OpportunityTransition);
            }
        }

        public override string NavigateUrl
        {
            get
            {
                var detailsLinkWithFilters = Utils.Urls.OpportunityDetailsLink(OpportunityTransition.Opportunity.Id.Value);
                return Utils.Generic.GetTargetUrlWithReturn(detailsLinkWithFilters, Constants.ApplicationPages.ConsTimelineReport);
            }
        }
    }
}
