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

        public ProjectExpenseReader() : base()
        {
            
        }

        public ProjectExpenseReader(DbDataReader reader) : base(reader)
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
                           Reimbursement = Reader.GetDecimal(_reimbIndex),
                           ProjectId = Reader.GetInt32(_projectIndex),
                           StartDate = Reader.GetDateTime(_startDateIndex),
                           EndDate = Reader.GetDateTime(_endDateIndex)
                       };
        }
    }
}

