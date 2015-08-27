using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Web.UI.HtmlControls;
using DataTransferObjects;
using DataTransferObjects.Reports.ByAccount;
using PraticeManagement.Utils.Excel;
using PraticeManagement.Utils;
using System.Data;
using System.Web.Script.Serialization;
using AjaxControlToolkit;

namespace PraticeManagement.Controls.Reports.ByAccount
{
    public partial class ByProject : System.Web.UI.UserControl
    {
        private const string ByAccountByProjectReportExport = "Account Report By Project";

        private const string ByProjectUrl = "~/Reports/ProjectSummaryReport.aspx?ProjectNumber={0}&StartDate={1}&EndDate={2}&PeriodSelected={3}";
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;

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
                                                    dataCellStyle, 
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle, 
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCurrancyCellStyle,
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

        private HtmlImage ImgBusinessUnitFilter { get; set; }

        private HtmlImage ImgProjectStatusFilter { get; set; }

        public HtmlImage ImgBillingFilter { get; set; }

        private PraticeManagement.Reporting.AccountSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.AccountSummaryReport)Page); }
        }

        private String BusinessUnitIds
        {
            get
            {
                if (HostingPage.BusinessUnitsFilteredIds == null)
                {
                    return HostingPage.BusinessUnitIds;
                }

                if (HostingPage.BusinessUnitsFilteredIds != null)
                {
                    return HostingPage.BusinessUnitsFilteredIds;
                }
                HostingPage.BusinessUnitsFilteredIds = cblBusinessUnits.SelectedItems;
                return cblBusinessUnits.SelectedItems;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            cblBilling.OKButtonId = cblBusinessUnits.OKButtonId = cblProjectStatus.OKButtonId = btnFilterOK.ClientID;
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.NumberFormatWithCommasAndDecimals);
        }

        protected string GetCurrencyDecimalFormat(double value)
        {
            return value.ToString(Constants.Formatting.CurrencyExcelReportFormat);
        }

        protected string GetCurrencyFormat(double value)
        {
            return value > 0 ? value.ToString(Constants.Formatting.CurrencyFormat) : "$0";
        }

        protected string GetVarianceSortValue(string variance)
        {
            if (variance.Equals("N/A"))
            {
                return int.MinValue.ToString();
            }
            return variance;
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            //“TimePeriod_ByProject_DateRange.xls”.  
            var filename = string.Format("Account_ByProject_{0}-{1}.xls", HostingPage.StartDate.Value.ToString("MM_dd_yyyy"), HostingPage.EndDate.Value.ToString("MM_dd_yyyy"));
            DataHelper.InsertExportActivityLogMessage(ByAccountByProjectReportExport);
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();
            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                var report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByProject(HostingPage.AccountId, BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value,
                     cblProjectStatus.SelectedItems, cblBilling.SelectedItemsXmlFormat));

                var reportdata = report.GroupedProjects.ToList();

                string filterApplied = "Filters applied to columns: ";
                List<string> filteredColoums = new List<string>();
                if (!cblBusinessUnits.AllItemsSelected)
                {
                    filteredColoums.Add("Project");
                }
                if (!cblProjectStatus.AllItemsSelected)
                {
                    filteredColoums.Add("Status");
                }
                if (!cblBilling.AllItemsSelected)
                {
                    filteredColoums.Add("Billing");
                }

                var account = ServiceCallers.Custom.Client(c => c.GetClientDetailsShort(HostingPage.AccountId));

                if (filteredColoums.Count > 0)
                {

                    for (int i = 0; i < filteredColoums.Count; i++)
                    {
                        if (i == filteredColoums.Count - 1)
                            filterApplied = filterApplied + filteredColoums[i] + ".";
                        else
                            filterApplied = filterApplied + filteredColoums[i] + ",";
                    }

                }


                if (reportdata.Count > 0)
                {
                    DataTable header1 = new DataTable();
                    header1.Columns.Add("Account By Project Report");
                    header1.Columns.Add(" ");
                    header1.Columns.Add("  ");

                    List<object> row1 = new List<object>();
                    row1.Add(account.HtmlEncodedName);
                    row1.Add(account.Code);
                    header1.Rows.Add(row1.ToArray());

                    List<object> row2 = new List<object>();
                    row2.Add(HostingPage.BusinessUnitsCount + " Business Unit(s)");
                    row2.Add(HostingPage.ProjectsCount + " Project(s)");
                    row2.Add(HostingPage.PersonsCount.ToString() == "1" ? HostingPage.PersonsCount + " Person" : HostingPage.PersonsCount + " People");
                    header1.Rows.Add(row2.ToArray());

                    List<object> row3 = new List<object>();
                    row3.Add(HostingPage.RangeForExcel);
                    header1.Rows.Add(row3.ToArray());

                    List<object> row4 = new List<object>();
                    if (filteredColoums.Count > 0)
                    {
                        row4.Add(filterApplied);
                        header1.Rows.Add(row4.ToArray());
                    }
                    headerRowsCount = header1.Rows.Count + 3;
                    var data = PrepareDataTable(reportdata);
                    coloumnsCount = data.Columns.Count;
                    sheetStylesList.Add(HeaderSheetStyle);
                    sheetStylesList.Add(DataSheetStyle);
                    var dataset = new DataSet();
                    dataset.DataSetName = "Account_ByProject";
                    dataset.Tables.Add(header1);
                    dataset.Tables.Add(data);
                    dataSetList.Add(dataset);
                }
                else
                {
                    string dateRangeTitle = "There are no Time Entries towards this range selected.";
                    DataTable header = new DataTable();
                    header.Columns.Add(dateRangeTitle);
                    sheetStylesList.Add(HeaderSheetStyle);
                    var dataset = new DataSet();
                    dataset.DataSetName = "Account_ByProject";
                    dataset.Tables.Add(header);
                    dataSetList.Add(dataset);
                }

                NPOIExcel.Export(filename, dataSetList, sheetStylesList);
            }
        }

        public DataTable PrepareDataTable(List<ProjectLevelGroupedHours> reportData)
        {
            DataTable data = new DataTable();
            List<object> rownew;
            List<object> row;

            data.Columns.Add("Account");
            data.Columns.Add("Account Name");
            data.Columns.Add("Business Unit");
            data.Columns.Add("Business Unit Name");
            data.Columns.Add("Project");
            data.Columns.Add("Project Name");
            data.Columns.Add("Status");
            data.Columns.Add("Billing Type");
            data.Columns.Add("Projected Hours");
            data.Columns.Add("Billable");
            data.Columns.Add("Non-Billable");
            data.Columns.Add("Actual Hours");
            data.Columns.Add("Total Estimated Billings");
            data.Columns.Add("Billable Hours Variance");
            foreach (var projectLevelGroupedHours in reportData)
            {

                row = new List<object>();
                row.Add(projectLevelGroupedHours.Project.Client.Code);
                row.Add(projectLevelGroupedHours.Project.Client.HtmlEncodedName);
                row.Add(projectLevelGroupedHours.Project.Group.Code);
                row.Add(projectLevelGroupedHours.Project.Group.HtmlEncodedName);
                row.Add(projectLevelGroupedHours.Project.ProjectNumber);
                row.Add(projectLevelGroupedHours.Project.HtmlEncodedName);
                row.Add(projectLevelGroupedHours.Project.Status.Name);
                row.Add(projectLevelGroupedHours.BillingType);
                row.Add(GetDoubleFormat(projectLevelGroupedHours.ForecastedHours));
                row.Add(GetDoubleFormat(projectLevelGroupedHours.BillableHours));
                row.Add(GetDoubleFormat(projectLevelGroupedHours.NonBillableHours));
                row.Add(GetDoubleFormat(projectLevelGroupedHours.TotalHours));
                row.Add(projectLevelGroupedHours.BillingType == "Fixed" ? "FF" : projectLevelGroupedHours.EstimatedBillings.ToString());
                row.Add(GetDoubleFormat(projectLevelGroupedHours.BillableHoursVariance));
                data.Rows.Add(row.ToArray());
            }
            return data;
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

        public void PopulateByProjectData(bool isPopulateFilters = true)
        {
            GroupByAccount report;
            if (isPopulateFilters)
            {
                report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByProject(HostingPage.AccountId, BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.ProjectStatusIds, null));
            }
            else
            {
                report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByProject(HostingPage.AccountId, BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value, cblProjectStatus.SelectedItems, cblBilling.SelectedItemsXmlFormat));
            }

            DataBindProject(report.GroupedProjects.ToArray(), isPopulateFilters);

            SetHeaderSectionValues(report);
        }

        private void SetHeaderSectionValues(GroupByAccount reportData)
        {
            HostingPage.UpdateHeaderSection = true;

            HostingPage.BusinessUnitsCount = reportData.BusinessUnitsCount;
            HostingPage.ProjectsCount = reportData.ProjectsCount;
            HostingPage.PersonsCount = reportData.PersonsCount;

            HostingPage.TotalProjectHours = (reportData.TotalProjectHours - reportData.BusinessDevelopmentHours) > 0 ? (reportData.TotalProjectHours - reportData.BusinessDevelopmentHours) : 0d;
            HostingPage.TotalProjectedHours = reportData.TotalProjectedHours;
            HostingPage.BDHours = reportData.BusinessDevelopmentHours;
            HostingPage.BillableHours = reportData.BillableHours;
            HostingPage.NonBillableHours = reportData.NonBillableHours;
        }

        protected void repProject_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                ImgBusinessUnitFilter = e.Item.FindControl("imgBusinessUnitFilter") as HtmlImage;
                ImgProjectStatusFilter = e.Item.FindControl("imgProjectStatusFilter") as HtmlImage;
                ImgBillingFilter = e.Item.FindControl("imgBilling") as HtmlImage;
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = (ProjectLevelGroupedHours)e.Item.DataItem;
                var lblEstimatedBillings = e.Item.FindControl("lblEstimatedBillings") as Label;
                var lblActualHours = e.Item.FindControl("lblActualHours") as Label;
                var lblExclamationMark = e.Item.FindControl("lblExclamationMark") as Label;
                var lblProjectName = e.Item.FindControl("lblProjectName") as Label;
                var hlProjectName = e.Item.FindControl("hlProjectName") as HyperLink;
                var hlActualHours = e.Item.FindControl("hlActualHours") as HyperLink;
                lblExclamationMark.Visible = dataItem.BillableHoursVariance < 0;
                lblEstimatedBillings.Text = dataItem.EstimatedBillings == -1 ? "FF" : GetCurrencyDecimalFormat(dataItem.EstimatedBillings).ToString();
                lblActualHours.Visible = lblProjectName.Visible = dataItem.Project.TimeEntrySectionId == 2 || dataItem.Project.TimeEntrySectionId == 3 || dataItem.Project.TimeEntrySectionId == 4;
                hlActualHours.Visible = hlProjectName.Visible = !(dataItem.Project.TimeEntrySectionId == 2 || dataItem.Project.TimeEntrySectionId == 3 || dataItem.Project.TimeEntrySectionId == 4);
            }
        }

        protected string GetProjectDetailsLink(int? projectId)
        {
            if (projectId.HasValue)
                return Utils.Generic.GetTargetUrlWithReturn(String.Format(Constants.ApplicationPages.DetailRedirectFormat, Constants.ApplicationPages.ProjectDetail, projectId.Value),
                                                            Constants.ApplicationPages.AccountSummaryReport);
            return string.Empty;
        }

        protected string GetReportByProjectLink(string projectNumber)
        {
            if (projectNumber != null)
                return String.Format(ByProjectUrl, projectNumber, (HostingPage.StartDate.HasValue) ? HostingPage.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) : null,
                    (HostingPage.EndDate.HasValue) ? HostingPage.EndDate.Value.Date.ToString(Constants.Formatting.EntryDateFormat) : null, "0");
            return string.Empty;
        }

        public void DataBindProject(ProjectLevelGroupedHours[] reportData, bool isPopulateFilters)
        {
            if (isPopulateFilters)
            {
                PopulateFilterPanels(reportData);
            }
            if (reportData.Length > 0 || cblBusinessUnits.Items.Count > 1 || cblProjectStatus.Items.Count > 1)
            {
                divEmptyMessage.Style["display"] = "none";
                repProject.Visible = btnExportToExcel.Enabled = true;
                repProject.DataSource = reportData;
                repProject.DataBind();

                cblBusinessUnits.SaveSelectedIndexesInViewState();
                cblProjectStatus.SaveSelectedIndexesInViewState();
                cblBilling.SaveSelectedIndexesInViewState();

                ImgBusinessUnitFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblBusinessUnits.FilterPopupClientID,
                  cblBusinessUnits.SelectedIndexes, cblBusinessUnits.CheckBoxListObject.ClientID, cblBusinessUnits.WaterMarkTextBoxBehaviorID);
                ImgProjectStatusFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblProjectStatus.FilterPopupClientID,
                  cblProjectStatus.SelectedIndexes, cblProjectStatus.CheckBoxListObject.ClientID, cblProjectStatus.WaterMarkTextBoxBehaviorID);
                ImgBillingFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblBilling.FilterPopupClientID,
                  cblBilling.SelectedIndexes, cblBilling.CheckBoxListObject.ClientID, cblBilling.WaterMarkTextBoxBehaviorID);
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repProject.Visible = btnExportToExcel.Enabled = false;
            }
        }

        private void PopulateFilterPanels(ProjectLevelGroupedHours[] reportData)
        {
            if (HostingPage.SetSelectedFilters)
            {
                var report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByProject(HostingPage.AccountId, HostingPage.BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.ProjectStatusIds, null));
                var businessUnitList = report.GroupedProjects.Select(r => new { Name = r.Project.Group.Name, Id = r.Project.Group.Id }).Distinct().Select(a => new ProjectGroup { Id = a.Id, Name = a.Name }).ToList().OrderBy(s => s.Name).ToArray();

                PopulateBusinessUnitFilter(businessUnitList);

                foreach (ListItem item in cblBusinessUnits.Items)
                {
                    if (reportData.Any(r => r.Project.Group.Id.Value.ToString() == item.Value))
                    {
                        item.Selected = true;
                    }
                    else
                    {
                        item.Selected = false;
                    }
                }

                var data = report.GroupedProjects.ToArray();
                PopulateProjectStatusFilter(data);
                PopulateBillingFilter(data);

            }
            else
            {
                ProjectGroup[] businessUnits = reportData.Select(r => new { Id = r.Project.Group.Id, Name = r.Project.Group.Name }).Distinct().ToList().OrderBy(s => s.Name).Select(pg => new ProjectGroup { Id = pg.Id, Name = pg.Name }).ToArray();
                PopulateBusinessUnitFilter(businessUnits);
                cblBusinessUnits.SelectAllItems(true);

                PopulateProjectStatusFilter(reportData);
                PopulateBillingFilter(reportData);
            }


        }

        private void PopulateBillingFilter(ProjectLevelGroupedHours[] reportData)
        {
            var billingtypes = reportData.Select(r => new { Text = string.IsNullOrEmpty(r.BillingType) ? "Unassigned" : r.BillingType, Value = r.BillingType }).Distinct().ToList().OrderBy(s => s.Value);
            DataHelper.FillListDefault(cblBilling.CheckBoxListObject, "All Billing Types", billingtypes.ToArray(), false, "Value", "Text");
            cblBilling.SelectAllItems(true);
        }

        private void PopulateBusinessUnitFilter(ProjectGroup[] businessUnits)
        {
            int height = 17 * businessUnits.Length;
            Unit unitHeight = new Unit((height + 17) > 50 ? 50 : height + 17);
            DataHelper.FillListDefault(cblBusinessUnits.CheckBoxListObject, "All Business Units", businessUnits, false, "Id", "HtmlEncodedName");
            cblBusinessUnits.Height = unitHeight;

        }

        private void PopulateProjectStatusFilter(ProjectLevelGroupedHours[] reportData)
        {
            var projectStatusIds = reportData.Select(r => new { Id = r.Project.Status.Id, Name = r.Project.Status.Name }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblProjectStatus.CheckBoxListObject, "All Status", projectStatusIds.ToArray(), false, "Id", "Name");
            cblProjectStatus.SelectAllItems(true);
        }

        protected void btnFilterOK_OnClick(object sender, EventArgs e)
        {
            HostingPage.BusinessUnitsFilteredIds = cblBusinessUnits.SelectedItems;
            PopulateByProjectData(false);
        }
    }
}

