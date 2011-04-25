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


namespace PraticeManagement
{
    public partial class CompanyPerformance : PracticeManagementPageBase
    {
        
        #region Constants

        private const int NumberOfFixedColumns = 5;
        private const int ProjectNumberColumnIndex = 0;
        private const int ClientNameColumnIndex = 1;
        private const int ProjectNameColumnIndex = 2;
        private const int StartDateColumnIndex = 3;
        private const int EndDateColumnIndex = 4;

        private const int TabNameColumnIndex = 0;

        private const int MaxPeriodLength = 24;

        private const int SummaryInfoProjectIndex = 0;
        private const int BenchProjectIndex = 1;
        private const int AdminProjectIndex = 2;
        private const int AvgRatesProjectIndex = 3;
        private const int ExpensesProjectIndex = 4;

        private const int FS_TotalRowCount = 10;
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

        private const int CR_TotalRowCount = 5;
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
        private const string PanelReportIdFormat = "pnlReport{0}";
        private const string CloseReportButtonIdFormat = "btnCloseReport{0}";
        private const string ReportCellIdFormat = "cellReport{0}";

        private const string TotalHeaderFormat = "Total ({0})";
        private const string CurrentYearText = "current year";
        private const string SelectedText = "selected";
        private const string InnerTableBeginner =
            "</td></tr><tr><td colspan=\"{0}\"><div class=\"bScroll\"><table border=\"1\" cellspacing=\"0\" cellpadding=\"1\"><tr><td colspan=\"{0}\">";
        private const string STR_SortExpression = "SortExpression";
        private const string STR_SortDirection = "SortDirection";
        private const string InnerTableTerminator = "</td></tr></table></div></td>";
        private const string ToolTipView = "{1}{0}<nobr>Buyer Name:&nbsp;{5}</nobr>{0}Start: {2:d}{0}End: {3:d}{4}";
        private const string AppendPersonFormat = "{0}{1} {2}";
        private const string CompPerfDataCssClass = "CompPerfData";
        private const string ReportFormat = "{0}&nbsp;{1}";

        private const string CompanyPerformanceFilterKey = "CompanyPerformanceFilterKey";

        #endregion

        #region Fields

        private bool userIsAdministrator;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates if there were ata least one field that was hidden
        /// </summary>
        private bool OneGreaterSeniorityExists { get; set; }

        /// <summary>
        /// Gets a selected period start.
        /// </summary>
        private DateTime PeriodStart
        {
            get
            {
                return new DateTime(mpPeriodStart.SelectedYear, mpPeriodStart.SelectedMonth, Constants.Dates.FirstDay);
            }
        }

        /// <summary>
        /// Gets a selected period end.
        /// </summary>
        private DateTime PeriodEnd
        {
            get
            {
                return
                    new DateTime(mpPeriodEnd.SelectedYear, mpPeriodEnd.SelectedMonth,
                        DateTime.DaysInMonth(mpPeriodEnd.SelectedYear, mpPeriodEnd.SelectedMonth));
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

        /// <summary>
        /// Gets a list of benches to be displayed.
        /// </summary>
        private Project[] BenchList
        {
            get
            {
                CompanyPerformanceState.Filter = GetFilterSettings();
                return CompanyPerformanceState.BenchList;
            }
        }

        /// <summary>
        /// Gets an expenses list.
        /// </summary>
        private IEnumerable<MonthlyExpense> ExpenseList
        {
            get
            {
                CompanyPerformanceState.Filter = GetFilterSettings();
                return CompanyPerformanceState.ExpenseList;
            }
        }

        /// <summary>
        /// Gets the data for the person stats report.
        /// </summary>
        private IEnumerable<PersonStats> PersonStats
        {
            get
            {
                CompanyPerformanceState.Filter = GetFilterSettings();
                return CompanyPerformanceState.PersonStats;
            }
        }

        private string GridViewSortExpression
        {
            get { return ViewState[STR_SortExpression] as string ?? string.Empty; }
            set { ViewState[STR_SortExpression] = value; }
        }

        private string GridViewSortDirection
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

                // Take the data from the cache to the LinkButtons are able to work.
                gvProjects.DataSource = ProjectList;
                gvProjects.DataBind();
            }

            custPeriodLengthLimit.ErrorMessage = custPeriodLengthLimit.ToolTip =
                string.Format(custPeriodLengthLimit.ErrorMessage, MaxPeriodLength);

            DisplaySelectedReport();

            
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

                if (userIsSalesperson)
                    cellCommissionsAndRates.Visible = true;
            }
            else
            {
                cellCommissionsAndRates.Visible = true;
            }

            // cblSalesperson.Enabled = cblPracticeManager.Enabled = userIsAdministrator;

            // Client side validator is not applicable here.
            reqSearchText.IsValid = true;

            SaveFilterSettings();
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
            bool hideAdvancedFilterValue =
                pnlAdvancedFilter_CollapsiblePanelExtender.ClientState != null ?
                Convert.ToBoolean(pnlAdvancedFilter_CollapsiblePanelExtender.ClientState) :
                pnlAdvancedFilter_CollapsiblePanelExtender.Collapsed;

            var filter =
                 new CompanyPerformanceFilterSettings
                     {
                     StartYear = mpPeriodStart.SelectedYear,
                     StartMonth = mpPeriodStart.SelectedMonth,
                     EndYear = mpPeriodEnd.SelectedYear,
                     EndMonth = mpPeriodEnd.SelectedMonth,
                     ClientIdsList = SelectedClientIds,
                     ProjectOwnerIdsList = SelectedProjectOwnerIds,
                     PracticeIdsList = SelectedPracticeIds,
                     SalespersonIdsList = SelectedSalespersonIds,
                     ProjectGroupIdsList = SelectedGroupIds,
                     ShowActive = chbActive.Checked,
                     ShowInternal = chbInternal.Checked,
                     ShowCompleted = chbCompleted.Checked,
                     ShowProjected = chbProjected.Checked,
                     ShowExperimental = chbExperimental.Checked,

                     TotalOnlySelectedDateWindow = chbPeriodOnly.Checked,
                     HideAdvancedFilter = hideAdvancedFilterValue
                 };
            return filter;
        }

