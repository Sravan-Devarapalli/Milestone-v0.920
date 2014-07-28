using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DataTransferObjects.Reports.ByAccount;
using System.Text;
using DataTransferObjects;
using PraticeManagement.Utils.Excel;
using System.Data;
using PraticeManagement.Utils;
using AjaxControlToolkit;
using System.Web.Script.Serialization;

namespace PraticeManagement.Controls.Reports.ByAccount
{
    public partial class ByBusinessUnit : System.Web.UI.UserControl
    {
        #region Properties

        private const string ByAccountByBusinessUnitReportExport = "Account Report By Business Unit";
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;
        private string sortOrder_Key = "sortOrder_Key";

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

        public string AccountName
        {
            get;
            set;
        }

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

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            btnExpandOrCollapseAll.Attributes["onclick"] = "return CollapseOrExpandAll(" + btnExpandOrCollapseAll.ClientID +
                                                          ", " + hdnCollapsed.ClientID +
                                                          ", " + hdncpeExtendersIds.ClientID +
                                                          ");";

            btnExpandOrCollapseAll.Text = btnExpandOrCollapseAll.ToolTip = (hdnCollapsed.Value.ToLower() == "true") ? "Expand All" : "Collapse All";
            cblBusinessUnits.OKButtonId = btnFilterOK.ClientID;
        }

