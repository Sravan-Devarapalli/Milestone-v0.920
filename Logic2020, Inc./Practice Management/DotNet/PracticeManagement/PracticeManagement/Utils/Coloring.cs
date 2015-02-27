using System;
using System.Drawing;
using System.Web.UI.DataVisualization.Charting;
using PraticeManagement.Configuration.ConsReportColoring;
using System.Web;
using System.IO;

namespace PraticeManagement.Utils
{
    /// <summary>
    /// Generic utils for coloring
    /// </summary>
    public class Coloring
    {
        #region Constants

        private static readonly Color DEFAULT_COLOR = Color.Black;
        private const string OpportuityDemandLegendFormat = "Opportunity with \"{0}\"  Sales Stage";

        #endregion Constants

        /// <summary>
        /// Returns color based on utilization value
        /// </summary>
        /// <param name="utilization">Utilization value in percents</param>
        /// <param name="isVac">Is that vacation period</param>
        /// <returns>Color based on config settings</returns>
        public static Color GetColorByUtilization(int utilization, int dayType, bool isHiredIntheEmployeementRange = true)
        {
            //if dayType == 1 =>it's timeoff if daytype == 2 =>it's companyholiday
            //  Get settings from web.config
            ConsReportColoringElementSection coloring =
                ConsReportColoringElementSection.ColorSettings;

            if (!isHiredIntheEmployeementRange)
                return coloring.HiredColor;

            //  If that's vacation, return vacation color
            if (dayType == 1)
                return coloring.VacationColor;
            if (dayType == 2)
                return coloring.CompanyHolidayColor;

            //  Iterate through all colors and check their min/max values
            foreach (ConsReportColoringElement color in coloring.Colors)
            {
                if (utilization >= color.MinValue &&
                        utilization <= color.MaxValue)
                    return color.ItemColor;
            }

            //  Return default color if nothing was foung in config
            return DEFAULT_COLOR;
        }

        /// <summary>
        /// Returns color based on capacity value
        /// </summary>
        /// <param name="utilization">Capacity value in percents</param>
        /// <param name="isVac">Is that vacation period</param>
        /// <returns>Color based on config settings</returns>
        public static Color GetColorByCapacity(int capacity, int dayType, bool isHiredIntheEmployeementRange, bool isWeekEnd)
        {
            //if dayType == 1 =>it's timeoff if daytype == 2 =>it's companyholiday
            //  Get settings from web.config
            ConsReportColoringElementSection coloring =
                ConsReportColoringElementSection.ColorSettings;

            if (!isHiredIntheEmployeementRange)
                return coloring.HiredColor;

            //  If that's vacation, return vacation color
            if (dayType == 1)
                return coloring.VacationColor;
            if (dayType == 2)
                return coloring.CompanyHolidayColor;

            if (isWeekEnd)
                return Color.FromArgb(255, 255, 255);

            if (capacity >= 50 && capacity <= 100)
            {
                return Color.FromArgb(255, 0, 0);//Red color.
            }
            else if (capacity >= 10 && capacity <= 49)
            {
                return Color.FromArgb(255, 255, 0);//Yellow Color.
            }
            else if (capacity >= -5 && capacity <= 9)
            {
                return Color.FromArgb(82, 178, 0);//Green Color.
            }
            else if (capacity <= -6)
            {
                return Color.FromArgb(51, 204, 255);//sky blue Color.
            }

            //  Return default color if nothing was foung in config
            return DEFAULT_COLOR;
        }

        public static void CapacityColorLegends(Legend legend)
        {
            //  Clear legend items first
            LegendItemsCollection legendItems = legend.CustomItems;
            legendItems.Clear();

            //  Iterate through all colors and put them on legend
            legendItems.Add(Color.FromArgb(255, 0, 0), "Capacity = 100 - 50%");//Red
            legendItems.Add(Color.FromArgb(255, 255, 0), "Capacity = 49 - 10%");//Yellow
            legendItems.Add(Color.FromArgb(82, 178, 0), "Capacity = 9 - (-5)%");//Green
            legendItems.Add(Color.FromArgb(51, 204, 255), "Capacity = (-6)+%");//Sky Blue.

            ConsReportColoringElementSection coloring =
                ConsReportColoringElementSection.ColorSettings;
            //  Add vacation item
            legendItems.Add(coloring.VacationColor, coloring.VacationTitle);
            // Add company holiday item
            legendItems.Add(coloring.CompanyHolidayColor, coloring.CompanyHolidaysTitle);
        }

        public static Color GetColorByConsultingDemand(DataTransferObjects.ConsultantDemandItem item)
        {
            if (item.ObjectType == 2 && item.ObjectStatusId == 3) //Project with Active status.
            {
                return Color.FromArgb(255, 0, 0); //Red.
            }
            else if (item.ObjectType == 2 && item.ObjectStatusId == 2) //Project with Projected status.
            {
                return Color.FromArgb(255, 255, 0); // Yellow.
            }
            else if (item.ObjectType == 1 && item.ObjectStatusId == 1) //Opportunity with A priority.
            {
                return Color.FromArgb(82, 178, 0); // Green.
            }
            else if (item.ObjectType == 1 && item.ObjectStatusId == 2) //Opportunity with B priority.
            {
                return Color.FromArgb(51, 204, 255); // Blue.
            }
            else
            {
                return Color.White;
            }
        }

