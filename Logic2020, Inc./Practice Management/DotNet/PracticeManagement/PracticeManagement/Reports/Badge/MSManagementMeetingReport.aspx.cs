using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.HtmlControls;
using System.Drawing;
using DataTransferObjects;
using DataTransferObjects.Utils;
using System.Data;
using PraticeManagement.Utils.Excel;
using PraticeManagement.Utils;
using DataTransferObjects.Reports;
using System.Text;
using PraticeManagement.Controls;
using PraticeManagement.PersonStatusService;
using System.ServiceModel;
using System.Text.RegularExpressions;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using DataTransferObjects.Filters;

namespace PraticeManagement.Reports.Badge
{
    public partial class MSManagementMeetingReport : System.Web.UI.Page
    {
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;

        private const decimal ActualRevenue = 93.04m;
        private const decimal TargetRevenue = 95.00m;
        private const decimal HoursUtilization = 1872m;
        private const decimal TotalRevenue = 14525700m;
        private const char Month_Start_Letter = 'K';
        private const string ManagedServiceCount_Format = "COUNTIF(J{0}:J{1}, \"Y\")";
        private const string AvailabilitySum_ByMonth_Format = "SUM({0}:{1})";
        private const string TotalAvailable_ByMonth_Format = "+{0}";
        private const string SUM_IF_Format = "SUMIF($C${0}:$C${1},$I{2},${3}${0}:${3}${1})";
        private const string SUM_IFS_Format = "SUMPRODUCT(--(C{0}:C{1}=I{3}), --(J{0}:J{1}=\"Y\"),{2}{0}:{2}{1})";
        private const string SUBTRACT_Format = "+{0}-{1}";
        private const string Multiplication_Format = "G{0}*H{1}";
        private const string DeficitResourceCalc_Format = "IF({0}>$H{1},\"-\",IF({0}=$H{1},0,{0}-$H{1}))";
        private const string RequiredCellLocation_Format = "+H{0}";
        private const string REVFTE_Format = "+F{0}*G{0}";
        private const string FTE_Format = "+H{0}/H{1}";
        private const string MangedCountCurrent_Format = "+K{0}";
        private const string RequiredCalc_Format = "+H{0}-H{1}";

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
                dataCellStyle.WrapText = true;
                dataCellStyle.IsBold = true;
                CellStyles[] dataCellStylearray = { dataCellStyle };
                RowStyles datarowStyle = new RowStyles(dataCellStylearray);
                RowStyles datarowStyle2 = new RowStyles(dataCellStylearray);
                datarowStyle2.Height = 600;

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle2, datarowStyle };

                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.MergeRegion.Add(new int[] { 0, 0, 0, coloumnsCount > 10 ? coloumnsCount : 11 - 1 });
                sheetStyle.IsAutoResize = false;

