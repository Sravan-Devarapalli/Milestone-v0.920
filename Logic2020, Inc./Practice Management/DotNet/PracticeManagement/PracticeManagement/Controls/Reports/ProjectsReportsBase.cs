using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;

namespace PraticeManagement.Controls.Reports
{
    public class ProjectsReportsBase : UserControl
    {
        protected const int MaxPeriodLength = 24;
        protected const string CompPerfDataCssClass = "CompPerfData";
        protected const int SummaryInfoProjectIndex = 0;
        private const int BenchProjectIndex = 1;
        private const int AdminProjectIndex = 2;
        protected const int AvgRatesProjectIndex = 3;
        private const int ExpensesProjectIndex = 4;

        /// <summary>
        /// Computes and displays total values;
        /// </summary>
        protected static Project[] GetFinancialSummary(
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

        protected static void AddMonthColumn(GridView gridView, int numberOfFixedColumns, DateTime periodStart, int monthsInPeriod)
        {
            while (gridView.Columns.Count > numberOfFixedColumns + 1)
            {
                gridView.Columns.RemoveAt(numberOfFixedColumns);
            }

            for (int i = numberOfFixedColumns, k = 0; k < monthsInPeriod; i++, k++)
            {
                var newColumn = new BoundField
                                    {
                                        HeaderText = 
                                            Resources.Controls.TableHeaderOpenTag +
                                            periodStart.ToString(Constants.Formatting.CompPerfMonthYearFormat) + 
                                            Resources.Controls.TableHeaderCloseTag,
                                        HtmlEncode = false
                                    };
                newColumn.HeaderStyle.Wrap = false;
                newColumn.HeaderStyle.CssClass = newColumn.ItemStyle.CssClass = "CompPerfMonthSummary";
                gridView.Columns.Insert(i, newColumn);

                periodStart = periodStart.AddMonths(1);
            }
        }

        protected static bool IsInMonth(DateTime date, DateTime periodStart, DateTime periodEnd)
        {
            var result =
                (date.Year > periodStart.Year ||
                 (date.Year == periodStart.Year && date.Month >= periodStart.Month)) &&
                (date.Year < periodEnd.Year || (date.Year == periodEnd.Year && date.Month <= periodEnd.Month));

            return result;
        }
    }
}
