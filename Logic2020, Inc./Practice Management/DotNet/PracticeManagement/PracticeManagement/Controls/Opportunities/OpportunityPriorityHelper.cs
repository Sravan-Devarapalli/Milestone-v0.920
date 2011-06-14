using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataTransferObjects;
using System.ServiceModel;
using PraticeManagement.OpportunityService;

namespace PraticeManagement.Controls.Opportunities
{
    public class OpportunityPriorityHelper
    {
        public static OpportunityPriority[] GetOpportunityPrioritiesListAll()
        {
            using (var serviceClient = new OpportunityServiceClient())
            {
                try
                {
                    OpportunityPriority[] opportunityPriorities = serviceClient.GetOpportunityPrioritiesListAll();
                    return opportunityPriorities;

                }
                catch (CommunicationException ex)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        public static OpportunityPriority[] GetOpportunityPriorities(bool isinserted)
        {
            using (var serviceClient = new OpportunityServiceClient())
            {
                try
                {
                    OpportunityPriority[] opportunityPriorities = serviceClient.GetOpportunityPriorities(isinserted);
                    return opportunityPriorities;

                }
                catch (CommunicationException ex)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        public static void InsertOpportunityPriority(OpportunityPriority opportunityPriority)
        {
            using (var serviceClient = new OpportunityServiceClient())
            {
                try
                {
                    serviceClient.InsertOpportunityPriority(opportunityPriority);

                }
                catch (CommunicationException ex)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        public static void UpdateOpportunityPriority(int oldPriorityId, OpportunityPriority opportunityPriority)
        {
            using (var serviceClient = new OpportunityServiceClient())
            {
                try
                {
                    serviceClient.UpdateOpportunityPriority(oldPriorityId,opportunityPriority);

                }
                catch (CommunicationException ex)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        public static void DeleteOpportunityPriority(int? updatedPriorityId, int deletedPriorityId)
        {
            using (var serviceClient = new OpportunityServiceClient())
            {
                try
                {
                    serviceClient.DeleteOpportunityPriority(updatedPriorityId, deletedPriorityId);

                }
                catch (CommunicationException ex)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        public static bool IsOpportunityPriorityInUse(int priorityId)
        {
            using (var serviceClient = new OpportunityServiceClient())
            {
                try
                {
                    return serviceClient.IsOpportunityPriorityInUse(priorityId);

                }
                catch (CommunicationException ex)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }
    }
}