        protected void gvProjects_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            HandleHeaderRow(e);

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var project = e.Row.DataItem as Project;
                var rowVisible = false;
                if (project != null)
                {
                    // Determine whether to display the project in the list.
                    rowVisible = IsProjectVisible(project);

                    e.Row.CssClass = GetCssClassByProjectStatus(project.Status.Id);

                    SeniorityAnalyzer personListAnalyzer = null;
                    if (project.Id.HasValue)
                    {
                        /* 
                         * TEMPORARY COMMENT 
                         * Will be then used to fix #1257
                         */
                        personListAnalyzer = new SeniorityAnalyzer(DataHelper.CurrentPerson);
                        personListAnalyzer.OneWithGreaterSeniorityExists(project.ProjectPersons);

                        FillProjectNumberCell(e, project);
                        FillClientNameCell(e, project);
                        FillProjectNameCell(e, project);
                        FillProjectStartCell(e, project);
                        FillProjectEndCell(e, project);
                        DefineLastCell(e);
                    }

                    FillMonthCells(e, project, personListAnalyzer);
                    FillTotalsCell(e, project, personListAnalyzer);
                }

                e.Row.Visible = rowVisible;
            }
        }

        private void FillMonthCells(GridViewRowEventArgs e, Project project, SeniorityAnalyzer personListAnalyzer)
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
                        e.Row.Cells[i].Wrap = false;
                        if (project.Id.HasValue)
                        {
                            // Project.Id != null is for regular projects only
                            bool greaterSeniorityExists = personListAnalyzer != null && personListAnalyzer.GreaterSeniorityExists;
                            string grossMargin = greaterSeniorityExists ?
                                Resources.Controls.HiddenCellText
                                :
                                interestValue.Value.GrossMargin.ToString();
                            e.Row.Cells[i].Text =
                                string.Format(Resources.Controls.ProjectInterestFormat,
                                interestValue.Value.Revenue,
                                grossMargin);

                            if (greaterSeniorityExists)
                                OneGreaterSeniorityExists = true;
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
            return new DateTime(mpPeriodStart.SelectedYear,
                    mpPeriodStart.SelectedMonth,
                    Constants.Dates.FirstDay);
        }

        private void FillTotalsCell(GridViewRowEventArgs e, Project project, SeniorityAnalyzer personListAnalyzer)
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
                                totalMargin.ToString();

                e.Row.Cells[e.Row.Cells.Count - 1].Text =
                    string.Format(
                    Resources.Controls.ProjectInterestFormat, totalRevenue, strTotalMargin);

                if (greaterSeniorityExists)
                    OneGreaterSeniorityExists = true;
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

        private void DefineLastCell(GridViewRowEventArgs e)
        {
            // Define the last cell in table to close bScroll Div 
            if (e.Row.RowIndex == ((Project[])gvProjects.DataSource).Length - 1)
            {
                if (!chbPrintVersion.Checked)
                {
                    var ltr = new Literal {Mode = LiteralMode.PassThrough, Text = InnerTableTerminator};
                    e.Row.Cells[e.Row.Cells.Count - 1].Controls.Add(ltr);
                }
                tdAddButton.ColSpan = e.Row.Cells.Count;
                tdProjectTabViewSwitch.ColSpan = e.Row.Cells.Count;
            }
        }

        private static string GetCssClassByProjectStatus(int statusId)
        {
            // Shading project according to its status
            switch (statusId)
            {
                case (int)ProjectStatusType.Projected:
                    return "ProjectedProject";

                case (int)ProjectStatusType.Experimental:
                    return "ExperimentalProject";

                case (int)ProjectStatusType.Completed:
                    return "CompletedProject";
            }

            return null;
        }

        private void HandleHeaderRow(GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                string totalHeaderText =
                    string.Format(TotalHeaderFormat, chbPeriodOnly.Checked ? SelectedText : CurrentYearText);
                e.Row.Cells[e.Row.Cells.Count - 1].Text = 
                    !chbPrintVersion.Checked ? 
                    string.Format(totalHeaderText + InnerTableBeginner, e.Row.Cells.Count) : 
                    totalHeaderText;

                for (int i = NumberOfFixedColumns; i < e.Row.Cells.Count - 1; i++)
                {
                    PopulateMiniReportCell(e, i);
                }
            }
        }

        private static void FillProjectEndCell(GridViewRowEventArgs e, Project project)
        {
            // Project End date cell content
            var lblEndDate = new Label {ID = LabelEndDateId};
            if (project.EndDate.HasValue)
            {
                lblEndDate.Text = project.EndDate.Value.ToShortDateString();
            }
            e.Row.Cells[EndDateColumnIndex].Controls.Add(lblEndDate);
            e.Row.Cells[StartDateColumnIndex].CssClass = "CompPerfPeriod";
        }

        private static void FillProjectStartCell(GridViewRowEventArgs e, Project project)
        {
            // Project Start date cell content
            var lblStartDate = new Label {ID = LabelStartDateId};
            if (project.StartDate.HasValue)
            {
                lblStartDate.Text = project.StartDate.Value.ToShortDateString();
            }
            e.Row.Cells[StartDateColumnIndex].Controls.Add(lblStartDate);
            e.Row.Cells[StartDateColumnIndex].CssClass = "CompPerfPeriod";
        }

        private void FillProjectNameCell(GridViewRowEventArgs e, Project project)
        {
            // Project name cell content                        
            // Use ProjectNameCell Control for prepare friendly ToolTip popUp window
            var btnProject = (ProjectNameCell)LoadControl(Constants.ApplicationControls.ProjectNameCellControl);
            btnProject.ButtonProjectNameId = ButtonProjectNameId;
            btnProject.ButtonProjectNameText = HttpUtility.HtmlEncode(project.Name);
            btnProject.ButtonProjectNameToolTip = PrepareToolTipView(project);
            btnProject.ButtonProjectNameHref = GetRedirectUrl(project.Id.Value, Constants.ApplicationPages.ProjectDetail);

            e.Row.Cells[ProjectNameColumnIndex].Controls.Add(btnProject);
        }

        private static string GetRedirectUrl(int argProjectId, string targetUrl)
        {
            return string.Format(Constants.ApplicationPages.DetailRedirectWithReturnFormat,
                                 targetUrl,
                                 argProjectId,
                                 Constants.ApplicationPages.Projects);
        }

        private static void FillClientNameCell(GridViewRowEventArgs e, Project project)
        {
            // Client name cell content
            var btnClient = new HyperLink
                                {
                                    ID = ButtonClientNameId,
                                    Text = HttpUtility.HtmlEncode(project.Client.Name),
                                    NavigateUrl = 
                                    GetRedirectUrl(project.Client.Id.Value, Constants.ApplicationPages.ClientDetails)
                                };
            e.Row.Cells[ClientNameColumnIndex].Controls.Add(btnClient);
        }

        private static void FillProjectNumberCell(GridViewRowEventArgs e, Project project)
        {
            // It is a regular project
            // Project Number cell content
            var lblProjectNumber = new Label
                                       {
                                           ID = LabelProjectNumberId,
                                           Text = HttpUtility.HtmlEncode(project.ProjectNumber)
                                       };
            e.Row.Cells[ProjectNumberColumnIndex].Controls.Add(lblProjectNumber);
            e.Row.Cells[ProjectNumberColumnIndex].CssClass = "CompPerfProjectNumber";
        }

        protected void gvProjects_Sorting(object sender, GridViewSortEventArgs e)
        {
            GridViewSortExpression = e.SortExpression;
            GridViewSortDirection = GetSortDirection();
            BindProjectGrid(false);

			DisplaySelectedReport();
        }

        private void BindProjectGrid(bool maintainSort)
        {
            gvProjects.DataSource = SortProjects(ProjectList);
            gvProjects.DataBind();
        }

        private string GetSortDirection()
        {
            switch (GridViewSortDirection)
            {
                case "Ascending":
                    GridViewSortDirection = "Descending";
                    break;
                case "Descending":
                    GridViewSortDirection = "Ascending";
                    break;
            }
            return GridViewSortDirection;
        }

        private Project[] SortProjects(Project[] projectList)
        {
            if (projectList != null & projectList.Length > 0)
            {
                if (!string.IsNullOrEmpty(GridViewSortExpression))
                {
                    Array.Sort(projectList, new ProjectComparer(GridViewSortExpression));
                    if (GridViewSortDirection != "Ascending")
                        Array.Reverse(projectList);
                }
                return projectList;
            }
            return ProjectList;
        }

        protected void gvFinancialSummary_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var project = e.Row.DataItem as Project;
                if (project != null)
                {
                    var monthBegin =
                        new DateTime(mpPeriodStart.SelectedYear,
                            mpPeriodStart.SelectedMonth,
                            Constants.Dates.FirstDay);

                    int periodLength = GetPeriodLength();

                    // Displaying the interest values (main cell data)
                    for (int i = 1, k = 0;
                        k < periodLength;
                        i++, k++, monthBegin = monthBegin.AddMonths(1))
                    {
                        var monthEnd =
                            new DateTime(monthBegin.Year,
                                monthBegin.Month,
                                DateTime.DaysInMonth(monthBegin.Year, monthBegin.Month));

                        foreach (KeyValuePair<DateTime, ComputedFinancials> interestValue in
                            project.ProjectedFinancialsByMonth)
                        {
                            if (IsInMonth(interestValue.Key, monthBegin, monthEnd))
                            {
                                e.Row.Cells[i].Wrap = false;

                                // Project.Id != null is for regular projects only
                                switch (e.Row.RowIndex)
                                {
                                    //Revenue
                                    case FS_RevenueRowIndex:
                                        e.Row.Cells[TabNameColumnIndex].Text = FS_RevenueRowName;
                                        e.Row.Cells[i].Text = interestValue.Value.Revenue.ToString();
                                        break;
                                    //COGS
                                    case FS_COGSRowIndex:
                                        e.Row.Cells[TabNameColumnIndex].Text = FS_COGSRowName;
                                        e.Row.Cells[i].Text =
                                            OneGreaterSeniorityExists ?
                                            Resources.Controls.HiddenCellText :
                                            interestValue.Value.Cogs.ToString();
                                        break;
                                    //GrossMargin
                                    case FS_GrossMarginRowIndex:
                                        e.Row.Cells[TabNameColumnIndex].Text = FS_GrossMarginRowName;
                                        e.Row.Cells[i].Text =
                                            OneGreaterSeniorityExists ?
                                            Resources.Controls.HiddenCellText :
                                            interestValue.Value.GrossMargin.ToString();
                                        break;
                                    // Margin %
                                    case FS_TargetMarginRowIndex:
                                        e.Row.Cells[TabNameColumnIndex].Text = FS_TargetMarginRowName;
                                        e.Row.Cells[i].Text =
                                            OneGreaterSeniorityExists ?
                                            Resources.Controls.HiddenCellText :
                                            string.Format(Constants.Formatting.PercentageFormat, interestValue.Value.TargetMargin);
                                        break;
                                    //Expenses
                                    case FS_ExpensesRowIndex:
                                        e.Row.Cells[TabNameColumnIndex].Text = FS_ExpensesRowName;
                                        e.Row.Cells[i].Text = interestValue.Value.Cogs.ToString();
                                        e.Row.Visible = userIsAdministrator;
                                        break;
                                    //SalesCommission
                                    case FS_SalesCommissionsRowIndex:
                                        e.Row.Cells[TabNameColumnIndex].Text = FS_SalesCommissionsRowName;
                                        e.Row.Cells[i].Text =
                                            OneGreaterSeniorityExists ?
                                            Resources.Controls.HiddenCellText :
                                            interestValue.Value.SalesCommission.ToString();
                                        break;
                                    //PMComission
                                    case FS_PMCommissionsRowIndex:
                                        e.Row.Cells[TabNameColumnIndex].Text = FS_PMCommissionsRowName;
                                        e.Row.Cells[i].Text =
                                            OneGreaterSeniorityExists ?
                                            Resources.Controls.HiddenCellText :
                                            interestValue.Value.PracticeManagementCommission.ToString();
                                        break;
                                    // Bench
                                    case FS_BenchRowIndex:
                                        e.Row.Cells[TabNameColumnIndex].Text = FS_BenchRowName;
                                        string grossMarginValue =
                                            OneGreaterSeniorityExists ?
                                            Resources.Controls.HiddenCellText :
                                            interestValue.Value.GrossMargin.ToString();
                                        e.Row.Cells[i].Text = grossMarginValue;
                                        break;
                                    // Admin
                                    case FS_AdminRowIndex:
                                        e.Row.Cells[TabNameColumnIndex].Text = FS_AdminRowName;
                                        e.Row.Cells[i].Text = interestValue.Value.Cogs.ToString();
                                        break;
                                    // Net Profit
                                    case FS_NetProfitRowIndex:
                                        e.Row.Cells[TabNameColumnIndex].Text = FS_NetProfitRowName;
                                        e.Row.Cells[i].Text = interestValue.Value.GrossMargin.ToString();
                                        e.Row.Visible = userIsAdministrator;
                                        break;
                                }
                            }
                        }
                    }

                    for (int i = 1; i < e.Row.Cells.Count; i++)
                    {
                        e.Row.Cells[i].CssClass = CompPerfDataCssClass;
                        e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;
                    }
                }
            }
        }

        protected void gvCommissionsAndRates_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var project = e.Row.DataItem as Project;
                if (project != null)
                {
                    var monthBegin =
                        new DateTime(mpPeriodStart.SelectedYear,
                            mpPeriodStart.SelectedMonth,
                            Constants.Dates.FirstDay);

                    int periodLength = GetPeriodLength();

                    // Displaying the interest values (main cell data)
                    for (int i = 1, k = 0;
                        k < periodLength;
                        i++, k++, monthBegin = monthBegin.AddMonths(1))
                    {
                        var monthEnd =
                            new DateTime(monthBegin.Year,
                                monthBegin.Month,
                                DateTime.DaysInMonth(monthBegin.Year, monthBegin.Month));

                        foreach (KeyValuePair<DateTime, ComputedFinancials> interestValue in
                            project.ProjectedFinancialsByMonth)
                        {
                            if (IsInMonth(interestValue.Key, monthBegin, monthEnd))
                            {
                                e.Row.Cells[i].Wrap = false;
								
                                // Project.Id != null is for regular projects only
                                switch (e.Row.RowIndex)
                                {
                                    //GrossMargin
                                    case CR_GrossMarginEligibleForCommissions:
                                        e.Row.Cells[TabNameColumnIndex].Text = "Gross Margin eligible for commissions";
                                        e.Row.Cells[i].Text = interestValue.Value.GrossMargin.ToString();
                                        break;
                                    //Sales Commision
                                    case CR_SalesCommissionsIndex:
                                        e.Row.Cells[TabNameColumnIndex].Text = "Sales Commissions";
                                        e.Row.Cells[i].Text = interestValue.Value.SalesCommission.ToString();
                                        break;
                                    //PMComission
                                    case CR_PMCommissionsIndex:
                                        e.Row.Cells[TabNameColumnIndex].Text = "PM Commissions";
                                        e.Row.Cells[i].Text = interestValue.Value.PracticeManagementCommission.ToString();
                                        break;
                                    //Avg Bill Rate
                                    case CR_AvgBillRateIndex:
                                        e.Row.Cells[TabNameColumnIndex].Text = "Avg Bill Rate";
										e.Row.Cells[i].Text =
											interestValue.Value.BillRate.HasValue ?
											interestValue.Value.BillRate.Value.ToString() : new PracticeManagementCurrency().ToString();
                                        break;
                                    //Avg Pay Rate
                                    case CR_AvgPayRateIndex:
                                        e.Row.Cells[TabNameColumnIndex].Text = "Avg Pay Rate";
                                        e.Row.Cells[i].Text =
											interestValue.Value.LoadedHourlyPay.HasValue ?
											interestValue.Value.LoadedHourlyPay.Value.ToString() : new PracticeManagementCurrency().ToString();
                                        break;
                                }
                            }
                        }
                    }

					for (int i = 1; i < e.Row.Cells.Count; i++)
					{
						e.Row.Cells[i].CssClass = CompPerfDataCssClass;
						e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;
					}
				}
            }
        }


        protected void gvBenchRollOffDates_RowDataBound(object sender, GridViewRowEventArgs e)
        {
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				var project = e.Row.DataItem as Project;
				bool rowVisible = false;

				if (project != null)
				{
					var monthBegin =
						new DateTime(mpPeriodStart.SelectedYear,
							mpPeriodStart.SelectedMonth,
							Constants.Dates.FirstDay);

					var periodLength = GetPeriodLength();
					
					// Displaying the interest values (main cell data)
                    var current = DataHelper.CurrentPerson;
                    for (int i = 1, k = 0;
						k < periodLength;
						i++, k++, monthBegin = monthBegin.AddMonths(1))
					{
						var monthEnd =
							new DateTime(monthBegin.Year,
								monthBegin.Month,
								DateTime.DaysInMonth(monthBegin.Year, monthBegin.Month));

						foreach (var interestValue in project.ProjectedFinancialsByMonth)
						{
							if (IsInMonth(interestValue.Key, monthBegin, monthEnd))
							{
								rowVisible = true;

								if (e.Row.Parent.Parent == gvBenchCosts)
								{
								    var seniority = 
                                        (new SeniorityAnalyzer(current)).IsOtherGreater(project.AccessLevel.Id);
									if (!seniority)
									{
										e.Row.Cells[i].Text = 
                                            string.Format(
                                                ReportFormat, 
                                                e.Row.Cells[i].Text, 
                                                (interestValue.Value.Timescale == TimescaleType.Salary ? 
                                                    interestValue.Value.GrossMargin.ToString()
                                                    :                                                    
                                                    Resources.Controls.HourlyLabel));
									}
									else
									{
										e.Row.Cells[i].Text =
											string.Format(ReportFormat, e.Row.Cells[i].Text, Resources.Controls.HiddenAmount);
									}
									e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;
								}
							}
						}
					}
				}

				for (int i = 1; i < e.Row.Cells.Count; i++)
				{
					e.Row.Cells[i].CssClass = CompPerfDataCssClass;
				}

				e.Row.Visible = rowVisible;
			}
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

			// Main GridView
            gvProjects.DataSource = ProjectList;
            gvProjects.DataBind();

			DisplaySelectedReport();
        }

		protected override void RedirectWithBack(string redirectUrl, string backUrl)
		{
			SaveFilterSettings();
			base.RedirectWithBack(redirectUrl, backUrl);
		}

		private void DisplaySelectedReport()
		{
		    

			View activeView = mvProjectTab.GetActiveView();

			if (activeView == vwFinancialSummary)
			{
				// Get neccessary projects info
				Project[] financialSummary = GetFinancialSummary(ProjectList, BenchList, ExpenseList, PeriodStart, PeriodEnd);

				// Financial Summary Grid
				gvFinancialSummary.DataSource = PopulateFinancialSummaryGrid(financialSummary, PeriodStart, PeriodEnd);
				gvFinancialSummary.DataBind();
			}
			else if (activeView == vwCommissionsAndRates)
			{
				// Get neccessary projects info
				Project[] financialSummary = GetFinancialSummary(ProjectList, BenchList, ExpenseList, PeriodStart, PeriodEnd);

				// Commissions And Rates Grid
				gvCommissionsAndRates.DataSource = PopulateCommissionsAndRatesGrid(financialSummary);
				gvCommissionsAndRates.DataBind();
			}
			else if (activeView == vwPersonStats)
			{
				DisplayPersonStatsReport(PersonStats);
			}
			else /*if (activeView == vwBenchRollOffDates)
			{
				// Bench Roll Off Dates
				gvBenchRollOffDates.DataSource = PopulateBenchRollOffDatesGrid(BenchList);
				gvBenchRollOffDates.DataBind();
			}
            else*/ if (activeView == vwBenchCosts)
            {
                // Bench Costs
                gvBenchCosts.DataSource = PopulateBenchRollOffDatesGrid(BenchList);
                gvBenchCosts.DataBind();
            }

		    
		}

		private void DisplayPersonStatsReport(IEnumerable<PersonStats> stats)
		{
		    for (var i = tblPersonStats.Rows[0].Cells.Count - 1; i > 0; i--)
		        for (var j = 0; j < tblPersonStats.Rows.Count; j++)
		            tblPersonStats.Rows[j].Cells.RemoveAt(i);

		    foreach (var t in stats)
			{
			    tblPersonStats.Rows[0].Cells.Add(
			        new TableHeaderCell { Text = t.Date.ToString(Constants.Formatting.CompPerfMonthYearFormat), CssClass = CompPerfDataCssClass, HorizontalAlign = HorizontalAlign.Center });
			    tblPersonStats.Rows[1].Cells.Add(new TableCell { Text = t.Revenue.ToString(), CssClass = CompPerfDataCssClass, HorizontalAlign = HorizontalAlign.Right });
			    tblPersonStats.Rows[2].Cells.Add(new TableCell { Text = t.EmployeesCount.ToString(), CssClass = CompPerfDataCssClass, HorizontalAlign = HorizontalAlign.Right });
			    tblPersonStats.Rows[3].Cells.Add(new TableCell { Text = t.ConsultantsCount.ToString(), CssClass = CompPerfDataCssClass, HorizontalAlign = HorizontalAlign.Right });
			    tblPersonStats.Rows[4].Cells.Add(
			        new TableCell { Text = t.VirtualConsultants.ToString(Constants.Formatting.TwoDecimalsFormat), CssClass = CompPerfDataCssClass, HorizontalAlign = HorizontalAlign.Right });
			    tblPersonStats.Rows[5].Cells.Add(
			        new TableCell { Text = t.VirtualConsultantsChange.ToString(Constants.Formatting.TwoDecimalsFormat), CssClass = CompPerfDataCssClass, HorizontalAlign = HorizontalAlign.Right });
			    tblPersonStats.Rows[6].Cells.Add(new TableCell { Text = t.RevenuePerEmployee.ToString(), CssClass = CompPerfDataCssClass, HorizontalAlign = HorizontalAlign.Right });
			    tblPersonStats.Rows[7].Cells.Add(new TableCell { Text = t.RevenuePerConsultant.ToString(), CssClass = CompPerfDataCssClass, HorizontalAlign = HorizontalAlign.Right });
                
			    var adminCostAsPerctageOfRev =
			        t.Revenue != 0 ?
			                           (decimal)t.AdminCosts / (decimal)t.Revenue : 0M;
			    tblPersonStats.Rows[8].Cells.Add(
			        new TableCell { Text = adminCostAsPerctageOfRev.ToString("P"), CssClass = CompPerfDataCssClass, HorizontalAlign = HorizontalAlign.Right });
			}
		    foreach (TableRow row in tblPersonStats.Rows)
		        foreach (TableCell cell in row.Cells)
		            cell.Visible = true;

		    if (!userIsAdministrator)
            {
                tblPersonStats.Rows[2].Visible = false;
                tblPersonStats.Rows[6].Visible = false;
                tblPersonStats.Rows[8].Visible = false;
            }
		}

        /// <summary>
        /// Computes and displays total values;
        /// </summary>
        private static Project[] GetFinancialSummary(
            IEnumerable<Project> projects, 
            IEnumerable<Project> benches, 
            IEnumerable<MonthlyExpense> expenses, 
            DateTime periodStart, 
            DateTime periodEnd)
        {
            

            // Prepare Financial Summary GridView            
            var financialSummaryRevenue = new Project();
            var financialAvgRates = new Project();
            var financialExpenses = new Project();

            financialSummaryRevenue.ProjectedFinancialsByMonth = new Dictionary<DateTime, ComputedFinancials>();
            financialAvgRates.ProjectedFinancialsByMonth = new Dictionary<DateTime, ComputedFinancials>();
            financialExpenses.ProjectedFinancialsByMonth = new Dictionary<DateTime, ComputedFinancials>();
            
            for (var dtTemp = periodStart; dtTemp <= periodEnd; dtTemp = dtTemp.AddMonths(1))
            {
                var financials = new ComputedFinancials();
                var avgRates = new ComputedFinancials();
                var expense = new ComputedFinancials();
                var totalExpenses = new PracticeManagementCurrency();

                // Looking through the projects
                foreach (Project project in projects)
                {
                    foreach (KeyValuePair<DateTime, ComputedFinancials> projectFinancials in project.ProjectedFinancialsByMonth)
                    {
                        if (projectFinancials.Key.Year == dtTemp.Year &&
							projectFinancials.Key.Month == dtTemp.Month &&
							project.Id.HasValue)
                        {
                            financials.Revenue += projectFinancials.Value.Revenue;
                            financials.Cogs += projectFinancials.Value.Cogs;
                            financials.GrossMargin += projectFinancials.Value.GrossMargin;

                            // Expenses                            
                            financials.RevenueNet += projectFinancials.Value.RevenueNet;
                            financials.SalesCommission += projectFinancials.Value.SalesCommission;
                            financials.PracticeManagementCommission += projectFinancials.Value.PracticeManagementCommission;

							// Average rates
							avgRates.Revenue += projectFinancials.Value.Revenue;
							avgRates.RevenueNet += projectFinancials.Value.RevenueNet;
							avgRates.Cogs += projectFinancials.Value.Cogs;
							avgRates.HoursBilled += projectFinancials.Value.HoursBilled;
                        }
                    }
                }

                // Net Profit = GM - (Expenses + Bench + Sales Commissions + PM Commissions + Admin)
                foreach (var expenseItem in expenses)
                    if (expenseItem.MonthlyAmount.ContainsKey(dtTemp))
                        totalExpenses += expenseItem.MonthlyAmount[dtTemp];

                expense.Cogs = totalExpenses;                

                financialSummaryRevenue.ProjectedFinancialsByMonth.Add(dtTemp, financials);
                financialAvgRates.ProjectedFinancialsByMonth.Add(dtTemp, avgRates);
                financialExpenses.ProjectedFinancialsByMonth.Add(dtTemp, expense);
            }

            var financialSummary = new Project[5];
            financialSummary[SummaryInfoProjectIndex] = financialSummaryRevenue;

            foreach (var project in benches)
                switch (project.Name)
                {
                    case "Bench":
                        financialSummary[BenchProjectIndex] = project;
                        break;

                    case "Admin":
                        financialSummary[AdminProjectIndex] = project;
                        break;
                }

            financialSummary[AvgRatesProjectIndex] = financialAvgRates;
            financialSummary[ExpensesProjectIndex] = financialExpenses;

            

            return financialSummary;
        }

		private static Project[] PopulateFinancialSummaryGrid(
            IList<Project> project, 
            DateTime periodStart, 
            DateTime periodEnd)
		{
		    

			var financialSummaryGrid = new Project[FS_TotalRowCount];
			for (var i = 0; i < FS_TotalRowCount; i++)
			{
				financialSummaryGrid[i] = project[SummaryInfoProjectIndex];
			}
			financialSummaryGrid[FS_ExpensesRowIndex] = project[ExpensesProjectIndex];
			financialSummaryGrid[FS_BenchRowIndex] = project[BenchProjectIndex];
			financialSummaryGrid[FS_AdminRowIndex] = project[AdminProjectIndex];

			//Net Profit calculation
			var financialNetProfit = new Project
			                             {
			                                 ProjectedFinancialsByMonth = new Dictionary<DateTime, ComputedFinancials>()
			                             };

		    for (DateTime dtTemp = periodStart; dtTemp <= periodEnd; dtTemp = dtTemp.AddMonths(1))
			{
				var financials = new ComputedFinancials();

				foreach (KeyValuePair<DateTime, ComputedFinancials> projectFinancials in
					financialSummaryGrid[FS_GrossMarginRowIndex].ProjectedFinancialsByMonth)
				{
					if (projectFinancials.Key.Year == dtTemp.Year && projectFinancials.Key.Month == dtTemp.Month)
					{
					    // Net Profit = GM - (Expenses + Bench + Sales Commissions + PM Commissions + Admin)
					    var expRow = financialSummaryGrid[FS_ExpensesRowIndex];
					    var salesComRow = financialSummaryGrid[FS_SalesCommissionsRowIndex];
					    var pmComRow = financialSummaryGrid[FS_PMCommissionsRowIndex];
					    var benchRow = financialSummaryGrid[FS_BenchRowIndex];
					    var adminRow = financialSummaryGrid[FS_AdminRowIndex];
					    financials.GrossMargin =
							projectFinancials.Value.GrossMargin -
							((expRow != null && expRow.ProjectedFinancialsByMonth.ContainsKey(dtTemp) ?
							  expRow.ProjectedFinancialsByMonth[dtTemp].Cogs : 0)
								+
							  (salesComRow != null && salesComRow.ProjectedFinancialsByMonth.ContainsKey(dtTemp) ?
							   salesComRow.ProjectedFinancialsByMonth[dtTemp].SalesCommission : 0)
								+
							  (pmComRow != null && pmComRow.ProjectedFinancialsByMonth.ContainsKey(dtTemp) ?
							   pmComRow.ProjectedFinancialsByMonth[dtTemp].PracticeManagementCommission : 0)
								+
							  (benchRow != null && benchRow.ProjectedFinancialsByMonth.ContainsKey(dtTemp) ?
							   benchRow.ProjectedFinancialsByMonth[dtTemp].Cogs : 0)
								+
							  (adminRow != null && adminRow.ProjectedFinancialsByMonth.ContainsKey(dtTemp) ?
							   adminRow.ProjectedFinancialsByMonth[dtTemp].Cogs : 0)
							);
					}
				}
				financialNetProfit.ProjectedFinancialsByMonth.Add(dtTemp, financials);
			}

			financialSummaryGrid[FS_NetProfitRowIndex] = financialNetProfit;

		    

			return financialSummaryGrid;
		}

        private static Project[] PopulateCommissionsAndRatesGrid(Project[] project)
        {
            var commissionsAndRatesGrid = new Project[CR_TotalRowCount];
            for (var i = 0; i < CR_TotalRowCount; i++)
            {
                commissionsAndRatesGrid[i] = project[SummaryInfoProjectIndex];
            }
            commissionsAndRatesGrid[CR_AvgBillRateIndex] = project[AvgRatesProjectIndex];
            commissionsAndRatesGrid[CR_AvgPayRateIndex] = project[AvgRatesProjectIndex];

            return commissionsAndRatesGrid;
        }

        private static Project[] PopulateBenchRollOffDatesGrid(Project[] project)
        {
            // exclude from project Bench Total and Admin expense => (project.Length - 2)
            // only bench Person bind
            var benchRollOffDatesGrid = new Project[project.Length - 2];
            for (var i = 0; i < project.Length - 2; i++)
            {
                benchRollOffDatesGrid[i] = project[i];
            }
            return benchRollOffDatesGrid;
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
                    cblProjectGroup.AllSelectedReturnType = ScrollingDropDown.AllSelectedType.Null;
                }

                PraticeManagement.Controls.DataHelper.FillSalespersonList(
                    person, cblSalesperson,
                    Resources.Controls.AllSalespersonsText, 
                    false);

                PraticeManagement.Controls.DataHelper.FillProjectOwnerList(cblProjectOwner,
                    Resources.Controls.AllPracticeMgrsText,
					null,
					false);

                PraticeManagement.Controls.DataHelper.FillPracticeList(
                    person,
                    cblPractice,
                    Resources.Controls.AllPracticesText);

                PraticeManagement.Controls.DataHelper.FillClientsAndGroups(
                    cblClient, cblProjectGroup);

                // Set the default viewable interval.
				mpPeriodStart.SelectedYear = filter.StartYear;
				mpPeriodStart.SelectedMonth = filter.StartMonth;

				mpPeriodEnd.SelectedYear = filter.EndYear;
				mpPeriodEnd.SelectedMonth = filter.EndMonth;

				chbPeriodOnly.Checked = filter.TotalOnlySelectedDateWindow;

				chbActive.Checked = filter.ShowActive;
				chbCompleted.Checked = filter.ShowCompleted;
				chbExperimental.Checked = filter.ShowExperimental;
				chbProjected.Checked = filter.ShowProjected;

                SelectedClientIds = filter.ClientIdsList;
                SelectedPracticeIds = filter.PracticeIdsList;
                SelectedProjectOwnerIds = filter.ProjectOwnerIdsList;
                SelectedSalespersonIds = filter.SalespersonIdsList;
                SelectedGroupIds = filter.ProjectGroupIdsList;

				pnlAdvancedFilter_CollapsiblePanelExtender.Collapsed = filter.HideAdvancedFilter;
				pnlAdvancedFilter.Style[HtmlTextWriterStyle.Overflow] = "hidden";
				pnlAdvancedFilter.Height = filter.HideAdvancedFilter ? Unit.Pixel(0) : Unit.Empty;
            }
            else
            {
				Page.Validate(valsPerformance.ValidationGroup);
            }

            if (!IsPostBack || Page.IsValid)
            {
                var periodStart =
                    new DateTime(mpPeriodStart.SelectedYear, mpPeriodStart.SelectedMonth, Constants.Dates.FirstDay);
                var monthsInPeriod = GetPeriodLength();

                AddMonthColumn(gvProjects, NumberOfFixedColumns, periodStart, monthsInPeriod);
                AddMonthColumn(gvFinancialSummary, 1, periodStart, monthsInPeriod);
                AddMonthColumn(gvCommissionsAndRates, 1, periodStart, monthsInPeriod);
                //AddMonthColumn(gvBenchRollOffDates, 1, periodStart, monthsInPeriod);
                AddMonthColumn(gvBenchCosts, 1, periodStart, monthsInPeriod);
            }

            
        }

        private static CompanyPerformanceFilterSettings InitFilter()
        {
            return SerializationHelper.DeserializeCookie(CompanyPerformanceFilterKey) as CompanyPerformanceFilterSettings ??
                   new CompanyPerformanceFilterSettings();
        }

        private static void AddMonthColumn(GridView gridView, int numberOfFixedColumns, DateTime periodStart, int monthsInPeriod)
        {            
            while (gridView.Columns.Count > numberOfFixedColumns + 1)
            {
                gridView.Columns.RemoveAt(numberOfFixedColumns);
            }

            for (int i = numberOfFixedColumns, k = 0; k < monthsInPeriod; i++, k++)
            {
                var newColumn = new BoundField
                                    {
                                        HeaderText = periodStart.ToString(Constants.Formatting.CompPerfMonthYearFormat)
                                    };
                newColumn.HeaderStyle.Wrap = false;
                newColumn.HeaderStyle.CssClass = newColumn.ItemStyle.CssClass = "CompPerfMonthSummary";
                gridView.Columns.Insert(i, newColumn);

                periodStart = periodStart.AddMonths(1);
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
                (mpPeriodEnd.SelectedYear - mpPeriodStart.SelectedYear) * Constants.Dates.LastMonth +
                (mpPeriodEnd.SelectedMonth - mpPeriodStart.SelectedMonth + 1);
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

		/// <summary>
		/// Generates a month mini-report view.
		/// </summary>
		/// <param name="e">Grid View header row.</param>
		/// <param name="i">Header cell index.</param>
        private void PopulateMiniReportCell(GridViewRowEventArgs e, int i)
        {
            var lblHeader =
                new Label { Text = e.Row.Cells[i].Text, ID = string.Format(LabelHeaderIdFormat, i) };
            var pnlReport = new Panel { ID = string.Format(PanelReportIdFormat, i), CssClass = MINIREPORT_CSSCLASS };

            var closeButtonId = string.Format(CloseReportButtonIdFormat, i);
            var tblReport = new Table();
            tblReport.Rows.Add(new TableRow());
            tblReport.Rows[0].Cells.Add(new TableHeaderCell());
            tblReport.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Right;
            tblReport.Rows[0].Cells[0].Controls.Add(
                new HtmlInputButton { ID = closeButtonId, Value = "X" });

            tblReport.Rows.Add(new TableRow());
            tblReport.Rows[1].Cells.Add(new TableCell { ID = string.Format(ReportCellIdFormat, i) });

            pnlReport.Controls.Add(tblReport);
            pnlReport.Controls.Add(
                new DynamicPopulateExtender
                    {
                    PopulateTriggerControlID = lblHeader.ID,
                    TargetControlID = tblReport.Rows[1].Cells[0].ID,
                    ContextKey = lblHeader.Text + "," +
                                chbProjected.Checked.ToString() + "," +
                                chbCompleted.Checked.ToString() + "," +
                                chbActive.Checked.ToString() + "," +
                                chbExperimental.Checked.ToString() + "," +
                                this.chbInternal.Checked.ToString(),
                    ServiceMethod = POPULATE_EXTENDER_SERVICEMETHOD,
                    UpdatingCssClass = POPULATE_EXTENDER_UPDATINGCSS
                });

            e.Row.Cells[i].Text = string.Empty;
            e.Row.Cells[i].Controls.Add(lblHeader);
            e.Row.Cells[i].Controls.Add(pnlReport);

            var animShow =
                new AnimationExtender
                    {
                    TargetControlID = lblHeader.ID,
                    Animations = string.Format(ANIMATION_SHOW_SCRIPT, pnlReport.ID)
                };

            var animHide =
                new AnimationExtender
                    {
                    TargetControlID = closeButtonId,
                    Animations = string.Format(ANIMATION_HIDE_SCRIPT, pnlReport.ID)
                };

            e.Row.Cells[i].Controls.Add(animShow);
            e.Row.Cells[i].Controls.Add(animHide);
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

        protected void buttonAddProject_Click(object sender, EventArgs e)
        {
            Redirect(Constants.ApplicationPages.ProjectDetail);
        }

        protected void btnView_Command(object sender, CommandEventArgs e)
        {
            int viewIndex = int.Parse((string)e.CommandArgument);
            SelectView((Control)sender, viewIndex);
        }

        private void SelectView(Control sender, int viewIndex)
        {
            mvProjectTab.ActiveViewIndex = viewIndex;

            foreach (TableCell cell in tblProjectTabViewSwitch.Rows[0].Cells)
                cell.CssClass = string.Empty;

            ((WebControl)sender.Parent).CssClass = "SelectedSwitch";

			DisplaySelectedReport();
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

        #endregion
    }
}

