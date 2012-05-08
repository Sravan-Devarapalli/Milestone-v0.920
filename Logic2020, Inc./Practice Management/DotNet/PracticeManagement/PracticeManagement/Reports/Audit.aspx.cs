using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;

namespace PraticeManagement.Reporting
{
    public partial class Audit : System.Web.UI.Page
    {
        public DateTime? StartDate
        {
            get
            {
                int selectedVal = 0;
                if (int.TryParse(ddlPeriod.SelectedValue, out selectedVal))
                {
                    var now = Utils.Generic.GetNowWithTimeZone();
                    if (selectedVal == 0)
                    {
                        return diRange.FromDate.Value;
                    }
                    else if (selectedVal == 15)
                    {
                        return Utils.Calendar.MonthStartDate(now);
                    }
                    else if (selectedVal == -15)
                    {
                        if (now.Day < 16)
                        {
                            return Utils.Calendar.LastMonthSecondHalfStartDate(now);
                        }
                        else
                        {
                            return Utils.Calendar.CurrentMonthSecondHalfStartDate(now);
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
                    var now = Utils.Generic.GetNowWithTimeZone();
                    if (selectedVal == 0)
                    {
                        return diRange.ToDate.Value;
                    }
                    else if (selectedVal == 15)
                    {

                        return Utils.Calendar.MonthFirstHalfEndDate(now);

                    }
                    else if (selectedVal == -15)
                    {
                        if (now.Day < 16)
                        {
                            return Utils.Calendar.LastMonthEndDate(now);
                        }
                        else
                        {
                            return Utils.Calendar.MonthEndDate(now);
                        }
                    }
                }
                return null;
            }
        }

        public String Range
        {
            get
            {
                string range = string.Empty;
                if (StartDate.HasValue && EndDate.HasValue)
                {
                    if (StartDate.Value == Utils.Calendar.MonthStartDate(StartDate.Value) && EndDate.Value == Utils.Calendar.MonthEndDate(StartDate.Value))
                    {
                        range = StartDate.Value.ToString("MMMM yyyy");
                    }
                    else if (StartDate.Value == Utils.Calendar.YearStartDate(StartDate.Value) && EndDate.Value == Utils.Calendar.YearEndDate(StartDate.Value))
                    {
                        range = StartDate.Value.ToString("yyyy");
                    }
                    else
                    {
                        range = StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat);
                    }
                }
                return range;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var now = Utils.Generic.GetNowWithTimeZone();
            diRange.FromDate = StartDate.HasValue ? StartDate : Utils.Calendar.WeekStartDate(now);
            diRange.ToDate = EndDate.HasValue ? EndDate : Utils.Calendar.WeekEndDate(now);
            lblCustomDateRange.Text = string.Format("({0}&nbsp;-&nbsp;{1})",
                    diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat),
                    diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat)
                    );

            if (ddlPeriod.SelectedValue == "0")
            {
                lblCustomDateRange.Attributes.Add("class", "");
                imgCalender.Attributes.Add("class", "");
            }
            else
            {
                lblCustomDateRange.Attributes.Add("class", "displayNone");
                imgCalender.Attributes.Add("class", "displayNone");
            }

            hdnStartDate.Value = diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat);
            hdnEndDate.Value = diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat);
            if (!IsPostBack)
            {
                var payrollP2Item = ddlPeriod.Items.FindByValue("-15");
                if (now.Day < 16)
                {
                    payrollP2Item.Text = "Payroll - Previous";
                }
                else
                {
                    payrollP2Item.Text = "Payroll - Current";
                }
            }

        }

        protected void btnUpdate_OnClick(object sender, EventArgs e)
        {

        }

        protected void ddlPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPeriod.SelectedValue != "0")
            {
               // SelectView();
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

        protected void btnCustDatesOK_Click(object sender, EventArgs e)
        {
            Page.Validate(valSumDateRange.ValidationGroup);
            if (Page.IsValid)
            {
                hdnStartDate.Value = StartDate.Value.Date.ToShortDateString();
                hdnEndDate.Value = EndDate.Value.Date.ToShortDateString();
                //SelectView();
            }
            else
            {
                mpeCustomDates.Show();
            }
        }

        private void PopulateHeaderSection(List<PersonLevelTimeEntriesHistory> reportDataByPerson, List<ProjectLevelTimeEntriesHistory> reportDataByProject)
        {
            if (reportDataByPerson != null)
            {
                int noOfEmployees = reportDataByPerson.Count;
                ltrCount.Text = noOfEmployees + " Person(s) Affected";
                lbRange.Text = Range;
                ltrlBillableNetChange.Text = reportDataByPerson.Sum(p => p.BillableNetChange).ToString(Constants.Formatting.DoubleValue);
                ltrlNonBillableNetChange.Text = reportDataByPerson.Sum(p => p.NonBillableNetChange).ToString(Constants.Formatting.DoubleValue);
                ltrlNetChange.Text = reportDataByPerson.Sum(p => p.NetChange).ToString(Constants.Formatting.DoubleValue);
            }
            else
            {
                int noOfProjects = reportDataByProject.Count;
                ltrCount.Text = noOfProjects + " Project(s) Affected";
                lbRange.Text = Range;
                ltrlBillableNetChange.Text = reportDataByProject.Sum(p => p.BillableNetChange).ToString(Constants.Formatting.DoubleValue);
                ltrlNonBillableNetChange.Text = reportDataByProject.Sum(p => p.NonBillableNetChange).ToString(Constants.Formatting.DoubleValue);
                ltrlNetChange.Text = reportDataByProject.Sum(p => p.NetChange).ToString(Constants.Formatting.DoubleValue);
            }
        }

    }
}
