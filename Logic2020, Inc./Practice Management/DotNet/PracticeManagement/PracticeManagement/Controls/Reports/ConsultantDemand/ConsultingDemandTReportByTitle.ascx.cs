using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports.ConsultingDemand;
using System.Web.Script.Serialization;
using AjaxControlToolkit;
using System.Text;

namespace PraticeManagement.Controls.Reports.ConsultantDemand
{
    public partial class ConsultingDemandTReportByTitle : System.Web.UI.UserControl
    {
        private string ConsultantDetailReportExport = "Consultant Detail Report Export";
        public Button BtnExportToExcelButton { get { return btnExportToExcel; } }
        public bool isTitles;
        private PraticeManagement.Reports.ConsultingDemand_New HostingPage
        {
            get { return ((PraticeManagement.Reports.ConsultingDemand_New)Page); }
        }
        public List<ConsultantGroupbyTitle> PopUpFilteredTitle { get; set; }
        public List<ConsultantGroupbySkill> PopUpFilteredSkill { get; set; }
        private List<KeyValuePair<string, string>> CollapsiblePanelExtenderClientIds
        {
            get;
            set;
        }
        public string HeaderTitle
        {
            get;
            set;
        }
        private DateTime? StartDate
        {
            get
            {
                if(Page is PraticeManagement.Reports.ConsultingDemand_New)
                {
                    var hostingPage = (PraticeManagement.Reports.ConsultingDemand_New)Page;
                    return hostingPage.StartDate;
                }
                return null;
            }
        }

        private DateTime? EndDate
        {
            get
            {
                if (Page is PraticeManagement.Reports.ConsultingDemand_New)
                {
                    var hostingPage = (PraticeManagement.Reports.ConsultingDemand_New)Page;
                    return hostingPage.EndDate;
                }
                return null;
            }
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
        protected void btnExportToExcel_OnClick(object sender,EventArgs e)
        {

            DataHelper.InsertExportActivityLogMessage(ConsultantDetailReportExport);
            

            if (StartDate.HasValue && EndDate.HasValue)
            {
               
                StringBuilder sb = new StringBuilder();

                sb.Append("Month: ");
                sb.Append("\t");
                sb.Append(StartDate.Value.ToString("MMMM yyyy"));
                sb.Append("to ");
                sb.Append("\t");
                sb.Append(EndDate.Value.ToString("MMMM yyyy"));
                sb.AppendLine();
                sb.AppendLine();
                if (isTitles == true)
                {
                    string titles = HostingPage.hdnTitlesProp;
                    PopUpFilteredTitle = ServiceCallers.Custom.Report(r => r.ConsultingDemandTransactionReportByTitle(StartDate.Value, EndDate.Value, titles)).ToList();
                }
                else
                {
                    string skills = HostingPage.hdnSkillsProp;
                    PopUpFilteredSkill = ServiceCallers.Custom.Report(r => r.ConsultingDemandTransactionReportBySkill(StartDate.Value, EndDate.Value, skills)).ToList();
                }

                if ((PopUpFilteredTitle != null && PopUpFilteredTitle.Count > 0) || (PopUpFilteredSkill !=null && PopUpFilteredSkill.Count > 0))
                {
                    //Header
                    /* Account	Account Name	Business Unit	Business Unit Name	Project	Project Name	Phase	
                    Work Type	Work Type Name	Date	Billable Hours	Non-Billable Hours	Total Hours	Note */
                    sb.Append("Title ");
                    sb.Append("\t");
                    sb.Append("SkillSet ");
                    sb.Append("\t");
                    sb.Append("OpportunityNumber ");
                    sb.Append("\t");
                    sb.Append("ProjectNumber ");
                    sb.Append("\t");
                    sb.Append("AccountName ");
                    sb.Append("\t");
                    sb.Append("ProjectName ");
                    sb.Append("\t");
                    sb.Append("ResourceStartDate ");
                    sb.Append("\t");
                    sb.AppendLine();

                    //Data
                    if (isTitles)
                    {
                        foreach (var gbytitle in PopUpFilteredTitle)
                        {
                            foreach (var detailsbyTitle in gbytitle.ConsultantDetails)
                            {
                                sb.Append(gbytitle.Title);
                                sb.Append("\t");
                                sb.Append(detailsbyTitle.Skill);
                                sb.Append("\t");
                                sb.Append(detailsbyTitle.OpportunityNumber);
                                sb.Append("\t");
                                sb.Append(detailsbyTitle.ProjectNumber);
                                sb.Append("\t");
                                sb.Append(detailsbyTitle.AccountName);
                                sb.Append("\t");
                                sb.Append(detailsbyTitle.ProjectName);
                                sb.Append("\t");
                                sb.Append(detailsbyTitle.ResourceStartDate);
                                sb.Append("\t");
                                sb.AppendLine();
                            }
                        }
                    }
                    else
                    {
                        foreach (var gbyskill in PopUpFilteredSkill)
                        {
                            foreach (var detailsbySkill in gbyskill.ConsultantDetails)
                            {
                                sb.Append(detailsbySkill.Title);
                                sb.Append("\t");
                                sb.Append(gbyskill.Skill);
                                sb.Append("\t");
                                sb.Append(detailsbySkill.OpportunityNumber);
                                sb.Append("\t");
                                sb.Append(detailsbySkill.ProjectNumber);
                                sb.Append("\t");
                                sb.Append(detailsbySkill.AccountName);
                                sb.Append("\t");
                                sb.Append(detailsbySkill.ProjectName);
                                sb.Append("\t");
                                sb.Append(detailsbySkill.ResourceStartDate);
                                sb.Append("\t");
                                sb.AppendLine();
                            }
                        }
                    }
                }
                else
                {
                    sb.Append("This consultant does not have demand for the selected period.");
                }
                var filename = string.Format("{0}_{1}_{2}.xls","ConsultantDemandDetail", StartDate.Value.ToString("MM.dd.yyyy"), EndDate.Value.ToString("MM.dd.yyyy"));
                GridViewExportUtil.Export(filename, sb);

            }
        }

        protected void repTitles_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                CollapsiblePanelDateExtenderClientIds = new List<string>();
                Label lblTitleSkill = (Label)e.Item.FindControl("lblTitleSkill");
                if (isTitles)
                {
                    lblTitleSkill.Text = "Skill Set";
                }
                else
                {
                    lblTitleSkill.Text = "Title";
                }
            }
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
             
