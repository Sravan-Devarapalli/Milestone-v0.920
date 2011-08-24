using System;
using System.Data.SqlTypes;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;
using PraticeManagement.Controls;

namespace PraticeManagement.Utils
{
    public class ActivityLogHelper
    {
        public static ActivityLogItem[] GetActivities(
            string sourceFilter,
            DateTime startDateFilter,
            DateTime endDateFilter,
            string personId,
            string projectId,
            string opportunityId,
            string milestoneId,
            int startRow,
            int maxRows)
        {
            var context = GetContext(startDateFilter,endDateFilter, sourceFilter, personId, projectId, opportunityId, milestoneId);

            return ServiceCallers.Custom.ActivityLog(
                client => client.ActivityLogList(context, maxRows, startRow / maxRows));
        }

        private static ActivityLogSelectContext GetContext(
            DateTime startDateFilter,
            DateTime endDateFilter,
            string sourceFilter,
            string personId,
            string projectId,
            string opportunityId,
            string milestoneId)
        {
            var prsId = Generic.ParseNullableInt(personId);
            var prjId = Generic.ParseNullableInt(projectId);
            var optId = Generic.ParseNullableInt(opportunityId);
            var mlId = Generic.ParseNullableInt(milestoneId);
            var source = (ActivityEventSource)Enum.Parse(typeof(ActivityEventSource), sourceFilter);

            return new ActivityLogSelectContext
            {
                Source = source,
                StartDate = startDateFilter,
                EndDate = endDateFilter,
                PersonId = prsId,
                ProjectId = prjId,
                OpportunityId = optId,
                MilestoneId = mlId
            };
        }

        public static int GetActivitiesCount(
            string sourceFilter,
            DateTime startDateFilter,
            DateTime endDateFilter,
            string personId,
            string projectId,
            string opportunityId,
            string milestoneId)
        {
            var context = GetContext(startDateFilter,endDateFilter, sourceFilter, personId, projectId, opportunityId, milestoneId);

            return ServiceCallers.Custom.ActivityLog(client => client.ActivityLogGetCount(context));
        }
    }
}
