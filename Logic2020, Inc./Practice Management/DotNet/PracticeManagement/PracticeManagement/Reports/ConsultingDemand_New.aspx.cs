using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;

namespace PraticeManagement.Reports
{
    public partial class ConsultingDemand_New : System.Web.UI.Page
    {
        #region Constants

        public const string PipelineTitle = "PipeLineTitle";

        public const string PipelineSkill = "PipeLineSkill";

        public const string TransactionTitle = "TransactionTitle";

        public const string TransactionSkill = "TransactionSkill";

        public const string PipeLine = "PipeLine";


        #endregion

        #region Properties

        public DateTime? StartDate
        {
            get
            {
                int selectedVal = 0;
                if (hdnCustomOk.Value == "true")
                {
                    return Convert.ToDateTime(hdnStartDate.Value);
                }
                else
                {
                    if (int.TryParse(hdnPeriodValue.Value, out selectedVal))
                    {
                        if (selectedVal == 0)
                        {
                            return Convert.ToDateTime(hdnStartDate.Value);
                        }
                        else
                        {
                            var now = Utils.Generic.GetNowWithTimeZone();
                            return Utils.Calendar.MonthStartDate(now);

                        }
                    }
                    return null;
                }
            }
        }

        public DateTime? EndDate
        {
            get
            {
                int selectedVal = 0;
                if (hdnCustomOk.Value == "true")
                {
                    return Convert.ToDateTime(hdnEndDate.Value);
                }
                else
                {
                    if (int.TryParse(hdnPeriodValue.Value, out selectedVal))
                    {
                        var now = Utils.Generic.GetNowWithTimeZone();
                        if (selectedVal == 0)
                        {
                            return Convert.ToDateTime(hdnEndDate.Value);
                        }
                        else if (selectedVal == 1)
                        {
                            return Utils.Calendar.MonthEndDate(now);
                        }
                        else if (selectedVal == 2)
                        {
                            return Utils.Calendar.Next2MonthEndDate(now);
                        }
                        else if (selectedVal == 3)
                        {
                            return Utils.Calendar.Next3MonthEndDate(now);
                        }
                        else
                        {
                            return Utils.Calendar.Next4MonthEndDate(now);
                        }
                    }
                    return null;
                }
            }
        }

        public List<string> MonthNames
        {
            get
            {
                if (StartDate.HasValue && EndDate.HasValue)
                    return Utils.Calendar.GetMonthYearWithInThePeriod(StartDate.Value, EndDate.Value);
                else
                    return new List<string>();
            }
        }

        public bool isSelectAllTitles { get { return cblTitles.areAllSelected; } }

        public bool isSelectAllSkills { get { return cblSkills.areAllSelected; } }

        public string hdnTitlesProp { get { return hdnTitles.Value; } }

        public string hdnSkillsProp { get { return hdnSkills.Value; } }

        public int RolesCount { get; set; }

        public string GraphType
        {
            get
            {
                if (hdnGraphType.Value == "0")
                {
                    return string.Empty;
                }
                else if (hdnGraphType.Value == "PipeLine")
                {
                    return hdnPipelineTitleOrSkill.Value;
                }
                else
                {
                    return hdnGraphType.Value;
                }
            }
            set
            {
                if (value == PipelineTitle || value == PipelineTitle)
                {
                    hdnPipelineTitleOrSkill.Value = value;
                    hdnGraphType.Value = "PipeLine";
                }
                else
                {
                    hdnGraphType.Value = value;
                }
            }
        }

        public PraticeManagement.Controls.Reports.ConsultantDemand.ConsultingDemandSummary SummaryControl
        {
            get
            {
                return ucSummary;
            }
        }

        public PraticeManagement.Controls.Reports.ConsultantDemand.ConsultingDemandGraphs GraphControl
        {
            get
            {
                return ucGraphs;
            }
        }

        #endregion

        #region Events