                Label lblHeader1 = (Label)e.Item.FindControl("lblHeader");
                if (isTitles)
                {
                    ConsultantGroupbyTitle consDetail  = (ConsultantGroupbyTitle)e.Item.DataItem;
                    lblHeader1.Text = consDetail.Title;
                    Repeater rep = (Repeater)e.Item.FindControl("repDetails");
                    rep.DataSource = consDetail.ConsultantDetails;
                    rep.DataBind();
                }
                else
                {
                    ConsultantGroupbySkill consDetail = (ConsultantGroupbySkill)e.Item.DataItem;
                    lblHeader1.Text = consDetail.Skill;
                    Repeater rep = (Repeater)e.Item.FindControl("repDetails");
                    rep.DataSource = consDetail.ConsultantDetails;
                    rep.DataBind();
                }
                var cpeDate = e.Item.FindControl("cpeDetail") as CollapsiblePanelExtender;
                cpeDate.BehaviorID = Guid.NewGuid().ToString();
                CollapsiblePanelDateExtenderClientIds.Add(cpeDate.BehaviorID);
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
        protected void repDetails_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            { 
                Label lblTitleSkillItem = (Label)e.Item.FindControl("lblTitleSkillItem");
                Label lblOpportunityNumber = (Label)e.Item.FindControl("lblOpportunityNumber");
                Label lblProjectNumber = (Label)e.Item.FindControl("lblProjectNumber");
                Label lblAccountName = (Label)e.Item.FindControl("lblAccountName");
                Label lblProjectName = (Label)e.Item.FindControl("lblProjectName");
                Label lblRsrcStartDate = (Label)e.Item.FindControl("lblRsrcStartDate");

                if(e.Item.DataItem.GetType() == typeof(ConsultantDemandDetailsByMonthByTitle))
                {
                    ConsultantDemandDetailsByMonthByTitle consDetails = (ConsultantDemandDetailsByMonthByTitle)e.Item.DataItem;
                    lblOpportunityNumber.Text = consDetails.OpportunityNumber;
                    lblProjectNumber.Text = consDetails.ProjectNumber;
                    lblProjectName.Text = consDetails.ProjectName;
                    lblAccountName.Text = consDetails.AccountName;
                    lblRsrcStartDate.Text = consDetails.ResourceStartDate.ToString("MM/dd/yyyy");
                    lblTitleSkillItem.Text = consDetails.Skill;
                }
                else if (e.Item.DataItem.GetType() == typeof(ConsultantDemandDetailsByMonthBySkill))
                {
                    ConsultantDemandDetailsByMonthBySkill consDetails = (ConsultantDemandDetailsByMonthBySkill)e.Item.DataItem;
                    lblOpportunityNumber.Text = consDetails.OpportunityNumber;
                    lblProjectNumber.Text = consDetails.ProjectNumber;
                    lblProjectName.Text = consDetails.ProjectName;
                    lblAccountName.Text = consDetails.AccountName;
                    lblRsrcStartDate.Text = consDetails.ResourceStartDate.ToString("MM/dd/yyyy");
                    lblTitleSkillItem.Text = consDetails.Title;
                }
            }
        }

        public void PopulateData(bool isTitle)
        {
            isTitles = isTitle;
            if (isTitle)
            {
                repTitles.DataSource = PopUpFilteredTitle;
                repTitles.DataBind();
            }
            else
            {
                repTitles.DataSource = PopUpFilteredSkill;
                repTitles.DataBind();    
            }
        }
    }
}

