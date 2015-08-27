using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using DataTransferObjects;
using PraticeManagement.Utils.Excel;
using System.Data;
using PraticeManagement.Utils;

namespace PraticeManagement.Controls.Reports
{
    public partial class NonbillableSummary : System.Web.UI.UserControl
    {

        private const string NonbillableReportSummaryExport = "Non billable Report Summary";
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;

        private PraticeManagement.Reports.NonBillableReport HostingPage
        {
            get { return ((PraticeManagement.Reports.NonBillableReport)Page); }
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

                CellStyles wrapdataCellStyle = new CellStyles();
                wrapdataCellStyle.WrapText = true;

                CellStyles dataDateCellStyle = new CellStyles();
                dataDateCellStyle.DataFormat = "mm/dd/yy;@";

                var dataCurrancyCellStyle = new CellStyles { DataFormat = "$#,##0.00_);($#,##0.00)" };

                CellStyles dataNumberDateCellStyle1 = new CellStyles();
                dataNumberDateCellStyle1.DataFormat = "#,##0.00";

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
                                                    dataNumberDateCellStyle1,
                                                    dataNumberDateCellStyle1,
                                                    dataNumberDateCellStyle1,
                                                    dataCurrancyCellStyle,
                                                    dataCurrancyCellStyle,
                                                    dataCurrancyCellStyle,
                                                    dataCurrancyCellStyle,
                                                    dataCurrancyCellStyle,
                                                    dataCurrancyCellStyle,
                                                    dataCurrancyCellStyle,
                                                    dataCurrancyCellStyle,
                                                    dataCurrancyCellStyle,
                                                    dataCurrancyCellStyle
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

        private DataTable PrepareDataTable(List<ProjectLevelGroupedHours> projectsList)
        {
            DataTable data = new DataTable();
            data.Columns.Add("Project Number");
            data.Columns.Add("Account");
            data.Columns.Add("Business Unit");
            data.Columns.Add("Project Name");
            data.Columns.Add("Practice Area");
            data.Columns.Add("Salesperson");
            data.Columns.Add("Project Access");
            data.Columns.Add("Engagement Manager");
            data.Columns.Add("Executive in Charge");
            data.Columns.Add("Resource");
            data.Columns.Add("Billable hours");
            data.Columns.Add("Non billable hours");
            data.Columns.Add("Total hours");
            data.Columns.Add("Billable Revenue");
            data.Columns.Add("Lost Revenue");
            data.Columns.Add("Potential Revenue");
            data.Columns.Add("Billable cost");
            data.Columns.Add("Non billable cost");
            data.Columns.Add("Total cost");
            data.Columns.Add("Billable Margin");
            data.Columns.Add("Actual Margin");
            data.Columns.Add("Cost/Hour");
            data.Columns.Add("Hourly(Billable) Rate");
            foreach (var pro in projectsList)
            {
                var row = new List<object>();
                var flhr = pro.PersonLevelDetails.Sum(p => p.TotalHours) == 0 ? 0 : pro.PersonLevelDetails.Sum(p => p.TotalCost) / pro.PersonLevelDetails.Sum(p => p.TotalHours);
                var billrate = pro.PersonLevelDetails.Sum(p => p.BillableHours) == 0 ? 0 : pro.PersonLevelDetails.Sum(p => p.BillableRevenue) / pro.PersonLevelDetails.Sum(p => p.BillableHours);
                row.Add(pro.Project.ProjectNumber);
                row.Add(pro.Project.Client.Name);
                row.Add(pro.Project.Group.Name);
                row.Add(pro.Project.Name);
                row.Add(pro.Project.Practice.Name);
                row.Add(pro.Project.SalesPersonName);
                row.Add(string.IsNullOrEmpty(pro.Project.ProjectManagerNames) ? string.Empty : pro.Project.ProjectManagerNames);
                row.Add(string.IsNullOrEmpty(pro.Project.SeniorManagerName) ? string.Empty : pro.Project.SeniorManagerName);
                row.Add(pro.Project.DirectorName);
                row.Add("");
                row.Add(pro.PersonLevelDetails.Sum(p => p.BillableHours));
                row.Add(pro.PersonLevelDetails.Sum(p => p.ProjectNonBillableHours));
                row.Add(pro.PersonLevelDetails.Sum(p => p.BillableHours) + pro.PersonLevelDetails.Sum(p => p.ProjectNonBillableHours));
                row.Add(string.Format(NPOIExcel.CustomColorKey, pro.PersonLevelDetails.Sum(p => p.BillableRevenue) >= 0 ? "black" : "red", pro.PersonLevelDetails.Sum(p => p.BillableRevenue)));
                row.Add(string.Format(NPOIExcel.CustomColorKey, pro.PersonLevelDetails.Sum(p => p.LostRevenue) >= 0 ? "black" : "red", pro.PersonLevelDetails.Sum(p => p.LostRevenue)));
                row.Add(string.Format(NPOIExcel.CustomColorKey, pro.PersonLevelDetails.Sum(p => p.PotentialRevenue) >= 0 ? "black" : "red", pro.PersonLevelDetails.Sum(p => p.PotentialRevenue)));
                row.Add(string.Format(NPOIExcel.CustomColorKey, pro.PersonLevelDetails.Sum(p => p.BillableCost) >= 0 ? "black" : "red", pro.PersonLevelDetails.Sum(p => p.BillableCost)));
                row.Add(string.Format(NPOIExcel.CustomColorKey, pro.PersonLevelDetails.Sum(p => p.NonBillableCost) >= 0 ? "black" : "red", pro.PersonLevelDetails.Sum(p => p.NonBillableCost)));
                row.Add(string.Format(NPOIExcel.CustomColorKey, pro.PersonLevelDetails.Sum(p => p.TotalCost) >= 0 ? "black" : "red", pro.PersonLevelDetails.Sum(p => p.TotalCost)));
                row.Add(string.Format(NPOIExcel.CustomColorKey, pro.PersonLevelDetails.Sum(p => p.BillableMargin) >= 0 ? "black" : "red", pro.PersonLevelDetails.Sum(p => p.BillableMargin)));
                row.Add(string.Format(NPOIExcel.CustomColorKey, pro.PersonLevelDetails.Sum(p => p.ActualMargin) >= 0 ? "black" : "red", pro.PersonLevelDetails.Sum(p => p.ActualMargin)));
                row.Add(string.Format(NPOIExcel.CustomColorKey, flhr >= 0 ? "black" : "red", flhr));
                row.Add(string.Format(NPOIExcel.CustomColorKey, billrate >= 0 ? "black" : "red", billrate));
                data.Rows.Add(row.ToArray());
            }
            return data;
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            var filename = string.Format("NonBillableReportSummary-{0}-{1}.xls",
                HostingPage.StartDate.Value.ToString("MM_dd_yyyy"), HostingPage.EndDate.Value.ToString("MM_dd_yyyy"));
            DataHelper.InsertExportActivityLogMessage(NonbillableReportSummaryExport);
            var sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();
            var report =
                ServiceCallers.Custom.Report(
                    r =>
                        r.NonBillableReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.SelectedProjectOption == "1" ? HostingPage.ProjectNumber : null, HostingPage.SelectedProjectOption == "3" ? HostingPage.DirectorIds : null, HostingPage.SelectedProjectOption == "4" ? HostingPage.BusinessUnitIds : null, HostingPage.SelectedProjectOption == "5" ? HostingPage.PracticeIds : null).ToList());

            if (report.Count > 0)
            {
                string dateRangeTitle = string.Format("Non billable report for the period: {0} to {1}", HostingPage.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat), HostingPage.EndDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                headerRowsCount = header.Rows.Count + 3;
                var data = PrepareDataTable(report);
                coloumnsCount = data.Columns.Count;
                sheetStylesList.Add(HeaderSheetStyle);
                sheetStylesList.Add(DataSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "Nonbillable_Summary";
                dataset.Tables.Add(header);
                dataset.Tables.Add(data);
                dataSetList.Add(dataset);
            }
            else
            {
                string dateRangeTitle = "There are no resources for the project for the report paramenters selected.";
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                sheetStylesList.Add(HeaderSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "Nonbillable_Summary";
                dataset.Tables.Add(header);
                dataSetList.Add(dataset);
            }
            NPOIExcel.Export(filename, dataSetList, sheetStylesList);
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.NumberFormatWithCommasAndDecimals);
        }

        protected string GetCurrencyFormat(double value)
        {
            var currency = new PracticeManagementCurrency();
            currency.Value = Convert.ToDecimal(value);
            return currency.ToString();
        }

        protected void hlProjectNumber_Click(object sender, EventArgs e)
        {
            var lnkbtnProject = sender as LinkButton;
            var projectNumber = lnkbtnProject.Text.Trim();
            var report = ServiceCallers.Custom.Report(r => r.NonBillableReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, projectNumber, null, null, null).ToList());
            HostingPage.AssignProjectForDetail(report.FirstOrDefault());
        }

