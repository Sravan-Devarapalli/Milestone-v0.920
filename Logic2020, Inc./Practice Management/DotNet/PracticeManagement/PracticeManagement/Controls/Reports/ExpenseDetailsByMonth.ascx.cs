using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Data;
using DataTransferObjects;
using PraticeManagement.Utils.Excel;
using PraticeManagement.Utils;
using PraticeManagement.Objects;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PraticeManagement.Controls.Reports
{
    public partial class ExpenseDetailsByMonth : System.Web.UI.UserControl
    {
        #region fields

        private int headerRowsCount = 1;
        private int coloumnsCount = 1;
        private const string CurrencyDisplayFormat = "$###,###,###,###,###,##0.##";
        private string RowSpliter = Guid.NewGuid().ToString();
        private string ColoumSpliter = Guid.NewGuid().ToString();

        #endregion fields

        #region Properties

        public bool IsProjectExpenseDetails
        {
            get;
            set;
        }

        public bool IsExpensetypeDetails
        {
            get;
            set;
        }

        private PraticeManagement.Reports.ExpenseReport HostingPage
        {
            get
            {
                return ((PraticeManagement.Reports.ExpenseReport)Page);
            }
        }

        public int? ProjectId
        {
            get
            {
                if (Page is PraticeManagement.Reports.ExpenseReport && IsByProject)
                {
                    return ((PraticeManagement.Reports.ExpenseReport)Page).ByProjectControl.SelectedProjectId;
                }
                return null;
            }
        }

        public int? ExpensetypeId
        {
            get
            {
                if (Page is PraticeManagement.Reports.ExpenseReport && !IsByProject)
                {
                    return ((PraticeManagement.Reports.ExpenseReport)Page).ByExpenseTypeControl.SelectedExpenseTypeId;
                }
                return null;
            }
        }

        public bool IsByProject
        {
            get
            {
                return (bool)ViewState["IsByProject"];
            }
            set
            {
                ViewState["IsByProject"] = value;
            }
        }

        private List<ExpenseSummary> DetailedReportData
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

                List<CellStyles> headerCellStyleList = new List<CellStyles>() { headerCellStyle };
                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataStartDateCellStyle = new CellStyles();
                dataStartDateCellStyle.DataFormat = "mm/dd/yy;@";

                CellStyles dataNumberDateCellStyle = new CellStyles();
                dataNumberDateCellStyle.DataFormat = "$#,##0.00_);($#,##0.00)";

                CellStyles dataPercentCellStyle = new CellStyles();
                dataPercentCellStyle.DataFormat = "0.00%";

                CellStyles dataStyle = new CellStyles();

                List<CellStyles> dataCellStylearray = new List<CellStyles> { dataStyle, dataStyle, dataStyle, dataStyle, dataStartDateCellStyle, dataStartDateCellStyle, dataStyle };
                if (HostingPage.ShowEstimatedExpense || HostingPage.ShowActualExpense)
                {
                    dataCellStylearray.AddRange(new List<CellStyles> { dataNumberDateCellStyle });
                }
                if (HostingPage.ShowEstimatedExpense && HostingPage.ShowActualExpense)
                {
                    dataCellStylearray.AddRange(new List<CellStyles> { dataNumberDateCellStyle, dataNumberDateCellStyle, dataPercentCellStyle, dataNumberDateCellStyle });
                }
                else
                {
                    dataCellStylearray.AddRange(new List<CellStyles> { dataPercentCellStyle, dataNumberDateCellStyle });
                }
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

        private TableStyles _PdfProjectListTableStyle;

        private TableStyles PdfProjectListTableStyle
        {
            get
            {
                if (_PdfProjectListTableStyle == null)
                {
                    TdStyles HeaderStyle = new TdStyles("center", true, false, 7, 1);
                    HeaderStyle.BackgroundColor = "light-gray";
                    TdStyles ContentStyle1 = new TdStyles("left", false, false, 10, 1);
                    TdStyles ContentStyle2 = new TdStyles("center", false, false, 9, 1);

                    TdStyles[] HeaderStyleArray = { HeaderStyle };
                    TdStyles[] ContentStyleArray = { ContentStyle2 };

                    TrStyles HeaderRowStyle = new TrStyles(HeaderStyleArray);
                    TrStyles ContentRowStyle = new TrStyles(ContentStyleArray);

                    TrStyles[] RowStyleArray = { HeaderRowStyle, ContentRowStyle };
                    float[] widths = { 0.075f, 0.09f, 0.09f, 0.1f, 0.1f, 0.075f, 0.09f, 0.09f, 0.1f, 0.1f, 0.1f, 0.09f };
                    _PdfProjectListTableStyle = new TableStyles(widths, RowStyleArray, 100, "custom", new int[] { 245, 250, 255 });
                    _PdfProjectListTableStyle.IsColoumBorders = false;
                }
                return _PdfProjectListTableStyle;
            }
        }

        #endregion Properties

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void DataBindByExpenseDetail(List<ExpenseSummary> reportData)
        {
            if (reportData.Count > 0)
            {
                DetailedReportData = reportData;
                var data = reportData.Select(s => s.MonthStartDate).Distinct();
                repMonths.Visible = true;
                repMonths.DataSource = data;
                repMonths.DataBind();
                btnExportToPDF.Enabled =
                    btnExportToExcel.Enabled = true;
                divEmptyMessage.Style["display"] = "none";
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
            }
        }

        protected void repMilestoneExpenses_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lblExpenseName = e.Item.FindControl("lblExpenseName") as Label;
                var lblReimburse = e.Item.FindControl("lblReimburse") as Label;
                ExpenseSummary dataitem = (ExpenseSummary)e.Item.DataItem;
                lblExpenseName.Text = IsByProject ? dataitem.Expense.MilestoneName + " > " + dataitem.Expense.HtmlEncodedName : dataitem.Project.ProjectNumber + " > " + dataitem.Expense.MilestoneName + " > " + dataitem.Expense.HtmlEncodedName;
                lblReimburse.Text = HostingPage.ShowEstimatedExpense && !HostingPage.ShowActualExpense ? dataitem.Expense.EstimatedReimbursementAmount.ToString("###,###,###,###,###,##0.##") : dataitem.Expense.ReimbursementAmount.ToString("###,###,###,###,###,##0.##");
            }
            if (e.Item.ItemType == ListItemType.Header)
            {
                var lblExpenseNameHeader = e.Item.FindControl("lblProjectNameHeader") as Label;
                lblExpenseNameHeader.Text = IsByProject ? "Milestone Name> Expense Name" : "Project Number> Milestone Name> Expense Name";
            }

        }

        protected string GetDateFormat(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty;
        }

        protected void repMonths_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lblMonthName = e.Item.FindControl("lblMonthName") as Label;
                var repexpenseDetails = e.Item.FindControl("repMilestoneExpenses") as Repeater;
                DateTime dataitem = (DateTime)e.Item.DataItem;

                lblMonthName.Text = dataitem.ToString(Constants.Formatting.CompPerfMonthYearFormat);
                if (ProjectId.HasValue)
                {
                    repexpenseDetails.DataSource = DetailedReportData.Where(r => r.Project.Id == ProjectId && r.MonthStartDate == dataitem).ToList();
                }
                else if (ExpensetypeId.HasValue)
                {
                    repexpenseDetails.DataSource = DetailedReportData.Where(r => r.Expense.Type.Id == ExpensetypeId.Value && r.MonthStartDate == dataitem).ToList();
                }

                repexpenseDetails.DataBind();
            }
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "", "ShowOrHideColumns('" + HostingPage.ShowActualExpense + "','" + HostingPage.ShowEstimatedExpense + "');", true);
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage("Expense Report");
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();

            var report = ServiceCallers.Custom.Report(r => r.ExpenseDetailReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, ProjectId, ExpensetypeId)).ToList();

            Project project = ProjectId.HasValue ? report.FirstOrDefault().Project : null;
            ExpenseType expenseType = ExpensetypeId.HasValue ? report.FirstOrDefault().Expense.Type : null;
            var fileName = string.Format("{0}_ExpenseSummary.xls", project != null ? project.ProjectNumber + "-" + project.Name : "Expense Type");

            if (report.Count > 0)
            {
                DataTable header1 = new DataTable();
                header1.Columns.Add(string.Format("Expense Report-{0}", project != null ? project.ProjectNumber + "-" + project.Name : expenseType.Name));

                List<object> row1 = new List<object>();
                row1.Add(string.Format("{0}-{1}", HostingPage.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat), HostingPage.EndDate.Value.ToString(Constants.Formatting.EntryDateFormat)));
                header1.Rows.Add(row1.ToArray());
                headerRowsCount = header1.Rows.Count + 3;

                DataTable data;

                data = PrepareDataTable(report.OrderBy(o => o.MonthStartDate).ToList());
                coloumnsCount = data.Columns.Count;
                var dataset = new DataSet();
                dataset.DataSetName = "ExpenseByMonth_Report";
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
                dataset.DataSetName = "ExpenseByMonth_Report";
                dataset.Tables.Add(header);
                dataSetList.Add(dataset);
            }

            NPOIExcel.Export(fileName, dataSetList, sheetStylesList);
        }

        public DataTable PrepareDataTable(List<ExpenseSummary> report)
        {
            bool showActuals = HostingPage.ShowActualExpense;
            bool showEstimated = HostingPage.ShowEstimatedExpense;

            DataTable data = new DataTable();
            List<object> row;
            data.Columns.Add("Project Number");
            data.Columns.Add("Milestone");
            data.Columns.Add("Expense Name");
            data.Columns.Add("Expense Type");
            data.Columns.Add("Start Date");
            data.Columns.Add("End Date");
            data.Columns.Add("Month");
            if (showEstimated)
            {
                data.Columns.Add("Estimated Expense");
            }
            if (showActuals)
            {
                data.Columns.Add("Actual Expense");
            }
            if (showActuals && showEstimated)
            {
                data.Columns.Add("Difference");
            }
            data.Columns.Add("Reimbursement %");
            data.Columns.Add("Reimburse Amount");

            foreach (var expense in report)
            {
                row = new List<object>();
                row.Add(expense.Project.ProjectNumber);
                row.Add(expense.Expense.MilestoneName);
                row.Add(expense.Expense.Name);
                row.Add(expense.Expense.Type.Name);
                row.Add(expense.Expense.StartDate.ToString(Constants.Formatting.EntryDateFormat));
                row.Add(expense.Expense.EndDate.ToString(Constants.Formatting.EntryDateFormat));
                row.Add(expense.MonthStartDate.ToString(Constants.Formatting.CompPerfMonthYearFormat));
                if (showEstimated)
                {
                    row.Add(expense.Expense.ExpectedAmount.ToString(CurrencyDisplayFormat));
                }
                if (showActuals)
                {
                    row.Add(expense.Expense.Amount.ToString(CurrencyDisplayFormat));
                }
                if (showActuals && showEstimated)
                {
                    row.Add(expense.Expense.Difference.ToString(CurrencyDisplayFormat));
                }
                row.Add(expense.Expense.Reimbursement / 100);
                row.Add(showEstimated && !showActuals ? expense.Expense.EstimatedReimbursementAmount.ToString(CurrencyDisplayFormat) : expense.Expense.ReimbursementAmount.ToString(CurrencyDisplayFormat));
                data.Rows.Add(row.ToArray());
            }
            return data;
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {
            PDFExport();
        }

        public void PDFExport()
        {
            var data = ServiceCallers.Custom.Report(r => r.ExpenseDetailReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, ProjectId, ExpensetypeId)).ToList();
            data = data.OrderBy(o => o.MonthStartDate).ToList();
            HtmlToPdfBuilder builder = new HtmlToPdfBuilder(iTextSharp.text.PageSize.A4_LANDSCAPE);
            string filename = "ExpenseByProject.pdf";
            byte[] pdfDataInBytes = this.RenderPdf(builder, data);

            HttpContext.Current.Response.ContentType = "Application/pdf";
            HttpContext.Current.Response.AddHeader(
                "content-disposition", string.Format("attachment; filename={0}", Utils.Generic.EncodedFileName(filename)));

            int len = pdfDataInBytes.Length;
            int bytes;
            byte[] buffer = new byte[1024];
            Stream outStream = HttpContext.Current.Response.OutputStream;
            using (MemoryStream stream = new MemoryStream(pdfDataInBytes))
            {
                while (len > 0 && (bytes = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outStream.Write(buffer, 0, bytes);
                    HttpContext.Current.Response.Flush();
                    len -= bytes;
                }
            }
        }

        private byte[] RenderPdf(HtmlToPdfBuilder builder, List<ExpenseSummary> expenses)
        {
            int pageCount = GetPageCount(builder, expenses);
            MemoryStream file = new MemoryStream();
            Document document = new Document(builder.PageSize);
            document.SetPageSize(iTextSharp.text.PageSize.A4_LANDSCAPE.Rotate());

            MyPageEventHandler e = new MyPageEventHandler()
            {
                PageCount = pageCount,
                PageNo = 1
            };
            PdfWriter writer = PdfWriter.GetInstance(document, file);
            writer.PageEvent = e;
            document.Open();
            var styles = new List<TrStyles>();
            if (expenses.Count > 0)
            {
                string reportDataInPdfString = string.Empty;

                if (HostingPage.ShowActualExpense && !HostingPage.ShowEstimatedExpense)
                {
                    reportDataInPdfString += string.Format("Project Number{0}Milestone{0}Expense Name{0}Expense Type{0}Start Date{0}End Date{0}Month{0}Actual Expense{0}Reimbursement %{0}Reimburse Amount{1}", ColoumSpliter, RowSpliter);

                    foreach (var expense in expenses)
                    {
                        reportDataInPdfString += String.Format("{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{1}", ColoumSpliter, RowSpliter, expense.Project.ProjectNumber, expense.Expense.MilestoneName, expense.Expense.HtmlEncodedName,
                       expense.Expense.Type.Name, expense.Expense.StartDate.ToString(Constants.Formatting.EntryDateFormat), expense.Expense.EndDate.ToString(Constants.Formatting.EntryDateFormat),
                       expense.MonthStartDate.ToString(Constants.Formatting.CompPerfMonthYearFormat), expense.Expense.Amount.ToString(CurrencyDisplayFormat), expense.Expense.Reimbursement, expense.Expense.ReimbursementAmount.ToString(CurrencyDisplayFormat));
                    }
                }
                if (HostingPage.ShowEstimatedExpense && !HostingPage.ShowActualExpense)
                {
                    reportDataInPdfString += string.Format("Project Number{0}Milestone{0}Expense Name{0}Expense Type{0}Start Date{0}End Date{0}Month{0}Estimated Expense{0}Reimbursement %{0}Reimburse Amount{1}", ColoumSpliter, RowSpliter);

                    foreach (var expense in expenses)
                    {
                        reportDataInPdfString += String.Format("{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{1}", ColoumSpliter, RowSpliter, expense.Project.ProjectNumber, expense.Expense.MilestoneName, expense.Expense.HtmlEncodedName,
                       expense.Expense.Type.Name, expense.Expense.StartDate.ToString(Constants.Formatting.EntryDateFormat), expense.Expense.EndDate.ToString(Constants.Formatting.EntryDateFormat),
                       expense.MonthStartDate.ToString(Constants.Formatting.CompPerfMonthYearFormat), expense.Expense.ExpectedAmount.ToString(CurrencyDisplayFormat), expense.Expense.Reimbursement, expense.Expense.EstimatedReimbursementAmount.ToString(CurrencyDisplayFormat));
                    }
                }
                if (HostingPage.ShowActualExpense && HostingPage.ShowEstimatedExpense)
                {
                    reportDataInPdfString += string.Format("Project Number{0}Milestone{0}Expense Name{0}Expense Type{0}Start Date{0}End Date{0}Month{0}Estimated Expense{0}Actual Expense{0}Difference{0}Reimbursement %{0}Reimburse Amount{1}", ColoumSpliter, RowSpliter);

                    foreach (var expense in expenses)
                    {
                        reportDataInPdfString += String.Format("{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{1}", ColoumSpliter, RowSpliter, expense.Project.ProjectNumber, expense.Expense.MilestoneName, expense.Expense.HtmlEncodedName,
                       expense.Expense.Type.Name, expense.Expense.StartDate.ToString(Constants.Formatting.EntryDateFormat), expense.Expense.EndDate.ToString(Constants.Formatting.EntryDateFormat),
                       expense.MonthStartDate.ToString(Constants.Formatting.CompPerfMonthYearFormat), expense.Expense.ExpectedAmount.ToString(CurrencyDisplayFormat), expense.Expense.Amount.ToString(CurrencyDisplayFormat), expense.Expense.Difference.ToString(CurrencyDisplayFormat), expense.Expense.Reimbursement, expense.Expense.ReimbursementAmount.ToString(CurrencyDisplayFormat));
                    }
                }
                else
                {
                    reportDataInPdfString += string.Format("Project Number{0}Milestone{0}Expense Name{0}Expense Type{0}start date{0}End Date{0}month{0}Reimbursement %{0}Reimbursement Amount{1}", ColoumSpliter, RowSpliter);

                    foreach (var expense in expenses)
                    {
                        reportDataInPdfString += String.Format("{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{1}", ColoumSpliter, RowSpliter, expense.Project.ProjectNumber, expense.Expense.MilestoneName, expense.Expense.HtmlEncodedName,
                       expense.Expense.Type.Name, expense.Expense.StartDate.ToString(Constants.Formatting.EntryDateFormat), expense.Expense.EndDate.ToString(Constants.Formatting.EntryDateFormat),
                       expense.MonthStartDate.ToString(Constants.Formatting.CompPerfMonthYearFormat), expense.Expense.Reimbursement, expense.Expense.ReimbursementAmount.ToString(CurrencyDisplayFormat));
                    }
                }

                var table = builder.GetPdftable(reportDataInPdfString, PdfProjectListTableStyle, RowSpliter, ColoumSpliter);

                document.Add((IElement)table);
            }
            else
            {
                document.Add((IElement)PDFHelper.GetPdfHeaderLogo());
            }
            document.Close();
            return file.ToArray();
        }

        private int GetPageCount(HtmlToPdfBuilder builder, List<ExpenseSummary> Expenses)
        {
            MemoryStream file = new MemoryStream();
            Document document = new Document(builder.PageSize);
            document.SetPageSize(iTextSharp.text.PageSize.A4_LANDSCAPE.Rotate());
            MyPageEventHandler e = new MyPageEventHandler()
            {
                PageCount = 0,
                PageNo = 1
            };
            PdfWriter writer = PdfWriter.GetInstance(document, file);
            writer.PageEvent = e;
            document.Open();
            var styles = new List<TrStyles>();
            if (Expenses.Count > 0)
            {
                string reportDataInPdfString = string.Empty;

                if (HostingPage.ShowActualExpense && !HostingPage.ShowEstimatedExpense)
                {
                    reportDataInPdfString += string.Format("Project Number{0}Milestone{0}Expense Name{0}Expense Type{0}start date{0}End Date{0}month{0}Actual Expense{0}Reimbursement %{0}Reimbursement Amount{1}", ColoumSpliter, RowSpliter);

                    foreach (var expense in Expenses)
                    {
                        reportDataInPdfString += String.Format("{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{1}", ColoumSpliter, RowSpliter, expense.Project.ProjectNumber, expense.Expense.MilestoneName, expense.Expense.HtmlEncodedName,
                       expense.Expense.Type.Name, expense.Expense.StartDate.ToString(Constants.Formatting.EntryDateFormat), expense.Expense.EndDate.ToString(Constants.Formatting.EntryDateFormat),
                       expense.MonthStartDate.ToString(Constants.Formatting.CompPerfMonthYearFormat), expense.Expense.Amount.ToString(CurrencyDisplayFormat), expense.Expense.Reimbursement, expense.Expense.ReimbursementAmount.ToString(CurrencyDisplayFormat));
                    }
                }
                if (HostingPage.ShowEstimatedExpense && !HostingPage.ShowActualExpense)
                {
                    reportDataInPdfString += string.Format("Project Number{0}Milestone{0}Expense Name{0}Expense Type{0}start date{0}End Date{0}month{0}Estimated Expense{0}Reimbursement %{0}Reimbursement Amount{1}", ColoumSpliter, RowSpliter);

                    foreach (var expense in Expenses)
                    {
                        reportDataInPdfString += String.Format("{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{1}", ColoumSpliter, RowSpliter, expense.Project.ProjectNumber, expense.Expense.MilestoneName, expense.Expense.HtmlEncodedName,
                       expense.Expense.Type.Name, expense.Expense.StartDate.ToString(Constants.Formatting.EntryDateFormat), expense.Expense.EndDate.ToString(Constants.Formatting.EntryDateFormat),
                       expense.MonthStartDate.ToString(Constants.Formatting.CompPerfMonthYearFormat), expense.Expense.ExpectedAmount.ToString(CurrencyDisplayFormat), expense.Expense.Reimbursement, expense.Expense.EstimatedReimbursementAmount.ToString(CurrencyDisplayFormat));
                    }
                }
                if (HostingPage.ShowActualExpense && HostingPage.ShowEstimatedExpense)
                {
                    reportDataInPdfString += string.Format("Project Number{0}Milestone{0}Expense Name{0}Expense Type{0}start date{0}End Date{0}month{0}Estimated Expense{0}Actual Expense{0}Difference{0}Reimbursement %{0}Reimbursement Amount{1}", ColoumSpliter, RowSpliter);

                    foreach (var expense in Expenses)
                    {
                        reportDataInPdfString += String.Format("{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{1}", ColoumSpliter, RowSpliter, expense.Project.ProjectNumber, expense.Expense.MilestoneName, expense.Expense.HtmlEncodedName,
                       expense.Expense.Type.Name, expense.Expense.StartDate.ToString(Constants.Formatting.EntryDateFormat), expense.Expense.EndDate.ToString(Constants.Formatting.EntryDateFormat),
                       expense.MonthStartDate.ToString(Constants.Formatting.CompPerfMonthYearFormat), expense.Expense.ExpectedAmount.ToString(CurrencyDisplayFormat), expense.Expense.Amount.ToString(CurrencyDisplayFormat), expense.Expense.Difference.ToString(CurrencyDisplayFormat), expense.Expense.Reimbursement, expense.Expense.ReimbursementAmount.ToString(CurrencyDisplayFormat));
                    }
                }
                else
                {
                    reportDataInPdfString += string.Format("Project Number{0}Milestone{0}Expense Name{0}Expense Type{0}start date{0}End Date{0}month{0}Reimbursement %{0}Reimbursement Amount{1}", ColoumSpliter, RowSpliter);

                    foreach (var expense in Expenses)
                    {
                        reportDataInPdfString += String.Format("{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{1}", ColoumSpliter, RowSpliter, expense.Project.ProjectNumber, expense.Expense.MilestoneName, expense.Expense.HtmlEncodedName,
                       expense.Expense.Type.Name, expense.Expense.StartDate.ToString(Constants.Formatting.EntryDateFormat), expense.Expense.EndDate.ToString(Constants.Formatting.EntryDateFormat),
                       expense.MonthStartDate.ToString(Constants.Formatting.CompPerfMonthYearFormat), expense.Expense.Reimbursement, expense.Expense.ReimbursementAmount.ToString(CurrencyDisplayFormat));
                    }
                }

                var table = builder.GetPdftable(reportDataInPdfString, PdfProjectListTableStyle, RowSpliter, ColoumSpliter);
                document.Add((IElement)table);
            }
            else
            {
                document.Add((IElement)PDFHelper.GetPdfHeaderLogo());
            }
            return writer.CurrentPageNumber;
        }
    }
}
