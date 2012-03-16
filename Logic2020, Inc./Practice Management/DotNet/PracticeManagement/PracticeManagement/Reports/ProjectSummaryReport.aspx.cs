using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PraticeManagement.Reporting
{
    public partial class ProjectSummaryReport : System.Web.UI.Page
    {
        #region Properties

        public Dictionary<DateTime, String> DatesList
        {
            get
            {
                var list = new Dictionary<DateTime, String>();

                if (StartDate != null && EndDate != null)
                {
                    var days = EndDate.Value.Subtract(StartDate.Value).Days;
                    if (days <= 7)
                    {
                        //Single Day.
                        for (int day = 0; day <= days; day++)
                        {
                            DateTime _startDate = StartDate.Value.AddDays(day);
                            list.Add(_startDate, _startDate.ToString("MM/dd/yyyy"));
                        }
                    }
                    else if (days > 7 && days <= 31)
                    {
                        //Single Week.
                        DateTime _startDate = StartDate.Value;
                        DateTime _endDate = Utils.Calendar.WeekEndDate(_startDate);
                        while (_startDate <= EndDate)
                        {
                            list.Add(_startDate, _startDate.ToString("MM/dd/yyyy") + " - " + (_endDate <= EndDate ? _endDate.ToString("MM/dd/yyyy") : EndDate.Value.ToString("MM/dd/yyyy")));
                            _startDate = Utils.Calendar.WeekStartDate(_endDate.AddDays(1));
                            _endDate = Utils.Calendar.WeekEndDate(_endDate.AddDays(1));
                        }
                    }
                    else if (days > 31 && days <= 366)
                    {
                        //Single Month.
                        DateTime _startDate = StartDate.Value;
                        DateTime _endDate = Utils.Calendar.MonthEndDate(_startDate);
                        while (_startDate <= EndDate)
                        {
                            list.Add(_startDate, _startDate.ToString("MMM - yyyy"));
                            _startDate = Utils.Calendar.MonthStartDate(_endDate.AddDays(1));
                            _endDate = Utils.Calendar.MonthEndDate(_endDate.AddDays(1));
                        }
                    }
                    else if (days > 366)
                    {
                        //Single Year.
                        DateTime _startDate = StartDate.Value;
                        DateTime _endDate = Utils.Calendar.YearEndDate(_startDate);
                        while (_startDate <= EndDate)
                        {
                            list.Add(_startDate, _startDate.ToString("yyyy"));
                            _startDate = Utils.Calendar.YearStartDate(_endDate.AddDays(1));
                            _endDate = Utils.Calendar.YearEndDate(_endDate.AddDays(1));
                        }
                    }
                }

                return list;
            }
        }


        public DateTime? StartDate
        {
            get;
            set;
        }

        public DateTime? EndDate
        {
            get;
            set;
        }


        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                LoadActiveView();
            }
        }

        protected void btnView_Command(object sender, CommandEventArgs e)
        {
            int viewIndex = int.Parse((string)e.CommandArgument);
            SelectView((Control)sender, viewIndex);
            LoadActiveView();
        }

        private void SelectView(Control sender, int viewIndex)
        {
            mvProjectSummaryReport.ActiveViewIndex = viewIndex;

            SetCssClassEmpty();

            ((WebControl)sender.Parent).CssClass = "SelectedSwitch";
        }

        private void SetCssClassEmpty()
        {
            foreach (TableCell cell in tblProjectsummaryReportViewSwitch.Rows[0].Cells)
            {
                cell.CssClass = string.Empty;
            }
        }

        private void LoadActiveView()
        {
            if (mvProjectSummaryReport.ActiveViewIndex == 0)
            {
                PopulateByResourceData();
            }
            else if (mvProjectSummaryReport.ActiveViewIndex == 1)
            {
                PopulateByWorkTypeData();
            }
            else
            {
                PopulateMatrixData();
            }
        }

        private void PopulateMatrixData()
        {

            var data = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByResourceAndWorkType("P081100", string.Empty, string.Empty));

            ucByMatrix.DataBindResource(data);
            ucBillableAndNonBillable.BillablValue = (data.Count() > 0) ? data.Sum(d => d.BillabileTotal).ToString() : "0";
            ucBillableAndNonBillable.NonBillablValue = (data.Count() > 0) ? data.Sum(d => d.NonBillableTotal).ToString() : "0";

        }

        private void PopulateByResourceData()
        {

            var data = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByResource("P081100", string.Empty, string.Empty));

            foreach (var personLevelGroupedHour in data)
            {
                foreach (var item in personLevelGroupedHour.GroupedHoursList)
                {
                    if (StartDate == null || StartDate.Value > item.StartDate)
                    {
                        StartDate = item.StartDate;
                    }
                    if (EndDate == null || EndDate.Value < item.StartDate)
                    {
                        EndDate = item.StartDate;
                    }
                }
            }

            ucByResource.DataBindResource(data, DatesList);
            ucBillableAndNonBillable.BillablValue = (data.Count() > 0) ? data.Sum(d => d.BillabileTotal).ToString() : "0";
            ucBillableAndNonBillable.NonBillablValue = (data.Count() > 0) ? data.Sum(d => d.NonBillableTotal).ToString() : "0";

        }

        private void PopulateByWorkTypeData()
        {
            var data = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByWorkType("P081100", string.Empty, string.Empty));

            foreach (var personLevelGroupedHour in data)
            {
                foreach (var item in personLevelGroupedHour.GroupedHoursList)
                {
                    if (StartDate == null || StartDate.Value > item.StartDate)
                    {
                        StartDate = item.StartDate;
                    }
                    if (EndDate == null || EndDate.Value < item.StartDate)
                    {
                        EndDate = item.StartDate;
                    }
                }
            }


            ucByWorktype.DataBindResource(data, DatesList);
            ucBillableAndNonBillable.BillablValue = (data.Count() > 0) ? data.Sum(d => d.BillabileTotal).ToString() : "0";
            ucBillableAndNonBillable.NonBillablValue = (data.Count() > 0) ? data.Sum(d => d.NonBillableTotal).ToString() : "0";

        }
    }
}
