using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.Controls.Opportunities;

namespace PraticeManagement.Utils
{
    public static class OpportunitiesHelper
    {
        private static readonly IDictionary<string, string> _opportunityStatuses
           = new Dictionary<string, string>()
                {
                    {"Active", "AciveOpportunity"},
                    {"Won", "WonOpportunity"},
                    {"Lost", "LostOpportunity"},
                    {"Experimental", "ExperimentalOpportunity"},
                    {"Inactive", "InactiveOpportunity"}
                };

        public static string GetIndicatorClassByStatus(string statusName)
        {
            return _opportunityStatuses[statusName];
        }



        #region Summary

        #region Constants

        private const string PercentageSummaryFormat = "{0} = {1} ({2}%)";
        private const string ExportPercentageSummaryFormat = "{0} = {1} {2}";
        private const string PercentageFormat = "&nbsp;({0}%)";
        private const string NameFormat = "{0} =";
        private const string ExcelSummaryValuesFormat = "&nbsp; {0}";
        private const string ExcelValueFormat = "&nbsp; {0} = {1}";
        private const string CurrencyDisplayFormat = "$###,###,###,###,###,##0";
        private const string ConstantSpace = "&nbsp;&nbsp;&nbsp;&nbsp;";
        private const string ViewStateOpportunityList = "ViewStateOppList";
        private const string ViewStatePriorityTrendList = "ViewStatePriorityTrendList";
        private const string ViewStateStatusChangesList = "ViewStateStatusChangesList";
        private const string UpKey = "Up";
        private const string DownKey = "Down";
        private const string ActiveKey = "Active";
        private const string InactiveKey = "Inactive";
        private const string LostKey = "Lost";
        private const string WonKey = "Won";
        private const string ExcelDateFormat = "mso-number-format";
        private const string ExcelDateFormatStyle = "mm-dd-yyyy";
        private const string BoldFormat = "&nbsp;<span style=\" font-weight:bold;\"> {0} </span>";

        private const int days = 7;

        #endregion Constants

        #region Properties

        private static Opportunity[] OpportunitiesList { get; set; }

        private static Dictionary<string, int> PriorityTrendList { get; set; }

        private static Dictionary<string, int> StatusChangesList { get; set; }

        private static bool? IsExporting { get; set; }

        #endregion

        #region Methods

        #region Export

        public static Table GetFormatedSummaryDetails(Opportunity[] opportunityList, Dictionary<string, int> priorityTrendList, Dictionary<string, int> statusChangesList, bool isExporting = false)
        {
            IsExporting = isExporting;
            OpportunitiesList = opportunityList;
            PriorityTrendList = priorityTrendList;
            StatusChangesList = statusChangesList;
            return ExportSummaryDetails(OpportunitiesList);
        }

        private static Table ExportSummaryDetails(Opportunity[] opportunityList)
        {
            int totalOpportunities = opportunityList.Count();
            int uniqueClientsCount = opportunityList.Select(opp => opp.Client.Id).Distinct().Count();
            decimal? totalEstimateRevenue = opportunityList.Sum(opp => opp.EstimatedRevenue);

            Table summaryTable = new Table();
            TableRow tableRow = new TableRow();
            TableCell col1 = new TableCell();
            TableCell col2 = new TableCell();
            TableCell col3 = new TableCell();
            TableCell col4 = new TableCell();
            TableCell col5 = new TableCell();

            col1.VerticalAlign = col2.VerticalAlign = col3.VerticalAlign = col4.VerticalAlign = col5.VerticalAlign = VerticalAlign.Top;

            col1.Style["padding-left"] = "5px";
            col1.Style["padding-right"] = col2.Style["padding-right"] = col3.Style["padding-right"] = col4.Style["padding-right"] = col5.Style["padding-right"] = "10px";

            var col1data = ExportSummaryColumn1(opportunityList, totalOpportunities, uniqueClientsCount);

            var col2data = ExportSummaryColumn2(opportunityList, totalOpportunities);

            var col3data = ExportSummaryColumn3(opportunityList);

            var col4data = ExportSummaryColumn4(opportunityList, totalEstimateRevenue.Value);

            var col5data = ExportSummaryColumn5(opportunityList);

            col1.Controls.Add(col1data);
            col2.Controls.Add(col2data);
            col3.Controls.Add(col3data);
            col4.Controls.Add(col4data);
            col5.Controls.Add(col5data);
            tableRow.Controls.Add(col1);
            tableRow.Controls.Add(col2);
            tableRow.Controls.Add(col3);
            tableRow.Controls.Add(col4);
            tableRow.Controls.Add(col5);
            summaryTable.Controls.Add(tableRow);
            summaryTable.Width = Unit.Percentage(100);

            //return summaryTable;
            return summaryTable;
        }

