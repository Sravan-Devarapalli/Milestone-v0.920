using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using PraticeManagement.Controls;
using DataTransferObjects;

namespace PraticeManagement.Reporting
{
    public partial class ProjectSummaryReport : System.Web.UI.Page
    {
        #region Properties


        public DateTime? StartDate
        {
            get
            {
                int selectedVal = 0;
                if (int.TryParse(ddlPeriod.SelectedValue, out selectedVal) && selectedVal == 0)
                {
                    return diRange.FromDate.Value;
                }
                else 
                {
                    ListItem li = ddlPeriod.SelectedItem;
                    string startDateString = li.Attributes["startdate"];
                    DateTime startDate ;
                    if (DateTime.TryParse(startDateString, out startDate))
                    {
                        return startDate;
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
                if (int.TryParse(ddlPeriod.SelectedValue, out selectedVal) && selectedVal == 0)
                {
                    return diRange.ToDate.Value;
                }
                else
                {
                    ListItem li = ddlPeriod.SelectedItem;
                    string enddateString = li.Attributes["enddate"];
                    DateTime enddate;
                    if (DateTime.TryParse(enddateString, out enddate))
                    {
                        return enddate;
                    }
                }
                return null;
            }
        }

        public String ProjectNumber
        {
            get
            {
                return txtProjectNumber.Text.ToUpper();
            }
        }

        public string ProjectRange
        {
            get
            {
                ListItem li =  ddlPeriod.SelectedItem;
                string milestoneName = li.Text;
                if (!StartDate.HasValue || !EndDate.HasValue)
                {
                    return string.Empty;
                }
                else if (li.Value != "0")
                {
                    return milestoneName + " (" + StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat) + ")";
                }
                else
                {
                    return StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat);
                }
            }
        }

        public int? MilestoneId
        {
            get
            {
                return (ddlPeriod.SelectedValue != "*" && ddlPeriod.SelectedValue != "0") ? (int?)Convert.ToInt32(ddlPeriod.SelectedValue) : null;
            }
        }

        public string PeriodSelected
        {
            get
            {
                return ddlPeriod.SelectedValue;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var clients = DataHelper.GetAllClientsSecure(null, true, true);
                DataHelper.FillListDefault(ddlClients, "-- Select an Account -- ", clients as object[], false);
            }

        }

        protected void txtProjectNumber_OnTextChanged(object sender, EventArgs e)
        {   
            PopulateddlPeriod(ProjectNumber);
            LoadActiveView();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var now = Utils.Generic.GetNowWithTimeZone();
            diRange.FromDate = StartDate.HasValue ? StartDate : Utils.Calendar.WeekStartDate(now);
            diRange.ToDate = EndDate.HasValue ? EndDate : Utils.Calendar.WeekEndDate(now);
            lblCustomDateRange.Text = string.Format("({0}&nbsp;-&nbsp;{1})",
                    diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat),
                    diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat)
                    );

            if (ddlPeriod.SelectedValue == "0")
            {
                lblCustomDateRange.Attributes.Add("class", "");
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

        private void LoadActiveView()
        {
            if (!string.IsNullOrEmpty(ProjectNumber) && ddlView.SelectedValue != string.Empty)
            {
                mvProjectSummaryReport.ActiveViewIndex = Convert.ToInt32(ddlView.SelectedValue);

                divWholePage.Style.Remove("display");
                if (mvProjectSummaryReport.ActiveViewIndex == 0)
                {
                    PopulateByResourceData();
                }
                else if (mvProjectSummaryReport.ActiveViewIndex == 1)
                {
                    PopulateByWorkTypeData();
                }
            }
            else
            {
                divWholePage.Style.Add("display", "none");
            }
        }

        protected void ddlView_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadActiveView();
        }

        protected void ddlPeriod_SelectedIndexChanged(object sender, EventArgs e)
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

