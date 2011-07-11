using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.ServiceModel.Activation;
using System.Transactions;
using DataAccess;
using DataAccess.Other;
using DataTransferObjects;

namespace PracticeManagementService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class MilestoneService : IMilestoneService
    {
        #region IMilestoneService Members

        /// <summary>
        /// Saves Default Project-milestone details into DB. Persons not assigned to any Project-Milestone 
        /// can enter time entery for this default Project Milestone.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="projectId"></param>
        /// <param name="mileStoneId"></param>
        /// 
        public void SaveDefaultMilestone(int? clientId, int? projectId, int? milestoneId, int? lowerBound, int? upperBound)
        {
            MilestoneDAL.SaveDefaultMilestone(clientId, projectId, milestoneId, lowerBound, upperBound);
        }

        /// <summary>
        /// Gets the latest default milestone from DB.
        /// </summary>
        /// <returns></returns>
        public DefaultMilestone GetDefaultMilestone()
        {
            return MilestoneDAL.GetDefaultMilestone();
        }

        /// <summary>
        /// Retrieves the list of <see cref="Milestone"/>s for the specified <see cref="Project"/>.
        /// </summary>
        /// <param name="projectId">
        /// An ID of the <see cref="Project"/> to retrieve the <see cref="Milestone"/>s for.
        /// </param>
        /// <returns>The list of the <see cref="Project"/>'s <see cref="Milestone"/>s.</returns>
        public List<Milestone> MilestoneListByProject(int projectId)
        {
            List<Milestone> result = MilestoneDAL.MilestoneListByProject(projectId);

            //foreach (Milestone milestone in result)
            //{
            //    PersonRateCalculator.CalculateExpense(
            //        MilestonePersonDAL.MilestonePersonListByMilestone(milestone.Id.Value));
            //}

            return result;
        }

        public List<Milestone> MilestoneListByProjectForTimeEntryByProjectReport(int projectId)
        {
           return  MilestoneDAL.MilestoneListByProjectForTimeEntryByProjectReport(projectId);
        }

        /// <summary>
        /// Reatrives a <see cref="Milestone"/> by a specified ID.
        /// </summary>
        /// <param name="milestoneId">The ID of the <see cref="Milestone"/> to be retrieved.</param>
        /// <returns>The <see cref="Milestone"/> object if found and null otherwise.</returns>
        public Milestone GetMilestoneDetail(int milestoneId)
        {
            Milestone result = GetById(milestoneId, false);

            if (result != null)
            {
                result.ComputedFinancials = ComputedFinancialsDAL.FinancialsGetByMilestone(milestoneId);

                result.MilestonePersons =
                    PersonRateCalculator.GetMilestonePersonListByMilestone(milestoneId).ToArray();

                // Total milestone amount (Revenue)
                if (result.IsHourlyAmount)
                {
                    result.Amount = 0M;
                    foreach (var milestonePerson in result.MilestonePersons)
                        foreach (var entry in milestonePerson.Entries)
                            result.Amount += entry.ProjectedWorkload * entry.HourlyAmount;
                }

                if (result.Project != null)
                {
                    // Practice management commission
                    List<Commission> managementCommission =
                        CommissionDAL.CommissionGetByProjectType(result.Project.Id.Value,
                        CommissionType.PracticeManagement);
                    result.Project.ManagementCommission =
                        managementCommission.Count > 0 ? managementCommission[0] : null;

                    // Sales commission
                    result.Project.SalesCommission =
                        CommissionDAL.CommissionGetByProjectType(result.Project.Id.Value,
                        CommissionType.Sales);
                }
            }

            return result;
        }

        public Milestone GetMilestoneById(int milestoneId)
        {
            return GetById(milestoneId);
        }

        /// <summary>
        /// Get's milestone data without person entries
        /// </summary>
        /// <param name="milestoneId">Milestone Id</param>
        /// <returns></returns>
        public Milestone GetMilestoneDataById(int milestoneId)
        {
            return GetById(milestoneId, false);
        }

        private static Milestone GetById(int milestoneId)
        {
            return GetById(milestoneId, true);
        }

        private static Milestone GetById(int milestoneId, bool fetchMilestonePersons)
        {
            var milestone = MilestoneDAL.GetById(milestoneId);

            if (fetchMilestonePersons)
                milestone.MilestonePersons =
                    PersonRateCalculator.GetMilestonePersonListByMilestoneNoFinancials(milestoneId).ToArray();

            return milestone;
        }

        /// <summary>
        /// Saves a <see cref="Milestone"/> to the database.
        /// </summary>
        /// <param name="milestone">The <see cref="Milestone"/> to be saved.</param>
        /// <param name="userName">A current user.</param>
        /// <returns>An ID of the saved record.</returns>
        public int SaveMilestoneDetail(Milestone milestone, string userName)
        {
            if (!milestone.Id.HasValue)
            {
                MilestoneDAL.MilestoneInsert(milestone, userName);
            }
            else
            {
                MilestoneDAL.MilestoneUpdate(milestone, userName);
            }
            
            return milestone.Id.Value;
        }

        /// <summary>
        /// Deletes a <see cref="Milestone"/> from the database.
        /// </summary>
        /// <param name="milestone">The <see cref="Milestone"/> to be deleted.</param>
        /// <param name="userName">A current user.</param>
        public void DeleteMilestone(Milestone milestone, string userName)
        {
            MilestoneDAL.MilestoneDelete(milestone, userName);
        }

        /// <summary>
        /// Moves the specified milestone and optionally future milestones forward and backward.
        /// </summary>
        /// <param name="milestoneId">An ID of the <see cref="Milestone"/> to be moved.</param>
        /// <param name="shiftDays">A number of days to move.</param>
        /// <param name="moveFutureMilestones">Determines whether future milestones must be moved too.</param>
        public void MilestoneMove(int milestoneId, int shiftDays, bool moveFutureMilestones)
        {
            MilestoneDAL.MilestoneMove(milestoneId, shiftDays, moveFutureMilestones);
        }

        /// <summary>
        /// Moves the specified milestone end date forward and backward.
        /// </summary>
        /// <param name="milestoneId">An ID of the <see cref="Milestone"/> to be moved.</param>
        /// <param name="shiftDays">A number of days to move.</param>
        public void MilestoneMoveEnd(int milestoneId, int milestonePersonId, int shiftDays)
        {
            MilestoneDAL.MilestoneMoveEnd(milestoneId, milestonePersonId, shiftDays);
        }

        /// <summary>
        /// Clones a specified milestones and set a specified duration to a new one.
        /// </summary>
        /// <param name="milestoneId">An ID of the milestone to be cloned.</param>
        /// <param name="cloneDuration">A clone's duration.</param>
        /// <returns>An ID of a new milestone.</returns>
        public int MilestoneClone(int milestoneId, int cloneDuration)
        {
            int result = MilestoneDAL.MilestoneClone(milestoneId, cloneDuration);

            return result;
        }

        public bool CheckIfExpensesExistsForMilestonePeriod(int milestoneId, DateTime? startDate, DateTime? endDate)
        {
            return MilestoneDAL.CheckIfExpensesExistsForMilestonePeriod(milestoneId, startDate, endDate);
        }

        public bool CanMoveFutureMilestones(int milestoneId, int shiftDays)
        {
            return MilestoneDAL.CanMoveFutureMilestones(milestoneId, shiftDays);
        }

        #endregion

        #region Implementation of IDataTransferObjectManipulator<ProjectExpense> and custom methods

        /// <summary>
        /// Insert entity to the database
        /// </summary>
        /// <param name="entity">Entity instance</param>
        /// <returns>Id of the inserted entity</returns>
        public int AddProjectExpense(ProjectExpense entity)
        {
            return (new ProjectExpenseDal()).Add(entity);
        }

        /// <summary>
        /// Remove entity from the database
        /// </summary>
        /// <param name="entity">Entity instance</param>
        public void RemoveProjectExpense(ProjectExpense entity)
        {
            (new ProjectExpenseDal()).Remove(entity);
        }

        /// <summary>
        /// Update entity in the database
        /// </summary>
        public void UpdateProjectExpense(ProjectExpense entity)
        {
            (new ProjectExpenseDal()).Update(entity);
        }

        public List<Note> NoteListByTargetId(int targetId, int noteTargetId)
        {
            return NoteDAL.NoteListByTargetId(targetId, (NoteTarget)noteTargetId);
        }

        public int NoteInsert(Note note)
        {
            NoteDAL.NoteInsert(note);
            return note.Id.Value;
        }

        public void NoteUpdate(Note note)
        {
            NoteDAL.NoteUpdate(note);
            
        }

        public void NoteDelete(int noteId)
        {
            NoteDAL.NoteDelete(noteId);
        }

        /// <summary>
        /// Get entity by Id
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <returns>Entity instance</returns>
        public ProjectExpense GetProjectExpenseById(int id)
        {
            return (new ProjectExpenseDal()).GetById(id);
        }

        /// <summary>
        /// Get entity by Id
        /// </summary>
        /// <param name="entity">Entity instance</param>
        /// <returns>Entity instance</returns>
        public ProjectExpense[] GetProjectExpensesForMilestone(ProjectExpense entity)
        {
            return (new ProjectExpenseDal()).GetForMilestone(entity);
        }


        #endregion
    }
}

