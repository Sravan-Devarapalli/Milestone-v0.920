using System;
using DataTransferObjects.Filters;
using DataTransferObjects;
using PraticeManagement.Utils;
using PraticeManagement.Controls;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace PraticeManagement.Reports
{
    public partial class ExpenseReport : System.Web.UI.Page
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
                                return Utils.Calendar.QuarterStartDate(now, 1);
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

                            if (selectedVal == -30)
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
                        if (selectedVal > 0)
                        {
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
                            if (selectedVal == -30)
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

        public int NumberOfMonths
        {
            get
            {
                return Math.Abs((StartDate.Value.Month - EndDate.Value.Month) + 12 * (StartDate.Value.Year - EndDate.Value.Year)) + 1;
            }
        }

        public PraticeManagement.Controls.Reports.ExpenseSummaryByProject ByProjectControl
        {
            get
            {
                return tpByProject;
            }
        }

        public PraticeManagement.Controls.Reports.ExpenseSummaryByType ByExpenseTypeControl
        {
            get
            {
                return tpByExpenseType;
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

        public string AccountsSelected
        {
            get
            {
                return cblAccounts.SelectedItems;
            }
            set
            {
                cblAccounts.SelectedItems = value;
            }
        }

        public string PracticesSelected
        {
            get
            {
                return cblPractices.SelectedItems;
            }
            set
            {
                cblPractices.SelectedItems = value;
            }
        }

        public string DivisionsSelected
        {
            get
            {
                return cblDivisions.SelectedItems;
            }
            set
            {
                cblDivisions.SelectedItems = value;
            }
        }

        public string ExpenseTypesSelected
        {
            get
            {
                return cblExpenseType.SelectedItems;
            }
            set
            {
                cblExpenseType.SelectedItems = value;
            }
        }

        public string ProjectsSelected
        {
            get
            {
                return cblProject.SelectedItems;
            }
            set
            {
                cblProject.SelectedItems = value;
            }
        }

        public bool ShowActualExpense
        {
            get
            {
                return chbActualExpense.Checked;
            }
            set
            {
                chbActualExpense.Checked = value;
            }
        }

        public bool ShowEstimatedExpense
        {
            get
            {
                return chbEstimatedExpense.Checked;
            }
            set
            {
                chbEstimatedExpense.Checked = value;
            }
        }

        public bool showActive
        {
            get
            {
                return chbActive.Checked;
            }
            set
            {
                chbActive.Checked = value;
            }
        }

        public bool showAtRisk
        {
            get
            {
                return chbAtRisk.Checked;
            }
            set
            {
                chbAtRisk.Checked = value;
            }
        }

        public bool showProjected
        {
            get
            {
                return chbprojected.Checked;
            }
            set
            {
                chbprojected.Checked = value;
            }
        }

        public bool showCompleted
        {
            get
            {
                return chbCompleted.Checked;
            }
            set
            {
                chbCompleted.Checked = value;
            }
        }

        public bool showProposed
        {
            get
            {
                return chbProposed.Checked;
            }
            set
            {
                chbProposed.Checked = value;
            }
        }

        public bool showInActive
        {
            get
            {
                return chbInactive.Checked;
            }
            set
            {
                chbInactive.Checked = value;
            }
        }

        public bool showExperimental
        {
            get
            {
                return chbExperimental.Checked;
            }
            set
            {
                chbExperimental.Checked = value;
            }
        }

        public string SelectedView
        {
            get
            {
                return ddlView.SelectedValue;
            }
        }

        private bool IsFromBrowserSession
        {
            get;
            set;
        }


        #endregion Propeties

        #region PageEvents

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateControls();
            }

            AddAttributesToCheckBoxes(cblPractices);
            AddAttributesToCheckBoxes(cblDivisions);
            AddAttributesToCheckBoxes(cblAccounts);
            AddAttributesToCheckBoxes(cblProject);
            AddAttributesToCheckBoxes(cblExpenseType);

            if (hdnFiltersChanged.Value == "false")
            {
                btnResetFilter.Attributes.Add("disabled", "true");
            }
            else
            {
                btnResetFilter.Attributes.Remove("disabled");
            }
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
            var clFromDate = diRange.FindControl("clFromDate") as AjaxControlToolkit.CalendarExtender;
            var clToDate = diRange.FindControl("clToDate") as AjaxControlToolkit.CalendarExtender;
            hdnStartDateCalExtenderBehaviourId.Value = clFromDate.BehaviorID;
            hdnEndDateCalExtenderBehaviourId.Value = clToDate.BehaviorID;

            if (!IsPostBack & IsFromBrowserSession)
            {
                SelectView();

            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "ShowRespectiveFilters();", true);


            }
        }

        protected void cblAccount_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            FillProjectsForClients();
        }

        private void FillProjectsForClients()
        {
            cblProject.Items.Clear();
            DataHelper.FillProjectsForClients(cblProject, AccountsSelected);
            cblProject.SelectAll();
        }
        #endregion PageEvents

        #region Methods

        private void PopulateControls()
        {
            DataHelper.FillPracticeListOnlyActive(cblPractices, Resources.Controls.AllPracticesText);
            cblPractices.SelectAll();
            DataHelper.FillProjectDivisionList(cblDivisions, false, true);
            cblDivisions.SelectAll();
            DataHelper.FillClientList(cblAccounts, "All Accounts");
            cblAccounts.SelectAll();
            DataHelper.FillExpenseTypeList(cblExpenseType, true);
            cblExpenseType.SelectAll();
            FillProjectsForClients();
        }

        public void SelectView(bool isReset = false)
        {
            if (!isReset)
            {
                divWholePage.Style.Remove("display");
                int viewIndex = int.Parse(ddlView.SelectedValue);
                mvExpenseReport.ActiveViewIndex = viewIndex;
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
                if (mvExpenseReport.ActiveViewIndex == 0)
                {
                    tpByProject.SwitchView(tpByProject.LnkbtnSummaryObject, 0);
                }
                else if (mvExpenseReport.ActiveViewIndex == 1)
                {
                    tpByExpenseType.SwitchView(tpByExpenseType.LnkbtnSummaryObject, 0);
                }
            }
        }

        private void SaveFilterValuesForSession()
        {
            ExpenseReportFilters filter = new ExpenseReportFilters();
            filter.ReportPeriod = ddlPeriod.SelectedValue;
            filter.SelectedView = ddlView.SelectedValue;
            filter.StartDate = diRange.FromDate.Value;
            filter.EndDate = diRange.ToDate.Value;
            filter.AccountIds = AccountsSelected;
            filter.DivisionIds = DivisionsSelected;
            filter.PracticeIds = PracticesSelected;
            filter.ExpenseTypeIds = ExpenseTypesSelected;
            filter.IncudeActuals = ShowActualExpense;
            filter.IncludeEstimated = ShowEstimatedExpense;
            filter.IsActive = showActive;
            filter.IsCompleted = showCompleted;
            filter.IsExperimental = showExperimental;
            filter.IsInactive = showInActive;
            filter.IsProjected = showProjected;
            filter.IsProposed = showProposed;
            filter.IsAtRisk = showAtRisk;
            ReportsFilterHelper.SaveFilterValues(ReportName.ExpenseReport, filter);
        }

        private void GetFilterValuesForSession()
        {
            var filters = ReportsFilterHelper.GetFilterValues(ReportName.ExpenseReport) as ExpenseReportFilters;
            if (filters != null)
            {
                ddlPeriod.SelectedValue = filters.ReportPeriod;
                ddlView.SelectedValue = filters.SelectedView;
                diRange.FromDate = filters.StartDate;
                diRange.ToDate = filters.EndDate;
                AccountsSelected = filters.AccountIds;
                DivisionsSelected = filters.DivisionIds;
                PracticesSelected = filters.PracticeIds;
                ExpenseTypesSelected = filters.ExpenseTypeIds;
                FillProjectsForClients();
                ShowActualExpense = filters.IncudeActuals;
                ShowEstimatedExpense = filters.IncludeEstimated;
                showActive = filters.IsActive;
                showCompleted = filters.IsCompleted;
                showExperimental = filters.IsExperimental;
                showInActive = filters.IsInactive;
                showProjected = filters.IsProjected;
                showProposed = filters.IsProposed;
                showAtRisk = filters.IsAtRisk;
                IsFromBrowserSession = true;
            }
            else
            {
                Reset();
                IsFromBrowserSession = false;
            }
        }

        private void Reset()
        {
            ddlPeriod.SelectedValue = "30";
            ddlView.SelectedValue = "0";
            ShowActualExpense = true;
            ShowEstimatedExpense = true;
            showActive = true;
            showCompleted = true;
            showExperimental = false;
            showInActive = false;
            showProjected = true;
            showProposed = true;
            showAtRisk = true;
            cblAccounts.SelectAll();
            cblPractices.SelectAll();
            cblDivisions.SelectAll();
            cblPractices.SelectAll();
            cblExpenseType.SelectAll();
            FillProjectsForClients();
            cblProject.SelectAll();
            hdnFiltersChanged.Value = "false";
            btnResetFilter.Attributes.Add("disabled", "true");
            SelectView(true);
            upnlBody.Update();
        }
        protected void btnUpdateView_OnClick(object sender, EventArgs e)
        {
            SelectView();
            upnlBody.Update();
            SaveFilterValuesForSession();
        }
        protected void btnResetFilter_OnClick(object sender, EventArgs e)
        {
            Reset();
        }

        private void AddAttributesToCheckBoxes(ScrollingDropDown ddlpractices)
        {
            foreach (System.Web.UI.WebControls.ListItem item in ddlpractices.Items)
            {
                item.Attributes.Add("onclick", "EnableResetButton();");
            }
        }
        #endregion Methods
    }
}

