using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Xsl;
using AjaxControlToolkit;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.ProjectService;
using PraticeManagement.Security;
using PraticeManagement.Utils;
using PraticeManagement.Configuration;
using System.Linq;
using System.Drawing;
namespace PraticeManagement
{
    public partial class Projects : PracticeManagementPageBase
    {
        #region Constants
        private const string CurrencyDisplayFormat = "$###,###,###,###,###,##0";
        private const string CurrencyExcelReportFormat = "$####,###,###,###,###,##0.00";
        private const int NumberOfFixedColumns = 6;
        private const int ProjectStateColumnIndex = 0;
        private const int ProjectNumberColumnIndex = 1;
        private const int ClientNameColumnIndex = 2;
        private const int ProjectNameColumnIndex = 3;
        private const int StartDateColumnIndex = 4;
        private const int EndDateColumnIndex = 5;

        private const int TabNameColumnIndex = 0;

        private const int MaxPeriodLength = 24;

        private const int FS_RevenueRowIndex = 0;
        private const int FS_COGSRowIndex = 1;
        private const int FS_GrossMarginRowIndex = 2;
        private const int FS_TargetMarginRowIndex = 3;
        private const int FS_ExpensesRowIndex = 4;
        private const int FS_SalesCommissionsRowIndex = 5;
        private const int FS_PMCommissionsRowIndex = 6;
        private const int FS_BenchRowIndex = 7;
        private const int FS_AdminRowIndex = 8;
        private const int FS_NetProfitRowIndex = 9;

        private const int CR_GrossMarginEligibleForCommissions = 0;
        private const int CR_SalesCommissionsIndex = 1;
        private const int CR_PMCommissionsIndex = 2;
        private const int CR_AvgBillRateIndex = 3;
        private const int CR_AvgPayRateIndex = 4;

        private const string FS_RevenueRowName = "Revenue";
        private const string FS_COGSRowName = "COGS";
        private const string FS_GrossMarginRowName = "Gross margin";
        private const string FS_TargetMarginRowName = "Margin %";
        private const string FS_ExpensesRowName = "Expenses";
        private const string FS_SalesCommissionsRowName = "Sales Commissions";
        private const string FS_PMCommissionsRowName = "PM Commissions";
        private const string FS_BenchRowName = "Bench";
        private const string FS_AdminRowName = "Admin";
        private const string FS_NetProfitRowName = "Net Profit";

        private const string ButtonClientNameId = "btnClientName";
        private const string LabelProjectNumberId = "lblProjectNumber";
        private const string ButtonProjectNameId = "btnProjectName";
        private const string LabelStartDateId = "lblStartDate";
        private const string LabelEndDateId = "lblEndDate";


        private const string LabelHeaderIdFormat = "lblHeader{0}";
        private const string LabelHeaderIdToolTipView = "{0} Monthly Report";
        private const string PanelHeaderIdFormat = "pnlHeader{0}";
        private const string PanelReportIdFormat = "pnlReport{0}";
        private const string CloseReportButtonIdFormat = "btnCloseReport{0}";
        private const string ReportCellIdFormat = "cellReport{0}";

        private const string TotalHeaderFormat = "Total ({0})";
        private const string EntireProjectPeriod = "Entire Project Period";
        private const string SelectedText = "selected";
        private const string STR_SortExpression = "SortExpression";
        private const string STR_SortDirection = "SortDirection";
        private const string STR_SortColumnId = "SortColumnId";
        private const string ToolTipView = "{1}{0}<nobr>Buyer Name:&nbsp;{5}</nobr>{0}Start: {2:d}{0}End: {3:d}{4}";
        private const string AppendPersonFormat = "{0}{1} {2}";
        private const string CompPerfDataCssClass = "CompPerfData";
        private const string CompPerfHeaderDivCssClass = "ie-bg no-wrap";
        private const string HintDivCssClass = "hint";
        private const string AlternatingRowCssClass = "rowEven";
        private const string OneGreaterSeniorityExistsKey = "ProjectsListOneGreaterSeniorityExists";

        private const string CompanyPerformanceFilterKey = "CompanyPerformanceFilterKey";

        protected const string PagerNextCommand = "Next";
        protected const string PagerPrevCommand = "Prev";

        #endregion

        #region Fields

        private bool userIsAdministrator;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates if there were ata least one field that was hidden
        /// </summary>
        private bool OneGreaterSeniorityExists
        {
            get
            {
                if (Session[OneGreaterSeniorityExistsKey] == null)
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean(Session[OneGreaterSeniorityExistsKey]);
                }
            }
            set
            {
                Session[OneGreaterSeniorityExistsKey] = value;
            }
        }

        /// <summary>
        /// Gets a selected period start.
        /// </summary>
        private DateTime PeriodStart
        {
            get
            {
                return diRange.FromDate.Value;
            }
        }

        /// <summary>
        /// Gets a selected period end.
        /// </summary>
        private DateTime PeriodEnd
        {
            get
            {
                return diRange.ToDate.Value;
            }
        }

        private string SelectedClientIds
        {
            get
            {
                return cblClient.SelectedItems;
            }
            set
            {
                cblClient.SelectedItems = value;
            }
        }

        private string SelectedSalespersonIds
        {
            get
            {
                return cblSalesperson.SelectedItems;
            }
            set
            {
                cblSalesperson.SelectedItems = value;
            }
        }

        private string SelectedPracticeIds
        {
            get
            {
                return cblPractice.SelectedItems;
            }
            set
            {
                cblPractice.SelectedItems = value;
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
            set
            {
                cblProjectOwner.SelectedItems = value;
            }
        }


        /// <summary>
        /// Gets a list of projects to be displayed.
        /// </summary>
        private Project[] ProjectList
        {
            get
            {
                CompanyPerformanceState.Filter = GetFilterSettings();
                return CompanyPerformanceState.ProjectList;
            }
        }

        private int ListViewSortColumnId
        {
            get { return ViewState[STR_SortColumnId] != null ? (int)ViewState[STR_SortColumnId] : ProjectNumberColumnIndex; }
            set { ViewState[STR_SortColumnId] = value; }
        }

        private string PrevListViewSortExpression
        {
            get { return ViewState[STR_SortExpression] as string ?? string.Empty; }
            set { ViewState[STR_SortExpression] = value; }
        }

        private string ListViewSortDirection
        {
            get { return ViewState[STR_SortDirection] as string ?? "Ascending"; }
            set { ViewState[STR_SortDirection] = value; }
        }

        /// <summary>
        /// Gets a text to be searched for.
        /// </summary>
        public string SearchText
        {
            get
            {
                return txtSearchText.Text;
            }
        }

        public bool IsSearchClicked
        {
            get;
            set;
        }

        #endregion

        #region Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            userIsAdministrator = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);

            if (!IsPostBack)
            {
                // Version information
                SetCurrentAssemblyVersion();
            }
            else
            {
                PreparePeriodView();
            }

            //custPeriodLengthLimit.ErrorMessage = custPeriodLengthLimit.ToolTip = string.Format(custPeriodLengthLimit.ErrorMessage, MaxPeriodLength);
        }

