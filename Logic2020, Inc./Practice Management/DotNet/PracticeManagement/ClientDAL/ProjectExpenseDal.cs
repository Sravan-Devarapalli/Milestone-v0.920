using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataAccess.Generic;
using DataAccess.Readers;
using DataTransferObjects;

namespace DataAccess
{
    public class ProjectExpenseDal : DalBase<ProjectExpense, ProjectExpenseReader>
    {
        #region Overrides of DalBase<ProjectExpense>

        #region Procedure names

        protected override string GetByIdProcedure
        {
            get { return Constants.ProcedureNames.ProjectExpenses.GetById; }
        }

        protected override string AddProcedure
        {
            get { return Constants.ProcedureNames.ProjectExpenses.Insert; }
        }

        protected override string UpdateProcedure
        {
            get { return Constants.ProcedureNames.ProjectExpenses.Update; }
        }

        protected override string RemoveProcedure
        {
            get { return Constants.ProcedureNames.ProjectExpenses.Delete; }
        }

        #endregion

        #region Base initializers

        protected override SqlParameter InitAddCommand(ProjectExpense entity, SqlCommand command)
        {
            InitPropertiesNoId(command, entity);
            command.Parameters.AddWithValue(Constants.ParameterNames.ProjectId, entity.ProjectId);
           

            var outParam =
                new SqlParameter(Constants.ParameterNames.ExpenseId, SqlDbType.Int)
                    {Direction = ParameterDirection.Output};

            command.Parameters.Add(outParam);

            return outParam;
        }

        protected override void InitRemoveCommand(ProjectExpense entity, SqlCommand command)
        {
            InitSingleId(command, entity);
        }

        protected override void InitUpdateCommand(ProjectExpense entity, SqlCommand command)
        {
            InitSingleId(command, entity);
            InitPropertiesNoId(command, entity);
        }

        protected override void InitGetById(ProjectExpense entity, SqlCommand command)
        {
            InitSingleId(command, entity);
        }

        protected override EntityReaderBase<ProjectExpense> InitEntityReader(DbDataReader reader)
        {
            return new ProjectExpenseReader(reader);
        }

        #endregion

        #region Derived initializers

        private static void InitPropertiesNoId(SqlCommand command, ProjectExpense entity)
        {
            command.Parameters.AddWithValue(Constants.ParameterNames.ExpenseName, entity.Name);
            command.Parameters.AddWithValue(Constants.ParameterNames.ExpenseAmount, entity.Amount);
            command.Parameters.AddWithValue(Constants.ParameterNames.ExpenseReimbursement, entity.Reimbursement);
            command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, entity.StartDate);
            command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, entity.EndDate);
        }

        private static void InitSingleId(SqlCommand command, IIdNameObject entity)
        {
            command.Parameters.AddWithValue(Constants.ParameterNames.ExpenseId, entity.Id.Value);
        }

        #endregion

        #region Custom data access

        public ProjectExpense[] GetForMilestone(ProjectExpense projectExpense)
        {
            return ExecuteReader(
                        projectExpense, 
                        Constants.ProcedureNames.ProjectExpenses.GetAllForMilestone,
                        GetForMilestoneInitializer);
        }

        public ProjectExpense[] GetForProject(ProjectExpense projectExpense)
        {
            return ExecuteReader(
                        projectExpense,
                        Constants.ProcedureNames.ProjectExpenses.GetAllForProject,
                        GetForProjectInitializer);
        }

        private static void GetForMilestoneInitializer(ProjectExpense projectExpense, SqlCommand sqlCommand)
        {
            sqlCommand.Parameters.AddWithValue(Constants.ParameterNames.MilestoneId, projectExpense.Milestone.Id);
        }

        private static void GetForProjectInitializer(ProjectExpense projectExpense, SqlCommand sqlCommand)
        {
            sqlCommand.Parameters.AddWithValue(Constants.ParameterNames.ProjectId, projectExpense.ProjectId);
        }

        #endregion

        #endregion
    }
}
