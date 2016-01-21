using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;
using PraticeManagement.Security;
using System.Collections;
using System.Web.Security;
using PraticeManagement.Utils.Excel;
using System.Data;
using PraticeManagement.Utils;
using DataTransferObjects.Filters;

namespace PraticeManagement.Controls.Reports
{
    public partial class BenchCosts : ProjectsReportsBase
    {
        private const string ReportFormat = "{0}&nbsp;{1}";
        private const string ConsultantNameSortOrder = "ConsultantNameSortOrder";
        private const string PracticeSortOrder = "PracticeSortOrder";
        private const string StatusSortOrder = "StatusSortOrder";
        private const string Descending = "Descending";
        private const string Ascending = "Ascending";
        private const string ReportContextKey = "ReportContext";
        private const string BenchListKey = "BenchList";
        private const string UserIsAdminKey = "UserIsAdmin";
        private const string CompPerfDataWithPaddingRightCssClass = "CompPerfData padRight10Imp textRight";
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;
        private const string BenchReportExport = "Bench Report";
        private Dictionary<DateTime, Decimal> monthlyTotals;
        private const string BenchNegativeFormat = "({0})";
        private const string BenchReportHeader = "Bench Report-({0}-{1})";
        private const string Legend1 = "Person was hired during {0} month.";
        private const string Legend2 = "Person was terminated during {0} month.";
        private const string Legend3 = "Person was changed from salaried to hourly compensation during {0} month.";
        private const string Legend4 = "Person was changed from hourly to salaried compensation during {0} month. "; 

        private BenchReportContext ReportContext
        {
            get
            {
                if (ViewState[ReportContextKey] == null)
                {
                    BenchReportContext reportContext = new BenchReportContext()
                    {
                        Start = BegPeriod,
                        End = EndPeriod,
                        ActiveProjects = chbActiveProjects.Checked,
                        ProjectedProjects = chbProjectedProjects.Checked,
                        ExperimentalProjects = chbExperimentalProjects.Checked,
                        ProposedProjects = chbProposed.Checked,
                        CompletedProjects = chbCompletedProjects.Checked,
                        UserName = DataHelper.CurrentPerson.Alias,
                        PracticeIds = string.IsNullOrEmpty(cblPractices.SelectedItems) ? string.Empty : cblPractices.SelectedItems,
                        IncludeOverheads = chbIncludeOverHeads.Checked,
                        IncludeZeroCostEmployees = chbIncludeZeroCostEmps.Checked,
                    };
                    ViewState[ReportContextKey] = reportContext;
                }
                return ViewState[ReportContextKey] as BenchReportContext;
            }
            set
            {
                ViewState[ReportContextKey] = value;
            }
        }

        public DateTime BegPeriod
        {
            get
            {
                DateTime currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, Constants.Dates.FirstDay);
                var periodSelected = int.Parse(ddlPeriod.SelectedValue);

                if (periodSelected > 0)
                {
                    DateTime startMonth = new DateTime();


                    if (periodSelected < 13)
                    {
                        startMonth = currentMonth;
                    }
                    else
                    {
                        Dictionary<string, DateTime> fPeriod = DataHelper.GetFiscalYearPeriod(currentMonth);
                        startMonth = fPeriod["StartMonth"];
                    }
                    return startMonth;

                }
                else if (periodSelected < 0)
                {
                    DateTime startMonth = new DateTime();

                    if (periodSelected > -13)
                    {
                        startMonth = currentMonth.AddMonths(Convert.ToInt32(ddlPeriod.SelectedValue));
                    }
                    else
                    {
                        Dictionary<string, DateTime> fPeriod = DataHelper.GetFiscalYearPeriod(currentMonth.AddYears(-1));
                        startMonth = fPeriod["StartMonth"];
                    }
                    return startMonth;
                }
                else
                {
                    return diRange.FromDate.Value;
                }
            }
        }

