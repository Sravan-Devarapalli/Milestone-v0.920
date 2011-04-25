using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
using System.Web.UI.HtmlControls;
namespace PraticeManagement
{
    public partial class RevenueGoals : System.Web.UI.Page
    {
        #region Constants

        public const string SmallDisplayFormat = "##0.00";
        private const string ViewStateDirectorSortOrder = "DirectorSortOrder";
        private const string ViewStateDirectorSortDirection = "DirectorSortDirection";
        private const string ViewStateBDMSortOrder = "BDMSortOrder";
        private const string ViewStateBDMSortDirection = "BDMSortDirection";
        private const string ViewStatePracticeAreaSortOrder = "PracticeAreaSortOrder";
        private const string ViewStatePracticeAreaSortDirection = "PracticeAreaSortDirection";
        public const string CssArrowClass = "arrow";
        private const string grdDirectorBudgetId = "grdDirectorBudget";
        private const string grdBDMBudgetId = "grdBDManagers";
        private const string grdPraticeAreaBugetId = "grdPracticeAreas";
        private const string viewStateGroupedBDManagers = "GroupedBDManagers";
        private const string viewStateGroupedPracticeAreas = "GroupedPracticeAreas";
        private const string viewStateGroupedDirectors = "GroupedDirectors";
        private const int NumberOfFixedColumns = 1;
        private const int PersonNameCellIndex = 0;
        private const string PersonNameCellInnerHtmlTemplate = "<DIV style='WIDTH: 170px; PADDING-RIGHT: 2px'>{0}</DIV>";
        private const string MonthCellInnerHtmlTemplate = "<TABLE {2} width='100%'> <TR><TD align='{3}'>{0}</TD></TR><TR><TD align='{3}'>{1}</TD></TR></TABLE>";

        private const string STRDirectorReportSortExpression = "DirectorReportSortExpression";
        private const string STRDirectorReportSortDirection = "DirectorReportSortDirection";
        private const string STRDirectorReportSortColumnId = "DirectorReportSortColumnId";

        private const string STRBDMReportSortExpression = "BDMReportSortExpression";
        private const string STRBDMReportSortDirection = "BDMReportSortDirection";
        private const string STRBDMReportSortColumnId = "BDMReportSortColumnId";

        private const string STRPMReportSortExpression = "PMReportSortExpression";
        private const string STRPMReportSortDirection = "PMReportSortDirection";
        private const string STRPMReportSortColumnId = "PMReportSortColumnId";

        private const string AllBDMsText = "All Business Development Managers";
        #endregion

        #region Properties

        DateTime StartDate
        {
            get
            {
                return new DateTime(mpFromControl.SelectedYear, mpFromControl.SelectedMonth, 1);
            }
        }

        DateTime EndDate
        {
            get
            {
                return new DateTime(mpToControl.SelectedYear, mpToControl.SelectedMonth, DateTime.DaysInMonth(mpToControl.SelectedYear, mpToControl.SelectedMonth));
            }
        }
        public ProjectsGroupedByPerson[] GroupedDirectors
        {
            get
            {
                var value = ViewState[viewStateGroupedDirectors];

                if (value == null)
                {
                    using (var serviceClient = new ProjectService.ProjectServiceClient())
                    {
                        value = serviceClient.CalculateBudgetForPersons(
                            StartDate,
                            EndDate,
                            chbProjected.Checked,
                            chbCompleted.Checked,
                            chbActive.Checked,
                            chbInternal.Checked,
                            chbExperimental.Checked,
                            chbInactive.Checked,
                            SelectedPracticeIds,
                            chbExcludeInternalPractices.Checked,
                            string.Empty,
                            BudgetCategoryType.ClientDirector);
                    }
                    ViewState[viewStateGroupedDirectors] = value;
                }
                return (ProjectsGroupedByPerson[])value;
            }
            set
            {
                ViewState[viewStateGroupedDirectors] = value;
            }
        }

        public ProjectsGroupedByPerson[] GroupedBDManagers
        {
            get
            {
                var value = ViewState[viewStateGroupedBDManagers];

                if (value == null)
                {
                    using (var serviceClient = new ProjectService.ProjectServiceClient())
                    {
                        value = serviceClient.CalculateBudgetForPersons(
                                                    StartDate,
                                                    EndDate,
                                                    chbProjected.Checked,
                                                    chbCompleted.Checked,
                                                    chbActive.Checked,
                                                    chbInternal.Checked,
                                                    chbExperimental.Checked,
                                                    chbInactive.Checked,
                                                    SelectedPracticeIds,
                                                    chbExcludeInternalPractices.Checked,
                                                    SelectedBDMIds,
                                                    BudgetCategoryType.BusinessDevelopmentManager);
                    }
                    ViewState[viewStateGroupedBDManagers] = value;
                }
                return (ProjectsGroupedByPerson[])value;
            }
            set
            {
                ViewState[viewStateGroupedBDManagers] = value;
            }
        }

