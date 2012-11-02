using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using DataTransferObjects.TimeEntry;
using System.Xml.Linq;
using PraticeManagement.Controls.Generic.EnableDisableExtForAdminSection;
using System.Web.UI.HtmlControls;
using System.Web.Script.Serialization;

namespace PraticeManagement.Controls.TimeEntry
{
    public partial class AdministrativeTimeEntryBar : System.Web.UI.UserControl
    {
        #region Constants

        private const string AdministrativeTimeEntryBar_TbAcutualHoursClientIds = "AdministrativeTimeEntryBar_TbAcutualHoursClientIds";
        private const string steId = "ste";
        private const string workTypeOldIdAttribute = "workTypeOldId";
        private const string previousIdAttribute = "previousId";

        #endregion

        #region Properties

        public string TdCellSectionClientID { get { return tdPlusSection.ClientID; } }

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

        public bool IsPTO
        {
            get
            {
                return ViewState["ISPTO_KEY"] != null ? (bool)ViewState["ISPTO_KEY"] : false;
            }
            set
            {
                ViewState["ISPTO_KEY"] = value;
            }
        }

        public bool IsSickLeave
        {
            get
            {
                return ViewState["IsSickLeave_KEY"] != null ? (bool)ViewState["IsSickLeave_KEY"] : false;
            }
            set
            {
                ViewState["IsSickLeave_KEY"] = value;
            }
        }

        public bool IsHoliday
        {
            get
            {
                return ViewState["ISHOLIDAY_KEY"] != null ? (bool)ViewState["ISHOLIDAY_KEY"] : false;
            }
            set
            {
                ViewState["ISHOLIDAY_KEY"] = value;
            }
        }

        public bool IsORT
        {
            get
            {
                return ViewState["IsORT_KEY"] != null ? (bool)ViewState["IsORT_KEY"] : false;
            }
            set
            {
                ViewState["IsORT_KEY"] = value;
            }
        }

        public bool IsUnpaid
        {
            get
            {
                return ViewState["IsUnpaid_KEY"] != null ? (bool)ViewState["IsUnpaid_KEY"] : false;
            }
            set
            {
                ViewState["IsUnpaid_KEY"] = value;
            }
        }


        public TimeTypeRecord[] TimeTypes { get; set; }

        public TimeTypeRecord SelectedTimeType { get; set; }

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

        public List<string> TblApprovedByClientIds
        {
            get;
            set;
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

            AddAttributesToTimeTypesDropdown(ddlTimeTypes, TimeTypes);
            if (!(IsPTO || IsHoliday || IsUnpaid || IsSickLeave))
            {
                if (!string.IsNullOrEmpty(imgDropTes.Attributes[TimeEntry_New.workTypeOldId]))
                {
                    imgDropTes.Visible =
                    ddlTimeTypes.Enabled = !HostingPage.IsReadOnly;
                }
            }
        }

        protected void repEntries_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                TblApprovedByClientIds = new List<string>();
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ste = e.Item.FindControl(steId) as AdministrativeSingleTimeEntry;

                var calendarItem = (XElement)e.Item.DataItem;

                ste.IsHolidayDate = calendarItem.Attribute(XName.Get(TimeEntry_New.CssClassXname)).Value == "DayOff";

                var textBoxId = ste.Controls[1].ClientID;

                ste.DataBindApprovedManagers();

                TbAcutualHoursClientIds = TbAcutualHoursClientIds ?? new Dictionary<int, string>();

                TbAcutualHoursClientIds.Add(e.Item.ItemIndex, textBoxId);
                TbAcutualHoursClientIds = TbAcutualHoursClientIds;
                extTotalHours.ControlsToCheck += textBoxId + ";";

                ste.HorizontalTotalCalculatorExtenderId = extTotalHours.ClientID;
                ste.IsNoteRequired = calendarItem.Attribute(XName.Get(TimeEntry_New.IsNoteRequiredXname)).Value;
                ste.IsChargeCodeTurnOff = calendarItem.Attribute(XName.Get(TimeEntry_New.IsChargeCodeOffXname)).Value;

