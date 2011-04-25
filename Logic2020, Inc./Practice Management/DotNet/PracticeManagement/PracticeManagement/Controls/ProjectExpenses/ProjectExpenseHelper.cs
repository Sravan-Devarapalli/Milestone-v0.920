using System;
using DataTransferObjects;
using PraticeManagement.MilestoneService;

namespace PraticeManagement.Controls.ProjectExpenses
{
    public class ProjectExpenseHelper
    {
        #region Implementation of IDataTransferObjectManipulator<ProjectExpense>

        /// <summary>
        /// 	Get entity by Id
        /// </summary>
        /// <param name = "id">Id of the entity</param>
        /// <returns>Entity instance</returns>
        public static ProjectExpense GetProjectExpenseById(int id)
        {
            return
                ServiceCallers.Invoke<MilestoneServiceClient, ProjectExpense>(
                    client => client.GetProjectExpenseById(id));
        }

        /// <summary>
        /// 	Insert entity to the database
        /// </summary>
        /// <param name = "entity">Entity instance</param>
        /// <returns>Id of the inserted entity</returns>
        public static int AddProjectExpense(ProjectExpense entity)
        {
            return
                ServiceCallers.Invoke<MilestoneServiceClient, int>(
                    client => client.AddProjectExpense(entity));
        }

        /// <summary>
        /// 	Remove entity from the database
        /// </summary>
        /// <param name = "entity">Entity instance</param>
        public static void RemoveProjectExpense(ProjectExpense entity)
        {
            ServiceCallers.Invoke<MilestoneServiceClient>(
                client => client.RemoveProjectExpense(entity));
        }

        /// <summary>
        /// 	Update entity in the database
        /// </summary>
        public static void UpdateProjectExpense(ProjectExpense entity)
        {
            ServiceCallers.Invoke<MilestoneServiceClient>(
                client => client.UpdateProjectExpense(entity));
        }

        /// <summary>
        /// 	Update entity in the database
        /// </summary>
        public static ProjectExpense[] ProjectExpensesForMilestone(int milestoneId)
        {
            var projectExpense = new ProjectExpense {Milestone = new Milestone{Id = milestoneId}};

            var expensesForMilestone = 
                ServiceCallers.Invoke<MilestoneServiceClient, ProjectExpense[]>(
                    client => client.GetProjectExpensesForMilestone(projectExpense));

            // Add fake row to the datatable so we can still see the footer and add new rows
            //  We know that this row is fake because Id will not have value
            if (expensesForMilestone.Length == 0)
                expensesForMilestone = new[] {projectExpense};

            return expensesForMilestone;
        }

        public static void AddProjectExpense(string name, string amount, string reimb, string milestoneId)
        {
            AddProjectExpense(
                    new ProjectExpense
                        {
                            Name = name,
                            Amount = Convert.ToDecimal(amount),
                            Reimbursement = Convert.ToDecimal(reimb),
                            Milestone = new Milestone
                                            {
                                                Id = Convert.ToInt32(milestoneId)
                                            }
                        }
                );
        }

        #endregion
    }
}
