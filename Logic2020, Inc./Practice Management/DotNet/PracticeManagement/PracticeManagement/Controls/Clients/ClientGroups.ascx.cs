using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.ProjectGroupService;
using System.Text;

namespace PraticeManagement.Controls.Clients
{
    public partial class ClientGroups : UserControl
    {
        private const string CLIENT_GROUPS_KEY = "ClientGroupsList";
        private const string DefaultGroup = "Default Group";


        #region ProjectGroupsProperties

        private PraticeManagement.ClientDetails HostingPage
        {
            get { return ((PraticeManagement.ClientDetails)Page); }
        }

        public Dictionary<int, ProjectGroup> ClientGroupsList
        {
            get
            {
                if (ViewState[CLIENT_GROUPS_KEY] == null)
                {
                    ViewState[CLIENT_GROUPS_KEY] = new Dictionary<int, ProjectGroup>();
                }

                return (Dictionary<int, ProjectGroup>)ViewState[CLIENT_GROUPS_KEY];
            }
            set { ViewState[CLIENT_GROUPS_KEY] = value; }
        }

        protected int? ClientId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(Request.QueryString[Constants.QueryStringParameterNames.Id]);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                List<ProjectGroup> groups;
                if (ClientId == 0)
                {
                    groups = new List<ProjectGroup>() { new ProjectGroup() { Id = 0, Name = DefaultGroup, Code = "B0001", IsActive = true } };
                }
                else
                {
                    groups = ServiceCallers.Custom.Group(g => g.GroupListAll(ClientId, null).ToList());

                }
                DisplayGroups((IEnumerable<ProjectGroup>)groups);
            }
        }

        #region ProjectGroupsMethods

        private bool ValidateBusinessGroupName(string businessGroupName)
        {
            return ClientGroupsList.Where(p => p.Value.Name.Replace(" ", "").ToLowerInvariant() == businessGroupName.Replace(" ", "").ToLowerInvariant()).Count() == 0;
        }

        protected void custNewGroupName_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = ValidateBusinessGroupName(e.Value);
        }

        protected void gvGroups_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvGroups.EditIndex = e.NewEditIndex;
            e.Cancel = true;
            gvGroups.DataSource = ClientGroupsList;
            gvGroups.DataBind();
        }

        protected void gvGroups_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            CustomValidator custGroupName = (CustomValidator)gvGroups.Rows[e.RowIndex].FindControl("custNewGroupName");
            Dictionary<int, ProjectGroup> tmp = ClientGroupsList;
            int groupId = int.Parse(((HiddenField)gvGroups.Rows[e.RowIndex].FindControl("hidKey")).Value);
            string oldGroupName = tmp[groupId].Name;
            string groupName = ((TextBox)gvGroups.Rows[e.RowIndex].FindControl("txtGroupName")).Text;
            Page.Validate("UpdateGroup");

            if (oldGroupName.ToLowerInvariant() != groupName.ToLowerInvariant())
            {
                custGroupName.IsValid = ValidateBusinessGroupName(groupName);
            }


            if (Page.IsValid)
            {
                bool isActive = ((CheckBox)gvGroups.Rows[e.RowIndex].FindControl("chbIsActiveEd")).Checked;
                UpDateProductGroup(groupId, groupName, isActive);
                tmp[groupId].Name = groupName;
                tmp[groupId].IsActive = isActive;
                ClientGroupsList = tmp;
                HostingPage.ProjectsControl.DataBind();
                gvGroups.EditIndex = -1;
                gvGroups.DataSource = ClientGroupsList;
                gvGroups.DataBind();
                e.Cancel = true;
            }
        }

        protected void gvGroups_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Dictionary<int, ProjectGroup> tmp = ClientGroupsList;
            int key = int.Parse(((HiddenField)gvGroups.Rows[e.RowIndex].FindControl("hidKey")).Value);

            if (tmp.ContainsKey(key))
                if (!tmp[key].InUse)
                {
                    using (var serviceClient = new ProjectGroupServiceClient())
                    {
                        try
                        {
                            serviceClient.ProjectGroupDelete(key);
                        }
                        catch (FaultException<ExceptionDetail>)
                        {
                            serviceClient.Abort();
                            throw;
                        }
                    }
                    tmp.Remove(key);
                }

            ClientGroupsList = tmp;
            gvGroups.DataSource = ClientGroupsList;
            gvGroups.DataBind();
            e.Cancel = true;
        }

        protected void gvGroups_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvGroups.EditIndex = -1;
            gvGroups.DataSource = ClientGroupsList;
            gvGroups.DataBind();
            e.Cancel = true;
        }

        protected void gvGroups_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (((ProjectGroup)DataBinder.Eval(e.Row.DataItem, "Value")).Name == DefaultGroup)
                {
                    e.Row.Cells[0].Controls[0].Visible = false;
                    e.Row.Cells[gvGroups.Columns.Count - 1].Visible = false;
                }
                else
                {
                    bool inUse = ((ProjectGroup)DataBinder.Eval(e.Row.DataItem, "Value")).InUse;
                    e.Row.Cells[gvGroups.Columns.Count - 1].Visible = !inUse;
                    if (!inUse)
                    {
                        try
                        {
                            ((ImageButton)e.Row.Cells[gvGroups.Columns.Count - 1].Controls[0]).ToolTip = "Delete Business Unit";

                        }
                        catch
                        {
                            e.Row.Cells[gvGroups.Columns.Count - 1].ToolTip = "Delete Business Unit";
                        }

                    }

                    try
                    {
                        ((ImageButton)e.Row.Cells[0].Controls[0]).ToolTip = "Edit Business Unit";

                    }
                    catch
                    {
                        e.Row.Cells[0].ToolTip = "Edit Business Unit";
                    }
                }

                if ((e.Row.RowState & DataControlRowState.Edit) != 0)
                {
                    try
                    {
                        ((ImageButton)e.Row.Cells[0].Controls[0]).ToolTip = "Confirm";
                        ((ImageButton)e.Row.Cells[0].Controls[2]).ToolTip = "Cancel";
                        e.Row.Cells[gvGroups.Columns.Count - 1].ToolTip = "";
                    }
                    catch
                    {
                        e.Row.Cells[0].ToolTip = "";
                        e.Row.Cells[gvGroups.Columns.Count - 1].ToolTip = "";
                    }
                }
            }
        }


        protected void btnAddGroup_Click(object sender, EventArgs e)
        {
            Page.Validate("NewGroup");
            if (Page.IsValid)
            {
                string groupName = txtNewGroupName.Text;
                ProjectGroup group;
                if (ClientId == 0)
                {
                    group = new ProjectGroup { Id = (ClientGroupsList.Keys.Count() > 0 ? ClientGroupsList.Keys.Min() - 1 : 0), Name = groupName, IsActive = chbGroupActive.Checked, InUse = false };
                    plusMakeVisible(true);
                }
                else
                {
                    group = new ProjectGroup { Id = AddProjectGroup(groupName, chbGroupActive.Checked), Name = groupName, IsActive = chbGroupActive.Checked, InUse = false };
                }                
                ClientGroupsList.Add(group.Id.Value, group);               
                gvGroups.DataSource = ClientGroupsList;
                gvGroups.DataBind();
            }
        }

        private void DisplayGroups(IEnumerable<ProjectGroup> groups)
        {
            ClientGroupsList = new Dictionary<int, ProjectGroup>();
            foreach (var projectGroup in groups)
                ClientGroupsList.Add(projectGroup.Id.Value, projectGroup);

            gvGroups.DataSource = ClientGroupsList;
            gvGroups.DataBind();
        }

        private int AddProjectGroup(string groupName, bool isActive)
        {
            if (ClientId.HasValue)
                using (var serviceGroups = new ProjectGroupServiceClient())
                {
                    try
                    {
                        int result = serviceGroups.ProjectGroupInsert(ClientId.Value, groupName, isActive);
                        plusMakeVisible(true);
                        return result;
                    }
                    catch (FaultException<ExceptionDetail>)
                    {
                        serviceGroups.Abort();
                        throw;
                    }
                }

            return -1;
        }

        private void UpDateProductGroup(int groupId, string groupName, bool isActive)
        {
            if (ClientId.HasValue)
                using (var serviceClient = new ProjectGroupServiceClient())
                {
                    try
                    {
                        serviceClient.UpDateProductGroup(ClientId.Value, groupId, groupName, isActive);
                    }
                    catch (FaultException<ExceptionDetail>)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }
        }

        protected void btnPlus_Click(object sender, EventArgs e)
        {
            plusMakeVisible(false);
        }

        private void plusMakeVisible(bool isplusVisible)
        {
            if (isplusVisible)
            {
                btnPlus.Visible = true;
                btnAddGroup.Visible = false;
                btnCancel.Visible = false;
                txtNewGroupName.Visible = false;
                chbGroupActive.Visible = false;
            }
            else
            {
                btnPlus.Visible = false;
                btnAddGroup.Visible = true;
                btnCancel.Visible = true;
                txtNewGroupName.Text = string.Empty;
                txtNewGroupName.Visible = true;
                chbGroupActive.Visible = true;
            }

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            plusMakeVisible(true);
        }

        #endregion
    }
}