                if (IsHoliday || IsUnpaid || ste.IsHolidayDate)
                {
                    if (!IsHoliday && !IsUnpaid && ste.IsHolidayDate)
                    {
                        ste.DisabledReadonly = true;
                    }
                    else
                    {
                        ste.Disabled = true;
                    }
                }
                else
                {
                    extEnableDisable.ControlsToCheck += textBoxId + ";";
                }

                ste.IsPTO = IsPTO;
                ste.IsHoliday = IsHoliday;
                ste.IsORT = IsORT;
                ste.IsUnpaid = IsUnpaid;
                ste.IsSickLeave = IsSickLeave;
                ste.TimeTypeRecord = HostingPage.AllAdministrativeTimeTypes.FirstOrDefault(t => t.Id == SelectedTimeType.Id);


                DateTime date = Convert.ToDateTime(calendarItem.Attribute(XName.Get(TimeEntry_New.DateXname)).Value);
                if (!HostingPage.AdminExtenderHoursControls.ContainsKey(date))
                {
                    HostingPage.AdminExtenderHoursControls.Add(date, "");
                }
                if (!HostingPage.AdminExtenderNotesControls.ContainsKey(date))
                {
                    HostingPage.AdminExtenderNotesControls.Add(date, "");
                }
                if (!HostingPage.AdminExtenderApprovedManagersControls.ContainsKey(date))
                {
                    HostingPage.AdminExtenderApprovedManagersControls.Add(date, "");
                }
                if (!HostingPage.AdminExtenderHiddenNotesControls.ContainsKey(date))
                {
                    HostingPage.AdminExtenderHiddenNotesControls.Add(date, "");
                }
                if (!HostingPage.AdminExtenderHiddenManagersControls.ContainsKey(date))
                {
                    HostingPage.AdminExtenderHiddenManagersControls.Add(date, "");
                }
                if (!HostingPage.AdminExtenderDeleteControls.ContainsKey(date))
                {
                    HostingPage.AdminExtenderDeleteControls.Add(date, "");
                }

                if (!HostingPage.AdminExtenderCloseControls.ContainsKey(date))
                {
                    HostingPage.AdminExtenderCloseControls.Add(date, "");
                }

                var tbNotesId = (ste.FindControl("tbNotes") as TextBox).ClientID;
                var hdNotesId = (ste.FindControl("hdnNotes") as HiddenField).ClientID;
                var ddlManagerId = (ste.FindControl("ddlApprovedManagers") as DropDownList).ClientID;
                var hdnManagerId = (ste.FindControl("hdnApprovedManagerId") as HiddenField).ClientID;
                var imgClear = (ste.FindControl("imgClear") as HtmlGenericControl).ClientID;
                var cpp = (ste.FindControl("cpp") as LinkButton).ClientID;
                var btnSaveNotes = (ste.FindControl("btnSaveNotes") as Button).ClientID;

                HostingPage.AdminExtenderHoursControls[date] = textBoxId + ";" + HostingPage.AdminExtenderHoursControls[date];
                HostingPage.AdminExtenderNotesControls[date] = tbNotesId + ";" + HostingPage.AdminExtenderNotesControls[date];
                HostingPage.AdminExtenderApprovedManagersControls[date] = ddlManagerId + ";" + HostingPage.AdminExtenderApprovedManagersControls[date];
                HostingPage.AdminExtenderHiddenNotesControls[date] = hdNotesId + ";" + HostingPage.AdminExtenderHiddenNotesControls[date];
                HostingPage.AdminExtenderHiddenManagersControls[date] = hdnManagerId + ";" + HostingPage.AdminExtenderHiddenManagersControls[date];
                HostingPage.AdminExtenderDeleteControls[date] = imgClear + ";" + HostingPage.AdminExtenderDeleteControls[date];
                HostingPage.AdminExtenderCloseControls[date] = cpp + ";" + HostingPage.AdminExtenderCloseControls[date];

                ste.ParentCalendarItem = calendarItem;

                var nbterecord = (calendarItem.HasElements && calendarItem.Descendants(XName.Get(TimeEntry_New.TimeEntryRecordXname)).Where(ter => ter.Attribute(XName.Get("IsChargeable")).Value.ToLowerInvariant() == "false").ToList().Count > 0) ? calendarItem.Descendants(XName.Get("TimeEntryRecord")).Where(ter => ter.Attribute(XName.Get("IsChargeable")).Value.ToLowerInvariant() == "false").First() : null;
                InitTimeEntryControl(ste, date, nbterecord);