        protected void repNonbillable_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lblProjectManagers = e.Item.FindControl("lblProjectManagers") as Label;
                var lblBillableHours = e.Item.FindControl("lblBillableHours") as Label;
                var lblNonBillableHours = e.Item.FindControl("lblNonBillableHours") as Label;
                var lblTotalHours = e.Item.FindControl("lblTotalHours") as Label;
                var lblBillableRevenue = e.Item.FindControl("lblBillableRevenue") as Label;
                var lblLostRevenue = e.Item.FindControl("lblLostRevenue") as Label;
                var lblPotentialRevenue = e.Item.FindControl("lblPotentialRevenue") as Label;
                var lblBillablecost = e.Item.FindControl("lblBillablecost") as Label;
                var lblNonbillableCost = e.Item.FindControl("lblNonbillableCost") as Label;
                var lblTotalCost = e.Item.FindControl("lblTotalCost") as Label;
                var lblBillableMargin = e.Item.FindControl("lblBillableMargin") as Label;
                var lblActualMargin = e.Item.FindControl("lblActualMargin") as Label;
                var lblFLHR = e.Item.FindControl("lblFLHR") as Label;
                var lblHourlyRate = e.Item.FindControl("lblHourlyRate") as Label;
                var lblSeniorManager = e.Item.FindControl("lblSeniorManager") as Label;