        private static Table ExportSummaryColumn1(Opportunity[] opportunityList, int totalOpportunities, int uniqueClientsCount)
        {
            var data1 = AddTotalOpportuintiesSummaryCell(totalOpportunities);
            var data2 = AddUniqueClientsCell(uniqueClientsCount);
            var data3 = AddTopClientsCell(opportunityList, totalOpportunities);

            return ExportSummaryColumnWithMultipleRows(data1, data2, data3);
        }

        private static Table ExportSummaryColumn2(Opportunity[] opportunityList, int totalOpportunities)
        {
            var data1 = AddPrioritySummaryCell(opportunityList, totalOpportunities);
            var data2 = AddPriorityTrendingCell();

            return ExportSummaryColumnWithMultipleRows(data1, data2);
        }

        private static Table ExportSummaryColumn3(Opportunity[] opportunityList)
        {
            var data1 = AddOpportunityStatusChangesCell();

            Table data2 = null;
            if (IsExporting != null && IsExporting.Value == false)
                data2 = AddOpportunityPriorityAgingCell(opportunityList);
            else
            {
                data2 = AddOpportunityPriorityAgingCellForExport(opportunityList);
            }

            return ExportSummaryColumnWithMultipleRows(data1, data2);
        }

        private static Table ExportSummaryColumn4(Opportunity[] opportunityList, decimal totalEstimateRevenue)
        {
            var data1 = AddTotalEstimatedRevenueCell(opportunityList, totalEstimateRevenue);
            var data2 = AddEstimateRevenueByPriorityCell(opportunityList, totalEstimateRevenue);

            return ExportSummaryColumnWithMultipleRows(data1, data2);
        }

        private static Table ExportSummaryColumn5(Opportunity[] opportunityList)
        {
            var data1 = AddTotalEstimateRevenueByPractice(opportunityList);
            var table = ExportSummaryColumnWithMultipleRows(data1);
            table.Width = Unit.Percentage(100);
            return table;
        }

        private static Table ExportSummaryColumnWithMultipleRows(params Table[] data)
        {
            Table table = new Table();
            foreach (var item in data)
            {
                var cell = new TableCell();
                cell.Controls.Add(item);

                var row = new TableRow();
                row.Controls.Add(cell);

                table.Controls.Add(row);
            }
            return table;
        }

        private static void AddHeaderRow(string headerText, Table table, bool needToOccupyTwoCells = true)
        {
            var headerRow = new TableRow();
            var td = new TableCell();
            td.Text = headerText;
            td.Font.Bold = true;
            td.HorizontalAlign = HorizontalAlign.Left;
            if (needToOccupyTwoCells)
            {
                td.ColumnSpan = 2;
            }

            headerRow.Controls.Add(td);

            table.Controls.Add(headerRow);
        }

        private static void AddDataRowByKeyValuePair(Dictionary<string, int> keyValuePair, string key, Table table, string key2 = null)
        {
            var dataRow = new TableRow();
            var dataCell = new TableCell();
            dataCell.HorizontalAlign = (IsExporting.HasValue && IsExporting.Value) ? HorizontalAlign.Left : HorizontalAlign.Right;
            dataCell.Font.Bold = false;

            if (key2 != null)
            {
                int count = keyValuePair.ContainsKey(key) ? keyValuePair[key] : 0;
                count = count + (keyValuePair.ContainsKey(key2) ? keyValuePair[key2] : 0);
                if (IsExporting.HasValue && IsExporting.Value)
                {
                    dataCell.Text = string.Format(ExcelValueFormat, key2 + "/" + key, count);
                }
                else
                {
                    AddNameCell(key2 + "/" + key, dataRow);
                    dataCell.Text = string.Format(ExcelSummaryValuesFormat,  count);
                }
            }
            else
            {
                if (IsExporting.HasValue && IsExporting.Value)
                {
                    dataCell.Text = string.Format(ExcelValueFormat, key, keyValuePair.ContainsKey(key) ? keyValuePair[key] : 0);
                }
                else
                {
                    AddNameCell(key, dataRow);
                    dataCell.Text = string.Format(ExcelSummaryValuesFormat, keyValuePair.ContainsKey(key) ? keyValuePair[key].ToString() : "0");
                }
            }

            dataRow.Controls.Add(dataCell);
            table.Controls.Add(dataRow);
        }

