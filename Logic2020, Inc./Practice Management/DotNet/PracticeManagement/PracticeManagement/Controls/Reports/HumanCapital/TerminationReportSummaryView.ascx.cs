using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DataTransferObjects;
using DataTransferObjects.Reports.HumanCapital;
using PraticeManagement.Utils.Excel;
using PraticeManagement.Utils;
using System.Data;

namespace PraticeManagement.Controls.Reports.HumanCapital
{
    public partial class TerminationReportSummaryView : System.Web.UI.UserControl
    {

        #region Properties

        private int coloumnsCount = 1;
        private int headerRowsCount = 1;
        private string TerminationReportExport = "Termination Report";

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
                sheetStyle.MergeRegion.Add(new int[] { 1, 1, 0, coloumnsCount - 1 });
                sheetStyle.MergeRegion.Add(new int[] { 2, 2, 0, coloumnsCount - 1 });
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
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                   dataDateCellStyle,
                                                   dataDateCellStyle,
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

        private HtmlImage ImgTitleFilter { get; set; }

        private HtmlImage ImgPayTypeFilter { get; set; }

        private HtmlImage ImgHiredateFilter { get; set; }

        private HtmlImage ImgPersonStatusTypeFilter { get; set; }

        private HtmlImage ImgDivisionFilter { get; set; }

        private HtmlImage ImgRecruiterFilter { get; set; }

        private HtmlImage ImgTerminationDateFilter { get; set; }

        private HtmlImage ImgTerminationReasonFilter { get; set; }

        private Label ImgTerminationReasonFilterHidden { get; set; }

        public Button BtnExportToExcelButton { get { return btnExportToExcel; } }

        public TerminationPersonsInRange PopUpFilteredPerson { get; set; }

