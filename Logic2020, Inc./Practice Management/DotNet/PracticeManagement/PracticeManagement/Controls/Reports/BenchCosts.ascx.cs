using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;
using PraticeManagement.Security;
using System.Collections;
using System.Web.Security;

namespace PraticeManagement.Controls.Reports
{
    public partial class BenchCosts : ProjectsReportsBase
    {
        private const string ReportFormat = "{0}&nbsp;{1}";
        private const string ConsultantNameSortOrder = "ConsultantNameSortOrder";
        private const string PracticeSortOrder = "PracticeSortOrder";
        private const string StatusSortOrder = "StatusSortOrder";
        private const string Descending = "Descending";
        private const string Ascending = "Ascending";
        private const string ReportContextKey = "ReportContext";
        private const string BenchListKey = "BenchList";
        private const string UserIsAdminKey = "UserIsAdmin";
        private Dictionary<DateTime, Decimal> monthlyTotals;
        private BenchReportContext ReportContext
        {
            get
            {
                if (ViewState[ReportContextKey] == null)
                {
                    BenchReportContext reportContext = new BenchReportContext()
                    {
                        Start = mpStartDate.MonthBegin,
                        End = mpEndDate.MonthEnd,
                        ActivePersons = cbActivePersons.Checked,
                        ProjectedPersons = cbProjectedPersons.Checked,
                        ActiveProjects = chbActiveProjects.Checked,
                        ProjectedProjects = chbProjectedProjects.Checked,
                        ExperimentalProjects = chbExperimentalProjects.Checked,
                        CompletedProjects = chbCompletedProjects.Checked,
                        UserName = DataHelper.CurrentPerson.Alias,
                        PracticeIds = string.IsNullOrEmpty(cblPractices.SelectedItems) ? string.Empty : cblPractices.SelectedItems,
                        IncludeOverheads = chbIncludeOverHeads.Checked,
                        IncludeZeroCostEmployees = chbIncludeZeroCostEmps.Checked,
                        TimeScaleIds = string.IsNullOrEmpty(cblPayType.SelectedItems) ? string.Empty : cblPayType.SelectedItems

                    };
                    ViewState[ReportContextKey] = reportContext;
                }
                return ViewState[ReportContextKey] as BenchReportContext;
            }
            set
            {
                ViewState[ReportContextKey] = value;
            }
        }

        private IEnumerable<Project> BenchList
        {
            get
            {
                if (ViewState[BenchListKey] == null)
                {
                    var benchList = GetBenchList();
                    ViewState[BenchListKey] = benchList;
                }
                return ViewState[BenchListKey] as IEnumerable<Project>;
            }
            set
            {
                ViewState[BenchListKey] = value;
            }
        }


        private Dictionary<DateTime, Decimal> MonthlyTotals
        {
            get
            {
                if (monthlyTotals == null)
                {
                    monthlyTotals = new Dictionary<DateTime, decimal>();
                }
                return monthlyTotals;
            }
            set
            {
                monthlyTotals = value;
            }

        }

        private bool UserIsAdmin
        {
            get
            {
                if (ViewState[UserIsAdminKey] == null)
                {
                    ViewState[UserIsAdminKey] = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
                }
                return bool.Parse(ViewState[UserIsAdminKey].ToString());
            }

        }

        protected void btnSortConsultant_Click(object sender, EventArgs e)
        {
            var sortOrder = gvBenchCosts.Attributes[ConsultantNameSortOrder];
            IEnumerable<Project> benchList = null;
            if (chbSeperateInternalExternal.Checked)
            {
                benchList = BenchList.ToList().FindAll(P => !P.Practice.IsCompanyInternal);
            }
            else
            {
                benchList = BenchList;
            }

            if (string.IsNullOrEmpty(sortOrder) || sortOrder == Descending)
            {
                benchList = benchList.OrderByDescending(P => P.Name);
                gvBenchCosts.Attributes[ConsultantNameSortOrder] = Ascending;
            }
            else
            {
                benchList = benchList.OrderBy(P => P.Name);
                gvBenchCosts.Attributes[ConsultantNameSortOrder] = Descending;
            }
            gvBenchCosts.Attributes[PracticeSortOrder] = Ascending;
            gvBenchCosts.Attributes[StatusSortOrder] = Ascending;
            AddMonthColumn(3, ReportContext.Start, GetPeriodLength(), gvBenchCosts);
            gvBenchCosts.DataSource = benchList;
            gvBenchCosts.DataBind();
        }