        private static void AddNameCell(string name, TableRow row, bool needEqualToSymbol = true)
        {
            var nameCell = new TableCell();
            nameCell.HorizontalAlign = HorizontalAlign.Right;
            nameCell.VerticalAlign = VerticalAlign.Top;
            nameCell.Font.Bold = false;
            //nameCell.Wrap = true;
            //nameCell.Style.Add("white-space", "normal !important;");
            //nameCell.Style.Add("word-wrap", "break-word");

            if (!(IsExporting.HasValue && IsExporting.Value))
            {
                string trunckatedName = name;
                if (trunckatedName.Length > 30)
                {
                    string value = string.Empty;
                    for (int i = 29; i < trunckatedName.Length; i = i + 30)
                    {
                        value = trunckatedName.Insert(i, "<br/>");
                    }
                    if (!string.IsNullOrEmpty(value))
                    {
                        name = value;
                    }
                }
            }
            
            nameCell.Text = needEqualToSymbol ? string.Format(NameFormat, name) : name;

            row.Controls.Add(nameCell);
        }

        private static void AddEmptyDataRow(Table table)
        {
            var emptyRow = new TableRow();
            var emptyCell = new TableCell();
            emptyCell.Text = "&nbsp;";
            emptyRow.Controls.Add(emptyCell);

            table.Controls.Add(emptyRow);
        }

        private static Table AddTotalOpportuintiesSummaryCell(int totalOpportunities)
        {
            Table totalOpp = new Table();

            AddHeaderRow("Total Opportunities", totalOpp);

            AddDataRowWithTwoCells(totalOpportunities.ToString(), totalOpp);

            AddEmptyDataRow(totalOpp);

            return totalOpp;
        }

        private static Table AddUniqueClientsCell(int uniqueClientsCount)
        {
            Table uniqueClients = new Table();
            AddHeaderRow("Unique Clients", uniqueClients);

            AddDataRowWithTwoCells(uniqueClientsCount.ToString(), uniqueClients);

            AddEmptyDataRow(uniqueClients);

            return uniqueClients;
        }

        private static Table AddTopClientsCell(Opportunity[] opportunityList, int totalOpportunities)
        {
            var list = (from o in opportunityList
                        group o by o.Client.Id into result
                        orderby result.Count() descending
                        select new
                        {
                            clientName = result.Select(c => c.Client.Name).First(),
                            OpportunityCount = result.Count(),
                            clientSummary = string.Format(PercentageFormat, (result.Count() * 100) / totalOpportunities),
                            clientEstimatedRevenue = result.Sum(c => c.EstimatedRevenue)
                        }
                        ).Take(3);

            Table topClientsTable = new Table();

            AddHeaderRow("Top 3 Clients", topClientsTable);
            
            foreach (var item in list)
            {
                if (IsExporting.HasValue && IsExporting.Value)
                {
                    AddDataRowWithTwoCells(item.clientName, item.OpportunityCount + item.clientSummary, topClientsTable, string.Format(BoldFormat, item.clientEstimatedRevenue.Value.ToString(CurrencyDisplayFormat) ), false);
                }
                else
                {
                    AddDataRowWithTwoCells(item.clientName, item.OpportunityCount + " " + item.clientSummary, topClientsTable, string.Format(BoldFormat, item.clientEstimatedRevenue.Value.ToString(CurrencyDisplayFormat) ), true);
                }
            }

            AddEmptyDataRow(topClientsTable);

            return topClientsTable;
        }

