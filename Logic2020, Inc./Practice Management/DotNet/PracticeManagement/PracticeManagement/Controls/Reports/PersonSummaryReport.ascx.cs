using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Text;
using PraticeManagement.Utils.Excel;
using System.Data;
using PraticeManagement.Utils;

namespace PraticeManagement.Controls.Reports
{
    public partial class PersonSummaryReport : System.Web.UI.UserControl
    {
        private string PersonSummaryReportExport = "Person Summary Report";
        private string ShowPanel = "ShowPanel('{0}', '{1}','{2}');";
        private string HidePanel = "HidePanel('{0}');";
        private string OnMouseOver = "onmouseover";
        private string OnMouseOut = "onmouseout";
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

                CellStyles dataCellStyle = new CellStyles();
                CellStyles[] dataCellStylearray = { dataCellStyle };
                RowStyles datarowStyle = new RowStyles(dataCellStylearray);

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
                                                    dataCellStyle,
                                                    dataCellStyle, 
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle, 
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

        private Label LblProjectedHours { get; set; }

        private Label LblBillable { get; set; }

        private Label LblNonBillable { get; set; }

        private Label LblActualHours { get; set; }

        private Label LblBillableHoursVariance { get; set; }


        private PraticeManagement.Reporting.PersonDetailTimeReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.PersonDetailTimeReport)Page); }
        }

        protected void repSummary_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                LblProjectedHours = e.Item.FindControl("lblProjectedHours") as Label;
                LblBillable = e.Item.FindControl("lblBillable") as Label;
                LblNonBillable = e.Item.FindControl("lblNonBillable") as Label;
                LblActualHours = e.Item.FindControl("lblActualHours") as Label;
                LblBillableHoursVariance = e.Item.FindControl("lblBillableHoursVariance") as Label;
            }
        }

        public void DatabindRepepeaterSummary(List<TimeEntriesGroupByClientAndProject> timeEntriesGroupByClientAndProjectList)
        {
            if (timeEntriesGroupByClientAndProjectList.Count > 0)
            {
                divEmptyMessage.Style["display"] = "none";
                repSummary.Visible = true;
                repSummary.DataSource = timeEntriesGroupByClientAndProjectList;
                repSummary.DataBind();
                PopulateHeaderHoverLabels(timeEntriesGroupByClientAndProjectList.ToList());
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repSummary.Visible = false;
            }
        }

        private void PopulateHeaderHoverLabels(List<TimeEntriesGroupByClientAndProject> reportData)
        {
            LblProjectedHours.Attributes[OnMouseOver] = string.Format(ShowPanel, LblProjectedHours.ClientID, pnlTotalProjectedHours.ClientID, 0);
            LblProjectedHours.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalProjectedHours.ClientID);

            LblBillable.Attributes[OnMouseOver] = string.Format(ShowPanel, LblBillable.ClientID, pnlTotalBillableHours.ClientID, 0);
            LblBillable.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalBillableHours.ClientID);

            LblNonBillable.Attributes[OnMouseOver] = string.Format(ShowPanel, LblNonBillable.ClientID, pnlTotalNonBillableHours.ClientID, 0);
            LblNonBillable.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalNonBillableHours.ClientID);

            LblActualHours.Attributes[OnMouseOver] = string.Format(ShowPanel, LblActualHours.ClientID, pnlTotalActualHours.ClientID, 0);
            LblActualHours.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalActualHours.ClientID);

            LblBillableHoursVariance.Attributes[OnMouseOver] = string.Format(ShowPanel, LblBillableHoursVariance.ClientID, pnlBillableHoursVariance.ClientID, 0);
            LblBillableHoursVariance.Attributes[OnMouseOut] = string.Format(HidePanel, pnlBillableHoursVariance.ClientID);
            double totalBillableHoursVariance = reportData.Sum(p => p.BillableHoursVariance);

            lblTotalProjectedHours.Text = reportData.Sum(p => p.ProjectedHours).ToString(Constants.Formatting.DoubleValue);
            lblTotalBillableHours.Text = lblTotalBillablePanlActual.Text = reportData.Sum(p => p.BillableHours).ToString(Constants.Formatting.DoubleValue);
            lblTotalNonBillableHours.Text = lblTotalNonBillablePanlActual.Text = reportData.Sum(p => p.NonBillableHours).ToString(Constants.Formatting.DoubleValue);
            lblTotalActualHours.Text = reportData.Sum(p => p.TotalHours).ToString(Constants.Formatting.DoubleValue);
            lblTotalBillableHoursVariance.Text = totalBillableHoursVariance.ToString(Constants.Formatting.DoubleValue);
            if (totalBillableHoursVariance < 0)
            {
                lblExclamationMarkPanl.Visible = true;
            }
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            
            DataHelper.InsertExportActivityLogMessage(PersonSummaryReportExport);
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();
            // mso-number-format:"0\.00"
            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                var timeEntriesGroupByClientAndProjectList = ServiceCallers.Custom.Report(r => r.PersonTimeEntriesSummary(HostingPage.SelectedPersonId, HostingPage.StartDate.Value, HostingPage.EndDate.Value)).ToList();

                int personId = HostingPage.SelectedPersonId;
                var person = ServiceCallers.Custom.Person(p => p.GetStrawmanDetailsById(personId));
                string personType = person.IsOffshore ? "Offshore" : string.Empty;
                string payType = person.CurrentPay.TimescaleName;
                string personStatusAndType = string.IsNullOrEmpty(personType) && string.IsNullOrEmpty(payType) ? string.Empty :
                                                                                 string.IsNullOrEmpty(payType) ? personType :
                                                                                 string.IsNullOrEmpty(personType) ? payType :
                                                                                                                     payType + ", " + personType;
                var filename = string.Format("{0}_{1}_{2}_{3}_{4}.xls", person.LastName, (string.IsNullOrEmpty(person.PrefferedFirstName) ? person.FirstName : person.PrefferedFirstName), "Summary", HostingPage.StartDate.Value.ToString("MM.dd.yyyy"), HostingPage.EndDate.Value.ToString("MM.dd.yyyy"));

                if (timeEntriesGroupByClientAndProjectList.Count > 0)
                {

                    DataTable header1 = new DataTable();
                    header1.Columns.Add(person.EmployeeNumber + " - " + (string.IsNullOrEmpty(person.PrefferedFirstName) ? person.FirstName : person.PrefferedFirstName) + " " + person.LastName);

                    header1.Rows.Add(personStatusAndType);
                    header1.Rows.Add(HostingPage.RangeForExcel);

                    headerRowsCount = header1.Rows.Count + 3;
                    var data = PrepareDataTable(timeEntriesGroupByClientAndProjectList);
                    coloumnsCount = data.Columns.Count;
                    sheetStylesList.Add(HeaderSheetStyle);
                    sheetStylesList.Add(DataSheetStyle);
                    var dataset = new DataSet();
                    dataset.DataSetName = "PersonSummary";
                    dataset.Tables.Add(header1);
                    dataset.Tables.Add(data);
                    dataSetList.Add(dataset);
                }
                else
                {
                    string dateRangeTitle = "This person has not entered Time Entries for the selected period.";
                    DataTable header = new DataTable();
                    header.Columns.Add(dateRangeTitle);
                    sheetStylesList.Add(HeaderSheetStyle);
                    var dataset = new DataSet();
                    dataset.DataSetName = "PersonSummary";
                    dataset.Tables.Add(header);
                    dataSetList.Add(dataset);
                }
                //“[LastName]_[FirstName]-[“Summary” or “Detail”]-[StartOfRange]_[EndOfRange].xls”.  
                //example :Hong-Turney_Jason-Summary-03.01.2012_03.31.2012.xlsx
                
                NPOIExcel.Export(filename, dataSetList, sheetStylesList);
            }
        }

        public DataTable PrepareDataTable(List<TimeEntriesGroupByClientAndProject> report)
        {
            DataTable data = new DataTable();
            List<object> rownew;
            List<object> row;

            data.Columns.Add("Account");
            data.Columns.Add("Account Name");
            data.Columns.Add("Business Unit");
            data.Columns.Add("Business Unit Name");
            data.Columns.Add("Project");
            data.Columns.Add("Project Name");
            data.Columns.Add("Status");
            data.Columns.Add("Billing");
            data.Columns.Add("Projected Hours");
            data.Columns.Add("Billable");
            data.Columns.Add("Non-Billable");
            data.Columns.Add("Actual Hours");
            data.Columns.Add("Billable Hours Variance");
            data.Columns.Add("Percent of Total Hours this Period");
            foreach (var timeEntriesGroupByClientAndProject in report)
            {
                row = new List<object>();
                row.Add(timeEntriesGroupByClientAndProject.Client.Code);
                row.Add(timeEntriesGroupByClientAndProject.Client.HtmlEncodedName);
                row.Add(timeEntriesGroupByClientAndProject.Project.Group.Code);
                row.Add(timeEntriesGroupByClientAndProject.Project.Group.HtmlEncodedName);
                row.Add(timeEntriesGroupByClientAndProject.Project.ProjectNumber);
                row.Add(timeEntriesGroupByClientAndProject.Project.HtmlEncodedName);
                row.Add(timeEntriesGroupByClientAndProject.Project.Status.Name);
                row.Add(timeEntriesGroupByClientAndProject.BillableType);
                row.Add(GetDoubleFormat(timeEntriesGroupByClientAndProject.ProjectedHours));
                row.Add(GetDoubleFormat(timeEntriesGroupByClientAndProject.BillableHours));
                row.Add(GetDoubleFormat(timeEntriesGroupByClientAndProject.NonBillableHours));
                row.Add(GetDoubleFormat(timeEntriesGroupByClientAndProject.TotalHours));
                row.Add(GetDoubleFormat(timeEntriesGroupByClientAndProject.BillableHoursVariance));
                row.Add((double)timeEntriesGroupByClientAndProject.ProjectTotalHoursPercent/100);
                data.Rows.Add(row.ToArray());
            }
            return data;
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

    }
}