                TblApprovedByClientIds.Add(ste.ApprovedByClientId);

            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                ddlTimeTypes.Attributes["JsonApprovedByClientIds"] = serializer.Serialize(TblApprovedByClientIds);
            }
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
            int personId = HostingPage.SelectedPerson.Id.Value;
            DateTime[] dates = HostingPage.SelectedDates;
            int workTypeOldID;
            int.TryParse(imgDropTes.Attributes[workTypeOldIdAttribute], out workTypeOldID);

            //Remove Work type from xml
            HostingPage.RemoveWorktypeFromXMLForAdminstrativeSection(repItemIndex);

            //Delete TimeEntry from database
            if (workTypeOldID > 0)
            {
                ServiceCallers.Custom.TimeEntry(te => te.DeleteTimeEntry(accountId, projectId, personId, workTypeOldID, dates[0], dates[dates.Length - 1], Context.User.Identity.Name));
                var calendarItems = ServiceCallers.Custom.Calendar(c => c.GetPersonCalendar(dates[0], dates[dates.Length - 1], personId, null));

                HostingPage.UpdateCalendarItemAndBind(calendarItems);
            }

        }

        protected void ddlTimeTypes_DataBound(object sender, EventArgs e)
        {
            if (ddlTimeTypes.Items.FindByValue("-1") == null)
                ddlTimeTypes.Items.Insert(0, (new ListItem("- - Select Work Type - -", "-1")));
        }

        #endregion

        #region Methods

        protected string GetDayOffCssClass(XElement calendarItem)
        {
            return calendarItem.Attribute(XName.Get(TimeEntry_New.CssClassXname)).Value;
        }

        private void InitTimeEntryControl(AdministrativeSingleTimeEntry ste, DateTime date, XElement terXlement)
        {
            ste.DateBehind = date;
            ste.TimeEntryRecordElement = terXlement;
        }

        public void UpdateTimeEntries()
        {

            if (IsPTO || IsHoliday || IsUnpaid || IsSickLeave)
            {
                ddlTimeTypes.Visible = false;
                lblTimeType.Visible = true;
                imgDropTes.Visible = false;
            }
            else
            {
                lblTimeType.Visible = false;
                ddlTimeTypes.Items.Clear();
                ddlTimeTypes.DataSource = TimeTypes;
                ddlTimeTypes.DataBind();

                if (SelectedTimeType != null && SelectedTimeType.Id > 0)
                {
                    ListItem selectedTimeType = null;

                    selectedTimeType = ddlTimeTypes.Items.FindByValue(SelectedTimeType.Id.ToString());

                    if (selectedTimeType == null)
                    {
                        var timetype = ServiceCallers.Custom.TimeType(te => te.GetWorkTypeById(SelectedTimeType.Id));
                        selectedTimeType = new ListItem(timetype.Name, SelectedTimeType.Id.ToString());
                        selectedTimeType.Attributes.Add(TimeEntry_New.IsORTXname, timetype.IsORTTimeType.ToString());
                        selectedTimeType.Attributes.Add(TimeEntry_New.IsW2HourlyTimeTypeAttribute, (timetype.IsW2HourlyAllowed ? 1 : 0).ToString());
                        selectedTimeType.Attributes.Add(TimeEntry_New.IsW2SalaryTimeTypeAttribute, (timetype.IsW2SalaryAllowed ? 1 : 0).ToString());
                        ddlTimeTypes.Items.Add(selectedTimeType);
                        ddlTimeTypes.Attributes[TimeEntry_New.selectedInActiveWorktypeid] = SelectedTimeType.Id.ToString();
                        ddlTimeTypes.Attributes[TimeEntry_New.selectedInActiveWorktypeName] = timetype.Name;
                        ddlTimeTypes.Attributes[TimeEntry_New.selectedInActiveWorktypeIsORT] = timetype.IsORTTimeType.ToString();
                        ddlTimeTypes.Attributes[TimeEntry_New.selectedInActiveWorktypeIsW2Salary] = (timetype.IsW2SalaryAllowed ? 1 : 0).ToString();
                        ddlTimeTypes.Attributes[TimeEntry_New.selectedInActiveWorktypeIsW2Hourly] = (timetype.IsW2HourlyAllowed ? 1 : 0).ToString();

                    }

                    ddlTimeTypes.SelectedValue = selectedTimeType.Value;
                }
                else
                {
                    ddlTimeTypes.SelectedIndex = 0;
                }
                ddlTimeTypes.Attributes[previousIdAttribute] = ddlTimeTypes.SelectedValue.ToString();
                HostingPage.DdlWorkTypeIdsList += ddlTimeTypes.ClientID + ";";

            }

            hdnworkTypeId.Value = SelectedTimeType.Id.ToString();
            lblTimeType.Text = lblTimeType.ToolTip = "&nbsp;" + SelectedTimeType.Name;
            tes.DataSource = TeBarDataSource;
            tes.DataBind();
        }

        internal void RoundActualHours(List<XElement> calendarItemElements)
        {
            for (int k = 0; k < calendarItemElements.Count; k++)
            {
                var nonbillableSte = tes.Items[k].FindControl(steId) as AdministrativeSingleTimeEntry;
                var calendarItemElement = calendarItemElements[k];
                if (calendarItemElement.HasElements)
                {
                    var nonBillableElements = calendarItemElement.Descendants(XName.Get(TimeEntry_New.TimeEntryRecordXname)).ToList().Count > 0 ? calendarItemElement.Descendants(XName.Get(TimeEntry_New.TimeEntryRecordXname)).Where(ter => ter.Attribute(XName.Get(TimeEntry_New.IsChargeableXname)).Value.ToLowerInvariant() == "false").ToList() : null;
                    if (nonBillableElements != null && nonBillableElements.Count > 0)
                    {
                        var nonbillableElement = nonBillableElements != null && nonBillableElements.Count > 0 ? nonBillableElements.First() : null;
                        nonbillableSte.RoundActualHours(nonbillableElement);
                    }
                }
            }
        }

        internal void UpdateNoteAndActualHours(List<XElement> calendarItemElements)
        {
            for (int k = 0; k < calendarItemElements.Count; k++)
            {

                var nonbillableSte = tes.Items[k].FindControl(steId) as AdministrativeSingleTimeEntry;

                var calendarItemElement = calendarItemElements[k];
                if (calendarItemElement.HasElements)
                {
                    var nonBillableElements = calendarItemElement.Descendants(XName.Get(TimeEntry_New.TimeEntryRecordXname)).ToList().Count > 0 ? calendarItemElement.Descendants(XName.Get(TimeEntry_New.TimeEntryRecordXname)).Where(ter => ter.Attribute(XName.Get(TimeEntry_New.IsChargeableXname)).Value.ToLowerInvariant() == "false").ToList() : null;
                    if (nonBillableElements != null && nonBillableElements.Count > 0)
                    {
                        var nonbillableElement = nonBillableElements != null && nonBillableElements.Count > 0 ? nonBillableElements.First() : null;
                        nonbillableSte.UpdateEditedValues(nonbillableElement, ddlTimeTypes.SelectedItem.Attributes[TimeEntry_New.IsORTXname] == true.ToString());
                    }
                }
                else
                {
                    //Add Element
                    var nonBillableElement = new XElement(TimeEntry_New.TimeEntryRecordXname);
                    nonBillableElement.SetAttributeValue(XName.Get(TimeEntry_New.IsChargeableXname), "false");
                    nonBillableElement.SetAttributeValue(XName.Get(TimeEntry_New.IsReviewedXname), "Pending");
                    nonbillableSte.UpdateEditedValues(nonBillableElement, ddlTimeTypes.SelectedItem.Attributes[TimeEntry_New.IsORTXname] == true.ToString());

                    if (nonBillableElement.Attribute(XName.Get(TimeEntry_New.ActualHoursXname)).Value != "" || nonBillableElement.Attribute(XName.Get(TimeEntry_New.NoteXname)).Value != "")
                    {
                        calendarItemElement.Add(nonBillableElement);
                    }

                }
            }


        }

        internal void UpdateVerticalTotalCalculatorExtenderId(int index, string clientId)
        {
            var nonbillableSte = tes.Items[index].FindControl(steId) as AdministrativeSingleTimeEntry;
            nonbillableSte.UpdateVerticalTotalCalculatorExtenderId(clientId);
        }

        internal void AddAttributeToPTOTextBox(int index)
        {
            if (IsPTO)
            {
                var nonbillableSte = tes.Items[index].FindControl(steId) as AdministrativeSingleTimeEntry;
                nonbillableSte.AddAttributeToPTOTextBox(extTotalHours.ClientID);
            }
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
                var nonbillableASte = tesItem.FindControl(steId) as AdministrativeSingleTimeEntry;

                if (!isThereAtleastOneTimeEntryrecord)
                {
                    isThereAtleastOneTimeEntryrecord = nonbillableASte.IsThereAtleastOneTimeEntryrecord;
                }

                nonbillableASte.ValidateNoteAndHours(ddlTimeTypes.SelectedItem.Attributes[TimeEntry_New.IsORTXname] == true.ToString());
            }

            if (isThereAtleastOneTimeEntryrecord && !IsPTO && !IsHoliday && !IsUnpaid && !ValideWorkTypeDropDown() && !IsSickLeave)
            {
                HostingPage.IsValidWorkType = false;
            }
        }

        internal void UpdateAccountAndProjectWorkType(XElement accountAndProjectSelectionElement, XElement workTypeElement)
        {
            var workTypeId = (IsPTO || IsHoliday || IsUnpaid || IsSickLeave) ? Convert.ToInt32(hdnworkTypeId.Value) : Convert.ToInt32(ddlTimeTypes.SelectedValue); ;
            workTypeElement.Attribute(XName.Get(TimeEntry_New.IdXname)).Value = workTypeId.ToString();
            accountAndProjectSelectionElement.Attribute(XName.Get(TimeEntry_New.IsORTXname)).Value = ddlTimeTypes.SelectedItem.Attributes[TimeEntry_New.IsORTXname];

            if (workTypeId > 0 && !(IsPTO || IsHoliday || IsUnpaid || IsSickLeave))
            {
                Triple<int, int, int> result = ServiceCallers.Custom.TimeType(tt => tt.GetAdministrativeChargeCodeValues(workTypeId));
                accountAndProjectSelectionElement.Attribute(XName.Get(TimeEntry_New.AccountIdXname)).Value = result.First.ToString();
                accountAndProjectSelectionElement.Attribute(XName.Get(TimeEntry_New.ProjectIdXname)).Value = result.Second.ToString();
                accountAndProjectSelectionElement.Attribute(XName.Get(TimeEntry_New.BusinessUnitIdXname)).Value = result.Third.ToString();
            }

        }

        private void AddAttributesToTimeTypesDropdown(CustomDropDown ddlTimeTypes, TimeTypeRecord[] data)
        {
            if (data != null)
            {
                foreach (ListItem item in ddlTimeTypes.Items)
                {
                    if (!string.IsNullOrEmpty(item.Value) && Convert.ToInt32(item.Value) >= 0)
                    {
                        var id = Convert.ToInt32(item.Value);
                        if (data.Any(tt => tt.Id == id))
                        {
                            var obj = data.Where(tt => tt.Id == id).First();
                            if (obj != null)
                            {
                                item.Attributes.Add(TimeEntry_New.IsORTXname, obj.IsORTTimeType.ToString());
                                item.Attributes.Add(TimeEntry_New.IsW2HourlyTimeTypeAttribute, (obj.IsW2HourlyAllowed ? 1 : 0).ToString());
                                item.Attributes.Add(TimeEntry_New.IsW2SalaryTimeTypeAttribute, (obj.IsW2SalaryAllowed ? 1 : 0).ToString());
                            }
                        }
                    }
                    else
                    {
                        item.Attributes.Add(TimeEntry_New.IsORTXname, false.ToString());
                        item.Attributes.Add(TimeEntry_New.IsW2HourlyTimeTypeAttribute, 0.ToString());
                        item.Attributes.Add(TimeEntry_New.IsW2SalaryTimeTypeAttribute, 0.ToString());
                    }
                }
            }
        }

        #endregion

    }
}

