using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.TimeEntry;
using DataTransferObjects;
using System.Xml.Linq;

namespace PraticeManagement.Controls.TimeEntry
{
    public partial class BillableAndNonBillableTimeEntryBar : System.Web.UI.UserControl
    {
        #region Constants

        private const string TeBarDataSourceViewstate = "6sdr8832C-A37A-497F-82A9-58As40C759499";
        private const string BillableAndNonBillableTimeEntryBar_BillableTbAcutualHoursClientIds = "BillableAndNonBillableTimeEntryBar_BillableTbAcutualHoursClientIds";
        private const string BillableAndNonBillableTimeEntryBar_NonBillableTbAcutualHoursClientIds = "BillableAndNonBillableTimeEntryBar_NonBillableTbAcutualHoursClientIds";
        private const string workTypeOldIdAttribute = "workTypeOldId";
        private const string tbNotesId = "tbNotes";
        private const string previousIdAttribute = "previousId";
        private const string steId = "ste";

        #endregion

        #region Properties

        public List<XElement> TeBarDataSource
        {
            get;
            set;
        }

        public TimeEntry_New HostingPage
        {
            get
            {
                return ((TimeEntry_New)Page);
            }
        }

        public string AccountId
        {
            set
            {
                extEnableDisable.AccountId = value;
            }
        }

        public string BusinessUnitId
        {
            set
            {
                extEnableDisable.BusinessUnitId = value;
            }
        }

        public string ProjectId
        {
            set
            {
                extEnableDisable.ProjectId = value;
            }
        }

        public string TdCellProjectSectionClientID
        {
            get
            {
                return tdplusProjectSection.ClientID;
            }
        }

        public TimeTypeRecord[] WorkTypes { get; set; }

        public TimeTypeRecord SelectedWorkType { get; set; }

        public Dictionary<int, string> BillableTbAcutualHoursClientIds
        {
            get
            {
                return ViewState[BillableAndNonBillableTimeEntryBar_BillableTbAcutualHoursClientIds] as Dictionary<int, string>;
            }
            set
            {
                ViewState[BillableAndNonBillableTimeEntryBar_BillableTbAcutualHoursClientIds] = value;
            }
        }

        public Dictionary<int, string> NonBillableTbAcutualHoursClientIds
        {
            get
            {
                return ViewState[BillableAndNonBillableTimeEntryBar_NonBillableTbAcutualHoursClientIds] as Dictionary<int, string>;
            }
            set
            {
                ViewState[BillableAndNonBillableTimeEntryBar_NonBillableTbAcutualHoursClientIds] = value;
            }
        }

        #endregion

        #region Control events

        protected void Page_PreRender(object sender, EventArgs e)
        {
            extEnableDisable.WeekStartDate = HostingPage.SelectedDates[0].ToString();
            extEnableDisable.PersonId = HostingPage.SelectedPerson.Id.ToString();
            extEnableDisable.PopUpBehaviourId = TimeEntry_New.mpeTimetypeAlertMessageBehaviourId;
            if (!string.IsNullOrEmpty(imgDropTes.Attributes[TimeEntry_New.workTypeOldId]) && imgDropTes.Attributes[TimeEntry_New.workTypeOldId] != "-1")
            {
                imgDropTes.Visible =
                ddlTimeTypes.Enabled = !HostingPage.IsReadOnly;
            }
        }

        protected void repEntries_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var billableAndNonBillableSte = e.Item.FindControl(steId) as BillableAndNonBillableSingleTimeEntry;

                var calendarItem = (XElement)e.Item.DataItem;

                var billableTbId = billableAndNonBillableSte.BillableTextBoxClientId;
                var nonBillableTbId = billableAndNonBillableSte.NonBillableTextBoxClientId;
                var tbNotes = billableAndNonBillableSte.FindControl(tbNotesId) as TextBox;

                extTotalHours.ControlsToCheck += billableTbId + ";" + nonBillableTbId + ";";
                extEnableDisable.ControlsToCheck += billableTbId + ";" + nonBillableTbId + ";";

                billableAndNonBillableSte.HorizontalTotalCalculatorExtenderId = extTotalHours.ClientID;

                billableAndNonBillableSte.IsNoteRequired = calendarItem.Attribute(XName.Get(TimeEntry_New.IsNoteRequiredXname)).Value;
                billableAndNonBillableSte.IsHourlyRevenue = calendarItem.Attribute(XName.Get(TimeEntry_New.IsHourlyRevenueXname)).Value;
                billableAndNonBillableSte.IsChargeCodeTurnOff = calendarItem.Attribute(XName.Get(TimeEntry_New.IsChargeCodeOffXname)).Value;

