using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Web.UI.HtmlControls;
using System.Text;
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
        private string ProjectDetailUrl = "ProjectDetail.aspx?id={0}";
        private string ByProjectUrl = "~/Reports/ProjectSummaryReport.aspx?ProjectNumber={0}&StartDate={1}&EndDate={2}&PeriodSelected={3}";
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;
        private string sortOrder_Key = "sortOrder_Key";

        public bool sortAscend
        {
            get
            {
                if (ViewState[sortOrder_Key] == null)
                {
                    ViewState[sortOrder_Key] = true;
                }
                return (bool)ViewState[sortOrder_Key];
            }
            set
            {
                ViewState[sortOrder_Key] = value;
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
                dataCellStyle.IsBold = true;
                dataCellStyle.BorderStyle = NPOI.SS.UserModel.BorderStyle.NONE;
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
                CellStyles headerCellStyle = new CellStyles();
                headerCellStyle.IsBold = true;
                headerCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
                List<CellStyles> headerCellStyleList = new List<CellStyles>();
                headerCellStyleList.Add(headerCellStyle);
                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataCellStyle = new CellStyles();

                CellStyles dataCurrancyCellStyle = new CellStyles();
                dataCurrancyCellStyle.DataFormat = "$#,##0.00_);($#,##0.00)";

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

                RowStyles datarowStyle = new RowStyles(dataCellStylearray);

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.TopRowNo = headerRowsCount;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanColSplit = 0;
                sheetStyle.FreezePanRowSplit = headerRowsCount;

                return sheetStyle;
            }
        }

        private HtmlImage ImgBusinessUnitFilter { get; set; }

        private HtmlImage ImgProjectStatusFilter { get; set; }

        public HtmlImage ImgBillingFilter { get; set; }

        private List<string> CollapsiblePanelDateExtenderClientIds
        {
            get;
            set;
        }

        private List<CollapsiblePanelExtender> CollapsiblePanelDateExtenderList
        {
            get;
            set;
        }

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
            btnExpandOrCollapseAll.Attributes["onclick"] = "return CollapseOrExpandAll(" + btnExpandOrCollapseAll.ClientID +
                                                         ", " + hdnCollapsed.ClientID +
                                                         ", " + hdncpeExtendersIds.ClientID +
                                                         ");";

            btnExpandOrCollapseAll.Text = btnExpandOrCollapseAll.ToolTip = (hdnCollapsed.Value.ToLower() == "true") ? "Expand All" : "Collapse All";
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
                var report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByProject(HostingPage.ClientdirectorId, HostingPage.AccountIds, BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value,
                     cblProjectStatus.SelectedItems, cblBilling.SelectedItemsXmlFormat)).ToList();

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

                if (report.Count > 0)
                {
                    DataTable header1 = new DataTable();
                    header1.Columns.Add("Account By Project Report");
                    header1.Columns.Add(" ");
                    header1.Columns.Add("  ");
                    header1.Columns.Add("   ");

                    List<object> row1 = new List<object>();
                    row1.Add(HostingPage.ClientdirectorName);
                    header1.Rows.Add(row1.ToArray());

                    List<object> row2 = new List<object>();
                    row2.Add(HostingPage.AccountsCount + " Account(s)");
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
                    var data = PrepareDataTable(report);
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

        public DataTable PrepareDataTable(List<GroupByAccount> reportData)
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
            foreach (var report in reportData)
            {
                foreach (var projectLevelGroupedHours in report.GroupedProjects)
                {
                    row = new List<object>();
                    row.Add(projectLevelGroupedHours.Project.Client.Code);
                    row.Add(projectLevelGroupedHours.Project.Client.Name);
                    row.Add(projectLevelGroupedHours.Project.Group.Code);
                    row.Add(projectLevelGroupedHours.Project.Group.Name);
                    row.Add(projectLevelGroupedHours.Project.ProjectNumber);
                    row.Add(projectLevelGroupedHours.Project.Name);
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
            }
            return data;
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

        public void PopulateByProjectData(bool isPopulateFilters = true)
        {
            GroupByAccount[] report;
            if (isPopulateFilters)
            {
                report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByProject(HostingPage.ClientdirectorId, HostingPage.AccountIds, BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value, null, null));
            }
            else
            {
                report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByProject(HostingPage.ClientdirectorId, HostingPage.AccountIds, BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value, cblProjectStatus.SelectedItems, cblBilling.SelectedItemsXmlFormat));
            }

            DataBindProject(report, isPopulateFilters);

            SetHeaderSectionValues(report.ToList());
        }

        private void SetHeaderSectionValues(List<GroupByAccount> reportData)
        {
            HostingPage.UpdateHeaderSection = true;

            HostingPage.AccountsCount = reportData.Count;
            HostingPage.BusinessUnitsCount = reportData.Sum(pg => pg.BusinessUnitsCount);
            HostingPage.ProjectsCount = reportData.Sum(pg => pg.ProjectsCount);
            HostingPage.PersonsCount = reportData.Count > 0 ? reportData[0].PersonsCount : 0;

            HostingPage.TotalProjectHours = reportData.Sum(pg => pg.TotalProjectHours);
            HostingPage.TotalProjectedHours = reportData.Sum(pg => pg.TotalProjectedHours);
            HostingPage.BDHours = reportData.Sum(pg => pg.BusinessDevelopmentHours);
            HostingPage.BillableHours = reportData.Sum(pg => pg.BillableHours);
            HostingPage.NonBillableHours = reportData.Sum(pg => pg.NonBillableHours);
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

        protected void repAccountDetails_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {

            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = (ProjectLevelGroupedHours)e.Item.DataItem;

                var lblActualHours = e.Item.FindControl("lblActualHours") as Label;
                var lblExclamationMark = e.Item.FindControl("lblExclamationMark") as Label;
                var lblProjectName = e.Item.FindControl("lblProjectName") as Label;
                var hlProjectName = e.Item.FindControl("hlProjectName") as HyperLink;
                var hlActualHours = e.Item.FindControl("hlActualHours") as HyperLink;
                lblExclamationMark.Visible = dataItem.BillableHoursVariance < 0;

                lblActualHours.Visible = lblProjectName.Visible = dataItem.Project.TimeEntrySectionId == 2 || dataItem.Project.TimeEntrySectionId == 3 || dataItem.Project.TimeEntrySectionId == 4;
                hlActualHours.Visible = hlProjectName.Visible = !(dataItem.Project.TimeEntrySectionId == 2 || dataItem.Project.TimeEntrySectionId == 3 || dataItem.Project.TimeEntrySectionId == 4);
            }
        }

        protected void repClientsByProject_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                CollapsiblePanelDateExtenderClientIds = new List<string>();
                CollapsiblePanelDateExtenderList = new List<CollapsiblePanelExtender>();
                ImgBusinessUnitFilter = e.Item.FindControl("imgBusinessUnitFilter") as HtmlImage;
                ImgProjectStatusFilter = e.Item.FindControl("imgProjectStatusFilter") as HtmlImage;
                ImgBillingFilter = e.Item.FindControl("imgBilling") as HtmlImage;
            }

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater repDetails = (Repeater)e.Item.FindControl("repAccountDetails");
                var dataitem = (GroupByAccount)e.Item.DataItem;
                var result = dataitem.GroupedProjects;
                repDetails.DataSource = result;
                var cpeDetails = e.Item.FindControl("cpeDetails") as CollapsiblePanelExtender;
                cpeDetails.BehaviorID = Guid.NewGuid().ToString();
                CollapsiblePanelDateExtenderClientIds.Add(cpeDetails.BehaviorID);
                CollapsiblePanelDateExtenderList.Add(cpeDetails);
                repDetails.DataBind();
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var output = jss.Serialize(CollapsiblePanelDateExtenderClientIds);
                hdncpeExtendersIds.Value = output;
            }
        }

        protected string GetProjectDetailsLink(int? projectId)
        {
            if (projectId.HasValue)
                return Utils.Generic.GetTargetUrlWithReturn(String.Format(Constants.ApplicationPages.DetailRedirectFormat, Constants.ApplicationPages.ProjectDetail, projectId.Value),
                                                            Constants.ApplicationPages.AccountSummaryReport);
            else
                return string.Empty;
        }

        protected string GetReportByProjectLink(string projectNumber)
        {
            if (projectNumber != null)
                return String.Format(ByProjectUrl, projectNumber, (HostingPage.StartDate.HasValue) ? HostingPage.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) : null,
                    (HostingPage.EndDate.HasValue) ? HostingPage.EndDate.Value.Date.ToString(Constants.Formatting.EntryDateFormat) : null, "0");
            else
                return string.Empty;
        }

        public void DataBindProject(GroupByAccount[] reportData, bool isPopulateFilters)
        {
            if (isPopulateFilters)
            {
                PopulateFilterPanels(reportData);
            }
            if (reportData.Length > 0 || cblBusinessUnits.Items.Count > 1 || cblProjectStatus.Items.Count > 1)
            {
                divEmptyMessage.Style["display"] = "none";
                if (reportData.Length == 1)
                {
                    repProject.Visible = btnExportToExcel.Enabled = true;
                    repClientsByProject.Visible = btnExpandOrCollapseAll.Visible = false;
                    repProject.DataSource = reportData[0].GroupedProjects;
                    repProject.DataBind();
                }
                else
                {
                    repClientsByProject.Visible = btnExpandOrCollapseAll.Visible = btnExportToExcel.Enabled = true;
                    repProject.Visible = false;
                    repClientsByProject.DataSource = reportData;
                    repClientsByProject.DataBind();
                }

                ImgBusinessUnitFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblBusinessUnits.FilterPopupClientID,
                cblBusinessUnits.SelectedIndexes, cblBusinessUnits.CheckBoxListObject.ClientID, cblBusinessUnits.WaterMarkTextBoxBehaviorID);
                ImgProjectStatusFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblProjectStatus.FilterPopupClientID,
                  cblProjectStatus.SelectedIndexes, cblProjectStatus.CheckBoxListObject.ClientID, cblProjectStatus.WaterMarkTextBoxBehaviorID);
                ImgBillingFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblBilling.FilterPopupClientID,
                  cblBilling.SelectedIndexes, cblBilling.CheckBoxListObject.ClientID, cblBilling.WaterMarkTextBoxBehaviorID);

                cblBusinessUnits.SaveSelectedIndexesInViewState();
                cblProjectStatus.SaveSelectedIndexesInViewState();
                cblBilling.SaveSelectedIndexesInViewState();
            }
            else
            {
                btnExpandOrCollapseAll.Visible = false;
                divEmptyMessage.Style["display"] = "";
                repProject.Visible = btnExportToExcel.Enabled = repClientsByProject.Visible = false;
            }
        }

        private void PopulateFilterPanels(GroupByAccount[] reportData)
        {
            if (HostingPage.SetSelectedFilters)
            {
                var report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByProject(HostingPage.ClientdirectorId, HostingPage.AccountIds, HostingPage.BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value, null, null));
                var businessUnitList = new List<ProjectGroup>();
                foreach (var item in reportData)
                {
                    var businessunits = item.GroupedProjects.Select(r => new { Name = r.Project.Group.Name, Id = r.Project.Group.Id, ClientId = item.Account.Id, ClientName = item.Account.Name }).Distinct().Select(a => new ProjectGroup { Id = a.Id, Name = a.Name, Client = new Client() { Id = a.ClientId, Name = a.ClientName } }).ToList();
                    businessUnitList.AddRange(businessunits);
                }

                PopulateBusinessUnitFilter(businessUnitList.ToArray());
                foreach (ListItem item in cblBusinessUnits.Items)
                {
                    
                    foreach (var reportItem in reportData)
                    {
                        if (reportData.SelectMany(b => b.GroupedProjects).Any(r => r.Project.Group.Id.Value.ToString() == item.Value))
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                    }
                }
               
                PopulateProjectStatusFilter(reportData);
                PopulateBillingFilter(reportData);
            }
            else
            {
                var businessUnits = new List<ProjectGroup>();
                foreach (var item in reportData)
                {
                    var businessunit = item.GroupedProjects.Select(r => new { Id = r.Project.Group.Id, Name = r.Project.Group.Name, ClientId = item.Account.Id, ClientName = item.Account.Name }).Distinct().ToList().OrderBy(s => s.Name).Select(pg => new ProjectGroup { Id = pg.Id, Name = pg.Name, Client = new Client() { Id = pg.ClientId, Name = pg.ClientName } }).ToList();
                    businessUnits.AddRange(businessunit);
                }

                PopulateBusinessUnitFilter(businessUnits.ToArray());
                cblBusinessUnits.SelectAllItems(true);

                PopulateProjectStatusFilter(reportData);
                PopulateBillingFilter(reportData);
            }
        }

        private void PopulateBillingFilter(GroupByAccount[] reportData)
        {
            var billingtypes = new List<ProjectGroup>();
            foreach (var item in reportData)
            {
                var report = item.GroupedProjects.Select(r => new { Name = string.IsNullOrEmpty(r.BillingType) ? "Unassigned" : r.BillingType, Code = r.BillingType }).Distinct().Select(p => new ProjectGroup { Code = p.Code, Name = p.Name }).ToList();
                foreach (var i in report)
                {
                    if (!billingtypes.Exists(p => p.Name == i.Name))
                        billingtypes.Add(i);
                }
            }
            DataHelper.FillListDefault(cblBilling.CheckBoxListObject, "All Billing Types", billingtypes.OrderBy(p => p.Name).ToArray(), false, "Code", "Name");
            cblBilling.SelectAllItems(true);
        }

        private void PopulateBusinessUnitFilter(ProjectGroup[] businessUnits)
        {
            int height = 17 * businessUnits.Length;
            Unit unitHeight = new Unit((height + 17) > 50 ? 50 : height + 17);
            DataHelper.FillListDefault(cblBusinessUnits.CheckBoxListObject, "All Business Units", businessUnits, false, "Id", "ClientProjectGroupFormat");
            cblBusinessUnits.Height = unitHeight;

        }

        private void PopulateProjectStatusFilter(GroupByAccount[] reportData)
        {
            var projectStatusIds = new List<ProjectStatus>();
            foreach (var item in reportData)
            {
                var report = item.GroupedProjects.Select(r => new { Id = r.Project.Status.Id, Name = r.Project.Status.Name }).Distinct().Select(p => new ProjectStatus { Id = p.Id, Name = p.Name }).ToList().OrderBy(s => s.Name);
                projectStatusIds.AddRange(report);
            }
            DataHelper.FillListDefault(cblProjectStatus.CheckBoxListObject, "All Status", projectStatusIds.Distinct().ToArray(), false, "Id", "Name");
            cblProjectStatus.SelectAllItems(true);
        }

        protected void btnFilterOK_OnClick(object sender, EventArgs e)
        {
            HostingPage.BusinessUnitsFilteredIds = cblBusinessUnits.SelectedItems;
            PopulateByProjectData(false);
        }

        protected void btnAccount_Command(object sender, CommandEventArgs e)
        {
            var report = PopulateData();
            if (sortAscend)
                report = report.OrderBy(b => b.Account.Name).ToList();
            else
                report = report.OrderByDescending(b => b.Account.Name).ToList();
            BindData(report);
        }

        protected void btnProjectedHours_Command(object sender, CommandEventArgs e)
        {
            var report = PopulateData();
            if (sortAscend)
            {
                report = report.OrderBy(b => b.TotalProjectedHours).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedProjects.OrderBy(p => p.ForecastedHours).ToList();
                    item.GroupedProjects = group;
                }
            }
            else
            {
                report = report.OrderByDescending(b => b.TotalProjectedHours).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedProjects.OrderByDescending(p => p.ForecastedHours).ToList();
                    item.GroupedProjects = group;
                }
            }
            BindData(report);
        }

        protected void btnBillableHours_Command(object sender, CommandEventArgs e)
        {
            var report = PopulateData();
            if (sortAscend)
            {
                report = report.OrderBy(b => b.BillableHours).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedProjects.OrderBy(p => p.BillableHours).ToList();
                    item.GroupedProjects = group;
                }
            }
            else
            {
                report = report.OrderByDescending(b => b.BillableHours).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedProjects.OrderByDescending(p => p.BillableHours).ToList();
                    item.GroupedProjects = group;
                }
            }
            BindData(report);
        }

        protected void btnNonBillableHours_Command(object sender, CommandEventArgs e)
        {
            var report = PopulateData();
            if (sortAscend)
            {
                report = report.OrderBy(b => b.NonBillableHours).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedProjects.OrderBy(p => p.NonBillableHours).ToList();
                    item.GroupedProjects = group;
                }
            }
            else
            {
                report = report.OrderByDescending(b => b.NonBillableHours).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedProjects.OrderByDescending(p => p.NonBillableHours).ToList();
                    item.GroupedProjects = group;
                }
            }
            BindData(report);
        }

        protected void btnActualHours_Command(object sender, CommandEventArgs e)
        {
            var report = PopulateData();
            if (sortAscend)
            {
                report = report.OrderBy(b => b.TotalActualHours).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedProjects.OrderBy(p => p.TotalHours).ToList();
                    item.GroupedProjects = group;
                }
            }
            else
            {
                report = report.OrderByDescending(b => b.TotalActualHours).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedProjects.OrderByDescending(p => p.TotalHours).ToList();
                    item.GroupedProjects = group;
                }
            }
            BindData(report);
        }

        protected void btnBillableHoursVariance_Command(object sender, CommandEventArgs e)
        {
            var report = PopulateData();
            if (sortAscend)
            {
                report = report.OrderBy(b => b.TotalBillableHoursVariance).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedProjects.OrderBy(p => p.BillableHoursVariance).ToList();
                    item.GroupedProjects = group;
                }
            }
            else
            {
                report = report.OrderByDescending(b => b.TotalBillableHoursVariance).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedProjects.OrderByDescending(p => p.BillableHoursVariance).ToList();
                    item.GroupedProjects = group;
                }
            }
            BindData(report);
        }

        protected void btnProjectNumber_Command(object sender, CommandEventArgs e)
        {
            var report = PopulateData();
            if (sortAscend)
            {
                report = report.OrderBy(b => b.ProjectsCount).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedProjects.OrderBy(p => p.Project.ProjectNumber).ToList();
                    item.GroupedProjects = group;
                }
            }
            else
            {
                report = report.OrderByDescending(b => b.ProjectsCount).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedProjects.OrderByDescending(p => p.Project.ProjectNumber).ToList();
                    item.GroupedProjects = group;
                }
            }
            BindData(report);
        }

        protected void btnStatus_Command(object sender, CommandEventArgs e)
        {
            var report = PopulateData();
            if (sortAscend)
            {
                foreach (var item in report)
                {
                    var group = item.GroupedProjects.OrderBy(p => p.Project.Status.Name).ToList();
                    item.SortType = 1;
                    item.GroupedProjects = group;
                }
                report = report.OrderBy(p => p, new GroupByAccount()).ToList();
            }
            else
            {
                foreach (var item in report)
                {
                    var group = item.GroupedProjects.OrderByDescending(p => p.Project.Status.Name).ToList();
                    item.SortType = 1;
                    item.GroupedProjects = group;
                }
                report = report.OrderByDescending(p => p, new GroupByAccount()).ToList();
            }
            BindData(report);
        }

        protected void btnBillingType_Command(object sender, CommandEventArgs e)
        {
            var report = PopulateData();
            if (sortAscend)
            {
                foreach (var item in report)
                {
                    var group = item.GroupedProjects.OrderBy(p => p.BillingType).ToList();
                    item.SortType = 2;
                    item.GroupedProjects = group;
                }
                report = report.OrderBy(p => p, new GroupByAccount()).ToList();
            }
            else
            {
                foreach (var item in report)
                {
                    var group = item.GroupedProjects.OrderByDescending(p => p.BillingType).ToList();
                    item.SortType = 2;
                    item.GroupedProjects = group;
                }
                report = report.OrderByDescending(p => p, new GroupByAccount()).ToList();
            }
            BindData(report);
        }

        protected void btnTotalEstBillings_Command(object sender, CommandEventArgs e)
        {
            var report = PopulateData();
            if (sortAscend)
            {
                foreach (var item in report)
                {
                    var group = item.GroupedProjects.OrderBy(p => p.EstimatedBillings).ToList();
                    item.SortType = 3;
                    item.GroupedProjects = group;
                }
                report = report.OrderBy(p => p, new GroupByAccount()).ToList();
            }
            else
            {
                foreach (var item in report)
                {
                    var group = item.GroupedProjects.OrderByDescending(p => p.EstimatedBillings).ToList();
                    item.SortType = 3;
                    item.GroupedProjects = group;
                }
                report = report.OrderByDescending(p => p, new GroupByAccount()).ToList();
            }
            BindData(report);
        }

        protected List<GroupByAccount> PopulateData()
        {
            return ServiceCallers.Custom.Report(r => r.AccountSummaryReportByProject(HostingPage.ClientdirectorId, HostingPage.AccountIds, BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value, cblProjectStatus.SelectedItems, cblBilling.SelectedItemsXmlFormat)).ToList();
        }

        public void BindData(List<GroupByAccount> report)
        {
            sortAscend = !sortAscend;
            DataBindProject(report.ToArray(), true);
            SetHeaderSectionValues(report.ToList());
            if (hdnCollapsed.Value.ToLower() != "true")
            {
                foreach (var cpe in CollapsiblePanelDateExtenderList)
                {
                    cpe.Collapsed = false;
                }
            }
        }

    }
}

