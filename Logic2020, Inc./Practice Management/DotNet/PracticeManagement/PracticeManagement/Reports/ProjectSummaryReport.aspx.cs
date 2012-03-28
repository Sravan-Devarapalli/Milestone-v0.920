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
            LoadActiveView();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadActiveView();
            }
        }

        protected void btnView_Command(object sender, CommandEventArgs e)
        {
            int viewIndex = int.Parse((string)e.CommandArgument);
            SelectView((Control)sender, viewIndex);
            LoadActiveView();
        }

        private void SelectView(Control sender, int viewIndex)
        {
            mvProjectSummaryReport.ActiveViewIndex = viewIndex;

            SetCssClassEmpty();

            ((WebControl)sender.Parent).CssClass = "SelectedSwitch";
        }

        private void SetCssClassEmpty()
        {
            foreach (TableCell cell in tblProjectsummaryReportViewSwitch.Rows[0].Cells)
            {
                cell.CssClass = string.Empty;
            }
        }

        private void LoadActiveView()
        {
            if (!string.IsNullOrEmpty(ProjectNumber))
            {
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



        private void PopulateByResourceData()
        {
            try
            {
                msgError.ClearMessage();
                divWholePage.Style.Remove("display");
                var data = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByResource(ProjectNumber));
                ucByResource.DataBindResource(data);
            }
            catch (Exception ex)
            {
                msgError.ShowErrorMessage(ex.Message);
                divWholePage.Style.Add("display", "none");
            }
            //ucBillableAndNonBillable.BillablValue = (data.Count() > 0) ? data.Sum(d => d.BillabileHours).ToString() : "0";
            //ucBillableAndNonBillable.NonBillablValue = (data.Count() > 0) ? data.Sum(d => d.NonBillableHours).ToString() : "0";

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
            LoadActiveView();
            ClearFilters();
        }
    }
}

