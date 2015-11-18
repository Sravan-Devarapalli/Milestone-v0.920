using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Utils.Excel;
using System.Data;
using PraticeManagement.Utils;
using DataTransferObjects;

namespace PraticeManagement.Reports.Badge
{
    public partial class BadgeRequestNotApprovedReport : System.Web.UI.Page
    {
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
                CellStyles[] dataCellStylearray = { dataCellStyle };
                RowStyles datarowStyle = new RowStyles(dataCellStylearray);

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };

                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.MergeRegion.Add(new int[] { 0, 0, 0, coloumnsCount > 5 ? coloumnsCount : 6 - 1 });
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

                CellStyles dataDateCellStyle = new CellStyles();
                dataDateCellStyle.DataFormat = "mm/dd/yy;@";
                dataDateCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

                List<CellStyles> headerCellStyleList = new List<CellStyles>() { headerCellStyle };

                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataCellStyle = new CellStyles();

                var dataCellStylearray = new List<CellStyles>() { dataCellStyle,dataCellStyle, dataCellStyle, dataCellStyle, dataDateCellStyle,dataCellStyle, dataDateCellStyle, dataDateCellStyle, dataDateCellStyle };

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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateData();
        }

        public void PopulateData()
        {
            var badgeList = ServiceCallers.Custom.Report(r=>r.GetBadgeRequestNotApprovedList());
            if (badgeList.Length > 0)
            {
                divEmptyMessage.Style.Add("display", "none");
                repbadgeNotApproved.DataSource = badgeList;
                repbadgeNotApproved.DataBind();
                repbadgeNotApproved.Visible = true;
            }
            else
            {
                repbadgeNotApproved.Visible = false;
                divEmptyMessage.Style.Remove("display");
            }
        }

        protected string GetDateFormat(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty;
        }

        protected string GetProjectDetailsLink(int? projectId)
        {
            return Utils.Generic.GetTargetUrlWithReturn(String.Format(Constants.ApplicationPages.DetailRedirectFormat, Constants.ApplicationPages.ProjectDetail, projectId.Value),
                                                        Constants.ApplicationPages.PersonsPage);
        }

        protected string GetMilestoneDetailsLink(int? milestoneId, int? projectId)
        {
            return Utils.Generic.GetTargetUrlWithReturn(String.Format(Constants.ApplicationPages.RedirectProjectIdFormat, Constants.ApplicationPages.MilestoneDetail, milestoneId.Value, projectId.Value),
                                                            Constants.ApplicationPages.PersonsPage);
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            var filename = "BadgeRequestNotApprovedReport.xls";
            var sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();
            var report = ServiceCallers.Custom.Report(r => r.GetBadgeRequestNotApprovedList()).ToList();
            if (report.Count > 0)
            {
                var dateRangeTitle = "Badge Requests Not Approved Report";
                var header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                headerRowsCount = header.Rows.Count + 3;
                var data = PrepareDataTable(report);
                coloumnsCount = data.Columns.Count;
                sheetStylesList.Add(HeaderSheetStyle);
                sheetStylesList.Add(DataSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "BadgeNotApproved";
                dataset.Tables.Add(header);
                dataset.Tables.Add(data);
                dataSetList.Add(dataset);
            }
            else
            {
                string dateRangeTitle = "There are no resources whose badge requestes are not approved.";
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                sheetStylesList.Add(HeaderSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "BadgeNotApproved";
                dataset.Tables.Add(header);
                dataSetList.Add(dataset);
            }
            NPOIExcel.Export(filename, dataSetList, sheetStylesList);
        }

        public DataTable PrepareDataTable(List<MSBadge> report)
        {
            DataTable data = new DataTable();
            List<object> row;

            data.Columns.Add("Current Badge Requests Not Yet Approved");
            data.Columns.Add("Resource Level");
            data.Columns.Add("Project #");
            data.Columns.Add("Milestone");
            data.Columns.Add("Project Stage");
            data.Columns.Add("Request Date");
            data.Columns.Add("Requester");
            data.Columns.Add("Badge Start");
            data.Columns.Add("Badge End");
            data.Columns.Add("18-mos Clock End Date");
            foreach (var reportItem in report)
            {
                row = new List<object>();
                row.Add(reportItem.Person.Name);
                row.Add(reportItem.Person.Title.TitleName);
                row.Add(reportItem.Project.ProjectNumber);
                row.Add(reportItem.Milestone.Description);
                row.Add(reportItem.Project.Status.StatusType.ToString());
                row.Add(reportItem.PlannedEndDate.HasValue ? reportItem.PlannedEndDate.Value.ToShortDateString() : string.Empty);
                row.Add(reportItem.Requester);
                row.Add(reportItem.BadgeStartDate.HasValue ? reportItem.BadgeStartDate.Value.ToShortDateString() : string.Empty);
                row.Add(reportItem.BadgeEndDate.HasValue ? reportItem.BadgeEndDate.Value.ToShortDateString() : string.Empty);
                row.Add(reportItem.ProjectBadgeEndDate.HasValue ? reportItem.ProjectBadgeEndDate.Value.ToShortDateString() : string.Empty);
                data.Rows.Add(row.ToArray());
            }
            return data;
        }
    }
}