                BillableTbAcutualHoursClientIds = BillableTbAcutualHoursClientIds ?? new Dictionary<int, string>();
                BillableTbAcutualHoursClientIds.Add(e.Item.ItemIndex, billableTbId);
                BillableTbAcutualHoursClientIds = BillableTbAcutualHoursClientIds;

                NonBillableTbAcutualHoursClientIds = NonBillableTbAcutualHoursClientIds ?? new Dictionary<int, string>();
                NonBillableTbAcutualHoursClientIds.Add(e.Item.ItemIndex, nonBillableTbId);
                NonBillableTbAcutualHoursClientIds = NonBillableTbAcutualHoursClientIds;

                var bterecord = (calendarItem.HasElements && calendarItem.Descendants(XName.Get(TimeEntry_New.TimeEntryRecordXname)).Where(ter => ter.Attribute(XName.Get(TimeEntry_New.IsChargeableXname)).Value.ToLowerInvariant() == "true").ToList().Count > 0) ? calendarItem.Descendants(XName.Get(TimeEntry_New.TimeEntryRecordXname)).Where(ter => ter.Attribute(XName.Get(TimeEntry_New.IsChargeableXname)).Value.ToLowerInvariant() == "true").First() : null;
                var nbterecord = (calendarItem.HasElements && calendarItem.Descendants(XName.Get(TimeEntry_New.TimeEntryRecordXname)).Where(ter => ter.Attribute(XName.Get(TimeEntry_New.IsChargeableXname)).Value.ToLowerInvariant() == "false").ToList().Count > 0) ? calendarItem.Descendants(XName.Get(TimeEntry_New.TimeEntryRecordXname)).Where(ter => ter.Attribute(XName.Get(TimeEntry_New.IsChargeableXname)).Value.ToLowerInvariant() == "false").First() : null;
                var date = Convert.ToDateTime(calendarItem.Attribute(XName.Get(TimeEntry_New.DateXname)).Value);

