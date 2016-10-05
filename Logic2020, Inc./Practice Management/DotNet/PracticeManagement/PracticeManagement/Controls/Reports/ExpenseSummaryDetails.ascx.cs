using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using DataTransferObjects;
using PraticeManagement.Utils.Excel;
using System.Data;
using PraticeManagement.Utils;
using System.Web.Script.Serialization;
using AjaxControlToolkit;

namespace PraticeManagement.Controls.Reports
{
    public partial class ExpenseSummaryDetails : System.Web.UI.UserControl
    {
        private int headerRowsCount = 1;
        private int coloumnsCount = 1;
        private const string CurrencyDisplayFormat = "$###,###,###,###,###,##0.##";

        private List<ExpenseSummary> DetailedReportData
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

        private List<string> CollapsiblePanelExtenderClientIds
        {
            get;
            set;
        }


        public bool ByProject
        {
            get;
            set;
        }
        public bool ByExpense
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

                List<CellStyles> dataCellStylearray = new List<CellStyles> { dataStyle, dataStyle, dataStyle, dataStyle, dataStartDateCellStyle, dataStartDateCellStyle, dataStyle, dataNumberDateCellStyle };
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
        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "HidePanel();", true);
        }
        public void PopulateData()
        {
            List<ExpenseSummary> report = null;

            report = ServiceCallers.Custom.Report(r => r.DetailedExpenseSummary(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.AccountsSelected, HostingPage.DivisionsSelected, HostingPage.PracticesSelected, HostingPage.ProjectsSelected, HostingPage.showActive, HostingPage.showProjected, HostingPage.showCompleted, HostingPage.showProposed, HostingPage.showInActive, HostingPage.showExperimental, null)).ToList();

            BindDetailedReport(report, true);
        }

        public void PopulateExpenseData()
        {
            List<ExpenseSummary> report = null;
            report = ServiceCallers.Custom.Report(r => r.DetailedExpenseSummary(HostingPage.StartDate.Value, HostingPage.EndDate.Value, null, null, null, null, true, true, true, true, true, true, HostingPage.ExpenseTypesSelected)).ToList();
            BindDetailedReport(report, false);
        }

        private void BindDetailedReport(List<ExpenseSummary> reportData, bool isByProject)
        {
            if (reportData != null && reportData.Count() > 0)
            {
                DetailedReportData = reportData;
                divEmptyMessage.Style["display"] = "none";
                repExpense.Visible = true;
                btnExpand.Disabled = false;
                btnExportToExcel.Enabled = true;

                IsByProject = isByProject;

                List<ProjectStartDate> data = new List<ProjectStartDate>();

                if (IsByProject)
                {
                    foreach (var d in reportData.Select(r => r.Project.Id).Distinct().ToList())
                    {
                        var name = reportData.FirstOrDefault(r => r.Project.Id == d);
                        data.Add(new ProjectStartDate { Id = d, HtmlEncodedName = name.Project.ProjectNumber + "-" + name.Project.HtmlEncodedName });

                    }
                }
                else
                {
                    foreach (var d in reportData.Select(r => r.Expense.Type.Id).Distinct().ToList())
                    {
                        var name = reportData.FirstOrDefault(r => r.Expense.Type.Id == d);
                        data.Add(new ProjectStartDate { Id = d, HtmlEncodedName = name.Expense.Type.Name });
                    }
                }

                repExpense.DataSource = data.OrderBy(o => o.HtmlEncodedName).ToList(); ;
                repExpense.DataBind();
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repExpense.Visible = false;
                btnExpand.Disabled = true;
                btnExportToExcel.Enabled = false;
            }
        }

        protected void repExpense_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var repMonths = e.Item.FindControl("repMonth") as Repeater;
                var lblProjectName = e.Item.FindControl("lblProjectName") as Repeater;

                ProjectStartDate dataitem = (ProjectStartDate)e.Item.DataItem;
                List<ProjectStartDate> data = new List<ProjectStartDate>();

                if (IsByProject)
                {
                    foreach (var d in DetailedReportData.Where(r => r.Project.Id == dataitem.Id).Select(s => s.MonthStartDate).Distinct())
                    {
                        data.Add(new ProjectStartDate { Id = dataitem.Id, StartDate = d });
                    }
                }
                else
                {
                    foreach (var d in DetailedReportData.Where(r => r.Expense.Type.Id == dataitem.Id).Select(s => s.MonthStartDate).Distinct())
                    {
                        data.Add(new ProjectStartDate { Id = dataitem.Id, StartDate = d });
                    }
                }
                repMonths.DataSource = data;
                repMonths.DataBind();
            }
        }


        protected void repMonth_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lblMonthName = e.Item.FindControl("lblMonthName") as Label;
                var repexpenseDetails = e.Item.FindControl("repMilestoneExpenses") as Repeater;
                ProjectStartDate dataitem = (ProjectStartDate)e.Item.DataItem;

                lblMonthName.Text = dataitem.StartDate.ToString(Constants.Formatting.CompPerfMonthYearFormat);
                if (IsByProject)
                {
                    repexpenseDetails.DataSource = DetailedReportData.Where(r => r.Project.Id == dataitem.Id && r.MonthStartDate == dataitem.StartDate).ToList();
                }
                else
                {
                    repexpenseDetails.DataSource = DetailedReportData.Where(r => r.Expense.Type.Id == dataitem.Id && r.MonthStartDate == dataitem.StartDate).ToList();
                }
                repexpenseDetails.DataBind();
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
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "", "ShowOrHideColumns('" + HostingPage.ShowActualExpense + "','" + HostingPage.ShowEstimatedExpense + "');", true);
        }

        protected string GetDateFormat(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty;
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage("Expense Summary Report");
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();
            List<ExpenseSummary> report = null;

            if (HostingPage.SelectedView == "0")
            {
                report = ServiceCallers.Custom.Report(r => r.DetailedExpenseSummary(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.AccountsSelected, HostingPage.DivisionsSelected, HostingPage.PracticesSelected, HostingPage.ProjectsSelected, HostingPage.showActive, HostingPage.showProjected, HostingPage.showCompleted, HostingPage.showProposed, HostingPage.showInActive, HostingPage.showExperimental, null)).ToList();
            }
            else if (HostingPage.SelectedView == "1")
            {
                report = ServiceCallers.Custom.Report(r => r.DetailedExpenseSummary(HostingPage.StartDate.Value, HostingPage.EndDate.Value, null, null, null, null, true, true, true, true, true, true, HostingPage.ExpenseTypesSelected)).ToList();
            }
            var fileName = string.Format("ExpenseSummaryDetails.xls");

            if (report.Count > 0)
            {
                DataTable header1 = new DataTable();
                header1.Columns.Add(string.Format("Expense Report"));

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
    }

    public class ProjectStartDate
    {
        public int? Id
        {
            get;
            set;
        }

        public DateTime StartDate
        {
            get;
            set;
        }

        public string HtmlEncodedName
        {
            get;
            set;
        }
    }
}