        public static void DemandColorLegends(Legend legend)
        {
            //  Clear legend items first
            LegendItemsCollection legendItems = legend.CustomItems;
            legendItems.Clear();

            //  Iterate through all colors and put them on legend
            legendItems.Add(Color.FromArgb(255, 0, 0), "Project with Active Status");//Red
            legendItems.Add(Color.FromArgb(255, 255, 0), "Project with Projected Status");//Yellow

            var salesStages = SettingsHelper.DemandOpportunitySalesStages;
            if (salesStages != null && salesStages.ContainsKey(Constants.OpportunityPriorityIds.PriorityIdOfA))
            {
                legendItems.Add(Color.FromArgb(82, 178, 0), string.Format(OpportuityDemandLegendFormat, salesStages[Constants.OpportunityPriorityIds.PriorityIdOfA]));//Green
            }
            if (salesStages != null && salesStages.ContainsKey(Constants.OpportunityPriorityIds.PriorityIdOfB))
            {
                legendItems.Add(Color.FromArgb(51, 204, 255), string.Format(OpportuityDemandLegendFormat, salesStages[Constants.OpportunityPriorityIds.PriorityIdOfB]));//Sky Blue.
            }
        }

        /// <summary>
        /// Adds color coding to legend
        /// </summary>
        /// <param name="legend">Legend to put colors to</param>
        public static void ColorLegend(Legend legend, bool includeBadgeStatus)
        {
            //  Clear legend items first
            LegendItemsCollection legendItems = legend.CustomItems;
            legendItems.Clear();

            //  Iterate through all colors and put them on legend
            ConsReportColoringElementSection coloring =
                ConsReportColoringElementSection.ColorSettings;
            foreach (ConsReportColoringElement color in coloring.Colors)
            {
                var legendItem = new LegendItem();
                legendItem.Name = color.Title;
                legendItem.Color = color.ItemColor;
                legendItem.ImageStyle = LegendImageStyle.Rectangle;
                legendItem.MarkerStyle = MarkerStyle.Square;
                legendItem.MarkerSize = 50;
                legendItem.MarkerColor = Color.Black;
                legendItems.Add(legendItem);
            }

            var vacationLgn = new LegendItem();
            vacationLgn.Name = coloring.VacationTitle;
            vacationLgn.Color = coloring.VacationColor;
            vacationLgn.ImageStyle = LegendImageStyle.Rectangle;
            vacationLgn.MarkerStyle = MarkerStyle.Square;
            vacationLgn.MarkerSize = 50;
            vacationLgn.MarkerColor = Color.Black;
            legendItems.Add(vacationLgn);
            //  Add vacation item
            //legendItems.Add(coloring.VacationColor, coloring.VacationTitle);
            // Add company holiday item
            var companylgn = new LegendItem();
            companylgn.Name = coloring.CompanyHolidaysTitle;
            companylgn.Color = coloring.CompanyHolidayColor;
            companylgn.ImageStyle = LegendImageStyle.Rectangle;
            companylgn.MarkerStyle = MarkerStyle.Square;
            companylgn.MarkerSize = 50;
            companylgn.MarkerColor = Color.Black;
            legendItems.Add(companylgn);
            //legendItems.Add(coloring.CompanyHolidayColor, coloring.CompanyHolidaysTitle);

            if (includeBadgeStatus)
            {
                //MS Badged legend
                var legItem3 = new LegendItem();
                legItem3.Color = Color.White;
                legItem3.ImageStyle = LegendImageStyle.Rectangle;
                legItem3.BackHatchStyle = ChartHatchStyle.Vertical;
                legItem3.BackSecondaryColor = Color.Black;
                legItem3.BackSecondaryColor = Color.Black;
                legItem3.MarkerStyle = MarkerStyle.Square;
                legItem3.MarkerSize = 50;
                legItem3.MarkerColor = Color.Black;
                legItem3.Name = "18 mo Window Active";
                legendItems.Add(legItem3);

                //MS Badged legend
                var legItem2 = new LegendItem();
                legItem2.Color = Color.White;
                legItem2.ImageStyle = LegendImageStyle.Rectangle;
                legItem2.BackHatchStyle = ChartHatchStyle.LargeGrid;
                legItem2.BackSecondaryColor = Color.Black;
                legItem2.MarkerStyle = MarkerStyle.Square;
                legItem2.MarkerSize = 50;
                legItem2.MarkerColor = Color.Black;
                legItem2.Name = "MS Badged";
                legendItems.Add(legItem2);

                //6-Month Break and Block Badged legend
                var legItem1 = new LegendItem();
                legItem1.Color = Color.White;
                legItem1.ImageStyle = LegendImageStyle.Rectangle;
                legItem1.BackHatchStyle = ChartHatchStyle.Divot;
                legItem1.BackSecondaryColor = Color.Black;
                legItem1.Name = "6-Month Break/Block";
                legItem1.MarkerStyle = MarkerStyle.Square;
                legItem1.MarkerSize = 50;
                legItem1.MarkerColor = Color.Black;
                legendItems.Add(legItem1);
            }
        }

        /// <summary>
        /// Converts string into Color
        /// </summary>
        /// <param name="strColor">Color in 'rrr.ggg.bbb' format</param>
        /// <returns>Color parsed</returns>
        public static Color StringToColor(string strColor, string separator)
        {
            try
            {
                //  Split colors into R, G, B
                string[] rgb =
                    strColor.Split(
                        new[] { separator },
                        StringSplitOptions.RemoveEmptyEntries);

                // Init color
                return Color.FromArgb(
                    Convert.ToInt32(rgb[0]),
                    Convert.ToInt32(rgb[1]),
                    Convert.ToInt32(rgb[2]));
            }
            catch
            {
                return DEFAULT_COLOR;
            }
        }
    }
}
