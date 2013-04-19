using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.ProjectGroupService;

namespace PraticeManagement.Controls.Clients
{
    public partial class ClientGroups : UserControl
    {
        private const string CLIENT_GROUPS_KEY = "ClientGroupsList";

        #region ProjectGroupsProperties

        private PraticeManagement.ClientDetails HostingPage
        {
            get { return ((PraticeManagement.ClientDetails)Page); }
        }

        public int BusinessGroupId { get; set; }

        public List<ProjectGroup> ClientGroupsList
        {
            get
            {
                if (ViewState[CLIENT_GROUPS_KEY] == null)
                {
                    List<ProjectGroup> groups;
                    if (ClientId == 0)
                    {
                        groups = new List<ProjectGroup>() { new ProjectGroup() { Id = -1, Name = ProjectGroup.DefaultGroupName, Code = ProjectGroup.DefaultGroupCode, IsActive = true, BusinessGroupId = -1 } };
                    }
                    else
                    {
                        groups = ServiceCallers.Custom.Group(g => g.GroupListAll(ClientId, null).OrderBy(p => p.Name).ToList());
                    }
                    ViewState[CLIENT_GROUPS_KEY] = groups.ToList();
                }

                return ((IEnumerable<ProjectGroup>)ViewState[CLIENT_GROUPS_KEY]).ToList();
            }
            set
            {
                if (value != null)
                {
                    value = value.OrderBy(g => g.Name).ToList();
                }
                ViewState[CLIENT_GROUPS_KEY] = value;
            }
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
                GetBusinessGroupList();
            }
        }

        #region ProjectGroupsMethods

