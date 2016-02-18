using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.ProjectGroupService;
using DataTransferObjects;
using PraticeManagement.PracticeService;
using PraticeManagement.Controls;
using System.Text;
using PraticeManagement.PersonService;
using DataTransferObjects.Filters;
using PraticeManagement.Utils;

namespace PraticeManagement.Reports
{
    public partial class ProjectFeedbackReport : System.Web.UI.Page
    {
        #region Constants

        private const string QuarterRangeFormat = "Quarter {2} ({0} - {1})";
        private const string YTDRangeFormat = "Year To Date ({0} - {1})";
        private const string CustomRangeFormat = "{0} - {1}";

        #endregion
        public string AccountIds
        {
            get
            {
                if (cblAccount.Items.Count == 0)
                    return "";
                else if (cblAccount.areAllSelected)
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

        public string PayTypes
        {
            get
            {
                return cblPayTypes.areAllSelected ? null : cblPayTypes.SelectedItems;
            }
        }

        public string BusinessGroupIds
        {
            get
            {
                if (cblBusinessGroup.Items.Count == 0)
                    return "";
                else if (cblBusinessGroup.areAllSelected)
                    return null;
                else
                {
                    var clientList = new StringBuilder();
                    foreach (ListItem item in cblBusinessGroup.Items)
                        if (item.Selected)
                            clientList.Append(item.Value).Append(',');
                    return clientList.ToString();
                }
            }
        }

        public string PracticeIds
        {
            get
            {
                if (cblPractices.Items.Count == 0)
                    return "";
                else if (cblPractices.areAllSelected)
                    return null;
                else
                {
                    var clientList = new StringBuilder();
                    foreach (ListItem item in cblPractices.Items)
                        if (item.Selected)
                            clientList.Append(item.Value).Append(',');
                    return clientList.ToString();
                }
            }
        }

        public string DirectorIds
        {
            get
            {
                if (cblDirector.Items.Count == 0)
                    return "";
                else if (cblDirector.areAllSelected)
                    return null;
                else
                {
                    var clientList = new StringBuilder();
                    foreach (ListItem item in cblDirector.Items)
                        if (item.Selected)
                            clientList.Append(item.Value).Append(',');
                    return clientList.ToString();
                }
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
                        if (selectedVal == 30)
                        {
                            return Utils.Calendar.MonthStartDate(now);
                        }
                        else if (selectedVal == 1)
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
                        else if (selectedVal == -30)
                        {
                            return Utils.Calendar.LastMonthStartDate(now);
                        }
                        else
                        {
                            return Utils.Calendar.YearStartDate(now);
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

                        if (selectedVal == 30)
                        {
                            return Utils.Calendar.MonthEndDate(now);
                        }
                        else if (selectedVal == -30)
                        {
                            return Utils.Calendar.LastMonthEndDate(now);
                        }
                        else if (selectedVal == 1)
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
                        else
                        {
                            return now;
                        }
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
                    switch (RangeValueSelected)
                    {
                        case 30:
                        case -30: range = StartDate.Value.ToString(Constants.Formatting.FullMonthYearFormat);
                                  break;
                        case 1:
                        case 2:
                        case 3:
                        case 4: range = string.Format(QuarterRangeFormat, StartDate.Value.ToString(Constants.Formatting.EntryDateFormat), 
                                                      EndDate.Value.ToString(Constants.Formatting.EntryDateFormat), RangeValueSelected); 
                                break;
                        case -1: range = string.Format(YTDRangeFormat, StartDate.Value.ToString(Constants.Formatting.EntryDateFormat), 
                                                       EndDate.Value.ToString(Constants.Formatting.EntryDateFormat)); 
                                 break;
                        default: range = string.Format(CustomRangeFormat, StartDate.Value.ToString(Constants.Formatting.EntryDateFormat), 
                                                       EndDate.Value.ToString(Constants.Formatting.EntryDateFormat)); 
                                 break;
                    }
                }
                return range;
            }
        }

        public int RangeValueSelected
        {
            get
            {
                int selectedValue = -1;
                int.TryParse(ddlPeriod.SelectedValue, out selectedValue);
                return selectedValue;
            }
        }

        public bool IsDefaultPeriodSelected
        {
            get
            {
                return ddlPeriod.SelectedValue == "Please Select";
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

        public bool ExcludeInternalPractices
        {
            get
            {
                return chbExcludeInternal.Checked;
            }
        }

        public bool SetSelectedFilters { get; set; }

        public int CompletedCount { get; set; }

        public int NotCompletedCount { get; set; }

        public int CanceledCount { get; set; }

        public bool UpdateHeaderSection { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillInitAccountsList();
                FillInitBusinessGroupForAccount();
                FillInitPracticesList();
                FillInitDirectorsList();
                DataHelper.FillTimescaleList(this.cblPayTypes, Resources.Controls.AllTypes);
                cblPayTypes.SelectAll();

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

        protected void btnCustDatesOK_Click(object sender, EventArgs e)
        {
            Page.Validate(valSum.ValidationGroup);
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

        protected void btnCustDatesCancel_OnClick(object sender, EventArgs e)
        {
            diRange.FromDate = Convert.ToDateTime(hdnStartDate.Value);
            diRange.ToDate = Convert.ToDateTime(hdnEndDate.Value);
        }

        protected void cstvalPeriodRange_ServerValidate(object source, ServerValidateEventArgs args)
        {
            CustomValidator cvCustomDates = source as CustomValidator;
            DateTime? startDate = diRange.FromDate;
            DateTime? endDate = diRange.ToDate;
            if (startDate.HasValue && endDate.HasValue && startDate != DateTime.MinValue && endDate != DateTime.MinValue)
            {
                args.IsValid = startDate.Value.AddYears(1) > endDate.Value;
            }
            else
            {
                args.IsValid = true;
            }
        }

        private void FillInitAccountsList()
        {
            var allClients = ServiceCallers.Custom.Client(c => c.ClientListAllWithoutPermissions());
            allClients = allClients.Where(c => c.Inactive == false).ToArray();
            DataHelper.FillListDefaultWithEncodedName(cblAccount, "All Accounts", allClients,
                                             false);
            foreach (ListItem item in cblAccount.Items)
            {
                item.Selected = true;
            }
        }

        private void FillInitBusinessGroupForAccount()
        {
            using (var serviceClient = new ProjectGroupServiceClient())
            {
                BusinessGroup[] businessGroupList = serviceClient.GetBusinessGroupList(AccountIds, null);
                businessGroupList = businessGroupList.Where(b => b.IsActive == true).OrderBy(p => p.Client.Name).ThenBy(p => p.Name).ToArray();
                DataHelper.FillListDefault(cblBusinessGroup, "All Business Groups", businessGroupList, false, "Id", "ClientBusinessGroupFormat");
                foreach (ListItem item in cblBusinessGroup.Items)
                {
                    item.Selected = true;
                }
            }
        }

        private void FillInitPracticesList()
        {
            using (var serviceClient = new PracticeServiceClient())
            {
                var practiceList = serviceClient.GetPracticeList();
                practiceList = practiceList.Where(p => p.IsActive == true).ToArray();
                DataHelper.FillListDefaultWithEncodedName(cblPractices, "All Practice Areas", practiceList,
                                            false);
                foreach (ListItem item in cblPractices.Items)
                {
                    item.Selected = true;
                }
            }
        }

        private void FillInitDirectorsList()
        {
            using (var serviceClient = new PersonServiceClient())
            {
                var directorsList = serviceClient.GetPersonListWithRole("Client Director");
                DataHelper.FillListDefaultWithEncodedName(cblDirector, "All Executives in Charge", directorsList,
                                            false);
                foreach (ListItem item in cblDirector.Items)
                {
                    item.Selected = true;
                }
            }
        }

        private void SelectView()
        {
            if (StartDate.HasValue && EndDate.HasValue && AccountIds != "" && BusinessGroupIds != "" && PayTypes != "" && DirectorIds != "" && PracticeIds != "" && !IsDefaultPeriodSelected)
            {
                divWholePage.Style.Remove("display");
                LoadActiveView();
            }
            else
            {
                divWholePage.Style.Add("display", "none");
            }
        }

        private void LoadActiveView()
        {
            int activeView = mvProjectFeedbackReport.ActiveViewIndex;
            switch (activeView)
            {
                case 0:
                    PopulateFeedbackSummaryReport();
                    break;
            }
        }

        private void PopulateFeedbackSummaryReport()
        {
            feedbackSummary.PopulateReport(false);
        }

        protected void ddlPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveFilterValuesForSession();
            if (ddlPeriod.SelectedValue != "0")
            {
                SelectView();
            }
            else
            {
                mpeCustomDates.Show();
            }
            
        }

        protected void cblAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveFilterValuesForSession();
            FillInitBusinessGroupForAccount();
            SelectView();


        }

        protected void cblBusinessGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveFilterValuesForSession();
            SelectView();
            
        }

        protected void chbExcludeInternal_CheckedChanged(object sender, EventArgs e)
        {
            SaveFilterValuesForSession();
            SelectView();
        }

        protected void cblDirector_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveFilterValuesForSession();
            SelectView();
           
        }

        protected void cblPractice_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveFilterValuesForSession();
            SelectView();
            
        }

        protected void cblPayTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveFilterValuesForSession();
            SelectView();
            
        }