        private static Table AddPrioritySummaryCell(Opportunity[] prioritySummary, int totalOpportunities)
        {
            var calc = (from o in prioritySummary
                        where o.Status.Id == (int)OpportunityStatusType.Active
                        orderby o.Priority.SortOrder ascending
                        group o by o.Priority.Priority into result
                        select new
                        {
                            priorityName = result.Key,
                            priorityCount = result.Count(),
                            priorityPercentage = string.Format(PercentageFormat, (result.Count() * 100) / totalOpportunities)
                        }
                        );

            Table prioritySummaryTable = new Table();
            AddHeaderRow("Summary by Priority", prioritySummaryTable, !(IsExporting.HasValue && IsExporting.Value));

            foreach (var item in calc)
            {
                if (IsExporting.HasValue && IsExporting.Value)
                {
                    string value = string.Format(ExcelValueFormat, item.priorityName, item.priorityCount + item.priorityPercentage);

                    TableRow row = new TableRow();
                    TableCell cell = new TableCell();
                    cell.HorizontalAlign = HorizontalAlign.Left;
                    cell.Text = string.Format(ExcelSummaryValuesFormat, value);

                    row.Controls.Add(cell);
                    prioritySummaryTable.Controls.Add(row);
                    //AddDataRowWithTwoCells(value, prioritySummaryTable);
                }
                else
                {
                    AddDataRowWithTwoCells(item.priorityName, item.priorityCount.ToString(), prioritySummaryTable, item.priorityPercentage);
                }
            }

            AddEmptyDataRow(prioritySummaryTable);

            return prioritySummaryTable;
        }

        private static Table AddPriorityTrendingCell()
        {
            Table priorityTrendingTable = new Table();
            string headerText = string.Format("Priority Trending (last {0} days)", days);
            AddHeaderRow(headerText, priorityTrendingTable, !(IsExporting.HasValue && IsExporting.Value));


            TableRow dataRow = new TableRow();
            TableCell dataCell = new TableCell();
            dataCell.HorizontalAlign = HorizontalAlign.Justify;

            Table dataTable = new Table();
            dataTable.Width = Unit.Percentage(60);
            var priorityTrendList = PriorityTrendList;
            AddDataRowByKeyValuePair(priorityTrendList, UpKey, dataTable);
            AddDataRowByKeyValuePair(priorityTrendList, DownKey, dataTable);

            dataCell.Controls.Add(dataTable);
            dataRow.Controls.Add(dataCell);
            priorityTrendingTable.Controls.Add(dataRow);

            AddEmptyDataRow(priorityTrendingTable);

            return priorityTrendingTable;
        }

        private static Table AddOpportunityStatusChangesCell()
        {
            Table table = new Table();
            string headerText = string.Format("Opportunity Status Changes (last {0} days)", days);
            AddHeaderRow(headerText, table);

            TableRow dataRow = new TableRow();
            TableCell dataCell = new TableCell();

            Table dataTable = new Table();
            dataTable.Width = Unit.Percentage(50);
            var list = StatusChangesList;
            AddDataRowByKeyValuePair(list, ActiveKey, dataTable);
            AddDataRowByKeyValuePair(list, InactiveKey, dataTable, LostKey);//Note:- Showing Lost and Inactive count in one cell as per the requirement
            AddDataRowByKeyValuePair(list, WonKey, dataTable);

            dataCell.Controls.Add(dataTable);
            dataRow.Controls.Add(dataCell);
            table.Controls.Add(dataRow);

            AddEmptyDataRow(table);

            return table;
        }

