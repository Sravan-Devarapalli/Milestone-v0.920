using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using PraticeManagement.Controls;
using DataTransferObjects;
using iTextSharp.text.pdf;
using PraticeManagement.Objects;
using DataTransferObjects.Reports;
using PraticeManagement.Configuration;
using System.IO;
using iTextSharp.text;

namespace PraticeManagement.Reporting
{
    public partial class ProjectSummaryReport : Page
    {
        public const string ProjectNumberKey = "ProjectNumber";
        public const string StartDateKey = "StartDate";
        public const string EndDateKey = "EndDate";
        public const string PeriodSelectedkey = "PeriodSelected";
        #region Properties

        public string ProjectNumberFromQueryString
        {
            get
            {
                return Request.QueryString[ProjectNumberKey];
            }
        }
        public string StartDateFromQueryString
        {
            get
            {
                return Request.QueryString[StartDateKey];
            }
        }
        public string EndDatFromQueryString
        {
            get
            {
                return Request.QueryString[EndDateKey];
            }
        }
        public string PeriodSelectedFromQueryString
        {
            get
            {
                return Request.QueryString[PeriodSelectedkey];
            }
        }

        public DateTime? StartDate
        {
            get
            {
                System.Web.UI.WebControls.ListItem li = ddlPeriod.SelectedItem;

                if (li.Attributes["milestone"] == null)
                {
                    int selectedVal = 0;

                    if (int.TryParse(ddlPeriod.SelectedValue, out selectedVal))
                    {
                        if (selectedVal == 0)
                        {
                            return diRange.FromDate.Value;
                        }
                        else
                        {
                            var now = Utils.Generic.GetNowWithTimeZone();
                            if (selectedVal > 0)
                            {
                                if (selectedVal == 7)
                                {
                                    return Utils.Calendar.WeekStartDate(now);
                                }
                                else if (selectedVal == 30)
                                {
                                    return Utils.Calendar.MonthStartDate(now);
                                }
                                else if (selectedVal == 15)
                                {
                                    return Utils.Calendar.PayrollCurrentStartDate(now);
                                }
                                else
                                {
                                    return Utils.Calendar.YearStartDate(now);
                                }

                            }
                            else if (selectedVal < 0)
                            {
                                if (selectedVal == -7)
                                {
                                    return Utils.Calendar.LastWeekStartDate(now);
                                }
                                else if (selectedVal == -15)
                                {
                                    return Utils.Calendar.PayrollPerviousStartDate(now);
                                }
                                else if (selectedVal == -30)
                                {
                                    return Utils.Calendar.LastMonthStartDate(now);
                                }
                                else
                                {
                                    return Utils.Calendar.LastYearStartDate(now);
                                }
                            }
                            else
                            {
                                return diRange.FromDate.Value;
                            }
                        }
                    }
                }
                else
                {
                    string startDateString = li.Attributes["startdate"];
                    DateTime startDate;
                    if (DateTime.TryParse(startDateString, out startDate))
                    {
                        return startDate;
                    }
                }
                return null;
            }
        }

        public DateTime? EndDate
        {
            get
            {
                System.Web.UI.WebControls.ListItem li = ddlPeriod.SelectedItem;

                if (li.Attributes["milestone"] == null)
                {
                    int selectedVal = 0;

                    if (int.TryParse(ddlPeriod.SelectedValue, out selectedVal))
                    {
                        if (selectedVal == 0)
                        {
                            return diRange.ToDate.Value;
                        }
                        else
                        {
                            var now = Utils.Generic.GetNowWithTimeZone();
                            if (selectedVal > 0)
                            {
                                if (selectedVal == 7)
                                {
                                    return Utils.Calendar.WeekEndDate(now);
                                }
                                else if (selectedVal == 15)
                                {
                                    return Utils.Calendar.PayrollCurrentEndDate(now);
                                }
                                else if (selectedVal == 30)
                                {
                                    return Utils.Calendar.MonthEndDate(now);
                                }
                                else
                                {
                                    return Utils.Calendar.YearEndDate(now);
                                }
                            }
                            else if (selectedVal < 0)
                            {
                                if (selectedVal == -7)
                                {
                                    return Utils.Calendar.LastWeekEndDate(now);
                                }
                                else if (selectedVal == -15)
                                {
                                    return Utils.Calendar.PayrollPerviousEndDate(now);
                                }
                                else if (selectedVal == -30)
                                {
                                    return Utils.Calendar.LastMonthEndDate(now);
                                }
                                else
                                {
                                    return Utils.Calendar.LastYearEndDate(now);
                                }
                            }
                            else
                            {
                                return diRange.ToDate.Value;
                            }
                        }
                    }
                }
                else
                {
                    string enddateString = li.Attributes["enddate"];
                    DateTime enddate;
                    if (DateTime.TryParse(enddateString, out enddate))
                    {
                        return enddate;
                    }
                }
                return null;
            }
        }

