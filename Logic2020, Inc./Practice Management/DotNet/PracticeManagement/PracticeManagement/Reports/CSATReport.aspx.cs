using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using PraticeManagement.Utils;
using DataTransferObjects;
using DataTransferObjects.Filters;

namespace PraticeManagement.Reports
{
    public partial class CSATReport : System.Web.UI.Page
    {
      
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
                        if (selectedVal == -1)
                        {
                            return now;
                        }
                        else if (selectedVal == 1)
                        {
                            return Utils.Calendar.MonthStartDate(now);
                        }
                        else if (selectedVal == 2)
                        {
                            return Utils.Calendar.LastMonthStartDate(now);
                        }
                        else if (selectedVal == 3 || selectedVal == 7)
                        {
                            return Utils.Calendar.QuarterStartDate(now, 1);
                        }
                        else if (selectedVal == 4)
                        {
                            return Utils.Calendar.QuarterStartDate(now, 2);
                        }
                        else if (selectedVal == 5)
                        {
                            return Utils.Calendar.QuarterStartDate(now, 3);
                        }
                        else
                        {
                            return Utils.Calendar.QuarterStartDate(now, 4);
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
                        if (selectedVal == 1)
                        {
                            return Utils.Calendar.MonthEndDate(now);
                        }
                        else if (selectedVal == 2)
                        {
                            return Utils.Calendar.LastMonthEndDate(now);
                        }
                        else if (selectedVal == 3)
                        {
                            return Utils.Calendar.QuarterEndDate(now, 1);
                        }
                        else if (selectedVal == 4)
                        {
                            return Utils.Calendar.QuarterEndDate(now, 2);
                        }
                        else if (selectedVal == 5)
                        {
                            return Utils.Calendar.QuarterEndDate(now, 3);
                        }
                        else if (selectedVal == 6)
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

        public System.Web.UI.HtmlControls.HtmlTable HeaderTable
        {
            get
            {
                return tblHeader;
            }
        }

        public int RangeSelected
        {
            get
            {
                int selectedValue = 0;
                int.TryParse(ddlPeriod.SelectedValue, out selectedValue);
                return selectedValue;
            }
        }

        public String Range
        {
            get
            {
                string range = string.Empty;
                if (StartDate.HasValue && EndDate.HasValue)
                {
                    switch (RangeSelected)
                    {
                        case 1:
                        case 2:
                            range = StartDate.Value.ToString("MMMM yyyy");
                            break;
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                            range = "Quarter " + (RangeSelected - 2).ToString() + " (" + StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat) + ")";
                            break;
                        case 7:
                            range = "Year To Date (" + StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat) + ")";
                            break;
                        default:
                            range = StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat);
                            break;
                    }
                }
                return range;
            }
        }

        public string SelectedPractices
        {
            get
            {
                return cblPractices.SelectedItems;
            }
        }

        public string SelectedAccounts
        {
            get
            {
                return cblAccount.SelectedItems;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (this.cblPractices != null && this.cblPractices.Items.Count == 0)
                {
                    DataHelper.FillPracticeListOnlyActive(this.cblPractices, Resources.Controls.AllPracticesText);
                }

                if (this.cblAccount != null && this.cblAccount.Items.Count == 0)
                {
                    DataHelper.FillClientList(this.cblAccount, Resources.Controls.AllClientsText);
                }
                cblPractices.SelectAll();
                cblAccount.SelectAll();
                GetFilterValuesForSession();
            }
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
                lblCustomDateRange.Attributes.Add("class", "fontBold");
                imgCalender.Attributes.Add("class", "");
                divCustomDates.Visible = true;
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
                SaveFilterValuesForSession();
            }


        }

        protected void ddlPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPeriod.SelectedValue != "0")
            {
                if (ddlPeriod.SelectedValue == "-1")
                {
                    hdnPeriod.Value = ddlPeriod.SelectedValue;
                }
                divCustomDates.Visible = false;
                SelectView();
                SaveFilterValuesForSession();
            }
            else
            {
                divCustomDates.Visible = true;
                mpeCustomDates.Show();
            }
        }

        protected void Filters_Changed(object sender, EventArgs e)
        {
            if (cblAccount.SelectedItems == "" || cblPractices.SelectedItems == "")
            {
                divReport.Visible = false;
            }
            else
            {
                LoadActiveView();
                SaveFilterValuesForSession();
            }
        }

        private void LoadActiveView()
        {

            int activeView = mvCSATReport.ActiveViewIndex;
            if (activeView == 0)
            {
                if (SelectedAccounts != string.Empty && SelectedPractices != string.Empty && ddlPeriod.SelectedValue !="-1")
                {
                    divReport.Visible = true;
                    hdnPeriod.Value = ddlPeriod.SelectedValue;
                    ucCSATSummary.PopulateData();
                }
                else
                {
                    divReport.Visible = false;
                }
            }
        }

        private void SelectView()
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                LoadActiveView();
            }
        }

        public void PopulateHeaderSection()
        {
            lbRange.Text = Range;
            List<int> cSATVaraibles = ServiceCallers.Custom.Project(p => p.CSATReportHeader(StartDate.Value, EndDate.Value, SelectedPractices, SelectedAccounts)).ToList();
            int noOfCompletedCSATsWithoutFilters = cSATVaraibles[0] + cSATVaraibles[1] + cSATVaraibles[2];
            int noOfCompletedCSATsWithFilters = cSATVaraibles[3] + cSATVaraibles[4] + cSATVaraibles[5];
            int netPromoterScoreWithoutFilters = noOfCompletedCSATsWithoutFilters != 0 ?(cSATVaraibles[0] * 100 / noOfCompletedCSATsWithoutFilters) - (cSATVaraibles[2] * 100 / noOfCompletedCSATsWithoutFilters):0;
            int netPromoterScoreWithFilters = noOfCompletedCSATsWithFilters!=0?(cSATVaraibles[3] * 100 / noOfCompletedCSATsWithFilters) - (cSATVaraibles[5] * 100 / noOfCompletedCSATsWithFilters):0;
            lblNetPromoterScoreAllCompany.Text = netPromoterScoreWithoutFilters.ToString();
            lblNetPromoterScoreBasedOnFilters.Text = netPromoterScoreWithFilters.ToString();
            lblCompletedCSATs.Text = noOfCompletedCSATsWithFilters.ToString();

            lblNetPromoterScoreAllCompany.Attributes["onmouseover"] = "ShowCalculationPanel(\"" + lblNetPromoterScoreAllCompany.ClientID + "\",\"" + pnlNetPromoterScoreVariables.ClientID + "\",\"" + cSATVaraibles[0].ToString() + "\",\"" + cSATVaraibles[1] + "\",\"" + cSATVaraibles[2] + "\",\"" + noOfCompletedCSATsWithoutFilters + "\",\"" + (noOfCompletedCSATsWithoutFilters != 0 ? (cSATVaraibles[0] * 100 / noOfCompletedCSATsWithoutFilters) : 0) + "\",\"" + (noOfCompletedCSATsWithoutFilters !=0 ?(cSATVaraibles[2] * 100 / noOfCompletedCSATsWithoutFilters):0) + "\",\"" + netPromoterScoreWithoutFilters + "\");";
            lblNetPromoterScoreBasedOnFilters.Attributes["onmouseover"] = "ShowCalculationPanel(\"" + lblNetPromoterScoreBasedOnFilters.ClientID + "\",\"" + pnlNetPromoterScoreVariables.ClientID + "\",\"" + cSATVaraibles[3].ToString() + "\",\"" + cSATVaraibles[4] + "\",\"" + cSATVaraibles[5] + "\",\"" + noOfCompletedCSATsWithFilters + "\",\"" + (noOfCompletedCSATsWithFilters != 0 ? (cSATVaraibles[3] * 100 / noOfCompletedCSATsWithFilters) : 0) + "\",\"" + (noOfCompletedCSATsWithFilters !=0 ?(cSATVaraibles[5] * 100 / noOfCompletedCSATsWithFilters):0) + "\",\"" + netPromoterScoreWithFilters + "\");";

            lblNetPromoterScoreAllCompany.Attributes["onmouseout"] = lblNetPromoterScoreBasedOnFilters.Attributes["onmouseout"] = "HidePanel(\"" + pnlNetPromoterScoreVariables.ClientID + "\");";

            imgNetPromoterScoreWithoutFilters.Attributes["onmouseover"] = "ShowPanel(\"" + imgNetPromoterScoreWithoutFilters.ClientID + "\",\"" + pnlCSATCalculation.ClientID + "\");";
            imgNetPromoterScoreWithFilters.Attributes["onmouseover"] = "ShowPanel(\"" + imgNetPromoterScoreWithFilters.ClientID + "\",\"" + pnlCSATCalculation.ClientID + "\");";
            imgNetPromoterScoreWithoutFilters.Attributes["onmouseout"] = imgNetPromoterScoreWithFilters.Attributes["onmouseout"] = "HidePanel(\"" + pnlCSATCalculation.ClientID + "\");";

        }

        private void SwitchView(Control control, int viewIndex)
        {
            SelectView(control, viewIndex);
            SelectView();
        }

        private void SetCssClassEmpty()
        {
            foreach (TableCell cell in tblCSATViewSwitch.Rows[0].Cells)
            {
                cell.CssClass = string.Empty;
            }
        }

        public void SelectView(Control sender, int viewIndex)
        {
            mvCSATReport.ActiveViewIndex = viewIndex;

            SetCssClassEmpty();

            ((WebControl)sender.Parent).CssClass = "SelectedSwitch";
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

        protected void btnCustDatesCancel_OnClick(object sender, EventArgs e)
        {
            if (hdnPeriod.Value != "0")
            {
                ddlPeriod.SelectedValue = hdnPeriod.Value;
                divCustomDates.Visible = false;
            }
        }

        protected void btnView_Command(object sender, CommandEventArgs e)
        {

            int viewIndex = int.Parse((string)e.CommandArgument);

            SwitchView((Control)sender, viewIndex);
        }

        private void SaveFilterValuesForSession()
        {
            CSATReportFilters filter = new CSATReportFilters();
            filter.PracticeIds = cblPractices.SelectedItems;
            filter.ClientIds = cblAccount.SelectedItems;
            filter.ReportPeriod = ddlPeriod.SelectedValue;
            filter.ReportStartDate = diRange.FromDate;
            filter.ReportEndDate = diRange.ToDate;
            ReportsFilterHelper.SaveFilterValues(ReportName.CSATReport, filter);
        }

        private void GetFilterValuesForSession()
        {
            var filters = ReportsFilterHelper.GetFilterValues(ReportName.CSATReport) as CSATReportFilters;
            if (filters != null)
            {
                cblPractices.UnSelectAll();
                cblPractices.SelectedItems = filters.PracticeIds;
                cblAccount.UnSelectAll();
                cblAccount.SelectedItems = filters.ClientIds;
                ddlPeriod.SelectedValue = filters.ReportPeriod;
                diRange.FromDate = filters.ReportStartDate;
                diRange.ToDate = filters.ReportEndDate;
                SelectView();
            }
        }
    }

}

