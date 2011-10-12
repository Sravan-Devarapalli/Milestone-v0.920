﻿using System;
using DataTransferObjects;
using System.Collections.Generic;

namespace PraticeManagement.Objects
{
    class DetailedUtilizationReportMilestoneItem : DetailedUtilizationReportBaseItem
    {
        #region Constants

        private const string DetailsTooltipFormat = "{0:F2} h/d - {1} - {2} ({3}) - {4} ({5} - {6}) - {7}";
        private const string NotEndDate = "No End Date Specified.";
        private const string DetailsLabelFormat = "{0:F2} h/d - {1}";

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
                    Entry.HoursPerDay,
                    Entry.ParentMilestone.Project.ProjectNumber);
            }
        }

        public override string Tooltip
        {
            get
            {
                return string.Format(
                    DetailsTooltipFormat,
                    Entry.HoursPerDay,
                    Entry.ParentMilestone.Project.Client.Name,
                    Entry.ParentMilestone.Project.Name,
                    Entry.ParentMilestone.Project.ProjectNumber,
                    Entry.ParentMilestone.Description,
                    Entry.StartDate.ToString("MM/dd/yyyy"),
                    Entry.EndDate.HasValue
                        ? Entry.EndDate.Value.ToString("MM/dd/yyyy")
                        : NotEndDate,
                    GetProjectManagers(Entry.ParentMilestone.Project.ProjectManagers));
            }
        }


        private string GetProjectManagers(List<Person> list)
        {
            string names = string.Empty;
            foreach (var person in list)
            {
                names += person.Name + "; ";
            }

            return names;
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
                    Constants.ApplicationPages.UtilizationTimelineWithFilterQueryStringAndDetails);
            }
        }

        #endregion
    }
}
