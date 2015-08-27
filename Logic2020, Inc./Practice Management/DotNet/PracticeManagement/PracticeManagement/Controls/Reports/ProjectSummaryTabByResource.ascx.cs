using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Text;
using System.Web.UI.HtmlControls;
using iTextSharp.text.pdf;
using PraticeManagement.Objects;
using System.IO;
using iTextSharp.text;
using PraticeManagement.Configuration;
using DataTransferObjects;
using PraticeManagement.Utils.Excel;
using System.Data;
using PraticeManagement.Utils;

namespace PraticeManagement.Controls.Reports
{
    public partial class ProjectSummaryTabByResource : System.Web.UI.UserControl
    {
        private string ProjectSummaryByResourceExport = "Project Summary Report By Resource";
        private string ByPersonByResourceUrl = "PersonDetailTimeReport.aspx?StartDate={0}&EndDate={1}&PeriodSelected={2}&PersonId={3}";
        private string ShowPanel = "ShowPanel('{0}', '{1}','{2}');";
        private string HidePanel = "HidePanel('{0}');";
        private string OnMouseOver = "onmouseover";
        private string OnMouseOut = "onmouseout";
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;

        #region Variables

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

                CellStyles dataNumberDataCellStyle = new CellStyles();
                dataNumberDataCellStyle.DataFormat = "$#,##0.00";

