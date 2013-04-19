using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.ProjectGroupService;
using System.ServiceModel;

namespace PraticeManagement.Controls.Clients
{
    public partial class ClientBusinessGroups : System.Web.UI.UserControl
    {
        private const string CLIENT_GROUPS_KEY = "ClientBusinessGroupsList";

        #region ProjectGroupsProperties

        private PraticeManagement.ClientDetails HostingPage
        {
            get { return ((PraticeManagement.ClientDetails)Page); }
        }

        public List<BusinessGroup> ClientGroupsList
        {
            get
            {
                if (ViewState[CLIENT_GROUPS_KEY] == null)
                {
                    List<BusinessGroup> groups;
                    if (ClientId == 0)
                    {
                        groups = new List<BusinessGroup>() { new BusinessGroup() { Id = 0, Name = BusinessGroup.DefaultBusinessGroupName, Code = BusinessGroup.DefaultBusinessGroupCode, IsActive = true } };
                    }
                    else
                    {
                        groups = ServiceCallers.Custom.Group(g => g.GetBusinessGroupList(ClientId, null).OrderBy(p => p.Name).ToList());
                    }
                    ViewState[CLIENT_GROUPS_KEY] = groups.ToList();
                }

                return ((IEnumerable<BusinessGroup>)ViewState[CLIENT_GROUPS_KEY]).ToList();
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
        }

        #region ProjectGroupsMethods

        private bool ValidateBusinessGroupName(string businessGroupName, int groupId)
        {
            return ClientGroupsList.Where(p => p.Name.Replace(" ", "").ToLowerInvariant() == businessGroupName.Replace(" ", "").ToLowerInvariant() && (groupId != p.Id || groupId == -1)).Count() == 0;
        }

        protected void custNewGroupName_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = ValidateBusinessGroupName(e.Value, -1);
        }

        protected void imgEdit_OnClick(object sender, EventArgs e)
        {
            var imgEdit = sender as ImageButton;
            var gvGroupsItem = imgEdit.NamingContainer as GridViewRow;
            gvBusinessGroups.EditIndex = gvGroupsItem.DataItemIndex;
            gvBusinessGroups.DataSource = ClientGroupsList;
            gvBusinessGroups.DataBind();
            plusMakeVisible(true);
        }

        protected void imgUpdate_OnClick(object sender, EventArgs e)
        {
            var imgEdit = sender as ImageButton;
            var row = imgEdit.NamingContainer as GridViewRow;

            CustomValidator custGroupName = (CustomValidator)row.FindControl("custUpdateGroupName");
            List<BusinessGroup> tmp = ClientGroupsList;
            int groupId = int.Parse(((HiddenField)row.FindControl("hidKey")).Value);
            BusinessGroup oldBusinessGroup = tmp.Any(g => g.Id == groupId) ? tmp.First(g => g.Id == groupId) : null;
            string oldGroupName = oldBusinessGroup != null ? oldBusinessGroup.Name : string.Empty;
            string groupName = ((TextBox)row.FindControl("txtGroupName")).Text;
            Page.Validate("UpdateBusinessGroup");

            if (oldGroupName.ToLowerInvariant() != groupName.ToLowerInvariant())
            {
                custGroupName.IsValid = ValidateBusinessGroupName(groupName, groupId);
            }

            if (Page.IsValid)
            {
                bool isActive = ((CheckBox)row.FindControl("chbIsActiveEd")).Checked;
                UpdateBusinessGroup(groupId, groupName, isActive);
                
                if (oldBusinessGroup != null)
                {
                    oldBusinessGroup.Name = groupName;
                    oldBusinessGroup.IsActive = isActive;
                }
                HostingPage.ProjectsControl.DataBind();
                gvBusinessGroups.EditIndex = -1;
                DisplayGroups(tmp);
            }
        }