        public ProjectsGroupedByPractice[] GroupedPracticeAreas
        {
            get
            {
                var value = ViewState[viewStateGroupedPracticeAreas];

                if (value == null)
                {
                    using (var serviceClient = new ProjectService.ProjectServiceClient())
                    {
                        value = serviceClient.CalculateBudgetForPractices(
                                                                StartDate,
                                                                EndDate,
                                                                chbProjected.Checked,
                                                                chbCompleted.Checked,
                                                                chbActive.Checked,
                                                                chbInternal.Checked,
                                                                chbExperimental.Checked,
                                                                chbInactive.Checked,
                                                                SelectedPracticeIds,
                                                                chbExcludeInternalPractices.Checked
                            );
                    }
                    ViewState[viewStateGroupedPracticeAreas] = value;
                }
                return (ProjectsGroupedByPractice[])value;
            }
            set
            {
                ViewState[viewStateGroupedPracticeAreas] = value;
            }
        }


        private string PrevDirectorReportSortExpression
        {
            get { return ViewState[STRDirectorReportSortExpression] as string ?? string.Empty; }
            set { ViewState[STRDirectorReportSortExpression] = value; }
        }

        private string DirectorReportSortDirection
        {
            get { return ViewState[STRDirectorReportSortDirection] as string ?? "Ascending"; }
            set { ViewState[STRDirectorReportSortDirection] = value; }
        }

        private int DirectorReportSortColumnId
        {
            get { return ViewState[STRDirectorReportSortColumnId] != null ? (int)ViewState[STRDirectorReportSortColumnId] : 0; }
            set { ViewState[STRDirectorReportSortColumnId] = value; }
        }

        private string PrevAccountManagerSortExpression
        {
            get { return ViewState[STRBDMReportSortExpression] as string ?? string.Empty; }
            set { ViewState[STRBDMReportSortExpression] = value; }
        }

        private string AccountManagerSortDirection
        {
            get { return ViewState[STRBDMReportSortDirection] as string ?? "Ascending"; }
            set { ViewState[STRBDMReportSortDirection] = value; }
        }

        private int BDMReportSortColumnId
        {
            get { return ViewState[STRBDMReportSortColumnId] != null ? (int)ViewState[STRBDMReportSortColumnId] : 0; }
            set { ViewState[STRBDMReportSortColumnId] = value; }
        }

        private string PrevPMReportSortExpression
        {
            get { return ViewState[STRPMReportSortExpression] as string ?? string.Empty; }
            set { ViewState[STRPMReportSortExpression] = value; }
        }

        private string PMReportSortDirection
        {
            get { return ViewState[STRPMReportSortDirection] as string ?? "Ascending"; }
            set { ViewState[STRPMReportSortDirection] = value; }
        }

