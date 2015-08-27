using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using DataTransferObjects.Reports;
using PraticeManagement.Utils;
using PraticeManagement.Utils.Excel;

namespace PraticeManagement.Controls.Reports
{
    public partial class TimePeriodSummaryByResource : System.Web.UI.UserControl
    {
        private string TimePeriodSummaryReportExport = "TimePeriod Summary Report By Resource";

        private string TimePeriodSummaryReportPayCheckExport = "TimePeriod Summary Report By Resource(ADP)";

        private string ShowPanel = "ShowPanel('{0}', '{1}','{2}');";
        private string HidePanel = "HidePanel('{0}');";
        private string OnMouseOver = "onmouseover";
        private string OnMouseOut = "onmouseout";
        private string ByPersonByResourceUrl = "PersonDetailTimeReport.aspx?StartDate={0}&EndDate={1}&PeriodSelected={2}&PersonId={3}";
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
                CellStyles dataPercentageCellStyle = new CellStyles();
                dataPercentageCellStyle.DataFormat = "0.00%";

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
                                                    dataPercentageCellStyle
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

        private SheetStyles DataSheetStyleForADP
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
                CellStyles dataDecimalCellStyle = new CellStyles();
                dataDecimalCellStyle.DataFormat = "0.00";
                bool isUserAdminstrator = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
                var dataCellStylearray = new List<CellStyles>() {dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle };
                if (isUserAdminstrator)
                    dataCellStylearray.Add(dataCellStyle);
                dataCellStylearray.Add(dataDecimalCellStyle);
                for (int i = 0; i < WorkTypeCount; i++)
                    dataCellStylearray.Add(dataDecimalCellStyle);
                dataCellStylearray.Add(dataDecimalCellStyle);
                

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

        public ModalPopupExtender PersonDetailPopup
        {
            get
            {
                return mpePersonDetailReport;
            }
        }

        public int SelectedPersonForDetails
        {
            get
            {
                return (int)ViewState["SelectedPersonForDetail"];
            }
            set
            {
                ViewState["SelectedPersonForDetail"] = value;
            }
        }

        private HtmlImage ImgSeniorityFilter { get; set; }

        private HtmlImage ImgPayTypeFilter { get; set; }

        private HtmlImage ImgOffshoreFilter { get; set; }

        private HtmlImage ImgPersonStatusTypeFilter { get; set; }

        private HtmlImage ImgDivisionFilter { get; set; }

        private Label LblTotalBillable { get; set; }

        private Label LblTotalNonBillable { get; set; }

        private Label LblTotalBD { get; set; }

        private Label LblTotalInternal { get; set; }

        private Label LblTotalTimeOff { get; set; }

        private Label LblTotalHours { get; set; }

        private Label LblAvailableHours { get; set; }

        private Label LblHeaderBillableUtilization { get; set; }

        private int WorkTypeCount { get; set; }

