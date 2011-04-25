using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web.UI.MobileControls;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls.Generic.TotalCalculator;
using PraticeManagement.Utils;
using System.Web.UI;
using System.Linq;

namespace PraticeManagement.Controls.TimeEntry
{
    public partial class TimeEntries : UserControl
    {
        #region Tab Index

        private static short _tabIndex = DefaultTabIndex;

        public static short TabIndex
        {
            get
            {
                return _tabIndex++;
            }
        }

        protected static void FlushTabIndex()
        {
            _tabIndex = DefaultTabIndex;
        }

        #endregion

        #region Constants

        private const string CONTROL_TeBar = "bar";
        private const string CONTROL_TeRepeater = "tes";
        private const string CONTROL_TeSingle = "ste";
        private const string CONTROL_TeActualHours = "tbActualHours";
        private const string CONTROL_columnTotalHours = "extTotalHours";
        private const string CONTROL_lblcolumnTotalHours = "lblTotalHours";
        private const string VIEW_STATE_SELECTED_PERSON = "1F789A8C-DCAC-498C-803D-594725E3C706";
        private const string VIEW_STATE_SELECTED_DATES = "AD8AD17B-AB33-4A39-802B-091E58090FC1";
        private const string VIEW_STATE_SELECTED_MPES = "A753603D-4746-4B07-B914-29F040022F6C";
        private const string dropdown_ErrorMessageKey = "MilestoneProjectTimeType";
        private const int DefaultTabIndex = 5;

        #endregion

        #region Properties

        public Person SelectedPerson
        {
            get
            {
                return ViewState[VIEW_STATE_SELECTED_PERSON] as Person;
            }

            set
            {
                ViewState[VIEW_STATE_SELECTED_PERSON] = value;
            }
        }

        public DateTime[] SelectedDates
        {
            get
            {
                return ViewState[VIEW_STATE_SELECTED_DATES] as DateTime[];
            }

            set
            {
                ViewState[VIEW_STATE_SELECTED_DATES] = value;
            }
        }

        public MilestonePersonEntry[] MilestonePersonEntries
        {
            get
            {
                return ViewState[VIEW_STATE_SELECTED_MPES] as MilestonePersonEntry[];
            }

            set
            {
                ViewState[VIEW_STATE_SELECTED_MPES] = value;
            }
        }

