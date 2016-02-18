using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using PraticeManagement.Controls;
using DataTransferObjects.Filters;
using PraticeManagement.Utils;
using DataTransferObjects;

namespace PraticeManagement.Reporting
{
    public partial class TimePeriodSummaryReport : Page
    {
        #region Properties

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
                        if (selectedVal > 0)
                        {
                            if (selectedVal == 1)
                            {
                                return Utils.Calendar.QuarterStartDate(now,1);
                            }
                            else if (selectedVal == 2)
                            {
                                return Utils.Calendar.QuarterStartDate(now, 2);
                            }
                            else if (selectedVal == 3)
                            {
                                return Utils.Calendar.QuarterStartDate(now, 3);
                            }
                            else if (selectedVal == 4)
                            {
                                return Utils.Calendar.QuarterStartDate(now, 4);
                            }
                            else if (selectedVal == 7)
                            {
                                return Utils.Calendar.WeekStartDate(now);
                            }
                            else if (selectedVal == 30)
                            {
                                return Utils.Calendar.MonthStartDate(now);
                            }
                            else if (selectedVal == 15)
                            {
                                return Utils.Calendar.PayrollCurrentStartDate(now);
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
                            else if (selectedVal == -15)
                            {
                                return Utils.Calendar.PayrollPerviousStartDate(now);
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
                        DateTime firstDay = new DateTime(now.Year, now.Month, 1);
                        if (selectedVal > 0)
                        {
                            //7
                            //15
                            //30
                            //365
                            if (selectedVal == 1)
                            {
                                return Utils.Calendar.QuarterEndDate(now, 1);
                            }
                            else if (selectedVal == 2)
                            {
                                return Utils.Calendar.QuarterEndDate(now, 2);
                            }
                            else if (selectedVal == 3)
                            {
                                return Utils.Calendar.QuarterEndDate(now, 3);
                            }
                            else if (selectedVal == 4)
                            {
                                return Utils.Calendar.QuarterEndDate(now, 4);
                            }
                            else if (selectedVal == 7)
                            {
                                return Utils.Calendar.WeekEndDate(now);
                            }
                            else if (selectedVal == 15)
                            {
                              return Utils.Calendar.PayrollCurrentEndDate(now);
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
                            else if (selectedVal == -15)
                            {
                                return Utils.Calendar.PayrollPerviousEndDate(now);
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
                return null;
            }
        }

        public String RangeSelected
        {
            get
            {
                return ddlPeriod.SelectedValue;
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

        public String RangeForExcel
        {
            get
            {
                string range = string.Empty;
                if (StartDate.HasValue && EndDate.HasValue)
                {
                    if (StartDate.Value == Utils.Calendar.YearStartDate(StartDate.Value) && EndDate.Value == Utils.Calendar.YearEndDate(StartDate.Value))
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

        public string SelectedView
        {
            get
            {
                return ddlView.SelectedValue;
            }
        }

        public bool IncludePersonWithNoTimeEntries
        {
            get
            {
                return chkIncludePersons.Checked;
            }
        }

        public PraticeManagement.Controls.Reports.TimePeriodSummaryByResource ByResourceControl
        {
            get
            {
                return tpByResource;
            }
        }

        public PraticeManagement.Controls.Reports.TimePeriodSummaryByProject ByProjectControl
        {
            get
            {
                return tpByProject;
            }
        }

        public int AccountId
        {
            get
            {
                int accountId = 0;
                if (ViewState["TimePeriodSummaryReportAccountId"] != null)
                {
                    int.TryParse(ViewState["TimePeriodSummaryReportAccountId"].ToString(), out accountId);
                }
                return accountId;
            }

            set
            {
                ViewState["TimePeriodSummaryReportAccountId"] = value;
            }
        }

        public string BusinessUnitIds
        {
            get
            {
                if (ViewState["TimePeriodSummaryReportBusinessUnitIds"] != null)
                {
                    return ViewState["TimePeriodSummaryReportBusinessUnitIds"].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                ViewState["TimePeriodSummaryReportBusinessUnitIds"] = value;
            }
        }

        public double Total
        {
            get
            {
                if (ViewState["TimePeriodSummaryReportTotal"] != null)
                {
                    return (double)ViewState["TimePeriodSummaryReportTotal"];
                }
                else
                {
                    return 0d;
                }
            }

            set
            {
                ViewState["TimePeriodSummaryReportTotal"] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetFilterValuesForSession();
            }
            var now = Utils.Generic.GetNowWithTimeZone();
            diRange.FromDate = StartDate.HasValue ? StartDate : Utils.Calendar.WeekStartDate(now);
            diRange.ToDate = EndDate.HasValue ? EndDate : Utils.Calendar.WeekEndDate(now);
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

            if (!IsPostBack)
            {
                SelectView();
            }
        }

        private void SelectView()
        {
            if (ddlPeriod.SelectedValue != "Please Select" && ddlView.SelectedValue != "Please Select")
            {
                divWholePage.Style.Remove("display");
                int viewIndex = int.Parse(ddlView.SelectedValue);
                mvTimePeriodReport.ActiveViewIndex = viewIndex;
                LoadActiveView();
            }
            else {
                divWholePage.Style.Add("display", "none");
            }

        }

        protected void btnCustDatesOK_Click(object sender, EventArgs e)
        {
            Page.Validate(valSumDateRange.ValidationGroup);
            if (Page.IsValid)
            {
                hdnStartDate.Value = StartDate.Value.Date.ToShortDateString();
                hdnEndDate.Value = EndDate.Value.Date.ToShortDateString();
                SelectView();
                SaveFilterValuesForSession();
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
                SelectView();
            }
            else
            {
                mpeCustomDates.Show();
            }
            SaveFilterValuesForSession();
        }

        protected void btnCustDatesCancel_OnClick(object sender, EventArgs e)
        {
            diRange.FromDate = Convert.ToDateTime(hdnStartDate.Value);
            diRange.ToDate = Convert.ToDateTime(hdnEndDate.Value);
        }

        protected void ddlView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlView.SelectedValue == "0")
            {
                chkIncludePersons.Enabled = true;
            }
            else if (ddlView.SelectedValue == "1")
            {
                chkIncludePersons.Checked = chkIncludePersons.Enabled = false;
            }
            SelectView();
            SaveFilterValuesForSession();
        }

        protected void chkIncludePersons_CheckedChanged(object sender, EventArgs e)
        {
            SelectView();
            SaveFilterValuesForSession();
        }

        private void LoadActiveView()
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                if (mvTimePeriodReport.ActiveViewIndex == 0)
                {
                    tpByResource.PopulateByResourceData();
                }
                else if (mvTimePeriodReport.ActiveViewIndex == 1)
                {
                    tpByProject.PopulateByProjectData();
                }
                else
                {
                    PopulateByWorkTypeData();
                }
            }
        }

        private void PopulateByWorkTypeData()
        {
            //var data = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryReportByWorkType(StartDate.Value, EndDate.Value, string.Empty, string.Empty));
            //ucByWorktype.DataBindResource(data, DatesList);
            //ucBillableAndNonBillable.BillablValue = (data.Count() > 0) ? data.Sum(d => d.BillabileTotal).ToString() : "0";
            //ucBillableAndNonBillable.NonBillablValue = (data.Count() > 0) ? data.Sum(d => d.NonBillableTotal).ToString() : "0";
        }


        private void SaveFilterValuesForSession()
        {
            TimeReports filter = new TimeReports();
            filter.IncludePersonsWithNoTime = chkIncludePersons.Checked;
            filter.ReportPeriod = ddlPeriod.SelectedValue;
            filter.SelectedView = ddlView.SelectedValue;
            filter.StartDate = diRange.FromDate.Value;
            filter.EndDate = diRange.ToDate.Value;
            ReportsFilterHelper.SaveFilterValues(ReportName.ByTimePeriodReport, filter);
        }

        private void GetFilterValuesForSession()
        {
            var filters = ReportsFilterHelper.GetFilterValues(ReportName.ByTimePeriodReport) as TimeReports;
             if (filters != null)
             {
                 chkIncludePersons.Checked = filters.IncludePersonsWithNoTime;
                 ddlPeriod.SelectedValue = filters.ReportPeriod;
                 ddlView.SelectedValue = filters.SelectedView;
                 diRange.FromDate = filters.StartDate;
                 diRange.ToDate = filters.EndDate;
             }
        }

    }
}