        private int PMReportSortColumnId
        {
            get { return ViewState[STRPMReportSortColumnId] != null ? (int)ViewState[STRPMReportSortColumnId] : 0; }
            set { ViewState[STRPMReportSortColumnId] = value; }
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

        private string SelectedBDMIds
        {
            get
            {
                return cblBDMs.SelectedItems;
            }
            set
            {
                cblBDMs.SelectedItems = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                var person = DataHelper.CurrentPerson;
                if (person == null || person.Seniority.Id > 35)
                {
                    Response.Redirect(@"~\GuestPages\AccessDenied.aspx");
                }

                int selectedYear = DateTime.Now.Year;
                DataHelper.FillPracticeList(this.cblPractice, Resources.Controls.AllPracticesText);

                SelectedPracticeIds = null;
                InitDateRange();
                BindBDManagers();
            }
            if (hdnFiltersChanged.Value == "false")
            {
                btnResetFilter.Attributes.Add("disabled", "true");
            }
            else
            {
                btnResetFilter.Attributes.Remove("disabled");
            }
            AddAttributesToCheckBoxes(this.cblPractice);
            AddAttributesToCheckBoxes(this.cblBDMs);

        }

        public string GetMonthAmountDiff(Dictionary<DateTime, ComputedFinancials> projectedFinancialsDict, DateTime monthStartDate,
            ref PracticeManagementCurrency goal)
        {

            if (projectedFinancialsDict != null && projectedFinancialsDict.Any(kvp => kvp.Key == monthStartDate))
            {
                var financials = projectedFinancialsDict.First(kvp => kvp.Key == monthStartDate).Value;
                if (financials.Revenue != 0)
                {
                    var res = (financials.RevenueNet - financials.Revenue).ToString(); // Entered - Actual
                    goal.Value = financials.Revenue;

                    if ((financials.RevenueNet - financials.Revenue) > 0)
                    {
                        res = "<SPAN class='Revenue'>" + res + "</SPAN>";
                    }
                    return res;
                }

                return string.Empty;
            }
            return "---";
        }

        public string GetMonthPrcntgDiff(Dictionary<DateTime, ComputedFinancials> projectedFinancialsDict, DateTime monthStartDate)
        {
            if (projectedFinancialsDict != null && projectedFinancialsDict.Any(kvp => kvp.Key == monthStartDate))
            {
                var financials = projectedFinancialsDict.First(kvp => kvp.Key == monthStartDate).Value;
                if (financials.Revenue != 0)
                {
                    return ((financials.RevenueNet - financials.Revenue) * 100 / financials.Revenue).Value.ToString(SmallDisplayFormat) + "%";// Entered - Actual
                }
                return string.Empty;
            }
            return "---";
        }

        public string GetPersonGrandTotalAmountDiff(Dictionary<DateTime, ComputedFinancials> projectedFinancialsDict,
            ref PracticeManagementCurrency goal)
        {
            if (projectedFinancialsDict.Count > 0 && projectedFinancialsDict.Values.Any(f => f.Revenue != 0))
            {
                PracticeManagementCurrency actualRevTotal = 0, enteredRevTotal = 0;

                foreach (var item in projectedFinancialsDict.Values)
                {
                    if (item.Revenue != 0)
                    {
                        actualRevTotal += item.RevenueNet;
                        enteredRevTotal += item.Revenue;
                    }
                }
                goal.Value = enteredRevTotal;
                var res = ((actualRevTotal - enteredRevTotal)).ToString();
                if ((actualRevTotal - enteredRevTotal) > 0)
                {
                    res = "<SPAN class='Revenue'>" + res + "</SPAN>";
                }

                return res;
            }
            return string.Empty;
        }

        public string GetMonthGrandTotalAmountDiff(int CType, DateTime monthBegin, ref PracticeManagementCurrency goal)
        {
            var monthFinancials = new List<ComputedFinancials>();
            var categoryType = (BudgetCategoryType)CType;
            var groupedPersons = new List<ProjectsGroupedByPerson>();
            var groupedPractices = new List<ProjectsGroupedByPractice>();
            if (categoryType == BudgetCategoryType.ClientDirector)
            {
                groupedPersons = GroupedDirectors.ToList();
            }
            else if (categoryType == BudgetCategoryType.BusinessDevelopmentManager)
            {
                groupedPersons = GroupedBDManagers.ToList();
            }
            else
            {
                groupedPractices = GroupedPracticeAreas.ToList();
            }
            if (categoryType != BudgetCategoryType.PracticeArea)
            {
                foreach (var person in groupedPersons)
                {
                    if (person.ProjectedFinancialsByMonth != null &&
                                person.ProjectedFinancialsByMonth.Any(f => f.Key == monthBegin))
                    {
                        monthFinancials.Add(person.ProjectedFinancialsByMonth.First(f => f.Key == monthBegin).Value);
                    }
                }
            }
            else
            {
                foreach (var practice in groupedPractices)
                {
                    if (practice.ProjectedFinancialsByMonth != null &&
                        practice.ProjectedFinancialsByMonth.Any(f => f.Key == monthBegin))
                    {
                        monthFinancials.Add(practice.ProjectedFinancialsByMonth.First(f => f.Key == monthBegin).Value);
                    }
                }
            }

            if (monthFinancials.Any(f => f.Revenue != 0))
            {
                PracticeManagementCurrency actualRevTotal = 0, enteredRevTotal = 0;

                foreach (var item in monthFinancials)
                {
                    if (item.Revenue != 0)
                    {
                        actualRevTotal += item.RevenueNet;
                        enteredRevTotal += item.Revenue;
                    }
                }
                var res = ((actualRevTotal - enteredRevTotal)).ToString();
                goal = enteredRevTotal;
                if ((actualRevTotal - enteredRevTotal) > 0)
                {
                    res = "<SPAN class='Revenue'>" + res + "</SPAN>";
                }
                return res;
            }
            return string.Empty;
        }

        public string GetMonthGrandPrcntgAmountDiff(int CType, DateTime monthBegin)
        {
            var monthFinancials = new List<ComputedFinancials>();

            var categoryType = (BudgetCategoryType)CType;
            var groupedPersons = new List<ProjectsGroupedByPerson>();
            var groupedPractices = new List<ProjectsGroupedByPractice>();
            if (categoryType == BudgetCategoryType.ClientDirector)
            {
                groupedPersons = GroupedDirectors.ToList();
            }
            else if (categoryType == BudgetCategoryType.BusinessDevelopmentManager)
            {
                groupedPersons = GroupedBDManagers.ToList();
            }
            else
            {
                groupedPractices = GroupedPracticeAreas.ToList();
            }
            if (categoryType != BudgetCategoryType.PracticeArea)
            {
                foreach (var person in groupedPersons)
                {
                    if (person.ProjectedFinancialsByMonth != null &&
                                person.ProjectedFinancialsByMonth.Any(f => f.Key == monthBegin))
                    {
                        monthFinancials.Add(person.ProjectedFinancialsByMonth.First(f => f.Key == monthBegin).Value);
                    }
                }
            }
            else
            {
                foreach (var practice in groupedPractices)
                {
                    if (practice.ProjectedFinancialsByMonth != null &&
                        practice.ProjectedFinancialsByMonth.Any(f => f.Key == monthBegin))
                    {
                        monthFinancials.Add(practice.ProjectedFinancialsByMonth.First(f => f.Key == monthBegin).Value);
                    }
                }
            }

            if (monthFinancials.Any(f => f.Revenue != 0))
            {
                decimal actualRevTotal = 0, enteredRevTotal = 0;

                foreach (var item in monthFinancials)
                {
                    if (item.Revenue != 0)
                    {
                        actualRevTotal += item.RevenueNet;
                        enteredRevTotal += item.Revenue;
                    }
                }
                if (enteredRevTotal != 0)
                {
                    return ((actualRevTotal - enteredRevTotal) * 100 / enteredRevTotal).ToString(SmallDisplayFormat) + "%";
                }
            }
            return string.Empty;
        }

        public string GetPersonGrandTotalPrcntgDiff(Dictionary<DateTime, ComputedFinancials> projectedFinancialsDict)
        {
            if (projectedFinancialsDict.Count > 0 && projectedFinancialsDict.Values.Any(f => f.Revenue != 0))
            {
                decimal actualRevTotal = 0, enteredRevTotal = 0;

                foreach (var item in projectedFinancialsDict.Values)
                {
                    if (item.Revenue != 0)
                    {
                        actualRevTotal += item.RevenueNet;
                        enteredRevTotal += item.Revenue;
                    }
                }
                if (enteredRevTotal != 0)
                {
                    return ((actualRevTotal - enteredRevTotal) * 100 / enteredRevTotal).ToString(SmallDisplayFormat) + "%";
                }
            }
            return string.Empty;
        }

        public string GetGrandTotalAmountDiff(int CType, ref PracticeManagementCurrency goal)
        {
            var monthFinancials = new List<ComputedFinancials>();
            var groupedPersons = new List<ProjectsGroupedByPerson>();
            var groupedPractices = new List<ProjectsGroupedByPractice>();
            var categoryType = (BudgetCategoryType)CType;
            if (categoryType == BudgetCategoryType.ClientDirector)
            {
                groupedPersons = GroupedDirectors.ToList();
            }
            else if (categoryType == BudgetCategoryType.BusinessDevelopmentManager)
            {
                groupedPersons = GroupedBDManagers.ToList();
            }
            else
            {
                groupedPractices = GroupedPracticeAreas.ToList();
            }
            if (categoryType != BudgetCategoryType.PracticeArea)
            {
                foreach (var person in groupedPersons)
                {
                    if (person.ProjectedFinancialsByMonth != null)
                    {
                        monthFinancials.AddRange(person.ProjectedFinancialsByMonth.Values);
                    }
                }
            }
            else
            {
                foreach (var practice in groupedPractices)
                {
                    if (practice.ProjectedFinancialsByMonth != null)
                    {
                        monthFinancials.AddRange(practice.ProjectedFinancialsByMonth.Values);
                    }
                }
            }

            if (monthFinancials.Any(f => f.Revenue != 0))
            {
                PracticeManagementCurrency actualRevTotal = 0, enteredRevTotal = 0;

                foreach (var item in monthFinancials)
                {
                    if (item.Revenue != 0)
                    {
                        actualRevTotal += item.RevenueNet;
                        enteredRevTotal += item.Revenue;
                    }
                }
                goal = enteredRevTotal;
                var res = ((actualRevTotal - enteredRevTotal)).ToString();
                if ((actualRevTotal - enteredRevTotal) > 0)
                {
                    res = "<SPAN class='Revenue'>" + res + "</SPAN>";
                }
                return res;
            }
            return string.Empty;
        }

        public string GetGrandPrcntgAmountDiff(int CType)
        {
            var monthFinancials = new List<ComputedFinancials>();
            var groupedPersons = new List<ProjectsGroupedByPerson>();
            var groupedPractices = new List<ProjectsGroupedByPractice>();
            var categoryType = (BudgetCategoryType)CType;
            if (categoryType == BudgetCategoryType.ClientDirector)
            {
                groupedPersons = GroupedDirectors.ToList();
            }
            else if (categoryType == BudgetCategoryType.BusinessDevelopmentManager)
            {
                groupedPersons = GroupedBDManagers.ToList();
            }
            else
            {
                groupedPractices = GroupedPracticeAreas.ToList();
            }
            if (categoryType != BudgetCategoryType.PracticeArea)
            {
                foreach (var person in groupedPersons)
                {
                    if (person.ProjectedFinancialsByMonth != null)
                    {
                        monthFinancials.AddRange(person.ProjectedFinancialsByMonth.Values);
                    }
                }
            }
            else
            {
                foreach (var practice in groupedPractices)
                {
                    if (practice.ProjectedFinancialsByMonth != null)
                    {
                        monthFinancials.AddRange(practice.ProjectedFinancialsByMonth.Values);
                    }
                }
            }

            if (monthFinancials.Any(f => f.Revenue != 0))
            {
                decimal actualRevTotal = 0, enteredRevTotal = 0;

                foreach (var item in monthFinancials)
                {
                    if (item.Revenue != 0)
                    {
                        actualRevTotal += item.RevenueNet;
                        enteredRevTotal += item.Revenue;
                    }
                }
                if (enteredRevTotal != 0)
                {
                    return ((actualRevTotal - enteredRevTotal) * 100 / enteredRevTotal).ToString(SmallDisplayFormat) + "%";
                }
            }
            return string.Empty;
        }

        protected void mpFromControl_OnSelectedValueChanged(object sender, EventArgs e)
        {
            UpdateToDate();
            BindBDManagers();
        }

        protected void mpToControl_OnSelectedValueChanged(object sender, EventArgs e)
        {
            var selectedYear = mpToControl.SelectedYear;
            var selectedMonth = mpToControl.SelectedMonth;
            UpdateToDate();
            if (mpFromControl.SelectedYear == selectedYear && mpFromControl.SelectedMonth > selectedMonth)
            {
                mpToControl.SelectedYear = mpFromControl.SelectedYear;
                mpToControl.SelectedMonth = mpFromControl.SelectedMonth;
            }
            BindBDManagers();
        }

        protected void lvBudget_OnDataBinding(object sender, EventArgs e)
        {
            var budgetListview = sender as ListView;
            var periodStart = StartDate;
            var monthsInPeriod = GetPeriodLength();
            var row = budgetListview.FindControl("lvHeader") as System.Web.UI.HtmlControls.HtmlTableRow;
            AddMonthColumn(row, periodStart, monthsInPeriod, NumberOfFixedColumns);
            row.Cells[row.Cells.Count - 1].InnerHtml = " <div class='ie-bg' style='padding-right:3px;'>Grand Total</div>";
        }

        protected void lvPersonBudget_OnItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var groupedPerson = (e.Item as ListViewDataItem).DataItem as ProjectsGroupedByPerson;
                var row = e.Item.FindControl("testTr") as HtmlTableRow;

                var monthsInPeriod = GetPeriodLength();
                for (int i = 0; i < monthsInPeriod + 1; i++)   // + 1 means a cell for total column
                {
                    var td = new HtmlTableCell() { };
                    td.Attributes["class"] = "CompPerfMonthSummary";
                    row.Cells.Insert(row.Cells.Count, td);
                }
                if (groupedPerson != null)
                {
                    FillPersonCell(row, groupedPerson.LastName + ", " + groupedPerson.FirstName);
                    FillMonthCells(row, groupedPerson.ProjectedFinancialsByMonth);
                    FillTotalCell(row, groupedPerson.ProjectedFinancialsByMonth);
                }
            }
        }

