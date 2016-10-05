using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Web.UI.HtmlControls;
using PraticeManagement.Utils.Excel;
using System.Data;
using PraticeManagement.Utils;
using PraticeManagement.Objects;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PraticeManagement.Controls.Reports
{
    public partial class ExpenseSummaryByType : System.Web.UI.UserControl
    {
        #region Constants

        private const int NumberOfFixedColumns = 1;
        private int headerRowsCount = 1;
        private int coloumnsCount = 1;
        public string CurrencyDisplayFormat = "$###,###,###,###,###,##0";

        #endregion Constants

        #region Properties

        private PraticeManagement.Reports.ExpenseReport HostingPage
        {
            get
            {
                return ((PraticeManagement.Reports.ExpenseReport)Page);
            }
        }

        public int? SelectedExpenseTypeId
        {
            get
            {
                return (int?)ViewState["SelectedExpenseTypeIdForDetails"];
            }
            set
            {
                ViewState["SelectedExpenseTypeIdForDetails"] = value;
            }
        }

        public int? projectId
        {
            get;
            set;
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
                dataCellStyle.IsBold = true;
                dataCellStyle.WrapText = true;
                dataCellStyle.BorderStyle = NPOI.SS.UserModel.BorderStyle.None;
                dataCellStyle.FontHeight = 200;

                CellStyles[] dataCellStylearray = { dataCellStyle };
                RowStyles datarowStyle1 = new RowStyles(dataCellStylearray);
                RowStyles datarowStyle2 = new RowStyles(dataCellStylearray);
                datarowStyle1.Height = 350;
                datarowStyle2.Height = 1250;

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle2, datarowStyle1 };

                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.MergeRegion.Add(new int[] { 0, 0, 0, coloumnsCount - 1 });
                sheetStyle.MergeRegion.Add(new int[] { 1, 1, 0, coloumnsCount - 1 });
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

                CellStyles dateCellStyle = new CellStyles();
                dateCellStyle.IsBold = true;
                dateCellStyle.WrapText = true;
                dateCellStyle.DataFormat = "[$-409]mmm-yy;@";

                List<CellStyles> headerCellStyleList = new List<CellStyles>() { headerCellStyle };

                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataStartDateCellStyle = new CellStyles();
                dataStartDateCellStyle.DataFormat = "mm/dd/yy;@";

                CellStyles dataNumberDateCellStyle = new CellStyles();
                dataNumberDateCellStyle.DataFormat = "$#,##0.00_);($#,##0.00)";

                CellStyles dataStyle = new CellStyles();

                CellStyles[] dataCellStylearray = { dataStyle, dataNumberDateCellStyle };
                List<int> coloumnWidth = new List<int>();
                for (int i = 0; i < 3; i++)
                    coloumnWidth.Add(0);
                RowStyles datarowStyle = new RowStyles(dataCellStylearray.ToArray());
                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };

                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.TopRowNo = headerRowsCount;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanColSplit = 0;
                sheetStyle.FreezePanRowSplit = headerRowsCount;
                sheetStyle.ColoumnWidths = coloumnWidth;
                sheetStyle.IsAutoResize = true;
                return sheetStyle;
            }
        }

        private HtmlImage imgExpenseTypeFilter { get; set; }

        public LinkButton LnkbtnSummaryObject
        {
            get
            {
                return lnkbtnSummary;
            }
        }

        private List<ExpenseSummary> MonthlyDetails
        {
            get;
            set;
        }

        #endregion Properties


        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void SwitchView(Control control, int viewIndex)
        {
            SelectView(control, viewIndex);
            LoadActiveTabInByResource();
        }

        private void SetCssClassEmpty()
        {
            foreach (TableCell cell in tblViewSwitch.Rows[0].Cells)
            {
                cell.CssClass = string.Empty;
            }
        }

        public void SelectView(Control sender, int viewIndex)
        {
            mvReport.ActiveViewIndex = viewIndex;

            SetCssClassEmpty();

            ((WebControl)sender.Parent).CssClass = "SelectedSwitch";
        }

        private void LoadActiveTabInByResource()
        {
            if (mvReport.ActiveViewIndex == 0)
            {
                PopulateData();
            }
            else
            {
                PopulateDetailedReport();

            }
            //HostingPage.UpdReport.Update();
        }

        protected void btnView_Command(object sender, CommandEventArgs e)
        {
            int viewIndex = int.Parse((string)e.CommandArgument);
            SwitchView((Control)sender, viewIndex);
        }

        public void PopulateData()
        {
            ExpenseSummary[] report = ServiceCallers.Custom.Report(r => r.GetExpenseSummaryGroupedBytype(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.ExpenseTypesSelected));

            BindExpenseTypeData(report);
        }

        public void PopulateDetailedReport()
        {
            ucExpenseSummaryDetails.ByProject = false;
            ucExpenseSummaryDetails.ByExpense = true;
            ucExpenseSummaryDetails.PopulateExpenseData();
        }

        public void BindExpenseTypeData(ExpenseSummary[] reportData)
        {
            if (reportData != null && reportData.Count() > 0)
            {
                divEmptyMessage.Style["display"] = "none";
                repExpenseType.Visible = true;
                repExpenseType.DataSource = reportData;
                repExpenseType.DataBind();
                btnExportToExcel.Enabled = true;

                ltTypeCount.Text = reportData.Count() + " Expense Types";
            }
            else
            {
                ltTypeCount.Text = "No records";
                divEmptyMessage.Style["display"] = "";
                repExpenseType.Visible = false;
                btnExportToExcel.Enabled = false;
            }
            lbRange.Text = HostingPage.Range;
        }

        protected void repExpenseType_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                var row = e.Item.FindControl("lvHeader") as HtmlTableRow;
                AddMonthHeaderCells(row);
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var row = e.Item.FindControl("lvItem") as HtmlTableRow;
                if (row.Cells.Count == NumberOfFixedColumns)
                {
                    var monthsInPeriod = HostingPage.NumberOfMonths;
                    for (int i = 0; i < monthsInPeriod + 1; i++)   // + 1 means a cell for total column
                    {
                        var td = new HtmlTableCell() { };
                        td.Attributes["class"] = "CompPerfMonthSummary";
                        row.Cells.Insert(row.Cells.Count, td);
                    }
                }
                var expenceByMonth = e.Item.DataItem as ExpenseSummary;
                MonthlyDetails = ServiceCallers.Custom.Report(r => r.ExpenseDetailReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, projectId, expenceByMonth.Type.Id)).ToList();
                FillMonthCells(row, expenceByMonth);
            }
        }

        private void AddMonthHeaderCells(HtmlTableRow row)
        {
            if (row != null)
            {
                DateTime periodStart = Utils.Calendar.MonthStartDate(HostingPage.StartDate.Value);
                while (row.Cells.Count > NumberOfFixedColumns + 1)
                {
                    row.Cells.RemoveAt(NumberOfFixedColumns);
                }
                for (int i = NumberOfFixedColumns, k = 0; k < HostingPage.NumberOfMonths; i++, k++)
                {
                    var newColumn = new HtmlTableCell("TH");
                    row.Cells.Insert(i, newColumn);

                    row.Cells[i].InnerHtml = periodStart.ToString(Constants.Formatting.CompPerfMonthYearFormat);
                    row.Cells[i].Attributes["class"] = "CompPerfMonthSummary";

                    periodStart = periodStart.AddMonths(1);
                }
                var totalColumn = new HtmlTableCell("TH");
                totalColumn.InnerHtml = "Total";
                totalColumn.Attributes["class"] = "CompPerfMonthSummary";
                row.Cells.Insert(row.Cells.Count, totalColumn);
            }
        }

        private void FillMonthCells(HtmlTableRow row, ExpenseSummary expenseSummary)
        {
            DateTime monthBegin = Utils.Calendar.MonthStartDate(HostingPage.StartDate.Value);
            int monthsInPeriod = HostingPage.NumberOfMonths;
            bool showActuals = HostingPage.ShowActualExpense;
            bool showEtimated = HostingPage.ShowEstimatedExpense;

            for (int i = NumberOfFixedColumns, k = 0;
               k < monthsInPeriod;
               i++, k++, monthBegin = monthBegin.AddMonths(1))
            {
                DateTime monthEnd = Utils.Calendar.MonthEndDate(monthBegin);


                if (expenseSummary.MonthlyExpectedExpenses != null && showEtimated)
                {
                    foreach (KeyValuePair<DateTime, decimal> interestValue in expenseSummary.MonthlyExpectedExpenses)
                    {
                        if (IsInMonth(interestValue.Key, monthBegin, monthEnd))
                        {

                            List<ExpenseSummary> tempList = MonthlyDetails.Where(r => r.MonthStartDate == interestValue.Key).ToList();

                            Label lblEst = new Label();
                            lblEst.Text = interestValue.Value.ToString(CurrencyDisplayFormat) + "<br />";
                            lblEst.CssClass = "EstimatedExpense";
                            lblEst.Attributes["Description"] = DataHelper.GetTextForHover(tempList, false, true); //"<table><tr><td>Estimated</td><td>Amount</td></ tr><tr><td>abc</td><td>100</td></ tr></ table>";
                            lblEst.Attributes["onmouseout"] = "HidePanel();";
                            lblEst.Attributes["onmouseover"] = "SetTooltipText(this.attributes['Description'].value,this);";
                            row.Cells[i].Controls.Add(lblEst);
                        }
                    }
                }


                if (expenseSummary.MonthlyExpenses != null && showActuals)
                {
                    foreach (KeyValuePair<DateTime, decimal> interestValue in expenseSummary.MonthlyExpenses)
                    {
                        if (IsInMonth(interestValue.Key, monthBegin, monthEnd))
                        {
                            if (HostingPage.ShowActualExpense)
                            {
                                List<ExpenseSummary> tempList = MonthlyDetails.Where(r => r.MonthStartDate == interestValue.Key).ToList();

                                Label lblAct = new Label();
                                lblAct.Text = interestValue.Value.ToString(CurrencyDisplayFormat);
                                lblAct.CssClass = "ActualExpense";
                                lblAct.Attributes["Description"] = DataHelper.GetTextForHover(tempList, true, true);
                                lblAct.Attributes["onmouseout"] = "HidePanel();";
                                lblAct.Attributes["onmouseover"] = "SetTooltipText(this.attributes['Description'].value,this);";
                                row.Cells[i].Controls.Add(lblAct);

                            }
                        }
                    }
                }

                if (row.Cells[i].Controls.Count == 0)
                {
                    row.Cells[i].InnerHtml = "-";
                }
            }
            if (showEtimated)
            {
                row.Cells[row.Cells.Count - 1].InnerHtml = "<span class=\"EstimatedExpense\">" + expenseSummary.TotalExpectedAmount.ToString(CurrencyDisplayFormat) + "</span><br />";
            }
            if (showActuals)
            {
                row.Cells[row.Cells.Count - 1].InnerHtml += "<span class=\"ActualExpense\">" + expenseSummary.TotalExpense.ToString(CurrencyDisplayFormat) + "</span>";
            }
        }

        private static bool IsInMonth(DateTime date, DateTime periodStart, DateTime periodEnd)
        {
            var result =
                (date.Year > periodStart.Year ||
                (date.Year == periodStart.Year && date.Month >= periodStart.Month)) &&
                (date.Year < periodEnd.Year || (date.Year == periodEnd.Year && date.Month <= periodEnd.Month));

            return result;
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {

            DataHelper.InsertExportActivityLogMessage("Expense Report");
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();


            var report = ServiceCallers.Custom.Report(r => r.GetExpenseSummaryGroupedBytype(HostingPage.StartDate.Value, HostingPage.EndDate.Value, null)).ToList();

            var fileName = string.Format("ExpenseSummary By ExpenseType.xls");

            if (report.Count > 0)
            {
                DataTable header1 = new DataTable();
                header1.Columns.Add(string.Format("Expense Report_ByExpenseType"));

                List<object> row1 = new List<object>();
                row1.Add(string.Format("{0}-{1}\n{2}", HostingPage.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat), HostingPage.EndDate.Value.ToString(Constants.Formatting.EntryDateFormat), report.Count() + " Expense Types"));
                header1.Rows.Add(row1.ToArray());
                headerRowsCount = header1.Rows.Count + 3;

                DataTable data;

                data = PrepareDataTable(report);
                coloumnsCount = data.Columns.Count;
                var dataset = new DataSet();
                dataset.DataSetName = "ExpenseByExpenseType_Report";
                dataset.Tables.Add(header1);
                dataset.Tables.Add(data);
                dataSetList.Add(dataset);
                sheetStylesList.Add(HeaderSheetStyle);
                sheetStylesList.Add(DataSheetStyle);
            }
            else
            {
                string dateRangeTitle = "There are no Records the selected parameters.";
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                sheetStylesList.Add(HeaderSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "ExpenseByExpenseType_Report";
                dataset.Tables.Add(header);
                dataSetList.Add(dataset);
            }

            NPOIExcel.Export(fileName, dataSetList, sheetStylesList);
        }

        public DataTable PrepareDataTable(List<ExpenseSummary> report)
        {

            int monthsInPeriod = HostingPage.NumberOfMonths;
            bool showActuals = HostingPage.ShowActualExpense;
            bool showEstimated = HostingPage.ShowEstimatedExpense;

            DataTable data = new DataTable();
            List<object> row;
            data.Columns.Add("Expense Type");

            for (int i = 0; i < monthsInPeriod; i++)
            {
                if (showEstimated)
                {
                    data.Columns.Add(Utils.Calendar.MonthStartDate(HostingPage.StartDate.Value).AddMonths(i).ToString(Constants.Formatting.CompPerfMonthYearFormat) + "(Estimated Expense)");
                }
                if (showActuals)
                {
                    data.Columns.Add(Utils.Calendar.MonthStartDate(HostingPage.StartDate.Value).AddMonths(i).ToString(Constants.Formatting.CompPerfMonthYearFormat) + "(Actual Expense)");
                }
            }

            if (showActuals)
            {
                data.Columns.Add("Total - Actual");
            }
            if (showEstimated)
            {
                data.Columns.Add("Total - Estimated");
            }

            foreach (var expense in report)
            {
                DateTime monthBegin = Utils.Calendar.MonthStartDate(HostingPage.StartDate.Value);
                row = new List<object>();
                row.Add(expense.Type.Name);

                var colValue = 0M;
                var colExpValue = 0M;
                for (int k = 0;
               k < monthsInPeriod;
               k++, monthBegin = monthBegin.AddMonths(1))
                {
                    DateTime monthEnd = Utils.Calendar.MonthEndDate(monthBegin);
                    colValue = 0M;
                    colExpValue = 0M;
                    if (showEstimated)
                    {
                        if (expense.MonthlyExpectedExpenses != null)
                        {
                            foreach (KeyValuePair<DateTime, decimal> interestValue in expense.MonthlyExpectedExpenses)
                            {
                                if (IsInMonth(interestValue.Key, monthBegin, monthEnd))
                                {
                                    colExpValue = interestValue.Value;
                                }
                            }
                        }
                        row.Add(colExpValue > 0 ? colExpValue.ToString(CurrencyDisplayFormat) : (object)"-");
                    }

                    if (showActuals)
                    {
                        if (expense.MonthlyExpenses != null)
                        {
                            foreach (KeyValuePair<DateTime, decimal> interestValue in expense.MonthlyExpenses)
                            {
                                if (IsInMonth(interestValue.Key, monthBegin, monthEnd))
                                {
                                    colValue = interestValue.Value;
                                }
                            }
                        }
                        row.Add(colValue > 0 ? colValue.ToString(CurrencyDisplayFormat) : (object)"-");
                    }
                }
                colValue = 0M;
                colExpValue = 0M;
                if (showEstimated)
                {
                    row.Add(expense.TotalExpectedAmount.ToString(CurrencyDisplayFormat));
                }
                if (showActuals)
                {
                    row.Add(expense.TotalExpense.ToString(CurrencyDisplayFormat));
                }
                data.Rows.Add(row.ToArray());
            }
            return data;
        }

        protected void lnkExpenseType_OnClick(object sender, EventArgs e)
        {
            var lnkExpenseType = sender as LinkButton;
            int expenseTypeId;
            int.TryParse(lnkExpenseType.Attributes["ExpenseTypeId"], out expenseTypeId);
            SelectedExpenseTypeId = expenseTypeId;

            var list = ServiceCallers.Custom.Report(r => r.ExpenseDetailReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, projectId, SelectedExpenseTypeId)).ToList();

            ucExpenseDetailsByExpenseType.Visible = true;
            ucExpenseDetailsByExpenseType.IsByProject = false;
            ucExpenseDetailsByExpenseType.DataBindByExpenseDetail(list.OrderBy(o => o.MonthStartDate).ToList());
            ltrlExpenseType.Text = "<b class=\"colorGray\">" + lnkExpenseType.Text + "</b>";

            this.PopulateData();
            mpeExpenseTypeDetailReport.Show();
        }

    }
}
