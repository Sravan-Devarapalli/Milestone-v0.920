using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.Controls.Reports.HumanCapital;
using PraticeManagement.Utils;
using DataTransferObjects.Filters;

namespace PraticeManagement.Reporting
{
    public partial class NewHireReport : System.Web.UI.Page
    {
        #region constants

        private const string W2Hourly = "W2-Hourly";
        private const string W2Salary = "W2-Salary";
        private string NewHireReportExport = "New Hire Report";

        #endregion

        #region Properties

        //ddlPeriod Items
        //This Month   : 1
        //Last Month   : 2
        //Q1           : 3
        //Q2           : 4
        //Q3           : 5
        //Q4           : 6
        //Year To Date : 7
        //Custom Dates : 0

        public DateTime? StartDate
        {
            get
            {
                int selectedVal = 0;
                if (int.TryParse(ddlPeriod.SelectedValue, out selectedVal))
                {
                    if (selectedVal == 0)
                    {
                        return diRange.FromDate.Value;
                    }
                    else
                    {
                        var now = Utils.Generic.GetNowWithTimeZone();
                        if (selectedVal == 1)
                        {
                            return Utils.Calendar.MonthStartDate(now);
                        }
                        else if (selectedVal == 2)
                        {
                            return Utils.Calendar.LastMonthStartDate(now);
                        }
                        else if (selectedVal == 3 || selectedVal == 7)
                        {
                            return Utils.Calendar.QuarterStartDate(now, 1);
                        }
                        else if (selectedVal == 4)
                        {
                            return Utils.Calendar.QuarterStartDate(now, 2);
                        }
                        else if (selectedVal == 5)
                        {
                            return Utils.Calendar.QuarterStartDate(now, 3);
                        }
                        else
                        {
                            return Utils.Calendar.QuarterStartDate(now, 4);
                        }
                    }
                }
                return null;
            }
        }

        public DateTime? EndDate
        {
            get
            {
                int selectedVal = 0;
                if (int.TryParse(ddlPeriod.SelectedValue, out selectedVal))
                {
                    if (selectedVal == 0)
                    {
                        return diRange.ToDate.Value;
                    }
                    else
                    {
                        var now = Utils.Generic.GetNowWithTimeZone();
                        if (selectedVal == 1)
                        {
                            return Utils.Calendar.MonthEndDate(now);
                        }
                        else if (selectedVal == 2)
                        {
                            return Utils.Calendar.LastMonthEndDate(now);
                        }
                        else if (selectedVal == 3)
                        {
                            return Utils.Calendar.QuarterEndDate(now, 1);
                        }
                        else if (selectedVal == 4)
                        {
                            return Utils.Calendar.QuarterEndDate(now, 2);
                        }
                        else if (selectedVal == 5)
                        {
                            return Utils.Calendar.QuarterEndDate(now, 3);
                        }
                        else if (selectedVal == 6)
                        {
                            return Utils.Calendar.QuarterEndDate(now, 4);
                        }
                        else
                        {
                            return now;
                        }
                    }
                }
                return null;
            }
        }

        public int RangeSelected
        {
            get
            {
                int selectedValue = 0;
                int.TryParse(ddlPeriod.SelectedValue, out selectedValue);
                return selectedValue;
            }
        }

