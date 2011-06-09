using System.Collections.Generic;
using System.ServiceModel;

using DataTransferObjects;
using DataTransferObjects.ContextObjects;

namespace PracticeManagementService
{
	[ServiceContract]
	public interface IOpportunityService
	{
		/// <summary>
		/// Retrives a list of the opportunities for excel export.
		/// </summary>
		/// <returns>A list of the <see cref="Opportunity"/> objects.</returns>
		[OperationContract]
		System.Data.DataSet OpportunityGetExcelSet();

	    /// <summary>
	    /// List Opportunity Transitions
	    /// </summary>
	    /// <param name="opportunityId"></param>
	    /// <param name="statusType"></param>
	    /// <returns></returns>
	    [OperationContract]
        List<OpportunityTransition> GetOpportunityTransitions(int opportunityId, OpportunityTransitionStatusType statusType);

        /// <summary>
        /// List Opportunity Transitions
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<OpportunityTransition> GetOpportunityTransitionsByPerson(int personId);

	    /// <summary>
	    /// Inserts Opportunity Transition
	    /// </summary>
	    /// <param name="transition">Opportunity Transition</param>
	    [OperationContract]
	    int OpportunityTransitionInsert(OpportunityTransition transition);

        /// <summary>
        /// Inserts Opportunity Transition
        /// </summary>
        [OperationContract]
        void OpportunityTransitionDelete(int transitionId);

        /// <summary>
        /// Retrives a list of the opportunities by the specified conditions.
        /// </summary>
        /// <param name="activeOnly">Determines whether only active opportunities must are retrieved.</param>
        /// <param name="looked">Determines a text to be searched within the opportunity name.</param>
        /// <param name="clientId">Determines a client to retrieve the opportunities for.</param>
        /// <param name="salespersonId">Determines a salesperson to retrive the opportunities for.</param>
        /// <returns>A list of the <see cref="Opportunity"/> objects.</returns>
        [OperationContract]
        List<Opportunity> OpportunityListAll(OpportunityListContext context);

        [OperationContract]
        List<Opportunity> OpportunityListAllShort(OpportunityListContext context);
        
        [OperationContract]
        List<OpportunityPriority> GetOpportunityPrioritiesListAll();

        [OperationContract]
        List<OpportunityPriority> GetOpportunityPriorities(bool isinserted);

        [OperationContract]
        void InsertOpportunityPriority(OpportunityPriority opportunityPriority);

        [OperationContract]
        void UpdateOpportunityPriority(int oldPriorityId, OpportunityPriority opportunityPriority);

        [OperationContract]
        void DeleteOpportunityPriority(int? updatedPriorityId, int deletedPriorityId);

		/// <summary>
		/// Retrives an <see cref="Opportunity"/> be a specified ID.
		/// </summary>
		/// <param name="opportunityId">An ID of the record to be retrieved.</param>
		/// <returns>An <see cref="Opportunity"/> object if found and null otherwise.</returns>
		[OperationContract]
		Opportunity GetById(int opportunityId);

		/// <summary>
		/// Saves a new <see cref="Opportunity"/> into the database.
		/// </summary>
		/// <param name="userName">The name of the current user.</param>
		/// <param name="opportunity">The data to be saved.</param>
		[OperationContract]
		int? OpportunitySave(Opportunity opportunity, string userName);

		/// <summary>
		/// Retrieves a list of the Opportunity Statuses.
		/// </summary>
		/// <returns>A list of the <see cref="OpportunityStatus"/> objects.</returns>
		[OperationContract]
		List<OpportunityStatus> OpportunityStatusListAll();

		/// <summary>
		/// Retrieves a list of the Opportunity Transition Statuses objects.
		/// </summary>
		/// <returns>A list of the <see cref="OpportunityTransitionStatus"/> objects.</returns>
		[OperationContract]
		List<OpportunityTransitionStatus> OpportunityTransitionStatusListAll();

	    /// <summary>
	    /// Creates a project from an opportunity.
	    /// </summary>
	    /// <param name="opportunityId">An ID of the opportunity to be converted.</param>
	    /// <param name="userName">A current user.</param>
	    [OperationContract]
		int OpportunityConvertToProject(int opportunityId, string userName);

        /// <summary>
        /// Gets a opportunity id by the oportunity number
        /// </summary>
        /// <param name="opportunityNumber">Number of requested opportunity</param>
        /// <returns>Opportunity ID</returns>
        [OperationContract]
        int? GetOpportunityId(string opportunityNumber);

        ///<summary>
        /// Gets proposed persons of an opportunity
        ///</summary>
        ///<param name="opportunityId">An ID of the opportunity</param>
        ///<returns>A list of proposed persons of an opportunity</returns>
        [OperationContract]
        List<Person> GetOpportunityPersons(int opportunityId);

        ///<summary>
        /// Creates a project from an opportunity. 
        ///</summary>
        ///<param name="opportunityId">An ID of the opportunity to be converted</param>
        ///<param name="userName">A Current User</param>
        ///<returns>Project Id</returns>
        [OperationContract]
        int ConvertOpportunityToProject(int opportunityId, string userName, bool hasPersons);

        ///<summary>
        /// Inserts proposed person into the opportunity
        ///</summary>
        ///<param name="opportunityId">An ID of the opportunity</param>
        ///<param name="personId">An Id of the person</param>
        [OperationContract]
        void OpportunityPersonInsert(int opportunityId, string personIdList,string outSideResources);

        ///<summary>
        /// Deletes proposed person from the Opportunity
        ///</summary>
        ///<param name="opportunityId">An ID of the opportunity</param>
        ///<param name="personId">An Id of the person</param>
        [OperationContract]
        void OpportunityPersonDelete(int opportunityId, string personIdList);

        ///<summary>
        /// Deletes an Opportunity
        ///</summary>
        ///<param name="opportunityId">An ID of the opportunity</param>
        ///<param name="userName">Name of the User</param>
        [OperationContract]
        void OpportunityDelete(int opportunityId, string userName);
    }
}

