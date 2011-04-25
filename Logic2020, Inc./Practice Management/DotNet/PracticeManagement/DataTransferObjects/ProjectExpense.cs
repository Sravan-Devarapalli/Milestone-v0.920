using System.Runtime.Serialization;

namespace DataTransferObjects
{
    /// <summary>
    /// 
    /// </summary>
    public class ProjectExpense : IIdNameObject
    {
        [DataMember]
        public int? Id { get; set; }

        /// <summary>
        /// Expense name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Expense $
        /// </summary>
        [DataMember]
        public decimal Amount { get; set; }

        /// <summary>
        /// Reimbursement %
        /// </summary>
        [DataMember]
        public decimal Reimbursement { get; set; }

        /// <summary>
        /// Milestone
        /// </summary>
        [DataMember]
        public Milestone Milestone { get; set; }

        /// <summary>
        /// Reimbursed $
        /// </summary>
        public decimal ReimbursementAmount
        {
            get
            {
                return 0.01M*Amount*Reimbursement;
            }
        }

    }
}

