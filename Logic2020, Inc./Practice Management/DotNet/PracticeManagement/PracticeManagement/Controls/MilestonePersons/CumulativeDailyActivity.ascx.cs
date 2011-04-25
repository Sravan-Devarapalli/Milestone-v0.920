using System;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using DataTransferObjects;

namespace PraticeManagement.Controls.MilestonePersons
{
    public partial class CumulativeDailyActivity : UserControl
    {
        #region Constants

        private const string PROJECTED_SERIES = "Projected";
        private const string ACTUAL_SERIES = "Actual";
        private const string REGRESSION_SERIES = "Expectation";
        private const string CUMULATIVE_SUFFIX = " Cumulative";
        private const string PROJECTED_SERIES_CUMULATIVE = PROJECTED_SERIES + CUMULATIVE_SUFFIX;
        private const string PROJECTED_SERIES_TOTAL = "Projected Total";
        private const string ACTUAL_SERIES_CUMULATIVE = ACTUAL_SERIES + CUMULATIVE_SUFFIX;
        private const string REGRESSION_SERIES_CUMULATIVE = REGRESSION_SERIES + CUMULATIVE_SUFFIX;

        #endregion

        public IActivityPeriod ActivityPeriod
        {
            set
            {
                try
                {
                    if (value != null)
                    {
                        PutDataOnGraph(ACTUAL_SERIES_CUMULATIVE, new ActualHoursPeriod(value));
                        PutDataOnGraph(REGRESSION_SERIES_CUMULATIVE, new ExpectedHoursPeriod(value));
                        PutDataOnGraph(PROJECTED_SERIES_CUMULATIVE, new TotalProjectedHoursPeriod(value));
                        PutDataOnGraph(PROJECTED_SERIES_TOTAL, new ProjectedHoursPeriod(value));
                    }
                }
                catch (Exception exc)
                {
                    activityChart.Visible = false;
                    mlErrors.ShowErrorMessage(exc.Message);
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        private void PutDataOnGraph(string seriesName, DatePeriodBase baseDates)
        {
            DataPointCollection points = activityChart.Series[seriesName].Points;
            foreach (DatePoint pt in baseDates.CumulativePeriod)
                points.AddXY(pt.Date, pt.Value);
        }
    }
}