        protected void repClientsByBusinessUnit_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Header)
            {
                CollapsiblePanelDateExtenderClientIds = new List<string>();
                CollapsiblePanelDateExtenderList = new List<CollapsiblePanelExtender>();
                ImgBusinessUnitFilter = e.Item.FindControl("imgBusinessUnitFilter") as HtmlImage;
            }

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater repDetails = (Repeater)e.Item.FindControl("repAccountDetails");
                var dataitem = (GroupByAccount)e.Item.DataItem;
                var result = dataitem.GroupedBusinessUnits;
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

        protected void repBusinessUnit_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                ImgBusinessUnitFilter = e.Item.FindControl("imgBusinessUnitFilter") as HtmlImage;
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = (BusinessUnitLevelGroupedHours)e.Item.DataItem;
                var lblExclamationMark = e.Item.FindControl("lblExclamationMark") as Label;
                var lblAccount = e.Item.FindControl("lblAccount") as Label;
                lblAccount.Text = AccountName;
                lblExclamationMark.Visible = dataItem.BillableHoursVariance < 0;
            }
        }

        protected void btnFilterOK_OnClick(object sender, EventArgs e)
        {
            HostingPage.BusinessUnitsFilteredIds = cblBusinessUnits.SelectedItems;
            PopulateByBusinessUnitReport(false);
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.NumberFormatWithCommasAndDecimals);
        }

        public void PopulateByBusinessUnitReport(bool isPopulateFilters = true)
        {
            List<GroupByAccount> report;
            if (isPopulateFilters)
            {
                report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByBusinessUnit(HostingPage.ClientdirectorId, HostingPage.AccountIds, BusinessUnitIds,HostingPage.ProjectStatusIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value)).ToList();
            }
            else
            {
                report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByBusinessUnit(HostingPage.ClientdirectorId, HostingPage.AccountIds, BusinessUnitIds, HostingPage.ProjectStatusIds,HostingPage.StartDate.Value, HostingPage.EndDate.Value)).ToList();
            }

            DataBindBusinesUnit(report.ToArray(), isPopulateFilters);

            SetHeaderSectionValues(report);
        }

        private void SetHeaderSectionValues(List<GroupByAccount> reportData)
        {
            HostingPage.UpdateHeaderSection = true;

            HostingPage.AccountsCount = reportData.Count;
            HostingPage.BusinessUnitsCount = reportData.Sum(pg => pg.BusinessUnitsCount);
            HostingPage.ProjectsCount = reportData.Sum(pg => pg.ProjectsCount);
            HostingPage.PersonsCount = reportData.Count > 0 ? reportData[0].PersonsCount : 0;

            HostingPage.TotalProjectHours = (reportData.Sum(pg => pg.TotalProjectHours) - reportData.Sum(pg => pg.BusinessDevelopmentHours)) > 0 ? (reportData.Sum(pg => pg.TotalProjectHours) - reportData.Sum(pg => pg.BusinessDevelopmentHours)) : 0d;
            HostingPage.TotalProjectedHours = reportData.Sum(pg => pg.TotalProjectedHours);
            HostingPage.BDHours = reportData.Sum(pg => pg.BusinessDevelopmentHours);
            HostingPage.BillableHours = reportData.Sum(pg => pg.BillableHours);
            HostingPage.NonBillableHours = reportData.Sum(pg => pg.NonBillableHours) + HostingPage.BDHours;
        }

        public void DataBindBusinesUnit(GroupByAccount[] reportData, bool isPopulateFilters)
        {
            var reportDataList = reportData.ToList();
            if (isPopulateFilters)
            {
                PopulateFilterPanels(reportDataList);
            }
            if (reportDataList.Count > 0 || cblBusinessUnits.Items.Count > 1)
            {
                divEmptyMessage.Style["display"] = "none";
                if (reportDataList.Count == 1)
                {
                    repBusinessUnit.Visible = btnExportToExcel.Enabled = true;
                    repClientsByBusinessUnit.Visible = false;
                    AccountName = reportDataList[0].Account.HtmlEncodedName;
                    repBusinessUnit.DataSource = reportDataList[0].GroupedBusinessUnits;
                    repBusinessUnit.DataBind();
                    btnExpandOrCollapseAll.Visible = false;
                }
                else
                {
                    repClientsByBusinessUnit.Visible = btnExportToExcel.Enabled = btnExpandOrCollapseAll.Visible = true;
                    repBusinessUnit.Visible = false;
                    repClientsByBusinessUnit.DataSource = reportDataList;
                    repClientsByBusinessUnit.DataBind();
                }
                cblBusinessUnits.SaveSelectedIndexesInViewState();
                ImgBusinessUnitFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblBusinessUnits.FilterPopupClientID,
                  cblBusinessUnits.SelectedIndexes, cblBusinessUnits.CheckBoxListObject.ClientID, cblBusinessUnits.WaterMarkTextBoxBehaviorID);
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repBusinessUnit.Visible = btnExpandOrCollapseAll.Visible = btnExportToExcel.Enabled = repClientsByBusinessUnit.Visible = false;
            }
        }

        private void PopulateFilterPanels(List<GroupByAccount> reportData)
        {
            if (HostingPage.SetSelectedFilters)
            {
                var report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByBusinessUnit(HostingPage.ClientdirectorId, HostingPage.AccountIds, HostingPage.BusinessUnitIds,HostingPage.ProjectStatusIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value));

                var businessUnitList = report.SelectMany(b => b.GroupedBusinessUnits.Select(r => new ProjectGroup { Name = r.BusinessUnit.Name, Id = r.BusinessUnit.Id, Client = new Client() { Id = r.BusinessUnit.Client.Id, Name = r.BusinessUnit.Client.Name } }).Distinct().ToList().OrderBy(s => s.ClientProjectGroupFormat)).ToArray();

                PopulateBusinessUnitFilter(businessUnitList);

                foreach (ListItem item in cblBusinessUnits.Items)
                {
                    if (reportData.SelectMany(b => b.GroupedBusinessUnits).Any(r => r.BusinessUnit.Id.Value.ToString() == item.Value))
                    {
                        item.Selected = true;
                    }
                    else
                    {
                        item.Selected = false;
                    }
                }
            }
            else
            {
                var businessUnitList = reportData.SelectMany(b => b.GroupedBusinessUnits.Select(r => new ProjectGroup { Name = r.BusinessUnit.Name, Id = r.BusinessUnit.Id, Client = new Client() { Id = r.BusinessUnit.Client.Id, Name = r.BusinessUnit.Client.Name } }).Distinct().ToList().OrderBy(s => s.ClientProjectGroupFormat)).ToArray();
                PopulateBusinessUnitFilter(businessUnitList);
                cblBusinessUnits.SelectAllItems(true);
            }
        }

        private void PopulateBusinessUnitFilter(ProjectGroup[] businessUnits)
        {
            DataHelper.FillListDefault(cblBusinessUnits.CheckBoxListObject, "All Business Units", businessUnits, false, "Id", "ClientProjectGroupFormat");
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            //“TimePeriod_ByProject_DateRange.xls”.  
            var filename = string.Format("Account_ByBusinessUnit_{0}-{1}.xls", HostingPage.StartDate.Value.ToString("MM_dd_yyyy"), HostingPage.EndDate.Value.ToString("MM_dd_yyyy"));
            DataHelper.InsertExportActivityLogMessage(ByAccountByBusinessUnitReportExport);
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();
            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                var report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByBusinessUnit(HostingPage.ClientdirectorId, HostingPage.AccountIds, BusinessUnitIds,HostingPage.ProjectStatusIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value)).ToList();

                string filterApplied = "Filters applied to columns: ";
                List<string> filteredColoums = new List<string>();
                if (!cblBusinessUnits.AllItemsSelected)
                {
                    filteredColoums.Add("Business Unit");
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
                    header1.Columns.Add("Account By Business Unit Report");
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
                    dataset.DataSetName = "Account_ByBusinessUnit";
                    dataset.Tables.Add(header1);
                    dataset.Tables.Add(data);
                    dataSetList.Add(dataset);
                }
                else
                {
                    string dateRangeTitle = "There are no projects with Active or Completed statuses for the report parameters selected.";
                    DataTable header = new DataTable();
                    header.Columns.Add(dateRangeTitle);
                    sheetStylesList.Add(HeaderSheetStyle);
                    var dataset = new DataSet();
                    dataset.DataSetName = "Account_ByBusinessUnit";
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
            data.Columns.Add("# of Active Projects");
            data.Columns.Add("# of Completed Projects");
            data.Columns.Add("Projected Hours");
            data.Columns.Add("Billable");
            data.Columns.Add("Non-Billable");
            data.Columns.Add("Actual Hours");
            data.Columns.Add("BD");
            data.Columns.Add("Total BU Hours");
            data.Columns.Add("Billable Hours Variance");
            foreach (var account in reportData)
            {
                foreach (var businessUnitLevelGroupedHours in account.GroupedBusinessUnits)
                {

                    row = new List<object>();
                    row.Add(account.Account.Code);
                    row.Add(account.Account.Name);
                    row.Add(businessUnitLevelGroupedHours.BusinessUnit.Code);
                    row.Add(businessUnitLevelGroupedHours.BusinessUnit.Name);
                    row.Add(businessUnitLevelGroupedHours.ActiveProjectsCount);
                    row.Add(businessUnitLevelGroupedHours.CompletedProjectsCount);
                    row.Add(GetDoubleFormat(businessUnitLevelGroupedHours.ForecastedHours));
                    row.Add(GetDoubleFormat(businessUnitLevelGroupedHours.BillableHours));
                    row.Add(GetDoubleFormat(businessUnitLevelGroupedHours.NonBillableHours));
                    row.Add(GetDoubleFormat(businessUnitLevelGroupedHours.ActualHours));
                    row.Add(GetDoubleFormat(businessUnitLevelGroupedHours.BusinessDevelopmentHours));
                    row.Add(GetDoubleFormat(businessUnitLevelGroupedHours.TotalHours));
                    row.Add(GetDoubleFormat(businessUnitLevelGroupedHours.BillableHoursVariance));
                    data.Rows.Add(row.ToArray());
                }
            }
            return data;
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

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

        protected void btnBusinessUnit_Command(object sender, CommandEventArgs e)
        {
            var report = PopulateData();
            if (sortAscend)
            {
                report = report.OrderBy(b => b.BusinessUnitsCount).ThenBy(p => p.GroupedBusinessUnits[0].BusinessUnit.Name).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedBusinessUnits.OrderBy(p => p.BusinessUnit.Name).ToList();
                    item.GroupedBusinessUnits = group;
                }
            }
            else
            {
                report = report.OrderByDescending(b => b.BusinessUnitsCount).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedBusinessUnits.OrderByDescending(p => p.BusinessUnit.Name).ToList();
                    item.GroupedBusinessUnits = group;
                }
            }
            BindData(report);
        }

        protected void btnActiveProjects_Command(object sender, CommandEventArgs e)
        {
            var report = PopulateData();
            if (sortAscend)
            {
                report = report.OrderBy(b => b.ActiveProjectsCount).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedBusinessUnits.OrderBy(p => p.ActiveProjectsCount).ToList();
                    item.GroupedBusinessUnits = group;
                }
            }
            else
            {
                report = report.OrderByDescending(b => b.ActiveProjectsCount).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedBusinessUnits.OrderByDescending(p => p.ActiveProjectsCount).ToList();
                    item.GroupedBusinessUnits = group;
                }
            }
            BindData(report);
        }

        protected void btnCompletedProjects_Command(object sender, CommandEventArgs e)
        {
            var report = PopulateData();
            if (sortAscend)
            {
                report = report.OrderBy(b => b.CompletedProjectsCount).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedBusinessUnits.OrderBy(p => p.CompletedProjectsCount).ToList();
                    item.GroupedBusinessUnits = group;
                }
            }
            else
            {
                report = report.OrderByDescending(b => b.CompletedProjectsCount).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedBusinessUnits.OrderByDescending(p => p.CompletedProjectsCount).ToList();
                    item.GroupedBusinessUnits = group;
                }
            }
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
                    var group = item.GroupedBusinessUnits.OrderBy(p => p.ForecastedHours).ToList();
                    item.GroupedBusinessUnits = group;
                }
            }
            else
            {
                report = report.OrderByDescending(b => b.TotalProjectedHours).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedBusinessUnits.OrderByDescending(p => p.ForecastedHours).ToList();
                    item.GroupedBusinessUnits = group;
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
                    var group = item.GroupedBusinessUnits.OrderBy(p => p.BillableHours).ToList();
                    item.GroupedBusinessUnits = group;
                }
            }
            else
            {
                report = report.OrderByDescending(b => b.BillableHours).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedBusinessUnits.OrderByDescending(p => p.BillableHours).ToList();
                    item.GroupedBusinessUnits = group;
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
                    var group = item.GroupedBusinessUnits.OrderBy(p => p.NonBillableHours).ToList();
                    item.GroupedBusinessUnits = group;
                }
            }
            else
            {
                report = report.OrderByDescending(b => b.NonBillableHours).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedBusinessUnits.OrderByDescending(p => p.NonBillableHours).ToList();
                    item.GroupedBusinessUnits = group;
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
                    var group = item.GroupedBusinessUnits.OrderBy(p => p.ActualHours).ToList();
                    item.GroupedBusinessUnits = group;
                }
            }
            else
            {
                report = report.OrderByDescending(b => b.TotalActualHours).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedBusinessUnits.OrderByDescending(p => p.ActualHours).ToList();
                    item.GroupedBusinessUnits = group;
                }
            }
            BindData(report);
        }

        protected void btnBDHours_Command(object sender, CommandEventArgs e)
        {
            var report = PopulateData();
            if (sortAscend)
            {
                report = report.OrderBy(b => b.BusinessDevelopmentHours).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedBusinessUnits.OrderBy(p => p.BusinessDevelopmentHours).ToList();
                    item.GroupedBusinessUnits = group;
                }
            }
            else
            {
                report = report.OrderByDescending(b => b.BusinessDevelopmentHours).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedBusinessUnits.OrderByDescending(p => p.BusinessDevelopmentHours).ToList();
                    item.GroupedBusinessUnits = group;
                }
            }
            BindData(report);
        }

        protected void btnTotalBUHours_Command(object sender, CommandEventArgs e)
        {
            var report = PopulateData();
            if (sortAscend)
            {
                report = report.OrderBy(b => b.TotalProjectHours).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedBusinessUnits.OrderBy(p => p.TotalHours).ToList();
                    item.GroupedBusinessUnits = group;
                }
            }
            else
            {
                report = report.OrderByDescending(b => b.TotalProjectHours).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedBusinessUnits.OrderByDescending(p => p.TotalHours).ToList();
                    item.GroupedBusinessUnits = group;
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
                    var group = item.GroupedBusinessUnits.OrderBy(p => p.BillableHoursVariance).ToList();
                    item.GroupedBusinessUnits = group;
                }
            }
            else
            {
                report = report.OrderByDescending(b => b.TotalBillableHoursVariance).ToList();
                foreach (var item in report)
                {
                    var group = item.GroupedBusinessUnits.OrderByDescending(p => p.BillableHoursVariance).ToList();
                    item.GroupedBusinessUnits = group;
                }
            }
            BindData(report);
        }

        protected List<GroupByAccount> PopulateData()
        {
            return ServiceCallers.Custom.Report(r => r.AccountSummaryReportByBusinessUnit(HostingPage.ClientdirectorId, HostingPage.AccountIds, BusinessUnitIds,HostingPage.ProjectStatusIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value)).ToList();
        }

        public void BindData(List<GroupByAccount> report)
        {
            sortAscend = !sortAscend;
            DataBindBusinesUnit(report.ToArray(), true);
            SetHeaderSectionValues(report);
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

