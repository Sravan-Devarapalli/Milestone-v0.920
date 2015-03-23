using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using PraticeManagement.Controls;
using DataTransferObjects.Reports;

namespace PraticeManagement.Reports
{
    public partial class UtilizationReport : System.Web.UI.Page
    {
        public DateTime WeekStartDate
        { get; set; }

        public DateTime WeekEndDate
        { get; set; }

        public DateTime YearStartDate
        {
            get
            {
                var now = Utils.Generic.GetNowWithTimeZone();
                return Utils.Calendar.YearStartDate(now);
            }
        }

        public DateTime YTDEndDate
        {
            get
            {
                return Utils.Generic.GetNowWithTimeZone();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var now = Utils.Generic.GetNowWithTimeZone();
                WeekStartDate = Utils.Calendar.WeekStartDate(now);
                WeekEndDate = Utils.Calendar.WeekStartDate(now);
                PopulateDropdownValues();
                PopulateUtilizationValues();
            }
        }

        protected void ddlPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateUtilizationValues();
        }

        public void PopulateDropdownValues()
        {
            var currentPerson = DataHelper.CurrentPerson;
            var userIsAdministrator = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
            if (userIsAdministrator)
            {
                var persons = ServiceCallers.Custom.Person(p => p.PersonsListHavingActiveStatusDuringThisPeriod(WeekStartDate, WeekEndDate));
                DataHelper.FillPersonList(ddlPerson, null, persons, null);
                ListItem selectedPerson = ddlPerson.Items.FindByValue(currentPerson.Id.Value.ToString()); //ddlPerson.SelectedValue = currentPerson.Id.Value.ToString();
                if (selectedPerson != null)
                {
                    ddlPerson.SelectedValue = currentPerson.Id.Value.ToString();
                }
            }
            else
            {
                lblPerson.Visible = true;
                ddlPerson.Visible = false;
                lblPerson.Text = currentPerson.HtmlEncodedName;
            }
        }

        public void PopulateUtilizationValues()
        {
            var currentPerson = DataHelper.CurrentPerson;
            var userIsAdministrator = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
            var totalHours = new PersonTimeEntriesTotals();
            var personId = userIsAdministrator ? Convert.ToInt32(ddlPerson.SelectedValue) : currentPerson.Id.Value;
            totalHours = ServiceCallers.Custom.Report(r => r.UtilizationReport(personId, YearStartDate, YTDEndDate));
            lblTargetHours.Text = totalHours.AvailableHours.ToString();
            lblBillableTime.Text = lblBillableTime2.Text = totalHours.BillableHoursUntilToday.ToString();
            lblAllocatedBillable.Text = totalHours.ProjectedHours.ToString();
            lblUtilization.Text = totalHours.BillableUtilizationPercentage;
            lblAllocatedVsTarget.Text = totalHours.BillableAllocatedVsTarget;
            lblAllocatedVsTarget.Style["color"] = totalHours.BillableAllocatedVsTargetValue >= 0 ? "Black" : "#F00";
        }
    }
}
