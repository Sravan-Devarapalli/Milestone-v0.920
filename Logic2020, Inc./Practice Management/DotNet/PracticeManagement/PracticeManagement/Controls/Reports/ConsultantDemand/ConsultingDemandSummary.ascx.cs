using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using DataTransferObjects.Reports.ConsultingDemand;

namespace PraticeManagement.Controls.Reports.ConsultantDemand
{
    public partial class ConsultingDemandSummary : System.Web.UI.UserControl
    {
        private string ConsultantSummaryReportExport = "Consultant Summary Report Export";
        private string ShowPanel = "ShowPanel('{0}', '{1}','{2}');";
        private string HidePanel = "HidePanel('{0}');";
        private string OnMouseOver = "onmouseover";
        private string OnMouseOut = "onmouseout";
        private List<string> monthNames = new List<string>();
        private Dictionary<string, int> monthTotalCounts = new Dictionary<string, int>();

        private PraticeManagement.Reports.ConsultingDemand_New HostingPage
        {
            get { return ((PraticeManagement.Reports.ConsultingDemand_New)Page); }
        }

        public ModalPopupExtender ConsultantDetailPopup
        {
            get
            {
                return mpeConsultantDetailReport;

            }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        
        protected void repResource_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                Repeater repMonthHeader = (Repeater)e.Item.FindControl("repMonthHeader");
                repMonthHeader.DataSource = monthNames;
                repMonthHeader.DataBind();
                var lblTotal = (Label)e.Item.FindControl("lblTotal");
                var pnlTotal = (Panel)e.Item.FindControl("pnlTotal");
                var lblTotalForecastedDemand = (Label)e.Item.FindControl("lblTotalForecastedDemand");
                PopulateHeaderHoverLabels(lblTotal, pnlTotal, lblTotalForecastedDemand, monthTotalCounts.Values.Sum(),50);
            }
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater repMonthDemandCounts = (Repeater)e.Item.FindControl("repMonthDemandCounts");
                ConsultantGroupbyTitleSkill dataItem = (ConsultantGroupbyTitleSkill)e.Item.DataItem;
                repMonthDemandCounts.DataSource = dataItem.MonthCount.Values.ToList();
                repMonthDemandCounts.DataBind();