        private void SetCurrentAssemblyVersion()
        {
            string version = Generic.SystemVersion;
            lblCurrentVersion.Text = version;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Security
            if (!userIsAdministrator)
            {
                Person current = DataHelper.CurrentPerson;
                int? personId = current != null ? current.Id : null;

                bool userIsSalesperson =
                    Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SalespersonRoleName);
                bool userIsPracticeManager =
                    Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.PracticeManagerRoleName);
                bool userIsDirector =
                    Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName);

                SelectItemInControl(userIsSalesperson, cblSalesperson, personId);
                SelectItemInControl((userIsPracticeManager || userIsDirector), cblProjectOwner, personId);// #2817: userIsDirector is added as per the requirement.

                //Label lblViewingRecords = (Label)GetPager().FindControl("currentPage");
                //lblViewingRecords.Text = lvProjects.Items.Count.ToString();
            }

            // cblSalesperson.Enabled = cblPracticeManager.Enabled = userIsAdministrator;

            // Client side validator is not applicable here.
            reqSearchText.IsValid = true;

            SaveFilterSettings();
            lblCustomDateRange.Text = string.Format("({0}&nbsp;-&nbsp;{1})",
                    diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat),
                    diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat)
                    );
            if (ddlPeriod.SelectedValue == "0")
            {
                lblCustomDateRange.Attributes.Remove("class");
                imgCalender.Attributes.Remove("class");
            }
            else
            {
                lblCustomDateRange.Attributes.Add("class", "displayNone");
                imgCalender.Attributes.Add("class", "displayNone");
            }

            var pager = GetPager();
            if (pager != null && !IsSearchClicked)
            {
                if (ddlView.SelectedValue != "1")
                {
                    if (pager.PageSize != Convert.ToInt32(ddlView.SelectedValue))
                    {
                        Response.Redirect(Request.Url.AbsoluteUri);
                    }
                }
                else
                {
                    if (pager.PageSize != pager.TotalRowCount)
                    {
                        Response.Redirect(Request.Url.AbsoluteUri);
                    }
                }
            }

            hdnStartDate.Value = diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat);
            hdnEndDate.Value = diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat);
            hdnStartDateTxtBoxId.Value = (diRange.FindControl("tbFrom") as TextBox).ClientID;
            hdnEndDateTxtBoxId.Value = (diRange.FindControl("tbTo") as TextBox).ClientID;
        }

        public void ddlPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            int periodSelected = Convert.ToInt32(ddlPeriod.SelectedValue);

            SetPeriodSelection(periodSelected);

            ValidateAndDisplay();

        }

        public void ddlView_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetddlView();

            lvProjects.DataSource = ProjectList;
            lvProjects.DataBind();

            StyledUpdatePanel.Update();
        }

        public void btnSearch_Clicked(object sender, EventArgs e)
        {

            var projectList = ProjectList.Where(pro => pro.Name.ToLower().Contains(txtSearchText.Text.ToLower()) ||
                                                pro.Client.Name.ToLower().Contains(txtSearchText.Text.ToLower()) ||
                                                (pro.Milestones != null && pro.Milestones.Any(m => m.Description.ToLower().Contains(txtSearchText.Text.ToLower())))
                                            ).ToList();
            if (projectList != null)
            {
                CompanyPerformanceState.ProjectList = projectList.ToArray<Project>();
                lvProjects.DataSource = ProjectList;
                lvProjects.DataBind();
            }
            IsSearchClicked = true;

            StyledUpdatePanel.Update();
        }

        private void SetddlView()
        {
            DataPager pager = GetPager();
            if (pager != null)
            {
                if (ddlView.SelectedValue != "1")
                {
                    pager.SetPageProperties(0, Convert.ToInt32(ddlView.SelectedValue), false);
                }
                else
                {
                    pager.SetPageProperties(0, pager.TotalRowCount, false);
                }
            }
        }

        //private void UpdateToDate()
        //{
        //    DropDownList monthToControl = mpPeriodEnd.FindControl("ddlMonth") as DropDownList;
        //    DropDownList yearToControl = mpPeriodEnd.FindControl("ddlYear") as DropDownList;

        //    //remove all the year items less than mpFromControl.SelectedYear in mpToControl.
        //    RemoveToControls(mpPeriodStart.SelectedYear, yearToControl);

        //    if (mpPeriodStart.SelectedYear >= mpPeriodEnd.SelectedYear)
        //    {
        //        //remove all the month items less than mpFromControl.SelectedMonth in mpToControl.
        //        RemoveToControls(mpPeriodStart.SelectedMonth, monthToControl);

        //        if (mpPeriodStart.SelectedYear > mpPeriodEnd.SelectedYear ||
        //            mpPeriodStart.SelectedMonth > mpPeriodEnd.SelectedMonth)
        //        {
        //            mpPeriodEnd.SelectedYear = mpPeriodStart.SelectedYear;
        //            mpPeriodEnd.SelectedMonth = mpPeriodStart.SelectedMonth;
        //        }
        //    }
        //    else
        //    {
        //        RemoveToControls(0, monthToControl);
        //    }
        //}

        private void RemoveToControls(int FromSelectedValue, DropDownList yearToControl)
        {
            foreach (ListItem toYearItem in yearToControl.Items)
            {
                var toYearItemInt = Convert.ToInt32(toYearItem.Value);
                if (toYearItemInt < FromSelectedValue)
                {
                    toYearItem.Enabled = false;
                }
                else
                {
                    toYearItem.Enabled = true;
                }
            }
        }

        private void SetPeriodSelection(int periodSelected)
        {
            DateTime currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, Constants.Dates.FirstDay);
            if (periodSelected > 0)
            {
                DateTime startMonth = new DateTime();
                DateTime endMonth = new DateTime();

                if (periodSelected < 13)
                {
                    startMonth = currentMonth;
                    endMonth = currentMonth.AddMonths(Convert.ToInt32(ddlPeriod.SelectedValue) - 1);
                }
                else
                {
                    Dictionary<string, DateTime> fPeriod = DataHelper.GetFiscalYearPeriod(currentMonth);
                    startMonth = fPeriod["StartMonth"];
                    endMonth = fPeriod["EndMonth"];
                }
                diRange.FromDate = startMonth;
                diRange.ToDate = new DateTime(endMonth.Year, endMonth.Month, DateTime.DaysInMonth(endMonth.Year, endMonth.Month));
            }
            else if (periodSelected < 0)
            {
                DateTime startMonth = new DateTime();
                DateTime endMonth = new DateTime();

                if (periodSelected > -13)
                {
                    startMonth = currentMonth.AddMonths(Convert.ToInt32(ddlPeriod.SelectedValue) + 1);
                    endMonth = currentMonth;
                }
                else
                {
                    Dictionary<string, DateTime> fPeriod = DataHelper.GetFiscalYearPeriod(currentMonth.AddYears(-1));
                    startMonth = fPeriod["StartMonth"];
                    endMonth = fPeriod["EndMonth"];
                }
                diRange.FromDate = startMonth;
                diRange.ToDate = new DateTime(endMonth.Year, endMonth.Month, DateTime.DaysInMonth(endMonth.Year, endMonth.Month));
            }
            else
            {
                mpeCustomDates.Show();
            }
        }


        /// <summary>
        /// Selects item in list control according to some condition
        /// </summary>
        /// <param name="condition">If true, the item will be selected</param>
        /// <param name="targetControl">Control to select item in</param>
        /// <param name="lookFor">Value to look for in the control</param>
        private static void SelectItemInControl(bool condition, ListControl targetControl, int? lookFor)
        {
            if (condition)
            {
                ListItem targetItem = targetControl.Items.FindByValue(lookFor == null ? null : lookFor.ToString());
                if (targetItem != null)
                    targetItem.Selected = true;
            }
        }

        /// <summary>
        /// Stores a current filter set.
        /// </summary>
        private void SaveFilterSettings()
        {
            CompanyPerformanceFilterSettings filter = GetFilterSettings();
            SerializationHelper.SerializeCookie(filter, CompanyPerformanceFilterKey);
        }

        /// <summary>
        /// Gets a current filter settings.
        /// </summary>
        /// <returns>The <see cref="CompanyPerformanceFilterSettings"/> with a current filter.</returns>
        private CompanyPerformanceFilterSettings GetFilterSettings()
        {
            var filter =
                 new CompanyPerformanceFilterSettings
                 {
                     StartYear = diRange.FromDate.Value.Year,
                     StartMonth = diRange.FromDate.Value.Month,
                     StartDay = diRange.FromDate.Value.Day,
                     EndYear = diRange.ToDate.Value.Year,
                     EndMonth = diRange.ToDate.Value.Month,
                     EndDay = diRange.ToDate.Value.Day,
                     ClientIdsList = SelectedClientIds,
                     ProjectOwnerIdsList = SelectedProjectOwnerIds,
                     PracticeIdsList = SelectedPracticeIds,
                     SalespersonIdsList = SelectedSalespersonIds,
                     ProjectGroupIdsList = SelectedGroupIds,
                     ShowActive = chbActive.Checked,
                     ShowCompleted = chbCompleted.Checked,
                     ShowProjected = chbProjected.Checked,
                     ShowInternal = chbInternal.Checked,
                     ShowExperimental = chbExperimental.Checked,
                     ShowInactive = chbInactive.Checked,
                     PeriodSelected = Convert.ToInt32(ddlPeriod.SelectedValue),
                     ViewSelected = Convert.ToInt32(ddlView.SelectedValue),

                     CalculateRangeSelected = (ProjectCalculateRangeType)Enum.Parse(typeof(ProjectCalculateRangeType), ddlCalculateRange.SelectedValue),
                     HideAdvancedFilter = false
                 };
            return filter;
        }

        protected void lvProjects_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var row = e.Item.FindControl("boundingRow") as HtmlTableRow;
                if (row.Cells.Count == NumberOfFixedColumns)
                {
                    var monthsInPeriod = GetPeriodLength();
                    for (int i = 0; i < monthsInPeriod + 1; i++)   // + 1 means a cell for total column
                    {
                        var td = new HtmlTableCell() { };
                        td.Attributes["class"] = "CompPerfMonthSummary";
                        row.Cells.Insert(row.Cells.Count, td);
                    }
                }

                var project = (e.Item as ListViewDataItem).DataItem as Project;
                var rowVisible = false;
                if (project != null)
                {
                    // Determine whether to display the project in the list.
                    rowVisible = IsProjectVisible(project);


                    string cssClass = ProjectHelper.GetIndicatorClassByStatusId(project.Status.Id);
                    /*row.Attributes["class"] = row.Attributes["class"] == AlternatingRowCssClass ?
                                                string.IsNullOrEmpty(cssClass) ? row.Attributes["class"] : string.Format("{0} {1}", cssClass, AlternatingRowCssClass) :
                                                cssClass;*/

                    if (project.Status.Id == 3 && project.Attachment == null)
                    {
                        cssClass = "ActiveProjectWithoutSOW";
                    }

                    SeniorityAnalyzer personListAnalyzer = null;

                    if (project.Id.HasValue)
                    {
                        /* 
                         * TEMPORARY COMMENT 
                         * Will be then used to fix #1257
                         */
                        personListAnalyzer = new SeniorityAnalyzer(DataHelper.CurrentPerson);
                        personListAnalyzer.OneWithGreaterSeniorityExists(project.ProjectPersons);

                        FillProjectStateCell(e.Item, cssClass, project.Status);
                        FillProjectNumberCell(e.Item, project);
                        FillClientNameCell(e.Item, project);
                        FillProjectNameCell(e.Item, project);
                        FillProjectStartCell(e.Item, project);
                        FillProjectEndCell(e.Item, project);
                    }

                    FillMonthCells(row, project, personListAnalyzer);
                    FillTotalsCell(row, project, personListAnalyzer);
                }
            }
        }

        private void FillMonthCells(HtmlTableRow row, Project project, SeniorityAnalyzer personListAnalyzer)
        {
            DateTime monthBegin = GetMonthBegin();

            int periodLength = GetPeriodLength();

            // Displaying the interest values (main cell data)
            for (int i = NumberOfFixedColumns, k = 0;
                k < periodLength;
                i++, k++, monthBegin = monthBegin.AddMonths(1))
            {
                DateTime monthEnd = GetMonthEnd(ref monthBegin);

                foreach (KeyValuePair<DateTime, ComputedFinancials> interestValue in
                    project.ProjectedFinancialsByMonth)
                {
                    if (IsInMonth(interestValue.Key, monthBegin, monthEnd))
                    {
                        //row.Cells[i].Wrap = false;
                        if (project.Id.HasValue)
                        {
                            // Project.Id != null is for regular projects only
                            bool greaterSeniorityExists = personListAnalyzer != null && personListAnalyzer.GreaterSeniorityExists;
                            string grossMargin = greaterSeniorityExists ?
                                Resources.Controls.HiddenCellText
                                :
                                interestValue.Value.GrossMargin.Value.ToString(CurrencyDisplayFormat);

                            row.Cells[i].InnerHtml = GetMonthReportTableAsHtml(interestValue.Value.Revenue, grossMargin);
                            //string.Format("<div class=\"cell-pad\">{0}</div>",
                            //string.Format(Resources.Controls.ProjectInterestFormat,
                            //interestValue.Value.Revenue,
                            //grossMargin));

                            //if (greaterSeniorityExists)
                            //    OneGreaterSeniorityExists = true;
                        }
                    }
                }
            }
        }

        private static DateTime GetMonthEnd(ref DateTime monthBegin)
        {
            return new DateTime(monthBegin.Year,
                    monthBegin.Month,
                    DateTime.DaysInMonth(monthBegin.Year, monthBegin.Month));
        }

        private DateTime GetMonthBegin()
        {
            return new DateTime(diRange.FromDate.Value.Year,
                    diRange.FromDate.Value.Month,
                    Constants.Dates.FirstDay);
        }

        private void FillTotalsCell(HtmlTableRow row, Project project, SeniorityAnalyzer personListAnalyzer)
        {
            // Project totals
            PracticeManagementCurrency totalRevenue = 0M;
            PracticeManagementCurrency totalMargin = 0M;

            // Calculate Total Revenue and Margin for current Project
            if (project.ComputedFinancials != null)
            {
                totalRevenue = project.ComputedFinancials.Revenue;
                totalMargin = project.ComputedFinancials.GrossMargin;
            }

            // Render Total Revenue and Margin for current Project
            if (project.Id.HasValue)
            {
                // Displaying the project totals
                bool greaterSeniorityExists = personListAnalyzer != null && personListAnalyzer.GreaterSeniorityExists;
                string strTotalMargin = greaterSeniorityExists ?
                                Resources.Controls.HiddenCellText
                                :
                                totalMargin.Value.ToString(CurrencyDisplayFormat);

                row.Cells[row.Cells.Count - 1].InnerHtml = GetMonthReportTableAsHtml(totalRevenue, strTotalMargin);
                //string.Format(
                //Resources.Controls.ProjectInterestFormat, totalRevenue, strTotalMargin);
                row.Cells[row.Cells.Count - 1].Attributes["class"] = "CompPerfTotalSummary";
                //if (greaterSeniorityExists)
                //    OneGreaterSeniorityExists = true;
            }
        }

        private bool IsProjectVisible(Project project)
        {
            return
                // Project has no milestones - we should display it anyware
                !project.StartDate.HasValue || !project.EndDate.HasValue ||
                // Project has some milestone(s) and we determine whether it falls into the selected date range
                (project.StartDate.Value <= PeriodEnd && project.EndDate >= PeriodStart);
        }

        private static void FillProjectEndCell(ListViewItem e, Project project)
        {
            var row = e.FindControl("boundingRow") as HtmlTableRow;
            // Project End date cell content
            var lblEndDate = e.FindControl(LabelEndDateId) as Label;
            if (project.EndDate.HasValue)
            {
                lblEndDate.Text = project.EndDate.Value.ToString("MM/dd/yyyy");
            }
            var indentDiv = new Panel() { CssClass = "cell-pad" };
            indentDiv.Controls.Add(lblEndDate);

            row.Cells[EndDateColumnIndex].Controls.Add(indentDiv);
            row.Cells[StartDateColumnIndex].Attributes["class"] = "CompPerfPeriod";
        }

        private static void FillProjectStartCell(ListViewItem e, Project project)
        {
            var row = e.FindControl("boundingRow") as HtmlTableRow;

            // Project Start date cell content
            var lblStartDate = e.FindControl(LabelStartDateId) as Label;

            if (project.StartDate.HasValue)
            {
                lblStartDate.Text = project.StartDate.Value.ToString("MM/dd/yyyy");
            }

            var indentDiv = new Panel() { CssClass = "cell-pad" };
            indentDiv.Controls.Add(lblStartDate);

            row.Cells[StartDateColumnIndex].Controls.Add(indentDiv);
            row.Cells[StartDateColumnIndex].Attributes["class"] = "CompPerfPeriod";
        }

        private void FillProjectStateCell(ListViewItem e, string cssClass, ProjectStatus status)
        {
            var row = e.FindControl("boundingRow") as HtmlTableRow;
            // Project name cell content                        
            // Use ProjectNameCell Control for prepare friendly ToolTip popUp window

            var btnProject = (ProjectNameCellRounded)LoadControl(Constants.ApplicationControls.ProjectNameCellRoundedControl);
            btnProject.ButtonProjectNameId = ButtonProjectNameId;
            btnProject.ButtonProjectNameText = HttpUtility.HtmlEncode("");
            // Shading project according to its status
            btnProject.ButtonProjectNameToolTip = status.Name;

            if (status.Id == (int)ProjectStatusType.Active)
            {
                if (cssClass == "ActiveProjectWithoutSOW")
                {
                    btnProject.ButtonProjectNameToolTip = "Active without SOW";
                }
                else
                {
                    btnProject.ButtonProjectNameToolTip = "Active with SOW";
                }
            }

            btnProject.ButtonCssClass = cssClass;
            btnProject.ToolTipOffsetY = -25;
            btnProject.ToolTipOffsetX = 5;
            var indentDiv = new Panel() { CssClass = "cell-pad" };
            indentDiv.Controls.Add(btnProject);
            row.Cells[ProjectStateColumnIndex].Controls.Add(indentDiv);

        }

        private void FillProjectNameCell(ListViewItem e, Project project)
        {
            var row = e.FindControl("boundingRow") as HtmlTableRow;
            // Project name cell content                        
            // Use ProjectNameCell Control for prepare friendly ToolTip popUp window

            var btnProject = (ProjectNameCellRounded)LoadControl(Constants.ApplicationControls.ProjectNameCellRoundedControl);
            btnProject.ButtonProjectNameId = ButtonProjectNameId;
            btnProject.ButtonProjectNameText = HttpUtility.HtmlEncode(project.Name);
            btnProject.ButtonProjectNameToolTip = PrepareToolTipView(project);
            btnProject.ButtonProjectNameHref = GetRedirectUrl(project.Id.Value, Constants.ApplicationPages.ProjectDetail);
            btnProject.ToolTipOffsetX = 5;
            btnProject.ToolTipOffsetY = -15;
            btnProject.ToolTipPopupPosition = HoverMenuPopupPosition.Right;

            var indentDiv = new Panel() { CssClass = "cell-pad" };
            indentDiv.Controls.Add(btnProject);

            row.Cells[ProjectNameColumnIndex].Controls.Add(indentDiv);
        }

        private static string GetRedirectUrl(int argProjectId, string targetUrl)
        {
            return string.Format(Constants.ApplicationPages.DetailRedirectWithReturnFormat,
                                 targetUrl,
                                 argProjectId,
                                 Constants.ApplicationPages.Projects);
        }

        private static void FillClientNameCell(ListViewItem e, Project project)
        {
            var row = e.FindControl("boundingRow") as HtmlTableRow;
            // Client name cell content
            var btnClient = e.FindControl(ButtonClientNameId) as HyperLink;

            btnClient.Text = HttpUtility.HtmlEncode(project.Client.Name);
            btnClient.NavigateUrl = GetRedirectUrl(project.Client.Id.Value, Constants.ApplicationPages.ClientDetails);

            var indentDiv = new Panel() { CssClass = "cell-pad" };
            indentDiv.Controls.Add(btnClient);

            row.Cells[ClientNameColumnIndex].Controls.Add(indentDiv);
        }

        private static void FillProjectNumberCell(ListViewItem e, Project project)
        {
            var row = e.FindControl("boundingRow") as HtmlTableRow;
            var lblProjectNumber = e.FindControl(LabelProjectNumberId) as Label;

            lblProjectNumber.Text = HttpUtility.HtmlEncode(project.ProjectNumber);

            var indentDiv = new Panel() { CssClass = "cell-pad" };
            indentDiv.Controls.Add(lblProjectNumber);

            row.Cells[ProjectNumberColumnIndex].Controls.Add(indentDiv);
            row.Cells[ProjectNumberColumnIndex].Attributes["class"] = "CompPerfProjectNumber";
        }

        private void BindProjectGrid()
        {
            lvProjects.DataSource = SortProjects(ProjectList);
            lvProjects.DataBind();
        }

        private string GetSortDirection()
        {
            switch (ListViewSortDirection)
            {
                case "Ascending":
                    ListViewSortDirection = "Descending";
                    break;
                case "Descending":
                    ListViewSortDirection = "Ascending";
                    break;
            }
            return ListViewSortDirection;
        }

        private Project[] SortProjects(Project[] projectList)
        {
            if (projectList != null & projectList.Length > 0)
            {
                if (!string.IsNullOrEmpty(PrevListViewSortExpression))
                {
                    var row = lvProjects.FindControl("lvHeader") as System.Web.UI.HtmlControls.HtmlTableRow;
                    var sortExpression = string.Empty;
                    if (row.Cells[ListViewSortColumnId].HasControls())
                    {
                        foreach (var ctrl in row.Cells[ListViewSortColumnId].Controls)
                        {
                            if (ctrl is LinkButton)
                            {
                                var lb = ctrl as LinkButton;
                                sortExpression = lb.Text;
                            }
                        }
                    }
                    Array.Sort(projectList, new ProjectComparer(sortExpression));
                    if (ListViewSortDirection != "Ascending")
                        Array.Reverse(projectList);
                }
                return projectList;
            }
            return ProjectList;
        }

        protected void custPeriod_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = GetPeriodLength() > 0;
        }

        protected void custPeriodLengthLimit_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = GetPeriodLength() <= MaxPeriodLength;
        }

        protected void btnUpdateView_Click(object sender, EventArgs e)
        {
            ValidateAndDisplay();
        }

        protected void btnClientName_Command(object sender, CommandEventArgs e)
        {
            Redirect(string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                Constants.ApplicationPages.ClientDetails,
                e.CommandArgument));
        }

        private static bool IsInMonth(DateTime date, DateTime periodStart, DateTime periodEnd)
        {
            var result =
                (date.Year > periodStart.Year ||
                (date.Year == periodStart.Year && date.Month >= periodStart.Month)) &&
                (date.Year < periodEnd.Year || (date.Year == periodEnd.Year && date.Month <= periodEnd.Month));

            return result;
        }

        private void ValidateAndDisplay()
        {
            // This validator is not applicable here.
            reqSearchText.IsValid = true;

            Page.Validate(valsPerformance.ValidationGroup);
            if (Page.IsValid)
            {
                Display();

                SetddlView();
                lvProjects.DataBind();

                StyledUpdatePanel.Update();
            }
        }

        /// <summary>
        /// Adds to the performance grid one for each the month withing the selected period.
        /// </summary>
        protected override void Display()
        {
            PreparePeriodView();

            // Clean up the cached values
            CompanyPerformanceState.Clear();
            OneGreaterSeniorityExists = false;
            var personListAnalyzer = new SeniorityAnalyzer(DataHelper.CurrentPerson);

            foreach (var project in ProjectList)
            {
                var greaterSeniorityExists = personListAnalyzer.OneWithGreaterSeniorityExists(project.ProjectPersons);
                if (greaterSeniorityExists)
                {
                    OneGreaterSeniorityExists = greaterSeniorityExists;
                    break;
                }
            }

            // Main GridView
            lvProjects.DataSource = ProjectList;
            lvProjects.DataBind();

            if (!IsPostBack)
            {
                SetddlView();

                lvProjects.DataBind();
            }
        }

        protected override void RedirectWithBack(string redirectUrl, string backUrl)
        {
            SaveFilterSettings();
            base.RedirectWithBack(redirectUrl, backUrl);
        }

        /// <summary>
        /// Computes and displays total values;
        /// </summary>
        private Project CalculateSummaryTotals(
            IList<Project> projects,
            DateTime periodStart,
            DateTime periodEnd)
        {

            int? defaultProjectId = MileStoneConfigurationManager.GetProjectId();
            int defaultProjectIdValue = defaultProjectId.HasValue ? defaultProjectId.Value : 0;
            // Prepare Financial Summary GridView            
            var financialSummaryRevenue = new Project
            {
                ProjectedFinancialsByMonth =
                    new Dictionary<DateTime, ComputedFinancials>(),
                ComputedFinancials = new ComputedFinancials()
            };

            for (var dtTemp = periodStart; dtTemp <= periodEnd; dtTemp = dtTemp.AddMonths(1))
            {
                var financials = new ComputedFinancials();

                // Looking through the projects
                foreach (var project in projects)
                {

                    if (project.Id.HasValue && project.Id.Value == defaultProjectIdValue)
                        continue;

                    foreach (var projectFinancials in project.ProjectedFinancialsByMonth)
                    {
                        if (projectFinancials.Key.Year == dtTemp.Year &&
                            projectFinancials.Key.Month == dtTemp.Month &&
                            project.Id.HasValue)
                        {
                            financials.Revenue += projectFinancials.Value.Revenue;
                            financials.GrossMargin += projectFinancials.Value.GrossMargin;
                        }
                    }
                }

                financialSummaryRevenue.ProjectedFinancialsByMonth.Add(dtTemp, financials);

                var projectsHavingFinancials = projects.Where(project => project.ComputedFinancials != null && project.Id.Value != defaultProjectIdValue);

                if (projectsHavingFinancials != null)
                {
                    financialSummaryRevenue.ComputedFinancials.GrossMargin = projectsHavingFinancials.Sum(proj => proj.ComputedFinancials.GrossMargin);
                    financialSummaryRevenue.ComputedFinancials.Revenue = projectsHavingFinancials.Sum(proj => proj.ComputedFinancials.Revenue);
                }
            }

            return financialSummaryRevenue;
        }

        /// <summary>
        /// Executes preliminary operations to the view be ready to display the data.
        /// </summary>
        private void PreparePeriodView()
        {

            if (!IsPostBack)
            {
                var filter = InitFilter();

                //  If current user is administrator, don't apply restrictions
                var person =
                    Roles.IsUserInRole(
                        DataHelper.CurrentPerson.Alias,
                        DataTransferObjects.Constants.RoleNames.AdministratorRoleName)
                    ? null : DataHelper.CurrentPerson;

                // If person is not administrator, return list of values when [All] is selected
                //      this is needed because we apply restrictions and don't want
                //      NULL to be returned, because that would mean all and restrictions
                //      are not going to be applied
                if (person != null)
                {
                    cblSalesperson.AllSelectedReturnType = ScrollingDropDown.AllSelectedType.AllItems;
                    cblProjectOwner.AllSelectedReturnType = ScrollingDropDown.AllSelectedType.AllItems;
                    cblPractice.AllSelectedReturnType = ScrollingDropDown.AllSelectedType.AllItems;
                    cblClient.AllSelectedReturnType = ScrollingDropDown.AllSelectedType.AllItems;
                    cblProjectGroup.AllSelectedReturnType = ScrollingDropDown.AllSelectedType.AllItems;
                }

                PraticeManagement.Controls.DataHelper.FillSalespersonList(
                    person, cblSalesperson,
                    Resources.Controls.AllSalespersonsText,
                    false);

                PraticeManagement.Controls.DataHelper.FillProjectOwnerList(cblProjectOwner,
                    "All Owners",
                    null,
                    true,
                    person);

                PraticeManagement.Controls.DataHelper.FillPracticeList(
                    person,
                    cblPractice,
                    Resources.Controls.AllPracticesText);

                PraticeManagement.Controls.DataHelper.FillClientsAndGroups(
                    cblClient, cblProjectGroup);

                // Set the default viewable interval.
                diRange.FromDate = filter.PeriodStart;
                diRange.ToDate = filter.PeriodEnd;

                //chbPeriodOnly.Checked = filter.TotalOnlySelectedDateWindow;

                chbActive.Checked = filter.ShowActive;
                chbCompleted.Checked = filter.ShowCompleted;
                chbExperimental.Checked = filter.ShowExperimental;
                chbProjected.Checked = filter.ShowProjected;
                chbInternal.Checked = filter.ShowInternal;
                chbInactive.Checked = filter.ShowInactive;

                SelectedClientIds = filter.ClientIdsList;
                SelectedPracticeIds = filter.PracticeIdsList;
                SelectedProjectOwnerIds = filter.ProjectOwnerIdsList;
                SelectedSalespersonIds = filter.SalespersonIdsList;
                SelectedGroupIds = filter.ProjectGroupIdsList;

                ddlPeriod.SelectedIndex = ddlPeriod.Items.IndexOf(ddlPeriod.Items.FindByValue(filter.PeriodSelected.ToString()));
                ddlView.SelectedIndex = ddlView.Items.IndexOf(ddlView.Items.FindByValue(filter.ViewSelected.ToString()));
                ddlCalculateRange.SelectedIndex = ddlCalculateRange.Items.IndexOf(ddlCalculateRange.Items.FindByValue(filter.CalculateRangeSelected.ToString()));
            }
            else
            {
                Page.Validate(valsPerformance.ValidationGroup);
            }
        }

        private static CompanyPerformanceFilterSettings InitFilter()
        {
            return SerializationHelper.DeserializeCookie(CompanyPerformanceFilterKey) as CompanyPerformanceFilterSettings ??
                   new CompanyPerformanceFilterSettings();
        }

        private void AddMonthColumn(HtmlTableRow row, DateTime periodStart, int monthsInPeriod, int insertPosition)
        {
            if (row != null)
            {
                while (row.Cells.Count > NumberOfFixedColumns + 1)
                {
                    row.Cells.RemoveAt(NumberOfFixedColumns);
                }

                for (int i = insertPosition, k = 0; k < monthsInPeriod; i++, k++)
                {
                    var newColumn = new HtmlTableCell("td");
                    row.Cells.Insert(i, newColumn);

                    row.Cells[i].InnerHtml = periodStart.ToString(Constants.Formatting.CompPerfMonthYearFormat);
                    row.Cells[i].Attributes["class"] = "CompPerfMonthSummary";

                    periodStart = periodStart.AddMonths(1);
                }
            }
        }

        /// <summary>
        /// Prepare ToolTip for project Name cell
        /// </summary>
        /// <param name="project">project</param>
        /// <returns>ToolTip</returns>
        private static string PrepareToolTipView(Project project)
        {
            var persons = new StringBuilder();
            var personList = new List<MilestonePerson>();
            foreach (var projectPerson in project.ProjectPersons)
            {
                var personExist = false;
                if (personList != null)
                {
                    foreach (var mp in personList)
                        if (mp.Person.Id == projectPerson.Person.Id)
                        {
                            personExist = true;
                            break;
                        }
                }
                if (!personExist)
                {
                    personList.Add(projectPerson);
                }
            }
            foreach (var t in personList)
                persons.AppendFormat(AppendPersonFormat,
                                     Environment.NewLine,
                                     HttpUtility.HtmlEncode(t.Person.FirstName),
                                     HttpUtility.HtmlEncode(t.Person.LastName));

            return string.Format(ToolTipView,
                Environment.NewLine,
                HttpUtility.HtmlEncode(project.Name),
                project.StartDate,
                project.EndDate,
                persons,
                HttpUtility.HtmlEncode(project.BuyerName));
        }

        /// <summary>
        /// Calculates a length of the selected period in the mounths.
        /// </summary>
        /// <returns>The number of the months within the selected period.</returns>
        private int GetPeriodLength()
        {
            int mounthsInPeriod =
                (diRange.ToDate.Value.Year - diRange.FromDate.Value.Year) * Constants.Dates.LastMonth +
                (diRange.ToDate.Value.Month - diRange.FromDate.Value.Month + 1);
            return mounthsInPeriod;
        }

        private const string ANIMATION_SHOW_SCRIPT =
                        @"<OnClick>
                        	<Sequence>
                        		<Parallel Duration=""0"" AnimationTarget=""{0}"">
                        			<Discrete Property=""style"" propertyKey=""border"" ValuesScript=""['thin solid navy']""/>
                        		</Parallel>
                        		<Parallel Duration="".4"" Fps=""20"" AnimationTarget=""{0}"">
                        			<Resize Width=""250"" Height=""160"" Unit=""px"" />
                        		</Parallel>
                        	</Sequence>
                        </OnClick>";

        private const string ANIMATION_HIDE_SCRIPT =
                        @"<OnClick>
                        	<Sequence>
                        		<Parallel Duration="".4"" Fps=""20"" AnimationTarget=""{0}"">
                        			<Resize Width=""0"" Height=""0"" Unit=""px"" />
                        		</Parallel>
                        		<Parallel Duration=""0"" AnimationTarget=""{0}"">
                        			<Discrete Property=""style"" propertyKey=""border"" ValuesScript=""['none']""/>
                        		</Parallel>
                        	</Sequence>
                        </OnClick>";

        private const string POPULATE_EXTENDER_SERVICEMETHOD = "RenderMonthMiniReport";
        private const string POPULATE_EXTENDER_UPDATINGCSS = "Loading";
        private const string MINIREPORT_CSSCLASS = "MiniReport";
        private const string CORRECT_MINI_REPORT_POS_FUNCTION = "correctMonthMiniReportPosition('{0}', '{1}', '{2}');";

        /// <summary>
        /// Generates a month mini-report view.
        /// </summary>
        /// <param name="e">Grid View header row.</param>
        /// <param name="i">Header cell index.</param>
        private void PopulateMiniReportCell(HtmlTableRow row, int i)
        {
            var pnlHeader = new Panel() { ID = string.Format(PanelHeaderIdFormat, i), CssClass = CompPerfHeaderDivCssClass };

            var lblHeader =
                new Label { Text = row.Cells[i].InnerHtml };
            var lblHeaderHint =
                new Label { Text = "&nbsp;", ID = string.Format(LabelHeaderIdFormat, i), CssClass = HintDivCssClass };

            var pnlReport = new Panel { ID = string.Format(PanelReportIdFormat, i), CssClass = MINIREPORT_CSSCLASS };

            var closeButtonId = string.Format(CloseReportButtonIdFormat, i);
            var tblReport = new Table();
            tblReport.Rows.Add(new TableRow());
            tblReport.Rows[0].Cells.Add(new TableHeaderCell());
            tblReport.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Right;
            var closeButton = new HtmlInputButton { ID = closeButtonId, Value = "X" };
            closeButton.Attributes["class"] = "mini-report-close";
            tblReport.Rows[0].Cells[0].Controls.Add(closeButton);

            tblReport.Rows.Add(new TableRow());
            tblReport.Rows[1].Cells.Add(new TableCell { ID = string.Format(ReportCellIdFormat, i) });

            pnlReport.Controls.Add(tblReport);
            pnlReport.Controls.Add(
                new DynamicPopulateExtender
                {
                    PopulateTriggerControlID = lblHeaderHint.ID,
                    TargetControlID = tblReport.Rows[1].Cells[0].ID,
                    ContextKey = lblHeader.Text + "," +
                                this.chbProjected.Checked.ToString() + "," +
                                this.chbCompleted.Checked.ToString() + "," +
                                this.chbActive.Checked.ToString() + "," +
                                this.chbExperimental.Checked.ToString() + "," +
                                this.chbInternal.Checked.ToString() + "," +
                                this.chbInactive.Checked.ToString(),

                    ServiceMethod = POPULATE_EXTENDER_SERVICEMETHOD,
                    UpdatingCssClass = POPULATE_EXTENDER_UPDATINGCSS,
                });

            row.Cells[i].InnerHtml = string.Empty;
            pnlHeader.Controls.Add(lblHeader);
            pnlHeader.Controls.Add(lblHeaderHint);
            row.Cells[i].Controls.Add(pnlHeader);
            row.Cells[i].Controls.Add(pnlReport);

            var animShow =
                new AnimationExtender
                {
                    TargetControlID = lblHeaderHint.ID,
                    Animations = string.Format(ANIMATION_SHOW_SCRIPT, pnlReport.ID)
                };

            var animHide =
                new AnimationExtender
                {
                    TargetControlID = closeButtonId,
                    Animations = string.Format(ANIMATION_HIDE_SCRIPT, pnlReport.ID)
                };

            row.Cells[i].Controls.Add(animShow);
            row.Cells[i].Controls.Add(animHide);

            lblHeaderHint.Attributes["onclick"]
                = string.Format(
                    CORRECT_MINI_REPORT_POS_FUNCTION,
                    pnlReport.ClientID,
                    lblHeader.ClientID,
                    StyledUpdatePanel.FindControl("horisontalScrollDiv").ClientID);

            lblHeaderHint.ToolTip = string.Format(LabelHeaderIdToolTipView, lblHeader.Text);
        }

        /// <summary>
        /// Generates a month mini-report.
        /// </summary>
        /// <param name="contextKey">Determines a month to thje report be generated for.</param>
        /// <returns>A report rendered to HTML.</returns>
        [WebMethod]
        [ScriptMethod]
        public static string RenderMonthMiniReport(string contextKey)
        {
            var result = new StringBuilder();
            string monthYear = string.Empty;
            bool showProjected = false,
                showCompleted = false,
                showActive = false,
                showExperimental = false,
                showInternal = false,
                showInactive = false;

            ExtractFilterValues(
                                contextKey,
                                ref monthYear,
                                ref showProjected,
                                ref showCompleted,
                                ref showActive,
                                ref showExperimental,
                                ref showInternal,
                                ref showInactive
                                );

            DateTime requestedMonth;
            if (DateTime.TryParseExact(monthYear,
                Constants.Formatting.CompPerfMonthYearFormat,
                CultureInfo.CurrentUICulture,
                DateTimeStyles.None,
                out requestedMonth))
            {
                using (var serviceClient = new ProjectServiceClient())
                {
                    try
                    {
                        string xml = serviceClient.MonthMiniReport(
                            requestedMonth,
                            HttpContext.Current.User.Identity.Name,
                            showProjected,
                            showCompleted,
                            showActive,
                            showExperimental,
                            showInternal,
                            showInactive);
                        var transform = new XslCompiledTransform();
                        transform.Load(
                            HttpContext.Current.Server.MapPath(Constants.ReportTemplates.MonthMiniReport));
                        using (TextReader txt = new StringReader(xml))
                        using (XmlReader reader = XmlReader.Create(txt))
                        {
                            using (TextWriter writer = new StringWriter(result))
                            {
                                transform.Transform(reader, new XsltArgumentList(), writer);
                            }
                        }
                    }
                    catch (CommunicationException)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }
            }

            return result.ToString();
        }

        private static void ExtractFilterValues(
                                                string contextKey,
                                                ref string monthYear,
                                                ref bool showProjected,
                                                ref bool showCompleted,
                                                ref bool showActive,
                                                ref bool showExperimental,
                                                ref bool showInternal,
                                                ref bool showInactive
                                                )
        {
            string[] parameters = contextKey.Split(',');

            monthYear = parameters[0];
            showProjected = Boolean.Parse(parameters[1]);
            showCompleted = Boolean.Parse(parameters[2]);
            showActive = Boolean.Parse(parameters[3]);
            showExperimental = Boolean.Parse(parameters[4]);
            showInternal = Boolean.Parse(parameters[5]);
            showInactive = Boolean.Parse(parameters[6]);
        }


        protected void btnView_Command(object sender, CommandEventArgs e)
        {
            ((WebControl)((Control)sender).Parent).CssClass = "SelectedSwitch";
        }

        #endregion

        protected void lvProjects_DataBinding(object sender, EventArgs e)
        {
            //horisontalScrollDiv.CssClass = chbPrintVersion.Checked ? string.Empty : "xScroll cp";

            var periodStart = diRange.FromDate.Value;
            var monthsInPeriod = GetPeriodLength();
            var row = lvProjects.FindControl("lvHeader") as System.Web.UI.HtmlControls.HtmlTableRow;
            Page.Validate(valsPerformance.ValidationGroup);
            if (!IsPostBack || Page.IsValid)
                AddMonthColumn(row, periodStart, monthsInPeriod, NumberOfFixedColumns);

            string totalHeaderText = string.Format(TotalHeaderFormat, ddlCalculateRange.SelectedItem.Text);// chbPeriodOnly.Checked ? SelectedText : EntireProjectPeriod);
            var div = new Panel() { CssClass = CompPerfHeaderDivCssClass };
            div.Controls.Add(new Label() { Text = totalHeaderText });

            var stringWriter = new System.IO.StringWriter();
            using (HtmlTextWriter wr = new HtmlTextWriter(stringWriter))
            {
                div.RenderControl(wr);
                var s = stringWriter.ToString();
                row.Cells[row.Cells.Count - 1].InnerHtml = s;
            }

            for (int i = NumberOfFixedColumns; i < row.Cells.Count - 1; i++)
            {
                PopulateMiniReportCell(row, i);
            }

            // fill summary
            row = lvProjects.FindControl("lvSummary") as System.Web.UI.HtmlControls.HtmlTableRow;
            while (row.Cells.Count > 1)
            {
                row.Cells.RemoveAt(1);
            }

            for (int i = 0; i < monthsInPeriod + 1; i++)   // + 1 means a cell for total column
            {
                var td = new HtmlTableCell() { };
                td.Attributes["class"] = "CompPerfMonthSummary";
                row.Cells.Insert(row.Cells.Count, td);
            }
            var summary = CalculateSummaryTotals(ProjectList, periodStart, PeriodEnd);

            FillSummaryTotalRow(monthsInPeriod, summary, row);

        }

        private void FillSummaryTotalRow(int periodLength, Project summary, System.Web.UI.HtmlControls.HtmlTableRow row)
        {
            DateTime monthBegin = GetMonthBegin();

            // Displaying the interest values (main cell data)
            for (int i = 1, k = 0;
                k < periodLength;
                i++, k++, monthBegin = monthBegin.AddMonths(1))
            {
                DateTime monthEnd = GetMonthEnd(ref monthBegin);

                foreach (KeyValuePair<DateTime, ComputedFinancials> interestValue in
                    summary.ProjectedFinancialsByMonth)
                {
                    if (IsInMonth(interestValue.Key, monthBegin, monthEnd))
                    {
                        var grossMargin = OneGreaterSeniorityExists ? Resources.Controls.HiddenCellText : interestValue.Value.GrossMargin.Value.ToString(CurrencyDisplayFormat);
                        row.Cells[i].InnerHtml = GetMonthReportTableAsHtml(interestValue.Value.Revenue, grossMargin);

                        //string.Format(Resources.Controls.ProjectInterestFormat,
                        //interestValue.Value.Revenue,
                        //interestValue.Value.GrossMargin);
                    }
                }
            }


            // Project totals
            PracticeManagementCurrency totalRevenue = 0M;
            PracticeManagementCurrency totalMargin = 0M;

            // Calculate Total Revenue and Margin for current Project
            if (summary.ComputedFinancials != null)
            {
                totalRevenue = summary.ComputedFinancials.Revenue;
                totalMargin = summary.ComputedFinancials.GrossMargin;
            }
            var totalGrossMargin = OneGreaterSeniorityExists ? Resources.Controls.HiddenCellText : totalMargin.Value.ToString(CurrencyDisplayFormat);
            row.Cells[row.Cells.Count - 1].InnerHtml = GetMonthReportTableAsHtml(totalRevenue, totalGrossMargin);
        }

        #region Month table from resources
        private Table GetMonthReportTable(PracticeManagementCurrency revenue, string margin)
        {
            var reportTable = new Table() { Width = Unit.Percentage(100) };
            var tr = new TableRow() { CssClass = "Revenue" };
            tr.Cells.Add(new TableCell() { HorizontalAlign = HorizontalAlign.Left, Text = "" });

            tr.Cells.Add(new TableCell() { HorizontalAlign = HorizontalAlign.Right, Text = string.Format(PracticeManagementCurrency.RevenueFormat, revenue.Value.ToString(CurrencyDisplayFormat)) });
            reportTable.Rows.Add(tr);
            tr = new TableRow() { CssClass = "Margin" };
            tr.Cells.Add(new TableCell() { HorizontalAlign = HorizontalAlign.Left, Text = "" });
            tr.Cells.Add(new TableCell()
            {
                HorizontalAlign = HorizontalAlign.Right,
                Text = margin.IndexOf("-") == 0 ?
                       string.Format(PracticeManagementCurrency.BenchFormat, margin) :
                            string.Format(PracticeManagementCurrency.MarginFormat, margin)
            });
            reportTable.Rows.Add(tr);

            return reportTable;
        }

        private Table GetMonthReportTable(PracticeManagementCurrency revenue, PracticeManagementCurrency margin)
        {
            return GetMonthReportTable(revenue, margin.Value.ToString(CurrencyDisplayFormat));
        }

        //private string GetMonthReportTableAsHtml(PracticeManagementCurrency revenue, PracticeManagementCurrency margin)
        //{
        //    return GetMonthReportTableAsHtml(revenue, margin.Value.ToString(CurrencyDisplayFormat));
        //}

        private string GetMonthReportTableAsHtml(PracticeManagementCurrency revenue, string margin)
        {
            string outterHtml = string.Empty;

            var stringWriter = new System.IO.StringWriter();


            var table = GetMonthReportTable(revenue, margin);

            var div = new Panel() { CssClass = "cell-pad" };
            div.Controls.Add(table);
            using (HtmlTextWriter wr = new HtmlTextWriter(stringWriter))
            {
                div.RenderControl(wr);
                outterHtml = stringWriter.ToString();
            }

            return outterHtml;
        }
        #endregion

        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage("Projects");

            var projectsData = (from pro in ProjectList
                                where pro != null
                                select new
                                {
                                    ProjectID = pro.Id != null ? pro.Id.ToString() : string.Empty,
                                    ProjectNumber = pro.ProjectNumber != null ? pro.ProjectNumber.ToString() : string.Empty,
                                    Client = (pro.Client != null && pro.Client.Name != null) ? pro.Client.Name.ToString() : string.Empty,
                                    ProjectName = pro.Name != null ? pro.Name : string.Empty,
                                    BuyerName = pro.BuyerName != null ? pro.BuyerName : string.Empty,
                                    Status = (pro.Status != null && pro.Status.Name != null) ? pro.Status.Name.ToString() : string.Empty,
                                    StartDate = pro.StartDate != null ? pro.StartDate.Value.ToString("MM/dd/yyyy") : string.Empty,
                                    EndDate = pro.EndDate != null ? pro.EndDate.Value.ToString("MM/dd/yyyy") : string.Empty,
                                    PracticeArea = (pro.Practice != null && pro.Practice.Name != null) ? pro.Practice.Name : string.Empty,
                                    Revenue = (pro.ComputedFinancials != null && pro.ComputedFinancials.Revenue != null) ? pro.ComputedFinancials.Revenue.Value : 0,
                                    Margin = (pro.ComputedFinancials != null && pro.ComputedFinancials.GrossMargin != null) ? pro.ComputedFinancials.GrossMargin.Value : 0,
                                    PM = (pro.ProjectManager != null && pro.ProjectManager.Name != null) ? pro.ProjectManager.Name : string.Empty,
                                    Salesperson = (pro.SalesPersonName != null) ? pro.SalesPersonName : string.Empty,
                                    ClientDirector = (pro.Director != null && pro.Director.Name != null) ? pro.Director.Name.ToString() : string.Empty
                                }).ToList();//Note: If you add any extra property to this anonymous type object then change cell index values in  FormatExcelReport method.

            GridView projectsGrid = new GridView();
            projectsGrid.DataSource = projectsData;
            projectsGrid.DataMember = "excelDataTable";
            projectsGrid.DataBind();
            projectsGrid.Visible = false;
            FormatExcelReport(projectsGrid);

            if (projectsGrid.HeaderRow != null && projectsGrid.HeaderRow.Cells.Count > 0)
            {
                projectsGrid.HeaderRow.Cells[0].Visible = false;
            }

            GridViewExportUtil.Export("Projects.xls", projectsGrid);
        }

        protected void btnExportAllToExcel_Click(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage("Projects");

            var projectsList = GetProjectListAll();

            var projectsData = (from pro in projectsList
                                where pro != null
                                select new
                                {
                                    ProjectID = pro.Id != null ? pro.Id.ToString() : string.Empty,
                                    ProjectNumber = pro.ProjectNumber != null ? pro.ProjectNumber.ToString() : string.Empty,
                                    Client = (pro.Client != null && pro.Client.Name != null) ? pro.Client.Name.ToString() : string.Empty,
                                    ProjectName = pro.Name != null ? pro.Name : string.Empty,
                                    BuyerName = pro.BuyerName != null ? pro.BuyerName : string.Empty,
                                    Status = (pro.Status != null && pro.Status.Name != null) ? pro.Status.Name.ToString() : string.Empty,
                                    StartDate = pro.StartDate != null ? pro.StartDate.Value.ToString("MM/dd/yyyy") : string.Empty,
                                    EndDate = pro.EndDate != null ? pro.EndDate.Value.ToString("MM/dd/yyyy") : string.Empty,
                                    PracticeArea = (pro.Practice != null && pro.Practice.Name != null) ? pro.Practice.Name : string.Empty,
                                    Revenue = (pro.ComputedFinancials != null && pro.ComputedFinancials.Revenue != null) ? pro.ComputedFinancials.Revenue.Value : 0,
                                    Margin = (pro.ComputedFinancials != null && pro.ComputedFinancials.GrossMargin != null) ? pro.ComputedFinancials.GrossMargin.Value : 0,
                                    PM = (pro.ProjectManager != null && pro.ProjectManager.Name != null) ? pro.ProjectManager.Name : string.Empty,
                                    Salesperson = (pro.SalesPersonName != null) ? pro.SalesPersonName : string.Empty,
                                    ClientDirector = (pro.Director != null && pro.Director.Name != null) ? pro.Director.Name.ToString() : string.Empty
                                }).ToList();

            GridView projectsGrid = new GridView();
            projectsGrid.DataSource = projectsData;
            projectsGrid.DataMember = "excelDataTable";
            projectsGrid.DataBind();
            projectsGrid.Visible = false;
            FormatExcelReport(projectsGrid);

            if (projectsGrid.HeaderRow != null && projectsGrid.HeaderRow.Cells.Count > 0)
            {
                projectsGrid.HeaderRow.Cells[0].Visible = false;
            }

            GridViewExportUtil.Export("Projects.xls", projectsGrid);
        }

        private Project[] GetProjectListAll()
        {
            using (var serviceClient = new ProjectService.ProjectServiceClient())
            {
                return serviceClient.GetProjectListCustom(true, true, true, true);
            }
        }

        private void FormatExcelReport(GridView projectsGrid)
        {
            foreach (GridViewRow row in projectsGrid.Rows)
            {
                SeniorityAnalyzer personListAnalyzer = null;
                personListAnalyzer = new SeniorityAnalyzer(DataHelper.CurrentPerson);
                personListAnalyzer.OneWithGreaterSeniorityExists(DataHelper.GetPersonsInMilestone(new Project { Id = Convert.ToInt32(row.Cells[0].Text) }));
                bool greaterSeniorityExists = personListAnalyzer != null && personListAnalyzer.GreaterSeniorityExists;

                row.Cells[0].Visible = false;

                Decimal revenueValue = Convert.ToDecimal(row.Cells[9].Text);
                if (revenueValue < 0)
                {
                    row.Cells[9].ForeColor = Color.Red;
                    row.Cells[9].Text = revenueValue.ToString(CurrencyExcelReportFormat);
                }
                else
                {
                    row.Cells[9].ForeColor = Color.Green;
                    row.Cells[9].Text = revenueValue.ToString(CurrencyExcelReportFormat);
                }

                Decimal marginValue = Convert.ToDecimal(row.Cells[10].Text);
                if (greaterSeniorityExists)
                {
                    row.Cells[10].Text = Resources.Controls.HiddenCellText;
                    row.Cells[10].ForeColor = Color.Purple;
                }
                else if (marginValue < 0)
                {
                    row.Cells[10].ForeColor = Color.Red;
                    row.Cells[10].Text = marginValue.ToString(CurrencyExcelReportFormat);
                }
                else
                {
                    row.Cells[10].ForeColor = Color.Purple;
                    row.Cells[10].Text = marginValue.ToString(CurrencyExcelReportFormat);
                }


            }
        }

        protected void lvProjects_Sorted(object sender, EventArgs e)
        {
            var row = lvProjects.FindControl("lvHeader") as System.Web.UI.HtmlControls.HtmlTableRow;
            for (int i = 1; i < row.Cells.Count; i++)
            {
                HtmlTableCell cell = row.Cells[i];

                if (cell.HasControls())
                {
                    foreach (var ctrl in cell.Controls)
                    {
                        if (ctrl is LinkButton)
                        {
                            var lb = ctrl as LinkButton;
                            lb.CssClass = "arrow";
                            if (i == ListViewSortColumnId)
                            {
                                lb.CssClass += string.Format(" sort-{0}", ListViewSortDirection == "Ascending" ? "up" : "down");
                            }
                        }
                    }
                }
            }
        }

        protected void lvProjects_Sorting(object sender, ListViewSortEventArgs e)
        {
            if (PrevListViewSortExpression != e.SortExpression)
            {
                PrevListViewSortExpression = e.SortExpression;
                ListViewSortDirection = e.SortDirection.ToString();
            }
            else
            {
                ListViewSortDirection = GetSortDirection();
            }

            ListViewSortColumnId = GetSortColumnId(e.SortExpression);
            BindProjectGrid();
        }

        private int GetSortColumnId(string sortExpression)
        {
            int sortColumn = -1;
            return int.TryParse(sortExpression, out sortColumn) ? sortColumn : ProjectNumberColumnIndex;
        }

        protected void lvProjects_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            var dpProject = GetPager();
            dpProject.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);

            lvProjects.DataSource = ProjectList;
            lvProjects.DataBind();
        }

        protected void lvProjects_OnDataBound(object sender, EventArgs e)
        {
            var pager = GetPager();
            if (pager != null)
            {
                pager.Visible = (pager.PageSize < pager.TotalRowCount);
                lblTotalCount.Text = GetTotalCount().ToString();
                lblCurrentPageCount.Text = (pager.StartRowIndex + 1).ToString() + "&nbsp;-&nbsp;" + (pager.StartRowIndex + GetCurrentPageCount()).ToString();
            }
            else
            {
                lblTotalCount.Text = GetCurrentPageCount().ToString();

                lblCurrentPageCount.Text = "0&nbsp;-&nbsp;0";
            }

        }

        protected void Pager_PagerCommand(object sender, DataPagerCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case PagerNextCommand:
                    int nextPageStartIndex = e.Item.Pager.StartRowIndex + e.Item.Pager.PageSize;
                    if (nextPageStartIndex <= e.TotalRowCount)
                    {
                        e.NewStartRowIndex = nextPageStartIndex;
                        e.NewMaximumRows = e.Item.Pager.MaximumRows;
                    }
                    break;
                case PagerPrevCommand:
                    int prevPageStartIndex = e.Item.Pager.StartRowIndex - e.Item.Pager.PageSize;
                    if (prevPageStartIndex >= 0)
                    {
                        e.NewStartRowIndex = prevPageStartIndex;
                        e.NewMaximumRows = e.Item.Pager.MaximumRows;
                    }
                    break;
                default:
                    throw new ArgumentException(
                        string.Format(
                            "Cannot process the command '{0}'. Expected = 'Prev, Next'",
                            e.CommandName));
            }
        }

        protected bool IsNeedToShowNextButton()
        {
            int currentRecords = GetCurrentPageCount();
            var pager = GetPager();
            //return pager.StartRowIndex + pager.PageSize <= pager.TotalRowCount;

            if (ddlView.SelectedValue == "1")
            {
                return false;
            }
            else
            {
                return !((lvProjects.Items.Count == 0) || (currentRecords == GetTotalCount()) || (currentRecords < Convert.ToInt32(ddlView.SelectedValue)));
            }
        }

        protected bool IsNeedToShowPrevButton()
        {
            return GetPager().StartRowIndex != 0;
        }

        private DataPager GetPager()
        {
            return (DataPager)lvProjects.FindControl("dpProjects");
            //return dpProjects;
        }

        public int GetCurrentPageCount()
        {
            return lvProjects.Items.Count;
        }

        public int GetTotalCount()
        {
            return GetPager().TotalRowCount;
        }
    }
}

