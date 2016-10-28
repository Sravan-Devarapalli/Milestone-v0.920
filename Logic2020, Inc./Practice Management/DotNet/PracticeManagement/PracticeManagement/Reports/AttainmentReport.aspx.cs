using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using DataTransferObjects.Financials;
using DataTransferObjects.Reports;
using PraticeManagement.Controls;
using PraticeManagement.Security;
using PraticeManagement.Utils;
using PraticeManagement.Utils.Excel;

namespace PraticeManagement.Reports
{
    public partial class AttainmentReport : System.Web.UI.Page
    {
        private const string Revenue = "Revenue";
        private const string ServiceRevenue = "Services Revenue";
        private const string Margin = "Cont. Margin";
        private const string ExportDateRangeFormat = "Date Range: {0} - {1}";

        private bool renderMonthColumns;
        private int headerRowsCount = 1;
        private int billingheaderRowsCount = 1;
        private int attributionHeaderRowsCount = 1;
        private int coloumnsCount = 1;
        private int billingcoloumnsCount = 1;
        private int attributionColoumnsCount = 1;

        private Project[] _ExportProjectList = null;

        private Project[] ExportProjectList
        {
            get
            {
                if (_ExportProjectList == null)
                {
                    _ExportProjectList = ProjectList;
                }
                return (Project[])_ExportProjectList;
            }
        }

        private AttainmentBillableutlizationReport[] _BillableUtlizationList = null;

        private AttainmentBillableutlizationReport[] BillableUtlizationList
        {
            get
            {
                if (_BillableUtlizationList == null)
                {
                    _BillableUtlizationList = ServiceCallers.Custom.Report(p => p.AttainmentBillableutlizationReport(diRange.FromDate.Value, diRange.ToDate.Value));
                }
                return (AttainmentBillableutlizationReport[])_BillableUtlizationList;
            }
        }

        private Project[] _ProjectAttributionList = null;

        private Project[] ProjectAttributionList
        {
            get
            {
                if (_ProjectAttributionList == null)
                {
                    _ProjectAttributionList = ServiceCallers.Custom.Report(p => p.ProjectAttributionReport(diRange.FromDate.Value, diRange.ToDate.Value));
                }
                return (Project[])_ProjectAttributionList;
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

                CellStyles monthNameHeaderCellStyle = new CellStyles();
                monthNameHeaderCellStyle.DataFormat = "[$-409]mmmm-yy;@";
                monthNameHeaderCellStyle.IsBold = true;
                monthNameHeaderCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

                List<CellStyles> headerCellStyleList = new List<CellStyles>();
                for (int i = 0; i < 12; i++)//there are 12 columns before month columns.
                    headerCellStyleList.Add(headerCellStyle);
                if (renderMonthColumns)
                {
                    var monthsInPeriod = GetPeriodLength();
                    for (int i = 0; i < monthsInPeriod; i++)
                    {
                        headerCellStyleList.Add(monthNameHeaderCellStyle);
                    }
                }
                headerCellStyleList.Add(headerCellStyle);

                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataCellStyle = new CellStyles();

                CellStyles wrapdataCellStyle = new CellStyles();
                wrapdataCellStyle.WrapText = true;

                CellStyles dataStartDateCellStyle = new CellStyles();
                dataStartDateCellStyle.DataFormat = "mm/dd/yy;@";

                CellStyles dataNumberDateCellStyle = new CellStyles();
                dataNumberDateCellStyle.DataFormat = "$#,##0.00_);($#,##0.00)";

                CellStyles[] dataCellStylearray = { dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataStartDateCellStyle, dataStartDateCellStyle, dataCellStyle, dataCellStyle };
                List<CellStyles> dataCellStyleList = dataCellStylearray.ToList();
                if (renderMonthColumns)
                {
                    var monthsInPeriod = GetPeriodLength();
                    for (int i = 0; i < monthsInPeriod; i++)
                    {
                        dataCellStyleList.Add(dataNumberDateCellStyle);
                    }
                }
                dataCellStyleList.Add(dataNumberDateCellStyle);
                dataCellStyleList.Add(wrapdataCellStyle);
                dataCellStyleList.Add(dataCellStyle);
                dataCellStyleList.Add(dataCellStyle);
                dataCellStyleList.Add(dataCellStyle);
                dataCellStyleList.Add(dataCellStyle);
                for (int i = 0; i < 5; i++)
                {
                    dataCellStyleList.Add(dataNumberDateCellStyle);
                }

                RowStyles datarowStyle = new RowStyles(dataCellStyleList.ToArray());

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.TopRowNo = headerRowsCount;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanColSplit = 1;
                sheetStyle.FreezePanRowSplit = headerRowsCount;
                return sheetStyle;
            }
        }