        public DateTime EndPeriod
        {
            get
            {
                DateTime currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, Constants.Dates.FirstDay);
                var periodSelected = int.Parse(ddlPeriod.SelectedValue);

                if (periodSelected > 0)
                {
                    DateTime endMonth = new DateTime();

                    if (periodSelected < 13)
                    {
                        endMonth = currentMonth.AddMonths(Convert.ToInt32(ddlPeriod.SelectedValue) - 1);
                    }
                    else
                    {
                        Dictionary<string, DateTime> fPeriod = DataHelper.GetFiscalYearPeriod(currentMonth);
                        endMonth = fPeriod["EndMonth"];
                    }
                    return new DateTime(endMonth.Year, endMonth.Month, DateTime.DaysInMonth(endMonth.Year, endMonth.Month));
                }
                else if (periodSelected < 0)
                {
                    DateTime endMonth = new DateTime();

                    if (periodSelected > -13)
                    {
                        endMonth = currentMonth.AddMonths(Convert.ToInt32(ddlPeriod.SelectedValue));
                    }
                    else
                    {
                        Dictionary<string, DateTime> fPeriod = DataHelper.GetFiscalYearPeriod(currentMonth.AddYears(-1));
                        endMonth = fPeriod["EndMonth"];
                    }
                    return new DateTime(endMonth.Year, endMonth.Month, DateTime.DaysInMonth(endMonth.Year, endMonth.Month));
                }
                else
                {
                    return diRange.ToDate.Value;
                }
            }
        }

        private IEnumerable<Project> BenchList
        {
            get
            {
                if (ViewState[BenchListKey] == null)
                {
                    var benchList = GetBenchList();
                    ViewState[BenchListKey] = benchList;
                }
                return ViewState[BenchListKey] as IEnumerable<Project>;
            }
            set
            {
                ViewState[BenchListKey] = value;
            }
        }

        private Dictionary<DateTime, Decimal> MonthlyTotals
        {
            get
            {
                if (monthlyTotals == null)
                {
                    monthlyTotals = new Dictionary<DateTime, decimal>();
                }
                return monthlyTotals;
            }
            set
            {
                monthlyTotals = value;
            }

        }

        private bool UserIsAdmin
        {
            get
            {
                if (ViewState[UserIsAdminKey] == null)
                {
                    ViewState[UserIsAdminKey] = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
                }
                return bool.Parse(ViewState[UserIsAdminKey].ToString());
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
                dataCellStyle.IsBold = true;
                dataCellStyle.BorderStyle = NPOI.SS.UserModel.BorderStyle.None;
                dataCellStyle.FontHeight = 200;
                CellStyles[] dataCellStylearray = { dataCellStyle };
                RowStyles datarowStyle = new RowStyles(dataCellStylearray);
                datarowStyle.Height = 350;

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
                var headerCellStyle = new CellStyles
                {
                    IsBold = true,
                    HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center
                };
                var headerDateCellStyle = new CellStyles
                {
                    IsBold = true,
                    HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center,
                    DataFormat = "[$-409]mmmm-yy;@"
                };
                var headerCellStyleList = new List<CellStyles> { headerCellStyle, headerCellStyle, headerCellStyle, headerDateCellStyle };
                var headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                var dataCellStyle = new CellStyles();
                dataCellStyle.WrapText = true;
                var currencyDataCellStyle = new CellStyles();
                currencyDataCellStyle.DataFormat = "$#,##0.00_);($#,##0.00)";

                List<CellStyles> dataCellStylearray = new List<CellStyles> { dataCellStyle, 
                                                    dataCellStyle,
                                                    dataCellStyle
                                                  };
                for (var i = 0; i < GetPeriodLength(); i++)
                {
                    dataCellStylearray.Add(currencyDataCellStyle);
                }
                dataCellStylearray.Add(currencyDataCellStyle);
                dataCellStylearray.Add(dataCellStyle);

                var datarowStyle = new RowStyles(dataCellStylearray.ToArray());
                RowStyles[] rowStylearray = { headerrowStyle};
                var sheetStyle = new SheetStyles(rowStylearray)
                {
                    TopRowNo = headerRowsCount,
                    IsFreezePane = true,
                    FreezePanColSplit = 0,
                    FreezePanRowSplit = headerRowsCount
                };

                return sheetStyle;
            }
        }

        private SheetStyles LegendDataSheetStyle
        {
            get
            {
                var headerCellStyle = new CellStyles
                {
                    IsBold = true,
                    HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center
                };

                var headerCellStyleList = new List<CellStyles> { headerCellStyle };
                var headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                var dataCellStyle = new CellStyles();

                List<CellStyles> dataCellStylearray = new List<CellStyles> { dataCellStyle };

                var datarowStyle = new RowStyles(dataCellStylearray.ToArray());
                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                var sheetStyle = new SheetStyles(rowStylearray)
                {
                    TopRowNo = headerRowsCount
                };

                return sheetStyle;
            }
        }

        private List<int> SheetHeightForExternalPractice
        {
            get;
            set;
        }

        private List<int> SheetHeightForInternalPractice
        {
            get;
            set;
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            var filename = string.Format("BenchReport_{0}-{1}.xls",
                BegPeriod.ToString("MM_dd_yyyy"), EndPeriod.ToString("MM_dd_yyyy"));
            DataHelper.InsertExportActivityLogMessage(BenchReportExport);
            int dataRowsCount = 0;
            var sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();
            var benchList = BenchList.OrderBy(P => P.Name).ToList();
            if (benchList.Count > 0)
            {
                var header = new DataTable();
                header.Columns.Add(string.Format(BenchReportHeader, BegPeriod.ToString(Constants.Formatting.EntryDateFormat), EndPeriod.ToString(Constants.Formatting.EntryDateFormat)));
                List<object> row1 = new List<object>();
                row1.Add("Ran on " + DateTime.Today.ToString(Constants.Formatting.EntryDateFormat));
                header.Rows.Add(row1.ToArray());

                List<object> row3 = new List<object>();
                row3.Add(chbIncludeOverHeads.Checked ? "Include Overheads in Calculations" : "Exclude Overheads in Calculations");
                header.Rows.Add(row3.ToArray());

                if (chbSeperateInternalExternal.Checked)
                {
                    List<object> row2 = new List<object>();
                    row2.Add("Persons with External Practices");
                    header.Rows.Add(row2.ToArray());
                }
                headerRowsCount = header.Rows.Count + 3;

                var data = PrepareDataTable(chbSeperateInternalExternal.Checked ? benchList.ToList().FindAll(P => !P.Practice.IsCompanyInternal) : benchList, true);
                coloumnsCount = data.Columns.Count;
                dataRowsCount = data.Rows.Count;
                var dataStyle = DataSheetStyle;

                
                var rowStylesList = dataStyle.rowStyles.ToList();
                for (int i = 0; i < SheetHeightForExternalPractice.Count; i++)
                {
                    var dataCellStyle = new CellStyles();
                    dataCellStyle.WrapText = true;
                    var currencyDataCellStyle = new CellStyles();
                    currencyDataCellStyle.DataFormat = "$#,##0.00_);($#,##0.00)";
                    List<CellStyles> dataCellStylearray = new List<CellStyles> { dataCellStyle, 
                                                    dataCellStyle,
                                                    dataCellStyle
                                                  };
                    for (var j = 0; j < GetPeriodLength(); j++)
                    {
                        dataCellStylearray.Add(currencyDataCellStyle);
                    }
                    dataCellStylearray.Add(currencyDataCellStyle);
                    dataCellStylearray.Add(dataCellStyle);

                    var datarowStyle = new RowStyles(dataCellStylearray.ToArray());
                    datarowStyle.Height = (short)SheetHeightForExternalPractice[i];
                    rowStylesList.Add(datarowStyle);
                }
                dataStyle.rowStyles = rowStylesList.ToArray();
                var dataset = new DataSet { DataSetName = chbSeperateInternalExternal.Checked ? "BenchCosts_ExternalPractices" : "BenchCost_Report" };
                dataset.Tables.Add(header);
                dataset.Tables.Add(data);
                dataSetList.Add(dataset);
                sheetStylesList.Add(HeaderSheetStyle);
                sheetStylesList.Add(dataStyle);
                if (chbSeperateInternalExternal.Checked && benchList.ToList().FindAll(P => P.Practice.IsCompanyInternal).Count > 0)
                {
                    var internalData = PrepareDataTable(benchList.ToList().FindAll(P => P.Practice.IsCompanyInternal), false);
                    var headerInternal = new DataTable();
                    headerInternal.Columns.Add("Bench Report");

                    List<object> row11 = new List<object>();
                    row11.Add("Ran on " + DateTime.Today.ToString(Constants.Formatting.EntryDateFormat));
                    headerInternal.Rows.Add(row11.ToArray());

                    List<object> row33 = new List<object>();
                    row33.Add(chbIncludeOverHeads.Checked ? "Include Overheads in Calculations" : "Exclude Overheads in Calculations");
                    headerInternal.Rows.Add(row33.ToArray());

                    List<object> row2 = new List<object>();
                    row2.Add("Persons with Internal Practices");
                    headerInternal.Rows.Add(row2.ToArray());
                    dataRowsCount = internalData.Rows.Count;
                    var internalDataStyle = DataSheetStyle;
                    var rowStylesList_Internal = internalDataStyle.rowStyles.ToList();
                    for (int i = 0; i < SheetHeightForInternalPractice.Count; i++)
                    {
                        var dataCellStyle = new CellStyles();
                        dataCellStyle.WrapText = true;
                        var currencyDataCellStyle = new CellStyles();
                        currencyDataCellStyle.DataFormat = "$#,##0.00_);($#,##0.00)";
                        List<CellStyles> dataCellStylearray = new List<CellStyles> { dataCellStyle, 
                                                    dataCellStyle,
                                                    dataCellStyle
                                                  };
                        for (var j = 0; j < GetPeriodLength(); j++)
                        {
                            dataCellStylearray.Add(currencyDataCellStyle);
                        }
                        dataCellStylearray.Add(currencyDataCellStyle);
                        dataCellStylearray.Add(dataCellStyle);

                        var datarowStyle = new RowStyles(dataCellStylearray.ToArray());
                        datarowStyle.Height = (short)SheetHeightForInternalPractice[i];
                        rowStylesList_Internal.Add(datarowStyle);
                    }
                    internalDataStyle.rowStyles = rowStylesList_Internal.ToArray();
                    sheetStylesList.Add(HeaderSheetStyle);
                    sheetStylesList.Add(internalDataStyle);
                    var datasetInternal = new DataSet { DataSetName = "BenchCosts_InternalPractices" };
                    datasetInternal.Tables.Add(headerInternal);
                    datasetInternal.Tables.Add(internalData);
                    dataSetList.Add(datasetInternal);
                }
            }
            else
            {
                const string dateRangeTitle = "There are no people towards this range selected.";
                var header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                sheetStylesList.Add(HeaderSheetStyle);
                var dataset = new DataSet { DataSetName = "BenchCost_Report" };
                dataset.Tables.Add(header);
                dataSetList.Add(dataset);
            }

            NPOIExcel.Export(filename, dataSetList, sheetStylesList);
        }

        public void PrepareLegendTable(DataTable data)
        {
            data.Rows.Add("");
            data.Rows.Add(string.Format(NPOIExcel.CustomColorWithBoldKey, "black", "Legend"));
            data.Rows.Add(string.Format(NPOIExcel.SuperscriptKey, "black", " - Person was hired during this month.", "1", "1"));
            data.Rows.Add(string.Format(NPOIExcel.SuperscriptKey, "black", " - Person was terminated during this month.", "2", "1"));
            data.Rows.Add(string.Format(NPOIExcel.SuperscriptKey, "black", " - Person was changed from salaried to hourly compensation during this month.", "3", "1"));
            data.Rows.Add(string.Format(NPOIExcel.SuperscriptKey, "black", " - Person was changed from hourly to salaried compensation during this month.", "4", "1"));
        }

        public DataTable PrepareDataTable(List<Project> reportData, bool isExternal)
        {
            SheetHeightForExternalPractice = new List<int>();
            SheetHeightForInternalPractice = new List<int>();
            var data = new DataTable();
            var periodLength = GetPeriodLength();
            var periodStart = BegPeriod;

            data.Columns.Add("Consultant Name");
            data.Columns.Add("Practice Area");
            data.Columns.Add("Status");
            for (var i = 0; i < GetPeriodLength(); i++)
            {
                data.Columns.Add(periodStart.ToString("MMM, yyyy"));
                periodStart = periodStart.AddMonths(1);
            }
            data.Columns.Add("Grand Total");
            data.Columns.Add("Legend Note");
            foreach (var report in reportData)
            {
                string legendNote = "";
                int legendsCount = 0;
                var row = new List<object>
                    {
                        string.Format(NPOIExcel.CustomColorKey, "black",report.Name),
                        string.Format(NPOIExcel.CustomColorKey, "black",report.Practice.Name),
                        string.Format(NPOIExcel.CustomColorKey, "black",report.ProjectNumber),
                    };
                var monthBegin = BegPeriod;
                var list = new ArrayList();
                PracticeManagementCurrency lastBenchValue=new PracticeManagementCurrency();
                lastBenchValue.Value = 0M;
                for (int i = 3, k = 0; k < periodLength; i++, k++, monthBegin = monthBegin.AddMonths(1))
                {
                    var monthEnd =
                        new DateTime(monthBegin.Year,
                            monthBegin.Month,
                            DateTime.DaysInMonth(monthBegin.Year, monthBegin.Month));
                    object benchValueColor = "";
                    foreach (var interestValue in report.ProjectedFinancialsByMonth)
                    {
                        string benchValue;
                        if (IsInMonth(interestValue.Key, monthBegin, monthEnd))
                        {
                            benchValueColor = string.Format(NPOIExcel.CustomColorKey, interestValue.Value.Timescale == TimescaleType.Salary ? "red" : "black", (interestValue.Value.Timescale == TimescaleType.Salary ?
                                                            Convert.ToDecimal(interestValue.Value.GrossMargin) :
                                                            (object)Resources.Controls.HourlyLabel));
                            benchValue = (interestValue.Value.Timescale == TimescaleType.Salary ?
                                                            Convert.ToDecimal(interestValue.Value.GrossMargin).ToString() :
                                                            Resources.Controls.HourlyLabel);
                            if (!MonthlyTotals.Any(kvp => kvp.Key == monthBegin))
                            {
                                MonthlyTotals.Add(monthBegin, 0M);
                            }
                            if (interestValue.Value.Timescale == TimescaleType.Salary)
                            {
                                MonthlyTotals[monthBegin] += Convert.ToDecimal(benchValue);
                            }

                            if (interestValue.Value.Timescale == TimescaleType.Salary)
                            {
                                list.Add(Convert.ToDecimal(interestValue.Value.GrossMargin));
                                if (interestValue.Value.GrossMargin.Value == 0M)
                                {
                                    benchValue = "0";
                                    benchValueColor = string.Format(NPOIExcel.CustomColorKey, "black", 0M); //(object)string.Format(NPOIExcel.CustomColorKey, "green", 0);
                                }
                            }
                            else
                            {
                                list.Add(benchValue);
                            }
                            string superScriptContent = string.Empty;
                            if (report.StartDate.HasValue
                                && report.StartDate.Value.Year == interestValue.Key.Year
                                && report.StartDate.Value.Month == interestValue.Key.Month
                                )
                            {
                                legendsCount++;
                                superScriptContent = "1";
                                legendNote += (string.IsNullOrEmpty(legendNote) ? string.Empty : "\n") + string.Format(Legend1, interestValue.Key.ToString("MMMM-yyyy"));
                            }
                            if (report.EndDate.HasValue
                                && report.EndDate.Value.Year == interestValue.Key.Year
                                && report.EndDate.Value.Month == interestValue.Key.Month)
                            {
                                legendsCount++;
                                superScriptContent += (string.IsNullOrEmpty(superScriptContent) ? string.Empty : ",") + "2";
                                legendNote += (string.IsNullOrEmpty(legendNote) ? string.Empty : "\n") + string.Format(Legend2, interestValue.Key.ToString("MMMM-yyyy"));
                            }
                            if (interestValue.Value.TimescaleChangeStatus > 0)
                            {
                                switch (interestValue.Value.TimescaleChangeStatus)
                                {
                                    case 1: superScriptContent += (string.IsNullOrEmpty(superScriptContent) ? string.Empty : ",") + "3";
                                        legendNote += (string.IsNullOrEmpty(legendNote) ? string.Empty : "\n") + string.Format(Legend3, interestValue.Key.ToString("MMMM-yyyy"));
                                        legendsCount++;
                                        break;
                                    case 2:
                                    case 3: superScriptContent += (string.IsNullOrEmpty(superScriptContent) ? string.Empty : ",") + "4";
                                        legendNote += (string.IsNullOrEmpty(legendNote) ? string.Empty : "\n") + string.Format(Legend4, interestValue.Key.ToString("MMMM-yyyy"));
                                        legendsCount++;
                                        break;
                                    default: break;
                                }
                            }
                            if (!string.IsNullOrEmpty(superScriptContent))
                            {
                                if (interestValue.Value.Timescale == TimescaleType.Salary)
                                {
                                    if (interestValue.Value.GrossMargin.Value == 0M)
                                    {
                                        benchValueColor = string.Format(NPOIExcel.CustomColorKey, "green", 0M);
                                        //benchValueColor = string.Format(NPOIExcel.SuperscriptKey, "green", "0.00", superScriptContent, "0");
                                    }
                                    else
                                    {
                                        benchValueColor = string.Format(NPOIExcel.CustomColorKey, "red", Convert.ToDecimal(interestValue.Value.GrossMargin));
                                        //benchValueColor = string.Format(NPOIExcel.SuperscriptKey, "red", ((PracticeManagementCurrency)Convert.ToDecimal(interestValue.Value.GrossMargin)).FormattedValue(), superScriptContent, "0");
                                    }
                                }
                                else
                                {
                                    benchValueColor = string.Format(NPOIExcel.CustomColorKey, "black", (object)Resources.Controls.HourlyLabel);
                                    //benchValueColor = string.Format(NPOIExcel.SuperscriptKey, "black", (object)Resources.Controls.HourlyLabel, superScriptContent, "0");
                                }
                            }

                            if (list.OfType<Decimal>().ToList().Count <= 0)
                            {
                                lastBenchValue.Value = 0M;
                            }
                            else
                            {
                                lastBenchValue.Value = list.OfType<Decimal>().ToList().Sum();
                            }
                        }
                    }
                    row.Add(benchValueColor);
                }

                if (lastBenchValue.Value == 0M)
                {
                    row.Add(string.Format(NPOIExcel.CustomColorKey, "black", 0M));
                }
                else
                {
                    row.Add(string.Format(NPOIExcel.CustomColorKey, "red", Convert.ToDecimal(lastBenchValue)));
                }
                row.Add(string.Format(NPOIExcel.CustomColorKey, "black", legendNote));
                data.Rows.Add(row.ToArray());
                if (isExternal)
                    SheetHeightForExternalPractice.Add(legendsCount == 0 ? 300 : legendsCount * 300);
                else
                    SheetHeightForInternalPractice.Add(legendsCount == 0 ? 300 : legendsCount * 300);
            }
            List<decimal> grandTotals = GetGrandTotalValues(isExternal);
            var totalsRow = new List<object>()
                {
                    string.Format(NPOIExcel.CustomColorWithBoldKey, "black","Grand Total"),
                    "",
                    ""
                };
            for (int i = 3; i < data.Columns.Count-1; i++)
                totalsRow.Add(string.Format(NPOIExcel.CustomColorKey, "red",grandTotals[i - 3]));
            data.Rows.Add(totalsRow.ToArray());
            return data;
        }

        public List<decimal> GetGrandTotalValues(bool isExternal)
        {
            var grandTotals = new List<decimal>();
            GridView grid = isExternal ? gvBenchCosts : gvBenchCostsInternal;
            grandTotals = FooterSum(grandTotals, grid);
            MonthlyTotals = null;
            return grandTotals;
        }

        public List<decimal> FooterSum(List<decimal> grandTotals, GridView grid)
        {
            GridViewRow footer = grid.FooterRow;
            var monthBegin =
                        new DateTime(ReportContext.Start.Year,
                            ReportContext.Start.Month,
                            Constants.Dates.FirstDay);
            var perodLenth = GetPeriodLength();
            if (footer != null)
            {
                for (int i = 3; i < grid.Columns.Count; i++)
                {
                    if (i < (3 + perodLenth))
                    {
                        if (MonthlyTotals.Any(kvp => kvp.Key == monthBegin.AddMonths(i - 3)))
                        {
                            var total = MonthlyTotals[monthBegin.AddMonths(i - 3)];
                            grandTotals.Add(total);
                        }
                    }
                    else if (i == grid.Columns.Count - 1)
                    {
                        var grandTotal = MonthlyTotals.Values.Sum();
                        grandTotals.Add(grandTotal);
                    }
                }
            }
            return grandTotals;
        }

        protected void btnSortConsultant_Click(object sender, EventArgs e)
        {
            var sortOrder = gvBenchCosts.Attributes[ConsultantNameSortOrder];
            IEnumerable<Project> benchList = null;
            if (chbSeperateInternalExternal.Checked)
            {
                benchList = BenchList.ToList().FindAll(P => !P.Practice.IsCompanyInternal);
            }
            else
            {
                benchList = BenchList;
            }

            if (string.IsNullOrEmpty(sortOrder) || sortOrder == Descending)
            {
                benchList = benchList.OrderByDescending(P => P.Name);
                gvBenchCosts.Attributes[ConsultantNameSortOrder] = Ascending;
            }
            else
            {
                benchList = benchList.OrderBy(P => P.Name);
                gvBenchCosts.Attributes[ConsultantNameSortOrder] = Descending;
            }
            gvBenchCosts.Attributes[PracticeSortOrder] = Ascending;
            gvBenchCosts.Attributes[StatusSortOrder] = Ascending;
            AddMonthColumn(3, ReportContext.Start, GetPeriodLength(), gvBenchCosts);
            gvBenchCosts.DataSource = benchList;
            gvBenchCosts.DataBind();
            bntExportExcel.Visible = benchList.Count() > 0;
        }

        protected void btnSortInternalConsultant_Click(object sender, EventArgs e)
        {
            var sortOrder = gvBenchCostsInternal.Attributes[ConsultantNameSortOrder];


            var benchList = BenchList.ToList().FindAll(P => P.Practice.IsCompanyInternal);



            if (string.IsNullOrEmpty(sortOrder) || sortOrder == Descending)
            {
                benchList = (benchList).OrderByDescending(P => P.Name).ToList();
                gvBenchCostsInternal.Attributes[ConsultantNameSortOrder] = Ascending;
            }
            else
            {
                benchList = benchList.OrderBy(P => P.Name).ToList();
                gvBenchCostsInternal.Attributes[ConsultantNameSortOrder] = Descending;
            }
            gvBenchCostsInternal.Attributes[PracticeSortOrder] = Ascending;
            gvBenchCostsInternal.Attributes[StatusSortOrder] = Ascending;
            AddMonthColumn(3, ReportContext.Start, GetPeriodLength(), gvBenchCostsInternal);
            gvBenchCostsInternal.DataSource = benchList;
            gvBenchCostsInternal.DataBind();
            gvBenchCostsInternal.Focus();
            bntExportExcel.Visible = BenchList.Count() > 0;
        }

        protected void btnSortPractice_Click(object sender, EventArgs e)
        {

            var sortOrder = gvBenchCosts.Attributes[PracticeSortOrder];
            IEnumerable<Project> benchList = null;
            if (chbSeperateInternalExternal.Checked)
            {
                benchList = BenchList.ToList().FindAll(P => !P.Practice.IsCompanyInternal);
            }
            else
            {
                benchList = BenchList;
            }
            if (string.IsNullOrEmpty(sortOrder) || sortOrder == Ascending)
            {
                benchList = benchList.OrderBy(P => P.Practice == null ? string.Empty : P.Practice.Name);
                gvBenchCosts.Attributes[PracticeSortOrder] = Descending;
            }
            else
            {
                benchList = benchList.OrderByDescending(P => P.Practice == null ? string.Empty : P.Practice.Name);
                gvBenchCosts.Attributes[PracticeSortOrder] = Ascending;
            }
            gvBenchCosts.Attributes[ConsultantNameSortOrder] = Ascending;
            gvBenchCosts.Attributes[StatusSortOrder] = Ascending;
            AddMonthColumn(3, ReportContext.Start, GetPeriodLength(), gvBenchCosts);
            gvBenchCosts.DataSource = benchList;
            gvBenchCosts.DataBind();
            bntExportExcel.Visible = BenchList.Count() > 0;
        }

        protected void btnSortInternalPractice_Click(object sender, EventArgs e)
        {
            var sortOrder = gvBenchCostsInternal.Attributes[PracticeSortOrder];
            var benchList = BenchList.ToList().FindAll(P => P.Practice.IsCompanyInternal);

            if (string.IsNullOrEmpty(sortOrder) || sortOrder == Ascending)
            {
                benchList = benchList.OrderBy(P => P.Practice == null ? string.Empty : P.Practice.Name).ToList();
                gvBenchCostsInternal.Attributes[PracticeSortOrder] = Descending;
            }
            else
            {
                benchList = benchList.OrderByDescending(P => P.Practice == null ? string.Empty : P.Practice.Name).ToList();
                gvBenchCostsInternal.Attributes[PracticeSortOrder] = Ascending;
            }
            gvBenchCostsInternal.Attributes[ConsultantNameSortOrder] = Ascending;
            gvBenchCostsInternal.Attributes[StatusSortOrder] = Ascending;
            AddMonthColumn(3, ReportContext.Start, GetPeriodLength(), gvBenchCostsInternal);
            gvBenchCostsInternal.DataSource = benchList;
            gvBenchCostsInternal.DataBind();
            gvBenchCostsInternal.Focus();
            bntExportExcel.Visible = BenchList.Count() > 0;
        }

        protected void btnSortStatus_Click(object sender, EventArgs e)
        {

            var sortOrder = gvBenchCosts.Attributes[StatusSortOrder];
            IEnumerable<Project> benchList = null;
            if (chbSeperateInternalExternal.Checked)
            {
                benchList = BenchList.ToList().FindAll(P => !P.Practice.IsCompanyInternal);
            }
            else
            {
                benchList = BenchList;
            }

            if (string.IsNullOrEmpty(sortOrder) || sortOrder == Ascending)
            {
                benchList = benchList.OrderBy(P => P.ProjectNumber);
                gvBenchCosts.Attributes[StatusSortOrder] = Descending;
            }
            else
            {

                benchList = benchList.OrderByDescending(P => P.ProjectNumber);
                gvBenchCosts.Attributes[StatusSortOrder] = Ascending;
            }
            gvBenchCosts.Attributes[ConsultantNameSortOrder] = Ascending;
            gvBenchCosts.Attributes[PracticeSortOrder] = Ascending;
            AddMonthColumn(3, ReportContext.Start, GetPeriodLength(), gvBenchCosts);
            gvBenchCosts.DataSource = benchList;
            gvBenchCosts.DataBind();
            bntExportExcel.Visible = BenchList.Count() > 0;
        }

        protected void btnSortInternalStatus_Click(object sender, EventArgs e)
        {

            var sortOrder = gvBenchCostsInternal.Attributes[StatusSortOrder];

            var benchList = BenchList.ToList().FindAll(P => P.Practice.IsCompanyInternal);


            if (string.IsNullOrEmpty(sortOrder) || sortOrder == Ascending)
            {
                benchList = benchList.OrderBy(P => P.ProjectNumber).ToList();
                gvBenchCostsInternal.Attributes[StatusSortOrder] = Descending;
            }
            else
            {

                benchList = benchList.OrderByDescending(P => P.ProjectNumber).ToList();
                gvBenchCostsInternal.Attributes[StatusSortOrder] = Ascending;
            }
            gvBenchCostsInternal.Attributes[ConsultantNameSortOrder] = Ascending;
            gvBenchCostsInternal.Attributes[PracticeSortOrder] = Ascending;
            AddMonthColumn(3, ReportContext.Start, GetPeriodLength(), gvBenchCostsInternal);
            gvBenchCostsInternal.DataSource = benchList;
            gvBenchCostsInternal.DataBind();
            gvBenchCostsInternal.Focus();
            bntExportExcel.Visible = BenchList.Count() > 0;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                DataHelper.FillPracticeList(this.cblPractices, Resources.Controls.AllPracticesText);

                SelectAllItems(this.cblPractices);
                //DatabindGrid();
                lblExternalPractices.Visible = false;
                hrDirectorAndPracticeSeperator.Visible = false;
                lblInternalPractices.Visible = false;
                GetFilterValuesForSession();
            }
            if (hdnFiltersChanged.Value == "false")
            {
                btnResetFilter.Attributes.Add("disabled", "true");
            }
            else
            {
                btnResetFilter.Attributes.Remove("disabled");
            }
            AddAttributesToCheckBoxes(this.cblPractices);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            diRange.FromDate = BegPeriod;
            diRange.ToDate = EndPeriod;
            lblCustomDateRange.Text = string.Format("({0}&nbsp;-&nbsp;{1})",
                    diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat),
                    diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat)
                    );
            if (ddlPeriod.SelectedValue == "0")
            {
                lblCustomDateRange.Attributes.Add("class", "fontBold");
                imgCalender.Attributes.Add("class", "");
            }
            else
            {
                lblCustomDateRange.Attributes.Add("class", "displayNone");
                imgCalender.Attributes.Add("class", "displayNone");
            }
            hdnStartDate.Value = diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat);
            hdnEndDate.Value = diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat);
            var clFromDate = diRange.FindControl("clFromDate") as AjaxControlToolkit.CalendarExtender;
            var clToDate = diRange.FindControl("clToDate") as AjaxControlToolkit.CalendarExtender;
            hdnStartDateCalExtenderBehaviourId.Value = clFromDate.BehaviorID;
            hdnEndDateCalExtenderBehaviourId.Value = clToDate.BehaviorID;
        }

        private void DatabindGrid()
        {
            Page.Validate();
            if (Page.IsValid)
            {
                var benchList = BenchList;
                AddMonthColumn(3, new DateTime(ReportContext.Start.Year, ReportContext.Start.Month, Constants.Dates.FirstDay), GetPeriodLength(), gvBenchCosts);

                if (chbSeperateInternalExternal.Checked)
                {
                    AddMonthColumn(3, new DateTime(ReportContext.Start.Year, ReportContext.Start.Month, Constants.Dates.FirstDay), GetPeriodLength(), gvBenchCostsInternal);
                    gvBenchCosts.DataSource = benchList.ToList().FindAll(Q => !Q.Practice.IsCompanyInternal).OrderBy(P => P.Name);
                    gvBenchCostsInternal.DataSource = benchList.ToList().FindAll(Q => Q.Practice.IsCompanyInternal).OrderBy(P => P.Name);
                    gvBenchCosts.DataBind();
                    gvBenchCostsInternal.DataBind();
                    divBenchCostsInternal.Visible = lblExternalPractices.Visible = true;
                    bntExportExcel.Visible = BenchList.Count() > 0;
                }
                else
                {
                    gvBenchCosts.DataSource = benchList.OrderBy(P => P.Name);
                    gvBenchCosts.DataBind();
                    divBenchCostsInternal.Visible = lblExternalPractices.Visible = false;
                    bntExportExcel.Visible = BenchList.Count() > 0;
                }
            }
        }

        protected void custPeriod_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = GetPeriodLength() > 0;
        }

        protected void custPeriodLengthLimit_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = GetPeriodLength() <= MaxPeriodLength;
            custPeriodLengthLimit.ErrorMessage = string.Format(custPeriodLengthLimit.ErrorMessage, MaxPeriodLength);
            custPeriodLengthLimit.ToolTip = custPeriodLengthLimit.ErrorMessage;
        }

        private static Project[] PopulateBenchRollOffDatesGrid(Project[] project)
        {
            // exclude from project Bench Total and Admin expense => (project.Length - 2)
            // only bench Person bind
            var benchRollOffDatesGrid = new Project[project.Length - 2];
            for (var i = 0; i < project.Length - 2; i++)
            {
                benchRollOffDatesGrid[i] = project[i];
            }
            return benchRollOffDatesGrid;
        }

        /// <summary>
        /// Calculates a length of the selected period in the mounths.
        /// </summary>
        /// <returns>The number of the months within the selected period.</returns>
        private int GetPeriodLength()
        {
            int mounthsInPeriod =
                (ReportContext.End.Year - ReportContext.Start.Year) * Constants.Dates.LastMonth +
                (ReportContext.End.Month - ReportContext.Start.Month + 1);
            return mounthsInPeriod;
        }

        private IEnumerable<Project> GetBenchList()
        {
            var benchList = ReportsHelper.GetBenchListWithoutBenchTotalAndAdminCosts(ReportContext);
            var benchListTemp = benchList.ToList<Project>().FindAll(p => (p.Practice != null
                                                            && !string.IsNullOrEmpty(p.Practice.Name)
                                                            && !string.IsNullOrEmpty(p.ProjectNumber)
                                                            )
                                                     );
            return benchListTemp.FindAll(p => p.ProjectedFinancialsByMonth.Values.Any(q => ((chbIncludeZeroCostEmps.Checked) || q.GrossMargin.Value != 0M) && q.Timescale == TimescaleType.Salary));
        }

        protected void gvBenchRollOffDates_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var grid = sender as GridView;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var project = e.Row.DataItem as Project;
                bool rowVisible = false;
                if (project != null)
                {
                    var monthBegin =
                        new DateTime(ReportContext.Start.Year,
                            ReportContext.Start.Month,
                            Constants.Dates.FirstDay);

                    var periodLength = GetPeriodLength();

                    ArrayList list = new ArrayList();

                    // Displaying the interest values (main cell data)
                    var current = DataHelper.CurrentPerson;

                    for (int i = 3, k = 0; k < periodLength; i++, k++, monthBegin = monthBegin.AddMonths(1))
                    {
                        var monthEnd =
                            new DateTime(monthBegin.Year,
                                monthBegin.Month,
                                DateTime.DaysInMonth(monthBegin.Year, monthBegin.Month));
                        grid.Columns[i].Visible = true;
                        grid.Columns[i].HeaderText =
                            Resources.Controls.TableHeaderOpenTag +
                            monthBegin.ToString("MMM, yyyy") +
                            Resources.Controls.TableHeaderCloseTag;

                        foreach (var interestValue in project.ProjectedFinancialsByMonth)
                        {
                            if (IsInMonth(interestValue.Key, monthBegin, monthEnd))
                            {
                                rowVisible = true;

                                if (e.Row.Parent.Parent == grid)
                                {
                                    /* According to Bug# 2631
                                     For the "external" table results, the person running the report should only be able to see costs for subordinates (based on seniority level), not anyone at their same level or higher.
                                     For the "internal" table results, only persons with Admin or Partner seniority levels should be able to see costs.  Everyone else should see "(Hidden)".
                                     */
                                    bool seniority = true;
                                    if (grid.ID == "gvBenchCostsInternal")
                                    {
                                        if ((UserIsAdmin || Seniority.GetSeniorityValueById(current.Seniority.Id) == Seniority.GetSeniorityValueById(((int)PersonSeniorityType.Partner))))
                                        {
                                            seniority = false;
                                        }
                                    }
                                    else
                                    {
                                        seniority = (new SeniorityAnalyzer(current)).IsOtherGreater(project.AccessLevel.Id);
                                    }
                                    if (!seniority)
                                    {
                                        e.Row.Cells[i].Text = (interestValue.Value.Timescale == TimescaleType.Salary ?
                                                                Convert.ToDecimal(interestValue.Value.GrossMargin).ToString() :
                                                                Resources.Controls.HourlyLabel);

                                        if (!MonthlyTotals.Any(kvp => kvp.Key == monthBegin))
                                        {
                                            MonthlyTotals.Add(monthBegin, 0M);
                                        }
                                        if (interestValue.Value.Timescale == TimescaleType.Salary)
                                        {
                                            MonthlyTotals[monthBegin] += Convert.ToDecimal(e.Row.Cells[i].Text);
                                        }

                                        if (interestValue.Value.Timescale == TimescaleType.Salary)
                                        {
                                            list.Add(Convert.ToDecimal(interestValue.Value.GrossMargin));
                                            if (interestValue.Value.GrossMargin.Value == 0M)
                                            {
                                                e.Row.Cells[i].Attributes.Add("class", "colorGreen");
                                            }
                                        }
                                        else
                                        {
                                            list.Add(e.Row.Cells[i].Text);
                                        }
                                        string superScriptContent = string.Empty;
                                        if (project.StartDate.HasValue
                                            && project.StartDate.Value.Year == interestValue.Key.Year
                                            && project.StartDate.Value.Month == interestValue.Key.Month
                                            )
                                        {
                                            superScriptContent = "1";
                                        }
                                        if (project.EndDate.HasValue
                                            && project.EndDate.Value.Year == interestValue.Key.Year
                                            && project.EndDate.Value.Month == interestValue.Key.Month)
                                        {
                                            superScriptContent += (string.IsNullOrEmpty(superScriptContent) ? string.Empty : ",") + "2";
                                        }
                                        if (interestValue.Value.TimescaleChangeStatus > 0)
                                        {
                                            switch (interestValue.Value.TimescaleChangeStatus)
                                            {
                                                case 1: superScriptContent += (string.IsNullOrEmpty(superScriptContent) ? string.Empty : ",") + "3";
                                                    break;
                                                case 2:
                                                case 3: superScriptContent += (string.IsNullOrEmpty(superScriptContent) ? string.Empty : ",") + "4";
                                                    break;
                                                default: break;
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(superScriptContent))
                                        {
                                            if (interestValue.Value.Timescale == TimescaleType.Salary)
                                            {
                                                if (interestValue.Value.GrossMargin.Value == 0M)
                                                {
                                                    e.Row.Cells[i].Text = "<span class=\"colorGreen\">0.00" + "<sup>" + superScriptContent + "</sup></span>";
                                                }
                                                else
                                                {
                                                    e.Row.Cells[i].Text = "<span class=\"colorRed\">" + interestValue.Value.GrossMargin.ToString() +
                                                                            "<sup>" + superScriptContent + "</sup></span>";
                                                }

                                            }
                                            else
                                            {
                                                e.Row.Cells[i].Text += "<sup>" + superScriptContent + "</sup>";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        e.Row.Cells[i].Text = string.Format(ReportFormat, e.Row.Cells[i].Text, Resources.Controls.HiddenAmount);

                                        list.Add(e.Row.Cells[i].Text);
                                    }

                                    if (list.OfType<Decimal>().ToList().Count <= 0)
                                    {
                                        e.Row.Cells[grid.Columns.Count - 1].Text = string.Empty;
                                    }
                                    else
                                    {
                                        e.Row.Cells[grid.Columns.Count - 1].Text = list.OfType<Decimal>().ToList().Sum().ToString();
                                    }
                                }
                            }
                        }

                    }
                }

                for (int i = 3; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].CssClass = CompPerfDataWithPaddingRightCssClass;
                }

                e.Row.Visible = rowVisible;
            }
        }

        protected void btnUpdateView_Click(object sender, EventArgs e)
        {
            PopulateData();
            SaveFilterValuesForSession();
        }

        private void PopulateData()
        {
            ReportContext = null;
            BenchList = null;
            divBenchCostsInternal.Visible = gvBenchCosts.Visible = true;
            DatabindGrid();
            if (chbSeperateInternalExternal.Checked)
            {
                lblExternalPractices.Visible = true;
                hrDirectorAndPracticeSeperator.Visible = true;
                lblInternalPractices.Visible = true;
            }
            gvBenchCosts.Attributes[ConsultantNameSortOrder] = Descending;
            gvBenchCosts.Attributes[PracticeSortOrder] = Ascending;
            gvBenchCosts.Attributes[StatusSortOrder] = Ascending;
        }

        private static string GetPersonDetailsUrl(object args)
        {
            return string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                 Constants.ApplicationPages.PersonDetail,
                                 args);
        }

        protected string GetPersonDetailsUrlWithReturn(object id)
        {
            return Utils.Generic.GetTargetUrlWithReturn(GetPersonDetailsUrl(id), Request.Url.AbsoluteUri);
        }

        private void AddMonthColumn(int numberOfFixedColumns, DateTime periodStart, int monthsInPeriod, GridView grid)
        {
            // Remove columns from previous report if there was one
            for (var i = numberOfFixedColumns; i < gvBenchCosts.Columns.Count; i++)
                grid.Columns[i].Visible = false;

            // Add columns for new months);
            for (int i = numberOfFixedColumns, k = 0; k < monthsInPeriod; i++, k++)
            {
                grid.Columns[i].HeaderText =
                    Resources.Controls.TableHeaderOpenTag +
                    periodStart.ToString("MMM, yyyy") +
                    Resources.Controls.TableHeaderCloseTag;
                periodStart = periodStart.AddMonths(1);
            }
            //Add column for Grand Total
            grid.Columns[gvBenchCosts.Columns.Count - 1].HeaderText = "Grand Total";
            grid.Columns[gvBenchCosts.Columns.Count - 1].Visible = true;
        }

        private void SelectAllItems(ScrollingDropDown ddlpractices)
        {
            foreach (ListItem item in ddlpractices.Items)
            {
                item.Selected = true;
            }
        }

        protected void btnResetFilter_Click(object sender, EventArgs e)
        {
            SelectAllItems(cblPractices);
            ddlPeriod.SelectedValue = "1";
            diRange.FromDate = BegPeriod;
            diRange.ToDate = EndPeriod;
            chbActiveProjects.Checked = chbProjectedProjects.Checked = chbProjectedProjects.Checked = chbProposed.Checked = true;
            chbExperimentalProjects.Checked = false;
            chbIncludeZeroCostEmps.Checked = false;
            chbIncludeOverHeads.Checked = true;
            chbSeperateInternalExternal.Checked = true;
            chbCompletedProjects.Checked = true;
            ReportContext = null;
            BenchList = null;

            //DatabindGrid();
            //gvBenchCosts.Attributes[ConsultantNameSortOrder] = Descending;
            //gvBenchCosts.Attributes[PracticeSortOrder] = Ascending;
            //gvBenchCosts.Attributes[StatusSortOrder] = Ascending;
            divBenchCostsInternal.Visible = lblExternalPractices.Visible = gvBenchCosts.Visible = false;
            hdnFiltersChanged.Value = "false";
            btnResetFilter.Attributes.Add("disabled", "true");
        }

        protected void gvBench_OnDataBound(object sender, EventArgs e)
        {
            //BenchList
            GridView grid = (GridView)sender;
            GridViewRow footer = grid.FooterRow;
            var monthBegin =
                        new DateTime(ReportContext.Start.Year,
                            ReportContext.Start.Month,
                            Constants.Dates.FirstDay);
            var perodLenth = GetPeriodLength();
            if (footer != null)
            {
                footer.Cells[1].Text = "Grand Total :";
                footer.Cells[1].CssClass = "Left";
                for (int i = 3; i < grid.Columns.Count; i++)
                {
                    //Decimal total = 0;
                    foreach (GridViewRow row in grid.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            Decimal value;
                            bool isDecimal = Decimal.TryParse(row.Cells[i].Text, out value);

                            if (isDecimal)
                            {
                                row.Cells[i].Text = ((PracticeManagementCurrency)Convert.ToDecimal(row.Cells[i].Text)).ToString();
                            }
                        }
                    }
                    if (i < (3 + perodLenth))
                    {
                        if (MonthlyTotals.Any(kvp => kvp.Key == monthBegin.AddMonths(i - 3)))
                            footer.Cells[i].Text = ((PracticeManagementCurrency)MonthlyTotals[monthBegin.AddMonths(i - 3)]).ToString();
                    }
                    else if (i == grid.Columns.Count - 1)
                    {
                        footer.Cells[i].Text = ((PracticeManagementCurrency)MonthlyTotals.Values.Sum()).ToString();
                    }
                }
                MonthlyTotals = null;
            }
        }

        private void AddAttributesToCheckBoxes(ScrollingDropDown ddlpractices)
        {
            foreach (ListItem item in ddlpractices.Items)
            {
                item.Attributes.Add("onclick", "EnableResetButton();");
            }
        }

        private void SaveFilterValuesForSession()
        {
            BenchCostReportFilters filter = new BenchCostReportFilters();
            filter.PracticeIds = cblPractices.SelectedItems;
            filter.IsActive = chbActiveProjects.Checked;
            filter.IsProjected = chbProjectedProjects.Checked;
            filter.IsProposed = chbProposed.Checked;
            filter.IsCompleted = chbCompletedProjects.Checked;
            filter.IsExperimental = chbExperimentalProjects.Checked;
            filter.IncludeOverheads = chbIncludeOverHeads.Checked;
            filter.IncludeZeroCost = chbIncludeZeroCostEmps.Checked;
            filter.SeperateInternalExternalTables = chbSeperateInternalExternal.Checked;
            filter.ReportPeriod = ddlPeriod.SelectedValue;
            filter.ReportStartDate = diRange.FromDate;
            filter.ReportEndDate = diRange.ToDate;
            ReportsFilterHelper.SaveFilterValues(ReportName.BenchCostReport, filter);
        }

        private void GetFilterValuesForSession()
        {
            var filters = ReportsFilterHelper.GetFilterValues(ReportName.BenchCostReport) as BenchCostReportFilters;
            if (filters != null)
            {
                cblPractices.UnSelectAll();
                cblPractices.SelectedItems = filters.PracticeIds;
                chbActiveProjects.Checked = filters.IsActive;
                chbCompletedProjects.Checked = filters.IsCompleted;
                chbExperimentalProjects.Checked = filters.IsExperimental;
                chbProjectedProjects.Checked = filters.IsProjected;
                chbProposed.Checked = filters.IsProposed;
                chbIncludeOverHeads.Checked = filters.IncludeOverheads;
                chbIncludeZeroCostEmps.Checked = filters.IncludeZeroCost;
                chbSeperateInternalExternal.Checked = filters.SeperateInternalExternalTables;
                ddlPeriod.SelectedValue = filters.ReportPeriod;
                diRange.FromDate = filters.ReportStartDate;
                diRange.ToDate = filters.ReportEndDate;
                PopulateData();
            }
        }
    }
}