        private static Table AddOpportunityPriorityAgingCell(Opportunity[] opportunityList)
        {
            Table tholder = new Table();
            var row = new TableRow();
            TableCell cell = new TableCell();

            Table table = new Table();
            table.CssClass = "CellPaddingClass";
            var headerRow = new TableRow();
            TableCell headerCell = new TableCell();
            headerCell.Text = "Opportunity Aging";
            headerCell.Font.Bold = true;
            headerCell.HorizontalAlign = HorizontalAlign.Left;
            headerCell.Style.Add("padding-left", "0px !important");



            TableRow age1 = new TableRow();
            TableRow age2 = new TableRow();
            TableRow age3 = new TableRow();

            TableCell tblCell1 = new TableCell();
            tblCell1.Text = "00-30 Days =";
            tblCell1.Font.Bold = false;
            tblCell1.HorizontalAlign = HorizontalAlign.Right;

            TableCell tblCell2 = new TableCell();
            tblCell2.Text = "31-60 Days =";
            tblCell2.Font.Bold = false;
            tblCell2.HorizontalAlign = HorizontalAlign.Right;

            TableCell tblCell3 = new TableCell();
            tblCell3.Text = "61-120+ Days =";
            tblCell3.Font.Bold = false;
            tblCell3.HorizontalAlign = HorizontalAlign.Right;

            headerRow.Controls.Add(headerCell);

            var priorityList = OpportunityPriorityHelper.GetOpportunityPriorities(true).OrderBy(p => p.SortOrder);
            var priorities = priorityList.Select(p => p.Priority).ToArray();
            var priorityOrderList = opportunityList.OrderBy(opp => opp.Priority.SortOrder).ToArray();

            foreach (var priorityName in priorities)
            {
                TableCell headerLabel = new TableCell();
                headerLabel.Font.Bold = true;
                headerLabel.HorizontalAlign = HorizontalAlign.Center;
                headerLabel.Text = priorityName;
                headerRow.Controls.Add(headerLabel);
            }

            age1.Controls.Add(tblCell1);
            age2.Controls.Add(tblCell2);
            age3.Controls.Add(tblCell3);

            FillOpportunityPriorityAgeCell(priorityOrderList, null, 30, priorities, age1);
            FillOpportunityPriorityAgeCell(priorityOrderList, 31, 60, priorities, age2);
            FillOpportunityPriorityAgeCell(priorityOrderList, 61, null, priorities, age3);

            table.Controls.Add(headerRow);
            table.Controls.Add(age1);
            table.Controls.Add(age2);
            table.Controls.Add(age3);
            AddEmptyDataRow(table);

            cell.Controls.Add(table);
            row.Controls.Add(cell);
            tholder.Controls.Add(row);
            return tholder;
        }

        private static void FillOpportunityPriorityAgeCell(Opportunity[] opportunityList, int? startAge, int? endAge, string[] opportunityPriorities, TableRow tr)
        {

            var list = opportunityList.Where(opp => (startAge.HasValue && DateTime.Now.Subtract(opp.CreateDate).Days >= startAge) && (endAge.HasValue && DateTime.Now.Subtract(opp.CreateDate).Days <= endAge)
                                                                                || (!startAge.HasValue && DateTime.Now.Subtract(opp.CreateDate).Days <= endAge)
                                                                                || (!endAge.HasValue && DateTime.Now.Subtract(opp.CreateDate).Days >= startAge)
                                                                        ).ToArray();
            var ageLessThan31List = (from o in list
                                     orderby o.Priority.SortOrder ascending
                                     group o by o.Priority.Priority into result
                                     select new
                                     {
                                         priority = result.Key,
                                         priorityCount = result.Count()
                                     }
                                        );

            foreach (var item in opportunityPriorities)
            {
                TableCell td = new TableCell();

                var ite = ageLessThan31List.Where(a => a.priority.ToString() == item);

                if (ite.Count() == 0)
                {
                    td.Text = ite.Count().ToString("#00");
                }
                else
                {
                    td.Text = ite.Select(a => a.priorityCount).First().ToString("#00");
                }
                td.HorizontalAlign = HorizontalAlign.Center;
                td.Font.Bold = false;
                tr.Controls.Add(td);
            }

        }