                InitTimeEntryControl(billableAndNonBillableSte, date, bterecord, nbterecord);

            }

        }

        protected void ddlTimeTypes_DataBound(object sender, EventArgs e)
        {
            if (ddlTimeTypes.Items.FindByValue("-1") == null)
                ddlTimeTypes.Items.Insert(0, (new ListItem("- - Select Work Type - -", "-1")));
        }

        protected void imgDropTes_Click(object sender, ImageClickEventArgs e)
        {

            var imgDropTes = ((ImageButton)(sender));

            var repProjectTesItem = imgDropTes.NamingContainer.NamingContainer as RepeaterItem;
            var ProjectTesItemIndex = repProjectTesItem.ItemIndex;

            int projectId = Convert.ToInt32(imgDropTes.Attributes[TimeEntry_New.ProjectIdXname]);
            int accountId = Convert.ToInt32(imgDropTes.Attributes[TimeEntry_New.AccountIdXname]);
            int workTypeOldID;
            int.TryParse(imgDropTes.Attributes[workTypeOldIdAttribute], out workTypeOldID);
            int personId = HostingPage.SelectedPerson.Id.Value;
            DateTime[] dates = HostingPage.SelectedDates;

            //Remove Worktype from xml
            HostingPage.RemoveWorktypeFromXMLForProjectSection(accountId, projectId, ProjectTesItemIndex);

            //Delete TimeEntry from database
            if (workTypeOldID > 0)
            {
                ServiceCallers.Custom.TimeEntry(te => te.DeleteTimeEntry(accountId, projectId, personId, workTypeOldID, dates[0], dates[dates.Length - 1], Context.User.Identity.Name));
            }
        }

        #endregion

        #region Methods

        protected string GetDayOffCssClass(XElement calendarItem)
        {
            return calendarItem.Attribute(XName.Get(TimeEntry_New.CssClassXname)).Value;
        }

        private void InitTimeEntryControl(BillableAndNonBillableSingleTimeEntry ste, DateTime date, XElement bterXlement, XElement nonbterXlement)
        {
            ste.DateBehind = date;
            ste.TimeEntryRecordBillableElement = bterXlement;
            ste.TimeEntryRecordNonBillableElement = nonbterXlement;
        }

        public void UpdateTimeEntries()
        {

            ddlTimeTypes.Items.Clear();
            ddlTimeTypes.DataSource = WorkTypes;
            ddlTimeTypes.DataBind();

            if (SelectedWorkType != null && SelectedWorkType.Id > 0)
            {
                ddlTimeTypes.SelectedValue = SelectedWorkType.Id.ToString();
                if (ddlTimeTypes.SelectedIndex == 0)
                {
                    string timetypename = ServiceCallers.Custom.TimeType(te => te.GetWorkTypeNameById(SelectedWorkType.Id));
                    ddlTimeTypes.Items.Add(new ListItem(timetypename, SelectedWorkType.Id.ToString()));
                    ddlTimeTypes.SelectedValue = SelectedWorkType.Id.ToString();
                    ddlTimeTypes.Attributes[TimeEntry_New.selectedInActiveWorktypeid] = SelectedWorkType.Id.ToString();
                    ddlTimeTypes.Attributes[TimeEntry_New.selectedInActiveWorktypeName] = timetypename;
                }
            }
            else
            {
                ddlTimeTypes.SelectedIndex = 0;
            }

            ddlTimeTypes.Attributes[previousIdAttribute] = ddlTimeTypes.SelectedValue.ToString();
            HostingPage.DdlWorkTypeIdsList += ddlTimeTypes.ClientID + ";";



            tes.DataSource = TeBarDataSource;
            tes.DataBind();
        }

        internal void UpdateNoteAndActualHours(List<XElement> calendarItemElements)
        {
            for (int k = 0; k < calendarItemElements.Count; k++)
            {

                var billableAndNonbillableSte = tes.Items[k].FindControl(steId) as BillableAndNonBillableSingleTimeEntry;

                var calendarItemElement = calendarItemElements[k];
                if (calendarItemElement.HasElements)
                {
                    var bElements = calendarItemElement.Descendants(XName.Get(TimeEntry_New.TimeEntryRecordXname)).ToList().Count > 0 ? calendarItemElement.Descendants(XName.Get(TimeEntry_New.TimeEntryRecordXname)).Where(ter => ter.Attribute(XName.Get(TimeEntry_New.IsChargeableXname)).Value.ToLowerInvariant() == "true").ToList() : null;
                    var nonBillableElements = calendarItemElement.Descendants(XName.Get(TimeEntry_New.TimeEntryRecordXname)).ToList().Count > 0 ? calendarItemElement.Descendants(XName.Get(TimeEntry_New.TimeEntryRecordXname)).Where(ter => ter.Attribute(XName.Get(TimeEntry_New.IsChargeableXname)).Value.ToLowerInvariant() == "false").ToList() : null;

                    if (bElements != null && bElements.Count > 0)
                    {
                        var billableElement = bElements.First();
                        billableAndNonbillableSte.UpdateBillableElementEditedValues(billableElement);

                        Decimal i = 1;
                        Decimal.TryParse(billableElement.Attribute(XName.Get(TimeEntry_New.ActualHoursXname)).Value, out i);

                        bool isHoursZero = (i == 0);
                        if ((billableElement.Attribute(XName.Get(TimeEntry_New.ActualHoursXname)).Value == string.Empty && billableElement.Attribute(XName.Get(TimeEntry_New.NoteXname)).Value == "" && !HostingPage.IsSaving) || (billableElement.Attribute(XName.Get("ActualHours")).Value == string.Empty && HostingPage.IsSaving))
                        {
                            billableElement.Remove();
                        }

                    }
                    else
                    {
                        AddBillableElement(billableAndNonbillableSte, calendarItemElement);
                    }

                    if (nonBillableElements != null && nonBillableElements.Count > 0)
                    {
                        var nonBillableElement = nonBillableElements.First();
                        billableAndNonbillableSte.UpdateNonBillableElementEditedValues(nonBillableElement);

                        if ((nonBillableElement.Attribute(XName.Get(TimeEntry_New.ActualHoursXname)).Value == string.Empty && nonBillableElement.Attribute(XName.Get("Note")).Value == "" && !HostingPage.IsSaving) || (nonBillableElement.Attribute(XName.Get(TimeEntry_New.ActualHoursXname)).Value == string.Empty && HostingPage.IsSaving))
                        {
                            nonBillableElement.Remove();
                        }

                    }
                    else
                    {
                        AddNonBillableElement(billableAndNonbillableSte, calendarItemElement);
                    }
                }
                else
                {
                    //Add Element
                    AddBillableElement(billableAndNonbillableSte, calendarItemElement);
                    AddNonBillableElement(billableAndNonbillableSte, calendarItemElement);
                }
            }


        }

        private void AddBillableElement(BillableAndNonBillableSingleTimeEntry billableSte, XElement calendarItemElement)
        {
            var billableElement = new XElement(TimeEntry_New.TimeEntryRecordXname);
            billableElement.SetAttributeValue(XName.Get(TimeEntry_New.IsChargeableXname), "true");
            billableElement.SetAttributeValue(XName.Get(TimeEntry_New.IsReviewedXname), "Pending");
            billableSte.UpdateBillableElementEditedValues(billableElement);

            if (billableElement.Attribute(XName.Get(TimeEntry_New.ActualHoursXname)).Value != "" || (!HostingPage.IsSaving && billableElement.Attribute(XName.Get(TimeEntry_New.NoteXname)).Value != ""))
            {
                calendarItemElement.Add(billableElement);
            }
        }

        private void AddNonBillableElement(BillableAndNonBillableSingleTimeEntry nonBillableSte, XElement calendarItemElement)
        {
            var nonBillableElement = new XElement(TimeEntry_New.TimeEntryRecordXname);
            nonBillableElement.SetAttributeValue(XName.Get(TimeEntry_New.IsChargeableXname), "false");
            nonBillableElement.SetAttributeValue(XName.Get(TimeEntry_New.IsReviewedXname), "Pending");
            nonBillableSte.UpdateNonBillableElementEditedValues(nonBillableElement);

            if (nonBillableElement.Attribute(XName.Get(TimeEntry_New.ActualHoursXname)).Value != "" || (!HostingPage.IsSaving && nonBillableElement.Attribute(XName.Get(TimeEntry_New.NoteXname)).Value != ""))
            {
                calendarItemElement.Add(nonBillableElement);
            }
        }

        internal void UpdateWorkType(XElement workTypeElement, XElement accountAndProjectSelectionElement)
        {
            workTypeElement.Attribute(XName.Get(TimeEntry_New.IdXname)).Value = ddlTimeTypes.SelectedValue;
            string OldId = workTypeElement.Attribute(XName.Get(TimeEntry_New.OldIdXname)) != null ? workTypeElement.Attribute(XName.Get(TimeEntry_New.OldIdXname)).Value : null;
            if (!String.IsNullOrEmpty(OldId) && ddlTimeTypes.SelectedValue != OldId)
            {
                //update the ischargecodeturnoff in the xml for the calenderitems
                int accountId = Convert.ToInt32(accountAndProjectSelectionElement.Attribute(XName.Get(TimeEntry_New.AccountIdXname)).Value);
                int projectId = Convert.ToInt32(accountAndProjectSelectionElement.Attribute(XName.Get(TimeEntry_New.ProjectIdXname)).Value);
                int businessUnitId = Convert.ToInt32(accountAndProjectSelectionElement.Attribute(XName.Get(TimeEntry_New.BusinessUnitIdXname)).Value);
                int personId = HostingPage.SelectedPerson.Id.Value;
                DateTime startDate = HostingPage.SelectedDates[0];
                DateTime endDate = HostingPage.SelectedDates[HostingPage.SelectedDates.Length - 1];
                int timeEntryId = Convert.ToInt32(ddlTimeTypes.SelectedValue);
                Dictionary<DateTime, bool> isChargeCodeTurnOffList = ServiceCallers.Custom.TimeEntry(p => p.GetIsChargeCodeTurnOffByPeriod(personId, accountId, businessUnitId, projectId, timeEntryId, startDate, endDate));
                var calendarItemElements = workTypeElement.Descendants(XName.Get(TimeEntry_New.CalendarItemXname)).ToList();

                for (int j = 0; j < calendarItemElements.Count; j++)
                {
                    var calendarItemElement = calendarItemElements[j];
                    calendarItemElement.Attribute(XName.Get(TimeEntry_New.IsChargeCodeOffXname)).Value = isChargeCodeTurnOffList[startDate.AddDays(j)].ToString();
                }


            }
        }

        internal void UpdateVerticalTotalCalculatorExtenderId(int index, string clientId)
        {
            var billableAndNonbillableSte = tes.Items[index].FindControl(steId) as BillableAndNonBillableSingleTimeEntry;
            billableAndNonbillableSte.UpdateVerticalTotalCalculatorExtenderId(clientId);
        }

        private bool ValideWorkTypeDropDown()
        {
            var result = ddlTimeTypes.SelectedIndex > 0;
            if (result)
            {
                ddlTimeTypes.Style["background-color"] = "none";
            }
            else
            {
                ddlTimeTypes.Style["background-color"] = "red";
            }

            return result;
        }

        internal void ValidateAll()
        {
            bool isThereAtleastOneTimeEntryrecord = false;

            foreach (RepeaterItem tesItem in tes.Items)
            {
                var billableAndNonbillableSte = tesItem.FindControl(steId) as BillableAndNonBillableSingleTimeEntry;

                if (!isThereAtleastOneTimeEntryrecord)
                {
                    isThereAtleastOneTimeEntryrecord = billableAndNonbillableSte.IsThereAtleastOneTimeEntryrecord;
                }

                billableAndNonbillableSte.ValidateNoteAndHours();
            }

            if (isThereAtleastOneTimeEntryrecord && !ValideWorkTypeDropDown())
            {
                HostingPage.IsValidWorkType = false;
            }

        }

        #endregion

    }
}

