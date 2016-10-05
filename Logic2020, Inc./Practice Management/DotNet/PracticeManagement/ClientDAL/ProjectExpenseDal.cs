using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataAccess.Generic;
using DataAccess.Readers;
using DataTransferObjects;
using System.Collections.Generic;
using DataAccess.Other;
using System;

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

        #endregion Procedure names

        #region Base initializers

        protected override SqlParameter InitAddCommand(ProjectExpense entity, SqlCommand command)
        {
            InitPropertiesNoId(command, entity);
            command.Parameters.AddWithValue(Constants.ParameterNames.ProjectId, entity.ProjectId);
            command.Parameters.AddWithValue(Constants.ParameterNames.MilestoneId, entity.Milestone.Id);

            var outParam =
                new SqlParameter(Constants.ParameterNames.ExpenseId, SqlDbType.Int) { Direction = ParameterDirection.Output };

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

        #endregion Base initializers

        #region Derived initializers

        private static void InitPropertiesNoId(SqlCommand command, ProjectExpense entity)
        {
            command.Parameters.AddWithValue(Constants.ParameterNames.ExpenseName, entity.Name);
            command.Parameters.AddWithValue(Constants.ParameterNames.ExpenseAmount, entity.Amount);
            command.Parameters.AddWithValue(Constants.ParameterNames.ExpenseReimbursement, entity.Reimbursement);
            command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, entity.StartDate);
            command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, entity.EndDate);
            command.Parameters.AddWithValue(Constants.ParameterNames.ExpenseTypeId, entity.Type.Id);
            command.Parameters.AddWithValue(Constants.ParameterNames.ExpectedAmount, entity.ExpectedAmount);
        }

        private static void InitSingleId(SqlCommand command, IIdNameObject entity)
        {
            command.Parameters.AddWithValue(Constants.ParameterNames.ExpenseId, entity.Id.Value);
        }

        #endregion Derived initializers

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

        public static List<ExpenseType> GetAllExpenseTypesList()
        {
            var result = new List<ExpenseType>();
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.ProjectExpenses.GetAllExpenseTypes, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadExpenseTypes(reader, result);
                    return result;
                }
            }
        }

        private static void ReadExpenseTypes(SqlDataReader reader, List<ExpenseType> result)
        {
            try
            {
                int idIndex = reader.GetOrdinal(Constants.ColumnNames.Id);
                int nameIndex = reader.GetOrdinal(Constants.ColumnNames.Name);
                if (!reader.HasRows) return;
                while (reader.Read())
                {
                    var expenseType = new ExpenseType()
                    {
                        Id = reader.GetInt32(idIndex),
                        Name = reader.GetString(nameIndex),
                    };
                    result.Add(expenseType);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Custom data access

        #endregion Overrides of DalBase<ProjectExpense>
    }
}

