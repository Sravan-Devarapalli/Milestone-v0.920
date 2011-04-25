using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using DataTransferObjects;

namespace PraticeManagement.Controls.Reports
{
    public partial class CommissionsAndRates : ProjectsReportsBase
    {
        private const int TabNameColumnIndex = 0;

        private const int CR_TotalRowCount = 5;
        private const int CR_GrossMarginEligibleForCommissions = 0;
        private const int CR_SalesCommissionsIndex = 1;
        private const int CR_PMCommissionsIndex = 2;
        private const int CR_AvgBillRateIndex = 3;
        private const int CR_AvgPayRateIndex = 4;

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

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Validate();
            if (!IsPostBack || Page.IsValid)
            {
                var periodStart =
                    new DateTime(mpPeriodStart.SelectedYear, mpPeriodStart.SelectedMonth, Constants.Dates.FirstDay);
                var monthsInPeriod = GetPeriodLength();

                AddMonthColumn(gvCommissionsAndRates, 1, periodStart, monthsInPeriod);

                Project[] financialSummary =
                    GetFinancialSummary(
                        CompanyPerformanceState.ProjectList,
                        CompanyPerformanceState.BenchList,
                        CompanyPerformanceState.ExpenseList, PeriodStart, PeriodEnd);

                gvCommissionsAndRates.DataSource = PopulateCommissionsAndRatesGrid(financialSummary);
                gvCommissionsAndRates.DataBind();
            }
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
    }
}
