﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.Utils;
using PraticeManagement.Utils.Excel;
using System.Data;
using PraticeManagement.PersonStatusService;
using System.ServiceModel;
using System.Text;

namespace PraticeManagement.Reports.Badge
{
    public partial class AllEmployees18MoClockReport : System.Web.UI.Page
    {
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;

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
                sheetStyle.MergeRegion.Add(new int[] { 0, 0, 0, coloumnsCount > 12 ? coloumnsCount : 13 - 1 });
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

                CellStyles dataDateCellStyle = new CellStyles();
                dataDateCellStyle.DataFormat = "mm/dd/yy;@";
                dataDateCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;

                List<CellStyles> headerCellStyleList = new List<CellStyles>() { headerCellStyle };

                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataCellStyle = new CellStyles();

                var dataCellStylearray = new List<CellStyles>() { dataCellStyle,dataCellStyle, dataCellStyle, dataDateCellStyle, dataDateCellStyle, dataCellStyle, dataDateCellStyle, dataDateCellStyle };

                RowStyles datarowStyle = new RowStyles(dataCellStylearray.ToArray());
                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.TopRowNo = headerRowsCount;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanColSplit = 0;
                sheetStyle.FreezePanRowSplit = headerRowsCount;
                return sheetStyle;
            }
        }

        public string PersonStatus
        {
            get
            {
                var clientList = new StringBuilder();
                foreach (ListItem item in cblPersonStatus.Items)
                {
                    if (item.Selected)
                        clientList.Append(item.Value).Append(',');
                    if (item.Value == "1" && item.Selected)
                    {
                        clientList.Append("2").Append(',');
                        clientList.Append("5").Append(',');
                    }
                }
                return clientList.ToString();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
               DataHelper.FillTimescaleList(this.cblPayTypes, Resources.Controls.AllTypes);
                cblPayTypes.SelectItems(new List<int>() { 2 });
                FillPersonStatusList();
                cblPersonStatus.SelectItems(new List<int>() { 1, 5 });
                PopulateData(true);
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

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            var filename = "AllEmployees18MoClockReport.xls";
            var sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();
            var report = ServiceCallers.Custom.Report(r => r.GetAllBadgeDetails(cblPayTypes.SelectedItems,PersonStatus)).ToList();
            if (report.Count > 0)
            {
                string dateRangeTitle = "All Employees' 18-Month Clock Dates";
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                headerRowsCount = header.Rows.Count + 3;
                var data = PrepareDataTable(report);
                coloumnsCount = data.Columns.Count;
                sheetStylesList.Add(HeaderSheetStyle);
                sheetStylesList.Add(DataSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "AllEmployees18MoDates";
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
                dataset.DataSetName = "AllEmployees18MoDates";
                dataset.Tables.Add(header);
                dataSetList.Add(dataset);
            }
            NPOIExcel.Export(filename, dataSetList, sheetStylesList);
        }

        public DataTable PrepareDataTable(List<MSBadge> report)
        {
            DateTime now = SettingsHelper.GetCurrentPMTime();

            DataTable data = new DataTable();
            List<object> row;

            data.Columns.Add("Resource Name");
            data.Columns.Add("Pay Type");
            data.Columns.Add("Level");
            data.Columns.Add("18-Month Clock Start Date");
            data.Columns.Add("18-Month Clock End Date");
            data.Columns.Add("Time Left on Clock");
            data.Columns.Add("6-Month Break Start Date");
            data.Columns.Add("6-Month Break End Date");

            foreach (var reportItem in report)
            {
                var timeLeft = "";
                if (reportItem.BadgeStartDate.HasValue)
                {
                    if (now.Date >= reportItem.BreakStartDate.Value.Date && now.Date <= reportItem.BreakEndDate.Value.Date)
                    {
                        timeLeft = "On 6-month break";
                    }
                    else
                    {
                        timeLeft = reportItem.BadgeDuration >= 0 ? reportItem.BadgeDuration + " months" : "";
                    }
                }
                else
                {
                    timeLeft = "Clock not started yet";
                }

                row = new List<object>();
                row.Add(reportItem.Person.Name);
                row.Add(reportItem.Person.CurrentPay.TimescaleName);
                row.Add(reportItem.Person.Title.TitleName);
                row.Add(reportItem.BadgeStartDate.HasValue ? reportItem.BadgeStartDate.Value.ToShortDateString() : string.Empty);
                row.Add(reportItem.BadgeEndDate.HasValue ? reportItem.BadgeEndDate.Value.ToShortDateString() : string.Empty);
                row.Add(timeLeft);
                row.Add(reportItem.BreakStartDate.HasValue ? reportItem.BreakStartDate.Value.ToShortDateString() : string.Empty);
                row.Add(reportItem.BreakEndDate.HasValue ? reportItem.BreakEndDate.Value.ToShortDateString() : string.Empty);

                data.Rows.Add(row.ToArray());
            }
            return data;
        }

        protected void btnUpdateView_Click(object sender, EventArgs e)
        {
            PopulateData(true);
        }

        public void PopulateData(bool isFromUpdateBtn)
        {
            var badgeList = ServiceCallers.Custom.Report(r => r.GetAllBadgeDetails(isFromUpdateBtn ? cblPayTypes.SelectedItems : null,PersonStatus));
            if (badgeList.Length > 0)
            {
                divEmptyMessage.Style.Add("display", "none");
                repAllEmployeesClock.DataSource = badgeList;
                repAllEmployeesClock.DataBind();
                repAllEmployeesClock.Visible = true;
            }
            else
            {
                repAllEmployeesClock.Visible = false;
                divEmptyMessage.Style.Remove("display");
            }
        }

        protected string GetDateFormat(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty;
        }

        protected void repAllEmployeesClock_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DateTime now = SettingsHelper.GetCurrentPMTime();
                var dataItem = (MSBadge)e.Item.DataItem;
                var lblDuration = e.Item.FindControl("lblDuration") as Label;
                if (dataItem.BadgeStartDate.HasValue)
                {
                    if (now.Date >= dataItem.BreakStartDate.Value.Date && now.Date <= dataItem.BreakEndDate.Value.Date)
                    {
                        lblDuration.Text = "On 6-month break";
                        return;
                    }
                    lblDuration.Text = dataItem.BadgeDuration >= 0 ? dataItem.BadgeDuration + " months" : "";
                }
                else
                {
                    lblDuration.Text = "Clock not started yet";
                }
            }
        }
    }
}

