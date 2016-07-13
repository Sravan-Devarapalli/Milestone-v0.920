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
            string vendorId,
            string opportunityId,
            string milestoneId,
            int startRow,
            int maxRows,
            bool practiceAreas = true,
            bool sowBudget = true,
            bool director = true,
            bool poAmount = true,
            bool capabilities = true,
            bool newOrExtension = true,
            bool poNumber = true,
            bool projectStatus = true,
            bool salesPerson = true,
            bool projectOwner = true,
            bool recordPerChange = false,
            bool division = true,
            bool channel = true,
            bool offering = true,
            bool revenueType = true)
        {
            var context = GetContext(startDateFilter, endDateFilter, sourceFilter, personId, projectId,vendorId, opportunityId, milestoneId, practiceAreas, sowBudget, director, poAmount, capabilities, newOrExtension, poNumber, projectStatus, salesPerson, projectOwner, recordPerChange,division,channel,offering,revenueType);
            
            
            var result= ServiceCallers.Custom.ActivityLog(
                client => client.ActivityLogList(context, maxRows, startRow / maxRows));
            return result;
        }

        private static ActivityLogSelectContext GetContext(
            DateTime startDateFilter,
            DateTime endDateFilter,
            string sourceFilter,
            string personId,
            string projectId,
            string vendorId,
            string opportunityId,
            string milestoneId,
            bool practiceAreas,
            bool sowBudget,
            bool director,
            bool poAmount,
            bool capabilities,
            bool newOrExtension,
            bool poNumber,
            bool projectStatus,
            bool salesPerson,
            bool projectOwner,
            bool recordPerChange,
             bool division,
            bool channel,
            bool offering,
            bool revenueType)
        {
            var prsId = Generic.ParseNullableInt(personId);
            var prjId = Generic.ParseNullableInt(projectId);
            var optId = Generic.ParseNullableInt(opportunityId);
            var mlId = Generic.ParseNullableInt(milestoneId);
            var vnId = Generic.ParseNullableInt(vendorId);
            var source = (ActivityEventSource)Enum.Parse(typeof(ActivityEventSource), sourceFilter);

            return new ActivityLogSelectContext
            {
                Source = source,
                StartDate = startDateFilter,
                EndDate = endDateFilter,
                PersonId = prsId,
                ProjectId = prjId,
                VendorId = vnId,
                OpportunityId = optId,
                MilestoneId = mlId,
                PracticeAreas = practiceAreas,
                SowBudget = sowBudget,
                Director = director,
                POAmount = poAmount,
                Capabilities = capabilities,
                NewOrExtension = newOrExtension,
                ProjectStatus = projectStatus,
                SalesPerson = salesPerson,
                ProjectOwner = projectOwner,
                PONumber = poNumber,
                RecordPerSingleChange = recordPerChange,
                Division=division,
                Channel=channel,
                Offering=offering,
                RevenueType=revenueType
            };
        }

        public static int GetActivitiesCount(
            string sourceFilter,
            DateTime startDateFilter,
            DateTime endDateFilter,
            string personId,
            string projectId,
            string vendorId,
            string opportunityId,
            string milestoneId,
            bool practiceAreas = true,
            bool sowBudget = true,
            bool director = true,
            bool poAmount = true,
            bool capabilities = true,
            bool newOrExtension = true,
            bool poNumber = true,
            bool projectStatus = true,
            bool salesPerson = true,
            bool projectOwner = true,
            bool recordPerChange = false,
            bool division = true,
            bool channel = true,
            bool offering = true,
            bool revenueType = true)
        {
            var context = GetContext(startDateFilter, endDateFilter, sourceFilter, personId, projectId,vendorId, opportunityId, milestoneId, practiceAreas, sowBudget, director, poAmount, capabilities, newOrExtension, poNumber, projectStatus, salesPerson, projectOwner, recordPerChange, division, channel, offering, revenueType);

            return ServiceCallers.Custom.ActivityLog(client => client.ActivityLogGetCount(context));
        }
    }
}

