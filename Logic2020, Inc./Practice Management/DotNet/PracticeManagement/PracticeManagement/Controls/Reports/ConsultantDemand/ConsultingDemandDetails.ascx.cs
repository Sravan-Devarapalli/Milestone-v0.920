using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI.HtmlControls;
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

        private string ShowPanel = "ShowPanel('{0}', '{1}','{2}');";
        private string HidePanel = "HidePanel('{0}');";
        private string OnMouseOver = "onmouseover";
        private string OnMouseOut = "onmouseout";
        private string sortColumn_Key = "sortColumn_Key";
        private string sortOrder_Key = "sortOrder_Key";

        public int GrandTotal = 0;

        public bool sortAscend
        {
            get
            {
                if (ViewState[sortOrder_Key] == null)
                {
                    ViewState[sortOrder_Key] = true;
                }
                return (bool)ViewState[sortOrder_Key];
            }
            set
            {
                ViewState[sortOrder_Key] = value;
            }
        }

        public string sortColumn
        {
            get
            {
                if (ViewState[sortColumn_Key] == null)
                {
                    ViewState[sortColumn_Key] = "";
                }
                return (string)ViewState[sortColumn_Key];
            }
            set
            {
                ViewState[sortColumn_Key] = value;
            }
        }

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

        private List<CollapsiblePanelExtender> CollapsiblePanelDateExtenderList
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

        protected void btnTitleSkill_Command(object sender, CommandEventArgs e)
        {
            sortColumn = e.CommandArgument.ToString();
            PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : "");
            sortAscend = !sortAscend;
        }

        protected void btnOpportunityNumber_Command(object sender, CommandEventArgs e)
        {
            sortColumn = e.CommandArgument.ToString();
            PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.OpportunityNumber + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.OpportunityNumber + "," + Constants.ConsultingDemandSortColumnNames.Title + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            sortAscend = !sortAscend;
        }

        protected void btnProjectNumber_Command(object sender, CommandEventArgs e)
        {
            sortColumn = e.CommandArgument.ToString();
            PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.ProjectNumber + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.ProjectNumber + "," + Constants.ConsultingDemandSortColumnNames.Title + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            sortAscend = !sortAscend;
        }

        protected void btnAccountName_Command(object sender, CommandEventArgs e)
        {
            sortColumn = e.CommandArgument.ToString();
            PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.AccountName + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.AccountName + "," + Constants.ConsultingDemandSortColumnNames.Title + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            sortAscend = !sortAscend;
        }

        protected void btnProjectName_Command(object sender, CommandEventArgs e)
        {
            sortColumn = e.CommandArgument.ToString();
            PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.ProjectName + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.ProjectName + "," + Constants.ConsultingDemandSortColumnNames.Title + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            sortAscend = !sortAscend;
        }

        protected void btnResourceStartDate_Command(object sender, CommandEventArgs e)
        {
            sortColumn = e.CommandArgument.ToString();
            PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.ResourceStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.ResourceStartDate + "," + Constants.ConsultingDemandSortColumnNames.Title + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            sortAscend = !sortAscend;
        }

        protected void btnTotal_Command(object sender, CommandEventArgs e)
        {
            sortColumn = e.CommandArgument.ToString();
            PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.Count + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.Count + "," + Constants.ConsultingDemandSortColumnNames.Title + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            sortAscend = !sortAscend;
        }

        protected void btnMonthYear_Command(object sender, CommandEventArgs e)
        {
            sortColumn = e.CommandArgument.ToString();
            PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.MonthStartDate + "," + Constants.ConsultingDemandSortColumnNames.Title + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            sortAscend = !sortAscend;
        }

        protected void btnMonthTitleSkill_Command(object sender, CommandEventArgs e)
        {
            sortColumn = e.CommandArgument.ToString();
            PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.Title + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            sortAscend = !sortAscend;
        }

        protected void btnMonthOpportunityNumber_Command(object sender, CommandEventArgs e)
        {
            sortColumn = e.CommandArgument.ToString();
            PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.OpportunityNumber + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.OpportunityNumber + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + "," + Constants.ConsultingDemandSortColumnNames.Title + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            sortAscend = !sortAscend;
        }

        protected void btnMonthProjectNumber_Command(object sender, CommandEventArgs e)
        {
            sortColumn = e.CommandArgument.ToString();
            PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.ProjectNumber + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.ProjectNumber + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + "," + Constants.ConsultingDemandSortColumnNames.Title + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            sortAscend = !sortAscend;
        }

        protected void btnMonthAccountName_Command(object sender, CommandEventArgs e)
        {
            sortColumn = e.CommandArgument.ToString();
            PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.AccountName + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.AccountName + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + "," + Constants.ConsultingDemandSortColumnNames.Title + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            sortAscend = !sortAscend;
        }

        protected void btnMonthProjectName_Command(object sender, CommandEventArgs e)
        {
            sortColumn = e.CommandArgument.ToString();
            PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.ProjectName + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.ProjectName + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + "," + Constants.ConsultingDemandSortColumnNames.Title + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            sortAscend = !sortAscend;
        }

        protected void btnMonthResourceStartDate_Command(object sender, CommandEventArgs e)
        {
            sortColumn = e.CommandArgument.ToString();
            PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.ResourceStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.ResourceStartDate + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + "," + Constants.ConsultingDemandSortColumnNames.Title + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            sortAscend = !sortAscend;
        }

        protected void btnMonthTotal_Command(object sender, CommandEventArgs e)
        {
            sortColumn = e.CommandArgument.ToString();
            PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.Count + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.Count + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + "," + Constants.ConsultingDemandSortColumnNames.Title + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            sortAscend = !sortAscend;
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
            PopulateData(true, "");
            groupBy = hdnGroupBy.Value;
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
                CollapsiblePanelDateExtenderList = new List<CollapsiblePanelExtender>();
                var lblTotal = (Label)e.Item.FindControl("lblTotal");
                var pnlTotal = (Panel)e.Item.FindControl("pnlTotal");
                var lblTotalForecastedDemand = (Label)e.Item.FindControl("lblTotalForecastedDemand");
                PopulateHeaderHoverLabels(lblTotal, pnlTotal, lblTotalForecastedDemand, GrandTotal, 80);
            }

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater repDetails = (Repeater)e.Item.FindControl("repTitlesDetails");
                ConsultantGroupbyTitleSkill dataitem = (ConsultantGroupbyTitleSkill)e.Item.DataItem;
                var result = dataitem.ConsultantDetails;
                repDetails.DataSource = result;
                var cpeDetails = e.Item.FindControl("cpeDetails") as CollapsiblePanelExtender;
                cpeDetails.BehaviorID = Guid.NewGuid().ToString();
                CollapsiblePanelDateExtenderClientIds.Add(cpeDetails.BehaviorID);
                CollapsiblePanelDateExtenderList.Add(cpeDetails);
                repDetails.DataBind();
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var output = jss.Serialize(CollapsiblePanelDateExtenderClientIds);
                hdncpeExtendersIds.Value = output;
                //btnExpandOrCollapseAll.Text = btnExpandOrCollapseAll.ToolTip = "Expand All";
                //hdnCollapsed.Value = "true";
            }
        }

        protected void repTitlesDetails_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HyperLink hypOpportunity = (HyperLink)e.Item.FindControl("hlOpportunityNumber");
                HyperLink hypProject = (HyperLink)e.Item.FindControl("hlProjectNumber");

                ConsultantDemandDetails item = (ConsultantDemandDetails)e.Item.DataItem;
                hypOpportunity.ToolTip = hypProject.ToolTip = item.ProjectDescription;
            }
        }

        protected void repByMonth_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                CollapsiblePanelDateExtenderClientIds = new List<string>();
                CollapsiblePanelDateExtenderList = new List<CollapsiblePanelExtender>();
                HtmlTableCell thTitleSkill = (HtmlTableCell)e.Item.FindControl("thTitleSkill");
                if (!string.IsNullOrEmpty(_hdIsSummaryPage.Value) && _hdIsSummaryPage.Value == true.ToString())
                {
                    thTitleSkill.Visible = false;
                }
                var lblTotal = (Label)e.Item.FindControl("lblTotal");
                var pnlTotal = (Panel)e.Item.FindControl("pnlTotal");
                var lblTotalForecastedDemand = (Label)e.Item.FindControl("lblTotalForecastedDemand");
                PopulateHeaderHoverLabels(lblTotal, pnlTotal, lblTotalForecastedDemand, GrandTotal, 80);
            }

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater repDetails = (Repeater)e.Item.FindControl("repDetails");
                ConsultantGroupByMonth dataitem = (ConsultantGroupByMonth)e.Item.DataItem;
                var result = dataitem.ConsultantDetailsByMonth;
                var cpeDetails = e.Item.FindControl("cpeDetails") as CollapsiblePanelExtender;
                if (result.Count > 0)
                {
                    repDetails.DataSource = result;
                    cpeDetails.BehaviorID = Guid.NewGuid().ToString();
                    CollapsiblePanelDateExtenderClientIds.Add(cpeDetails.BehaviorID);
                    CollapsiblePanelDateExtenderList.Add(cpeDetails);
                    repDetails.DataBind();
                }
                else
                {
                    var imgDetails = e.Item.FindControl("imgDetails") as Image;
                    var lbMonth = e.Item.FindControl("lbMonth") as Label;
                    imgDetails.Visible = false;
                    lbMonth.Style.Remove("display");
                    lbMonth.Text = "&nbsp;&nbsp;&nbsp;";
                    cpeDetails.Enabled = false;
                }

                HtmlTableCell tdTitleSkill = (HtmlTableCell)e.Item.FindControl("tdTitleSkill");
                if (!string.IsNullOrEmpty(_hdIsSummaryPage.Value) && _hdIsSummaryPage.Value == true.ToString())
                {
                    tdTitleSkill.Visible = false;
                }
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var output = jss.Serialize(CollapsiblePanelDateExtenderClientIds);
                hdncpeExtendersIds.Value = output;
                //btnExpandOrCollapseAll.Text = btnExpandOrCollapseAll.ToolTip = "Expand All";
                //hdnCollapsed.Value = "true";
            }
        }

        protected void repDetails_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HtmlTableCell tdTitleSkill = (HtmlTableCell)e.Item.FindControl("tdTitleSkill");
                if (!string.IsNullOrEmpty(_hdIsSummaryPage.Value) && _hdIsSummaryPage.Value == true.ToString())
                {
                    tdTitleSkill.Visible = false;
                }
                HyperLink hypOpportunity = (HyperLink)e.Item.FindControl("hlOpportunityNumber");
                HyperLink hypProject = (HyperLink)e.Item.FindControl("hlProjectNumber");

                ConsultantDemandDetailsByMonth item = (ConsultantDemandDetailsByMonth)e.Item.DataItem;
                hypOpportunity.ToolTip = hypProject.ToolTip = item.ProjectDescription;
            }

        }

        protected string GetOpportunityDetailsLink(int? opportunityId)
        {
            if (opportunityId.HasValue)
                return Utils.Generic.GetTargetUrlWithReturn(String.Format(Constants.ApplicationPages.DetailRedirectFormat, Constants.ApplicationPages.OpportunityDetail, opportunityId.Value),
                                                            Constants.ApplicationPages.ConsultingDemand_New);
            else
                return string.Empty;
        }

        protected string GetProjectDetailsLink(int? projectId)
        {
            if (projectId.HasValue)
                return Utils.Generic.GetTargetUrlWithReturn(String.Format(Constants.ApplicationPages.DetailRedirectFormat, Constants.ApplicationPages.ProjectDetail, projectId.Value),
                                                            Constants.ApplicationPages.ConsultingDemand_New);
            else
                return string.Empty;
        }

        public void PopulateData(bool isFromHostingPageOrGroupByMonth, string sortColumns = "")
        {
            if (!string.IsNullOrEmpty(_hdIsSummaryPage.Value) && _hdIsSummaryPage.Value == true.ToString())
            {
                btnGroupBy.Visible = false;
            }
            string title = string.IsNullOrEmpty(hdTitle.Value) ? null : hdTitle.Value;
            string skill = string.IsNullOrEmpty(hdSkill.Value) ? null : hdSkill.Value;
            string groupby = hdnGroupBy.Value;
            if (groupby == "title")
            {
                repByMonth.Visible = false;
                repByTitleSkill.Visible = true;
                List<ConsultantGroupbyTitleSkill> data = ServiceCallers.Custom.Report(r => r.ConsultingDemandDetailsByTitleSkill(HostingPage.StartDate.Value, HostingPage.EndDate.Value, title, skill, sortColumns)).ToList();
                sortColumn = isFromHostingPageOrGroupByMonth ? "TitleSkill" : sortColumn;
                sortAscend = isFromHostingPageOrGroupByMonth ? true : sortAscend;
                GrandTotal = data.Sum(p => p.TotalCount);
                repByTitleSkill.DataSource = data;
                repByTitleSkill.DataBind();
            }
            else if (groupby == "month")
            {
                repByTitleSkill.Visible = false;
                repByMonth.Visible = true;
                List<ConsultantGroupByMonth> data = ServiceCallers.Custom.Report(r => r.ConsultingDemandDetailsByMonth(HostingPage.StartDate.Value, HostingPage.EndDate.Value, title, skill, sortColumns, false)).ToList();
                sortColumn = isFromHostingPageOrGroupByMonth ? "MonthYear" : sortColumn;
                sortAscend = isFromHostingPageOrGroupByMonth ? true : sortAscend;
                GrandTotal = data.Sum(p => p.TotalCount);
                repByMonth.DataSource = data;
                repByMonth.DataBind();
            }
            if (!isFromHostingPageOrGroupByMonth && hdnCollapsed.Value.ToLower() != "true")
            {
                foreach (var cpe in CollapsiblePanelDateExtenderList)
                {
                    cpe.Collapsed = false;
                }
            }
            else if (isFromHostingPageOrGroupByMonth)
            {
                hdnCollapsed.Value = "true";
                btnExpandOrCollapseAll.Text = "Expand All";
            }
            if (!string.IsNullOrEmpty(_hdIsSummaryPage.Value) && _hdIsSummaryPage.Value == true.ToString())
            {
                var hostingPage = (PraticeManagement.Reports.ConsultingDemand_New)Page;
                hostingPage.SummaryControl.ConsultantDetailPopup.Show();
            }

        }

        private void PopulateHeaderHoverLabels(Label lblMonthName, Panel pnlMonthName, Label lblForecastedCount, int count, int position)
        {
            lblMonthName.Attributes[OnMouseOver] = string.Format(ShowPanel, lblMonthName.ClientID, pnlMonthName.ClientID, position);
            lblMonthName.Attributes[OnMouseOut] = string.Format(HidePanel, pnlMonthName.ClientID);
            lblForecastedCount.Text = count.ToString();
        }
    }
}