        private static Table AddOpportunityPriorityAgingCellForExport(Opportunity[] opportunityList)
        {
            Table table = new Table();
            var headerRow = new TableRow();
            TableCell headerCell = new TableCell();
            headerCell.Text = "Opportunity Aging";
            headerCell.Font.Bold = true;
            headerCell.HorizontalAlign = HorizontalAlign.Left;

            TableCell headerLabel = new TableCell();
            headerLabel.Font.Bold = true;

            headerLabel.HorizontalAlign = HorizontalAlign.Left;

            TableRow age1 = new TableRow();
            TableRow age2 = new TableRow();
            TableRow age3 = new TableRow();

            TableCell tblCell1 = new TableCell();
            tblCell1.Text = "&nbsp; 00-30 Days =";
            tblCell1.Font.Bold = false;
            tblCell1.HorizontalAlign = HorizontalAlign.Justify;

            TableCell tblCell2 = new TableCell();
            tblCell2.Text = "&nbsp; 31-60 Days =";
            tblCell2.Font.Bold = false;
            tblCell2.HorizontalAlign = HorizontalAlign.Justify;

            TableCell tblCell3 = new TableCell();
            tblCell3.Text = "&nbsp; 61-120+ Days =";
            tblCell3.Font.Bold = false;
            tblCell3.HorizontalAlign = HorizontalAlign.Justify;
            
            var priorityList = OpportunityPriorityHelper.GetOpportunityPriorities(true).OrderBy(p => p.SortOrder);
            var priorities = priorityList.Select(p => p.Priority).ToArray();
            var priorityOrderList = opportunityList.OrderBy(opp => opp.Priority.SortOrder).ToArray();
            foreach (var item in priorities)
            {
                headerLabel.Text = headerLabel.Text + ConstantSpace + (item.Count() < 2 ? "&nbsp;" + item : item);
            }

            TableCell age1Count = new TableCell();
            age1Count.Text = FillOpportunityPriorityAgeCellForExport(priorityOrderList, null, 30, priorities);
            age1Count.Font.Bold = false;
            TableCell age2Count = new TableCell();
            age2Count.Text = FillOpportunityPriorityAgeCellForExport(priorityOrderList, 31, 60, priorities);
            age2Count.Font.Bold = false;
            TableCell age3Count = new TableCell();
            age3Count.Text = FillOpportunityPriorityAgeCellForExport(priorityOrderList, 61, null, priorities);
            age3Count.Font.Bold = false;

            headerRow.Controls.Add(headerCell);
            headerRow.Controls.Add(headerLabel);
            age1.Controls.Add(tblCell1);
            age2.Controls.Add(tblCell2);
            age3.Controls.Add(tblCell3);
            age1.Controls.Add(age1Count);
            age2.Controls.Add(age2Count);
            age3.Controls.Add(age3Count);
            table.Controls.Add(headerRow);
            table.Controls.Add(age1);
            table.Controls.Add(age2);
            table.Controls.Add(age3);
            AddEmptyDataRow(table);

            return table;
        }

        private static string FillOpportunityPriorityAgeCellForExport(Opportunity[] opportunityList, int? startAge, int? endAge, string[] opportunityPriorities)
        {
            string cellText = string.Empty;
            var list = opportunityList.Where(opp => (startAge.HasValue && DateTime.Now.Subtract(opp.CreateDate).Days >= startAge) && (endAge.HasValue && DateTime.Now.Subtract(opp.CreateDate).Days <= endAge)
                                                                                || (!startAge.HasValue && DateTime.Now.Subtract(opp.CreateDate).Days <= endAge)
                                                                                || (!endAge.HasValue && DateTime.Now.Subtract(opp.CreateDate).Days >= startAge)
                                                                        ).ToArray();
            var ageLessThan31List = (from o in list
                                     orderby o.Priority.SortOrder ascending
                                     group o by o.Priority.Priority into result
                                     select new
                                     {
                                         priority = result.Key,
                                         priorityCount = result.Count()
                                     }
                                        );

            foreach (var item in opportunityPriorities)
            {
                var ite = ageLessThan31List.Where(a => a.priority.ToString() == item);

                if (ite.Count() == 0)
                {
                    cellText = cellText + ConstantSpace + ite.Count().ToString("#00");
                }
                else
                {
                    cellText = cellText + ConstantSpace + ite.Select(a => a.priorityCount).First().ToString("#00");
                }
            }
            return cellText;
        }
        
        private static Table AddTotalEstimatedRevenueCell(Opportunity[] opportunityList, decimal totalEstimateRevenue)
        {
            Table table = new Table();
            AddHeaderRow("Total Estimated Revenue", table);

            AddDataRowWithTwoCells(totalEstimateRevenue.ToString(CurrencyDisplayFormat), table);

            AddEmptyDataRow(table);

            return table;
        }

        private static void AddDataRowWithTwoCells(string value, Table table)
        {
            TableRow dataRow = new TableRow();
            TableCell cell = new TableCell();
            cell.HorizontalAlign = HorizontalAlign.Left;
            cell.Text = string.Format(ExcelSummaryValuesFormat, value);
            cell.ColumnSpan = 2;
            cell.Font.Bold = false;
            dataRow.Controls.Add(cell);

            table.Controls.Add(dataRow);
        }