                var lnkConsultant = e.Item.FindControl("lnkConsultant") as LinkButton;
                var imgZoomIn = e.Item.FindControl("imgZoomIn") as HtmlImage;
                lnkConsultant.Attributes["onmouseover"] = string.Format("document.getElementById(\'{0}\').style.visibility='visible';", imgZoomIn.ClientID);
                lnkConsultant.Attributes["onmouseout"] = string.Format("document.getElementById(\'{0}\').style.visibility='hidden';", imgZoomIn.ClientID);
            }
        }

        protected void repMonthHeader_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lblMonthName = (Label)e.Item.FindControl("lblMonthName");
                var pnlMonthName = (Panel)e.Item.FindControl("pnlMonthName");
                var lblForecastedCount = (Label)e.Item.FindControl("lblTotalForecastedDemand");
                PopulateHeaderHoverLabels(lblMonthName, pnlMonthName, lblForecastedCount, monthTotalCounts[lblMonthName.Text],0);
            }
        }

        public void PopulateHeaderHoverLabels(Label lblMonthName, Panel pnlMonthName, Label lblForecastedCount,int count,int position)
        {
            lblMonthName.Attributes[OnMouseOver] = string.Format(ShowPanel, lblMonthName.ClientID, pnlMonthName.ClientID, position);
            lblMonthName.Attributes[OnMouseOut] = string.Format(HidePanel, pnlMonthName.ClientID);
            lblForecastedCount.Text = count.ToString();
        }

        public void PopulateData()
        {
            List<ConsultantGroupbyTitleSkill> data;
            data = ServiceCallers.Custom.Report(r => r.ConsultingDemandSummary(HostingPage.StartDate.Value, HostingPage.EndDate.Value,null)).ToList();
            btnExportToExcel.Enabled =
                repResource.Visible = true;
            if (data.Any())
            {
                monthNames = data.First().MonthCount.Keys.ToList();
                foreach (string month in monthNames)
                    monthTotalCounts.Add(month,data.Sum(p => p.MonthCount[month]));
            }
            repResource.DataSource = data;
            repResource.DataBind();
        }

        protected void lnkConsultant_OnClick(object sender,EventArgs e)
        {
            var lnkConsultant = sender as LinkButton;
            ConsultantDetailReport._hdSkill.Value = lnkConsultant.Attributes["Skill"];
            ConsultantDetailReport._hdTitle.Value = lnkConsultant.Attributes["Title"];
            ConsultantDetailReport.groupBy = "month";
            ConsultantDetailReport.PopulateData();
            lblConsultant.Text = ConsultantDetailReport._hdTitle.Value + "," + ConsultantDetailReport._hdSkill.Value;
            lblTotalCount.Text = ConsultantDetailReport.GrandTotal.ToString();
            ConsultantDetailReport._hdIsSummaryPage.Value = true.ToString();
            mpeConsultantDetailReport.Show();
            
            
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(ConsultantSummaryReportExport);

            // mso-number-format:"0\.00"
            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                var ConsultantSummaryReportExportList = ServiceCallers.Custom.Report(r => r.ConsultingDemandDetailsByTitleSkill(HostingPage.StartDate.Value, HostingPage.EndDate.Value, "", "")).ToList();

                StringBuilder sb = new StringBuilder();
                sb.Append("From Month"); 
                sb.Append("\t"); 
                sb.Append(HostingPage.StartDate.Value.ToString("MMMM yyyy"));
                sb.Append("\t"); 
                sb.Append("To Month"); 
                sb.Append("\t"); 
                sb.Append(HostingPage.EndDate.Value.ToString("MMMM yyyy"));
                sb.Append("\t");
                sb.AppendLine();
                if (ConsultantSummaryReportExportList.Count > 0)
                {
                    //Header
                    sb.Append("Title");
                    sb.Append("\t");
                    sb.Append("SkillSet");
                    sb.Append("\t");
                    sb.Append("Opportunity Number");
                    sb.Append("\t");
                    sb.Append("Project Number");
                    sb.Append("\t");
                    sb.Append("Account Name");
                    sb.Append("\t");
                    sb.Append("Project Name");
                    sb.Append("\t");
                    sb.Append("Resource Start Date");
                    sb.Append("\t");

                    sb.AppendLine();

                    //Data
                    foreach (var item in ConsultantSummaryReportExportList)
                    {
                        sb.Append(item.Title);
                        sb.Append("\t");
                        sb.Append(item.Skill);
                        sb.Append("\t");
                        for (int item2 = 0; item2 < item.ConsultantDetails.Count;item2++)
                        {
                            sb.Append(item.ConsultantDetails[item2].OpportunityNumber);
                            sb.Append("\t");
                            sb.Append(item.ConsultantDetails[item2].ProjectNumber);
                            sb.Append("\t");
                            sb.Append(item.ConsultantDetails[item2].AccountName);
                            sb.Append("\t");
                            sb.Append(item.ConsultantDetails[item2].ProjectName);
                            sb.Append("\t");
                            sb.Append(item.ConsultantDetails[item2].ResourceStartDate);
                            sb.Append("\t");
                            sb.AppendLine();
                            if (item.ConsultantDetails.Count - 1 != item2)
                            {
                                sb.Append(item.Title);
                                sb.Append("\t");
                                sb.Append(item.Skill);
                                sb.Append("\t");
                            }
                        }
                        sb.AppendLine();
                    }

                }
                else
                {
                    sb.Append("No Consultants are there for the selected period.");
                }
                //“[LastName]_[FirstName]-[“Summary” or “Detail”]-[StartOfRange]_[EndOfRange].xls”.  
                //example :Hong-Turney_Jason-Summary-03.01.2012_03.31.2012.xlsx
                var filename = string.Format("{0}_{1}_{2}.xls", "ConsultantSummary", HostingPage.StartDate.Value.ToString("MM.dd.yyyy"), HostingPage.EndDate.Value.ToString("MM.dd.yyyy"));
                GridViewExportUtil.Export(filename, sb);
            }
        }
    }
}
