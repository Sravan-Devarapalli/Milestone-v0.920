using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using PraticeManagement.PersonStatusService;
using System.ServiceModel;
using DataTransferObjects;
using System.Web.Script.Serialization;
using AjaxControlToolkit;
using System.Data;
using PraticeManagement.Utils.Excel;
using PraticeManagement.Utils;
using PraticeManagement.Objects;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using DataTransferObjects.Filters;

namespace PraticeManagement.Reports.Badge
{
    public partial class PersonsByProject : System.Web.UI.Page
    {
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;

        private const string PersonsByProjectFiltersKey = "PersonsByProjectFiltersKey";

        private string RowSpliter = Guid.NewGuid().ToString();

        private string ColoumSpliter = Guid.NewGuid().ToString();

        private TableStyles _PdfProjectPersonsSummaryTableStyle;

        private TableStyles PdfProjectPersonsSummaryTableStyle
        {
            get
            {
                if (_PdfProjectPersonsSummaryTableStyle == null)
                {
                    TdStyles headerStyle1 = new TdStyles("left", true, false, 10, 1);
                    TdStyles headerStyle2 = new TdStyles("center", true, false, 10, 1);
                    TdStyles contentStyle1 = new TdStyles("left", false, false, 10, 1);
                    TdStyles contentStyle2 = new TdStyles("center", false, false, 10, 1);

                    TdStyles[] headerStyleArray = { headerStyle1, headerStyle2 };
                    TdStyles[] contentStyleArray = { contentStyle1, contentStyle2 };

                    TrStyles headerRowStyle = new TrStyles(headerStyleArray);
                    TrStyles contentRowStyle = new TrStyles(contentStyleArray);

                    TrStyles[] rowStyleArray = { }; 
                    float[] widths = { 0.125f, 0.1f, 0.075f, 0.1f, 0.1f, 0.1f, 0.1f, 0.075f, 0.075f, 0.075f, 0.075f };
                    _PdfProjectPersonsSummaryTableStyle = new TableStyles(widths, rowStyleArray, 100);
                    _PdfProjectPersonsSummaryTableStyle.IsColoumBorders = false;
                }
                return (TableStyles)_PdfProjectPersonsSummaryTableStyle;
            }
        }

        private TrStyles PdfProjectHeaderRowStyle
        {
            get
            {
                TdStyles headerStyle1 = new TdStyles("left", true, false, 10, 1);
                TdStyles[] headerStyleArray = { headerStyle1 };
                return new TrStyles(headerStyleArray, "custom", new int[] { 212, 208, 201 });
            }
        }

        private TrStyles PdfMilestoneHeaderRowStyle
        {
            get
            {
                TdStyles headerStyle1 = new TdStyles("left", true, false, 10, 1);
                TdStyles[] headerStyleArray = { headerStyle1 };
                return new TrStyles(headerStyleArray, "custom", new int[] { 236, 233, 217 });
            }
        }

        private TrStyles PdfResourceHeaderRowStyle
        {
            get
            {
                TdStyles headerStyle1 = new TdStyles("left", true, false, 10, 1);
                TdStyles[] headerStyleArray = { headerStyle1 };
                return new TrStyles(headerStyleArray);
            }
        }

        private TrStyles PdfContentRowStyle
        {
            get
            {
                TdStyles contentStyle1 = new TdStyles("left", false, false, 10, 1);
                TdStyles[] contentStyleArray = { contentStyle1 };
                return new TrStyles(contentStyleArray);
            }
        }

        public string Accounts
        {
            get
            {
                return cblClient.SelectedItems;
            }
        }

        public string PayTypes
        {
            get
            {
                return cblPayTypes.SelectedItems;
            }
        }

        public string PersonStatuses
        {
            get
            {
                return cblPersonStatus.SelectedItems;
            }
        }

        public string ProjectStatuses
        {
            get
            {
                return cblProjectTypes.SelectedItems;
            }
        }

        public string Practices
        {
            get
            {
                return cblPractices.SelectedItems;
            }
        }

        public bool ExcludeInternals
        {
            get
            {
                return chkExcludeInternalPractices.Checked;
            }
        }

        private List<string> CollapsiblePanelExtenderProjectClientIds
        {
            get;
            set;
        }