        public void GetBusinessGroupList()
        {
            using (var serviceClient = new ProjectGroupServiceClient())
            {
                try
                {
                    List<BusinessGroup> businessGroupList1 = new List<BusinessGroup>();
                    if (ClientId != 0)
                    {
                        ddlAddBusinessGroup.Items.Clear();
                        BusinessGroup[] businessGroupList = serviceClient.GetBusinessGroupList(ClientId, null);
                        DataHelper.FillListDefault(ddlAddBusinessGroup, null, businessGroupList.OrderBy(g => g.Name).ToArray(), true);
                    }
                    else
                    {
                        ddlAddBusinessGroup.Items.Clear();
                        ListItem item = new ListItem();
                        item.Text = "Default";
                        item.Value = "-1";
                        ddlAddBusinessGroup.Items.Add(item);
                    }
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        private bool ValidateProjectGroupName(string projectGroupName, int groupId)
        {
            return ClientGroupsList.Where(p => p.Name.Replace(" ", "").ToLowerInvariant() == projectGroupName.Replace(" ", "").ToLowerInvariant() && (groupId != p.Id || groupId == -1)).Count() == 0;
        }

        protected void custNewGroupName_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = ValidateProjectGroupName(e.Value, -1);
        }

        protected void imgEdit_OnClick(object sender, EventArgs e)
        {
            var imgEdit = sender as ImageButton;
            var gvGroupsItem = imgEdit.NamingContainer as GridViewRow;
            gvGroups.EditIndex = gvGroupsItem.DataItemIndex;
            gvGroups.DataSource = ClientGroupsList;
            gvGroups.DataBind();
            plusMakeVisible(true);
        }

        protected void imgUpdate_OnClick(object sender, EventArgs e)
        {
            var imgEdit = sender as ImageButton;
            var row = imgEdit.NamingContainer as GridViewRow;

            CustomValidator custGroupName = (CustomValidator)row.FindControl("custNewGroupName");
            List<ProjectGroup> tmp = ClientGroupsList;
            int groupId = int.Parse(((HiddenField)row.FindControl("hidKey")).Value);
            string oldGroupName = tmp.Any(g => g.Id == groupId) ? tmp.First(g => g.Id == groupId).Name : string.Empty;
            int oldBusinessGroupId = tmp.Any(g => g.Id == groupId) ? tmp.First(g => g.Id == groupId).BusinessGroupId : 0;
            string groupName = ((TextBox)row.FindControl("txtGroupName")).Text;
            Page.Validate("UpdateGroup");

            if (oldGroupName.ToLowerInvariant() != groupName.ToLowerInvariant())
            {
                custGroupName.IsValid = ValidateProjectGroupName(groupName, groupId);
            }
            DropDownList businessGroupName = (DropDownList)row.FindControl("ddlBusinessGroup");
            BusinessGroupId = Convert.ToInt32(businessGroupName.SelectedValue);

            if (Page.IsValid)
            {
                bool isActive = ((CheckBox)row.FindControl("chbIsActiveEd")).Checked;
                ProjectGroupUpdate(groupId, groupName, isActive, BusinessGroupId);
                tmp.First(g => g.Id == groupId).BusinessGroupId = BusinessGroupId;
                tmp.First(g => g.Id == groupId).Name = groupName;
                tmp.First(g => g.Id == groupId).IsActive = isActive;
                ClientGroupsList = tmp;
                HostingPage.ProjectsControl.DataBind();
                gvGroups.EditIndex = -1;
                gvGroups.DataSource = ClientGroupsList;
                gvGroups.DataBind();
            }
        }

        protected void imgDelete_OnClick(object sender, EventArgs e)
        {
            var imgDelete = sender as ImageButton;
            var row = imgDelete.NamingContainer as GridViewRow;

            List<ProjectGroup> tmp = ClientGroupsList;
            int key = int.Parse(((HiddenField)row.FindControl("hidKey")).Value);

            if (tmp.Any(g => g.Id == key))
                if (!tmp.First(g => g.Id == key).InUse)
                {
                    using (var serviceClient = new ProjectGroupServiceClient())
                    {
                        try
                        {
                            serviceClient.ProjectGroupDelete(key, HostingPage.User.Identity.Name);
                        }
                        catch (FaultException<ExceptionDetail>)
                        {
                            serviceClient.Abort();
                            throw;
                        }
                    }
                    tmp.Remove(tmp.First(g => g.Id == key));
                }

            ClientGroupsList = tmp;
            gvGroups.DataSource = ClientGroupsList;
            gvGroups.DataBind();
        }

        protected void imgCancel_OnClick(object sender, EventArgs e)
        {
            gvGroups.EditIndex = -1;
            gvGroups.DataSource = ClientGroupsList;
            gvGroups.DataBind();
        }

        protected void gvGroups_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var imgDelete = e.Row.FindControl("imgDelete") as ImageButton;
                if (imgDelete != null)
                {
                    var projectgroup = ((ProjectGroup)e.Row.DataItem);
                    imgDelete.Visible = projectgroup.Code != ProjectGroup.DefaultGroupCode && !projectgroup.InUse;
                }
                if (e.Row.RowIndex == gvGroups.EditIndex)
                {
                    DropDownList ddlBusinessGroup = (DropDownList)e.Row.FindControl("ddlBusinessGroup");

                    using (var serviceClient = new ProjectGroupServiceClient())
                    {
                        try
                        {
                            HiddenField hdnBusinessUnitId = (HiddenField)e.Row.FindControl("hdnBusinessUnitId2");
                            List<BusinessGroup> businessGroupList1 = new List<BusinessGroup>();
                            if (ClientId != 0)
                            {
                                businessGroupList1 = serviceClient.GetBusinessGroupList(ClientId, null).ToList();
                                DataHelper.FillListDefault(ddlBusinessGroup, null, businessGroupList1.OrderBy(g => g.Name).ToArray(), true);
                            }
                            if (businessGroupList1.Count == 0)
                            {
                                ListItem item = new ListItem();
                                item.Text = "Default";
                                item.Value = "-1";
                                ddlBusinessGroup.Items.Add(item);
                            }
                            else
                            {
                                BusinessGroup[] businessGroupList2 = serviceClient.GetBusinessGroupList(null, Convert.ToInt32(hdnBusinessUnitId.Value));
                                ddlBusinessGroup.SelectedValue = businessGroupList2.First().Id.Value.ToString();
                            }
                        }
                        catch (CommunicationException)
                        {
                            serviceClient.Abort();
                            throw;
                        }
                    }
                }
                else
                {
                    using (var serviceClient = new ProjectGroupServiceClient())
                    {
                        try
                        {
                            HiddenField hdnBusinessUnitId = (HiddenField)e.Row.FindControl("hdnBusinessUnitId1");
                            Label lblBusinessGroup = (Label)e.Row.FindControl("lblBusinessGroup");
                            if (ClientId != 0)
                            {

                                BusinessGroup[] businessGroupList = serviceClient.GetBusinessGroupList(null, Convert.ToInt32(hdnBusinessUnitId.Value));
                                lblBusinessGroup.Text = businessGroupList.Length > 0 ? businessGroupList.First().Name.Length > 36 ? businessGroupList.First().Name.Substring(0, 34) + "...." : businessGroupList.First().Name : "Default";
                                lblBusinessGroup.ToolTip = businessGroupList.First().Name;
                            }
                            else
                            {
                                lblBusinessGroup.ToolTip = lblBusinessGroup.Text = "Default";
                            }

                        }
                        catch (CommunicationException)
                        {
                            serviceClient.Abort();
                            throw;
                        }
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
                    List<ProjectGroup> tmp = ClientGroupsList;
                    group = new ProjectGroup { Id = (ClientGroupsList.Count() > 0 ? ClientGroupsList.Min(g => g.Id) - 1 : 0), Name = groupName, IsActive = chbGroupActive.Checked, InUse = false };
                    plusMakeVisible(true);
                    tmp.Add(group);
                    ClientGroupsList = tmp;
                }
                else
                {
                    group = new ProjectGroup { Id = AddProjectGroup(groupName, chbGroupActive.Checked), Name = groupName, IsActive = chbGroupActive.Checked, InUse = false };
                    ClientGroupsList = null;
                }
                DisplayGroups(ClientGroupsList);
            }
        }

        public void DisplayGroups(List<ProjectGroup> groups, bool fromMainPage = false)
        {
            if (fromMainPage)
            {
                gvGroups.EditIndex = -1;
                plusMakeVisible(true);
            }
            if (!(ClientId == 0 && groups == null))
            ClientGroupsList = groups;
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

                        ProjectGroup projectGroup = new ProjectGroup();
                        projectGroup.Name = groupName;
                        projectGroup.IsActive = isActive;
                        projectGroup.ClientId = ClientId.Value;
                        projectGroup.BusinessGroupId = Convert.ToInt32(ddlAddBusinessGroup.SelectedValue);
                        int result = serviceGroups.ProjectGroupInsert(projectGroup, HostingPage.User.Identity.Name);
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

        private void ProjectGroupUpdate(int groupId, string groupName, bool isActive, int businessGroupId)
        {
            if (ClientId.HasValue)
                using (var serviceClient = new ProjectGroupServiceClient())
                {
                    try
                    {
                        ProjectGroup projectGroup = new ProjectGroup();
                        projectGroup.Name = groupName;
                        projectGroup.Id = groupId;
                        projectGroup.IsActive = isActive;
                        projectGroup.ClientId = ClientId.Value;
                        projectGroup.BusinessGroupId = businessGroupId;
                        serviceClient.ProjectGroupUpdate(projectGroup, HostingPage.User.Identity.Name);
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
            GetBusinessGroupList();
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
                ddlAddBusinessGroup.Visible = false;
            }
            else
            {
                btnPlus.Visible = false;
                btnAddGroup.Visible = true;
                btnCancel.Visible = true;
                txtNewGroupName.Text = string.Empty;
                txtNewGroupName.Visible = true;
                chbGroupActive.Visible = true;
                ddlAddBusinessGroup.Visible = true;
                if (gvGroups.EditIndex > -1)
                {
                    gvGroups.EditIndex = -1;
                    DisplayGroups(ClientGroupsList);
                }
            }

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            plusMakeVisible(true);
        }

        #endregion
    }
}