        protected void imgDelete_OnClick(object sender, EventArgs e)
        {
            var imgDelete = sender as ImageButton;
            var row = imgDelete.NamingContainer as GridViewRow;

            List<BusinessGroup> tmp = ClientGroupsList;
            int key = int.Parse(((HiddenField)row.FindControl("hidKey")).Value);

            if (tmp.Any(g => g.Id == key) && !tmp.First(g => g.Id == key).InUse)
            {
                using (var serviceClient = new ProjectGroupServiceClient())
                {
                    try
                    {
                        serviceClient.BusinessGroupDelete(key, HostingPage.User.Identity.Name);
                    }
                    catch (FaultException<ExceptionDetail>)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }
                tmp.Remove(tmp.First(g => g.Id == key));
            }
            DisplayGroups(tmp);
        }

        protected void imgCancel_OnClick(object sender, EventArgs e)
        {
            gvBusinessGroups.EditIndex = -1;
            gvBusinessGroups.DataSource = ClientGroupsList;
            gvBusinessGroups.DataBind();
        }

        protected void gvBusinessGroups_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var imgDelete = e.Row.FindControl("imgDelete") as ImageButton;
                if (imgDelete != null)
                {
                    var projectgroup = ((BusinessGroup)(e.Row.DataItem));
                    imgDelete.Visible = projectgroup.Code != BusinessGroup.DefaultBusinessGroupCode && !projectgroup.InUse;
                }

            }
        }

        protected void btnAddGroup_Click(object sender, EventArgs e)
        {
            Page.Validate("NewBusinessGroup");
            if (Page.IsValid)
            {
                string groupName = txtNewGroupName.Text;
                BusinessGroup group;
                if (ClientId == 0)
                {
                    List<BusinessGroup> tmp = ClientGroupsList;
                    group = new BusinessGroup { Id = (ClientGroupsList.Count() > 0 ? ClientGroupsList.Min(g => g.Id) - 1 : 0), Name = groupName, IsActive = chbGroupActive.Checked, InUse = false };
                    plusMakeVisible(true);
                    tmp.Add(group);
                    ClientGroupsList = tmp;
                }
                else
                {
                    group = new BusinessGroup { Id = AddBusinessGroup(groupName, chbGroupActive.Checked), Name = groupName, IsActive = chbGroupActive.Checked, InUse = false };
                    ClientGroupsList = null;
                }
                
                DisplayGroups(ClientGroupsList);
            }
        }

        public void DisplayGroups(List<BusinessGroup> groups, bool fromMainPage = false)
        {
            if (fromMainPage)
            {
                gvBusinessGroups.EditIndex = -1;
                plusMakeVisible(true);
            }
            if(!(ClientId == 0 && groups == null))
                ClientGroupsList = groups;
            gvBusinessGroups.DataSource = ClientGroupsList;
            gvBusinessGroups.DataBind();
        }

        private int AddBusinessGroup(string groupName, bool isActive)
        {
            if (ClientId.HasValue)
                using (var serviceGroups = new ProjectGroupServiceClient())
                {
                    try
                    {
                        BusinessGroup businessGroup = new BusinessGroup();
                        businessGroup.ClientId = ClientId.Value;
                        businessGroup.Name = groupName;
                        businessGroup.IsActive = isActive;
                        int result = serviceGroups.BusinessGroupInsert(businessGroup, HostingPage.User.Identity.Name);
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

        private void UpdateBusinessGroup(int groupId, string groupName, bool isActive)
        {
            if (ClientId.HasValue)
                using (var serviceClient = new ProjectGroupServiceClient())
                {
                    try
                    {
                        BusinessGroup businessGroup = new BusinessGroup();
                        businessGroup.ClientId = ClientId.Value;
                        businessGroup.Name = groupName;
                        businessGroup.IsActive = isActive;
                        businessGroup.Id = groupId;
                        serviceClient.BusinessGroupUpdate(businessGroup, HostingPage.User.Identity.Name);
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
                if (gvBusinessGroups.EditIndex > -1)
                {
                    gvBusinessGroups.EditIndex = -1;
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

