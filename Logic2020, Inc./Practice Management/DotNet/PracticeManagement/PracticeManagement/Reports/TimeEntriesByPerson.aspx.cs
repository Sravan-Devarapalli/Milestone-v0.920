using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using DataTransferObjects.TimeEntry;
using DataTransferObjects.Utils;
using PraticeManagement.Controls;
using PraticeManagement.Controls.Persons;
using PraticeManagement.Controls.TimeEntry;
using DataTransferObjects.CompositeObjects;
using System.Linq;
using System.Web.Security;
using DataTransferObjects;

namespace PraticeManagement.Sandbox
{
    public partial class TimeEntriesByPerson : PracticeManagementPageBase
    {
        protected DateTime CurrDate;
        protected double GrandTotal;
        protected double ProjectTotals;
        protected int ColspanForTotals;
        protected void btnUpdate_OnClick(object sender, EventArgs e)
        {
            dlPersons.DataBind();
            System.Web.UI.ScriptManager.RegisterClientScriptBlock(updReport, updReport.GetType(), "", "SetDivWidth();", true);
            if (hdnFiltersChanged.Value == "false")
            {
                btnResetFilter.Attributes.Add("disabled", "true");
            }
            else
            {
                btnResetFilter.Attributes.Remove("disabled");
            }
            AddAttributesToCheckBoxes(this.cblPractices);
            AddAttributesToCheckBoxes(this.cblTimeScales);
            AddAttributesToCheckBoxes(this.cblPersons);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Utils.Generic.InitStartEndDate(diRange);
                Populatepersons(true);
                DataHelper.FillPracticeList(this.cblPractices, Resources.Controls.AllPracticesText);
                DataHelper.FillTimescaleList(this.cblTimeScales, Resources.Controls.AllTypes);
                SelectAllItems(this.cblPractices);
                SelectAllItems(this.cblTimeScales);
                if (hdnFiltersChanged.Value == "false")
                {
                    btnResetFilter.Attributes.Add("disabled", "true");
                }
                else
                {
                    btnResetFilter.Attributes.Remove("disabled");
                }
                AddAttributesToCheckBoxes(this.cblPractices);
                AddAttributesToCheckBoxes(this.cblTimeScales);
                AjaxControlToolkit.CalendarExtender fromDateExt = diRange.FindControl("clFromDate") as AjaxControlToolkit.CalendarExtender;
                AjaxControlToolkit.CalendarExtender toDateExt = diRange.FindControl("clToDate") as AjaxControlToolkit.CalendarExtender;

                TextBox fromDate = diRange.FindControl("tbFrom") as TextBox;
                TextBox toDate = diRange.FindControl("tbTo") as TextBox;

                fromDate.AutoPostBack = true;
                toDate.AutoPostBack = true;
                fromDate.CausesValidation = true;
                toDate.CausesValidation = true;
                fromDateExt.OnClientDateSelectionChanged = "EnableResetButton";
                toDateExt.OnClientDateSelectionChanged = "EnableResetButton";
                //fromDate.Attributes.Add("onchange", "EnableResetButton();");
                //toDate.Attributes.Add("onchange", "EnableResetButton();");

            }

            TextBox from= diRange.FindControl("tbFrom") as TextBox;
            TextBox to = diRange.FindControl("tbTo") as TextBox;
            from.TextChanged += diRange_SelectionChanged;
            to.TextChanged += diRange_SelectionChanged;
                     
        }

              
        private void AddAttributesToCheckBoxes(ScrollingDropDown ddlpractices)
        {
            foreach (ListItem item in ddlpractices.Items)
            {
                item.Attributes.Add("onclick", "EnableResetButton();");
            }
        }

        private string GetStatusIds()
        {
            string statusList = string.Empty;
            if (chbActivePersons.Checked)
                statusList += ((int)PersonStatusType.Active).ToString();
            if (chbInactivePersons.Checked)
                statusList += (string.IsNullOrEmpty(statusList) ? string.Empty : ",") + ((int)PersonStatusType.Inactive).ToString();
            if (chbTerminatedPersons.Checked)
                statusList += (string.IsNullOrEmpty(statusList) ? string.Empty : ",") + ((int)PersonStatusType.Terminated).ToString();
            if (string.IsNullOrEmpty(statusList))
                statusList = "-1";
            return statusList;
        }

        protected override void Display()
        {
            //Utils.Generic.InitStartEndDate(diRange);
        }

