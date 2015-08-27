using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Web.UI.HtmlControls;
using DataTransferObjects.TimeEntry;
using System.Text;
using PraticeManagement.Utils.Excel;
using PraticeManagement.Utils;
using System.Data;

namespace PraticeManagement.Controls.Reports
{
    public partial class ByworkType : System.Web.UI.UserControl
    {
        private string ShowPanel = "ShowPanel('{0}', '{1}','{2}');";
        private string HidePanel = "HidePanel('{0}');";
        private string OnMouseOver = "onmouseover";
        private string OnMouseOut = "onmouseout";
        private string ProjectSummaryReportExport = "ProjectSummary Report By WorkType";
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;

        private SheetStyles HeaderSheetStyle
        {
            get
            {
                CellStyles cellStyle = new CellStyles();
                cellStyle.IsBold = true;
                cellStyle.BorderStyle = NPOI.SS.UserModel.BorderStyle.None;
                cellStyle.FontHeight = 200;
                CellStyles[] cellStylearray = { cellStyle };
                RowStyles headerrowStyle = new RowStyles(cellStylearray);
                headerrowStyle.Height = 350;

                RowStyles[] rowStylearray = { headerrowStyle};

                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
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

                CellStyles dataPercentCellStyle = new CellStyles();
                dataPercentCellStyle.DataFormat = "0.00%";

                CellStyles[] dataCellStylearray = { dataCellStyle,
                                                    dataCellStyle, 
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataPercentCellStyle
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

        private Label LblBillable { get; set; }

        private Label LblNonBillable { get; set; }

        private Label LblActualHours { get; set; }

        private PraticeManagement.Reporting.ProjectSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.ProjectSummaryReport)Page); }
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        private void PopulateWorkTypeTotalHoursPercent(WorkTypeLevelGroupedHours[] reportData)
        {
            double grandTotal = reportData.Sum(t => t.TotalHours);
            grandTotal = Math.Round(grandTotal, 2);
            if (grandTotal > 0)
            {
                foreach (WorkTypeLevelGroupedHours workTypeLevelGroupedHours in reportData)
                {
                    workTypeLevelGroupedHours.WorkTypeTotalHoursPercent = Convert.ToInt32((workTypeLevelGroupedHours.TotalHours / grandTotal) * 100);
                }
            }
        }