        private PraticeManagement.Reporting.TerminationReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.TerminationReport)Page); }
        }

        private string PayTypes
        {
            get
            {
                return cblPayTypes.SelectedItemsXmlFormat != null ? cblPayTypes.SelectedItemsXmlFormat : HostingPage.PayTypes;
            }
        }

        private string Titles
        {
            get
            {
                return cblTitles.SelectedItemsXmlFormat != null ? cblTitles.SelectedItemsXmlFormat : HostingPage.Titles;
            }
        }

        private string TerminationReasons
        {
            get
            {
                return cblTerminationReason.SelectedItemsXmlFormat != null ? cblTerminationReason.SelectedItemsXmlFormat : HostingPage.TerminationReasons;
            }
        }

        #endregion

        #region PageEvents

        protected void Page_Load(object sender, EventArgs e)
        {
            cblRecruiter.OKButtonId = cblHireDate.OKButtonId = cblTitles.OKButtonId = cblPayTypes.OKButtonId = cblPersonStatusType.OKButtonId = cblDivision.OKButtonId = cblTerminationReason.OKButtonId = cblTerminationDate.OKButtonId = btnFilterOK.ClientID;
        }

        #endregion

        #region ControlEvents

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            var filename = string.Format("{0}_{1}-{2}.xls", "TerminationReport", HostingPage.StartDate.Value.ToString("MM.dd.yyyy"), HostingPage.EndDate.Value.ToString("MM.dd.yyyy"));
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();
            var btn = sender as Button;
            bool isGraphViewPopUp = false, generateExcel = false;
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            Boolean.TryParse(btn.Attributes["IsGraphViewPopUp"], out isGraphViewPopUp);
            generateExcel = isGraphViewPopUp && DateTime.TryParse(btn.Attributes["startDate"], out startDate) && DateTime.TryParse(btn.Attributes["endDate"], out endDate);

            if (!generateExcel && HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                startDate = HostingPage.StartDate.Value;
                endDate = HostingPage.EndDate.Value;
                generateExcel = true;
            }
            if (generateExcel)
            {
                List<Person> report;
                if (isGraphViewPopUp)
                {
                    report = ServiceCallers.Custom.Report(r => r.TerminationReport(startDate, endDate, HostingPage.PayTypes, null, null, null, null, false, null, null, null, null)).PersonList;
                }
                else
                {
                    report = ServiceCallers.Custom.Report(r => r.TerminationReport(startDate, endDate, HostingPage.PayTypes, null, HostingPage.Titles, HostingPage.TerminationReasons, HostingPage.Practices, HostingPage.ExcludeInternalProjects, null, null, null, null)).PersonList;
                }
                
                report = report.OrderBy(p => p.PersonLastFirstName).ToList();
                DataHelper.InsertExportActivityLogMessage(TerminationReportExport);
                if (report.Count > 0)
                {
                    DataTable header1 = new DataTable();
                    header1.Columns.Add("Termination Report");
                    header1.Rows.Add(report.Count + " Terminations");
                    header1.Rows.Add(isGraphViewPopUp ? startDate.ToString("MMM yyyy") : HostingPage.Range);

                    headerRowsCount = header1.Rows.Count + 3;

                    var data = PrepareDataTable(report);
                    coloumnsCount = data.Columns.Count;
                    sheetStylesList.Add(HeaderSheetStyle);
                    sheetStylesList.Add(DataSheetStyle);
                    var dataset = new DataSet();
                    dataset.DataSetName = "TerminationReport";
                    dataset.Tables.Add(header1);
                    dataset.Tables.Add(data);
                    dataSetList.Add(dataset);
                }
                else
                {
                    string dateRangeTitle = "There are no Person Terminations for the selected range.";
                    DataTable header = new DataTable();
                    header.Columns.Add(dateRangeTitle);
                    sheetStylesList.Add(HeaderSheetStyle);
                    var dataset = new DataSet();
                    dataset.DataSetName = "TerminationReport";
                    dataset.Tables.Add(header);
                    dataSetList.Add(dataset);
                }

                NPOIExcel.Export(filename, dataSetList, sheetStylesList);   
            }
        }

        public DataTable PrepareDataTable(List<Person> reportData)
        {
            DataTable data = new DataTable();
            List<object> rownew;
            List<object> row;

            data.Columns.Add("Employee Id");
            data.Columns.Add("Resource");
            data.Columns.Add("Title");
            data.Columns.Add("Pay Types");
            data.Columns.Add("Status");
            data.Columns.Add("Recruiter");
            data.Columns.Add("Hire Date");
            data.Columns.Add("Termination Date");
            data.Columns.Add("Termination Reason");

            foreach (var person in reportData)
            {

                row = new List<object>();
                row.Add(person.EmployeeNumber);
                row.Add(person.HtmlEncodedName);
                row.Add(person.Title != null ? person.Title.HtmlEncodedTitleName : string.Empty);
                row.Add(person.CurrentPay != null ? person.CurrentPay.TimescaleName : string.Empty);
                row.Add(person.Status.Name);
                row.Add(person.RecruiterId.HasValue ? person.RecruiterLastFirstName : string.Empty);
                row.Add(person.HireDate);
                row.Add(person.TerminationDate);
                row.Add(person.TerminationReason);
                
                data.Rows.Add(row.ToArray());
            }
            return data;
        }

        protected void repResource_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                ImgDivisionFilter = e.Item.FindControl("imgDivisionFilter") as HtmlImage;
                ImgTitleFilter = e.Item.FindControl("imgTitleFilter") as HtmlImage;
                ImgPayTypeFilter = e.Item.FindControl("imgPayTypeFilter") as HtmlImage;
                ImgHiredateFilter = e.Item.FindControl("imgHiredateFilter") as HtmlImage;
                ImgPersonStatusTypeFilter = e.Item.FindControl("imgPersonStatusTypeFilter") as HtmlImage;
                ImgRecruiterFilter = e.Item.FindControl("imgRecruiterFilter") as HtmlImage;
                ImgTerminationDateFilter = e.Item.FindControl("imgTerminationdateFilter") as HtmlImage;
                ImgTerminationReasonFilter = e.Item.FindControl("imgTerminationReasonFilter") as HtmlImage;
                ImgTerminationReasonFilterHidden = e.Item.FindControl("ImgTerminationReasonFilterHidden") as Label;
            }
        }

        protected void btnFilterOK_OnClick(object sender, EventArgs e)
        {
            PopulateData();
        }

        #endregion

        #region Methods

        protected string GetDateFormat(DateTime date)
        {
            return date.ToString(Constants.Formatting.EntryDateFormat);
        }

        public void PopulateData(bool isPopUp = false)
        {
            TerminationPersonsInRange data;
            if (!isPopUp)
            {
                if (HostingPage.SetSelectedFilters)
                {
                    data = ServiceCallers.Custom.Report(r => r.TerminationReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.PayTypes, null, HostingPage.Titles, HostingPage.TerminationReasons, HostingPage.Practices, HostingPage.ExcludeInternalProjects, null, null, null, null));
                    PopulateFilterPanels(data.PersonList);
                }
                else
                {
                    data = ServiceCallers.Custom.Report(r => r.TerminationReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, PayTypes, cblPersonStatusType.SelectedItemsXmlFormat, Titles, TerminationReasons, HostingPage.Practices, HostingPage.ExcludeInternalProjects, cblDivision.SelectedItemsXmlFormat, cblRecruiter.SelectedItemsXmlFormat, cblHireDate.SelectedItemsXmlFormat, cblTerminationDate.SelectedItemsXmlFormat));
                }
            }
            else
            {
                data = PopUpFilteredPerson;
            }
            DataBindResource(data, isPopUp);
        }

        private void RemoveFilters()
        {
            ImgTitleFilter.Visible =
            ImgPayTypeFilter.Visible =
            ImgHiredateFilter.Visible =
            ImgDivisionFilter.Visible =
            ImgPersonStatusTypeFilter.Visible =
            ImgRecruiterFilter.Visible =
            ImgTerminationDateFilter.Visible =
        ImgTerminationReasonFilterHidden.Visible =
            ImgTerminationReasonFilter.Visible = false;
        }

        public void DataBindResource(TerminationPersonsInRange reportData, bool isPopUp)
        {
            var reportDataList = reportData.PersonList.ToList();
            if (reportDataList.Count > 0 || cblTitles.Items.Count > 1 || cblPayTypes.Items.Count > 1 || cblHireDate.Items.Count > 1 || cblDivision.Items.Count > 1 || cblPersonStatusType.Items.Count > 1 || cblRecruiter.Items.Count > 1 || cblTerminationDate.Items.Count > 1 || cblTerminationReason.Items.Count > 1)
            {
                divEmptyMessage.Attributes["class"] = "displayNone";
                btnExportToExcel.Enabled =
                repResource.Visible = true;
                repResource.DataSource = reportDataList;
                repResource.DataBind();
                if (!isPopUp)
                {
                    SetAttribitesForFiltersImages();
                }
                else
                {
                    RemoveFilters();
                }
            }
            else
            {
                divEmptyMessage.Attributes["class"] = "EmptyMessagediv";
                btnExportToExcel.Enabled =
                repResource.Visible = false;
            }
            if (!isPopUp)
            {
                HostingPage.PopulateHeaderSection(reportData);
            }
        }

        private void SetAttribitesForFiltersImages()
        {
            cblTitles.SaveSelectedIndexesInViewState();
            cblPayTypes.SaveSelectedIndexesInViewState();
            cblRecruiter.SaveSelectedIndexesInViewState();
            cblDivision.SaveSelectedIndexesInViewState();
            cblHireDate.SaveSelectedIndexesInViewState();
            cblPersonStatusType.SaveSelectedIndexesInViewState();
            cblTerminationDate.SaveSelectedIndexesInViewState();
            cblTerminationReason.SaveSelectedIndexesInViewState();

            ImgTitleFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblTitles.FilterPopupClientID,
              cblTitles.SelectedIndexes, cblTitles.CheckBoxListObject.ClientID, cblTitles.WaterMarkTextBoxBehaviorID);

            ImgPayTypeFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblPayTypes.FilterPopupClientID,
               cblPayTypes.SelectedIndexes, cblPayTypes.CheckBoxListObject.ClientID, cblPayTypes.WaterMarkTextBoxBehaviorID);

            ImgHiredateFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblHireDate.FilterPopupClientID,
               cblHireDate.SelectedIndexes, cblHireDate.CheckBoxListObject.ClientID, cblHireDate.WaterMarkTextBoxBehaviorID);

            ImgDivisionFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblDivision.FilterPopupClientID,
               cblDivision.SelectedIndexes, cblDivision.CheckBoxListObject.ClientID, cblDivision.WaterMarkTextBoxBehaviorID);

            ImgPersonStatusTypeFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblPersonStatusType.FilterPopupClientID,
               cblPersonStatusType.SelectedIndexes, cblPersonStatusType.CheckBoxListObject.ClientID, cblPersonStatusType.WaterMarkTextBoxBehaviorID);

            ImgRecruiterFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblRecruiter.FilterPopupClientID,
              cblRecruiter.SelectedIndexes, cblRecruiter.CheckBoxListObject.ClientID, cblRecruiter.WaterMarkTextBoxBehaviorID);

            ImgTerminationDateFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblTerminationDate.FilterPopupClientID,
             cblTerminationDate.SelectedIndexes, cblTerminationDate.CheckBoxListObject.ClientID, cblTerminationDate.WaterMarkTextBoxBehaviorID);

            ImgTerminationReasonFilter.Attributes["onclick"] = string.Format("ClickHiddenImg(\'{4}\');Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblTerminationReason.FilterPopupClientID,
             cblTerminationReason.SelectedIndexes, cblTerminationReason.CheckBoxListObject.ClientID, cblTerminationReason.WaterMarkTextBoxBehaviorID, ImgTerminationReasonFilterHidden.ClientID);
        }

        private void PopulateFilterPanels(List<Person> reportData)
        {
            PopulateDivisionFilter(reportData);
            PopulateTitleFilter(reportData);
            PopulateHireDateFilter(reportData);
            PopulatePayTypeFilter(reportData);
            PopulatPersonStatusTypeFilter(reportData);
            PopulateRecruiterFilter(reportData);
            PopulateTerminationDateFilter(reportData);
            PopulateTerminationReasonFilter(reportData);
        }

        private void PopulateTerminationDateFilter(List<Person> reportData)
        {
            var terminationDateList = reportData.Select(r => new { Text = r.TerminationDate.Value.ToString("MMM yyyy"), Value = r.TerminationDate.Value.ToString("MM/01/yyyy"), orderby = r.TerminationDate.Value.ToString("yyyy/MM") }).Distinct().ToList().OrderBy(s => s.orderby);
            DataHelper.FillListDefault(cblTerminationDate.CheckBoxListObject, "All Months ", terminationDateList.ToArray(), false, "Value", "Text");
            cblTerminationDate.SelectAllItems(true);
        }

        private void PopulateTerminationReasonFilter(List<Person> reportData)
        {
            var terminationReasons = reportData.Select(r => new { Id = r.TerminationReasonid, Name = r.TerminationReason }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblTerminationReason.CheckBoxListObject, "All Reasons", terminationReasons.ToArray(), false, "Id", "Name");
            cblTerminationReason.SelectAllItems(true);
        }

        private void PopulateDivisionFilter(List<Person> reportData)
        {
            var divisionList = reportData.Select(r => new { Value = (int)r.DivisionType == 0 ? string.Empty : ((int)r.DivisionType).ToString(), Name = (int)r.DivisionType == 0 ? Constants.FilterKeys.Unassigned : r.DivisionType.ToString() }).Distinct().ToList().OrderBy(d => d.Value).ToArray();
            DataHelper.FillListDefault(cblDivision.CheckBoxListObject, "All Division Types", divisionList, false, "Value", "Name");
            cblDivision.SelectAllItems(true);
        }

        private void PopulateHireDateFilter(List<Person> reportData)
        {
            var hireDateList = reportData.Select(r => new { Text = r.HireDate.ToString("MMM yyyy"), Value = r.HireDate.ToString("MM/01/yyyy"), orderby = r.HireDate.ToString("yyyy/MM") }).Distinct().ToList().OrderBy(s => s.orderby);
            DataHelper.FillListDefault(cblHireDate.CheckBoxListObject, "All Months ", hireDateList.ToArray(), false, "Value", "Text");
            cblHireDate.SelectAllItems(true);
        }

        private void PopulatPersonStatusTypeFilter(List<Person> reportData)
        {
            var personStatusTypeList = reportData.Select(r => new { Name = r.Status.Name, Id = r.Status.Id }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblPersonStatusType.CheckBoxListObject, "All Person Status Types", personStatusTypeList.ToArray(), false, "Id", "Name");
            cblPersonStatusType.SelectAllItems(true);
        }

        private void PopulateTitleFilter(List<Person> reportData)
        {
            var titles = reportData.Select(r => new { TitleId = r.Title != null ? r.Title.TitleId : 0, TitleName = r.Title != null ? r.Title.HtmlEncodedTitleName : Constants.FilterKeys.Unassigned }).Distinct().ToList().OrderBy(s => s.TitleName);
            DataHelper.FillListDefault(cblTitles.CheckBoxListObject, "All Titles", titles.ToArray(), false, "TitleId", "TitleName");
            cblTitles.SelectAllItems(true);
        }

        private void PopulatePayTypeFilter(List<Person> reportData)
        {
            var payTypes = reportData.Select(r => new { Text = r.CurrentPay == null || string.IsNullOrEmpty(r.CurrentPay.TimescaleName) ? Constants.FilterKeys.Unassigned : r.CurrentPay.TimescaleName, Value = r.CurrentPay == null || string.IsNullOrEmpty(r.CurrentPay.TimescaleName) ? 0 : (int)r.CurrentPay.Timescale }).Distinct().ToList().OrderBy(t => t.Text);
            DataHelper.FillListDefault(cblPayTypes.CheckBoxListObject, "All Pay Types", payTypes.ToArray(), false, "Value", "Text");
            cblPayTypes.SelectAllItems(true);
        }

        private void PopulateRecruiterFilter(List<Person> reportData)
        {
            var recruiters = reportData.Select(r => new { Text = r.RecruiterId.HasValue ? r.RecruiterLastFirstName : Constants.FilterKeys.Unassigned, Value = r.RecruiterId.HasValue ? r.RecruiterId.Value : 0 }).Distinct().ToList().OrderBy(t => t.Text);
            DataHelper.FillListDefault(cblRecruiter.CheckBoxListObject, "All Recruiter(s)", recruiters.ToArray(), false, "Value", "Text");
            cblRecruiter.SelectAllItems(true);
        }

        #endregion
    }
}

