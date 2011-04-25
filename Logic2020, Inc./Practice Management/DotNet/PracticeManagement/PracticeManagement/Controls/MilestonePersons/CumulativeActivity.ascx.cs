using System;
using System.Web.UI.DataVisualization.Charting;
using DataTransferObjects;

namespace PraticeManagement.Controls.MilestonePersons
{
    public partial class CumulativeActivity : System.Web.UI.UserControl
    {
        #region Constants

        private const string TOTAL_PROJECTED_SERIES = "Total Projected";
        private const string ACTUAL_TO_DATE_SERIES = "Actual to Date";
        private const string PROJECTED_AND_ACTUAL_SERIES = "Projected and Actual";

        #endregion

        #region Properties

        public IActivityPeriod ActivityPeriod
        {
            set
            {
                ShowChart(value);
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        #endregion

        #region Methods

        private void ShowChart(IActivityPeriod activityPeriod)
        {
            double totalProjected = (new ProjectedHoursPeriod(activityPeriod)).TotalProjectedHours;
            double actualToDate = (new ActualHoursPeriod(activityPeriod)).ActualToDate;
            double forecastedToDate = (new ActualHoursPeriod(activityPeriod)).ForecastedToDate;

            AddPoint(chCumulative.Series[TOTAL_PROJECTED_SERIES], totalProjected);
            AddPoint(chCumulative.Series[ACTUAL_TO_DATE_SERIES], actualToDate);
            AddPoint(chCumulative.Series[PROJECTED_AND_ACTUAL_SERIES], totalProjected - actualToDate);

            chCumulative.ChartAreas[0].RecalculateAxesScale();
        }

        private static void AddPoint(Series serie, double pointValue)
        {
            DataPoint pt = serie.Points.Add(pointValue);
            pt.Label = pointValue.ToString(Constants.Formatting.DoubleFormat);
        }

        #endregion
    }
}
