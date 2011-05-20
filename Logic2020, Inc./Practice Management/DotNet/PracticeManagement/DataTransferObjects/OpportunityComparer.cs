using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Runtime.Serialization;
using DataTransferObjects.ContextObjects;

namespace DataTransferObjects
{
    [DataContract]
    public enum OpportunitySortOrder
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        Priority = 1,
        [EnumMember]
        StatusName = 2,
        [EnumMember]
        OpportunityName = 3,
        [EnumMember]
        ClientName = 4,
        [EnumMember]
        StartDate = 5,
        [EnumMember]
        BuyerName = 6,
        [EnumMember]
        Salesperson = 7,
        [EnumMember]
        CreateDate = 8,
        [EnumMember]
        OpportunityIndex = 9,
        [EnumMember]
        RevenueType = 10,
        [EnumMember]
        Updated = 11,
        [EnumMember]
        Number = 12,
        [EnumMember]
        Owner = 13,
        [EnumMember]
        EstimatedRevenue = 14,
    }

    /// <summary>
    /// Uses for to sort a list of the <see cref="Opportunity"/> objects.
    /// </summary>
    public class OpportunityComparer : IComparer<Opportunity>
    {
        #region Constants

        public const string PersonNameFormat = "{0}, {1}";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a sort order for the comparer.
        /// </summary>
        public OpportunitySortOrder SortOrder
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a sort direction for the comparer.
        /// </summary>
        public SortDirection SortDirection
        {
            get;
            set;
        }

        #endregion

        #region Construction

        public OpportunityComparer(OpportunitySortingContext context)
        {
            this.SortOrder = Utils.Generic.ToEnum<OpportunitySortOrder>(context.OrderBy, OpportunitySortOrder.None);
            this.SortDirection = Utils.Generic.ToEnum<SortDirection>(context.SortDirection, SortDirection.Ascending);
        }

        #endregion

        #region IComparer<Opportunity> Members

        /// <summary>
        /// Compares two <see cref="Opportunity"/> objects.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(Opportunity x, Opportunity y)
        {
            int result;
            switch (SortOrder)
            {
                case OpportunitySortOrder.None:
                    result = 0;
                    break;
                case OpportunitySortOrder.Priority:

                    result = (x.Priority != null ? x.Priority.SortOrder : 0).CompareTo(
                        (y.Priority != null ? y.Priority.SortOrder : 0));
                    break;
                case OpportunitySortOrder.OpportunityName:
                    result = string.Compare(x.Name, y.Name);
                    break;
                case OpportunitySortOrder.ClientName:
                    result =
                        string.Compare(
                        x.Client != null ? x.Client.Name : string.Empty,
                        y.Client != null ? y.Client.Name : string.Empty);
                    break;
                case OpportunitySortOrder.StatusName:
                    result =
                        string.Compare(
                        x.Status != null ? x.Status.Name : string.Empty,
                        y.Status != null ? y.Status.Name : string.Empty);
                    break;
                case OpportunitySortOrder.StartDate:
                    result =
                        DateTime.Compare(
                        x.ProjectedStartDate.HasValue ? x.ProjectedStartDate.Value : DateTime.MinValue,
                        y.ProjectedStartDate.HasValue ? y.ProjectedStartDate.Value : DateTime.MinValue);
                    break;
                case OpportunitySortOrder.BuyerName:
                    result = string.Compare(x.BuyerName, y.BuyerName);
                    break;
                case OpportunitySortOrder.Salesperson:
                    string salespersonX =
                        x.Salesperson != null ?
                        string.Format(PersonNameFormat, x.Salesperson.LastName, x.Salesperson.FirstName) : string.Empty;
                    string salespersonY =
                        y.Salesperson != null ?
                        string.Format(PersonNameFormat, y.Salesperson.LastName, y.Salesperson.FirstName) : string.Empty;
                    result = string.Compare(salespersonX, salespersonY);
                    break;
                case OpportunitySortOrder.CreateDate:
                    result = x.DaysOld.CompareTo(y.DaysOld);
                    break;

                case OpportunitySortOrder.OpportunityIndex:
                    result = Nullable.Compare(x.OpportunityIndex, y.OpportunityIndex);
                    break;

                case OpportunitySortOrder.RevenueType:
                    result = x.OpportunityRevenueType.CompareTo(y.OpportunityRevenueType);
                    break;

                case OpportunitySortOrder.Updated:
                    result = x.LastChange.CompareTo(y.LastChange);
                    break;

                case OpportunitySortOrder.Number:
                    result = x.OpportunityNumber.CompareTo(y.OpportunityNumber);
                    break;
                case OpportunitySortOrder.Owner:
                    string ownerX =
                        x.Owner != null ?
                        x.Owner.LastName : string.Empty;
                    string ownerY =
                        y.Owner != null ?
                        y.Owner.LastName : string.Empty;
                    result = string.Compare(ownerX, ownerY);
                    break;
                case OpportunitySortOrder.EstimatedRevenue:
                    result = Nullable.Compare(x.EstimatedRevenue, y.EstimatedRevenue);
                    break;

                default:
                    throw new NotImplementedException();
            }

            if (result == 0)
            {
                result = Nullable.Compare<int>(x.Id, y.Id);
            }

            return SortDirection == SortDirection.Ascending ? result : -result;
        }

        #endregion
    }
}

