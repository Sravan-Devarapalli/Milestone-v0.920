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

namespace PraticeManagement.Controls.Reports.ByAccount
{
    public partial class ByBusinessUnit : System.Web.UI.UserControl
    {
        #region Properties

        private const string ByAccountByBusinessUnitReportExport = "Account Report By Business Unit";
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;

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
                CellStyles headerCellStyle = new CellStyles();
                headerCellStyle.IsBold = true;
                headerCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
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

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            cblBusinessUnits.OKButtonId = btnFilterOK.ClientID;
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
            GroupByAccount report;
            if (isPopulateFilters)
            {
                report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByBusinessUnit(HostingPage.AccountId, BusinessUnitIds, HostingPage.ProjectStatusIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value));
            }
            else
            {
                report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByBusinessUnit(HostingPage.AccountId, BusinessUnitIds, HostingPage.ProjectStatusIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value));
            }

            DataBindBusinesUnit(report.GroupedBusinessUnits.ToArray(), isPopulateFilters);

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
            HostingPage.NonBillableHours = reportData.NonBillableHours + HostingPage.BDHours;
        }

        public void DataBindBusinesUnit(BusinessUnitLevelGroupedHours[] reportData, bool isPopulateFilters)
        {
            var reportDataList = reportData.ToList();
            if (isPopulateFilters)
            {
                PopulateFilterPanels(reportDataList);
            }
            if (reportDataList.Count > 0 || cblBusinessUnits.Items.Count > 1)
            {
                divEmptyMessage.Style["display"] = "none";
                repBusinessUnit.Visible = btnExportToExcel.Enabled = true;
                repBusinessUnit.DataSource = reportDataList;
                repBusinessUnit.DataBind();
                cblBusinessUnits.SaveSelectedIndexesInViewState();
                ImgBusinessUnitFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblBusinessUnits.FilterPopupClientID,
                  cblBusinessUnits.SelectedIndexes, cblBusinessUnits.CheckBoxListObject.ClientID, cblBusinessUnits.WaterMarkTextBoxBehaviorID);
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repBusinessUnit.Visible = btnExportToExcel.Enabled = false;
            }
        }

        private void PopulateFilterPanels(List<BusinessUnitLevelGroupedHours> reportData)
        {
            if (HostingPage.SetSelectedFilters)
            {

                var report = ServiceCallers.Custom.Report(r => r. AccountSummaryReportByBusinessUnit(HostingPage.AccountId, HostingPage.BusinessUnitIds, HostingPage.ProjectStatusIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value));

                var businessUnitList = report.GroupedBusinessUnits.Select(r => new ProjectGroup { Name = r.BusinessUnit.Name, Id = r.BusinessUnit.Id }).Distinct().ToList().OrderBy(s => s.Name).ToArray();

                PopulateBusinessUnitFilter(businessUnitList);

                foreach (ListItem item in cblBusinessUnits.Items)
                {
                    if (reportData.Any(r => r.BusinessUnit.Id.Value.ToString() == item.Value))
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
                var businessUnitList = reportData.Select(r => new ProjectGroup { Name = r.BusinessUnit.Name, Id = r.BusinessUnit.Id }).Distinct().ToList().OrderBy(s => s.Name).ToArray();
                PopulateBusinessUnitFilter(businessUnitList);
                cblBusinessUnits.SelectAllItems(true);
            }
        }

        private void PopulateBusinessUnitFilter(ProjectGroup[] businessUnits)
        {
            DataHelper.FillListDefault(cblBusinessUnits.CheckBoxListObject, "All Business Units", businessUnits, false, "Id", "HtmlEncodedName");

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
                var report = ServiceCallers.Custom.Report(r => r.AccountSummaryReportByBusinessUnit(HostingPage.AccountId, BusinessUnitIds, HostingPage.ProjectStatusIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value));
                var reportdata = report.GroupedBusinessUnits.ToList();

                string filterApplied = "Filters applied to columns: ";
                List<string> filteredColoums = new List<string>();
                if (!cblBusinessUnits.AllItemsSelected)
                {
                    filteredColoums.Add("Business Unit");
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
                    header1.Columns.Add("Account By Business Unit Report");
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

                    var data = PrepareDataTable(report, reportdata);
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

        public DataTable PrepareDataTable(GroupByAccount report, List<BusinessUnitLevelGroupedHours> reportData)
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
            foreach (var businessUnitLevelGroupedHours in reportData)
            {

                row = new List<object>();
                row.Add(report.Account.Code);
                row.Add(report.Account.HtmlEncodedName);
                row.Add(businessUnitLevelGroupedHours.BusinessUnit.Code);
                row.Add(businessUnitLevelGroupedHours.BusinessUnit.HtmlEncodedName);
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
            return data;
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }
    }
}

