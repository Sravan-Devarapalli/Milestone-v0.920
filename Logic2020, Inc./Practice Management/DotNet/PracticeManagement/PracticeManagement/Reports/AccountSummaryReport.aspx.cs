using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using PraticeManagement.Controls.Reports.ByAccount;
using System.Text;
using PraticeManagement.FilterObjects;
using DataTransferObjects;
using PraticeManagement.ClientService;
using System.ServiceModel;

namespace PraticeManagement.Reporting
{
    public partial class AccountSummaryReport : PracticeManagementPageBase
    {
        #region Properties

        public int? ClientdirectorId
        {
            get
            {
                return ddlDirector.SelectedValue == string.Empty ? null : (int?)Convert.ToInt32(ddlDirector.SelectedValue);
            }
        }

        public string ClientdirectorName
        {
            get
            {
                return ddlDirector.SelectedItem.Text;
            }
        }

        public string AccountIds
        {
            get
            {
                if (cblAccount.Items.Count == 0)
                    return null;
                else
                {
                    var clientList = new StringBuilder();
                    foreach (ListItem item in cblAccount.Items)
                        if (item.Selected)
                            clientList.Append(item.Value).Append(',');
                    return clientList.ToString();
                }
            }
        }

        public bool UpdateHeaderSection { get; set; }

        public int AccountsCount { get { return Convert.ToInt32(ViewState["ACCOUNTSCOUNT_Key"]); } set { ViewState["ACCOUNTSCOUNT_Key"] = value; } }

        public int BusinessUnitsCount { get { return Convert.ToInt32(ViewState["BusinessUnitsCount_Key"]); } set { ViewState["BusinessUnitsCount_Key"] = value; } }

        public int ProjectsCount { get { return Convert.ToInt32(ViewState["ProjectsCount_Key"]); } set { ViewState["ProjectsCount_Key"] = value; } }

        public int PersonsCount { get { return Convert.ToInt32(ViewState["PersonsCount_Key"]); } set { ViewState["PersonsCount_Key"] = value; } }

        public Double TotalProjectHours { get; set; }

        public Double TotalProjectedHours { get; set; }

        public Double BDHours { get; set; }

        public Double BillableHours { get; set; }

        public Double NonBillableHours { get; set; }

        public String HeaderCountText
        {
            get
            {
                return string.Format("{0} Account(s), {1} Business Unit(s), {2} Project(s), {3}", AccountsCount, BusinessUnitsCount, ProjectsCount, PersonsCount.ToString() == "1" ? PersonsCount + " Person" : PersonsCount + " People");
            }
        }

        public String HeaderCountTextForBDView
        {
            get
            {
                return string.Format("{0} Account(s), {1} Business Unit(s), {2}", AccountsCount, BusinessUnitsCount, PersonsCount.ToString() == "1" ? PersonsCount + " Person" : PersonsCount + " People");
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

        //Created for excel report range
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

        public string AccountFilteredIds
        {
            get
            {
                return ViewState["AccountFilteredIds"] as string;
            }
            set
            {
                ViewState["AccountFilteredIds"] = value;
            }
        }

        public string BusinessUnitsFilteredIds
        {
            get
            {
                return ViewState["BusinessUnitsFilteredIds"] as string;
            }
            set
            {
                ViewState["BusinessUnitsFilteredIds"] = value;
            }
        }

        #endregion

        #region Events

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.FillDirectorsList(ddlDirector, "All Client Directors", null);
                FillInitAccountsList();
                FillInitProjectGroupList();
            }
        }

        private void FillInitProjectGroupList()
        {
            DataHelper.FillBusinessUnitsByClients(cblProjectGroup, AccountIds, "All Business Units", false);
            foreach (ListItem item in cblProjectGroup.Items)
            {
                item.Selected = true;
            }
        }

