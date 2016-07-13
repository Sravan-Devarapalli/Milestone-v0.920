using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.Utils.Excel;
using System.Data;
using PraticeManagement.Utils;
using PraticeManagement.Objects;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web.Security;
using DataTransferObjects.Filters;

namespace PraticeManagement.Reports
{
    public partial class ProjectsList : System.Web.UI.Page
    {
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;
        private const string projectsListExportText = "Projects List Report For the Period: {0} to {1}";
        private const string allCapabilitiesText = "All Capabilities";
        private const string projectsListFilterKey = "projectsListFilterKey";
        private string RowSpliter = Guid.NewGuid().ToString();

        private string ColoumSpliter = Guid.NewGuid().ToString();

        public DateTime StartDate
        {
            get
            {
                DateTime currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, Constants.Dates.FirstDay);
                var periodSelected = int.Parse(ddlPeriod.SelectedValue);

                if (periodSelected > 0)
                {
                    DateTime startMonth = new DateTime();
                    if (periodSelected < 13)
                    {
                        startMonth = currentMonth;
                    }
                    else
                    {
                        Dictionary<string, DateTime> fPeriod = DataHelper.GetFiscalYearPeriod(currentMonth);
                        startMonth = fPeriod["StartMonth"];
                    }
                    return startMonth;

                }
                else if (periodSelected < 0)
                {
                    DateTime startMonth = new DateTime();

                    if (periodSelected > -13)
                    {
                        startMonth = currentMonth.AddMonths(Convert.ToInt32(ddlPeriod.SelectedValue) + 1);
                    }
                    else
                    {
                        Dictionary<string, DateTime> fPeriod = DataHelper.GetFiscalYearPeriod(currentMonth.AddYears(-1));
                        startMonth = fPeriod["StartMonth"];
                    }
                    return startMonth;
                }
                else
                {
                    return diRange.FromDate.Value;
                }
            }
        }

