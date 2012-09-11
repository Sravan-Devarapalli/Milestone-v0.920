using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using PraticeManagement.Controls;
using DataTransferObjects;
using System.Web.Security;


namespace PraticeManagement.Reporting
{
    public partial class PersonDetailTimeReport : Page
    {

        public DateTime? StartDate
        {
            get
            {
                int selectedVal = 0;
                if (int.TryParse(ddlPeriod.SelectedValue, out selectedVal))
                {
                    if (selectedVal == 0)
                    {
                        return diRange.FromDate.Value;
                    }
                    else
                    {
                        var now = Utils.Generic.GetNowWithTimeZone();
                        if (selectedVal > 0)
                        {
                            if (selectedVal == 7)
                            {
                                return Utils.Calendar.WeekStartDate(now);
                            }
                            else if (selectedVal == 30)
                            {
                                return Utils.Calendar.MonthStartDate(now);
                            }
                            else if (selectedVal == 15)
                            {
                                return Utils.Calendar.PayrollCurrentStartDate(now);
                            }
                            else
                            {
                                return Utils.Calendar.YearStartDate(now);
                            }

                        }
                        else if (selectedVal < 0)
                        {
                            if (selectedVal == -7)
                            {
                                return Utils.Calendar.LastWeekStartDate(now);
                            }
                            else if (selectedVal == -15)
                            {
                                return Utils.Calendar.PayrollPerviousStartDate(now);
                            }
                            else if (selectedVal == -30)
                            {
                                return Utils.Calendar.LastMonthStartDate(now);
                            }
                            else if (selectedVal == -365)
                            {
                                return Utils.Calendar.LastYearStartDate(now);
                            }
                            else if (selectedVal == -1)
                            {
                                return SelectedPerson.HireDate;
                            }
                        }
                        else
                        {
                            return diRange.FromDate.Value;
                        }
                    }
                }
                return null;
            }
        }

        public DateTime? EndDate
        {
            get
            {
                int selectedVal = 0;
                if (int.TryParse(ddlPeriod.SelectedValue, out selectedVal))
                {
                    if (selectedVal == 0)
                    {
                        return diRange.ToDate.Value;
                    }
                    else
                    {
                        var now = Utils.Generic.GetNowWithTimeZone();
                        DateTime firstDay = new DateTime(now.Year, now.Month, 1);
                        if (selectedVal > 0)
                        {
                            //7
                            //15
                            //30
                            //365
                            if (selectedVal == 7)
                            {
                                return Utils.Calendar.WeekEndDate(now);
                            }
                            else if (selectedVal == 15)
                            {
                                return Utils.Calendar.PayrollCurrentEndDate(now);
                            }
                            else if (selectedVal == 30)
                            {
                                return Utils.Calendar.MonthEndDate(now);
                            }
                            else if (selectedVal == 365)
                            {
                                return Utils.Calendar.YearEndDate(now);
                            }
                        }
                        else if (selectedVal < 0)
                        {
                            if (selectedVal == -7)
                            {
                                return Utils.Calendar.LastWeekEndDate(now);
                            }
                            else if (selectedVal == -15)
                            {
                                return Utils.Calendar.PayrollPerviousEndDate(now);
                            }
                            else if (selectedVal == -30)
                            {
                                return Utils.Calendar.LastMonthEndDate(now);
                            }
                            else if (selectedVal == -365)
                            {
                                return Utils.Calendar.LastYearEndDate(now);
                            }
                            else if (selectedVal == -1)
                            {
                                if (SelectedPerson.TerminationDate.HasValue)
                                {
                                    return SelectedPerson.TerminationDate;
                                }
                                else
                                {
                                    return now;
                                }

                            }
                        }
                        else
                        {
                            return diRange.ToDate.Value;
                        }
                    }
                }
                return null;
            }
        }

