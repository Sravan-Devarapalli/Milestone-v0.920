using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using DataTransferObjects;
using System.ComponentModel;
using PraticeManagement.Security;
using System.Data;
using PraticeManagement.Utils;
using PraticeManagement.Utils.Excel;
using DataTransferObjects.Reports;

namespace PraticeManagement.Reports
{
    public partial class AttainmentReport : System.Web.UI.Page
    {
        private const string Revenue = "Revenue";
        private const string Margin = "Cont. Margin";
        private const string ExportDateRangeFormat = "Date Range: {0} - {1}";

        private bool renderMonthColumns;
        private int headerRowsCount = 1;
        private int billingheaderRowsCount = 1;
        private int coloumnsCount = 1;
        private int billingcoloumnsCount = 1;

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
                    var now = Utils.Generic.GetNowWithTimeZone();
                    DateTime CurrentYearStartdate = Utils.Calendar.YearStartDate(now).Date;
                    DateTime CurrentYearEnddate = Utils.Calendar.YearEndDate(now).Date;
                    _BillableUtlizationList = ServiceCallers.Custom.Report(p => p.AttainmentBillableutlizationReport(CurrentYearStartdate, CurrentYearEnddate));
                }
                return (AttainmentBillableutlizationReport[])_BillableUtlizationList;
            }
        }

        private SheetStyles HeaderSheetStyle
        {
            get
            {
                CellStyles cellStyle = new CellStyles();
                cellStyle.IsBold = true;
                cellStyle.BorderStyle = NPOI.SS.UserModel.BorderStyle.NONE;
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
                headerCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;

                CellStyles monthNameHeaderCellStyle = new CellStyles();
                monthNameHeaderCellStyle.DataFormat = "[$-409]mmmm-yy;@";
                monthNameHeaderCellStyle.IsBold = true;
                monthNameHeaderCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;

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
                dataStartDateCellStyle.DataFormat = "mm/dd/yyyy";

                CellStyles dataNumberDateCellStyle = new CellStyles();
                dataNumberDateCellStyle.DataFormat = "_($#,##0.00_);[Red]($#,##0.00)";


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

                RowStyles datarowStyle = new RowStyles(dataCellStyleList.ToArray());

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.TopRowNo = headerRowsCount;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanColSplit = 0;
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
                cellStyle.BorderStyle = NPOI.SS.UserModel.BorderStyle.NONE;
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
                headerCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;

                CellStyles monthNameHeaderCellStyle = new CellStyles();
                monthNameHeaderCellStyle.DataFormat = "[$-409]mmmm-yy;@";
                monthNameHeaderCellStyle.IsBold = true;
                monthNameHeaderCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;

                List<CellStyles> headerCellStyleList = new List<CellStyles>();
                for (int i = 1; i <= 3; i++)
                    headerCellStyleList.Add(headerCellStyle);
                for (int i = 1; i <= 12; i++)
                    headerCellStyleList.Add(monthNameHeaderCellStyle);
                headerCellStyleList.Add(headerCellStyle);

                List<int> coloumnWidth = new List<int>();
                for (int i = 1; i <= 3; i++)
                    coloumnWidth.Add(0);
                for (int i = 1; i <= 12; i++)
                    coloumnWidth.Add(13);
                for (int i = 1; i <= 5; i++)
                    coloumnWidth.Add(8);

                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataCellStyle = new CellStyles();
                CellStyles decimaldataCellStyle = new CellStyles();
                decimaldataCellStyle.DataFormat = "0.0%";

                CellStyles[] dataCellStylearray = { dataCellStyle, dataCellStyle, dataCellStyle, decimaldataCellStyle };
                List<CellStyles> dataCellStyleList = dataCellStylearray.ToList();

                RowStyles datarowStyle = new RowStyles(dataCellStyleList.ToArray());

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.TopRowNo = billingheaderRowsCount;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanColSplit = 0;
                sheetStyle.FreezePanRowSplit = billingheaderRowsCount;
                sheetStyle.ColoumnWidths = coloumnWidth;
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
                return CompanyPerformanceState.ProjectList;

            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetPeriodSelection(3);
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
                     ShowInactive = false,
                     PeriodSelected = Convert.ToInt32(ddlPeriod.SelectedValue),
                     ViewSelected = -1,
                     CalculateRangeSelected = (ProjectCalculateRangeType)1,
                     HideAdvancedFilter = false,
                     UseActualTimeEntries = true
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
                names += person.Name + "\n";
            }

            names = names.Remove(names.LastIndexOf("\n"));
            return names;
        }

        private static bool IsInMonth(DateTime date, DateTime periodStart, DateTime periodEnd)
        {
            var result =
                (date.Year > periodStart.Year ||
                (date.Year == periodStart.Year && date.Month >= periodStart.Month)) &&
                (date.Year < periodEnd.Year || (date.Year == periodEnd.Year && date.Month <= periodEnd.Month));

            return result;
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
            data.Columns.Add("Project Manager(s)");
            data.Columns.Add("Senior Manager");
            data.Columns.Add("Director");
            data.Columns.Add("Pricing List");
            //	  data.Columns.Add("CSAT OWNER");
            foreach (var propertyBag in propertyBags)
            {
                var objects = new object[data.Columns.Count];
                int column = 0;
                Project project = new Project();
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(propertyBag))
                {
                    if (property.Name != "ProjectID")
                    {
                        objects[column] = property.GetValue(propertyBag);
                        if (property.Name == "ProjectNumber")
                        {
                            project = projectsList.Where(p => p.ProjectNumber == property.GetValue(propertyBag).ToString()).FirstOrDefault();
                        }
                        if (property.Name == "Type")
                        {
                            bool isMargin = property.GetValue(propertyBag).ToString() == Margin;
                            //Add month columns.

                            SeniorityAnalyzer personListAnalyzer = new SeniorityAnalyzer(DataHelper.CurrentPerson);
                            personListAnalyzer.OneWithGreaterSeniorityExists(project.ProjectPersons);
                            bool greaterSeniorityExists = personListAnalyzer != null && personListAnalyzer.GreaterSeniorityExists;
                            //PracticeManagementCurrency columnValue = 0M;
                            //columnValue.FormatStyle = marginType ? NumberFormatStyle.Margin : NumberFormatStyle.Revenue;

                            var columnValue = 0M;
                            if (renderMonthColumns)
                            {
                                var monthStart = periodStart;
                                // Displaying the month values (main cell data)
                                for (int k = 0;
                                    k < monthsInPeriod;
                                    k++, monthStart = monthStart.AddMonths(1))
                                {
                                    column++;
                                    columnValue = 0M;
                                    DateTime monthEnd = GetMonthEnd(ref monthStart);

                                    if (project.ProjectedFinancialsByMonth != null)
                                    {
                                        foreach (KeyValuePair<DateTime, ComputedFinancials> interestValue in
                                            project.ProjectedFinancialsByMonth)
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
                                }
                            }

                            column++;
                            columnValue = 0M;
                            if (project.ComputedFinancials != null && !greaterSeniorityExists)
                            {
                                columnValue = isMargin ? (useActuals ? project.ComputedFinancials.ActualGrossMargin : project.ComputedFinancials.GrossMargin) : (useActuals ? project.ComputedFinancials.ActualRevenue : project.ComputedFinancials.Revenue);
                            }
                            string totalColomncolor = columnValue < 0 ? "red" : isMargin ? "purple" : "green";
                            objects[column] = string.Format(NPOIExcel.CustomColorKey, totalColomncolor, greaterSeniorityExists && isMargin ? (object)"(Hidden)" : columnValue);
                        }
                        else if (property.Name == "ProjectManagers")
                        {
                            objects[column] = project.ProjectManagers != null ? GetProjectManagers(project.ProjectManagers) : string.Empty;
                        }
                        column++;
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
                    coloumnsAll.Add(bu.RangeType);
                }
                var now = Utils.Generic.GetNowWithTimeZone();
                var yearStart = Utils.Calendar.YearStartDate(now);

                List<object> row;

                data.Columns.Add("Resource Name");
                data.Columns.Add("Title");
                data.Columns.Add("Pay Type");
                foreach (string s in coloumnsAll)
                {
                    data.Columns.Add(s);
                }

                foreach (var per in attainmentBillableutlizationList)
                {
                    row = new List<object>();
                    int i;
                    row.Add(per.Person != null ? per.Person.PersonLastFirstName : "");
                    row.Add(per.Person != null && per.Person.Title != null ? per.Person.Title.HtmlEncodedTitleName : "");
                    row.Add(per.Person != null && per.Person.CurrentPay != null ? per.Person.CurrentPay.TimescaleName : "");
                    for (i = 0; i < per.BillableUtilizationList.Count; i++)
                    {
                        row.Add(per.BillableUtilizationList[i].BillableUtilization == -1 ? "" : per.BillableUtilizationList[i].BillableUtilization.ToString());
                    }
                    data.Rows.Add(row.ToArray());
                }
            }
            return data;
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
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
                                    PricingList = (pro.PricingList != null && pro.PricingList.Name != null) ? pro.PricingList.Name : string.Empty
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
                                              PricingList = (pro.PricingList != null && pro.PricingList.Name != null) ? pro.PricingList.Name : string.Empty
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
            billableUtilheader.Columns.Add("Billable Utilization: " + CurrentYearStartdate.Year);
            billingheaderRowsCount = billableUtilheader.Rows.Count + 3;

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

            NPOIExcel.Export("AttainmentReportingDataSource.xls", dataSetList, sheetStylesList);
        }
    }
}

