﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using PraticeManagement.Controls;


namespace PraticeManagement.Reporting
{
    public partial class PersonDetailTimeReport : System.Web.UI.Page
    {

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
                        //7
                        //30
                        //365
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.FillPersonList(ddlPerson, "Please Select a Person", 1);
                ddlPerson.SelectedValue = DataHelper.CurrentPerson.Id.Value.ToString();
            }
        }

        protected void ddlPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadActiveView();
        }

        protected void btnView_Command(object sender, CommandEventArgs e)
        {
            int viewIndex = int.Parse((string)e.CommandArgument);
            SelectView((Control)sender, viewIndex);
            LoadActiveView();
        }


        private void SetCssClassEmpty()
        {
            foreach (TableCell cell in tblPersonViewSwitch.Rows[0].Cells)
            {
                cell.CssClass = string.Empty;
            }
        }

        private void SelectView(Control sender, int viewIndex)
        {
            mvPersonDetailReport.ActiveViewIndex = viewIndex;

            SetCssClassEmpty();

            ((WebControl)sender.Parent).CssClass = "SelectedSwitch";
        }

        private void LoadActiveView()
        {
            if (mvPersonDetailReport.ActiveViewIndex == 0)
            {
                int personId =Convert.ToInt32(ddlPerson.SelectedValue);

                var list = ServiceCallers.Custom.Report(r=>r.PersonTimeEntriesSummary(personId,StartDate,EndDate)).ToList();
                ucpersonSummaryReport.DatabindRepepeaterSummary(list);
            }
            else
            {
                int personId = Convert.ToInt32(ddlPerson.SelectedValue);
                var list = ServiceCallers.Custom.Report(r => r.PersonTimeEntriesDetails(personId, StartDate, EndDate)).ToList();
                ucpersonDetailReport.DatabindRepepeaterProjectDetails(list);
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
            var tbFrom = diRange.FindControl("tbFrom") as TextBox;
            var tbTo = diRange.FindControl("tbTo") as TextBox;
            var clFromDate = diRange.FindControl("clFromDate") as CalendarExtender;
            var clToDate = diRange.FindControl("clToDate") as CalendarExtender;
            hdnStartDateTxtBoxId.Value = tbFrom.ClientID;
            hdnEndDateTxtBoxId.Value = tbTo.ClientID;
            hdnStartDateCalExtenderBehaviourId.Value = clFromDate.BehaviorID;
            hdnEndDateCalExtenderBehaviourId.Value = clToDate.BehaviorID;

            if (!IsPostBack)
            {
                LoadActiveView();
            }

        }
    }
}