        public String Range
        {
            get
            {
                string range = string.Empty;
                int selectedVal = 0;
                if (int.TryParse(ddlPeriod.SelectedValue, out selectedVal) && StartDate.HasValue && EndDate.HasValue)
                {
                    if (selectedVal == -1)
                    {
                        range = "Total Employment (" + StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat) + ")";
                    }
                    else
                    {
                        if (StartDate.Value == Utils.Calendar.MonthStartDate(StartDate.Value) && EndDate.Value == Utils.Calendar.MonthEndDate(StartDate.Value))
                        {
                            range = StartDate.Value.ToString("MMMM yyyy");
                        }
                        else if (StartDate.Value == Utils.Calendar.YearStartDate(StartDate.Value) && EndDate.Value == Utils.Calendar.YearEndDate(StartDate.Value))
                        {
                            range = StartDate.Value.ToString("yyyy");
                        }
                        else
                        {
                            range = StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat);
                        }
                    }
                }
                return range;
            }
        }

        public int SelectedPersonId
        {
            get
            {
                return Convert.ToInt32(ddlPerson.SelectedValue);
            }
        }

        public Person SelectedPerson
        {
            get
            {
                Person person;
                if (ViewState["SelectedPerson"] == null)
                {
                    person = ServiceCallers.Custom.Person(p => p.GetPersonById(SelectedPersonId));
                    ViewState["SelectedPerson"] = person;
                }
                else
                {
                    person = (Person)ViewState["SelectedPerson"];
                    if (person.Id != SelectedPersonId)
                    {
                        person = ServiceCallers.Custom.Person(p => p.GetPersonById(SelectedPersonId));
                        ViewState["SelectedPerson"] = person;
                    }
                }
                return person;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            dlPersonDiv.Style.Add("display", "none");
            if (!IsPostBack)
            {
                bool userIsAdministrator = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
                bool userIsDirector = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName);
                bool userIsBusinessUnitManager = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.BusinessUnitManagerRoleName);

                var currentPerson = DataHelper.CurrentPerson;

                if (userIsAdministrator || userIsDirector || userIsBusinessUnitManager)
                {
                    string statusIds = (int)PersonStatusType.Active + "," + (int)PersonStatusType.TerminationPending;
                    DataHelper.FillPersonList(ddlPerson, null, statusIds, false);
                }
                else
                {
                    ddlPerson.Items.Clear();
                    var logInPerson = ServiceCallers.Custom.Person(p => p.GetStrawmanDetailsById(currentPerson.Id.Value));
                    ddlPerson.Items.Add(new ListItem(logInPerson.PersonLastFirstName, currentPerson.Id.Value.ToString()));
                    imgSearch.Visible = false;
                }

                ddlPerson.SelectedValue = currentPerson.Id.Value.ToString();

            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

            if (timeEntryReportHeader.Count == 2)
            {
                tdFirst.Attributes["class"] = "Width50Percent";
                tdThird.Attributes["class"] = "Width20Percent";
            }
            else if (timeEntryReportHeader.Count == 1)
            {
                tdFirst.Attributes["class"] = "Width35Percent";
                tdThird.Attributes["class"] = "Width35Percent";
            }

            int personId = int.Parse(ddlPerson.SelectedItem.Value);
            Person person = ServiceCallers.Custom.Person(p => p.GetStrawmanDetailsById(personId));
            lblPersonname.ToolTip = lblPersonname.Text = ddlPerson.SelectedItem.Text;
            string personType = person.IsOffshore ? "Offshore" : string.Empty;
            string payType = person.CurrentPay.TimescaleName;
            string personStatusAndType = string.IsNullOrEmpty(personType) && string.IsNullOrEmpty(payType) ? string.Empty :
                                                                             string.IsNullOrEmpty(payType) ? personType :
                                                                             string.IsNullOrEmpty(personType) ? payType :
                                                                                                                 payType + ", " + personType;
            lblPersonStatus.ToolTip = lblPersonStatus.Text = personStatusAndType;
            lbRange.ToolTip = lbRange.Text = Range;
            var now = Utils.Generic.GetNowWithTimeZone();
            diRange.FromDate = StartDate.HasValue ? StartDate : Utils.Calendar.WeekStartDate(now);
            diRange.ToDate = EndDate.HasValue ? EndDate : Utils.Calendar.WeekEndDate(now);
            lblCustomDateRange.Text = string.Format("({0}&nbsp;-&nbsp;{1})",
                    diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat),
                    diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat)
                    );

            if (ddlPeriod.SelectedValue == "0")
            {
                lblCustomDateRange.Attributes.Add("class", "fontBold");
                imgCalender.Attributes.Add("class", "");
            }
            else
            {
                lblCustomDateRange.Attributes.Add("class", "displayNone");
                imgCalender.Attributes.Add("class", "displayNone");
            }

            hdnStartDate.Value = diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat);
            hdnEndDate.Value = diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat);

            if (!IsPostBack)
            {
                LoadActiveView();
            }

        }

        protected void ddlPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            SwitchView(lnkbtnSummary, 0);
        }

        protected void btnCustDatesOK_Click(object sender, EventArgs e)
        {
            Page.Validate(valSumDateRange.ValidationGroup);
            if (Page.IsValid)
            {
                hdnStartDate.Value = StartDate.Value.Date.ToShortDateString();
                hdnEndDate.Value = EndDate.Value.Date.ToShortDateString();
                LoadActiveView();
            }
            else
            {
                mpeCustomDates.Show();
            }
        }

        protected void btnCustDatesCancel_OnClick(object sender, EventArgs e)
        {
            diRange.FromDate = Convert.ToDateTime(hdnStartDate.Value);
            diRange.ToDate = Convert.ToDateTime(hdnEndDate.Value);
        }

        protected void ddlPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (ddlPeriod.SelectedValue != "Please Select")
            {
                if (ddlPeriod.SelectedValue != "0")
                {
                    LoadActiveView();
                }
                else
                {
                    mpeCustomDates.Show();
                }
            }
            else
            {
                SwitchView(lnkbtnSummary, 0);
            }
        }

        protected void btnView_Command(object sender, CommandEventArgs e)
        {
            int viewIndex = int.Parse((string)e.CommandArgument);
            SwitchView((Control)sender, viewIndex);
        }

        private void SwitchView(Control control, int viewIndex)
        {
            SelectView(control, viewIndex);
            LoadActiveView();
        }

        private void SetCssClassEmpty()
        {
            foreach (TableCell cell in tblPersonViewSwitch.Rows[0].Cells)
            {
                cell.CssClass = string.Empty;
            }
        }

        private void SelectView(Control sender, int viewIndex)
        {
            mvPersonDetailReport.ActiveViewIndex = viewIndex;

            SetCssClassEmpty();

            ((WebControl)sender.Parent).CssClass = "SelectedSwitch";
        }

        private void LoadActiveView()
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                divWholePage.Style.Remove("display");
                Triple<double, double, double> result = ServiceCallers.Custom.Report(r => r.GetPersonTimeEntriesTotalsByPeriod(SelectedPersonId, StartDate.Value, EndDate.Value));
                PopulateTotalSection(result.First, result.Second, result.Third);
                if (mvPersonDetailReport.ActiveViewIndex == 0)
                {
                    PopulateSummaryDetails();
                }
                else
                {
                    PopulatePersonDetailReportDetails();
                }
            }
            else
            {
                divWholePage.Style.Add("display", "none");
            }
        }

        private void PopulatePersonDetailReportDetails()
        {
            var list = ServiceCallers.Custom.Report(r => r.PersonTimeEntriesDetails(SelectedPersonId, StartDate.Value, EndDate.Value)).ToList();
            ucpersonDetailReport.DatabindRepepeaterPersonDetails(list);
        }

        private void PopulateSummaryDetails()
        {
            var list = ServiceCallers.Custom.Report(r => r.PersonTimeEntriesSummary(SelectedPersonId, StartDate.Value, EndDate.Value)).ToList();
            ucpersonSummaryReport.DatabindRepepeaterSummary(list);
        }

        private void PopulateTotalSection(double billableHours, double nonBillableHours, double utlizationPercentage)
        {
            var billablePercent = 0;
            var nonBillablePercent = 0;
            if (billableHours != 0 || nonBillableHours != 0)
            {
                billablePercent = DataTransferObjects.Utils.Generic.GetBillablePercentage(billableHours, nonBillableHours);
                nonBillablePercent = (100 - billablePercent);
            }

            ltrlUtilization.Text = ((int)utlizationPercentage).ToString() + "%";
            ltrlBillableHours.Text = billableHours.ToString(Constants.Formatting.DoubleValue);
            ltrlNonBillableHours.Text = nonBillableHours.ToString(Constants.Formatting.DoubleValue);
            ltrlTotalHours.Text = (billableHours + nonBillableHours).ToString(Constants.Formatting.DoubleValue);
            ltrlBillablePercent.Text = billablePercent.ToString();
            ltrlNonBillablePercent.Text = nonBillablePercent.ToString();

            if (billablePercent == 0 && nonBillablePercent == 0)
            {
                trBillable.Height = "1px";
                trNonBillable.Height = "1px";
            }
            else if (billablePercent == 100)
            {
                trBillable.Height = "80px";
                trNonBillable.Height = "1px";
            }
            else if (billablePercent == 0 && nonBillablePercent == 100)
            {
                trBillable.Height = "1px";
                trNonBillable.Height = "80px";
            }
            else
            {
                int billablebarHeight = (int)(((float)80 / (float)100) * billablePercent);
                trBillable.Height = billablebarHeight.ToString() + "px";
                trNonBillable.Height = (80 - billablebarHeight).ToString() + "px";
            }

        }

        protected void lnkPerson_OnClick(object sender, EventArgs e)
        {
            LinkButton lbtn = sender as LinkButton;

            int personId = Convert.ToInt32(lbtn.Attributes["PersonId"]);

            var selectedItem = ddlPerson.Items.FindByValue(personId.ToString());

            if (selectedItem != null)
            {
                ddlPerson.SelectedValue = personId.ToString();
            }
            else
            {
                Person person = ServiceCallers.Custom.Person(p => p.GetStrawmanDetailsById(personId));
                ddlPerson.Items.Add(new ListItem(person.PersonLastFirstName, personId.ToString()));
                ddlPerson.SelectedValue = personId.ToString();
            }

            ddlPeriod.SelectedValue = "Please Select";
            LoadActiveView();
            txtSearch.Text = "";
        }

        protected String GetPersonFirstLastName(Person person)
        {
            return person.PersonLastFirstName;
        }

        protected void btnSearch_OnClick(object sender, EventArgs e)
        {
            string looked = txtSearch.Text;
            if (!string.IsNullOrEmpty(looked))
            {
                var personList = ServiceCallers.Custom.Person(p => p.GetPersonListBySearchKeyword(looked));
                dlPersonDiv.Style.Add("display", "");
                if (personList.Length > 0)
                {
                    repPersons.Visible = true;
                    divEmptyResults.Style.Add("display", "none");
                }
                else
                {
                    repPersons.Visible = false;
                    divEmptyResults.Style.Add("display", "");
                }
                repPersons.DataSource = personList;
                repPersons.DataBind();
                btnSearch.Attributes.Remove("disabled");
            }
            mpePersonSearch.Show();
        }

    }
}