        protected void repWorkType_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                LblBillable = e.Item.FindControl("lblBillable") as Label;
                LblNonBillable = e.Item.FindControl("lblNonBillable") as Label;
                LblActualHours = e.Item.FindControl("lblActualHours") as Label;
            }
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(ProjectSummaryReportExport);
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();
            var project = ServiceCallers.Custom.Project(p => p.GetProjectShortByProjectNumber(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.StartDate, HostingPage.EndDate));
            List<WorkTypeLevelGroupedHours> report = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByWorkType(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.PeriodSelected == "*" ? null : HostingPage.StartDate, HostingPage.PeriodSelected == "*" ? null : HostingPage.EndDate, null)).ToList();
            report = report.OrderBy(p => p.WorkType.Name).ToList();
            PopulateWorkTypeTotalHoursPercent(report.ToArray());

            var filename = string.Format("{0}_{1}_{2}.xls", project.ProjectNumber, project.Name, "_ByWorkType");
            filename = filename.Replace(' ', '_');
            if (report.Count > 0)
            {
                DataTable header1 = new DataTable();
                header1.Columns.Add(project.Client.HtmlEncodedName);
                header1.Columns.Add(project.Group.HtmlEncodedName);

                var row1 = new List<object>();
                row1.Add(string.Format("{0} - {1}", project.ProjectNumber, project.HtmlEncodedName));
                row1.Add("");
                header1.Rows.Add(row1.ToArray());

                var row2 = new List<object>();
                row2.Add(string.IsNullOrEmpty(project.BillableType) ? project.Status.Name : project.Status.Name + ", " + project.BillableType);
                row2.Add("");
                header1.Rows.Add(row2.ToArray());

                var row3 = new List<object>();
                row3.Add(HostingPage.ProjectRangeForExcel);
                row3.Add("");
                header1.Rows.Add(row3.ToArray());

                headerRowsCount = header1.Rows.Count + 3;

                var data = PrepareDataTable(report);
                coloumnsCount = data.Columns.Count;
                sheetStylesList.Add(HeaderSheetStyle);
                sheetStylesList.Add(DataSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "Project_ByWorkType";
                dataset.Tables.Add(header1);
                dataset.Tables.Add(data);
                dataSetList.Add(dataset);

            }
            else
            {
                string dateRangeTitle = "There are no Time Entries towards this project.";
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                sheetStylesList.Add(HeaderSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "Project_ByWorkType";
                dataset.Tables.Add(header);
                dataSetList.Add(dataset);
            }
            
            NPOIExcel.Export(filename, dataSetList, sheetStylesList);
        }

        public DataTable PrepareDataTable(List<WorkTypeLevelGroupedHours> report)
        {
            DataTable data = new DataTable();
            List<object> rownew;
            List<object> row;

            data.Columns.Add("WorkType");
            data.Columns.Add("Billable");
            data.Columns.Add("Non-Billable");
            data.Columns.Add("Actual Hours");
            data.Columns.Add("Percent of Total Actual Hours");
            
            foreach (var item in report)
            {
                row = new List<object>();
                row.Add(item.WorkType.Name);
                row.Add(GetDoubleFormat(item.BillableHours));
                row.Add(GetDoubleFormat(item.NonBillableHours));
                row.Add(GetDoubleFormat(item.TotalHours));
                row.Add(GetPercentageFormat(item.WorkTypeTotalHoursPercent));
                data.Rows.Add(row.ToArray());
            }
            return data;
        } 

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

        protected string GetPercentageFormat(double value)
        {
            var result = (double)(value/100);
            return result.ToString();
        }

        public void DataBindResource(WorkTypeLevelGroupedHours[] reportData)
        {
            if (reportData.Length > 0)
            {
                divEmptyMessage.Style["display"] = "none";
                repWorkType.Visible = true;
                PopulateWorkTypeTotalHoursPercent(reportData);
                repWorkType.DataSource = reportData;
                repWorkType.DataBind();

                //populate header hover
                LblBillable.Attributes[OnMouseOver] = string.Format(ShowPanel, LblBillable.ClientID, pnlTotalBillableHours.ClientID, 0);
                LblBillable.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalBillableHours.ClientID);

                LblNonBillable.Attributes[OnMouseOver] = string.Format(ShowPanel, LblNonBillable.ClientID, pnlTotalNonBillableHours.ClientID, 0);
                LblNonBillable.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalNonBillableHours.ClientID);

                LblActualHours.Attributes[OnMouseOver] = string.Format(ShowPanel, LblActualHours.ClientID, pnlTotalActualHours.ClientID, 0);
                LblActualHours.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalActualHours.ClientID);
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repWorkType.Visible = false;
            }
            PopulateHeaderSection(reportData);
        }

        public void PopulateByWorkTypeData()
        {
            WorkTypeLevelGroupedHours[] data;
            data = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByWorkType(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.PeriodSelected == "*" ? null : HostingPage.StartDate, HostingPage.PeriodSelected == "*" ? null : HostingPage.EndDate, null));
            DataBindResource(data);
        }

        private void PopulateHeaderSection(WorkTypeLevelGroupedHours[] reportData)
        {
            var project = ServiceCallers.Custom.Project(p => p.GetProjectShortByProjectNumber(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.StartDate, HostingPage.EndDate));
            double billableHours = reportData.Sum(p => p.BillableHours);
            double nonBillableHours = reportData.Sum(p => p.NonBillableHours);
            double projectedHours = reportData.Length > 0 ? reportData[0].ForecastedHours : 0d;
            double actualHours = reportData.Sum(p => p.TotalHours);

            var billablePercent = 0;
            var nonBillablePercent = 0;
            if (billableHours != 0 || nonBillableHours != 0)
            {
                billablePercent = DataTransferObjects.Utils.Generic.GetBillablePercentage(billableHours, nonBillableHours);
                nonBillablePercent = (100 - billablePercent);
            }

            ltrlAccount.Text = project.Client.HtmlEncodedName;
            ltrlBusinessUnit.Text = project.Group.HtmlEncodedName;
            ltrlProjectedHours.Text = projectedHours.ToString(Constants.Formatting.DoubleValue);
            ltrlProjectName.Text = project.HtmlEncodedName;
            ltrlProjectNumber.Text = project.ProjectNumber;
            ltrlProjectStatusAndBillingType.Text = string.IsNullOrEmpty(project.BillableType) ? project.Status.Name : project.Status.Name + ", " + project.BillableType;
            ltrlProjectRange.Text = HostingPage.ProjectRange;
            ltrlTotalHours.Text = (billableHours + nonBillableHours).ToString(Constants.Formatting.DoubleValue);
            ltrlBillableHours.Text = lblTotalBillableHours.Text = lblTotalBillablePanlActual.Text = billableHours.ToString(Constants.Formatting.DoubleValue);
            ltrlNonBillableHours.Text = lblTotalNonBillableHours.Text = lblTotalNonBillablePanlActual.Text = nonBillableHours.ToString(Constants.Formatting.DoubleValue);
            ltrlBillablePercent.Text = billablePercent.ToString();
            ltrlNonBillablePercent.Text = nonBillablePercent.ToString();
            lblTotalActualHours.Text = actualHours.ToString(Constants.Formatting.DoubleValue);

            if (billablePercent == 0 && nonBillablePercent == 0)
            {
                trBillable.Height = "1px";
                trNonBillable.Height = "1px";
            }
            else if (billablePercent == 100)
            {
                trBillable.Height = "80px";
                trNonBillable.Height = "1px";
            }
            else if (billablePercent == 0 && nonBillablePercent == 100)
            {
                trBillable.Height = "1px";
                trNonBillable.Height = "80px";
            }
            else
            {
                int billablebarHeight = (int)(((float)80 / (float)100) * billablePercent);
                trBillable.Height = billablebarHeight.ToString() + "px";
                trNonBillable.Height = (80 - billablebarHeight).ToString() + "px";
            }
        }
    }
}

