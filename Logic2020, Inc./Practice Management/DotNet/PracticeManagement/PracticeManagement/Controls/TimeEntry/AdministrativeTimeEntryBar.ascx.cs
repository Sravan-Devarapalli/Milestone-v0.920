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
        public List<XElement> TeBarDataSource
        {
            get;
            set;
        }

        public bool Disabled { get; set; }


        public bool IsPTO { get; set; }

        public TimeTypeRecord WorkType { get; set; }

        public Dictionary<int, string> TbAcutualHoursClientIds
        {
            get
            {
                return ViewState["AdministrativeTimeEntryBar_TbAcutualHoursClientIds"] as Dictionary<int, string>;
            }
            set
            {
                ViewState["AdministrativeTimeEntryBar_TbAcutualHoursClientIds"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            
        }


        public TimeEntry_New HostingPage
        {
            get
            {
                return ((TimeEntry_New)Page);
            }
        }


        protected string GetDayOffCssCalss(XElement calendarItem)
        {
            return calendarItem.Attribute(XName.Get("CssClass")).Value;
        }


        private void InitTimeEntryControl(SingleTimeEntry_New ste, DateTime date, XElement terXlement)
        {
            ste.DateBehind = date;
            ste.TimeEntryRecordElement = terXlement;
        }

        protected void repEntries_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ste = e.Item.FindControl("ste") as SingleTimeEntry_New;

                var calendarItem = (XElement)e.Item.DataItem;

                var textBoxId = ste.Controls[1].ClientID;

                TbAcutualHoursClientIds = TbAcutualHoursClientIds ?? new Dictionary<int, string>();

                TbAcutualHoursClientIds.Add(e.Item.ItemIndex, textBoxId);
                TbAcutualHoursClientIds = TbAcutualHoursClientIds;
                extTotalHours.ControlsToCheck += textBoxId + ";";

                ste.HorizontalTotalCalculatorExtenderId = extTotalHours.ClientID;
                ste.IsNoteRequired = calendarItem.Attribute(XName.Get("IsNoteRequired")).Value;
                ste.Disabled = Disabled;
                ste.IsPTO = IsPTO;

                var nbterecord = (calendarItem.HasElements && calendarItem.Descendants(XName.Get("TimeEntryRecord")).Where(ter => ter.Attribute(XName.Get("IsChargeable")).Value.ToLowerInvariant() == "false").ToList().Count > 0) ? calendarItem.Descendants(XName.Get("TimeEntryRecord")).Where(ter => ter.Attribute(XName.Get("IsChargeable")).Value.ToLowerInvariant() == "false").First() : null;
                var date = Convert.ToDateTime(calendarItem.Attribute(XName.Get("Date")).Value);
                InitTimeEntryControl(ste, date, nbterecord);


            }
        }

        protected void imgDropTes_Click(object sender, ImageClickEventArgs e)
        {
            //var daysNumber = HostingPage.SelectedDates.Length;
            //var startDate = HostingPage.SelectedDates[0];
            //var endDate = HostingPage.SelectedDates[daysNumber - 1];

            //TimeEntryHelper.RemoveTimeEntries(ddlProjectMilestone.SelectedValue, ddlTimeTypes.SelectedValue, startDate, endDate);

            //HostingPage.TimeEntryControl.UpdateTimeEntries();
            //HostingPage.ClearDirtyState();
            //HostingPage.SetAttributesToDropDowns();
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

                var nonbillableSte = tes.Items[k].FindControl("ste") as SingleTimeEntry_New;

                var calendarItemElement = calendarItemElements[k];
                if (calendarItemElement.HasElements)
                {
                    var nonBillableElements = calendarItemElement.Descendants(XName.Get("TimeEntryRecord")).ToList().Count > 0 ? calendarItemElement.Descendants(XName.Get("TimeEntryRecord")).Where(ter => ter.Attribute(XName.Get("IsChargeable")).Value.ToLowerInvariant() == "false").ToList() : null;
                    if (nonBillableElements != null && nonBillableElements.Count > 0)
                    {
                        var nonbillableElement = nonBillableElements != null && nonBillableElements.Count > 0 ? nonBillableElements.First() : null;
                        nonbillableSte.UpdateEditedValues(nonbillableElement);
                    }
                }
                else
                {
                    //Add Element
                    var nonBillableElement = new XElement("TimeEntryRecord");
                    nonBillableElement.SetAttributeValue(XName.Get("IsChargeable"), "false");
                    nonbillableSte.UpdateEditedValues(nonBillableElement);

                    if (nonBillableElement.Attribute(XName.Get("ActualHours")).Value != "" || nonBillableElement.Attribute(XName.Get("Note")).Value != "")
                    {
                        calendarItemElement.Add(nonBillableElement);
                    }

                }
            }


        }


        internal void UpdateVerticalTotalCalculatorExtenderId(int index, string clientId)
        {
            var nonbillableSte = tes.Items[index].FindControl("ste") as SingleTimeEntry_New;
            nonbillableSte.UpdateVerticalTotalCalculatorExtenderId(clientId);

        }

        internal void ValidateAll()
        {
            foreach (RepeaterItem tesItem in tes.Items)
            {
                var nonbillableSte = tesItem.FindControl("ste") as SingleTimeEntry_New;
                nonbillableSte.ValidateNoteAndHours();
            }
        }

        internal void UpdateWorkType(XElement workTypeElement)
        {
            workTypeElement.Attribute(XName.Get("Id")).Value = hdnworkTypeId.Value;
        }

    }
}