        public String Range
        {
            get
            {
                string range = string.Empty;
                if (StartDate.HasValue && EndDate.HasValue)
                {
                    switch (RangeSelected)
                    {
                        case 1:
                        case 2:
                            range = StartDate.Value.ToString("MMMM yyyy");
                            break;
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                            range = "Quarter " + (RangeSelected - 2).ToString() + " (" + StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat) + ")";
                            break;
                        case 7:
                            range = "Year To Date (" + StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat) + ")";
                            break;
                        default:
                            range = StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat);
                            break;
                    }
                }
                return range;
            }
        }

        public String GraphRange
        {
            get
            {
                string range = string.Empty;
                if (StartDate.HasValue && EndDate.HasValue)
                {
                    switch (RangeSelected)
                    {
                        case 1:
                        case 2:
                            range = StartDate.Value.ToString("MMMM yyyy");
                            break;
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                            range = "Quarter " + (RangeSelected - 2).ToString();
                            break;
                        case 7:
                            range = "Year To Date";
                            break;
                        default:
                            range = StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat);
                            break;
                    }
                }
                return range;
            }
        }

        public string PersonStatus
        {
            get
            {
                return cblPersonStatus.SelectedItemsXmlFormat;
            }
        }

        public string PayTypes
        {
            get
            {
                return cblTimeScales.SelectedItemsXmlFormat;
            }
        }

        public string Practices
        {
            get
            {
                return cblPractices.SelectedItemsXmlFormat;
            }
        }

        public bool ExcludeInternalProjects
        {
            get
            {
                return chbInternalProjects.Checked;
            }
        }

        public bool SetSelectedFilters { get; set; }

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (this.cblPractices != null && this.cblPractices.Items.Count == 0)
                {
                    DataHelper.FillPracticeList(this.cblPractices, Resources.Controls.AllPracticesText);
                }
                if (this.cblTimeScales != null && this.cblTimeScales.Items.Count == 0)
                {
                    DataHelper.FillTimescaleList(this.cblTimeScales, Resources.Controls.AllTypes);
                }
                if (this.cblPersonStatus != null && this.cblPersonStatus.Items.Count == 0)
                {
                    DataHelper.FillPersonStatusList(this.cblPersonStatus, Resources.Controls.AllTypes);
                }
                SetDefalultfilter();
                GetFilterValuesForSession();
                LoadActiveView();
            }

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (humanCapitalReportsHeader.Count == 1)
            {
                tdFirst.Attributes["class"] = "width30P";
                tdSecond.Attributes["class"] = "Width40P";
                tdThird.Attributes["class"] = "width30P";
                ParametersTable.Attributes["class"] = "WholeWidth ParametersTable";
            }
            else if (humanCapitalReportsHeader.Count == 2)
            {
                tdFirst.Attributes["class"] = "Width10Percent";
                tdSecond.Attributes["class"] = "Width80Percent";
                tdThird.Attributes["class"] = "Width10Percent";
                ParametersTable.Attributes["class"] = "WholeWidth";
            }

            diRange.FromDate = StartDate;
            diRange.ToDate = EndDate;
            lblCustomDateRange.Text = string.Format("({0}&nbsp;-&nbsp;{1})",
                    diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat),
                    diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat)
                    );
            if (ddlPeriod.SelectedValue == "0")
            {
                lblCustomDateRange.Attributes.Add("class", "fontBold");
                imgCalender.Attributes.Add("class", "");
            }
            else
            {
                lblCustomDateRange.Attributes.Add("class", "displayNone");
                imgCalender.Attributes.Add("class", "displayNone");
            }
            hdnStartDate.Value = diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat);
            hdnEndDate.Value = diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat);
        }

        #endregion

        #region Methods

        private void SelectDefaultTimeScaleItems(ScrollingDropDown cblTimeScales)
        {
            foreach (ListItem item in cblTimeScales.Items)
            {
                item.Selected = (item.Text == W2Hourly || item.Text == W2Salary);
            }
        }

        private void SelectDefaultPersonStatus(ScrollingDropDown cblPersonStatus)
        {
            SelectAllItems(cblPersonStatus);
        }

        private void SelectAllItems(ScrollingDropDown ddlpractices)
        {
            foreach (ListItem item in ddlpractices.Items)
            {
                item.Selected = true;
            }
        }

        private void SetDefalultfilter()
        {
            ddlPeriod.SelectedValue = "7";
            this.chbInternalProjects.Checked = false;
            SelectAllItems(this.cblPractices);
            SelectDefaultTimeScaleItems(this.cblTimeScales);
            SelectDefaultPersonStatus(this.cblPersonStatus);
        }

        private void populateGraph(int count, int percentage, Literal ltr, HtmlTableRow tr)
        {
            ltr.Text = count.ToString();
            if (percentage == 0)
            {
                tr.Height = "1px";
            }
            else
            {
                tr.Height = percentage + "px";
            }
        }

        public void PopulateHeaderSection(List<Person> reportData)
        {
            int w2SalaryCount = reportData.Count(p => p.CurrentPay != null && p.CurrentPay.Timescale == TimescaleType.Salary);
            int w2HourlyCount = reportData.Count(p => p.CurrentPay != null && p.CurrentPay.Timescale == TimescaleType.Hourly);
            int contractorCount = reportData.Count(p => p.CurrentPay != null && (p.CurrentPay.Timescale == TimescaleType._1099Ctc || p.CurrentPay.Timescale == TimescaleType.PercRevenue));
            List<int> ratioList = (new int[] { w2SalaryCount, w2HourlyCount, contractorCount }).ToList();
            int height = 80;
            List<int> percentageList = DataTransferObjects.Utils.Generic.GetProportionateRatio(ratioList, height);

            ltPersonCount.Text = reportData.Count + " New Hires";
            lbRange.Text = Range;
            ltrlTotalEmployees.Text = (w2SalaryCount + w2HourlyCount).ToString();
            ltrlTotalContractors.Text = contractorCount.ToString();

            ltrlW2SalaryCount.Text = w2SalaryCount.ToString();
            ltrlW2HourlyCount.Text = w2HourlyCount.ToString();
            ltrlContractorsCount.Text = contractorCount.ToString();

            populateGraph(w2SalaryCount, percentageList[0], ltrlW2SalaryCount, trW2Salary);
            populateGraph(w2HourlyCount, percentageList[1], ltrlW2HourlyCount, trW2Hourly);
            populateGraph(contractorCount, percentageList[2], ltrlContractorsCount, trContrator);
        }

        private void LoadActiveView()
        {
            SetSelectedFilters = true;
            if (mvNewHireReport.ActiveViewIndex == 0)
            {
                tpSummary.PopulateData();
            }
            else
            {
                tpGraph.hlnkGraphHiddenField.Text = NewHireGraphView.SeeNewHiresbyRecruiter;
                tpGraph.PopulateGraph();
            }
        }

        private void SwitchView(Control control, int viewIndex)
        {
            SelectView(control, viewIndex);
            LoadActiveView();
        }

        private void SelectView(Control sender, int viewIndex)
        {
            mvNewHireReport.ActiveViewIndex = viewIndex;

            SetCssClassEmpty();

            ((WebControl)sender.Parent).CssClass = "SelectedSwitch";
        }

        private void SetCssClassEmpty()
        {
            foreach (TableCell cell in tblPersonViewSwitch.Rows[0].Cells)
            {
                cell.CssClass = string.Empty;
            }
        }

        protected string GetDateFormat(DateTime date)
        {
            return date.ToString(Constants.Formatting.EntryDateFormat);
        }

        #endregion

        #region Control Events

        protected void Filters_Changed(object sender, EventArgs e)
        {
            LoadActiveView();
            SaveFilterValuesForSession();
        }

        protected void btnView_Command(object sender, CommandEventArgs e)
        {
            int viewIndex = int.Parse((string)e.CommandArgument);
            SwitchView((Control)sender, viewIndex);
        }

        protected void ddlPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPeriod.SelectedValue != "0")
            {
                LoadActiveView();
                SaveFilterValuesForSession();
            }
            else
            {
                mpeCustomDates.Show();
            }
            
        }

        protected void btnCustDatesOK_Click(object sender, EventArgs e)
        {
            Page.Validate(valSumDateRange.ValidationGroup);
            if (Page.IsValid)
            {
                hdnStartDate.Value = StartDate.Value.Date.ToShortDateString();
                hdnEndDate.Value = EndDate.Value.Date.ToShortDateString();
                LoadActiveView();
                SaveFilterValuesForSession();
            }
            else
            {
                mpeCustomDates.Show();
            }
        }

        protected void btnCustDatesCancel_OnClick(object sender, EventArgs e)
        {
            diRange.FromDate = Convert.ToDateTime(hdnStartDate.Value);
            diRange.ToDate = Convert.ToDateTime(hdnEndDate.Value);
        }

        public void SaveFilterValuesForSession()
        {
            NewHireReportFilters filter = new NewHireReportFilters();
            filter.PracticeIds = cblPractices.SelectedItems;
            filter.ReportPeriod = ddlPeriod.SelectedValue;
            filter.ReportStartDate = diRange.FromDate;
            filter.ReportEndDate = diRange.ToDate;
            filter.ExcludeInternalPractices = chbInternalProjects.Checked;
            filter.PersonStatusIds = cblPersonStatus.SelectedItems;
            filter.PayTypeIds = cblTimeScales.SelectedItems;
            ReportsFilterHelper.SaveFilterValues(ReportName.NewHireReport, filter);
        }

        private void GetFilterValuesForSession()
        {
            var filters = ReportsFilterHelper.GetFilterValues(ReportName.NewHireReport) as NewHireReportFilters;
            if (filters != null)
            {
                cblPractices.UnSelectAll();
                cblPractices.SelectedItems = filters.PracticeIds;
                diRange.FromDate = filters.ReportStartDate;
                diRange.ToDate = filters.ReportEndDate;
                ddlPeriod.SelectedValue = filters.ReportPeriod;
                chbInternalProjects.Checked = filters.ExcludeInternalPractices;
                cblPersonStatus.UnSelectAll();
                cblPersonStatus.SelectedItems = filters.PersonStatusIds;
                cblTimeScales.UnSelectAll();
                cblTimeScales.SelectedItems = filters.PayTypeIds;
            }
        }

        #endregion
    }
}

