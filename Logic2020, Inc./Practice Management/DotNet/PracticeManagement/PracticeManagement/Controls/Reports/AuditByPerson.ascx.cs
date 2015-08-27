using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Text;
using DataTransferObjects.TimeEntry;
using DataTransferObjects;
using PraticeManagement.Utils.Excel;
using System.Data;
using PraticeManagement.Utils;

namespace PraticeManagement.Controls.Reports
{
    public partial class AuditByPerson : System.Web.UI.UserControl
    {
        private string AuditReportExport = "Audit Report By Person";
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

                CellStyles dataDateCellStyle = new CellStyles();
                dataDateCellStyle.DataFormat = "mm/dd/yy;@";

                CellStyles[] dataCellStylearray = { dataCellStyle,
                                                    dataCellStyle, 
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                  dataDateCellStyle,
                                                  dataDateCellStyle,
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


        private PraticeManagement.Reporting.Audit HostingPage
        {
            get { return ((PraticeManagement.Reporting.Audit)Page); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected string GetDateFormat(DateTime date)
        {
            return date.ToString(Constants.Formatting.ReportDateFormat);
        }

        public void PopulateByResourceData(PersonLevelTimeEntriesHistory[] reportDataByPerson)
        {
            var reportDataList = reportDataByPerson.OrderBy(p => p.Person.PersonLastFirstName).ToList();

            if (reportDataList.Count > 0)
            {
                divEmptyMessage.Style["display"] = "none";
                repPersons.Visible = true;
                repPersons.DataSource = reportDataList;
                repPersons.DataBind();
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repPersons.Visible = false;
            }
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(AuditReportExport);
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();
            //“Time_Entry_Audit_[StartOfRange]_[EndOfRange].xls”.  
            var filename = string.Format("{0}_{1}-{2}.xls", "Time_Entry_Audit", HostingPage.StartDate.Value.ToString("MM.dd.yyyy"), HostingPage.EndDate.Value.ToString("MM.dd.yyyy"));
            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                List<PersonLevelTimeEntriesHistory> report = ServiceCallers.Custom.Report(r => r.TimeEntryAuditReportByPerson(HostingPage.StartDate.Value, HostingPage.EndDate.Value)).ToList();
                report = report.OrderBy(p => p.Person.PersonLastFirstName).ToList();
                if (report.Count > 0)
                {
                    DataTable header1 = new DataTable();
                    header1.Columns.Add("Time Entry Audit");
                    header1.Rows.Add(report.Count + " Person(s) Affected");
                    header1.Rows.Add(HostingPage.RangeForExcel);
                    headerRowsCount = header1.Rows.Count + 3;
                    var data = PrepareDataTable(report);
                    coloumnsCount = data.Columns.Count;
                    sheetStylesList.Add(HeaderSheetStyle);
                    sheetStylesList.Add(DataSheetStyle);
                    var dataset = new DataSet();
                    dataset.DataSetName = "TimeEntryAudit_ByPerson";
                    dataset.Tables.Add(header1);
                    dataset.Tables.Add(data);
                    dataSetList.Add(dataset);
                }
                else
                {
                    string dateRangeTitle = "There are no Time Entries that were changed afterwards by any Employee for the selected range.";
                    DataTable header = new DataTable();
                    header.Columns.Add(dateRangeTitle);
                    sheetStylesList.Add(HeaderSheetStyle);
                    var dataset = new DataSet();
                    dataset.DataSetName = "TimeEntryAudit_ByPerson";
                    dataset.Tables.Add(header);
                    dataSetList.Add(dataset);
                }
                
                NPOIExcel.Export(filename, dataSetList, sheetStylesList);
            }
        }

        public DataTable PrepareDataTable(List<PersonLevelTimeEntriesHistory> report)
        {
            DataTable data = new DataTable();
            List<object> rownew;
            List<object> row;

            data.Columns.Add("Employee Id");
            data.Columns.Add("Person Name");
            data.Columns.Add("Status");
            data.Columns.Add("Pay Types");
            data.Columns.Add("Affected Date");
            data.Columns.Add("Modified Date");
            data.Columns.Add("Account Name");
            data.Columns.Add("Business Unit");
            data.Columns.Add("Project");
            data.Columns.Add("Project Name");
            data.Columns.Add("Phase");
            data.Columns.Add("Work Type");
            data.Columns.Add("B/NB");
            data.Columns.Add("Original");
            data.Columns.Add("New");
            data.Columns.Add("New Change");
            data.Columns.Add("Note");
            foreach (PersonLevelTimeEntriesHistory personLevelTimeEntriesHistory in report)
            {
                foreach (TimeEntryRecord timeEntryRecord in personLevelTimeEntriesHistory.TimeEntryRecords)
                {
                    row = new List<object>();
                    row.Add(personLevelTimeEntriesHistory.Person.EmployeeNumber);
                    row.Add(personLevelTimeEntriesHistory.Person.HtmlEncodedName);
                    row.Add(personLevelTimeEntriesHistory.Person.Status.Name);
                    row.Add(personLevelTimeEntriesHistory.Person.CurrentPay != null ? personLevelTimeEntriesHistory.Person.CurrentPay.TimescaleName : string.Empty);
                    row.Add(timeEntryRecord.ChargeCodeDate);
                    row.Add(timeEntryRecord.ModifiedDate);
                    row.Add(timeEntryRecord.ChargeCode.Client.HtmlEncodedName);
                    row.Add(timeEntryRecord.ChargeCode.ProjectGroup.HtmlEncodedName);
                    row.Add(timeEntryRecord.ChargeCode.Project.ProjectNumber);
                    row.Add(timeEntryRecord.ChargeCode.Project.HtmlEncodedName);
                    row.Add(timeEntryRecord.ChargeCode.Phase);
                    row.Add(timeEntryRecord.ChargeCode.TimeType.Name);
                    row.Add(timeEntryRecord.IsChargeable ? "B" : "NB");
                    row.Add(timeEntryRecord.OldHours);
                    row.Add(timeEntryRecord.ActualHours);
                    row.Add(timeEntryRecord.NetChange);
                    row.Add(timeEntryRecord.HtmlEncodedNoteForExport);
                    data.Rows.Add(row.ToArray());
                }
            }
            return data;
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

        protected void btnGroupBy_OnClick(object sender, EventArgs e)
        {
            HostingPage.SelectView(1);
        }

        protected bool GetNonBillableImageVisibility(int timeEntrySection, bool isChargeable)
        {
            return !isChargeable && timeEntrySection == (int)TimeEntrySectionType.Project;
        }

    }
}

