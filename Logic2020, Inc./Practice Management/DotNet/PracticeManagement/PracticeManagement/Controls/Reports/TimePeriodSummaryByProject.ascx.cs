using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Web.UI.HtmlControls;
using System.Text;
using DataTransferObjects.Reports.ByAccount;
using ByBusinessDevelopment = PraticeManagement.Controls.Reports.ByAccount.ByBusinessDevelopment;
using PraticeManagement.Utils.Excel;
using PraticeManagement.Utils;
using System.Data;

namespace PraticeManagement.Controls.Reports
{
    public partial class TimePeriodSummaryByProject : System.Web.UI.UserControl
    {
        private string TimePeriodSummaryReportExport = "TimePeriod Summary Report By Project";
        private string ByProjectByResourceUrl = "ProjectSummaryReport.aspx?StartDate={0}&EndDate={1}&PeriodSelected={2}&ProjectNumber={3}";
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

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle};

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

        private HtmlImage ImgClientFilter { get; set; }

        private HtmlImage ImgProjectStatusFilter { get; set; }

        private Label LblProjectedHours { get; set; }

        private Label LblBillable { get; set; }

        private Label LblNonBillable { get; set; }

        private Label LblActualHours { get; set; }

        private Label LblBillableHoursVariance { get; set; }

        public string SelectedProjectNumber
        {
            get
            {
                return (string)ViewState["SelectedProjectNumberForDetails"];
            }
            set
            {
                ViewState["SelectedProjectNumberForDetails"] = value;
            }
        }

        public AjaxControlToolkit.ModalPopupExtender MpeProjectDetailReport
        {
            get
            {
                return mpeProjectDetailReport;
            }
        }

        public ByBusinessDevelopment ByBusinessDevelopmentControl
        {
            get
            {
                return ucGroupByBusinessDevelopment;
            }
        }

