using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Utils;
using PraticeManagement.Controls.Generic.Filtering;
using DataTransferObjects.ContextObjects;

namespace PraticeManagement.Controls.Generic
{
    public enum OpportunityListFilterMode
    {
        GenericFilter,
        ByTargetPerson
    }

    public partial class OpportunityList : PracticeManagementFilterControl<OpportunitySortingContext>
    {
        private const string ViewStateSortOrder = "SortOrder";
        private const string ViewStateSortDirection = "SortDirection";
        private const string CssArrowClass = "arrow";

        public bool AllowAutoRedirectToDetails { get; set; }
        public OpportunityListFilterMode FilterMode { get; set; }
        public int? TargetPersonId { get; set; }

        protected string OrderBy
        {
            get
            {
                return GetViewStateValue<string>(ViewStateSortOrder, null);
            }
            set
            {
                SetViewStateValue(ViewStateSortOrder, value);
            }
        }

        protected SortDirection SortDirection
        {
            get
            {
                return GetViewStateValue(ViewStateSortDirection, SortDirection.Ascending);
            }
            set
            {
                SetViewStateValue(ViewStateSortDirection, value);
            }
        }

        public Opportunity[] GetOpportunities()
        {
            return
                FilterMode == OpportunityListFilterMode.GenericFilter ?
                DataHelper.GetFilteredOpportunities()
                :
                DataHelper.GetOpportunitiesForTargetPerson(TargetPersonId);
        }

        protected void lvOpportunities_Sorting(object sender, ListViewSortEventArgs e)
        {
            var newOrder = e.SortExpression;

            if (newOrder == OrderBy)
            {
                SortDirection =
                    SortDirection == SortDirection.Ascending ?
                        SortDirection.Descending : SortDirection.Ascending;
            }
            else
            {
                OrderBy = newOrder;
                SortDirection = SortDirection.Ascending;
            }

            SetHeaderIconsAccordingToSordOrder();
            FireFilterOptionsChanged();
        }

        protected override void ResetControls()
        {
            OrderBy = null;
            SortDirection = SortDirection.Ascending;

            SetHeaderIconsAccordingToSordOrder();
        }

        protected override void InitControls()
        {
            
            OrderBy = Filter.OrderBy;
            SortDirection = DataTransferObjects.Utils.Generic.ToEnum<SortDirection>(Filter.SortDirection, SortDirection.Ascending);

            SetHeaderIconsAccordingToSordOrder();
        }

        protected override OpportunitySortingContext InitFilter()
        {
            return new OpportunitySortingContext
                        {
                            SortDirection = SortDirection.ToString(),
                            OrderBy = OrderBy
                        };
        }

        private void SetHeaderIconsAccordingToSordOrder()
        {
            var row = lvOpportunities.FindControl("lvHeader") as System.Web.UI.HtmlControls.HtmlTableRow;

            if (row != null)
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    var cell = row.Cells[i];

                    if (cell.HasControls())
                    {
                        foreach (var ctrl in cell.Controls)
                        {
                            if (ctrl is LinkButton)
                            {
                                var lb = ctrl as LinkButton;
                                lb.CssClass = CssArrowClass;
                                if (lb.CommandArgument == OrderBy)
                                    lb.CssClass = GetCssClass();
                            }
                        }
                    }
                }
        }

        protected string GetCssClass()
        {
            return string.Format("{0} sort-{1}",
                                CssArrowClass,
                                SortDirection.ToString() == SortDirection.Ascending.ToString() ? "up" : "down");
        }

        public void DatabindOpportunities()
        {
            var opportunities = GetOpportunities();
            lvOpportunities.DataSource = opportunities;
            lvOpportunities.DataBind();

            //  IsPostBack here means that method is called on postback
            //      so it means that it's coming from search and we should redirect if there's the only result
            if (IsPostBack && lvOpportunities.Items.Count == 1 && AllowAutoRedirectToDetails)
            {
                var detailsLink =
                    Urls.OpportunityDetailsLink(opportunities[0].Id.Value);

                PraticeManagement.Utils.Generic.RedirectWithReturnTo(detailsLink, Request.Url.AbsoluteUri, Response);
            }
        }

        protected string GetOpportunityDetailsLink(int opportunityId, int index)
        {
            return Utils.Generic.GetTargetUrlWithReturn(Urls.OpportunityDetailsLink(opportunityId), Request.Url.AbsoluteUri);
        }

        protected string GetPersonDetailsLink(int personId, int index)
        {
            return Urls.GetPersonDetailsUrl(
                     new Person(personId),
                     Request.Url.AbsoluteUri);
        }

        protected static string GetDaysOld(DateTime date, bool IsCreateDate)
        {
            var span = DateTime.Now.Subtract(date);

            var days = span.Days;

            if (IsCreateDate)
            {
                return days > 0 ? string.Format("{0}", days) : "Current";
            }
            else
            {
                return days > 0 ? string.Format("{0} day{1}", days, days == 1 ? string.Empty : "s") : "Current";
            }
        }

        protected static string GetFormattedEstimatedRevenue(Decimal? estimatedRevenue)
        {
            return estimatedRevenue.GetFormattedEstimatedRevenue();
        }

        protected static string GetRevenueTypeCaption(RevenueType type)
        {
            if (type == RevenueType.Undefined || type == RevenueType.Unknown)
            {
                return Constants.Formatting.UnknownValue;
            }
            return type.ToString();
        }

        protected static bool IsNeedToShowPerson(Person person)
        {
            if (person == null)
            {
                return false;
            }

            if (person.Status.Name == PersonStatusType.Inactive.ToString()
                || person.Status.Name == PersonStatusType.Terminated.ToString())
            {
                return false;
            }

            return true;
        }

    }
}

