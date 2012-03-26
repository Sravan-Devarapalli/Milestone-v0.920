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
                return null;
            }
        }

        #endregion

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

        private void SwitchView(Control control, int viewIndex)
        {
            SelectView(control, viewIndex);
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
                hdnStartDate.Value = StartDate.Value.Date.ToShortDateString();
                hdnEndDate.Value = EndDate.Value.Date.ToShortDateString();
                LoadActiveView();
            }
            else
            {
                mpeCustomDates.Show();
            }
        }

        protected void ddlPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPeriod.SelectedValue != "Please Select")
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
            else
            {
                SwitchView(lnkbtnResource, 0);
            }
        }

        private void LoadActiveView()
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                divWholePage.Style.Remove("display");
                if (mvTimePeriodReport.ActiveViewIndex == 0)
                {
                    PopulateByResourceData();
                }
                else if (mvTimePeriodReport.ActiveViewIndex == 1)
                {
                    PopulateByProjectData();
                }
                else
                {
                    PopulateByWorkTypeData();
                }
            }
            else
            {
                divWholePage.Style.Add("display", "none");
            }
        }

        private void PopulateByResourceData()
        {
            var data = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryReportByResource(StartDate.Value, EndDate.Value, null));
            tpByResource.DataBindResource(data);
            int billableHours = (int)data.Sum(p=>p.BillableHours);
            int nonBillableHours = (int)data.Sum(p=>p.NonBillableHours);
            ucBillableAndNonBillable.BillablValue = billableHours.ToString();
            ucBillableAndNonBillable.NonBillablValue = nonBillableHours.ToString();
        }

        private void PopulateByProjectData()
        {
            //string orderByCerteria = string.Empty;
            //var data = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryReportByProject(StartDate.Value, EndDate.Value, null, null, orderByCerteria));
            //tpByProject.DataBindProject(data, DatesList);

            //ucBillableAndNonBillable.BillablValue = (data.Count() > 0) ? data.Sum(d => d.BillabileTotal).ToString() : "0";
            //ucBillableAndNonBillable.NonBillablValue = (data.Count() > 0) ? data.Sum(d => d.NonBillableTotal).ToString() : "0";
        }

        private void PopulateByWorkTypeData()
        {
            //var data = ServiceCallers.Custom.Report(r => r.TimePeriodSummaryReportByWorkType(StartDate.Value, EndDate.Value, string.Empty, string.Empty));
            //ucByWorktype.DataBindResource(data, DatesList);
            //ucBillableAndNonBillable.BillablValue = (data.Count() > 0) ? data.Sum(d => d.BillabileTotal).ToString() : "0";
            //ucBillableAndNonBillable.NonBillablValue = (data.Count() > 0) ? data.Sum(d => d.NonBillableTotal).ToString() : "0";
        }

        public void SetGraphVisibility(bool visible)
        {
            ucBillableAndNonBillable.Visible = visible;
        }
    }
}

