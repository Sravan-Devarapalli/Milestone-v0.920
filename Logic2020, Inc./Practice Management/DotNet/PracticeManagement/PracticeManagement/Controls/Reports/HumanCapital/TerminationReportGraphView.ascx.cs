using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using System.Web.UI.DataVisualization.Charting;
using iTextSharp.text;
using System.Drawing;
using DataTransferObjects.Reports.HumanCapital;

namespace PraticeManagement.Controls.Reports.HumanCapital
{
    public partial class TerminationReportGraphView : System.Web.UI.UserControl
    {

        #region constant

        private const string MAIN_CHART_AREA_NAME = "MainArea";
        private const string SeeYearToDate = "See Year To Date";
        private const string SeeLast12Months = "See Last 12 Months";

        #endregion

        #region properties

        private PraticeManagement.Reporting.TerminationReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.TerminationReport)Page); }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region Methods


        protected void hlnkGraph_Click(object sender, EventArgs e)
        {
            if (hlnkGraph.Text == SeeYearToDate)
            {
                hlnkGraph.Text = SeeLast12Months;
            }
            else
            {
                hlnkGraph.Text = SeeYearToDate;
            }
            PopulateGraph();
        }

        protected void chrtTerminationAndAttritionLast12Months_Click(object sender, ImageMapEventArgs e)
        {
            string[] postBackDetails = e.PostBackValue.Split(',');

            lbTotalTerminations.Text = postBackDetails[1];
            var startDate = Utils.Calendar.MonthStartDate(Convert.ToDateTime(postBackDetails[0]));
            var endDate = Utils.Calendar.MonthEndDate(Convert.ToDateTime(postBackDetails[0]));
            List<Person> data = ServiceCallers.Custom.Report(r => r.TerminationReport(startDate, endDate, HostingPage.PayTypes, null, HostingPage.Seniorities, HostingPage.TerminationReasons, HostingPage.Practices, HostingPage.ExcludeInternalProjects, null, null, null, null)).PersonList;
            lbName.Text = "Month : " + postBackDetails[0];
            mpeDetailView.Show();
        }

        public void PopulateGraph()
        {
            var now = Utils.Generic.GetNowWithTimeZone();
            DateTime startDate;
            DateTime endDate;
            List<TerminationPersonsInRange> terminationPersonInRange;
            if (hlnkGraph.Text == SeeYearToDate)
            {
                startDate = Utils.Calendar.Last12MonthStartDate(now);
            }
            else
            {
                startDate = Utils.Calendar.YearStartDate(now);
            }
            endDate = Utils.Calendar.LastMonthEndDate(now);
            terminationPersonInRange = ServiceCallers.Custom.Report(r => r.TerminationReportGraph(startDate, endDate, HostingPage.PayTypes, HostingPage.Seniorities, HostingPage.TerminationReasons, HostingPage.Practices, HostingPage.ExcludeInternalProjects)).ToList();
            //HostingPage.PopulateHeaderSection(terminationPersonInRange);

            LoadChartData(terminationPersonInRange);
        }
        private void LoadChartData(List<TerminationPersonsInRange> data)
        {
            InitChart(data.Count);
            chrtTerminationAndAttritionLast12Months.DataSource = data;
            chrtTerminationAndAttritionLast12Months.DataBind();
        }

        private void InitChart(int count)
        {
            chrtTerminationAndAttritionLast12Months.Width = ((count < 4) ? 4 : count) * 70;
            chrtTerminationAndAttritionLast12Months.Height = 500;
            InitAxis(chrtTerminationAndAttritionLast12Months.ChartAreas[MAIN_CHART_AREA_NAME].AxisX, "Month", false);
            InitAxis(chrtTerminationAndAttritionLast12Months.ChartAreas[MAIN_CHART_AREA_NAME].AxisY, "Number of Terminations", true);
            InitAxis(chrtTerminationAndAttritionLast12Months.ChartAreas[MAIN_CHART_AREA_NAME].AxisY2, "Attrition Percentage", true);
            chrtTerminationAndAttritionLast12Months.ChartAreas[0].AxisY2.LabelStyle.Format = "{#}%";
            chrtTerminationAndAttritionLast12Months.ChartAreas[0].AxisY2.TextOrientation = TextOrientation.Rotated90;
            UpdateChartTitle();
        }

        private void UpdateChartTitle()
        {
            chrtTerminationAndAttritionLast12Months.Titles.Clear();
            if (hlnkGraph.Text == SeeYearToDate)
            {
                chrtTerminationAndAttritionLast12Months.Titles.Add("Termination and Attrition");
                chrtTerminationAndAttritionLast12Months.Titles.Add("Last 12 Months");
            }
            else
            {
                chrtTerminationAndAttritionLast12Months.Titles.Add("Termination and Attrition");
                chrtTerminationAndAttritionLast12Months.Titles.Add("Year To Date");
            }

            chrtTerminationAndAttritionLast12Months.Titles[0].Font = new System.Drawing.Font("Arial", 16, FontStyle.Bold);
            chrtTerminationAndAttritionLast12Months.Titles[1].Font = new System.Drawing.Font("Arial", 16, FontStyle.Bold);
        }

        private void InitAxis(Axis horizAxis, string title, bool isVertical)
        {
            horizAxis.IsStartedFromZero = true;
            if (!isVertical)
                horizAxis.Interval = 1;
            horizAxis.TextOrientation = isVertical ? TextOrientation.Rotated270 : TextOrientation.Horizontal;
            horizAxis.LabelStyle.Angle = isVertical ? 0 : 45;
            horizAxis.TitleFont = new System.Drawing.Font("Arial", 14, FontStyle.Bold);
            horizAxis.ArrowStyle = AxisArrowStyle.None;
            horizAxis.MajorGrid.Enabled = false;
            horizAxis.ToolTip = horizAxis.Title = title;

        }

        #endregion
    }
}