        protected void lvPracticeBudget_OnItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var groupedPractice = (e.Item as ListViewDataItem).DataItem as ProjectsGroupedByPractice;
                var row = e.Item.FindControl("testTr") as HtmlTableRow;

                var monthsInPeriod = GetPeriodLength();
                for (int i = 0; i < monthsInPeriod + 1; i++)   // + 1 means a cell for total column
                {
                    var td = new HtmlTableCell() { };
                    td.Attributes["class"] = "CompPerfMonthSummary";
                    row.Cells.Insert(row.Cells.Count, td);
                }
                if (groupedPractice != null)
                {
                    FillPersonCell(row, groupedPractice.Name);
                    FillMonthCells(row, groupedPractice.ProjectedFinancialsByMonth);
                    FillTotalCell(row, groupedPractice.ProjectedFinancialsByMonth);
                }
            }
        }

        protected void lvBudget_OnPreRender(object sender, EventArgs e)
        {
            var grandTotalRowInnerHtml = "</td></tr><tr class='summary' height='35px'><td style='padding-left:3px;border-left:2px solid black;border-bottom:1px solid black;border-top:1px solid black;'>";
            var budgetListView = sender as ListView;
            int categoryType = 1;
            if (budgetListView.ID == "lvPracticeBudget")
            {
                categoryType = 2;
            }
            else if (budgetListView.ID == "lvBDMBudget")
            {
                categoryType = 3;
            }

            if (budgetListView.Items.Any())
            {
                var lastItem = budgetListView.Items.Last();
                var lastRow = lastItem.FindControl("testTr") as HtmlTableRow;
                grandTotalRowInnerHtml += "Grand Total";

                int periodLength = GetPeriodLength();
                DateTime monthBegin = StartDate;
                for (int monthIndex = NumberOfFixedColumns;
                        monthIndex < (NumberOfFixedColumns + periodLength);
                        monthIndex++, monthBegin = monthBegin.AddMonths(1))
                {
                    var goal = new PracticeManagementCurrency();
                    string amountDiff = GetMonthGrandTotalAmountDiff(categoryType, monthBegin, ref  goal);
                    string toolTipInfo = (!String.IsNullOrEmpty(amountDiff) ? " title='Goal: " + GetGoalString(goal.ToString()) + "' " : string.Empty);
                    if (monthBegin.Month == DateTime.Now.Month && monthBegin.Year == DateTime.Now.Year)
                    {
                        grandTotalRowInnerHtml += "</td><td style='border:1px solid black;'>";
                    }
                    else
                    {
                        grandTotalRowInnerHtml += "</td><td style='border-bottom:1px solid black;border-top:1px solid black;'>";
                    }
                    grandTotalRowInnerHtml += string.Format(MonthCellInnerHtmlTemplate,
                                                        amountDiff,
                                                        GetMonthGrandPrcntgAmountDiff(categoryType, monthBegin),
                                                        toolTipInfo,
                                                        "right"
                                                        );

                }

                var grandTotalgoal = new PracticeManagementCurrency();
                string grandTotalAmountDiff = GetGrandTotalAmountDiff(categoryType, ref  grandTotalgoal);
                string grandTotalToolTipInfo = (!String.IsNullOrEmpty(grandTotalAmountDiff) ? " title='Goal: " + GetGoalString(grandTotalgoal.ToString()) + "' " : string.Empty);
                grandTotalRowInnerHtml += ("</td><td style='border-right:1px solid black;border-bottom:1px solid black;border-top:1px solid black;'>" + string.Format(MonthCellInnerHtmlTemplate,
                                                   grandTotalAmountDiff,
                                                   GetGrandPrcntgAmountDiff(categoryType),
                                                   grandTotalToolTipInfo,
                                                    "right"
                                                    ));
                lastRow.Cells[lastRow.Cells.Count - 1].InnerHtml += grandTotalRowInnerHtml;
            }
        }

        protected void lvBudget_OnSorting(object sender, ListViewSortEventArgs e)
        {
            var budgetListView = sender as ListView;
            if (budgetListView.ID == "lvDirectorBudget")
            {
                if (PrevDirectorReportSortExpression != e.SortExpression)
                {
                    PrevDirectorReportSortExpression = e.SortExpression;
                    DirectorReportSortDirection = e.SortDirection.ToString();
                }
                else
                {
                    DirectorReportSortDirection = GetSortDirection(DirectorReportSortDirection);
                }
                DirectorReportSortColumnId = GetSortColumnId(e.SortExpression);

                budgetListView.DataSource = SortList(GroupedDirectors.ToList(), PrevDirectorReportSortExpression, DirectorReportSortDirection);
                budgetListView.DataBind();
            }
            else if (budgetListView.ID == "lvBDMBudget")
            {
                if (PrevAccountManagerSortExpression != e.SortExpression)
                {
                    PrevAccountManagerSortExpression = e.SortExpression;
                    AccountManagerSortDirection = e.SortDirection.ToString();
                }
                else
                {
                    AccountManagerSortDirection = GetSortDirection(AccountManagerSortDirection);
                }
                BDMReportSortColumnId = GetSortColumnId(e.SortExpression);

                budgetListView.DataSource = SortList(GroupedBDManagers.ToList(), PrevAccountManagerSortExpression, AccountManagerSortDirection);
                budgetListView.DataBind();
            }
            else if (budgetListView.ID == "lvPracticeBudget")
            {
                if (PrevPMReportSortExpression != e.SortExpression)
                {
                    PrevPMReportSortExpression = e.SortExpression;
                    PMReportSortDirection = e.SortDirection.ToString();
                }
                else
                {
                    PMReportSortDirection = GetSortDirection(PMReportSortDirection);
                }
                PMReportSortColumnId = GetSortColumnId(e.SortExpression);
                budgetListView.DataSource = SortList(GroupedPracticeAreas.ToList());
                budgetListView.DataBind();
            }
        }

        protected void lvBudget_Sorted(object sender, EventArgs e)
        {
            var lvBudget = sender as ListView;
            var row = lvBudget.FindControl("lvHeader") as System.Web.UI.HtmlControls.HtmlTableRow;
            for (int i = 0; i < row.Cells.Count; i++)
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
                            if (lvBudget.ID == "lvDirectorBudget")
                            {
                                if (i == DirectorReportSortColumnId)
                                {
                                    lb.CssClass += string.Format(" sort-{0}", DirectorReportSortDirection == "Ascending" ? "up" : "down");
                                }
                            }
                            else if (lvBudget.ID == "lvBDMBudget")
                            {
                                if (i == BDMReportSortColumnId)
                                {
                                    lb.CssClass += string.Format(" sort-{0}", AccountManagerSortDirection == "Ascending" ? "up" : "down");
                                }
                            }
                            else if (lvBudget.ID == "lvPracticeBudget")
                            {
                                if (i == PMReportSortColumnId)
                                {
                                    lb.CssClass += string.Format(" sort-{0}", PMReportSortDirection == "Ascending" ? "up" : "down");
                                }
                            }
                        }
                    }
                }
            }
        }

        private object SortList(List<ProjectsGroupedByPractice> GroupedPractices)
        {
            List<ProjectsGroupedByPractice> sortedpractices = GroupedPractices;
            if (GroupedPractices != null && GroupedPractices.Count > 0)
            {
                if (!string.IsNullOrEmpty(PrevPMReportSortExpression) && PMReportSortDirection != "Ascending")
                {
                    sortedpractices = GroupedPractices.OrderByDescending(practice => practice.Name).ToList();
                }
                else
                {
                    sortedpractices = GroupedPractices.OrderBy(practice => practice.Name).ToList();
                }
            }
            return sortedpractices;
        }

        private List<ProjectsGroupedByPerson> SortList(List<ProjectsGroupedByPerson> groupedDirectors, string prevSortDirection, string sortDirection)
        {
            if (groupedDirectors != null && groupedDirectors.Count > 0)
            {
                if (!string.IsNullOrEmpty(prevSortDirection))
                {
                    if (sortDirection != "Ascending")
                    {
                        groupedDirectors = groupedDirectors.OrderByDescending(person => person.LastName + " , " + person.FirstName).ToList();
                    }
                    else
                    {
                        groupedDirectors = groupedDirectors.OrderBy(person => person.LastName + " , " + person.FirstName).ToList();
                    }
                }
            }
            return groupedDirectors;
        }

        private int GetSortColumnId(string sortExpression)
        {
            int sortColumn = -1;
            return int.TryParse(sortExpression, out sortColumn) ? sortColumn : 0;
        }

        private string GetSortDirection(string sortDirection)
        {
            switch (sortDirection)
            {
                case "Ascending":
                    sortDirection = "Descending";
                    break;
                case "Descending":
                    sortDirection = "Ascending";
                    break;
            }
            return sortDirection;
        }

        private void FillPersonCell(HtmlTableRow row, string PersonName)
        {
            row.Cells[PersonNameCellIndex].InnerHtml = string.Format(PersonNameCellInnerHtmlTemplate, PersonName);
        }

        private void FillMonthCells(HtmlTableRow row, Dictionary<DateTime, ComputedFinancials> projectedFinancialsByMonth)
        {
            int periodLength = GetPeriodLength();
            DateTime monthBegin = StartDate;
            for (int monthIndex = NumberOfFixedColumns;
                        monthIndex < (NumberOfFixedColumns + periodLength);
                        monthIndex++, monthBegin = monthBegin.AddMonths(1))
            {
                var goal = new PracticeManagementCurrency();
                string amountDiff = GetMonthAmountDiff(projectedFinancialsByMonth, monthBegin, ref  goal);
                string toolTipInfo = (!String.IsNullOrEmpty(amountDiff) && amountDiff != "---" ? " title='Goal: " + GetGoalString(goal.ToString()) + "' " : string.Empty);
                row.Cells[monthIndex].InnerHtml = string.Format(MonthCellInnerHtmlTemplate,
                                                    amountDiff,
                                                    GetMonthPrcntgDiff(projectedFinancialsByMonth, monthBegin),
                                                    toolTipInfo,
                                                    amountDiff == "---" ? "center" : "right");

                if (monthBegin.Month == DateTime.Now.Month && monthBegin.Year == DateTime.Now.Year)
                {
                    row.Cells[monthIndex].Attributes["style"] = "border-left:1px solid black;border-right:1px solid black;";
                }
            }
        }

        private string GetGoalString(string goal)
        {
            var lowerGoal = goal.ToLower();
            if (lowerGoal.Contains("</span>"))
            {
                return lowerGoal.Substring(lowerGoal.IndexOf('>'), lowerGoal.IndexOf("</span>") - lowerGoal.IndexOf('>'));
            }
            else
            {
                return goal;
            }

        }

        private void FillTotalCell(HtmlTableRow row, Dictionary<DateTime, ComputedFinancials> projectedFinancialsByMonth)
        {
            var goal = new PracticeManagementCurrency();
            string amountDiff = GetPersonGrandTotalAmountDiff(projectedFinancialsByMonth, ref  goal);
            string toolTipInfo = (!String.IsNullOrEmpty(amountDiff) ? " title='Goal: " + GetGoalString(goal.ToString()) + "' " : string.Empty);
            row.Cells[row.Cells.Count - 1].InnerHtml = string.Format(MonthCellInnerHtmlTemplate,
                                                            amountDiff,
                                                            GetPersonGrandTotalPrcntgDiff(projectedFinancialsByMonth),
                                                            toolTipInfo,
                                                            "right"
                                                            );
        }

        private int GetPeriodLength()
        {
            int mounthsInPeriod =
            (mpToControl.SelectedYear - mpFromControl.SelectedYear) * Constants.Dates.LastMonth +
            (mpToControl.SelectedMonth - mpFromControl.SelectedMonth + 1);

            return mounthsInPeriod;
        }

        private void AddMonthColumn(HtmlTableRow row, DateTime periodStart, int monthsInPeriod, int insertPosition)
        {
            if (row != null)
            {
                while (row.Cells.Count > NumberOfFixedColumns + 1)
                {
                    row.Cells.RemoveAt(NumberOfFixedColumns);
                }

                row.Cells[row.Cells.Count - 1].Attributes["Style"] = "";

                for (int i = insertPosition, k = 0; k < monthsInPeriod; i++, k++)
                {
                    var newColumn = new HtmlTableCell("td");
                    row.Cells.Insert(i, newColumn);

                    row.Cells[i].InnerHtml = "<div class='ie-bg no-wrap'>" + periodStart.ToString(Constants.Formatting.MonthYearFormat) + "</div>";
                    row.Cells[i].Attributes["class"] = "ie-bg";
                    if (periodStart.Month == DateTime.Now.Month && periodStart.Year == DateTime.Now.Year)
                    {
                        row.Cells[i].Attributes["Style"] = "border-left:1px solid black;border-right:1px solid black;border-bottom:1px solid black;";
                    }
                    row.Cells[i].Attributes["align"] = "center";
                    periodStart = periodStart.AddMonths(1);
                }
            }
        }

        protected void btnUpdate_OnClick(object sender, EventArgs e)
        {
            if (hdnFiltersChangedSinceLastUpdate.Value == "true")
            {
                GroupedDirectors = null;
                GroupedBDManagers = null;
                GroupedPracticeAreas = null;
                hdnFiltersChangedSinceLastUpdate.Value = "false";
            }
            lblDirectorEmptyMessage.Visible = false;
            lblPracticeEmptyMessage.Visible = false;
            lblBDMEmptyMessage.Visible = false;
            if (ddlGoalsFor.SelectedIndex == 0)
            {
                hrDirectorAndPracticeSeperator.Visible = hrPracticeAndACMgrSeperator.Visible = true;
            }
            else
            {
                hrDirectorAndPracticeSeperator.Visible = hrPracticeAndACMgrSeperator.Visible = false;
            }

            if (ddlGoalsFor.SelectedIndex == 0 || ddlGoalsFor.SelectedIndex == 1)
            {
                if (GroupedDirectors.Any())
                {
                    lvDirectorBudget.DataSource = GroupedDirectors;
                    lvDirectorBudget.DataBind();
                    lvDirectorBudget.Visible = true;
                    lblDirectorEmptyMessage.Visible = false;
                }
                else
                {
                    lvDirectorBudget.Visible = false;
                    lblDirectorEmptyMessage.Visible = true;
                }
            }
            else
            {
                lvDirectorBudget.Visible = false;
            }

            if (ddlGoalsFor.SelectedIndex == 0 || ddlGoalsFor.SelectedIndex == 2)
            {
                if (GroupedPracticeAreas.Any())
                {
                    lvPracticeBudget.DataSource = GroupedPracticeAreas;
                    lvPracticeBudget.DataBind();
                    lvPracticeBudget.Visible = true;
                    lblPracticeEmptyMessage.Visible = false;
                }
                else
                {
                    lvPracticeBudget.Visible = false;
                    lblPracticeEmptyMessage.Visible = true;
                }
            }
            else
            {
                lvPracticeBudget.Visible = false;
            }

            if (ddlGoalsFor.SelectedIndex == 0 || ddlGoalsFor.SelectedIndex == 3)
            {
                if (GroupedBDManagers.Any())
                {
                    lvBDMBudget.DataSource = GroupedBDManagers;
                    lvBDMBudget.DataBind();
                    lvBDMBudget.Visible = true;
                    lblBDMEmptyMessage.Visible = false;
                }
                else
                {
                    lvBDMBudget.Visible = false;
                    lblBDMEmptyMessage.Visible = true;
                }
            }
            else
            {

                lvBDMBudget.Visible = false;

            }

            if (hdnFiltersChanged.Value == "false")
            {
                btnResetFilter.Attributes.Add("disabled", "true");
            }
            else
            {
                btnResetFilter.Attributes.Remove("disabled");
            }

            upDirectorRevenueGoals.Update();
            upPracticeRevenueGoals.Update();
            upBDMRevenueGoals.Update();
        }

        private void AddAttributesToCheckBoxes(ScrollingDropDown ddlpractices)
        {
            foreach (ListItem item in ddlpractices.Items)
            {
                item.Attributes.Add("onclick", "EnableResetButton();");
            }
        }

        private void UpdateToDate()
        {
            DropDownList monthToControl = mpToControl.FindControl("ddlMonth") as DropDownList;
            DropDownList yearToControl = mpToControl.FindControl("ddlYear") as DropDownList;

            //remove all the year items less than mpFromControl.SelectedYear in mpToControl.
            RemoveToControls(mpFromControl.SelectedYear, yearToControl);

            if (mpFromControl.SelectedYear >= mpToControl.SelectedYear)
            {
                //remove all the month items less than mpFromControl.SelectedMonth in mpToControl.
                RemoveToControls(mpFromControl.SelectedMonth, monthToControl);

                if (mpFromControl.SelectedYear > mpToControl.SelectedYear ||
                    mpFromControl.SelectedMonth > mpToControl.SelectedMonth)
                {
                    mpToControl.SelectedYear = mpFromControl.SelectedYear;
                    mpToControl.SelectedMonth = mpFromControl.SelectedMonth;
                }
            }
            else
            {
                RemoveToControls(0, monthToControl);
            }
        }

        private void InitDateRange()
        {
            var FromSelectedmonth = DateTime.Now.Month - 1;
            var ToSelectedmonth = DateTime.Now.Month + 1;
            mpFromControl.SelectedYear = FromSelectedmonth < 1 ? DateTime.Now.Year - 1 : DateTime.Now.Year;
            mpFromControl.SelectedMonth = FromSelectedmonth < 1 ? 12 + FromSelectedmonth : FromSelectedmonth;

            mpToControl.SelectedYear = ToSelectedmonth > 12 ? DateTime.Now.Year + 1 : DateTime.Now.Year;
            mpToControl.SelectedMonth = ToSelectedmonth > 12 ? ToSelectedmonth - 12 : ToSelectedmonth;
            UpdateToDate();
        }

        private void BindBDManagers()
        {
            DataHelper.FillBusinessDevelopmentManagersList(cblBDMs, AllBDMsText, StartDate, EndDate);
            SelectedBDMIds = null;
            AddAttributesToCheckBoxes(this.cblBDMs);
        }


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
    }
}
