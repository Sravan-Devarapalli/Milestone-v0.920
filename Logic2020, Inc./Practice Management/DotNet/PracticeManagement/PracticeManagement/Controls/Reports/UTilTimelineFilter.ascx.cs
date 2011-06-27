using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;
using System.Drawing;

namespace PraticeManagement.Controls.Reports
{
    public partial class UTilTimeLineFilter : System.Web.UI.UserControl
    {
        private const int DEFAULT_STEP = 7;
        private const int DAYS_FORWARD = 184;
        private string RESOURE_DICTIONARY_FILTER_LIST_KEY = "RESOURE_DICTIONARY_FILTER_LIST_KEYS";
        public bool ActivePersons { get { return chbActivePersons.Checked; } }
        public bool ProjectedPersons { get { return chbProjectedPersons.Checked; } }
        public string PracticesSelected { get { return cblPractices.SelectedItems; } }
        public bool ActiveProjects { get { return chbActiveProjects.Checked; } }
        public bool ProjectedProjects { get { return chbProjectedProjects.Checked; } }
        public bool ExperimentalProjects { get { return chbExperimentalProjects.Checked; } }
        public bool InternalProjects { get { return chbInternalProjects.Checked; } }
        public string TimescalesSelected { get { return cblTimeScales.SelectedItems; } }
        public bool ExcludeInternalPractices { get { return chkExcludeInternalPractices.Checked; } }
        public string SortDirection { get { return this.rbSortbyAsc.Checked ? "Desc" : "Asc"; } }
        public int SortId { get { return Convert.ToInt32(ddlSortBy.SelectedItem.Value); } }
        public int AvgUtil { get { return ParseInt(ddlAvgUtil.SelectedValue, int.MaxValue); } }
        public int Granularity { get { return ParseInt(ddlDetalization.SelectedValue, DEFAULT_STEP); } }
        public int Period { get { return ParseInt((EndPeriod.Subtract(BegPeriod).Days + 1).ToString(), DAYS_FORWARD); } }
        public DateTime BegPeriod
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
                        return now.AddDays(1 - now.Day).Date;
                    }
                    else
                    {
                        return now.AddDays(1 - now.Day).AddMonths(selectedVal + 1).Date;
                    }
                }
            }
        }
        public string DetalizationSelectedValue { get { return ddlDetalization.SelectedValue; } }
        public DateTime EndPeriod
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
                    if (selectedVal > 0)
                    {
                        return now.AddDays(-1 * now.Day).AddMonths(selectedVal).Date;
                    }
                    else
                    {
                        return now.AddDays(-1 * now.Day).AddMonths(1).Date;
                    }
                }
            }
        }

        private Dictionary<string, string> resoureDictionary
        {

            get
            {
                if (ViewState[RESOURE_DICTIONARY_FILTER_LIST_KEY] == null)
                {
                    ViewState[RESOURE_DICTIONARY_FILTER_LIST_KEY] = new Dictionary<string, string>();
                }

                return (Dictionary<string, string>)ViewState[RESOURE_DICTIONARY_FILTER_LIST_KEY];
            }
            set { ViewState[RESOURE_DICTIONARY_FILTER_LIST_KEY] = value; }
        }

        public bool IsSampleReport
        {
            get
            {
                return hdnIsSampleReport.Value.ToLowerInvariant() == "true" ? true : false;
            }
            set
            {
                hdnIsSampleReport.Value = value.ToString();
            }
        }

        public bool BtnUpdateViewVisible
        {
            get { return btnUpdateView.Visible; }
            set
            {
                if (!value)
                {
                    btnUpdateView.Width = 0;
                }

                btnUpdateView.Visible = value;
            }
        }

        public bool BtnResetFilterVisible
        {
            get { return btnResetFilter.Visible; }
            set
            {
                if (!value)
                {
                    btnResetFilter.Width = 0;
                }

                btnResetFilter.Visible = value;
            }
        }

        public bool BtnSaveReportVisible
        {
            get { return btnSaveReport.Visible; }
            set
            {
                if (!value)
                {
                    btnSaveReport.Width = 0;
                }

                btnSaveReport.Visible = value;
            }
        }

        private static int ParseInt(string val, int def)
        {
            try
            {
                return int.Parse(val);
            }
            catch
            {
                return def;
            }
        }

        public delegate void OnUpDateViewClick(object sender, EventArgs e);
        public event OnUpDateViewClick EvntHandler_OnUpDateView_Click;

        public delegate void OnResetFilterClick(object sender, EventArgs e);
        public event OnResetFilterClick EvntHandler_OnResetFilter_Click;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.FillPracticeList(this.cblPractices, Resources.Controls.AllPracticesText);
                DataHelper.FillTimescaleList(this.cblTimeScales, Resources.Controls.AllTypes);

                if (IsSampleReport)
                {
                    PopulateControls();
                }
                else
                {
                    SelectAllItems(this.cblPractices);
                    SelectAllItems(this.cblTimeScales);
                }
            }
            lblMessage.Text = string.Empty;
            AddAttributesToCheckBoxes(this.cblPractices);
            AddAttributesToCheckBoxes(this.cblTimeScales);

            if (hdnFiltersChanged.Value == "false")
            {
                btnResetFilter.Attributes.Add("disabled", "true");
            }
            else
            {
                btnResetFilter.Attributes.Remove("disabled");
            }
        }

        public void PopulateControls()
        {
            if (cblPractices != null && cblPractices.Items.Count == 0)
            {
                DataHelper.FillPracticeList(cblPractices, Resources.Controls.AllPracticesText);
            }

            if (cblTimeScales != null && cblTimeScales.Items.Count == 0)
            {
                DataHelper.FillTimescaleList(cblTimeScales, Resources.Controls.AllTypes);
            }

            resoureDictionary = DataHelper.GetResourceKeyValuePairs(SettingsType.Reports);

            if (resoureDictionary != null && resoureDictionary.Keys.Count > 0)
            {
                diRange.FromDate = Convert.ToDateTime(resoureDictionary[Constants.ResourceKeys.StartDateKey]);

                diRange.ToDate = Convert.ToDateTime(resoureDictionary[Constants.ResourceKeys.EndDateKey]);
                ddlPeriod.SelectedValue = resoureDictionary[Constants.ResourceKeys.PeriodKey];
                ddlDetalization.SelectedValue = ddlDetalization.Items.FindByValue(resoureDictionary[Constants.ResourceKeys.GranularityKey]).Value;
                ddlAvgUtil.SelectedValue = ddlAvgUtil.Items.FindByValue(resoureDictionary[Constants.ResourceKeys.AvgUtilKey]).Value;
                ddlSortBy.SelectedValue = ddlSortBy.Items.FindByValue(resoureDictionary[Constants.ResourceKeys.SortIdKey]).Value;

                chbActivePersons.Checked = Convert.ToBoolean(resoureDictionary[Constants.ResourceKeys.ActivePersonsKey]);
                chbProjectedPersons.Checked = Convert.ToBoolean(resoureDictionary[Constants.ResourceKeys.ProjectedPersonsKey]);
                chbActiveProjects.Checked = Convert.ToBoolean(resoureDictionary[Constants.ResourceKeys.ActiveProjectsKey]);
                chbProjectedProjects.Checked = Convert.ToBoolean(resoureDictionary[Constants.ResourceKeys.ProjectedProjectsKey]);
                chbExperimentalProjects.Checked = Convert.ToBoolean(resoureDictionary[Constants.ResourceKeys.ExperimentalProjectsKey]);
                chbInternalProjects.Checked = Convert.ToBoolean(resoureDictionary[Constants.ResourceKeys.InternalProjectsKey]);
                chkExcludeInternalPractices.Checked = Convert.ToBoolean(resoureDictionary[Constants.ResourceKeys.ExcludeInternalPracticesKey]);
                rbSortbyAsc.Checked = resoureDictionary[Constants.ResourceKeys.SortDirectionKey] == "Desc" ? true : false;

                SelectItems(this.cblPractices, resoureDictionary[Constants.ResourceKeys.PracticeIdListKey]);
                SelectItems(this.cblTimeScales, resoureDictionary[Constants.ResourceKeys.TimescaleIdListKey]);
            }

        }

        private void SelectItems(ScrollingDropDown scrollingDropDown, string commaSeperatedList)
        {
            if (!String.IsNullOrEmpty(commaSeperatedList))
            {
                if (commaSeperatedList[0] == ',')
                {
                    SelectAllItems(scrollingDropDown);
                }
                else
                {
                    string[] splitLetter = { "," };
                    string[] splitArray = commaSeperatedList.Split(splitLetter, StringSplitOptions.RemoveEmptyEntries);

                    if (splitArray.Count() > 0)
                    {
                        foreach (ListItem item in scrollingDropDown.Items)
                        {
                            if (splitArray.Any(m => m == item.Value))
                            {
                                item.Selected = true;
                            }
                        }
                    }
                }
            }

        }

        private void AddAttributesToCheckBoxes(ScrollingDropDown ddlpractices)
        {
            foreach (ListItem item in ddlpractices.Items)
            {
                item.Attributes.Add("onclick", "EnableResetButton();");
            }
        }

        private void SelectAllItems(ScrollingDropDown ddlpractices)
        {
            foreach (ListItem item in ddlpractices.Items)
            {
                item.Selected = true;
            }
        }

        public void Resetfilter()
        {
            this.chbActivePersons.Checked = true;
            this.chbProjectedPersons.Checked = false;
            this.chbActiveProjects.Checked = true;
            this.chbInternalProjects.Checked = true;
            this.chbProjectedProjects.Checked = true;
            this.chbExperimentalProjects.Checked = false;
            this.chkExcludeInternalPractices.Checked = false;
            SelectAllItems(this.cblPractices);
            SelectAllItems(this.cblTimeScales);
        }

        public string PracticesFilterText()
        {
            string PracticesFilterText = "Not Including All Practice Areas";
            if (cblPractices.Items.Count > 0)
            {
                if (cblPractices.Items[0].Selected)
                {
                    PracticesFilterText = "Including All Practice Areas";
                }
                else
                {
                    for (int index = 1; index < cblPractices.Items.Count; index++)
                    {
                        if (!cblPractices.Items[index].Selected)
                        {
                            return PracticesFilterText + (this.chkExcludeInternalPractices.Checked ? ";Excluding Internal Practice Areas" : string.Empty);
                        }
                    }
                    PracticesFilterText = "Including All Practice Areas";
                }
            }
            return PracticesFilterText + (this.chkExcludeInternalPractices.Checked ? ";Excluding Internal Practice Areas" : string.Empty);
        }

        protected void btnUpdateView_OnClick(object sender, EventArgs e)
        {
            if (EvntHandler_OnUpDateView_Click != null)
            {
                EvntHandler_OnUpDateView_Click(sender, e);
            }
        }

        protected void btnResetFilter_OnClick(object sender, EventArgs e)
        {
            ddlPeriod.SelectedIndex = 0;
            ddlDetalization.SelectedIndex = 1;
            ddlAvgUtil.SelectedIndex = 0;
            ddlSortBy.SelectedIndex = 0;
            Resetfilter();
            rbSortbyAsc.Checked = false;
            rbSortbyDesc.Checked = true;

            if (EvntHandler_OnResetFilter_Click != null)
            {
                EvntHandler_OnResetFilter_Click(sender, e);
            }

            hdnFiltersChanged.Value = "false";
            btnResetFilter.Attributes.Add("disabled", "true");
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            diRange.FromDate = BegPeriod;
            diRange.ToDate = EndPeriod;
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
            var clFromDate = diRange.FindControl("clFromDate") as AjaxControlToolkit.CalendarExtender;
            var clToDate = diRange.FindControl("clToDate") as AjaxControlToolkit.CalendarExtender;
            tbFrom.Attributes.Add("onchange", "ChangeStartEndDates();");
            tbTo.Attributes.Add("onchange", "ChangeStartEndDates();");
            hdnStartDateTxtBoxId.Value = tbFrom.ClientID;
            hdnEndDateTxtBoxId.Value = tbTo.ClientID;
            hdnStartDateCalExtenderBehaviourId.Value = clFromDate.BehaviorID;
            hdnEndDateCalExtenderBehaviourId.Value = clToDate.BehaviorID;
        }

        protected void btnSaveReport_OnClick(object sender, EventArgs e)
        {
            SaveFilters();
            lblMessage.Text = "Report Filter details are saved succeessfully.";
            lblMessage.ForeColor = Color.Green;
        }

        public void SaveFilters()
        {
            Dictionary<string, string> reportFilterDictionary = new Dictionary<string, string>();

            reportFilterDictionary.Add(Constants.ResourceKeys.StartDateKey, BegPeriod.ToString());
            reportFilterDictionary.Add(Constants.ResourceKeys.GranularityKey, Granularity.ToString());
            reportFilterDictionary.Add(Constants.ResourceKeys.ProjectedPersonsKey, ProjectedPersons.ToString());
            reportFilterDictionary.Add(Constants.ResourceKeys.ProjectedProjectsKey, ProjectedProjects.ToString());
            reportFilterDictionary.Add(Constants.ResourceKeys.ActivePersonsKey, ActivePersons.ToString());
            reportFilterDictionary.Add(Constants.ResourceKeys.ActiveProjectsKey, ActiveProjects.ToString());
            reportFilterDictionary.Add(Constants.ResourceKeys.ExperimentalProjectsKey, ExperimentalProjects.ToString());
            reportFilterDictionary.Add(Constants.ResourceKeys.TimescaleIdListKey, TimescalesSelected.ToString());
            reportFilterDictionary.Add(Constants.ResourceKeys.PracticeIdListKey, PracticesSelected.ToString());
            reportFilterDictionary.Add(Constants.ResourceKeys.ExcludeInternalPracticesKey, ExcludeInternalPractices.ToString());
            reportFilterDictionary.Add(Constants.ResourceKeys.InternalProjectsKey, InternalProjects.ToString());
            reportFilterDictionary.Add(Constants.ResourceKeys.SortIdKey, SortId.ToString());
            reportFilterDictionary.Add(Constants.ResourceKeys.SortDirectionKey, SortDirection.ToString());
            reportFilterDictionary.Add(Constants.ResourceKeys.AvgUtilKey, AvgUtil.ToString());
            reportFilterDictionary.Add(Constants.ResourceKeys.EndDateKey, EndPeriod.ToString());
            reportFilterDictionary.Add(Constants.ResourceKeys.PeriodKey, ddlPeriod.SelectedValue);
            DataHelper.SaveResourceKeyValuePairs(SettingsType.Reports, reportFilterDictionary);
        }
    }
}
