using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;
using System.Linq;

namespace PraticeManagement.Controls.Reports
{
    public partial class BenchReport : System.Web.UI.UserControl
    {
        private const string ConsultantNameSortOrder = "ConsultantNameSortOrder";
        private const string PracticeSortOrder = "PracticeSortOrder";
        private const string StatusSortOrder = "StatusSortOrder";
        private const string Descending = "Descending";
        private const string Ascending = "Ascending";
        private const string ReportContextKey = "ReportContext";
        private const string BenchListKey = "BenchList";


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
                        ActiveProjects = true,
                        ProjectedProjects = true,
                        ExperimentalProjects = true,
                        UserName = DataHelper.CurrentPerson.Alias,
                        PracticeIds = string.IsNullOrEmpty(cblPractices.SelectedItems) ? string.Empty : cblPractices.SelectedItems
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.FillPracticeList(this.cblPractices, Resources.Controls.AllPracticesText);
                SelectAllItems(this.cblPractices);
                DatabindGrid();
            }
        }

        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage("Bench Report");
            GridViewExportUtil.Export("BenchRollOff.xls", gvBench);
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

        private void SelectAllItems(ScrollingDropDown ddlpractices)
        {
            foreach (ListItem item in ddlpractices.Items)
            {
                item.Selected = true;
            }
        }

        private void DatabindGrid()
        {
            var benchList = BenchList;
            AddMonthColumn(3, new DateTime(ReportContext.Start.Year, ReportContext.Start.Month, Constants.Dates.FirstDay), GetPeriodLength());

            gvBench.DataSource = benchList.OrderBy(P => P.Name);
            gvBench.DataBind();

            if (gvBench.Rows.Count > 0)
            {
                btnExportToExcel.Visible = true;
            }
            else
            {
                btnExportToExcel.Visible = false;
            }
        }

        protected void btnUpdateReport_Click(object sender, EventArgs e)
        {
            Page.Validate(valSummaryPerformance.ValidationGroup);
            if (Page.IsValid)
            {
                ReportContext = null;
                BenchList = null;
                DatabindGrid();
                gvBench.Attributes[ConsultantNameSortOrder] = Descending;
                gvBench.Attributes[PracticeSortOrder] = Ascending;
                gvBench.Attributes[StatusSortOrder] = Ascending;
            }
        }

        protected void btnSortConsultant_Click(object sender, EventArgs e)
        {
            var sortOrder = gvBench.Attributes[ConsultantNameSortOrder];
            var benchList = BenchList;
            if (string.IsNullOrEmpty(sortOrder) || sortOrder == Descending)
            {
                benchList = benchList.OrderByDescending(P => P.Name);
                gvBench.Attributes[ConsultantNameSortOrder] = Ascending;
            }
            else
            {
                benchList = benchList.OrderBy(P => P.Name);
                gvBench.Attributes[ConsultantNameSortOrder] = Descending;
            }
            gvBench.Attributes[PracticeSortOrder] = Ascending;
            gvBench.Attributes[StatusSortOrder] = Ascending;
            AddMonthColumn(3, ReportContext.Start, GetPeriodLength());
            gvBench.DataSource = benchList;
            gvBench.DataBind();
        }

        protected void btnSortPractice_Click(object sender, EventArgs e)
        {

            var sortOrder = gvBench.Attributes[PracticeSortOrder];
            var benchList = BenchList;
            if (string.IsNullOrEmpty(sortOrder) || sortOrder == Ascending)
            {
                benchList = benchList.OrderBy(P => P.Practice == null ? string.Empty : P.Practice.Name);
                gvBench.Attributes[PracticeSortOrder] = Descending;
            }
            else
            {
                benchList = benchList.OrderByDescending(P => P.Practice == null ? string.Empty : P.Practice.Name);
                gvBench.Attributes[PracticeSortOrder] = Ascending;
            }
            gvBench.Attributes[ConsultantNameSortOrder] = Ascending;
            gvBench.Attributes[StatusSortOrder] = Ascending;
            AddMonthColumn(3, ReportContext.Start, GetPeriodLength());
            gvBench.DataSource = benchList;
            gvBench.DataBind();

        }

        protected void btnSortStatus_Click(object sender, EventArgs e)
        {

            var sortOrder = gvBench.Attributes[StatusSortOrder];
            var benchList = BenchList;

            if (string.IsNullOrEmpty(sortOrder) || sortOrder == Ascending)
            {
                benchList = benchList.OrderBy(P => P.ProjectNumber);
                gvBench.Attributes[StatusSortOrder] = Descending;
            }
            else
            {

                benchList = benchList.OrderByDescending(P => P.ProjectNumber);
                gvBench.Attributes[StatusSortOrder] = Ascending;
            }
            gvBench.Attributes[ConsultantNameSortOrder] = Ascending;
            gvBench.Attributes[PracticeSortOrder] = Ascending;
            AddMonthColumn(3, ReportContext.Start, GetPeriodLength());
            gvBench.DataSource = benchList;
            gvBench.DataBind();
        }

        private void AddMonthColumn(int numberOfFixedColumns, DateTime periodStart, int monthsInPeriod)
        {
            // Remove columns from previous report if there was one
            for (var i = numberOfFixedColumns; i < gvBench.Columns.Count; i++)
                gvBench.Columns[i].Visible = false;

            // Add columns for new months);
            for (int i = numberOfFixedColumns, k = 0; k < monthsInPeriod; i++, k++)
            {
                gvBench.Columns[i].HeaderText =
                    Resources.Controls.TableHeaderOpenTag +
                    periodStart.ToString("MMM, yyyy") +
                    Resources.Controls.TableHeaderCloseTag;
                periodStart = periodStart.AddMonths(1);
            }
        }

        protected void gvBench_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var project = e.Row.DataItem as Project;
                bool rowVisible = false;

                if (project != null)
                {
                    //var monthBegin = rpReportFilter.MonthBegin;
                    var monthBegin = new DateTime(ReportContext.Start.Year, ReportContext.Start.Month, Constants.Dates.FirstDay); ;

                    int periodLength = GetPeriodLength();

                    // Displaying the interest values (main cell data)
                    for (int i = 3, k = 0;
                        k < periodLength;
                        i++, k++, monthBegin = monthBegin.AddMonths(1))
                    {
                        var monthEnd =
                            new DateTime(monthBegin.Year,
                                monthBegin.Month,
                                DateTime.DaysInMonth(monthBegin.Year, monthBegin.Month));

                        gvBench.Columns[i].Visible = true;
                        gvBench.Columns[i].HeaderText =
                            Resources.Controls.TableHeaderOpenTag +
                            monthBegin.ToString("MMM, yyyy") +
                            Resources.Controls.TableHeaderCloseTag;

                        foreach (KeyValuePair<DateTime, ComputedFinancials> interestValue in
                            project.ProjectedFinancialsByMonth)
                        {
                            if (IsInMonth(interestValue.Key, monthBegin, monthEnd) && interestValue.Value.GrossMargin.Value != 0M)
                            {
                                rowVisible = true;

                                if (interestValue.Value.EndDate.HasValue)
                                {
                                    if (IsInMonth(interestValue.Value.EndDate.Value, monthBegin, monthEnd))
                                    {
                                        e.Row.Cells[i].Text =
                                            interestValue.Value.EndDate.Value.ToShortDateString();
                                        e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;
                                    }
                                    else
                                    {
                                        e.Row.Cells[i].Text = Resources.Controls.BusyLabel;
                                        e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Center;
                                    }
                                }
                                else
                                {
                                    e.Row.Cells[i].Text = Resources.Controls.FreeLabel;
                                    e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Center;
                                }
                            }
                        }
                    }
                }

                for (int i = 3; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].CssClass = "CompPerfData";
                }

                e.Row.Visible = rowVisible;
            }
        }

        private static bool IsInMonth(DateTime date, DateTime periodStart, DateTime periodEnd)
        {
            bool result =
                (date.Year > periodStart.Year ||
                (date.Year == periodStart.Year && date.Month >= periodStart.Month)) &&
                (date.Year < periodEnd.Year || (date.Year == periodEnd.Year && date.Month <= periodEnd.Month));

            return result;
        }

        /// <summary>
        /// Calculates a length of the selected period in the mounths.
        /// </summary>
        /// <returns>The number of the months within the selected period.</returns>
        private int GetPeriodLength()
        {
            //return 1;
            int mounthsInPeriod =
                (ReportContext.End.Year - ReportContext.Start.Year) * Constants.Dates.LastMonth +
                (ReportContext.End.Month - ReportContext.Start.Month + 1);
            return mounthsInPeriod;
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

        protected void custPeriod_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = ((mpEndDate.SelectedYear - mpStartDate.SelectedYear) * Constants.Dates.LastMonth +
                            (mpEndDate.SelectedMonth - mpStartDate.SelectedMonth + 1)) > 0;
        }
    }
}