        #region PageEvents

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                hdnCustomOk.Value = "false";
                hdnPeriodValue.Value = "-1";
                trGtypes.Visible =
                trTitles.Visible =
                upnlTabCell.Visible = false;
                lblTitle.Text = "Title";
                List<string> titles = ServiceCallers.Custom.Person(p => p.GetStrawmenListAllShort(true)).Select(p => p.LastName).Distinct().ToList();
                DataHelper.FillListDefault(cblTitles, "All Titles", titles.Select(p => new { title = p }).ToArray(), false, "title", "title");
                tdSkills.Visible = false;
                tdTitles.Visible = true;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetCustomDatesTextboxes();
            }
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

        }

        #endregion

        protected void btnResetFilter_OnClick(object sender, EventArgs e)
        {
            ResetFilter();
        }

        protected void btnUpdateView_OnClick(object sender, EventArgs e)
        {
            hdnGraphType.Value = ddlGraphsTypes.SelectedValue;
            hdnPeriodValue.Value = ddlPeriod.SelectedValue;
            if (hdnPeriodValue.Value == "0")
            {
                hdnCustomOk.Value = "false";
            }
            LoadActiveView();
        }

        protected void btnCustDatesOK_Click(object sender, EventArgs e)
        {
            Page.Validate(valSum.ValidationGroup);

            if (Page.IsValid)
            {
                enableDisableResetButtons();
                DateTime? startDate = diRange.FromDate;
                DateTime? endDate = diRange.ToDate;
                if (startDate.HasValue && endDate.HasValue)
                {
                    hdnStartDate.Value = startDate.Value.Date.ToShortDateString();
                    hdnEndDate.Value = endDate.Value.Date.ToShortDateString();
                    hdnCustomOk.Value = "true";
                    SetCustomDatesTextboxes();
                }
            }
            else
            {
                mpeCustomDates.Show();
            }
        }

        protected void cstvalPeriodRange_ServerValidate(object source, ServerValidateEventArgs args)
        {
            CustomValidator cvCustomDates = source as CustomValidator;
            DateTime? startDate = diRange.FromDate;
            DateTime? endDate = diRange.ToDate;
            if (startDate.HasValue && endDate.HasValue && startDate != DateTime.MinValue && endDate != DateTime.MinValue)
            {
                args.IsValid = startDate.Value.AddMonths(4) > endDate.Value;
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void btnCustDatesCancel_OnClick(object sender, EventArgs e)
        {
            if (hdnCustomOk.Value == "false")
            {
                ddlPeriod.SelectedValue = hdnPeriodValue.Value;
            }
            enableDisableResetButtons();
        }

        protected void btnView_Command(object sender, CommandEventArgs e)
        {
            int viewIndex = int.Parse((string)e.CommandArgument);
            SwitchView((Control)sender, viewIndex);
        }

        protected void ddlGraphsTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            trTitles.Visible = true;
            if (ddlGraphsTypes.SelectedValue == TransactionTitle)
            {
                lblTitle.Text = "Title";
                List<string> titles = ServiceCallers.Custom.Person(p => p.GetStrawmenListAllShort(true)).OrderBy(p => p.LastName).Select(p => p.LastName).Distinct().ToList();
                DataHelper.FillListDefault(cblTitles, "All Titles", titles.Select(p => new { title = p }).ToArray(), false, "title", "title");
                tdSkills.Visible = false;
                tdTitles.Visible = true;
                cblTitles.SelectAll();
            }
            else if (ddlGraphsTypes.SelectedValue == TransactionSkill)
            {
                lblTitle.Text = "Skill";
                List<string> skills = ServiceCallers.Custom.Person(p => p.GetStrawmenListAllShort(true)).OrderBy(p=>p.FirstName).Select(p => p.FirstName).Distinct().ToList();
                DataHelper.FillListDefault(cblSkills, "All Skills", skills.Select(p => new { skill = p }).ToArray(), false, "skill", "skill");
                tdSkills.Visible = true;
                tdTitles.Visible = false;
                cblSkills.SelectAll();
            }
            else
            {
                trTitles.Visible = false;
            }
            enableDisableResetButtons();
        }

        protected void ddlPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            enableDisableResetButtons();
            if (ddlPeriod.SelectedValue == "0")
            {
                diRange.FromDate = StartDate.Value;
                diRange.ToDate = EndDate.Value;
                mpeCustomDates.Show();
            }
        }

        protected void cblTitles_SelectedIndexChanged(object sender, EventArgs e)
        {
            enableDisableResetButtons();
        }

        #endregion

        #region Methods

        private void enableDisableResetButtons()
        {
            bool enableUpdateView = true;
            bool enableResetFilter = true;

            enableUpdateView = ddlPeriod.SelectedValue != "-1";

            enableUpdateView = enableUpdateView && hdnPeriodValue.Value != ddlPeriod.SelectedValue;

            if (trGtypes.Visible)
            {
                enableUpdateView = enableUpdateView || (ddlGraphsTypes.SelectedIndex != 0 && hdnGraphType.Value != ddlGraphsTypes.SelectedValue);
                enableResetFilter = enableResetFilter || ddlGraphsTypes.SelectedIndex != 0;
            }
            if (trTitles.Visible)
            {
                if (tdTitles.Visible)
                {
                    enableUpdateView = enableUpdateView || (cblTitles.SelectedItems != hdnTitlesProp) ? cblTitles.isSelected : false;
                    enableResetFilter = enableResetFilter || (cblTitles.SelectedItems != hdnTitlesProp);

                }
                else
                {
                    enableUpdateView = enableUpdateView && (cblSkills.SelectedItems != hdnSkillsProp) ? cblSkills.isSelected : false;
                    enableResetFilter = enableResetFilter || (cblSkills.SelectedItems != hdnSkillsProp);
                }

            }
            if (lblCustomDateRange.Attributes["class"] == "fontBold")
            {
                enableUpdateView = enableUpdateView || (StartDate.Value.Date != diRange.FromDate.Value.Date || EndDate.Value.Date != diRange.ToDate.Value.Date);
            }
            btnUpdateView.Enabled = enableUpdateView;
            btnResetFilter.Enabled = enableResetFilter;
            upnlHeader.Update();
        }

        private void ResetFilter()
        {
            ddlPeriod.SelectedIndex = 0;
            if (trGtypes.Visible)
            {
                ddlGraphsTypes.SelectedIndex = 0;
                trTitles.Visible = false;
            }
            btnResetFilter.Enabled = btnUpdateView.Enabled = false;
        }

        private void LoadActiveView()
        {
            upnlTabCell.Visible = true;
            if (mvConsultingDemandReport.ActiveViewIndex == 0)
            {
                trGtypes.Visible = false;
                trTitles.Visible = false;
                ucSummary.PopulateData();
            }
            else if (mvConsultingDemandReport.ActiveViewIndex == 1)
            {
                trGtypes.Visible = false;
                trTitles.Visible = false;
                ucDetails.PopulateData(true, "");
            }
            else
            {
                trGtypes.Visible = true;
                trTitles.Visible = ddlGraphsTypes.SelectedIndex != 0 && ddlGraphsTypes.SelectedValue != "PipeLine";
                string selectedValues = null;
                if (ddlGraphsTypes.SelectedValue == TransactionTitle)
                {
                    selectedValues = cblTitles.SelectedItems;
                    hdnTitles.Value = selectedValues;
                }
                else if (ddlGraphsTypes.SelectedValue == TransactionSkill)
                {
                    selectedValues = cblSkills.SelectedItems;
                    hdnSkills.Value = selectedValues;
                }
                else if (ddlGraphsTypes.SelectedValue == "PipeLine")
                {
                    GraphType = PipelineTitle;
                }
                ucGraphs.PopulateGraph();
            }
            enableDisableResetButtons();
        }

        private void SwitchView(Control control, int viewIndex)
        {
            SelectView(control, viewIndex);
            if (mvConsultingDemandReport.ActiveViewIndex == 2)
            {
                ddlGraphsTypes.SelectedValue = TransactionTitle;
                hdnGraphType.Value = ddlGraphsTypes.SelectedValue;
                cblTitles.SelectAll();
                lblTitle.Text = "Title";
                tdSkills.Visible = false;
                tdTitles.Visible = true;
                cblSkills.SelectAll();
            }
            enableDisableResetButtons();
            ddlPeriod.SelectedValue = hdnPeriodValue.Value;
            LoadActiveView();
        }

        private void SelectView(Control sender, int viewIndex)
        {
            mvConsultingDemandReport.ActiveViewIndex = viewIndex;

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

        private void SetCustomDatesTextboxes()
        {
            var now = Utils.Generic.GetNowWithTimeZone();
            diRange.FromDate = StartDate.HasValue ? StartDate.Value : now;
            diRange.ToDate = EndDate.HasValue ? EndDate.Value : Utils.Calendar.Next4MonthEndDate(now);
            lblCustomDateRange.Text = string.Format("({0}&nbsp;-&nbsp;{1})",
                    diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat),
                    diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat)
                    );
            hdnStartDate.Value = diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat);
            hdnEndDate.Value = diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat);
        }

        #endregion

    }
}

