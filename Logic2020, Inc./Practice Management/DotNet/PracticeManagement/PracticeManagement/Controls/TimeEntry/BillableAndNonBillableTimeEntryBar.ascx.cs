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

        private const string TeBarDataSourceViewstate = "6sdr8832C-A37A-497F-82A9-58As40C759499";
        private const string AccountIdXname = "AccountId";
        private const string ProjectIdXname = "ProjectId";
        private const string WorkTypeXname = "WorkType";
        private const string workTypeOldId = "workTypeOldId";

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


        public TimeTypeRecord[] WorkTypes { get; set; }

        public TimeTypeRecord SelectedWorkType { get; set; }

        public Dictionary<int, string> BillableTbAcutualHoursClientIds
        {
            get
            {
                return ViewState["BillableAndNonBillableTimeEntryBar_BillableTbAcutualHoursClientIds"] as Dictionary<int, string>;
            }
            set
            {
                ViewState["BillableAndNonBillableTimeEntryBar_BillableTbAcutualHoursClientIds"] = value;
            }
        }

        public Dictionary<int, string> NonBillableTbAcutualHoursClientIds
        {
            get
            {
                return ViewState["BillableAndNonBillableTimeEntryBar_NonBillableTbAcutualHoursClientIds"] as Dictionary<int, string>;
            }
            set
            {
                ViewState["BillableAndNonBillableTimeEntryBar_NonBillableTbAcutualHoursClientIds"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected bool steItem_OnReadyToUpdateTE(object sender, ReadyToUpdateTeArguments args)
        {
            return false;
        }

        protected string GetDayOffCssCalss(XElement calendarItem)
        {
            return calendarItem.Attribute(XName.Get("CssClass")).Value;
        }

        private void InitTimeEntryControl(BillableAndNonBillableSingleTimeEntry ste, DateTime date, XElement bterXlement, XElement nonbterXlement)
        {
            ste.DateBehind = date;
            ste.TimeEntryRecordBillableElement = bterXlement;
            ste.TimeEntryRecordNonBillableElement = nonbterXlement;
            //var selMpe = Array.FindAll(HostingPage.MilestonePersonEntries, mpe => mpe.MilestonePersonId == cell.MilestoneBehind.MilestonePersonId);

            //DisableInvalidDatesAndChargeability(ste, dateBehind, selMpe);
        }

        protected void repEntries_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var billableAndNonBillableSte = e.Item.FindControl("ste") as BillableAndNonBillableSingleTimeEntry;

                var calendarItem = (XElement)e.Item.DataItem;

                var billableTbId = billableAndNonBillableSte.BillableTextBoxClientId;
                var nonBillableTbId = billableAndNonBillableSte.NonBillableTextBoxClientId;
                var tbNotes = billableAndNonBillableSte.FindControl("tbNotes") as TextBox;

                extTotalHours.ControlsToCheck += billableTbId + ";" + nonBillableTbId + ";";
                extEnableDisable.ControlsToCheck += billableTbId + ";" + nonBillableTbId + ";";

                billableAndNonBillableSte.HorizontalTotalCalculatorExtenderId = extTotalHours.ClientID;

                billableAndNonBillableSte.IsNoteRequired = calendarItem.Attribute(XName.Get("IsNoteRequired")).Value;
                billableAndNonBillableSte.IsHourlyRevenue = calendarItem.Attribute(XName.Get("IsHourlyRevenue")).Value;

                BillableTbAcutualHoursClientIds = BillableTbAcutualHoursClientIds ?? new Dictionary<int, string>();
                BillableTbAcutualHoursClientIds.Add(e.Item.ItemIndex, billableTbId);
                BillableTbAcutualHoursClientIds = BillableTbAcutualHoursClientIds;

                NonBillableTbAcutualHoursClientIds = NonBillableTbAcutualHoursClientIds ?? new Dictionary<int, string>();
                NonBillableTbAcutualHoursClientIds.Add(e.Item.ItemIndex, nonBillableTbId);
                NonBillableTbAcutualHoursClientIds = NonBillableTbAcutualHoursClientIds;

                var bterecord = (calendarItem.HasElements && calendarItem.Descendants(XName.Get("TimeEntryRecord")).Where(ter => ter.Attribute(XName.Get("IsChargeable")).Value.ToLowerInvariant() == "true").ToList().Count > 0) ? calendarItem.Descendants(XName.Get("TimeEntryRecord")).Where(ter => ter.Attribute(XName.Get("IsChargeable")).Value.ToLowerInvariant() == "true").First() : null;
                var nbterecord = (calendarItem.HasElements && calendarItem.Descendants(XName.Get("TimeEntryRecord")).Where(ter => ter.Attribute(XName.Get("IsChargeable")).Value.ToLowerInvariant() == "false").ToList().Count > 0) ? calendarItem.Descendants(XName.Get("TimeEntryRecord")).Where(ter => ter.Attribute(XName.Get("IsChargeable")).Value.ToLowerInvariant() == "false").First() : null;
                var date = Convert.ToDateTime(calendarItem.Attribute(XName.Get("Date")).Value);

                InitTimeEntryControl(billableAndNonBillableSte, date, bterecord, nbterecord);

            }

        }

        protected void ddlTimeTypes_DataBound(object sender, EventArgs e)
        {
            if (ddlTimeTypes.Items.FindByValue("-1") == null)
                ddlTimeTypes.Items.Insert(0, (new ListItem("1. Select Work Type", "-1")));
            AddTitlestoListItems(ddlTimeTypes);
        }

        public void AddTitlestoListItems(DropDownList dropDownList)
        {
            if (dropDownList == null)
            {
                return;
            }

            foreach (ListItem item in dropDownList.Items)
            {
                item.Attributes.Add("title", item.Text);
            }
        }

        protected void imgDropTes_Click(object sender, ImageClickEventArgs e)
        {

            var imgDropTes = ((ImageButton)(sender));

            var repProjectTesItem = imgDropTes.NamingContainer.NamingContainer as RepeaterItem;
            var ProjectTesItemIndex = repProjectTesItem.ItemIndex;

            int projectId = Convert.ToInt32(imgDropTes.Attributes[ProjectIdXname]);
            int accountId = Convert.ToInt32(imgDropTes.Attributes[AccountIdXname]);
            int workTypeOldID;
            int.TryParse(imgDropTes.Attributes[workTypeOldId], out workTypeOldID);
            int personId = HostingPage.SelectedPerson.Id.Value;
            DateTime[] dates = HostingPage.SelectedDates;
            //remove from xml

            HostingPage.RemoveWorktypeFromXMLForProjectSection(accountId, projectId, ProjectTesItemIndex);

            //remove from database
            if (workTypeOldID > 0)
            {
                //old one
                ServiceCallers.Custom.TimeEntry(te => te.DeleteTimeEntry(accountId, projectId, personId, workTypeOldID, dates[0], dates[dates.Length - 1], Context.User.Identity.Name));
            }
        }

        public void UpdateTimeEntries()
        {

            ddlTimeTypes.Items.Clear();
            ddlTimeTypes.DataSource = WorkTypes;
            ddlTimeTypes.DataBind();

            if (SelectedWorkType != null && SelectedWorkType.Id > 0)
            {
                ddlTimeTypes.SelectedValue = SelectedWorkType.Id.ToString();
            }
            else
            {
                ddlTimeTypes.SelectedIndex = 0;
            }

            HostingPage.DdlWorkTypeIdsList += ddlTimeTypes.ClientID + ";";



            tes.DataSource = TeBarDataSource;
            tes.DataBind();
        }

        internal void UpdateNoteAndActualHours(List<XElement> calendarItemElements)
        {
            for (int k = 0; k < calendarItemElements.Count; k++)
            {

                var billableAndNonbillableSte = tes.Items[k].FindControl("ste") as BillableAndNonBillableSingleTimeEntry;

                var calendarItemElement = calendarItemElements[k];
                if (calendarItemElement.HasElements)
                {
                    var bElements = calendarItemElement.Descendants(XName.Get("TimeEntryRecord")).ToList().Count > 0 ? calendarItemElement.Descendants(XName.Get("TimeEntryRecord")).Where(ter => ter.Attribute(XName.Get("IsChargeable")).Value.ToLowerInvariant() == "true").ToList() : null;
                    var nonBillableElements = calendarItemElement.Descendants(XName.Get("TimeEntryRecord")).ToList().Count > 0 ? calendarItemElement.Descendants(XName.Get("TimeEntryRecord")).Where(ter => ter.Attribute(XName.Get("IsChargeable")).Value.ToLowerInvariant() == "false").ToList() : null;

                    if (bElements != null && bElements.Count > 0)
                    {
                        var billableElement = bElements.First();
                        billableAndNonbillableSte.UpdateBillableElementEditedValues(billableElement);

                        Decimal i = 1;
                        Decimal.TryParse(billableElement.Attribute(XName.Get("ActualHours")).Value, out i);

                        bool isHoursZero = (i == 0);
                        if ((billableElement.Attribute(XName.Get("ActualHours")).Value == string.Empty && billableElement.Attribute(XName.Get("Note")).Value == "" && !HostingPage.IsSaving) || (billableElement.Attribute(XName.Get("ActualHours")).Value == string.Empty && HostingPage.IsSaving))
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

                        if ((nonBillableElement.Attribute(XName.Get("ActualHours")).Value == string.Empty && nonBillableElement.Attribute(XName.Get("Note")).Value == "" && !HostingPage.IsSaving) || (nonBillableElement.Attribute(XName.Get("ActualHours")).Value == string.Empty && HostingPage.IsSaving))
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
            var billableElement = new XElement("TimeEntryRecord");
            billableElement.SetAttributeValue(XName.Get("IsChargeable"), "true");
            billableElement.SetAttributeValue(XName.Get("IsReviewed"), "Pending");
            billableSte.UpdateBillableElementEditedValues(billableElement);

            if (billableElement.Attribute(XName.Get("ActualHours")).Value != "" || (!HostingPage.IsSaving && billableElement.Attribute(XName.Get("Note")).Value != ""))
            {
                calendarItemElement.Add(billableElement);
            }
        }

        private void AddNonBillableElement(BillableAndNonBillableSingleTimeEntry nonBillableSte, XElement calendarItemElement)
        {
            var nonBillableElement = new XElement("TimeEntryRecord");
            nonBillableElement.SetAttributeValue(XName.Get("IsChargeable"), "false");
            nonBillableElement.SetAttributeValue(XName.Get("IsReviewed"), "Pending");
            nonBillableSte.UpdateNonBillableElementEditedValues(nonBillableElement);

            if (nonBillableElement.Attribute(XName.Get("ActualHours")).Value != "" || (!HostingPage.IsSaving && nonBillableElement.Attribute(XName.Get("Note")).Value != ""))
            {
                calendarItemElement.Add(nonBillableElement);
            }
        }

        internal void UpdateWorkType(XElement workTypeElement)
        {
            workTypeElement.Attribute(XName.Get("Id")).Value = ddlTimeTypes.SelectedValue;
        }

        internal void UpdateVerticalTotalCalculatorExtenderId(int index, string clientId)
        {
            var billableAndNonbillableSte = tes.Items[index].FindControl("ste") as BillableAndNonBillableSingleTimeEntry;
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
                var billableAndNonbillableSte = tesItem.FindControl("ste") as BillableAndNonBillableSingleTimeEntry;

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

    }
}

