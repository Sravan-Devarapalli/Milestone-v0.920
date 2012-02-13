using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using DataTransferObjects.TimeEntry;
using System.Xml.Linq;

namespace PraticeManagement.Controls.TimeEntry
{
    public partial class AdministrativeTimeEntryBar : System.Web.UI.UserControl
    {
        #region Constants

        private const string AdministrativeTimeEntryBar_TbAcutualHoursClientIds = "AdministrativeTimeEntryBar_TbAcutualHoursClientIds";
        private const string steId = "ste";

        #endregion

        #region Properties

        public List<XElement> TeBarDataSource
        {
            get;
            set;
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

        public bool IsPTO { get; set; }

        public bool IsHoliday { get; set; }

        public TimeTypeRecord WorkType { get; set; }

        public Dictionary<int, string> TbAcutualHoursClientIds
        {
            get
            {
                return ViewState[AdministrativeTimeEntryBar_TbAcutualHoursClientIds] as Dictionary<int, string>;
            }
            set
            {
                ViewState[AdministrativeTimeEntryBar_TbAcutualHoursClientIds] = value;
            }
        }

        public TimeEntry_New HostingPage
        {
            get
            {
                return ((TimeEntry_New)Page);
            }
        }

        #endregion

        #region Control events

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            extEnableDisable.WeekStartDate = HostingPage.SelectedDates[0].ToString();
            extEnableDisable.PersonId = HostingPage.SelectedPerson.Id.ToString();
            extEnableDisable.PopUpBehaviourId = TimeEntry_New.mpeTimetypeAlertMessageBehaviourId;
        }

        protected void repEntries_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ste = e.Item.FindControl(steId) as SingleTimeEntry_New;

                var calendarItem = (XElement)e.Item.DataItem;

                var textBoxId = ste.Controls[1].ClientID;

                TbAcutualHoursClientIds = TbAcutualHoursClientIds ?? new Dictionary<int, string>();

                TbAcutualHoursClientIds.Add(e.Item.ItemIndex, textBoxId);
                TbAcutualHoursClientIds = TbAcutualHoursClientIds;
                extTotalHours.ControlsToCheck += textBoxId + ";";
                if (IsPTO)
                {
                    extEnableDisable.ControlsToCheck += textBoxId + ";";
                }

                ste.HorizontalTotalCalculatorExtenderId = extTotalHours.ClientID;
                ste.IsNoteRequired = calendarItem.Attribute(XName.Get(TimeEntry_New.IsNoteRequiredXname)).Value;
                ste.IsChargeCodeTurnOff = calendarItem.Attribute(XName.Get(TimeEntry_New.IsChargeCodeOffXname)).Value;

                if (IsHoliday)
                {
                    ste.Disabled = true;
                }

                ste.IsPTO = IsPTO;
                ste.IsHoliday = IsHoliday;

                var nbterecord = (calendarItem.HasElements && calendarItem.Descendants(XName.Get(TimeEntry_New.TimeEntryRecordXname)).Where(ter => ter.Attribute(XName.Get("IsChargeable")).Value.ToLowerInvariant() == "false").ToList().Count > 0) ? calendarItem.Descendants(XName.Get("TimeEntryRecord")).Where(ter => ter.Attribute(XName.Get("IsChargeable")).Value.ToLowerInvariant() == "false").First() : null;
                var date = Convert.ToDateTime(calendarItem.Attribute(XName.Get(TimeEntry_New.DateXname)).Value);
                InitTimeEntryControl(ste, date, nbterecord);


            }
        }

        #endregion

        #region Methods

        protected string GetDayOffCssCalss(XElement calendarItem)
        {
            return calendarItem.Attribute(XName.Get(TimeEntry_New.CssClassXname)).Value;
        }

        private void InitTimeEntryControl(SingleTimeEntry_New ste, DateTime date, XElement terXlement)
        {
            ste.DateBehind = date;
            ste.TimeEntryRecordElement = terXlement;
        }

        public void UpdateTimeEntries()
        {
            hdnworkTypeId.Value = WorkType.Id.ToString();
            lblTimeType.Text = lblTimeType.ToolTip = WorkType.Name;
            tes.DataSource = TeBarDataSource;
            tes.DataBind();
        }

        internal void UpdateNoteAndActualHours(List<XElement> calendarItemElements)
        {
            for (int k = 0; k < calendarItemElements.Count; k++)
            {

                var nonbillableSte = tes.Items[k].FindControl(steId) as SingleTimeEntry_New;

                var calendarItemElement = calendarItemElements[k];
                if (calendarItemElement.HasElements)
                {
                    var nonBillableElements = calendarItemElement.Descendants(XName.Get(TimeEntry_New.TimeEntryRecordXname)).ToList().Count > 0 ? calendarItemElement.Descendants(XName.Get(TimeEntry_New.TimeEntryRecordXname)).Where(ter => ter.Attribute(XName.Get(TimeEntry_New.IsChargeableXname)).Value.ToLowerInvariant() == "false").ToList() : null;
                    if (nonBillableElements != null && nonBillableElements.Count > 0)
                    {
                        var nonbillableElement = nonBillableElements != null && nonBillableElements.Count > 0 ? nonBillableElements.First() : null;
                        nonbillableSte.UpdateEditedValues(nonbillableElement);
                    }
                }
                else
                {
                    //Add Element
                    var nonBillableElement = new XElement(TimeEntry_New.TimeEntryRecordXname);
                    nonBillableElement.SetAttributeValue(XName.Get(TimeEntry_New.IsChargeableXname), "false");
                    nonbillableSte.UpdateEditedValues(nonBillableElement);

                    if (nonBillableElement.Attribute(XName.Get(TimeEntry_New.ActualHoursXname)).Value != "" || nonBillableElement.Attribute(XName.Get(TimeEntry_New.NoteXname)).Value != "")
                    {
                        calendarItemElement.Add(nonBillableElement);
                    }

                }
            }


        }

        internal void UpdateVerticalTotalCalculatorExtenderId(int index, string clientId)
        {
            var nonbillableSte = tes.Items[index].FindControl(steId) as SingleTimeEntry_New;
            nonbillableSte.UpdateVerticalTotalCalculatorExtenderId(clientId);
        }

        internal void AddAttributeToPTOTextBox(int index)
        {
            if (IsPTO)
            {
                var nonbillableSte = tes.Items[index].FindControl(steId) as SingleTimeEntry_New;
                nonbillableSte.AddAttributeToPTOTextBox(extTotalHours.ClientID);
            }
        }

        internal void ValidateAll()
        {
            foreach (RepeaterItem tesItem in tes.Items)
            {
                var nonbillableSte = tesItem.FindControl(steId) as SingleTimeEntry_New;
                nonbillableSte.IsPTO = IsPTO;
                nonbillableSte.ValidateNoteAndHours();
            }
        }

        internal void UpdateWorkType(XElement workTypeElement)
        {
            workTypeElement.Attribute(XName.Get(TimeEntry_New.IdXname)).Value = hdnworkTypeId.Value;
        }

        #endregion

    }
}

