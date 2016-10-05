using System;
using System.Runtime.Serialization;
using System.Web;

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

        public string HtmlEncodedName
        {
            get
            {
                return HttpUtility.HtmlEncode(Name);
            }
        }

        [DataMember]
        public int ProjectId { get; set; }

        /// <summary>
        /// Expense Start Date
        /// </summary>
        [DataMember]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Expense End Date
        /// </summary>
        [DataMember]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Expense $
        /// </summary>
        [DataMember]
        public decimal Amount { get; set; }

        /// <summary>
        /// ExpectedAmount $
        /// </summary>
        [DataMember]
        public decimal ExpectedAmount { get; set; }

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

        [DataMember]
        public string MilestoneName { get; set; }
        /// <summary>
        /// Reimbursed $
        /// </summary>
        public decimal ReimbursementAmount
        {
            get
            {
                return 0.01M * Amount * Reimbursement;
            }
        }

        public decimal EstimatedReimbursementAmount
        {
            get
            {
                return 0.01M * ExpectedAmount * Reimbursement;
            }
        }

        public decimal Difference
        {
            get
            {
                return ExpectedAmount - Amount;
            }
        }

        [DataMember]
        public ExpenseType Type
        {
            get;
            set;
        }

    }
}