        protected void btnSortInternalConsultant_Click(object sender, EventArgs e)
        {
            var sortOrder = gvBenchCostsInternal.Attributes[ConsultantNameSortOrder];


            var benchList = BenchList.ToList().FindAll(P => P.Practice.IsCompanyInternal);



            if (string.IsNullOrEmpty(sortOrder) || sortOrder == Descending)
            {
                benchList = (benchList).OrderByDescending(P => P.Name).ToList();
                gvBenchCostsInternal.Attributes[ConsultantNameSortOrder] = Ascending;
            }
            else
            {
                benchList = benchList.OrderBy(P => P.Name).ToList();
                gvBenchCostsInternal.Attributes[ConsultantNameSortOrder] = Descending;
            }
            gvBenchCostsInternal.Attributes[PracticeSortOrder] = Ascending;
            gvBenchCostsInternal.Attributes[StatusSortOrder] = Ascending;
            AddMonthColumn(3, ReportContext.Start, GetPeriodLength(), gvBenchCostsInternal);
            gvBenchCostsInternal.DataSource = benchList;
            gvBenchCostsInternal.DataBind();

            gvBenchCostsInternal.Focus();
        }

        protected void btnSortPractice_Click(object sender, EventArgs e)
        {

            var sortOrder = gvBenchCosts.Attributes[PracticeSortOrder];
            IEnumerable<Project> benchList = null;
            if (chbSeperateInternalExternal.Checked)
            {
                benchList = BenchList.ToList().FindAll(P => !P.Practice.IsCompanyInternal);
            }
            else
            {
                benchList = BenchList;
            }
            if (string.IsNullOrEmpty(sortOrder) || sortOrder == Ascending)
            {
                benchList = benchList.OrderBy(P => P.Practice == null ? string.Empty : P.Practice.Name);
                gvBenchCosts.Attributes[PracticeSortOrder] = Descending;
            }
            else
            {
                benchList = benchList.OrderByDescending(P => P.Practice == null ? string.Empty : P.Practice.Name);
                gvBenchCosts.Attributes[PracticeSortOrder] = Ascending;
            }
            gvBenchCosts.Attributes[ConsultantNameSortOrder] = Ascending;
            gvBenchCosts.Attributes[StatusSortOrder] = Ascending;
            AddMonthColumn(3, ReportContext.Start, GetPeriodLength(), gvBenchCosts);
            gvBenchCosts.DataSource = benchList;
            gvBenchCosts.DataBind();

        }

        protected void btnSortInternalPractice_Click(object sender, EventArgs e)
        {
            var sortOrder = gvBenchCostsInternal.Attributes[PracticeSortOrder];
            var benchList = BenchList.ToList().FindAll(P => P.Practice.IsCompanyInternal);

            if (string.IsNullOrEmpty(sortOrder) || sortOrder == Ascending)
            {
                benchList = benchList.OrderBy(P => P.Practice == null ? string.Empty : P.Practice.Name).ToList();
                gvBenchCostsInternal.Attributes[PracticeSortOrder] = Descending;
            }
            else
            {
                benchList = benchList.OrderByDescending(P => P.Practice == null ? string.Empty : P.Practice.Name).ToList();
                gvBenchCostsInternal.Attributes[PracticeSortOrder] = Ascending;
            }
            gvBenchCostsInternal.Attributes[ConsultantNameSortOrder] = Ascending;
            gvBenchCostsInternal.Attributes[StatusSortOrder] = Ascending;
            AddMonthColumn(3, ReportContext.Start, GetPeriodLength(), gvBenchCostsInternal);
            gvBenchCostsInternal.DataSource = benchList;
            gvBenchCostsInternal.DataBind();
            gvBenchCostsInternal.Focus();
        }