        private void PopulateByResourceData()
        {
            try
            {
                msgError.ClearMessage();
                divWholePage.Style.Remove("display");
                ucByResource.LoadActiveTabInByResource();
            }
            catch (Exception ex)
            {
                msgError.ShowErrorMessage(ex.Message);
                divWholePage.Style.Add("display", "none");
            }
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

        protected void btnclose_OnClick(object sender, EventArgs e)
        {
            ClearFilters();
        }

        private void ClearFilters()
        {
            ltrlNoProjectsText.Visible = repProjectNamesList.Visible = false;
            ClearAndAddFirsItemForDdlProjects();
            ddlProjects.SelectedIndex = ddlClients.SelectedIndex = 0;
            txtProjectSearch.Text = string.Empty;
            btnProjectSearch.Attributes["disabled"] = "disabled";
        }

        protected void btnProjectSearch_Click(object sender, EventArgs e)
        {
            List<Project> list = ServiceCallers.Custom.Report(r => r.ProjectSearchByName(txtProjectSearch.Text)).ToList();

            btnProjectSearch.Attributes.Remove("disabled");

            if (list.Count > 0)
            {
                ltrlNoProjectsText.Visible = false;
                repProjectNamesList.Visible = true;
                repProjectNamesList.DataSource = list;
                repProjectNamesList.DataBind();
            }
            else
            {
                repProjectNamesList.Visible = false;
                ltrlNoProjectsText.Visible = true;
            }

            mpeProjectSearch.Show();

        }

        protected void lnkProjectNumber_OnClick(object sender, EventArgs e)
        {
            var lnkProjectNumber = sender as LinkButton;
            PopulateControls(lnkProjectNumber.Attributes["ProjectNumber"]);
        }

        private void PopulateByWorkTypeData()
        {
            //var data = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByWorkType(ProjectNumber, string.Empty, string.Empty));


            //ucByWorktype.DataBindResource(data, null);
            //ucBillableAndNonBillable.BillablValue = (data.Count() > 0) ? data.Sum(d => d.BillabileTotal).ToString() : "0";
            //ucBillableAndNonBillable.NonBillablValue = (data.Count() > 0) ? data.Sum(d => d.NonBillableTotal).ToString() : "0";

        }

        protected void ddlClients_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ClearAndAddFirsItemForDdlProjects();

            if (ddlClients.SelectedIndex != 0)
            {
                ddlProjects.Enabled = true;

                int clientId = Convert.ToInt32(ddlClients.SelectedItem.Value);

                var projects = DataHelper.GetProjectsByClientId(clientId);

                projects = projects.OrderBy(p => p.Status.Name).ThenBy(p => p.ProjectNumber).ToArray();

                foreach (var project in projects)
                {
                    var li = new ListItem(project.ProjectNumber + " - " + project.Name,
                                           project.ProjectNumber.ToString());

                    li.Attributes[Constants.Variables.OptionGroup] = project.Status.Name;

                    ddlProjects.Items.Add(li);

                }
            }

            mpeProjectSearch.Show();
        }

        private void ClearAndAddFirsItemForDdlProjects()
        {
            ListItem firstItem = new ListItem("-- Select a Project --", string.Empty);
            ddlProjects.Items.Clear();
            ddlProjects.Items.Add(firstItem);
            ddlProjects.Enabled = false;

        }

        protected void ddlProjects_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlProjects.SelectedValue != string.Empty)
            {
                var projectNumber = ddlProjects.SelectedItem.Value;

                PopulateControls(projectNumber);
            }
            else
            {
                mpeProjectSearch.Show();
            }
        }

        private void PopulateControls(string projectNumber)
        {
            txtProjectNumber.Text = projectNumber;
            PopulateddlPeriod(projectNumber);
            LoadActiveView();
            ClearFilters();
        }

        private void PopulateddlPeriod(string projectNumber)
        {
            var list = ServiceCallers.Custom.Report(r => r.GetMilestonesForProject(ProjectNumber));

            ddlPeriod.Items.Clear();
            var listItem = new ListItem("Entire Project", "*");
            if (list.Length > 0)
            {
                DateTime projectStartDate = list.Min(m => m.StartDate);
                DateTime projectEndDate = list.Max(m => m.ProjectedDeliveryDate);
                listItem.Attributes.Add("startdate", projectStartDate.ToString("MM/dd/yyyy"));
                listItem.Attributes.Add("enddate", projectEndDate.ToString("MM/dd/yyyy"));
            }
            ddlPeriod.Items.Add(listItem);
            foreach (var milestone in list)
            {
                ListItem li = new ListItem() { Text = milestone.Description, Value = milestone.Id.Value.ToString() };
                li.Attributes.Add("startdate", milestone.StartDate.ToString("MM/dd/yyyy"));
                li.Attributes.Add("enddate", milestone.ProjectedDeliveryDate.ToString("MM/dd/yyyy"));
                ddlPeriod.Items.Add(li);
            }
            var customListItem = new ListItem("Custom Dates", "0");
            ddlPeriod.Items.Add(customListItem);
            ddlPeriod.SelectedValue = "*";
        }
    }
}

