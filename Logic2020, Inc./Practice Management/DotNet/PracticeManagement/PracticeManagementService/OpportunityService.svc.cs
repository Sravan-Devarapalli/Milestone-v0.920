using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Transactions;
using DataAccess;
using DataAccess.Other;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;
using System;

namespace PracticeManagementService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceKnownType(typeof(OpportunityTransition))]
    [ServiceKnownType(typeof(Opportunity))]
    [ServiceKnownType(typeof(OpportunityStatus))]
    public class OpportunityService : IOpportunityService
    {
        #region IOpportunityService Members

        public List<OpportunityTransition> GetOpportunityTransitions(int opportunityId,
                                                                     OpportunityTransitionStatusType statusType)
        {
            return OpportunityTransitionDAL.OpportunityTransitionGetByOpportunity(opportunityId, statusType);
        }

        public List<OpportunityTransition> GetOpportunityTransitionsByPerson(int personId)
        {
            return OpportunityTransitionDAL.OpportunityTransitionGetByPerson(personId);
        }

        public void UpdatePriorityIdForOpportunity(int opportunityId, int priorityId, string userName)
        {
            OpportunityDAL.UpdatePriorityIdForOpportunity(opportunityId, priorityId, userName);
        }

        /// <summary>
        /// 	Inserts Opportunity Transition
        /// </summary>
        /// <param name = "transition">Opportunity Transition</param>
        public int OpportunityTransitionInsert(OpportunityTransition transition)
        {
            return OpportunityTransitionDAL.OpportunityTransitionInsert(transition);
        }

        public void OpportunityTransitionDelete(int transitionId)
        {
            OpportunityTransitionDAL.OpportunityTransitionDelete(transitionId);
        }

        /// <summary>
        /// 	Retrives a list of the opportunities by the specified conditions.
        /// </summary>
        /// <param name = "activeOnly">Determines whether only active opportunities must are retrieved.</param>
        /// <param name = "looked">Determines a text to be searched within the opportunity name.</param>
        /// <param name = "clientId">Determines a client to retrieve the opportunities for.</param>
        /// <param name = "salespersonId">Determines a salesperson to retrive the opportunities for.</param>
        /// <returns>A list of the <see cref = "Opportunity" /> objects.</returns>
        public List<Opportunity> OpportunityListAll(OpportunityListContext context)
        {
            var opportunities = OpportunityDAL.OpportunityListAll(context);
            OpportunityDAL.FillProposedPersons(opportunities);
            return opportunities;
        }

        public List<Opportunity> OpportunityListAllShort(OpportunityListContext context)
        {
            return OpportunityDAL.OpportunityListAllShort(context);
        }

        public List<OpportunityPriority> GetOpportunityPrioritiesListAll()
        {
            return OpportunityDAL.GetOpportunityPrioritiesListAll();
        }

        public List<OpportunityPriority> GetOpportunityPriorities(bool isinserted)
        {
            return OpportunityDAL.GetOpportunityPriorities(isinserted);
        }

        public void InsertOpportunityPriority(OpportunityPriority opportunityPriority)
        {
            OpportunityDAL.InsertOpportunityPriority(opportunityPriority);
        }

        public void UpdateOpportunityPriority(int oldPriorityId, OpportunityPriority opportunityPriority, string userName)
        {
            OpportunityDAL.UpdateOpportunityPriority(oldPriorityId, opportunityPriority, userName);
        }


        public void DeleteOpportunityPriority(int? updatedPriorityId, int deletedPriorityId, string userName)
        {
            OpportunityDAL.DeleteOpportunityPriority(updatedPriorityId, deletedPriorityId, userName);
        }


        public bool IsOpportunityPriorityInUse(int priorityId)
        {
            return OpportunityDAL.IsOpportunityPriorityInUse(priorityId);
        }

        /// <summary>
        /// 	Retrives an <see cref = "Opportunity" /> be a specified ID.
        /// </summary>
        /// <param name = "opportunityId">An ID of the record to be retrieved.</param>
        /// <returns>An <see cref = "Opportunity" /> object if found and null otherwise.</returns>
        public Opportunity GetById(int opportunityId)
        {
            return OpportunityDAL.OpportunityGetById(opportunityId);
        }

        /// <summary>
        /// 	Retrives <see cref = "Opportunity" /> data to be exported to excel.
        /// </summary>
        /// <returns>An <see cref = "Opportunity" /> object if found and null otherwise.</returns>
        public DataSet OpportunityGetExcelSet()
        {
            var result =
                OpportunityDAL.OpportunityGetExcelSet();

            return result;
        }

        /// <summary>
        /// 	Saves a new <see cref = "Opportunity" /> into the database.
        /// </summary>
        /// <param name = "userName">The name of the current user.</param>
        /// <param name = "opportunity">The data to be saved.</param>
        public int? OpportunitySave(Opportunity opportunity, string userName)
        {
            if (!opportunity.Id.HasValue)
                OpportunityDAL.OpportunityInsert(opportunity, userName);
            else
                OpportunityDAL.OpportunityUpdate(opportunity, userName);

            return opportunity.Id;
        }

        /// <summary>
        /// 	Retrieves a list of the Opportunity Statuses.
        /// </summary>
        /// <returns>A list of the <see cref = "OpportunityStatus" /> objects.</returns>
        public List<OpportunityStatus> OpportunityStatusListAll()
        {
            var result = OpportunityStatusDAL.OpportunityStatusListAll();

            return result;
        }

        /// <summary>
        /// 	Retrieves a list of the Opportunity Transition Statuses objects.
        /// </summary>
        /// <returns>A list of the <see cref = "OpportunityTransitionStatus" /> objects.</returns>
        public List<OpportunityTransitionStatus> OpportunityTransitionStatusListAll()
        {
            var result =
                OpportunityTransitionStatusDAL.OpportunityTransitionStatusListAll();

            return result;
        }

        /// <summary>
        /// 	Creates a project from an opportunity.
        /// </summary>
        /// <param name = "opportunityId">An ID of the opportunity to be converted.</param>
        /// <param name = "userName">A current user.</param>
        public int OpportunityConvertToProject(int opportunityId, string userName)
        {
            var project = OpportunityDAL.OpportunityConvertToProject(opportunityId, userName);

            return project;
        }

        /// <summary>
        /// Gets a opportunity id by the oportunity number
        /// </summary>
        /// <param name="opportunityNumber">Number of requested opportunity</param>
        /// <returns>Opportunity ID</returns>
        public int? GetOpportunityId(string opportunityNumber)
        {
            return OpportunityDAL.GetOpportunityId(opportunityNumber);
        }

        ///<summary>
        /// Gets proposed persons of an opportunity
        ///</summary>
        ///<param name="opportunityId">An ID of the opportunity</param>
        ///<returns>A list of proposed persons of an opportunity</returns>
        public List<OpportunityPerson> GetOpportunityPersons(int opportunityId)
        {
            return OpportunityDAL.GetOpportunityPersons(opportunityId);
        }

        ///<summary>
        /// Creates a project from an opportunity. 
        ///</summary>
        ///<param name="opportunityId">An ID of the opportunity to be converted</param>
        ///<param name="userName">A Current User</param>
        ///<returns>Project Id</returns>
        public int ConvertOpportunityToProject(int opportunityId, string userName, bool hasPersons)
        {
            var project = OpportunityDAL.ConvertOpportunityToProject(opportunityId, userName, hasPersons);

            return project;
        }

        ///<summary>
        /// Inserts proposed person into the opportunity
        ///</summary>
        ///<param name="opportunityId">An ID of the opportunity</param>
        ///<param name="personId">An Id of the person</param>
        public void OpportunityPersonInsert(int opportunityId, string personIdList, string outSideResources)
        {
            OpportunityDAL.OpportunityPersonInsert(opportunityId, personIdList, outSideResources);
        }

        ///<summary>
        /// Deletes proposed person from the Opportunity
        ///</summary>
        ///<param name="opportunityId">An ID of the opportunity</param>
        ///<param name="personId">An Id of the person</param>
        public void OpportunityPersonDelete(int opportunityId, string personIdList)
        {
            OpportunityDAL.OpportunityPersonDelete(opportunityId, personIdList);
        }

        public void OpportunityDelete(int opportunityId, string userName)
        {
            OpportunityDAL.OpportunityDelete(opportunityId, userName);//It will delete only Inactive and Experimental Opportunities as per #2702.
        }

        public IDictionary<string, int> GetOpportunityPriorityTransitionCount(int daysPrevious)
        {
            return OpportunityDAL.GetOpportunityPriorityTransitionCount(daysPrevious);
        }

        public IDictionary<string, int> GetOpportunityStatusChangeCount(int daysPrevious)
        {
            return OpportunityDAL.GetOpportunityStatusChangeCount(daysPrevious);
        }

        public List<Opportunity> FilteredOpportunityListAll(bool showActive, bool showExperimental, bool showInactive, bool showLost, bool showWon, string clientIdsList, string opportunityGroupIdsList, string opportunityOwnerIdsList, string salespersonIdsList)
        {
            try
            {
                List<Opportunity> opportunities = OpportunityDAL.FilteredOpportunityListAll(showActive, showExperimental, showInactive, showLost, showWon, clientIdsList, opportunityGroupIdsList, opportunityOwnerIdsList, salespersonIdsList);
                return opportunities;
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "FilteredOpportunityListAll", "OpportunityService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        public List<Opportunity> OpportunitySearchText(string looked, int personId)
        {
            try
            {
                return OpportunityDAL.OpportunitySearchText(looked, personId);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "OpportunitySearchText", "OpportunityService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        #endregion
    }
}