        protected void btnSortStatus_Click(object sender, EventArgs e)
        {

            var sortOrder = gvBenchCosts.Attributes[StatusSortOrder];
            IEnumerable<Project> benchList = null;
            if (chbSeperateInternalExternal.Checked)
            {
                benchList = BenchList.ToList().FindAll(P => !P.Practice.IsCompanyInternal);
            }
            else
            {
                benchList = BenchList;
            }

            if (string.IsNullOrEmpty(sortOrder) || sortOrder == Ascending)
            {
                benchList = benchList.OrderBy(P => P.ProjectNumber);
                gvBenchCosts.Attributes[StatusSortOrder] = Descending;
            }
            else
            {

                benchList = benchList.OrderByDescending(P => P.ProjectNumber);
                gvBenchCosts.Attributes[StatusSortOrder] = Ascending;
            }
            gvBenchCosts.Attributes[ConsultantNameSortOrder] = Ascending;
            gvBenchCosts.Attributes[PracticeSortOrder] = Ascending;
            AddMonthColumn(3, ReportContext.Start, GetPeriodLength(), gvBenchCosts);
            gvBenchCosts.DataSource = benchList;
            gvBenchCosts.DataBind();
        }

        protected void btnSortInternalStatus_Click(object sender, EventArgs e)
        {

            var sortOrder = gvBenchCostsInternal.Attributes[StatusSortOrder];

            var benchList = BenchList.ToList().FindAll(P => P.Practice.IsCompanyInternal);


            if (string.IsNullOrEmpty(sortOrder) || sortOrder == Ascending)
            {
                benchList = benchList.OrderBy(P => P.ProjectNumber).ToList();
                gvBenchCostsInternal.Attributes[StatusSortOrder] = Descending;
            }
            else
            {

                benchList = benchList.OrderByDescending(P => P.ProjectNumber).ToList();
                gvBenchCostsInternal.Attributes[StatusSortOrder] = Ascending;
            }
            gvBenchCostsInternal.Attributes[ConsultantNameSortOrder] = Ascending;
            gvBenchCostsInternal.Attributes[PracticeSortOrder] = Ascending;
            AddMonthColumn(3, ReportContext.Start, GetPeriodLength(), gvBenchCostsInternal);
            gvBenchCostsInternal.DataSource = benchList;
            gvBenchCostsInternal.DataBind();
            gvBenchCostsInternal.Focus();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                DataHelper.FillPracticeList(this.cblPractices, Resources.Controls.AllPracticesText);
                DataHelper.FillTimescaleList(this.cblPayType, Resources.Controls.AllTypes);

                SelectAllItems(this.cblPayType);
                SelectAllItems(this.cblPractices);
                //DatabindGrid();
                lblExternalPractices.Visible = false;
                hrDirectorAndPracticeSeperator.Visible = false;
                lblInternalPractices.Visible = false;
            }
            if (hdnFiltersChanged.Value == "false")
            {
                btnResetFilter.Attributes.Add("disabled", "true");
            }
            else
            {
                btnResetFilter.Attributes.Remove("disabled");
            }
            AddAttributesToCheckBoxes(this.cblPayType);
            AddAttributesToCheckBoxes(this.cblPractices);
        }

        private void DatabindGrid()
        {
            Page.Validate();
            if (Page.IsValid)
            {
                var benchList = BenchList;
                AddMonthColumn(3, new DateTime(ReportContext.Start.Year, ReportContext.Start.Month, Constants.Dates.FirstDay), GetPeriodLength(), gvBenchCosts);

                if (chbSeperateInternalExternal.Checked)
                {
                    AddMonthColumn(3, new DateTime(ReportContext.Start.Year, ReportContext.Start.Month, Constants.Dates.FirstDay), GetPeriodLength(), gvBenchCostsInternal);
                    gvBenchCosts.DataSource = benchList.ToList().FindAll(Q => !Q.Practice.IsCompanyInternal).OrderBy(P => P.Name);
                    gvBenchCostsInternal.DataSource = benchList.ToList().FindAll(Q => Q.Practice.IsCompanyInternal).OrderBy(P => P.Name);
                    gvBenchCosts.DataBind();
                    gvBenchCostsInternal.DataBind();
                    divBenchCostsInternal.Visible = lblExternalPractices.Visible = true;
                }
                else
                {
                    gvBenchCosts.DataSource = benchList.OrderBy(P => P.Name);
                    gvBenchCosts.DataBind();
                    divBenchCostsInternal.Visible = lblExternalPractices.Visible = false;
                }
            }
        }

