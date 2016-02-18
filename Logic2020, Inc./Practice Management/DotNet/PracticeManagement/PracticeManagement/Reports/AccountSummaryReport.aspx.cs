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
using PraticeManagement.Utils;
using DataTransferObjects.Filters;

namespace PraticeManagement.Reporting
{
    public partial class AccountSummaryReport : PracticeManagementPageBase
    {
        #region Properties

        public int AccountId
        {
            get
            {
                int accountId = 0;
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

        public string ProjectStatusIds
        {
            get
            {
                if (cblProjectStatus.Items.Count == 0)
                    return null;
                else
                {
                    var clientList = new StringBuilder();
                    foreach (ListItem item in cblProjectStatus.Items)
                        if (item.Selected)
                            clientList.Append(item.Value).Append(',');
                    return clientList.ToString();
                }
            }
        }

        public bool UpdateHeaderSection { get; set; }

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
                return string.Format("{0} Business Unit(s), {1} Project(s), {2}", BusinessUnitsCount, ProjectsCount, PersonsCount.ToString() == "1" ? PersonsCount + " Person" : PersonsCount + " People");
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
                var allClients = ServiceCallers.Custom.Client(c => c.ClientListAllWithoutPermissions());
                DataHelper.FillListDefault(ddlAccount, "- - Select Account - -", allClients, false);

                //var cookie = SerializationHelper.DeserializeCookie(Constants.FilterKeys.ByAccountReportFitlerCookie) as ByAccountReportFilter;
                //if (cookie != null)
                //{
                //    var targetItem = ddlAccount.Items.FindByValue(cookie.AccountId);
                //    if (targetItem != null)
                //    {
                //        ddlAccount.SelectedValue = targetItem.Value;

                //        if (ddlAccount.SelectedIndex != 0)
                //        {
                //            DataHelper.FillProjectGroupListWithInactiveGroups(cblProjectGroup, Convert.ToInt32(ddlAccount.SelectedValue), null, "All Business Units", false);

                //            cblProjectGroup.SelectedItems = cookie.BusinessUnitIds;
                //        }
                //        else
                //        {
                //            FillInitProjectGroupList();
                //        }

                //    }
                //    else
                //    {
                //        FillInitProjectGroupList();
                //    }

                //    var targetItems = ddlPeriod.Items.FindByValue(cookie.RangeSelected.ToString());

                //    ddlPeriod.SelectedValue = targetItems.Value;

                //    if (cookie.RangeSelected.ToString() == "0")
                //    {
                //        diRange.FromDate = cookie.StartDate;
                //        diRange.ToDate = cookie.EndDate;
                //    }

                //    //SelectView();//Updating in Prerender
                //}
                //else
                //{
                FillInitProjectGroupList();
                //}

                //ddlAccount.SelectedValue = "";
               
                //ddlAccount_SelectedIndexChanged(ddlAccount, new EventArgs());

                ddlPeriod.SelectedValue = "30";//This Month - as per 3201 
                DataHelper.FillProjectStatusList(cblProjectStatus, "All Project Statuses", null);
                cblProjectStatus.SelectAll();

            }

        }

        protected void cblProjectStatus_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            SelectView();
            SaveFilterValuesForSession();
        }

        private void FillInitProjectGroupList()
        {
            cblProjectGroup.DataSource = new List<ListItem> { new ListItem("All Business Units", String.Empty) };
            cblProjectGroup.DataBind();

            foreach (ListItem item in cblProjectGroup.Items)
            {
                item.Selected = true;
            }
        }

        protected void Page_Prerender(object sender, EventArgs e)
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

            if (UpdateHeaderSection)
            {
                PopulateHeaderSection();
            }
        }

        #endregion

        #region Control Events

        protected void ddlAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Fill BusinessUnits.
            BusinessUnitsFilteredIds = null;

            FillProjectGroup();
           
            //ddlPeriod.SelectedValue = "Please Select";
            SelectView();

            SaveFilterValuesForSession();

        }

        protected void cblProjectGroup_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            BusinessUnitsFilteredIds = null;
            SelectView();
            SaveFilterValuesForSession();
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
                SaveFilterValuesForSession();
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
            SaveFilterValuesForSession();
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
                AccountId = ddlAccount.SelectedValue,
                BusinessUnitIds = cblProjectGroup.SelectedItems,
                ProjectStatusIds = cblProjectStatus.SelectedItems,
                RangeSelected = ddlPeriod.SelectedValue,
                StartDate = StartDate,
                EndDate = EndDate
            };

            return filter;
        }

        private void SelectView()
        {
            if (StartDate.HasValue && EndDate.HasValue && AccountId != 0 && (BusinessUnitIds == null || !string.IsNullOrEmpty(BusinessUnitIds)))
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

        private void FillProjectGroup()
        {
            if (ddlAccount.SelectedIndex != 0)
            {
                DataHelper.FillProjectGroupListWithInactiveGroups(cblProjectGroup, Convert.ToInt32(ddlAccount.SelectedValue), null, "All Business Units", false);

                foreach (ListItem item in cblProjectGroup.Items)
                {
                    item.Selected = true;
                }
            }
            else
            {
                DataHelper.FillListDefaultWithEncodedName(cblProjectGroup, "All Business Units", null,
                                             false);
            }
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
            ltAccount.Text = HttpUtility.HtmlEncode(AccountName);
            ltHeaderCount.Text = HeaderCountText;
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

        private void SaveFilterValuesForSession()
        {
            AccountSummaryFilters filter = new AccountSummaryFilters();
            filter.AccountId = ddlAccount.SelectedValue;
            filter.BusinessUnitIds = BusinessUnitIds;
            filter.ReportPeriod = RangeSelected;
            filter.ReportStartDate = StartDate;
            filter.ReportEndDate = EndDate;
            filter.ProjectStatusIds = ProjectStatusIds;
            ReportsFilterHelper.SaveFilterValues(ReportName.AccountSummaryReport, filter);
        }

        private void GetFilterValuesForSession()
        {
            var filters = ReportsFilterHelper.GetFilterValues(ReportName.AccountSummaryReport) as AccountSummaryFilters;
            if (filters != null)
            {
                ddlPeriod.SelectedValue = filters.ReportPeriod;
                ddlAccount.SelectedValue = filters.AccountId;
                FillProjectGroup();
                cblProjectGroup.SelectedItems = filters.BusinessUnitIds;
                diRange.FromDate = filters.ReportStartDate;
                diRange.ToDate = filters.ReportEndDate;
                cblProjectStatus.SelectedItems = filters.ProjectStatusIds;
            }
        }
        #endregion
    }
}