        private SheetStyles BillingHeaderSheetStyle
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
                sheetStyle.MergeRegion.Add(new int[] { 0, 0, 0, billingcoloumnsCount - 1 });
                sheetStyle.IsAutoResize = false;

                return sheetStyle;
            }
        }

        private SheetStyles BillableUtilSheetDataSheetStyle
        {
            get
            {
                CellStyles headerCellStyle = new CellStyles();
                headerCellStyle.IsBold = true;
                headerCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                headerCellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

                CellStyles monthNameHeaderCellStyle = new CellStyles();
                monthNameHeaderCellStyle.DataFormat = "[$-409]mmmm-yy;@";
                monthNameHeaderCellStyle.IsBold = true;
                monthNameHeaderCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

                List<CellStyles> headerCellStyleList = new List<CellStyles>();
                for (int i = 1; i <= 3; i++)
                    headerCellStyleList.Add(headerCellStyle);
                for (int i = 1; i <= 36; i++)
                {
                    headerCellStyleList.Add(monthNameHeaderCellStyle);
                }
                headerCellStyleList.Add(headerCellStyle);

                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataCellStyle = new CellStyles();
                CellStyles decimaldataCellStyle = new CellStyles();
                decimaldataCellStyle.DataFormat = "0.0%";

                CellStyles[] dataCellStylearray = { dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, decimaldataCellStyle, dataCellStyle, dataCellStyle, decimaldataCellStyle, dataCellStyle, dataCellStyle, decimaldataCellStyle,
                                                  dataCellStyle, dataCellStyle, decimaldataCellStyle,dataCellStyle, dataCellStyle, decimaldataCellStyle,dataCellStyle, dataCellStyle, decimaldataCellStyle,
                                                  dataCellStyle, dataCellStyle, decimaldataCellStyle,dataCellStyle, dataCellStyle, decimaldataCellStyle,dataCellStyle, dataCellStyle, decimaldataCellStyle,
                                                  dataCellStyle, dataCellStyle, decimaldataCellStyle,dataCellStyle, dataCellStyle, decimaldataCellStyle,dataCellStyle, dataCellStyle, decimaldataCellStyle,
                                                  dataCellStyle, dataCellStyle, decimaldataCellStyle,dataCellStyle, dataCellStyle, decimaldataCellStyle,dataCellStyle, dataCellStyle, decimaldataCellStyle,
                                                  dataCellStyle, dataCellStyle, decimaldataCellStyle,dataCellStyle, dataCellStyle, decimaldataCellStyle
                                                  };
                //for (int i = 0; i < 17; i++)
                //{
                //    dataCellStylearray.ToList().Add(dataCellStyle);
                //    dataCellStylearray.ToList().Add(dataCellStyle);
                //    dataCellStylearray.ToList().Add(decimaldataCellStyle);
                //    dataCellStylearray.ToArray();
                //}
                List<CellStyles> dataCellStyleList = dataCellStylearray.ToList();

                RowStyles datarowStyle = new RowStyles(dataCellStyleList.ToArray());

                RowStyles[] rowStylearray = { headerrowStyle, headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.MergeRegion.Add(new int[] { 2, 3, 0, 0 });
                sheetStyle.MergeRegion.Add(new int[] { 2, 3, 1, 1 });
                sheetStyle.MergeRegion.Add(new int[] { 2, 3, 2, 2 });
                for (int i = 0; i < 17; i++)
                {
                    sheetStyle.MergeRegion.Add(new int[] { 2, 2, (i + 1) * 3, (i + 1) * 3 + 2 });
                }
                sheetStyle.TopRowNo = billingheaderRowsCount;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanColSplit = 0;
                sheetStyle.FreezePanRowSplit = billingheaderRowsCount + 1;
                sheetStyle.FreezePanColSplit = 3;
                //sheetStyle.ColoumnWidths = coloumnWidth;
                return sheetStyle;
            }
        }

        private SheetStyles AttributionHeaderSheetStyle
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
                sheetStyle.MergeRegion.Add(new int[] { 0, 0, 0, attributionColoumnsCount - 1 });
                sheetStyle.IsAutoResize = false;

                return sheetStyle;
            }
        }

        private SheetStyles AttributionDataSheetStyle
        {
            get
            {
                CellStyles headerCellStyle = new CellStyles();
                headerCellStyle.IsBold = true;
                headerCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                headerCellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

                List<CellStyles> headerCellStyleList = new List<CellStyles>();
                for (int i = 1; i <= 14; i++)
                    headerCellStyleList.Add(headerCellStyle);

                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataCellStyle = new CellStyles();

                CellStyles dataStartDateCellStyle = new CellStyles();
                dataStartDateCellStyle.DataFormat = "mm/dd/yy;@";

                CellStyles decimaldataCellStyle = new CellStyles();
                decimaldataCellStyle.DataFormat = "0.0%";

                List<CellStyles> dataCellStyleList = new List<CellStyles>();

                for (int i = 1; i <= 11; i++)
                    dataCellStyleList.Add(dataCellStyle);
                dataCellStyleList.Add(dataStartDateCellStyle);
                dataCellStyleList.Add(dataStartDateCellStyle);
                dataCellStyleList.Add(decimaldataCellStyle);

                RowStyles datarowStyle = new RowStyles(dataCellStyleList.ToArray());

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);

                sheetStyle.TopRowNo = attributionHeaderRowsCount;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanRowSplit = attributionHeaderRowsCount;
                sheetStyle.FreezePanColSplit = 0;
                return sheetStyle;
            }
        }

        private static DateTime GetMonthEnd(ref DateTime monthBegin)
        {
            return new DateTime(monthBegin.Year,
                    monthBegin.Month,
                    DateTime.DaysInMonth(monthBegin.Year, monthBegin.Month));
        }

        private DateTime GetMonthBegin()
        {
            return new DateTime(diRange.FromDate.Value.Year,
                    diRange.FromDate.Value.Month,
                    Constants.Dates.FirstDay);
        }

        private Project[] ProjectList
        {
            get
            {
                CompanyPerformanceState.Filter = GetFilterSettings();
                bool isProjectSummaryCachedToday = false;
                if ((ddlPeriod.SelectedValue == "-13" || ddlPeriod.SelectedValue == "13") && isProjectSummaryCachedToday)
                {
                    CompanyPerformanceState.Filter.FinancialsFromCache = true;
                }
                else
                {
                    CompanyPerformanceState.Filter.FinancialsFromCache = false;
                }
                return CompanyPerformanceState.AttainmentProjectList;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetPeriodSelection(13);
                GetFilterValuesForSession();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var now = Utils.Generic.GetNowWithTimeZone();

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
        }

        private CompanyPerformanceFilterSettings GetFilterSettings()
        {
            var filter =
                 new CompanyPerformanceFilterSettings
                 {
                     StartYear = diRange.FromDate.Value.Year,
                     StartMonth = diRange.FromDate.Value.Month,
                     StartDay = diRange.FromDate.Value.Day,
                     EndYear = diRange.ToDate.Value.Year,
                     EndMonth = diRange.ToDate.Value.Month,
                     EndDay = diRange.ToDate.Value.Day,
                     ClientIdsList = null,
                     ProjectOwnerIdsList = null,
                     PracticeIdsList = null,
                     SalespersonIdsList = null,
                     ProjectGroupIdsList = null,
                     ShowActive = true,
                     ShowCompleted = true,
                     ShowProjected = true,
                     ShowInternal = false,
                     ShowExperimental = false,
                     ShowProposed = true,
                     ShowInactive = false,
                     ShowAtRisk = true,
                     PeriodSelected = Convert.ToInt32(ddlPeriod.SelectedValue),
                     ViewSelected = -1,
                     CalculateRangeSelected = (ProjectCalculateRangeType)1,
                     HideAdvancedFilter = false,
                     IsMonthsColoumnsShown = true,
                     IsQuarterColoumnsShown = true,
                     IsYearToDateColoumnsShown = true
                 };
            return filter;
        }

        protected void ddlPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            int periodSelected = Convert.ToInt32(ddlPeriod.SelectedValue);

            SetPeriodSelection(periodSelected);
        }

        protected void btnCustDatesOK_Click(object sender, EventArgs e)
        {
            hdnPeriod.Value = ddlPeriod.SelectedValue;
        }

        protected void btnCustDatesCancel_OnClick(object sender, EventArgs e)
        {
            if (hdnPeriod.Value != ddlPeriod.SelectedValue)
            {
                ddlPeriod.SelectedValue = hdnPeriod.Value;
            }
        }

        private void SetPeriodSelection(int periodSelected)
        {
            DateTime currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, Constants.Dates.FirstDay);
            if (periodSelected > 0)
            {
                DateTime startMonth = new DateTime();
                DateTime endMonth = new DateTime();

                if (periodSelected < 13)
                {
                    startMonth = currentMonth;
                    endMonth = currentMonth.AddMonths(Convert.ToInt32(ddlPeriod.SelectedValue) - 1);
                }
                else
                {
                    Dictionary<string, DateTime> fPeriod = DataHelper.GetFiscalYearPeriod(currentMonth);
                    startMonth = fPeriod["StartMonth"];
                    endMonth = fPeriod["EndMonth"];
                }
                diRange.FromDate = startMonth;
                diRange.ToDate = new DateTime(endMonth.Year, endMonth.Month, DateTime.DaysInMonth(endMonth.Year, endMonth.Month));
                hdnPeriod.Value = ddlPeriod.SelectedValue;
            }
            else if (periodSelected < 0)
            {
                DateTime startMonth = new DateTime();
                DateTime endMonth = new DateTime();

                if (periodSelected > -13)
                {
                    startMonth = currentMonth.AddMonths(Convert.ToInt32(ddlPeriod.SelectedValue) + 1);
                    endMonth = currentMonth;
                }
                else
                {
                    Dictionary<string, DateTime> fPeriod = DataHelper.GetFiscalYearPeriod(currentMonth.AddYears(-1));
                    startMonth = fPeriod["StartMonth"];
                    endMonth = fPeriod["EndMonth"];
                }
                diRange.FromDate = startMonth;
                diRange.ToDate = new DateTime(endMonth.Year, endMonth.Month, DateTime.DaysInMonth(endMonth.Year, endMonth.Month));
                hdnPeriod.Value = ddlPeriod.SelectedValue;
            }
            else
            {
                mpeCustomDates.Show();
            }
        }

        private int GetPeriodLength()
        {
            int mounthsInPeriod =
                (diRange.ToDate.Value.Year - diRange.FromDate.Value.Year) * Constants.Dates.LastMonth +
                (diRange.ToDate.Value.Month - diRange.FromDate.Value.Month + 1);
            return mounthsInPeriod;
        }

        private string GetProjectManagers(List<Person> list)
        {
            string names = string.Empty;
            foreach (var person in list)
            {
                names += person.Name + "; ";
            }

            names = names.Remove(names.LastIndexOf("; "));
            return names;
        }

        private static bool IsInMonth(RangeType range, DateTime periodStart, DateTime periodEnd)
        {
            var result =
                (range.StartDate.Year > periodStart.Year ||
                (range.StartDate.Year == periodStart.Year && range.StartDate.Month >= periodStart.Month)) &&
                (range.StartDate.Year < periodEnd.Year || (range.StartDate.Year == periodEnd.Year && range.StartDate.Month <= periodEnd.Month)) && (range.Range != "Q1" && range.Range != "Q2" && range.Range != "Q3" && range.Range != "Q4" && range.Range != "YTD");

            return result;
        }

        private static bool IsInRange(RangeType range, string rangeType)
        {
            return range.Range == rangeType;
        }

        private DataTable PrepareDataTable(Project[] projectsList, Object[] propertyBags, bool useActuals)
        {
            var periodStart = GetMonthBegin();
            var monthsInPeriod = GetPeriodLength();

            DataTable data = new DataTable();

            data.Columns.Add("Project Number");
            data.Columns.Add("Account");
            data.Columns.Add("Business Group");
            data.Columns.Add("Business Unit");
            data.Columns.Add("Buyer");
            data.Columns.Add("Project Name");
            data.Columns.Add("New/Extension");
            data.Columns.Add("Status");
            data.Columns.Add("Start Date");
            data.Columns.Add("End Date");
            data.Columns.Add("Practice Area");
            data.Columns.Add("Type");
            //Add Month and Total columns.
            if (renderMonthColumns)
            {
                for (int i = 0; i < monthsInPeriod; i++)
                {
                    data.Columns.Add(periodStart.AddMonths(i).ToString(Constants.Formatting.EntryDateFormat));
                }
            }
            data.Columns.Add("Total");
            data.Columns.Add("Salesperson");
            data.Columns.Add("Project Access");
            data.Columns.Add("Engagement Manager");
            data.Columns.Add("Executive in Charge");
            data.Columns.Add("Pricing List");
            data.Columns.Add("Q1 Total");
            data.Columns.Add("Q2 Total");
            data.Columns.Add("Q3 Total");
            data.Columns.Add("Q4 Total");
            data.Columns.Add("YTD");
            foreach (var propertyBag in propertyBags)
            {
                var objects = new object[data.Columns.Count];
                int column = 0;
                Project project = new Project();
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(propertyBag))
                {
                    if (property.Name != "ProjectID")
                    {
                        if (property.Name == "ProjectNumber")
                        {
                            project = projectsList.Where(p => p.ProjectNumber == property.GetValue(propertyBag).ToString()).FirstOrDefault();
                        }
                        if (property.Name == "QuartersColumn")
                        {
                            bool isMargin = property.GetValue(propertyBag).ToString() == Margin;
                            //Add month columns.
                            SeniorityAnalyzer personListAnalyzer = new SeniorityAnalyzer(DataHelper.CurrentPerson);
                            personListAnalyzer.OneWithGreaterSeniorityExists(project.ProjectPersons);
                            bool greaterSeniorityExists = personListAnalyzer != null && personListAnalyzer.GreaterSeniorityExists;
                            var columnValue = 0M;
                            var now = Utils.Generic.GetNowWithTimeZone();
                            string rangeType = "";
                            // Displaying the month values (main cell data)
                            for (int k = 0; k < 5; k++)
                            {
                                if (k != 4)
                                {
                                    rangeType = "Q" + (k + 1);
                                }
                                else
                                {
                                    rangeType = "YTD";
                                }
                                columnValue = 0M;
                                if (project.ProjectedFinancialsByRange != null)
                                {
                                    foreach (KeyValuePair<RangeType, ComputedFinancials> interestValue in
                                        project.ProjectedFinancialsByRange)
                                    {
                                        if (IsInRange(interestValue.Key, rangeType))
                                        {
                                            columnValue = isMargin ? (useActuals ? interestValue.Value.ActualGrossMargin : interestValue.Value.GrossMargin) : (useActuals ? interestValue.Value.ActualRevenue : interestValue.Value.Revenue);
                                            break;
                                        }
                                    }
                                }

                                string color = columnValue < 0 ? "red" : isMargin ? "purple" : "green";
                                objects[column] = string.Format(NPOIExcel.CustomColorKey, color, greaterSeniorityExists && isMargin ? (object)"(Hidden)" : columnValue);
                                column++;
                            }
                        }

                        else if (property.Name == "Type")
                        {
                            objects[column] = property.GetValue(propertyBag) == Revenue ? ServiceRevenue: property.GetValue(propertyBag);
                            column++;
                            bool isMargin = property.GetValue(propertyBag).ToString() == Margin;
                            //Add month columns.
                            SeniorityAnalyzer personListAnalyzer = new SeniorityAnalyzer(DataHelper.CurrentPerson);
                            personListAnalyzer.OneWithGreaterSeniorityExists(project.ProjectPersons);
                            bool greaterSeniorityExists = personListAnalyzer != null && personListAnalyzer.GreaterSeniorityExists;
                            var columnValue = 0M;
                            if (renderMonthColumns)
                            {
                                var monthStart = periodStart;
                                // Displaying the month values (main cell data)
                                for (int k = 0;
                                    k < monthsInPeriod;
                                    k++, monthStart = monthStart.AddMonths(1))
                                {
                                    columnValue = 0M;
                                    DateTime monthEnd = GetMonthEnd(ref monthStart);

                                    if (project.ProjectedFinancialsByRange != null)
                                    {
                                        foreach (KeyValuePair<RangeType, ComputedFinancials> interestValue in
                                            project.ProjectedFinancialsByRange)
                                        {
                                            if (IsInMonth(interestValue.Key, monthStart, monthEnd))
                                            {
                                                columnValue = isMargin ? (useActuals ? interestValue.Value.ActualGrossMargin : interestValue.Value.GrossMargin) : (useActuals ? interestValue.Value.ActualRevenue : interestValue.Value.Revenue);
                                                break;
                                            }
                                        }
                                    }

                                    string color = columnValue < 0 ? "red" : isMargin ? "purple" : "green";
                                    objects[column] = string.Format(NPOIExcel.CustomColorKey, color, greaterSeniorityExists && isMargin ? (object)"(Hidden)" : columnValue);
                                    column++;
                                }
                            }
                            columnValue = 0M;
                            if (project.ComputedFinancials != null && !greaterSeniorityExists)
                            {
                                columnValue = isMargin ? (useActuals ? project.ComputedFinancials.ActualGrossMargin : project.ComputedFinancials.GrossMargin) : (useActuals ? project.ComputedFinancials.ActualRevenue : project.ComputedFinancials.Revenue);
                            }
                            string totalColomncolor = columnValue < 0 ? "red" : isMargin ? "purple" : "green";
                            objects[column] = string.Format(NPOIExcel.CustomColorKey, totalColomncolor, greaterSeniorityExists && isMargin ? (object)"(Hidden)" : columnValue);
                            column++;
                        }
                        else if (property.Name == "ProjectManagers")
                        {
                            objects[column] = project.ProjectManagers != null ? GetProjectManagers(project.ProjectManagers) : string.Empty;
                            column++;
                        }
                        else
                        {
                            objects[column] = property.GetValue(propertyBag);
                            column++;
                        }
                    }
                }
                data.Rows.Add(objects);
            }
            return data;
        }

        private DataTable PrepareDataTableForBillableUtilization(AttainmentBillableutlizationReport[] attainmentBillableutlizationList)
        {
            DataTable data = new DataTable();
            if (attainmentBillableutlizationList.Length > 0)
            {
                List<string> coloumnsAll = new List<string>();

                foreach (var bu in attainmentBillableutlizationList[0].BillableUtilizationList)
                {
                    var yearStarDate = Utils.Calendar.YearStartDate(diRange.FromDate.Value);

                    if (bu.RangeType != "Q1" && bu.RangeType != "Q2" && bu.RangeType != "Q3" && bu.RangeType != "Q4" && bu.RangeType != "YTD")
                    {
                        int monthNumber;
                        if (int.TryParse(bu.RangeType.Substring(1).ToString(), out monthNumber))
                        {
                            coloumnsAll.Add(Utils.Calendar.MonthStartDateByMonthNumber(yearStarDate, monthNumber).ToString());
                        }
                    }
                    else
                    {
                        coloumnsAll.Add(bu.RangeType);
                    }
                    coloumnsAll.Add("");
                    coloumnsAll.Add("");
                }
                List<object> row;

                data.Columns.Add("Resource Name");
                data.Columns.Add("Title");
                data.Columns.Add("Pay Type");
                foreach (string s in coloumnsAll)
                {
                    data.Columns.Add(s);
                }
                row = new List<object>();
                row.Add(string.Empty); row.Add(string.Empty); row.Add(string.Empty);
                for (int i = 0; i < attainmentBillableutlizationList[0].BillableUtilizationList.Count; i++)
                {
                    row.Add("Billable Hours");
                    row.Add("Available Hours");
                    row.Add("Billable Utilization");
                }
                data.Rows.Add(row.ToArray());
                foreach (var per in attainmentBillableutlizationList)
                {
                    row = new List<object>();
                    int i;
                    row.Add(per.Person != null ? per.Person.PersonLastFirstName : "");
                    row.Add(per.Person != null && per.Person.Title != null ? per.Person.Title.HtmlEncodedTitleName : "");
                    row.Add(per.Person != null && per.Person.CurrentPay != null ? per.Person.CurrentPay.TimescaleName : "");
                    for (i = 0; i < per.BillableUtilizationList.Count; i++)
                    {
                        //To reduce the "Case When" Cost in sproc,returning the Available n Billable hours.Showing them in excel is based on the Billable Utilization %
                        row.Add(per.BillableUtilizationList[i].BillableUtilization == -1 ? "" : per.BillableUtilizationList[i].BillableHours.ToString());
                        row.Add(per.BillableUtilizationList[i].BillableUtilization == -1 ? "" : per.BillableUtilizationList[i].AvailableHours.ToString());
                        row.Add(per.BillableUtilizationList[i].BillableUtilization == -1 ? "" : per.BillableUtilizationList[i].BillableUtilization.ToString());
                    }
                    data.Rows.Add(row.ToArray());
                }
            }
            return data;
        }

        private DataTable PrepareDataTableForAttribution(Project[] attributionList)
        {
            DataTable data = new DataTable();
            if (attributionList.Length > 0)
            {
                List<object> row;

                data.Columns.Add("Record Type");
                data.Columns.Add("Attribution Type");
                data.Columns.Add("Project Status");
                data.Columns.Add("Project Number");
                data.Columns.Add("Account");
                data.Columns.Add("Business Group");
                data.Columns.Add("Business Unit");
                data.Columns.Add("Project Name");
                data.Columns.Add("New/Extension");
                data.Columns.Add("Name");
                data.Columns.Add("Title");
                data.Columns.Add("Start Date");
                data.Columns.Add("End Date");
                data.Columns.Add("%Commission");

                foreach (var pro in attributionList)
                {
                    int i;
                    for (i = 0; i < pro.AttributionList.Count; i++)
                    {
                        row = new List<object>();
                        row.Add(pro.AttributionList[i].AttributionRecordType.ToString());
                        row.Add(pro.AttributionList[i].AttributionType.ToString());
                        row.Add(pro.Status.Name);
                        row.Add(pro.ProjectNumber);
                        row.Add(pro.Client.HtmlEncodedName);
                        row.Add(pro.BusinessGroup.Name);
                        row.Add(pro.Group.Name);
                        row.Add(pro.HtmlEncodedName);
                        row.Add(pro.BusinessType == (BusinessType)0 ? string.Empty :DataTransferObjects.Utils.Generic.GetDescription(pro.BusinessType));
                        row.Add(pro.AttributionList[i].TargetName);
                        row.Add(pro.AttributionList[i].Title.HtmlEncodedTitleName);
                        row.Add(pro.AttributionList[i].StartDate.Value.ToShortDateString());
                        row.Add(pro.AttributionList[i].EndDate.Value.ToShortDateString());
                        row.Add(pro.AttributionList[i].CommissionPercentage);
                        data.Rows.Add(row.ToArray());
                    }
                }
            }
            return data;
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            SaveFilterValuesForSession();
            DataHelper.InsertExportActivityLogMessage("Attainment Export");

            var projectsData = (from pro in ExportProjectList
                                where pro != null
                                select new
                                {
                                    ProjectID = pro.Id != null ? pro.Id.ToString() : string.Empty,
                                    ProjectNumber = pro.ProjectNumber != null ? pro.ProjectNumber.ToString() : string.Empty,
                                    Account = (pro.Client != null && pro.Client.HtmlEncodedName != null) ? pro.Client.HtmlEncodedName.ToString() : string.Empty,
                                    BusinessGroup = (pro.BusinessGroup != null && pro.BusinessGroup.Name != null) ? pro.BusinessGroup.Name : string.Empty,
                                    BusinessUnit = (pro.Group != null && pro.Group.Name != null) ? pro.Group.Name : string.Empty,
                                    Buyer = pro.BuyerName != null ? pro.BuyerName : string.Empty,
                                    ProjectName = pro.Name != null ? pro.Name : string.Empty,
                                    BusinessType = (pro.BusinessType != (BusinessType)0) ? DataHelper.GetDescription(pro.BusinessType).ToString() : string.Empty,
                                    Status = (pro.Status != null && pro.Status.Name != null) ? pro.Status.Name.ToString() : string.Empty,
                                    StartDate = pro.StartDate.HasValue ? pro.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty,
                                    EndDate = pro.EndDate.HasValue ? pro.EndDate.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty,
                                    PracticeArea = (pro.Practice != null && pro.Practice.Name != null) ? pro.Practice.Name : string.Empty,
                                    Type = Revenue,
                                    Salesperson = (pro.SalesPersonName != null) ? (pro.Client != null && pro.Client.IsHouseAccount ? "House Account" : pro.SalesPersonName) : (pro.Client != null && pro.Client.IsHouseAccount ? "House Account" : string.Empty),
                                    ProjectManagers = string.Empty,
                                    SeniorManager = (pro.SeniorManagerName != null) ? pro.SeniorManagerName : string.Empty,
                                    Director = (pro.Director != null && pro.Director.Name != null) ? (pro.Client != null && pro.Client.IsHouseAccount ? "House Account" : pro.Director.Name.ToString()) : (pro.Client != null && pro.Client.IsHouseAccount ? "House Account" : string.Empty),
                                    PricingList = (pro.PricingList != null && pro.PricingList.Name != null) ? pro.PricingList.Name : string.Empty,
                                    QuartersColumn = Revenue
                                }).ToList();//Note: If you add any extra property to this anonymous type object then change insertPosition of month cells in RowDataBound.

            var projectsDataWithMargin = (from pro in ExportProjectList
                                          where pro != null
                                          select new
                                          {
                                              ProjectID = pro.Id != null ? pro.Id.ToString() : string.Empty,
                                              ProjectNumber = pro.ProjectNumber != null ? pro.ProjectNumber.ToString() : string.Empty,
                                              Account = (pro.Client != null && pro.Client.HtmlEncodedName != null) ? pro.Client.HtmlEncodedName.ToString() : string.Empty,
                                              BusinessGroup = (pro.BusinessGroup != null && pro.BusinessGroup.Name != null) ? pro.BusinessGroup.Name : string.Empty,
                                              BusinessUnit = (pro.Group != null && pro.Group.Name != null) ? pro.Group.Name : string.Empty,
                                              Buyer = pro.BuyerName != null ? pro.BuyerName : string.Empty,
                                              ProjectName = pro.Name != null ? pro.Name : string.Empty,
                                              BusinessType = (pro.BusinessType != (BusinessType)0) ? DataHelper.GetDescription(pro.BusinessType) : string.Empty,
                                              Status = (pro.Status != null && pro.Status.Name != null) ? pro.Status.Name.ToString() : string.Empty,
                                              StartDate = pro.StartDate.HasValue ? pro.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty,
                                              EndDate = pro.EndDate.HasValue ? pro.EndDate.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty,
                                              PracticeArea = (pro.Practice != null && pro.Practice.Name != null) ? pro.Practice.Name : string.Empty,
                                              Type = Margin,
                                              Salesperson = (pro.SalesPersonName != null) ? (pro.Client != null && pro.Client.IsHouseAccount ? "House Account" : pro.SalesPersonName) : (pro.Client != null && pro.Client.IsHouseAccount ? "House Account" : string.Empty),
                                              ProjectManagers = string.Empty,
                                              SeniorManager = (pro.SeniorManagerName != null) ? pro.SeniorManagerName : string.Empty,
                                              Director = (pro.Director != null && pro.Director.Name != null) ? (pro.Client != null && pro.Client.IsHouseAccount ? "House Account" : pro.Director.Name.ToString()) : (pro.Client != null && pro.Client.IsHouseAccount ? "House Account" : string.Empty),
                                              PricingList = (pro.PricingList != null && pro.PricingList.Name != null) ? pro.PricingList.Name : string.Empty,
                                              QuartersColumn = Margin
                                          }).ToList();

            projectsData.AddRange(projectsDataWithMargin);
            projectsData = projectsData.OrderBy(s => (s.Status == ProjectStatusType.Projected.ToString()) ? s.StartDate : s.EndDate).ThenBy(s => s.ProjectNumber).ThenByDescending(s => s.Type).ToList();

            renderMonthColumns = true;
            var data = PrepareDataTable(ExportProjectList, (object[])projectsData.ToArray(), false);
            var dataActual = PrepareDataTable(ExportProjectList, (object[])projectsData.ToArray(), true);

            var now = Utils.Generic.GetNowWithTimeZone();
            DateTime CurrentYearStartdate = Utils.Calendar.YearStartDate(now).Date;
            var blliableUtilization = PrepareDataTableForBillableUtilization(BillableUtlizationList);
            billingcoloumnsCount = blliableUtilization.Columns.Count;
            DataTable billableUtilheader = new DataTable();
            billableUtilheader.Columns.Add("Billable Utilization: " + diRange.FromDate.Value.Year);
            billingheaderRowsCount = billableUtilheader.Rows.Count + 3;

            var attributionReport = PrepareDataTableForAttribution(ProjectAttributionList);
            attributionColoumnsCount = attributionReport.Columns.Count;
            DataTable attributionHeader = new DataTable();
            attributionHeader.Columns.Add("Commissions - " + diRange.FromDate.Value.Year);
            attributionHeaderRowsCount = attributionHeader.Rows.Count + 3;

            string dateRangeTitle = string.Format(ExportDateRangeFormat, diRange.FromDate.Value.ToShortDateString(), diRange.ToDate.Value.ToShortDateString());
            DataTable header = new DataTable();
            header.Columns.Add(dateRangeTitle);
            headerRowsCount = header.Rows.Count + 3;
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            coloumnsCount = data.Columns.Count;
            sheetStylesList.Add(HeaderSheetStyle);
            sheetStylesList.Add(DataSheetStyle);
            sheetStylesList.Add(HeaderSheetStyle);
            sheetStylesList.Add(DataSheetStyle);
            sheetStylesList.Add(BillingHeaderSheetStyle);
            sheetStylesList.Add(BillableUtilSheetDataSheetStyle);
            sheetStylesList.Add(AttributionHeaderSheetStyle);
            sheetStylesList.Add(AttributionDataSheetStyle);

            var dataSetList = new List<DataSet>();
            var dataset = new DataSet();
            dataset.DataSetName = "Summary - Projected";
            dataset.Tables.Add(header);
            dataset.Tables.Add(data);
            dataSetList.Add(dataset);

            var datasetActual = new DataSet();
            datasetActual.Tables.Add(header.Clone());
            datasetActual.Tables.Add(dataActual);
            datasetActual.DataSetName = "Summary - Actuals";
            dataSetList.Add(datasetActual);

            var datasetBillableUtil = new DataSet();
            datasetBillableUtil.DataSetName = "Billable Utilization";
            datasetBillableUtil.Tables.Add(billableUtilheader);
            datasetBillableUtil.Tables.Add(blliableUtilization);
            dataSetList.Add(datasetBillableUtil);

            var datasetAttribution = new DataSet();
            datasetAttribution.DataSetName = "Sales Delivery Attributions";
            datasetAttribution.Tables.Add(attributionHeader);
            datasetAttribution.Tables.Add(attributionReport);
            dataSetList.Add(datasetAttribution);

            NPOIExcel.Export("AttainmentReportingDataSource.xls", dataSetList, sheetStylesList);
            
        }

        private void SaveFilterValuesForSession()
        {
            string filter = ddlPeriod.SelectedValue;
            ReportsFilterHelper.SaveFilterValues(ReportName.AttainmentReport, filter);
        }

        private void GetFilterValuesForSession()
        {
            var filters = ReportsFilterHelper.GetFilterValues(ReportName.AttainmentReport) as string;
            if (filters != null)
            {
                ddlPeriod.SelectedValue = filters;
            }
        }
    }
}