        private PraticeManagement.Reporting.TimePeriodSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.TimePeriodSummaryReport)Page); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            cblClients.OKButtonId = cblProjectStatus.OKButtonId = btnFilterOK.ClientID;
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected string GetCurrencyFormat(double value)
        {
            return value > 0 ? value.ToString(Constants.Formatting.CurrencyFormat) : "$0";
        }

        protected string GetVarianceSortValue(string variance)
        {
            if (variance.Equals("N/A"))
            {
                return int.MinValue.ToString();
            }
            return variance;
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(TimePeriodSummaryReportExport);
            var filename = string.Format("TimePeriod_ByProject_{0}-{1}.xls", HostingPage.StartDate.Value.ToString("MM_dd_yyyy"), HostingPage.EndDate.Value.ToString("MM_dd_yyyy"));
            var dataSetList = new List<DataSet>();
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                var report = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryReportByProject(HostingPage.StartDate.Value, HostingPage.EndDate.Value,
                    cblClients.SelectedItems, cblProjectStatus.SelectedItems)).ToList();

                string filterApplied = "Filters applied to columns: ";
                List<string> filteredColoums = new List<string>();
                if (!cblClients.AllItemsSelected)
                {
                    filteredColoums.Add("Project");
                }
                if (!cblProjectStatus.AllItemsSelected)
                {
                    filteredColoums.Add("Status");
                }

                if (report.Count > 0)
                {
                    DataTable header1 = new DataTable();
                    header1.Columns.Add("Time Period By Project Report");

                    header1.Rows.Add(report.Count + " Projects");

                    header1.Rows.Add(HostingPage.RangeForExcel);
                    if (filteredColoums.Count > 0)
                    {
                        for (int i = 0; i < filteredColoums.Count; i++)
                        {
                            if (i == filteredColoums.Count - 1)
                                filterApplied = filterApplied + filteredColoums[i] + ".";
                            else
                                filterApplied = filterApplied + filteredColoums[i] + ",";
                        }
                        header1.Rows.Add(filterApplied);
                    }
                    headerRowsCount = header1.Rows.Count + 3;
                    var data = PrepareDataTable(report);
                    coloumnsCount = data.Columns.Count;
                    sheetStylesList.Add(HeaderSheetStyle);
                    sheetStylesList.Add(DataSheetStyle);
                    var dataset = new DataSet();
                    dataset.DataSetName = "TimePeriod_ByProject";
                    dataset.Tables.Add(header1);
                    dataset.Tables.Add(data);
                    dataSetList.Add(dataset);
                }
                else
                {
                    string dateRangeTitle = "There are no Time Entries towards this range selected.";
                    DataTable header = new DataTable();
                    header.Columns.Add(dateRangeTitle);
                    sheetStylesList.Add(HeaderSheetStyle);
                    var dataset = new DataSet();
                    dataset.DataSetName = "TimePeriod_ByProject";
                    dataset.Tables.Add(header);
                    dataSetList.Add(dataset);
                }
                NPOIExcel.Export(filename, dataSetList, sheetStylesList);
            }
        }

        public DataTable PrepareDataTable(List<ProjectLevelGroupedHours> report)
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

            foreach (var item in report)
            {
                row = new List<object>();
                row.Add(item.Project.Client.Code);
                row.Add(item.Project.Client.HtmlEncodedName);
                row.Add(item.Project.Group.Code);
                row.Add(item.Project.Group.HtmlEncodedName);
                row.Add(item.Project.ProjectNumber);
                row.Add(item.Project.HtmlEncodedName);
                row.Add(item.Project.Status.Name);
                row.Add(item.BillingType);
                row.Add(GetDoubleFormat(item.ForecastedHours));
                row.Add(GetDoubleFormat(item.BillableHours));
                row.Add(GetDoubleFormat(item.NonBillableHours));
                row.Add(GetDoubleFormat(item.TotalHours));
                row.Add((item.BillableHoursVariance));
                data.Rows.Add(row.ToArray());
            }
            return data;
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

        public void PopulateByProjectData(bool isPopulateFilters = true)
        {
            ProjectLevelGroupedHours[] data;
            if (isPopulateFilters)
            {
                data = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryReportByProject(HostingPage.StartDate.Value, HostingPage.EndDate.Value, null, null));
            }
            else
            {
                data = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryReportByProject(HostingPage.StartDate.Value, HostingPage.EndDate.Value, cblClients.SelectedItems, cblProjectStatus.SelectedItems));
            }
            DataBindProject(data, isPopulateFilters);
        }

        protected void repProject_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                ImgClientFilter = e.Item.FindControl("imgClientFilter") as HtmlImage;
                ImgProjectStatusFilter = e.Item.FindControl("imgProjectStatusFilter") as HtmlImage;

                LblProjectedHours = e.Item.FindControl("lblProjectedHours") as Label;
                LblBillable = e.Item.FindControl("lblBillable") as Label;
                LblNonBillable = e.Item.FindControl("lblNonBillable") as Label;
                LblActualHours = e.Item.FindControl("lblActualHours") as Label;
                LblBillableHoursVariance = e.Item.FindControl("lblBillableHoursVariance") as Label;
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = (ProjectLevelGroupedHours)e.Item.DataItem;
                var lnkProject = e.Item.FindControl("lnkProject") as LinkButton;
                var imgZoomIn = e.Item.FindControl("imgZoomIn") as HtmlImage;
                var LnkActualHours = e.Item.FindControl("lnkActualHours") as LinkButton;

                lnkProject.Attributes["onmouseover"] = string.Format("document.getElementById(\'{0}\').style.display='';", imgZoomIn.ClientID);
                lnkProject.Attributes["onmouseout"] = string.Format("document.getElementById(\'{0}\').style.display='none';", imgZoomIn.ClientID);
                LnkActualHours.Attributes["NavigationUrl"] = string.Format(ByProjectByResourceUrl, HostingPage.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat),
                    HostingPage.EndDate.Value.Date.ToString(Constants.Formatting.EntryDateFormat), HostingPage.RangeSelected, dataItem.Project.ProjectNumber);
            }
        }

        protected void lnkProject_OnClick(object sender, EventArgs e)
        {
            var lnkProject = sender as LinkButton;
            SelectedProjectNumber = lnkProject.Attributes["ProjectNumber"];

            var businessDevelopmentProj = ServiceCallers.Custom.Project(p => p.GetBusinessDevelopmentProject());
            string totalHours = string.Empty;
            if (businessDevelopmentProj.Any(p=>p.ProjectNumber.ToUpper() == SelectedProjectNumber.ToUpper()))
            {
                HostingPage.AccountId = Convert.ToInt32(lnkProject.Attributes["AccountId"]);
                HostingPage.BusinessUnitIds = lnkProject.Attributes["GroupId"] + ",";
                ucGroupByBusinessDevelopment.Visible = true;
                ucProjectDetailReport.Visible = false;
                ucGroupByBusinessDevelopment.PopulateByBusinessDevelopment();
                totalHours = GetDoubleFormat(HostingPage.Total);
            }
            else
            {

                var list = ServiceCallers.Custom.Report(r => r.ProjectDetailReportByResource(SelectedProjectNumber, null,
                     HostingPage.StartDate, HostingPage.EndDate,
                    null,false)).ToList();

                totalHours = GetDoubleFormat(list.Sum(l => l.TotalHours));
                ucGroupByBusinessDevelopment.Visible = false;
                ucProjectDetailReport.Visible = true;
                ucProjectDetailReport.DataBindByResourceDetail(list);
            }

            ltrlProject.Text = "<b class=\"colorGray\">" +
                lnkProject.Attributes["ClientName"] + " > " + lnkProject.Attributes["GroupName"] + " > </b><b>" + lnkProject.Text + "</b>";
            ltrlProjectDetailTotalhours.Text = totalHours;

            mpeProjectDetailReport.Show();
        }

        protected string GetProjectName(string projectNumber, string name)
        {
            return projectNumber + " - " + name;
        }

        public void DataBindProject(ProjectLevelGroupedHours[] reportData, bool isPopulateFilters)
        {
            if (isPopulateFilters)
            {
                PopulateFilterPanels(reportData);
            }
            if (reportData.Length > 0 || cblClients.Items.Count > 1 || cblProjectStatus.Items.Count > 1)
            {
                divEmptyMessage.Style["display"] = "none";
                repProject.Visible = true;
                repProject.DataSource = reportData;
                repProject.DataBind();
                cblClients.SaveSelectedIndexesInViewState();
                cblProjectStatus.SaveSelectedIndexesInViewState();
                ImgClientFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblClients.FilterPopupClientID,
                  cblClients.SelectedIndexes, cblClients.CheckBoxListObject.ClientID, cblClients.WaterMarkTextBoxBehaviorID);
                ImgProjectStatusFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblProjectStatus.FilterPopupClientID,
                  cblProjectStatus.SelectedIndexes, cblProjectStatus.CheckBoxListObject.ClientID, cblProjectStatus.WaterMarkTextBoxBehaviorID);

                //Populate header hover               
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
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repProject.Visible = false;
            }
            PopulateHeaderSection(reportData);
        }

        private void PopulateFilterPanels(ProjectLevelGroupedHours[] reportData)
        {
            PopulateClientFilter(reportData);
            PopulateProjectStatusFilter(reportData);
        }

        private void PopulateClientFilter(ProjectLevelGroupedHours[] reportData)
        {
            var clients = reportData.Select(r => new { Id = r.Project.Client.Id, Name = r.Project.Client.HtmlEncodedName }).Distinct().ToList().OrderBy(s => s.Name).ToArray();
            int height = 17 * clients.Length;
            Unit unitHeight = new Unit((height + 17) > 50 ? 50 : height + 17);
            DataHelper.FillListDefault(cblClients.CheckBoxListObject, "All Clients", clients, false, "Id", "Name");
            cblClients.Height = unitHeight;
            cblClients.SelectAllItems(true);
        }

        private void PopulateProjectStatusFilter(ProjectLevelGroupedHours[] reportData)
        {
            var projectStatusIds = reportData.Select(r => new { Id = r.Project.Status.Id, Name = r.Project.Status.Name }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblProjectStatus.CheckBoxListObject, "All Status", projectStatusIds.ToArray(), false, "Id", "Name");
            cblProjectStatus.SelectAllItems(true);
        }

        private void PopulateHeaderSection(ProjectLevelGroupedHours[] reportData)
        {
            double billableHours = reportData.Sum(p => p.BillableHours);
            double nonBillableHours = reportData.Sum(p => p.NonBillableHours);
            double totalProjectedHours = reportData.Sum(p => p.ForecastedHours);
            double totalActualHours = reportData.Sum(p => p.TotalHours);
            double totalBillableHoursVariance = reportData.Sum(p => p.BillableHoursVariance);

            int noOfEmployees = reportData.Length;
            var billablePercent = 0;
            var nonBillablePercent = 0;
            if (billableHours != 0 || nonBillableHours != 0)
            {
                billablePercent = DataTransferObjects.Utils.Generic.GetBillablePercentage(billableHours, nonBillableHours);
                nonBillablePercent = (100 - billablePercent);
            }
            ltProjectCount.Text = noOfEmployees + " Projects";
            lbRange.Text = HostingPage.Range;
            ltrlTotalHours.Text = lblTotalActualHours.Text = totalActualHours.ToString(Constants.Formatting.DoubleValue);
            ltrlAvgHours.Text = noOfEmployees > 0 ? ((billableHours + nonBillableHours) / noOfEmployees).ToString(Constants.Formatting.DoubleValue) : "0.00";
            ltrlBillableHours.Text = lblTotalBillableHours.Text = lblTotalBillablePanlActual.Text = billableHours.ToString(Constants.Formatting.DoubleValue);
            ltrlNonBillableHours.Text = lblTotalNonBillableHours.Text = lblTotalNonBillablePanlActual.Text = nonBillableHours.ToString(Constants.Formatting.DoubleValue);
            ltrlBillablePercent.Text = billablePercent.ToString();
            ltrlNonBillablePercent.Text = nonBillablePercent.ToString();
            lblTotalProjectedHours.Text = totalProjectedHours.ToString(Constants.Formatting.DoubleValue);
            lblTotalBillableHoursVariance.Text = totalBillableHoursVariance.ToString(Constants.Formatting.DoubleValue);
            if (totalBillableHoursVariance < 0)
            {
                lblExclamationMarkPanl.Visible = true;
            }

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

        protected void btnFilterOK_OnClick(object sender, EventArgs e)
        {
            PopulateByProjectData(false);
        }

    }
}

