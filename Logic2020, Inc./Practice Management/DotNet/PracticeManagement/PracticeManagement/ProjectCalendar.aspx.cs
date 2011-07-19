using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using DataTransferObjects;
using PraticeManagement.Utils;
using PraticeManagement.Objects;
using PraticeManagement.Configuration.ConsReportColoring;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using AjaxControlToolkit;

namespace PraticeManagement
{
    public partial class ProjectCalendar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DateTime today = Utils.Generic.GetNowWithTimeZone();
                var thisMonthFirst = new DateTime(today.Year, today.Month, Constants.Dates.FirstDay);
                var startDate = thisMonthFirst.AddMonths(-1);
                var endDate = thisMonthFirst.AddMonths(2).AddDays(-1);
                lblRange.Text = string.Format("{0} - {1}", startDate.ToString("MM/dd/yyyy"), endDate.ToString("MM/dd/yyyy"));
                ShowDetailedReport(startDate, endDate);
            }
        }

        private void ShowDetailedReport(DateTime repStartDate, DateTime repEndDate)
        {
            chartProjectDetails.Visible = true;
            chartProjectDetails.Titles["ProjectsTitle"].Text = "Project Calendar";
            var points = chartProjectDetails.Series["Projects"].Points;
            points.Clear();

            // Get report
            var bars = DataHelper.GetProjects(repStartDate, repEndDate);

            var minDate = DateTime.MaxValue;
            var maxDate = DateTime.MinValue;

            for (int barIndex = 0; barIndex < bars.Count; barIndex++)
            {
                //  Add point to the plot
                var ptStart = bars[barIndex].StartDate;
                var ptEnd = bars[barIndex].EndDate.AddDays(1);

                var ind = points.AddXY(barIndex + 2, ptStart, ptEnd);
                var pt = points[ind];
                //
                switch (bars[barIndex].BarType)
                {
                    case DetailedProjectReportItem.ItemType.ProjectedProject:
                        pt.Color = Color.FromArgb(217, 211, 68);
                        break;

                    case DetailedProjectReportItem.ItemType.ActiveProject:
                        pt.Color = Color.FromArgb(142, 213, 55);
                        break;
                }

                SetBarTextAndTooltip(repStartDate, repEndDate, bars[barIndex], pt);

                if (bars[barIndex].Project.Status.Id == (int)ProjectStatusType.Active
                    && bars[barIndex].Project.Attachment == null)
                {
                    SetAlterNativeColorsForBar(ptStart, ptEnd, barIndex, points);
                }

                // Find min and max dates
                if (minDate.CompareTo(ptStart) > 0) minDate = ptStart;
                if (maxDate.CompareTo(ptEnd) < 0) maxDate = ptEnd;
            }

            SetAxisMaxAndMin(repStartDate, repEndDate);

            chartProjectDetails.Height = 2 * (Resources.Controls.TimelineDetailedHeaderFooterHeigth - 15) +
                                  bars.Count * Resources.Controls.TimelineDetailedItemHeigth;
        }

        private void SetBarTextAndTooltip(DateTime repStartDate, DateTime repEndDate, DetailedProjectReportItem item, DataPoint pt)
        {
            double width = chartProjectDetails.Width.Value;

            int diff = repEndDate.Subtract(repStartDate).Days;

            double onedayWidth = width / diff;

            double totalDays = pt.YValues[1] - pt.YValues[0];

            double lblWidth = onedayWidth * totalDays;

            double txtWidth = GetTextWidth(item.Label);

            if (txtWidth < lblWidth)
            {
                // Set proper label and make it clickable
                pt.Label = item.Label;
            }
            else
            {
                txtWidth = GetTextWidth(item.smallLabel);
                if (txtWidth < lblWidth)
                    pt.Label = item.smallLabel;
            }
            pt.ToolTip = item.Tooltip;
        }

        private void SetAlterNativeColorsForBar(DateTime ptStart, DateTime ptEnd, int barIndex, DataPointCollection points)
        {
            int length = ptEnd.Subtract(ptStart).Days;

            for (int i = 0; i < length; i++)
            {
                var index = points.AddXY(barIndex + 2, ptStart.AddDays(i), ptStart.AddDays(i + 1));
                var point = points[index];

                if (i == 0 || i % 2 == 0)
                {
                    point.Color = Color.FromArgb(142, 213, 55);
                }
                else
                {
                    point.Color = Color.FromArgb(217, 211, 68);
                }
            }
        }

        private Double GetTextWidth(string lableText)
        {
            SizeF size = Graphics.FromImage(new Bitmap(1, 1)).MeasureString(lableText, new Font("Arial", 11));
            Double width = Convert.ToDouble(size.Width);

            return width;
        }

        private void SetAxisMaxAndMin(DateTime minDate, DateTime maxDate)
        {
            var horizAxis = chartProjectDetails.ChartAreas["ProjectsArea"].AxisY;
            horizAxis.Minimum = minDate.ToOADate();
            horizAxis.Maximum = maxDate.ToOADate();


            var horizAxis2 = chartProjectDetails.ChartAreas["ProjectsArea"].AxisY2;
            horizAxis2.Minimum = minDate.ToOADate();
            horizAxis2.Maximum = maxDate.ToOADate();
        }

        private void PrepareTooltip(DataPointCustomProperties pt)
        {
            ProjectNameCellRounded btnProject = (ProjectNameCellRounded)LoadControl(Constants.ApplicationControls.ProjectNameCellRoundedControl);
            btnProject.ButtonProjectNameId = pt.Label;

            btnProject.ButtonProjectNameToolTip = pt.ToolTip; //PrepareToolTipView(project);
            btnProject.ToolTipOffsetX = 5;
            btnProject.ToolTipOffsetY = -15;
            btnProject.ToolTipPopupPosition = HoverMenuPopupPosition.Right;
            var indentDiv = new Panel() { CssClass = "cell-pad" };
            indentDiv.Controls.Add(btnProject);
            chartProjectDetails.Controls.Add(indentDiv);

        }

        protected void imgbtnNavigateRange_Click(object sender, EventArgs e)
        {
            var button = sender as ImageButton;
            var dates = lblRange.Text.Split('-');
            var startDate = DateTime.Parse(dates[0]);
            var endDate = DateTime.Parse(dates[1]);


            if (button.ID == "imgbtnPrevious")
            {
                var temp = startDate;
                endDate = temp.AddDays(-1);
                startDate = temp.AddMonths(-3);
            }
            else
            {
                var temp = endDate;
                startDate = temp.AddDays(1);
                endDate = startDate.AddMonths(3).AddDays(-1);
            }
            lblRange.Text = string.Format("{0} - {1}", startDate.ToString("MM/dd/yyyy"), endDate.ToString("MM/dd/yyyy"));
            ShowDetailedReport(startDate, endDate);
        }
    }
}
