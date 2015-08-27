using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using DataTransferObjects;
using PraticeManagement.Utils.Excel;
using PraticeManagement.Utils;
using System.Data;

namespace PraticeManagement.Controls.Reports
{
    public partial class NonbillableDetail : System.Web.UI.UserControl
    {
        private const string NonbillableReportDetailExport = "Non billable Report Detail";
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;
        public int count = 0;

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

        public ProjectLevelGroupedHours CurrentProject
        {
            get;
            set;
        }

        public string ProjectNumber
        {
            get
            {
                if (ViewState["ProjectNumberKey"] == null)
                {
                    ViewState["ProjectNumberKey"] = string.Empty;
                }
                return ViewState["ProjectNumberKey"] as string;
            }
            set
            {
                ViewState["ProjectNumberKey"] = value;
            }
        }

        private PraticeManagement.Reports.NonBillableReport HostingPage
        {
            get { return ((PraticeManagement.Reports.NonBillableReport)Page); }
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
            for (int i = 0; i < projectsList[0].PersonLevelDetails.Count;i++)
            {
                var row = new List<object>();
                if (i == 0)
                {
                    row.Add(projectsList[0].Project.ProjectNumber);
                    row.Add(projectsList[0].Project.Client.Name);
                    row.Add(projectsList[0].Project.Group.Name);
                    row.Add(projectsList[0].Project.Name);
                    row.Add(projectsList[0].Project.Practice.Name);
                    row.Add(projectsList[0].Project.SalesPersonName);
                    row.Add(string.IsNullOrEmpty(projectsList[0].Project.ProjectManagerNames) ? string.Empty : projectsList[0].Project.ProjectManagerNames);
                    row.Add(string.IsNullOrEmpty(projectsList[0].Project.SeniorManagerName) ? string.Empty : projectsList[0].Project.SeniorManagerName);
                    row.Add(projectsList[0].Project.DirectorName);
                }
                else
                {
                    for (int j = 0; j < 9; j++)
                        row.Add("");
                }
                row.Add(projectsList[0].PersonLevelDetails[i].Person.Name);
                row.Add(projectsList[0].PersonLevelDetails[i].BillableHours);
                row.Add(projectsList[0].PersonLevelDetails[i].ProjectNonBillableHours);
                row.Add(projectsList[0].PersonLevelDetails[i].BillableHours + projectsList[0].PersonLevelDetails[i].ProjectNonBillableHours);
                row.Add(string.Format(NPOIExcel.CustomColorKey, projectsList[0].PersonLevelDetails[i].BillableRevenue >= 0 ? "black" : "red", projectsList[0].PersonLevelDetails[i].BillableRevenue));
                row.Add(string.Format(NPOIExcel.CustomColorKey, projectsList[0].PersonLevelDetails[i].LostRevenue >= 0 ? "black" : "red", projectsList[0].PersonLevelDetails[i].LostRevenue));
                row.Add(string.Format(NPOIExcel.CustomColorKey, projectsList[0].PersonLevelDetails[i].PotentialRevenue >= 0 ? "black" : "red", projectsList[0].PersonLevelDetails[i].PotentialRevenue));
                row.Add(string.Format(NPOIExcel.CustomColorKey, projectsList[0].PersonLevelDetails[i].BillableCost >= 0 ? "black" : "red", projectsList[0].PersonLevelDetails[i].BillableCost));
                row.Add(string.Format(NPOIExcel.CustomColorKey, projectsList[0].PersonLevelDetails[i].NonBillableCost >= 0 ? "black" : "red", projectsList[0].PersonLevelDetails[i].NonBillableCost));
                row.Add(string.Format(NPOIExcel.CustomColorKey, projectsList[0].PersonLevelDetails[i].TotalCost >= 0 ? "black" : "red", projectsList[0].PersonLevelDetails[i].TotalCost));
                row.Add(string.Format(NPOIExcel.CustomColorKey, projectsList[0].PersonLevelDetails[i].BillableMargin >= 0 ? "black" : "red", projectsList[0].PersonLevelDetails[i].BillableMargin));
                row.Add(string.Format(NPOIExcel.CustomColorKey, projectsList[0].PersonLevelDetails[i].ActualMargin >= 0 ? "black" : "red", projectsList[0].PersonLevelDetails[i].ActualMargin));
                row.Add(string.Format(NPOIExcel.CustomColorKey, projectsList[0].PersonLevelDetails[i].FLHR >= 0 ? "black" : "red", projectsList[0].PersonLevelDetails[i].FLHR));
                row.Add(string.Format(NPOIExcel.CustomColorKey, projectsList[0].PersonLevelDetails[i].BillRate >= 0 ? "black" : "red", projectsList[0].PersonLevelDetails[i].BillRate));
                data.Rows.Add(row.ToArray());
            }
            var lastrow = new List<object>();
            var flhr = projectsList[0].PersonLevelDetails.Sum(p => p.TotalHours) == 0 ? 0 : projectsList[0].PersonLevelDetails.Sum(p => p.TotalCost) / projectsList[0].PersonLevelDetails.Sum(p => p.TotalHours);
            var billrate = projectsList[0].PersonLevelDetails.Sum(p => p.BillableHours) == 0 ? 0 : projectsList[0].PersonLevelDetails.Sum(p => p.BillableRevenue) / projectsList[0].PersonLevelDetails.Sum(p => p.BillableHours);
            for (int j = 0; j < 10; j++)
                lastrow.Add("");
            lastrow.Add(string.Format(NPOIExcel.CustomColorKey, "black", projectsList[0].PersonLevelDetails.Sum(p => p.BillableHours)));
            lastrow.Add(string.Format(NPOIExcel.CustomColorKey, "black", projectsList[0].PersonLevelDetails.Sum(p => p.ProjectNonBillableHours)));
            lastrow.Add(string.Format(NPOIExcel.CustomColorKey, "black", projectsList[0].PersonLevelDetails.Sum(p => p.BillableHours) + projectsList[0].PersonLevelDetails.Sum(p => p.ProjectNonBillableHours)));
            lastrow.Add(string.Format(NPOIExcel.CustomColorKey, projectsList[0].PersonLevelDetails.Sum(p => p.BillableRevenue) >= 0 ? "black" : "red", projectsList[0].PersonLevelDetails.Sum(p => p.BillableRevenue)));
            lastrow.Add(string.Format(NPOIExcel.CustomColorKey, projectsList[0].PersonLevelDetails.Sum(p => p.LostRevenue) >= 0 ? "black" : "red", projectsList[0].PersonLevelDetails.Sum(p => p.LostRevenue)));
            lastrow.Add(string.Format(NPOIExcel.CustomColorKey, projectsList[0].PersonLevelDetails.Sum(p => p.PotentialRevenue) >= 0 ? "black" : "red", projectsList[0].PersonLevelDetails.Sum(p => p.PotentialRevenue)));
            lastrow.Add(string.Format(NPOIExcel.CustomColorKey, projectsList[0].PersonLevelDetails.Sum(p => p.BillableCost) >= 0 ? "black" : "red", projectsList[0].PersonLevelDetails.Sum(p => p.BillableCost)));
            lastrow.Add(string.Format(NPOIExcel.CustomColorKey, projectsList[0].PersonLevelDetails.Sum(p => p.NonBillableCost) >= 0 ? "black" : "red", projectsList[0].PersonLevelDetails.Sum(p => p.NonBillableCost)));
            lastrow.Add(string.Format(NPOIExcel.CustomColorKey, projectsList[0].PersonLevelDetails.Sum(p => p.TotalCost) >= 0 ? "black" : "red", projectsList[0].PersonLevelDetails.Sum(p => p.TotalCost)));
            lastrow.Add(string.Format(NPOIExcel.CustomColorKey, projectsList[0].PersonLevelDetails.Sum(p => p.BillableMargin) >= 0 ? "black" : "red", projectsList[0].PersonLevelDetails.Sum(p => p.BillableMargin)));
            lastrow.Add(string.Format(NPOIExcel.CustomColorKey, projectsList[0].PersonLevelDetails.Sum(p => p.ActualMargin) >= 0 ? "black" : "red", projectsList[0].PersonLevelDetails.Sum(p => p.ActualMargin)));
            lastrow.Add(string.Format(NPOIExcel.CustomColorKey, flhr >= 0 ? "black" : "red", flhr));
            lastrow.Add(string.Format(NPOIExcel.CustomColorKey, billrate >= 0 ? "black" : "red", billrate));
            data.Rows.Add(lastrow.ToArray());
            return data;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            var filename = string.Format("NonBillableReportDetail-{0}-{1}.xls",
               HostingPage.StartDate.Value.ToString("MM_dd_yyyy"), HostingPage.EndDate.Value.ToString("MM_dd_yyyy"));
            DataHelper.InsertExportActivityLogMessage(NonbillableReportDetailExport);
            var sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();
            var report =
               ServiceCallers.Custom.Report(r => r.NonBillableReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, ProjectNumber, null, null, null).ToList());

