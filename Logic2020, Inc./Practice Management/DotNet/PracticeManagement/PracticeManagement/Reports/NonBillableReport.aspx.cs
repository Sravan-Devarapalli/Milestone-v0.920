using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using PraticeManagement.ProjectGroupService;
using DataTransferObjects;
using System.ServiceModel;
using System.Text;
using DataTransferObjects.Reports;

namespace PraticeManagement.Reports
{
    public partial class NonBillableReport : System.Web.UI.Page
    {

        public string DirectorIds
        {
            get
            {
                if (cblDirectors.Items.Count == 0)
                    return null;
                else
                {
                    var directorsList = new StringBuilder();
                    foreach (ListItem item in cblDirectors.Items)
                        if (item.Selected)
                            directorsList.Append(item.Value).Append(',');
                    return directorsList.ToString();
                }
            }
        }

        public string BusinessUnitIds
        {
            get
            {
                if (cblProjectGroup.Items.Count == 0)
                    return null;
                else
                {
                    var groupList = new StringBuilder();
                    foreach (ListItem item in cblProjectGroup.Items)
                        if (item.Selected)
                            groupList.Append(item.Value).Append(',');
                    return groupList.ToString();
                }
            }
        }

        public string PracticeIds
        {
            get
            {
                if (cblPractices.Items[0].Selected == true)
                    return null;
                else if (cblPractices.Items.Count == 0)
                    return string.Empty;
                else
                {
                    var practiceList = new StringBuilder();
                    foreach (ListItem item in cblPractices.Items)
                        if (item.Selected)
                            practiceList.Append(item.Value).Append(',');
                    return practiceList.ToString();
                }
            }
        }

        public string ProjectNumber
        {
            get
            {
                return txtProjectNumber.Text.Trim();
            }
        }

        public DateTime? StartDate
        {
            get
            {
                return diRange.FromDate.HasValue ? (DateTime?)diRange.FromDate.Value : null;
            }
        }

        public DateTime? EndDate
        {
            get
            {
                return diRange.ToDate.HasValue ? (DateTime?)diRange.ToDate.Value : null;
            }
        }

        public string SelectedProjectOption
        {
            get
            {
                return ddlProjectsOption.SelectedValue;
            }
        }

        public ProjectLevelGroupedHours ProjectForDetail
        {
            get;set;
        }

        public bool FromUpdateClick
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FromUpdateClick = false;
            if (!IsPostBack)
            {   FillInitPracticesList();
                FillInitDirectorsList();
                FillInitBusinessUnits();
                var clients = DataHelper.GetAllClientsSecure(null, true, true);
                DataHelper.FillListDefault(ddlClients, "-- Select an Account -- ", clients as object[], false);
                var now = Utils.Generic.GetNowWithTimeZone();
                diRange.FromDate = Utils.Calendar.MonthStartDate(now);
                diRange.ToDate = Utils.Calendar.MonthEndDate(now);
            }
        }

        private void FillInitPracticesList()
        {
            DataHelper.FillPracticeList(cblPractices, Resources.Controls.AllPracticesText);
            cblPractices.SelectAll();
        }

        private void FillInitDirectorsList()
        {
            DataHelper.FillDirectorsList(cblDirectors, "All Executives in Charge", null);
            cblDirectors.SelectAll();
        }

