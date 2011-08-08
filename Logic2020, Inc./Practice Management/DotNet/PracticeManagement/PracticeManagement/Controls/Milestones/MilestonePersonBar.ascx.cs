using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.PersonService;
using System.ServiceModel;
using PraticeManagement.MilestoneService;

namespace PraticeManagement.Controls.Milestones
{
    public partial class MilestonePersonBar : System.Web.UI.UserControl
    {
        private const string DuplPersonName = "The specified person is already assigned on this milestone.";
        private ExceptionDetail _internalException;

        private PraticeManagement.MilestonePersonList HostingPage
        {
            get { return ((PraticeManagement.MilestonePersonList)Page); }
        }

        public string BarColor
        {
            set
            {
                trBar.Style["background-color"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }


        #region Validation

        protected void reqHourlyRevenue_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = !HostingPage.Milestone.IsHourlyAmount || !string.IsNullOrEmpty(e.Value);
        }

        protected void custPersonStartInsert_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = dpPersonStartInsert.DateValue.Date >= HostingPage.Milestone.StartDate.Date;
        }

        protected void custPersonEndInsert_ServerValidate(object sender, ServerValidateEventArgs args)
        {

            var dpPersonEnd = dpPersonEndInsert;

            var bar = btnInsert.NamingContainer.NamingContainer as RepeaterItem;

            Person person = HostingPage.GetPersonBySelectedValue(ddlPerson.SelectedValue);

            bool isGreaterThanMilestone = dpPersonEnd.DateValue <= HostingPage.Milestone.ProjectedDeliveryDate;

            if (!isGreaterThanMilestone)
            {
                HostingPage.lblMoveMilestoneDateObject.Text = dpPersonEnd.DateValue.ToShortDateString();

                bool terminationAndCompensation =
                    ChechTerminationAndCompensation(dpPersonEnd.DateValue, person);

                HostingPage.cellMoveMilestoneObject.Visible = terminationAndCompensation;
                HostingPage.cellTerminationOrCompensationObject.Visible =
                    !terminationAndCompensation;
            }


            //HostingPage.pnlChangeMilestoneObject.Visible = !isGreaterThanMilestone;

            //TimeSpan shift = dpPersonEnd.DateValue.Subtract(HostingPage.Milestone.ProjectedDeliveryDate);

            //HostingPage.pnlChangeMilestoneObject.Attributes["milestonePersonId"] = "";
            //HostingPage.pnlChangeMilestoneObject.Attributes["ShiftDays"] = shift.Days.ToString();

            //HostingPage.btnMoveMilestoneObject.Enabled = false;

            args.IsValid = isGreaterThanMilestone;
        }

        protected void custPeriodOvberlappingInsert_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            var dpPersonStart = dpPersonStartInsert;
            var dpPersonEnd = dpPersonEndInsert;

            DateTime startDate = dpPersonStart.DateValue;
            DateTime endDate =
                dpPersonEnd.DateValue != DateTime.MinValue ? dpPersonEnd.DateValue : HostingPage.Milestone.ProjectedDeliveryDate;

            Person person = HostingPage.GetPersonBySelectedValue(ddlPerson.SelectedValue);

            List<MilestonePersonEntry> entries = new List<MilestonePersonEntry>();

            foreach (var item in HostingPage.MilestonePersons)
            {
                entries.AddRange(item.Entries);
            }
            entries = entries.OrderBy(entry => entry.ThisPerson.LastName).ThenBy(ent => ent.StartDate).AsQueryable().ToList();
            // Validate overlapping with other entries.
            for (int i = 0; i < entries.Count; i++)
            {
                if (i != HostingPage.gvMilestonePersonEntriesObject.EditIndex && entries[i].ThisPerson.Id == person.Id.Value)
                {
                    DateTime entryStartDate = entries[i].StartDate;
                    DateTime entryEndDate =
                        entries[i].EndDate.HasValue
                            ?
                                entries[i].EndDate.Value
                            : HostingPage.Milestone.ProjectedDeliveryDate;

                    if ((startDate >= entryStartDate && startDate <= entryEndDate) ||
                        (endDate >= entryStartDate && endDate <= entryEndDate))
                    {
                        e.IsValid = false;
                        break;
                    }
                }
            }
        }

        protected void custPeriodVacationOverlappingInsert_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            var custPersonStartDate = custPersonStartInsert;
            var compPersonEnd = compPersonEndInsert;

            var ddlPersonName = ddlPerson;

            var isStartDateValid = ((System.Web.UI.WebControls.BaseValidator)(custPersonStartDate)).IsValid;
            var isEndDateValid = compPersonEnd.IsValid;

