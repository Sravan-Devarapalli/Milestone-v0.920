using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using DataTransferObjects;
using System.Web.UI.HtmlControls;
using PraticeManagement.Utils.Excel;
using System.Data;
using PraticeManagement.Utils;

namespace PraticeManagement.Controls.Reports
{
    public partial class BillingReportSummary : System.Web.UI.UserControl
    {
        private const string BillingReportExport = "Billing Report";
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;

        private PraticeManagement.Reports.BillingReport HostingPage
        {
            get { return ((PraticeManagement.Reports.BillingReport)Page); }
        }

        private SheetStyles HeaderSheetStyle
        {
            get
            {
                var cellStyle = new CellStyles();
                cellStyle.IsBold = true;
                cellStyle.BorderStyle = NPOI.SS.UserModel.BorderStyle.None;
                cellStyle.FontHeight = 350;
                CellStyles[] cellStylearray = { cellStyle };
                var headerrowStyle = new RowStyles(cellStylearray);
                headerrowStyle.Height = 500;

                var dataCellStyle = new CellStyles();
                dataCellStyle.IsBold = true;
                dataCellStyle.BorderStyle = NPOI.SS.UserModel.BorderStyle.None;
                dataCellStyle.FontHeight = 200;
                CellStyles[] dataCellStylearray = { dataCellStyle };
                var datarowStyle = new RowStyles(dataCellStylearray);
                datarowStyle.Height = 350;

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };

                var sheetStyle = new SheetStyles(rowStylearray);
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
                var headerCellStyleList = new List<CellStyles> { headerCellStyle };
                var headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                var dataCellStyle = new CellStyles();

                var dataCurrancyCellStyle = new CellStyles { DataFormat = "$#,##0.00_);($#,##0.00)" };

                CellStyles[] dataCellStylearray = { dataCellStyle, 
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                 HostingPage.IsHoursUnitOfMeasure?dataCellStyle:dataCurrancyCellStyle,
                                                 HostingPage.IsHoursUnitOfMeasure?dataCellStyle:dataCurrancyCellStyle,
                                                 HostingPage.IsHoursUnitOfMeasure?dataCellStyle:dataCurrancyCellStyle,
                                                 HostingPage.IsHoursUnitOfMeasure?dataCellStyle:dataCurrancyCellStyle,
                                                 HostingPage.IsHoursUnitOfMeasure?dataCellStyle:dataCurrancyCellStyle,
                                                 HostingPage.IsHoursUnitOfMeasure?dataCellStyle:dataCurrancyCellStyle,
                                                 dataCellStyle, 
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle
                                                  };

                var datarowStyle = new RowStyles(dataCellStylearray);

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
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

        private HtmlImage ImgAccountFilter { get; set; }

        private HtmlImage ImgPracticeFilter { get; set; }

        private HtmlImage ImgSalespersonFilter { get; set; }

        private HtmlImage ImgProjectManagerFilter { get; set; }

        private HtmlImage ImgSeniorManagerFilter { get; set; }

        private HtmlImage ImgDirectorFilter { get; set; }

        private String DirectorIds
        {
            get
            {
                if (HostingPage.DirectorFilteredIds == null)
                {
                    return HostingPage.DirectorIds;
                }

                if (HostingPage.DirectorFilteredIds != null)
                {
                    return HostingPage.DirectorFilteredIds;
                }
                HostingPage.DirectorFilteredIds = cblDirectorFilter.SelectedItems;
                return cblDirectorFilter.SelectedItems;
            }
        }

        private String AccountIds
        {
            get
            {
                if (HostingPage.AccountFilteredIds == null)
                {
                    return HostingPage.AccountIds;
                }

                if (HostingPage.AccountFilteredIds != null)
                {
                    return HostingPage.AccountFilteredIds;
                }
                HostingPage.AccountFilteredIds = cblAccountFilter.SelectedItems;
                return cblAccountFilter.SelectedItems;
            }
        }

        public PracticeManagementCurrency TotalRangeProjected
        {
            get;
            set;
        }

        public PracticeManagementCurrency TotalRangeActual
        {
            get;
            set;
        }

        public PracticeManagementCurrency TotalRangeDifference
        {
            get;
            set;
        }

        public PracticeManagementCurrency TotalSOWBudget
        {
            get;
            set;
        }

        public PracticeManagementCurrency TotalActualtoDate
        {
            get;
            set;
        }

        public PracticeManagementCurrency TotalRemaining
        {
            get;
            set;
        }

        public double TotalForecastedHours
        {
            get;
            set;
        }

        public double TotalActualHours
        {
            get;
            set;
        }

        public double TotalForecastedHoursInRange
        {
            get;
            set;
        }

        public double TotalActualHoursInRange
        {
            get;
            set;
        }

        public double TotalDifferenceInHours
        {
            get;
            set;
        }

        public double TotalRemainingHours
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            cblAccountFilter.OKButtonId = cblPracticeFilter.OKButtonId = cblProjectManagers.OKButtonId = cblSalespersonFilter.OKButtonId = cblSeniorManager.OKButtonId = cblDirectorFilter.OKButtonId = btnFilterOK.ClientID;
        }

