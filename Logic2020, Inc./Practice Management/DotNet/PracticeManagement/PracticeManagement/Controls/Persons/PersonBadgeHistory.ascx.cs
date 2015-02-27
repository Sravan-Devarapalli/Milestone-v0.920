using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PraticeManagement.Controls.Persons
{
    public partial class PersonBadgeHistory : System.Web.UI.UserControl
    {
        public bool IsBlockHistoryLoaded { get; set; }

        public bool IsOverrideHistoryLoaded { get; set; }

        public bool IsBadgeHistoryLoaded { get; set; }

        public string Width { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            //PopulateData();
        }

        private PraticeManagement.PersonDetail HostingPage
        {
            get { return ((PraticeManagement.PersonDetail)Page); }
        }

        public void PopulateData()
        {
            var history = ServiceCallers.Custom.Person(p => p.GetBadgeHistoryByPersonId(HostingPage.PersonId.Value));
            if (history == null || history.BlockHistory == null)
            {
                IsBlockHistoryLoaded = repBlockHistory.Visible = false;
                divBlockEmptyMessage.Style["display"] = "";
            }
            else
            {
                IsBlockHistoryLoaded = repBlockHistory.Visible = true;
                divBlockEmptyMessage.Style["display"] = "none";
                repBlockHistory.DataSource = history.BlockHistory;
                repBlockHistory.DataBind();
            }

            if (history == null || history.OverrideHistory == null)
            {
                IsOverrideHistoryLoaded=repOverrideHistory.Visible = false;
                divOverrideEmpty.Style["display"] = "";
            }
            else
            {
                IsOverrideHistoryLoaded=repOverrideHistory.Visible = true;
                divOverrideEmpty.Style["display"] = "none";
                repOverrideHistory.DataSource = history.OverrideHistory;
                repOverrideHistory.DataBind();
            }

            if (history == null || history.BadgeHistory == null)
            {
                IsBadgeHistoryLoaded= repBadgeHistory.Visible = false;
                divBadgeHistoryEmpty.Style["display"] = "";
            }
            else
            {
                IsBadgeHistoryLoaded=repBadgeHistory.Visible = true;
                divBadgeHistoryEmpty.Style["display"] = "none";
                repBadgeHistory.DataSource = history.BadgeHistory;
                repBadgeHistory.DataBind();
            }
            SetPopupWidth();
        }

        public void SetPopupWidth()
        {
            if (IsBlockHistoryLoaded || IsBadgeHistoryLoaded || IsOverrideHistoryLoaded)
                Width = "Width1300Px";
            else
                Width = "Width600Px";
        }

        protected string GetDateTimeFormat(DateTime date)
        {
            return date.ToString(Constants.Formatting.DateTimeFormat);
        }

        protected string GetDateFormat(DateTime date)
        {
            return date.ToString(Constants.Formatting.EntryDateFormat);
        }

        protected void repBlockHistory_DataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = (DataTransferObjects.MSBadge)e.Item.DataItem;
                var lblBlockStartDate = e.Item.FindControl("lblBlockStartDate") as Label;
                var lblBlockEndDate = e.Item.FindControl("lblBlockEndDate") as Label;
                lblBlockStartDate.Text = dataItem.BlockStartDate.HasValue ? GetDateFormat(dataItem.BlockStartDate.Value) : "---";
                lblBlockEndDate.Text = dataItem.BlockEndDate.HasValue ? GetDateFormat(dataItem.BlockEndDate.Value) : "---";
            }
        }

        protected void repOverrideHistory_DataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = (DataTransferObjects.MSBadge)e.Item.DataItem;
                var lblOverrideStartDate = e.Item.FindControl("lblOverrideStartDate") as Label;
                var lblOverrideEndDate = e.Item.FindControl("lblOverrideEndDate") as Label;
                lblOverrideStartDate.Text = dataItem.ExceptionStartDate.HasValue ? GetDateFormat(dataItem.ExceptionStartDate.Value) : "---";
                lblOverrideEndDate.Text = dataItem.ExceptionEndDate.HasValue ? GetDateFormat(dataItem.ExceptionEndDate.Value) : "---";
            }
        }

        protected void repBadgeHistory_DataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = (DataTransferObjects.MSBadge)e.Item.DataItem;
                var lblBadgeStartDate = e.Item.FindControl("lblBadgeStartDate") as Label;
                var lblBadgeStartDateSource = e.Item.FindControl("lblBadgeStartDateSource") as Label;
                var lblProjectPlannedEndDate = e.Item.FindControl("lblProjectPlannedEndDate") as Label;
                var lblProjectPlannedEndDateSource = e.Item.FindControl("lblProjectPlannedEndDateSource") as Label;
                var lblBadgeEndDate = e.Item.FindControl("lblBadgeEndDate") as Label;
                var lblBadgeEndDateSource = e.Item.FindControl("lblBadgeEndDateSource") as Label;
                var lblBreakStartDate = e.Item.FindControl("lblBreakStartDate") as Label;
                var lblBreakEndDate = e.Item.FindControl("lblBreakEndDate") as Label;

                lblBadgeStartDate.Text = dataItem.BadgeStartDate.HasValue ? GetDateFormat(dataItem.BadgeStartDate.Value) : "---";
                lblProjectPlannedEndDate.Text = dataItem.PlannedEndDate.HasValue ? GetDateFormat(dataItem.PlannedEndDate.Value) : "---";
                lblBadgeEndDate.Text = dataItem.BadgeEndDate.HasValue ? GetDateFormat(dataItem.BadgeEndDate.Value) : "---";
                lblBreakStartDate.Text = dataItem.BreakStartDate.HasValue ? GetDateFormat(dataItem.BreakStartDate.Value) : "---";
                lblBreakEndDate.Text = dataItem.BreakEndDate.HasValue ? GetDateFormat(dataItem.BreakEndDate.Value) : "---";

                lblBadgeStartDateSource.Text = string.IsNullOrEmpty(dataItem.BadgeStartDateSource) ? "---" : dataItem.BadgeStartDateSource;
                lblProjectPlannedEndDateSource.Text = string.IsNullOrEmpty(dataItem.PlannedEndDateSource) ? "---" : dataItem.PlannedEndDateSource;
                lblBadgeEndDateSource.Text = string.IsNullOrEmpty(dataItem.BadgeEndDateSource) ? "---" : dataItem.BadgeEndDateSource;
            }
        }
    }
}
