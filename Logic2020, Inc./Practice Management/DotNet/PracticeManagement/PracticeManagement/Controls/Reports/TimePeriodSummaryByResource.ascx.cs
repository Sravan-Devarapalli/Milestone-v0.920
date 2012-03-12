using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;

namespace PraticeManagement.Controls.Reports
{
    public partial class TimePeriodSummaryByResource : System.Web.UI.UserControl
    {
        #region Constants

        private const string Repeater_ResourceHeaders = "repResourceHeaders";
        private const string Repeater_ResourceHoursPerDay = "repResourceHoursPerDay";

        #endregion

        public List<DateTime> dates { get; set; }
        
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void repResource_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                var repResourceHeaders = e.Item.FindControl(Repeater_ResourceHeaders) as Repeater;
                repResourceHeaders.DataSource = dates;
                repResourceHeaders.DataBind();
            }
            else if(e.Item.ItemType == ListItemType.Item)
            {
                var repResourceHoursPerDay = e.Item.FindControl(Repeater_ResourceHoursPerDay) as Repeater;

                var resourceHoursPerDay = new Dictionary<DateTime, string>();
                var groupedHours = ((PersonLevelGroupedHours)e.Item.DataItem).GroupedHoursList;

                foreach (var day in dates)
                {
                    resourceHoursPerDay.Add(day.Date, groupedHours.Any(d => d.StartDate == day.Date) ? groupedHours.Where(d => d.StartDate == day.Date).First().BillabileTotal.ToString() : string.Empty);
                }

                repResourceHoursPerDay.DataSource = resourceHoursPerDay;
                repResourceHoursPerDay.DataBind();
            }
        }

        protected void repResourceHoursPerDay_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
 
        }

        public void DataBindResource(PersonLevelGroupedHours[] reportData, List<DateTime> datesList)
        {
            dates = datesList;
            repResource.DataSource = reportData;
            repResource.DataBind();
        }
    }
}
