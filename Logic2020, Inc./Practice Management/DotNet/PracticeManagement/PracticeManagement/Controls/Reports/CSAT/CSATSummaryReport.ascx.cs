using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using System.Text;
using System.Data;
using PraticeManagement.Utils;
using PraticeManagement.Utils.Excel;

namespace PraticeManagement.Controls.Reports.CSAT
{
    public partial class CSATSummaryReport : System.Web.UI.UserControl
    {
        private const string CSATReportExport = "CSAT Report Export";

        private int coloumnsCount = 1;

        private int headerRowsCount = 1;

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

                CellStyles wrapdataCellStyle = new CellStyles();
                wrapdataCellStyle.WrapText = true;

                CellStyles dataDateCellStyle = new CellStyles();
                dataDateCellStyle.DataFormat = "mm/dd/yy;@";

                CellStyles dataNumberDateCellStyle = new CellStyles();
                dataNumberDateCellStyle.DataFormat = "$#,##0";

                CellStyles dataNumberDateCellStyle1 = new CellStyles();
                dataNumberDateCellStyle1.DataFormat = "#,##0";

                CellStyles[] dataCellStylearray = { dataCellStyle,
                                                    dataCellStyle, 
                                                    dataCellStyle, 
                                                    dataCellStyle, 
                                                    dataCellStyle, 
                                                    dataCellStyle, 
                                                    dataCellStyle, 
                                                    dataCellStyle, 
                                                    dataNumberDateCellStyle,
                                                    dataCellStyle, 
                                                    dataDateCellStyle, 
                                                    dataDateCellStyle, 
                                                    dataDateCellStyle,
                                                    dataCellStyle, 
                                                    dataCellStyle ,
                                                    dataCellStyle,
                                                    wrapdataCellStyle,
                                                    dataCellStyle,
                                                    dataDateCellStyle, 
                                                    dataDateCellStyle, 
                                                    dataDateCellStyle,
                                                    dataCellStyle,
                                                    dataNumberDateCellStyle1,
                                                    dataCellStyle
                                                  };
                //only comments column need to set the 100 as width
                List<int> coloumnWidth = new List<int>();
                for (int i = 1; i < 24; i++)
                    coloumnWidth.Add(0);
                coloumnWidth.Add(100);

