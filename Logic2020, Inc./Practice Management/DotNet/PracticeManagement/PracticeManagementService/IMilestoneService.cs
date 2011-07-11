using System;
using System.Collections.Generic;
using System.ServiceModel;
using DataTransferObjects;

namespace PracticeManagementService
{
	[ServiceContract]
	public interface IMilestoneService
	{

        /// <summary>
        /// Saves Default Project-milestone details into DB. Persons not assigned to any Project-Milestone 
        /// can enter time entery for this default Project Milestone.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="projectId"></param>
        /// <param name="mileStoneId"></param> 
        [OperationContract]
        void SaveDefaultMilestone(int? clientId, int? projectId, int? milestoneId, int? lowerBound, int? upperBound);

        /// <summary>
        /// Gets the latest default milestone from DB.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        DefaultMilestone GetDefaultMilestone();

		/// <summary>
		/// Retrieves the list of <see cref="Milestone"/>s for the specified <see cref="Project"/>.
		/// </summary>
		/// <param name="projectId">
		/// An ID of the <see cref="Project"/> to retrieve the <see cref="Milestone"/>s for.
		/// </param>
		/// <returns>The list of the <see cref="Project"/>'s <see cref="Milestone"/>s.</returns>
		[OperationContract]
		List<Milestone> MilestoneListByProject(int projectId);

        [OperationContract]
        List<Milestone> MilestoneListByProjectForTimeEntryByProjectReport(int projectId);
		/// <summary>
		/// Reatrives a <see cref="Milestone"/> by a specified ID.
		/// </summary>
		/// <param name="milestoneId">The ID of the <see cref="Milestone"/> to be retrieved.</param>
		/// <returns>The <see cref="Milestone"/> object if found and null otherwise.</returns>
		[OperationContract]
		Milestone GetMilestoneDetail(int milestoneId);

		/// <summary>
		/// Saves a <see cref="Milestone"/> to the database.
		/// </summary>
		/// <param name="milestone">The <see cref="Milestone"/> to be saved.</param>
		/// <param name="userName">A current user.</param>
		/// <returns>An ID of the saved record.</returns>
		[OperationContract]
		int SaveMilestoneDetail(Milestone milestone, string userName);

		/// <summary>
		/// Deletes a <see cref="Milestone"/> from the database.
		/// </summary>
		/// <param name="milestone">The <see cref="Milestone"/> to be deleted.</param>
		/// <param name="userName">A current user.</param>
		[OperationContract]
		void DeleteMilestone(Milestone milestone, string userName);

		/// <summary>
		/// Moves the specified milestone and optionally future milestones forward and backward.
		/// </summary>
		/// <param name="milestoneId">An ID of the <see cref="Milestone"/> to be moved.</param>
		/// <param name="shiftDays">A number of days to move.</param>
		/// <param name="moveFutureMilestones">Determines whether future milestones must be moved too.</param>
		[OperationContract]
		void MilestoneMove(int milestoneId, int shiftDays, bool moveFutureMilestones);

	    /// <summary>
	    /// Moves the specified milestone end date forward and backward.
	    /// </summary>
	    /// <param name="milestoneId">An ID of the <see cref="Milestone"/> to be moved.</param>
	    /// <param name="milestonePersonId"></param>
	    /// <param name="shiftDays">A number of days to move.</param>
	    [OperationContract]
        void MilestoneMoveEnd(int milestoneId, int milestonePersonId, int shiftDays);

		/// <summary>
		/// Clones a specified milestones and set a specified duration to a new one.
		/// </summary>
		/// <param name="milestoneId">An ID of the milestone to be cloned.</param>
		/// <param name="cloneDuration">A clone's duration.</param>
		/// <returns>An ID of a new milestone.</returns>
		[OperationContract]
		int MilestoneClone(int milestoneId, int cloneDuration);

        /// <summary>
        /// Reatrives a <see cref="Milestone"/> by a specified ID without financial and entries info
        /// </summary>
        /// <param name="milestoneId">The ID of the <see cref="Milestone"/> to be retrieved.</param>
        /// <returns>The <see cref="Milestone"/> object if found and null otherwise.</returns>
        [OperationContract]
        Milestone GetMilestoneById(int milestoneId);

	    /// <summary>
	    /// Get's milestone data without person entries
	    /// </summary>
	    /// <param name="milestoneId">Milestone Id</param>
	    /// <returns></returns>
        [OperationContract]
        Milestone GetMilestoneDataById(int milestoneId);

        [OperationContract]
        bool CheckIfExpensesExistsForMilestonePeriod(int milestoneId, DateTime? startDate, DateTime? endDate);


        [OperationContract]
        bool CanMoveFutureMilestones(int milestoneId, int shiftDays);

	    /// <summary>
	    /// Insert entity to the database
	    /// </summary>
	    /// <param name="entity">Entity instance</param>
	    /// <returns>Id of the inserted entity</returns>
        [OperationContract]
        int AddProjectExpense(ProjectExpense entity);

	    /// <summary>
	    /// Remove entity from the database
	    /// </summary>
	    /// <param name="entity">Entity instance</param>
        [OperationContract]
        void RemoveProjectExpense(ProjectExpense entity);

	    /// <summary>
	    /// Update entity in the database
	    /// </summary>
        [OperationContract]
        void UpdateProjectExpense(ProjectExpense entity);

        /// <summary>
        /// List notes by target
        /// </summary>
        [OperationContract]
        List<Note> NoteListByTargetId(int targetId, int noteTargetId);

        /// <summary>
        /// Note insert
        /// </summary>
        [OperationContract]
        int NoteInsert(Note note);

        /// <summary>
        /// Note insert
        /// </summary>
        [OperationContract]
        void NoteUpdate(Note note);

        /// <summary>
        /// Note Delete
        /// </summary>
        [OperationContract]
        void NoteDelete(int noteId);

	    /// <summary>
	    /// Get entity by Id
	    /// </summary>
	    /// <param name="id">Id of the entity</param>
	    /// <returns>Entity instance</returns>
        [OperationContract]
        ProjectExpense GetProjectExpenseById(int id);

	    /// <summary>
	    /// Get entity by Id
	    /// </summary>
	    /// <param name="entity">Entity instance</param>
	    /// <returns>Entity instance</returns>
        [OperationContract]
        ProjectExpense[] GetProjectExpensesForMilestone(ProjectExpense entity);
	}
}

