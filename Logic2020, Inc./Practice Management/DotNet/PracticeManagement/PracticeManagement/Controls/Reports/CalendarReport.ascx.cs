using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Utils;
using System.Data;
using PraticeManagement.Utils.Excel;
using DataTransferObjects;

namespace PraticeManagement.Controls.Reports
{
    public partial class CalendarReport : System.Web.UI.UserControl
    {
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;

        private PraticeManagement.Reports.CalendarReport HostingPage
        {
            get { return ((PraticeManagement.Reports.CalendarReport)Page); }
        }

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

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void PopulateData()
        {
            var report = ServiceCallers.Custom.Person(p => p.GetPTOReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.IncludeCompanyHolidays)).ToList();
            repSummary.DataSource = report;
            repSummary.DataBind();
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            var filename = string.Format("TimeOffReport_{0}_{1}.xls", HostingPage.StartDate.Value.ToString(Constants.Formatting.DateFormatWithoutDelimiter), HostingPage.EndDate.Value.ToString(Constants.Formatting.DateFormatWithoutDelimiter));
            var dataSetList = new List<DataSet>();
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            var report = ServiceCallers.Custom.Person(p => p.GetPTOReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.IncludeCompanyHolidays)).ToArray();
            if (report.Length > 0)
            {
                string dateRangeTitle = string.Format("Time Off Report For the Period: {0} to {1}", HostingPage.EndDate.Value.ToString(Constants.Formatting.EntryDateFormat), HostingPage.EndDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                headerRowsCount = header.Rows.Count + 3;
                var data = PrepareDataTable(report);
                coloumnsCount = data.Columns.Count;
                sheetStylesList.Add(HeaderSheetStyle);
                sheetStylesList.Add(DataSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "TimeOff_Report";
                dataset.Tables.Add(header);
                dataset.Tables.Add(data);
                dataSetList.Add(dataset);
            }
            else
            {
                string dateRangeTitle = "There are no Time Off Entries towards this range selected.";
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                sheetStylesList.Add(HeaderSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "TimeOff_Report";
                dataset.Tables.Add(header);
                dataSetList.Add(dataset);
            }
            NPOIExcel.Export(filename, dataSetList, sheetStylesList);
        }

        private DataTable PrepareDataTable(Person[] personsList)
        {
            DataTable data = new DataTable();
            List<object> row;

            data.Columns.Add("Person ID");
            data.Columns.Add("Person Name");
            data.Columns.Add("Time Off Type");
            data.Columns.Add("Time Off Start Date");
            data.Columns.Add("Time Off End Date");
            data.Columns.Add("Project Number");
            data.Columns.Add("Project Name");
            data.Columns.Add("Project Status");
            data.Columns.Add("Account");
            data.Columns.Add("Business Group");
            data.Columns.Add("Business Unit");
            data.Columns.Add("Practice Area");
            data.Columns.Add("Project Access");
            data.Columns.Add("Engagement Manager");
            data.Columns.Add("Executive in Charge");

            foreach (var pro in personsList)
            {
                row = new List<object>();
                row.Add(pro.EmployeeNumber);
                row.Add(pro.PersonLastFirstName);
                row.Add(pro.TimeOff.TimeType.Name);
                row.Add(pro.TimeOff.TimeOffStartDate);
                row.Add(pro.TimeOff.TimeOffEndDate);
                row.Add((pro.TimeOff.Project != null && pro.TimeOff.Project.ProjectNumber != null) ? pro.TimeOff.Project.ProjectNumber : "");
                row.Add((pro.TimeOff.Project != null && pro.TimeOff.Project.Name != null) ? pro.TimeOff.Project.Name : "");
                row.Add((pro.TimeOff.Project != null && pro.TimeOff.Project.Status.Name != null) ? pro.TimeOff.Project.Status.Name : "");
                row.Add((pro.TimeOff.Project != null && pro.TimeOff.Project.Client.Id != null) ? pro.TimeOff.Project.Client.Name : "");
                row.Add((pro.TimeOff.Project != null && pro.TimeOff.Project.BusinessGroup.Id != null) ? pro.TimeOff.Project.BusinessGroup.Name : "");
                row.Add((pro.TimeOff.Project != null && pro.TimeOff.Project.Group.Id != null) ? pro.TimeOff.Project.Group.Name : "");
                row.Add((pro.TimeOff.Project != null) ? pro.TimeOff.Project.Practice.Name : "");
                row.Add((pro.TimeOff.Project != null && pro.TimeOff.Project.ProjectManagerNames != null) ? pro.TimeOff.Project.ProjectManagerNames : "");
                row.Add((pro.TimeOff.Project != null && pro.TimeOff.Project.SeniorManagerName != null) ? pro.TimeOff.Project.SeniorManagerName : "");
                row.Add((pro.TimeOff.Project != null && pro.TimeOff.Project.Director.Id != null) ? pro.TimeOff.Project.Director.Name : "");
                data.Rows.Add(row.ToArray());
            }
            return data;
        }
    }
}

