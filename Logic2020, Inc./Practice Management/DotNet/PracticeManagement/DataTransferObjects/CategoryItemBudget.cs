using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class CategoryItemBudget
    {
        [DataMember]
        public int ItemId
        {
            get;
            set;
        }
        [DataMember]
        public BudgetCategoryType CategoryTypeId
        {
            get;
            set;
        }

        [DataMember]
        public int Month
        {
            get;
            set;
        }

        [DataMember]
        public decimal Amount
        {
            get;
            set;
        }

    }
}

