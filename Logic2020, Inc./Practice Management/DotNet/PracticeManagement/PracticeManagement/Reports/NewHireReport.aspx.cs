using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using DataTransferObjects;
using System.Web.UI.HtmlControls;

namespace PraticeManagement.Reporting
{
    public partial class NewHireReport : System.Web.UI.Page
    {
        #region constants

        private const string W2Hourly = "W2-Hourly";
        private const string W2Salary = "W2-Salary";

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
                        else if (selectedVal == 3 || selectedVal == 7)
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
                            range = "Quater" + (RangeSelected - 2) + StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat);
                            break;
                        default:
                            range = StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat);
                            break;
                    }
                }
                return range;
            }
        }

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
                SelectDefaultTimeScaleItems(cblTimeScales);
            }

            AddAttributesToCheckBoxes(this.cblPractices);
            AddAttributesToCheckBoxes(this.cblTimeScales);
            if (hdnFiltersChanged.Value == "false")
            {
                btnResetFilter.Attributes.Add("disabled", "true");
            }
            else
            {
                btnResetFilter.Attributes.Remove("disabled");
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
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
            var tbFrom = diRange.FindControl("tbFrom") as TextBox;
            var tbTo = diRange.FindControl("tbTo") as TextBox;
            var clFromDate = diRange.FindControl("clFromDate") as AjaxControlToolkit.CalendarExtender;
            var clToDate = diRange.FindControl("clToDate") as AjaxControlToolkit.CalendarExtender;
            hdnStartDateTxtBoxId.Value = tbFrom.ClientID;
            hdnEndDateTxtBoxId.Value = tbTo.ClientID;
            hdnStartDateCalExtenderBehaviourId.Value = clFromDate.BehaviorID;
            hdnEndDateCalExtenderBehaviourId.Value = clToDate.BehaviorID;
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

        private void AddAttributesToCheckBoxes(ScrollingDropDown ddl)
        {
            foreach (ListItem item in ddl.Items)
            {
                item.Attributes.Add("onclick", "EnableResetButton();");
            }
        }

        private void SelectAllItems(ScrollingDropDown ddlpractices)
        {
            foreach (ListItem item in ddlpractices.Items)
            {
                item.Selected = true;
            }
        }

        private void Resetfilter()
        {
            ddlPeriod.SelectedValue = "7";
            this.chbActivePersons.Checked = this.chbProjectedPersons.Checked = this.chbTerminatedPersons.Checked = true;
            this.chbInternalProjects.Checked = false;
            SelectAllItems(this.cblPractices);
            SelectDefaultTimeScaleItems(this.cblTimeScales);
        }

        private void populateGraph(int count,int percentage,Literal ltr,HtmlTableRow tr)
        {
            ltr.Text = count.ToString();
            if (percentage == 0)
            {
                tr.Height = "1px";
            }
            else
            {
                tr.Height = percentage +"px";
            }
        }

        private void PopulateHeaderSection(List<Person> reportData)
        {
            int w2SalaryCount = reportData.Count(p => p.CurrentPay.Timescale == TimescaleType.Salary);
            int w2HourlyCount = reportData.Count(p => p.CurrentPay.Timescale == TimescaleType.Hourly);
            int contractorCount = reportData.Count(p => p.CurrentPay.Timescale == TimescaleType._1099Ctc || p.CurrentPay.Timescale == TimescaleType.PercRevenue );
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

        #endregion

        #region Control Events

        protected void btnUpdateView_OnClick(object sender, EventArgs e)
        {
            List<Person> data = ServiceCallers.Custom.Person(r => r.GetStrawmenListAll()).ToList();

            PopulateHeaderSection(data);
        }

        protected void btnResetFilter_OnClick(object sender, EventArgs e)
        {
            Resetfilter();
            hdnFiltersChanged.Value = "false";
            btnResetFilter.Attributes.Add("disabled", "true");
        }

        #endregion
    }
}
