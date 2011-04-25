using System;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PraticeManagement.Controls.Reports
{
    public partial class ConsultantsReport : UserControl
    {
        #region Contants

        private static readonly Color W2_HOURLY_COLOR = Color.FromArgb(255, 255, 229);
        private static readonly Color W2_SALARY_COLOR = Color.FromArgb(242, 255, 229);
        private static readonly Color HOURLY_1099_COLOR = Color.FromArgb(255, 242, 229);
        private static readonly Color HOURLY_POR_COLOR = Color.FromArgb(229, 242, 255);

        private const string AVAILABLE_HOURS_COLUMN_NAME = "Available Hours";
        private const string HOLIDAY_HOURS_COLUMN_NAME = "Holiday Hours";
        private const string PROJECTED_HOURS_COLUMN_NAME = "Projected Hours";
        private const string PAY_TYPE_ID_COLUMN_NAME = "TimescaleId";
        private const string BENCH_HOURS_COLUMN_NAME = "Bench Hours";

        private const string EXCEL_FILE_NAME = "Consultants_{0}.xls";

        private const string TOTALS_CELL_TEXT = "Totals: ";

        private const string REPORT_TITLE_TEMPLATE = "Consultant utilization report - {0}";

        private const int TOTALS_CELL_INDEX = 1;

        private const int HOLIDAY_CELL_INDEX = 5;
        private const int AVAILABLE_CELL_INDEX = 6;
        private const int PROJECTED_CELL_INDEX = 7;
        private const int BENCH_CELL_INDEX = 8;
        private const int UTILIZATION_CELL_INDEX = 9;

        #endregion

        #region Total variables

        private int _totalAvailable;
        private int _totalHoliday;
        private int _totalProjected;
        private int _totalBench;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            cell1009HourlyColor.BackColor = HOURLY_1099_COLOR;
            cell1009PORColor.BackColor = HOURLY_POR_COLOR;
            cellW2HourlyColor.BackColor = W2_HOURLY_COLOR;
            cellW2SalaryColor.BackColor = W2_SALARY_COLOR;
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            GridViewExportUtil.Export(
                string.Format(EXCEL_FILE_NAME, ReportFilter.MonthBegin.ToString("MMM_yyyy")), 
                gvConsultantsReport, 
                string.Format(REPORT_TITLE_TEMPLATE, ReportFilter.MonthBegin.ToString("Y")));
        }

        protected void btnConsultantName_Command(object sender, CommandEventArgs e)
        {
            string personId = e.CommandArgument.ToString();
            string redirectPath = 
                string.Format(
                            Constants.ApplicationPages.DetailRedirectWithReturnFormat,
                            Constants.ApplicationPages.PersonDetail, personId, 
                            Constants.ApplicationPages.ConsultantsUtilizationReport);

            Response.Redirect(redirectPath);
        }

        protected void gvConsultantsReport_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                case DataControlRowType.DataRow:
                    _totalAvailable += GetDataFromCell(e, AVAILABLE_HOURS_COLUMN_NAME);
                    _totalHoliday += GetDataFromCell(e, HOLIDAY_HOURS_COLUMN_NAME);
                    _totalProjected += GetDataFromCell(e, PROJECTED_HOURS_COLUMN_NAME);
                    _totalBench += GetDataFromCell(e, BENCH_HOURS_COLUMN_NAME);

                    ChangeBackground(e);
                    break;

                case DataControlRowType.Footer:
                    e.Row.Cells[TOTALS_CELL_INDEX].Text = TOTALS_CELL_TEXT;

                    e.Row.Cells[AVAILABLE_CELL_INDEX].Text = _totalAvailable.ToString();
                    e.Row.Cells[HOLIDAY_CELL_INDEX].Text = _totalHoliday.ToString();
                    e.Row.Cells[PROJECTED_CELL_INDEX].Text = _totalProjected.ToString();
                    e.Row.Cells[BENCH_CELL_INDEX].Text = _totalBench.ToString();
                    e.Row.Cells[UTILIZATION_CELL_INDEX].Text = GetUtilization() + "%";
                    break;
            }
        }

        /// <summary>
        /// Change background according to the pay type
        /// </summary>
        /// <param name="e"></param>
        private static void ChangeBackground(GridViewRowEventArgs e)
        {
            switch (GetDataFromCell(e, PAY_TYPE_ID_COLUMN_NAME))
            {
                case 1:
                    e.Row.BackColor = W2_HOURLY_COLOR;
                    break;

                case 2:
                    e.Row.BackColor = W2_SALARY_COLOR;
                    break;

                case 3:
                    e.Row.BackColor = HOURLY_1099_COLOR;
                    break;

                case 4:
                    e.Row.BackColor = HOURLY_POR_COLOR;
                    break;
            }
        }

        private double GetUtilization()
        {
            double utilization = 0.0;
            if (_totalAvailable > 0)
                utilization = Math.Round(100.0 * _totalProjected / _totalAvailable, 1);
            return utilization;
        }

        private static int GetDataFromCell(GridViewRowEventArgs e, string columnName)
        {
            object cellValue = GetCellValue(e, columnName);
            return IsEmptyValue(cellValue) ? 0 : Convert.ToInt32(cellValue);
        }

        private static bool IsEmptyValue(object dataAvaliable)
        {
            return dataAvaliable == DBNull.Value;
        }

        private static object GetCellValue(GridViewRowEventArgs e, string columnName)
        {
            return DataBinder.Eval(e.Row.DataItem, columnName);
        }

        protected void gvConsultantsReport_DataBound(object sender, EventArgs e)
        {
            DataBindOrHideChart();
        }

        private void DataBindOrHideChart()
        {
            //if (gvConsultantsReport.Rows.Count > 0)
            //{
            //    UtilizationChart.DataSource = odsReport;
            //    UtilizationChart.DataBind();
            //    UtilizationChart.Visible = true;
            //}
            //else
            //    UtilizationChart.Visible = false;
        }

        protected void btnUpdateView_OnClick(object sender, EventArgs e)
        {

        }

        protected void btnResetFilter_OnClick(object sender, EventArgs e)
        {
            ReportFilter.ResetBasicFilter();
        }
    }
}