        protected void btnView_Command(object sender, CommandEventArgs e)
        {
            int viewIndex = int.Parse((string)e.CommandArgument);
            SetSelectedFilters = true;
            SwitchView((Control)sender, viewIndex);
        }

        private void SwitchView(Control control, int viewIndex)
        {
            SelectView(control, viewIndex);
            SelectView();
        }

        public void SelectView(Control sender, int viewIndex)
        {
            mvProjectFeedbackReport.ActiveViewIndex = viewIndex;

            SetCssClassEmpty();

            ((WebControl)sender.Parent).CssClass = "SelectedSwitch";
        }

        private void SetCssClassEmpty()
        {
            foreach (TableCell cell in tblProjectViewSwitch.Rows[0].Cells)
            {
                cell.CssClass = string.Empty;
            }
        }

        public void PopulateHeaderSection()
        {
            lbRange.Text = Range;
            int totalStatusCount = CanceledCount + NotCompletedCount + CompletedCount;
            ltrlCanceledCount.Text = ltrlCanceled.Text = CanceledCount.ToString();
            ltrlNotCompletedCount.Text = ltrlNotCompleted.Text = NotCompletedCount.ToString();
            ltrlCompletedCount.Text = ltrlCompleted.Text = CompletedCount.ToString();

            if (CanceledCount == 0)
                trCancel.Height = "1px";
            else
            {
                int barHeight = (int)((float)80 * CanceledCount / totalStatusCount);
                trCancel.Height = barHeight.ToString() + "px";
            }

            if (NotCompletedCount == 0)
                trNotCompleted.Height = "1px";
            else
            {
                int barHeight = (int)((float)80 * NotCompletedCount / totalStatusCount);
                trNotCompleted.Height = barHeight.ToString() + "px";
            }

            if (CompletedCount == 0)
                trCompleted.Height = "1px";
            else
            {
                int barHeight = (int)Math.Round((float)80 * CompletedCount / totalStatusCount);
                trCompleted.Height = barHeight.ToString() + "px";
            }
        }