        private List<string> CollapsiblePanelExtenderMilestoneClientIds
        {
            get;
            set;
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
                sheetStyle.MergeRegion.Add(new int[] { 0, 0, 0, coloumnsCount > 5 ? coloumnsCount : 6 - 1 });
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

                CellStyles dataDateCellStyle = new CellStyles();
                dataDateCellStyle.DataFormat = "mm/dd/yyyy;@";
                dataDateCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

                List<CellStyles> headerCellStyleList = new List<CellStyles>() { headerCellStyle };

                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataCellStyle = new CellStyles();
                var dataCellStylearray = new List<CellStyles>() { dataCellStyle, dataCellStyle, dataDateCellStyle, dataDateCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataCellStyle, dataDateCellStyle };

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

        protected void Page_Load(object sender, EventArgs e)
        {
            btnExpandOrCollapseAll.Attributes["onclick"] = "return CollapseOrExpandAll(" + btnExpandOrCollapseAll.ClientID +
                                                          ", " + hdnCollapsed.ClientID +
                                                          ", " + hdncpeExtendersIds.ClientID +
                
                                                          ");";
            btnExpandOrCollapseAll.Text = btnExpandOrCollapseAll.ToolTip = (hdnCollapsed.Value.ToLower() == "true") ? "Expand All" : "Collapse All";

            if (!IsPostBack)
            {
                DataHelper.FillTimescaleList(this.cblPayTypes, Resources.Controls.AllTypes);
                cblPayTypes.SelectItems(new List<int>() { 1, 2 });
                FillPersonStatusList();
                cblPersonStatus.SelectItems(new List<int>() { 1, 5 });
                DataHelper.FillProjectStatusList(cblProjectTypes, Resources.Controls.AllTypes);
                cblProjectTypes.SelectItems(new List<int>() { 1, 2, 3, 4, 8});
                DataHelper.FillPracticeList(cblPractices, Resources.Controls.AllPracticesText);
                cblPractices.SelectAll();
                var allClients = ServiceCallers.Custom.Client(c => c.ClientListAllWithoutPermissions());
                DataHelper.FillListDefaultWithEncodedName(cblClient, "All Accounts", allClients,
                                                 false);
                cblClient.SelectAll();
                GetFilterValuesForSession();
            }
        }

        public void FillPersonStatusList()
        {
            using (var serviceClient = new PersonStatusServiceClient())
            {
                try
                {
                    var statuses = serviceClient.GetPersonStatuses();
                    statuses = statuses.ToArray();
                    DataHelper.FillListDefault(cblPersonStatus, Resources.Controls.AllTypes, statuses, false);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        private string GetFilterText(ScrollingDropDown dropDownObject)
        {
            if (dropDownObject.areAllSelected)
            {
                return "All " + dropDownObject.DropDownListType + dropDownObject.PluralForm;
            }
            else
            {
                if (dropDownObject.SelectedValues.Count > 1)
                {
                    return "Multiple " + dropDownObject.DropDownListType + dropDownObject.PluralForm;
                }
                else
                {
                    return !string.IsNullOrEmpty(dropDownObject.SelectedItemsText) ? dropDownObject.SelectedItemsText.TrimEnd(',') : string.Empty;
                }
            }
        }

        protected void btnUpdateView_Click(object sender, EventArgs e)
        {


            SaveFilterValuesForSession();
            PopulateData();
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            var dataSetList = new List<DataSet>();
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();

            var data = ServiceCallers.Custom.Project(p => p.PersonsByProjectReport(Accounts, PayTypes, PersonStatuses, Practices, ProjectStatuses, ExcludeInternals)).ToList();

            var filename = "PersonByProject.xls";

            if (data.Count > 0)
            {
                string dateRangeTitle = "Persons By Project";
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);

                header.Rows.Add("Account: " + GetFilterText(cblClient));
                header.Rows.Add("Project Status: " + GetFilterText(cblProjectTypes));
                header.Rows.Add("Pay Type: " + GetFilterText(cblPayTypes));
                header.Rows.Add("Person Status: " + GetFilterText(cblPersonStatus));
                header.Rows.Add("Practice Area: " + GetFilterText(cblPractices));

                headerRowsCount = header.Rows.Count + 3;
                var dataTbl = PrepareDataTable(data);
                coloumnsCount = dataTbl.Columns.Count;
                sheetStylesList.Add(HeaderSheetStyle);
                sheetStylesList.Add(DataSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "PersonByProject";
                dataset.Tables.Add(header);
                dataset.Tables.Add(dataTbl);
                dataSetList.Add(dataset);
            }
            else
            {
                string dateRangeTitle = "There are no Time Entries towards this project.";
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                sheetStylesList.Add(HeaderSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "PersonByProject";
                dataset.Tables.Add(header);
                dataSetList.Add(dataset);
            }
            NPOIExcel.Export(filename, dataSetList, sheetStylesList);
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {
            PDFExport();
        }

        public DataTable PrepareDataTable(List<Project> report)
        {
            DataTable data = new DataTable();
            List<object> row;

            data.Columns.Add("Project Number");
            data.Columns.Add("Project Name");
            data.Columns.Add("Start Date");
            data.Columns.Add("End Date");
            data.Columns.Add("Milestone Name");
            data.Columns.Add("Resource Name");
            data.Columns.Add("Resource Level");
            data.Columns.Add("Person Status");
            data.Columns.Add("Milestone Resource Start Date");
            data.Columns.Add("Milestone Resource End Date");
            data.Columns.Add("Badge Start Date");
            data.Columns.Add("Badge End Date");
            data.Columns.Add("Organic Break Start Date");
            data.Columns.Add("Oraganic Break End Date");
            data.Columns.Add("MSFT Block Start Date");
            data.Columns.Add("MSFT Block End Date");

            foreach (var project in report)
            {
                foreach (var milestone in project.Milestones)
                {
                    foreach (var person in milestone.People)
                    {
                        row = new List<object>();
                        row.Add(project.ProjectNumber);
                        row.Add(project.Name);
                        row.Add(project.StartDate);
                        row.Add(project.EndDate);
                        row.Add(milestone.Description.ToString());
                        row.Add(person.Name);
                        row.Add(person.Title.TitleName);
                        row.Add(person.Status.Name);
                        row.Add(person.ResourceStartDate);
                        row.Add(person.ResourceEndDate);
                        row.Add(person.Badge.BadgeStartDate.HasValue ? person.Badge.BadgeStartDate.Value.ToShortDateString() : "Clock not yet started");
                        row.Add(person.Badge.BadgeEndDate.HasValue ? person.Badge.BadgeEndDate.Value.ToShortDateString() : "Clock not yet started");
                        row.Add(person.Badge.OrganicBreakStartDate.HasValue ? person.Badge.OrganicBreakStartDate.Value.ToShortDateString() : string.Empty);
                        row.Add(person.Badge.OrganicBreakEndDate.HasValue ? person.Badge.OrganicBreakEndDate.Value.ToShortDateString() : string.Empty);
                        row.Add(person.Badge.BlockStartDate.HasValue ? person.Badge.BlockStartDate.Value.ToShortDateString() : string.Empty);
                        row.Add(person.Badge.BlockEndDate.HasValue ? person.Badge.BlockEndDate.Value.ToShortDateString() : string.Empty);
                        data.Rows.Add(row.ToArray());
                    }
                }
            }
            return data;
        }

        protected string GetDateFormat(DateTime date)
        {
            return date.ToString(Constants.Formatting.EntryDateFormat);
        }

        public void PopulateData()
        {
            divWholePage.Style["display"] = "";
            var data = ServiceCallers.Custom.Project(p => p.PersonsByProjectReport(Accounts, PayTypes, PersonStatuses, Practices, ProjectStatuses, ExcludeInternals)).ToList();
            if (data.Count > 0)
            {
                repProjects.Visible = true;
                divEmptyMessage.Style["display"] = "none";
                tblRange.Style["display"] = "";
                repProjects.DataSource = data;
                repProjects.DataBind();
            }
            else
            {
                tblRange.Style["display"] = "none";
                repProjects.Visible = false;
                divEmptyMessage.Style["display"] = "";
            }
        }

        protected void repProjects_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                CollapsiblePanelExtenderProjectClientIds = new List<string>();
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var repMilestones = e.Item.FindControl("repMilestones") as Repeater;
                var dataitem = (Project)e.Item.DataItem;
                repMilestones.DataSource = dataitem.Milestones;
                repMilestones.DataBind();
                var cpeMilestones = e.Item.FindControl("cpeMilestones") as CollapsiblePanelExtender;
                cpeMilestones.BehaviorID = Guid.NewGuid().ToString();
                CollapsiblePanelExtenderProjectClientIds.Add(cpeMilestones.BehaviorID);
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var output = jss.Serialize(CollapsiblePanelExtenderProjectClientIds);
                hdncpeExtendersIds.Value = output;
                JavaScriptSerializer milestoneJss = new JavaScriptSerializer();
                var milestneOutput = jss.Serialize(CollapsiblePanelExtenderMilestoneClientIds);
                hdncpeMilestoneExtIds.Value = milestneOutput;
                btnExpandOrCollapseAll.Text = btnExpandOrCollapseAll.ToolTip = "Expand All";
                hdnCollapsed.Value = "true";
            }
        }

        protected void repMilestones_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var repResources = e.Item.FindControl("repResources") as Repeater;
                var dataitem = (Milestone)e.Item.DataItem;
                repResources.DataSource = dataitem.People;
                repResources.DataBind();
            }
        }

        protected void repResources_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lblResourceStart = e.Item.FindControl("lblResourceStartDate") as Label;
                var lblResourceEnd = e.Item.FindControl("lblResourceEndDate") as Label;
                var lblBadgeStart = e.Item.FindControl("lblBadgeStart") as Label;
                var lblBadgeEnd = e.Item.FindControl("lblBadgeEnd") as Label;
                var lblOrganicStart = e.Item.FindControl("lblOrganicStart") as Label;
                var lblOrganicEnd = e.Item.FindControl("lblOrganicEnd") as Label;
                var lblBlockStart = e.Item.FindControl("lblBlockStart") as Label;
                var lblBlockEnd = e.Item.FindControl("lblBlockEnd") as Label;
                var dataitem = (Person)e.Item.DataItem;
                lblResourceStart.Text = dataitem.ResourceStartDate.ToString(Constants.Formatting.EntryDateFormat);
                lblResourceEnd.Text = dataitem.ResourceEndDate.ToString(Constants.Formatting.EntryDateFormat);
                lblBadgeStart.Text = dataitem.Badge.BadgeStartDate.HasValue ? dataitem.Badge.BadgeStartDate.Value.ToString(Constants.Formatting.EntryDateFormat) : "Clock not yet started";
                lblBadgeEnd.Text = dataitem.Badge.BadgeEndDate.HasValue ? dataitem.Badge.BadgeEndDate.Value.ToString(Constants.Formatting.EntryDateFormat) : "Clock not yet started";
                lblOrganicStart.Text = dataitem.Badge.OrganicBreakStartDate.HasValue ? dataitem.Badge.OrganicBreakStartDate.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty;
                lblOrganicEnd.Text = dataitem.Badge.OrganicBreakEndDate.HasValue ? dataitem.Badge.OrganicBreakEndDate.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty;
                lblBlockStart.Text = dataitem.Badge.BlockStartDate.HasValue ? dataitem.Badge.BlockStartDate.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty;
                lblBlockEnd.Text = dataitem.Badge.BlockEndDate.HasValue ? dataitem.Badge.BlockEndDate.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty;
            }
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
                foreach (var project in projects)
                {
                    reportDataInPdfString = String.Format("{0} - {1} ({2} - {3}){5}!!!{5}!!!{5}!!!{5}!!!{5}!!!{5}!!!{5}!!!{5}!!!{5}{5}{4}", project.ProjectNumber, project.Name, project.StartDate.Value.ToString("MM/dd/yyyy"), project.EndDate.Value.ToString("MM/dd/yyyy"), RowSpliter, ColoumSpliter);
                    styles.Add(PdfProjectHeaderRowStyle);
                    foreach (var milestone in project.Milestones)
                    {
                        reportDataInPdfString += String.Format("{0} ({1} to {2}){4}!!!{4}!!!{4}!!!{4}!!!{4}!!!{4}!!!{4}!!!{4}!!!{4}{4}{3}",
                            milestone.Description, milestone.StartDate.ToString("MM/dd/yyyy"), milestone.ProjectedDeliveryDate.ToString("MM/dd/yyyy"), RowSpliter, ColoumSpliter);

                        styles.Add(PdfMilestoneHeaderRowStyle);
                        reportDataInPdfString += string.Format("Resource Name{0}Resource Level{0}Person Status{0}Milestone Resource Start Date{0}Milestone Resource End Date{0}Badge Start{0}Badge End{0}Organic Break Start{0}Organic Break End{0}MSFT Block Start{0}MSFT Block End{1}", ColoumSpliter, RowSpliter);
                        styles.Add(PdfResourceHeaderRowStyle);
                        foreach (var resource in milestone.People)
                        {
                            reportDataInPdfString += String.Format("{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{1}",
                            ColoumSpliter, RowSpliter, resource.Name, resource.Title.TitleName, resource.Status.Name,
                            resource.ResourceStartDate.ToString(Constants.Formatting.EntryDateFormat),
                            resource.ResourceEndDate.ToString(Constants.Formatting.EntryDateFormat),
                            resource.Badge.BadgeStartDate.HasValue ? resource.Badge.BadgeStartDate.Value.ToString(Constants.Formatting.EntryDateFormat) : "Clock not yet started",
                            resource.Badge.BadgeEndDate.HasValue ? resource.Badge.BadgeEndDate.Value.ToString(Constants.Formatting.EntryDateFormat) : "Clock not yet started",
                            resource.Badge.OrganicBreakStartDate.HasValue ? resource.Badge.OrganicBreakStartDate.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty,
                            resource.Badge.OrganicBreakEndDate.HasValue ? resource.Badge.OrganicBreakEndDate.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty,
                            resource.Badge.BlockStartDate.HasValue ? resource.Badge.BlockStartDate.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty,
                            resource.Badge.BlockEndDate.HasValue ? resource.Badge.BlockEndDate.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty
                            );
                            styles.Add(PdfContentRowStyle);
                        }
                    }
                    PdfProjectPersonsSummaryTableStyle.trStyles = styles.ToArray();
                    float[] widths = { 0.125f, 0.1f, 0.075f, 0.1f, 0.1f, 0.1f, 0.1f, 0.075f, 0.075f, 0.075f, 0.075f };
                    PdfProjectPersonsSummaryTableStyle.widths = widths;
                    var table = builder.GetPdftablePersonsByProject(reportDataInPdfString, PdfProjectPersonsSummaryTableStyle, RowSpliter, ColoumSpliter);
                    document.Add((IElement)table);
                }
                document.NewPage();
            }
            else
            {
                document.Add((IElement)PDFHelper.GetPdfHeaderLogo());
            }
            return writer.CurrentPageNumber;
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
                foreach (var project in projects)
                {
                    reportDataInPdfString = String.Format("{0} - {1} ({2} - {3}){5}!!!{5}!!!{5}!!!{5}!!!{5}!!!{5}!!!{5}!!!{5}!!!{5}{5}{4}", project.ProjectNumber, project.Name, project.StartDate.Value.ToString("MM/dd/yyyy"), project.EndDate.Value.ToString("MM/dd/yyyy"), RowSpliter, ColoumSpliter);
                    styles.Clear();
                    styles.Add(PdfProjectHeaderRowStyle);
                    foreach (var milestone in project.Milestones)
                    {
                        reportDataInPdfString += String.Format("{0} ({1} to {2}){4}!!!{4}!!!{4}!!!{4}!!!{4}!!!{4}!!!{4}!!!{4}!!!{4}{4}{3}",
                            milestone.Description, milestone.StartDate.ToString("MM/dd/yyyy"), milestone.ProjectedDeliveryDate.ToString("MM/dd/yyyy"), RowSpliter, ColoumSpliter);

                        styles.Add(PdfMilestoneHeaderRowStyle);
                        reportDataInPdfString += string.Format("Resource Name{0}Resource Level{0}Person Status{0}Milestone Resource Start Date{0}Milestone Resource End Date{0}Badge Start{0}Badge End{0}Organic Break Start{0}Organic Break End{0}MSFT Block Start{0}MSFT Block End{1}", ColoumSpliter, RowSpliter);
                        styles.Add(PdfResourceHeaderRowStyle);
                        foreach (var resource in milestone.People)
                        {
                            reportDataInPdfString += String.Format("{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{1}",
                            ColoumSpliter, RowSpliter, resource.Name, resource.Title.TitleName, resource.Status.Name,
                            resource.ResourceStartDate.ToString(Constants.Formatting.EntryDateFormat),
                            resource.ResourceEndDate.ToString(Constants.Formatting.EntryDateFormat),
                            resource.Badge.BadgeStartDate.HasValue ? resource.Badge.BadgeStartDate.Value.ToString(Constants.Formatting.EntryDateFormat) : "Clock not yet started",
                            resource.Badge.BadgeEndDate.HasValue ? resource.Badge.BadgeEndDate.Value.ToString(Constants.Formatting.EntryDateFormat) : "Clock not yet started",
                            resource.Badge.OrganicBreakStartDate.HasValue ? resource.Badge.OrganicBreakStartDate.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty,
                            resource.Badge.OrganicBreakEndDate.HasValue ? resource.Badge.OrganicBreakEndDate.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty,
                            resource.Badge.BlockStartDate.HasValue ? resource.Badge.BlockStartDate.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty,
                            resource.Badge.BlockEndDate.HasValue ? resource.Badge.BlockEndDate.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty
                            );
                            styles.Add(PdfContentRowStyle);
                        }
                    }
                    PdfProjectPersonsSummaryTableStyle.trStyles = styles.ToArray();
                    float[] widths = { 0.125f, 0.1f, 0.075f, 0.1f, 0.1f, 0.1f, 0.1f, 0.075f, 0.075f, 0.075f, 0.075f };
                    PdfProjectPersonsSummaryTableStyle.widths = widths;
                    var table = builder.GetPdftablePersonsByProject(reportDataInPdfString, PdfProjectPersonsSummaryTableStyle, RowSpliter, ColoumSpliter);
                    document.Add((IElement)table);
                }
            }
            else
            {
                document.Add((IElement)PDFHelper.GetPdfHeaderLogo());
            }
            document.Close();
            return file.ToArray();
        }

        public void PDFExport()
        {
            var data = ServiceCallers.Custom.Project(p => p.PersonsByProjectReport(Accounts, PayTypes, PersonStatuses, Practices, ProjectStatuses, ExcludeInternals)).ToList();

            HtmlToPdfBuilder builder = new HtmlToPdfBuilder(iTextSharp.text.PageSize.A4_LANDSCAPE);
            string filename = "PersonByProject.pdf";
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

        private void SaveFilterValuesForSession()
        {
            PersonsByProjectFilters filter = new PersonsByProjectFilters();
            filter.ClientIds = Accounts;
            filter.PayTypeIds = PayTypes;
            filter.PersonStatusIds = PersonStatuses;
            filter.ProjectStatusIds = ProjectStatuses;
            filter.PracticeIds = Practices;
            filter.ExcludeInternalPractices = chkExcludeInternalPractices.Checked;
            ReportsFilterHelper.SaveFilterValues(ReportName.PesonsByProject, filter);
        }

        private void GetFilterValuesForSession()
        {
            var filters = ReportsFilterHelper.GetFilterValues(ReportName.PesonsByProject) as PersonsByProjectFilters;
            if (filters != null)
            {
                cblClient.UnSelectAll();
                cblClient.SelectedItems = filters.ClientIds;
                cblPayTypes.UnSelectAll();
                cblPayTypes.SelectedItems = filters.PayTypeIds;
                cblPersonStatus.UnSelectAll();
                cblPersonStatus.SelectedItems = filters.PersonStatusIds;
                cblProjectTypes.UnSelectAll();
                cblProjectTypes.SelectedItems = filters.ProjectStatusIds;
                cblPractices.UnSelectAll();
                cblPractices.SelectedItems = filters.PracticeIds;
                chkExcludeInternalPractices.Checked = filters.ExcludeInternalPractices;
                PopulateData();
            }
        }
    }
}