        protected void custPeriod_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = GetPeriodLength() > 0;
        }

        protected void custPeriodLengthLimit_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = GetPeriodLength() <= MaxPeriodLength;
            custPeriodLengthLimit.ErrorMessage = string.Format(custPeriodLengthLimit.ErrorMessage, MaxPeriodLength);
            custPeriodLengthLimit.ToolTip = custPeriodLengthLimit.ErrorMessage;
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
        /// Calculates a length of the selected period in the mounths.
        /// </summary>
        /// <returns>The number of the months within the selected period.</returns>
        private int GetPeriodLength()
        {
            int mounthsInPeriod =
                (ReportContext.End.Year - ReportContext.Start.Year) * Constants.Dates.LastMonth +
                (ReportContext.End.Month - ReportContext.Start.Month + 1);
            return mounthsInPeriod;
        }

        private IEnumerable<Project> GetBenchList()
        {
            var benchList = ReportsHelper.GetBenchListWithoutBenchTotalAndAdminCosts(ReportContext);
            return benchList.ToList<Project>().FindAll(p => (p.Practice != null
                                                            && !string.IsNullOrEmpty(p.Practice.Name)
                                                            && !string.IsNullOrEmpty(p.ProjectNumber)
                                                            )
                                                     );
        }

        protected void gvBenchRollOffDates_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var grid = sender as GridView;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var project = e.Row.DataItem as Project;
                bool rowVisible = false;
                if (project != null)
                {

                    if (project.Id.HasValue && project.Id.Value == 3834)
                    {
                    }
                    var monthBegin =
                        new DateTime(ReportContext.Start.Year,
                            ReportContext.Start.Month,
                            Constants.Dates.FirstDay);

                    var periodLength = GetPeriodLength();

                    ArrayList list = new ArrayList();

                    // Displaying the interest values (main cell data)
                    var current = DataHelper.CurrentPerson;

                    for (int i = 3, k = 0; k < periodLength; i++, k++, monthBegin = monthBegin.AddMonths(1))
                    {
                        var monthEnd =
                            new DateTime(monthBegin.Year,
                                monthBegin.Month,
                                DateTime.DaysInMonth(monthBegin.Year, monthBegin.Month));
                        grid.Columns[i].Visible = true;
                        grid.Columns[i].HeaderText =
                            Resources.Controls.TableHeaderOpenTag +
                            monthBegin.ToString("MMM, yyyy") +
                            Resources.Controls.TableHeaderCloseTag;

                        foreach (var interestValue in project.ProjectedFinancialsByMonth)
                        {
                            if (IsInMonth(interestValue.Key, monthBegin, monthEnd))
                            {
                                rowVisible = true;

                                if (e.Row.Parent.Parent == grid)
                                {
                                    /* According to Bug# 2631
                                     For the "external" table results, the person running the report should only be able to see costs for subordinates (based on seniority level), not anyone at their same level or higher.
                                     For the "internal" table results, only persons with Admin or Partner seniority levels should be able to see costs.  Everyone else should see "(Hidden)".
                                     */
                                    bool seniority = true;
                                    if (grid.ID == "gvBenchCostsInternal")
                                    {
                                        if ((UserIsAdmin || current.Seniority.Id == ((int)PersonSeniorityType.Partner)))
                                        {
                                            seniority = false;
                                        }
                                    }
                                    else
                                    {
                                        seniority = (new SeniorityAnalyzer(current)).IsOtherGreater(project.AccessLevel.Id);
                                    }
                                    if (!seniority)
                                    {
                                        e.Row.Cells[i].Text = (interestValue.Value.Timescale == TimescaleType.Salary ?
                                                                Convert.ToDecimal(interestValue.Value.GrossMargin).ToString() :
                                                                Resources.Controls.HourlyLabel);

                                        if (!MonthlyTotals.Any(kvp => kvp.Key == monthBegin))
                                        {
                                            MonthlyTotals.Add(monthBegin, 0M);
                                        }
                                        if (interestValue.Value.Timescale == TimescaleType.Salary)
                                        {
                                            MonthlyTotals[monthBegin] += Convert.ToDecimal(e.Row.Cells[i].Text);
                                        }

                                        if (interestValue.Value.Timescale == TimescaleType.Salary)
                                        {
                                            list.Add(Convert.ToDecimal(interestValue.Value.GrossMargin));
                                            if (interestValue.Value.GrossMargin.Value == 0M)
                                            {
                                                e.Row.Cells[i].Attributes.Add("style", "color:green;");
                                            }
                                        }
                                        else
                                        {
                                            list.Add(e.Row.Cells[i].Text);
                                        }
                                        string superScriptContent = string.Empty;
                                        if (project.StartDate.HasValue
                                            && project.StartDate.Value.Year == interestValue.Key.Year
                                            && project.StartDate.Value.Month == interestValue.Key.Month
                                            )
                                        {
                                            superScriptContent = "1";
                                        }
                                        if (project.EndDate.HasValue
                                            && project.EndDate.Value.Year == interestValue.Key.Year
                                            && project.EndDate.Value.Month == interestValue.Key.Month)
                                        {
                                            superScriptContent += (string.IsNullOrEmpty(superScriptContent) ? string.Empty : ",") + "2";
                                        }
                                        if (interestValue.Value.TimescaleChangeStatus > 0)
                                        {
                                            switch (interestValue.Value.TimescaleChangeStatus)
                                            {
                                                case 1: superScriptContent += (string.IsNullOrEmpty(superScriptContent) ? string.Empty : ",") + "3";
                                                    break;
                                                case 2:
                                                case 3: superScriptContent += (string.IsNullOrEmpty(superScriptContent) ? string.Empty : ",") + "4";
                                                    break;
                                                default: break;
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(superScriptContent))
                                        {
                                            if (interestValue.Value.Timescale == TimescaleType.Salary)
                                            {
                                                if (interestValue.Value.GrossMargin.Value == 0M)
                                                {
                                                    e.Row.Cells[i].Text = "<span style='color:green;'>0.00" + "<sup style='font-size:11px;'>" + superScriptContent + "</sup></span>";
                                                }
                                                else
                                                {
                                                    e.Row.Cells[i].Text = "<span style='color:red;'>" + interestValue.Value.GrossMargin.ToString() +
                                                                            "<sup style='font-size:11px;'>" + superScriptContent + "</sup></span>";
                                                }

                                            }
                                            else
                                            {
                                                e.Row.Cells[i].Text += "<sup style='font-size:11px;'>" + superScriptContent + "</sup>";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        e.Row.Cells[i].Text = string.Format(ReportFormat, e.Row.Cells[i].Text, Resources.Controls.HiddenAmount);

                                        list.Add(e.Row.Cells[i].Text);
                                    }

                                    e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;

                                    if (list.OfType<Decimal>().ToList().Count <= 0)
                                    {
                                        e.Row.Cells[grid.Columns.Count - 1].Text = string.Empty;
                                    }
                                    else
                                    {
                                        e.Row.Cells[grid.Columns.Count - 1].Text = list.OfType<Decimal>().ToList().Sum().ToString();
                                    }

                                    e.Row.Cells[grid.Columns.Count - 1].HorizontalAlign = HorizontalAlign.Right;
                                }
                            }
                        }

                    }
                }

                for (int i = 3; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].CssClass = CompPerfDataCssClass;
                }

                e.Row.Visible = rowVisible;
            }
        }

        protected void btnUpdateView_Click(object sender, EventArgs e)
        {
            ReportContext = null;
            BenchList = null;
            divBenchCostsInternal.Visible = gvBenchCosts.Visible = true;
            DatabindGrid();
            if (chbSeperateInternalExternal.Checked)
            {
                lblExternalPractices.Visible = true;
                hrDirectorAndPracticeSeperator.Visible = true;
                lblInternalPractices.Visible = true;
            }

            gvBenchCosts.Attributes[ConsultantNameSortOrder] = Descending;
            gvBenchCosts.Attributes[PracticeSortOrder] = Ascending;
            gvBenchCosts.Attributes[StatusSortOrder] = Ascending;
        }

        private static string GetPersonDetailsUrl(object args)
        {
            return string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                 Constants.ApplicationPages.PersonDetail,
                                 args);
        }

        protected string GetPersonDetailsUrlWithReturn(object id)
        {
            return Utils.Generic.GetTargetUrlWithReturn(GetPersonDetailsUrl(id), Request.Url.AbsoluteUri);
        }

        private void AddMonthColumn(int numberOfFixedColumns, DateTime periodStart, int monthsInPeriod, GridView grid)
        {
            // Remove columns from previous report if there was one
            for (var i = numberOfFixedColumns; i < gvBenchCosts.Columns.Count; i++)
                grid.Columns[i].Visible = false;

            // Add columns for new months);
            for (int i = numberOfFixedColumns, k = 0; k < monthsInPeriod; i++, k++)
            {
                grid.Columns[i].HeaderText =
                    Resources.Controls.TableHeaderOpenTag +
                    periodStart.ToString("MMM, yyyy") +
                    Resources.Controls.TableHeaderCloseTag;
                periodStart = periodStart.AddMonths(1);
            }
            //Add column for Grand Total
            grid.Columns[gvBenchCosts.Columns.Count - 1].HeaderText = "Grand Total";
            grid.Columns[gvBenchCosts.Columns.Count - 1].Visible = true;
        }

        private void SelectAllItems(ScrollingDropDown ddlpractices)
        {
            foreach (ListItem item in ddlpractices.Items)
            {
                item.Selected = true;
            }
        }

        protected void btnResetFilter_Click(object sender, EventArgs e)
        {
            SelectAllItems(cblPractices);
            SelectAllItems(cblPayType);
            mpStartDate.SelectedYear = DateTime.Today.Year;
            mpStartDate.SelectedMonth = DateTime.Today.Month;
            mpEndDate.SelectedYear = DateTime.Today.Year;
            mpEndDate.SelectedMonth = DateTime.Today.Month;
            cbActivePersons.Checked = true;
            cbProjectedPersons.Checked = true;
            chbActiveProjects.Checked = chbProjectedProjects.Checked = chbProjectedProjects.Checked = true;
            chbExperimentalProjects.Checked = false;
            chbIncludeZeroCostEmps.Checked = false;
            chbIncludeOverHeads.Checked = true;
            chbSeperateInternalExternal.Checked = true;
            ReportContext = null;
            BenchList = null;

            //DatabindGrid();
            //gvBenchCosts.Attributes[ConsultantNameSortOrder] = Descending;
            //gvBenchCosts.Attributes[PracticeSortOrder] = Ascending;
            //gvBenchCosts.Attributes[StatusSortOrder] = Ascending;
            divBenchCostsInternal.Visible = lblExternalPractices.Visible = gvBenchCosts.Visible = false;
            hdnFiltersChanged.Value = "false";
            btnResetFilter.Attributes.Add("disabled", "true");
        }

        protected void gvBench_OnDataBound(object sender, EventArgs e)
        {
            //BenchList
            GridView grid = (GridView)sender;
            GridViewRow footer = grid.FooterRow;
            var monthBegin =
                        new DateTime(ReportContext.Start.Year,
                            ReportContext.Start.Month,
                            Constants.Dates.FirstDay);
            var perodLenth = GetPeriodLength();
            if (footer != null)
            {
                footer.Cells[1].Text = "Grand Total :";
                for (int i = 3; i < grid.Columns.Count; i++)
                {
                    //Decimal total = 0;
                    foreach (GridViewRow row in grid.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            Decimal value;
                            bool isDecimal = Decimal.TryParse(row.Cells[i].Text, out value);

                            if (isDecimal)
                            {
                                //total += value;

                                row.Cells[i].Text = ((PracticeManagementCurrency)Convert.ToDecimal(row.Cells[i].Text)).ToString();
                            }
                            row.Cells[i].Style.Add("padding-right", "10px");
                        }
                    }
                    if (i < (3 + perodLenth))
                    {
                        if (MonthlyTotals.Any(kvp => kvp.Key == monthBegin.AddMonths(i - 3)))
                            footer.Cells[i].Text = ((PracticeManagementCurrency)MonthlyTotals[monthBegin.AddMonths(i - 3)]).ToString();
                    }
                    else if (i == grid.Columns.Count - 1)
                    {

                        footer.Cells[i].Text = ((PracticeManagementCurrency)MonthlyTotals.Values.Sum()).ToString();
                    }
                    footer.Cells[i].HorizontalAlign = HorizontalAlign.Right;
                    footer.Cells[i].Style.Add("padding-right", "10px");
                }
                MonthlyTotals = null;
            }
        }

        private void AddAttributesToCheckBoxes(ScrollingDropDown ddlpractices)
        {
            foreach (ListItem item in ddlpractices.Items)
            {
                item.Attributes.Add("onclick", "EnableResetButton();");
            }
        }

    }
}