        public IEnumerable<TimeEntryBar> Bars
        {
            get
            {
                foreach (Control control in tes.Controls)
                    yield return control.FindControl(CONTROL_TeBar) as TimeEntryBar;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void UpdateTimeEntries()
        {
            if (SelectedDates.Length > 0)
            {
                if (SelectedPerson != null)
                    DatabindAndShowGrid(false);
                else
                    mlMessage.ShowInfoMessage(Resources.Messages.PersonIsNotAssgned);
            }
        }

        internal void DatabindAndShowGrid(bool addEmpty)
        {
            var grid = TimeEntryHelper.GetTeGrid(
                                SelectedPerson,
                                SelectedDates,
                                MilestonePersonEntries);

            bool isEmpty = grid.IsEmptyResultSet;
            if (!isEmpty)
            {
                repEntriesHeader.DataSource = SelectedDates;
                repEntriesHeader.DataBind();

                if (addEmpty)
                    grid.AddEmptyRow();

                tes.DataSource = grid;
                tes.DataBind();

                repTotalHours.DataSource = SelectedDates;
                repTotalHours.DataBind();

                FlushTabIndex();
            }
            else
            {
                mlMessage.ShowInfoMessage(Resources.Messages.PersonIsNotAssgned);
            }

            pnlGrid.Visible = !isEmpty;
        }

        //public void bar_OnRowRemoved(object sender, EventArgs e)
        //{
        //    UpdateTimeEntries();
        //}

        protected void repEntries_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var bar = e.Item.FindControl(CONTROL_TeBar) as TimeEntryBar;
            var row = e.Item.DataItem as TeGridRow;

            bar.RowBehind = row;
            bar.UpdateTimeEntries();
        }

        internal bool SaveData()
        {
            var saved = true;

            foreach (var bar in Bars)
            {
                if (bar.TimeEntryControls.Any(te => te.TimeEntryBehind != null || te.Modified))
                {
                    var ddlProjectMilestone = bar.FindControl("ddlProjectMilestone") as DropDownList;
                    var ddlTimeTypes = bar.FindControl("ddlTimeTypes") as DropDownList;

                    if (ddlProjectMilestone.SelectedIndex == 0)
                    {
                        ddlProjectMilestone.BorderColor = System.Drawing.Color.Red;
                        Session[dropdown_ErrorMessageKey] = "True";

                    }
                    if (ddlTimeTypes.SelectedIndex == 0)
                    {
                        ddlTimeTypes.BorderColor = System.Drawing.Color.Red;
                        Session[dropdown_ErrorMessageKey] = "True";
                    }
                }


                foreach (var entryControl in bar.TimeEntryControls)
                {
                    try
                    {
                        bool itemSaved = true;
                        if (entryControl.Modified)
                            itemSaved = entryControl.SaveTimeEntry();

                        if (!itemSaved)
                            saved = false;
                    }
                    catch (Exception exception)
                    {
                        mlMessage.ShowErrorMessage(exception.Message);
                        saved = false;
                    }
                }
            }

            return saved;
        }

        protected void Page_PreRender(object sender, EventArgs eventArgs)
        {
            extSpreadsheetTotalHours.ControlsToCheck = string.Empty;
            foreach (RepeaterItem barItem in tes.Items)
            {
                var bar = barItem.FindControl(CONTROL_TeBar) as TimeEntryBar;
                var teRepeater = (Repeater)bar.FindControl(CONTROL_TeRepeater);
                foreach (RepeaterItem teSingle in teRepeater.Items)
                {
                    SingleTimeEntry ste = teSingle.FindControl(CONTROL_TeSingle) as SingleTimeEntry;
                    var tbActualHours = ste.FindControl(CONTROL_TeActualHours);

                    extSpreadsheetTotalHours.ControlsToCheck += tbActualHours.ClientID + ";";
                    ste.SpreadSheetTotalCalculatorExtenderId = extSpreadsheetTotalHours.ClientID;
                }

            }

        }

       /* protected void repTotalHours_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var controlIDList = new List<string>();
            var extColumnTotalHours = (TotalCalculatorExtender)e.Item.FindControl(CONTROL_columnTotalHours);

            foreach (RepeaterItem barItem in tes.Items)
            {
                var bar = barItem.FindControl(CONTROL_TeBar) as TimeEntryBar;
                var teRepeater = (Repeater)bar.FindControl(CONTROL_TeRepeater);
                var teSingle = (SingleTimeEntry)teRepeater.Items[e.Item.ItemIndex].FindControl(CONTROL_TeSingle);
                var tbActualHours = teSingle.FindControl(CONTROL_TeActualHours);

                controlIDList.Add(tbActualHours.ClientID);
                teSingle.HorizontalTotalCalculatorExtenderId = extColumnTotalHours.ClientID;
                teSingle.SpreadSheetTotalCalculatorExtenderId = extSpreadsheetTotalHours.ClientID;
            }

            extColumnTotalHours.ControlsToCheck = string.Join(";", controlIDList.ToArray());

            extSpreadsheetTotalHours.ControlsToCheck
                = string.Format(
                    "{0};{1}",
                    extSpreadsheetTotalHours.ControlsToCheck,
                    extColumnTotalHours.ControlsToCheck).Trim(';');
        }

        protected void repTotalHours_DataBinding(object sender, EventArgs eventArgs)
        {
            extSpreadsheetTotalHours.ControlsToCheck = string.Empty;
        } 
        */
    }
}