        protected void repBillingReport_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                var lblLifetoDateProjected = e.Item.FindControl("lblLifetoDateProjected") as Label;
                var lblLifetoDateActual = e.Item.FindControl("lblLifetoDateActual") as Label;
                var lblLifetoDateRemaining = e.Item.FindControl("lblLifetoDateRemaining") as Label;
                ImgAccountFilter = e.Item.FindControl("ImgAccountFilter") as HtmlImage;
                ImgPracticeFilter = e.Item.FindControl("ImgPracticeFilter") as HtmlImage;
                ImgSalespersonFilter = e.Item.FindControl("ImgSalespersonFilter") as HtmlImage;
                ImgProjectManagerFilter = e.Item.FindControl("ImgProjectManagerFilter") as HtmlImage;
                ImgSeniorManagerFilter = e.Item.FindControl("ImgSeniorManagerFilter") as HtmlImage;
                ImgDirectorFilter = e.Item.FindControl("ImgDirectorFilter") as HtmlImage;
                if (HostingPage.IsHoursUnitOfMeasure)
                {
                    lblLifetoDateProjected.Text = "Total Projected Hours";
                    lblLifetoDateActual.Text = "Actual to Date";
                    lblLifetoDateRemaining.Text = "Remaining";
                }
                else
                {
                    lblLifetoDateProjected.Text = "SOW Budget";
                    lblLifetoDateActual.Text = "Actual to Date";
                    lblLifetoDateRemaining.Text = "Remaining";
                }
            }

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lblLifetoDateProjectedValue = e.Item.FindControl("lblLifetoDateProjectedValue") as Label;
                var lblLifetoDateActualValue = e.Item.FindControl("lblLifetoDateActualValue") as Label;
                var lblLifetoDateRemainingValue = e.Item.FindControl("lblLifetoDateRemainingValue") as Label;
                var lblProjectManagers = e.Item.FindControl("lblProjectManagers") as Label;
                var lblRangeProjectedValue = e.Item.FindControl("lblRangeProjectedValue") as Label;
                var lblRangeActual = e.Item.FindControl("lblRangeActual") as Label;
                var lblRangeDifference = e.Item.FindControl("lblRangeDifference") as Label;
                var lblDirector = e.Item.FindControl("lblDirector") as Label;
                var dataitem = (BillingReport)e.Item.DataItem;
                if (HostingPage.IsHoursUnitOfMeasure)
                {
                    lblLifetoDateProjectedValue.Text = GetDoubleFormat(dataitem.ForecastedHours);
                    lblLifetoDateActualValue.Text = GetDoubleFormat(dataitem.ActualHours);
                    lblLifetoDateRemainingValue.Text = GetDoubleFormat(dataitem.RemainingHours);
                    lblRangeActual.Text = GetDoubleFormat(dataitem.ActualHoursInRange);
                    lblRangeProjectedValue.Text = GetDoubleFormat(dataitem.ForecastedHoursInRange);
                    lblRangeDifference.Text = GetDoubleFormat(dataitem.DifferenceInHours);
                }
                else
                {
                    lblLifetoDateProjectedValue.Text = dataitem.SOWBudget.ToString();
                    lblLifetoDateActualValue.Text = dataitem.ActualToDate.ToString();
                    lblLifetoDateRemainingValue.Text = dataitem.Remaining.ToString();
                    lblRangeActual.Text = dataitem.RangeActual.ToString();
                    lblRangeProjectedValue.Text = dataitem.RangeProjected.ToString();
                    lblRangeDifference.Text = dataitem.DifferenceInCurrency.ToString();
                }
                lblProjectManagers.Text = (dataitem.Project.ProjectManagers == null || dataitem.Project.ProjectManagers.Count == 0) ? string.Empty : dataitem.ProjectMangers;
                lblDirector.Text = dataitem.Project.Director.Id.HasValue ? dataitem.Project.Director.HtmlEncodedName : string.Empty;
            }
            if (e.Item.ItemType == ListItemType.Footer)
            {
                var lblTotalLifetoDateProjectedValue = e.Item.FindControl("lblTotalLifetoDateProjectedValue") as Label;
                var lblTotalLifetoDateActualValue = e.Item.FindControl("lblTotalLifetoDateActualValue") as Label;
                var lblTotalLifetoDateRemainingValue = e.Item.FindControl("lblTotalLifetoDateRemainingValue") as Label;
                var lblTotalRangeProjectedValue = e.Item.FindControl("lblTotalRangeProjectedValue") as Label;
                var lblTotalRangeActual = e.Item.FindControl("lblTotalRangeActual") as Label;
                var lblTotalRangeDifference = e.Item.FindControl("lblTotalRangeDifference") as Label;
                if (HostingPage.IsHoursUnitOfMeasure)
                {
                    lblTotalLifetoDateProjectedValue.Text = GetDoubleFormat(TotalForecastedHours);
                    lblTotalLifetoDateActualValue.Text = GetDoubleFormat(TotalActualHours);
                    lblTotalLifetoDateRemainingValue.Text = GetDoubleFormat(TotalRemainingHours);
                    lblTotalRangeProjectedValue.Text = GetDoubleFormat(TotalForecastedHoursInRange);
                    lblTotalRangeActual.Text = GetDoubleFormat(TotalActualHoursInRange);
                    lblTotalRangeDifference.Text = GetDoubleFormat(TotalDifferenceInHours);
                }
                else
                {
                    lblTotalLifetoDateProjectedValue.Text = TotalSOWBudget.ToString();
                    lblTotalLifetoDateActualValue.Text = TotalActualtoDate.ToString();
                    lblTotalLifetoDateRemainingValue.Text = TotalRemaining.ToString();
                    lblTotalRangeProjectedValue.Text = TotalRangeProjected.ToString();
                    lblTotalRangeActual.Text = TotalRangeActual.ToString();
                    lblTotalRangeDifference.Text = TotalRangeDifference.ToString();
                }
            }
        }

        protected string GetProjectDetailsLink(int? projectId)
        {
            if (projectId.HasValue)
                return Utils.Generic.GetTargetUrlWithReturn(String.Format(Constants.ApplicationPages.DetailRedirectFormat, Constants.ApplicationPages.ProjectDetail, projectId.Value),
                                                            Constants.ApplicationPages.BillingReport);
            return string.Empty;
        }

        protected void btnFilterOK_OnClick(object sender, EventArgs e)
        {
            HostingPage.DirectorFilteredIds = cblDirectorFilter.SelectedItems;
            HostingPage.AccountFilteredIds = cblAccountFilter.SelectedItems;
            PopulateData(false);
        }

        public bool IsHoursData()
        {
            return HostingPage.IsHoursUnitOfMeasure;
        }

        public void PopulateData(bool isPopulateFilters = true)
        {
            List<BillingReport> report;
            if (isPopulateFilters)
            {
                report = HostingPage.IsHoursUnitOfMeasure ? ServiceCallers.Custom.Report(r => r.BillingReportByHours(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.PracticeIds, HostingPage.AccountIds, HostingPage.BusinessUnitIds, HostingPage.DirectorIds, null, null, null).ToList())
                    : ServiceCallers.Custom.Report(r => r.BillingReportByCurrency(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.PracticeIds, HostingPage.AccountIds, HostingPage.BusinessUnitIds, HostingPage.DirectorIds, null, null, null).ToList());
            }
            else
            {
                report = HostingPage.IsHoursUnitOfMeasure ? ServiceCallers.Custom.Report(r => r.BillingReportByHours(HostingPage.StartDate.Value, HostingPage.EndDate.Value, cblPracticeFilter.SelectedItems, AccountIds, HostingPage.BusinessUnitIds, DirectorIds, cblSalespersonFilter.SelectedItems, cblProjectManagers.SelectedItems, cblSeniorManager.SelectedItems).ToList())
                    : ServiceCallers.Custom.Report(r => r.BillingReportByCurrency(HostingPage.StartDate.Value, HostingPage.EndDate.Value, cblPracticeFilter.SelectedItems, AccountIds, HostingPage.BusinessUnitIds, DirectorIds, cblSalespersonFilter.SelectedItems, cblProjectManagers.SelectedItems, cblSeniorManager.SelectedItems).ToList());
            }
            AssignTotalValues(HostingPage.IsHoursUnitOfMeasure, report);
            DataBindBusinesUnit(report.ToArray(), isPopulateFilters);
        }

        public void AssignTotalValues(bool isHours, List<BillingReport> report)
        {
            if (isHours)
            {
                TotalActualHours = report.Sum(s => s.ActualHours);
                TotalActualHoursInRange = report.Sum(s => s.ActualHoursInRange);
                TotalDifferenceInHours = report.Sum(s => s.DifferenceInHours);
                TotalForecastedHours = report.Sum(s => s.ForecastedHours);
                TotalForecastedHoursInRange = report.Sum(s => s.ForecastedHoursInRange);
                TotalRemainingHours = report.Sum(s => s.RemainingHours);
            }
            else
            {
                TotalActualtoDate = report.Sum(s => s.ActualToDate);
                TotalRangeActual = report.Sum(s => s.RangeActual);
                TotalRangeDifference = report.Sum(s => s.DifferenceInCurrency);
                TotalRangeProjected = report.Sum(s => s.RangeProjected);
                TotalRemaining = report.Sum(s => s.Remaining);
                TotalSOWBudget = report.Sum(s => s.SOWBudget);
            }
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.NumberFormatWithCommasAndDecimals);
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            //“TimePeriod_ByProject_DateRange.xls”.  
            var filename = string.Format("BillingReport_{0}-{1}.xls",
                HostingPage.StartDate.Value.ToString("MM_dd_yyyy"), HostingPage.EndDate.Value.ToString("MM_dd_yyyy"));
            DataHelper.InsertExportActivityLogMessage(BillingReportExport);
            var sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();
            var report = HostingPage.IsHoursUnitOfMeasure ? ServiceCallers.Custom.Report(r => r.BillingReportByHours(HostingPage.StartDate.Value, HostingPage.EndDate.Value, cblPracticeFilter.SelectedItems, AccountIds, HostingPage.BusinessUnitIds, DirectorIds, cblSalespersonFilter.SelectedItems, cblProjectManagers.SelectedItems, cblSeniorManager.SelectedItems).ToList().OrderBy(p => p.Project.ProjectNumber).ToList())
                    : ServiceCallers.Custom.Report(r => r.BillingReportByCurrency(HostingPage.StartDate.Value, HostingPage.EndDate.Value, cblPracticeFilter.SelectedItems, AccountIds, HostingPage.BusinessUnitIds, DirectorIds, cblSalespersonFilter.SelectedItems, cblProjectManagers.SelectedItems, cblSeniorManager.SelectedItems).ToList().OrderBy(p => p.Project.ProjectNumber).ToList());
            AssignTotalValues(HostingPage.IsHoursUnitOfMeasure, report);
            var filterApplied = "Filters applied to columns: ";
            var filteredColoums = new List<string>();
            if (!cblAccountFilter.AllItemsSelected)
            {
                filteredColoums.Add("Account");
            }
            if (!cblPracticeFilter.AllItemsSelected)
            {
                filteredColoums.Add("Practice");
            }
            if (!cblDirectorFilter.AllItemsSelected)
            {
                filteredColoums.Add("Executive in Charge");
            }
            if (!cblProjectManagers.AllItemsSelected)
            {
                filteredColoums.Add("Project Access");
            }
            if (!cblSalespersonFilter.AllItemsSelected)
            {
                filteredColoums.Add("Salesperson");
            }
            if (!cblSeniorManager.AllItemsSelected)
            {
                filteredColoums.Add("Engagement Manager");
            }

            if (filteredColoums.Count > 0)
            {
                for (var i = 0; i < filteredColoums.Count; i++)
                {
                    if (i == filteredColoums.Count - 1)
                        filterApplied = filterApplied + filteredColoums[i] + ".";
                    else
                        filterApplied = filterApplied + filteredColoums[i] + ",";
                }
            }

            if (report.Count > 0)
            {
                var header1 = new DataTable();
                header1.Columns.Add("Billing Report");

                var row3 = new List<object> { HostingPage.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + HostingPage.EndDate.Value.ToString(Constants.Formatting.EntryDateFormat) };
                header1.Rows.Add(row3.ToArray());

                var row4 = new List<object>();
                if (filteredColoums.Count > 0)
                {
                    row4.Add(filterApplied);
                    header1.Rows.Add(row4.ToArray());
                }
                headerRowsCount = header1.Rows.Count + 3;
                var data = PrepareDataTable(report);
                coloumnsCount = data.Columns.Count;
                sheetStylesList.Add(HeaderSheetStyle);
                sheetStylesList.Add(DataSheetStyle);
                var dataset = new DataSet { DataSetName = "BillingReport" };
                dataset.Tables.Add(header1);
                dataset.Tables.Add(data);
                dataSetList.Add(dataset);
            }
            else
            {
                const string dateRangeTitle = "There are no projects with active,projected or proposed statuses in this range selected.";
                var header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                sheetStylesList.Add(HeaderSheetStyle);
                var dataset = new DataSet { DataSetName = "BillingReport" };
                dataset.Tables.Add(header);
                dataSetList.Add(dataset);
            }

            NPOIExcel.Export(filename, dataSetList, sheetStylesList);
        }

        public void DataBindBusinesUnit(BillingReport[] reportData, bool isPopulateFilters)
        {
            var reportDataList = reportData.ToList();
            if (isPopulateFilters)
            {
                PopulateFilterPanels(reportDataList);
            }
            if (reportDataList.Count > 0)
            {
                repBillingReport.Visible = btnExportToExcel.Enabled = tdLifetoDate.Visible = true;
                divEmptyMessage.Style["display"] = "none";
                repBillingReport.DataSource = reportData;
                repBillingReport.DataBind();

                ImgAccountFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblAccountFilter.FilterPopupClientID,
                  cblAccountFilter.SelectedIndexes, cblAccountFilter.CheckBoxListObject.ClientID, cblAccountFilter.WaterMarkTextBoxBehaviorID);
                ImgPracticeFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblPracticeFilter.FilterPopupClientID,
                  cblPracticeFilter.SelectedIndexes, cblPracticeFilter.CheckBoxListObject.ClientID, cblPracticeFilter.WaterMarkTextBoxBehaviorID);
                ImgSalespersonFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblSalespersonFilter.FilterPopupClientID,
                  cblSalespersonFilter.SelectedIndexes, cblSalespersonFilter.CheckBoxListObject.ClientID, cblSalespersonFilter.WaterMarkTextBoxBehaviorID);
                ImgProjectManagerFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblProjectManagers.FilterPopupClientID,
                  cblProjectManagers.SelectedIndexes, cblProjectManagers.CheckBoxListObject.ClientID, cblProjectManagers.WaterMarkTextBoxBehaviorID);
                ImgSeniorManagerFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblSeniorManager.FilterPopupClientID,
                  cblSeniorManager.SelectedIndexes, cblSeniorManager.CheckBoxListObject.ClientID, cblSeniorManager.WaterMarkTextBoxBehaviorID);
                ImgDirectorFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblDirectorFilter.FilterPopupClientID,
                  cblDirectorFilter.SelectedIndexes, cblDirectorFilter.CheckBoxListObject.ClientID, cblDirectorFilter.WaterMarkTextBoxBehaviorID);
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repBillingReport.Visible = btnExportToExcel.Enabled = tdLifetoDate.Visible = false;
            }
        }

        private void PopulateFilterPanels(List<BillingReport> reportData)
        {
            var clients = reportData.Select(r => new { Id = r.Project.Client.Id, Name = r.Project.Client.HtmlEncodedName }).Distinct().Select(p => new Client { Id = p.Id, Name = p.Name }).ToList().OrderBy(s => s.Name);
            var practices = reportData.Select(r => new { Id = r.Project.Practice.Id, Name = r.Project.Practice.HtmlEncodedName }).Distinct().Select(p => new Practice { Id = p.Id, Name = p.Name }).ToList().OrderBy(s => s.Name);
            var salespersons = reportData.Select(r => new { Id = r.Project.SalesPersonId, Name = r.Project.SalesPersonName }).Distinct().Select(p => new Practice { Id = p.Id, Name = p.Name }).ToList().OrderBy(s => s.Name);
            var projectManagersList = new List<Person>();
            foreach (var item in reportData)
            {
                var projectManagers = item.Project.ProjectManagers.Select(r => new { Id = r.Id, FirstName = r.FirstName, LastName = r.LastName }).Distinct().Select(a => new Person { Id = a.Id, FirstName = a.FirstName, LastName = a.LastName }).ToList();
                projectManagersList.AddRange(projectManagers);
            }
            var seniorManagers = reportData.Select(r => new { Id = r.Project.SeniorManagerId, Name = r.Project.SeniorManagerName }).Distinct().Select(p => new Practice { Id = p.Id, Name = p.Name }).ToList().OrderBy(s => s.Name);
            var directors = reportData.Select(r => new { Id = r.Project.Director.Id, FirstName = r.Project.Director.FirstName, LastName = r.Project.Director.LastName }).Distinct().Select(p => new Person { Id = p.Id, FirstName = p.FirstName, LastName = p.LastName }).ToList().OrderBy(s => s.Name);
            PopulateAccountFilter(clients.Distinct().ToArray());
            PopulatePracticeFilter(practices.Distinct().ToArray());
            PopulateSalespersonFilter(salespersons.Distinct().ToArray());
            PopulateProjectManagerFilter(projectManagersList.Distinct().Where(p => p.Id != null).ToArray());
            PopulateSeniorManagerFilter(seniorManagers.Distinct().Where(p => p.Id != -1).ToArray());
            PopulateDirectorFilter(directors.Where(d=>d.Id.HasValue).Distinct().ToArray());

            cblAccountFilter.SaveSelectedIndexesInViewState();
            cblPracticeFilter.SaveSelectedIndexesInViewState();
            cblSalespersonFilter.SaveSelectedIndexesInViewState();
            cblProjectManagers.SaveSelectedIndexesInViewState();
            cblSeniorManager.SaveSelectedIndexesInViewState();
            cblDirectorFilter.SaveSelectedIndexesInViewState();
        }

        private void PopulateAccountFilter(Client[] accounts)
        {
            DataHelper.FillListDefault(cblAccountFilter.CheckBoxListObject, "All Accounts", accounts, false);
            cblAccountFilter.SelectAllItems(true);
        }

        private void PopulatePracticeFilter(Practice[] practices)
        {
            DataHelper.FillListDefault(cblPracticeFilter.CheckBoxListObject, "All Practice Areas", practices, false);
            cblPracticeFilter.SelectAllItems(true);
        }

        private void PopulateSalespersonFilter(Practice[] salesPersons)
        {
            DataHelper.FillListDefault(cblSalespersonFilter.CheckBoxListObject, "All Salespersons", salesPersons, false);
            cblSalespersonFilter.SelectAllItems(true);
        }

        private void PopulateProjectManagerFilter(Person[] managers)
        {
            DataHelper.FillListDefault(cblProjectManagers.CheckBoxListObject, "All People with Project Access", managers, false);
            cblProjectManagers.SelectAllItems(true);
        }

        private void PopulateSeniorManagerFilter(Practice[] seniorManagers)
        {
            DataHelper.FillListDefault(cblSeniorManager.CheckBoxListObject, "All Engagement Managers", seniorManagers, false);
            cblSeniorManager.SelectAllItems(true);
        }

        private void PopulateDirectorFilter(Person[] directors)
        {
            DataHelper.FillListDefault(cblDirectorFilter.CheckBoxListObject, "All Executives in Charge", directors, false);
            cblDirectorFilter.SelectAllItems(true);
        }

        public DataTable PrepareDataTable(List<BillingReport> reportData)
        {
            var data = new DataTable();
            data.Columns.Add("ProjectNumber");
            data.Columns.Add("Account");
            data.Columns.Add("Project Name");
            data.Columns.Add("Practice Area");
            data.Columns.Add(HostingPage.IsHoursUnitOfMeasure ? "Total Projected Hours" : "SOW Budget");
            data.Columns.Add("Actual to Date");
            data.Columns.Add("Remaining");
            data.Columns.Add("Range Projected");
            data.Columns.Add("Range Actual");
            data.Columns.Add("Difference");
            data.Columns.Add("Salesperson");
            data.Columns.Add("Project Access");
            data.Columns.Add("Engagement Manager");
            data.Columns.Add("Executive in Charge");
            data.Columns.Add("PONumber");
            foreach (var report in reportData)
            {
                List<object> row;
                if (HostingPage.IsHoursUnitOfMeasure)
                {
                    row = new List<object>{              
                                    report.Project.ProjectNumber,
                                    report.Project.Client.Name,
                                    report.Project.Name,
                                    report.Project.Practice.Name,
                                    GetDoubleFormat(report.ForecastedHours),
                                    GetDoubleFormat(report.ActualHours),
                                    GetDoubleFormat(report.RemainingHours),
                                    GetDoubleFormat(report.ForecastedHoursInRange),
                                    GetDoubleFormat(report.ActualHoursInRange),
                                    GetDoubleFormat(report.DifferenceInHours),
                                    report.Project.SalesPersonName,
                                    report.ProjectMangers,
                                    report.Project.SeniorManagerName,
                                    report.Project.Director.Id.HasValue?report.Project.Director.Name:string.Empty,
                                    report.Project.PONumber
                                     };
                }

                else
                {
                    row = new List<object>{     
                                        report.Project.ProjectNumber,
                                        report.Project.Client.Name,
                                        report.Project.Name,
                                        report.Project.Practice.Name,
                                        report.SOWBudget < 0 ?string.Format(NPOIExcel.CustomColorKey, "red", Convert.ToDecimal(report.SOWBudget)): string.Format(NPOIExcel.CustomColorKey, "black",Convert.ToDecimal(report.SOWBudget)),
                                        report.ActualToDate < 0 ?string.Format(NPOIExcel.CustomColorKey, "red", Convert.ToDecimal(report.ActualToDate)): string.Format(NPOIExcel.CustomColorKey, "black",Convert.ToDecimal(report.ActualToDate)),
                                        report.Remaining < 0 ?string.Format(NPOIExcel.CustomColorKey, "red", Convert.ToDecimal(report.Remaining)): string.Format(NPOIExcel.CustomColorKey, "black",Convert.ToDecimal(report.Remaining)),
                                        report.RangeProjected < 0 ?string.Format(NPOIExcel.CustomColorKey, "red", Convert.ToDecimal(report.RangeProjected)): string.Format(NPOIExcel.CustomColorKey, "black",Convert.ToDecimal(report.RangeProjected)),
                                        report.RangeActual < 0 ?string.Format(NPOIExcel.CustomColorKey, "red", Convert.ToDecimal(report.RangeActual)): string.Format(NPOIExcel.CustomColorKey, "black",Convert.ToDecimal(report.RangeActual)),
                                        report.DifferenceInCurrency < 0 ?string.Format(NPOIExcel.CustomColorKey, "red",Convert.ToDecimal(report.DifferenceInCurrency)): string.Format(NPOIExcel.CustomColorKey, "black",Convert.ToDecimal(report.DifferenceInCurrency)),
                                        report.Project.SalesPersonName,
                                        report.ProjectMangers,
                                        report.Project.SeniorManagerName,
                                        report.Project.Director.Id.HasValue?report.Project.Director.Name:string.Empty,
                                        report.Project.PONumber
                                        };

                }
                data.Rows.Add(row.ToArray());
            }
            List<object> totalRow;
            if (HostingPage.IsHoursUnitOfMeasure)
            {
                totalRow = new List<object>{              
                                    string.Format(NPOIExcel.CustomColorWithBoldKey, "black", "Total"),
                                    "",
                                    "",
                                    "",
                                    GetDoubleFormat(TotalForecastedHours),
                                    GetDoubleFormat(TotalActualHours),
                                    GetDoubleFormat(TotalRemainingHours),
                                    GetDoubleFormat(TotalForecastedHoursInRange),
                                    GetDoubleFormat(TotalActualHoursInRange),
                                    GetDoubleFormat(TotalDifferenceInHours),
                                    "",
                                    "",
                                    "",
                                    "",
                                    ""
                                     };
            }

            else
            {
                totalRow = new List<object>{     
                                        string.Format(NPOIExcel.CustomColorWithBoldKey, "black", "Total"),
                                    "",
                                    "",
                                    "",
                                        TotalSOWBudget < 0 ?string.Format(NPOIExcel.CustomColorKey, "red", Convert.ToDecimal(TotalSOWBudget)): string.Format(NPOIExcel.CustomColorKey, "black",Convert.ToDecimal(TotalSOWBudget)),
                                        TotalActualtoDate < 0 ?string.Format(NPOIExcel.CustomColorKey, "red", Convert.ToDecimal(TotalActualtoDate)): string.Format(NPOIExcel.CustomColorKey, "black",Convert.ToDecimal(TotalActualtoDate)),
                                        TotalRemaining < 0 ?string.Format(NPOIExcel.CustomColorKey, "red", Convert.ToDecimal(TotalRemaining)): string.Format(NPOIExcel.CustomColorKey, "black",Convert.ToDecimal(TotalRemaining)),
                                        TotalRangeProjected < 0 ?string.Format(NPOIExcel.CustomColorKey, "red", Convert.ToDecimal(TotalRangeProjected)): string.Format(NPOIExcel.CustomColorKey, "black",Convert.ToDecimal(TotalRangeProjected)),
                                        TotalRangeActual < 0 ?string.Format(NPOIExcel.CustomColorKey, "red", Convert.ToDecimal(TotalRangeActual)): string.Format(NPOIExcel.CustomColorKey, "black",Convert.ToDecimal(TotalRangeActual)),
                                        TotalRangeDifference < 0 ?string.Format(NPOIExcel.CustomColorKey, "red",Convert.ToDecimal(TotalRangeDifference)): string.Format(NPOIExcel.CustomColorKey, "black",Convert.ToDecimal(TotalRangeDifference)),
                                        "",
                                        "",
                                        "",
                                        "",
                                        ""
                                        };

            }
            data.Rows.Add(totalRow.ToArray());
            return data;
        }
    }
}

