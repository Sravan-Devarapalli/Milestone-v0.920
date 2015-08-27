using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using DataTransferObjects.Reports.ConsultingDemand;
using PraticeManagement.Reports;
using System.Linq;
using PraticeManagement.Utils.Excel;
using PraticeManagement.Utils;
using System.Data;

namespace PraticeManagement.Controls.Reports.ConsultantDemand
{
    public partial class ConsultingDemandTReportByTitle : System.Web.UI.UserControl
    {
        private string ConsultantDetailReportExport = "Consultant Detail Report Export";
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;

        private PraticeManagement.Reports.ConsultingDemand_New HostingPage
        {
            get { return ((PraticeManagement.Reports.ConsultingDemand_New)Page); }
        }

        private SheetStyles HeaderSheetStyle
        {
            get
            {
                CellStyles cellStyle = new CellStyles();
                cellStyle.IsBold = true;
                cellStyle.BorderStyle = NPOI.SS.UserModel.BorderStyle.None;
                cellStyle.FontHeight = 350;
                CellStyles[] cellStylearray = { cellStyle };
                RowStyles headerrowStyle = new RowStyles(cellStylearray);
                headerrowStyle.Height = 500;

                CellStyles dataCellStyle = new CellStyles();
                CellStyles[] dataCellStylearray = { dataCellStyle };
                RowStyles datarowStyle = new RowStyles(dataCellStylearray);

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };

                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.MergeRegion.Add(new int[] { 0, 0, 0, coloumnsCount - 1 });
                sheetStyle.IsAutoResize = false;

                return sheetStyle;
            }
        }

        private SheetStyles DataSheetStyle
        {
            get
            {
                CellStyles headerCellStyle = new CellStyles();
                headerCellStyle.IsBold = true;
                headerCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                List<CellStyles> headerCellStyleList = new List<CellStyles>();
                headerCellStyleList.Add(headerCellStyle);
                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataCellStyle = new CellStyles();

                CellStyles dataDateCellStyle = new CellStyles();
                dataDateCellStyle.DataFormat = "mm/dd/yy;@";

                CellStyles[] dataCellStylearray = { dataCellStyle,
                                                    dataCellStyle, 
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataDateCellStyle
                                                  };

                RowStyles datarowStyle = new RowStyles(dataCellStylearray);

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.TopRowNo = headerRowsCount;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanColSplit = 0;
                sheetStyle.FreezePanRowSplit = headerRowsCount;

                return sheetStyle;
            }
        }

        private List<CollapsiblePanelExtender> CollapsiblePanelDateExtenderList
        {
            get;
            set;
        }

        public HiddenField _hdIsGraphPage
        {
            get
            {
                return hdIsGraphPage;
            }
        }

        private string sortColumn_Key = "sortColumn_Key";

        private string sortOrder_Key = "sortOrder_Key";

        public bool sortAscend
        {
            get
            {
                if (ViewState[sortOrder_Key] == null)
                {
                    ViewState[sortOrder_Key] = true;
                }
                return (bool)ViewState[sortOrder_Key];
            }
            set
            {
                ViewState[sortOrder_Key] = value;
            }
        }

        public string sortColumn
        {
            get
            {
                if (ViewState[sortColumn_Key] == null)
                {
                    ViewState[sortColumn_Key] = "";
                }
                return (string)ViewState[sortColumn_Key];
            }
            set
            {
                ViewState[sortColumn_Key] = value;
            }
        }

        public string BtnExportSelectedStartDate
        {
            set { btnExportToExcel.Attributes["startDate"] = value; }
            get
            {
                return btnExportToExcel.Attributes["startDate"];
            }
        }

        public string BtnExportSelectedEndDate
        {
            set { btnExportToExcel.Attributes["endDate"] = value; }
            get
            {
                return btnExportToExcel.Attributes["endDate"];
            }
        }

        public string BtnExportPipeLineSelectedValue
        {
            set { btnExportToExcel.Attributes["selectedValue"] = value; }
            get
            {
                return btnExportToExcel.Attributes["selectedValue"];
            }
        }

