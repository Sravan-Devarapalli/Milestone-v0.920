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

        public String ProjectNumber
        {
            get
            {
                return txtProjectNumber.Text;
            }
        }

        public string ProjectRange
        {
            get
            {
                ListItem li =  ddlPeriod.SelectedItem;
                string startDate = li.Attributes["startdate"];
                string endDate = li.Attributes["enddate"];
                string milestoneName = li.Text;
                return milestoneName + "( " + startDate + " - " + endDate + " )";
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

            LoadActiveView();

        }

        private void PopulateByResourceData()
        {
            try
            {
                msgError.ClearMessage();
                divWholePage.Style.Remove("display");
                var data = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByResource(ProjectNumber, ddlPeriod.SelectedValue != "*" ? (int?)Convert.ToInt32(ddlPeriod.SelectedValue) : null));
                ucByResource.DataBindResource(data);
            }
            catch (Exception ex)
            {
                msgError.ShowErrorMessage(ex.Message);
                divWholePage.Style.Add("display", "none");
            }
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
            DateTime projectStartDate = list.Min(m => m.StartDate);
            DateTime projectEndDate = list.Max(m => m.ProjectedDeliveryDate);
            listItem.Attributes.Add("startdate", projectStartDate.ToString("MM/dd/yyyy"));
            listItem.Attributes.Add("enddate", projectEndDate.ToString("MM/dd/yyyy"));
            ddlPeriod.Items.Add(listItem);

            foreach (var milestone in list)
            {
                ListItem li = new ListItem() { Text = milestone.Description, Value = milestone.Id.Value.ToString() };
                li.Attributes.Add("startdate", milestone.StartDate.ToString("MM/dd/yyyy"));
                li.Attributes.Add("enddate", milestone.ProjectedDeliveryDate.ToString("MM/dd/yyyy"));
                ddlPeriod.Items.Add(li);
            }
            ddlPeriod.SelectedValue = "*";
        }
    }
}

