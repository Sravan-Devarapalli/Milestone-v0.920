using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace PraticeManagement.Reporting
{
    public partial class TimePeriodSummaryReport : System.Web.UI.Page
    {
        #region Properties

        public DateTime StartDate
        {
            get
            {
                var selectedVal = int.Parse(ddlPeriod.SelectedValue);
                if (selectedVal == 0)
                {
                    return diRange.FromDate.Value;
                }
                else
                {
                    var now = Utils.Generic.GetNowWithTimeZone();
                    if (selectedVal > 0)
                    {
                        if (selectedVal == 7)
                        {
                            return Utils.Calendar.WeekStartDate(now);
                        }
                        else if (selectedVal == 30)
                        {
                            return Utils.Calendar.MonthStartDate(now);
                        }
                        else
                        {
                            return Utils.Calendar.YearStartDate(now);
                        }

                    }
                    else if (selectedVal < 0)
                    {
                        if (selectedVal == -7)
                        {
                            return Utils.Calendar.LastWeekStartDate(now);
                        }
                        else if (selectedVal == -30)
                        {
                            return Utils.Calendar.LastMonthStartDate(now);
                        }
                        else
                        {
                            return Utils.Calendar.LastYearStartDate(now);
                        }
                    }
                    else
                    {
                        return diRange.FromDate.Value;
                    }
                }
            }
        }

        public DateTime EndDate
        {
            get
            {
                var selectedVal = int.Parse(ddlPeriod.SelectedValue);
                if (selectedVal == 0)
                {
                    return diRange.ToDate.Value;
                }
                else
                {
                    var now = Utils.Generic.GetNowWithTimeZone();
                    DateTime firstDay = new DateTime(now.Year, now.Month, 1);
                    if (selectedVal > 0)
                    {
                        //7
                        //30
                        //365
                        if (selectedVal == 7)
                        {
                            return Utils.Calendar.WeekEndDate(now);
                        }
                        else if (selectedVal == 30)
                        {
                            return Utils.Calendar.MonthEndDate(now);
                        }
                        else
                        {
                            return Utils.Calendar.YearEndDate(now);
                        }
                    }
                    else if (selectedVal < 0)
                    {
                        if (selectedVal == -7)
                        {
                            return Utils.Calendar.LastWeekEndDate(now);
                        }
                        else if (selectedVal == -30)
                        {
                            return Utils.Calendar.LastMonthEndDate(now);
                        }
                        else
                        {
                            return Utils.Calendar.LastYearEndDate(now);
                        }
                    }
                    else
                    {
                        return diRange.ToDate.Value;
                    }
                }
            }
        }

        public List<DateTime> DatesList
        {
            get
            {
                var list = new List<DateTime>();
                var days = EndDate.Subtract(StartDate).Days;
                if (days <= 7)
                {
                    //Single Day.
                    for (int day = 0; day < EndDate.Subtract(StartDate).Days; day++)
                    {
                        list.Add(StartDate.AddDays(day));
                    }
                }
                else if(days > 7 && days <= 31)
                {
                    //Single Week.
                    for (DateTime day = StartDate; day <= EndDate; day = day.AddDays(6))
                    {
                        list.Add(day);
                    }
                }
                else if (days > 31 && days <= 366)
                {
                    //Single Month.
                    for (DateTime day = StartDate; day <= EndDate; day = Utils.Calendar.MonthEndDate(day))
                    {
                        list.Add(day);
                    }
                }
                return list;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
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
            var clFromDate = diRange.FindControl("clFromDate") as CalendarExtender;
            var clToDate = diRange.FindControl("clToDate") as CalendarExtender;
            hdnStartDateCalExtenderBehaviourId.Value = clFromDate.BehaviorID;
            hdnEndDateCalExtenderBehaviourId.Value = clToDate.BehaviorID;

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
            mvTimePeriodReport.ActiveViewIndex = viewIndex;

            SetCssClassEmpty();

            ((WebControl)sender.Parent).CssClass = "SelectedSwitch";
        }

        private void SetCssClassEmpty()
        {
            foreach (TableCell cell in tblTimePeriodReportViewSwitch.Rows[0].Cells)
            {
                cell.CssClass = string.Empty;
            }
        }

        protected void btnCustDatesOK_Click(object sender, EventArgs e)
        {
            Page.Validate(valSumDateRange.ValidationGroup);
            if (Page.IsValid)
            {
                hdnStartDate.Value = StartDate.Date.ToShortDateString();
                hdnEndDate.Value = EndDate.Date.ToShortDateString();
                LoadActiveView();
            }
            else
            {
                mpeCustomDates.Show();
            }
        }

        protected void ddlPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPeriod.SelectedValue != "0")
            {
                LoadActiveView();
            }
            else
            {
                mpeCustomDates.Show();
            }
        }

        private void LoadActiveView()
        {
            if (mvTimePeriodReport.ActiveViewIndex == 0)
            {
                PopulateByResourceData();
            }
        }

        private void PopulateByResourceData()
        {
            string orderByCerteria = string.Empty;
            var data = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryReportByResource(StartDate, EndDate, null, orderByCerteria));

            tpByResource.DataBindResource(data, DatesList);
        }
    }
}