        public DateTime StartDate
        {
            get
            {
                if (HostingPage.GraphType == "TransactionSkill" || HostingPage.GraphType == "TransactionTitle")
                    return Convert.ToDateTime(BtnExportSelectedStartDate);
                else
                    return HostingPage.StartDate.Value;
            }
        }

        public DateTime EndDate
        {
            get
            {
                if (HostingPage.GraphType == "TransactionSkill" || HostingPage.GraphType == "TransactionTitle")
                    return Convert.ToDateTime(BtnExportSelectedEndDate);
                else
                    return HostingPage.EndDate.Value;
            }
        }

        private List<string> CollapsiblePanelDateExtenderClientIds
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            btnExpandOrCollapseAll.Attributes["onclick"] = "return CollapseOrExpandAll(" + btnExpandOrCollapseAll.ClientID +
                                                           ", " + hdnCollapsed.ClientID +
                                                           ", " + hdncpeExtendersIds.ClientID +
                                                           ");";

            btnExpandOrCollapseAll.Text = btnExpandOrCollapseAll.ToolTip = (hdnCollapsed.Value.ToLower() == "true") ? "Expand All" : "Collapse All";
        }

        private void showPopUP()
        {
            if (!string.IsNullOrEmpty(_hdIsGraphPage.Value) && _hdIsGraphPage.Value == true.ToString())
            {
                var hostingPage = (PraticeManagement.Reports.ConsultingDemand_New)Page;
                hostingPage.GraphControl.ConsultantDetailPopup.Show();
            }
        }

        protected void btnTitleOrSkill1_Command(object sender, CommandEventArgs e)
        {
            showPopUP();
            sortColumn = e.CommandArgument.ToString();
            if (HostingPage.GraphType == ConsultingDemand_New.TransactionTitle)
            {
                PopulateData(false, sortAscend ? Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.Title);
            }
            else
            {
                PopulateData(false, sortAscend ? Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.Skill);
            }
            sortAscend = !sortAscend;

        }

        protected void btnTitleOrSkill2_Command(object sender, CommandEventArgs e)
        {
            showPopUP();
            sortColumn = e.CommandArgument.ToString();
            if (HostingPage.GraphType == ConsultingDemand_New.TransactionTitle)
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title : Constants.ConsultingDemandSortColumnNames.Skill + "," + Constants.ConsultingDemandSortColumnNames.Title);
            }
            else
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill : Constants.ConsultingDemandSortColumnNames.Title + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            }
            sortAscend = !sortAscend;
        }

        protected void btnOpportunityNumber_Command(object sender, CommandEventArgs e)
        {
            showPopUP();
            sortColumn = e.CommandArgument.ToString();
            if (HostingPage.GraphType == ConsultingDemand_New.TransactionTitle)
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.OpportunityNumber + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.OpportunityNumber + "," + Constants.ConsultingDemandSortColumnNames.Title);
            }
            else
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.OpportunityNumber + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.OpportunityNumber + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            }
            sortAscend = !sortAscend;
        }

        protected void btnSalesStage_Command(object sender, CommandEventArgs e)
        {
            showPopUP();
            sortColumn = e.CommandArgument.ToString();
            if (HostingPage.GraphType == ConsultingDemand_New.TransactionTitle)
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.SalesStage + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.SalesStage + "," + Constants.ConsultingDemandSortColumnNames.Title);
            }
            else
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.SalesStage + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.SalesStage + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            }
            sortAscend = !sortAscend;
        }

        protected void btnProjectNumber_Command(object sender, CommandEventArgs e)
        {
            showPopUP();
            sortColumn = e.CommandArgument.ToString();
            if (HostingPage.GraphType == ConsultingDemand_New.TransactionTitle)
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.ProjectNumber + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.ProjectNumber + "," + Constants.ConsultingDemandSortColumnNames.Title);
            }
            else
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.ProjectNumber + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.ProjectNumber + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            }
            sortAscend = !sortAscend;
        }

        protected void btnAccountName_Command(object sender, CommandEventArgs e)
        {
            showPopUP();
            sortColumn = e.CommandArgument.ToString();
            if (HostingPage.GraphType == ConsultingDemand_New.TransactionTitle)
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.AccountName + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.AccountName + "," + Constants.ConsultingDemandSortColumnNames.Title);
            }
            else
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.AccountName + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.AccountName + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            }
            sortAscend = !sortAscend;
        }

        protected void btnProjectName_Command(object sender, CommandEventArgs e)
        {
            showPopUP();
            sortColumn = e.CommandArgument.ToString();
            if (HostingPage.GraphType == ConsultingDemand_New.TransactionTitle)
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.ProjectName + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.ProjectName + "," + Constants.ConsultingDemandSortColumnNames.Title);
            }
            else
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.ProjectName + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.ProjectName + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            }
            sortAscend = !sortAscend;
        }

        protected void btnResourceStartDate_Command(object sender, CommandEventArgs e)
        {
            showPopUP();
            sortColumn = e.CommandArgument.ToString();
            if (HostingPage.GraphType == ConsultingDemand_New.TransactionTitle)
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.ResourceStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.ResourceStartDate + "," + Constants.ConsultingDemandSortColumnNames.Title);
            }
            else
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.ResourceStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.ResourceStartDate + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            }
            sortAscend = !sortAscend;
        }

        protected void btnMonth_Command(object sender, CommandEventArgs e)
        {
            showPopUP();
            sortColumn = e.CommandArgument.ToString();
            if (HostingPage.GraphType == ConsultingDemand_New.PipelineTitle)
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.MonthStartDate + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            }
            else
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.MonthStartDate + "," + Constants.ConsultingDemandSortColumnNames.Title);
            }
            sortAscend = !sortAscend;
        }

        protected void btnTitleSkill_Command(object sender, CommandEventArgs e)
        {
            showPopUP();
            sortColumn = e.CommandArgument.ToString();
            if (HostingPage.GraphType == ConsultingDemand_New.PipelineTitle)
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.Skill + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate);
            }
            else
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.Title + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate);
            }
            sortAscend = !sortAscend;
        }

        protected void btnMonthOpportunityNumber_Command(object sender, CommandEventArgs e)
        {
            showPopUP();
            sortColumn = e.CommandArgument.ToString();
            if (HostingPage.GraphType == ConsultingDemand_New.PipelineTitle)
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.OpportunityNumber + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.OpportunityNumber + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            }
            else
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.OpportunityNumber + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.OpportunityNumber + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + "," + Constants.ConsultingDemandSortColumnNames.Title);
            }
            sortAscend = !sortAscend;
        }

        protected void btnMonthSalesStage_Command(object sender, CommandEventArgs e)
        {
            showPopUP();
            sortColumn = e.CommandArgument.ToString();
            if (HostingPage.GraphType == ConsultingDemand_New.PipelineTitle)
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.SalesStage + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.SalesStage + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            }
            else
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.SalesStage + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.SalesStage + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + "," + Constants.ConsultingDemandSortColumnNames.Title);
            }
            sortAscend = !sortAscend;
        }

        protected void btnMonthProjectNumber_Command(object sender, CommandEventArgs e)
        {
            showPopUP();
            sortColumn = e.CommandArgument.ToString();
            if (HostingPage.GraphType == ConsultingDemand_New.PipelineTitle)
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.ProjectNumber + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.ProjectNumber + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            }
            else
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.ProjectNumber + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.ProjectNumber + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + "," + Constants.ConsultingDemandSortColumnNames.Title);
            }
            sortAscend = !sortAscend;
        }

        protected void btnMonthAccountName_Command(object sender, CommandEventArgs e)
        {
            showPopUP();
            sortColumn = e.CommandArgument.ToString();
            if (HostingPage.GraphType == ConsultingDemand_New.PipelineTitle)
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.AccountName + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.AccountName + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            }
            else
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.AccountName + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.AccountName + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + "," + Constants.ConsultingDemandSortColumnNames.Title);
            }
            sortAscend = !sortAscend;
        }

        protected void btnMonthProjectName_Command(object sender, CommandEventArgs e)
        {
            showPopUP();
            sortColumn = e.CommandArgument.ToString();
            if (HostingPage.GraphType == ConsultingDemand_New.PipelineTitle)
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.ProjectName + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.ProjectName + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            }
            else
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.ProjectName + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.ProjectName + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + "," + Constants.ConsultingDemandSortColumnNames.Title);
            }
            sortAscend = !sortAscend;
        }

        protected void btnMonthResourceStartDate_Command(object sender, CommandEventArgs e)
        {
            showPopUP();
            sortColumn = e.CommandArgument.ToString();
            if (HostingPage.GraphType == ConsultingDemand_New.PipelineTitle)
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.ResourceStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Skill + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.ResourceStartDate + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + "," + Constants.ConsultingDemandSortColumnNames.Skill);
            }
            else
            {
                PopulateData(false, e.CommandArgument.ToString() == sortColumn && sortAscend ? Constants.ConsultingDemandSortColumnNames.ResourceStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder + "," + Constants.ConsultingDemandSortColumnNames.Title + " " + Constants.ConsultingDemandSortColumnNames.SortDescendingOrder : Constants.ConsultingDemandSortColumnNames.ResourceStartDate + "," + Constants.ConsultingDemandSortColumnNames.MonthStartDate + "," + Constants.ConsultingDemandSortColumnNames.Title);
            }
            sortAscend = !sortAscend;
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            var dataSetList = new List<DataSet>();
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            List<ConsultantGroupbySkill> PopUpFilteredSkill = null;
            List<ConsultantGroupbyTitle> PopUpFilteredTitle = null;
            List<ConsultantGroupByMonth> PopUpFilteredByMonth = null;
            DataHelper.InsertExportActivityLogMessage(ConsultantDetailReportExport);
            if (HostingPage.GraphType == ConsultingDemand_New.TransactionTitle)
            {
                string titles = HostingPage.isSelectAllTitles ? null : HostingPage.hdnTitlesProp;
                PopUpFilteredTitle = ServiceCallers.Custom.Report(r => r.ConsultingDemandTransactionReportByTitle(StartDate, EndDate, titles, "", HostingPage.isSelectAllSalesStages ? null : HostingPage.hdnSalesStagesProp)).ToList();
            }
            else if (HostingPage.GraphType == ConsultingDemand_New.TransactionSkill)
            {
                string skills = HostingPage.isSelectAllSkills ? null : HostingPage.hdnSkillsProp;
                PopUpFilteredSkill = ServiceCallers.Custom.Report(r => r.ConsultingDemandTransactionReportBySkill(StartDate, EndDate, skills, "", HostingPage.isSelectAllSalesStages ? null : HostingPage.hdnSalesStagesProp)).ToList();
            }
            else if (HostingPage.GraphType == ConsultingDemand_New.PipelineTitle)
            {
                PopUpFilteredByMonth = ServiceCallers.Custom.Report(r => r.ConsultingDemandDetailsByMonth(StartDate, EndDate, BtnExportPipeLineSelectedValue, null, HostingPage.isSelectAllSalesStages ? null : HostingPage.hdnSalesStagesProp, "", true)).ToList();
            }
            else if (HostingPage.GraphType == ConsultingDemand_New.PipelineSkill)
            {
                PopUpFilteredByMonth = ServiceCallers.Custom.Report(r => r.ConsultingDemandDetailsByMonth(StartDate, EndDate, null, BtnExportPipeLineSelectedValue, HostingPage.isSelectAllSalesStages ? null : HostingPage.hdnSalesStagesProp, "", true)).ToList();
            }
            if ((PopUpFilteredTitle != null && PopUpFilteredTitle.Count > 0) || (PopUpFilteredSkill != null && PopUpFilteredSkill.Count > 0) || (PopUpFilteredByMonth != null && PopUpFilteredByMonth.Count > 0))
            {
                string dateRangeTitle = string.Format("Period: {0} to {1}", StartDate.ToString(Constants.Formatting.EntryDateFormat), EndDate.ToString(Constants.Formatting.EntryDateFormat));
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                headerRowsCount = header.Rows.Count + 3;
                var data = HostingPage.GraphType == ConsultingDemand_New.TransactionSkill ? PrepareSkillDataTable(PopUpFilteredSkill) : HostingPage.GraphType == ConsultingDemand_New.TransactionTitle ? PrepareTitleDataTable(PopUpFilteredTitle) : PrepareMonthDataTable(PopUpFilteredByMonth);
                coloumnsCount = data.Columns.Count;
                sheetStylesList.Add(HeaderSheetStyle);
                sheetStylesList.Add(DataSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "ConsultantDemandDetail";
                dataset.Tables.Add(header);
                dataset.Tables.Add(data);
                dataSetList.Add(dataset);
            }
            else
            {
                string dateRangeTitle = "This consultant does not have demand for the selected period.";
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                sheetStylesList.Add(HeaderSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "ConsultantDemandDetail";
                dataset.Tables.Add(header);
                dataSetList.Add(dataset);
            }
            var filename = string.Format("{0}_{1}_{2}.xls", "ConsultantDemandDetail", StartDate.ToString(Constants.Formatting.DateFormatWithoutDelimiter), EndDate.ToString(Constants.Formatting.DateFormatWithoutDelimiter));
            NPOIExcel.Export(filename, dataSetList, sheetStylesList);
        }

        private DataTable PrepareSkillDataTable(List<ConsultantGroupbySkill> PopUpFilteredSkill)
        {
            DataTable data = new DataTable();
            List<object> row;

            data.Columns.Add("Title");
            data.Columns.Add("Skill Set");
            data.Columns.Add("Sales Stage");
            data.Columns.Add("Opportunity Number");
            data.Columns.Add("Project Number");
            data.Columns.Add("Account Name");
            data.Columns.Add("Project Name");
            data.Columns.Add("Resource Start Date");

            foreach (var gbyskill in PopUpFilteredSkill)
            {
                foreach (var detailsbySkill in gbyskill.ConsultantDetails)
                {
                    row = new List<object>();
                    row.Add(detailsbySkill.HtmlEncodedTitle);
                    row.Add(gbyskill.HtmlEncodedSkill);
                    row.Add(detailsbySkill.SalesStage);
                    row.Add(detailsbySkill.OpportunityNumber);
                    row.Add(detailsbySkill.ProjectNumber);
                    row.Add(detailsbySkill.HtmlEncodedAccountName);
                    row.Add(detailsbySkill.HtmlEncodedProjectName);
                    row.Add(detailsbySkill.ResourceStartDate);
                    data.Rows.Add(row.ToArray());
                }
            }
            return data;
        }

        private DataTable PrepareTitleDataTable(List<ConsultantGroupbyTitle> PopUpFilteredTitle)
        {
            DataTable data = new DataTable();
            List<object> row;

            data.Columns.Add("Title");
            data.Columns.Add("Skill Set");
            data.Columns.Add("Sales Stage");
            data.Columns.Add("Opportunity Number");
            data.Columns.Add("Project Number");
            data.Columns.Add("Account Name");
            data.Columns.Add("Project Name");
            data.Columns.Add("Resource Start Date");

            foreach (var gbyskill in PopUpFilteredTitle)
            {
                foreach (var detailsbySkill in gbyskill.ConsultantDetails)
                {
                    row = new List<object>();
                    row.Add(gbyskill.HtmlEncodedTitle);
                    row.Add(detailsbySkill.HtmlEncodedSkill);
                    row.Add(detailsbySkill.SalesStage);
                    row.Add(detailsbySkill.OpportunityNumber);
                    row.Add(detailsbySkill.ProjectNumber);
                    row.Add(detailsbySkill.HtmlEncodedAccountName);
                    row.Add(detailsbySkill.HtmlEncodedProjectName);
                    row.Add(detailsbySkill.ResourceStartDate);
                    data.Rows.Add(row.ToArray());
                }
            }
            return data;
        }

        private DataTable PrepareMonthDataTable(List<ConsultantGroupByMonth> PopUpFilteredByMonth)
        {
            DataTable data = new DataTable();
            List<object> row;

            data.Columns.Add("Title");
            data.Columns.Add("Skill Set");
            data.Columns.Add("Sales Stage");
            data.Columns.Add("Opportunity Number");
            data.Columns.Add("Project Number");
            data.Columns.Add("Account Name");
            data.Columns.Add("Project Name");
            data.Columns.Add("Resource Start Date");

            foreach (var gbyskill in PopUpFilteredByMonth)
            {
                foreach (var detailsbySkill in gbyskill.ConsultantDetailsByMonth)
                {
                    row = new List<object>();
                    row.Add(detailsbySkill.HtmlEncodedTitle);
                    row.Add(detailsbySkill.HtmlEncodedSkill);
                    row.Add(detailsbySkill.SalesStage);
                    row.Add(detailsbySkill.OpportunityNumber);
                    row.Add(detailsbySkill.ProjectNumber);
                    row.Add(detailsbySkill.HtmlEncodedAccountName);
                    row.Add(detailsbySkill.HtmlEncodedProjectName);
                    row.Add(detailsbySkill.ResourceStartDate);
                    data.Rows.Add(row.ToArray());
                }
            }
            return data;
        }

        protected void repTitles_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                CollapsiblePanelDateExtenderClientIds = new List<string>();
                CollapsiblePanelDateExtenderList = new List<CollapsiblePanelExtender>();
                Label lblTitleSkill1 = (Label)e.Item.FindControl("lblTitleSkillFr");
                Label lblTitleSkill2 = (Label)e.Item.FindControl("lblTitleSkillSc");
                if (HostingPage.GraphType == "TransactionTitle")
                {
                    lblTitleSkill1.Text = "Title";
                    lblTitleSkill2.Text = "Skill Set";
                }
                else if (HostingPage.GraphType == "TransactionSkill")
                {

                    lblTitleSkill1.Text = "Skill Set";
                    lblTitleSkill2.Text = "Title";
                }

            }
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lblHeader1 = (Label)e.Item.FindControl("lblHeader");
                if (HostingPage.GraphType == "TransactionTitle")
                {
                    ConsultantGroupbyTitle consDetail = (ConsultantGroupbyTitle)e.Item.DataItem;
                    lblHeader1.Text = consDetail.HtmlEncodedTitle;
                    Repeater rep = (Repeater)e.Item.FindControl("repDetails");
                    rep.DataSource = consDetail.ConsultantDetails;
                    rep.DataBind();
                }
                else if (HostingPage.GraphType == "TransactionSkill")
                {
                    ConsultantGroupbySkill consDetail = (ConsultantGroupbySkill)e.Item.DataItem;
                    lblHeader1.Text = consDetail.HtmlEncodedSkill;
                    Repeater rep = (Repeater)e.Item.FindControl("repDetails");
                    rep.DataSource = consDetail.ConsultantDetails;
                    rep.DataBind();
                }
                var cpeDate = e.Item.FindControl("cpeDetail") as CollapsiblePanelExtender;
                cpeDate.BehaviorID = Guid.NewGuid().ToString();
                CollapsiblePanelDateExtenderClientIds.Add(cpeDate.BehaviorID);
                CollapsiblePanelDateExtenderList.Add(cpeDate);
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var output = jss.Serialize(CollapsiblePanelDateExtenderClientIds);
                hdncpeExtendersIds.Value = output;
                //btnExpandOrCollapseAll.Text = btnExpandOrCollapseAll.ToolTip = "Expand All";
                //hdnCollapsed.Value = "true";
            }
        }

        protected void repDetails_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lblTitleSkillItem = (Label)e.Item.FindControl("lblTitleSkillItem");
                Label lblSalesStage = (Label)e.Item.FindControl("lblSalesStage");
                HyperLink hlOpportunityNumber = (HyperLink)e.Item.FindControl("hlOpportunityNumber");
                HyperLink hlProjectNumber = (HyperLink)e.Item.FindControl("hlProjectNumber");
                Label lblAccountName = (Label)e.Item.FindControl("lblAccountName");
                Label lblProjectName = (Label)e.Item.FindControl("lblProjectName");
                Label lblRsrcStartDate = (Label)e.Item.FindControl("lblRsrcStartDate");

                if (e.Item.DataItem.GetType() == typeof(ConsultantDemandDetailsByMonthByTitle))
                {
                    ConsultantDemandDetailsByMonthByTitle consDetails = (ConsultantDemandDetailsByMonthByTitle)e.Item.DataItem;
                    hlOpportunityNumber.ToolTip = consDetails.HtmlEncodedProjectDescription;
                    hlOpportunityNumber.Text = consDetails.OpportunityNumber;
                    hlOpportunityNumber.NavigateUrl = GetOpportunityDetailsLink((int?)(consDetails.OpportunityId));
                    hlProjectNumber.Text = consDetails.ProjectNumber;
                    hlProjectNumber.ToolTip = consDetails.HtmlEncodedProjectDescription;
                    hlProjectNumber.NavigateUrl = GetProjectDetailsLink((int?)(consDetails.ProjectId));
                    lblProjectName.Text = consDetails.HtmlEncodedProjectName;
                    lblAccountName.Text = consDetails.HtmlEncodedAccountName;
                    lblSalesStage.Text = consDetails.SalesStage;
                    lblRsrcStartDate.Text = consDetails.ResourceStartDate.ToString("MM/dd/yyyy");
                    lblTitleSkillItem.Text = consDetails.HtmlEncodedSkill;
                }
                else if (e.Item.DataItem.GetType() == typeof(ConsultantDemandDetailsByMonthBySkill))
                {
                    ConsultantDemandDetailsByMonthBySkill consDetails = (ConsultantDemandDetailsByMonthBySkill)e.Item.DataItem;
                    hlOpportunityNumber.ToolTip = consDetails.HtmlEncodedProjectDescription;
                    hlOpportunityNumber.Text = consDetails.OpportunityNumber;
                    hlOpportunityNumber.NavigateUrl = GetOpportunityDetailsLink((int?)(consDetails.OpportunityId));
                    hlProjectNumber.Text = consDetails.ProjectNumber;
                    hlProjectNumber.NavigateUrl = GetProjectDetailsLink((int?)(consDetails.ProjectId));
                    lblProjectName.Text = consDetails.HtmlEncodedProjectName;
                    hlProjectNumber.ToolTip = consDetails.HtmlEncodedProjectDescription;
                    lblAccountName.Text = consDetails.HtmlEncodedAccountName;
                    lblSalesStage.Text = consDetails.SalesStage;
                    lblRsrcStartDate.Text = consDetails.ResourceStartDate.ToString("MM/dd/yyyy");
                    lblTitleSkillItem.Text = consDetails.HtmlEncodedTitle;
                }
            }
        }

        protected void repByMonth_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                CollapsiblePanelDateExtenderClientIds = new List<string>();
                CollapsiblePanelDateExtenderList = new List<CollapsiblePanelExtender>();
                Label lblheader = (Label)e.Item.FindControl("lblTilteOrSkillHeader");
                lblheader.Text = HostingPage.GraphType == "PipeLineTitle" ? "Skill" : "Title";
            }

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater repDetails = (Repeater)e.Item.FindControl("repByMonthDetails");
                ConsultantGroupByMonth dataitem = (ConsultantGroupByMonth)e.Item.DataItem;
                var cpeDetails = e.Item.FindControl("cpeDetails") as CollapsiblePanelExtender;
                var result = dataitem.ConsultantDetailsByMonth;
                if (result.Count > 0)
                {
                    repDetails.DataSource = result;

                    cpeDetails.BehaviorID = Guid.NewGuid().ToString();
                    CollapsiblePanelDateExtenderClientIds.Add(cpeDetails.BehaviorID);
                    CollapsiblePanelDateExtenderList.Add(cpeDetails);
                    repDetails.DataBind();
                }
                else
                {
                    var imgDetails = e.Item.FindControl("imgDetails") as Image;
                    var lbMonth = e.Item.FindControl("lbMonth") as Label;
                    imgDetails.Visible = false;
                    lbMonth.Style.Remove("display");
                    lbMonth.Text = "&nbsp;&nbsp;&nbsp;";
                    cpeDetails.Enabled = false;
                }

            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var output = jss.Serialize(CollapsiblePanelDateExtenderClientIds);
                hdncpeExtendersIds.Value = output;
            }
        }

        protected void repByMonthDetails_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lblitem = (Label)e.Item.FindControl("lblTilteOrSkillItem");
                ConsultantDemandDetailsByMonth item = (ConsultantDemandDetailsByMonth)e.Item.DataItem;
                lblitem.Text = HostingPage.GraphType == "PipeLineTitle" ? item.HtmlEncodedSkill : item.HtmlEncodedTitle;
            }
        }

        protected string GetOpportunityDetailsLink(int? opportunityId)
        {
            if (opportunityId.HasValue)
                return Utils.Generic.GetTargetUrlWithReturn(String.Format(Constants.ApplicationPages.DetailRedirectFormat, Constants.ApplicationPages.OpportunityDetail, opportunityId.Value),
                                                            Constants.ApplicationPages.ConsultingDemand_New);
            else
                return string.Empty;
        }

        protected string GetProjectDetailsLink(int? projectId)
        {
            if (projectId.HasValue)
                return Utils.Generic.GetTargetUrlWithReturn(String.Format(Constants.ApplicationPages.DetailRedirectFormat, Constants.ApplicationPages.ProjectDetail, projectId.Value),
                                                            Constants.ApplicationPages.ConsultingDemand_New);
            else
                return string.Empty;
        }

        public void PopulateData(bool isFromGraph, string sortColumns = "")
        {
            if (HostingPage.GraphType == "TransactionTitle")
            {
                List<ConsultantGroupbyTitle> data;
                data = ServiceCallers.Custom.Report(r => r.ConsultingDemandTransactionReportByTitle(StartDate, EndDate, HostingPage.hdnTitlesProp, sortColumns, HostingPage.hdnSalesStagesProp)).ToList();
                repTitles.Visible = true;
                repByMonth.Visible = false;
                repTitles.DataSource = data;
                HostingPage.RolesCount = data.Sum(p=>p.ConsultantDetails.Count);
                repTitles.DataBind();
                sortColumn = isFromGraph ? "Title" : sortColumn;
                sortAscend = isFromGraph ? true : sortAscend;
            }
            else if (HostingPage.GraphType == "TransactionSkill")
            {
                List<ConsultantGroupbySkill> data;
                data = ServiceCallers.Custom.Report(r => r.ConsultingDemandTransactionReportBySkill(StartDate, EndDate, HostingPage.hdnSkillsProp, sortColumns,HostingPage.hdnSalesStagesProp)).ToList();
                repTitles.Visible = true;
                repByMonth.Visible = false;
                repTitles.DataSource = data;
                HostingPage.RolesCount = data.Sum(p => p.ConsultantDetails.Count);
                sortColumn = isFromGraph ? "Skill" : sortColumn;
                sortAscend = isFromGraph ? true : sortAscend;
                repTitles.DataBind();
            }
            if (HostingPage.GraphType == "PipeLineTitle" || HostingPage.GraphType == "PipeLineSkill")
            {
                List<ConsultantGroupByMonth> data;
                if (HostingPage.GraphType == "PipeLineTitle")
                {
                    data = ServiceCallers.Custom.Report(r => r.ConsultingDemandDetailsByMonth(StartDate, EndDate, BtnExportPipeLineSelectedValue, null,HostingPage.isSelectAllSalesStages ? null : HostingPage.hdnSalesStagesProp, sortColumns, true)).ToList();
                }
                else
                {
                    data = ServiceCallers.Custom.Report(r => r.ConsultingDemandDetailsByMonth(StartDate, EndDate, null, BtnExportPipeLineSelectedValue, HostingPage.isSelectAllSalesStages ? null : HostingPage.hdnSalesStagesProp, sortColumns, true)).ToList();
                }
                sortColumn = isFromGraph ? "Month" : sortColumn;
                sortAscend = isFromGraph ? true : sortAscend;
                HostingPage.RolesCount = data.Sum(p => p.ConsultantDetailsByMonth.Count);
                repByMonth.Visible = true;
                repTitles.Visible = false;
                repByMonth.DataSource = data;
                repByMonth.DataBind();
            }
            if (!isFromGraph && hdnCollapsed.Value.ToLower() != "true")
            {
                foreach (var cpe in CollapsiblePanelDateExtenderList)
                {
                    cpe.Collapsed = false;
                }
            }
            else if (isFromGraph)
            {
                hdnCollapsed.Value = "true";
                btnExpandOrCollapseAll.Text = "Expand All";
            }
        }

    }
}

