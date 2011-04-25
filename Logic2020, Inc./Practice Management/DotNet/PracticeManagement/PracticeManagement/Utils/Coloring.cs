using System;
using System.Drawing;
using System.Web.UI.DataVisualization.Charting;
using PraticeManagement.Configuration.ConsReportColoring;

namespace PraticeManagement.Utils
{
    /// <summary>
    /// Generic utils for coloring
    /// </summary>
    public class Coloring
    {
        #region Constants

        private static Color DEFAULT_COLOR = Color.Black;

        #endregion

        /// <summary>
        /// Returns color based on utilization value
        /// </summary>
        /// <param name="utilization">Utilization value in percents</param>
        /// <param name="isVac">Is that vacation period</param>
        /// <returns>Color based on config settings</returns>
        public static Color GetColorByUtilization(int utilization, bool isVac , bool isHired=true)
        {
            //  Get settings from web.config
            ConsReportColoringElementSection coloring = 
                ConsReportColoringElementSection.ColorSettings;

            if(!isHired)
                return coloring.HiredColor;

            //  If that's vacation, return vacation color
            if (isVac)
                return coloring.VacationColor;

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
        /// Adds color coding to legend
        /// </summary>
        /// <param name="legend">Legend to put colors to</param>
        public static void ColorLegend(Legend legend)
        {
            //  Clear legend items first
            LegendItemsCollection legendItems = legend.CustomItems;
            legendItems.Clear();

            //  Iterate through all colors and put them on legend
            ConsReportColoringElementSection coloring = 
                ConsReportColoringElementSection.ColorSettings;
            foreach (ConsReportColoringElement color in coloring.Colors)
            {
                legendItems.Add(color.ItemColor, color.Title);
            }

            //  Add vacation item
            legendItems.Add(coloring.VacationColor, coloring.VacationTitle);
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
                        new string[]{separator}, 
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

