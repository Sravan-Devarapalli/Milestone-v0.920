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

                ste.HorizontalTotalCalculatorExtenderId = extTotalHours.ClientID;
                ste.IsNoteRequired = calendarItem.Attribute(XName.Get(TimeEntry_New.IsNoteRequiredXname)).Value;
                ste.IsChargeCodeTurnOff = calendarItem.Attribute(XName.Get(TimeEntry_New.IsChargeCodeOffXname)).Value;
               
                if (IsHoliday)
                {
                    ste.Disabled = true;
                }
                else
                {
                    extEnableDisable.ControlsToCheck += textBoxId + ";";
                }

                ste.IsPTO = IsPTO;
                ste.IsHoliday = IsHoliday;
                ste.IsAdminstrativeTimeType = true;
                DateTime date = Convert.ToDateTime(calendarItem.Attribute(XName.Get(TimeEntry_New.DateXname)).Value);
                if (!HostingPage.AdminExtenderHoursControls.ContainsKey(date))
                {
                    HostingPage.AdminExtenderHoursControls.Add(date, "");
                }
                if (!HostingPage.AdminExtenderNotesControls.ContainsKey(date))
                {
                    HostingPage.AdminExtenderNotesControls.Add(date, "");
                }
                if (!HostingPage.AdminExtenderHiddenNotesControls.ContainsKey(date))
                {
                    HostingPage.AdminExtenderHiddenNotesControls.Add(date, "");
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
                var imgClear = (ste.FindControl("imgClear") as HtmlGenericControl).ClientID;
                var cpp = (ste.FindControl("cpp") as LinkButton).ClientID;
                var btnSaveNotes = (ste.FindControl("btnSaveNotes") as Button).ClientID;
                
                HostingPage.AdminExtenderHoursControls[date] = textBoxId + ";" + HostingPage.AdminExtenderHoursControls[date];
                HostingPage.AdminExtenderNotesControls[date] = tbNotesId + ";" + HostingPage.AdminExtenderNotesControls[date];
                HostingPage.AdminExtenderHiddenNotesControls[date] = hdNotesId + ";" + HostingPage.AdminExtenderHiddenNotesControls[date];
                HostingPage.AdminExtenderDeleteControls[date] = imgClear + ";" + HostingPage.AdminExtenderDeleteControls[date];
                HostingPage.AdminExtenderCloseControls[date] = cpp + ";" + HostingPage.AdminExtenderCloseControls[date];
                
                ste.ParentCalendarItem = calendarItem;

                var nbterecord = (calendarItem.HasElements && calendarItem.Descendants(XName.Get(TimeEntry_New.TimeEntryRecordXname)).Where(ter => ter.Attribute(XName.Get("IsChargeable")).Value.ToLowerInvariant() == "false").ToList().Count > 0) ? calendarItem.Descendants(XName.Get("TimeEntryRecord")).Where(ter => ter.Attribute(XName.Get("IsChargeable")).Value.ToLowerInvariant() == "false").First() : null;
                InitTimeEntryControl(ste, date, nbterecord);

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

            //Remove Worktype from xml
            HostingPage.RemoveWorktypeFromXMLForAdminstrativeSection(repItemIndex);

            //Delete TimeEntry from database
            if (workTypeOldID > 0)
            {
                ServiceCallers.Custom.TimeEntry(te => te.DeleteTimeEntry(accountId, projectId, personId, workTypeOldID, dates[0], dates[dates.Length - 1], Context.User.Identity.Name));
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

        private void InitTimeEntryControl(SingleTimeEntry_New ste, DateTime date, XElement terXlement)
        {
            ste.DateBehind = date;
            ste.TimeEntryRecordElement = terXlement;
        }

        public void UpdateTimeEntries()
        {
            if (IsPTO || IsHoliday)
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
                        var timetypename = ServiceCallers.Custom.TimeType(te => te.GetWorkTypeNameById(SelectedTimeType.Id)); ;
                        selectedTimeType = new ListItem(timetypename, SelectedTimeType.Id.ToString());
                        ddlTimeTypes.Items.Add(selectedTimeType);
                        ddlTimeTypes.Attributes[TimeEntry_New.selectedInActiveWorktypeid] = SelectedTimeType.Id.ToString();
                        ddlTimeTypes.Attributes[TimeEntry_New.selectedInActiveWorktypeName] = timetypename;
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
            lblTimeType.Text = lblTimeType.ToolTip = SelectedTimeType.Name;
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
                    nonBillableElement.SetAttributeValue(XName.Get(TimeEntry_New.IsReviewedXname), "Pending");
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

            if (isThereAtleastOneTimeEntryrecord && !IsPTO && !IsHoliday && !ValideWorkTypeDropDown())
            {
                HostingPage.IsValidWorkType = false;
            }
        }

        internal void UpdateAccountAndProjectWorkType(XElement accountAndProjectSelectionElement, XElement workTypeElement)
        {
            var workTypeId = (IsPTO || IsHoliday) ? Convert.ToInt32(hdnworkTypeId.Value) : Convert.ToInt32(ddlTimeTypes.SelectedValue); ;
            workTypeElement.Attribute(XName.Get(TimeEntry_New.IdXname)).Value = workTypeId.ToString();

            if (workTypeId > 0 && !(IsPTO || IsHoliday))
            {
                Triple<int,int,int> result = ServiceCallers.Custom.TimeType(tt => tt.GetAdministrativeChargeCodeValues(workTypeId));
                accountAndProjectSelectionElement.Attribute(XName.Get(TimeEntry_New.AccountIdXname)).Value = result.First.ToString();
                accountAndProjectSelectionElement.Attribute(XName.Get(TimeEntry_New.ProjectIdXname)).Value = result.Second.ToString();
                accountAndProjectSelectionElement.Attribute(XName.Get(TimeEntry_New.BusinessUnitIdXname)).Value = result.Third.ToString();
            }

        }

        #endregion

    }
}

