using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Web.UI.HtmlControls;
using DataTransferObjects.TimeEntry;

namespace PraticeManagement.Controls.Reports
{
    public partial class ByworkType : System.Web.UI.UserControl
    {
        #region Constants

        private const string Repeater_WorkTypeHeaders = "repWorkTypeHeaders";
        private const string Repeater_WorkTypeHoursPerDay = "repWorkTypeHoursPerDay";

        #endregion

        #region Properties

        public Dictionary<DateTime, String> Dates { get; set; }


        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            extBillableNonBillableAndTotalExtender.ControlsToCheck = rbBillable.ClientID + ";" + rbCombined.ClientID + ";" + rbNonBillable.ClientID;
        }

        protected void repWorkType_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                var repResourceHeaders = e.Item.FindControl(Repeater_WorkTypeHeaders) as Repeater;
                repResourceHeaders.DataSource = Dates;
                repResourceHeaders.DataBind();
                extBillableNonBillableAndTotalExtender.TargetControlsToCheck = string.Empty;
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var repResourceHoursPerDay = e.Item.FindControl(Repeater_WorkTypeHoursPerDay) as Repeater;
                var tdPersonTotalHours = e.Item.FindControl("tdPersonTotalHours") as HtmlTableCell;
                extBillableNonBillableAndTotalExtender.TargetControlsToCheck += tdPersonTotalHours.ClientID + ";";
                var resourceHoursPerDay = new Dictionary<DateTime, GroupedHours>();
                var groupedHours = ((WorkTypeLevelGroupedHours)e.Item.DataItem).GroupedHoursList;

                foreach (var day in Dates)
                {
                    resourceHoursPerDay.Add(day.Key, GetHours(groupedHours, day));
                }

                repResourceHoursPerDay.DataSource = resourceHoursPerDay;
                repResourceHoursPerDay.DataBind();
            }
        }

        protected void repWorkTypeHoursPerDay_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var tdDayTotalHours = e.Item.FindControl("tdDayTotalHours") as HtmlTableCell;
                extBillableNonBillableAndTotalExtender.TargetControlsToCheck += tdDayTotalHours.ClientID + ";";
            }
        }


        private GroupedHours GetHours(List<GroupedHours> groupedHours, KeyValuePair<DateTime, string> day)
        {
            if (groupedHours.Any(d => d.StartDate.Date == day.Key.Date))
            {
                return groupedHours.First(gh => gh.StartDate.Date == day.Key.Date);
            }

            return new GroupedHours();
        }

       

        public void DataBindResource(WorkTypeLevelGroupedHours[] reportData, Dictionary<DateTime, String> datesList)
        {
            Dates = datesList;
            repWorkType.DataSource = reportData;
            repWorkType.DataBind();
        }
    }
}
