using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{
    [DataContract]
    [Serializable]
    public class ExpenseSummary
    {
        [DataMember]
        public Project Project
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<DateTime, decimal> MonthlyExpenses
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<DateTime, decimal> MonthlyExpectedExpenses
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<DateTime, decimal> MonthlyReimburseExpense
        {
            get;
            set;
        }

        [DataMember]
        public ExpenseType Type
        {
            get;
            set;
        }

        [DataMember]
        public ProjectExpense Expense
        {
            get;
            set;
        }

        [DataMember]
        public DateTime MonthStartDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime MonthEndDate
        {
            get;
            set;
        }

        public decimal TotalExpense
        {
            get
            {
                if (MonthlyExpenses != null)
                {
                    return MonthlyExpenses.Sum(e => e.Value);
                }
                return 0;
            }
        }

        public decimal TotalExpectedAmount
        {
            get
            {
                if (MonthlyExpectedExpenses != null)
                {
                    return MonthlyExpectedExpenses.Sum(e => e.Value);
                }
                return 0;
            }
        }

    }
}

