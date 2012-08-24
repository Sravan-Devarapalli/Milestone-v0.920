using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using DataTransferObjects;
using System.Web.UI.HtmlControls;
using System.Text;
using AjaxControlToolkit;
using DataTransferObjects.Reports.HumanCapital;

namespace PraticeManagement.Reporting
{
    public partial class TerminationReport : System.Web.UI.Page
    {
        #region constants

        private const string W2Hourly = "W2-Hourly";
        private const string W2Salary = "W2-Salary";
        private string TerminationReportExport = "Termination Report";

        #endregion

        #region Properties

        //ddlPeriod Items
        //Last Month : 1
        //Last 3 months : 2
        //Last 6 months : 3
        //Last 9 months : 4
        //Last 12 months : 5
        //Year to Date : 6
        //Custom Date Range: 0

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
                        if (selectedVal == 1)
                        {
                            return Utils.Calendar.LastMonthStartDate(now);
                        }
                        else if (selectedVal == 2)
                        {
                            return Utils.Calendar.Last3MonthStartDate(now);
                        }
                        else if (selectedVal == 3)
                        {
                            return Utils.Calendar.Last6MonthStartDate(now);
                        }
                        else if (selectedVal == 4)
                        {
                            return Utils.Calendar.Last9MonthStartDate(now);
                        }
                        else if (selectedVal == 5)
                        {
                            return Utils.Calendar.Last12MonthStartDate(now);
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

                        if (selectedVal == 6)
                        {
                            return now;
                        }
                        else
                        {
                            return Utils.Calendar.LastMonthEndDate(now);
                        }
                    }
                }
                return null;
            }
        }

        //ddlPeriod Items
        //Last Month : 1
        //Last 3 months : 2
        //Last 6 months : 3
        //Last 9 months : 4
        //Last 12 months : 5
        //Year to Date : 6
        //Custom Date Range: 0
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
                        case 3:
                        case 4:
                        case 5:
                            range = StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat);
                            break;
                        case 6:
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

        public int RangeSelected
        {
            get
            {
                int selectedValue = 0;
                int.TryParse(ddlPeriod.SelectedValue, out selectedValue);
                return selectedValue;
            }
        }

        public string PayTypes
        {
            get
            {
                return cblTimeScales.SelectedItemsXmlFormat;
            }
        }

        public string Seniorities
        {
            get
            {
                return cblSeniorities.SelectedItemsXmlFormat;
            }
        }

        public string TerminationReasons
        {
            get
            {
                return cblTerminationReasons.SelectedItemsXmlFormat;
            }
        }

        public string Practices
        {
            get
            {
                return cblPractices.SelectedItemsXmlFormat;
            }
        }

        public bool ExcludeInternalProjects
        {
            get
            {
                return chbInternalProjects.Checked;
            }
        }

        public bool SetSelectedFilters { get; set; }


        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (this.cblTimeScales != null && this.cblTimeScales.Items.Count == 0)
                {
                    DataHelper.FillTimescaleList(this.cblTimeScales, Resources.Controls.AllTypes);

                }
                if (this.cblSeniorities != null && this.cblSeniorities.Items.Count == 0)
                {
                    DataHelper.FillSenioritiesList(this.cblSeniorities, "All Seniorities");
                }
                if (this.cblTerminationReasons != null && this.cblTerminationReasons.Items.Count == 0)
                {
                    DataHelper.FillTerminationReasonsList(this.cblTerminationReasons, "All Reasons");
                }
                if (this.cblPractices != null && this.cblPractices.Items.Count == 0)
                {
                    DataHelper.FillPracticeList(this.cblPractices, Resources.Controls.AllPracticesText);
                }
                SetDefalultfilter();
                LoadActiveView();
                LoadAttrition();
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
        }

        #endregion

        #region Methods

        private void SetDefalultfilter()
        {
            ddlPeriod.SelectedValue = "6";
            this.chbInternalProjects.Checked = false;
            SelectAllItems(this.cblPractices);
            SelectAllItems(this.cblSeniorities);
            SelectAllItems(this.cblTerminationReasons);
            SelectDefaultTimeScaleItems(this.cblTimeScales);
        }

        private void SelectAllItems(ScrollingDropDown ddlItems)
        {
            foreach (ListItem item in ddlItems.Items)
            {
                item.Selected = true;
            }
        }

        private void SelectDefaultTimeScaleItems(ScrollingDropDown cblTimeScales)
        {
            foreach (ListItem item in cblTimeScales.Items)
            {
                item.Selected = (item.Text == W2Hourly || item.Text == W2Salary);
            }
        }

        private void populateGraph(int count, int percentage, Literal ltr, HtmlTableRow tr)
        {
            ltr.Text = count.ToString();
            if (percentage == 0)
            {
                tr.Height = "1px";
            }
            else
            {
                tr.Height = percentage + "px";
            }
        }

        public void PopulateHeaderSection(TerminationPersonsInRange reportData)
        {
            List<int> ratioList = (new int[] { reportData.TerminationsW2SalaryCountInTheRange, reportData.TerminationsW2HourlyCountInTheRange, reportData.TerminationsContractorsCountInTheRange }).ToList();
            int height = 80;
            List<int> percentageList = DataTransferObjects.Utils.Generic.GetProportionateRatio(ratioList, height);

            ltPersonCount.Text = reportData.PersonList.Count + " Terminations";
            lbRange.Text = Range;

            ltrlTotalEmployees.Text = reportData.TerminationsEmployeeCountInTheRange.ToString();
            ltrlTotalContractors.Text = reportData.TerminationsContractorsCountInTheRange.ToString();
            ltrlW2SalaryCount.Text = reportData.TerminationsW2SalaryCountInTheRange.ToString();
            ltrlW2HourlyCount.Text = reportData.TerminationsW2HourlyCountInTheRange.ToString();
            ltrlContractorsCount.Text = reportData.TerminationsContractorsCountInTheRange.ToString();

            populateGraph(reportData.TerminationsW2SalaryCountInTheRange, percentageList[0], ltrlW2SalaryCount, trW2Salary);
            populateGraph(reportData.TerminationsW2HourlyCountInTheRange, percentageList[1], ltrlW2HourlyCount, trW2Hourly);
            populateGraph(reportData.TerminationsContractorsCountInTheRange, percentageList[2], ltrlContractorsCount, trContrator);
        }

        /// <summary>
        /// Loads the attrition data with respect to date range.
        /// </summary>
        private void LoadAttrition()
        {
            List<TerminationPersonsInRange> data = ServiceCallers.Custom.Report(r => r.TerminationReportGraph(StartDate.Value, EndDate.Value)).ToList();
            double attrition = 0;
            int terminationsEmployeeCountInTheRange = 0;
            int activePersonsCountAtTheBeginning = data.First(s => s.StartDate == StartDate.Value.Date).ActivePersonsCountAtTheBeginning;
            int newHiresCountInTheRange = 0;
            foreach(var termiantionPerson in data)
            {
                attrition += termiantionPerson.Attrition;
                terminationsEmployeeCountInTheRange += termiantionPerson.TerminationsEmployeeCountInTheRange;                
                newHiresCountInTheRange += termiantionPerson.NewHiresCountInTheRange;
            }
            lblAttrition.Text = lblPopUpArrition.Text = attrition.ToString("0.00%");
            lblPopUPTerminations.Text = lblPopUPTerminationsCount.Text = lblPopUPTerminationsCountDenominator.Text = terminationsEmployeeCountInTheRange.ToString();
            lblPopUPActivensCount.Text = lblPopUPActivens.Text = activePersonsCountAtTheBeginning.ToString();
            lblPopUPNewHiresCount.Text = lblPopUPNewHires.Text = newHiresCountInTheRange.ToString();

        }

        private void LoadActiveView()
        {
            SetSelectedFilters = true;
            if (mvTerminationReport.ActiveViewIndex == 0)
            {
                tpSummary.PopulateData();
            }
            else
            {
                tpGraph.PopulateGraph();
            }
        }

        private void SwitchView(Control control, int viewIndex)
        {
            SelectView(control, viewIndex);
            LoadActiveView();
        }

        private void SelectView(Control sender, int viewIndex)
        {
            mvTerminationReport.ActiveViewIndex = viewIndex;

            SetCssClassEmpty();

            ((WebControl)sender.Parent).CssClass = "SelectedSwitch";
        }

        private void SetCssClassEmpty()
        {
            foreach (TableCell cell in tblPersonViewSwitch.Rows[0].Cells)
            {
                cell.CssClass = string.Empty;
            }

        }

        protected string GetDateFormat(DateTime date)
        {
            return date.ToString(Constants.Formatting.EntryDateFormat);
        }


        #endregion

        #region Control Events

        protected void Filters_Changed(object sender, EventArgs e)
        {
            LoadActiveView();
        }

        protected void btnView_Command(object sender, CommandEventArgs e)
        {
            int viewIndex = int.Parse((string)e.CommandArgument);
            SwitchView((Control)sender, viewIndex);
        }

        protected void ddlPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPeriod.SelectedValue != "0")
            {
                LoadActiveView();
                LoadAttrition();
            }
            else
            {
                mpeCustomDates.Show();
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
                LoadAttrition();
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

        #region Export
        public void ExportToExcel(List<Person> data, bool isPopUp, string popUpRange)
        {

            DataHelper.InsertExportActivityLogMessage(TerminationReportExport);
            StringBuilder sb = new StringBuilder();
            sb.Append("Termination Report");
            sb.Append("\t");
            sb.AppendLine();
            sb.Append(data.Count + " Terminations");
            sb.Append("\t");
            sb.AppendLine();
            sb.Append(isPopUp ? popUpRange : Range);
            sb.Append("\t");
            sb.AppendLine();
            sb.AppendLine();

            if (data.Count > 0)
            {
                //Header
                sb.Append("Resource");
                sb.Append("\t");
                sb.Append("Seniority");
                sb.Append("\t");
                sb.Append("Pay Types");
                sb.Append("\t");
                sb.Append("Status");
                sb.Append("\t");
                sb.Append("Recruiter");
                sb.Append("\t");
                sb.Append("Hire Date");
                sb.Append("\t");
                sb.Append("Termination Date");
                sb.Append("\t");
                sb.Append("Termination Reason");
                sb.Append("\t");
                sb.AppendLine();

                //Data
                foreach (var person in data)
                {
                    sb.Append(person.HtmlEncodedName);
                    sb.Append("\t");
                    sb.Append(person.Seniority.Name);
                    sb.Append("\t");
                    sb.Append(person.CurrentPay.TimescaleName);
                    sb.Append("\t");
                    sb.Append(person.Status.Name);
                    sb.Append("\t");
                    sb.Append(person.RecruiterCommission.Count > 0 ? person.RecruiterCommission.First().Recruiter.PersonLastFirstName : string.Empty);
                    sb.Append("\t");
                    sb.Append(GetDateFormat(person.HireDate));
                    sb.Append("\t");
                    sb.Append(GetDateFormat(person.TerminationDate.Value));
                    sb.Append("\t");
                    sb.Append(person.TerminationReason);
                    sb.Append("\t");
                    sb.AppendLine();
                }

            }
            else
            {
                sb.Append("There are no Person Terminations for the selected range.");
            }
            //“TerminationReport_[StartOfRange]_[EndOfRange].xls”.  
            var filename = string.Format("{0}_{1}-{2}.xls", "TerminationReport", StartDate.Value.ToString("MM.dd.yyyy"), EndDate.Value.ToString("MM.dd.yyyy"));
            GridViewExportUtil.Export(filename, sb);
        }
        #endregion
    }
}