        private static void AddDataRowWithTwoCells(string value1, string value2, Table table, string value3 = null, bool keepEqualToSymbolSeperately = false)
        {
            TableRow dataRow = new TableRow();
            AddNameCell(value1, dataRow, !keepEqualToSymbolSeperately);

            if (keepEqualToSymbolSeperately)
            {
                TableCell cell1 = new TableCell();
                cell1.Text = "&nbsp;=&nbsp;";
                cell1.VerticalAlign = VerticalAlign.Middle;
                dataRow.Controls.Add(cell1);
            }

            TableCell cell = new TableCell();
            cell.HorizontalAlign = HorizontalAlign.Right;
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.Text = value2;
            cell.Font.Bold = false;
            dataRow.Controls.Add(cell);

            if (!string.IsNullOrEmpty(value3))
            {
                TableCell cell2 = new TableCell();
                cell2.HorizontalAlign = (IsExporting.HasValue && IsExporting.Value) ? HorizontalAlign.Left : HorizontalAlign.Right;
                cell2.Font.Bold = false;
                cell2.Text = value3;
                cell2.VerticalAlign = VerticalAlign.Middle;

                dataRow.Controls.Add(cell2);
            }

            table.Controls.Add(dataRow);
        }

        private static Table AddEstimateRevenueByPriorityCell(Opportunity[] opportunityList, decimal totalEstimateRevenue)
        {
            Table table = new Table();
            AddHeaderRow("Est. Revenue by Priority", table);

            var list = (from o in opportunityList
                        orderby o.Priority.SortOrder ascending
                        group o by o.Priority.Priority into result
                        select new
                        {
                            priority = result.Key,
                            priorityEstimateRevenue = result.Sum(opp => opp.EstimatedRevenue)
                        }
                        );

            foreach (var item in list)
            {
                var percentage = (item.priorityEstimateRevenue * 100) / totalEstimateRevenue;
                string value = string.Format(PercentageSummaryFormat, item.priority, item.priorityEstimateRevenue.Value.ToString(CurrencyDisplayFormat), percentage.Value.ToString("0.0"));

                if (IsExporting.HasValue && IsExporting.Value)
                {
                    AddDataRowWithTwoCells(value, table);
                }
                else
                {
                    AddDataRowWithTwoCells(item.priority, item.priorityEstimateRevenue.Value.ToString(CurrencyDisplayFormat), table, string.Format(PercentageFormat, percentage.Value.ToString("0.0")));
                }
            }

            AddEmptyDataRow(table);

            return table;
        }

        private static Table AddTotalEstimateRevenueByPractice(Opportunity[] opportunityList)
        {
            Table table = new Table();
            table.Width = Unit.Percentage(100);
            AddHeaderRow("Total Estimated Revenue by Practice", table);

            var list = (from o in opportunityList
                        orderby o.Practice.Name ascending
                        group o by new { o.Practice.Id, o.Practice.Name } into result
                        select new
                        {
                            practice = result.Key,
                            practiceEstimateRevenue = result.Sum(opp => opp.EstimatedRevenue)
                        }
                        );

            foreach (var item in list)
            {
                AddDataRowWithThreeCells(item.practice.Name, string.Format(ExcelSummaryValuesFormat, item.practiceEstimateRevenue.Value.ToString(CurrencyDisplayFormat)), table);
            }

            AddEmptyDataRow(table);

            return table;
        }
        
        private static void AddDataRowWithThreeCells(string name, string value, Table table)
        {
            var dataRow = new TableRow();
            var cell1 = new TableCell();
            var cell2 = new TableCell();
            cell1.ColumnSpan = 2;
            cell1.Font.Bold = false;

            cell1.Text = name;
            dataRow.Controls.Add(cell1);

            cell2.Text = value;
            cell2.Font.Bold = false;
            cell2.HorizontalAlign = HorizontalAlign.Right;
            dataRow.Controls.Add(cell2);
            table.Controls.Add(dataRow);
        }

        #endregion

        #endregion

        #endregion
    }
}

