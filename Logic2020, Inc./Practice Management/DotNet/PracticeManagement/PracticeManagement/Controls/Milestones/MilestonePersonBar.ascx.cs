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

        private PraticeManagement.MilestoneDetail HostingPage
        {
            get { return ((PraticeManagement.MilestoneDetail)Page); }
        }

        private MilestonePersonList HostingControl
        {
            get { return HostingPage.MilestonePersonEntryListControlObject; }
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

            Person person = HostingControl.GetPersonBySelectedValue(ddlPerson.SelectedValue);

            bool isGreaterThanMilestone = dpPersonEnd.DateValue <= HostingPage.Milestone.ProjectedDeliveryDate;
            args.IsValid = isGreaterThanMilestone;
        }

        protected void custPeriodOvberlappingInsert_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            var dpPersonStart = dpPersonStartInsert;
            var dpPersonEnd = dpPersonEndInsert;

            DateTime startDate = dpPersonStart.DateValue;
            DateTime endDate =
                dpPersonEnd.DateValue != DateTime.MinValue ? dpPersonEnd.DateValue : HostingPage.Milestone.ProjectedDeliveryDate;

            Person person = HostingControl.GetPersonBySelectedValue(ddlPerson.SelectedValue);

            List<MilestonePersonEntry> entries = new List<MilestonePersonEntry>();

            foreach (var item in HostingControl.MilestonePersons)
            {
                entries.AddRange(item.Entries);
            }
            entries = entries.OrderBy(entry => entry.ThisPerson.LastName).ThenBy(ent => ent.StartDate).AsQueryable().ToList();
            // Validate overlapping with other entries.
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].ThisPerson.Id == person.Id.Value)
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

            Person person = HostingControl.GetPersonBySelectedValue(ddlPerson.SelectedValue);

            List<MilestonePerson> MilestonePersonList = HostingControl.MilestonePersons.Where(mp => mp.Person.Id.Value == person.Id.Value).AsQueryable().ToList();

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

        protected void custDuplicatedPersonInsert_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!String.IsNullOrEmpty(HostingControl.ExMessage))
            {
                args.IsValid = !(HostingControl.ExMessage == DuplPersonName);
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
            InsertPerson(bar);
        }

        private bool InsertPerson(RepeaterItem bar, bool isSaveCommit = true, bool iSDatBindRows = true)
        {

            HostingControl.vsumMileStonePersonsObject.ValidationGroup = "MilestonePersonEntry" + bar.ItemIndex.ToString();

            Page.Validate(HostingControl.vsumMileStonePersonsObject.ValidationGroup);
            if (Page.IsValid)
            {
                HostingControl.lblResultMessageObject.ClearMessage();
                HostingControl.AddAndBindRow(bar, isSaveCommit, iSDatBindRows);

                if (isSaveCommit && iSDatBindRows)
                HostingControl.RemoveItemAndDaabindRepeater(bar.ItemIndex);
            }

            return Page.IsValid;

        }

        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            var bar = btnCancel.NamingContainer.NamingContainer as RepeaterItem;

            HostingControl.RemoveItemAndDaabindRepeater(bar.ItemIndex);
        }

        protected void imgCopy_OnClick(object sender, EventArgs e)
        {
            var bar = imgCopy.NamingContainer.NamingContainer as RepeaterItem;
            HostingControl.CopyItemAndDaabindRepeater(bar.ItemIndex);
        }


        protected string GetValidationGroup()
        {
            var bar = btnInsert.NamingContainer.NamingContainer as RepeaterItem;
            return "MilestonePersonEntry" + bar.ItemIndex.ToString();
        }

        internal bool ValidateAll(RepeaterItem mpBar, bool isSaveCommit)
        {
            return InsertPerson(mpBar, isSaveCommit);
        }

        internal bool SaveAll(RepeaterItem mpBar, bool isSaveCommit, bool iSDatBindRows)
        {
            return InsertPerson(mpBar, isSaveCommit, iSDatBindRows);
        }

    }
}

