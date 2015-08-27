using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports.ByAccount;
using System.Text;
using PraticeManagement.Reporting;
using PraticeManagement.Utils.Excel;
using PraticeManagement.Utils;
using System.Data;
using DataTransferObjects;

namespace PraticeManagement.Controls.Reports.ByAccount
{
    public partial class ByBusinessDevelopment : System.Web.UI.UserControl
    {
        #region Properties

        private const string Text_GroupByBusinessUnit = "Group by Business Unit";
        private const string Text_GroupByPerson = "Group by Person";
        private const string AccountDetailByBusinessDevelopmentExport = "Account Detail Report By Business Development";
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;

        #endregion

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

                CellStyles dateformatCellStyle = new CellStyles();
                dateformatCellStyle.DataFormat = "mm/dd/yy;@";

                CellStyles[] dataCellStylearray = { dataCellStyle, 
                                                    dataCellStyle,
                                                    dateformatCellStyle,
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

        protected void Page_Load(object sender, EventArgs e)
        {
            btnExpandOrCollapseAll.Attributes["onclick"] = "return CollapseOrExpandAll(" + btnExpandOrCollapseAll.ClientID +
                                                           ", " + hdnCollapsed.ClientID +
                                                           ", " + hdncpeExtendersIds.ClientID +
                                                           ");";

            btnExpandOrCollapseAll.Text = btnExpandOrCollapseAll.ToolTip = (hdnCollapsed.Value.ToLower() == "true") ? "Expand All" : "Collapse All";
        }

        protected void btnGroupBy_Click(object sender, EventArgs e)
        {
            var btnGroupBy = sender as Button;
            if (btnGroupBy.Text == Text_GroupByPerson)
            {
                btnGroupByBU.Visible = true;
                btnGroupByPerson.Visible = false;
                mvBusinessDevelopmentReport.ActiveViewIndex = 1;
                PopulateGroupByPerson();
            }
            else
            {
                mvBusinessDevelopmentReport.ActiveViewIndex = 0;
                btnGroupByBU.Visible = false;
                btnGroupByPerson.Visible = true;
                PopulateGroupByBusinessUnit();
            }
        }

        private void PopulateGroupByBusinessUnit()
        {
            if (Page is AccountSummaryReport)
            {
                var hostingPage = Page as AccountSummaryReport;
                tpByBusinessUnit.PopulateData(hostingPage.AccountId, hostingPage.BusinessUnitIds, hostingPage.StartDate.Value, hostingPage.EndDate.Value);
            }
            else if (Page is TimePeriodSummaryReport)
            {
                var hostingPage = Page as TimePeriodSummaryReport;
                hostingPage.Total = tpByBusinessUnit.PopulateData(hostingPage.AccountId, hostingPage.BusinessUnitIds, hostingPage.StartDate.Value, hostingPage.EndDate.Value);
                hostingPage.ByProjectControl.MpeProjectDetailReport.Show();
            }
        }

        private void PopulateGroupByPerson()
        {
            if (Page is AccountSummaryReport)
            {
                var hostingPage = Page as AccountSummaryReport;
                tpByPerson.PopulateData(hostingPage.AccountId, hostingPage.BusinessUnitIds, hostingPage.StartDate.Value, hostingPage.EndDate.Value);
            }
            else if (Page is TimePeriodSummaryReport)
            {
                var hostingPage = Page as TimePeriodSummaryReport;
                hostingPage.Total = tpByPerson.PopulateData(hostingPage.AccountId, hostingPage.BusinessUnitIds, hostingPage.StartDate.Value, hostingPage.EndDate.Value);
                hostingPage.ByProjectControl.MpeProjectDetailReport.Show();
            }
        }

        public void ApplyAttributes(int count)
        {
            btnExpandOrCollapseAll.Visible =
                          btnExportToExcel.Enabled = count > 0;
            if (count == 0)
                btnGroupByBU.Visible = btnGroupByPerson.Visible = false;
        }

        public void SetExpandCollapseIdsTohiddenField(string output)
        {
            hdncpeExtendersIds.Value = output;
            btnExpandOrCollapseAll.Text = btnExpandOrCollapseAll.ToolTip = "Expand All";
            hdnCollapsed.Value = "true";
        }

        public void PopulateByBusinessDevelopment()
        {
            if (mvBusinessDevelopmentReport.ActiveViewIndex == 1)
            {
                PopulateGroupByPerson();
            }
            else
            {
                PopulateGroupByBusinessUnit();
            }
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            int accountId = 0;
            string businessUnitIds = string.Empty;
            DateTime? startDate;
            DateTime? endDate;
            string range = string.Empty;
            int businessUnitsCount = 0, projectsCount = 0, personsCount = 0;

            if (Page is TimePeriodSummaryReport)
            {
                var hostingPage = Page as TimePeriodSummaryReport;
                accountId = hostingPage.AccountId;
                businessUnitIds = hostingPage.BusinessUnitIds;
                startDate = hostingPage.StartDate;
                endDate = hostingPage.EndDate;
                range = hostingPage.RangeForExcel;
            }
            else
            {
                var hostingPage = Page as AccountSummaryReport;
                accountId = hostingPage.AccountId;
                businessUnitIds = hostingPage.BusinessUnitIds;
                startDate = hostingPage.StartDate;
                endDate = hostingPage.EndDate;
                range = hostingPage.RangeForExcel;
            }

            DataHelper.InsertExportActivityLogMessage(AccountDetailByBusinessDevelopmentExport);
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();

            List<BusinessUnitLevelGroupedHours> data = ServiceCallers.Custom.Report(r => r.AccountReportGroupByBusinessUnit(accountId, businessUnitIds, startDate.Value, endDate.Value)).ToList();

            var account = ServiceCallers.Custom.Client(c => c.GetClientDetailsShort(accountId));

            if (Page is TimePeriodSummaryReport)
            {
                businessUnitsCount = data.Select(r => r.BusinessUnit.Id.Value).Distinct().Count();
                projectsCount = data.Count > 0 ? 1 : 0;
                personsCount = data.SelectMany(g => g.PersonLevelGroupedHoursList.Select(p => p.Person.Id.Value)).Distinct().Count();
            }
            else
            {
                var hostingPage = Page as AccountSummaryReport;
                businessUnitsCount = hostingPage.BusinessUnitsCount;
                projectsCount = hostingPage.ProjectsCount;
                personsCount = hostingPage.PersonsCount;
            }
            var filename = string.Format("{0}_{1}_{2}.xls", account.Code, account.Name, "_ByBusinessDevlopment");
            filename = filename.Replace(' ', '_');
            if (data.Count > 0)
            {
                DataTable header1 = new DataTable();
                header1.Columns.Add("Account By Business Development Report");
                header1.Columns.Add(" ");
                header1.Columns.Add("  ");

                List<object> row1 = new List<object>();
                row1.Add(account.HtmlEncodedName);
                row1.Add(account.Code);
                header1.Rows.Add(row1.ToArray());

                List<object> row2 = new List<object>();
                row2.Add(businessUnitsCount + " Business Unit(s)");
                row2.Add(projectsCount + " Project(s)");
                row2.Add(personsCount.ToString() == "1" ? personsCount + " Person" : personsCount + " People");
                header1.Rows.Add(row2.ToArray());

                List<object> row3 = new List<object>();
                row3.Add(range);
                header1.Rows.Add(row3.ToArray());

                headerRowsCount = header1.Rows.Count + 3;
                var data1 = PrepareDataTable(data);
                coloumnsCount = data1.Columns.Count;
                sheetStylesList.Add(HeaderSheetStyle);
                sheetStylesList.Add(DataSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "Account_ByBusinessDevlopment";
                dataset.Tables.Add(header1);
                dataset.Tables.Add(data1);
                dataSetList.Add(dataset);
            }
            else
            {
                string dateRangeTitle = "There are no Time Entries towards this account.";
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                sheetStylesList.Add(HeaderSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "Account_ByBusinessDevlopment";
                dataset.Tables.Add(header);
                dataSetList.Add(dataset);
            }

            NPOIExcel.Export(filename, dataSetList, sheetStylesList);
        }

        public DataTable PrepareDataTable(List<BusinessUnitLevelGroupedHours> reportData)
        {
            DataTable data = new DataTable();
            List<object> rownew;
            List<object> row;

            data.Columns.Add("Employee Id");
            data.Columns.Add("Resource");
            data.Columns.Add("Date");
            data.Columns.Add("Work Type");
            data.Columns.Add("Work Type Name");
            data.Columns.Add("Business Unit");
            data.Columns.Add("Business Unit Name");
            data.Columns.Add("Non-Billable");
            data.Columns.Add("Total");
            data.Columns.Add("Note");

            foreach (var buLevelGroupedHours in reportData)
            {

                foreach (var personLevelGroupedHoursList in buLevelGroupedHours.PersonLevelGroupedHoursList)
                {
                    foreach (var groupByDate in personLevelGroupedHoursList.DayTotalHours)
                    {

                        foreach (var dateLevel in groupByDate.DayTotalHoursList)
                        {

                            row = new List<object>();
                            row.Add(personLevelGroupedHoursList.Person.EmployeeNumber);
                            row.Add(personLevelGroupedHoursList.Person.HtmlEncodedName);
                            row.Add(groupByDate.Date);
                            row.Add(dateLevel.TimeType.Code);
                            row.Add(dateLevel.TimeType.Name);
                            row.Add(buLevelGroupedHours.BusinessUnit.Code);
                            row.Add(buLevelGroupedHours.BusinessUnit.HtmlEncodedName);
                            row.Add(dateLevel.NonBillableHours);
                            row.Add(dateLevel.TotalHours);
                            row.Add(dateLevel.NoteForExport);
                            data.Rows.Add(row.ToArray());
                        }
                    }
                }
            }
            return data;
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }
    }
}

