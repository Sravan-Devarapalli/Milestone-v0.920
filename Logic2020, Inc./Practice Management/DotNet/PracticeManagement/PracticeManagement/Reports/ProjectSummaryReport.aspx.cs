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
                var days = EndDate.Subtract(StartDate).Days;
                if (days <= 7)
                {
                    //Single Day.
                    for (int day = 0; day <= EndDate.Subtract(StartDate).Days; day++)
                    {
                        DateTime _startDate = StartDate.AddDays(day);
                        list.Add(_startDate, _startDate.ToString("MM/dd/yyyy"));
                    }
                }
                else if (days > 7 && days <= 31)
                {
                    //Single Week.
                    DateTime _startDate = StartDate;
                    DateTime _endDate = Utils.Calendar.WeekEndDate(_startDate);
                    while (_startDate <= EndDate)
                    {
                        list.Add(_startDate, _startDate.ToString("MM/dd/yyyy") + " - " + (_endDate <= EndDate ? _endDate.ToString("MM/dd/yyyy") : EndDate.ToString("MM/dd/yyyy")));
                        _startDate = Utils.Calendar.WeekStartDate(_endDate.AddDays(1));
                        _endDate = Utils.Calendar.WeekEndDate(_endDate.AddDays(1));
                    }
                }
                else if (days > 31 && days <= 366)
                {
                    //Single Month.
                    DateTime _startDate = StartDate;
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
                    DateTime _startDate = StartDate;
                    DateTime _endDate = Utils.Calendar.YearEndDate(_startDate);
                    while (_startDate <= EndDate)
                    {
                        list.Add(_startDate, _startDate.ToString("yyyy"));
                        _startDate = Utils.Calendar.YearStartDate(_endDate.AddDays(1));
                        _endDate = Utils.Calendar.YearEndDate(_endDate.AddDays(1));
                    }
                }
                return list;
            }
        }


        public DateTime StartDate
        {
            get;
            set;
        }

        public DateTime EndDate
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
        }

        private void PopulateByResourceData()
        {
            string orderByCerteria = string.Empty;
            var data = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByResource("P081100", string.Empty, string.Empty));
            StartDate = DateTime.MinValue;
            EndDate = DateTime.MaxValue;

            foreach (var personLevelGroupedHour in data)
            {
                foreach (var item in personLevelGroupedHour.GroupedHoursList)
                {
                    if (StartDate == DateTime.MinValue || StartDate > item.StartDate)
                    {
                        StartDate = item.StartDate;
                    }
                    if (EndDate == DateTime.MaxValue || EndDate < item.StartDate)
                    {
                        EndDate = item.StartDate;
                    }
                }
            }

            tpByResource.DataBindResource(data, DatesList);
        }

        private void PopulateByWorkTypeData()
        {

        }
    }
}