                return sheetStyle;
            }
        }

        public List<DataTransferObjects.Reports.ManagementMeetingReport> PersonsList
        {
            get;
            set;
        }

        public List<ManagementMeetingReport> AvailableResources
        {
            get;
            set;
        }

        public List<TotalByRange> TotalsList { get; set; }

        public DateTime StartDate
        {
            get
            {
                var now = SettingsHelper.GetCurrentPMTime();
                return PraticeManagement.Utils.Calendar.MonthStartDate(now);
            }
        }

        public DateTime EndDate
        {
            get
            {
                var endDate = SettingsHelper.GetCurrentPMTime().AddMonths(17);
                return PraticeManagement.Utils.Calendar.MonthEndDate(endDate);
            }
        }

        public DateTime AverageEndDate
        {
            get
            {
                var endDate = SettingsHelper.GetCurrentPMTime().AddMonths(5);
                return PraticeManagement.Utils.Calendar.MonthEndDate(endDate);
            }
        }

        public string PersonStatus
        {
            get
            {
                var statusList = new StringBuilder();
                foreach (ListItem item in cblPersonStatus.Items)
                {
                    if (item.Selected)
                        statusList.Append(item.Value).Append(',');
                    if (item.Value == "1" && item.Selected)
                    {
                        statusList.Append("2").Append(',');
                        statusList.Append("5").Append(',');
                    }
                }
                return statusList.ToString();
            }
        }

        public decimal TotalTargetResources { get; set; }

        public SheetStyles DataStyles
        {
            get;
            set;
        }

        public string Paytypes
        {
            get
            {
                return cblPayTypes.areAllSelected ? null : cblPayTypes.SelectedItems;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.FillTimescaleList(this.cblPayTypes, Resources.Controls.AllTypes, true);
                cblPayTypes.SelectItems(new List<int>() { 1, 2 });
                FillPersonStatusList();
                cblPersonStatus.SelectItems(new List<int>() { 1 });
                divWholePage.Visible = true;
                GetFilterValuesForSession();
            }
        }

        public void FillPersonStatusList()
        {
            using (var serviceClient = new PersonStatusServiceClient())
            {
                try
                {
                    var statuses = serviceClient.GetPersonStatuses();
                    statuses = statuses.Where(p => p.Id != 2 && p.Id != 5).ToArray();
                    DataHelper.FillListDefault(cblPersonStatus, Resources.Controls.AllTypes, statuses, false);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        protected void btnUpdateView_Click(object sender, EventArgs e)
        {
            Page.Validate("BadgeReport");
            if (!Page.IsValid)
            {
                divWholePage.Visible = false;
                return;
            }
            SaveFilterValuesForSession();
            PopulateData();
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            var filename = string.Format("ManagementMeetingReport_{0}-{1}.xls", StartDate.ToString("MM_dd_yyyy"), EndDate.ToString("MM_dd_yyyy"));
            var sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();
            var statuses = PersonStatus;
            var report = ServiceCallers.Custom.Report(r => r.ManagedServiceReportByPerson(Paytypes, PersonStatus, StartDate, EndDate)).ToList();
            PersonsList = report.OrderBy(p => p.Person.Name).ToList();
            if (PersonsList.Count > 0)
            {
                string dateRangeTitle = string.Format("18-Month Management Meeting Report for the period: {0} to {1}", StartDate.ToString(Constants.Formatting.EntryDateFormat), EndDate.ToString(Constants.Formatting.EntryDateFormat));
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                string paytypeText = "";
                if (cblPayTypes.areAllSelected)
                {
                    paytypeText = "All Types";
                }
                else
                {
                    foreach (ListItem item in cblPayTypes.Items)
                    {
                        if (!item.Selected)
                            continue;
                        paytypeText += item.Text + ",";
                    }
                    paytypeText = paytypeText.TrimEnd(',');
                }
                paytypeText += ";\n";

                string personStatusText = "";
                if (cblPersonStatus.areAllSelected)
                {
                    personStatusText = "All Types";
                }
                else
                {
                    foreach (ListItem item in cblPersonStatus.Items)
                    {
                        if (!item.Selected)
                            continue;
                        personStatusText += item.Text + ",";
                    }
                    personStatusText = personStatusText.TrimEnd(',');
                }
                personStatusText += ";\n";

                header.Rows.Add("PayTypes: " + paytypeText + "Person Status: " + personStatusText);
                headerRowsCount = header.Rows.Count + 3;
                AvailableResources = GetAvailableResourcesByTitle();
                var data = PrepareDataTable(PersonsList, AvailableResources);
                coloumnsCount = data.Columns.Count;
                sheetStylesList.Add(HeaderSheetStyle);
                DataStyles.IsAutoResize = true;
                sheetStylesList.Add(DataStyles);
                var dataset = new DataSet();
                dataset.DataSetName = "ManagementMeetingReport";
                dataset.Tables.Add(header);
                dataset.Tables.Add(data);
                dataSetList.Add(dataset);
            }
            else
            {
                string dateRangeTitle = "There are no resources for the selected filters.";
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                sheetStylesList.Add(HeaderSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "ManagementMeetingReport";
                dataset.Tables.Add(header);
                dataSetList.Add(dataset);
            }
            NPOIExcel.Export(filename, dataSetList, sheetStylesList);
        }

        public string GetAppropriateLetter(int i)
        {
            int a = Month_Start_Letter + i;
            return Convert.ToChar(a).ToString();
        }

        public DataTable PrepareDataTable(List<ManagementMeetingReport> report, List<ManagementMeetingReport> avlResourcesByTitle)
        {
            var dataStyle = new SheetStyles(new RowStyles[0]);
            var rowStylesList = dataStyle.rowStyles.ToList();

            CellStyles dataPercentageStyle = new CellStyles();
            dataPercentageStyle.DataFormat = "0%";

            CellStyles dataCurrencyStyle = new CellStyles();
            dataCurrencyStyle.DataFormat = "$#,##0.00_);($#,##0.00)";

            CellStyles headerWrapCellStyle = new CellStyles();
            headerWrapCellStyle.IsBold = true;
            headerWrapCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            //headerWrapCellStyle.DataFormat = "@";
            headerWrapCellStyle.WrapText = true;

            CellStyles dataHeaderDateCellStyle = new CellStyles();
            dataHeaderDateCellStyle.DataFormat = "[$-409]mmm-yy;@";
            dataHeaderDateCellStyle.IsBold = true;
            dataHeaderDateCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

            List<CellStyles> headerCellStyleList = new List<CellStyles>();
            headerCellStyleList.Add(headerWrapCellStyle);
            var headerrowStyle = new RowStyles(headerCellStyleList.ToArray());
            rowStylesList.Add(headerrowStyle);
            report = report.OrderBy(p => p.Person.Title.TitleName).ThenBy(p => p.Person.Name).ToList();
            avlResourcesByTitle = avlResourcesByTitle.OrderBy(t => t.Title.HtmlEncodedTitleName).ToList();

            DataTable data = new DataTable();
            List<object> row;
            DateTime now = SettingsHelper.GetCurrentPMTime();
            List<TotalByRange> total;
            data.Columns.Add("Resource");
            data.Columns.Add("Pay Type");
            data.Columns.Add("Resource Level");
            data.Columns.Add("Badge Start Date");
            data.Columns.Add("Badge End Date");
            data.Columns.Add("Block Start Date");
            data.Columns.Add("Block End Date");
            data.Columns.Add("Projected Re-badge Date");
            data.Columns.Add("Time Left on Clock");
            data.Columns.Add("Managed Service");
            foreach (var header in report[0].Range)
            {
                var dateFormat = header.StartDate.ToString(Constants.Formatting.CompPerfMonthYearFormat);
                data.Columns.Add(dateFormat);
            }
            var dataCellStyle = new CellStyles();
            dataCellStyle.WrapText = true;

            var dataCenterAlignedCell = new CellStyles();
            dataCenterAlignedCell.WrapText = true;
            dataCenterAlignedCell.HorizontalAlignment = HorizontalAlignment.Center;

            CellStyles dataDateCellStyle = new CellStyles();
            dataDateCellStyle.DataFormat = "mm/dd/yy;@";

            CellStyles dataNumberStyle = new CellStyles();
            dataNumberStyle.DataFormat = "#,##0_);[Red](#,##0)";

            CellStyles dataTotalNumberStyle = new CellStyles();
            dataTotalNumberStyle.DataFormat = "#,##0_);[Red](#,##0)";
            dataTotalNumberStyle.IsBold = true;

            foreach (var reportItem in report)
            {
                var dataCellStylearray = new List<CellStyles>() { dataCellStyle, dataCellStyle, dataCellStyle };
                dataCellStylearray.Add(dataDateCellStyle);
                dataCellStylearray.Add(dataDateCellStyle);
                dataCellStylearray.Add(dataDateCellStyle);
                dataCellStylearray.Add(dataDateCellStyle);
                dataCellStylearray.Add(dataDateCellStyle);
                dataCellStylearray.Add(dataCellStyle);
                dataCellStylearray.Add(dataCellStyle);

                row = new List<object>();
                var timeLeftText = "";
                row.Add(reportItem.Person.Name);
                row.Add(reportItem.Person.CurrentPay.TimescaleName);
                row.Add(reportItem.Person.Title != null ? reportItem.Person.Title.TitleName : string.Empty);
                row.Add(reportItem.Person.Badge.BadgeStartDate.HasValue ? reportItem.Person.Badge.BadgeStartDate.Value.ToShortDateString() : string.Empty);
                row.Add(reportItem.Person.Badge.BadgeEndDate.HasValue ? reportItem.Person.Badge.BadgeEndDate.Value.ToShortDateString() : string.Empty);
                row.Add(reportItem.Person.Badge.BlockStartDate.HasValue ? reportItem.Person.Badge.BlockStartDate.Value.ToShortDateString() : string.Empty);
                row.Add(reportItem.Person.Badge.BlockEndDate.HasValue ? reportItem.Person.Badge.BlockEndDate.Value.ToShortDateString() : string.Empty);
                row.Add(reportItem.Person.Badge.PlannedEndDate.HasValue ? reportItem.Person.Badge.PlannedEndDate.Value.ToShortDateString() : string.Empty);
                if (reportItem.Person.Badge.BadgeStartDate.HasValue)
                {
                    if (now.Date >= reportItem.Person.Badge.BreakStartDate.Value.Date && now.Date <= reportItem.Person.Badge.BreakEndDate.Value.Date)
                    {
                        timeLeftText = "On 6-month break";
                    }
                    else if (now.Date > reportItem.Person.Badge.BreakEndDate.Value.Date)
                    {
                        timeLeftText = "Clock not restarted yet";
                    }
                    else
                    {
                        var duration = Utils.Calendar.GetMonths(now, reportItem.Person.Badge.BadgeEndDate.Value);
                        timeLeftText = duration > 0 ? (duration == 1 ? duration + " month" : duration + " months") : "";
                    }
                }
                else
                {
                    timeLeftText = "Clock not started yet";
                }
                row.Add(timeLeftText);
                row.Add(reportItem.Person.Badge.IsMSManagedService ? "Y" : null);

                foreach (var item in reportItem.Range)
                {
                    dataCellStylearray.Add(dataNumberStyle);
                    row.Add(item.Available);

                }
                data.Rows.Add(row.ToArray());
                var datarowStyle = new RowStyles(dataCellStylearray.ToArray());
                rowStylesList.Add(datarowStyle);
            }


            //Total for Table-1

            row = new List<object>();

            for (int i = 0; i < 8; i++)
            {
                row.Add(string.Empty);
            }

            row.Add(string.Format(NPOIExcel.CustomColorWithBoldKey, "black", "TOTAL"));
            total = GetTotalsList();
            var dataTotalRowStyle = new List<CellStyles>() { dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCenterAlignedCell };

            var ResourceStartCell = headerRowsCount + 1;
            var ResourceEndCell = report.Count + headerRowsCount;
            var ResourceTableTotalRowNumber = ResourceEndCell + 1;
            for (int i = 0; i < total.Count; i++)
            {
                var ttl = total[i];
                row.Add(ttl.Total);
                var dataTotalNumberStyle1 = new CellStyles();
                dataTotalNumberStyle1.DataFormat = "#,##0_);[Red](#,##0)";
                dataTotalNumberStyle1.IsBold = true;
                if (i == 0)
                {
                    dataTotalNumberStyle1.CellFormula = string.Format(ManagedServiceCount_Format, ResourceStartCell, ResourceEndCell);
                }
                else if (i == total.Count - 1)
                {
                    dataTotalNumberStyle1.CellFormula = string.Format(AvailabilitySum_ByMonth_Format, "AB" + ResourceStartCell, "AB" + ResourceEndCell);
                }
                else if (i == total.Count - 2)
                {
                    dataTotalNumberStyle1.CellFormula = string.Format(AvailabilitySum_ByMonth_Format, "AA" + ResourceStartCell, "AA" + ResourceEndCell);
                }
                else
                {
                    dataTotalNumberStyle1.CellFormula = string.Format(AvailabilitySum_ByMonth_Format, GetAppropriateLetter(i - 1) + ResourceStartCell, GetAppropriateLetter(i - 1) + ResourceEndCell);
                }
                dataTotalRowStyle.Add(dataTotalNumberStyle1);
            }
            rowStylesList.Add(new RowStyles(dataTotalRowStyle.ToArray()));
            data.Rows.Add(row.ToArray());

            //Table-2
            row.Clear();
            var dataEmptyRowStyle = new List<CellStyles>();
            for (int i = 0; i < data.Columns.Count; i++)
            {
                dataEmptyRowStyle.Add(dataCellStyle);
                row.Add(string.Empty);
            }
            rowStylesList.Add(new RowStyles(dataEmptyRowStyle.ToArray()));
            data.Rows.Add(row.ToArray());

            //Header for table-2
            row.Clear();

            for (int i = 0; i < 8; i++)
            {
                row.Add(string.Empty);
            }
            row.Add(string.Format(NPOIExcel.CustomColorWithBoldKey, "black", "Total Available"));
            row.Add(string.Empty);
            foreach (var header in avlResourcesByTitle[0].Range)
            {
                row.Add(string.Format(NPOIExcel.CustomColorWithBoldKey, "black", header.StartDate.ToString(Constants.Formatting.CompPerfMonthYearFormat)));
            }
            rowStylesList.Add(headerrowStyle);
            data.Rows.Add(row.ToArray());
            var currentRowNumber = ResourceTableTotalRowNumber + 3;
            var AvailableStartRow = currentRowNumber;
            foreach (var avlItem in avlResourcesByTitle)
            {

                var dataCellStylearray = new List<CellStyles>();
                row = new List<object>();
                for (int i = 0; i < 8; i++)
                {
                    dataCellStylearray.Add(dataCellStyle);
                    row.Add(string.Empty);
                }
                row.Add(avlItem.Title.TitleName);
                row.Add(string.Empty);
                dataCellStylearray.Add(dataCellStyle);
                dataCellStylearray.Add(dataCellStyle);
                for (int i = 0; i < avlItem.Range.Count; i++)
                {
                    var item = avlItem.Range[i];
                    var dataNumberStyle1 = new CellStyles();
                    dataNumberStyle1.DataFormat = "#,##0_);[Red](#,##0)";
                    if (i == avlItem.Range.Count - 1)
                    {
                        dataNumberStyle1.CellFormula = string.Format(SUM_IF_Format, ResourceStartCell, ResourceEndCell, currentRowNumber, "AB");
                    }
                    else if (i == avlItem.Range.Count - 2)
                    {
                        dataNumberStyle1.CellFormula = string.Format(SUM_IF_Format, ResourceStartCell, ResourceEndCell, currentRowNumber, "AA");
                    }
                    else
                    {
                        dataNumberStyle1.CellFormula = string.Format(SUM_IF_Format, ResourceStartCell, ResourceEndCell, currentRowNumber, GetAppropriateLetter(i));
                    }
                    dataCellStylearray.Add(dataNumberStyle1);
                    row.Add(item.AvailableResourcesCount);
                }
                var datarowStyle = new RowStyles(dataCellStylearray.ToArray());
                rowStylesList.Add(datarowStyle);
                data.Rows.Add(row.ToArray());
                currentRowNumber++;
            }
            var AvailableRowEnd = currentRowNumber--;
            //Total for Table 2
            row.Clear();

            for (int i = 0; i < 8; i++)
            {
                row.Add(string.Empty);
            }
            row.Add(string.Format(NPOIExcel.CustomColorWithBoldKey, "black", "TOTAL"));
            row.Add(string.Empty);
            total = GetTotalsList(avlResourcesByTitle);
            var dataTotalRowStyle_Formula = new List<CellStyles>() { dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCenterAlignedCell, dataCellStyle };
            currentRowNumber++;
            for (int i = 0; i < total.Count; i++)
            {
                var ttl = total[i];
                row.Add(ttl.Total);
                var totalFormulaStyle = new CellStyles();
                totalFormulaStyle.DataFormat = "#,##0_);[Red](#,##0)";
                totalFormulaStyle.IsBold = true;
                if (i == total.Count - 1)
                {
                    totalFormulaStyle.CellFormula = string.Format(TotalAvailable_ByMonth_Format, "AB" + ResourceTableTotalRowNumber);
                }
                else if (i == total.Count - 2)
                {
                    totalFormulaStyle.CellFormula = string.Format(TotalAvailable_ByMonth_Format, "AA" + ResourceTableTotalRowNumber);
                }
                else
                {
                    totalFormulaStyle.CellFormula = string.Format(TotalAvailable_ByMonth_Format, GetAppropriateLetter(i) + ResourceTableTotalRowNumber);
                }
                dataTotalRowStyle_Formula.Add(totalFormulaStyle);
            }

            rowStylesList.Add(new RowStyles(dataTotalRowStyle_Formula.ToArray()));
            data.Rows.Add(row.ToArray());

            //Table-3
            row.Clear();
            for (int i = 0; i < data.Columns.Count; i++)
            {
                row.Add(string.Empty);
            }
            rowStylesList.Add(new RowStyles(dataEmptyRowStyle.ToArray()));
            currentRowNumber++;
            data.Rows.Add(row.ToArray());
            //Header for table-3
            row.Clear();
            for (int i = 0; i < 8; i++)
            {
                row.Add(string.Empty);
            }
            row.Add(string.Format(NPOIExcel.CustomColorWithBoldKey, "black", "Managed Service"));
            row.Add(string.Empty);
            foreach (var header in avlResourcesByTitle[0].Range)
            {
                row.Add(string.Format(NPOIExcel.CustomColorWithBoldKey, "black", header.StartDate.ToString(Constants.Formatting.CompPerfMonthYearFormat)));
            }

            rowStylesList.Add(headerrowStyle);
            currentRowNumber++;
            data.Rows.Add(row.ToArray());
            var manageServiceTableStartRow = currentRowNumber + 1;
            foreach (var avlItem in avlResourcesByTitle)
            {
                var dataCellStylearray = new List<CellStyles>();
                row = new List<object>();
                for (int i = 0; i < 8; i++)
                {
                    dataCellStylearray.Add(dataCellStyle);
                    row.Add(string.Empty);
                }

                row.Add(avlItem.Title.TitleName);
                row.Add(string.Empty);
                dataCellStylearray.Add(dataCellStyle);
                dataCellStylearray.Add(dataCellStyle);
                for (int i = 0; i < avlItem.Range.Count; i++)
                {
                    var item = avlItem.Range[i];
                    var dataFormulaStyle = new CellStyles();
                    dataFormulaStyle.DataFormat = "#,##0_);[Red](#,##0)";
                    if (i == avlItem.Range.Count - 1)
                    {
                        dataFormulaStyle.CellFormula = string.Format(SUM_IFS_Format, ResourceStartCell, ResourceEndCell, "AB", currentRowNumber + 1);
                    }
                    else if (i == avlItem.Range.Count - 2)
                    {
                        dataFormulaStyle.CellFormula = string.Format(SUM_IFS_Format, ResourceStartCell, ResourceEndCell, "AA", currentRowNumber + 1);
                    }
                    else
                    {
                        dataFormulaStyle.CellFormula = string.Format(SUM_IFS_Format, ResourceStartCell, ResourceEndCell, GetAppropriateLetter(i), currentRowNumber + 1);
                    }
                    dataCellStylearray.Add(dataFormulaStyle);
                    row.Add(item.MSCount);
                }
                var datarowStyle = new RowStyles(dataCellStylearray.ToArray());
                rowStylesList.Add(datarowStyle);
                data.Rows.Add(row.ToArray());
                currentRowNumber++;
            }
            var manageServiceTableEndRow = currentRowNumber;
            //Total for Table-3
            row.Clear();
            var dataTotalRowStyle_Manage = new List<CellStyles>() { dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCenterAlignedCell, dataCenterAlignedCell };
            for (int i = 0; i < 8; i++)
            {
                row.Add(string.Empty);
            }
            row.Add(string.Format(NPOIExcel.CustomColorWithBoldKey, "black", "TOTAL"));
            row.Add(string.Empty);
            currentRowNumber++;
            var managedTotalCountRow = currentRowNumber;
            for (int i = 0; i < total.Count; i++)
            {
                var ttl = total[i];
                row.Add(ttl.TotalResourcesWithMS);
                var totalFormulaStyle1 = new CellStyles();
                totalFormulaStyle1.DataFormat = "#,##0_);[Red](#,##0)";
                totalFormulaStyle1.IsBold = true;
                if (i == total.Count - 1)
                {
                    totalFormulaStyle1.CellFormula = string.Format(AvailabilitySum_ByMonth_Format, "AB" + manageServiceTableStartRow, "AB" + manageServiceTableEndRow);
                }
                else if (i == total.Count - 2)
                {
                    totalFormulaStyle1.CellFormula = string.Format(AvailabilitySum_ByMonth_Format, "AA" + manageServiceTableStartRow, "AA" + manageServiceTableEndRow);
                }
                else
                {
                    totalFormulaStyle1.CellFormula = string.Format(AvailabilitySum_ByMonth_Format, GetAppropriateLetter(i) + manageServiceTableStartRow, GetAppropriateLetter(i) + manageServiceTableEndRow);
                }
                dataTotalRowStyle_Manage.Add(totalFormulaStyle1);
            }
            rowStylesList.Add(new RowStyles(dataTotalRowStyle_Manage.ToArray()));
            data.Rows.Add(row.ToArray());
            currentRowNumber++;
            //Table-4
            row.Clear();
            for (int i = 0; i < data.Columns.Count; i++)
            {
                row.Add(string.Empty);
            }
            rowStylesList.Add(new RowStyles(dataEmptyRowStyle.ToArray()));
            currentRowNumber++;
            data.Rows.Add(row.ToArray());

            //Header for table-4
            row.Clear();

            for (int i = 0; i < 8; i++)
            {
                row.Add(string.Empty);
            }
            row.Add(string.Format(NPOIExcel.CustomColorWithBoldKey, "black", "Total Available (w/o Managed Service)"));

            row.Add(string.Empty);
            foreach (var header in avlResourcesByTitle[0].Range)
            {
                row.Add(string.Format(NPOIExcel.CustomColorWithBoldKey, "black", header.StartDate.ToString(Constants.Formatting.CompPerfMonthYearFormat)));
            }
            rowStylesList.Add(headerrowStyle);
            data.Rows.Add(row.ToArray());
            currentRowNumber++;
            var AvailableWithoutManageStart = currentRowNumber;
            int j = 0;
            foreach (var avlItem in avlResourcesByTitle)
            {
                var dataCellStylearray = new List<CellStyles>();
                row = new List<object>();
                for (int i = 0; i < 8; i++)
                {
                    dataCellStylearray.Add(dataCellStyle);
                    row.Add(string.Empty);
                }

                row.Add(avlItem.Title.TitleName);
                row.Add(string.Empty);
                dataCellStylearray.Add(dataCellStyle);
                dataCellStylearray.Add(dataCellStyle);
                for (int i = 0; i < avlItem.Range.Count; i++)
                {
                    var item = avlItem.Range[i];
                    var totalFormulaStyle1 = new CellStyles();
                    totalFormulaStyle1.DataFormat = "#,##0_);[Red](#,##0)";
                    if (i == avlItem.Range.Count - 1)
                    {
                        totalFormulaStyle1.CellFormula = string.Format(SUBTRACT_Format, "AB" + (AvailableStartRow + j), "AB" + (manageServiceTableStartRow + j));
                    }
                    else if (i == avlItem.Range.Count - 2)
                    {
                        totalFormulaStyle1.CellFormula = string.Format(SUBTRACT_Format, "AA" + (AvailableStartRow + j), "AA" + (manageServiceTableStartRow + j));
                    }
                    else
                    {
                        totalFormulaStyle1.CellFormula = string.Format(SUBTRACT_Format, GetAppropriateLetter(i) + (AvailableStartRow + j), GetAppropriateLetter(i) + (manageServiceTableStartRow + j));
                    }
                    dataCellStylearray.Add(totalFormulaStyle1);
                    row.Add(item.AvaliableResourcesWithOutMS);
                }
                var datarowStyle = new RowStyles(dataCellStylearray.ToArray());
                rowStylesList.Add(datarowStyle);
                data.Rows.Add(row.ToArray());
                currentRowNumber++;
                j++;
            }
            var AvailableWithoutManageEnd = currentRowNumber - 1;
            //Total for Table-4
            row.Clear();
            for (int i = 0; i < 8; i++)
            {
                row.Add(string.Empty);
            }
            row.Add(string.Format(NPOIExcel.CustomColorWithBoldKey, "black", "TOTAL"));
            row.Add(string.Empty);
            var dataTotalRowStyle_WithoutManage = new List<CellStyles>() { dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCenterAlignedCell, dataCenterAlignedCell };
            for (int i = 0; i < total.Count; i++)
            {
                var ttl = total[i];
                row.Add(ttl.TotalAvailableResourcesWithOutMS);
                var totalFormulaStyle1 = new CellStyles();
                totalFormulaStyle1.DataFormat = "#,##0_);[Red](#,##0)";
                totalFormulaStyle1.IsBold = true;
                if (i == total.Count - 1)
                {
                    totalFormulaStyle1.CellFormula = string.Format(AvailabilitySum_ByMonth_Format, "AB" + AvailableWithoutManageStart, "AB" + AvailableWithoutManageEnd);
                }
                else if (i == total.Count - 2)
                {
                    totalFormulaStyle1.CellFormula = string.Format(AvailabilitySum_ByMonth_Format, "AA" + AvailableWithoutManageStart, "AA" + AvailableWithoutManageEnd);
                }
                else
                {
                    totalFormulaStyle1.CellFormula = string.Format(AvailabilitySum_ByMonth_Format, GetAppropriateLetter(i) + AvailableWithoutManageStart, GetAppropriateLetter(i) + AvailableWithoutManageEnd);
                }
                dataTotalRowStyle_WithoutManage.Add(totalFormulaStyle1);
            }
            rowStylesList.Add(new RowStyles(dataTotalRowStyle_WithoutManage.ToArray()));
            data.Rows.Add(row.ToArray());
            currentRowNumber++;
            //Table-5
            var defaultValues = GetValuesForReport();
            defaultValues.ManagedServiceResources = total.First().TotalResourcesWithMS;
            CalculateDeficit(defaultValues, true);
            row.Clear();
            for (int i = 0; i < data.Columns.Count; i++)
            {
                row.Add(string.Empty);
            }
            rowStylesList.Add(new RowStyles(dataEmptyRowStyle.ToArray()));
            data.Rows.Add(row.ToArray());
            currentRowNumber++;
            //Header for table-5
            row.Clear();
            for (int i = 0; i < 6; i++)
            {
                row.Add(string.Empty);
            }
            row.Add("AVG");
            row.Add("Target");
            row.Add("Deficit");
            row.Add(string.Empty);
            foreach (var header in avlResourcesByTitle[0].Range)
            {
                row.Add(string.Format(NPOIExcel.CustomColorWithBoldKey, "black", header.StartDate.ToString(Constants.Formatting.CompPerfMonthYearFormat)));
            }
            rowStylesList.Add(headerrowStyle);
            data.Rows.Add(row.ToArray());
            currentRowNumber++;

            var deficitTableStartRow = currentRowNumber;
            var requiredCellRow = deficitTableStartRow + avlResourcesByTitle.Count;
            j = 0;
            foreach (var avlItem in avlResourcesByTitle)
            {
                var dataCellStylearray = new List<CellStyles>();
                row = new List<object>();
                for (int i = 0; i < 6; i++)
                {
                    dataCellStylearray.Add(dataCellStyle);
                    row.Add(string.Empty);
                }
                row.Add(avlItem.Average / 100);
                row.Add(avlItem.Target);
                row.Add(avlItem.Title.TitleName);
                row.Add(string.Empty);
                dataCellStylearray.Add(dataPercentageStyle);

                var totalFormulaStyle1 = new CellStyles();
                totalFormulaStyle1.DataFormat = "#,##0";
                totalFormulaStyle1.CellFormula = string.Format(Multiplication_Format, currentRowNumber, requiredCellRow);
                dataCellStylearray.Add(totalFormulaStyle1);
                dataCellStylearray.Add(dataCellStyle);
                dataCellStylearray.Add(dataCellStyle);
                for (int i = 0; i < avlItem.Range.Count; i++)
                {
                    var item = avlItem.Range[i];
                    var totalFormulaStyle2 = new CellStyles();
                    totalFormulaStyle2.DataFormat = "#,##0_);[Red](#,##0)";
                    if (i == avlItem.Range.Count - 1)
                    {
                        totalFormulaStyle2.CellFormula = string.Format(DeficitResourceCalc_Format, "AB" + (AvailableWithoutManageStart + j), currentRowNumber);
                    }
                    else if (i == avlItem.Range.Count - 2)
                    {
                        totalFormulaStyle2.CellFormula = string.Format(DeficitResourceCalc_Format, "AA" + (AvailableWithoutManageStart + j), currentRowNumber);
                    }
                    else
                    {
                        totalFormulaStyle2.CellFormula = string.Format(DeficitResourceCalc_Format, GetAppropriateLetter(i) + (AvailableWithoutManageStart + j), currentRowNumber);
                    }

                    dataCellStylearray.Add(totalFormulaStyle2);
                    if (item.RequiredResources == "-")
                        row.Add(item.RequiredResources);
                    else
                        row.Add(item.RequiredResourcesCount);
                }
                var datarowStyle = new RowStyles(dataCellStylearray.ToArray());
                rowStylesList.Add(datarowStyle);
                data.Rows.Add(row.ToArray());
                currentRowNumber++;
                j++;
            }
            var deficitTableEndRow = currentRowNumber - 1;
            var resourceRequiredLineNumber = currentRowNumber + 7;
            //Total for Table-5
            row.Clear();
            var dataTotalStyle = new List<CellStyles>();
            for (int i = 0; i < 7; i++)
            {
                dataTotalStyle.Add(dataCellStyle);
                row.Add(string.Empty);
            }

            row.Add(TotalTargetResources);
            row.Add("TOTAL");
            row.Add(string.Empty);
            var totalFormulaStyle3 = new CellStyles();
            totalFormulaStyle3.DataFormat = "#,##0";
            totalFormulaStyle3.CellFormula = string.Format(RequiredCellLocation_Format, resourceRequiredLineNumber);
            dataTotalStyle.Add(totalFormulaStyle3);
            dataTotalStyle.Add(headerWrapCellStyle);
            dataTotalStyle.Add(dataCellStyle);
            for (int i = 0; i < TotalsList.Count; i++)
            {
                var ttl = TotalsList[i];
                var totalFormulaStyle2 = new CellStyles();
                totalFormulaStyle2.DataFormat = "#,##0_);[Red](#,##0)";
                totalFormulaStyle2.IsBold = true;

                if (i == TotalsList.Count - 1)
                {
                    totalFormulaStyle2.CellFormula = string.Format(AvailabilitySum_ByMonth_Format, "AB" + deficitTableStartRow, "AB" + deficitTableEndRow);
                }
                else if (i == TotalsList.Count - 2)
                {
                    totalFormulaStyle2.CellFormula = string.Format(AvailabilitySum_ByMonth_Format, "AA" + deficitTableStartRow, "AA" + deficitTableEndRow);
                }
                else
                {
                    totalFormulaStyle2.CellFormula = string.Format(AvailabilitySum_ByMonth_Format, GetAppropriateLetter(i) + deficitTableStartRow, GetAppropriateLetter(i) + deficitTableEndRow);
                }

                dataTotalStyle.Add(totalFormulaStyle2);
                row.Add(ttl.TotalRequiredResources * -1);
            }
            rowStylesList.Add(new RowStyles(dataTotalStyle.ToArray()));
            data.Rows.Add(row.ToArray());
            currentRowNumber++;
            //Table-6
            row.Clear();
            for (int i = 0; i < data.Columns.Count; i++)
            {
                row.Add(string.Empty);
            }
            rowStylesList.Add(new RowStyles(dataEmptyRowStyle.ToArray()));
            data.Rows.Add(row.ToArray());
            currentRowNumber++;
            //Headings for Table-6
            row.Clear();
            for (int i = 0; i < 4; i++)
            {
                row.Add(string.Empty);
            }
            row.Add("Actual Rev/Hour");
            row.Add("Target Rev/Hour");
            row.Add("Hours - 90% Utilization");
            row.Add("Target Rev/Annual");
            rowStylesList.Add(headerrowStyle);
            data.Rows.Add(row.ToArray());
            currentRowNumber++;
            var targetRevenueLineNumber = currentRowNumber;
            row.Clear();
            var dataTargetStyle = new List<CellStyles>();
            for (int i = 0; i < 4; i++)
            {
                dataTargetStyle.Add(dataCellStyle);
                row.Add(string.Empty);
            }
            row.Add(defaultValues.ActualRevenuePerHour);
            row.Add(defaultValues.TargetRevenuePerHour);
            row.Add(defaultValues.HoursUtilization);
            row.Add(txtTotalRevenue.Text);
            row.Add("Total Revenue");
            var hoursUtilStyle = new CellStyles();
            hoursUtilStyle.DataFormat = "#,##0.00";
            dataTargetStyle.Add(dataCurrencyStyle);
            dataTargetStyle.Add(dataCurrencyStyle);
            dataTargetStyle.Add(hoursUtilStyle);
            dataTargetStyle.Add(dataCurrencyStyle);
            dataTargetStyle.Add(dataCellStyle);
            rowStylesList.Add(new RowStyles(dataTargetStyle.ToArray()));
            data.Rows.Add(row.ToArray());
            currentRowNumber++;
            var revFteLineNumber = currentRowNumber;
            row.Clear();

            var dataRequiredTableStyle = new List<CellStyles>();
            for (int i = 0; i < 7; i++)
            {
                row.Add(string.Empty);
                dataRequiredTableStyle.Add(dataCellStyle);
            }
            row.Add(defaultValues.TargetRevenuePerHour * defaultValues.HoursUtilization);
            row.Add("Rev/FTE");
            var dataCurrencyStyle1 = new CellStyles();
            dataCurrencyStyle1.DataFormat = "$#,##0.00_);($#,##0.00)";
            dataCurrencyStyle1.CellFormula = string.Format(REVFTE_Format, targetRevenueLineNumber);
            dataRequiredTableStyle.Add(dataCurrencyStyle1);
            dataRequiredTableStyle.Add(dataCellStyle);
            rowStylesList.Add(new RowStyles(dataRequiredTableStyle.ToArray()));
            data.Rows.Add(row.ToArray());
            currentRowNumber++;
            row.Clear();

            var dataCellStylearray1 = new List<CellStyles>();

            for (int i = 0; i < 7; i++)
            {
                dataCellStylearray1.Add(dataCellStyle);
                row.Add(string.Empty);
            }
            row.Add(lblFTE.Text);
            row.Add("FTE");

            var totalFormulaStyle4 = new CellStyles();
            totalFormulaStyle4.DataFormat = "#,##0";
            totalFormulaStyle4.CellFormula = string.Format(FTE_Format, targetRevenueLineNumber, revFteLineNumber);
            dataCellStylearray1.Add(totalFormulaStyle4);
            var fteRowNumber = currentRowNumber;
            dataCellStylearray1.Add(dataCellStyle);
            rowStylesList.Add(new RowStyles(dataCellStylearray1.ToArray()));
            data.Rows.Add(row.ToArray());
            currentRowNumber++;
            row.Clear();
            dataTargetStyle.Clear();
            for (int i = 0; i < 7; i++)
            {
                dataTargetStyle.Add(dataCellStyle);
                row.Add(string.Empty);
            }

            var totalFormulaStyle5 = new CellStyles();
            totalFormulaStyle5.DataFormat = "#,##0";
            totalFormulaStyle5.CellFormula = string.Format(MangedCountCurrent_Format, managedTotalCountRow);
            dataTargetStyle.Add(totalFormulaStyle5);
            var managedRowNumber = currentRowNumber;
            row.Add(lblManagedServiceCount.Text);
            row.Add("Managed Service");
            dataTargetStyle.Add(dataCellStyle);
            rowStylesList.Add(new RowStyles(dataTargetStyle.ToArray()));
            data.Rows.Add(row.ToArray());
            currentRowNumber++;
            row.Clear();
            dataTargetStyle.Clear();
            for (int i = 0; i < 7; i++)
            {
                dataTargetStyle.Add(dataCellStyle);
                row.Add(string.Empty);
            }
            row.Add(lblRequiredResources.Text);
            row.Add("Required");

            var totalFormulaStyle6 = new CellStyles();
            totalFormulaStyle6.DataFormat = "#,##0";
            totalFormulaStyle6.CellFormula = string.Format(RequiredCalc_Format, fteRowNumber, managedRowNumber);
            dataTargetStyle.Add(totalFormulaStyle6);

            dataTargetStyle.Add(dataCellStyle);
            rowStylesList.Add(new RowStyles(dataTargetStyle.ToArray()));
            data.Rows.Add(row.ToArray());
            DataStyles = new SheetStyles(rowStylesList.ToArray());
            DataStyles.TopRowNo = headerRowsCount;
            return data;
        }

        protected string GetDateFormat(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty;
        }

        protected string GetHeaderDateFormat(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString(Constants.Formatting.CompPerfMonthYearFormat) : string.Empty;
        }

        protected void repAllEmployeesMSReport_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                var repDatesHeaders = e.Item.FindControl("repDatesHeaders") as Repeater;
                var dates = PersonsList.First().Range;
                repDatesHeaders.DataSource = dates;
                repDatesHeaders.DataBind();
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var repCount = e.Item.FindControl("repCount") as Repeater;
                var dataItem = (ManagementMeetingReport)e.Item.DataItem;
                repCount.DataSource = dataItem.Range;
                repCount.DataBind();
                DateTime now = SettingsHelper.GetCurrentPMTime();
                var lblDuration = e.Item.FindControl("lblDuration") as Label;
                var lblManagedService = e.Item.FindControl("lblManagedService") as Label;
                if (dataItem.Person.Badge.IsMSManagedService)
                {
                    lblManagedService.Text = "Y";
                }
                else
                {
                    lblManagedService.Text = string.Empty;
                }
                if (dataItem.Person.Badge.BadgeStartDate.HasValue)
                {
                    if (now.Date >= dataItem.Person.Badge.BreakStartDate.Value.Date && now.Date <= dataItem.Person.Badge.BreakEndDate.Value.Date)
                    {
                        lblDuration.Text = "On 6-month break";
                    }
                    else if (now.Date > dataItem.Person.Badge.BreakEndDate.Value.Date)
                    {
                        lblDuration.Text = "Clock not restarted yet";
                    }
                    else
                    {
                        var duration = Utils.Calendar.GetMonths(now, dataItem.Person.Badge.BadgeEndDate.Value);
                        lblDuration.Text = duration > 0 ? (duration == 1 ? duration + " month" : duration + " months") : "";
                    }
                }
                else
                {
                    lblDuration.Text = "Clock not started yet";
                }
            }
            if (e.Item.ItemType == ListItemType.Footer)
            {
                var repTotal = e.Item.FindControl("repTotal") as Repeater;
                repTotal.DataSource = GetTotalsList();
                repTotal.DataBind();
            }
        }

        protected void repAvailableResources_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                var repDatesHeaders = e.Item.FindControl("repDatesHeaders") as Repeater;
                var dates = PersonsList.First().Range;
                repDatesHeaders.DataSource = dates;
                repDatesHeaders.DataBind();
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var repCount = e.Item.FindControl("repResourceCount") as Repeater;
                var dataItem = (ManagementMeetingReport)e.Item.DataItem;
                repCount.DataSource = dataItem.Range;
                repCount.DataBind();
            }
            else
            {
                if (e.Item.ItemType == ListItemType.Footer)
                {
                    var repTotal = e.Item.FindControl("repTotal") as Repeater;
                    repTotal.DataSource = TotalsList;
                    repTotal.DataBind();
                    var lblTotalTargetResources = e.Item.FindControl("lblTotalTargetResources") as Label;
                    if (lblTotalTargetResources != null)
                    {
                        lblTotalTargetResources.Text = TotalTargetResources.ToString();
                    }
                }
            }
        }

        protected void repRequiredCount_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lblResourceCount = e.Item.FindControl("lblResources") as Label;
                if (lblResourceCount.Text == "-" || lblResourceCount.Text == "0")
                {
                    lblResourceCount.ForeColor = Color.Black;
                }
                else
                {
                    lblResourceCount.ForeColor = Color.Red;
                }
            }

        }

        protected void repTotal_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lblTotalResources = e.Item.FindControl("lblTotalResources") as Label;
                var dataItem = (TotalByRange)e.Item.DataItem;
                if (dataItem.TotalRequiredResources == 0.00m)
                {
                    lblTotalResources.ForeColor = Color.Black;
                }
                else
                {
                    var total = Math.Round(dataItem.TotalRequiredResources, MidpointRounding.AwayFromZero);
                    lblTotalResources.Text = "(" + total + ")";
                    lblTotalResources.ForeColor = Color.Red;
                }

            }
        }

        //To find Totals for the Table-1 by Range
        public List<TotalByRange> GetTotalsList()
        {
            var list = new List<TotalByRange>();
            var people = PersonsList;
            list.Add(new TotalByRange { Total = people.Count(p => p.Person.Badge.IsMSManagedService) });
            foreach (var person in people)
            {
                foreach (var range in person.Range)
                {
                    if (list.Any(r => r.StartDate == range.StartDate))
                    {
                        var item = list.First(r => r.StartDate == range.StartDate);
                        item.Total += range.AvailableCount;
                    }
                    else
                    {
                        var item = new TotalByRange()
                        {
                            StartDate = range.StartDate,
                            Total = range.AvailableCount
                        };
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        //To find Totals for the Table-2,Table-3,Table-4 by Range
        public List<TotalByRange> GetTotalsList(List<ManagementMeetingReport> obj)
        {
            var list = new List<TotalByRange>();
            var people = obj;
            foreach (var person in people)
            {
                foreach (var range in person.Range)
                {
                    if (list.Any(r => r.StartDate == range.StartDate))
                    {
                        var item = list.First(r => r.StartDate == range.StartDate);
                        item.Total += range.AvailableResourcesCount;
                        item.TotalResourcesWithMS += range.MSCount;
                        item.TotalAvailableResourcesWithOutMS += range.AvaliableResourcesWithOutMS;
                    }
                    else
                    {
                        var item = new TotalByRange()
                        {
                            StartDate = range.StartDate,
                            Total = range.AvailableResourcesCount,
                            TotalResourcesWithMS = range.MSCount,
                            TotalAvailableResourcesWithOutMS = range.AvaliableResourcesWithOutMS,
                        };
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        //To find Totals for the Table-5 by Range
        public void GetTotalRequiredResources()
        {
            TotalsList = new List<TotalByRange>();
            foreach (var data in AvailableResources)
            {
                TotalTargetResources += data.Target;
                foreach (var range in data.Range)
                {
                    if (TotalsList.Any(r => r.StartDate == range.StartDate))
                    {
                        var item = TotalsList.FirstOrDefault(r => r.StartDate == range.StartDate);
                        item.TotalRequiredResources += (range.RequiredResourcesCount * -1);
                    }
                    else
                    {
                        var item = new TotalByRange()
                        {
                            StartDate = range.StartDate,
                            TotalRequiredResources = (range.RequiredResourcesCount * -1)
                        };
                        TotalsList.Add(item);
                    }
                }
            }
        }

        //Group the Resources By Title
        public List<ManagementMeetingReport> GetAvailableResourcesByTitle()
        {
            var list = new List<ManagementMeetingReport>();
            var people = PersonsList;
            foreach (var person in people)
            {
                if (list.Any(l => l.Title.TitleId == person.Title.TitleId))
                {
                    var item = list.FirstOrDefault(l => l.Title.TitleId == person.Title.TitleId);
                    foreach (var r in person.Range)
                    {
                        var range = item.Range.FirstOrDefault(s => s.StartDate == r.StartDate);
                        range.AvailableResourcesCount += r.AvailableCount;
                        range.MSCount += person.Person.Badge.IsMSManagedService && r.IsAvailable ? 1 : 0;
                    }

                }
                else
                {
                    var item = new ManagementMeetingReport() { Title = person.Title, Range = person.Range };

                    foreach (var r in person.Range)
                    {
                        var range = item.Range.FirstOrDefault(s => s.StartDate == r.StartDate);
                        range.AvailableResourcesCount += r.AvailableCount;
                        range.MSCount += person.Person.Badge.IsMSManagedService && r.IsAvailable ? 1 : 0;
                    }

                    list.Add(item);
                }
            }
            return list;
        }

        public void PopulateData()
        {
            PersonsList = ServiceCallers.Custom.Report(r => r.ManagedServiceReportByPerson(Paytypes, PersonStatus, StartDate, EndDate)).ToList();
            if (PersonsList.Any())
            {
                divWholePage.Visible = true;
                lblRange.Text = StartDate.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.ToString(Constants.Formatting.EntryDateFormat); lblRange.Text = StartDate.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.ToString(Constants.Formatting.EntryDateFormat);
                divEmptyMessage.Visible = false;
                repAllEmployeesMSReport.DataSource = PersonsList.OrderBy(p => p.Title.TitleName).ThenBy(p => p.Person.Name);
                repAllEmployeesMSReport.DataBind();
                AvailableResources = GetAvailableResourcesByTitle();
                TotalsList = GetTotalsList(AvailableResources);
                repAvailableResources.DataSource = AvailableResources.OrderBy(t => t.Title.TitleName);
                repAvailableResources.DataBind();
                repManagedService.DataSource = AvailableResources.OrderBy(t => t.Title.TitleName);
                repManagedService.DataBind();
                repAvlResourcesWithOutManagedService.DataSource = AvailableResources.OrderBy(t => t.Title.TitleName);
                repAvlResourcesWithOutManagedService.DataBind();

                int managedServiceCount = TotalsList.First().TotalResourcesWithMS;
                var report = GetValuesForReport();
                report.ManagedServiceResources = managedServiceCount;
                CalculateDeficit(report, false);
            }
            else
            {
                divWholePage.Visible = false;
                divEmptyMessage.Visible = true;
            }
        }

        //To get the default or onload values for the Table-6
        public RevenueReport GetValuesForReport()
        {
            string userLogin = Page.User.Identity.Name;
            RevenueReport report;
            report = ServiceCallers.Custom.Report(r => r.GetManagedParametersByPerson(userLogin));

            if (report == null)
            {
                report = new RevenueReport()
                {
                    ActualRevenuePerHour = ActualRevenue,
                    HoursUtilization = HoursUtilization,
                    TargetRevenuePerHour = TargetRevenue,
                    TotalRevenuePerAnnual = TotalRevenue
                };
            }
            return report;
        }

        //Calculcations for the Table-5
        public void CalculateDeficit(RevenueReport report, bool isExcel)
        {
            txtActualRevenuePerHour.Text = report.ActualRevenuePerHour.ToString();
            txtTargetRevenuePerHour.Text = report.TargetRevenuePerHour.ToString();
            txtHoursUtilization.Text = report.HoursUtilization.ToString();
            txtTotalRevenue.Text = report.TotalRevenuePerAnnual.ToString();
            var revenuePerFTE = report.TargetRevenuePerHour * report.HoursUtilization;
            lblRevenuePerFTE.Text = revenuePerFTE.ToString();
            report.FTE = report.TotalRevenuePerAnnual / revenuePerFTE;
            var fTE = report.FTE;
            if (!isExcel)
            {
                report.FTE = Math.Round(report.FTE, MidpointRounding.AwayFromZero);
            }
            lblFTE.Text = report.FTE.ToString();
            lblManagedServiceCount.Text = report.ManagedServiceResources.ToString();
            decimal requiredResources = (fTE - report.ManagedServiceResources);
            lblRequiredResources.Text = Math.Round((fTE - report.ManagedServiceResources), MidpointRounding.AwayFromZero).ToString(); //requiredResources.ToString();
            if (PersonsList == null)
            {
                PersonsList = ServiceCallers.Custom.Report(r => r.ManagedServiceReportByPerson(Paytypes, PersonStatus, StartDate, EndDate)).ToList();
                AvailableResources = GetAvailableResourcesByTitle();
            }
            string titles = string.Empty;
            foreach (var resource in AvailableResources)
            {
                titles += resource.Title.TitleId + ",";
            }
            var resourceCountForAverage = ServiceCallers.Custom.Report(r => r.GetAveragePercentagesByTitles(Paytypes, PersonStatus, titles, StartDate, AverageEndDate));
            //Calculte totals
            GroupbyTitle totalCountByRange = new GroupbyTitle() { ResourcesCount = new List<BadgedResourcesByTime>() };
            foreach (var resourceByTitle in resourceCountForAverage)
            {
                foreach (var range in resourceByTitle.ResourcesCount)
                {
                    if (totalCountByRange.ResourcesCount.Any(r => r.StartDate == range.StartDate))
                    {
                        var item = totalCountByRange.ResourcesCount.First(r => r.StartDate == range.StartDate);
                        item.ResourceCount += range.ResourceCount;
                    }
                    else
                    {
                        var temp = new BadgedResourcesByTime();
                        temp.StartDate = range.StartDate;
                        temp.ResourceCount = range.ResourceCount;
                        totalCountByRange.ResourcesCount.Add(temp);
                    }
                }
            }
            //Calculte Percentage for each individual Title
            foreach (var resourceByTitle in resourceCountForAverage)
            {
                foreach (var range in resourceByTitle.ResourcesCount)
                {
                    var total = totalCountByRange.ResourcesCount.First(t => t.StartDate == range.StartDate);
                    decimal percent = total.ResourceCount == 0 ? 0 : (range.ResourceCount * 100M / total.ResourceCount);
                    range.Percentage = percent;
                }
            }
            //Calculte Avg Percentage for Title
            foreach (var resourcePercentage in resourceCountForAverage)
            {
                decimal percentage = 0;
                var temp = AvailableResources.First(r => r.Title.TitleId == resourcePercentage.Title.TitleId);
                foreach (var range in resourcePercentage.ResourcesCount)
                {
                    percentage += range.Percentage;
                }
                temp.Average = percentage / 6;
            }
            foreach (var data in AvailableResources)
            {
                data.Target = (data.Average * requiredResources) / 100M;
                foreach (var range in data.Range)
                {
                    decimal temp = data.Target;
                    
                    range.RequiredResourcesCount = (temp >= range.AvaliableResourcesWithOutMS) ? range.AvaliableResourcesWithOutMS - temp : 0;
                    range.RequiredResources = (temp >= range.AvaliableResourcesWithOutMS) ? (range.AvaliableResourcesWithOutMS == temp ? "0" : "(" + Math.Round((temp - range.AvaliableResourcesWithOutMS),MidpointRounding.AwayFromZero) + ")") : "-";
                }
                if (!isExcel)
                {
                    data.Target = Math.Round(data.Target);
                    data.Average = Math.Round(data.Average);
                }
            }
            GetTotalRequiredResources();
            repDeficit.DataSource = AvailableResources.OrderBy(t => t.Title.TitleName);
            repDeficit.DataBind();
        }

        protected void btnSubmit_Clicked(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                RevenueReport report = new RevenueReport()
                {
                    ActualRevenuePerHour = Convert.ToDecimal(txtActualRevenuePerHour.Text),
                    HoursUtilization = Convert.ToDecimal(txtHoursUtilization.Text),
                    TargetRevenuePerHour = Convert.ToDecimal(txtTargetRevenuePerHour.Text),
                    TotalRevenuePerAnnual = Convert.ToDecimal(txtTotalRevenue.Text),
                    ManagedServiceResources = Convert.ToInt32(lblManagedServiceCount.Text)
                };
                string userLogin = Page.User.Identity.Name;
                ServiceCallers.Custom.Report(r => r.SaveManagedParametersByPerson(userLogin, report.ActualRevenuePerHour, report.TargetRevenuePerHour, report.HoursUtilization, report.TotalRevenuePerAnnual));
                CalculateDeficit(report, false);
            }
        }

        private void SaveFilterValuesForSession()
        {
            MsManagementReportFilters filter = new MsManagementReportFilters();
            filter.PersonStatusIds = cblPersonStatus.SelectedItems;
            filter.PayTypeIds = cblPayTypes.SelectedItems;
            ReportsFilterHelper.SaveFilterValues(ReportName.MSManagementMeetingReport, filter);
        }

        private void GetFilterValuesForSession()
        {
            var filters = ReportsFilterHelper.GetFilterValues(ReportName.MSManagementMeetingReport) as MsManagementReportFilters;
            if (filters != null)
            {
                cblPayTypes.UnSelectAll();
                cblPayTypes.SelectedItems=filters.PayTypeIds;
                cblPersonStatus.UnSelectAll();
                cblPersonStatus.SelectedItems=filters.PersonStatusIds;
            }
            PopulateData();
        }
    }
}

