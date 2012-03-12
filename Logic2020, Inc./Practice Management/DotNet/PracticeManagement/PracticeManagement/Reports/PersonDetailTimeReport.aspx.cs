using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using PraticeManagement.Controls;
using DataTransferObjects;


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
                DataHelper.FillPersonList(ddlPerson, null, 1);
                ddlPerson.SelectedValue = DataHelper.CurrentPerson.Id.Value.ToString();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

            lblPersonname.ToolTip = lblPersonname.Text = ddlPerson.SelectedItem.Text;
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

        protected void ddlPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlPeriod.SelectedValue = "7";
            LoadActiveView();
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
                PopulateSummaryDetails();
            }
            else
            {
                PopulatePersonDetailReportDetails();
            }
        }
        private void PopulatePersonDetailReportDetails()
        {
            int personId = Convert.ToInt32(ddlPerson.SelectedValue);
            var list = ServiceCallers.Custom.Report(r => r.PersonTimeEntriesDetails(personId, StartDate, EndDate)).ToList();
            ucpersonDetailReport.DatabindRepepeaterProjectDetails(list);
            Triple<double, double, double> result = ServiceCallers.Custom.Report(r => r.GetPersonTimeEntriesTotalsByPeriod(personId, StartDate, EndDate));
            PopulateTotalSection(result.First, result.Second, result.Third);
        }

        private void PopulateSummaryDetails()
        {
            int personId = Convert.ToInt32(ddlPerson.SelectedValue);
            var list = ServiceCallers.Custom.Report(r => r.PersonTimeEntriesSummary(personId, StartDate, EndDate)).ToList();
            ucpersonSummaryReport.DatabindRepepeaterSummary(list);
            PopulateTotalSection(list.Sum(l => l.BillableHours), list.Sum(l => l.NonBillableHours), list.Sum(l => l.BillableValue));
        }


        private void PopulateTotalSection(double billableHours, double nonBillableHours, double totalValue)
        {
            billableHours = Math.Round(billableHours, 2);
            nonBillableHours = Math.Round(nonBillableHours, 2);
            totalValue = Math.Round(totalValue, 2);
            var billablePercent = 0;
            var nonBillablePercent = 0;
            if (billableHours != 0 || nonBillableHours != 0)
            {
                billablePercent = DataTransferObjects.Utils.Generic.GetBillablePercentage(billableHours, nonBillableHours);
                nonBillablePercent = (100 - billablePercent);
            }

            ltrlBillableHours.Text = billableHours.ToString(Constants.Formatting.DoubleValue);
            ltrlNonBillableHours.Text = nonBillableHours.ToString(Constants.Formatting.DoubleValue);
            ltrlTotalHours.Text = (billableHours + nonBillableHours).ToString(Constants.Formatting.DoubleValueWithZeroPadding);
            ltrlBillablePercent.Text = billablePercent.ToString();
            ltrlNonBillablePercent.Text = nonBillablePercent.ToString();
            ltrlTotalValue.Text = totalValue.ToString("C2");
            
            if (billablePercent == 0)
            {
                trBillable.Visible = false;
                if (nonBillablePercent == 0)
                {
                    trNonBillable.Visible = false;
                }
                else
                {
                    trNonBillable.Visible = true;
                    trNonBillable.Height = "60px";
                }
            }
            else if (billablePercent == 100)
            {
                trBillable.Visible = true;
                trNonBillable.Visible = false;
                trBillable.Height = "60px";
            }
            else
            {
		int billablebarHeight = (int)(((float)60 / (float)100) * billablePercent);
                trBillable.Visible = trNonBillable.Visible = true;
                trBillable.Height = billablebarHeight.ToString() + "px";
                trNonBillable.Height = (60 - billablebarHeight).ToString() + "px";
            }

        }

    }
}

