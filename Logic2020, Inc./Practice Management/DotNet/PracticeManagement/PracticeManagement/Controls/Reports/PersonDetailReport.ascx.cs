using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using AjaxControlToolkit;
using System.Web.UI.HtmlControls;
using System.Web.Script.Serialization;
namespace PraticeManagement.Controls.Reports
{
    public partial class PersonDetailReport : System.Web.UI.UserControl
    {

        private List<KeyValuePair<string, string>> CollapsiblePanelExtenderClientIds
        {
            get;
            set;
        }

        private List<string> CollapsiblePanelDateExtenderClientIds
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            btnExpandOrCollapseAll.Attributes["onclick"] = "return CollapseOrExpandAll(" + btnExpandOrCollapseAll.ClientID +
                                                            ", " + hdnCollapsed.ClientID +
                                                            ", " + hdncpeExtendersIds.ClientID +
                                                            ");";

            btnExpandOrCollapseAll.Text = btnExpandOrCollapseAll.ToolTip = (hdnCollapsed.Value.ToLower() == "true") ? "Expand All" : "Collapse All";

        }

        public void DatabindRepepeaterProjectDetails(List<TimeEntriesGroupByClientAndProject> timeEntriesGroupByClientAndProjectList)
        {
            repProjects.DataSource = timeEntriesGroupByClientAndProjectList;
            repProjects.DataBind();
        }

        protected void repProjects_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                CollapsiblePanelExtenderClientIds = new List<KeyValuePair<string, string>>();

            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var repDate = e.Item.FindControl("repDate") as Repeater;
                TimeEntriesGroupByClientAndProject dataitem = (TimeEntriesGroupByClientAndProject)e.Item.DataItem;
                var cpeProject = e.Item.FindControl("cpeProject") as CollapsiblePanelExtender;
                cpeProject.BehaviorID = cpeProject.ClientID + e.Item.ItemIndex.ToString();

                repDate.DataSource = dataitem.DayTotalHours;
                repDate.DataBind();

                JavaScriptSerializer jss = new JavaScriptSerializer();
                var output = jss.Serialize(CollapsiblePanelDateExtenderClientIds);

                KeyValuePair<string, string> kvPair = new KeyValuePair<string, string>(cpeProject.BehaviorID, output);
                CollapsiblePanelExtenderClientIds.Add(kvPair);

            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var output = jss.Serialize(CollapsiblePanelExtenderClientIds);

                hdncpeExtendersIds.Value = output;
                btnExpandOrCollapseAll.Text = btnExpandOrCollapseAll.ToolTip = "Collapse All";
                hdnCollapsed.Value = "false";
            }
        }


       



        protected void repDate_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                CollapsiblePanelDateExtenderClientIds = new List<string>();
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var repWorktype = e.Item.FindControl("repWorktype") as Repeater;
                TimeEntriesGroupByDate dataitem = (TimeEntriesGroupByDate)e.Item.DataItem;
                var rep = sender as Repeater;

                var cpeDate = e.Item.FindControl("cpeDate") as CollapsiblePanelExtender;
                cpeDate.BehaviorID = Guid.NewGuid().ToString();
                CollapsiblePanelDateExtenderClientIds.Add(cpeDate.BehaviorID);

                repWorktype.DataSource = dataitem.DayTotalHoursList;
                repWorktype.DataBind();
            }
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

    }
}

