using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using DataTransferObjects.Reports;

namespace PraticeManagement.Controls.Reports.ByPerson
{
    public partial class GroupByProject : System.Web.UI.UserControl
    {
        private int SectionId
        {
            get;
            set;
        }

        public void DatabindRepepeaterPersonDetails(List<TimeEntriesGroupByClientAndProject> timeEntriesGroupByClientAndProjectList, string name)
        {
            lblPerson.Text = name;
            lblTotalHours.Text = GetDoubleFormat(timeEntriesGroupByClientAndProjectList.Sum(p => p.TotalHours));
            repProjects.Visible = true;
            repProjects.DataSource = timeEntriesGroupByClientAndProjectList;
            repProjects.DataBind();
        }

        protected string GetDateFormat(DateTime date)
        {
            return date.ToString(Constants.Formatting.ReportDateFormat);
        }

        protected bool GetNonBillableImageVisibility(int sectionId, double nonBillableHours)
        {
            return sectionId == -1 ? SectionId == 1 && nonBillableHours > 0 : sectionId == 1 && nonBillableHours > 0;
        }

        protected string GetProjectStatus(string status)
        {
            return string.IsNullOrEmpty(status) ? "" : "(" + status + ")";
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected bool GetNoteVisibility(String note)
        {
            if (String.IsNullOrEmpty(note))
            {
                return false;
            }
            return true;

        }

        protected void repProjects_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {


            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var repDate = e.Item.FindControl("repDate") as Repeater;
                TimeEntriesGroupByClientAndProject dataitem = (TimeEntriesGroupByClientAndProject)e.Item.DataItem;
                SectionId = dataitem.Project.TimeEntrySectionId;

                repDate.DataSource = dataitem.DayTotalHours;
                repDate.DataBind();
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
            }
        }

        protected void repDate_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {

            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var repWorktype = e.Item.FindControl("repWorktype") as Repeater;
                TimeEntriesGroupByDate dataitem = (TimeEntriesGroupByDate)e.Item.DataItem;
                repWorktype.DataSource = dataitem.DayTotalHoursList;
                var rep = sender as Repeater;
                repWorktype.DataBind();
            }
        }

    }
}