        private void FillInitBusinessUnits()
        {
            using (var serviceClient = new ProjectGroupServiceClient())
            {
                try
                {
                    ProjectGroup[] groups = serviceClient.GroupListAll(null, null).OrderBy(g => g.Name).ToArray();
                    DataHelper.FillListDefault(cblProjectGroup, "All Business Units", groups.OrderBy(g=>g.ClientProjectGroupFormat).ToArray(), false, "Id", "ClientProjectGroupFormat");
                    cblProjectGroup.SelectAll();
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        protected void btnUpdateView_Click(object sender, EventArgs e)
        {
            Page.Validate(valSum.ValidationGroup);
            if (Page.IsValid)
            {
                FromUpdateClick = true;
                SwitchView((Control)lnkbtnSummary, 0);
                lblRange.Text = StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat);
            }
        }

        protected void ddlProjectsOption_SelecteIndexChanged(object sender, EventArgs e)
        {
            txtProjectNumber.Visible = imgProjectSearch.Visible = false;
            cblDirectors.Attributes.Add("style", "display:none");
            cblProjectGroup.Attributes.Add("style", "display:none");
            cblPractices.Attributes.Add("style", "display:none");
            sdeProjectGroup.Display = sdeDirectors.Display = sdePractices.Display = "none";
            btnUpdateView.Enabled = true;
            txtProjectNumber.Text = string.Empty;
            cblDirectors.SelectAll();
            cblProjectGroup.SelectAll();
            cblPractices.SelectAll();
            switch (ddlProjectsOption.SelectedValue)
            {
                case "0": btnUpdateView.Enabled = false; break;
                case "1": txtProjectNumber.Visible = imgProjectSearch.Visible = true; break;
                case "3": cblDirectors.Attributes.Add("style", "display:inline-block;"); cblDirectors.Attributes.Add("style", "width:239px;"); sdeDirectors.Display = "inline-block;"; break;
                case "4": cblProjectGroup.Attributes.Add("style", "display:inline-block;"); cblProjectGroup.Attributes.Add("style", "width:239px;"); sdeProjectGroup.Display = "inline-block;"; break;
                case "5": cblPractices.Attributes.Add("style", "display:inline-block;"); ; cblPractices.Attributes.Add("style", "width:239px;"); sdePractices.Display = "inline-block;"; break;
            }
        }

        protected void btnclose_OnClick(object sender, EventArgs e)
        {
            ClearFilters();
        }

        protected void btnView_Command(object sender, CommandEventArgs e)
        {

            int viewIndex = int.Parse((string)e.CommandArgument);

            SwitchView((Control)sender, viewIndex);
        }

        private void SelectView()
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                divWholePage.Style.Remove("display");
                LoadActiveView();
            }
            else
            {
                divWholePage.Style.Add("display", "none");
            }
        }

        private void SwitchView(Control control, int viewIndex)
        {
            SelectView(control, viewIndex);
            SelectView();
        }

        private void SetCssClassEmpty()
        {
            foreach (TableCell cell in tblProjectViewSwitch.Rows[0].Cells)
            {
                cell.CssClass = string.Empty;
            }
        }

        public void SelectView(Control sender, int viewIndex)
        {
            mvAccountReport.ActiveViewIndex = viewIndex;

            SetCssClassEmpty();

            ((WebControl)sender.Parent).CssClass = "SelectedSwitch";
        }

        private void LoadActiveView()
        {
            int activeView = mvAccountReport.ActiveViewIndex;
            switch (activeView)
            {
                case 0:
                    nonbillableSummary.PopulateData();
                    break;
                case 1:nonbillableDetail.PopulateData(ProjectForDetail);
                    break;
            }
        }

        public void AssignProjectForDetail(ProjectLevelGroupedHours project)
        {
            ProjectForDetail = project;
            SwitchView((Control)lnkbtnDetail, 1);
        }

        private void ClearAndAddFirsItemForDdlProjects()
        {
            System.Web.UI.WebControls.ListItem firstItem = new System.Web.UI.WebControls.ListItem("-- Select a Project --", string.Empty);
            ddlProjects.Items.Clear();
            ddlProjects.Items.Add(firstItem);
            ddlProjects.Enabled = false;
        }

        private void ClearFilters()
        {
            ltrlNoProjectsText.Visible = repProjectNamesList.Visible = false;
            ClearAndAddFirsItemForDdlProjects();
            ddlProjects.SelectedIndex = ddlClients.SelectedIndex = 0;
            txtProjectSearch.Text = string.Empty;
            btnProjectSearch.Attributes["disabled"] = "disabled";
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
                    var li = new System.Web.UI.WebControls.ListItem(project.ProjectNumber + " - " + project.Name,
                                           project.ProjectNumber.ToString());

                    li.Attributes[Constants.Variables.OptionGroup] = project.Status.Name;

                    ddlProjects.Items.Add(li);

                }
            }

            mpeProjectSearch.Show();
        }

        protected void ddlProjects_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlProjects.SelectedValue != string.Empty)
            {
                var projectNumber = ddlProjects.SelectedItem.Value;

                txtProjectNumber.Text = projectNumber;
            }
            else
            {
                mpeProjectSearch.Show();
            }
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
            txtProjectNumber.Text = lnkProjectNumber.Attributes["ProjectNumber"];
        }
    }
}

