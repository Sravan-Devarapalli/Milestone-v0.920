﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using PraticeManagement.Controls;

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

            var data = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByResource(ProjectNumber));

            ucByResource.DataBindResource(data);
            ucBillableAndNonBillable.BillablValue = (data.Count() > 0) ? data.Sum(d => d.BillabileHours).ToString() : "0";
            ucBillableAndNonBillable.NonBillablValue = (data.Count() > 0) ? data.Sum(d => d.NonBillableHours).ToString() : "0";

        }

        private void PopulateByWorkTypeData()
        {
            var data = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByWorkType(ProjectNumber, string.Empty, string.Empty));


            ucByWorktype.DataBindResource(data, null);
            ucBillableAndNonBillable.BillablValue = (data.Count() > 0) ? data.Sum(d => d.BillabileTotal).ToString() : "0";
            ucBillableAndNonBillable.NonBillablValue = (data.Count() > 0) ? data.Sum(d => d.NonBillableTotal).ToString() : "0";

        }

        protected void ddlClients_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ListItem firstItem = new ListItem("-- Select a Project --", string.Empty);
            ddlProjects.Items.Clear();
            ddlProjects.Enabled = false;
            if (ddlClients.SelectedIndex != 0)
            {
                ddlProjects.Enabled = true;

                int? clientId = Convert.ToInt32(ddlClients.SelectedItem.Value);
                //  If current user is administrator, don't apply restrictions
                //bool isUserAdministrator = Roles.IsUserInRole(DataHelper.CurrentPerson.Alias, DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
                ////  adding SeniorLeadership role as per #2930.
                //bool isUserSeniorLeadership = Roles.IsUserInRole(DataHelper.CurrentPerson.Alias, DataTransferObjects.Constants.RoleNames.SeniorLeadershipRoleName);

                //int? personId = (isUserAdministrator || isUserSeniorLeadership) ? null : DataHelper.CurrentPerson.Id;
                var projects = DataHelper.GetTimeEntryProjectsByClientId(clientId, null, false);

                ListItem[] items = projects.Select(
                                                     project => new ListItem(
                                                                             project.Name + " - " + project.ProjectNumber,
                                                                             project.ProjectNumber.ToString()
                                                                            )
                                                   ).ToArray();
                ddlProjects.Items.Add(firstItem);
                ddlProjects.Items.AddRange(items);
            }
            else
            {
                ddlProjects.Items.Add(firstItem);
            }

            mpeProjectSearch.Show();
        }

        protected void ddlProjects_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlProjects.SelectedValue != string.Empty)
            {
                var projectNumber = ddlProjects.SelectedItem.Value;

                txtProjectNumber.Text = projectNumber;
                LoadActiveView();
                ddlProjects.SelectedIndex = ddlClients.SelectedIndex = 0;
            }
            else
            {
                mpeProjectSearch.Show();
            }
        }
    }
}
