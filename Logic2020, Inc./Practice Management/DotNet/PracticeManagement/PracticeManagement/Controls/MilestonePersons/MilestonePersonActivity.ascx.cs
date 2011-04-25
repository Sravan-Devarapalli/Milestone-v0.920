using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using DataTransferObjects;

namespace PraticeManagement.Controls.MilestonePersons
{
    public partial class MilestonePersonActivity : UserControl
    {
        #region Constants

        private const string PROJECTED_SERIES = "Projected";
        private const string ACTUAL_SERIES = "Actual";
        private const string REGRESSION_SERIES = "Expectation";
        private const string VALUE_FIELD = "Value";
        private const string DATE_FIELD = "Date";
        private const string POINT_FORMAT = "Tooltip=Value{F2}";
        private const string ACTUAL_POINTS_SERIES = "Actual (points)";

        #endregion

        public IActivityPeriod ActivityPeriod
        {
            set
            {
                try
                {
                    if (value != null)
                    {
                        DatePeriodBase projected = new ProjectedHoursPeriod(value);
                        PutDataOnGraph(
                                          activityChart.Series[PROJECTED_SERIES],
                                          projected.Period);

                        IEnumerable<DatePoint> actualPeriod = new ActualHoursPeriod(value).Period;
                        PutDataOnGraph(
                                          activityChart.Series[ACTUAL_SERIES],
                                          actualPeriod);
                        PutDataOnGraph(
                                          activityChart.Series[ACTUAL_POINTS_SERIES],
                                          actualPeriod);

                        DatePeriodBase regression = new ExpectedHoursPeriod(value);
                        PutDataOnGraph(
                                          activityChart.Series[REGRESSION_SERIES],
                                          regression.Period);
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

        private static void PutDataOnGraph(Series series, IEnumerable<DatePoint> period)
        {
            series.Points.DataBind(period, DATE_FIELD, VALUE_FIELD, POINT_FORMAT);
        }
    }
}