        private PraticeManagement.Reporting.TimePeriodSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.TimePeriodSummaryReport)Page); }
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected string GetNumberFormatWithCommas(double value)
        {
            return value.ToString(Constants.Formatting.NumberFormatWithCommas);
        }

        protected string GetPercentageFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue) + "%";
        }

        protected string GetPercentageFormatForExcel(double value)
        {
            var result = (double)(value / 100);
            return result.ToString();
        }

        protected string GetPayTypeSortValue(string payType, string name)
        {
            if (string.IsNullOrEmpty(payType))
            {
                return "-1" + name;
            }
            return payType + name;
        }

        protected bool IsPersonTerminated(int statusId)
        {
            return statusId == 2;//Terminated
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(TimePeriodSummaryReportExport);
            var filename = string.Format("{0}_{1}-{2}.xls", "TimePeriod_ByResource", HostingPage.StartDate.Value.ToString("MM.dd.yyyy"), HostingPage.EndDate.Value.ToString("MM.dd.yyyy"));
            var dataSetList = new List<DataSet>();
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                var report = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryReportByResource(HostingPage.StartDate.Value,
                   HostingPage.EndDate.Value, HostingPage.IncludePersonWithNoTimeEntries, cblOffShore.SelectedItemsXmlFormat,
                   cblSeniorities.SelectedItems,
                   cblPayTypes.SelectedItemsXmlFormat, cblPersonStatusType.SelectedItems, cblDivision.SelectedItemsXmlFormat)).ToList();

                string filterApplied = "Filters applied to columns: ";
                List<string> filteredColoums = new List<string>();
                if (!cblOffShore.AllItemsSelected || !cblPersonStatusType.AllItemsSelected || !cblDivision.AllItemsSelected)
                {
                    filteredColoums.Add("Resource");
                }
                if (!cblSeniorities.AllItemsSelected)
                {
                    filteredColoums.Add("Title");
                }
                if (!cblPayTypes.AllItemsSelected)
                {
                    filteredColoums.Add("Pay Type");
                }

                if (report.Count > 0)
                {
                    DataTable header1 = new DataTable();
                    header1.Columns.Add("Time Period By Resource Report");
                    header1.Rows.Add(report.Count + " Employees");
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
                    dataset.DataSetName = "TimePeriod_ByResource";
                    dataset.Tables.Add(header1);
                    dataset.Tables.Add(data);
                    dataSetList.Add(dataset);
                }
                else
                {
                    string dateRangeTitle = "There are no Time Entries by any Employee  for the selected range.";
                    DataTable header = new DataTable();
                    header.Columns.Add(dateRangeTitle);
                    sheetStylesList.Add(HeaderSheetStyle);
                    var dataset = new DataSet();
                    dataset.DataSetName = "TimePeriod_ByResource";
                    dataset.Tables.Add(header);
                    dataSetList.Add(dataset);
                }
                NPOIExcel.Export(filename, dataSetList, sheetStylesList);
            }
        }

        public DataTable PrepareDataTable(List<PersonLevelGroupedHours> report)
        {
            DataTable data = new DataTable();
            List<object> rownew;
            List<object> row;

            data.Columns.Add("Employee Id");
            data.Columns.Add("Resource");
            data.Columns.Add("Title");
            data.Columns.Add("Pay Types");
            data.Columns.Add("Is Offshore");
            data.Columns.Add("Billable");
            data.Columns.Add("Non-Billable");
            data.Columns.Add("BD");
            data.Columns.Add("Internal");
            data.Columns.Add("Time-Off");
            data.Columns.Add("Actual Hours");
            data.Columns.Add("Available Hours");
            data.Columns.Add("Billable Utilization");

            foreach (var item in report)
            {
                row = new List<object>();
                row.Add(item.Person.EmployeeNumber != null ? item.Person.EmployeeNumber : "");
                row.Add(item.Person.HtmlEncodedName != null ? item.Person.HtmlEncodedName : "");
                row.Add((item.Person.Title.HtmlEncodedTitleName != null) ? item.Person.Title.HtmlEncodedTitleName : "");
                row.Add((item.Person.CurrentPay.TimescaleName != null) ? item.Person.CurrentPay.TimescaleName : "");
                row.Add(item.Person.IsOffshore ? "Yes" : "No");
                row.Add(GetDoubleFormat(item.BillableHours));
                row.Add(GetDoubleFormat(item.ProjectNonBillableHours));
                row.Add(GetDoubleFormat(item.BusinessDevelopmentHours));
                row.Add(GetDoubleFormat(item.InternalHours));
                row.Add(GetDoubleFormat(item.AdminstrativeHours));
                row.Add(GetDoubleFormat(item.TotalHours));
                row.Add(GetDoubleFormat(item.AvailableHours));
                row.Add(GetPercentageFormatForExcel(item.Person.BillableUtilizationPercent));
                data.Rows.Add(row.ToArray());
            }
            return data;
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {
        }

        protected void btnPayCheckExport_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(TimePeriodSummaryReportPayCheckExport);
            var dataSetList = new List<DataSet>();
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                List<PersonLevelPayCheck> personLevelPayCheckList = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryByResourcePayCheck(HostingPage.StartDate.Value, HostingPage.EndDate.Value,
                    HostingPage.IncludePersonWithNoTimeEntries, cblOffShore.SelectedItemsXmlFormat, cblSeniorities.SelectedItems, cblPayTypes.SelectedItemsXmlFormat, cblPersonStatusType.SelectedItems, cblDivision.SelectedItemsXmlFormat)).ToList();

                string filterApplied = "Filters applied to columns: ";
                List<string> filteredColoums = new List<string>();
                if (!cblOffShore.AllItemsSelected || !cblPersonStatusType.AllItemsSelected || !cblDivision.AllItemsSelected)
                {
                    filteredColoums.Add("Resource");
                }
                if (!cblSeniorities.AllItemsSelected)
                {
                    filteredColoums.Add("Title");
                }
                if (!cblPayTypes.AllItemsSelected)
                {
                    filteredColoums.Add("Pay Type");
                }
                if (personLevelPayCheckList.Count > 0)
                {
                    DataTable header1 = new DataTable();
                    header1.Columns.Add("ADP");
                    header1.Rows.Add(personLevelPayCheckList.Count.ToString() + " Employees");
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

                    WorkTypeCount = personLevelPayCheckList[0].WorkTypeLevelTimeOffHours.Keys.Count;
                    headerRowsCount = header1.Rows.Count + 3;
                    var data = PrepareDataTableForADP(personLevelPayCheckList);
                    coloumnsCount = data.Columns.Count;
                    sheetStylesList.Add(HeaderSheetStyle);
                    sheetStylesList.Add(DataSheetStyleForADP);
                    var dataset = new DataSet();
                    dataset.DataSetName = "TimePeriod_ByResource_ADP";
                    dataset.Tables.Add(header1);
                    dataset.Tables.Add(data);
                    dataSetList.Add(dataset);
                }
                else
                {
                    string dateRangeTitle = "There are no Time Entries by any Employee  for the selected range.";
                    DataTable header = new DataTable();
                    header.Columns.Add(dateRangeTitle);
                    sheetStylesList.Add(HeaderSheetStyle);
                    var dataset = new DataSet();
                    dataset.DataSetName = "TimePeriod_ByResource_ADP";
                    dataset.Tables.Add(header);
                    dataSetList.Add(dataset);
                }

                //“ADP_[StartOfRange]_[EndOfRange].xls”.
                var filename = string.Format("{0}_{1}-{2}.xls", "ADP", HostingPage.StartDate.Value.ToString("MM.dd.yyyy"), HostingPage.EndDate.Value.ToString("MM.dd.yyyy"));
                NPOIExcel.Export(filename, dataSetList, sheetStylesList);

            }
        }

        public DataTable PrepareDataTableForADP(List<PersonLevelPayCheck> report)
        {
            DataTable data = new DataTable();
            List<object> rownew;
            List<object> row;

            bool isUserAdminstrator = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
            Dictionary<string, double> workTypeLevelTimeOffHours = report[0].WorkTypeLevelTimeOffHours;
            data.Columns.Add("Branch ID");
            data.Columns.Add("Dept ID");
            data.Columns.Add("Employee ID");
            if (isUserAdminstrator)
                data.Columns.Add("ADP ID");
            data.Columns.Add("Last Name");
            data.Columns.Add("First Name");
            data.Columns.Add("Pay Types");
            data.Columns.Add("Hours");
            foreach (string worktype in workTypeLevelTimeOffHours.Keys)
            {
                data.Columns.Add(worktype);
            }
            data.Columns.Add("Total");

            foreach (var item in report)
            {
                row = new List<object>();
                row.Add(item.BranchID);
                row.Add(item.DeptID);
                row.Add(item.Person.EmployeeNumber);
                if (isUserAdminstrator)
                    row.Add(item.Person.PaychexID);
                row.Add(item.Person.LastName);
                row.Add(item.Person.FirstName);
                row.Add(item.Person.CurrentPay.TimescaleName);
                row.Add(item.TotalHoursExcludingTimeOff);
                foreach (string worktype in workTypeLevelTimeOffHours.Keys)
                    row.Add(item.WorkTypeLevelTimeOffHours[worktype]);
                row.Add(item.TotalHoursIncludingTimeOff);
                data.Rows.Add(row.ToArray());
            }
            return data;
        }

        protected void btnFilterOK_OnClick(object sender, EventArgs e)
        {
            PopulateByResourceData(false);
        }

        protected void lnkPerson_OnClick(object sender, EventArgs e)
        {
            var lnkPerson = sender as LinkButton;
            var personId = Convert.ToInt32(lnkPerson.Attributes["PersonId"]);
            SelectedPersonForDetails = personId;
            var list = ServiceCallers.Custom.Report(r => r.PersonTimeEntriesDetails(personId, HostingPage.StartDate.Value, HostingPage.EndDate.Value)).ToList();
            ucPersonDetailReport.DatabindRepepeaterPersonDetails(list, lnkPerson.Text);

            mpePersonDetailReport.Show();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            cblOffShore.OKButtonId = cblSeniorities.OKButtonId = cblPayTypes.OKButtonId = cblPersonStatusType.OKButtonId = cblDivision.OKButtonId = btnFilterOK.ClientID;
        }

        protected void repResource_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                ImgSeniorityFilter = e.Item.FindControl("imgSeniorityFilter") as HtmlImage;
                ImgPayTypeFilter = e.Item.FindControl("imgPayTypeFilter") as HtmlImage;
                ImgOffshoreFilter = e.Item.FindControl("imgOffShoreFilter") as HtmlImage;
                ImgPersonStatusTypeFilter = e.Item.FindControl("imgPersonStatusTypeFilter") as HtmlImage;
                ImgDivisionFilter = e.Item.FindControl("imgDivisionFilter") as HtmlImage;

                LblTotalBillable = e.Item.FindControl("lblTotalBillable") as Label;
                LblTotalNonBillable = e.Item.FindControl("lblTotalNonBillable") as Label;
                LblTotalBD = e.Item.FindControl("lblTotalBD") as Label;
                LblTotalInternal = e.Item.FindControl("lblTotalInternal") as Label;
                LblTotalTimeOff = e.Item.FindControl("lblTotalTimeOff") as Label;
                LblTotalHours = e.Item.FindControl("lblTotalHours") as Label;
                LblAvailableHours = e.Item.FindControl("lblAvailableHours") as Label;
                LblHeaderBillableUtilization = e.Item.FindControl("lblHeaderBillableUtilization") as Label;
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = (PersonLevelGroupedHours)e.Item.DataItem;
                var lnkPerson = e.Item.FindControl("lnkPerson") as LinkButton;
                var imgZoomIn = e.Item.FindControl("imgZoomIn") as HtmlImage;
                var LnkBillableUtilizationPercentage = e.Item.FindControl("lnkBillableUtilizationPercentage") as LinkButton;

                lnkPerson.Attributes["onmouseover"] = string.Format("document.getElementById(\'{0}\').style.visibility='visible';", imgZoomIn.ClientID);
                lnkPerson.Attributes["onmouseout"] = string.Format("document.getElementById(\'{0}\').style.visibility='hidden';", imgZoomIn.ClientID);
                //Respective Person's Id,Person's Name will be added from client side to below path when click on the link button.
                LnkBillableUtilizationPercentage.Attributes["NavigationUrl"] = string.Format(ByPersonByResourceUrl, HostingPage.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat),
                    HostingPage.EndDate.Value.Date.ToString(Constants.Formatting.EntryDateFormat), HostingPage.RangeSelected, dataItem.Person.Id);
            }
        }

        public void PopulateByResourceData(bool isPopulateFilters = true)
        {
            PersonLevelGroupedHours[] data;
            if (isPopulateFilters)
            {
                data = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryReportByResource(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.IncludePersonWithNoTimeEntries, null, null, null, null, null));
            }
            else
            {
                data = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryReportByResource(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.IncludePersonWithNoTimeEntries, cblOffShore.SelectedItemsXmlFormat, cblSeniorities.SelectedItems, cblPayTypes.SelectedItemsXmlFormat, cblPersonStatusType.SelectedItems, cblDivision.SelectedItemsXmlFormat));
            }

            DataBindResource(data, isPopulateFilters);
        }

        public void DataBindResource(PersonLevelGroupedHours[] reportData, bool isPopulateFilters)
        {
            var reportDataList = reportData.ToList();
            if (isPopulateFilters)
            {
                PopulateFilterPanels(reportDataList);
            }
            if (reportDataList.Count > 0 || cblSeniorities.Items.Count > 1 || cblPayTypes.Items.Count > 1)
            {
                divEmptyMessage.Style["display"] = "none";
                repResource.Visible = true;
                repResource.DataSource = reportDataList;
                repResource.DataBind();
                cblSeniorities.SaveSelectedIndexesInViewState();
                cblPayTypes.SaveSelectedIndexesInViewState();
                cblOffShore.SaveSelectedIndexesInViewState();
                cblDivision.SaveSelectedIndexesInViewState();
                cblPersonStatusType.SaveSelectedIndexesInViewState();
                ImgSeniorityFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblSeniorities.FilterPopupClientID,
                  cblSeniorities.SelectedIndexes, cblSeniorities.CheckBoxListObject.ClientID, cblSeniorities.WaterMarkTextBoxBehaviorID);

                ImgPayTypeFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblPayTypes.FilterPopupClientID,
                   cblPayTypes.SelectedIndexes, cblPayTypes.CheckBoxListObject.ClientID, cblPayTypes.WaterMarkTextBoxBehaviorID);

                ImgOffshoreFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblOffShore.FilterPopupClientID,
                   cblOffShore.SelectedIndexes, cblOffShore.CheckBoxListObject.ClientID, cblOffShore.WaterMarkTextBoxBehaviorID);

                ImgDivisionFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblDivision.FilterPopupClientID,
                   cblDivision.SelectedIndexes, cblDivision.CheckBoxListObject.ClientID, cblDivision.WaterMarkTextBoxBehaviorID);

                ImgPersonStatusTypeFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblPersonStatusType.FilterPopupClientID,
                   cblPersonStatusType.SelectedIndexes, cblPersonStatusType.CheckBoxListObject.ClientID, cblPersonStatusType.WaterMarkTextBoxBehaviorID);

                PopulateSumLabels(reportDataList);
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repResource.Visible = false;
            }

            PopulateHeaderSection(reportDataList);
        }

        private void PopulateSumLabels(List<PersonLevelGroupedHours> reportData)
        {
            LblTotalBillable.Attributes[OnMouseOver] = string.Format(ShowPanel, LblTotalBillable.ClientID, pnlTotalBillableHours.ClientID, 0);
            LblTotalBillable.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalBillableHours.ClientID);

            LblTotalNonBillable.Attributes[OnMouseOver] = string.Format(ShowPanel, LblTotalNonBillable.ClientID, pnlTotalNonBillableHours.ClientID, 0);
            LblTotalNonBillable.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalNonBillableHours.ClientID);

            LblTotalBD.Attributes[OnMouseOver] = string.Format(ShowPanel, LblTotalBD.ClientID, pnlTotalBD.ClientID, 0);
            LblTotalBD.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalBD.ClientID);

            LblTotalInternal.Attributes[OnMouseOver] = string.Format(ShowPanel, LblTotalInternal.ClientID, pnlTotalInternalHours.ClientID, 0);
            LblTotalInternal.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalInternalHours.ClientID);

            LblTotalTimeOff.Attributes[OnMouseOver] = string.Format(ShowPanel, LblTotalTimeOff.ClientID, pnlTotalTimeOffHours.ClientID, 0);
            LblTotalTimeOff.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalTimeOffHours.ClientID);

            LblTotalHours.Attributes[OnMouseOver] = string.Format(ShowPanel, LblTotalHours.ClientID, pnlTotalHours.ClientID, 0);
            LblTotalHours.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalHours.ClientID);

            LblAvailableHours.Attributes[OnMouseOver] = string.Format(ShowPanel, LblAvailableHours.ClientID, pnlTotalAvailableHours.ClientID, 0);
            LblAvailableHours.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalAvailableHours.ClientID);

            LblHeaderBillableUtilization.Attributes[OnMouseOver] = string.Format(ShowPanel, LblHeaderBillableUtilization.ClientID, pnlTotalBillableUtilization.ClientID, 0);
            LblHeaderBillableUtilization.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalBillableUtilization.ClientID);

            lblBillable.Text =
            pthLblBillable.Text = reportData.Sum(p => p.BillableHours).ToString(Constants.Formatting.DoubleValue);

            lblNonBillable.Text =
            pthLblNonBillable.Text = reportData.Sum(p => p.ProjectNonBillableHours).ToString(Constants.Formatting.DoubleValue);

            lblBD.Text =
            pthLblBD.Text = reportData.Sum(p => p.BusinessDevelopmentHours).ToString(Constants.Formatting.DoubleValue);

            lblInternal.Text =
            pthLblInternal.Text = reportData.Sum(p => p.InternalHours).ToString(Constants.Formatting.DoubleValue);

            lblTimeOff.Text =
            pthLblTimeOff.Text = reportData.Sum(p => p.AdminstrativeHours).ToString(Constants.Formatting.DoubleValue);

            pthLblGrandTotal.Text = reportData.Sum(p => p.TotalHours).ToString(Constants.Formatting.DoubleValue);

            lblPanelTotalAvailableHours.Text = reportData.Sum(p => p.AvailableHours).ToString(Constants.Formatting.NumberFormatWithCommas);
        }

        private void PopulateFilterPanels(List<PersonLevelGroupedHours> reportData)
        {
            PopulateOffshoreFilter(reportData);
            PopulateSeniorityFilter(reportData);
            PopulatePayTypeFilter(reportData);
            PopulatPersonStatusTypeFilter(reportData);
            PopulateDivisionFilter(reportData);
        }

        private void PopulateOffshoreFilter(List<PersonLevelGroupedHours> reportData)
        {
            var offshoreList = reportData.Select(r => new { Name = r.Person.OffshoreText }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblOffShore.CheckBoxListObject, "All Resource Types", offshoreList.ToArray(), false, "Name", "Name");
            cblOffShore.SelectAllItems(true);
        }

        private void PopulateDivisionFilter(List<PersonLevelGroupedHours> reportData)
        {
            var divisionList = reportData.Select(r => new { Value = (int)r.Person.DivisionType == 0 ? string.Empty : ((int)r.Person.DivisionType).ToString(), Name = (int)r.Person.DivisionType == 0 ? "Unassigned" : r.Person.DivisionType.ToString() }).Distinct().ToList().OrderBy(d => d.Value).ToArray();
            DataHelper.FillListDefault(cblDivision.CheckBoxListObject, "All Division Types", divisionList, false, "Value", "Name");
            cblDivision.SelectAllItems(true);
        }

        private void PopulatPersonStatusTypeFilter(List<PersonLevelGroupedHours> reportData)
        {
            var personStatusTypeList = reportData.Select(r => new { Name = r.Person.Status.Name, Id = r.Person.Status.Id }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblPersonStatusType.CheckBoxListObject, "All Person Status Types", personStatusTypeList.ToArray(), false, "Id", "Name");
            cblPersonStatusType.SelectAllItems(true);
        }

        private void PopulateSeniorityFilter(List<PersonLevelGroupedHours> reportData)
        {
            var titles = reportData.Select(r => new { Id = r.Person.Title.TitleId, Name = r.Person.Title.HtmlEncodedTitleName }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblSeniorities.CheckBoxListObject, "All Titles", titles.ToArray(), false, "Id", "Name");
            cblSeniorities.SelectAllItems(true);
        }

        private void PopulatePayTypeFilter(List<PersonLevelGroupedHours> reportData)
        {
            var payTypes = reportData.Select(r => new { Text = string.IsNullOrEmpty(r.Person.CurrentPay.TimescaleName) ? "Unassigned" : r.Person.CurrentPay.TimescaleName, Value = r.Person.CurrentPay.TimescaleName }).Distinct().ToList().OrderBy(t => t.Value);
            DataHelper.FillListDefault(cblPayTypes.CheckBoxListObject, "All Pay Types", payTypes.ToArray(), false, "Value", "Text");
            cblPayTypes.SelectAllItems(true);
        }

        private void PopulateHeaderSection(List<PersonLevelGroupedHours> reportData)
        {
            double billableHours = reportData.Sum(p => p.BillableHours);
            double nonBillableHours = reportData.Sum(p => p.NonBillableHours);
            double totalTimeOffHours = reportData.Sum(p => p.AdminstrativeHours);
            double pTOHours = reportData.Sum(p => p.PTOHours);
            double bereavementHours = reportData.Sum(p => p.BereavementHours);
            double juryDutyHours = reportData.Sum(p => p.JuryDutyHours);
            double oRTHours = reportData.Sum(p => p.ORTHours);
            double unpaidHours = reportData.Sum(p => p.UnpaidHours);
            double holidayHours = reportData.Sum(p => p.HolidayHours);
            double sickOrSafeLeaveHours = reportData.Sum(p => p.SickOrSafeLeaveHours);
            double totalAvailableHours = reportData.Sum(p => p.AvailableHoursUntilToday);
            double billableHoursUntilToday = reportData.Sum(p => p.BillableHoursUntilToday);

            int noOfEmployees = reportData.Count;
            var billablePercent = 0;
            var nonBillablePercent = 0;
            if (billableHours != 0 || nonBillableHours != 0)
            {
                billablePercent = DataTransferObjects.Utils.Generic.GetBillablePercentage(billableHours, nonBillableHours);
                nonBillablePercent = (100 - billablePercent);
            }
            ltPersonCount.Text = noOfEmployees + " Employees";
            lbRange.Text = HostingPage.Range;
            ltrlTotalHours.Text = (billableHours + nonBillableHours).ToString(Constants.Formatting.DoubleValue);
            ltrlTotalTimeoffHours.Text = totalTimeOffHours.ToString(Constants.Formatting.DoubleValue);
            ltrlAvgHours.Text = noOfEmployees > 0 ? ((billableHours + nonBillableHours) / noOfEmployees).ToString(Constants.Formatting.DoubleValue) : "0.00";
            lblTotalBillableUtilization.Text = lblBillableUtilization.Text = lblBillableUtilizationPercentage.Text = (totalAvailableHours > 0) ? (billableHoursUntilToday / totalAvailableHours).ToString(Constants.Formatting.DoubleValueWithPercent) : "0%";//noOfEmployees > 0 ? Math.Round((totalUtlization / noOfEmployees), 1).ToString() + "%" : "0%";
            ltrlBillableHours.Text = billableHours.ToString(Constants.Formatting.DoubleValue);
            lblTotalBillableHours.Text = lblTotalBillableHoursInBold.Text = billableHoursUntilToday.ToString(Constants.Formatting.NumberFormatWithCommas);
            ltrlNonBillableHours.Text = nonBillableHours.ToString(Constants.Formatting.DoubleValue);
            ltrlBillablePercent.Text = billablePercent.ToString();
            ltrlNonBillablePercent.Text = nonBillablePercent.ToString();
            lblTotalAvailableHours.Text = lblTotalAvailableHoursInBold.Text = totalAvailableHours.ToString(Constants.Formatting.NumberFormatWithCommas);

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

            lblPtoHours.Text = pTOHours.ToString(Constants.Formatting.DoubleValue);
            lblBereavementHours.Text = bereavementHours.ToString(Constants.Formatting.DoubleValue);
            lblJuryDutyHours.Text = juryDutyHours.ToString(Constants.Formatting.DoubleValue);
            lblOrtHours.Text = oRTHours.ToString(Constants.Formatting.DoubleValue);
            lblUnpaidHours.Text = unpaidHours.ToString(Constants.Formatting.DoubleValue);
            lblHolidayHours.Text = holidayHours.ToString(Constants.Formatting.DoubleValue);
            lblSickOrSafeLeaveHours.Text = sickOrSafeLeaveHours.ToString(Constants.Formatting.DoubleValue);

            ltrlTotalTimeoffHours.Attributes[OnMouseOver] = string.Format(ShowPanel, ltrlTotalTimeoffHours.ClientID, pnlTotalTimeOff.ClientID, 0);
            ltrlTotalTimeoffHours.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalTimeOff.ClientID);
            lblBillableUtilization.Attributes[OnMouseOver] = string.Format(ShowPanel, lblBillableUtilization.ClientID, pnlBillableUtilizationCalculation.ClientID, 50);
            lblBillableUtilization.Attributes[OnMouseOut] = string.Format(HidePanel, pnlBillableUtilizationCalculation.ClientID);
        }
    }
}
