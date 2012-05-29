using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using PraticeManagement.Controls.Reports.ByAccount;

namespace PraticeManagement.Reporting
{
    public partial class AccountSummaryReport : PracticeManagementPageBase
    {
        #region Properties

        public int AccountId
        {
            get
            {
                int accountId = -1;
                int.TryParse(ddlAccount.SelectedValue, out accountId);
                return accountId;
            }
        }

        public String AccountName
        {
            get
            {
                return ddlAccount.SelectedItem.Text;
            }
        }

        public bool UpdateHeaderSection { get; set; }

        public int BusinessUnitsCount { get; set; }

        public int ProjectsCount { get; set; }
        
        public int PersonsCount { get; set; }

        public Double TotalProjectHours { get; set; }
        
        public Double BDHours { get; set; }
        
        public Double BillableHours { get; set; }
        
        public Double NonBillableHours { get; set; }

        public String HeaderCountText
        {
            get
            {
                return string.Format("{0} BUs, {1} Projects, {2} Persons", BusinessUnitsCount, ProjectsCount, PersonsCount);
            }
        }

        public ByBusinessDevelopment ByBusinessDevelopmentControl
        {
            get
            {
                return tpByBusinessDevelopment;
            }
        }

        public String BusinessUnitIds
        {
            get
            {
                return cblProjectGroup.SelectedItems;
            }
        }

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
                            if (selectedVal == 7)
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

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var allClients = ServiceCallers.Custom.Client(c => c.ClientListAllWithoutPermissions());
                DataHelper.FillListDefault(ddlAccount, "- - Select Account - -", allClients, false);
                cblProjectGroup.Items.Add(new ListItem("All Business Units", String.Empty));
                cblProjectGroup.DataBind();
            }

        }

        protected void ddlAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Fill BusinessUnits.
            if (ddlAccount.SelectedIndex != 0)
            {
                DataHelper.FillProjectGroupList(cblProjectGroup, Convert.ToInt32(ddlAccount.SelectedValue), null, "All Business Units", false);

                foreach (ListItem item in cblProjectGroup.Items)
                {
                    item.Selected = true;
                }
            }
            else
            {
                cblProjectGroup.Items.Clear();
                cblProjectGroup.Items.Add(new ListItem("All Business Units", String.Empty));
                cblProjectGroup.DataBind();
            }
        }

        protected override void Display()
        {
            string rangeSelected = Request.QueryString[Constants.QueryStringParameterNames.RangeArgument];
            if (!string.IsNullOrEmpty(rangeSelected))
            {
                if (ddlPeriod.Items.FindByValue(rangeSelected) != null)
                {
                    ddlPeriod.SelectedValue = rangeSelected;
                }
                if (ddlPeriod.SelectedValue == "0")
                {
                    DateTime? startDate = GetArgumentDateTime(Constants.QueryStringParameterNames.StartDateArgument);
                    DateTime? endDate = GetArgumentDateTime(Constants.QueryStringParameterNames.EndDateArgument);
                    var now = Utils.Generic.GetNowWithTimeZone();
                    diRange.FromDate = startDate.HasValue ? startDate : Utils.Calendar.WeekStartDate(now);
                    diRange.ToDate = endDate.HasValue ? endDate : Utils.Calendar.WeekEndDate(now);
                }
            }
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            if (timeEntryReportHeader.Count == 1)
            {
                tdFirst.Style["width"] = timeEntryReportHeader.TdFirstWidth;
                tdSecond.Style["width"] = timeEntryReportHeader.TdSecondWidth;
                tdThird.Style["width"] = timeEntryReportHeader.TdThirdWidth;
            }
            else if (timeEntryReportHeader.Count == 2)
            {
                tdFirst.Style["width"] = "23%";
                tdSecond.Style["width"] = "30%";
                tdThird.Style["width"] = "47%";
            }
            else if (timeEntryReportHeader.Count == 3)
            {
                tdFirst.Style["width"] = "10%";
                tdSecond.Style["width"] = "30%";
                tdThird.Style["width"] = "60%";
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
                SelectView();
            }

            if (UpdateHeaderSection)
            {
                PopulateHeaderSection();
            }
        }

        protected void btnView_Command(object sender, CommandEventArgs e)
        {
            int viewIndex = int.Parse((string)e.CommandArgument);
            SwitchView((Control)sender, viewIndex);
        }

        private void SelectView()
        {
            if (ddlPeriod.SelectedValue != "Please Select")
            {
                divWholePage.Style.Remove("display");
                LoadActiveView();
            }
            else
            {
                divWholePage.Style.Add("display", "none");
            }

        }

        private void SwitchView(Control control, int viewIndex)
        {
            SelectView(control, viewIndex);
            LoadActiveView();
        }

        private void SetCssClassEmpty()
        {
            foreach (TableCell cell in tblProjectViewSwitch.Rows[0].Cells)
            {
                cell.CssClass = string.Empty;
            }
        }

        public void SelectView(Control sender, int viewIndex)
        {
            mvAccountReport.ActiveViewIndex = viewIndex;

            SetCssClassEmpty();

            ((WebControl)sender.Parent).CssClass = "SelectedSwitch";
        }

        private void LoadActiveView()
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                int activeView = mvAccountReport.ActiveViewIndex;
                switch (activeView)
                {
                    case 0:
                        PopulateByBusinessUnitReport();
                        break;
                    case 1:
                        PopulateByProjectReport();
                        break;
                    case 2:
                        PopulateByBusinessDevelopmentReport();
                        break;
                }
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
        }

        protected void btnCustDatesCancel_OnClick(object sender, EventArgs e)
        {
            diRange.FromDate = Convert.ToDateTime(hdnStartDate.Value);
            diRange.ToDate = Convert.ToDateTime(hdnEndDate.Value);
        }

        private void PopulateByBusinessUnitReport()
        {
            tpByBusinessUnit.PopulateByBusinessUnitReport();
        }

        private void PopulateByProjectReport()
        {
            tpByProject.PopulateByProjectData();
        }

        private void PopulateByBusinessDevelopmentReport()
        {
            tpByBusinessDevelopment.PopulateByBusinessDevelopment();
        }

        private void PopulateHeaderSection()
        {
            ltAccount.Text = AccountName;
            ltHeaderCount.Text = HeaderCountText;
            ltRange.Text = Range;

            ltrlTotalProjectHours.Text = TotalProjectHours.ToString(Constants.Formatting.DoubleValue);
            ltrlBDHours.Text = BDHours.ToString(Constants.Formatting.DoubleValue);
            ltrlBillableHours.Text = BillableHours.ToString(Constants.Formatting.DoubleValue);
            ltrlNonBillableHours.Text = NonBillableHours.ToString(Constants.Formatting.DoubleValue);

            var billablePercent = 0;
            var nonBillablePercent = 0;
            if (BillableHours != 0 || NonBillableHours != 0)
            {
                billablePercent = DataTransferObjects.Utils.Generic.GetBillablePercentage(BillableHours, NonBillableHours);
                nonBillablePercent = (100 - billablePercent);
            }

            ltrlBillablePercent.Text = billablePercent.ToString();
            ltrlNonBillablePercent.Text = nonBillablePercent.ToString();


            if (billablePercent == 0 && nonBillablePercent == 0)
            {
                trBillable.Height = "1px";
                trNonBillable.Height = "1px";
            }
            else if (billablePercent == 100)
            {
                trBillable.Height = "80px";
                trNonBillable.Height = "1px";
            }
            else if (billablePercent == 0 && nonBillablePercent == 100)
            {
                trBillable.Height = "1px";
                trNonBillable.Height = "80px";
            }
            else
            {
                int billablebarHeight = (int)(((float)80 / (float)100) * billablePercent);
                trBillable.Height = billablebarHeight.ToString() + "px";
                trNonBillable.Height = (80 - billablebarHeight).ToString() + "px";
            }
        }
    }
}