        public void SaveFilterValuesForSession()
        {
            ProjectFeedbackReportFilter filter = new ProjectFeedbackReportFilter();
            filter.PayTypeIds = cblPayTypes.SelectedItems;
            filter.ClientIds = cblAccount.SelectedItems;
            filter.BusinessUnitIds = cblBusinessGroup.SelectedItems;
            filter.ExecutiveInChargeIds = cblDirector.SelectedItems;
            filter.PracticeIds = cblPractices.SelectedItems;
            filter.ExcludeInternalPractices = chbExcludeInternal.Checked;
            filter.ReportPeriod = ddlPeriod.SelectedValue;
            filter.ReportStartDate = diRange.FromDate;
            filter.ReportEndDate = diRange.ToDate;
            ReportsFilterHelper.SaveFilterValues(ReportName.ProjectFeedbackReport, filter);
        }

        private void GetFilterValuesForSession()
        {
            var filters = ReportsFilterHelper.GetFilterValues(ReportName.ProjectFeedbackReport) as ProjectFeedbackReportFilter;
            if (filters != null)
            {
                cblPayTypes.SelectedItems = filters.PayTypeIds;
                cblAccount.SelectedItems = filters.ClientIds;
                FillInitBusinessGroupForAccount();
                cblBusinessGroup.SelectedItems = filters.BusinessUnitIds;
                cblDirector.SelectedItems = filters.ExecutiveInChargeIds;
                cblPractices.SelectedItems = filters.PracticeIds;
                chbExcludeInternal.Checked = filters.ExcludeInternalPractices;
                ddlPeriod.SelectedValue = filters.ReportPeriod;
                diRange.FromDate = filters.ReportStartDate;
                diRange.ToDate = filters.ReportEndDate;
                SelectView();
            }
        }
    }
}

