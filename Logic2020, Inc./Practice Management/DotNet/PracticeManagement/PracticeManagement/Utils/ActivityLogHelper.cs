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
            string periodFilter,
            string personId,
            string projectId,
            string opportunityId,
            string milestoneId,
            int startRow,
            int maxRows)
        {
            var context = GetContext(periodFilter, sourceFilter, personId, projectId, opportunityId, milestoneId);

            return ServiceCallers.Custom.ActivityLog(
                client => client.ActivityLogList(context, maxRows, startRow / maxRows));
        }

        private static ActivityLogSelectContext GetContext(
            string periodFilter,
            string sourceFilter,
            string personId,
            string projectId,
            string opportunityId,
            string milestoneId)
        {
            DateTime startDate;
            var today = DateTime.Today;
            var endDate = today.AddDays(1.0);

            switch ((DateFilterType)int.Parse(periodFilter))
            {
                case DateFilterType.Today:
                    startDate = today;
                    break;
                case DateFilterType.Week:
                    startDate = endDate.AddDays(-7.0);
                    break;
                case DateFilterType.Month:
                    startDate = endDate.AddMonths(-1);
                    break;
                case DateFilterType.Year:
                    startDate = endDate.AddYears(-1);
                    break;
                case DateFilterType.Unrestircted:
                    startDate = SqlDateTime.MinValue.Value;
                    break;
                default:
                    throw new InvalidOperationException(Resources.Controls.ActivityLogUnknownFilter);
            }

            var prsId = Generic.ParseNullableInt(personId);
            var prjId = Generic.ParseNullableInt(projectId);
            var optId = Generic.ParseNullableInt(opportunityId);
            var mlId = Generic.ParseNullableInt(milestoneId);
            var source = (ActivityEventSource)Enum.Parse(typeof(ActivityEventSource), sourceFilter);

            return new ActivityLogSelectContext
            {
                Source = source,
                StartDate = startDate,
                EndDate = endDate,
                PersonId = prsId,
                ProjectId = prjId,
                OpportunityId = optId,
                MilestoneId = mlId
            };
        }

        public static int GetActivitiesCount(
            string sourceFilter,
            string periodFilter,
            string personId,
            string projectId,
            string opportunityId,
            string milestoneId)
        {
            var context = GetContext(periodFilter, sourceFilter, personId, projectId, opportunityId, milestoneId);

            return ServiceCallers.Custom.ActivityLog(client => client.ActivityLogGetCount(context));
        }
    }
}
