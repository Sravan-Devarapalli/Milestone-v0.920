using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Web.UI.HtmlControls;

namespace PraticeManagement.Controls.Reports
{
    public partial class TimePeriodSummaryByProject : System.Web.UI.UserControl
    {
         #region Constants

        private const string Repeater_ProjectHeaders = "repProjectHeaders";
        private const string Repeater_ProjectHoursPerDay = "reProjectHoursPerDay";

        #endregion

        public Dictionary<DateTime, String> Dates { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            extBillableNonBillableAndTotalExtender.ControlsToCheck = rbBillable.ClientID + ";" + rbCombined.ClientID + ";" + rbNonBillable.ClientID;
        }

        protected void repProject_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                var repProjectHeaders = e.Item.FindControl(Repeater_ProjectHeaders) as Repeater;
                repProjectHeaders.DataSource = Dates;
                repProjectHeaders.DataBind();
                extBillableNonBillableAndTotalExtender.TargetControlsToCheck = string.Empty;
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var repProjectHoursPerDay = e.Item.FindControl(Repeater_ProjectHoursPerDay) as Repeater;
                var tdProjectTotalHours = e.Item.FindControl("tdProjectTotalHours") as HtmlTableCell;
                extBillableNonBillableAndTotalExtender.TargetControlsToCheck += tdProjectTotalHours.ClientID + ";";

                var projectHoursPerDay = new Dictionary<DateTime, GroupedHours>();
                var groupedHours = ((ProjectLevelGroupedHours)e.Item.DataItem).GroupedHoursList;

                foreach (var day in Dates)
                {
                    projectHoursPerDay.Add(day.Key, GetHours(groupedHours, day));
                }

                repProjectHoursPerDay.DataSource = projectHoursPerDay;
                repProjectHoursPerDay.DataBind();
            }
        }

        protected void reProjectHoursPerDay_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var tdDayTotalHours = e.Item.FindControl("tdDayTotalHours") as HtmlTableCell;
                extBillableNonBillableAndTotalExtender.TargetControlsToCheck += tdDayTotalHours.ClientID + ";";
            }
        }


        public void DataBindProject(ProjectLevelGroupedHours[] reportData, Dictionary<DateTime, String> datesList)
        {
            Dates = datesList;
            repProject.DataSource = reportData;
            repProject.DataBind();
        }

        private GroupedHours GetHours(List<GroupedHours> groupedHours, KeyValuePair<DateTime, string> day)
        {
            if (groupedHours.Any(d => d.StartDate.Date == day.Key.Date))
            {
                return groupedHours.First(gh => gh.StartDate.Date == day.Key.Date);
            }

            return new GroupedHours();
        }
    }
}
