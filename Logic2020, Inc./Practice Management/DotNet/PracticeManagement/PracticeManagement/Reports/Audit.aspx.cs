﻿using System;
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
                        return Utils.Calendar.PayrollCurrentStartDate(now);
                    }
                    else if (selectedVal == -15)
                    {
                        return Utils.Calendar.PayrollPerviousStartDate(now);
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
                        return Utils.Calendar.PayrollCurrentEndDate(now);
                    }
                    else if (selectedVal == -15)
                    {
                        return Utils.Calendar.PayrollPerviousEndDate(now);
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
        }

        protected void btnUpdate_OnClick(object sender, EventArgs e)
        {
            SelectView(1);
            btnUpdate.Enabled = false;
        }

        public void SelectView(int viewIndex)
        {
            if (ddlPeriod.SelectedValue != "Please Select")
            {
                divWholePage.Style.Remove("display");
                mvTimePeriodReport.ActiveViewIndex = viewIndex;
                LoadActiveView();
            }
            else
            {
                divWholePage.Style.Add("display", "none");
            }

        }

        private void LoadActiveView()
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                if (mvTimePeriodReport.ActiveViewIndex == 0)
                {
                    var reportDataByPerson = ServiceCallers.Custom.Report(r => r.TimeEntryAuditReportByPerson(StartDate.Value, EndDate.Value));
                    PopulateHeaderSection(reportDataByPerson, null);
                    tpByResource.PopulateByResourceData(reportDataByPerson);
                }
                else if (mvTimePeriodReport.ActiveViewIndex == 1)
                {
                    var reportDataByProject = ServiceCallers.Custom.Report(r => r.TimeEntryAuditReportByProject(StartDate.Value, EndDate.Value));
                    PopulateHeaderSection(null, reportDataByProject);
                    tpByProject.PopulateByProjectData(reportDataByProject);
                }
            }
        }

        protected void ddlPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPeriod.SelectedValue == "0")
            {
                mpeCustomDates.Show();
            }
            else
            {
                hdnperiodValue.Value = ddlPeriod.SelectedValue;
                btnUpdate.Enabled = true;
                divWholePage.Style.Add("display", "none");
            }
        }

        protected void btnCustDatesCancel_OnClick(object sender, EventArgs e)
        {
            ddlPeriod.SelectedValue = hdnperiodValue.Value;
            diRange.FromDate = Convert.ToDateTime(hdnStartDate.Value);
            diRange.ToDate = Convert.ToDateTime(hdnEndDate.Value);
        }

        protected void btnCustDatesOK_Click(object sender, EventArgs e)
        {
            Page.Validate(valSumDateRange.ValidationGroup);
            if (Page.IsValid)
            {
                hdnperiodValue.Value = ddlPeriod.SelectedValue;
                hdnStartDate.Value = StartDate.Value.Date.ToShortDateString();
                hdnEndDate.Value = EndDate.Value.Date.ToShortDateString();
                btnUpdate.Enabled = true;
                divWholePage.Style.Add("display", "none");
            }
            else
            {
                mpeCustomDates.Show();
            }
        }

        private void PopulateHeaderSection(PersonLevelTimeEntriesHistory[] reportDataByPerson, ProjectLevelTimeEntriesHistory[] reportDataByProject)
        {
            if (reportDataByPerson != null)
            {
                int noOfEmployees = reportDataByPerson.Count();
                ltrCount.Text = noOfEmployees + " Person(s) Affected";
                lbRange.Text = Range;
                ltrlBillableNetChange.Text = reportDataByPerson.Sum(p => p.BillableNetChange).ToString(Constants.Formatting.DoubleValue);
                ltrlNonBillableNetChange.Text = reportDataByPerson.Sum(p => p.NonBillableNetChange).ToString(Constants.Formatting.DoubleValue);
                ltrlNetChange.Text = reportDataByPerson.Sum(p => p.NetChange).ToString(Constants.Formatting.DoubleValue);
            }
            else
            {
                int noOfProjects = reportDataByProject.Count();
                ltrCount.Text = noOfProjects + " Project(s) Affected";
                lbRange.Text = Range;
                ltrlBillableNetChange.Text = reportDataByProject.Sum(p => p.BillableNetChange).ToString(Constants.Formatting.DoubleValue);
                ltrlNonBillableNetChange.Text = reportDataByProject.Sum(p => p.NonBillableNetChange).ToString(Constants.Formatting.DoubleValue);
                ltrlNetChange.Text = reportDataByProject.Sum(p => p.NetChange).ToString(Constants.Formatting.DoubleValue);
            }
        }

    }
}
