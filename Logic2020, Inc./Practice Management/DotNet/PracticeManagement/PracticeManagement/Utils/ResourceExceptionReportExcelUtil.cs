using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PraticeManagement.Utils.Excel;
using PraticeManagement.ConfigurationService;
using DataTransferObjects.Reports;
using System.Data;

namespace PraticeManagement.Utils
{
    public class ResourceExceptionReportExcelUtil
    {
        #region PrivateVariable
        private int ZeroExceptionHeaderRowsCount = 1;
        private int ZeroExceptioncoloumnsCount = 1;
        private int UnassignedHeaderRowsCount = 1;
        private int UnassignedcoloumnsCount = 1;
        private int AssignedHeaderRowsCount = 1;
        private int AssignedcoloumnsCount = 1;
        private DateTime? StartDate = null;
        private DateTime? EndDate = null;

        #endregion

        #region Properties

        public ResourceExceptionReport[] ZeroExceptionReportList
        {
            get;
            set;
        }

        public ResourceExceptionReport[] UnassignedExceptionReportList
        {
            get;
            set;
        }

        public ResourceExceptionReport[] AssignedExceptionReportList
        {
            get;
            set;
        }

        public byte[] Attachment { get; set; }

        private SheetStyles ZeroExceptionHeaderSheetStyle
        {
            get
            {
                CellStyles cellStyle = new CellStyles();
                cellStyle.IsBold = true;
                cellStyle.BorderStyle = NPOI.SS.UserModel.BorderStyle.None;
                cellStyle.FontHeight = 220;
                CellStyles[] cellStylearray = { cellStyle };
                RowStyles headerrowStyle = new RowStyles(cellStylearray);
                headerrowStyle.Height = 350;

                CellStyles dataCellStyle = new CellStyles();
                CellStyles[] dataCellStylearray = { dataCellStyle };
                RowStyles datarowStyle = new RowStyles(dataCellStylearray);

                RowStyles[] rowStylearray = { headerrowStyle, headerrowStyle, datarowStyle };

                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.MergeRegion.Add(new int[] { 0, 0, 0, ZeroExceptioncoloumnsCount - 1 });
                sheetStyle.MergeRegion.Add(new int[] { 1, 1, 0, ZeroExceptioncoloumnsCount - 1 });
                sheetStyle.IsAutoResize = false;

                return sheetStyle;
            }
        }

        private SheetStyles ZeroExceptionDataSheetStyle
        {
            get
            {
                CellStyles headerCellStyle = new CellStyles();
                headerCellStyle.IsBold = true;
                headerCellStyle.FontHeight = 220;
                headerCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                headerCellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

                List<CellStyles> headerCellStyleList = new List<CellStyles>();
                for (int i = 1; i <= 11; i++)
                    headerCellStyleList.Add(headerCellStyle);

                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataCellStyle = new CellStyles();

                CellStyles dataStartDateCellStyle = new CellStyles();
                dataStartDateCellStyle.DataFormat = "mm/dd/yyyy";

                List<CellStyles> dataCellStyleList = new List<CellStyles>();

                for (int i = 1; i <= 8; i++)
                    dataCellStyleList.Add(dataCellStyle);
                dataCellStyleList.Add(dataStartDateCellStyle);
                dataCellStyleList.Add(dataStartDateCellStyle);
                dataCellStyleList.Add(dataCellStyle);

                RowStyles datarowStyle = new RowStyles(dataCellStyleList.ToArray());

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);

                sheetStyle.TopRowNo = ZeroExceptionHeaderRowsCount;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanRowSplit = ZeroExceptionHeaderRowsCount;
                sheetStyle.FreezePanColSplit = 0;
                return sheetStyle;
            }
        }

        private SheetStyles UnassignedExceptionHeaderSheetStyle
        {
            get
            {
                CellStyles cellStyle = new CellStyles();
                cellStyle.IsBold = true;
                cellStyle.BorderStyle = NPOI.SS.UserModel.BorderStyle.None;
                cellStyle.FontHeight = 220;
                CellStyles[] cellStylearray = { cellStyle };
                RowStyles headerrowStyle = new RowStyles(cellStylearray);
                headerrowStyle.Height = 350;

                CellStyles dataCellStyle = new CellStyles();
                CellStyles[] dataCellStylearray = { dataCellStyle };
                RowStyles datarowStyle = new RowStyles(dataCellStylearray);

                RowStyles[] rowStylearray = { headerrowStyle, headerrowStyle, datarowStyle };

                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.MergeRegion.Add(new int[] { 0, 0, 0, UnassignedcoloumnsCount - 1 });
                sheetStyle.MergeRegion.Add(new int[] { 1, 1, 0, UnassignedcoloumnsCount - 1 });
                sheetStyle.IsAutoResize = false;

                return sheetStyle;
            }
        }