        protected void wsChoose_WeekChanged(object sender, WeekChangedEventArgs args)
        {
        }

        protected void pcPersons_PersonChanged(object sender, PersonChangedEventArguments args)
        {
        }

        protected int GetColspan(DateTime date, int index)
        {
            if (index == 0)
                CurrDate = diRange.FromDate.HasValue ? diRange.FromDate.Value : DateTime.Now;

            var days = date.Subtract(CurrDate).Days;
            var colspan = (index == 0 ? days : days - 1) * 2 + 1;

            CurrDate = date;

            return colspan;
        }

        protected void repTeTable_OnItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Footer)
            {
                var dsource = ((sender as Repeater).DataSource as Dictionary<TimeEntryHours, TimeEntryRecord[]>);
                if (dsource != null)
                {
                    var totalsFooter = e.Item.FindControl("dlTotals") as Repeater;
                    if (totalsFooter != null)
                    {
                        totalsFooter.DataSource = Generic.GetTotalsByDate(dsource);
                        totalsFooter.DataBind();
                    }
                    var tdextracolumns = e.Item.FindControl("tdExtracolumns") as System.Web.UI.HtmlControls.HtmlTableCell;
                    if (!dsource.Any())
                    {
                        if (tdextracolumns != null)
                        {
                            tdextracolumns.ColSpan = ColspanForTotals * 2 + 1;
                        }
                    }
                    else
                    {
                        if (tdextracolumns != null)
                        {
                            tdextracolumns.ColSpan = GetLastColspan();
                        }
                    }
                }
            }
        }

        protected void gvTimeEntries_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                var dsource = (sender as GridView).DataSource as List<TimeEntryRecord>;
                if (dsource != null)
                {
                    var tatalLabel = e.Row.FindControl("lblGvGridTotal") as Label;
                    tatalLabel.Text = GetTotalActualHours(dsource).ToString(PraticeManagement.Constants.Formatting.DoubleFormat);
                }
            }
        }

        private double GetTotalActualHours(List<TimeEntryRecord> timeEntries)
        {
            try
            {
                return timeEntries.Sum(item => item.ActualHours);
            }
            catch (Exception ex)
            {
                return 0.00;
            }
        }

        protected int GetLastColspan()
        {
            var endDate = diRange.ToDate.HasValue ? diRange.ToDate.Value : DateTime.Now;
            return GetColspan(endDate.AddDays(1), int.MaxValue);
        }

        protected void dlTotals_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex == 0)
                GrandTotal = 0;

            if (e.Item.DataItem is KeyValuePair<DateTime, double>)
                GrandTotal += ((KeyValuePair<DateTime, double>)e.Item.DataItem).Value;
        }

        protected void dlProject_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex == 0)
                ProjectTotals = 0;

            if (e.Item.DataItem is TimeEntryRecord)
                ProjectTotals += ((TimeEntryRecord)e.Item.DataItem).ActualHours;
        }

        protected void dlTotals_OnInit(object sender, EventArgs e)
        {
            GrandTotal = 0;
        }

        protected void dlProject_OnItemCreated(object sender, EventArgs e)
        {
            ColspanForTotals += 1;
        }

        protected void dlProject_OnInit(object sender, EventArgs e)
        {
            ColspanForTotals = 0;
        }

        protected void odsCalendar_OnSelecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (!diRange.FromDate.HasValue)
            {
                e.Cancel = true;
                return;
            }

            e.InputParameters["startDate"] = this.diRange.FromDate.Value;
            e.InputParameters["endDate"] = this.diRange.ToDate.HasValue ? this.diRange.ToDate.Value : DateTime.Today;
        }

        protected void odsPersonTimeEntries_OnSelecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (!diRange.FromDate.HasValue)
            {
                e.Cancel = true;
                return;
            }

            e.InputParameters["personIds"] = cblPersons.SelectedValues;
            e.InputParameters["startDate"] = this.diRange.FromDate.Value;
            e.InputParameters["endDate"] = this.diRange.ToDate.HasValue ? this.diRange.ToDate.Value : DateTime.Today;
            e.InputParameters["payTypeIds"] = this.cblTimeScales.SelectedValues;
            e.InputParameters["practiceIds"] = this.cblPractices.SelectedValues;
        }

        protected void odsCumulativeTes_OnSelecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (!diRange.FromDate.HasValue)
            {
                e.Cancel = true;
                return;
            }

            e.InputParameters["personIds"] = cblPersons.SelectedValues;//this.prfilter.SelectedValues;
            e.InputParameters["startDate"] = this.diRange.FromDate.Value;
            e.InputParameters["endDate"] = this.diRange.ToDate.HasValue ? this.diRange.ToDate.Value : DateTime.Today;
        }

        protected void btnResetFilter_OnClick(object sender, EventArgs e)
        {
            Utils.Generic.InitStartEndDate(diRange);
            this.chbActivePersons.Checked = true;
            this.chbInactivePersons.Checked = this.chbTerminatedPersons.Checked = false;
            SelectAllItems(this.cblPractices);
            SelectAllItems(this.cblTimeScales);
            Populatepersons(false);
            hdnFiltersChanged.Value = "false";
            btnResetFilter.Attributes.Add("disabled", "true");
            AddAttributesToCheckBoxes(this.cblPractices);
            AddAttributesToCheckBoxes(this.cblTimeScales);
            SelectCurrentPerson(DataHelper.CurrentPerson.Id);
        }

        private void SelectAllItems(ScrollingDropDown ddlpractices)
        {
            foreach (ListItem item in ddlpractices.Items)
            {
                item.Selected = true;
            }
        }

        protected void PersonStatus_OnCheckedChanged(object sender, EventArgs e)
        {
            var currentlySelectedPersons = this.cblPersons.SelectedValues;

            Populatepersons(false);
            this.cblPersons.DataSource = null;

            foreach (var selectedPersonId in currentlySelectedPersons)
            {

                ListItem item = this.cblPersons.Items.FindByValue(selectedPersonId.ToString());
                if (item != null)
                    item.Selected = true;
            }
            this.cblPersons.Items[0].Selected = false;
            if (hdnFiltersChanged.Value == "false")
            {
                btnResetFilter.Attributes.Add("disabled", "true");
            }
            else
            {
                btnResetFilter.Attributes.Remove("disabled");
            }
        }

        protected void diRange_SelectionChanged(object sender, EventArgs e)
        {
            if (diRange.FromDate <= diRange.ToDate)
            {
                PersonStatus_OnCheckedChanged(sender, e);
            }
        }

        private void Populatepersons(bool enableDisableChevron)
        {
            var currentPerson = DataHelper.CurrentPerson;
            var personRoles = Roles.GetRolesForUser(currentPerson.Alias);
            string statusIdsList = GetStatusIds();
            int? personId = null;
            if (!personRoles.Any(s => s == "Administrator"))
            {
                personId = currentPerson.Id;
                if (enableDisableChevron)
                {
                    this.cpe.Enabled = false;
                    pnlFilters.Visible = false;
                }

            }
            DataHelper.FillTimeEntryPersonListBetweenStartAndEndDates(this.cblPersons, Resources.Controls.AllPersons, null, statusIdsList, personId, diRange.FromDate, diRange.ToDate);
            AddAttributesToCheckBoxes(this.cblPersons);
        }

        protected void Page_SaveStateComplete(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SelectCurrentPerson(DataHelper.CurrentPerson.Id);
            }
        }

        private void SelectCurrentPerson(int? personId)
        {
            if (personId.HasValue)
            {
                var currentPersonItem = this.cblPersons.Items.FindByValue(personId.ToString());
                if (currentPersonItem != null)
                {
                    currentPersonItem.Selected = true;
                    
                }
            }
        }

        protected void dlPersons_OnItemDataBound(object sender, DataListItemEventArgs e)
        {

            var dataItem = (KeyValuePair<TimeEntriesGroupedByPersonProject, Dictionary<TimeEntryHours, TimeEntryRecord[]>>)e.Item.DataItem;
            if (dataItem.Key.GroupedTimeEtnries == null || dataItem.Key.GroupedTimeEtnries.Count == 0)
            {
                var divProjects = e.Item.FindControl("divProjects") as System.Web.UI.HtmlControls.HtmlGenericControl;
                var divTeTable = e.Item.FindControl("divTeTable") as System.Web.UI.HtmlControls.HtmlGenericControl;
                var lblnoDataMesssage = e.Item.FindControl("lblnoDataMesssage") as Label;
                lblnoDataMesssage.Visible = true;
                divProjects.Visible = false;
                divTeTable.Visible = false;
            }
        }
    }
}

