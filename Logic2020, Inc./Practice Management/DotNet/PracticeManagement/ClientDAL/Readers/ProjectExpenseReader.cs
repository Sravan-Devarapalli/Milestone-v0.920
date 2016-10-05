using System.Data;
using System.Data.Common;
using DataTransferObjects;

namespace DataAccess.Readers
{
    public class ProjectExpenseReader : EntityReaderBase<ProjectExpense>
    {
        private int _idIndex;
        private int _nameIndex;
        private int _amountIndex;
        private int _reimbIndex;
        private int _projectIndex;
        private int _startDateIndex;
        private int _endDateIndex;
        private int _milestoneNameIndex;
        private int _expenseTypeId;
        private int _expenseTypeName;
        private int _expectedAmount;

        public ProjectExpenseReader()
        {
        }

        public ProjectExpenseReader(DbDataReader reader)
            : base(reader)
        {
            InitIndexes(reader);
        }

        private void InitIndexes(IDataRecord reader)
        {
            _idIndex = reader.GetOrdinal(Constants.ColumnNames.ExpenseId);
            _nameIndex = reader.GetOrdinal(Constants.ColumnNames.ExpenseName);
            _amountIndex = reader.GetOrdinal(Constants.ColumnNames.ExpenseAmount);
            _reimbIndex = reader.GetOrdinal(Constants.ColumnNames.ExpenseReimbursement);
            _projectIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
            _startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
            _endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDate);
            _milestoneNameIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneName);
            _expenseTypeId = reader.GetOrdinal(Constants.ColumnNames.ExpenseTypeId);
            _expenseTypeName = reader.GetOrdinal(Constants.ColumnNames.ExpenseTypeName);
            _expectedAmount = reader.GetOrdinal(Constants.ColumnNames.ExpectedAmount);
        }

        public override void SetReader(DbDataReader value)
        {
            base.SetReader(value);

            InitIndexes(value);
        }

        public override ProjectExpense ReadEntity()
        {
            return new ProjectExpense
            {
                Id = Reader.GetInt32(_idIndex),
                Name = Reader.GetString(_nameIndex),
                Amount = Reader.GetDecimal(_amountIndex),
                ExpectedAmount=Reader.GetDecimal(_expectedAmount),
                Reimbursement = Reader.GetDecimal(_reimbIndex),
                ProjectId = Reader.GetInt32(_projectIndex),
                StartDate = Reader.GetDateTime(_startDateIndex),
                EndDate = Reader.GetDateTime(_endDateIndex),

                Milestone = new Milestone()
                {
                    Description = Reader.GetString(_milestoneNameIndex)
                },
                Type = new ExpenseType()
                {
                    Id = Reader.GetInt32(_expenseTypeId),
                    Name = Reader.GetString(_expenseTypeName)
                }
            };
        }
    }
}

