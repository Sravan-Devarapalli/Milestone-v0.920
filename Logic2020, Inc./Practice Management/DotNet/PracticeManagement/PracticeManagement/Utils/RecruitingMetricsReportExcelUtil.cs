using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PraticeManagement.Utils.Excel;
using DataTransferObjects;
using System.Data;
using PraticeManagement.Controls;
using PraticeManagement.ConfigurationService;

namespace PraticeManagement.Utils
{
    public class RecruitingMetricsReportExcelUtil
    {
        #region PrivateVariable
        
        private int headerRowsCountRM = 1; 
        private int coloumnsCountRM = 1;
        private DateTime? StartDate = null;
        private DateTime? EndDate = null;

        #endregion

        #region Properties

        public Person[] RecruitingMetricsReportList
        {
            get;
            set;
        }

        public byte[] Attachment { get; set; }

        private SheetStyles HeaderSheetStyleRM
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
                sheetStyle.MergeRegion.Add(new int[] { 0, 0, 0, coloumnsCountRM - 1 });
                sheetStyle.IsAutoResize = false;

                return sheetStyle;
            }
        }

        private SheetStyles DataSheetStyleRM
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
       
                CellStyles[] dataCellStylearray = {dataCellStyle,
                                                   dataCellStyle, 
                                                   dataCellStyle, 
                                                   dataCellStyle, 
                                                   dataDateCellStyle,
                                                   dataDateCellStyle,
                                                   dataCellStyle,
                                                   dataCellStyle,
                                                   dataCellStyle,
                                                   dataCellStyle,
                                                   dataCellStyle,
                                                   dataCellStyle,
                                                   dataCellStyle
                                                  };

                RowStyles datarowStyle = new RowStyles(dataCellStylearray);
                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.TopRowNo = headerRowsCountRM;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanColSplit = 0;
                sheetStyle.FreezePanRowSplit = headerRowsCountRM;

                return sheetStyle;
            }
        }

        #endregion

        #region Methods

        private DataTable PrepareDataTable(Person[] personsList)
        {
            DataTable data = new DataTable();
            List<object> rownew;
            List<object> row;

            data.Columns.Add("Employee ID");
            data.Columns.Add("Person Name");
            data.Columns.Add("Status");
            data.Columns.Add("Title");
            data.Columns.Add("Hire Date");
            data.Columns.Add("Termination Date");
            data.Columns.Add("Termination Reason");
            data.Columns.Add("Length of Tenure (in days)");
            data.Columns.Add("Pay Type");
            data.Columns.Add("Recruiter Name");
            data.Columns.Add("Passive/Active Candidate");
            data.Columns.Add("Target Company");
            data.Columns.Add("Recruiting Source");
            data.Columns.Add("Employee Referral");
            
            foreach (var pro in personsList)
            {
                row = new List<object>();
                int i;
                row.Add(pro.EmployeeNumber);
                row.Add(pro.PersonLastFirstName);
                row.Add(((PersonStatusType)pro.Status.Id).ToString());
                row.Add((pro.Title != null) ? pro.Title.TitleName : "");
                row.Add(pro.HireDate.ToString(Constants.Formatting.EntryDateFormat));
                row.Add((pro.TerminationDate != null) ? pro.TerminationDate.Value.ToString(Constants.Formatting.EntryDateFormat) : "");
                row.Add(pro.TerminationReasonid != null ? pro.TerminationReason : "");
                row.Add(pro.LengthOfTenture);
                row.Add(DataHelper.GetDescription(pro.CurrentPay.Timescale));
                row.Add((pro.RecruiterId != null) ? pro.RecruiterLastFirstName : "");
                row.Add(pro.JobSeekersStatusId != null ? (pro.JobSeekersStatus == JobSeekersStatus.ActiveCandidate?"Active":"Passive" ): "");
                row.Add(pro.TargetedCompanyRecruitingMetrics.RecruitingMetricsId != null ? pro.TargetedCompanyRecruitingMetrics.Name : "");
                row.Add(pro.SourceRecruitingMetrics.RecruitingMetricsId != null ? pro.SourceRecruitingMetrics.Name : "");
                row.Add(pro.EmployeeReferralId != null ? pro.EmployeeReferralLastFirstName : "");
                data.Rows.Add(row.ToArray());
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

        public bool EmailRecruitingMetricsReport()
        {
            if (Attachment == null || StartDate == null || EndDate == null)
                return false;

            using (var serviceClient = new ConfigurationServiceClient())
            {
                serviceClient.SendRecruitingMetricsReportEmail(StartDate.Value, EndDate.Value, Attachment);
            }
            return true;
        }

        public void ExceptionReport()
        {
            if (StartDate == null || EndDate == null)
                return;
            var dataSetList = GetDataSetList();
            List<SheetStyles> sheetStylesList = GetStyleSheetList();
            NPOIExcel.Export(string.Format("RecruitingMetricsReport_{0}_{1}.xls", StartDate.Value.ToString(Constants.Formatting.EntryDateFormat), EndDate.Value.ToString(Constants.Formatting.EntryDateFormat)), dataSetList, sheetStylesList);
        }

        private List<SheetStyles> GetStyleSheetList()
        {
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            sheetStylesList.Add(HeaderSheetStyleRM);
            sheetStylesList.Add(DataSheetStyleRM);
            return sheetStylesList;
        }

        private List<DataSet> GetDataSetList()
        {
            var dataSetList = new List<DataSet>();
            if (RecruitingMetricsReportList.Length>0)
             {
                string dateRangeTitle = string.Format("Recruiting Metrics for the period of {0} to {1}", StartDate.Value.ToString(Constants.Formatting.EntryDateFormat), EndDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                headerRowsCountRM = header.Rows.Count + 3;
                var data = PrepareDataTable(RecruitingMetricsReportList);
                coloumnsCountRM = data.Columns.Count;
                var dataset = new DataSet();
                dataset.DataSetName = "Recruiting Metrics";
                dataset.Tables.Add(header);
                dataset.Tables.Add(data);
                dataSetList.Add(dataset);
            }
            else
            {
                string dateRangeTitle = "There are no persons who were hired in the specified period.";
                DataTable header = new DataTable();
                header.Columns.Add(dateRangeTitle);
                var dataset = new DataSet();
                dataset.DataSetName = "Recruiting Metrics";
                dataset.Tables.Add(header);
                dataSetList.Add(dataset);
            }
            return dataSetList;
        }

        #endregion

        public RecruitingMetricsReportExcelUtil(DateTime startDate, DateTime endDate, Person[] recruitingMetricsReportList)
        {
            StartDate = (DateTime?)startDate;
            EndDate = (DateTime?)endDate;
            RecruitingMetricsReportList = recruitingMetricsReportList;
        }

        public RecruitingMetricsReportExcelUtil(DateTime startDate, DateTime endDate)
        {
            StartDate = (DateTime?)startDate;
            EndDate = (DateTime?)endDate;
            RecruitingMetricsReportList = ServiceCallers.Custom.Report(p => p.RecruitingMetricsReport(StartDate.Value, EndDate.Value));
        }
    }
}
