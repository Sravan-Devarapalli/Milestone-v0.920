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

        #region Properties

        public Dictionary<DateTime, String> dates { get; set; }
        public double total { get; set; }

        #endregion

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
            else if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var repResourceHoursPerDay = e.Item.FindControl(Repeater_ResourceHoursPerDay) as Repeater;

                var resourceHoursPerDay = new Dictionary<DateTime, double>();
                var groupedHours = ((PersonLevelGroupedHours)e.Item.DataItem).GroupedHoursList;

                foreach (var day in dates)
                {
                    resourceHoursPerDay.Add(day.Key, GetHours(groupedHours, day));
                }

                repResourceHoursPerDay.DataSource = resourceHoursPerDay;
                repResourceHoursPerDay.DataBind();
            }
        }

        private double GetHours(List<GroupedHours> groupedHours, KeyValuePair<DateTime, string> day)
        {
            double hours = 0;
            if (groupedHours.Any(d => d.StartDate.Date == day.Key.Date))
            {
                if (rbBillable.Checked)
                {
                    hours = groupedHours.Where(d => d.StartDate.Date == day.Key.Date).First().BillabileTotal;
                }
                else if (rbNonBillable.Checked)
                {
                    hours = groupedHours.Where(d => d.StartDate.Date == day.Key.Date).First().NonBillableTotal;
                }
                else
                {
                    hours = groupedHours.Where(d => d.StartDate.Date == day.Key.Date).First().CombinedTotal;
                }
            }

            return hours;
        }

        protected void repResourceHoursPerDay_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                total = 0;
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                total = total + ((KeyValuePair<DateTime, double>)e.Item.DataItem).Value;
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                var lblTotal = e.Item.FindControl("lblTotal") as Label;
                lblTotal.Text = total.ToString(Constants.Formatting.DoubleValue);
            }
        }

        public void DataBindResource(PersonLevelGroupedHours[] reportData, Dictionary<DateTime, String> datesList)
        {
            dates = datesList;
            repResource.DataSource = reportData;
            repResource.DataBind();
        }
    }
}