                var dataitem = (ProjectLevelGroupedHours)e.Item.DataItem;
                lblBillableHours.Text = GetDoubleFormat(dataitem.PersonLevelDetails.Sum(p => p.BillableHours));
                lblNonBillableHours.Text = GetDoubleFormat(dataitem.PersonLevelDetails.Sum(p => p.ProjectNonBillableHours));
                lblTotalHours.Text = GetDoubleFormat(dataitem.PersonLevelDetails.Sum(p => p.BillableHours) + dataitem.PersonLevelDetails.Sum(p => p.ProjectNonBillableHours));
                lblBillableRevenue.Text = GetCurrencyFormat(dataitem.PersonLevelDetails.Sum(p => p.BillableRevenue));
                lblLostRevenue.Text = GetCurrencyFormat(dataitem.PersonLevelDetails.Sum(p => p.LostRevenue));
                lblPotentialRevenue.Text = GetCurrencyFormat(dataitem.PersonLevelDetails.Sum(p => p.PotentialRevenue));
                lblBillablecost.Text = GetCurrencyFormat(dataitem.PersonLevelDetails.Sum(p => p.BillableCost));
                lblNonbillableCost.Text = GetCurrencyFormat(dataitem.PersonLevelDetails.Sum(p => p.NonBillableCost));
                lblTotalCost.Text = GetCurrencyFormat(dataitem.PersonLevelDetails.Sum(p => p.TotalCost));
                lblBillableMargin.Text = GetCurrencyFormat(dataitem.PersonLevelDetails.Sum(p => p.BillableMargin));
                lblActualMargin.Text = GetCurrencyFormat(dataitem.PersonLevelDetails.Sum(p => p.ActualMargin));
                lblFLHR.Text = GetCurrencyFormat(dataitem.PersonLevelDetails.Sum(p => p.TotalHours) == 0 ? 0 : dataitem.PersonLevelDetails.Sum(p => p.TotalCost) / dataitem.PersonLevelDetails.Sum(p => p.TotalHours));
                lblHourlyRate.Text = GetCurrencyFormat(dataitem.PersonLevelDetails.Sum(p => p.BillableHours) == 0 ? 0 : dataitem.PersonLevelDetails.Sum(p => p.BillableRevenue) / dataitem.PersonLevelDetails.Sum(p => p.BillableHours));
                lblProjectManagers.Text = string.IsNullOrEmpty(dataitem.Project.ProjectManagerNames) ? string.Empty : dataitem.Project.ProjectManagerNames;
                lblSeniorManager.Text = string.IsNullOrEmpty(dataitem.Project.SeniorManagerName) ? string.Empty : dataitem.Project.SeniorManagerName;
            }
            if (e.Item.ItemType == ListItemType.Footer)
            {

            }
        }

        public void PopulateData()
        {
            List<ProjectLevelGroupedHours> report;

            report = ServiceCallers.Custom.Report(r => r.NonBillableReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.SelectedProjectOption == "1" ? HostingPage.ProjectNumber : null, HostingPage.SelectedProjectOption == "3" ? HostingPage.DirectorIds : null, HostingPage.SelectedProjectOption == "4" ? HostingPage.BusinessUnitIds : null, HostingPage.SelectedProjectOption == "5" ? HostingPage.PracticeIds : null).ToList());
            if (report.Count > 0)
            {
                repNonbillable.Visible = btnExportToExcel.Enabled = true;
                divEmptyMessage.Style["display"] = "none";
                repNonbillable.DataSource = report;
                repNonbillable.DataBind();
                CalculateValues(report);
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repNonbillable.Visible = btnExportToExcel.Enabled = false;
            }

        }

        public void CalculateValues(List<ProjectLevelGroupedHours> report)
        {

        }
    }
}