        private void FillInitAccountsList()
        {
            var allClients = ServiceCallers.Custom.Client(c => c.ClientListAllWithoutPermissions());
            DataHelper.FillListDefaultWithEncodedName(cblAccount, "All Accounts", allClients,
                                             false);
            foreach (ListItem item in cblAccount.Items)
            {
                item.Selected = true;
            }
        }

        protected void Page_Prerender(object sender, EventArgs e)
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

            if (UpdateHeaderSection)
            {
                PopulateHeaderSection();
            }
        }

        #endregion

        #region Control Events

        protected void ddlDirector_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlPeriod.SelectedValue = "Please Select";
            SelectView();
        }

        protected void cblAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (var serviceClient = new ClientServiceClient())
            {
                try
                {
                    FillInitProjectGroupList();
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
            SelectView();
        }

        protected void cblProjectGroup_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            BusinessUnitsFilteredIds = null;
            SelectView();
        }

        public bool SetSelectedFilters { get; set; }

        protected void btnView_Command(object sender, CommandEventArgs e)
        {

            int viewIndex = int.Parse((string)e.CommandArgument);

            if (mvAccountReport.ActiveViewIndex != viewIndex && viewIndex != 2)
            {
                SetSelectedFilters = true;


            }

            SwitchView((Control)sender, viewIndex);
        }

        protected void btnCustDatesOK_Click(object sender, EventArgs e)
        {
            Page.Validate(valSumDateRange.ValidationGroup);
            if (Page.IsValid)
            {
                BusinessUnitsFilteredIds = null;
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
            BusinessUnitsFilteredIds = null;
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

        #endregion

        #endregion

        #region Methods

        protected override void Display()
        {
        }

        public void SaveFilters()
        {
            ByAccountReportFilter filter = GetFilterSettings();
            SerializationHelper.SerializeCookie(filter, Constants.FilterKeys.ByAccountReportFitlerCookie);
        }

        private ByAccountReportFilter GetFilterSettings()
        {
            var filter = new ByAccountReportFilter
            {
                ClientDirectorId = ddlDirector.SelectedValue,
                AccountIds = cblAccount.SelectedItems,
                BusinessUnitIds = cblProjectGroup.SelectedItems,
                RangeSelected = ddlPeriod.SelectedValue,
                StartDate = StartDate,
                EndDate = EndDate
            };

            return filter;
        }

        private void SelectView()
        {
            if (StartDate.HasValue && EndDate.HasValue && (AccountIds == null || !string.IsNullOrEmpty(AccountIds)) && (BusinessUnitIds == null || !string.IsNullOrEmpty(BusinessUnitIds)) && RangeSelected != "Please Select")
            {
                divWholePage.Style.Remove("display");
                LoadActiveView();
            }
            else
            {
                divWholePage.Style.Add("display", "none");
            }

            //if (AccountId == 0 && !StartDate.HasValue && !EndDate.HasValue)
            //{
            //    RemoveSavedFilters();
            //}
            //else
            //{
            SaveFilters();
            //}
        }

        private void SwitchView(Control control, int viewIndex)
        {
            SelectView(control, viewIndex);
            SelectView();
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
            ltAccount.Text = HttpUtility.HtmlEncode(ClientdirectorName);
            ltHeaderCount.Text = mvAccountReport.ActiveViewIndex == 2 ? HeaderCountTextForBDView : HeaderCountText;
            ltRange.Text = Range;
            ltrlTotalActualHours.Text = TotalProjectHours.ToString(Constants.Formatting.NumberFormatWithCommasAndDecimals);
            ltrlTotalProjectedHours.Text = TotalProjectedHours.ToString(Constants.Formatting.NumberFormatWithCommasAndDecimals);
            ltrlBDHours.Text = BDHours.ToString(Constants.Formatting.NumberFormatWithCommasAndDecimals);
            ltrlBillableHours.Text = BillableHours.ToString(Constants.Formatting.NumberFormatWithCommasAndDecimals);
            ltrlNonBillableHours.Text = NonBillableHours.ToString(Constants.Formatting.NumberFormatWithCommasAndDecimals);

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

        #endregion
    }
}

