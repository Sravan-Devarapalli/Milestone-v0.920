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
    public partial class NonBillableTimeEntryBar : System.Web.UI.UserControl
    {
        #region Constants

        private const string NonBillableTbAcutualHoursClientIds_NonBillableTimeEntryBar = "NonBillableTbAcutualHoursClientIds_NonBillableTimeEntryBar";
        private const string workTypeOldIdAttribute = "workTypeOldId";
        private const string previousIdAttribute = "previousId";
        private const string steId = "ste";

        #endregion

        #region Properties
        
        public List<XElement> TeBarDataSource { get; set; }

        public TimeTypeRecord[] TimeTypes { get; set; }

        public TimeTypeRecord SelectedTimeType { get; set; }

        public string TdCellSectionClientID { get { return tdPlusSection.ClientID; } }

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

        public Dictionary<int, string> NonBillableTbAcutualHoursClientIds
        {
            get
            {
                return ViewState[NonBillableTbAcutualHoursClientIds_NonBillableTimeEntryBar] as Dictionary<int, string>;
            }
            set
            {
                ViewState[NonBillableTbAcutualHoursClientIds_NonBillableTimeEntryBar] = value;
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
                var ste = e.Item.FindControl(steId) as SingleTimeEntry_New;

                var calendarItem = (XElement)e.Item.DataItem;

                var nonbillableTbId = ste.Controls[1].ClientID;
                extTotalHours.ControlsToCheck += nonbillableTbId + ";";
                extEnableDisable.ControlsToCheck += nonbillableTbId + ";";

                NonBillableTbAcutualHoursClientIds = NonBillableTbAcutualHoursClientIds ?? new Dictionary<int, string>();
                NonBillableTbAcutualHoursClientIds.Add(e.Item.ItemIndex, nonbillableTbId);
                NonBillableTbAcutualHoursClientIds = NonBillableTbAcutualHoursClientIds;

                ste.HorizontalTotalCalculatorExtenderId = extTotalHours.ClientID;
                ste.IsNoteRequired = calendarItem.Attribute(XName.Get(TimeEntry_New.IsNoteRequiredXname)).Value;
                ste.IsChargeCodeTurnOff = calendarItem.Attribute(XName.Get(TimeEntry_New.IsChargeCodeOffXname)).Value;

                var bterecord = (calendarItem.HasElements && calendarItem.Descendants(XName.Get(TimeEntry_New.TimeEntryRecordXname)).Where(ter => ter.Attribute(XName.Get("IsChargeable")).Value.ToLowerInvariant() == "false").ToList().Count > 0) ? calendarItem.Descendants(XName.Get(TimeEntry_New.TimeEntryRecordXname)).Where(ter => ter.Attribute(XName.Get(TimeEntry_New.IsChargeableXname)).Value.ToLowerInvariant() == "false").First() : null;
                var date = Convert.ToDateTime(calendarItem.Attribute(XName.Get(TimeEntry_New.DateXname)).Value);
                InitTimeEntryControl(ste, date, bterecord);
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

            var repItem = imgDropTes.NamingContainer.NamingContainer as RepeaterItem;
            var repItemIndex = repItem.ItemIndex;

            int projectId = Convert.ToInt32(imgDropTes.Attributes[TimeEntry_New.ProjectIdXname]);
            int accountId = Convert.ToInt32(imgDropTes.Attributes[TimeEntry_New.AccountIdXname]);
            int businessUnitId = Convert.ToInt32(imgDropTes.Attributes[TimeEntry_New.BusinessUnitIdXname]);
            int workTypeId = Convert.ToInt32(imgDropTes.Attributes[TimeEntry_New.WorkTypeXname]);
            int workTypeOldID;
            int.TryParse(imgDropTes.Attributes[workTypeOldIdAttribute], out workTypeOldID);
            int personId = HostingPage.SelectedPerson.Id.Value;
            DateTime[] dates = HostingPage.SelectedDates;

            //Remove Worktype from xml
            var project = ServiceCallers.Custom.Project(pro => pro.GetBusinessDevelopmentProject());
            bool isBusinessDevelopment = project.Id == projectId;
            HostingPage.RemoveWorktypeFromXMLForBusinessDevelopmentAndInternalSection(accountId, businessUnitId, projectId, repItemIndex, isBusinessDevelopment);


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

        private void InitTimeEntryControl(SingleTimeEntry_New ste, DateTime date, XElement terXlement)
        {
            ste.DateBehind = date;
            ste.TimeEntryRecordElement = terXlement;
        }


        public void UpdateTimeEntries()
        {
            ddlTimeTypes.Items.Clear();
            ddlTimeTypes.DataSource = TimeTypes;
            ddlTimeTypes.DataBind();

            if (SelectedTimeType != null && SelectedTimeType.Id > 0)
            {
                ddlTimeTypes.SelectedValue = SelectedTimeType.Id.ToString();
                if(ddlTimeTypes.SelectedIndex == 0)
                {
                    string timetypename = ServiceCallers.Custom.TimeType(te => te.GetWorkTypeNameById(SelectedTimeType.Id));
                    ddlTimeTypes.Items.Add(new ListItem(timetypename,SelectedTimeType.Id.ToString()));
                    ddlTimeTypes.SelectedValue = SelectedTimeType.Id.ToString();
                    ddlTimeTypes.Attributes[TimeEntry_New.selectedInActiveWorktypeid] = SelectedTimeType.Id.ToString();
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

                var nonBillableSte = tes.Items[k].FindControl(steId) as SingleTimeEntry_New;

                var calendarItemElement = calendarItemElements[k];

                if (calendarItemElement.HasElements)
                {
                    var nonbElements = calendarItemElement.Descendants(XName.Get(TimeEntry_New.TimeEntryRecordXname)).ToList().Count > 0 ? calendarItemElement.Descendants(XName.Get(TimeEntry_New.TimeEntryRecordXname)).Where(ter => ter.Attribute(XName.Get(TimeEntry_New.IsChargeableXname)).Value.ToLowerInvariant() == "false").ToList() : null;
                    if (nonbElements != null && nonbElements.Count > 0)
                    {
                        var nonBillableElement = nonbElements.First();
                        nonBillableSte.UpdateEditedValues(nonBillableElement);
                    }
                }
                else
                {
                    //Add Element
                    AddNonBillableElement(nonBillableSte, calendarItemElement);
                }
            }

        }

        private void AddNonBillableElement(SingleTimeEntry_New nonBillableSte, XElement calendarItemElement)
        {
            var nonBillableElement = new XElement(TimeEntry_New.TimeEntryRecordXname);
            nonBillableElement.SetAttributeValue(XName.Get(TimeEntry_New.IsChargeableXname), "false");
            nonBillableElement.SetAttributeValue(XName.Get(TimeEntry_New.IsReviewedXname), "Pending");
            nonBillableSte.UpdateEditedValues(nonBillableElement);

            if (nonBillableElement.Attribute(XName.Get(TimeEntry_New.ActualHoursXname)).Value != "" || (!HostingPage.IsSaving && nonBillableElement.Attribute(XName.Get(TimeEntry_New.NoteXname)).Value != ""))
            {
                calendarItemElement.Add(nonBillableElement);
            }
        }

        internal void UpdateWorkType(XElement workTypeElement,XElement accountAndProjectSelectionElement)
        {
            workTypeElement.Attribute(XName.Get(TimeEntry_New.IdXname)).Value = ddlTimeTypes.SelectedValue;
            string OldId = workTypeElement.Attribute(XName.Get(TimeEntry_New.OldIdXname)) != null ? workTypeElement.Attribute(XName.Get(TimeEntry_New.OldIdXname)).Value : null;
            if (!String.IsNullOrEmpty(OldId) && ddlTimeTypes.SelectedValue != OldId)
            {
                //need to update the ischargecodeturnoff in the xml for the calenderitems
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
            var nonbillableSte = tes.Items[index].FindControl(steId) as SingleTimeEntry_New;
            nonbillableSte.UpdateVerticalTotalCalculatorExtenderId(clientId);
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
                var nonbillableSte = tesItem.FindControl(steId) as SingleTimeEntry_New;

                if (!isThereAtleastOneTimeEntryrecord)
                {
                    isThereAtleastOneTimeEntryrecord = nonbillableSte.IsThereAtleastOneTimeEntryrecord;
                }

                nonbillableSte.ValidateNoteAndHours();

            }

            if (isThereAtleastOneTimeEntryrecord && !ValideWorkTypeDropDown())
            {
                HostingPage.IsValidWorkType = false;
            }
        }

        #endregion

    }
}

