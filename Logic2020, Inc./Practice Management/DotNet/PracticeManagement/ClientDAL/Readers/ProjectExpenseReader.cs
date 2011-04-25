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
        private int _milestoneIdIndex;
        private int _milestoneNameIndex;

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
            _milestoneIdIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneId);
            _milestoneNameIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneName);
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
                           Milestone = new Milestone
                                           {
                                               Id = Reader.GetInt32(_milestoneIdIndex),
                                               Description = Reader.GetString(_milestoneNameIndex)
                                           }
                       };
        }
    }
}

