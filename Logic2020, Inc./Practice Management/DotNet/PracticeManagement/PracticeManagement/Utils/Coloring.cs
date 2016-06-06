using System;
using System.Drawing;
using System.Web.UI.DataVisualization.Charting;
using PraticeManagement.Configuration.ConsReportColoring;
using System.Web;
using System.IO;
using System.Collections.Generic;

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
        public static Color GetColorByUtilization(int utilization, int dayType, bool isHiredIntheEmployeementRange = true, int targetUtil = -1, bool isW2Hourly = false)
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

            //For non-investment Resources
            if (targetUtil == -1)
            {
                if (isW2Hourly && (utilization >= 0 && utilization <= 25))
                {
                    return Color.FromArgb(253, 253, 214);
                }
                foreach (ConsReportColoringElement color in coloring.Colors)
                {
                    if (utilization >= color.MinValue &&
                            utilization <= color.MaxValue)
                        return color.ItemColor;
                }
            }
            else
            {
                if (utilization == 0)
                {
                    return Color.White;
                }
                foreach (ConsReportColoringElement color in coloring.InvestmentResourceColors)
                {
                    int resultant = utilization - targetUtil;
                    if (resultant >= color.MinValue &&
                            resultant <= color.MaxValue && color.MaxValue != 0)
                        return color.ItemColor;
                }

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
        public static Color GetColorByCapacity(int capacity, int dayType, bool isHiredIntheEmployeementRange, bool isWeekEnd, int targUtil = -1)
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

            if (targUtil != -1)
            {
                int targCapacity = 100 - targUtil;
                int threshold = capacity - targCapacity;
                if (capacity == 100)
                    return Color.White;
                else if (threshold >= 1 && threshold <= 10)
                    return Color.FromArgb(255, 255, 0);
                else if (threshold >= 11)
                    return Color.FromArgb(255, 0, 0);
                else if (threshold >= -10 && threshold <= 0)
                    return Color.FromArgb(82, 178, 0);
                else if (threshold <= -11)
                    return Color.FromArgb(51, 204, 255);
            }
            else
            {
                if (capacity >= 100 && capacity <= 100)
                {
                    return Color.FromArgb(255, 255, 255);
                }
                else if (capacity >= 75 && capacity <= 99)
                {
                    return Color.FromArgb(255, 0, 0);
                }
                else if (capacity >= 25 && capacity <= 74)
                {
                    return Color.FromArgb(255, 165, 0);
                }
                else if (capacity >= 10 && capacity <= 24)
                {
                    return Color.FromArgb(255, 255, 0);
                }
                else if (capacity >= 0 && capacity <= 9)
                {
                    return Color.FromArgb(82, 178, 0);
                }
                else if (capacity <= -1)
                {
                    return Color.FromArgb(51, 204, 255);
                }
            }

            //  Return default color if nothing was foung in config
            return DEFAULT_COLOR;
        }

        public static void CapacityColorLegends(Legend legend, bool isInvestment)
        {
            //  Clear legend items first
            LegendItemsCollection legendItems = legend.CustomItems;
            legendItems.Clear();
            ConsReportColoringElementSection coloring =
               ConsReportColoringElementSection.ColorSettings;
            //  Iterate through all colors and put them on legend
            if (legend.Name == "InvestmentResourcesLegend" || isInvestment)
            {
                legendItems.Add(Color.FromArgb(255, 255, 255), "Capacity = 100%");
                legendItems.Add(coloring.VacationColor, coloring.VacationTitle);
                legendItems.Add(Color.FromArgb(255, 0, 0), "11 or more points above target");

                legendItems.Add(Color.FromArgb(255, 255, 0), "1-10 point above target");
                legendItems.Add(Color.FromArgb(82, 178, 0), "0-10 points below target");
                legendItems.Add(Color.FromArgb(51, 204, 255), "11 or more points below target");
                legendItems.Add(coloring.CompanyHolidayColor, coloring.CompanyHolidaysTitle);
            }
            else
            {
                legendItems.Add(Color.FromArgb(255, 255, 255), "Capacity = 100%");
                legendItems.Add(coloring.VacationColor, coloring.VacationTitle);
                legendItems.Add(Color.FromArgb(255, 0, 0), "Capacity = 99 - 75%");
                legendItems.Add(Color.FromArgb(255, 165, 0), "Capacity = 74 - 25%");
                legendItems.Add(Color.FromArgb(255, 255, 0), "Capacity = 24 - 10%");
                legendItems.Add(Color.FromArgb(82, 178, 0), "Capacity = 9 - 0%");
                legendItems.Add(Color.FromArgb(51, 204, 255), "Capacity = (-1)+%");

                legendItems.Add(coloring.CompanyHolidayColor, coloring.CompanyHolidaysTitle);
            }


            //  Add vacation item

            // Add company holiday item

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
        public static void ColorLegend(Legend legend, bool includeBadgeStatus, bool ispdf, bool investmentLegend = false)
        {
            //  Clear legend items first
            LegendItemsCollection legendItems = legend.CustomItems;
            legendItems.Clear();

            //  Iterate through all colors and put them on legend
            ConsReportColoringElementSection coloring =
                    ConsReportColoringElementSection.ColorSettings;

            if (legend.Name == "InvestmentResourcesLegend" || investmentLegend)
            {
                investmentLegend = true;
                if (includeBadgeStatus)
                {
                    foreach (ConsReportColoringElement color in coloring.InvestmentResourceColors)
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
                    legendItems.Add(coloring.VacationColor, coloring.VacationTitle);
                    legendItems.Add(coloring.CompanyHolidayColor, coloring.CompanyHolidaysTitle);
                }
                else
                {
                    legendItems.Add(Color.FromArgb(255, 255, 255), "Utilization = 0%");
                    legendItems.Add(coloring.VacationColor, coloring.VacationTitle);
                    legendItems.Add(Color.FromArgb(255, 0, 0), "11 or more points below target");
                    legendItems.Add(Color.FromArgb(255, 255, 0), "1-10 point below target");
                    legendItems.Add(Color.FromArgb(82, 178, 0), "0-10 points above target");
                    legendItems.Add(coloring.CompanyHolidayColor, coloring.CompanyHolidaysTitle);
                    legendItems.Add(Color.FromArgb(51, 204, 255), "11 or more points above target");
                }
                
            }
            else
            {
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
                legendItems.Add(coloring.VacationColor, coloring.VacationTitle);
                legendItems.Add(coloring.CompanyHolidayColor, coloring.CompanyHolidaysTitle);
            }

           

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

                //Managed Service
                var legItem4 = new LegendItem();
                legItem4.Color = Color.White;
                legItem4.ImageStyle = LegendImageStyle.Rectangle;
                legItem4.BackHatchStyle = ChartHatchStyle.DiagonalBrick;
                legItem4.BackSecondaryColor = Color.Black;
                legItem4.Name = "Managed Service";
                legItem4.MarkerStyle = MarkerStyle.Square;
                legItem4.MarkerSize = 50;
                legItem4.MarkerColor = Color.Black;
                legendItems.Add(legItem4);
            }
           
            Order(legendItems, includeBadgeStatus, ispdf,investmentLegend);
            
        }

        private static void Order(LegendItemsCollection legendcollection, bool includeBadge, bool ispdf, bool investment)
        {
            LegendItemsCollection temp = legendcollection;
            List<LegendItem> x = new List<LegendItem>();
            for (int i = 0; i < temp.Count; i++)
            {
                x.Add(temp[i]);
            }
            legendcollection.Clear();
            if (!ispdf)
            {
                if (!includeBadge)
                {
                    legendcollection.Add(x[0]);
                    if (x.Count > 7)
                    {
                        legendcollection.Add(x[7]);
                    }
                    legendcollection.Add(x[1]);
                    if (x.Count == 9)
                    {
                        legendcollection.Add(x[8]);
                    }
                    legendcollection.Add(x[2]);
                    legendcollection.Add(x[6]);
                    legendcollection.Add(x[3]);
                    legendcollection.Add(x[4]);
                    legendcollection.Add(x[5]);
                    if (x.Count > 9)
                    {
                        for (int i = 9; i < x.Count; i++)
                        {
                            legendcollection.Add(x[i]);
                        }
                    }
                }
                else
                {
                    legendcollection.Add(x[0]);
                    legendcollection.Add(x[6]);
                    if (x.Count == 13)
                    {
                        legendcollection.Add(x[12]);
                    }
                    legendcollection.Add(x[1]);
                    legendcollection.Add(x[7]);
                    legendcollection.Add(x[2]);
                    legendcollection.Add(x[8]);
                    legendcollection.Add(x[3]);
                    legendcollection.Add(x[9]);
                    legendcollection.Add(x[4]);
                    legendcollection.Add(x[10]);
                    legendcollection.Add(x[5]);
                    if (x.Count == 13)
                    {
                        legendcollection.Add(x[11]);
                    }

                }
            }
            else  //pdf
            {
                if (!investment)
                {
                    if (!includeBadge)
                    {
                        legendcollection.Add(x[0]);
                        legendcollection.Add(x[5]);
                        legendcollection.Add(x[1]);
                        legendcollection.Add(x[6]);
                        legendcollection.Add(x[2]);
                        if (x.Count == 9)
                        {
                            legendcollection.Add(x[8]);
                        }
                        legendcollection.Add(x[3]);
                        if (x.Count > 7)
                        {
                            legendcollection.Add(x[7]);
                        }
                        legendcollection.Add(x[4]);
                        if (x.Count > 9)
                        {
                            for (int i = 9; i < x.Count; i++)
                            {
                                legendcollection.Add(x[i]);
                            }
                        }
                    }
                    else
                    {
                        legendcollection.Add(x[0]);
                        legendcollection.Add(x[5]);
                        legendcollection.Add(x[10]);
                        legendcollection.Add(x[1]);
                        legendcollection.Add(x[6]);
                        if (x.Count == 13)
                        {
                            legendcollection.Add(x[11]);
                        }
                        legendcollection.Add(x[2]);
                        legendcollection.Add(x[7]);
                        if (x.Count == 13)
                        {
                            legendcollection.Add(x[12]);
                        }
                        legendcollection.Add(x[3]);
                        legendcollection.Add(x[8]);
                        legendcollection.Add(x[4]);
                        legendcollection.Add(x[9]);
                    }
                }
                else {
                    if (!includeBadge)
                    {
                        legendcollection.Add(x[0]);
                        legendcollection.Add(x[1]);
                        legendcollection.Add(x[2]);
                        legendcollection.Add(x[5]);
                        legendcollection.Add(x[3]);
                        legendcollection.Add(x[4]);
                        legendcollection.Add(x[6]);
                    }
                    else {
                        
                        legendcollection.Add(x[0]);
                        legendcollection.Add(x[3]);
                        legendcollection.Add(x[8]);
                        legendcollection.Add(x[1]);
                        legendcollection.Add(x[5]);
                        legendcollection.Add(x[10]);
                        legendcollection.Add(x[2]);
                        legendcollection.Add(x[6]);
                        legendcollection.Add(x[7]);
                        legendcollection.Add(x[4]);
                        legendcollection.Add(x[9]);
                    }
                }
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

