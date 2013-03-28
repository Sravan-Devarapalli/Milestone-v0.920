using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using DataTransferObjects.Reports.ConsultingDemand;

namespace PraticeManagement.Controls.Reports.ConsultantDemand
{
    public partial class ConsultingDemandDetails : System.Web.UI.UserControl
    {
        private PraticeManagement.Reports.ConsultingDemand_New HostingPage
        {
            get { return ((PraticeManagement.Reports.ConsultingDemand_New)Page); }
        }

        public int GrandTotal = 0;

        public HiddenField _hdTitle
        {
            get
            {
                return hdTitle;
            }
        }

        public HiddenField _hdSkill
        {
            get
            {
                return hdSkill;
            }
        }

        public HiddenField _hdIsSummaryPage
        {
            get
            {
                return hdIsSummaryPage;
            }
        }

        public string groupBy
        {
            get
            {
                return hdnGroupBy.Value;
            }

            set
            {
                hdnGroupBy.Value = value;
                if (value == "month")
                {
                    btnGroupBy.ToolTip = btnGroupBy.Text = "Group By Title";
                }
                else
                {
                    btnGroupBy.ToolTip = btnGroupBy.Text = "Group By Month";
                }
                
            }
        }

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
            if (!IsPostBack)
            {
                hdnGroupBy.Value = "title";
            }
        }

        protected void btnGroupBy_OnClick(object sender, EventArgs e)
        {
            if (groupBy == "month")
            {
                hdnGroupBy.Value = "title";
            }
            else if (groupBy == "title")
            {
                hdnGroupBy.Value = "month";
            }
            PopulateData();

            if (!string.IsNullOrEmpty(_hdIsSummaryPage.Value) && _hdIsSummaryPage.Value == true.ToString())
            {
                var hostingPage = (PraticeManagement.Reports.ConsultingDemand_New)Page;
                hostingPage.SummaryControl.ConsultantDetailPopup.Show();
            }

        }

        protected void repByTitleSkill_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Header)
            {
                CollapsiblePanelDateExtenderClientIds = new List<string>();
            }

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater repDetails = (Repeater)e.Item.FindControl("repDetails");
                ConsultantGroupbyTitleSkill dataitem = (ConsultantGroupbyTitleSkill)e.Item.DataItem;
                var result = dataitem.ConsultantDetails;
                repDetails.DataSource = result;
                var cpeDetails = e.Item.FindControl("cpeDetails") as CollapsiblePanelExtender;
                cpeDetails.BehaviorID = Guid.NewGuid().ToString();
                CollapsiblePanelDateExtenderClientIds.Add(cpeDetails.BehaviorID);
                repDetails.DataBind();
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var output = jss.Serialize(CollapsiblePanelDateExtenderClientIds);
                hdncpeExtendersIds.Value = output;
                btnExpandOrCollapseAll.Text = btnExpandOrCollapseAll.ToolTip = "Expand All";
                hdnCollapsed.Value = "true";
            }
        }

        protected void repByMonth_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                CollapsiblePanelDateExtenderClientIds = new List<string>();
            }

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater repDetails = (Repeater)e.Item.FindControl("repDetails");
                ConsultantGroupByMonth dataitem = (ConsultantGroupByMonth)e.Item.DataItem;
                var result = dataitem.ConsultantDetailsByMonth;
                repDetails.DataSource = result;
                var cpeDetails = e.Item.FindControl("cpeDetails") as CollapsiblePanelExtender;
                cpeDetails.BehaviorID = Guid.NewGuid().ToString();
                CollapsiblePanelDateExtenderClientIds.Add(cpeDetails.BehaviorID);
                repDetails.DataBind();
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var output = jss.Serialize(CollapsiblePanelDateExtenderClientIds);
                hdncpeExtendersIds.Value = output;
                btnExpandOrCollapseAll.Text = btnExpandOrCollapseAll.ToolTip = "Expand All";
                hdnCollapsed.Value = "true";
            }
        }

        public void PopulateData()
        {
            string title = hdTitle.Value;
            string skill = hdSkill.Value;
            string groupby = hdnGroupBy.Value;
            if (groupby == "title")
            {
                repByMonth.Visible = false;
                repByTitleSkill.Visible = true;
                List<ConsultantGroupbyTitleSkill> data = ServiceCallers.Custom.Report(r => r.ConsultingDemandDetailsByTitleSkill(HostingPage.StartDate.Value, HostingPage.EndDate.Value, title, skill)).ToList();
                GrandTotal = data.Sum(p => p.TotalCount);
                repByTitleSkill.DataSource = data;
                repByTitleSkill.DataBind();
            }
            else if (groupby == "month")
            {
                repByTitleSkill.Visible = false;
                repByMonth.Visible = true;
                List<ConsultantGroupByMonth> data = ServiceCallers.Custom.Report(r => r.ConsultingDemandDetailsByMonth(HostingPage.StartDate.Value, HostingPage.EndDate.Value, title, skill)).ToList();
                GrandTotal = data.Sum(p => p.TotalCount);
                repByMonth.DataSource = data;
                repByMonth.DataBind();
            }
        }
    }
}
