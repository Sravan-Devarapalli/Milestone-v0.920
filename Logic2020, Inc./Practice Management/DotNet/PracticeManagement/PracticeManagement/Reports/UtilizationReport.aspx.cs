using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using PraticeManagement.Controls;
using DataTransferObjects.Reports;
using PraticeManagement.Utils;
using DataTransferObjects;

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
                WeekEndDate = Utils.Calendar.WeekEndDate(now);
                PopulateDropdownValues();
                PopulateUtilizationValues();
                GetFilterValuesForSession();
            }
        }

        protected void ddlPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateUtilizationValues();
            SaveFilterValuesForSession();
        }

        public void PopulateDropdownValues()
        {
            var currentPerson = DataHelper.CurrentPerson;
            var userIsAdministrator = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
            if (userIsAdministrator)
            {
                var persons = ServiceCallers.Custom.Person(p => p.PersonsListHavingActiveStatusDuringThisPeriod(WeekStartDate, WeekEndDate));
                DataHelper.FillPersonList(ddlPerson, null, persons, null,false,true);
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
            var isSalaryType = ServiceCallers.Custom.Person(r => r.IsPersonSalaryTypeInGivenRange(personId, YearStartDate, YTDEndDate));
            if (isSalaryType)
            {
                totalHours = ServiceCallers.Custom.Report(r => r.UtilizationReport(personId, YearStartDate, YTDEndDate));
            }
            lblTargetHours.Text = isSalaryType? totalHours.AvailableHours.ToString():"N/A";
            lblBillableTime.Text = isSalaryType ? totalHours.BillableHoursUntilToday.ToString() : "N/A";
            //lblBillableTime2.Text = totalHours.BillableHoursUntilToday.ToString();
            //lblAllocatedBillable.Text = totalHours.ProjectedHours.ToString();
            lblUtilization.Text = isSalaryType? totalHours.BillableUtilizationPercentage:"N/A";
            //lblAllocatedVsTarget.Text = totalHours.BillableAllocatedVsTarget;
            //lblAllocatedVsTarget.Style["color"] = totalHours.BillableAllocatedVsTargetValue >= 0 ? "Black" : "#F00";

        }

        private void SaveFilterValuesForSession()
        {
            string filter = ddlPerson.SelectedValue;
            ReportsFilterHelper.SaveFilterValues(ReportName.UtilizationReport, filter);
        }

        private void GetFilterValuesForSession()
        {
            var filter = ReportsFilterHelper.GetFilterValues(ReportName.UtilizationReport) as string;
            if (filter != null)
            {
                ddlPerson.SelectedValue = filter;
                PopulateUtilizationValues();
            }
        }
    }
}

