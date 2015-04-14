using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Utils.Excel;
using System.Data;
using PraticeManagement.Utils;
using PraticeManagement.Controls;

namespace PraticeManagement.Reports.Badge
{
    public partial class BadgeBlockedReport : System.Web.UI.Page
    {
        public const string StartDateKey = "StartDate";
        public const string EndDateKey = "EndDate";
        public const string PayTypesKey = "PayTypes";
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

                var dataCellStylearray = new List<CellStyles>() { dataCellStyle, dataCellStyle,dataDateCellStyle, dataDateCellStyle };

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

        public string StartDateFromQueryString
        {
            get
            {
                return Request.QueryString[StartDateKey];
            }
        }

        public string EndDateFromQueryString
        {
            get
            {
                return Request.QueryString[EndDateKey];
            }
        }
        
        public string PayTypesFromQueryString
        {
            get
            {
                return Request.QueryString[PayTypesKey];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dtpEnd.DateValue = DataTransferObjects.Utils.Generic.MonthEndDate(DateTime.Now);
                dtpStart.DateValue = DataTransferObjects.Utils.Generic.MonthStartDate(DateTime.Now);
                DataHelper.FillTimescaleList(this.cblPayTypes, Resources.Controls.AllTypes);
                cblPayTypes.SelectItems(new List<int>() { 1, 2 });
                if (!String.IsNullOrEmpty(StartDateFromQueryString))
                {
                    dtpStart.DateValue = Convert.ToDateTime(StartDateFromQueryString);
                    dtpEnd.DateValue = Convert.ToDateTime(EndDateFromQueryString);
                    cblPayTypes.SelectedItems = PayTypesFromQueryString == "null" ? null : PayTypesFromQueryString;
                    btnUpdateView_Click(btnUpdateView, new EventArgs());
                }
            }
        }

        protected void custNotMorethan2Years_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!reqBadgeStart.IsValid || !cvLastBadgeStart.IsValid || !reqbadgeEnd.IsValid || !cvbadgeEnd.IsValid)
                return;
            var totalMonths = (((dtpEnd.DateValue.Year - dtpStart.DateValue.Year) * 12) + dtpEnd.DateValue.Month - dtpStart.DateValue.Month);
            args.IsValid = totalMonths < 24;
        }

        protected void custNotBeforeJuly_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!reqBadgeStart.IsValid || !cvLastBadgeStart.IsValid || !reqbadgeEnd.IsValid || !cvbadgeEnd.IsValid)
                return;
            args.IsValid = dtpStart.DateValue >= new DateTime(2014, 7, 1);
        }

        protected void btnUpdateView_Click(object sender, EventArgs e)
        {
            Page.Validate("BadgeReport");
            if (!Page.IsValid)
            {
                divWholePage.Style.Add("display", "none");
                return;
            }
            divWholePage.Style.Remove("display");
            PopulateData();
        }

        public void PopulateData()
        {
            lblRange.Text = dtpStart.DateValue.ToString(Constants.Formatting.EntryDateFormat) + " - " + dtpEnd.DateValue.ToString(Constants.Formatting.EntryDateFormat);
            var paytypes = cblPayTypes.areAllSelected ? null : cblPayTypes.SelectedItems;
            var resources = ServiceCallers.Custom.Report(r => r.ListBadgeResourcesByType(paytypes,dtpStart.DateValue, dtpEnd.DateValue, false, false, true, false, false).ToList());
            repblocked.DataSource = resources;
            repblocked.DataBind();
            if (resources.Count > 0)
            {
                divEmptyMessage.Style.Add("display", "none");
                repblocked.Visible = tblRange.Visible = true;
            }
            else
            {
                divEmptyMessage.Style.Remove("display");
                repblocked.Visible = tblRange.Visible = false;
            }
        }

        protected void repblocked_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = (MSBadge)e.Item.DataItem;
                var lblBadgeStart = e.Item.FindControl("lblBadgeStart") as Label;
                var lblBadgeEnd = e.Item.FindControl("lblBadgeEnd") as Label;
                lblBadgeEnd.Text = dataItem.BlockEndDate.HasValue ? dataItem.BlockEndDate.Value.ToShortDateString() : string.Empty;
                lblBadgeStart.Text = dataItem.BlockStartDate.HasValue ? dataItem.BlockStartDate.Value.ToShortDateString() : string.Empty;
            }
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            var filename = string.Format("BlockedReport_{0}-{1}.xls", dtpStart.DateValue.ToString("MM_dd_yyyy"), dtpEnd.DateValue.ToString("MM_dd_yyyy"));
            var sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();
            var paytypes = cblPayTypes.areAllSelected ? null : cblPayTypes.SelectedItems;
            var report = ServiceCallers.Custom.Report(r => r.ListBadgeResourcesByType(paytypes,dtpStart.DateValue, dtpEnd.DateValue, false, false, true, false, false).ToList());
            if (report.Count > 0)
            {
                string dateRangeTitle = string.Format("Blocked report for the period: {0} to {1}", dtpStart.DateValue.ToString(Constants.Formatting.EntryDateFormat), dtpEnd.DateValue.ToString(Constants.Formatting.EntryDateFormat));
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                headerRowsCount = header.Rows.Count + 3;
                var data = PrepareDataTable(report);
                coloumnsCount = data.Columns.Count;
                sheetStylesList.Add(HeaderSheetStyle);
                sheetStylesList.Add(DataSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "Blocked";
                dataset.Tables.Add(header);
                dataset.Tables.Add(data);
                dataSetList.Add(dataset);
            }
            else
            {
                string dateRangeTitle = "There are no blocked resources for the selected dates.";
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                sheetStylesList.Add(HeaderSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "Blocked";
                dataset.Tables.Add(header);
                dataSetList.Add(dataset);
            }
            NPOIExcel.Export(filename, dataSetList, sheetStylesList);
        }

        public DataTable PrepareDataTable(List<MSBadge> report)
        {
            DataTable data = new DataTable();
            List<object> row;

            data.Columns.Add("List of Resources Blocked from MS Badge");
            data.Columns.Add("Resource Level");
            data.Columns.Add("Block Start");
            data.Columns.Add("Block End");
            foreach (var reportItem in report)
            {
                row = new List<object>();
                row.Add(reportItem.Person.Name);
                row.Add(reportItem.Person.Title.TitleName);
                row.Add(reportItem.BlockStartDate.HasValue ? reportItem.BlockStartDate.Value.ToShortDateString() : string.Empty);
                row.Add(reportItem.BlockEndDate.HasValue ? reportItem.BlockEndDate.Value.ToShortDateString() : string.Empty);
                data.Rows.Add(row.ToArray());
            }
            return data;
        }
    }
}

