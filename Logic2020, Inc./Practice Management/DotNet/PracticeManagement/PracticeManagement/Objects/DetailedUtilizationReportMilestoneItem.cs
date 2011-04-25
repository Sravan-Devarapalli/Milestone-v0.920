using System;
using DataTransferObjects;

namespace PraticeManagement.Objects
{
    class DetailedUtilizationReportMilestoneItem : DetailedUtilizationReportBaseItem
    {
        #region Constants

        private const string DetailsTooltipFormat = "{0} - {6} - {1} - {2} - {3} - {4} - {5:F2} h/d";
        private const string NotEndDate = "No End Date Specified.";
        private const string DetailsLabelFormat = "{0} - {3} - {1} - {2:F2} h/d";

        #endregion

        #region Properties

        public MilestonePersonEntry Entry { get; set; }

        #endregion

        #region Constructors

        public DetailedUtilizationReportMilestoneItem
            (DateTime reportStartDate, DateTime reportEndDate, MilestonePersonEntry entry) :
            base(reportStartDate, reportEndDate)
        {
            Entry = entry;
        }

        #endregion

        #region Overrides

        public override DateTime StartDate
        {
            get { return Entry.StartDate < ReportStartDate ? ReportStartDate : Entry.StartDate; }
        }

        public override DateTime EndDate
        {
            get
            {
                var endDate = Entry.EndDate;

                if (endDate.HasValue)
                    return endDate.Value > ReportEndDate
                               ? ReportEndDate : endDate.Value;

                return ReportEndDate;
            }
        }

        public override ItemType BarType
        {
            get
            {
                var project = Entry.ParentMilestone.Project;

                if (project != null && project.Status != null)
                    return project.Status.StatusType == ProjectStatusType.Projected ?
                                                                                        ItemType.ProjectedMilestone : ItemType.ActiveMilestone;

                return ItemType.ActiveMilestone;
            }
        }

        public override string Label
        {
            get
            {
                return string.Format(
                    DetailsLabelFormat,
                    Entry.ParentMilestone.Project.ProjectNumber,
                    Entry.ParentMilestone.Project.Client.Name,
                    Entry.ParentMilestone.Project.Name,
                    Entry.HoursPerDay);
            }
        }

        public override string Tooltip
        {
            get
            {
                return string.Format(
                    DetailsTooltipFormat,
                    Entry.ParentMilestone.Project.Client.Name,
                    Entry.ParentMilestone.Project.Name,
                    Entry.ParentMilestone.Description,
                    Entry.StartDate.ToShortDateString(),
                    Entry.EndDate.HasValue
                        ? Entry.EndDate.Value.ToShortDateString()
                        : NotEndDate,
                    Entry.HoursPerDay,
                    Entry.ParentMilestone.Project.ProjectNumber);
            }
        }

        public override string NavigateUrl
        {
            get
            {
                return string.Format(
                    Constants.ApplicationPages.MilestoneWithReturnFormat,   //  format
                    Constants.ApplicationPages.MilestoneDetail,             //  page
                    Entry.ParentMilestone.Id,                               //  milestone id
                    Entry.ParentMilestone.Project.Id,                       //  project id
                    Constants.ApplicationPages.UtilizationTimelineWithDetails);
            }
        }

        #endregion
    }
}
