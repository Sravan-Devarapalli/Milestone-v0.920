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
        private Dictionary<string, int> monthTotalCounts = new Dictionary<string, int>();
        private HtmlImage ImgTitleFilter { get; set; }
        private HtmlImage ImgSkillFilter { get; set; }

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
            cblSkill.OKButtonId = cblTitle.OKButtonId = btnFilterOK.ClientID;
        }

        protected void repResource_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                ImgTitleFilter = e.Item.FindControl("imgTitleFilter") as HtmlImage;
                ImgSkillFilter = e.Item.FindControl("imgSkillFilter") as HtmlImage;
                Repeater repMonthHeader = (Repeater)e.Item.FindControl("repMonthHeader");
                repMonthHeader.DataSource = HostingPage.MonthNames;
                repMonthHeader.DataBind();
                var lblTotal = (Label)e.Item.FindControl("lblTotal");
                var pnlTotal = (Panel)e.Item.FindControl("pnlTotal");
                var lblTotalForecastedDemand = (Label)e.Item.FindControl("lblTotalForecastedDemand");
                PopulateHeaderHoverLabels(lblTotal, pnlTotal, lblTotalForecastedDemand, monthTotalCounts.Values.Sum(), 50);
            }
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater repMonthDemandCounts = (Repeater)e.Item.FindControl("repMonthDemandCounts");
                ConsultantGroupbyTitleSkill dataItem = (ConsultantGroupbyTitleSkill)e.Item.DataItem;
                repMonthDemandCounts.DataSource = dataItem.MonthCount.Values.ToList();
                repMonthDemandCounts.DataBind();
            }
        }

        protected void repMonthHeader_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lblMonthName = (Label)e.Item.FindControl("lblMonthName");
                var pnlMonthName = (Panel)e.Item.FindControl("pnlMonthName");
                var lblForecastedCount = (Label)e.Item.FindControl("lblTotalForecastedDemand");
                int count = monthTotalCounts.Keys.Any(p => p == lblMonthName.Text) ? monthTotalCounts[lblMonthName.Text] : 0;
                PopulateHeaderHoverLabels(lblMonthName, pnlMonthName, lblForecastedCount, count, 0);
            }
        }

        protected void lnkConsultant_OnClick(object sender, EventArgs e)
        {
            var lnkConsultant = sender as LinkButton;
            ConsultantDetailReport._hdSkill.Value = lnkConsultant.Attributes["Skill"];
            ConsultantDetailReport._hdTitle.Value = lnkConsultant.Attributes["Title"];
            ConsultantDetailReport.groupBy = "month";
            ConsultantDetailReport._hdIsSummaryPage.Value = true.ToString();
            ConsultantDetailReport.Collapsed = true.ToString();
            ConsultantDetailReport.PopulateData(false);
            lblConsultant.Text = ConsultantDetailReport._hdTitle.Value + "," + ConsultantDetailReport._hdSkill.Value;
            lblTotalCount.Text = "Total: "+ConsultantDetailReport.GrandTotal.ToString();
            mpeConsultantDetailReport.Show();
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(ConsultantSummaryReportExport);

            // mso-number-format:"0\.00"
            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {

                var ConsultantSummaryReportExportList = ServiceCallers.Custom.Report(r => r.ConsultingDemandDetailsByTitleSkill(HostingPage.StartDate.Value, HostingPage.EndDate.Value, cblTitle.SelectedItems, cblSkill.SelectedItems, "")).ToList();

                StringBuilder sb = new StringBuilder();
                sb.Append("Period: ");
                sb.Append(HostingPage.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                sb.Append(" To ");
                sb.Append(HostingPage.EndDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                sb.Append("\t");
                sb.AppendLine();
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
                        for (int item2 = 0; item2 < item.ConsultantDetails.Count; item2++)
                        {
                            for (int i = 0; i < item.ConsultantDetails[item2].Count; i++)
                            {
                                sb.Append(item.ConsultantDetails[item2].OpportunityNumber);
                                sb.Append("\t");
                                sb.Append(item.ConsultantDetails[item2].ProjectNumber);
                                sb.Append("\t");
                                sb.Append(item.ConsultantDetails[item2].AccountName);
                                sb.Append("\t");
                                sb.Append(item.ConsultantDetails[item2].ProjectName);
                                sb.Append("\t");
                                sb.Append(item.ConsultantDetails[item2].ResourceStartDate.ToString(Constants.Formatting.EntryDateFormat));
                                sb.Append("\t");
                                sb.AppendLine();
                            }
                            if (item.ConsultantDetails.Count - 1 != item2)
                            {
                                sb.Append(item.Title);
                                sb.Append("\t");
                                sb.Append(item.Skill);
                                sb.Append("\t");
                            }
                        }
                    }

                }
                else
                {
                    sb.Append("No Consultants are there for the selected period.");
                }
                //“[LastName]_[FirstName]-[“Summary” or “Detail”]-[StartOfRange]_[EndOfRange].xls”.  cblTitle
                //example :Hong-Turney_Jason-Summary-03.01.2012_03.31.2012.xlsx
                var filename = string.Format("{0}_{1}_{2}.xls", "ConsultantSummary", HostingPage.StartDate.Value.ToString(Constants.Formatting.DateFormatWithoutDelimiter), HostingPage.EndDate.Value.ToString(Constants.Formatting.DateFormatWithoutDelimiter));
                GridViewExportUtil.Export(filename, sb);
            }
        }

        protected void btnFilterOK_OnClick(object sender, EventArgs e)
        {
            PopulateData(false);
        }

        public void PopulateData(bool isPopulateFilters = true)
        {
            List<ConsultantGroupbyTitleSkill> data;
            if (isPopulateFilters)
            {
                data = ServiceCallers.Custom.Report(r => r.ConsultingDemandSummary(HostingPage.StartDate.Value, HostingPage.EndDate.Value, null, null)).ToList();
            }
            else
            {
                data = ServiceCallers.Custom.Report(r => r.ConsultingDemandSummary(HostingPage.StartDate.Value, HostingPage.EndDate.Value, cblTitle.SelectedItems, cblSkill.SelectedItems)).ToList();
            }
            btnExportToExcel.Enabled = repResource.Visible = true;
            if (data.Any())
            {
                foreach (string month in HostingPage.MonthNames)
                    monthTotalCounts.Add(month, data.Sum(p => p.MonthCount[month]));
            }
            DataBindResource(data, isPopulateFilters);
        }

        private void DataBindResource(List<ConsultantGroupbyTitleSkill> reportData, bool isPopulateFilters)
        {
            var reportDataList = reportData.ToList();
            if (isPopulateFilters)
            {
                PopulateFilterPanels(reportDataList);
            }
            if (reportDataList.Count > 0 || cblTitle.Items.Count > 1 || cblSkill.Items.Count > 1)
            {
                divEmptyMessage.Style["display"] = "none";
                repResource.DataSource = reportData;
                repResource.DataBind();

                cblTitle.SaveSelectedIndexesInViewState();
                cblSkill.SaveSelectedIndexesInViewState();

                ImgTitleFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblTitle.FilterPopupClientID,
                  cblTitle.SelectedIndexes, cblTitle.CheckBoxListObject.ClientID, cblTitle.WaterMarkTextBoxBehaviorID);

                ImgSkillFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblSkill.FilterPopupClientID,
                  cblSkill.SelectedIndexes, cblSkill.CheckBoxListObject.ClientID, cblSkill.WaterMarkTextBoxBehaviorID);
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repResource.Visible = false;
            }
        }

        private void PopulateFilterPanels(List<ConsultantGroupbyTitleSkill> reportData)
        {
            PopulatTitleFilter(reportData);
            PopulatSkillFilter(reportData);
        }

        private void PopulatTitleFilter(List<ConsultantGroupbyTitleSkill> reportData)
        {
            var titleList = reportData.Select(r => new { Name = r.Title }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblTitle.CheckBoxListObject, "All Titles ", titleList.ToArray(), false, "Name", "Name");
            cblTitle.SelectAllItems(true);
        }

        private void PopulatSkillFilter(List<ConsultantGroupbyTitleSkill> reportData)
        {
            var skillList = reportData.Select(r => new { Name = r.Skill }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblSkill.CheckBoxListObject, "All Skills ", skillList.ToArray(), false, "Name", "Name");
            cblSkill.SelectAllItems(true);
        }

        private void PopulateHeaderHoverLabels(Label lblMonthName, Panel pnlMonthName, Label lblForecastedCount, int count, int position)
        {
            lblMonthName.Attributes[OnMouseOver] = string.Format(ShowPanel, lblMonthName.ClientID, pnlMonthName.ClientID, position);
            lblMonthName.Attributes[OnMouseOut] = string.Format(HidePanel, pnlMonthName.ClientID);
            lblForecastedCount.Text = count.ToString();
        }
    }
}