        public DateTime EndDate
        {
            get
            {
                DateTime currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, Constants.Dates.FirstDay);
                var periodSelected = int.Parse(ddlPeriod.SelectedValue);

                if (periodSelected > 0)
                {
                    DateTime endMonth = new DateTime();

                    if (periodSelected < 13)
                    {
                        endMonth = currentMonth.AddMonths(Convert.ToInt32(ddlPeriod.SelectedValue) - 1);
                    }
                    else
                    {
                        Dictionary<string, DateTime> fPeriod = DataHelper.GetFiscalYearPeriod(currentMonth);
                        endMonth = fPeriod["EndMonth"];
                    }
                    return new DateTime(endMonth.Year, endMonth.Month, DateTime.DaysInMonth(endMonth.Year, endMonth.Month));
                }
                else if (periodSelected < 0)
                {
                    DateTime endMonth = new DateTime();

                    if (periodSelected > -13)
                    {
                        endMonth = currentMonth;
                    }
                    else
                    {
                        Dictionary<string, DateTime> fPeriod = DataHelper.GetFiscalYearPeriod(currentMonth.AddYears(-1));
                        endMonth = fPeriod["EndMonth"];
                    }
                    return new DateTime(endMonth.Year, endMonth.Month, DateTime.DaysInMonth(endMonth.Year, endMonth.Month));
                }
                else
                {
                    return diRange.ToDate.Value;
                }
            }
        }

        private string SelectedClientIds
        {
            get
            {
                return cblClient.SelectedItems;
            }
        }

        private string SelectedSalespersonIds
        {
            get
            {
                return cblSalesperson.SelectedItems;
            }
        }

        private string SelectedPracticeIds
        {
            get
            {
                return cblPractice.SelectedItems;
            }
        }

        private string SelectedDivisionIds
        {
            get
            {
                return cblDivision.SelectedItems;
            }
        }

        private string SelectedChannelIds
        {
            get
            {
                return cblChannel.SelectedItems;
            }
        }

        private string SelectedRevenueTypeIds
        {
            get
            {
                return cblRevenueType.SelectedItems;
            }
        }

        private string SelectedOfferingIds
        {
            get
            {
                return cblOffering.SelectedItems;
            }
        }

        private string SelectedGroupIds
        {
            get
            {
                return cblProjectGroup.SelectedItems;
            }
            set
            {
                cblProjectGroup.SelectedItems = value;
            }
        }

        private string SelectedProjectOwnerIds
        {
            get
            {
                return cblProjectOwner.SelectedItems;
            }
        }

        private bool ShowActive
        {

            get { return chbActive.Checked; }
        }

        private bool ShowInternal { get { return chbInternal.Checked; } }

        private bool ShowProposed { get { return chbProposed.Checked; } }

        private bool ShowProjected { get { return chbProjected.Checked; } }

        private bool ShowInactive { get { return chbInactive.Checked; } }

        private bool ShowCompleted { get { return chbCompleted.Checked; } }

        private bool ShowExperimental { get { return chbExperimental.Checked; } }

        private List<Project> ProjectList { get; set; }

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
                dataCellStyle.WrapText = true;
                dataCellStyle.IsBold = true;
                CellStyles[] dataCellStylearray = { dataCellStyle };
                RowStyles datarowStyle = new RowStyles(dataCellStylearray);

                RowStyles[] rowStylearray = { headerrowStyle };

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
                CellStyles headerWrapCellStyle = new CellStyles();
                headerWrapCellStyle.IsBold = true;
                headerWrapCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                headerWrapCellStyle.WrapText = true;

                CellStyles headerCellStyle = new CellStyles();
                headerCellStyle.IsBold = true;
                headerCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

                CellStyles dataDateCellStyle = new CellStyles();
                dataDateCellStyle.DataFormat = "mm/dd/yy;@";

                dataDateCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

                List<CellStyles> headerCellStyleList = new List<CellStyles>();
                headerCellStyleList.Add(headerWrapCellStyle);
                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataCellStyle = new CellStyles();

                var dataCellStylearray = new List<CellStyles>() { dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataDateCellStyle, dataDateCellStyle, dataCellStyle };

                var coloumnWidth = new List<int>();
                for (int i = 0; i < 14; i++)
                    coloumnWidth.Add(0);
                coloumnWidth.Add(80);
                coloumnWidth.Add(0);
                RowStyles datarowStyle = new RowStyles(dataCellStylearray.ToArray());
                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.TopRowNo = headerRowsCount;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanColSplit = 0;
                sheetStyle.FreezePanRowSplit = headerRowsCount;
                sheetStyle.ColoumnWidths = coloumnWidth;
                return sheetStyle;
            }
        }

        private TableStyles _PdfProjectListTableStyle;

        private TableStyles PdfProjectListTableStyle
        {
            get
            {
                if (_PdfProjectListTableStyle == null)
                {
                    TdStyles HeaderStyle = new TdStyles("center", true, false, 7, 1);
                    HeaderStyle.BackgroundColor = "light-gray";
                    TdStyles ContentStyle1 = new TdStyles("left", false, false, 10, 1);
                    TdStyles ContentStyle2 = new TdStyles("center", false, false, 7, 1);

                    TdStyles[] HeaderStyleArray = { HeaderStyle };
                    TdStyles[] ContentStyleArray = { ContentStyle2 };

                    TrStyles HeaderRowStyle = new TrStyles(HeaderStyleArray);
                    TrStyles ContentRowStyle = new TrStyles(ContentStyleArray);

                    TrStyles[] RowStyleArray = { HeaderRowStyle, ContentRowStyle };
                    float[] widths = { 0.075f, 0.09f, 0.09f, 0.1f, 0.1f, 0.075f, 0.09f, 0.09f, 0.1f, 0.1f, 0.1f, 0.09f };
                    _PdfProjectListTableStyle = new TableStyles(widths, RowStyleArray, 100, "custom", new int[] { 245, 250, 255 });
                    _PdfProjectListTableStyle.IsColoumBorders = false;
                }
                return _PdfProjectListTableStyle;
            }
        }

        private List<PracticeCapability> Capabilities
        {
            get;
            set;
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            var filename = "ProjectsList.xls";
            var sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();
            ProjectList = ServiceCallers.Custom.Report(r => r.ProjectsListWithFilters(SelectedClientIds,
                                                                                        ShowProjected,
                                                                                        ShowCompleted,
                                                                                        ShowActive,
                                                                                        ShowInternal,
                                                                                        ShowExperimental,
                                                                                        ShowProposed,
                                                                                        ShowInactive,
                                                                                        StartDate,
                                                                                        EndDate,
                                                                                        SelectedSalespersonIds,
                                                                                        SelectedProjectOwnerIds,
                                                                                        SelectedPracticeIds,
                                                                                        SelectedGroupIds,
                                                                                        SelectedDivisionIds, SelectedChannelIds, SelectedRevenueTypeIds, SelectedOfferingIds,
                                                                                        Page.User.Identity.Name)).ToList();
            if (ProjectList.Count > 0)
            {
                string dateRangeTitle = string.Format(projectsListExportText, StartDate.ToString(Constants.Formatting.EntryDateFormat), EndDate.ToString(Constants.Formatting.EntryDateFormat));
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                headerRowsCount = header.Rows.Count + 3;
                var data = PrepareDataTable(ProjectList);
                coloumnsCount = data.Columns.Count;
                sheetStylesList.Add(HeaderSheetStyle);
                sheetStylesList.Add(DataSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "ProjectsList";
                dataset.Tables.Add(header);
                dataset.Tables.Add(data);
                dataSetList.Add(dataset);
            }
            else
            {
                string dateRangeTitle = "There are no resources for the selected filters.";
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                sheetStylesList.Add(HeaderSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "ProjectsList";
                dataset.Tables.Add(header);
                dataSetList.Add(dataset);
            }
            NPOIExcel.Export(filename, dataSetList, sheetStylesList);
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {
            PDFExport();
        }

        public void PDFExport()
        {
            var data = ServiceCallers.Custom.Report(r => r.ProjectsListWithFilters(SelectedClientIds, ShowProjected, ShowCompleted, ShowActive, ShowInternal, ShowExperimental,
                ShowProposed, ShowInactive, StartDate, EndDate, SelectedSalespersonIds, SelectedProjectOwnerIds, SelectedPracticeIds, SelectedGroupIds, SelectedDivisionIds, SelectedChannelIds, SelectedRevenueTypeIds, SelectedOfferingIds, Page.User.Identity.Name)).ToList();

            HtmlToPdfBuilder builder = new HtmlToPdfBuilder(iTextSharp.text.PageSize.A4_LANDSCAPE);
            string filename = "ProjectsList.pdf";
            byte[] pdfDataInBytes = this.RenderPdf(builder, data);

            HttpContext.Current.Response.ContentType = "Application/pdf";
            HttpContext.Current.Response.AddHeader(
                "content-disposition", string.Format("attachment; filename={0}", Utils.Generic.EncodedFileName(filename)));

            int len = pdfDataInBytes.Length;
            int bytes;
            byte[] buffer = new byte[1024];
            Stream outStream = HttpContext.Current.Response.OutputStream;
            using (MemoryStream stream = new MemoryStream(pdfDataInBytes))
            {
                while (len > 0 && (bytes = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outStream.Write(buffer, 0, bytes);
                    HttpContext.Current.Response.Flush();
                    len -= bytes;
                }
            }
        }

        private byte[] RenderPdf(HtmlToPdfBuilder builder, List<Project> projects)
        {
            int pageCount = GetPageCount(builder, projects);
            MemoryStream file = new MemoryStream();
            Document document = new Document(builder.PageSize);
            document.SetPageSize(iTextSharp.text.PageSize.A4_LANDSCAPE.Rotate());

            MyPageEventHandler e = new MyPageEventHandler()
            {
                PageCount = pageCount,
                PageNo = 1
            };
            PdfWriter writer = PdfWriter.GetInstance(document, file);
            writer.PageEvent = e;
            document.Open();
            var styles = new List<TrStyles>();
            if (projects.Count > 0)
            {
                string reportDataInPdfString = string.Empty;

                reportDataInPdfString += string.Format("Project Number{0}Account{0}Business Group{0}Business Unit{0}Project Name{0}Status{0}Start Date{0}End Date{0}Division{0}Practice Area{0}Channel{0}Channel-Sub{0}Revenue Type{0}Offering{0}Capabilities{0}Project Manager{0}Executive In Charge{1}", ColoumSpliter, RowSpliter);

                foreach (var project in projects)
                {
                    var projectCapabilityIdList = project.ProjectCapabilityIds.Split(',');
                    var flag = true;
                    foreach (var capability in Capabilities)
                    {
                        if (!projectCapabilityIdList.Any(p => p == capability.CapabilityId.ToString()))
                        {
                            flag = false;
                            break;
                        }
                    }
                    var capabilitiesText = flag ? allCapabilitiesText : project.Capabilities;

                    reportDataInPdfString += String.Format("{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}{0}{17}{0}{18}{1}", ColoumSpliter, RowSpliter, project.ProjectNumber, project.Client.Name, project.BusinessGroup.Name,
                    project.Group.Name, project.Name, project.Status.Name, project.StartDate.HasValue ? project.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty,
                    project.EndDate.HasValue ? project.EndDate.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty,
                    project.Division != null ? project.Division.Name : string.Empty,
                    project.Practice.Name,
                    project.Channel != null ? project.Channel.Name : string.Empty,
                project.SubChannel != null ? project.SubChannel : string.Empty,
                project.RevenueType != null ? project.RevenueType.Name : string.Empty,
                project.Offering != null ? project.Offering.Name : string.Empty,
                    capabilitiesText, project.ProjectManagerNames, project.ExecutiveInChargeName);
                }
                var table = builder.GetPdftable(reportDataInPdfString, PdfProjectListTableStyle, RowSpliter, ColoumSpliter);

                document.Add((IElement)table);
            }
            else
            {
                document.Add((IElement)PDFHelper.GetPdfHeaderLogo());
            }
            document.Close();
            return file.ToArray();
        }

        private int GetPageCount(HtmlToPdfBuilder builder, List<Project> projects)
        {
            MemoryStream file = new MemoryStream();
            Document document = new Document(builder.PageSize);
            document.SetPageSize(iTextSharp.text.PageSize.A4_LANDSCAPE.Rotate());
            MyPageEventHandler e = new MyPageEventHandler()
            {
                PageCount = 0,
                PageNo = 1
            };
            PdfWriter writer = PdfWriter.GetInstance(document, file);
            writer.PageEvent = e;
            document.Open();
            var styles = new List<TrStyles>();
            if (projects.Count > 0)
            {
                string reportDataInPdfString = string.Empty;

                reportDataInPdfString += string.Format("Project Number{0}Account{0}Business Group{0}Business Unit{0}Project Name{0}Status{0}Start Date{0}End Date{0}Practice Area{0}Capabilities{0}Project Manager{0}Executive In Charge{1}", ColoumSpliter, RowSpliter);

                foreach (var project in projects)
                {
                    reportDataInPdfString += String.Format("{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{1}", ColoumSpliter, RowSpliter, project.ProjectNumber, project.Client.Name, project.BusinessGroup.Name,
                    project.Group.Name, project.Name, project.Status.Name, project.StartDate.HasValue ? project.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty,
                    project.EndDate.HasValue ? project.EndDate.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty, project.Practice.Name, project.Capabilities, project.ProjectManagerNames, project.ExecutiveInChargeName);
                }
                var table = builder.GetPdftable(reportDataInPdfString, PdfProjectListTableStyle, RowSpliter, ColoumSpliter);
                document.Add((IElement)table);
            }
            else
            {
                document.Add((IElement)PDFHelper.GetPdfHeaderLogo());
            }
            return writer.CurrentPageNumber;
        }

        public DataTable PrepareDataTable(List<Project> report)
        {
            DateTime now = SettingsHelper.GetCurrentPMTime();

            DataTable data = new DataTable();
            List<object> row;

            data.Columns.Add("Project Number");
            data.Columns.Add("Account");
            data.Columns.Add("Business Group");
            data.Columns.Add("Business Unit ");
            data.Columns.Add("Project Name ");
            data.Columns.Add("Status");
            data.Columns.Add("Start Date");
            data.Columns.Add("End Date");
            data.Columns.Add("Division");
            data.Columns.Add("Practice Area");
            data.Columns.Add("Channel");
            data.Columns.Add("Channel-Sub");
            data.Columns.Add("Revenue Type");
            data.Columns.Add("Offering");
            data.Columns.Add("Capabilities");
            data.Columns.Add("Project Manager");
            data.Columns.Add("Excutive in charge");
            data.Columns.Add("Client Time Entry Required");
            data.Columns.Add("Previous Project Number");
            data.Columns.Add("Outsource Id Indicator");
            foreach (var reportItem in report)
            {
                var projectCapabilityIdList = reportItem.ProjectCapabilityIds.Split(',');
                var flag = true;
                foreach (var capability in Capabilities)
                {
                    if (!projectCapabilityIdList.Any(p => p == capability.CapabilityId.ToString()))
                    {
                        flag = false;
                        break;
                    }
                }
                var capabilitiesText = flag ? "All Capabilities" : reportItem.Capabilities;
                row = new List<object>();
                row.Add(reportItem.ProjectNumber);
                row.Add(reportItem.Client.Name);
                row.Add(reportItem.BusinessGroup.Name);
                row.Add(reportItem.Group.Name);
                row.Add(reportItem.Name);
                row.Add(reportItem.Status.Name);
                row.Add(reportItem.StartDate.HasValue ? reportItem.StartDate.Value.ToShortDateString() : string.Empty);
                row.Add(reportItem.EndDate.HasValue ? reportItem.EndDate.Value.ToShortDateString() : string.Empty);
                row.Add(reportItem.Division != null ? reportItem.Division.Name : string.Empty);
                row.Add(reportItem.Practice.Name);
                row.Add(reportItem.Channel != null ? reportItem.Channel.Name : string.Empty);
                row.Add(reportItem.SubChannel != null ? reportItem.SubChannel : string.Empty);
                row.Add(reportItem.RevenueType != null ? reportItem.RevenueType.Name : string.Empty);
                row.Add(reportItem.Offering != null ? reportItem.Offering.Name : string.Empty);
                row.Add(capabilitiesText);
                row.Add(reportItem.ProjectManagerNames);
                row.Add(reportItem.ExecutiveInChargeName);
                row.Add(reportItem.IsClientTimeEntryRequired ? "Yes" : "No");
                row.Add(reportItem.PreviousProject != null ? reportItem.PreviousProject.ProjectNumber : string.Empty);
                row.Add((reportItem.OutsourceId != 3) ? DataHelper.GetDescription((OutsourceId)reportItem.OutsourceId) : string.Empty);
                data.Rows.Add(row.ToArray());
            }
            return data;
        }

        protected string GetDateFormat(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            diRange.FromDate = StartDate;
            diRange.ToDate = EndDate;
            lblCustomDateRange.Text = string.Format("({0}&nbsp;-&nbsp;{1})",
                    diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat),
                    diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat)
                    );
            if (ddlPeriod.SelectedValue == "0")
            {
                lblCustomDateRange.Attributes.Add("class", "fontBold");
                imgCalender.Attributes.Add("class", "");
            }
            else
            {
                lblCustomDateRange.Attributes.Add("class", "displayNone");
                imgCalender.Attributes.Add("class", "displayNone");
            }
            hdnStartDate.Value = diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat);
            hdnEndDate.Value = diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat);
            hdnStartDateTxtBoxId.Value = (diRange.FindControl("tbFrom") as TextBox).ClientID;
            hdnEndDateTxtBoxId.Value = (diRange.FindControl("tbTo") as TextBox).ClientID;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var capabilities = ServiceCallers.Custom.Practice(p => p.GetPracticeCapabilities(null, null)).ToList();
            Capabilities = capabilities.Where(pc => pc.IsActive).ToList();
            divWholePage.Visible = false;
            if (!IsPostBack)
            {
                PreparePeriodView();
                GetFilterValuesForSession();
            }

            if (hdnFiltersChanged.Value == "false")
            {
                btnResetFilter.Attributes.Add("disabled", "true");
            }
            else
            {
                btnResetFilter.Attributes.Remove("disabled");
            }
            AddAttributesToCheckBoxes(this.cblClient);
            AddAttributesToCheckBoxes(cblProjectGroup);
            AddAttributesToCheckBoxes(cblPractice);
            AddAttributesToCheckBoxes(cblProjectOwner);
            AddAttributesToCheckBoxes(cblSalesperson);
            AddAttributesToCheckBoxes(cblDivision);
            AddAttributesToCheckBoxes(cblChannel);
            AddAttributesToCheckBoxes(cblRevenueType);
            AddAttributesToCheckBoxes(cblOffering);
        }

        private void AddAttributesToCheckBoxes(ScrollingDropDown ddlpractices)
        {
            foreach (System.Web.UI.WebControls.ListItem item in ddlpractices.Items)
            {
                item.Attributes.Add("onclick", "EnableResetButton();");
            }
        }

        protected void btnResetFilter_Click(object sender, EventArgs e)
        {
            PreparePeriodView();
            hdnFiltersChanged.Value = "false";
            btnResetFilter.Attributes.Add("disabled", "true");
            AddAttributesToCheckBoxes(cblClient);
            AddAttributesToCheckBoxes(cblProjectGroup);
            AddAttributesToCheckBoxes(cblPractice);
            AddAttributesToCheckBoxes(cblProjectOwner);
            AddAttributesToCheckBoxes(cblSalesperson);
            AddAttributesToCheckBoxes(cblDivision);
            AddAttributesToCheckBoxes(cblChannel);
            AddAttributesToCheckBoxes(cblRevenueType);
            AddAttributesToCheckBoxes(cblOffering);
        }

        private void PreparePeriodView()
        {
            var person =
                    (Roles.IsUserInRole(
                        DataHelper.CurrentPerson.Alias,
                        DataTransferObjects.Constants.RoleNames.AdministratorRoleName)
                        || Roles.IsUserInRole(
                        DataHelper.CurrentPerson.Alias,
                        DataTransferObjects.Constants.RoleNames.OperationsRoleName))
                    ? null : DataHelper.CurrentPerson;

            PraticeManagement.Controls.DataHelper.FillSalespersonList(
                     person, cblSalesperson,
                     Resources.Controls.AllSalespersonsText,
                     true);

            PraticeManagement.Controls.DataHelper.FillProjectOwnerList(cblProjectOwner,
                "All People with Project Access",
                true,
                person, true);

            PraticeManagement.Controls.DataHelper.FillPracticeList(
                person,
                cblPractice,
                Resources.Controls.AllPracticesText);

            DataHelper.FillProjectDivisionList(cblDivision, false, true);

            DataHelper.FillChannelList(cblChannel, "All Channels");

            DataHelper.FillOfferingsList(cblOffering, "All Offerings");

            DataHelper.FillRevenueTypeList(cblRevenueType, "All Revenue Types");

            PraticeManagement.Controls.DataHelper.FillClientsAndGroups(
                cblClient, cblProjectGroup);
            cblClient.SelectAll();
            cblProjectGroup.SelectAll();
            cblSalesperson.SelectAll();
            cblProjectOwner.SelectAll();
            cblPractice.SelectAll();
            cblDivision.SelectAll();
            cblChannel.SelectAll();
            cblOffering.SelectAll();
            cblRevenueType.SelectAll();

            chbActive.Checked = true;
            chbCompleted.Checked = true;
            chbProjected.Checked = true;
            chbProposed.Checked = true;
            chbInternal.Checked = true;
            chbExperimental.Checked = false;
            chbInactive.Checked = false;

            ddlPeriod.SelectedValue = "3";//selecting 'Next 3 Months' as default value in 'Period' drop down list
        }

        protected void btnUpdateView_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                divWholePage.Visible = false;
                return;
            }
            SaveFilterValuesForSession();
            PopulateData();
        }

        public void PopulateData()
        {
            ProjectList = ServiceCallers.Custom.Report(r => r.ProjectsListWithFilters(SelectedClientIds, ShowProjected, ShowCompleted, ShowActive, ShowInternal, ShowExperimental,
                ShowProposed, ShowInactive, StartDate, EndDate, SelectedSalespersonIds, SelectedProjectOwnerIds, SelectedPracticeIds, SelectedGroupIds, SelectedDivisionIds, SelectedChannelIds, SelectedRevenueTypeIds, SelectedOfferingIds, Page.User.Identity.Name)).ToList();
            if (ProjectList.Any())
            {
                divWholePage.Visible = true;
                divEmptyMessage.Visible = false;
                lblRange.Text = string.Format("{0}&nbsp;-&nbsp;{1}", StartDate.ToString(Constants.Formatting.EntryDateFormat), EndDate.ToString(Constants.Formatting.EntryDateFormat));
                repProjectsList.DataSource = ProjectList;
                repProjectsList.DataBind();
            }
            else
            {
                divWholePage.Visible = false;
                divEmptyMessage.Visible = true;
            }
        }

        protected string GetProjectDetailsLink(int? projectId)
        {
            return Utils.Generic.GetTargetUrlWithReturn(String.Format(Constants.ApplicationPages.DetailRedirectFormat, Constants.ApplicationPages.ProjectDetail, projectId.Value),
                                                        Constants.ApplicationPages.ProjectsListPage);
        }

        protected string GetAcountDetailsLink(int? accountId)
        {
            return Utils.Generic.GetTargetUrlWithReturn(String.Format(Constants.ApplicationPages.DetailRedirectFormat, Constants.ApplicationPages.ClientDetails, accountId.Value),
                                                            Constants.ApplicationPages.ProjectsListPage);
        }

        protected void repProjectsList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lblCapabilities = e.Item.FindControl("lblCapabilities") as Label;
                var dataItem = e.Item.DataItem as Project;
                //keeping 'All Capabilities' value in the 'Capabilities' column when selected all capabilities.
                var projectCapabilityIdList = dataItem.ProjectCapabilityIds.Split(',');
                var flag = true;
                foreach (var capability in Capabilities)
                {
                    if (!projectCapabilityIdList.Any(p => p == capability.CapabilityId.ToString()))
                    {
                        flag = false;
                        break;
                    }
                }
                lblCapabilities.Text = flag ? allCapabilitiesText : dataItem.Capabilities;
            }
        }

        private void SaveFilterValuesForSession()
        {
            ProjectsListFilters filter = new ProjectsListFilters();
            filter.ReportPeriod = ddlPeriod.SelectedValue;
            filter.ClientIds = SelectedClientIds;
            filter.SalesPersonIds = SelectedSalespersonIds;
            filter.PracticeIds = SelectedPracticeIds;
            filter.DivisionIds = SelectedDivisionIds;
            filter.ChannelIds = SelectedChannelIds;
            filter.RevenueTypeIds = SelectedRevenueTypeIds;
            filter.OfferingIds = SelectedOfferingIds;
            filter.BusinessUnitIds = SelectedGroupIds;
            filter.ProjectAccessPeopleIds = SelectedProjectOwnerIds;
            filter.IsActive = chbActive.Checked;
            filter.IsInactive = chbInactive.Checked;
            filter.IsInternal = chbInternal.Checked;
            filter.IsProjected = chbProjected.Checked;
            filter.IsProposed = chbProposed.Checked;
            filter.IsCompleted = chbCompleted.Checked;
            filter.IsExperimental = chbExperimental.Checked;
            filter.ReportStartDate = diRange.FromDate;
            filter.ReportEndDate = diRange.ToDate;
            ReportsFilterHelper.SaveFilterValues(ReportName.ProjectsList, filter);
        }

        private void GetFilterValuesForSession()
        {
            var filters = ReportsFilterHelper.GetFilterValues(ReportName.ProjectsList) as ProjectsListFilters;
            if (filters != null)
            {
                cblClient.UnSelectAll();
                cblClient.SelectedItems = filters.ClientIds;
                cblSalesperson.UnSelectAll();
                cblSalesperson.SelectedItems = filters.SalesPersonIds;
                cblProjectGroup.UnSelectAll();
                cblProjectGroup.SelectedItems = filters.BusinessUnitIds;
                cblPractice.UnSelectAll();
                cblPractice.SelectedItems = filters.PracticeIds;
                cblDivision.UnSelectAll();
                cblDivision.SelectedItems = filters.DivisionIds;
                cblChannel.UnSelectAll();
                cblChannel.SelectedItems = filters.ChannelIds;
                cblOffering.UnSelectAll();
                cblOffering.SelectedItems = filters.OfferingIds;
                cblRevenueType.UnSelectAll();
                cblRevenueType.SelectedItems = filters.RevenueTypeIds;
                cblProjectOwner.UnSelectAll();
                cblProjectOwner.SelectedItems = filters.ProjectAccessPeopleIds;
                chbActive.Checked = filters.IsActive;
                chbInactive.Checked = filters.IsInactive;
                chbInternal.Checked = filters.IsInternal;
                chbCompleted.Checked = filters.IsCompleted;
                chbExperimental.Checked = filters.IsExperimental;
                chbProjected.Checked = filters.IsProjected;
                chbProposed.Checked = filters.IsProposed;
                ddlPeriod.SelectedValue = filters.ReportPeriod;
                diRange.FromDate = filters.ReportStartDate;
                diRange.ToDate = filters.ReportEndDate;
                PopulateData();
            }
        }
    }
}

