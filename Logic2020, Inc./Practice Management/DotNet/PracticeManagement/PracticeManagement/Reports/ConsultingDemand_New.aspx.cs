﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;

namespace PraticeManagement.Reports
{
    public partial class ConsultingDemand_New : System.Web.UI.Page
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
                        return Utils.Calendar.MonthStartDate(now);
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
                    var now = Utils.Generic.GetNowWithTimeZone();
                    if (selectedVal == 0)
                    {
                        return diRange.ToDate.Value;
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

        public string hdnTitlesProp { get { return hdnTitles.Value; } }

        public string hdnSkillsProp { get { return hdnSkills.Value; } }

        public string titleOrSkill { get { return hdnTitleOrSkill.Value; } }

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

        protected void btnResetFilter_OnClick(object sender, EventArgs e)
        {
            ResetFilter();
        }

        protected void btnUpdateView_OnClick(object sender, EventArgs e)
        {
            LoadActiveView();
        }

        protected void btnCustDatesOK_Click(object sender, EventArgs e)
        {
            Page.Validate(valSumDateRange.ValidationGroup);
            if (Page.IsValid)
            {
                hdnStartDate.Value = StartDate.Value.Date.ToShortDateString();
                hdnEndDate.Value = EndDate.Value.Date.ToShortDateString();
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

        protected void btnView_Command(object sender, CommandEventArgs e)
        {
            int viewIndex = int.Parse((string)e.CommandArgument);
            SwitchView((Control)sender, viewIndex);
        }

        protected void ddlGraphsTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            trTitles.Visible = true;
            if (ddlGraphsTypes.SelectedValue == "1")
            {
                lblTitle.Text = "Title";
                List<string> titles = ServiceCallers.Custom.Person(p => p.GetStrawmenListAllShort(true)).Select(p => p.LastName).Distinct().ToList();
                DataHelper.FillListDefault(cblTitles, "All Titles", titles.Select(p => new { title = p }).ToArray(), false, "title", "title");
                tdSkills.Visible = false;
                tdTitles.Visible = true;
                cblTitles.SelectAll();
            }
            else if (ddlGraphsTypes.SelectedValue == "2")
            {
                lblTitle.Text = "Skill";
                List<string> skills = ServiceCallers.Custom.Person(p => p.GetStrawmenListAllShort(true)).Select(p => p.FirstName).Distinct().ToList();
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

            if (trGtypes.Visible)
            {
                enableUpdateView = enableUpdateView && ddlGraphsTypes.SelectedIndex != 0;
                enableResetFilter = enableResetFilter || ddlGraphsTypes.SelectedIndex != 0;
            }
            if (trTitles.Visible)
            {
                if (tdTitles.Visible)
                {
                    enableUpdateView = enableUpdateView && cblTitles.SelectedValues != null ? cblTitles.isSelected : false;
                    enableResetFilter = enableResetFilter || cblTitles.SelectedValues != null ? cblTitles.isSelected : false;

                }
                else
                {
                    enableUpdateView = enableUpdateView && cblSkills.SelectedValues != null ? cblSkills.isSelected : false;
                    enableResetFilter = enableResetFilter || cblSkills.SelectedValues != null ? cblSkills.isSelected : false;
                }

            }

            btnUpdateView.Enabled = enableUpdateView;
            btnResetFilter.Enabled = enableResetFilter;

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
                ucDetails.PopulateData();
            }
            else
            {
                trGtypes.Visible = true;
                trTitles.Visible = ddlGraphsTypes.SelectedIndex != 0;
                string selectedValues = null;
                if (ddlGraphsTypes.SelectedValue == "1" && ddlPeriod.SelectedValue != "-1")
                {
                    selectedValues = cblTitles.SelectedItems;
                    hdnTitles.Value = selectedValues;
                    hdnTitleOrSkill.Value = "Title";
                    ucGraphs.PopulateGraph(ddlGraphsTypes.SelectedValue, selectedValues);
                }
                else if (ddlGraphsTypes.SelectedValue == "2" && ddlPeriod.SelectedValue != "-1")
                {
                    selectedValues = cblSkills.SelectedItems;
                    hdnSkills.Value = selectedValues;
                    hdnTitleOrSkill.Value = "Skill";
                    ucGraphs.PopulateGraph(ddlGraphsTypes.SelectedValue, selectedValues);
                }

            }
            enableDisableResetButtons();
        }

        private void SwitchView(Control control, int viewIndex)
        {
            SelectView(control, viewIndex);
            if (mvConsultingDemandReport.ActiveViewIndex == 2)
            {
                ddlGraphsTypes.SelectedValue = "1";
                cblTitles.SelectAll();
                cblSkills.SelectAll();
            }
            enableDisableResetButtons();
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

        #endregion

    }
}