                RowStyles datarowStyle = new RowStyles(dataCellStylearray);

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.TopRowNo = headerRowsCount;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanColSplit = 0;
                sheetStyle.FreezePanRowSplit = headerRowsCount;
                sheetStyle.ColoumnWidths = coloumnWidth;
                return sheetStyle;
            }
        }

        private PraticeManagement.Reports.CSATReport HostingPage
        {
            get { return ((PraticeManagement.Reports.CSATReport)Page); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private DataTable PrepareDataTable(Project[] projectsList)
        {
            DataTable data = new DataTable();
            List<object> rownew;
            List<object> row;

            data.Columns.Add("Project Number");
            data.Columns.Add("Account");
            data.Columns.Add("Business Unit");
            data.Columns.Add("Business Group");
            data.Columns.Add("Project Status");
            data.Columns.Add("Buyer Name");
            data.Columns.Add("Project Name");
            data.Columns.Add("Project Manager");
            data.Columns.Add("Estimated Revenue");
            data.Columns.Add("CSAT Eligible?");
            data.Columns.Add("Start Date");
            data.Columns.Add("End Date");
            data.Columns.Add("Date of Project Completion");
            data.Columns.Add("Practice Area");
            data.Columns.Add("Salesperson");
            data.Columns.Add("Executive in Charge");
            data.Columns.Add("Project Access");
            data.Columns.Add("CSAT Owner");
            data.Columns.Add("CSAT Start Date");
            data.Columns.Add("CSAT End Date");
            data.Columns.Add("CSAT Completion Date");
            data.Columns.Add("CSAT Reviewer");
            data.Columns.Add("CSAT Score");
            data.Columns.Add("CSAT Comments");

            foreach (var pro in projectsList)
            {
                row = new List<object>();
                int i;
                row.Add(pro.ProjectNumber != null ? pro.ProjectNumber.ToString() : "");
                row.Add((pro.Client != null && pro.Client.HtmlEncodedName != null) ? pro.Client.Name.ToString() : "");
                row.Add((pro.Group != null && pro.Group.Name != null) ? pro.Group.Name : "");
                row.Add((pro.BusinessGroup != null && pro.BusinessGroup.Name != null) ? pro.BusinessGroup.Name : "");
                row.Add((pro.Status != null && pro.Status.Name != null) ? pro.Status.Name.ToString() : "");
                row.Add(pro.BuyerName);
                row.Add(pro.Name != null ? pro.Name : "");
                row.Add((pro.ProjectOwner != null && pro.ProjectOwner.Name != null) ? pro.ProjectOwner.Name : "");
                row.Add(pro.SowBudget.HasValue ? pro.SowBudget.Value.ToString() : "");
                row.Add(pro.IsCSATEligible ? "Yes" : "No");
                row.Add(pro.StartDate.HasValue && pro.StartDate.Value != DateTime.MinValue ? pro.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) : "");
                row.Add(pro.EndDate.HasValue && pro.EndDate.Value != DateTime.MinValue ? pro.EndDate.Value.ToString(Constants.Formatting.EntryDateFormat) : "");
                row.Add((pro.RecentCompletedStatusDate == null || pro.RecentCompletedStatusDate.Value == DateTime.MinValue) ? "" : pro.RecentCompletedStatusDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                row.Add((pro.Practice != null && pro.Practice.Name != null) ? HttpUtility.HtmlEncode(pro.Practice.Name) : "");
                row.Add((pro.SalesPersonName != null) ? pro.SalesPersonName : "");
                row.Add((pro.Director != null && pro.Director.Name != null && pro.Director.FirstName != null) ? HttpUtility.HtmlEncode(pro.Director.Name.ToString()) : "");
                row.Add(HttpUtility.HtmlEncode(pro.ProjectManagerNames.Replace(";", "\n")));
                row.Add(!string.IsNullOrEmpty(pro.CSATOwnerName) ? pro.CSATOwnerName : "");

                for (i = 0; i < pro.CSATList.Count; i++)
                {
                    row.Add(pro.CSATList[i].ReviewStartDate != DateTime.MinValue ? pro.CSATList[i].ReviewStartDate.ToString(Constants.Formatting.EntryDateFormat) : "");
                    row.Add(pro.CSATList[i].ReviewEndDate != DateTime.MinValue ? pro.CSATList[i].ReviewEndDate.ToString(Constants.Formatting.EntryDateFormat) : "");
                    row.Add(pro.CSATList[i].CompletionDate != DateTime.MinValue ? pro.CSATList[i].CompletionDate.ToString(Constants.Formatting.EntryDateFormat) : "");
                    row.Add(pro.CSATList[i].ReviewerName);
                    row.Add(pro.CSATList[i].ReferralScore == -2 ? string.Empty : pro.CSATList[i].ReferralScore == -1 ? "Not Applicable" : pro.CSATList[i].ReferralScore.ToString());
                    row.Add(pro.CSATList[i].Comments);
                    data.Rows.Add(row.ToArray());

                    if (i != pro.CSATList.Count - 1)
                    {
                        rownew = new List<object>();
                        rownew.AddRange(row);
                        rownew.RemoveRange(18, 6);
                        row = rownew;
                    }
                }
            }
            return data;
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(CSATReportExport);
            var filename = string.Format("CSAT_Report_{0}_{1}.xls", HostingPage.StartDate.Value.ToString(Constants.Formatting.DateFormatWithoutDelimiter), HostingPage.EndDate.Value.ToString(Constants.Formatting.DateFormatWithoutDelimiter));
            var dataSetList = new List<DataSet>();
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                var report = ServiceCallers.Custom.Project(r => r.CSATSummaryReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.SelectedPractices, HostingPage.SelectedAccounts, true)).ToArray();
                if (report.Length > 0)
                {
                    string dateRangeTitle = string.Format("CSAT Report For the Period: {0} to {1}", HostingPage.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat), HostingPage.EndDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                    DataTable header = new DataTable();
                    header.Columns.Add(dateRangeTitle);
                    headerRowsCount = header.Rows.Count + 3;
                    var data = PrepareDataTable(report);
                    coloumnsCount = data.Columns.Count;
                    sheetStylesList.Add(HeaderSheetStyle);
                    sheetStylesList.Add(DataSheetStyle);
                    var dataset = new DataSet();
                    dataset.DataSetName = "CSAT";
                    dataset.Tables.Add(header);
                    dataset.Tables.Add(data);
                    dataSetList.Add(dataset);
                }
                else
                {
                    string dateRangeTitle = "There are no CSAT Entries towards this range selected.";
                    DataTable header = new DataTable();
                    header.Columns.Add(dateRangeTitle);
                    sheetStylesList.Add(HeaderSheetStyle);
                    var dataset = new DataSet();
                    dataset.DataSetName = "CSAT";
                    dataset.Tables.Add(header);
                    dataSetList.Add(dataset);
                }
                NPOIExcel.Export(filename, dataSetList, sheetStylesList);
            }
        }

        protected string GetProjectDetailsLink(int? projectId, bool flag)
        {
            if (projectId.HasValue)
                return Utils.Generic.GetTargetUrlWithReturn(String.Format(Constants.ApplicationPages.DetailRedirectFormat, Constants.ApplicationPages.ProjectDetail, projectId.Value) + (flag ? "&CSAT=true" : string.Empty),
                                                            Constants.ApplicationPages.CSATReport);
            else
                return string.Empty;
        }

        protected string GetFormatedSowBudget(decimal? sow)
        {
            return sow.HasValue ? sow.Value.ToString(Constants.Formatting.CurrencyExcelReportFormatWithoutDecimal) : "$0";
        }

        protected void repSummary_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HyperLink hlCSATScore = (HyperLink)e.Item.FindControl("hlCSATScore");
                Label lblSymblvsble = (Label)e.Item.FindControl("lblSymblvsble");
                Project project = (Project)e.Item.DataItem;
                hlCSATScore.Text = project.CSATList[0].ReferralScore.ToString();
                lblSymblvsble.Text = project.HasMultipleCSATs ? "!" : "";
            }
        }

        public void PopulateData()
        {
            Project[] projects = ServiceCallers.Custom.Project(p => p.CSATSummaryReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.SelectedPractices, HostingPage.SelectedAccounts, false));
            if (projects.Length > 0)
            {
                repSummary.Visible = true;
                HostingPage.HeaderTable.Visible = true;
                btnExportToExcel.Enabled = true;
                divEmptyMessage.Style["display"] = "none";
                repSummary.DataSource = projects;
                repSummary.DataBind();
            }
            else
            {
                repSummary.Visible = false;
                btnExportToExcel.Enabled = false;
                divEmptyMessage.Style["display"] = "";
            }
            HostingPage.PopulateHeaderSection();
        }
    }
}