        public String ProjectNumber
        {
            get
            {
                return txtProjectNumber.Text.ToUpper();
            }
        }

        public string ProjectRange
        {
            get
            {
                System.Web.UI.WebControls.ListItem li = ddlPeriod.SelectedItem;
                if (!StartDate.HasValue || !EndDate.HasValue)
                {
                    return string.Empty;
                }
                else if (li.Attributes["milestone"] != null)
                {
                    string milestoneName = li.Attributes["milestone"];
                    return milestoneName + " (" + StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat) + ")";
                }
                else
                {
                    if (StartDate.Value == Utils.Calendar.MonthStartDate(StartDate.Value) && EndDate.Value == Utils.Calendar.MonthEndDate(StartDate.Value))
                    {
                        return StartDate.Value.ToString("MMMM yyyy");
                    }
                    else if (StartDate.Value == Utils.Calendar.YearStartDate(StartDate.Value) && EndDate.Value == Utils.Calendar.YearEndDate(StartDate.Value))
                    {
                        return StartDate.Value.ToString("yyyy");
                    }
                    else
                    {
                        return StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat);
                    }

                }
            }
        }

        public int? MilestoneId
        {
            get
            {
                return (ddlPeriod.SelectedItem.Attributes["milestone"] != null && ddlPeriod.SelectedValue != "*") ? (int?)Convert.ToInt32(ddlPeriod.SelectedValue) : null;
            }
        }

        public string PeriodSelected
        {
            get
            {
                return ddlPeriod.SelectedValue;
            }
        }

        public PraticeManagement.Controls.Reports.ProjectSummaryByResource ByResourceControl
        {
            get { return (PraticeManagement.Controls.Reports.ProjectSummaryByResource)ucByResource; }
        }

        #endregion

        #region Control Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var clients = DataHelper.GetAllClientsSecure(null, true, true);
                DataHelper.FillListDefault(ddlClients, "-- Select an Account -- ", clients as object[], false);
            }


        }

        protected void txtProjectNumber_OnTextChanged(object sender, EventArgs e)
        {
            ddlView.SelectedValue = "";
            ddlPeriod.SelectedValue = "*";
            PopulateddlPeriod(ProjectNumber);
            LoadActiveView();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (timeEntryReportHeader.Count == 2)
            {
                tdFirst.Attributes["class"] = "Width20Percent";
                tdSecond.Attributes["class"] = "ReportTdSecond width30P";
                tdThird.Attributes["class"] = "Width50Percent";
            }

            var now = Utils.Generic.GetNowWithTimeZone();
            diRange.FromDate = StartDate.HasValue ? StartDate : Utils.Calendar.WeekStartDate(now);
            diRange.ToDate = EndDate.HasValue ? EndDate : Utils.Calendar.WeekEndDate(now);

            if (!string.IsNullOrEmpty(PeriodSelectedFromQueryString) && !IsPostBack)
            {
                txtProjectNumber.Text = ProjectNumberFromQueryString;
                PopulateddlPeriod(ProjectNumber);
                ddlPeriod.SelectedValue = PeriodSelectedFromQueryString;
                if ((ddlPeriod.SelectedValue == "Please Select" || ddlPeriod.SelectedValue == "0") && !string.IsNullOrEmpty(StartDateFromQueryString))
                {
                    diRange.FromDate = Convert.ToDateTime(StartDateFromQueryString);
                    diRange.ToDate = Convert.ToDateTime(EndDatFromQueryString);
                    ddlPeriod.SelectedValue = "0";
                }
                ddlView.SelectedValue = "0";
            }

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
            string url = Request.Url.AbsoluteUri;
            btnCancelAndReturn.Visible = url.Contains("returnTo=");

            if (!IsPostBack)
            {
                LoadActiveView();
            }
        }

        protected void ddlView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mvProjectSummaryReport.ActiveViewIndex == 0)
            {
                ucByResource.SelectView(ucByResource.LnkbtnSummaryObject, 0);
            }

            LoadActiveView();
        }

        protected void ddlPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPeriod.SelectedValue != "0")
            {
                LoadActiveView();
            }
            else
            {
                mpeCustomDates.Show();
            }


        }

        protected void btnCustDatesOK_Click(object sender, EventArgs e)
        {
            Page.Validate(valSumDateRange.ValidationGroup);
            if (Page.IsValid)
            {
                hdnStartDate.Value = StartDate.Value.Date.ToShortDateString();
                hdnEndDate.Value = EndDate.Value.Date.ToShortDateString();
                LoadActiveView();
            }
            else
            {
                mpeCustomDates.Show();
            }
        }

        protected void btnCustDatesCancel_OnClick(object sender, EventArgs e)
        {
            diRange.FromDate = Convert.ToDateTime(hdnStartDate.Value);
            diRange.ToDate = Convert.ToDateTime(hdnEndDate.Value);
        }

        protected void btnclose_OnClick(object sender, EventArgs e)
        {
            ClearFilters();
        }

        protected void btnProjectSearch_Click(object sender, EventArgs e)
        {
            List<Project> list = ServiceCallers.Custom.Report(r => r.ProjectSearchByName(txtProjectSearch.Text)).ToList();

            btnProjectSearch.Attributes.Remove("disabled");

            if (list.Count > 0)
            {
                ltrlNoProjectsText.Visible = false;
                repProjectNamesList.Visible = true;
                repProjectNamesList.DataSource = list;
                repProjectNamesList.DataBind();
            }
            else
            {
                repProjectNamesList.Visible = false;
                ltrlNoProjectsText.Visible = true;
            }

            mpeProjectSearch.Show();

        }

        protected void lnkProjectNumber_OnClick(object sender, EventArgs e)
        {
            var lnkProjectNumber = sender as LinkButton;
            PopulateControls(lnkProjectNumber.Attributes["ProjectNumber"]);
        }

        protected void ddlClients_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ClearAndAddFirsItemForDdlProjects();

            if (ddlClients.SelectedIndex != 0)
            {
                ddlProjects.Enabled = true;

                int clientId = Convert.ToInt32(ddlClients.SelectedItem.Value);

                var projects = DataHelper.GetProjectsByClientId(clientId);

                projects = projects.OrderBy(p => p.Status.Name).ThenBy(p => p.ProjectNumber).ToArray();

                foreach (var project in projects)
                {
                    var li = new System.Web.UI.WebControls.ListItem(project.ProjectNumber + " - " + project.Name,
                                           project.ProjectNumber.ToString());

                    li.Attributes[Constants.Variables.OptionGroup] = project.Status.Name;

                    ddlProjects.Items.Add(li);

                }
            }

            mpeProjectSearch.Show();
        }

        protected void ddlProjects_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlProjects.SelectedValue != string.Empty)
            {
                var projectNumber = ddlProjects.SelectedItem.Value;

                PopulateControls(projectNumber);
            }
            else
            {
                mpeProjectSearch.Show();
            }
        }

        #endregion

        #region  Methods

        private void LoadActiveView()
        {
            if (!string.IsNullOrEmpty(ProjectNumber) && ddlView.SelectedValue != string.Empty)
            {
                mvProjectSummaryReport.ActiveViewIndex = Convert.ToInt32(ddlView.SelectedValue);
                try
                {
                    msgError.ClearMessage();
                    divWholePage.Style.Remove("display");
                    if (mvProjectSummaryReport.ActiveViewIndex == 0)
                    {
                        ucByResource.LoadActiveTabInByResource(true);
                    }
                    else if (mvProjectSummaryReport.ActiveViewIndex == 1)
                    {
                        ucByWorktype.PopulateByWorkTypeData();
                    }
                }
                catch (Exception ex)
                {
                    msgError.ShowErrorMessage(ex.Message);
                    divWholePage.Style.Add("display", "none");
                }
            }
            else
            {
                divWholePage.Style.Add("display", "none");
            }
        }

        private void ClearFilters()
        {
            ltrlNoProjectsText.Visible = repProjectNamesList.Visible = false;
            ClearAndAddFirsItemForDdlProjects();
            ddlProjects.SelectedIndex = ddlClients.SelectedIndex = 0;
            txtProjectSearch.Text = string.Empty;
            btnProjectSearch.Attributes["disabled"] = "disabled";
        }

        private void ClearAndAddFirsItemForDdlProjects()
        {
            System.Web.UI.WebControls.ListItem firstItem = new System.Web.UI.WebControls.ListItem("-- Select a Project --", string.Empty);
            ddlProjects.Items.Clear();
            ddlProjects.Items.Add(firstItem);
            ddlProjects.Enabled = false;

        }

        private void PopulateControls(string projectNumber)
        {
            ddlView.SelectedValue = "";
            ddlPeriod.SelectedValue = "*";
            txtProjectNumber.Text = projectNumber;
            PopulateddlPeriod(projectNumber);
            LoadActiveView();
            ClearFilters();
        }

        private void PopulateddlPeriod(string projectNumber)
        {
            var list = ServiceCallers.Custom.Report(r => r.GetMilestonesForProject(ProjectNumber));

            ddlPeriod.Items.Clear();
            var listItem = new System.Web.UI.WebControls.ListItem("Entire Project", "*");
            if (list.Length > 0)
            {
                DateTime projectStartDate = list.Min(m => m.StartDate);
                DateTime projectEndDate = list.Max(m => m.ProjectedDeliveryDate);
                listItem.Attributes.Add("startdate", projectStartDate.ToString("MM/dd/yyyy"));
                listItem.Attributes.Add("enddate", projectEndDate.ToString("MM/dd/yyyy"));
                listItem.Attributes.Add("milestone", listItem.Text);
            }
            ddlPeriod.Items.Add(listItem);

            //adding additional time phrases as a part of #3074
            var payrollCurrent = new System.Web.UI.WebControls.ListItem("Payroll – Current", "15");
            var payrollPrevious = new System.Web.UI.WebControls.ListItem("Payroll – Previous", "-15");
            var thisWeek = new System.Web.UI.WebControls.ListItem("This Week", "7");
            var thisMonth = new System.Web.UI.WebControls.ListItem("This Month", "30");
            var thisYear = new System.Web.UI.WebControls.ListItem("This Year", "365");
            var lastWeek = new System.Web.UI.WebControls.ListItem("Last Week", "-7");
            var lastMonth = new System.Web.UI.WebControls.ListItem("Last Month", "-30");
            var lastYear = new System.Web.UI.WebControls.ListItem("Last Year", "-365");
            ddlPeriod.Items.Add(payrollCurrent);
            ddlPeriod.Items.Add(payrollPrevious);
            ddlPeriod.Items.Add(thisWeek);
            ddlPeriod.Items.Add(thisMonth);
            ddlPeriod.Items.Add(thisYear);
            ddlPeriod.Items.Add(lastWeek);
            ddlPeriod.Items.Add(lastMonth);
            ddlPeriod.Items.Add(lastYear);

            foreach (var milestone in list)
            {

                System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem()
                {
                    Text = string.Format("{0} ({1} - {2})", milestone.Description, milestone.StartDate.ToString("MM/dd/yy"), milestone.ProjectedDeliveryDate.ToString("MM/dd/yy")),
                    Value = milestone.Id.Value.ToString()
                };
                li.Attributes.Add("startdate", milestone.StartDate.ToString("MM/dd/yyyy"));
                li.Attributes.Add("enddate", milestone.ProjectedDeliveryDate.ToString("MM/dd/yyyy"));
                li.Attributes.Add("milestone", milestone.Description);
                ddlPeriod.Items.Add(li);
            }
            var customListItem = new System.Web.UI.WebControls.ListItem("Custom Dates", "0");
            ddlPeriod.Items.Add(customListItem);
            ddlPeriod.SelectedValue = "*";
        }

        public string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        #endregion

        #region PDFVariables

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
                    headerStyle1.BackgroundColor = headerStyle2.BackgroundColor = "light-gray";
                    TdStyles contentStyle1 = new TdStyles("left", false, false, 10, 1);
                    TdStyles contentStyle2 = new TdStyles("center", false, false, 10, 1);

                    TdStyles[] headerStyleArray = { headerStyle1, headerStyle2 };
                    TdStyles[] contentStyleArray = { contentStyle1, contentStyle2 };

                    TrStyles headerRowStyle = new TrStyles(headerStyleArray);
                    TrStyles contentRowStyle = new TrStyles(contentStyleArray);

                    TrStyles[] rowStyleArray = { headerRowStyle, contentRowStyle };

                    float[] widths = { .25f, .12f, .1f, .15f, .1f, .28f };
                    _PdfProjectPersonsSummaryTableStyle = new TableStyles(widths, rowStyleArray, 100, "custom", new int[] { 245, 250, 255 });
                    _PdfProjectPersonsSummaryTableStyle.IsColoumBorders = false;
                }
                return (TableStyles)_PdfProjectPersonsSummaryTableStyle;
            }
        }

        private TableStyles _PdfProjectPersonsDetailTableStyle;

        private TableStyles PdfProjectPersonsDetailTableStyle
        {
            get
            {
                if (_PdfProjectPersonsDetailTableStyle == null)
                {
                    TdStyles HeaderStyle = new TdStyles("center", true, false, 10, 1);
                    HeaderStyle.BackgroundColor = "light-gray";
                    TdStyles ContentStyle1 = new TdStyles("left", false, false, 10, 1);
                    TdStyles ContentStyle2 = new TdStyles("center", false, false, 10, 1);

                    TdStyles[] HeaderStyleArray = { HeaderStyle };
                    TdStyles[] ContentStyleArray = { ContentStyle1, ContentStyle2, ContentStyle2, ContentStyle2, ContentStyle2, ContentStyle2, ContentStyle1 };

                    TrStyles HeaderRowStyle = new TrStyles(HeaderStyleArray);
                    TrStyles ContentRowStyle = new TrStyles(ContentStyleArray);

                    TrStyles[] RowStyleArray = { HeaderRowStyle, ContentRowStyle };
                    float[] widths = { .1f, .1f, .15f, .08f, .13f, .08f, .36f };
                    _PdfProjectPersonsDetailTableStyle = new TableStyles(widths, RowStyleArray, 100, "custom", new int[] { 245, 250, 255 });
                    _PdfProjectPersonsDetailTableStyle.IsColoumBorders = false;
                }
                return _PdfProjectPersonsDetailTableStyle;
            }
        }

        #endregion

        #region Pdf Methods

        private PdfPTable GetPdfReportHeader(List<PersonLevelGroupedHours> personLevelGroupedHoursList, Project project, int? personId)
        {
            List<PersonLevelGroupedHours> _personLevelGroupedHoursList = personLevelGroupedHoursList;
            if (personId.HasValue)
            {
                _personLevelGroupedHoursList = personLevelGroupedHoursList.Where(p => p.Person.Id == personId.Value).ToList();
            }

            double billableHours = _personLevelGroupedHoursList.Sum(p => p.DayTotalHours != null ? p.DayTotalHours.Sum(d => d.BillableHours) : p.BillableHours);
            double nonBillableHours = _personLevelGroupedHoursList.Sum(p => p.NonBillableHours);
            double projectedHours = _personLevelGroupedHoursList.Sum(p => p.ForecastedHours);

            PdfPTable outerTable = new PdfPTable(4);
            outerTable.WidthPercentage = 100;
            float[] outerWidths = { .5f, .2f, .2f, .1f };
            outerTable.SetWidths(outerWidths);

            var boldBaseFont = iTextSharp.text.pdf.BaseFont.CreateFont();
            var boldGrayFont = new Font(boldBaseFont, 14, Font.BOLD, BaseColor.GRAY);
            var boldFont = new Font(boldBaseFont, 14, Font.BOLD);
            var normalBaseFont = iTextSharp.text.pdf.BaseFont.CreateFont();
            var normalFont9 = new Font(normalBaseFont, 8, Font.NORMAL);
            var normalFont12 = new Font(normalBaseFont, 10, Font.NORMAL);

            //inner table1
            PdfPTable innerTable1 = new PdfPTable(1);
            innerTable1.WidthPercentage = 100;
            PdfPCell headerText1;
            PdfPCell headerText2;
            PdfPCell headerText3;
            PdfPCell headerText4;


            if (personId.HasValue)
            {
                headerText1 = new PdfPCell(new Phrase(_personLevelGroupedHoursList[0].Person.PersonLastFirstName, boldFont));
                headerText2 = new PdfPCell(new Phrase(project.Client.Name + ">" + project.Group.Name, boldGrayFont));
                headerText3 = new PdfPCell(new Phrase(string.Format("{0} - {1} ({2})", project.ProjectNumber, project.Name, string.IsNullOrEmpty(project.BillableType) ? project.Status.Name : project.Status.Name + ", " + project.BillableType), boldFont));
                headerText4 = new PdfPCell(new Phrase(ProjectRange, boldFont));
            }
            else
            {
                headerText1 = new PdfPCell(new Phrase(project.Client.Name + ">" + project.Group.Name, boldGrayFont));
                headerText2 = new PdfPCell(new Phrase(string.Format("{0} - {1}", project.ProjectNumber, project.Name), boldFont));
                headerText3 = new PdfPCell(new Phrase(string.IsNullOrEmpty(project.BillableType) ? project.Status.Name : project.Status.Name + ", " + project.BillableType, boldFont));
                headerText4 = new PdfPCell(new Phrase(ProjectRange, boldFont));
            }

            headerText1.BorderWidth = headerText2.BorderWidth = headerText3.BorderWidth = headerText4.BorderWidth = 0f;

            innerTable1.AddCell(headerText1);
            innerTable1.CompleteRow();
            innerTable1.AddCell(headerText2);
            innerTable1.CompleteRow();
            innerTable1.AddCell(headerText3);
            innerTable1.CompleteRow();
            innerTable1.AddCell(headerText4);
            innerTable1.CompleteRow();

            //inner table2
            PdfPTable innerTable2 = new PdfPTable(1);
            innerTable2.WidthPercentage = 100;
            PdfPCell headerText5 = new PdfPCell(new Phrase("Projected Hours", normalFont12));
            PdfPCell headerText6 = new PdfPCell(new Phrase(projectedHours.ToString(Constants.Formatting.DoubleValue), boldFont));
            headerText5.VerticalAlignment = Element.ALIGN_BOTTOM;
            headerText6.VerticalAlignment = Element.ALIGN_TOP;
            headerText5.HorizontalAlignment = headerText6.HorizontalAlignment = Element.ALIGN_CENTER;
            headerText5.BorderWidth = headerText6.BorderWidth = 0f;
            headerText5.FixedHeight = headerText6.FixedHeight = 30f;

            innerTable2.AddCell(headerText5);
            innerTable2.CompleteRow();
            innerTable2.AddCell(headerText6);
            innerTable2.CompleteRow();

            //inner table3
            PdfPTable innerTable3 = new PdfPTable(1);
            innerTable3.WidthPercentage = 100;
            PdfPCell headerText7 = new PdfPCell(new Phrase("Total Actual Hours", normalFont12));
            PdfPCell headerText8 = new PdfPCell(new Phrase((billableHours + nonBillableHours).ToString(Constants.Formatting.DoubleValue), boldFont));
            headerText7.VerticalAlignment = Element.ALIGN_BOTTOM;
            headerText8.VerticalAlignment = Element.ALIGN_TOP;
            headerText7.HorizontalAlignment = headerText8.HorizontalAlignment = Element.ALIGN_CENTER;
            headerText7.FixedHeight = headerText8.FixedHeight = 30f;
            headerText7.BorderWidth = headerText8.BorderWidth = 0f;

            innerTable3.AddCell(headerText7);
            innerTable3.CompleteRow();
            innerTable3.AddCell(headerText8);
            innerTable3.CompleteRow();


            //inner table4
            PdfPTable innerTable4 = new PdfPTable(1);
            innerTable4.WidthPercentage = 100;
            PdfPCell headerText9 = new PdfPCell(new Phrase("BILLABLE", normalFont9));
            PdfPCell headerText10 = new PdfPCell(new Phrase(billableHours.ToString(Constants.Formatting.DoubleValue), normalFont9));
            PdfPCell headerText11 = new PdfPCell(new Phrase("NON-BILLABLE", normalFont9));
            PdfPCell headerText12 = new PdfPCell(new Phrase(nonBillableHours.ToString(Constants.Formatting.DoubleValue), normalFont9));
            headerText9.VerticalAlignment = headerText11.VerticalAlignment = Element.ALIGN_BOTTOM;
            headerText10.VerticalAlignment = headerText12.VerticalAlignment = Element.ALIGN_TOP;
            headerText9.HorizontalAlignment = headerText10.HorizontalAlignment = headerText11.HorizontalAlignment = headerText12.HorizontalAlignment = Element.ALIGN_CENTER;
            headerText9.FixedHeight = headerText10.FixedHeight = headerText11.FixedHeight = headerText12.FixedHeight = 15f;
            headerText9.BorderWidth = headerText10.BorderWidth = headerText11.BorderWidth = headerText12.BorderWidth = 0f;

            innerTable4.AddCell(headerText9);
            innerTable4.CompleteRow();
            innerTable4.AddCell(headerText10);
            innerTable4.CompleteRow();
            innerTable4.AddCell(headerText11);
            innerTable4.CompleteRow();
            innerTable4.AddCell(headerText12);
            innerTable4.CompleteRow();

            PdfPCell innerTableCell1 = new PdfPCell(innerTable1);
            PdfPCell innerTableCell2 = new PdfPCell(innerTable2);
            PdfPCell innerTableCell3 = new PdfPCell(innerTable3);
            PdfPCell innerTableCell4 = new PdfPCell(innerTable4);
            innerTableCell1.BorderWidth = innerTableCell2.BorderWidth = innerTableCell3.BorderWidth = innerTableCell4.BorderWidth = 0f;
            innerTableCell1.PaddingBottom = 20f;

            outerTable.AddCell(innerTableCell1);
            outerTable.AddCell(innerTableCell2);
            outerTable.AddCell(innerTableCell3);
            outerTable.AddCell(innerTableCell4);
            outerTable.CompleteRow();
            return outerTable;
        }

        private String GetSummaryReportDataInPdfString(List<PersonLevelGroupedHours> data)
        {
            String _pdfProjectPersonsSummary = string.Empty;
            if (data.Count > 0)
            {
                //Header
                _pdfProjectPersonsSummary = string.Format("Resource{0}ProjectRole{0}Projected Hours{0}Billable{0}Non-Billable{0}Actual Hours{0}Billable Hours Variance{1}", ColoumSpliter, RowSpliter);

                var list = data.OrderBy(p => p.Person.PersonLastFirstName);

                //Data
                foreach (var byPerson in list)
                {
                    _pdfProjectPersonsSummary += String.Format("{0}{7}{1}{7}{2}{7}{3}{7}{4}{7}{5}{7}{6}{8}",
                        byPerson.Person.PersonLastFirstName,
                        byPerson.Person.ProjectRoleName,
                        GetDoubleFormat(byPerson.ForecastedHours),
                        GetDoubleFormat(byPerson.BillableHours),
                        GetDoubleFormat(byPerson.NonBillableHours),
                        GetDoubleFormat(byPerson.TotalHours),
                        (byPerson.BillableHoursVariance > 0) ? "+" + GetDoubleFormat(byPerson.BillableHoursVariance) : GetDoubleFormat(byPerson.BillableHoursVariance),
                        ColoumSpliter,
                        RowSpliter);
                }
            }

            return _pdfProjectPersonsSummary;
        }

        private String GetDetailReportDataInPdfString(List<PersonLevelGroupedHours> data, int personId)
        {
            String _pdfProjectPersonDetail = string.Empty;
            if (data.Count > 0)
            {
                List<PersonLevelGroupedHours> list = data.Where(p => p.Person.Id == personId).ToList();

                if (list[0].DayTotalHours != null)
                {
                    //Header
                    _pdfProjectPersonDetail = String.Format("Date{0}WorkType{0}WorkType Name{0}Billable{0}Non-Billable{0}Actual Hours{0}Note{1}", ColoumSpliter, RowSpliter);

                    //Data
                    foreach (var byPerson in list)
                    {
                        if (byPerson.DayTotalHours != null)
                        {
                            foreach (var byDateList in byPerson.DayTotalHours)
                            {
                                foreach (var byWorkType in byDateList.DayTotalHoursList)
                                {
                                    _pdfProjectPersonDetail += String.Format("{0}{7}{1}{7}{2}{7}{3}{7}{4}{7}{5}{7}{6}{8}",
                                        byDateList.Date.ToString("MM/dd/yyyy"),
                                        byWorkType.TimeType.Code,
                                        byWorkType.TimeType.Name,
                                        GetDoubleFormat(byWorkType.BillableHours),
                                        GetDoubleFormat(byWorkType.NonBillableHours),
                                        GetDoubleFormat(byWorkType.TotalHours),
                                        byWorkType.NoteForExport,
                                        ColoumSpliter,
                                        RowSpliter);
                                }
                            }
                        }
                    }
                }
                else
                {
                    _pdfProjectPersonDetail = string.Empty;
                }
            }

            return _pdfProjectPersonDetail;
        }

        private int GetPageCount(HtmlToPdfBuilder builder, List<PersonLevelGroupedHours> summaryData, List<PersonLevelGroupedHours> detailData, Project project)
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
            if (summaryData.Count > 0)
            {
                document.Add((IElement)GetPdfReportHeader(summaryData, project, null));
                string reportDataInPdfString = GetSummaryReportDataInPdfString(summaryData);
                var table = builder.GetPdftable(reportDataInPdfString, PdfProjectPersonsSummaryTableStyle, RowSpliter, ColoumSpliter);
                document.Add((IElement)table);
                document.NewPage();
                var personIds = detailData.Select(p => p.Person.Id).Distinct();
                int i = 0, personIdsCount = personIds.Count();
                foreach (int personId in personIds)
                {
                    i++;
                    string reportDetailDataInPdfString = GetDetailReportDataInPdfString(detailData, personId);
                    IElement detailTable = null;
                    if (!string.IsNullOrEmpty(reportDetailDataInPdfString))
                    {
                        detailTable = builder.GetPdftable(reportDetailDataInPdfString, PdfProjectPersonsDetailTableStyle, RowSpliter, ColoumSpliter);
                    }
                    else
                    {
                        detailTable = PDFHelper.GetPdfTableWithGivenString("There are no Time Entries entered towards this project for the selected date range.");
                    }
                    document.Add((IElement)GetPdfReportHeader(detailData, project, personId));
                    document.Add((IElement)detailTable);
                    if (i < personIdsCount)
                    {
                        document.NewPage();
                    }
                }
            }
            else
            {
                document.Add((IElement)PDFHelper.GetPdfHeaderLogo());
            }
            return writer.CurrentPageNumber;
        }

        private byte[] RenderPdf(HtmlToPdfBuilder builder, List<PersonLevelGroupedHours> summaryData, List<PersonLevelGroupedHours> detailData, Project project)
        {
            int pageCount = GetPageCount(builder, summaryData, detailData, project);
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
            if (summaryData.Count > 0)
            {
                document.Add((IElement)GetPdfReportHeader(summaryData, project, null));
                string reportDataInPdfString = GetSummaryReportDataInPdfString(summaryData);
                var table = builder.GetPdftable(reportDataInPdfString, PdfProjectPersonsSummaryTableStyle, RowSpliter, ColoumSpliter);
                document.Add((IElement)table);
                document.NewPage();
                var personIds = detailData.Select(p => p.Person.Id).Distinct();
                int i = 0, personIdsCount = personIds.Count();
                foreach (int personId in personIds)
                {
                    i++;
                    string reportDetailDataInPdfString = GetDetailReportDataInPdfString(detailData, personId);
                    IElement detailTable = null;
                    if (!string.IsNullOrEmpty(reportDetailDataInPdfString))
                    {
                        detailTable = builder.GetPdftable(reportDetailDataInPdfString, PdfProjectPersonsDetailTableStyle, RowSpliter, ColoumSpliter);
                    }
                    else
                    {
                        detailTable = PDFHelper.GetPdfTableWithGivenString("There are no Time Entries entered towards this project for the selected date range.");
                    }
                    document.Add((IElement)GetPdfReportHeader(detailData, project, personId));
                    document.Add((IElement)detailTable);
                    if (i < personIdsCount)
                    {
                        document.NewPage();
                    }
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
            var project = ServiceCallers.Custom.Project(p => p.GetProjectShortByProjectNumber(ProjectNumber, MilestoneId, StartDate, EndDate));
            List<PersonLevelGroupedHours> summaryData = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByResource(ProjectNumber,
                MilestoneId, PeriodSelected == "*" ? null : StartDate,
                PeriodSelected == "*" ? null : EndDate, ByResourceControl.cblProjectRolesControl.SelectedItemsXmlFormat)).ToList();
            List<PersonLevelGroupedHours> detailData = ServiceCallers.Custom.Report(r => r.ProjectDetailReportByResource(ProjectNumber, MilestoneId,
               PeriodSelected == "*" ? null : StartDate, PeriodSelected == "*" ? null : EndDate,
               ByResourceControl.cblProjectRolesControl.SelectedItemsXmlFormat)).ToList();

            HtmlToPdfBuilder builder = new HtmlToPdfBuilder(iTextSharp.text.PageSize.A4_LANDSCAPE);
            string filename = string.Format("{0}_{1}_{2}.pdf", project.ProjectNumber, project.Name, "_ByResource").Replace(' ', '_');
            byte[] pdfDataInBytes = this.RenderPdf(builder, summaryData, detailData, project);

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

        #endregion
    }
}

