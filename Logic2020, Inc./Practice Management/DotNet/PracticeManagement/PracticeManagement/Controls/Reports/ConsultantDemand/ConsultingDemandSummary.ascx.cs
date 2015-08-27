using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using DataTransferObjects.Reports.ConsultingDemand;
using PraticeManagement.Utils.Excel;
using System.Data;
using PraticeManagement.Utils;

namespace PraticeManagement.Controls.Reports.ConsultantDemand
{
    public partial class ConsultingDemandSummary : System.Web.UI.UserControl
    {
        private string ConsultantSummaryReportExport = "Consultant Summary Report Export";
        private string ShowPanel = "ShowPanel('{0}', '{1}','{2}');";
        private string HidePanel = "HidePanel('{0}');";
        private string OnMouseOver = "onmouseover";
        private string OnMouseOut = "onmouseout";
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;
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

        private SheetStyles HeaderSheetStyle
        {
            get
            {
                CellStyles cellStyle = new CellStyles();
                cellStyle.IsBold = true;
                cellStyle.BorderStyle = NPOI.SS.UserModel.BorderStyle.None;
                cellStyle.FontHeight = 350;
                CellStyles[] cellStylearray = { cellStyle };
                RowStyles headerrowStyle = new RowStyles(cellStylearray);
                headerrowStyle.Height = 500;

                CellStyles dataCellStyle = new CellStyles();
                CellStyles[] dataCellStylearray = { dataCellStyle };
                RowStyles datarowStyle = new RowStyles(dataCellStylearray);

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };

                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.MergeRegion.Add(new int[] { 0, 0, 0, coloumnsCount - 1 });
                sheetStyle.IsAutoResize = false;

                return sheetStyle;
            }
        }

        private SheetStyles DataSheetStyle
        {
            get
            {
                CellStyles headerCellStyle = new CellStyles();
                headerCellStyle.IsBold = true;
                headerCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                List<CellStyles> headerCellStyleList = new List<CellStyles>();
                headerCellStyleList.Add(headerCellStyle);
                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataCellStyle = new CellStyles();

                CellStyles dataDateCellStyle = new CellStyles();
                dataDateCellStyle.DataFormat = "mm/dd/yy;@";

                CellStyles[] dataCellStylearray = { dataCellStyle,
                                                    dataCellStyle, 
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataDateCellStyle
                                                  };

                RowStyles datarowStyle = new RowStyles(dataCellStylearray);

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.TopRowNo = headerRowsCount;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanColSplit = 0;
                sheetStyle.FreezePanRowSplit = headerRowsCount;

                return sheetStyle;
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
            lblTotalCount.Text = "Total: " + ConsultantDetailReport.GrandTotal.ToString();
            mpeConsultantDetailReport.Show();
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(ConsultantSummaryReportExport);
            var filename = string.Format("{0}_{1}_{2}.xls", "ConsultantSummary", HostingPage.StartDate.Value.ToString(Constants.Formatting.DateFormatWithoutDelimiter), HostingPage.EndDate.Value.ToString(Constants.Formatting.DateFormatWithoutDelimiter));
            var dataSetList = new List<DataSet>();
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                var report = ServiceCallers.Custom.Report(r => r.ConsultingDemandDetailsByTitleSkill(HostingPage.StartDate.Value, HostingPage.EndDate.Value, cblTitle.SelectedItems, cblSkill.SelectedItems, "")).ToList();
                if (report.Count > 0)
                {
                    string dateRangeTitle = string.Format("Period: {0} to {1}", HostingPage.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat), HostingPage.EndDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                    DataTable header = new DataTable();
                    header.Columns.Add(dateRangeTitle);
                    headerRowsCount = header.Rows.Count + 3;
                    var data = PrepareDataTable(report);
                    coloumnsCount = data.Columns.Count;
                    sheetStylesList.Add(HeaderSheetStyle);
                    sheetStylesList.Add(DataSheetStyle);
                    var dataset = new DataSet();
                    dataset.DataSetName = "ConsultingDemandSummary";
                    dataset.Tables.Add(header);
                    dataset.Tables.Add(data);
                    dataSetList.Add(dataset);
                }
                else
                {
                    string dateRangeTitle = "No Consultants are there for the selected period.";
                    DataTable header = new DataTable();
                    header.Columns.Add(dateRangeTitle);
                    sheetStylesList.Add(HeaderSheetStyle);
                    var dataset = new DataSet();
                    dataset.DataSetName = "ConsultingDemandSummary";
                    dataset.Tables.Add(header);
                    dataSetList.Add(dataset);
                }
                NPOIExcel.Export(filename, dataSetList, sheetStylesList);
            }
        }

        private DataTable PrepareDataTable(List<ConsultantGroupbyTitleSkill> projectsList)
        {
            DataTable data = new DataTable();
            List<object> row;

            data.Columns.Add("Title");
            data.Columns.Add("Skill Set");
            data.Columns.Add("Opportunity Number");
            data.Columns.Add("Project Number");
            data.Columns.Add("Account Name");
            data.Columns.Add("Project Name");
            data.Columns.Add("Resource Start Date");

            foreach (var pro in projectsList)
            {
                foreach (var item in pro.ConsultantDetails)
                {
                    row = new List<object>();
                    row.Add(pro.Title != null ? pro.HtmlEncodedTitle : "");
                    row.Add(pro.Skill != null ? pro.HtmlEncodedSkill : "");
                    row.Add((item.OpportunityNumber != null) ? item.OpportunityNumber : "");
                    row.Add((item.ProjectNumber != null) ? item.ProjectNumber : "");
                    row.Add((item.AccountName != null) ? item.HtmlEncodedAccountName : "");
                    row.Add(item.ProjectName != null ? item.HtmlEncodedProjectName : "");
                    row.Add(item.ResourceStartDate != null && item.ResourceStartDate != DateTime.MinValue ? item.ResourceStartDate.ToString(Constants.Formatting.EntryDateFormat) : "");
                    data.Rows.Add(row.ToArray());
                }
            }
            return data;
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
                btnExportToExcel.Enabled = repResource.Visible = false;

            }
        }

        private void PopulateFilterPanels(List<ConsultantGroupbyTitleSkill> reportData)
        {
            PopulatTitleFilter(reportData);
            PopulatSkillFilter(reportData);
        }

        private void PopulatTitleFilter(List<ConsultantGroupbyTitleSkill> reportData)
        {
            var titleList = reportData.Select(r => new { Name = r.HtmlEncodedTitle }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblTitle.CheckBoxListObject, "All Titles ", titleList.ToArray(), false, "Name", "Name");
            cblTitle.SelectAllItems(true);
        }

        private void PopulatSkillFilter(List<ConsultantGroupbyTitleSkill> reportData)
        {
            var skillList = reportData.Select(r => new { Name = r.HtmlEncodedSkill }).Distinct().ToList().OrderBy(s => s.Name);
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