                CellStyles[] dataCellStylearray = { dataCellStyle,
                                                    dataCellStyle, 
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle, 
                                                    dataCellStyle,
                                                   dataNumberDataCellStyle,
                                                   dataNumberDataCellStyle,
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

        private HtmlImage ImgProjectRoleFilter { get; set; }

        public FilteredCheckBoxList cblProjectRolesControl
        {
            get
            {
                return cblProjectRoles;
            }
        }

        private PraticeManagement.Reporting.ProjectSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.ProjectSummaryReport)Page.Page); }
        }

        private PraticeManagement.Controls.Reports.ProjectSummaryByResource HostingControl
        {
            get { return (PraticeManagement.Controls.Reports.ProjectSummaryByResource)HostingPage.ByResourceControl; }
        }

        private Label LblProjectedHours { get; set; }

        private Label LblBillable { get; set; }

        private Label LblNonBillable { get; set; }

        private Label LblActualHours { get; set; }

        private Label LblBillableHoursVariance { get; set; }

        #endregion

        #region Methods

        public void DataBindByResourceSummary(PersonLevelGroupedHours[] reportData, bool isFirstTime)
        {
            if (isFirstTime)
            {
                PopulateProjectRoleFilter(reportData.ToList());
            }
            if (reportData.Count() > 0 || cblProjectRoles.Items.Count > 1)
            {
                divEmptyMessage.Style["display"] = "none";
                repResource.Visible = true;
                repResource.DataSource = reportData;
                repResource.DataBind();
                cblProjectRoles.SaveSelectedIndexesInViewState();
                ImgProjectRoleFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblProjectRoles.FilterPopupClientID,
                  cblProjectRoles.SelectedIndexes, cblProjectRoles.CheckBoxListObject.ClientID, cblProjectRoles.WaterMarkTextBoxBehaviorID);

                //Populate header hover               
                PopulateHeaderHoverLabels(reportData.ToList());
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repResource.Visible = false;
            }
            btnExportToPDF.Enabled =
            btnExportToExcel.Enabled = reportData.Count() > 0;
        }

        private void PopulateHeaderHoverLabels(List<PersonLevelGroupedHours> reportData)
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
            lblTotalProjectedHours.Text = reportData.Sum(p => p.ForecastedHours).ToString(Constants.Formatting.DoubleValue);
            lblTotalBillableHours.Text = lblTotalBillablePanlActual.Text = reportData.Sum(p => p.BillableHours).ToString(Constants.Formatting.DoubleValue);
            lblTotalNonBillableHours.Text = lblTotalNonBillablePanlActual.Text = reportData.Sum(p => p.NonBillableHours).ToString(Constants.Formatting.DoubleValue);
            lblTotalActualHours.Text = reportData.Sum(p => p.TotalHours).ToString(Constants.Formatting.DoubleValue);
            lblTotalBillableHoursVariance.Text = totalBillableHoursVariance.ToString(Constants.Formatting.DoubleValue);
            if (totalBillableHoursVariance < 0)
            {
                lblExclamationMarkPanl.Visible = true;
            }
        }

        private void PopulateProjectRoleFilter(List<PersonLevelGroupedHours> reportData)
        {
            var projectRoles = reportData.Select(r => new { Text = string.IsNullOrEmpty(r.Person.ProjectRoleName) ? "Unassigned" : r.Person.ProjectRoleName, Value = r.Person.ProjectRoleName }).Distinct().ToList().OrderBy(s => s.Value);
            DataHelper.FillListDefault(cblProjectRoles.CheckBoxListObject, "All Project Roles", projectRoles.ToArray(), false, "Value", "Text");
            cblProjectRoles.SelectAllItems(true);
            cblProjectRoles.OKButtonId = btnUpdate.ClientID;
        }

        #endregion

        #region Control Methods

        protected void repResource_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                ImgProjectRoleFilter = e.Item.FindControl("imgProjectRoleFilter") as HtmlImage;

                LblProjectedHours = e.Item.FindControl("lblProjectedHours") as Label;
                LblBillable = e.Item.FindControl("lblBillable") as Label;
                LblNonBillable = e.Item.FindControl("lblNonBillable") as Label;
                LblActualHours = e.Item.FindControl("lblActualHours") as Label;
                LblBillableHoursVariance = e.Item.FindControl("lblBillableHoursVariance") as Label;
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = (PersonLevelGroupedHours)e.Item.DataItem;
                var LnkActualHours = e.Item.FindControl("lnkActualHours") as LinkButton;
                LnkActualHours.Attributes["NavigationUrl"] = string.Format(ByPersonByResourceUrl, (HostingPage.StartDate.HasValue) ? HostingPage.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) : null,
                    (HostingPage.EndDate.HasValue) ? HostingPage.EndDate.Value.Date.ToString(Constants.Formatting.EntryDateFormat) : null, (HostingPage.PeriodSelected != "*") ? HostingPage.PeriodSelected : "0", dataItem.Person.Id);
            }
        }

        public string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(ProjectSummaryByResourceExport);
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();

            var project = ServiceCallers.Custom.Project(p => p.GetProjectShortByProjectNumber(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.StartDate, HostingPage.EndDate));
            List<PersonLevelGroupedHours> report = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByResource(HostingPage.ProjectNumber,
                HostingPage.MilestoneId, HostingPage.PeriodSelected == "*" ? null : HostingPage.StartDate, HostingPage.PeriodSelected == "*" ? null : HostingPage.EndDate, cblProjectRoles.SelectedItemsXmlFormat)).ToList();

            string filterApplied = "Filters applied to columns: ";
            bool isFilterApplied = false;
            if (!cblProjectRoles.AllItemsSelected)
            {
                filterApplied = filterApplied + " ProjectRoles.";
                isFilterApplied = true;
            }

            var filename = string.Format("{0}_{1}_{2}.xls", project.ProjectNumber, project.Name, "_ByResourceSummary");
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

                var row4 = new List<object>();
                if (isFilterApplied)
                {
                    row4.Add(filterApplied);
                    row4.Add("");
                    header1.Rows.Add(row4.ToArray());
                }
                headerRowsCount = header1.Rows.Count + 3;
                var data = PrepareDataTable(report);
                coloumnsCount = data.Columns.Count;
                sheetStylesList.Add(HeaderSheetStyle);
                sheetStylesList.Add(DataSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "Project_ByResourceSummary";
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
                dataset.DataSetName = "Project_ByResourceSummary";
                dataset.Tables.Add(header);
                dataSetList.Add(dataset);
            }

            NPOIExcel.Export(filename, dataSetList, sheetStylesList);
        }

        public DataTable PrepareDataTable(List<PersonLevelGroupedHours> report)
        {
            DataTable data = new DataTable();
            List<object> rownew;
            List<object> row;

            data.Columns.Add("Employee Id");
            data.Columns.Add("Resource");
            data.Columns.Add("Project Role");
            data.Columns.Add("Projected Hours");
            data.Columns.Add("Billable");
            data.Columns.Add("Non-Billable");
            data.Columns.Add("Actual Hours");
            data.Columns.Add("Hourly Bill Rate");
            data.Columns.Add("Estimated Billings");
            data.Columns.Add("Billable Hours Variance");

            var list = report.OrderBy(p => p.Person.PersonLastFirstName);
            foreach (var item in list)
            {
                row = new List<object>();
                row.Add(item.Person.EmployeeNumber != null ? item.Person.EmployeeNumber : "");
                row.Add(item.Person.HtmlEncodedName != null ? item.Person.HtmlEncodedName : "");
                row.Add((item.Person.ProjectRoleName != null) ? item.Person.ProjectRoleName : "");
                row.Add(GetDoubleFormat(item.ForecastedHours));
                row.Add(GetDoubleFormat(item.BillableHours));
                row.Add(GetDoubleFormat(item.NonBillableHours));
                row.Add(GetDoubleFormat(item.TotalHours));
                row.Add(item.FormattedBillRateForExcel);
                row.Add(item.FormattedEstimatedBillingsForExcel);
                row.Add(GetDoubleFormat(item.BillableHoursVariance));
                data.Rows.Add(row.ToArray());
            }
            return data;
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(ProjectSummaryByResourceExport);
            HostingPage.PDFExport();
        }

        protected void btnUpdate_OnClick(object sender, EventArgs e)
        {
            HostingControl.PopulateByResourceSummaryReport();
        }

        #endregion

    }
}