            if (report[0].PersonLevelDetails.Count > 0)
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
                dataset.DataSetName = "Nonbillable_Detail";
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
                dataset.DataSetName = "Nonbillable_Detail";
                dataset.Tables.Add(header);
                dataSetList.Add(dataset);
            }
            NPOIExcel.Export(filename, dataSetList, sheetStylesList);
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.NumberFormatWithCommasAndDecimals);
        }

        protected string GetCurrencyDecimalFormat(double value)
        {
            var currency = new PracticeManagementCurrency();
            currency.Value = Convert.ToDecimal(value);
            return currency.ToString();
        }

        public void PopulateData(ProjectLevelGroupedHours project)
        {
            CurrentProject = project;
            ProjectNumber = project.Project.ProjectNumber;
            repNonbillable.DataSource = project.PersonLevelDetails;
            repNonbillable.DataBind();
        }

        protected void repNonbillable_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                count = 0;
            }

            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lblProjectNumber = e.Item.FindControl("lblProjectNumber") as Label;
                var lblAccount = e.Item.FindControl("lblAccount") as Label;
                var lblProjectGroup = e.Item.FindControl("lblProjectGroup") as Label;
                var lblProjectName = e.Item.FindControl("lblProjectName") as Label;
                var lblPractice = e.Item.FindControl("lblPractice") as Label;
                var lblSalesPerson = e.Item.FindControl("lblSalesPerson") as Label;
                var lblProjectManagers = e.Item.FindControl("lblProjectManagers") as Label;
                var lblSeniorManager = e.Item.FindControl("lblSeniorManager") as Label;
                var lblDirector = e.Item.FindControl("lblDirector") as Label;

                if (count == 0)
                {
                    lblProjectNumber.Text = CurrentProject.Project.ProjectNumber;
                    lblAccount.Text = CurrentProject.Project.Client.HtmlEncodedName;
                    lblProjectGroup.Text = CurrentProject.Project.Group.HtmlEncodedName;
                    lblProjectName.Text = CurrentProject.Project.HtmlEncodedName;
                    lblPractice.Text = CurrentProject.Project.Practice.HtmlEncodedName;
                    lblSalesPerson.Text = CurrentProject.Project.SalesPersonName;
                    lblProjectManagers.Text = string.IsNullOrEmpty(CurrentProject.Project.ProjectManagerNames) ? string.Empty : CurrentProject.Project.ProjectManagerNames;
                    lblSeniorManager.Text = CurrentProject.Project.SeniorManagerName;
                    lblDirector.Text = CurrentProject.Project.DirectorName;
                }
                else
                {
                    lblProjectNumber.Text = lblProjectGroup.Text = lblProjectName.Text = lblAccount.Text = lblPractice.Text = lblSalesPerson.Text = lblProjectManagers.Text = lblSeniorManager.Text = lblDirector.Text = string.Empty;
                }
                count++;
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                var lblTotalBillableHours = e.Item.FindControl("lblTotalBillableHours") as Label;
                var lblTotalNonbillableHours = e.Item.FindControl("lblTotalNonbillableHours") as Label;
                var lblTotalofTotalHours = e.Item.FindControl("lblTotalofTotalHours") as Label;
                var lblTotalBillableRevenue = e.Item.FindControl("lblTotalBillableRevenue") as Label;
                var lblTotalLostRevenue = e.Item.FindControl("lblTotalLostRevenue") as Label;
                var lblTotalPotentialRevnue = e.Item.FindControl("lblTotalPotentialRevnue") as Label;
                var lblTotalBillCost = e.Item.FindControl("lblTotalBillCost") as Label;
                var lblTotalNonbillableCost = e.Item.FindControl("lblTotalNonbillableCost") as Label;
                var lblTotalofTotalCost = e.Item.FindControl("lblTotalofTotalCost") as Label;
                var lblTotalBillMargin = e.Item.FindControl("lblTotalBillMargin") as Label;
                var lblTotalActualMargin = e.Item.FindControl("lblTotalActualMargin") as Label;
                var lblTotalFlhr = e.Item.FindControl("lblTotalFlhr") as Label;
                var lblTotalBillRate = e.Item.FindControl("lblTotalBillRate") as Label;

                lblTotalBillableHours.Text = GetDoubleFormat(CurrentProject.PersonLevelDetails.Sum(p => p.BillableHours));
                lblTotalNonbillableHours.Text = GetDoubleFormat(CurrentProject.PersonLevelDetails.Sum(p => p.ProjectNonBillableHours));
                lblTotalofTotalHours.Text = GetDoubleFormat(CurrentProject.PersonLevelDetails.Sum(p => p.BillableHours) + CurrentProject.PersonLevelDetails.Sum(p => p.ProjectNonBillableHours));
                lblTotalBillableRevenue.Text = GetCurrencyDecimalFormat(CurrentProject.PersonLevelDetails.Sum(p => p.BillableRevenue));
                lblTotalLostRevenue.Text = GetCurrencyDecimalFormat(CurrentProject.PersonLevelDetails.Sum(p => p.LostRevenue));
                lblTotalPotentialRevnue.Text = GetCurrencyDecimalFormat(CurrentProject.PersonLevelDetails.Sum(p => p.PotentialRevenue));
                lblTotalBillCost.Text = GetCurrencyDecimalFormat(CurrentProject.PersonLevelDetails.Sum(p => p.BillableCost));
                lblTotalNonbillableCost.Text = GetCurrencyDecimalFormat(CurrentProject.PersonLevelDetails.Sum(p => p.NonBillableCost));
                lblTotalofTotalCost.Text = GetCurrencyDecimalFormat(CurrentProject.PersonLevelDetails.Sum(p => p.TotalCost));
                lblTotalBillMargin.Text = GetCurrencyDecimalFormat(CurrentProject.PersonLevelDetails.Sum(p => p.BillableMargin));
                lblTotalActualMargin.Text = GetCurrencyDecimalFormat(CurrentProject.PersonLevelDetails.Sum(p => p.ActualMargin));
                lblTotalFlhr.Text = GetCurrencyDecimalFormat(CurrentProject.PersonLevelDetails.Sum(p => p.TotalHours) == 0 ? 0 : CurrentProject.PersonLevelDetails.Sum(p => p.TotalCost) / CurrentProject.PersonLevelDetails.Sum(p => p.TotalHours));
                lblTotalBillRate.Text = GetCurrencyDecimalFormat(CurrentProject.PersonLevelDetails.Sum(p => p.BillableHours) == 0 ? 0 : CurrentProject.PersonLevelDetails.Sum(p => p.BillableRevenue) / CurrentProject.PersonLevelDetails.Sum(p => p.BillableHours));
            }
        }
    }
}