        private SheetStyles UnassignedExceptionDataSheetStyle
        {
            get
            {
                CellStyles headerCellStyle = new CellStyles();
                headerCellStyle.IsBold = true;
                headerCellStyle.FontHeight = 220;
                headerCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                headerCellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

                List<CellStyles> headerCellStyleList = new List<CellStyles>();
                for (int i = 1; i <= 9; i++)
                    headerCellStyleList.Add(headerCellStyle);

                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataCellStyle = new CellStyles();

                List<CellStyles> dataCellStyleList = new List<CellStyles>();

                for (int i = 1; i <= 9; i++)
                    dataCellStyleList.Add(dataCellStyle);

                RowStyles datarowStyle = new RowStyles(dataCellStyleList.ToArray());

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);

                sheetStyle.TopRowNo = UnassignedHeaderRowsCount;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanRowSplit = UnassignedHeaderRowsCount;
                sheetStyle.FreezePanColSplit = 0;
                return sheetStyle;
            }
        }

        private SheetStyles AssignedExceptionHeaderSheetStyle
        {
            get
            {
                CellStyles cellStyle = new CellStyles();
                cellStyle.IsBold = true;
                cellStyle.BorderStyle = NPOI.SS.UserModel.BorderStyle.None;
                cellStyle.FontHeight = 220;
                CellStyles[] cellStylearray = { cellStyle };
                RowStyles headerrowStyle = new RowStyles(cellStylearray);
                headerrowStyle.Height = 350;

                CellStyles dataCellStyle = new CellStyles();
                CellStyles[] dataCellStylearray = { dataCellStyle };
                RowStyles datarowStyle = new RowStyles(dataCellStylearray);

                RowStyles[] rowStylearray = { headerrowStyle, headerrowStyle, datarowStyle };

                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.MergeRegion.Add(new int[] { 0, 0, 0, AssignedcoloumnsCount - 1 });
                sheetStyle.MergeRegion.Add(new int[] { 1, 1, 0, AssignedcoloumnsCount - 1 });
                sheetStyle.IsAutoResize = false;

                return sheetStyle;
            }
        }

        private SheetStyles AssignedExceptionDataSheetStyle
        {
            get
            {
                CellStyles headerCellStyle = new CellStyles();
                headerCellStyle.IsBold = true;
                headerCellStyle.FontHeight = 220;
                headerCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                headerCellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

                List<CellStyles> headerCellStyleList = new List<CellStyles>();
                for (int i = 1; i <= 10; i++)
                    headerCellStyleList.Add(headerCellStyle);

                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataCellStyle = new CellStyles();

                List<CellStyles> dataCellStyleList = new List<CellStyles>();

                for (int i = 1; i <= 10; i++)
                    dataCellStyleList.Add(dataCellStyle);

                RowStyles datarowStyle = new RowStyles(dataCellStyleList.ToArray());

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);

                sheetStyle.TopRowNo = AssignedHeaderRowsCount;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanRowSplit = AssignedHeaderRowsCount;
                sheetStyle.FreezePanColSplit = 0;
                return sheetStyle;
            }
        }

        #endregion

        #region Methods

        private DataTable PrepareDataTableForZeroException(ResourceExceptionReport[] reportList)
        {
            DataTable data = new DataTable();

            data.Columns.Add("Employee ID");
            data.Columns.Add("Employee Name");
            data.Columns.Add("Pay Type");
            data.Columns.Add("Is Offshore?");
            data.Columns.Add("Project #");
            data.Columns.Add("Project Name");
            data.Columns.Add("Status");
            data.Columns.Add("Milestone Name");
            data.Columns.Add("Milestone Start Date");
            data.Columns.Add("Milestone End Date");
            data.Columns.Add("Hourly Rate");

            if (reportList.Length > 0)
            {
                List<object> row;
                List<object> rownew;

                foreach (var item in reportList)
                {
                    row = new List<object>();
                    row.Add(item.Person.EmployeeNumber);
                    row.Add(item.Person.PersonLastFirstName);
                    row.Add(item.Person.CurrentPay.TimescaleName);
                    row.Add(item.Person.IsOffshore ? "Yes" : "No");
                    row.Add(item.Project.ProjectNumber);
                    row.Add(item.Project.Name);
                    row.Add(item.Project.Status.StatusType.ToString());
                    for (int i = 0; i < item.Project.Milestones.Count; i++)
                    {
                        row.Add(item.Project.Milestones[i].Description);
                        row.Add(item.Project.Milestones[i].StartDate);
                        row.Add(item.Project.Milestones[i].ProjectedDeliveryDate);
                        row.Add(item.Project.Milestones[i].Amount);
                        data.Rows.Add(row.ToArray());
                        if (i != item.Project.Milestones.Count - 1)
                        {
                            rownew = new List<object>();
                            rownew.AddRange(row);
                            rownew.RemoveRange(7, 4);
                            row = rownew;
                        }
                    }
                }
            }
            return data;
        }

        private DataTable PrepareDataTableForUnassignedException(ResourceExceptionReport[] reportList)
        {
            DataTable data = new DataTable();

            data.Columns.Add("Employee ID");
            data.Columns.Add("Employee Name");
            data.Columns.Add("Pay Type");
            data.Columns.Add("Is Offshore?");
            data.Columns.Add("Project #");
            data.Columns.Add("Project Name");
            data.Columns.Add("Status");
            data.Columns.Add("Billable Hours");
            data.Columns.Add("Non-Billable Hours");

            if (reportList.Length > 0)
            {
                List<object> row;

                foreach (var item in reportList)
                {
                    row = new List<object>();
                    row.Add(item.Person.EmployeeNumber);
                    row.Add(item.Person.PersonLastFirstName);
                    row.Add(item.Person.CurrentPay.TimescaleName);
                    row.Add(item.Person.IsOffshore ? "Yes" : "No");
                    row.Add(item.Project.ProjectNumber);
                    row.Add(item.Project.Name);
                    row.Add(item.Project.Status.StatusType.ToString());
                    row.Add(item.BillableHours);
                    row.Add(item.NonBillableHours);
                    data.Rows.Add(row.ToArray());
                }
            }
            return data;
        }

        private DataTable PrepareDataTableForAssignedException(ResourceExceptionReport[] reportList)
        {
            DataTable data = new DataTable();
            data.Columns.Add("Employee ID");
            data.Columns.Add("Employee Name");
            data.Columns.Add("Pay Type");
            data.Columns.Add("Is Offshore?");
            data.Columns.Add("Project #");
            data.Columns.Add("Project Name");
            data.Columns.Add("Status");
            data.Columns.Add("Projected Hours");
            data.Columns.Add("Billable Hours");
            data.Columns.Add("Non-Billable Hours");

            if (reportList.Length > 0)
            {
                List<object> row;
                foreach (var item in reportList)
                {
                    row = new List<object>();
                    row.Add(item.Person.EmployeeNumber);
                    row.Add(item.Person.PersonLastFirstName);
                    row.Add(item.Person.CurrentPay.TimescaleName);
                    row.Add(item.Person.IsOffshore ? "Yes" : "No");
                    row.Add(item.Project.ProjectNumber);
                    row.Add(item.Project.Name);
                    row.Add(item.Project.Status.StatusType.ToString());
                    row.Add(item.ProjectedHours);
                    row.Add(item.BillableHours);
                    row.Add(item.NonBillableHours);
                    data.Rows.Add(row.ToArray());
                }
            }
            return data;
        }

        public bool PopulateAttachment()
        {
            if (StartDate == null || EndDate == null)
                return false;
            var dataSetList = GetDataSetList();
            List<SheetStyles> sheetStylesList = GetStyleSheetList();
            Attachment = NPOIExcel.GetAttachment(dataSetList, sheetStylesList);
            return true;
        }

        public bool EmailExceptionReport()
        {
            if (Attachment == null || StartDate == null || EndDate == null)
                return false;

            using (var serviceClient = new ConfigurationServiceClient())
            {
                serviceClient.SendResourceExceptionReportsEmail(StartDate.Value, EndDate.Value, Attachment);
            }
            return true;
        }

        public void ExceptionReport()
        {
            if (StartDate == null || EndDate == null)
                return;
            var dataSetList = GetDataSetList();
            List<SheetStyles> sheetStylesList = GetStyleSheetList();
            NPOIExcel.Export(string.Format("ExceptionReporting_{0}_{1}.xls", StartDate.Value.ToString(Constants.Formatting.EntryDateFormat), EndDate.Value.ToString(Constants.Formatting.EntryDateFormat)), dataSetList, sheetStylesList);
        }

        private List<SheetStyles> GetStyleSheetList()
        {
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            sheetStylesList.Add(ZeroExceptionHeaderSheetStyle);
            sheetStylesList.Add(ZeroExceptionDataSheetStyle);
            sheetStylesList.Add(UnassignedExceptionHeaderSheetStyle);
            sheetStylesList.Add(UnassignedExceptionDataSheetStyle);
            sheetStylesList.Add(AssignedExceptionHeaderSheetStyle);
            sheetStylesList.Add(AssignedExceptionDataSheetStyle);

            return sheetStylesList;
        }

        private List<DataSet> GetDataSetList()
        {
            var ZeroExceptionReport = PrepareDataTableForZeroException(ZeroExceptionReportList);
            ZeroExceptioncoloumnsCount = ZeroExceptionReport.Columns.Count;
            DataTable ZeroExceptionReportHeader = new DataTable();
            ZeroExceptionReportHeader.Columns.Add("Exception Reporting: $0 Bill Rate");
            ZeroExceptionReportHeader.Rows.Add(StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat));
            ZeroExceptionHeaderRowsCount = ZeroExceptionReportHeader.Rows.Count + 3;

            var UnassignedExceptionReport = PrepareDataTableForUnassignedException(UnassignedExceptionReportList);
            UnassignedcoloumnsCount = UnassignedExceptionReport.Columns.Count;
            DataTable UnassignedExceptionReportHeader = new DataTable();
            UnassignedExceptionReportHeader.Columns.Add("Exception Reporting: Unassigned Resources Charging Hours to a Project ");
            UnassignedExceptionReportHeader.Rows.Add(StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat));
            UnassignedHeaderRowsCount = UnassignedExceptionReportHeader.Rows.Count + 3;

            var AssignedExceptionReport = PrepareDataTableForAssignedException(AssignedExceptionReportList);
            AssignedcoloumnsCount = AssignedExceptionReport.Columns.Count;
            DataTable AssignedExceptionReportHeader = new DataTable();
            AssignedExceptionReportHeader.Columns.Add("Exception Reporting: Assigned Resources Not Charging Hours to Assigned Project ");
            AssignedExceptionReportHeader.Rows.Add(StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat));
            AssignedHeaderRowsCount = AssignedExceptionReportHeader.Rows.Count + 3;

            var dataSetList = new List<DataSet>();
            var ZeroExceptiondataset = new DataSet();
            ZeroExceptiondataset.DataSetName = "$0 BillRate";
            ZeroExceptiondataset.Tables.Add(ZeroExceptionReportHeader);
            ZeroExceptiondataset.Tables.Add(ZeroExceptionReport);
            dataSetList.Add(ZeroExceptiondataset);

            var UnassignedExceptiondataset = new DataSet();
            UnassignedExceptiondataset.DataSetName = "Unassigned Resources";
            UnassignedExceptiondataset.Tables.Add(UnassignedExceptionReportHeader);
            UnassignedExceptiondataset.Tables.Add(UnassignedExceptionReport);
            dataSetList.Add(UnassignedExceptiondataset);

            var AssignedExceptiondataset = new DataSet();
            AssignedExceptiondataset.DataSetName = "No Chargeable Hours";
            AssignedExceptiondataset.Tables.Add(AssignedExceptionReportHeader);
            AssignedExceptiondataset.Tables.Add(AssignedExceptionReport);
            dataSetList.Add(AssignedExceptiondataset);

            return dataSetList;
        }

        #endregion

        public ResourceExceptionReportExcelUtil(DateTime startDate, DateTime endDate, ResourceExceptionReport[] zeroExceptionReportList, ResourceExceptionReport[] unassignedExceptionReportList, ResourceExceptionReport[] assignedExceptionReportList)
        {
            StartDate = (DateTime?)startDate;
            EndDate = (DateTime?)endDate;
            ZeroExceptionReportList = zeroExceptionReportList;
            UnassignedExceptionReportList = unassignedExceptionReportList;
            AssignedExceptionReportList = assignedExceptionReportList;
        }

        public ResourceExceptionReportExcelUtil(DateTime startDate, DateTime endDate)
        {
            StartDate = (DateTime?)startDate;
            EndDate = (DateTime?)endDate;
            ZeroExceptionReportList = ServiceCallers.Custom.Report(p => p.ZeroHourlyRateExceptionReport(StartDate.Value, EndDate.Value));
            UnassignedExceptionReportList = ServiceCallers.Custom.Report(p => p.ResourceAssignedOrUnassignedChargingExceptionReport(StartDate.Value, EndDate.Value, true));
            AssignedExceptionReportList = ServiceCallers.Custom.Report(p => p.ResourceAssignedOrUnassignedChargingExceptionReport(StartDate.Value, EndDate.Value, false));
        }
    }
}