            if (isStartDateValid && isEndDateValid)
            {
                var dpPersonStart = dpPersonStartInsert;
                var dpPersonEnd = dpPersonEndInsert;

                DateTime startDate = dpPersonStart.DateValue;
                DateTime endDate =
                    dpPersonEnd.DateValue != DateTime.MinValue ? dpPersonEnd.DateValue : HostingPage.Milestone.ProjectedDeliveryDate;

                // Validate overlapping with other entries.
                int days = GetPersonWorkDaysNumber(startDate, endDate, ddlPersonName);
                if (days == 0)
                {
                    e.IsValid = false;
                }
            }
        }

        protected void custPersonInsert_ServerValidate(object sender, ServerValidateEventArgs args)
        {

            Person person = HostingPage.GetPersonBySelectedValue(ddlPerson.SelectedValue);

            List<MilestonePerson> MilestonePersonList = HostingPage.MilestonePersons.Where(mp => mp.Person.Id.Value == person.Id.Value).AsQueryable().ToList();

            List<MilestonePersonEntry> entries = new List<MilestonePersonEntry>();

            foreach (var item in MilestonePersonList)
            {
                entries.AddRange(item.Entries);
            }

            foreach (MilestonePersonEntry entry in entries)
            {
                if (person == null ||
                    person.HireDate > entry.StartDate ||
                    (person.TerminationDate.HasValue && entry.EndDate.HasValue &&
                     person.TerminationDate.Value < entry.EndDate))
                {
                    args.IsValid = false;
                    break;
                }
            }
        }

        protected void custEntriesInsert_ServerValidate(object sender, ServerValidateEventArgs args)
        {

            Person person = HostingPage.GetPersonBySelectedValue(ddlPerson.SelectedValue);

            List<MilestonePerson> MilestonePersonList = HostingPage.MilestonePersons.Where(mp => mp.Person.Id.Value == person.Id.Value).AsQueryable().ToList();

            List<MilestonePersonEntry> entries = new List<MilestonePersonEntry>();

            foreach (var item in MilestonePersonList)
            {
                entries.AddRange(item.Entries);
            }

            args.IsValid = entries.Count > 0;
        }

        protected void custDuplicatedPersonInsert_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!String.IsNullOrEmpty(HostingPage.ExMessage))
            {
                args.IsValid = !(HostingPage.ExMessage == DuplPersonName);
            }
            else
            {
                args.IsValid = true;
            }
        }

        #endregion

        private int GetPersonWorkDaysNumber(DateTime startDate, DateTime endDate, DropDownList ddlPersonName)
        {
            int days = -1;
            if (!string.IsNullOrEmpty(ddlPersonName.SelectedValue))
            {
                using (var serviceClient = new PersonServiceClient())
                {
                    try
                    {
                        days = serviceClient.GetPersonWorkDaysNumber(int.Parse(ddlPersonName.SelectedValue), startDate,
                                                                     endDate);
                    }
                    catch (FaultException<ExceptionDetail> ex)
                    {
                        _internalException = ex.Detail;
                        serviceClient.Abort();
                        Page.Validate();
                    }
                    catch (CommunicationException ex)
                    {
                        _internalException = new ExceptionDetail(ex);
                        serviceClient.Abort();
                        Page.Validate();
                    }
                }
            }
            return days;
        }

        private bool ChechTerminationAndCompensation(DateTime endDate, Person person)
        {

            DateTime termDate =
                person.TerminationDate.HasValue
                    ?
                        person.TerminationDate.Value
                    :
                        DateTime.MaxValue;

            return
                termDate >= endDate &&
                DataHelper.IsCompensationCoversMilestone(person, person.HireDate, endDate);
        }

        protected void btnInsertPerson_Click(object sender, EventArgs e)
        {
            var bar = btnInsert.NamingContainer.NamingContainer as RepeaterItem;

            HostingPage.vsumMileStonePersonsObject.ValidationGroup = "MilestonePersonEntry" + bar.ItemIndex.ToString();

            Page.Validate(HostingPage.vsumMileStonePersonsObject.ValidationGroup);
            if (Page.IsValid)
            {
                HostingPage.lblResultMessageObject.ClearMessage();
                HostingPage.AddAndBindRow(bar);

                HostingPage.pnlChangeMilestoneObject.Visible = false;
                HostingPage.RemoveItemAndDaabindRepeater(bar.ItemIndex);
            }
        }

        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            var bar = btnCancel.NamingContainer.NamingContainer as RepeaterItem;

            HostingPage.RemoveItemAndDaabindRepeater(bar.ItemIndex);
        }


        protected void dpPersonStart_SelectionChanged(object sender, EventArgs e)
        {
            HostingPage.IsDirty = true;
            ((Control)sender).Focus();
        }

        protected void dpPersonEnd_SelectionChanged(object sender, EventArgs e)
        {
            HostingPage.IsDirty = true;
            ((Control)sender).Focus();
        }


        protected string GetValidationGroup()
        {
            var bar = btnInsert.NamingContainer.NamingContainer as RepeaterItem;
            return "MilestonePersonEntry" + bar.ItemIndex.ToString();
        }
    }
}

